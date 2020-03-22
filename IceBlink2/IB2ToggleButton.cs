using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IB2ToggleButton
    {
        [JsonIgnore]
        public GameView gv;
        public string tag = "";
        public string ImgOnFilename = "";
        public string ImgOffFilename = "";
        public bool toggleOn = false;
        public int X = 0;
        public int Y = 0;        
        public int Width = 0;
        public int Height = 0;
        public bool show = true;

        public IB2ToggleButton()
        {
            
        }

        public IB2ToggleButton(GameView g)
        {
            gv = g;

        }

        public void setupIB2ToggleButton(GameView g)
        {
            gv = g;

        }

        public bool getImpact(IB2Panel parentPanel, int x, int y)
        {
            //int Width = gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Width;
            //int Height = gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Height;
            if (show)
            {
                if ((x >= (int)((parentPanel.currentLocX + X) * gv.screenDensity)) && (x <= (int)((parentPanel.currentLocX + X + Width) * gv.screenDensity)))
                {
                    if ((y >= (int)((parentPanel.currentLocY + Y + gv.oYshift) * gv.screenDensity)) && (y <= (int)((parentPanel.currentLocY + Y + gv.oYshift + Height) * gv.screenDensity)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Draw(IB2Panel parentPanel)
        {
            if (show)
            {
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Height);
                IbRect dst = new IbRect(0, 0, 0, 0);
                if (gv.mod.useMinimalisticUI)
                {
                    dst = new IbRect((int)((parentPanel.currentLocX + this.X - 2 * gv.pS) * gv.screenDensity + gv.pS), (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + gv.pS), (int)((float)Width * gv.screenDensity) - 2*gv.pS, (int)((float)Height * gv.screenDensity) - 2*gv.pS);
                }
                else
                {
                    dst = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity), (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity), (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));
                }

                if (toggleOn)
                {
                    if (gv.mod.useMinimalisticUI)
                    {
                        IbRect src2 = new IbRect(0,0,100,100);
                        IbRect dst2 = new IbRect((int)((parentPanel.currentLocX + this.X - 2 * gv.pS) * gv.screenDensity), (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity), (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));
                        gv.DrawBitmap(gv.cc.GetFromBitmapList("tgl_bg"), src2, dst2, -0.01f, false, 0.75f, true);
                    }
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOnFilename), src, dst, -0.01f, false, 1f,true);
                }
                else
                {
                    if (gv.mod.useMinimalisticUI)
                    {
                        IbRect src2 = new IbRect(0, 0, 100, 100);
                        IbRect dst2 = new IbRect((int)((parentPanel.currentLocX + this.X - 2 * gv.pS) * gv.screenDensity), (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity), (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));
                        gv.DrawBitmap(gv.cc.GetFromBitmapList("tgl_bg"), src2, dst2, -0.01f, false, 0.75f,true);
                    }
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOffFilename), src, dst, -0.01f, false, 1f,true);
                }
            }
        }

        public void Update(int elapsed)
        {
            //animate button?
        }
    }
}
