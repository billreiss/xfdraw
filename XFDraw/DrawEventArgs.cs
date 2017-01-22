using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFDraw
{
    public class DrawEventArgs
    {
        DrawingContext context;

        internal DrawEventArgs(DrawingContext context)
        {
            this.context = context;
        }

        public DrawingContext Context
        {
            get
            {
                return context;
            }
        }
    }
}
