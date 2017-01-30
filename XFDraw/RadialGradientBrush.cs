using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFDraw.Numerics;

namespace XFDraw
{
    public class RadialGradientBrush : GradientBrush
    {
        public RadialGradientBrush()
        {
            Center = Vector2.One / 2;
            Radius = .5f;
        }

        public Vector2 Center { get; set; }
        public float Radius { get; set; }
    }
}
