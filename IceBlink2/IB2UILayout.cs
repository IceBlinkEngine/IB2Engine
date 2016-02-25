using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IB2UILayout
    {
        public GameView gv;
        public List<IB2Panel> panelList = new List<IB2Panel>();

        public IB2UILayout(GameView g)
        {
            gv = g;
        }

        public void Draw()
        {
            //iterate over all controls and draw
            foreach (IB2Panel pnl in panelList)
            {
                pnl.Draw();
            }
        }

        public void Update(int elapsed)
        {
            //animate hiding panels
            foreach (IB2Panel pnl in panelList)
            {
                pnl.Update(elapsed);
            }
        }
    }
}
