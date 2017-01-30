using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFDraw.Numerics;

namespace XFDraw
{
    public class LinearGradientBrush : GradientBrush
    {
        public LinearGradientBrush()
        {
            StartPoint = Vector2.Zero;
            EndPoint = Vector2.One;
        }

        public Vector2 StartPoint { get; set; }
        public Vector2 EndPoint { get; set; }
    }
}
