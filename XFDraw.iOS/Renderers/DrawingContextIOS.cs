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

namespace XFDraw.iOS.Renderers
{
    class DrawingContextIOS : DrawingContext
    {
        public nfloat maxY;
        public nfloat minY;
        public nfloat chartWidth;
        public nfloat chartHeight;
        public nfloat itemWidth;
        public nfloat xOffset;
        public nfloat yOffset;
        public CGRect viewport;
        public CoreGraphics.CGContext cgContext;
        public float fontSize = 25;
        float scale = 1;
        public nfloat textHeight;
        public nfloat axisPadding = 5;
        CTFont font;
        CTStringAttributes attrs;

        Color strokeColor;

        internal void BeginDraw(CGContext g)
        {
            cgContext = g;
        }

        Color fillColor;
        Color? fillColor2;
        Color textForegroundColor;

        CoreGraphics.CGColor aFill;
        CoreGraphics.CGColor aFill2;
        CoreGraphics.CGColor aStroke;
        CoreGraphics.CGColor aForeground;


        internal void DrawRect(nfloat x, nfloat y, nfloat width, nfloat height, nfloat borderThickness)
        {
            cgContext.SetLineWidth((nfloat)borderThickness / scale);
            cgContext.SetFillColor(aFill);
            cgContext.FillRect((CGRect)new CGRect(x, y, width, height));
            if (borderThickness > 0)
            {
                cgContext.SetStrokeColor(aStroke);
                cgContext.StrokeRect((CGRect)new CGRect(x, y, width, height));
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
                cgContext.SetLineWidth((nfloat)strokeThickness / scale);
                cgContext.SetFillColor(aFill);
                var path = GetRoundedRect(new CGRect(x, y, width, height), radiusX, radiusY);
                cgContext.AddPath(path);
                cgContext.FillPath();
                if (strokeThickness > 0)
                {
                    cgContext.AddPath(path);
                    cgContext.SetStrokeColor(aStroke);
                    cgContext.StrokePath();
                }
                cgContext.ClosePath();
            }
        }

        float ToRadians(float degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }
        private CGPath GetRoundedRect(CGRect rect, float radiusX, float radiusY)
        {
            CGPath path = new CGPath();
            path.AddRoundedRect(rect, radiusX, radiusY);
            return path;
        }

        public override void SetStrokeColor(Color color)
        {
            if (strokeColor != color)
            {
                strokeColor = color;
                aStroke = strokeColor.ToCGColor();
            }
        }

        public override void SetFillColor(Color color)
        {
            if (fillColor != color)
            {
                fillColor = color;
                aFill = fillColor.ToCGColor();
            }
        }

        public override void DrawLine(float x1, float y1, float x2, float y2, float strokeThickness)
        {
            cgContext.SetLineWidth((nfloat)strokeThickness / scale);
            var stroke = aStroke;
            cgContext.SetStrokeColor(stroke);
            CoreGraphics.CGPath path = new CoreGraphics.CGPath();
            path.AddLines(new CGPoint[] { new CGPoint(x1, y1), new CGPoint(x2, y2) });
            cgContext.AddPath(path);
            cgContext.DrawPath(CoreGraphics.CGPathDrawingMode.Stroke);
        }

        public override void DrawPolygon(List<PointF> vertices, float strokeThickness)
        {
            cgContext.SetLineWidth((nfloat)strokeThickness / scale);
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

        public override void DrawPolyline(List<PointF> vertices, float strokeThickness, bool isClosedPath = false)
        {
            if (strokeThickness == 0 || (float)aStroke.Alpha == 0 || !vertices.Any()) return;
            cgContext.SetLineWidth((nfloat)strokeThickness / scale);
            cgContext.SetStrokeColor(aStroke);
            CoreGraphics.CGPath path = new CoreGraphics.CGPath();
            path.AddLines(vertices.Select(p => new CGPoint(p.X, p.Y)).ToArray());
            if (isClosedPath) path.AddLineToPoint(new CGPoint(vertices[0].X, vertices[0].Y));
            cgContext.AddPath(path);
            cgContext.DrawPath(CoreGraphics.CGPathDrawingMode.Stroke);
        }
    }
}
