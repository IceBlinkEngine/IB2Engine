using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class IB2Portrait
    {
        [JsonIgnore]
        public GameView gv;
        public string name = "";
        public string ImgBGFilename = "";
        public string ImgFilename = "";
        public string ImgLUFilename = ""; //used for level up icon
        public string GlowFilename = "";
        public bool glowOn = false;
        public bool levelUpOn = false;
        public string TextHP = "";
        public string TextSP = "";
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;

        public IB2Portrait()
        {
            
        }

        public IB2Portrait(GameView g)
        {
            gv = g;
        }

        public void  setupIB2Portrait(GameView g)
        {
            gv = g;
        }

        public void setHover(IB2Panel parentPanel, int x, int y)
        {
            //int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
            //int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;
            glowOn = false;

            if ((x >= parentPanel.currentLocX + X) && (x <= (parentPanel.currentLocX + X + Width)))
            {
                if ((y >= parentPanel.currentLocY + Y + gv.oYshift) && (y <= (parentPanel.currentLocY + Y + gv.oYshift + Height)))
                {
                    if (!playedHoverSound)
                    {
                        playedHoverSound = true;
                        gv.playerButtonEnter.Play();
                    }
                    glowOn = true;
                }
            }
            playedHoverSound = false;
        }

        public bool getImpact(IB2Panel parentPanel, int x, int y)
        {
            //int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
            //int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;

            if ((x >= parentPanel.currentLocX + X) && (x <= (parentPanel.currentLocX + X + Width)))
            {
                if ((y >= parentPanel.currentLocY + Y + gv.oYshift) && (y <= (parentPanel.currentLocY + Y + gv.oYshift + Height)))
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

        public void Draw(IB2Panel parentPanel)
        {
            int pH = (int)((float)gv.screenHeight / 200.0f);
            int pW = (int)((float)gv.screenHeight / 200.0f);
            float fSize = (float)(gv.squareSize / 4) * scaler;
            //int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
            //int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;

            IbRect src = new IbRect(0, 0, Width, Height);
            IbRect dst = new IbRect(parentPanel.currentLocX + this.X, parentPanel.currentLocY + this.Y, (int)((float)Width), (int)((float)Height));
            IbRect dstBG = new IbRect(parentPanel.currentLocX + this.X - (int)(3 * gv.screenDensity),
                                        parentPanel.currentLocY + this.Y - (int)(3 * gv.screenDensity),
                                        (int)((float)Width) + (int)(6 * gv.screenDensity),
                                        (int)((float)Height) + (int)(6 * gv.screenDensity));
            
            IbRect srcGlow = new IbRect(0, 0, gv.cc.GetFromBitmapList(GlowFilename).PixelSize.Width, gv.cc.GetFromBitmapList(GlowFilename).PixelSize.Height);
            IbRect dstGlow = new IbRect(parentPanel.currentLocX + this.X - (int)(7 * gv.screenDensity),
                                        parentPanel.currentLocY + this.Y - (int)(7 * gv.screenDensity),
                                        (int)((float)Width) + (int)(15 * gv.screenDensity),
                                        (int)((float)Height) + (int)(15 * gv.screenDensity));

            gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgBGFilename), src, dstBG);

            if (glowOn)
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(GlowFilename), srcGlow, dstGlow);
            }

            if (!ImgFilename.Equals(""))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgFilename), src, dst);
            }

            if (!ImgLUFilename.Equals(""))
            {
                if (levelUpOn)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgLUFilename), src, dst);
                }
            }

            if (gv.mod.useUIBackground)
            {
                IbRect srcFrame = new IbRect(0, 0, gv.cc.ui_portrait_frame.PixelSize.Width, gv.cc.ui_portrait_frame.PixelSize.Height);
                IbRect dstFrame = new IbRect(parentPanel.currentLocX + this.X - (int)(5 * gv.screenDensity),
                                        parentPanel.currentLocY + this.Y - (int)(5 * gv.screenDensity),
                                        (int)((float)Width) + (int)(10 * gv.screenDensity),
                                        (int)((float)Height) + (int)(10 * gv.screenDensity));
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
            int ulY = Height - ((int)thisFontHeight * 2);

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(TextHP, parentPanel.currentLocX + this.X + ulX + x, parentPanel.currentLocY + this.Y + ulY - pH + y, scaler, Color.Black);
                }
            }
            gv.DrawText(TextHP, parentPanel.currentLocX + this.X + ulX, parentPanel.currentLocY + this.Y + ulY - pH, scaler, Color.Lime);

            //DRAW SP/SPmax
            // Measure string.
            //stringSize = gv.cc.MeasureString(TextSP, thisFont, this.Width);
            //stringSize = gv.cc.MeasureString(TextSP, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

            //ulX = ((int)(this.Width / 2)) - ((int)stringSize);
            //ulY = ((int)(this.Height / 2));
            ulX = pW * 1;
            ulY = Height - ((int)thisFontHeight * 1);

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(TextSP, parentPanel.currentLocX + this.X + ulX - pW + x, parentPanel.currentLocY + this.Y + ulY - pH + y, scaler, Color.Black);
                }
            }
            gv.DrawText(TextSP, parentPanel.currentLocX + this.X + ulX - pW, parentPanel.currentLocY + this.Y + ulY - pH, scaler, Color.Yellow);
        }

        public void Update(int elapsed)
        {
            //animate button?
        }
    }
}
