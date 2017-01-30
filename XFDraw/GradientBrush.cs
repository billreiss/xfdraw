using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFDraw
{
    public abstract class GradientBrush : Brush
    {
        public List<GradientStop> GradientStops { get; private set; }
        public GradientBrush()
        {
            GradientStops = new List<GradientStop>();
        }
    }
}
