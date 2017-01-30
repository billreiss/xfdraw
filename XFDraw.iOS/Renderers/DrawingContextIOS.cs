using CoreGraphics;
using CoreText;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Linq;
using UIKit;
using XFDraw.Numerics;

namespace XFDraw.iOS.Renderers
{
    class DrawingContextIOS : DrawingContext
    {
        private CoreGraphics.CGContext cgContext;
        private UIView view;

        internal void BeginDraw(CGContext g, UIView view)
        {
            cgContext = g;
            this.view = view;
        }

        bool doStroke;
        bool doFill;

        CoreGraphics.CGColor aFill;
        CoreGraphics.CGColor aStroke;
        CoreGraphics.CGGradient gFill;
        CoreGraphics.CGGradient gStroke;
        LinearGradientBrush lgStroke;
        LinearGradientBrush lgFill;
        RadialGradientBrush rgStroke;
        RadialGradientBrush rgFill;

        public override void DrawEllipse(float cx, float cy, float radiusx, float radiusy, float strokeThickness)
        {
            var rect = new CGRect(cx - radiusx, cy - radiusy, radiusx * 2, radiusy * 2);
            var path = new CGPath();
            path.AddEllipseInRect(rect);
            if (doFill)
            {
                FillPath(path);
            }
            if (strokeThickness > 0 && doStroke)
            {
                StrokePath(strokeThickness, path);
            }
        }

        public override void DrawRoundedRect(float x, float y, float width, float height, float radiusX, float radiusY, float strokeThickness)
        {
            var path = GetRoundedRect(new CGRect(x, y, width, height), radiusX, radiusY);
            if (doFill)
            {
                FillPath(path);
            }
            if (strokeThickness > 0 && doStroke)
            {
                StrokePath(strokeThickness, path);
            }
        }

        private CGPath GetRoundedRect(CGRect rect, float radiusX, float radiusY)
        {
            CGPath path = new CGPath();
            path.AddRoundedRect(rect, radiusX, radiusY);
            return path;
        }

        public override void SetStroke(Brush brush)
        {
            if (gStroke != null) gStroke.Dispose();
            if (brush is SolidColorBrush)
            {
                aStroke = SetPaint(brush);
                gStroke = null;
            }
            else if (brush is GradientBrush)
            {
                gStroke = SetGradient(brush);
                lgStroke = brush as LinearGradientBrush;
                rgStroke = brush as RadialGradientBrush;
                aStroke = null;
            }
            else
            {
                gStroke = null;
                aStroke = null;
            }
            doStroke = (aStroke != null || gStroke != null);
        }

        public override void SetFill(Brush brush)
        {
            if (gFill != null) gFill.Dispose();
            if (brush is SolidColorBrush)
            {
                aFill = SetPaint(brush);
                gFill = null;
            }
            else if (brush is GradientBrush)
            {
                gFill = SetGradient(brush);
                lgFill = brush as LinearGradientBrush;
                rgFill = brush as RadialGradientBrush;
                aFill = null;
            }
            else
            {
                gFill = null;
                aFill = null;
            }
            doFill = (aFill != null || gFill != null);
        }

        private CGGradient SetGradient(Brush brush)
        {
            var g = brush as GradientBrush;
            using (var colorSpace = CGColorSpace.CreateDeviceRGB())
            {
                CGGradient cg = new CGGradient(colorSpace, g.GradientStops.Select(gg => gg.Color.ToCGColor()).ToArray(), g.GradientStops.Select(gg => (nfloat)gg.Offset).ToArray());
                return cg;
            }
        }

        private CGColor SetPaint(Brush brush)
        {
            var sb = brush as SolidColorBrush;
            if (sb == null)
            {
                return null;
            }
            else if (sb.Opacity >= 0 && sb.Color.A > 0)
            {
                var xcolor = sb.Color;
                if (sb.Opacity != 1)
                {
                    xcolor = xcolor.MultiplyAlpha(sb.Opacity);
                }
                var color = xcolor.ToCGColor();
                return color;
            }
            return null;
        }

        public override void DrawLine(float x1, float y1, float x2, float y2, float strokeThickness)
        {
            cgContext.SetLineWidth((nfloat)strokeThickness);
            if (doStroke && strokeThickness > 0)
            {
                CoreGraphics.CGPath path = new CoreGraphics.CGPath();
                path.AddLines(new CGPoint[] { new CGPoint(x1, y1), new CGPoint(x2, y2) });
                StrokePath(strokeThickness, path);
            }
        }

        public override void DrawPolygon(List<Vector2> vertices, float strokeThickness)
        {
            cgContext.SetLineWidth((nfloat)strokeThickness);
            var stroke = aStroke;
            var fill = aFill;
            CoreGraphics.CGPath path = new CoreGraphics.CGPath();
            path.AddLines(vertices.Select(p => new CGPoint(p.X, p.Y)).ToArray());
            path.CloseSubpath();
            FillPath(path);
            DrawPolyline(vertices, strokeThickness, true);
        }

        public override void DrawPolyline(List<Vector2> vertices, float strokeThickness, bool isClosedPath = false)
        {
            if (strokeThickness == 0 || !doStroke || !vertices.Any()) return;
            cgContext.SetStrokeColor(aStroke);
            CoreGraphics.CGPath path = new CoreGraphics.CGPath();
            path.AddLines(vertices.Select(p => new CGPoint(p.X, p.Y)).ToArray());
            if (isClosedPath) path.CloseSubpath();
            StrokePath(strokeThickness, path);
        }

        private void FillPath(CGPath path)
        {
            if (gFill != null)
            {
                cgContext.SaveState();
                cgContext.AddPath(path);
                cgContext.ClosePath();
                cgContext.Clip();
                var bounds = path.BoundingBox;
                if (lgFill != null)
                {
                    Matrix3x2 transform = Matrix3x2.CreateScale((float)bounds.Width, (float)bounds.Height) * Matrix3x2.CreateTranslation((float)bounds.X, (float)bounds.Y);
                    var s = Vector2.Transform(lgFill.StartPoint, transform);
                    var e = Vector2.Transform(lgFill.EndPoint, transform);
                    cgContext.DrawLinearGradient(gFill, new CGPoint(s.X, s.Y), new CGPoint(e.X, e.Y), CGGradientDrawingOptions.DrawsBeforeStartLocation | CGGradientDrawingOptions.DrawsAfterEndLocation);
                }
                else if (rgFill != null)
                {
                    float maxSize = (float)Math.Max(bounds.Width, bounds.Height);
                    var radius = rgFill.Radius * maxSize;
                    var trans = new Vector2((float)bounds.X, (float)bounds.Y) + new Vector2((float)bounds.Width / 2, (float)bounds.Height / 2) - new Vector2(maxSize, maxSize) / 2;
                    Matrix3x2 transform = Matrix3x2.CreateScale(maxSize) * Matrix3x2.CreateTranslation(trans);
                    var c = Vector2.Transform(rgFill.Center, transform);
                    cgContext.DrawRadialGradient(gFill, new CGPoint(c.X, c.Y), 0, new CGPoint(c.X, c.Y), radius, CGGradientDrawingOptions.DrawsAfterEndLocation | CGGradientDrawingOptions.DrawsBeforeStartLocation);
                }
                cgContext.RestoreState();
            }
            else
            {
                cgContext.BeginPath();
                cgContext.SetFillColor(aFill);
                cgContext.AddPath(path);
                cgContext.DrawPath(CoreGraphics.CGPathDrawingMode.Fill);
            }
        }

        private void StrokePath(float strokeThickness, CGPath path)
        {
            if (strokeThickness == 0) return;
            if (gStroke != null)
            {
                var strokedPath = path.CopyByStrokingPath(strokeThickness, CGLineCap.Square, CGLineJoin.Miter, 10);
                cgContext.SaveState();
                cgContext.AddPath(strokedPath);
                cgContext.Clip();
                var bounds = strokedPath.BoundingBox;
                if (lgStroke != null)
                {
                    Matrix3x2 transform = Matrix3x2.CreateScale((float)bounds.Width, (float)bounds.Height) * Matrix3x2.CreateTranslation((float)bounds.X, (float)bounds.Y);
                    var s = Vector2.Transform(lgStroke.StartPoint, transform);
                    var e = Vector2.Transform(lgStroke.EndPoint, transform);
                    cgContext.DrawLinearGradient(gStroke, new CGPoint(s.X, s.Y), new CGPoint(e.X, e.Y), CGGradientDrawingOptions.DrawsBeforeStartLocation | CGGradientDrawingOptions.DrawsAfterEndLocation);
                }
                else if (rgStroke != null)
                {
                    float maxSize = (float)Math.Max(bounds.Width, bounds.Height);
                    var radius = rgStroke.Radius * maxSize;
                    var trans = new Vector2((float)bounds.X, (float)bounds.Y) + new Vector2((float)bounds.Width / 2, (float)bounds.Height / 2) - new Vector2(maxSize, maxSize) / 2;
                    Matrix3x2 transform = Matrix3x2.CreateScale(maxSize) * Matrix3x2.CreateTranslation(trans);
                    var c = Vector2.Transform(rgStroke.Center, transform);
                    cgContext.DrawRadialGradient(gStroke, new CGPoint(c.X, c.Y), 0, new CGPoint(c.X, c.Y), radius, CGGradientDrawingOptions.DrawsAfterEndLocation | CGGradientDrawingOptions.DrawsBeforeStartLocation);
                }
                cgContext.RestoreState();
            }
            else
            {
                cgContext.SetStrokeColor(aStroke);
                cgContext.SetLineWidth((nfloat)strokeThickness);
                cgContext.AddPath(path);
                cgContext.DrawPath(CoreGraphics.CGPathDrawingMode.Stroke);
            }
        }

        public override void DrawArc(float cx, float cy, float radius, float startAngle, float endAngle, bool includeCenterInStroke, float strokeThickness)
        {
            if (doFill)
            {
                cgContext.SetFillColor(aFill);
                var path = new CGPath();
                path.AddArc(cx, cy, radius, -ToRadians(startAngle), -ToRadians(endAngle), true);
                path.AddLineToPoint(cx, cy);
                path.CloseSubpath();
                FillPath(path);
            }
            if (strokeThickness > 0 && doStroke)
            {
                var path = new CGPath();
                path.AddArc(cx, cy, radius, -ToRadians(startAngle), -ToRadians(endAngle), true);
                if (includeCenterInStroke)
                {
                    path.AddLineToPoint(cx, cy);
                    path.CloseSubpath();
                }
                StrokePath(strokeThickness, path);
            }
        }
    }
}
