using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class IbbPortrait
    {
        //this class is handled differently than Android version
        public Bitmap ImgBG = null;
        public Bitmap Img = null;
        public Bitmap ImgLU = null;
        public Bitmap Glow = null;
        public bool glowOn = false;
        public string TextHP = "";
        public string TextSP = "";
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;
        public GameView gv;

        public IbbPortrait(GameView g, float sc)
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

            IbRect src = new IbRect(0, 0, this.ImgBG.PixelSize.Width, this.ImgBG.PixelSize.Height);
            IbRect src2 = new IbRect(0, 0, 0, 0);
            IbRect src3 = new IbRect(0, 0, 0, 0);

            if (this.Img != null)
            {
                src2 = new IbRect(0, 0, this.Img.PixelSize.Width, this.Img.PixelSize.Height);
            }
            if (this.ImgLU != null)
            {
                src3 = new IbRect(0, 0, this.ImgLU.PixelSize.Width, this.ImgLU.PixelSize.Height);
            }
            IbRect dstBG = new IbRect(this.X - (int)(3 * gv.screenDensity),
                                        this.Y - (int)(3 * gv.screenDensity),
                                        (int)((float)this.Width * gv.screenDensity) + (int)(6 * gv.screenDensity),
                                        (int)((float)this.Height * gv.screenDensity) + (int)(6 * gv.screenDensity));
            IbRect dst = new IbRect(this.X, this.Y, (int)((float)this.Width * gv.screenDensity), (int)((float)this.Height * gv.screenDensity));
            IbRect dstLU = new IbRect(this.X, this.Y, (int)((float)this.Width * gv.screenDensity), (int)((float)this.Height * gv.screenDensity));

            IbRect srcGlow = new IbRect(0, 0, this.Glow.PixelSize.Width, this.Glow.PixelSize.Height);
            IbRect dstGlow = new IbRect(this.X - (int)(7 * gv.screenDensity), 
                                        this.Y - (int)(7 * gv.screenDensity), 
                                        (int)((float)this.Width * gv.screenDensity) + (int)(15 * gv.screenDensity), 
                                        (int)((float)this.Height * gv.screenDensity) + (int)(15 * gv.screenDensity));

            gv.DrawBitmap(this.ImgBG, src, dstBG);

            if ((this.glowOn) && (this.Glow != null))
            {
                gv.DrawBitmap(this.Glow, srcGlow, dstGlow);
            }
            
            if (this.Img != null)
            {
                gv.DrawBitmap(this.Img, src2, dst);
            }            
            
            if (this.ImgLU != null)
            {
                gv.DrawBitmap(this.ImgLU, src3, dst);
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
            
            //DRAW HP/HPmax
            // Measure string.
            SizeF stringSize = gv.cc.MeasureString(TextHP, thisFont, this.Width);

            int ulX = ((int)(this.Width * gv.screenDensity) / 2) - ((int)stringSize.Width / 2);
            int ulY = ((int)(this.Height * gv.screenDensity / 2) / 2) + ((int)stringSize.Height / 2);
            ulX = pW * 0;
            ulY = this.Height - ((int)stringSize.Height * 2);

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(TextHP, this.X + ulX + x, this.Y + ulY - pH + y , scaler, Color.Black);
                }
            }
            gv.DrawText(TextHP, this.X + ulX, this.Y + ulY - pH, scaler, Color.Lime);
            
            //DRAW SP/SPmax
            // Measure string.
            stringSize = gv.cc.MeasureString(TextSP, thisFont, this.Width);

            ulX = ((int)(this.Width * gv.screenDensity / 2)) - ((int)stringSize.Width);
            ulY = ((int)(this.Height * gv.screenDensity / 2));
            ulX = pW * 1;
            ulY = this.Height - ((int)stringSize.Height * 1);

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(TextSP, this.X + ulX - pW + x, this.Y + ulY - pH + y, scaler, Color.Black);
                }
            }
            gv.DrawText(TextSP, this.X + ulX - pW, this.Y + ulY - pH, scaler, Color.Yellow);
        }
    }
}
