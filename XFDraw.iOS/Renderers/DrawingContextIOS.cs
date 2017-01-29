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
        public CoreGraphics.CGContext cgContext;

        internal void BeginDraw(CGContext g)
        {
            cgContext = g;
        }

        bool doStroke;
        bool doFill;

        CoreGraphics.CGColor aFill;
        CoreGraphics.CGColor aStroke;


        internal void DrawRect(nfloat x, nfloat y, nfloat width, nfloat height, nfloat borderThickness)
        {
            cgContext.SetLineWidth((nfloat)borderThickness);
            cgContext.SetFillColor(aFill);
            cgContext.FillRect((CGRect)new CGRect(x, y, width, height));
            if (borderThickness > 0)
            {
                cgContext.SetStrokeColor(aStroke);
                cgContext.StrokeRect((CGRect)new CGRect(x, y, width, height));
            }
        }

        public override void DrawEllipse(float cx, float cy, float radiusx, float radiusy, float strokeThickness)
        {
            cgContext.SetLineWidth((nfloat)strokeThickness);
            cgContext.SetFillColor(aFill);
            var rect = new CGRect(cx - radiusx, cy - radiusy, radiusx * 2, radiusy * 2);
            if (doFill)
            {
                cgContext.FillEllipseInRect(rect);
            }
            if (strokeThickness > 0 && doStroke)
            {
                cgContext.SetStrokeColor(aStroke);
                cgContext.StrokeEllipseInRect(rect);
            }
        }

        public override void DrawRoundedRect(float x, float y, float width, float height, float radiusX, float radiusY, float strokeThickness)
        {
            if (radiusX == 0 && radiusY == 0)
            {
                DrawRect(x, y, width, height, strokeThickness);
            }
            else
            {
                var path = GetRoundedRect(new CGRect(x, y, width, height), radiusX, radiusY);
                if (doFill)
                {
                    cgContext.SetFillColor(aFill);
                    cgContext.AddPath(path);
                    cgContext.FillPath();
                }
                if (strokeThickness > 0 && doStroke)
                {
                    cgContext.SetLineWidth((nfloat)strokeThickness);
                    cgContext.AddPath(path);
                    cgContext.SetStrokeColor(aStroke);
                    cgContext.StrokePath();
                }
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
            aStroke = SetPaint(brush, ref doStroke);
        }

        public override void SetFill(Brush brush)
        {
            aFill = SetPaint(brush, ref doFill);
        }

        private CGColor SetPaint(Brush brush, ref bool doPaint)
        {
            var sb = brush as SolidColorBrush;
            if (sb == null)
            {
                doPaint = false;
            }
            else if (sb.Opacity >= 0 && sb.Color.A > 0)
            {
                var xcolor = sb.Color;
                if (sb.Opacity != 1)
                {
                    xcolor = xcolor.MultiplyAlpha(sb.Opacity);
                }
                var color = xcolor.ToCGColor();
                doPaint = true;
                return color;
            }
            doPaint = false;
            return null;
        }

        public override void DrawLine(float x1, float y1, float x2, float y2, float strokeThickness)
        {
            cgContext.SetLineWidth((nfloat)strokeThickness);
            if (doStroke && strokeThickness > 0)
            {
                cgContext.SetStrokeColor(aStroke);
                CoreGraphics.CGPath path = new CoreGraphics.CGPath();
                path.AddLines(new CGPoint[] { new CGPoint(x1, y1), new CGPoint(x2, y2) });
                cgContext.AddPath(path);
                cgContext.DrawPath(CoreGraphics.CGPathDrawingMode.Stroke);
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
            cgContext.BeginPath();
            cgContext.AddPath(path);
            cgContext.SetFillColor(fill);
            cgContext.SetStrokeColor(stroke);
            cgContext.DrawPath(CoreGraphics.CGPathDrawingMode.Fill);
            DrawPolyline(vertices, strokeThickness, true);
        }

        public override void DrawPolyline(List<Vector2> vertices, float strokeThickness, bool isClosedPath = false)
        {
            if (strokeThickness == 0 || (float)aStroke.Alpha == 0 || !vertices.Any()) return;
            cgContext.SetLineWidth((nfloat)strokeThickness);
            cgContext.SetStrokeColor(aStroke);
            CoreGraphics.CGPath path = new CoreGraphics.CGPath();
            path.AddLines(vertices.Select(p => new CGPoint(p.X, p.Y)).ToArray());
            if (isClosedPath) path.AddLineToPoint(new CGPoint(vertices[0].X, vertices[0].Y));
            cgContext.AddPath(path);
            cgContext.DrawPath(CoreGraphics.CGPathDrawingMode.Stroke);
        }

        public override void DrawArc(float cx, float cy, float radius, float startAngle, float endAngle, bool includeCenterInStroke, float strokeThickness)
        {
            if (doFill)
            {
                cgContext.SetFillColor(aFill);
                cgContext.AddArc(cx, cy, radius, -ToRadians(startAngle), -ToRadians(endAngle), true);
                cgContext.AddLineToPoint(cx, cy);
                cgContext.ClosePath();
                cgContext.FillPath();
            }
            if (strokeThickness > 0 && doStroke)
            {
                cgContext.SetLineWidth((nfloat)strokeThickness);
                cgContext.AddArc(cx, cy, radius, -ToRadians(startAngle), -ToRadians(endAngle), true);
                if (includeCenterInStroke)
                {
                    cgContext.AddLineToPoint(cx, cy);
                    cgContext.ClosePath();
                }
                cgContext.SetStrokeColor(aStroke);
                cgContext.StrokePath();
            }
        }
    }
}
