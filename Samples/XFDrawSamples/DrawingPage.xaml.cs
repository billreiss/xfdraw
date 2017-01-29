using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using XFDraw;

namespace XFDrawSamples
{
    public partial class DrawingPage : ContentPage
    {
        List<PointF> polyLine = new List<PointF>() { new PointF(25, 225), new PointF(45, 200), new PointF(65, 275), new PointF(85, 205), new PointF(105, 225) };
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
            ctx.SetFill(null);
            ctx.DrawArc(75, 150, 50, 0, 90, false, 10);
            ctx.SetStrokeColor(Color.Blue);
            ctx.DrawArc(75, 150, 50, 90, 180, false, 10);
            ctx.SetStrokeColor(Color.Green);
            ctx.SetFillColor(Color.Navy);
            ctx.DrawRect(200, 25, 100, 50, 10);
            ctx.DrawRoundedRect(200, 100, 100, 50, 20, 20, 10);
            ctx.DrawPolyline(polyLine, 2, false);
        }
    }
}
