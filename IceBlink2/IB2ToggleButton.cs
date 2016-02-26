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
        public string name = "";
        public string ImgOnFilename = "";
        public string ImgOffFilename = "";
        public bool toggleOn = false;
        public int X = 0;
        public int Y = 0;
        //public int Width = 0;
        //public int Height = 0;

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
            int Width = gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Width;
            int Height = gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Height;

            if ((x >= parentPanel.currentLocX + X) && (x <= (parentPanel.currentLocX + X + Width)))
            {
                if ((y >= parentPanel.currentLocY + Y + gv.oYshift) && (y <= (parentPanel.currentLocY + Y + gv.oYshift + Height)))
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw(IB2Panel parentPanel)
        {
            IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Width, gv.cc.GetFromBitmapList(ImgOnFilename).PixelSize.Height);
            IbRect dst = new IbRect(parentPanel.currentLocX + X, parentPanel.currentLocY + Y, gv.squareSize / 2, gv.squareSize / 2);

            if (toggleOn)
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOnFilename), src, dst);
            }
            else
            {
                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgOffFilename), src, dst);
            }
        }

        public void Update(int elapsed)
        {
            //animate button?
        }
    }
}
