using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class IbbButton
    {
        //this class is handled differently than Android version
        public Bitmap Img = null;
        public Bitmap ImgOff = null;
        public Bitmap Img2 = null;
        public Bitmap Img3 = null;
        public Bitmap Glow = null;
        public bool buttonOn = true;
        public bool glowOn = false;
        public string Text = "";
        public string Quantity = "";
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;
        public GameView gv;

        public IbbButton(GameView g, float sc)
        {
            gv = g;
            scaler = sc;
        }

        public bool getImpact(int x, int y)
        {
            if ((x >= X) && (x <= (X + this.Width)))
            {
                if ((y >= Y + gv.oYshift) && (y <= (Y + gv.oYshift + this.Height)))
                {
                    if (!playedHoverSound)
                    {
                        playedHoverSound = true;
                        gv.playerButtonEnter.Play();
                    }
                    return true;
                }
            }
            playedHoverSound = false;
            return false;
        }

        public void Draw()
        {
            int pH = (int)((float)gv.screenHeight / 200.0f);
            int pW = (int)((float)gv.screenHeight / 200.0f);
            float fSize = (float)(gv.squareSize / 4) * scaler;

            IbRect src = new IbRect(0, 0, this.Img.Width, this.Img.Height);
            IbRect srcOff = new IbRect(0, 0, this.Img.Width, this.Img.Height);
            IbRect src2 = new IbRect(0, 0, this.Img.Width, this.Img.Height);
            IbRect src3 = new IbRect(0, 0, this.Img.Width, this.Img.Height);

            if (this.ImgOff != null)
            {
                srcOff = new IbRect(0, 0, this.ImgOff.Width, this.ImgOff.Width);
            }
            if (this.Img2 != null)
            {
                src2 = new IbRect(0, 0, this.Img2.Width, this.Img2.Width);
            }
            if (this.Img3 != null)
            {
                src3 = new IbRect(0, 0, this.Img3.Width, this.Img3.Width);
            }
            IbRect dst = new IbRect(this.X, this.Y, (int)((float)this.Width * gv.screenDensity), (int)((float)this.Height * gv.screenDensity));

            IbRect srcGlow = new IbRect(0, 0, this.Glow.Width, this.Glow.Height);
            IbRect dstGlow = new IbRect(this.X - (int)(7 * gv.screenDensity), 
                                        this.Y - (int)(7 * gv.screenDensity), 
                                        (int)((float)this.Width * gv.screenDensity) + (int)(15 * gv.screenDensity), 
                                        (int)((float)this.Height * gv.screenDensity) + (int)(15 * gv.screenDensity));
            
            if ((this.glowOn) && (this.Glow != null))
            {
                gv.DrawBitmap(this.Glow, srcGlow, dstGlow);
            }
            //draw button as OFF if set to off
            if ((!this.buttonOn) && (this.ImgOff != null))
            {
                gv.DrawBitmap(this.ImgOff, srcOff, dst);
            }
            else //draw button as ON (normal)
            {
                gv.DrawBitmap(this.Img, src, dst);
            }
            if (this.Img2 != null)
            {
                gv.DrawBitmap(this.Img2, src2, dst);
            }
            if (this.Img3 != null)
            {
                gv.DrawBitmap(this.Img3, src3, dst);
            }

            Font thisFont = gv.drawFontReg;
            if (scaler > 1.05f)
            {
                thisFont = gv.drawFontLarge;
            }
            else if (scaler < 0.95f)
            {
                thisFont = gv.drawFontSmall;
            }
            
            // Measure string.
            SizeF stringSize = gv.cc.MeasureString(Text, thisFont, this.Width);

            int ulX = ((int)(this.Width * gv.screenDensity) / 2) - ((int)stringSize.Width / 2);
            int ulY = ((int)(this.Height * gv.screenDensity / 2) / 2) + ((int)stringSize.Height / 2);

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(Text, this.X + ulX + x, this.Y + ulY - pH + y , scaler, Color.Black);
                }
            }
            gv.DrawText(Text, this.X + ulX, this.Y + ulY - pH, scaler, Color.White);
            
            // Measure string.
            stringSize = gv.cc.MeasureString(Quantity, thisFont, this.Width);

            ulX = ((int)(this.Width * gv.screenDensity / 2)) - ((int)stringSize.Width);
            ulY = ((int)(this.Height * gv.screenDensity / 2));

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(Quantity, this.X + ulX - pW + x, this.Y + ulY - pH + y, scaler, Color.Black);
                }
            }
            gv.DrawText(Quantity, this.X + ulX - pW, this.Y + ulY - pH, scaler, Color.White);
        }
    }
}
