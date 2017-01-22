using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XFDraw;

namespace XFDrawSamples
{
    public partial class MainPage : ContentPage
    {
        float boxSize = 100;
        Color fillColor = Color.Blue;
        public MainPage()
        {
            InitializeComponent();
        }

        protected void DrawingCanvas_Draw(object sender, DrawEventArgs args)
        {
            var ctx = args.Context;
            ctx.SetStrokeColor(Color.Red);
            ctx.SetFillColor(fillColor);
            ctx.DrawRoundedRect(210, 10, boxSize, boxSize, 10, 10, 5);
        }

        protected void Button_Clicked(object sender, EventArgs args)
        {
            borderLabel.Margin = new Thickness(40);
            fillColor = Color.Green;
            canvas.Invalidate();
        }
    }
}
