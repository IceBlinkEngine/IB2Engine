using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenPcSelector
    {
        //public gv.module gv.mod;
        public GameView gv;

        public List<IbbButton> btnPartyIndex = new List<IbbButton>();
        private IbbButton btnReturn = null;

        public string pcSelectorType = "spellcaster"; //spellcaster, target, itemuser
        public string callingScreen = "main"; //main, party, inventory        
        public int pcSelectorPcIndex = 0;
        
        public ScreenPcSelector(Module m, GameView g)
        {
            //gv.mod = m;
            gv = g;
            setControlsStart();
        }

        public void setControlsStart()
        {
            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padW = gv.squareSize / 6;
                        
            if (btnReturn == null)
            {
                btnReturn = new IbbButton(gv, 1.0f);
                btnReturn.Text = "RETURN SELECTED";
                btnReturn.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnReturn.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnReturn.X = (gv.screenWidth / 2) - (int)(170.0f * gv.screenDensity / 2.0f);
                btnReturn.Y = 10 * gv.squareSize + pH * 2;
                btnReturn.Height = (int)(50 * gv.screenDensity);
                btnReturn.Width = (int)(170 * gv.screenDensity);
            }            

            for (int x = 0; x < 6; x++)
            {
                IbbButton btnNew = new IbbButton(gv, 1.0f);
                btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
                btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnNew.X = ((x) * gv.squareSize) + (padW * (x + 1)) + gv.oXshift;
                btnNew.Y = pH * 2;
                btnNew.Height = (int)(50 * gv.screenDensity);
                btnNew.Width = (int)(50 * gv.screenDensity);

                btnPartyIndex.Add(btnNew);
            }
        }

        //PARTY SCREEN
        public void redrawPcSelector()
        {

            if (pcSelectorPcIndex >= gv.mod.playerList.Count)
            {
                pcSelectorPcIndex = 0;
            }
            Player pc = gv.mod.playerList[pcSelectorPcIndex];
            gv.sf.UpdateStats(pc);

            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padH = gv.squareSize / 6;
            int locY = 0;
            int locX = pW * 4;
            int textH = (int)gv.drawFontRegHeight;
            int spacing = textH;
            int tabX = pW * 50;
            int tabX2 = pW * 70;
            int leftStartY = btnPartyIndex[0].Y + btnPartyIndex[0].Height + (pH * 4);

            //DRAW EACH PC BUTTON
            int cntPCs = 0;
            foreach (IbbButton btn in btnPartyIndex)
            {
                if (cntPCs < gv.mod.playerList.Count)
                {
                    if (cntPCs == pcSelectorPcIndex) { btn.glowOn = true; }
                    else { btn.glowOn = false; }
                    btn.Draw();
                }
                cntPCs++;
            }

            //DRAW LEFT STATS
            //name            
            gv.DrawText("Name: " + pc.name, locX, locY += leftStartY, 1.0f, Color.White);

            //race
            gv.DrawText(gv.mod.raceLabel + ": " + gv.mod.getRace(pc.raceTag).name, locX, locY += spacing, 1.0f, Color.White);

            //gender
            if (pc.isMale)
            {
                gv.DrawText("Gender: Male", locX, locY += spacing, 1.0f, Color.White);
            }
            else
            {
                gv.DrawText("Gender: Female", locX, locY += spacing, 1.0f, Color.White);
            }

            //class
            gv.DrawText("Class: " + gv.mod.getPlayerClass(pc.classTag).name, locX, locY += spacing, 1.0f, Color.White);
            gv.DrawText("Level: " + pc.classLevel, locX, locY += spacing, 1.0f, Color.White);
            gv.DrawText("XP: " + pc.XP + "/" + pc.XPNeeded, locX, locY += spacing, 1.0f, Color.White);
            gv.DrawText("---------------", locX, locY += spacing, 1.0f, Color.White);
            
            //DRAW RIGHT STATS
            int actext = 0;
            if (gv.mod.ArmorClassAscending) { actext = pc.AC; }
            else { actext = 20 - pc.AC; }
            locY = 0;
            gv.DrawText("STR: " + pc.strength, tabX, locY += leftStartY);
            gv.DrawText("AC: " + actext, tabX2, locY);
            gv.DrawText("DEX: " + pc.dexterity, tabX, locY += spacing);
            gv.DrawText("HP: " + pc.hp + "/" + pc.hpMax, tabX2, locY);
            gv.DrawText("CON: " + pc.constitution, tabX, locY += spacing);
            gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, tabX2, locY);
            gv.DrawText("INT: " + pc.intelligence, tabX, locY += spacing);
            gv.DrawText("BAB: " + pc.baseAttBonus, tabX2, locY);
            gv.DrawText("WIS: " + pc.wisdom, tabX, locY += spacing);
            gv.DrawText("CHA: " + pc.charisma, tabX, locY += spacing);
                        
            btnReturn.Draw();
        }
        public void onTouchPcSelector(MouseEventArgs e, MouseEventType.EventType eventType)
        {
            try
            {
                btnReturn.glowOn = false;

                switch (eventType)
                {
                    case MouseEventType.EventType.MouseDown:
                    case MouseEventType.EventType.MouseMove:
                        int x = (int)e.X;
                        int y = (int)e.Y;

                        if (btnReturn.getImpact(x, y))
                        {
                            btnReturn.glowOn = true;
                        }
                        break;

                    case MouseEventType.EventType.MouseUp:
                        x = (int)e.X;
                        y = (int)e.Y;

                        btnReturn.glowOn = false;

                        Player pc = gv.mod.playerList[pcSelectorPcIndex];

                        if (btnReturn.getImpact(x, y))
                        {
                            if (pcSelectorType.Equals("spellcaster"))
                            {
                                gv.screenType = "main";
                            }
                        }
                        for (int j = 0; j < gv.mod.playerList.Count; j++)
                        {
                            if (btnPartyIndex[j].getImpact(x, y))
                            {
                                pcSelectorPcIndex = j;
                            }
                        }
                        break;
                }
            }
            catch
            { }
         }        
    }
}
