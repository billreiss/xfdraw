using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFDraw.Controls
{
    public class DrawingCanvas : View
    {
        public event EventHandler<DrawEventArgs> Draw;

        internal Action InvalidateCallback { get; set; }
        internal void DrawingCallback(DrawingContext context)
        {
            OnDraw(context);
            Draw?.Invoke(this, new DrawEventArgs(context));
        }

        protected virtual void OnDraw(DrawingContext context)
        {
        }

        public void Invalidate()
        {
            if (InvalidateCallback != null) InvalidateCallback();
        }
    }
}
