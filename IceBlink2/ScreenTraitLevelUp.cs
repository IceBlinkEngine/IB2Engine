using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenTraitLevelUp 
    {

	    private Module mod;
	    private GameView gv;
	
	    private int traitSlotIndex = 0;
	    private int slotsPerPage = 20;
	    private List<IbbButton> btnTraitSlots = new List<IbbButton>();
	    private IbbButton btnHelp = null;
	    private IbbButton btnSelect = null;
	    private IbbButton btnExit = null;
	    List<string> traitsToLearnTagsList = new List<string>();
	    private Player pc;
        public bool infoOnly = false; //set to true when called for info only
        private string stringMessageTraitLevelUp = "";
        private IbbHtmlTextBox description;
	
	
	    public ScreenTraitLevelUp(Module m, GameView g) 
	    {
		    mod = m;
		    gv = g;
		    setControlsStart();
		    pc = new Player();
		    stringMessageTraitLevelUp = gv.cc.loadTextToString("data/MessageTraitLevelUp.txt");
	    }
	
	    public void resetPC(bool info_only, Player p)
	    {
		    pc = p;
            infoOnly = info_only;
        }
	
	    public void setControlsStart()
	    {			
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;

            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;
		
		    if (btnSelect == null)
		    {
			    btnSelect = new IbbButton(gv, 0.8f);	
			    btnSelect.Text = "LEARN SELECTED TRAIT";
			    btnSelect.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnSelect.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnSelect.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
			    btnSelect.Y = 9 * gv.squareSize + pH * 2;
                btnSelect.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSelect.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnHelp == null)
		    {
			    btnHelp = new IbbButton(gv, 0.8f);	
			    btnHelp.Text = "HELP";
			    btnHelp.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnHelp.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnHelp.X = 5 * gv.squareSize + padW * 1 + gv.oXshift;
			    btnHelp.Y = 9 * gv.squareSize + pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    if (btnExit == null)
		    {
			    btnExit = new IbbButton(gv, 0.8f);	
			    btnExit.Text = "EXIT";
			    btnExit.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnExit.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnExit.X = (15 * gv.squareSize) - padW * 1 + gv.oXshift;
			    btnExit.Y = 9 * gv.squareSize + pH * 2;
                btnExit.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnExit.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }
		    for (int y = 0; y < slotsPerPage; y++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);
                gv.cc.DisposeOfBitmap(ref btnNew.Img);
                btnNew.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                gv.cc.DisposeOfBitmap(ref btnNew.Glow);
                btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    int x = y % 5;
			    int yy = y / 5;
			    btnNew.X = ((x + 4) * gv.squareSize) + (padW * (x+1)) + gv.oXshift;
			    btnNew.Y = (1 + yy) * gv.squareSize + (padW * yy);

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnTraitSlots.Add(btnNew);
		    }			
	    }
	
	    //CAST SELECTOR SCREEN (COMBAT and MAIN)
        public void redrawTraitLevelUp(bool inPcCreation)
        {
    	    traitsToLearnTagsList.Clear();
    	    fillToLearnList();
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
            int textH = (int)gv.drawFontRegHeight;
    	    int spacing = textH;
            int tabX = 5 * gv.squareSize + pW * 3;
            int noticeX = 5 * gv.squareSize + pW * 3;
    	    int noticeY = pH * 1 + spacing;
    	    int tabStartY = 4 * gv.squareSize + pW * 10;

            if (!infoOnly)
            {
                //DRAW TEXT		
                locY = (gv.squareSize * 0) + (pH * 2);
                gv.DrawText("Select One Trait to Learn", noticeX, pH * 1, 1.0f, Color.Gray);

                //DRAW NOTIFICATIONS
                if (isSelectedTraitSlotInKnownTraitsRange())
                {
                    Trait tr = GetCurrentlySelectedTrait();
                    
                    //check to see if already known
                    if (pc.knownTraitsTags.Contains(tr.tag))
                    {
                        //say that you already know this one
                        gv.DrawText("Already Known", noticeX, noticeY, 1.0f, Color.Yellow);
                    }
                    else //trait not known
                    {
                        //check if available to learn
                        if (isAvailableToLearn(tr.tag))
                        {
                            gv.DrawText("Available to Learn", noticeX, noticeY, 1.0f, Color.Lime);
                        }
                        else //not available yet
                        {
                            gv.DrawText("Trait Not Available to Learn Yet", noticeX, noticeY, 1.0f, Color.Red);
                        }
                    }
                }
            }	
		
		    //DRAW ALL TRAIT SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnTraitSlots)
		    {			
			    //Player pc = getCastingPlayer();
			
			    if (cntSlot == traitSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			
			    //show only traits for the PC class
			    if (cntSlot < pc.playerClass.traitsAllowed.Count)
			    {
				    TraitAllowed ta = pc.playerClass.traitsAllowed[cntSlot];
				    Trait tr = mod.getTraitByTag(ta.tag);

                    if (infoOnly)
                    {
                        if (pc.knownTraitsTags.Contains(tr.tag)) //check to see if already known, if so turn on button
                        {
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small");
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(tr.traitImage);
                        }
                        else //trait not known yet
                        {
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small_off");
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(tr.traitImage + "_off");
                        }
                    }
                    else
                    {
                        if (pc.knownTraitsTags.Contains(tr.tag)) //check to see if already known, if so turn off button
                        {
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small_off");
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(tr.traitImage + "_off");
                        }
                        else //trait not known yet
                        {
                            if (isAvailableToLearn(tr.tag)) //if available to learn, turn on button
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = gv.cc.LoadBitmap("btn_small");
                                gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = gv.cc.LoadBitmap(tr.traitImage);
                            }
                            else //not available to learn, turn off button
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = gv.cc.LoadBitmap("btn_small_off");
                                gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = gv.cc.LoadBitmap(tr.traitImage + "_off");
                            }
                        }
                    }				
			    }
			    else //slot is not in traits allowed index range
			    {
                    gv.cc.DisposeOfBitmap(ref btn.Img);
                    btn.Img = gv.cc.LoadBitmap("btn_small_off"); 
				    btn.Img2 = null;
			    }			
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;		
		    if (isSelectedTraitSlotInKnownTraitsRange())
		    {
                Trait tr = GetCurrentlySelectedTrait();
                
                string textToSpan = "<u>Description</u>" + "<BR>";
                textToSpan += "<b><i><big>" + tr.name + "</big></i></b><BR>";
                textToSpan += "Available at Level: " + getLevelAvailable(tr.tag) + "<BR>";
                textToSpan += "<BR>";
                textToSpan += tr.description;

                description.tbXloc = 11 * gv.squareSize;
                description.tbYloc = 1 * gv.squareSize;
                description.tbWidth = pW * 40;
                description.tbHeight = pH * 80;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox();
		    }

            if (infoOnly)
            {
                btnSelect.Text = "RETURN";
                btnSelect.Draw();
            }
            else
            {
                btnSelect.Text = "LEARN SELECTED TRAIT";
                btnHelp.Draw();
                btnExit.Draw();
                btnSelect.Draw();
            }            
        }
        public void onTouchTraitLevelUp(MouseEventArgs e, MouseEventType.EventType eventType, bool inPcCreation, bool inCombat)
	    {
		    btnHelp.glowOn = false;
		    btnExit.glowOn = false;
		    btnSelect.glowOn = false;
		
		    switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) e.X;
			    int y = (int) e.Y;
			    if (btnHelp.getImpact(x, y))
			    {
				    btnHelp.glowOn = true;
			    }
			    else if (btnSelect.getImpact(x, y))
			    {
				    btnSelect.glowOn = true;
			    }
			    else if (btnExit.getImpact(x, y))
			    {
				    btnExit.glowOn = true;
			    }
			    break;

            case MouseEventType.EventType.MouseUp:
                x = (int)e.X;
                y = (int)e.Y;
			
			    btnHelp.glowOn = false;
			    btnExit.glowOn = false;
			    btnSelect.glowOn = false;
			
			    for (int j = 0; j < slotsPerPage; j++)
			    {
				    if (btnTraitSlots[j].getImpact(x, y))
				    {
                        gv.PlaySound("btn_click");
					    traitSlotIndex = j;
				    }
			    }
			    if (btnHelp.getImpact(x, y))
			    {
                    if (!infoOnly)
                    {
                        gv.PlaySound("btn_click");
                        tutorialMessageTraitScreen();
                    }
			    }
			    else if (btnSelect.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
                    if (infoOnly)
                    {
                            if (inCombat)
                            {
                                gv.screenType = "combatParty";
                            }
                            else
                            {
                                gv.screenType = "party";
                            }
                        }
                    else
                    {
                        doSelectedTraitToLearn(inPcCreation);
                    }
			    }
			    else if (btnExit.getImpact(x, y))
			    {
                    if (!infoOnly)
                    {
                        gv.PlaySound("btn_click");
                        if (inPcCreation)
                        {
                            gv.screenType = "pcCreation";
                        }
                        else
                        {
                            gv.screenType = "party";
                        }
                    }						
			    }
			    break;		
		    }
	    }
    
        public void doSelectedTraitToLearn(bool inPcCreation)
        {
    	    if (isSelectedTraitSlotInKnownTraitsRange())
		    {
			    Trait tr = GetCurrentlySelectedTrait();
			    if (isAvailableToLearn(tr.tag))
			    {
				    //add trait
				    pc.knownTraitsTags.Add(tr.tag);
                    //public string useableInSituation = "Always"; //InCombat, OutOfCombat, Always, Passive
                    if (tr.useableInSituation.Contains("Always"))
                    {
                        pc.knownUsableTraitsTags.Add(tr.associatedSpellTag);
                        pc.knownOutsideCombatUsableTraitsTags.Add(tr.associatedSpellTag);
                        pc.knownInCombatUsableTraitsTags.Add(tr.associatedSpellTag);
                    }
                    if (tr.useableInSituation.Contains("OutOfCombat"))
                    {
                        pc.knownUsableTraitsTags.Add(tr.associatedSpellTag);
                        pc.knownOutsideCombatUsableTraitsTags.Add(tr.associatedSpellTag);
                    }
                    if (tr.useableInSituation.Contains("InCombat"))
                    {
                        pc.knownUsableTraitsTags.Add(tr.associatedSpellTag);
                        pc.knownInCombatUsableTraitsTags.Add(tr.associatedSpellTag);
                    }

                    //add permanent effects of trait to effect list of this pc
                    foreach (EffectTagForDropDownList efTag in tr.traitEffectTagList)
                    {
                        foreach (Effect ef in gv.mod.moduleEffectsList)
                        {
                            if (ef.tag == efTag.tag)
                            {
                                if (ef.isPermanent)
                                {
                                    bool doesNotExistAlfready = true;
                                    foreach (Effect ef2 in pc.effectsList)
                                    {
                                        if (ef2.tag == ef.tag)
                                        {
                                            doesNotExistAlfready = false;
                                            break;
                                        }
                                    }

                                    if (doesNotExistAlfready)
                                    {
                                        pc.effectsList.Add(ef);
                                    }
                                }
                            }
                        }
                    }

                    //else if in creation go back to partybuild				
                    if (inPcCreation)
				    {
					    //if there are spells to learn go to spell screen
					    List<string> spellTagsList = new List<string>();
				        spellTagsList = pc.getSpellsToLearn();
				        if (spellTagsList.Count > 0)
				        {
				    	    gv.screenSpellLevelUp.resetPC(false, pc, false);
				    	    gv.screenType = "learnSpellCreation";
				        }
				        else //no spells to learn
				        {				    
				    	    //save character, add them to the pcList of screenPartyBuild, and go back to build screen
				    	    gv.screenPcCreation.SaveCharacter(pc);
				    	    gv.screenPartyBuild.pcList.Add(pc);
				    	    gv.screenType = "partyBuild";
				        }
				    }			    
				    else
				    {
					    List<string> spellTagsList = new List<string>();
				        spellTagsList = pc.getSpellsToLearn();
				    
					    if (spellTagsList.Count > 0)
 	        	        {
						    gv.screenSpellLevelUp.resetPC(false, pc, false);
 	        		        gv.screenType = "learnSpellLevelUp";
 	        	        }
 	        	        else //no spells or traits to learn
 	        	        {
 	        	    	    gv.screenType = "party";
 						    gv.screenParty.traitGained += tr.name + ", ";
 						    gv.screenParty.doLevelUpSummary();
 	        	        }					
				    }
			    }
			    else
			    {
				    gv.sf.MessageBox("Can't learn that spell, try another or exit");
			    }
		    }	
        }
            
        public bool isAvailableToLearn(string spellTag)
        {
    	    if (traitsToLearnTagsList.Contains(spellTag))
    	    {
    		    return true;
    	    }
    	    return false;
        }    
        public void fillToLearnList()
        {
    	    traitsToLearnTagsList = pc.getTraitsToLearn(gv.mod);	    
        }    
        public Trait GetCurrentlySelectedTrait()
	    {
    	    TraitAllowed ta = pc.playerClass.traitsAllowed[traitSlotIndex];
		    return mod.getTraitByTag(ta.tag);
	    }
	    public bool isSelectedTraitSlotInKnownTraitsRange()
	    {
		    return traitSlotIndex < pc.playerClass.traitsAllowed.Count;
	    }	
	    public int getLevelAvailable(string tag)
	    {
		    TraitAllowed ta = pc.playerClass.getTraitAllowedByTag(tag);
		    if (ta != null)
		    {
			    return ta.atWhatLevelIsAvailable;
		    }
		    return 0;
	    }
	    public void tutorialMessageTraitScreen()
        {
		    gv.sf.MessageBoxHtml(this.stringMessageTraitLevelUp);	
        }
    }
}
