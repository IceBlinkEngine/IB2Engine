using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;

namespace IceBlink2
{

    //public class IB2Button : System.Windows.Forms.ButtonBase
    public class IB2Button
    {
        [JsonIgnore]
        public GameView gv;

        //public TextFormat textFormat;
        //public TextLayout textLayout;
        public string tag = "";
        public string ImgFilename = "";    //this is the normal button and color intensity
        public string ImgOffFilename = ""; //this is usually a grayed out button
        public string ImgOnFilename = "";  //useful for buttons that are toggled on like "Move"
        public string Img2Filename = "";   //usually used for an image on top of default button like arrows or inventory backpack image
        public string Img2OffFilename = "";   //usually used for turned off image on top of default button like spell not available
        public string Img3Filename = "";   //typically used for convo plus notification icon
        public string GlowFilename = "";   //typically the green border highlight when hoover over or press button
        [JsonConverter(typeof(StringEnumConverter))]
        public buttonState btnState = buttonState.Normal;
        public bool btnNotificationOn = true; //used to determine whether Img3 is shown or not
        public bool glowOn = false;
        public string Text = "";
        public string Quantity = "";
        public string HotKey = "";
        public int X = 0;
        public int Y = 0;
        public string IBScript = "none";
        public int Width = 0;
        public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;
        public bool show = true;

        public IB2Button()
        {
            
        }

        public IB2Button(GameView g)
        {
            gv = g;
        }

        public void setupIB2Button(GameView g)
        {
            gv = g;
        }

        public void setHover(IB2Panel parentPanel, int x, int y)
        {
            //int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
            //int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;
            if (show)
            {
                glowOn = false;

                if ((x >= (int)((parentPanel.currentLocX + X) * gv.screenDensity)) && (x <= (int)((parentPanel.currentLocX + X + Width) * gv.screenDensity)))
                {
                    if ((y >= (int)((parentPanel.currentLocY + Y + gv.oYshift) * gv.screenDensity)) && (y <= (int)((parentPanel.currentLocY + Y + gv.oYshift + Height) * gv.screenDensity)))
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
            if (show)
            {
                if ((x >= (int)((parentPanel.currentLocX + X) * gv.screenDensity)) && (x <= (int)((parentPanel.currentLocX + X + Width) * gv.screenDensity)))
                {
                    if ((y >= (int)((parentPanel.currentLocY + Y + gv.oYshift) * gv.screenDensity)) && (y <= (int)((parentPanel.currentLocY + Y + gv.oYshift + Height) * gv.screenDensity)))
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
            if (!gv.mod.currentArea.isOverviewMap)
            {
                if (gv.mod.currentArea.overviewOwnZoneMapExists && gv.mod.currentArea.showOverviewButtonOwnZoneMap && this.tag == "btnOwnZoneMap")
                {
                    this.show = true;
                }
                else if (this.tag == "btnOwnZoneMap")
                {
                    this.show = false;
                }

                if (gv.mod.currentArea.overviewMotherZoneMapExists && gv.mod.currentArea.showOverviewButtonMotherZoneMap && this.tag == "btnMotherZoneMap")
                {
                    this.show = true;
                }
                else if (this.tag == "btnMotherZoneMap")
                {
                    this.show = false;
                }

                if (gv.mod.currentArea.overviewGrandMotherZoneMapExists && gv.mod.currentArea.showOverviewButtonGrandMotherZoneMap && this.tag == "btnGrandMotherZoneMap")
                {
                    this.show = true;
                }
                else if (this.tag == "btnGrandMotherZoneMap")
                {
                    this.show = false;
                }
            }
            else
            {
                foreach (Area a in gv.mod.moduleAreasObjects)
                {
                    if (a.filenameOfGrandMotherZoneMap == gv.mod.overviewReturnAreaName)
                    {
                        if (a.overviewOwnZoneMapExists && a.showOverviewButtonOwnZoneMap && this.tag == "btnOwnZoneMap")
                        {
                            this.show = true;
                        }
                        else if (this.tag == "btnOwnZoneMap")
                        {
                            this.show = false;
                        }

                        if (a.overviewMotherZoneMapExists && a.showOverviewButtonMotherZoneMap && this.tag == "btnMotherZoneMap")
                        {
                            this.show = true;
                        }
                        else if (this.tag == "btnMotherZoneMap")
                        {
                            this.show = false;
                        }

                        if (a.overviewGrandMotherZoneMapExists && a.showOverviewButtonGrandMotherZoneMap && this.tag == "btnGrandMotherZoneMap")
                        {
                            this.show = true;
                        }
                        else if (this.tag == "btnGrandMotherZoneMap")
                        {
                            this.show = false;
                        }
                    }
                }
            }

            if (gv.mod.currentArea.isOverviewMap && (this.tag == "btnRation" || this.tag == "btnTorch" || this.tag == "btnZoom"))
            {
                this.show = false;
            }
            else if (this.tag == "btnRation" || this.tag == "btnTorch" || this.tag == "btnZoom")
            {
                this.show = true;
            }

            if (!gv.mod.useComplexCoordinateSystem && (this.tag == "btnZoom" || this.tag == "btnTorch" || this.tag == "btnRation"))
            {
                this.show = false;
            }

            if (!gv.mod.useRationSystem && this.tag == "btnRation")
            {
                this.show = false;
            }

            if (!gv.mod.useLightSystem && this.tag == "btnTorch")
            {
                this.show = false;
            }

            /*
            if (gv.mod.currentArea.isOverviewMap && (this.tag == "btnRation" || this.tag == "btnTorch"|| this.tag == "btnZoom"))
            {
                this.show = false;
            }
            else if (this.tag == "btnRation" || this.tag == "btnTorch" || this.tag == "btnZoom")
            {
                this.show = true;
            }
            */
            string timeOfDay = "none";
            //iddo
            if (this.tag == "btnZoom")
            {
                //int timeofday = gv.mod.WorldTime % (24 * 60);
                //int hour = timeofday / 60;
                //int minute = timeofday % 60;
                //string sMinute = minute + "";
                //if (minute < 10)
                //{
                    //sMinute = "0" + minute;
                //}

                int dawn = 5 * 60;
                int sunrise = 6 * 60;
                int day = 7 * 60;
                int sunset = 17 * 60;
                int dusk = 18 * 60;
                int night = 20 * 60;
                int time = gv.mod.WorldTime % 1440;

                //bool consumeLightEnergy = false;
                if ((time >= dawn) && (time < sunrise))
                {
                    timeOfDay = "dawnButton";
                    //gv.DrawBitmap(gv.cc.tint_dawn, src, dst, 0, false, 1.0f / flickerReduction * flicker / 100f);
                    //gv.DrawBitmap(gv.cc.tint_dawn, src, dst, 0, false, 1.0f);
                }
                else if ((time >= sunrise) && (time < day))
                {
                    timeOfDay = "sunriseButton";
                    //gv.DrawBitmap(gv.cc.tint_sunrise, src, dst, 0, false, 1.0f);
                }
                else if ((time >= day) && (time < sunset))
                {
                    timeOfDay = "dayButton";
                    //no tint for day
                }
                else if ((time >= sunset) && (time < dusk))
                {
                    timeOfDay = "sunsetButton";
                    //gv.DrawBitmap(gv.cc.tint_sunset, src, dst, 0, false, 1.0f);
                }
                else if ((time >= dusk) && (time < night))
                { 
                    timeOfDay = "duskButton";
                    //gv.DrawBitmap(gv.cc.tint_dusk, src, dst, 0, false, 1.0f);
                }
                else if ((time >= night) || (time < dawn))
                {
                    timeOfDay = "nightButton";
                    //berlin
                    //consumeLightEnergy = true;
                }
                this.ImgFilename = timeOfDay;
                ImgFilename = timeOfDay;
                ImgOnFilename = timeOfDay;
                Img2Filename = timeOfDay;
                Img2OffFilename = timeOfDay;
                Img3Filename = timeOfDay;
                ImgOffFilename = timeOfDay;
            }


            if (show)
            {
                int pH = (int)((float)gv.screenHeight / 200.0f);
                int pW = (int)((float)gv.screenHeight / 200.0f);
                float fSize = (float)(gv.squareSize / 4) * scaler;
                //int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
                //int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;

                IbRect src = new IbRect(0, 0, Width, Height);
                IbRect dst = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity), (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity), (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));

                IbRect srcGlow = new IbRect(0, 0, Width, Height);
                IbRect dstGlow = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity) - (int)(7 * gv.screenDensity),
                                            (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity) - (int)(7 * gv.screenDensity),
                                            (int)((float)Width * gv.screenDensity) + (int)(15 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(15 * gv.screenDensity));

                //draw glow first if on
                if (glowOn)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(GlowFilename), srcGlow, dstGlow, -0.01f, false, 1.0f, true);
                    //   gv.DrawBitmap(gv.cc.GetFromBitmapList("tgl_bg"), src2, dst2, -0.01f, false, 0.75f, true);
                }

                //draw the proper button State
                if (btnState == buttonState.On)
                { 
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOnFilename), src, dst, -0.01f, false, 1.0f, true);
                }
                else if (btnState == buttonState.Off)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOffFilename), src, dst, -0.01f, false, 1.0f, true);
                }
                else
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgFilename), src, dst, -0.01f, false, 1.0f, true);
                }
                //draw the standard overlay image if has one
                if ((btnState == buttonState.Off) && (!Img2OffFilename.Equals("")))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(Img2OffFilename), src, dst, -0.01f, false, 1.0f, true);
                }
                else if (!Img2Filename.Equals(""))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(Img2Filename), src, dst, -0.01f, false, 1.0f, true);
                }
                //draw the notification image if turned on (like a level up or additional convo nodes image)
                if ((btnNotificationOn) && (!Img3Filename.Equals("")))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(Img3Filename), src, dst, -0.01f, false, 1.0f, true);
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
                float ulX = ((Width * gv.screenDensity) - stringSize) / 2;
                float ulY = ((Height * gv.screenDensity) - thisFontHeight) / 2;

                if (scaler == 0.4f)
                {
                    ulY = ((Height * gv.screenDensity));
                }

                if (this.tag == "btnZoom")
                {
                    int timeofday = gv.mod.WorldTime % (24 * 60);
                    int hour = timeofday / 60;
                    int minute = timeofday % 60;
                    string sMinute = minute + "";
                    if (minute < 10)
                    {
                        sMinute = "0" + minute;
                    }

                    int txtH = (int)gv.drawFontRegHeight;
                    //Text = hour + ":" + sMinute;
                    Text = "";
                    
                }

                if (this.tag == "btnTorch")
                {
                    int numberOfLightSources = 0;
                    foreach (ItemRefs ir in gv.mod.partyInventoryRefsList)
                    {
                        if (ir.isLightSource)
                        {
                            numberOfLightSources += ir.quantity;
                        }
                    }

                    Text = numberOfLightSources.ToString();
                }
                /*
                    for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                            int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY + y);
                            if (Text.Contains("green") && Text.Contains("Ld"))
                            {
                                int length = Text.Length;
                                string text2 = "";
                                //Ld 13green:10
                                //ld 7green:9
                                if (length == 10)
                                {
                                    text2 = Text.Remove(5);
                                }
                                if (length == 9)
                                {
                                    text2 = Text.Remove(4);
                                }
                                // DRAW TEXT
                                stringSize = gv.cc.MeasureString(text2, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

                                //place in the center
                                ulX = ((Width * gv.screenDensity) - stringSize) / 2;
                                ulY = ((Height * gv.screenDensity) - thisFontHeight) / 2;

                                if (scaler == 0.4f)
                                {
                                    ulY = ((Height * gv.screenDensity));
                                }
                                xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                                yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY + y);
                                //int test = text2.Length;
                                gv.DrawTextOutlined(text2, xLoc, yLoc, scaler, Color.Black);
                            }
                            else
                            {
                                gv.DrawTextOutlined(Text, xLoc, yLoc, scaler, Color.Black);

                            }
                            //gv.DrawText(Text, xLoc, yLoc, scaler, Color.Black);
                        }
                    }
                }
                */
                int xLoc1 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX);
                int yLoc1 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY);
                if (Text.Contains("green") && Text.Contains("Ld"))
                {
                    int length = Text.Length;
                    string text2 = "";
                    //Ld 13green:10
                    //ld 7green:9
                    if (length == 10)
                    {
                        text2 = Text.Remove(5);
                    }
                    if (length == 9)
                    {
                        text2 = Text.Remove(4);
                    }
                    // DRAW TEXT
                    stringSize = gv.cc.MeasureString(text2, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

                    //place in the center
                    ulX = ((Width * gv.screenDensity) - stringSize) / 2;
                    ulY = ((Height * gv.screenDensity) - thisFontHeight) / 2;

                    if (scaler == 0.4f)
                    {
                        ulY = ((Height * gv.screenDensity));
                    }
                    xLoc1 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX);
                    yLoc1 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY);
                    gv.DrawTextOutlined(text2, xLoc1, yLoc1, scaler, Color.Lime);
                }
                else
                {
                    gv.DrawTextOutlined(Text, xLoc1, yLoc1, scaler, Color.White);
                }

                // DRAW QUANTITY
                stringSize = gv.cc.MeasureString(Quantity, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

                //place in the bottom right quadrant
                ulX = (((Width * gv.screenDensity) - stringSize) / 8) * 7;
                ulY = (((Height * gv.screenDensity) - thisFontHeight) / 8) * 7;
                if (this.tag == "btnZoom")
                {
                    //Quantity = gv.mod.timePerStepAfterSpeedCalc + " min";
                    int timeofday = gv.mod.WorldTime % (24 * 60);
                    int hour = timeofday / 60;
                    int minute = timeofday % 60;
                    string sMinute = minute + "";
                    if (minute < 10)
                    {
                        sMinute = "0" + minute;
                    }

                    int txtH = (int)gv.drawFontRegHeight;
                    //Text = hour + ":" + sMinute;
                    Quantity = hour + ":" + sMinute + "  ";
                }
                if (this.tag == "btnTorch")
                {
                    Quantity = gv.mod.currentLightUnitsLeft.ToString();
                }
                if (this.tag == "btnRation")
                {
                    Quantity = gv.mod.numberOfRationsRemaining.ToString();
                }

                /*
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                            int yLoc = 0;
                            if (this.tag == "btnZoom")
                            {
                                xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x - 3 * pW);
                                yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY + y) - pW;
                            }
                            else
                            {
                                yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY + y);
                            }
                            //int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY + y);
                            gv.DrawText(Quantity, xLoc, yLoc, scaler, Color.Black);
                        }
                        }
                }
                */
                int xLoc2 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX);
                int yLoc2 = 0;
                if (this.tag == "btnZoom")
                {
                    xLoc2 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX - 3*pW);
                    yLoc2 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY) - pW;
                }
                else
                {
                    yLoc2 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY);
                }
                //int yLoc2 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY);
                if (this.tag == "btnTorch" && gv.mod.partyLightOn)
                {
                    
                    int dawn = 5 * 60;
                    int sunrise = 6 * 60;
                    int day = 7 * 60;
                    int sunset = 17 * 60;
                    int dusk = 18 * 60;
                    int night = 20 * 60;
                    int time = gv.mod.WorldTime % 1440;

                    bool consumeLightEnergy = false;
                    if ((time >= dawn) && (time < sunrise))
                    {
                        //gv.DrawBitmap(gv.cc.tint_dawn, src, dst, 0, false, 1.0f / flickerReduction * flicker / 100f);
                        //gv.DrawBitmap(gv.cc.tint_dawn, src, dst, 0, false, 1.0f);
                    }
                    else if ((time >= sunrise) && (time < day))
                    {
                        //gv.DrawBitmap(gv.cc.tint_sunrise, src, dst, 0, false, 1.0f);
                    }
                    else if ((time >= day) && (time < sunset))
                    {
                        //no tint for day
                    }
                    else if ((time >= sunset) && (time < dusk))
                    {
                        //gv.DrawBitmap(gv.cc.tint_sunset, src, dst, 0, false, 1.0f);
                    }
                    else if ((time >= dusk) && (time < night))
                    {
                        //gv.DrawBitmap(gv.cc.tint_dusk, src, dst, 0, false, 1.0f);
                    }
                    else if ((time >= night) || (time < dawn))
                    {
                        //berlin
                        consumeLightEnergy = true;
                    }

                    if (!gv.mod.currentArea.UseDayNightCycle)
                    {
                        consumeLightEnergy = true;
                    }

                    if (!gv.mod.currentArea.useLightSystem)
                    {
                        consumeLightEnergy = false;
                    }
                    if (consumeLightEnergy)
                    {
                        gv.DrawTextOutlined(Quantity, xLoc2, yLoc2, scaler, Color.Yellow);
                    }
                    else
                    {
                        gv.DrawTextOutlined(Quantity, xLoc2, yLoc2, scaler, Color.White);
                    }
                }
                else
                {
                    gv.DrawTextOutlined(Quantity, xLoc2, yLoc2, scaler, Color.White);
                }

                // DRAW HOTKEY
                if (gv.showHotKeys)
                {
                    stringSize = gv.cc.MeasureString(HotKey, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

                    //place in the bottom center
                    ulX = ((Width * gv.screenDensity) - stringSize) / 2;
                    ulY = (((Height * gv.screenDensity) - thisFontHeight) / 4) * 3;

                    /*
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x != 0 || y != 0)
                            {
                                int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                                int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY + y);
                                gv.DrawText(HotKey, xLoc, yLoc, scaler, Color.Black);
                            }
                        }
                    }
                    */
                    int xLoc3 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX);
                    int yLoc3 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY);
                    gv.DrawTextOutlined(HotKey, xLoc3, yLoc3, scaler, Color.Red);
                }
            }
        }

       

        public void Update(int elapsed, IB2Panel parentPanel)
        {
            //animate button?
        }
    }
}
