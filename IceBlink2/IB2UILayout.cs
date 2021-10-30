using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IB2UILayout
    {
        [JsonIgnore]
        public GameView gv;
        public List<IB2Panel> panelList = new List<IB2Panel>();

        public IB2UILayout()
        {
            
        }

        public IB2UILayout(GameView g)
        {
            gv = g;
        }

        public void setupIB2UILayout(GameView g)
        {
            gv = g;
            foreach (IB2Panel pnl in panelList)
            {
                pnl.setupIB2Panel(gv);
            }
        }

        public void setHover(int x, int y)
        {
            //iterate over all controls and set glow on/off
            foreach (IB2Panel pnl in panelList)
            {
                pnl.setHover(x, y);
            }
        }

        public string getImpact(int x, int y)
        {
            //iterate over all controls and get impact
            foreach (IB2Panel pnl in panelList)
            {
                string rtn = pnl.getImpact(x, y);
                if (!rtn.Equals(""))
                {
                    return rtn;
                }
            }            
            return "";
        }

        public void Draw()
        {
            //iterate over all controls and draw
            if (!gv.mod.skipTextRender)
            {
                gv.mod.skipTextRender = true;
            }
            else
            {
                gv.mod.skipTextRender = false;
            }
                if (gv.mod.useMinimalisticUI && !gv.mod.currentArea.isOverviewMap)
                {
                    foreach (IB2Panel pnl in panelList)
                    {

                        if (pnl.tag.Equals("logPanel"))
                        {
                            pnl.DrawLogBackground();
                        }
                        else
                        {
                        //if (pnl.tag != "TraitsPanel")
                        //{
                            pnl.Draw();
                        //}
                        }
                    }
                }
                else
                {
                    foreach (IB2Panel pnl in panelList)
                    {
                        pnl.Draw();
                    }
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

        public IB2Button GetButtonByTag(string tag)
        {
            foreach (IB2Panel pnl in panelList)
            {
                foreach (IB2Button btn in pnl.buttonList)
                {
                    if (btn.tag.Equals(tag))
                    {
                        return btn;
                    }
                }
            }
            return null;
        }

        public IB2Panel GetPanelByTag(string tag)
        {
            foreach (IB2Panel pnl in panelList)
            {                
                if (pnl.tag.Equals(tag))
                {
                    return pnl;
                }                
            }
            return null;
        }

        public IB2ToggleButton GetToggleByTag(string tag)
        {
            foreach (IB2Panel pnl in panelList)
            {
                foreach (IB2ToggleButton btn in pnl.toggleList)
                {
                    if (btn.tag.Equals(tag))
                    {
                        return btn;
                    }
                }
            }
            return null;
        }
    }
}
