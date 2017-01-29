using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace XFDrawSamples
{
    public partial class ControlsPage : ContentPage
    {
        bool stopTimer = false;
        public ControlsPage()
        {
            var p = new XFDraw.Controls.CircularProgress();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Device.StartTimer(TimeSpan.FromSeconds(.1), OnTimer);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            stopTimer = true;
        }

        private bool OnTimer()
        {
            var p = progress.Progress + .01;
            progress.Progress = p % 1;
            return !stopTimer;
        }
    }
}
