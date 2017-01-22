using System.ComponentModel;
using Android.Graphics;
using Android.Graphics.Drawables;
using AButton = Android.Widget.Button;
using ACanvas = Android.Graphics.Canvas;
using GlobalResource = Android.Resource;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using XFDraw.Controls;
using XFDraw.Droid.Renderers;

[assembly: ExportRenderer(typeof(Border), typeof(BorderRendererDroid))]
namespace XFDraw.Droid.Renderers
{
    class BorderRendererDroid : VisualElementRenderer<Border>
    {
        bool _disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && !_disposed)
            {
                Background.Dispose();
                _disposed = true;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Border> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && e.OldElement == null)
                UpdateBackground();
        }

        void UpdateBackground()
        {
            this.SetBackground(new BorderDrawable(Element));
        }

        class BorderDrawable : Drawable
        {
            readonly Border _border;
            DrawingContextDroid _drawingContext = new DrawingContextDroid();

            bool _isDisposed;
            Bitmap _normalBitmap;

            public BorderDrawable(Border border)
            {
                _border = border;
                border.PropertyChanged += FrameOnPropertyChanged;
            }

            public override bool IsStateful
            {
                get { return false; }
            }

            public override int Opacity
            {
                get { return 0; }
            }

            public override void Draw(ACanvas canvas)
            {
                int width = Bounds.Width();
                int height = Bounds.Height();

                if (width <= 0 || height <= 0)
                {
                    if (_normalBitmap != null)
                    {
                        _normalBitmap.Dispose();
                        _normalBitmap = null;
                    }
                    return;
                }

                if (_normalBitmap == null || _normalBitmap.Height != height || _normalBitmap.Width != width)
                {
                    // If the user changes the orientation of the screen, make sure to detroy reference before
                    // reassigning a new bitmap reference.
                    if (_normalBitmap != null)
                    {
                        _normalBitmap.Dispose();
                        _normalBitmap = null;
                    }

                    _normalBitmap = CreateBitmap(false, width, height);
                }
                Bitmap bitmap = _normalBitmap;
                using (var paint = new Paint())
                    canvas.DrawBitmap(bitmap, 0, 0, paint);
            }

            public override void SetAlpha(int alpha)
            {
            }

            public override void SetColorFilter(ColorFilter cf)
            {
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing && !_isDisposed)
                {
                    if (_normalBitmap != null)
                    {
                        _normalBitmap.Dispose();
                        _normalBitmap = null;
                    }

                    _isDisposed = true;
                }

                base.Dispose(disposing);
            }

            protected override bool OnStateChange(int[] state)
            {
                return false;
            }

            Bitmap CreateBitmap(bool pressed, int width, int height)
            {
                Bitmap bitmap;
                using (Bitmap.Config config = Bitmap.Config.Argb8888)
                    bitmap = Bitmap.CreateBitmap(width, height, config);

                using (var canvas = new ACanvas(bitmap))
                {
                    DrawBorder(canvas);
                }

                return bitmap;
            }

            private void DrawBorder(ACanvas canvas)
            {
                _drawingContext.BeginDraw(canvas);
                _drawingContext.SetFillColor(_border.BackgroundColor);
                _drawingContext.SetStrokeColor(_border.BorderColor);
                double borderThickness = _border.BorderThickness;
                float offset = (float)borderThickness / 2;
                float width = (float)(_border.Width - borderThickness);
                float height = (float)(_border.Height - borderThickness);
                float radius = (float)_border.CornerRadius;
                _drawingContext.DrawRoundedRect(offset, offset, width, height, radius, radius, (float)borderThickness);
            }

            void FrameOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName || e.PropertyName == Frame.OutlineColorProperty.PropertyName)
                {
                    using (var canvas = new ACanvas(_normalBitmap))
                    {
                        DrawBorder(canvas);
                    }
                    InvalidateSelf();
                }
            }
        }
    }
}