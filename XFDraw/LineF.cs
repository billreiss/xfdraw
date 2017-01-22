using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFDraw
{
    public struct LineF
    {
        public PointF P1;
        public PointF P2;
        public LineF(PointF p1, PointF p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public LineF(float x1, float y1, float x2, float y2)
        {
            P1 = new PointF(x1, y1);
            P2 = new PointF(x2, y2);
        }
    }
}
