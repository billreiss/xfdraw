using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFDraw.Numerics;

namespace XFDraw.UWP.Renderers
{
    class DrawingContextUWP : DrawingContext
    {
        CanvasDrawingSession session;
        ICanvasBrush uStroke;
        ICanvasBrush uFill;
        internal void BeginDraw(CanvasDrawingSession session)
        {
            this.session = session;
        }

        public override void DrawArc(float cx, float cy, float radius, float startAngle, float endAngle, bool includeCenterInStroke, float strokeThickness)
        {
            var rect = new RectangleF(cx - radius, cy - radius, radius * 2, radius * 2);
            var center = new System.Numerics.Vector2(cx, cy);
            var start = -ToRadians(startAngle) + (float)(Math.PI * 2);
            var end = -ToRadians(endAngle) + (float)(Math.PI * 2);
            if (uFill != null)
            {
                CanvasPathBuilder builder = new CanvasPathBuilder(session);
                builder.BeginFigure(center);
                builder.AddArc(center, radius, radius, end, start - end);
                builder.EndFigure(CanvasFigureLoop.Closed);
                var geom = CanvasGeometry.CreatePath(builder);
                session.FillGeometry(geom, TransformBrush(rect, uFill));
            }
            if (uStroke != null && strokeThickness > 0)
            {
                CanvasPathBuilder builder = new CanvasPathBuilder(session);
                if (!includeCenterInStroke)
                {
                    var rotation = System.Numerics.Matrix3x2.CreateRotation(end);
                    builder.BeginFigure(center + System.Numerics.Vector2.Transform(new System.Numerics.Vector2(radius, 0), rotation));
                    builder.AddArc(center, radius, radius, end, start - end);
                    builder.EndFigure(CanvasFigureLoop.Open);
                }
                else
                {
                    builder.BeginFigure(center);
                    builder.AddArc(center, radius, radius, end, start - end);
                    builder.EndFigure(CanvasFigureLoop.Closed);
                }
                var geom = CanvasGeometry.CreatePath(builder);
                session.DrawGeometry(geom, TransformBrush(rect.Expand(strokeThickness / 2), uStroke), strokeThickness);
            }
        }

        public override void DrawEllipse(float cx, float cy, float radiusx, float radiusy, float strokeThickness)
        {
            var rect = new RectangleF(cx - radiusx, cy - radiusy , radiusx * 2, radiusy * 2);
            var center = new System.Numerics.Vector2(cx, cy);
            var geom = CanvasGeometry.CreateEllipse(session, center, radiusx, radiusy);
            if (uFill != null)
                session.FillGeometry(geom, TransformBrush(rect, uFill));
            if (uStroke != null && strokeThickness > 0)
                session.DrawGeometry(geom, TransformBrush(rect.Expand(strokeThickness/2), uStroke), strokeThickness);
        }

        public override void DrawLine(float x1, float y1, float x2, float y2, float strokeThickness)
        {
            if (uStroke != null && strokeThickness > 0)
            {
                RectangleF rect = new RectangleF(x1, y1, x2 - x1, y2 - y1).Expand(strokeThickness / 2);
                session.DrawLine(x1, y1, x2, y2, TransformBrush(rect, uStroke), strokeThickness);
            }
        }

        public override void DrawPolygon(List<XFDraw.Numerics.Vector2> vertices, float strokeThickness)
        {
            var v0 = vertices.FirstOrDefault();
            if (v0 == null) return;
            Numerics.RectangleF rect = FindBoundingBox(vertices);
            CanvasPathBuilder builder = new CanvasPathBuilder(session);
            builder.BeginFigure(v0.X, v0.Y);
            foreach (var v in vertices.Skip(1))
            {
                builder.AddLine(v.X, v.Y);
            }
            builder.EndFigure(CanvasFigureLoop.Closed);
            var geom = CanvasGeometry.CreatePath(builder);
            if (uFill != null)
            {
                session.FillGeometry(geom, TransformBrush(rect, uFill));                
            }
            if (uStroke != null && strokeThickness > 0)
            {
                session.DrawGeometry(geom, TransformBrush(rect.Expand(strokeThickness / 2), uStroke), strokeThickness);
            }
        }

        public override void DrawPolyline(List<XFDraw.Numerics.Vector2> vertices, float strokeThickness, bool isClosedPath = false)
        {
            var v0 = vertices.FirstOrDefault();
            if (v0 == null) return;
            if (uStroke != null && strokeThickness > 0)
            {
                CanvasPathBuilder builder = new CanvasPathBuilder(session);
                Numerics.RectangleF rect = FindBoundingBox(vertices);
                rect = rect.Expand(strokeThickness / 2);
                builder.BeginFigure(v0.X, v0.Y);
                foreach (var v in vertices.Skip(1))
                {
                    builder.AddLine(v.X, v.Y);
                }
                builder.EndFigure(isClosedPath ? CanvasFigureLoop.Closed : CanvasFigureLoop.Open);
                var geom = CanvasGeometry.CreatePath(builder); session.DrawGeometry(geom, TransformBrush(rect, uStroke), strokeThickness);
            }
        }

        public override void DrawRoundedRect(float x, float y, float width, float height, float radiusX, float radiusY, float strokeThickness)
        {
            var rect = new RectangleF(x, y, width, height);
            if (radiusX == 0 && radiusY == 0)
            {
                if (uFill != null)
                {
                    session.FillRectangle(x, y, width, height, TransformBrush(rect, uFill));
                }
                if (uStroke != null && strokeThickness > 0)
                {
                    session.DrawRectangle(x, y, width, height, TransformBrush(rect.Expand(strokeThickness / 2), uStroke), strokeThickness);
                }
            }
            else
            {
                if (uFill != null)
                {
                    session.FillRoundedRectangle(x, y, width, height, radiusX, radiusY, TransformBrush(rect, uFill));
                }
                if (uStroke != null)
                {
                    session.DrawRoundedRectangle(x, y, width, height, radiusX, radiusY, TransformBrush(rect.Expand(strokeThickness / 2), uStroke), strokeThickness);
                }
            }
        }

        private ICanvasBrush TransformBrush(RectangleF rect, ICanvasBrush brush)
        {
            return TransformBrush(rect.X, rect.Y, rect.Width, rect.Height, brush);
        }

        private ICanvasBrush TransformBrush(float x, float y, float width, float height, ICanvasBrush brush)
        {
            if (brush is CanvasLinearGradientBrush)
            {
                (brush as CanvasLinearGradientBrush).Transform = System.Numerics.Matrix3x2.CreateScale(width, height) * System.Numerics.Matrix3x2.CreateTranslation(x, y);
            }
            return brush;
        }

        public override void SetFill(Brush brush)
        {
            if (brush is SolidColorBrush)
            {
                var sb = brush as SolidColorBrush;
                uFill = new CanvasSolidColorBrush(session, ToWin2DColor(sb.Color));
            }
            else if (brush is LinearGradientBrush)
            {
                var lb = brush as LinearGradientBrush;
                var clgb = new CanvasLinearGradientBrush(session, lb.GradientStops.Select(s => new CanvasGradientStop() { Position = (float)s.Offset, Color = ToWin2DColor(s.Color) }).ToArray());
                clgb.StartPoint = new System.Numerics.Vector2(lb.StartPoint.X, lb.StartPoint.Y);
                clgb.EndPoint = new System.Numerics.Vector2(lb.EndPoint.X, lb.EndPoint.Y);
                uFill = clgb;
            }
            else
            {
                uFill = null;
            }
        }

        private static Windows.UI.Color ToWin2DColor(Xamarin.Forms.Color color)
        {
            return Windows.UI.Color.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
        }

        public override void SetStroke(Brush brush)
        {
            if (brush is SolidColorBrush)
            {
                var sb = brush as SolidColorBrush;
                uStroke = new CanvasSolidColorBrush(session, Windows.UI.Color.FromArgb((byte)(sb.Color.A * 255), (byte)(sb.Color.R * 255), (byte)(sb.Color.G * 255), (byte)(sb.Color.B * 255)));
            }
            else if (brush is LinearGradientBrush)
            {
                var lb = brush as LinearGradientBrush;
                var clgb = new CanvasLinearGradientBrush(session, lb.GradientStops.Select(s => new CanvasGradientStop() { Position = (float)s.Offset, Color = ToWin2DColor(s.Color) }).ToArray());
                clgb.StartPoint = new System.Numerics.Vector2(lb.StartPoint.X, lb.StartPoint.Y);
                clgb.EndPoint = new System.Numerics.Vector2(lb.EndPoint.X, lb.EndPoint.Y);
                uStroke = clgb;
            }
            else
            {
                uStroke = null;
            }
        }
    }
}
