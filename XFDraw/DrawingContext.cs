using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFDraw
{
    public abstract class DrawingContext
    {
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

        public abstract void DrawRoundedRect(float x, float y, float width, float height, float radiusX, float radiusY, float strokeThickness);
        public abstract void SetStrokeColor(Color color);
        public abstract void SetFillColor(Color color);
        public abstract void DrawLine(float x1, float y1, float x2, float y2, float strokeThickness);
        public abstract void DrawPolygon(List<PointF> vertices, float strokeThickness);
        public abstract void DrawPolyline(List<PointF> vertices, float strokeThickness, bool isClosedPath = false);
    }
}
