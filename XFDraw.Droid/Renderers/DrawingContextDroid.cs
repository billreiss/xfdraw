using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XColor = Xamarin.Forms.Color;

namespace XFDraw.Droid.Renderers
{
    internal class DrawingContextDroid : DrawingContext
    {
        public Canvas canvas;
        public XFDraw.RectangleF viewport;
        public float fontSize = 25;
        public float textHeight = 25;
        public float axisPadding = 10;

        bool doFill = false;
        bool doStroke = false;

        Paint aFill = new Paint();
        Paint aStroke = new Paint();
        static float fontScale = -1;

        public DrawingContextDroid()
        {
            aFill.SetStyle(Paint.Style.Fill);
            aStroke.SetStyle(Paint.Style.Stroke);
            if (fontScale < 0)
            {
                var metrics = Resources.System.DisplayMetrics;
                fontScale = metrics.Density / 2;
            }
            textHeight = 25 * fontScale;
            axisPadding = 10 * fontScale;
        }

        internal void BeginDraw(Canvas canvas)
        {
            this.canvas = canvas;
        }
        public override void SetStroke(Brush brush)
        {
            SetPaint(aStroke, brush, ref doStroke);
        }

        private void SetPaint(Paint paint, Brush brush, ref bool shouldPaint)
        {
            shouldPaint = true;
            if (brush == null)
            {
                shouldPaint = false;
                return;
            }
            if (brush is SolidColorBrush)
            {
                var color = (brush as SolidColorBrush).Color.ToAndroid();
                if (brush.Opacity != 1)
                {
                    color.A = (byte)(color.A * brush.Opacity);
                }
                if (color.A == 0)
                {
                    shouldPaint = false;
                    return;
                }
                paint.Color = color;
            }
        }

        public override void SetFill(Brush brush)
        {
            SetPaint(aFill, brush, ref doFill);
        }

        public override void DrawArc(float cx, float cy, float radius, float startAngle, float endAngle, bool includeCenterInStroke, float strokeThickness)
        {
            cx = ToPixels(cx);
            cy = ToPixels(cy);
            radius = ToPixels(radius);
            RectF rect = new RectF(cx - radius, cy - radius, cx + radius, cy + radius);
            var sweepAngle = endAngle - startAngle;
            if (sweepAngle < 0) sweepAngle += 360;
            if (doFill)
            {
                canvas.DrawArc(rect, -startAngle, -sweepAngle, true, aFill);
            }
            if (doStroke)
            {
                aStroke.StrokeWidth = ToPixels(strokeThickness);
                canvas.DrawArc(rect, -startAngle, -sweepAngle, includeCenterInStroke, aStroke);
            }
        }

        public override void DrawEllipse(float cx, float cy, float radiusx, float radiusy, float strokeThickness)
        {
            cx = ToPixels(cx);
            cy = ToPixels(cy);
            radiusx = ToPixels(radiusx);
            radiusy = ToPixels(radiusy);
            var rect = new RectF(cx - radiusx, cy - radiusy, cx + radiusx, cy + radiusy);
            if (doFill)
            {
                canvas.DrawOval(rect, aFill);
            }
            if (doStroke && strokeThickness > 0)
            {
                aStroke.StrokeWidth = ToPixels(strokeThickness);
                canvas.DrawOval(rect, aStroke);
            }
        }

        public override void DrawRoundedRect(float x, float y, float width, float height, float radiusX, float radiusY, float borderThickness)
        {
            x = ToPixels(x);
            y = ToPixels(y);
            width = ToPixels(width);
            height = ToPixels(height);
            radiusX = ToPixels(radiusX);
            radiusY = ToPixels(radiusY);

            if (doFill)
            {
                if (radiusX == 0 && radiusY == 0)
                {
                    canvas.DrawRect(new RectF(x, y, x + width, y + height), aFill);
                }
                else
                {
                    canvas.DrawRoundRect(new RectF(x, y, x + width, y + height), radiusX, radiusY, aFill);
                }
            }
            if (doStroke && borderThickness > 0)
            {
                aStroke.StrokeWidth = ToPixels(borderThickness);
                if (radiusX == 0 && radiusY == 0)
                {
                    canvas.DrawRect(new RectF(x, y, x + width, y + height), aStroke);
                }
                else
                {
                    canvas.DrawRoundRect(new RectF(x, y, x + width, y + height), radiusX, radiusY, aStroke);
                }
            }
        }

        public override void DrawLine(float x1, float y1, float x2, float y2, float lineThickness)
        {
            if (lineThickness > 0 && doStroke)
            {
                x1 = ToPixels(x1);
                y1 = ToPixels(y1);
                x2 = ToPixels(x2);
                y2 = ToPixels(y2);
                aStroke.StrokeWidth = ToPixels(lineThickness);
                aStroke.SetStyle(Paint.Style.Fill);
                canvas.DrawLine(x1, y1, x2, y2, aStroke);
                aStroke.SetStyle(Paint.Style.Stroke);
            }
        }

        public override void DrawPolygon(List<PointF> vertices, float lineThickness)
        {
            if (doFill) DrawPathInternal(vertices, true, aFill);
            DrawPolyline(vertices, lineThickness, true);
        }

        public override void DrawPolyline(List<PointF> vertices, float lineThickness, bool isClosedPath = false)
        {
            if (lineThickness == 0 || !doStroke) return;
            aStroke.StrokeWidth = ToPixels(lineThickness);
            if (doStroke) DrawPathInternal(vertices, isClosedPath, aStroke);
        }

        private void DrawPathInternal(List<PointF> vertices, bool isClosedPath, Paint paint)
        {
            Path path = new Path();
            var v1 = vertices.FirstOrDefault();
            bool first = true;
            foreach (var v in vertices)
            {
                if(!first)
                {
                    path.LineTo(ToPixels(v.X), ToPixels(v.Y));
                }
                else
                {
                    first = false;
                    path.MoveTo(ToPixels(v.X), ToPixels(v.Y));
                }
            }
            if (isClosedPath)
            {
                if (vertices.Any())
                {
                    path.LineTo(ToPixels(v1.X), ToPixels(v1.Y));
                }
                path.Close();
            }
            canvas.DrawPath(path, paint);
        }

        private float ToPixels(float value)
        {
            if (s_displayDensity == float.MinValue) SetupMetrics(Forms.Context);
            return value * s_displayDensity;
        }

        static void SetupMetrics(Context context)
        {
            if (s_displayDensity != float.MinValue)
                return;

            using (DisplayMetrics metrics = context.Resources.DisplayMetrics)
                s_displayDensity = metrics.Density;
        }

        static float s_displayDensity = float.MinValue;

    }
}