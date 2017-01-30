using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using XFDraw;
using XFDraw.Numerics;

namespace XFDrawSamples
{
    public partial class DrawingPage : ContentPage
    {
        List<Vector2> polyLine = new List<Vector2>() { new Vector2(25, 225), new Vector2(45, 200), new Vector2(65, 275), new Vector2(85, 205), new Vector2(105, 225) };
        public DrawingPage()
        {
            InitializeComponent();
        }

        protected void Canvas_Draw(object sender, DrawEventArgs args)
        {
            var ctx = args.Context;
            ctx.SetStrokeColor(Color.Red);
            ctx.SetFillColor(Color.Blue);
            ctx.DrawEllipse(75, 50, 50, 30, 5);
            ctx.DrawArc(75, 135, 40, 0, 90, true, 10);
            ctx.SetFill(null);
            ctx.SetStrokeColor(Color.Blue);
            ctx.DrawArc(75, 135, 40, 90, 180, false, 10);
            ctx.SetStrokeColor(Color.Orange);
            ctx.DrawArc(75, 135, 40, 180, 270, false, 10);
            ctx.SetStrokeColor(Color.Teal);
            ctx.DrawArc(75, 135, 40, 270, 360, false, 10);
            var sgb = new LinearGradientBrush();
            sgb.StartPoint = new Vector2(0, 1);
            sgb.EndPoint = new Vector2(1, 0);
            sgb.GradientStops.Add(new GradientStop() { Color = Color.Yellow, Offset = 0 });
            sgb.GradientStops.Add(new GradientStop() { Color = Color.Green, Offset = 1 });
            ctx.SetStroke(sgb);
            var fgb = new LinearGradientBrush();
            fgb.GradientStops.Add(new GradientStop() { Color = Color.Red, Offset = 0 });
            fgb.GradientStops.Add(new GradientStop() { Color = Color.Blue, Offset = 1 });
            ctx.SetFill(fgb);
            ctx.DrawRect(200, 25, 100, 50, 10);
            ctx.DrawRoundedRect(200, 100, 100, 50, 20, 20, 10);
//            ctx.SetStrokeColor(Color.Purple);
            //            ctx.DrawPolyline(polyLine, 4, false);
            ctx.DrawPolygon(polyLine, 4);
        }
    }
}
