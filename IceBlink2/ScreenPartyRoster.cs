using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenPartyRoster
    {
	    //public gv.module gv.mod;
	    public GameView gv;

	    public List<IbbButton> btnPartyIndex = new List<IbbButton>();
        public List<IbbButton> btnPartyRosterIndex = new List<IbbButton>();
	    private IbbButton btnDown = null;
	    private IbbButton btnUp = null;
	    private IbbButton btnHelp = null;
	    private IbbButton btnReturn = null;
	    private bool dialogOpen = false;
	    private int partyScreenPcIndex = 0;
	    private int partyRosterPcIndex = 0;
	    private bool lastClickedPlayerList = true;
	    private string stringMessagePartyRoster = "";

	    public ScreenPartyRoster(Module m, GameView g)
	    {
            //gv.mod = m;
            gv = g;
		    setControlsStart();
		    stringMessagePartyRoster = gv.cc.loadTextToString("data/MessagePartyRoster.txt");
	    }    
	    public void refreshPlayerTokens()
	    {
		    int cntPCs = 0;
		    foreach (IbbButton btn in btnPartyIndex)
		    {
			    if (cntPCs < gv.mod.playerList.Count)
			    {
                    gv.cc.DisposeOfBitmap(ref btn.Img2);
                    btn.Img2 = gv.cc.LoadBitmap(gv.mod.playerList[cntPCs].tokenFilename);						
			    }
			    else
			    {
				    btn.Img2 = null;
			    }
			    cntPCs++;
		    }
	    }
        public void refreshRosterPlayerTokens()
        {
            int cntPCs = 0;
            foreach (IbbButton btn in btnPartyRosterIndex)
            {
                if (cntPCs < gv.mod.partyRosterList.Count)
                {
                    gv.cc.DisposeOfBitmap(ref btn.Img2);
                    btn.Img2 = gv.cc.LoadBitmap(gv.mod.partyRosterList[cntPCs].tokenFilename);
                }
                else
                {
                    btn.Img2 = null;
                }
                cntPCs++;
            }
        }

	    public void setControlsStart()
	    {		
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;
		
		    for (int x = 0; x < gv.mod.MaxPartySize; x++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);
                gv.cc.DisposeOfBitmap(ref btnNew.Img);
                btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
                gv.cc.DisposeOfBitmap(ref btnNew.Glow);
                btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnNew.X = ((x+5) * gv.squareSize) + (padW * (x+1)) + gv.oXshift;
			    btnNew.Y = (gv.squareSize / 2) + (pH * 1);
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnPartyIndex.Add(btnNew);
		    }
            for (int x = 0; x < 6; x++)
            {
                IbbButton btnNew = new IbbButton(gv, 1.0f);
                gv.cc.DisposeOfBitmap(ref btnNew.Img);
                btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
                gv.cc.DisposeOfBitmap(ref btnNew.Glow);
                btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnNew.X = ((x+5) * gv.squareSize) + (padW * (x+1)) + gv.oXshift;
                btnNew.Y = (3 * gv.squareSize) + (pH * 2);
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                btnPartyRosterIndex.Add(btnNew);
            }
            for (int x = 0; x < 6; x++)
            {
                IbbButton btnNew = new IbbButton(gv, 1.0f);
                gv.cc.DisposeOfBitmap(ref btnNew.Img);
                btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
                gv.cc.DisposeOfBitmap(ref btnNew.Glow);
                btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnNew.X = ((x+5) * gv.squareSize) + (padW * (x + 1)) + gv.oXshift;
                btnNew.Y = (4 * gv.squareSize) + (pH * 3);
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                btnPartyRosterIndex.Add(btnNew);
            }
		    if (btnDown == null)
		    {
			    btnDown = new IbbButton(gv, 1.0f);
                btnDown.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnDown.Img2 = gv.cc.LoadBitmap("ctrl_down_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
                btnDown.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnDown.X = 7 * gv.squareSize + (gv.squareSize/2) - (pW * 1);
			    btnDown.Y = 2 * gv.squareSize - (pH * 2);
                btnDown.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnDown.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnUp == null)
		    {
			    btnUp = new IbbButton(gv, 1.0f);
                btnUp.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnUp.Img2 = gv.cc.LoadBitmap("ctrl_up_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnUp.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnUp.X = 8 * gv.squareSize + (gv.squareSize/2) + (pW * 1);
			    btnUp.Y = 2 * gv.squareSize - (pH * 2);
                btnUp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnUp.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnHelp == null)
		    {
			    btnHelp = new IbbButton(gv, 0.8f);	
			    btnHelp.Text = "HELP";
			    btnHelp.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnHelp.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnHelp.X = 5 * gv.squareSize + padW * 1 + gv.oXshift;
			    btnHelp.Y = 9 * gv.squareSize + (pH * 2);
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    if (btnReturn == null)
		    {
			    btnReturn = new IbbButton(gv, 1.2f);	
			    btnReturn.Text = "RETURN";
			    btnReturn.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnReturn.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnReturn.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
			    btnReturn.Y = 9 * gv.squareSize + (pH * 2);
                btnReturn.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturn.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
	    }
	
	    //PARTY SCREEN
        public void redrawPartyRoster()
        {    	
    	    if (partyScreenPcIndex >= gv.mod.playerList.Count)
    	    {
    		    partyScreenPcIndex = 0;
    	    }
    	    if (partyRosterPcIndex >= gv.mod.partyRosterList.Count)
    	    {
    		    partyRosterPcIndex = 0;
    	    }
    	    Player pc = null;
    	    if ((gv.mod.playerList.Count > 0) && (lastClickedPlayerList))
    	    {
    		    pc = gv.mod.playerList[partyScreenPcIndex];
    	    }
    	    else if ((gv.mod.partyRosterList.Count > 0) && (!lastClickedPlayerList))
    	    {
    		    pc = gv.mod.partyRosterList[partyRosterPcIndex];
    	    }
    	    if (pc != null)
    	    {
    		    gv.sf.UpdateStats(pc);
    	    }
            
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padH = gv.squareSize/6;
    	    int locY = 0;
    	    int locX = pW * 4;
    	    int textH = (int)gv.drawFontRegHeight;
            int spacing = textH;
    	    int tabX = pW * 50;
    	    int tabX2 = pW * 70;
    	    int leftStartY = 5 * gv.squareSize + (pH * 6);
    	    
            //Draw screen title name
		    int textWidth = (int)gv.cc.MeasureString("Current Party Members [" + gv.mod.MaxPartySize + " Maximum]", SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, gv.drawFontRegHeight);
            int ulX = (gv.screenWidth / 2) - (textWidth / 2);
		    gv.DrawText("Current Party Members [" + gv.mod.MaxPartySize + " Maximum]", ulX, pH * 3, 1.0f, Color.Gray);
		    		    
		    //DRAW EACH PC BUTTON
		    this.refreshPlayerTokens();

		    int cntPCs = 0;
		    foreach (IbbButton btn in btnPartyIndex)
		    {
			    if (cntPCs < gv.mod.playerList.Count)
			    {
				    if (cntPCs == partyScreenPcIndex) {btn.glowOn = true;}
				    else {btn.glowOn = false;}					
				    btn.Draw();
			    }
			    cntPCs++;
		    }

            //Draw screen title name
            textWidth = (int)gv.cc.MeasureString("Party Roster [Players in Reserve]", SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, gv.drawFontRegHeight);
            ulX = (gv.screenWidth / 2) - (textWidth / 2);
		    gv.DrawText("Party Roster [Players in Reserve]", ulX, 3 * gv.squareSize + (pH * 0), 1.0f, Color.Gray);

            //DRAW EACH ROSTER PC BUTTON
            this.refreshRosterPlayerTokens();

            cntPCs = 0;
            foreach (IbbButton btn in btnPartyRosterIndex)
            {
                if (cntPCs < gv.mod.partyRosterList.Count)
                {
                    if (cntPCs == partyRosterPcIndex) {btn.glowOn = true;}
                    else {btn.glowOn = false;}
                    btn.Draw();
                }
                cntPCs++;
            }
		
		    btnDown.Draw();
		    btnUp.Draw();
		    btnHelp.Draw();
		    btnReturn.Draw();

		    if (pc != null)
		    {
			    //DRAW LEFT STATS
			    gv.DrawText("Name: " + pc.name, locX, locY += leftStartY);
			    gv.DrawText(gv.mod.raceLabel + ": " + gv.mod.getRace(pc.raceTag).name, locX, locY += spacing);
			    if (pc.isMale)
			    {
				    gv.DrawText("Gender: Male", locX, locY += spacing);
			    }
			    else
			    {
				    gv.DrawText("Gender: Female", locX, locY += spacing);
			    }
			    gv.DrawText("Class: " + gv.mod.getPlayerClass(pc.classTag).name, locX, locY += spacing);			
			    gv.DrawText("Level: " + pc.classLevel, locX, locY += spacing);
			    gv.DrawText("XP: " + pc.XP + "/" + pc.XPNeeded, locX, locY += spacing);
			    gv.DrawText("---------------", locX, locY += spacing);
			
			    //draw spells known list
			    string allSpells = "";
			    foreach (string s in pc.knownSpellsTags)
			    {
				    Spell sp = gv.mod.getSpellByTag(s);
				    allSpells += sp.name + ", ";
			    }
			    gv.DrawText(pc.playerClass.spellLabelPlural + ": " + allSpells, locX, locY += spacing);
			
			    //draw traits known list
			    string allTraits = "";
			    foreach (string s in pc.knownTraitsTags)
			    {
				    Trait tr = gv.mod.getTraitByTag(s);
				    allTraits += tr.name + ", ";
			    }
			    gv.DrawText(pc.playerClass.traitLabelPlural + ": " + allTraits, locX, locY += spacing);
			
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
		    }
       }
        public void onTouchPartyRoster(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
            try
            {
                btnDown.glowOn = false;
                btnUp.glowOn = false;
                btnHelp.glowOn = false;
                btnReturn.glowOn = false;

                switch (eventType)
                {
                    case MouseEventType.EventType.MouseDown:
                    case MouseEventType.EventType.MouseMove:
                        int x = (int)e.X;
                        int y = (int)e.Y;

                        if (btnDown.getImpact(x, y))
                        {
                            btnDown.glowOn = true;
                        }
                        else if (btnUp.getImpact(x, y))
                        {
                            btnUp.glowOn = true;
                        }
                        else if (btnHelp.getImpact(x, y))
                        {
                            btnHelp.glowOn = true;
                        }
                        else if (btnReturn.getImpact(x, y))
                        {
                            btnReturn.glowOn = true;
                        }
                        break;

                    case MouseEventType.EventType.MouseUp:
                        x = (int)e.X;
                        y = (int)e.Y;

                        btnDown.glowOn = false;
                        btnUp.glowOn = false;
                        btnHelp.glowOn = false;
                        btnReturn.glowOn = false;

                        if (btnUp.getImpact(x, y))
                        {
                            //add selected PC to partyList and remove from pcList
                            if ((gv.mod.partyRosterList.Count > 0) && (gv.mod.playerList.Count < gv.mod.MaxPartySize))
                            {
                                Player copyPC = gv.mod.partyRosterList[partyRosterPcIndex].DeepCopy();
                                gv.cc.DisposeOfBitmap(ref copyPC.token);
                                copyPC.token = gv.cc.LoadBitmap(copyPC.tokenFilename);
                                gv.cc.DisposeOfBitmap(ref copyPC.portrait);
                                copyPC.portrait = gv.cc.LoadBitmap(copyPC.portraitFilename);
                                copyPC.playerClass = gv.mod.getPlayerClass(copyPC.classTag);
                                copyPC.race = gv.mod.getRace(copyPC.raceTag);
                                //Player copyPC = gv.mod.partyRosterList[partyRosterPcIndex];
                                gv.mod.playerList.Add(copyPC);
                                gv.mod.partyRosterList.RemoveAt(partyRosterPcIndex);
                            }
                        }
                        else if (btnDown.getImpact(x, y))
                        {
                            //remove selected from partyList and add to pcList
                            if (gv.mod.playerList.Count > 0)
                            {
                                Player copyPC = gv.mod.playerList[partyScreenPcIndex].DeepCopy();
                                gv.cc.DisposeOfBitmap(ref copyPC.token);
                                copyPC.token = gv.cc.LoadBitmap(copyPC.tokenFilename);
                                gv.cc.DisposeOfBitmap(ref copyPC.portrait);
                                copyPC.portrait = gv.cc.LoadBitmap(copyPC.portraitFilename);
                                copyPC.playerClass = gv.mod.getPlayerClass(copyPC.classTag);
                                copyPC.race = gv.mod.getRace(copyPC.raceTag);
                                gv.mod.partyRosterList.Add(copyPC);
                                gv.mod.playerList.RemoveAt(partyScreenPcIndex);
                            }
                        }
                        else if (btnHelp.getImpact(x, y))
                        {
                            tutorialPartyRoster();
                        }
                        else if (btnReturn.getImpact(x, y))
                        {
                            if (gv.mod.playerList.Count > 0)
                            {
                                //check to see if any non-removeable PCs are in roster
                                if (checkForNoneRemovablePcInRoster())
                                {
                                    return;
                                }
                                //check to see if mainPc is in party
                                if (checkForMainPc())
                                {
                                    gv.screenType = "main";
                                }
                                else
                                {
                                    gv.sf.MessageBoxHtml("You must have the Main PC (the first PC you created) in your party before exiting this screen");
                                }
                            }
                        }
                        for (int j = 0; j < gv.mod.playerList.Count; j++)
                        {
                            if (btnPartyIndex[j].getImpact(x, y))
                            {
                                partyScreenPcIndex = j;
                                lastClickedPlayerList = true;
                            }
                        }
                        for (int j = 0; j < gv.mod.partyRosterList.Count; j++)
                        {
                            if (btnPartyRosterIndex[j].getImpact(x, y))
                            {
                                partyRosterPcIndex = j;
                                lastClickedPlayerList = false;
                            }
                        }
                        break;
                }
            }
            catch
            {

            }
	    }

        public bool checkForMainPc()
        {
            foreach (Player pc in gv.mod.playerList)
            {
                if (pc.mainPc)
                {
                    return true;
                }
            }
            return false;
        }

        public bool checkForNoneRemovablePcInRoster()
        {
            foreach (Player pc in gv.mod.partyRosterList)
            {
                if (pc.nonRemoveablePc)
                {
                    gv.sf.MessageBoxHtml(pc.name + " must be in the active party before exiting this screen");
                    return true;
                }
            }
            return false;
        }

        public void tutorialPartyRoster()
        {
    	    gv.sf.MessageBoxHtml(this.stringMessagePartyRoster);
        }
    }
}
