using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFDraw
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected void DrawingCanvas_Draw(object sender, DrawEventArgs args)
        {
            var ctx = args.Context;
            ctx.SetStrokeColor(Color.Red);
            ctx.SetFillColor(Color.Blue);
            ctx.DrawRoundedRect(10, 10, 100, 100, 10, 10, 5);
        }
    }
}
