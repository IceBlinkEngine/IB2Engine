using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IbRectF
    {
        public float Left = 0;
        public float Top = 0;
        public float Width = 0;
        public float Height = 0;
        
        public IbRectF()
        {
        }
        public IbRectF(float left, float top, float width, float height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }
}
