using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class IbbPortrait
    {
        //this class is handled differently than Android version
        public Bitmap ImgBG = null;
        public Bitmap Img = null;
        public Bitmap ImgLU = null; //used for level up icon
        public Bitmap ImgChat = null; //used for new chat option availabe indicator icon
        public Bitmap Glow = null;
        public bool glowOn = false;
        public bool levelUpOn = false;
        public bool newChatOptionOn = false;
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
            IbRect src4 = new IbRect(0, 0, 0, 0);
            IbRect dstLU = new IbRect(0, 0, 0, 0);
            IbRect dstChat = new IbRect(0, 0, 0, 0);

            if (this.Img != null)
            {
                src2 = new IbRect(0, 0, this.Img.PixelSize.Width, this.Img.PixelSize.Height);
            }
            if (this.ImgLU != null)
            {
                src3 = new IbRect(0, 0, this.ImgLU.PixelSize.Width, this.ImgLU.PixelSize.Height);
            }
            if (this.ImgChat != null)
            {
                src4 = new IbRect(0, 0, this.ImgChat.PixelSize.Width, this.ImgChat.PixelSize.Height);
            }
            IbRect dstBG = new IbRect(this.X - (int)(3 * gv.screenDensity),
                                        this.Y - (int)(3 * gv.screenDensity),
                                        (int)((float)this.Width) + (int)(6 * gv.screenDensity),
                                        (int)((float)this.Height) + (int)(6 * gv.screenDensity));
            IbRect dst = new IbRect(this.X, this.Y, (int)((float)this.Width), (int)((float)this.Height));
            if (this.ImgLU != null)
            {
                dstLU = new IbRect(this.X, this.Y, this.ImgLU.PixelSize.Width, this.ImgLU.PixelSize.Height);
            }
            if (this.ImgChat != null)
            {
                dstChat = new IbRect(this.X, this.Y, this.ImgChat.PixelSize.Width, this.ImgChat.PixelSize.Height);
            }
            IbRect srcGlow = new IbRect(0, 0, this.Glow.PixelSize.Width, this.Glow.PixelSize.Height);
            IbRect dstGlow = new IbRect(this.X - (int)(7 * gv.screenDensity), 
                                        this.Y - (int)(7 * gv.screenDensity), 
                                        (int)((float)this.Width) + (int)(15 * gv.screenDensity), 
                                        (int)((float)this.Height) + (int)(15 * gv.screenDensity));

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
                if (levelUpOn)
                {
                    gv.DrawBitmap(this.ImgLU, src3, dstLU);
                }                
            }

            if (this.ImgChat != null)
            {
                 if (newChatOptionOn)
                {
                    //kvbkoeln 
                    gv.DrawBitmap(this.ImgChat, src4, dstChat);
                }
            }

            if (gv.mod.useUIBackground)
            {
                IbRect srcFrame = new IbRect(0, 0, gv.cc.ui_portrait_frame.PixelSize.Width, gv.cc.ui_portrait_frame.PixelSize.Height);
                IbRect dstFrame = new IbRect(this.X - (int)(5 * gv.screenDensity),
                                        this.Y - (int)(5 * gv.screenDensity),
                                        (int)((float)this.Width) + (int)(10 * gv.screenDensity),
                                        (int)((float)this.Height) + (int)(10 * gv.screenDensity));
                gv.DrawBitmap(gv.cc.ui_portrait_frame, srcFrame, dstFrame);
            }

            float thisFontHeight = gv.drawFontRegHeight;
            if (scaler > 1.05f)
            {
                thisFontHeight = gv.drawFontLargeHeight;
            }
            else if (scaler < 0.95f)
            {
                thisFontHeight = gv.drawFontSmallHeight;
            }
            
            //DRAW HP/HPmax
            // Measure string.
            //SizeF stringSize = gv.cc.MeasureString(TextHP, thisFont, this.Width);
            //float stringSize = gv.cc.MeasureString(TextHP, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

            //int ulX = ((int)(this.Width) / 2) - ((int)stringSize / 2);
            //int ulY = ((int)(this.Height / 2) / 2) + ((int)thisFontHeight / 2);
            int ulX = pW * 0;
            int ulY = this.Height - ((int)thisFontHeight * 2);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    gv.DrawText(TextHP, this.X + ulX + x, this.Y + ulY - pH + y , scaler, Color.Black);
                }
            }
            gv.DrawText(TextHP, this.X + ulX, this.Y + ulY - pH, scaler, Color.Lime);
            
            //DRAW SP/SPmax
            // Measure string.
            //stringSize = gv.cc.MeasureString(TextSP, thisFont, this.Width);
            //stringSize = gv.cc.MeasureString(TextSP, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

            //ulX = ((int)(this.Width / 2)) - ((int)stringSize);
            //ulY = ((int)(this.Height / 2));
            ulX = pW * 1;
            ulY = this.Height - ((int)thisFontHeight * 1);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    gv.DrawText(TextSP, this.X + ulX - pW + x, this.Y + ulY - pH + y, scaler, Color.Black);
                }
            }
            gv.DrawText(TextSP, this.X + ulX - pW, this.Y + ulY - pH, scaler, Color.Yellow);
        }
    }
}
