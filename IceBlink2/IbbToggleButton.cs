using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IbbToggleButton
    {
        //this class is handled differently than Android version
        public Bitmap ImgOn = null;
        public Bitmap ImgOff = null;
        public bool toggleOn = false;
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        //private Context gameContext;
        public GameView gv;

        public IbbToggleButton(GameView g)
        {
            gv = g;
            //gameContext = gmContext;
        }

        public bool getImpact(int x, int y)
        {
            if ((x >= X) && (x <= (X + this.Width)))
            {
                if ((y >= Y + gv.oYshift) && (y <= (Y + gv.oYshift + this.Height)))
                //if ((y >= Y) && (y <= (Y + this.Height)))
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw()
        {
            IbRect src = new IbRect(0, 0, this.ImgOn.Width, this.ImgOn.Height);
            IbRect dst = new IbRect(this.X, this.Y, this.ImgOn.Width, this.ImgOn.Height);

            if (this.toggleOn)
            {
                if (this.ImgOn != null)
                {
                    gv.DrawBitmap(this.ImgOn, src, dst);
                    //canvas.drawBitmap(this.ImgOn, src, dst, null);
                }
            }
            else
            {
                if (this.ImgOff != null)
                {
                    gv.DrawBitmap(this.ImgOff, src, dst);
                    //canvas.drawBitmap(this.ImgOff, src, dst, null);
                }
            }
        }
    }
}
