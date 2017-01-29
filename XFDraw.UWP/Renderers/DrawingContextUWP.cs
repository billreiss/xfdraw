using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                session.FillGeometry(geom, uFill);
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
                session.DrawGeometry(geom, uStroke, strokeThickness);
            }
        }

        public override void DrawEllipse(float cx, float cy, float radiusx, float radiusy, float strokeThickness)
        {
            var center = new System.Numerics.Vector2(cx, cy);
            var geom = CanvasGeometry.CreateEllipse(session, center, radiusx, radiusy);
            if (uFill != null)
                session.FillGeometry(geom, uFill);
            if (uStroke != null && strokeThickness > 0)
                session.DrawGeometry(geom, uStroke, strokeThickness);
        }

        public override void DrawLine(float x1, float y1, float x2, float y2, float strokeThickness)
        {
            if (uStroke != null) session.DrawLine(x1, y1, x2, y2, uStroke, strokeThickness);
        }

        public override void DrawPolygon(List<XFDraw.Numerics.Vector2> vertices, float strokeThickness)
        {
            var v0 = vertices.FirstOrDefault();
            if (v0 == null) return;
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
                session.FillGeometry(geom, uFill);                
            }
            if (uStroke != null)
            {
                session.DrawGeometry(geom, uStroke, strokeThickness);
            }
        }

        public override void DrawPolyline(List<XFDraw.Numerics.Vector2> vertices, float strokeThickness, bool isClosedPath = false)
        {
            var v0 = vertices.FirstOrDefault();
            if (v0 == null) return;
            CanvasPathBuilder builder = new CanvasPathBuilder(session);
            builder.BeginFigure(v0.X, v0.Y);
            foreach (var v in vertices.Skip(1))
            {
                builder.AddLine(v.X, v.Y);
            }
            builder.EndFigure(isClosedPath ? CanvasFigureLoop.Closed : CanvasFigureLoop.Open);
            var geom = CanvasGeometry.CreatePath(builder);
            if (uStroke != null)
            {
                session.DrawGeometry(geom, uStroke, strokeThickness);
            }
        }

        public override void DrawRoundedRect(float x, float y, float width, float height, float radiusX, float radiusY, float strokeThickness)
        {
            if (radiusX == 0 && radiusY == 0)
            {
                if (uFill != null)
                {
                    session.FillRectangle(x, y, width, height, uFill);
                }
                if (uStroke != null)
                {
                    session.DrawRectangle(x, y, width, height, uStroke, strokeThickness);
                }
            }
            else
            {
                if (uFill != null)
                {
                    session.FillRoundedRectangle(x, y, width, height, radiusX, radiusY, uFill);
                }
                if (uStroke != null)
                {
                    session.DrawRoundedRectangle(x, y, width, height, radiusX, radiusY, uStroke, strokeThickness);
                }
            }
        }

        public override void SetFill(Brush brush)
        {
            if (brush is SolidColorBrush)
            {
                var sb = brush as SolidColorBrush;
                uFill = new CanvasSolidColorBrush(session, Windows.UI.Color.FromArgb((byte)(sb.Color.A * 255), (byte)(sb.Color.R * 255), (byte)(sb.Color.G * 255), (byte)(sb.Color.B * 255)));
            }
            else
            {
                uFill = null;
            }
        }

        public override void SetStroke(Brush brush)
        {
            if (brush is SolidColorBrush)
            {
                var sb = brush as SolidColorBrush;
                uStroke = new CanvasSolidColorBrush(session, Windows.UI.Color.FromArgb((byte)(sb.Color.A * 255), (byte)(sb.Color.R * 255), (byte)(sb.Color.G * 255), (byte)(sb.Color.B * 255)));
            }
            else
            {
                uStroke = null;
            }
        }
    }
}
