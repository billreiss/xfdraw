using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFDraw.Numerics;

namespace XFDraw.Numerics
{
    public struct LineF
    {
        public Vector2 P1;
        public Vector2 P2;
        public LineF(Vector2 p1, Vector2 p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public LineF(float x1, float y1, float x2, float y2)
        {
            P1 = new Vector2(x1, y1);
            P2 = new Vector2(x2, y2);
        }
    }
}
