﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFDraw
{
    public struct RectangleF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public RectangleF(PointF topLeft, PointF bottomRight)
        {
            X = topLeft.X;
            Y = topLeft.Y;
            Width = bottomRight.X - topLeft.X;
            Height = bottomRight.Y - topLeft.Y;
        }

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        public float Right
        {
            get
            {
                return X + Width;
            }
        }

        public float Top
        {
            get
            {
                return Y;
            }
        }

        public float Left
        {
            get
            {
                return X;
            }
        }
    }
}
