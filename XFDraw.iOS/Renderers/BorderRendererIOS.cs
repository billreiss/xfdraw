using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XFDraw.Controls;
using XFDraw.iOS.Renderers;

[assembly: ExportRenderer(typeof(Border), typeof(BorderRendererIOS))]
namespace XFDraw.iOS.Renderers
{
    public class BorderRendererIOS : VisualElementRenderer<Border>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Border> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
                SetupLayer();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName || e.PropertyName == Xamarin.Forms.Frame.OutlineColorProperty.PropertyName ||
                e.PropertyName == Xamarin.Forms.Frame.HasShadowProperty.PropertyName)
                SetupLayer();
        }

        void SetupLayer()
        {
            Layer.CornerRadius = (nfloat)Element.CornerRadius;
            if (Element.BackgroundColor == Color.Default)
                Layer.BackgroundColor = UIColor.White.CGColor;
            else
                Layer.BackgroundColor = Element.BackgroundColor.ToCGColor();
            Layer.ShadowOpacity = 0;

            if (Element.BorderColor == Color.Default)
                Layer.BorderColor = UIColor.Clear.CGColor;
            else
            {
                Layer.BorderColor = Element.BorderColor.ToCGColor();
                Layer.BorderWidth = (nfloat)Element.BorderThickness;
            }

            Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            Layer.ShouldRasterize = true;
        }
    }
}
