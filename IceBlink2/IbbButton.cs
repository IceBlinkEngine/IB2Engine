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
        public Bitmap Img2 = null;
        public Bitmap Img3 = null;
        public Bitmap Glow = null;
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
            IbRect src2 = new IbRect(0, 0, this.Img.Width, this.Img.Height);
            IbRect src3 = new IbRect(0, 0, this.Img.Width, this.Img.Height);

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
            IbRect dstGlow = new IbRect(this.X - (int)(3 * gv.screenDensity), this.Y - (int)(3 * gv.screenDensity), (int)((float)this.Width * gv.screenDensity) + (int)(7 * gv.screenDensity), (int)((float)this.Height * gv.screenDensity) + (int)(7 * gv.screenDensity));
            
            if ((this.glowOn) && (this.Glow != null))
            {
                gv.DrawBitmap(this.Glow, srcGlow, dstGlow);
                //canvas.drawBitmap(this.Glow, srcGlow, dstGlow, null);
            }
            gv.DrawBitmap(this.Img, src, dst);
            //canvas.drawBitmap(this.Img, src, dst, null);
            if (this.Img2 != null)
            {
                gv.DrawBitmap(this.Img2, src2, dst);
                //canvas.drawBitmap(this.Img2, src2, dst, null);
            }
            if (this.Img3 != null)
            {
                gv.DrawBitmap(this.Img3, src3, dst);
                //canvas.drawBitmap(this.Img3, src3, dst, null);
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

            //IbRect bounds = new IbRect();
            //gv.drawFontReg.getTextBounds(Text, 0, Text.Length, bounds);
            int ulX = ((int)(this.Width * gv.screenDensity) / 2) - ((int)stringSize.Width / 2);
            int ulY = ((int)(this.Height * gv.screenDensity/2) / 2) + ((int)stringSize.Height / 2);
            gv.DrawText(Text, this.X + ulX, this.Y + ulY - pH, scaler, Color.White);
            //canvas.drawText(Text, this.X + ulX, this.Y + ulY - pH, mUiTextPaint);

            // Measure string.
            stringSize = gv.cc.MeasureString(Quantity, thisFont, this.Width);

            //bounds = new IbRect();
            //mUiTextPaint.getTextBounds(Quantity, 0, Quantity.Length, bounds);
            ulX = ((int)(this.Width * gv.screenDensity / 2)) - ((int)stringSize.Width);
            ulY = ((int)(this.Height * gv.screenDensity / 2));
            gv.DrawText(Quantity, this.X + ulX - pW, this.Y + ulY - pH, scaler, Color.White); 
            //canvas.drawText(Quantity, this.X + ulX - pW, this.Y + ulY - pH, mUiTextPaint);
        }
    }
}
