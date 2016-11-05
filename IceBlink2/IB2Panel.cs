using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IB2Panel
    {
        //this class is handled differently than Android version
        [JsonIgnore]
        public GameView gv;
        public string tag = "";
        public string backgroundImageFilename = "";
        public bool hiding = false;
        public bool showing = false;
        public int shownLocX = 0;
        public int shownLocY = 0;
        public int currentLocX = 0;
        public int currentLocY = 0;
        public int hiddenLocX = 0;
        public int hiddenLocY = 0;
        public int hidingXIncrement = 0;
        public int hidingYIncrement = 0;
        public int Width = 0;
        public int Height = 0;
        public List<IB2Button> buttonList = new List<IB2Button>();
        public List<IB2ToggleButton> toggleList = new List<IB2ToggleButton>();
        public List<IB2Portrait> portraitList = new List<IB2Portrait>();
        public List<IB2HtmlLogBox> logList = new List<IB2HtmlLogBox>();

        public IB2Panel()
        {
            
        }

        public IB2Panel(GameView g)
        {
            gv = g;
            currentLocX = shownLocX;
            currentLocY = shownLocY;
        }

        public void setupIB2Panel(GameView g)
        {
            gv = g;
            currentLocX = shownLocX;
            currentLocY = shownLocY;

            //try to do aspect ratio adjustemnets here
            //to do
            //default is 16:9 (=1,77777777...)
            //16:10 would be 1.6, ie increase y coordinate (move panels down)
            //4:3 would be 1.3333333333,ie increase y coordinate (move panels down)

            float yAdjustmentFactor = 0;
            float width = gv.screenWidth;
            float height = gv.screenHeight;
            float currentAspectRatio = width / height;
            yAdjustmentFactor = (1920f / 1080f) / currentAspectRatio;

            currentLocY = (int)(currentLocY * yAdjustmentFactor);

            foreach (IB2Button btn in buttonList)
            {
                btn.setupIB2Button(gv);
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                btn.setupIB2ToggleButton(gv);
            }
            foreach (IB2Portrait btn in portraitList)
            {
                btn.setupIB2Portrait(gv);
            }
            foreach (IB2HtmlLogBox log in logList)
            {
                log.setupIB2HtmlLogBox(gv);
            }
        }

        public void setHover(int x, int y)
        {
            //iterate over all controls and set glow on/off
            foreach (IB2Button btn in buttonList)
            {
                btn.setHover(this, x, y);
            }            
            foreach (IB2Portrait btn in portraitList)
            {
                btn.setHover(this, x, y);
            }
        }

        public string getImpact(int x, int y)
        {
            //iterate over all controls and get impact
            foreach (IB2Button btn in buttonList)
            {
                if (btn.getImpact(this, x, y))
                {
                    return btn.tag;
                }
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                if (btn.getImpact(this, x, y))
                {
                    return btn.tag;
                }
            }
            foreach (IB2Portrait btn in portraitList)
            {
                if (btn.getImpact(this, x, y))
                {
                    return btn.tag;
                }
            }
            foreach (IB2HtmlLogBox log in logList)
            {
                //log.onDrawLogBox();
            }
            return "";
        }

        public void Draw()
        {
            if (!gv.mod.useMinimalisticUI)
            {
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
                IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
                if ((this.tag != "InitiativePanel") && (this.tag != "logPanel"))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                }
                
                    if ((this.tag.Contains("logPanel")) && (gv.screenCombat.showIniBar) && (gv.screenType.Equals("combat")))
                    {
                        dst = new IbRect((int)(currentLocX * gv.screenDensity) + gv.pS, (int)(currentLocY * gv.screenDensity) + gv.squareSize + 4*gv.pS, (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity - gv.squareSize - 4*gv.pS));
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                    }
                    else if (this.tag.Contains("logPanel") && (gv.screenType.Equals("combat")))
                    {
                        dst = new IbRect((int)(currentLocX * gv.screenDensity) + gv.pS, (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                    }
                    else if (this.tag.Contains("logPanel"))
                    {
                        dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                    }

            }
            //IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
            //IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
            //gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);

            //iterate over all controls and draw
            foreach (IB2Button btn in buttonList)
            {
                btn.Draw(this);
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                btn.Draw(this);
            }
            foreach (IB2Portrait btn in portraitList)
            {
                btn.Draw(this);
            }

            if (!gv.mod.useMinimalisticUI)
            {
                foreach (IB2HtmlLogBox log in logList)
                {
                    log.onDrawLogBox(this);
                }
            }
        }

        public void DrawLogBackground()
        {
            //if (gv.screenType.Equals("main") )
            //{
                //IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
                //IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity - 3 * gv.pS), (int)(Width * gv.screenDensity + 2 * gv.pS), (int)(Height * gv.screenDensity - gv.squareSize + 7 * gv.pS));
                //gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.8f * gv.mod.logOpacity);
            //}
            //else
            //{
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
                IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity + gv.oXshift - 2.5 * gv.pS), (int)(currentLocY * gv.screenDensity) - 4 * gv.pS, (int)(Width * gv.screenDensity + 6.5 * gv.pS), (int)(Height * gv.screenDensity - gv.squareSize + 18 * gv.pS));
                gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.575f * gv.mod.logOpacity);
            //}

            foreach (IB2HtmlLogBox log in logList)
            {
                log.onDrawLogBox(this);
            }

            //foreach (IB2ToggleButton btn in toggleList)
            //{
                //btn.Draw(this);
            //}
            /*
            src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
            dst = new IbRect(gv.pS, (int)((gv.playerOffsetY*2+1 -2) * gv.squareSize + 2*gv.pS), (int)(5 * gv.squareSize), (int)(1 * gv.squareSize - 2*gv.pS ));
            gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
            */

            /*
            //iterate over all controls and draw
            foreach (IB2Button btn in buttonList)
            {
                btn.Draw(this);
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                btn.Draw(this);
            }
            foreach (IB2Portrait btn in portraitList)
            {
                btn.Draw(this);
            }
            */

        }

        public void Update(int elapsed)
        {
            //animate hiding panel
            if (hiding)
            {
                currentLocX += hidingXIncrement * elapsed;
                currentLocY += hidingYIncrement * elapsed;
                //hiding left and passed
                if ((hidingXIncrement < 0) && (currentLocX < hiddenLocX))
                {
                    currentLocX = hiddenLocX;
                    hiding = false;
                }
                //hiding right and passed
                if ((hidingXIncrement > 0) && (currentLocX > hiddenLocX))
                {
                    currentLocX = hiddenLocX;
                    hiding = false;
                }
                //hiding down and passed
                if ((hidingYIncrement > 0) && (currentLocY > hiddenLocY))
                {
                    currentLocY = hiddenLocY;
                    hiding = false;
                }
                //hiding up and passed
                if ((hidingYIncrement < 0) && (currentLocY < hiddenLocY))
                {
                    currentLocY = hiddenLocY;
                    hiding = false;
                }
            }
            else if (showing)
            {
                currentLocX -= hidingXIncrement * elapsed;
                currentLocY -= hidingYIncrement * elapsed;
                //showing right and passed
                if ((hidingXIncrement < 0) && (currentLocX > shownLocX))
                {
                    currentLocX = shownLocX;
                    showing = false;
                }
                //showing left and passed
                if ((hidingXIncrement > 0) && (currentLocX < shownLocX))
                {
                    currentLocX = shownLocX;
                    showing = false;
                }
                //showing up and passed
                if ((hidingYIncrement > 0) && (currentLocY < shownLocY))
                {
                    currentLocY = shownLocY;
                    showing = false;
                }
                //showing down and passed
                if ((hidingYIncrement < 0) && (currentLocY > shownLocY))
                {
                    currentLocY = shownLocY;
                    showing = false;
                }
            }
        }
    }
}
