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
using XFDraw.Numerics;
using XColor = Xamarin.Forms.Color;

namespace XFDraw.Droid.Renderers
{
    internal class DrawingContextDroid : DrawingContext
    {
        public Canvas canvas;
        public float fontSize = 25;
        public float textHeight = 25;
        public float axisPadding = 10;

        bool doFill = false;
        bool doStroke = false;

        Paint aFill = new Paint();
        Paint aStroke = new Paint();
        LinearGradientBrush gFill;
        LinearGradientBrush gStroke;

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
            SetPaint(aStroke, brush, true, ref doStroke);
        }

        private void SetPaint(Paint paint, Brush brush, bool isStroke, ref bool shouldPaint)
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
                if (isStroke)
                {
                    gStroke = null;
                }
                else
                {
                    gFill = null;
                }
            }
            else if (brush is LinearGradientBrush)
            {
                var lgb = brush as LinearGradientBrush;
                shouldPaint = true;
                if (isStroke)
                {
                    gStroke = lgb;
                }
                else
                {
                    gFill = lgb;
                }
            }
        }

        public override void SetFill(Brush brush)
        {
            SetPaint(aFill, brush, false, ref doFill);
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
                if (gFill != null)
                {
                    ApplyGradient(aFill, gFill, rect);
                }
                canvas.DrawArc(rect, -startAngle, -sweepAngle, true, aFill);
                if (gFill != null)
                {
                    aFill.SetShader(null);
                }
            }
            if (doStroke)
            {
                if (gStroke != null)
                {
                    RectangleF r = new RectangleF(rect.Left, rect.Top, rect.Width(), rect.Height());
                    r = r.Expand(strokeThickness / 2);
                    ApplyGradient(aStroke, gStroke, r);
                }
                aStroke.StrokeWidth = ToPixels(strokeThickness);
                canvas.DrawArc(rect, -startAngle, -sweepAngle, includeCenterInStroke, aStroke);
                if (gStroke != null)
                {
                    aStroke.SetShader(null);
                }
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
                if (gFill != null)
                {
                    ApplyGradient(aFill, gFill, rect);
                }
                canvas.DrawOval(rect, aFill);
                if (gFill != null)
                {
                    aFill.SetShader(null);
                }
            }
            if (doStroke && strokeThickness > 0)
            {
                if (gStroke != null)
                {
                    RectangleF r = new RectangleF(rect.Left, rect.Top, rect.Width(), rect.Height());
                    r = r.Expand(strokeThickness / 2);
                    ApplyGradient(aStroke, gStroke, r);
                }
                aStroke.StrokeWidth = ToPixels(strokeThickness);
                canvas.DrawOval(rect, aStroke);
                if (gStroke != null)
                {
                    aStroke.SetShader(null);
                }
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
                if (gFill != null)
                {
                    RectangleF rect = new RectangleF(x, y, width, height);
                    ApplyGradient(aFill, gFill, rect);
                }

                if (radiusX == 0 && radiusY == 0)
                {
                    canvas.DrawRect(new RectF(x, y, x + width, y + height), aFill);
                }
                else
                {
                    canvas.DrawRoundRect(new RectF(x, y, x + width, y + height), radiusX, radiusY, aFill);
                }
                if (gFill != null) aFill.SetShader(null);
            }
            if (doStroke && borderThickness > 0)
            {
                aStroke.StrokeWidth = ToPixels(borderThickness);
                if (gStroke != null)
                {
                    RectangleF rect = new RectangleF(x, y, width, height);
                    rect  = rect.Expand(borderThickness / 2);
                    ApplyGradient(aStroke, gStroke, rect);
                }
                if (radiusX == 0 && radiusY == 0)
                {
                    canvas.DrawRect(new RectF(x, y, x + width, y + height), aStroke);
                }
                else
                {
                    canvas.DrawRoundRect(new RectF(x, y, x + width, y + height), radiusX, radiusY, aStroke);
                }
                if (gStroke != null) aStroke.SetShader(null);
            }
        }

        private void ApplyGradient(Paint paint, LinearGradientBrush brush, RectF rect, bool needsConvertToPixels = false)
        {
            var transform = Matrix3x2.CreateScale(rect.Width(), rect.Height()) * Matrix3x2.CreateTranslation(rect.Left, rect.Top);
            var start = Vector2.Transform(brush.StartPoint, transform);
            var end = Vector2.Transform(brush.EndPoint, transform);
            var gradient = new LinearGradient(start.X, start.Y, end.X, end.Y, brush.GradientStops.Select(g => g.Color.ToAndroid().ToArgb()).ToArray(), brush.GradientStops.Select(g => (float)g.Offset).ToArray(), Shader.TileMode.Clamp);
            paint.SetShader(gradient);
        }

        private void ApplyGradient(Paint paint, LinearGradientBrush brush, RectangleF rect, bool needsConvertToPixels = false)
        {
            Matrix3x2 transform;
            if (!needsConvertToPixels)
            {
                transform = Matrix3x2.CreateScale(rect.Width, rect.Height) * Matrix3x2.CreateTranslation(rect.Left, rect.Top);
            }
            else
            {
                transform = Matrix3x2.CreateScale(ToPixels(rect.Width), ToPixels(rect.Height)) * Matrix3x2.CreateTranslation(ToPixels(rect.Left), ToPixels(rect.Top));
            }
            var start = Vector2.Transform(brush.StartPoint, transform);
            var end = Vector2.Transform(brush.EndPoint, transform);
            var gradient = new LinearGradient(start.X, start.Y, end.X, end.Y, brush.GradientStops.Select(g => g.Color.ToAndroid().ToArgb()).ToArray(), brush.GradientStops.Select(g => (float)g.Offset).ToArray(), Shader.TileMode.Clamp);
            paint.SetShader(gradient);
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
                if (gStroke != null)
                {
                    RectangleF rect = FindBoundingBox(new Vector2[] { new Vector2(x1, y1), new Vector2(x2, y2) }, 0);
                    rect = rect.Expand(lineThickness / 2);
                    ApplyGradient(aStroke, gStroke, rect);
                }
                aStroke.SetStyle(Paint.Style.Fill);
                canvas.DrawLine(x1, y1, x2, y2, aStroke);
                aStroke.SetStyle(Paint.Style.Stroke);
                if (gStroke != null)
                {
                    aStroke.SetShader(null);
                }
            }
        }

        public override void DrawPolygon(List<Vector2> vertices, float lineThickness)
        {
            if (doFill)
            {
                if (gFill != null)
                {
                    RectangleF rect = FindBoundingBox(vertices, 0);
                    ApplyGradient(aFill, gFill, rect, true);
                }
                DrawPathInternal(vertices, true, aFill);
                if (gFill != null) aFill.SetShader(null);
            }

            DrawPolyline(vertices, lineThickness, true);
        }

        public override void DrawPolyline(List<Vector2> vertices, float lineThickness, bool isClosedPath = false)
        {
            if (lineThickness == 0 || !doStroke) return;
            if (gStroke != null)
            {
                RectangleF rect = FindBoundingBox(vertices, lineThickness);
                ApplyGradient(aStroke, gStroke, rect, true);
            }
            aStroke.StrokeWidth = ToPixels(lineThickness);
            DrawPathInternal(vertices, isClosedPath, aStroke);
            if (gStroke != null) aStroke.SetShader(null);
        }

        private void DrawPathInternal(List<Vector2> vertices, bool isClosedPath, Paint paint)
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