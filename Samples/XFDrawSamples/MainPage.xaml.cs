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
        public MainPage()
        {
            InitializeComponent();
        }

        protected async void OnDrawingClicked(object sender, EventArgs args)
        {
            await NavigationService.Instance.NavigateToAsync(typeof(DrawingPage), true);
        }

        protected async void OnControlsClicked(object sender, EventArgs args)
        {
            await NavigationService.Instance.NavigateToAsync(typeof(ControlsPage), true);
        }
    }
}
