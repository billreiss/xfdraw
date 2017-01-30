using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XFDraw.Numerics;

namespace XFDraw
{
    public abstract class DrawingContext
    {
        SolidColorBrush colorBrush = new SolidColorBrush();

        public void SetStrokeColor(Color color)
        {
            if (color.A == 0)
            {
                SetStroke(null);
            }
            else
            {
                colorBrush.Color = color;
                SetStroke(colorBrush);
            }
        }

        public void SetFillColor(Color color)
        {
            if (color.A == 0)
            {
                SetFill(null);
            }
            else
            {
                colorBrush.Color = color;
                SetFill(colorBrush);
            }
        }

        public void DrawRect(RectangleF rect, float strokeThickness)
        {
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, strokeThickness);
        }

        public void DrawLine(LineF line, float strokeThickness)
        {
            DrawLine(line.P1.X, line.P1.Y, line.P2.X, line.P2.Y, strokeThickness);
        }

        public void DrawRects(IEnumerable<RectangleF> rects, float strokeThickness)
        {
            foreach (var rect in rects)
            {
                DrawRect(rect, strokeThickness);
            }
        }

        public void DrawLines(IEnumerable<LineF> lines, float strokeThickness)
        {
            foreach (var line in lines)
            {
                DrawLine(line, strokeThickness);
            }
        }

        public void DrawRect(float x, float y, float width, float height, float strokeThickness)
        {
            DrawRoundedRect(x, y, width, height, 0, 0, strokeThickness);
        }

        public void DrawCircle(float cx, float cy, float radius, float strokeThickness)
        {
            DrawEllipse(cx, cy, radius, radius, strokeThickness);
        }

        double ratio = (double)(Math.PI / 180);
        protected float ToRadians(float degrees)
        {
            return (float)(degrees * ratio);
        }

        public abstract void DrawArc(float cx, float cy, float radius, float startAngle, float endAngle, bool includeCenterInStroke, float strokeThickness);

        public abstract void DrawEllipse(float cx, float cy, float radiusx, float radiusy, float strokeThickness);

        public abstract void DrawRoundedRect(float x, float y, float width, float height, float radiusX, float radiusY, float strokeThickness);
        public abstract void SetStroke(Brush brush);
        public abstract void SetFill(Brush brush);
        public abstract void DrawLine(float x1, float y1, float x2, float y2, float strokeThickness);
        public abstract void DrawPolygon(List<Vector2> vertices, float strokeThickness);
        public abstract void DrawPolyline(List<Vector2> vertices, float strokeThickness, bool isClosedPath = false);
        protected RectangleF FindBoundingBox(IEnumerable<Vector2> vertices, float strokeThickness = 0)
        {
            if (!vertices.Any()) return new RectangleF(0, 0, 0, 0);
            float minX = vertices.First().X;
            float maxX = minX;
            float minY = vertices.First().Y;
            float maxY = minY;
            foreach (var v in vertices)
            {
                if (v.X < minX) minX = v.X;
                if (v.X > maxX) maxX = v.X;
                if (v.Y < minY) minY = v.Y;
                if (v.Y > maxY) maxY = v.Y;
            }
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
