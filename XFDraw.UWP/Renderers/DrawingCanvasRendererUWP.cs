using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;
using XFDraw;
using XFDraw.UWP.Renderers;

[assembly: ExportRenderer(typeof(DrawingCanvas), typeof(DrawingCanvasRendererUWP))]
namespace XFDraw.UWP.Renderers
{
    class DrawingCanvasRendererUWP : ViewRenderer<DrawingCanvas, CanvasControl>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DrawingCanvas> e)
        {
            base.OnElementChanged(e);
            if (Control == null && e.NewElement != null)
            {
                var canvas = new CanvasControl();
                canvas.Draw += Canvas_Draw;
                SetNativeControl(canvas);
            }
            if (e.OldElement != null)
            {
                (e.OldElement as DrawingCanvas).InvalidateCallback = null;
            }
            if (e.NewElement != null)
            {
                (e.NewElement as DrawingCanvas).InvalidateCallback = Invalidate;
            }
        }

        private void Invalidate()
        {
            Control.Invalidate();
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            DrawingContextUWP context = new DrawingContextUWP();
            context.BeginDraw(args.DrawingSession);
            Element.DrawingCallback(context);
        }
    }
}
