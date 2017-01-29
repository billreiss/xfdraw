using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFDraw.Controls
{
    public class CircularProgress : DrawingCanvas
    {
        public static readonly BindableProperty ProgressProperty = 
            BindableProperty.Create("Progress", typeof(double), 
                typeof(ProgressBar), 0d, 
                coerceValue: (bo, v) => Clamp((double)v, 0d, 1d), 
                propertyChanged:OnPropertyChanged);

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly BindableProperty StrokeThicknessProperty =
            BindableProperty.Create("StrokeThickness", typeof(double),
            typeof(ProgressBar), 5d,
            propertyChanged: OnPropertyChanged);

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly BindableProperty UnfilledColorProperty =
            BindableProperty.Create("UnfilledColor", typeof(Color),
            typeof(ProgressBar), Color.FromHex("#ECECEC"),
            propertyChanged: OnPropertyChanged);

        public Color UnfilledColor
        {
            get { return (Color)GetValue(UnfilledColorProperty); }
            set { SetValue(UnfilledColorProperty, value); }
        }

        public static readonly BindableProperty FilledColorProperty =
            BindableProperty.Create("FilledColor", typeof(Color),
            typeof(ProgressBar), Color.FromHex("#5484F5"),
            propertyChanged: OnPropertyChanged);

        public Color FilledColor
        {
            get { return (Color)GetValue(FilledColorProperty); }
            set { SetValue(FilledColorProperty, value); }
        }
        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var dc = bindable as DrawingCanvas;
            if (dc != null)
            {
                dc.Invalidate();
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            Invalidate();
            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        private static double Clamp(double v1, double min, double max)
        {
            if (v1 < min) return min;
            if (v1 > max) return max;
            return v1;
        }

        protected override void OnDraw(DrawingContext context)
        {
            base.OnDraw(context);
            double r = (Math.Min(this.Width, this.Height) - StrokeThickness) / 2;
            double cx = this.Width / 2;
            double cy = this.Height / 2;
            if (Progress < .0001)
            {
                context.SetStrokeColor(UnfilledColor);
                context.DrawCircle((float)cx, (float)cy, (float)r, (float)StrokeThickness);
            }
            else if (Progress > .9999)
            {
                context.SetStrokeColor(FilledColor);
                context.DrawCircle((float)cx, (float)cy, (float)r, (float)StrokeThickness);
            }
            else
            {
                float startAngle = (float)(90 - Progress * 360);
                float endAngle = 90f;
                context.SetStrokeColor(UnfilledColor);
                context.DrawArc((float)cx, (float)cy, (float)r, endAngle, startAngle + 360, false, (float)StrokeThickness);
                context.SetStrokeColor(FilledColor);
                context.DrawArc((float)cx, (float)cy, (float)r, startAngle, endAngle, false, (float)StrokeThickness);
            }
        }
    }
}
