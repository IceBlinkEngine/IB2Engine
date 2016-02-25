using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class IB2Button
    {
        public GameView gv;
        public string name = "";
        public string ImgFilename = "";    //this is the normal button and color intensity
        public string ImgOffFilename = ""; //this is usually a grayed out button
        public string ImgOnFilename = "";  //useful for buttons that are toggled on like "Move"
        public string Img2Filename = "";   //usually used for an image on top of default button like arrows or inventory backpack image
        public string Img2OffFilename = "";   //usually used for turned off image on top of default button like spell not available
        public string Img3Filename = "";   //typically used for convo plus notification icon
        public string GlowFilename = "";   //typically the green border highlight when hoover over or press button
        public buttonState btnState = buttonState.Normal;
        public bool btnNotificationOn = true; //used to determine whether Img3 is shown or not
        public bool glowOn = false;
        public string Text = "";
        public string Quantity = "";
        public string HotKey = "";
        public int X = 0;
        public int Y = 0;
        public string IBScript = "none";
        //public int Width = 0;
        //public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;

        public IB2Button(GameView g)
        {
            gv = g;
        }

        public void Draw(IB2Panel parentPanel)
        {
            int pH = (int)((float)gv.screenHeight / 200.0f);
            int pW = (int)((float)gv.screenHeight / 200.0f);
            float fSize = (float)(gv.squareSize / 4) * scaler;
            int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
            int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;

            IbRect src = new IbRect(0, 0, Width, Height);                        
            IbRect dst = new IbRect(parentPanel.currentLocX + this.X, parentPanel.currentLocY + this.Y, (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));

            IbRect srcGlow = new IbRect(0, 0, Width, Height);
            IbRect dstGlow = new IbRect(parentPanel.currentLocX + this.X - (int)(7 * gv.screenDensity),
                                        parentPanel.currentLocY + this.Y - (int)(7 * gv.screenDensity),
                                        (int)((float)Width) + (int)(15 * gv.screenDensity),
                                        (int)((float)Height) + (int)(15 * gv.screenDensity));

            //draw glow first if on
            if (glowOn)
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(GlowFilename), srcGlow, dstGlow);
            }            
            
            //draw the proper button State
            if (btnState == buttonState.On)
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOnFilename), src, dst);
            }
            else if (btnState == buttonState.Off)
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOffFilename), src, dst);
            }
            else
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgFilename), src, dst);
            }
            //draw the standard overlay image if has one
            if ((btnState == buttonState.Off) && (!Img2OffFilename.Equals("")))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img2OffFilename), src, dst);
            }
            else if (!Img2Filename.Equals(""))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img2Filename), src, dst);
            }
            //draw the notification image if turned on (like a level up or additional convo nodes image)
            if ((btnNotificationOn) && (!Img3Filename.Equals("")))
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(Img3Filename), src, dst);
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

            // DRAW TEXT
            float stringSize = gv.cc.MeasureString(Text, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

            //place in the center
            float ulX = ((Width) - stringSize) / 2;
            float ulY = ((Height) - thisFontHeight) / 2;

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(Text, parentPanel.currentLocX + this.X + ulX + x, parentPanel.currentLocY + this.Y + ulY + y, scaler, Color.Black);
                }
            }
            gv.DrawText(Text, parentPanel.currentLocX + this.X + ulX, parentPanel.currentLocY + this.Y + ulY, scaler, Color.White);

            // DRAW QUANTITY
            stringSize = gv.cc.MeasureString(Quantity, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

            //place in the bottom right quadrant
            ulX = (((Width) - stringSize) / 8) * 7;
            ulY = (((Height) - thisFontHeight) / 8) * 7;

            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    gv.DrawText(Quantity, parentPanel.currentLocX + this.X + ulX + x, parentPanel.currentLocY + this.Y + ulY + y, scaler, Color.Black);
                }
            }
            gv.DrawText(Quantity, parentPanel.currentLocX + this.X + ulX, parentPanel.currentLocY + this.Y + ulY, scaler, Color.White);

            // DRAW HOTKEY
            if (gv.showHotKeys)
            {
                stringSize = gv.cc.MeasureString(HotKey, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

                //place in the bottom center
                ulX = ((Width) - stringSize) / 2;
                ulY = (((Height) - thisFontHeight) / 4) * 3;

                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        gv.DrawText(HotKey, parentPanel.currentLocX + this.X + ulX + x, parentPanel.currentLocY + this.Y + ulY + y, scaler, Color.Black);
                    }
                }
                gv.DrawText(HotKey, parentPanel.currentLocX + this.X + ulX, parentPanel.currentLocY + this.Y + ulY, scaler, Color.Red);
            }
        }

        public void Update(int elapsed, IB2Panel parentPanel)
        {
            //animate button?
        }
    }
}
