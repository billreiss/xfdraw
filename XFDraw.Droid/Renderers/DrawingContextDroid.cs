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
        Paint paint = new Paint();
        public float fontSize = 25;
        public float textHeight = 25;
        public float axisPadding = 10;

        XColor strokeColor;
        XColor fillColor;
        XColor? fillColor2;

        Android.Graphics.Color aFill;
        Android.Graphics.Color aStroke;
        Android.Graphics.Color? aFill2;
        Android.Graphics.Color aForeground;
        static float fontScale = -1;

        public DrawingContextDroid()
        {
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
        public override void SetStrokeColor(XColor color)
        {
            if (strokeColor != color)
            {
                strokeColor = color;
                aStroke = strokeColor.ToAndroid();
            }
        }
        public override void SetFillColor(XColor color)
        {
            if (fillColor != color)
            {
                fillColor = color;
                aFill = fillColor.ToAndroid();
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

            if (aFill.A > 0)
            {
                paint.Color = aFill;
                paint.SetStyle(Paint.Style.Fill);
                if (radiusX == 0 && radiusY == 0)
                {
                    canvas.DrawRect(new RectF(x, y, x + width, y + height), paint);
                }
                else
                {
                    canvas.DrawRoundRect(new RectF(x, y, x + width, y + height), radiusX, radiusY, paint);
                }
            }
            if (strokeColor.A > 0)
            {
                paint.Color = aStroke;
                paint.SetStyle(Paint.Style.Stroke);
                paint.StrokeWidth = ToPixels(borderThickness);
                if (radiusX == 0 && radiusY == 0)
                {
                    canvas.DrawRect(new RectF(x, y, x + width, y + height), paint);
                }
                else
                {
                    canvas.DrawRoundRect(new RectF(x, y, x + width, y + height), radiusX, radiusY, paint);
                }
            }
        }

        public override void DrawLine(float x1, float y1, float x2, float y2, float lineThickness)
        {
            x1 = ToPixels(x1);
            y1 = ToPixels(y1);
            x2 = ToPixels(x2);
            y2 = ToPixels(y2);
            lineThickness = ToPixels(lineThickness);
            paint.Color = aStroke;
            paint.StrokeWidth = lineThickness;
            paint.SetStyle(Paint.Style.Fill);
            canvas.DrawLine(x1, y1, x2, y2, paint);
        }

        public override void DrawPolygon(List<PointF> vertices, float lineThickness)
        {
            paint.StrokeWidth = lineThickness;
            paint.Color = aFill;
            paint.SetStyle(Paint.Style.Fill);
            DrawPathInternal(vertices, true);
            DrawPolyline(vertices, lineThickness, true);
        }

        public override void DrawPolyline(List<PointF> vertices, float lineThickness, bool isClosedPath = false)
        {
            if (lineThickness == 0 || aStroke.A == 0) return;
            paint.StrokeWidth = lineThickness;
            paint.Color = aStroke;
            paint.SetStyle(Paint.Style.Stroke);
            DrawPathInternal(vertices, isClosedPath);
        }

        private void DrawPathInternal(List<PointF> vertices, bool isClosedPath)
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