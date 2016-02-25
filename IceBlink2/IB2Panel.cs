using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IB2Panel
    {
        //this class is handled differently than Android version
        public GameView gv;
        public string name = "";
        public string backgroundImageFilename = "";
        public int LocX = 0;
        public int LocY = 0;
        public int currentLocX = 0;
        public int currentLocY = 0;
        public int hiddenLocX = 0;
        public int hiddenLocY = 0;
        public int Width = 0;
        public int Height = 0;
        public List<IbbButton> buttonList = new List<IbbButton>();
        public List<IbbToggleButton> toggleList = new List<IbbToggleButton>();
        public List<IbbPortrait> portraitList = new List<IbbPortrait>();
        public List<IbbHtmlLogBox> logList = new List<IbbHtmlLogBox>();


        public IB2Panel(GameView g)
        {
            gv = g;
        }

        public void Draw()
        {
            IbRect src = new IbRect(0, 0, this.ImgBG.PixelSize.Width, this.ImgBG.PixelSize.Height);
            IbRect dst = new IbRect(this.LocX, this.LocY, Width, Height);
            gv.DrawBitmap(this.ImgBG, src, dst);

            //iterate over all controls and draw
            foreach (IbbButton btn in buttonList)
            {
                btn.Draw();
            }
            foreach (IbbToggleButton btn in toggleList)
            {
                btn.Draw();
            }
            foreach (IbbPortrait btn in portraitList)
            {
                btn.Draw();
            }
            foreach (IbbHtmlLogBox log in logList)
            {
                log.onDrawLogBox();
            }
        }

        public void Update(int elapsed)
        {
            //animate hiding panel
        }
    }
}
