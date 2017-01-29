using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XFDraw.Droid.Renderers;
using XFDraw;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(DrawingCanvas), typeof(DrawingCanvasRendererDroid))]
namespace XFDraw.Droid.Renderers
{
    public class DrawingCanvasRendererDroid : ViewRenderer<DrawingCanvas, Android.Views.View>
    {
        DrawingContextDroid drawingContext = new DrawingContextDroid();
        public DrawingCanvasRendererDroid()
        {
            this.SetWillNotDraw(false);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DrawingCanvas> e)
        {
            base.OnElementChanged(e);
            if (Control == null && e.NewElement != null)
            {
                SetNativeControl(new Android.Views.View(Context));
                Invalidate();
            }
            if (e.OldElement != null)
            {
                (e.OldElement as DrawingCanvas).InvalidateCallback = null;
            }
            if (e.NewElement != null)
            {
                (e.NewElement as DrawingCanvas).InvalidateCallback = InvalidateCanvas;
            }
        }

        private void InvalidateCanvas()
        {
            try
            {
                Invalidate();
            }
            catch
            {

            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Element != null) Element.InvalidateCallback = null;
            base.Dispose(disposing);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            drawingContext.BeginDraw(canvas);
            Element.DrawingCallback(drawingContext);
        }
    }
}