using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IbRect
    {
        public int Left = 0;
        public int Top = 0;
        public int Width = 0;
        public int Height = 0;
        
        public IbRect()
        {
        }
        public IbRect(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }
}
