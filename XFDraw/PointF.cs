using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFDraw
{
    public struct PointF : IEquatable<PointF>
    {
        public float X;
        public float Y;

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return this.Equals((PointF)obj);
        }

        public bool Equals(PointF other)
        {
            return other.X == this.X && other.Y == this.Y;
        }

        public static bool operator == (PointF p1, PointF p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(PointF p1, PointF p2)
        {
            return !p1.Equals(p2);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }
    }
}
