using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IbbPanel
    {
        //this class is handled differently than Android version
        public Bitmap ImgBG = null;
        public int LocX = 0;
        public int LocY = 0;
        public int Width = 0;
        public int Height = 0;
        public GameView gv;

        public IbbPanel(GameView g)
        {
            gv = g;
        }

        public void Draw()
        {
            IbRect src = new IbRect(0, 0, this.ImgBG.PixelSize.Width, this.ImgBG.PixelSize.Height);
            IbRect dst = new IbRect(this.LocX, this.LocY, Width, Height);            
            gv.DrawBitmap(this.ImgBG, src, dst);
        }
    }
}
