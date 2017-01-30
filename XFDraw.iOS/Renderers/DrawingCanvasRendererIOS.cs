using System;
using System.Collections.Generic;
using System.Text;
using CoreGraphics;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using XFDraw;
using XFDraw.iOS.Renderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(DrawingCanvas), typeof(DrawingCanvasRendererIOS))]
namespace XFDraw.iOS.Renderers
{
    public class DrawingCanvasRendererIOS : ViewRenderer<DrawingCanvas, UIView>
    {
        DrawingContextIOS drawingContext = new DrawingContextIOS();

        protected override void OnElementChanged(ElementChangedEventArgs<DrawingCanvas> e)
        {
            base.OnElementChanged(e);

            if (Control == null && e.NewElement != null)
            {
                SetNativeControl(new UIView());
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
            SetNeedsDisplay();
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                drawingContext.BeginDraw(g, Control);
                Element.DrawingCallback(drawingContext);
            }
        }
    }

}
