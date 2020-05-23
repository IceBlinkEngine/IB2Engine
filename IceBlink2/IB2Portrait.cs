using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;
using Bitmap = SharpDX.Direct2D1.Bitmap;


namespace IceBlink2
{
    public class IB2Portrait
    {
        [JsonIgnore]
        public GameView gv;
        public int playerNumber = 0;
        public string tag = "";
        public string ImgBGFilename = "";
        public string ImgFilename = "";
        public string ImgLUFilename = ""; //used for level up icon
        public string GlowFilename = "";
        public bool glowOn = false;
        public bool levelUpOn = false;

        public string TextHP = "";
        public string TextSP = "";
        public string levelUpSymbol = "";
        public string chatSymbol = "";
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;
        public bool show = false;

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

            float xMod = (gv.screenWidth / 1920f);
            float yMod = (gv.screenHeight / 1080f);

            int spacing = 0;
            if (gv.screenType == "main")
            {
                if (this.tag == "port0")
                {
                    spacing = (int)(7f * yMod);
                }
                if (this.tag == "port1")
                {
                    spacing = (int)(14f * yMod);
                }
                if (this.tag == "port2")
                {
                    spacing = (int)(21f * yMod);
                }
                if (this.tag == "port3")
                {
                    spacing = (int)(28f * yMod);
                }
                if (this.tag == "port4")
                {
                    spacing = (int)(35f * yMod);
                }
                if (this.tag == "port5")
                {
                    spacing = (int)(42f * yMod);
                }
            }
            else if (gv.screenType == "combat")
            {
                if (this.tag == "port0")
                {
                    spacing = (int)(-28f * yMod);
                }
                if (this.tag == "port1")
                {
                    spacing = (int)(-21f * yMod);
                }
                if (this.tag == "port2")
                {
                    spacing = (int)(-14f * yMod);
                }
                if (this.tag == "port3")
                {
                    spacing = (int)(-7f * yMod);
                }
                if (this.tag == "port4")
                {
                    spacing = (int)(0f * yMod);
                }
                if (this.tag == "port5")
                {
                    spacing = (int)(7f * yMod);
                }
            }

            if (gv.screenType == "main")
            {
                if (parentPanel.Height <= (6 * this.Height + 42f * yMod))
                {
                    spacing = 0;
                }
            }

            if (gv.screenType == "combat")
            {
                if (parentPanel.Height <= (6 * this.Height + 7f * yMod))
                {
                    spacing = 0;
                }
            }
            if (show)
            {
                glowOn = false;

                if ((x >= (int)((parentPanel.currentLocX + X) * xMod)) && (x <= (int)((parentPanel.currentLocX + X + Width) * xMod)))
                {
                    if ((y >= (int)((parentPanel.currentLocY + Y + gv.oYshift) * yMod) + spacing) && (y <= (int)((parentPanel.currentLocY + Y + gv.oYshift + Height) * yMod) + spacing))
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
        }

        public bool getImpact(IB2Panel parentPanel, int x, int y)
        {
            //int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
            //int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;
            float xMod = (gv.screenWidth / 1920f);
            float yMod = (gv.screenHeight / 1080f);


            int spacing = 0;
            if (gv.screenType == "main")
            {
                if (this.tag == "port0")
                {
                    spacing = (int)(7f * yMod);
                }
                if (this.tag == "port1")
                {
                    spacing = (int)(14f * yMod);
                }
                if (this.tag == "port2")
                {
                    spacing = (int)(21f * yMod);
                }
                if (this.tag == "port3")
                {
                    spacing = (int)(28f * yMod);
                }
                if (this.tag == "port4")
                {
                    spacing = (int)(35f * yMod);
                }
                if (this.tag == "port5")
                {
                    spacing = (int)(42f * yMod);
                }
            }
            else if (gv.screenType == "combat")
            {
                if (this.tag == "port0")
                {
                    spacing = (int)(-28f * yMod);
                }
                if (this.tag == "port1")
                {
                    spacing = (int)(-21f * yMod);
                }
                if (this.tag == "port2")
                {
                    spacing = (int)(-14f * yMod);
                }
                if (this.tag == "port3")
                {
                    spacing = (int)(-7f * yMod);
                }
                if (this.tag == "port4")
                {
                    spacing = (int)(0f * yMod);
                }
                if (this.tag == "port5")
                {
                    spacing = (int)(7f * yMod);
                }
            }


            if (gv.screenType == "main")
            {
                if (parentPanel.Height <= (6 * this.Height + 42f * yMod))
                {
                    spacing = 0;
                }
            }

            if (gv.screenType == "combat")
            {
                if (parentPanel.Height <= (6 * this.Height + 7f * yMod))
                {
                    spacing = 0;
                }
            }

            if (show)
            {
                if ((x >= (int)((parentPanel.currentLocX + X) * xMod)) && (x <= (int)((parentPanel.currentLocX + X + Width) * xMod)))
                {
                    if ((y >= (int)((parentPanel.currentLocY + Y + gv.oYshift) * yMod) + spacing) && (y <= (int)((parentPanel.currentLocY + Y + gv.oYshift + Height) * yMod) + spacing))
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
            }
            return false;
        }

        public void Draw(IB2Panel parentPanel)
        {
            if (show)
            {
                float thisFontHeight = gv.drawFontRegHeight;
                if (scaler > 1.05f)
                {
                    thisFontHeight = gv.drawFontLargeHeight;
                }
                else if (scaler < 0.95f)
                {
                    thisFontHeight = gv.drawFontSmallHeight;
                }

                int numberOfLinesOnPortrait = (int)((Height * gv.screenDensity) / thisFontHeight);
                int pixPerLine = (int)((Height * gv.screenDensity) / numberOfLinesOnPortrait);

                float xMod = (gv.screenWidth / 1920f);
                float yMod = (gv.screenHeight / 1080f);

                int spacing = 0;
                if (gv.screenType == "main")
                {
                    if (this.tag == "port0")
                    {
                        spacing = (int)(7f * yMod);
                    }
                    if (this.tag == "port1")
                    {
                        spacing = (int)(14f * yMod);
                    }
                    if (this.tag == "port2")
                    {
                        spacing = (int)(21f * yMod);
                    }
                    if (this.tag == "port3")
                    {
                        spacing = (int)(28f * yMod);
                    }
                    if (this.tag == "port4")
                    {
                        spacing = (int)(35f * yMod);
                    }
                    if (this.tag == "port5")
                    {
                        spacing = (int)(42f * yMod);
                    }
                }
                else if (gv.screenType == "combat")
                {
                    if (this.tag == "port0")
                    {
                        spacing = (int)(-28f * yMod);
                    }
                    if (this.tag == "port1")
                    {
                        spacing = (int)(-21f * yMod);
                    }
                    if (this.tag == "port2")
                    {
                        spacing = (int)(-14f * yMod);
                    }
                    if (this.tag == "port3")
                    {
                        spacing = (int)(-7f * yMod);
                    }
                    if (this.tag == "port4")
                    {
                        spacing = (int)(0f * yMod);
                    }
                    if (this.tag == "port5")
                    {
                        spacing = (int)(7f * yMod);
                    }
                }

                if (gv.screenType == "main")
                {
                    if (parentPanel.Height <= (6 * this.Height + 42f * yMod))
                    {
                        spacing = 0;
                    }
                }

                if (gv.screenType == "combat")
                {
                    if (parentPanel.Height <= (6 * this.Height + 7f * yMod))
                    {
                        spacing = 0;
                    }
                }


                //this.tag
                int pH = (int)((float)gv.screenHeight / 200.0f);
                int pW = (int)((float)gv.screenHeight / 200.0f);
                float fSize = (float)(gv.squareSize / 4) * scaler;
                //int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
                //int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;

                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height);
                IbRect srcBG = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgBGFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ImgBGFilename).PixelSize.Height);
                //IbRect dst = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity), (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity), (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));
                IbRect dst = new IbRect((int)((parentPanel.currentLocX + this.X) * xMod), (int)((parentPanel.currentLocY + this.Y) * yMod + spacing), (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));
                
                IbRect dstBG = new IbRect((int)((parentPanel.currentLocX + this.X) * xMod) - (int)(3 * xMod),
                                            (int)((parentPanel.currentLocY + this.Y) * yMod) - (int)(3 * yMod) + spacing,
                                            (int)((float)Width * gv.screenDensity) + (int)(6 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(6 * gv.screenDensity));
                                            


                IbRect srcGlow = new IbRect(0, 0, gv.cc.GetFromBitmapList(GlowFilename).PixelSize.Width, gv.cc.GetFromBitmapList(GlowFilename).PixelSize.Height);
                IbRect dstGlow = new IbRect((int)((parentPanel.currentLocX + this.X) * xMod) - (int)(7 * xMod),
                                            (int)((parentPanel.currentLocY + this.Y) * yMod) - (int)(7 * yMod) + spacing,
                                            (int)((float)Width * gv.screenDensity) + (int)(15 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(15 * gv.screenDensity));

                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgBGFilename), src, dstBG, -0.01f, false, 1.0f, true);

                if (glowOn)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(GlowFilename), srcGlow, dstGlow, -0.01f, false, 1.0f, true);
                }

                if (!ImgFilename.Equals(""))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgFilename), src, dst, -0.01f, false, 1.0f, true);
                }

                if (!ImgLUFilename.Equals(""))
                {
                    if (levelUpOn)
                    {
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgLUFilename), src, dst, -0.01f, false, 1.0f, true);
                    }
                }

                if (gv.mod.useUIBackground)
                {
                    IbRect srcFrame = new IbRect(0, 0, gv.cc.ui_portrait_frame.PixelSize.Width, gv.cc.ui_portrait_frame.PixelSize.Height);
                    IbRect dstFrame = new IbRect((int)((parentPanel.currentLocX + this.X) * xMod) - (int)(5 * xMod),
                                            (int)((parentPanel.currentLocY + this.Y) * yMod) - (int)(5 * yMod) + spacing,
                                            (int)((float)Width * gv.screenDensity) + (int)(10 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(10 * gv.screenDensity));
                    gv.DrawBitmap(gv.cc.ui_portrait_frame, srcFrame, dstFrame, -0.01f, false, 1.0f, true);
                }

              

                //DRAW HP/HPmax
                // Measure string.
                //SizeF stringSize = gv.cc.MeasureString(TextHP, thisFont, this.Width);
                //float stringSize = gv.cc.MeasureString(TextHP, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

                //int ulX = ((int)(this.Width) / 2) - ((int)stringSize / 2);
                //int ulY = ((int)(this.Height / 2) / 2) + ((int)thisFontHeight / 2);
                int ulX = pW * 0;
                int ulY = (int)(Height * yMod) - ((int)thisFontHeight * 2);
                /*
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                            int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH + y);
                            gv.DrawText(TextHP, xLoc, yLoc, scaler, Color.Black);
                        }
                    }
                }
                */
                //(int)((parentPanel.currentLocY + this.Y) * yMod + spacing)
                int xLoc1 = (int)((parentPanel.currentLocX + this.X ) * xMod + ulX);
                int yLoc1 = (int)((parentPanel.currentLocY + this.Y) * yMod + spacing + (Height * gv.screenDensity) - pixPerLine *2);
                gv.DrawTextOutlined(TextHP, xLoc1, yLoc1, scaler, Color.Lime);

                //DRAW SP/SPmax
                // Measure string.
                //stringSize = gv.cc.MeasureString(TextSP, thisFont, this.Width);
                //stringSize = gv.cc.MeasureString(TextSP, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

                //ulX = ((int)(this.Width / 2)) - ((int)stringSize);
                //ulY = ((int)(this.Height / 2));
                ulX = pW * 1;
                ulY = (int)(Height * yMod) - ((int)thisFontHeight * 1);

                /*
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX - pW + x);
                            int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH + y);
                            gv.DrawText(TextSP, xLoc, yLoc, scaler, Color.Black);
                        }
                    }
                }
                */
                int xLoc2 = (int)((parentPanel.currentLocX + this.X ) * xMod + ulX - pW);
                int yLoc2 = (int)((parentPanel.currentLocY + this.Y) * yMod + spacing + (Height * gv.screenDensity) - pixPerLine);             
                gv.DrawTextOutlined(TextSP, xLoc2, yLoc2, scaler, Color.Yellow);
                //gv.DrawTextOutlined(TextSP, xLoc2, yLoc2, gv.FontWeight.Normal, scaler, Color.Yellow);
                //DrawText(text, xLoc, yLoc, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, scaler, fontColor, false);
                //better reoute all to drawtext and from the to draw outlined

                //draw level up symbol
                //ulX = (int)(110 * gv.screenDensity) - pW * 9;
                //ulX = (int)(110 * xMod) - pW * 24;
                ulX = (int)(110 * 0.05f);
                ulY = (int)(Height * yMod) - ((int)thisFontHeight * 7) + pH;

                /*
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX - pW + x);
                            int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH + y);
                            gv.DrawText(levelUpSymbol, xLoc, yLoc, scaler * 1.2f, Color.Black);
                        }
                    }
                }
                */
                int xLoc3 = (int)((parentPanel.currentLocX + this.X ) * xMod + ulX);
                int yLoc3 = (int)((parentPanel.currentLocY + this.Y) * yMod + ulY - pH + spacing);
                gv.DrawTextOutlined(levelUpSymbol, xLoc3, yLoc3, scaler*1.2f, Color.CornflowerBlue);


                //draw chat symbol
                //int ulX5 = (int)(110 * xMod) - pW * 24;
                //int ulY5 = (int)(Height * gv.screenDensity) - ((int)thisFontHeight * 7) + 2*pH;
                int ulX5 = (int)(110 * 0.05f);
                int ulY5 = (int)(Height * yMod) - ((int)thisFontHeight * 6) + 3 * pH;
                /*
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX5 - pW + x);
                            int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY5 - pH + y);
                            gv.DrawText(chatSymbol, xLoc, yLoc, scaler * 0.5f, Color.Black);
                        }
                    }
                }
                */
                int xLoc4 = (int)((parentPanel.currentLocX + this.X ) * xMod + ulX5 - pW);
                int yLoc4 = (int)((parentPanel.currentLocY + this.Y) * yMod + ulY5 - pH + spacing);
                gv.DrawTextOutlined(chatSymbol, xLoc4, yLoc4, scaler * 0.25f, Color.Azure);

                //effects on portrait
                
                  int effectCounter = 0;
                            foreach (Effect ef in gv.mod.playerList[playerNumber].effectsList)
                            {
                                if ((!ef.isPermanent) && (ef.spriteFilename != "none") && (ef.spriteFilename != "") && (ef.spriteFilename != "None"))
                                {

                                    //float ggh = Width * gv.screenDensity;
                                    Bitmap fx = gv.cc.LoadBitmap(ef.spriteFilename);
                                    src = new IbRect(0, 0, fx.PixelSize.Width, fx.PixelSize.Width);
                                    IbRect dst2 = new IbRect(dst.Left + (int)((Width * gv.screenDensity) / 3f * 2f), dst.Top, (int)((Width * gv.screenDensity) / 3f), (int)((Width * gv.screenDensity) / 3f));
                                    effectCounter++;
                                    if (effectCounter == 2)
                                    {
                                        dst2 = new IbRect(dst.Left + (int)((Width * gv.screenDensity) / 3f * 2f), dst.Top + (int)((Width * gv.screenDensity) / 3f * 1f), (int)((Width * gv.screenDensity) / 3f), (int)((Width * gv.screenDensity) / 3f));
                                    }
                                    if (effectCounter == 3)
                                    {
                                        dst2 = new IbRect(dst.Left + (int)((Width * gv.screenDensity) / 3f * 2f), dst.Top + (int)((Width * gv.screenDensity) / 3f * 2f), (int)((Width * gv.screenDensity) / 3f), (int)((Width * gv.screenDensity) / 3f));
                                    }
                                    if (effectCounter == 4)
                                    {
                                        dst2 = new IbRect(dst.Left + (int)((Width * gv.screenDensity) / 3f * 1f), dst.Top, (int)((Width * gv.screenDensity) / 3f), (int)((Width * gv.screenDensity) / 3f));
                                    }
                                    if (effectCounter == 5)
                                    {
                                        dst2 = new IbRect(dst.Left + (int)((Width * gv.screenDensity) / 3f * 1f), dst.Top + (int)((Width * gv.screenDensity) / 3f * 1f), (int)((Width * gv.screenDensity) / 3f), (int)((Width * gv.screenDensity) / 3f));
                                    }
                                    if (effectCounter == 6)
                                    {
                                        dst2 = new IbRect(dst.Left + (int)((Width * gv.screenDensity) / 3f * 1f), dst.Top + (int)((Width * gv.screenDensity) / 3f * 2f), (int)((Width * gv.screenDensity) / 3f), (int)((Width * gv.screenDensity) / 3f));
                                    }
                                    if (effectCounter == 7)
                                    {
                                        dst2 = new IbRect(dst.Left, dst.Top, (int)((Width * gv.screenDensity) / 3f), (int)((Width * gv.screenDensity) / 3f));
                                    }
                                    if (effectCounter == 8)
                                    {
                                        dst2 = new IbRect(dst.Left, dst.Top + (int)((Width * gv.screenDensity) / 3f * 1f), (int)((Width * gv.screenDensity) / 3f), (int)((Width * gv.screenDensity) / 3f));
                                    }
                                    if (effectCounter == 9)
                                    {
                                        dst2 = new IbRect(dst.Left, dst.Top + (int)((Width * gv.screenDensity) / 3f * 2f), (int)((Width * gv.screenDensity) / 3f), (int)((Width * gv.screenDensity) / 3f));
                                    }
                                    gv.DrawBitmap(fx, src, dst2, -0.01f, false, 1.0f, true);
                                    gv.cc.DisposeOfBitmap(ref fx);
                                }
                            }
                
                 
            }

            //draw level up symbol
        }

        public void Update(int elapsed)
        {
            //animate button?
        }
    }
}
