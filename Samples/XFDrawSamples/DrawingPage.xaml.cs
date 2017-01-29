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
            ctx.SetStrokeColor(Color.Green);
            ctx.SetFillColor(Color.Navy);
            ctx.DrawRect(200, 25, 100, 50, 10);
            ctx.DrawRoundedRect(200, 100, 100, 50, 20, 20, 10);
            ctx.DrawPolyline(polyLine, 2, false);
        }
    }
}
