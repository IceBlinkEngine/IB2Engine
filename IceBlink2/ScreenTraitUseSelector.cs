using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenTraitUseSelector 
    {
	    //private gv.module gv.mod;
	    private GameView gv;
	
	    public int castingPlayerIndex = 0;
	    private int spellSlotIndex = 0;
	    private int slotsPerPage = 20;

        private int maxPages = 20;
        private int tknPageIndex = 0;

        private List<IbbButton> btnSpellSlots = new List<IbbButton>();

        private IbbButton btnTokensLeft = null;
        private IbbButton btnTokensRight = null;
        private IbbButton btnPageIndex = null;

        private IbbButton btnHelp = null;
	    private IbbButton btnSelect = null;
	    private IbbButton btnExit = null;
	    private string stringMessageCastSelector = "";
        private IbbHtmlTextBox description;
        public bool isInCombat = false;

        List<string> backupKnownOutsideCombatUsableTraitsTags = new List<string>();
        List<string> backupKnownInCombatUsableTraitsTags = new List<string>();
        //pc.knownOutsideCombatUsableTraitsTags
        //knownInCombatUsableTraitsTags

        public ScreenTraitUseSelector(Module m, GameView g) 
	    {
		    //gv.mod = m;
		    gv = g;

            //Maybe add own text for traits later or generalize text in MessageCastSelector
            //stringMessageCastSelector = gv.cc.loadTextToString("data/MessageCastSelector.txt");
	    }

        public void sortTraitsForLevelUp(Player pc)
        {
            //clear
            backupKnownOutsideCombatUsableTraitsTags.Clear();
            backupKnownInCombatUsableTraitsTags.Clear();
            
            List<string> traitsForLearningTags = new List<string>();
            List<TraitAllowed> traitsForLearning = new List<TraitAllowed>();

            if (!isInCombat)
            {
                TraitAllowed tempTA = new TraitAllowed();

                //add the known in battle useable traits
                foreach (string s in pc.knownOutsideCombatUsableTraitsTags)
                {
                    traitsForLearningTags.Add(s);
                }

                //traitsForLearningTags
                //sort the known in battle useable traits


                int levelCounter = 0;
                while (traitsForLearningTags.Count > 0)
                {
                    for (int i = traitsForLearningTags.Count - 1; i >= 0; i--)
                    {
                        foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                        {
                            if (ta.associatedSpellTag == traitsForLearningTags[i])
                            {
                                tempTA = ta.DeepCopy();
                                break;
                            }
                        }
                        if (levelCounter == tempTA.atWhatLevelIsAvailable)
                        {
                            backupKnownOutsideCombatUsableTraitsTags.Add(traitsForLearningTags[i]);
                            traitsForLearningTags.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }

            }
            //inside combat
            else
            {
                TraitAllowed tempTA = new TraitAllowed();

                //add the known in battle useable traits
                foreach (string s in pc.knownInCombatUsableTraitsTags)
                {
                    traitsForLearningTags.Add(s);
                }
             
                //traitsForLearningTags
                //sort the known in battle useable traits
               
                  
                int levelCounter = 0;
                while (traitsForLearningTags.Count > 0)
                {
                    for (int i = traitsForLearningTags.Count - 1; i >= 0; i--)
                    {
                        foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                        {
                            if (ta.associatedSpellTag == traitsForLearningTags[i])
                            {
                                tempTA = ta.DeepCopy();
                                break;
                            }
                        }
                        if (levelCounter == tempTA.atWhatLevelIsAvailable)
                        {
                            backupKnownInCombatUsableTraitsTags.Add(traitsForLearningTags[i]);
                            traitsForLearningTags.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }
            }
        }

        public void setControlsStart()
	    {			
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;

            //description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description = new IbbHtmlTextBox(gv, 3 * gv.squareSize + 2 * pW, 2 * gv.squareSize, gv.squareSize * 5, gv.squareSize * 6);
            description.showBoxBorder = false;

            //added
            if (btnTokensLeft == null)
            {
                btnTokensLeft = new IbbButton(gv, 1.0f);
                btnTokensLeft.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnTokensLeft.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
                btnTokensLeft.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnTokensLeft.X = (int)(5 * gv.squareSize) + (3 * pW);
                btnTokensLeft.Y = (2 * gv.squareSize);
                btnTokensLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTokensLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            //added
            if (btnPageIndex == null)
            {
                btnPageIndex = new IbbButton(gv, 1.0f);
                btnPageIndex.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
                btnPageIndex.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnPageIndex.Text = "1/20";
                btnPageIndex.X = (int)(6 * gv.squareSize) + (3 * pW);
                btnPageIndex.Y = (2 * gv.squareSize);
                btnPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            //added
            if (btnTokensRight == null)
            {
                btnTokensRight = new IbbButton(gv, 1.0f);
                btnTokensRight.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnTokensRight.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
                btnTokensRight.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnTokensRight.X = (int)(7f * gv.squareSize) + (3 * pW);
                btnTokensRight.Y = (2 * gv.squareSize);
                btnTokensRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTokensRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }

            if (btnSelect == null)
		    {
			    btnSelect = new IbbButton(gv, 0.8f);
                //add player class based label for "use" here, same in ScreenCastSelector later	
                
                btnSelect.Text = gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitAction.ToUpper() + " SELECTED " + gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.traitLabelSingular.ToUpper();
                
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
			    btnNew.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnNew.ImgOff = gv.cc.LoadBitmap("btn_small_off");
                btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    int x = y % 5;
			    int yy = y / 5;
			    btnNew.X = ((x+4) * gv.squareSize) + (padW * (x+1)) + gv.oXshift;
			    btnNew.Y = (3 + yy) * gv.squareSize + (padW * yy + padW);

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnSpellSlots.Add(btnNew);
		    }

            //DRAW ALL SPELL SLOTS:InCombat
            if (isInCombat)
            {
                int cntSlot = 0;
                foreach (IbbButton btn in btnSpellSlots)
                {
                    Player pc = getCastingPlayer();

                    if (cntSlot == spellSlotIndex) { btn.glowOn = true; }
                    else
                    {
                        btn.glowOn = false;
                    }

                    sortTraitsForLevelUp(pc);

                    //show only traits known by this pc and usable in combat
                    //if (cntSlot < pc.knownInCombatUsableTraitsTags.Count)
                    //{
                    if ((cntSlot + (tknPageIndex * slotsPerPage)) < backupKnownInCombatUsableTraitsTags.Count)
                    {
                        //TraitAllowed sa = pc.knownInCombatUsableTraitsTags[cntSlot];
                        //Trait sp = gv.mod.getTraitByTag(sa.tag);

                        //TraitAllowed sa = pc.knownInCombatUsableTraitsTags[cntSlot];

                        //the system assumes that traits have an associatedSpellTag property nd that these associted spell tags are written into knownInCombatUsableTraitsTags
                        Spell sp = gv.mod.getSpellByTag(backupKnownInCombatUsableTraitsTags[cntSlot + (tknPageIndex * slotsPerPage)]);
                        //TraitAllowed ta = backupTraitsAllowed[cntSlot + (tknPageIndex * slotsPerPage)];
                        //Trait tr = gv.mod.getTraitByTag(ta.tag);

                        //best sue same image for trait and spell, traitImage and spellImage should align then
                        gv.cc.DisposeOfBitmap(ref btn.Img2);
                        btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        gv.cc.DisposeOfBitmap(ref btn.Img2Off);
                        btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");

                        //if (pc.knownSpellsTags.Contains(sp.tag))
                        //{
                        //if (isInCombat) //all spells can be used in combat
                        //{
                            //btn.Img = gv.cc.LoadBitmap("btn_small");
                            btn.btnState = buttonState.Normal;
                        //}
                        //not in combat so check if spell can be used on adventure maps
                        //else if ((sp.useableInSituation.Equals("Always")) || (sp.useableInSituation.Equals("OutOfCombat")))
                        //{
                            //btn.Img = gv.cc.LoadBitmap("btn_small");
                            //btn.btnState = buttonState.Normal;
                            //btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        //}
                        //else //can't be used on adventure map
                        //{
                            //btn.btnState = buttonState.Off;
                            //btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                            //btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        //}
                        //}
                        //else //spell not known
                        //{
                        //btn.btnState = buttonState.Off;
                        //btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        //btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        //}
                    }
                    else //slot is not in spells allowed index range
                    {
                        btn.btnState = buttonState.Off;
                        btn.Img2 = null;
                        btn.Img2Off = null;
                    }
                    cntSlot++;
                }
            }

            //DRAW ALL SPELL SLOTS: Not InCombat
            else
            {
                int cntSlot = 0;
                foreach (IbbButton btn in btnSpellSlots)
                {
                    Player pc = getCastingPlayer();

                    if (cntSlot == spellSlotIndex) { btn.glowOn = true; }
                    else
                    {
                        btn.glowOn = false;
                    }

                    sortTraitsForLevelUp(pc);

                    //show only traits known by this pc and usable in combat
                    //if (cntSlot < pc.knownOutsideCombatUsableTraitsTags.Count)
                    //{
                    if ((cntSlot + (tknPageIndex * slotsPerPage)) < backupKnownOutsideCombatUsableTraitsTags.Count)
                    {
                        //TraitAllowed sa = pc.knownInCombatUsableTraitsTags[cntSlot];
                        //Trait sp = gv.mod.getTraitByTag(sa.tag);

                        //TraitAllowed sa = pc.knownInCombatUsableTraitsTags[cntSlot];
                        //Spell sp = gv.mod.getSpellByTag(pc.knownOutsideCombatUsableTraitsTags[cntSlot]);

                        //the system assumes that traits have an associatedSpellTag property nd that these associted spell tags are written into knownInCombatUsableTraitsTags
                        //Spell sp = gv.mod.getSpellByTag(pc.knownOutsideCombatUsableTraitsTags[cntSlot]);
                        Spell sp = gv.mod.getSpellByTag(backupKnownOutsideCombatUsableTraitsTags[cntSlot + (tknPageIndex * slotsPerPage)]);

                        btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");

                        //if (pc.knownSpellsTags.Contains(sp.tag))
                        //{
                        //if (isInCombat) //all spells can be used in combat
                        //{
                        //btn.Img = gv.cc.LoadBitmap("btn_small");
                        btn.btnState = buttonState.Normal;
                        //}
                        //not in combat so check if spell can be used on adventure maps
                        //else if ((sp.useableInSituation.Equals("Always")) || (sp.useableInSituation.Equals("OutOfCombat")))
                        //{
                        //btn.Img = gv.cc.LoadBitmap("btn_small");
                        //btn.btnState = buttonState.Normal;
                        //btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        //}
                        //else //can't be used on adventure map
                        //{
                        //btn.btnState = buttonState.Off;
                        //btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        //btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        //}
                        //}
                        //else //spell not known
                        //{
                        //btn.btnState = buttonState.Off;
                        //btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        //btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        //}
                    }
                    else //slot is not in spells allowed index range
                    {
                        btn.btnState = buttonState.Off;
                        btn.Img2 = null;
                        btn.Img2Off = null;
                    }
                    cntSlot++;
                }
            }

        }
	
	    //CAST SELECTOR SCREEN (COMBAT and MAIN)
        public void redrawTraitUseSelector(bool inCombat)
        {
            isInCombat = inCombat;
    	    //IF CONTROLS ARE NULL, CREATE THEM
    	    if (btnSelect == null)
    	    {
    		    setControlsStart();
    	    }
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
            //int textH = (int)gv.cc.MeasureString("GetHeight", gv.drawFontReg, gv.Width).Height;
            int textH = (int)gv.drawFontRegHeight;
            int spacing = textH; 
            //int spacing = (int)gv.mSheetTextPaint.getTextSize() + pH;
    	    int tabX = pW * 4;
    	    int noticeX = pW * 5;
    	    int noticeY = pH * 3 + spacing;
    	    int leftStartY = pH * 3;
    	    int tabStartY = 4 * gv.squareSize + pW * 10;

            //DRAW TEXT		
		    locY = (gv.squareSize * 0) + (pH * 2);
            //gv.mSheetTextPaint.setColor(Color.LTGRAY);
            //change labels, TODO
            //btnSelect.Text = gv.gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitAction + " SELECTED " + gv.gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitButtonInCombat;

            gv.DrawText("Select a " + gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.traitLabelSingular + " to " + gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitAction, noticeX, pH * 3);
		    //gv.mSheetTextPaint.setColor(Color.YELLOW);
		    gv.DrawText(getCastingPlayer().name + "  SP: " + getCastingPlayer().sp + "/" + getCastingPlayer().spMax, pW * 55, leftStartY);
            gv.DrawText(getCastingPlayer().name + "  HP: " + getCastingPlayer().hp + "/" + getCastingPlayer().hpMax, pW * 55, leftStartY + (int)(gv.squareSize/3));

            //DRAW NOTIFICATIONS
            //spellSlotIndex < getCastingPlayer().playerClass.spellsAllowed.Count;
            //if (cntSlot <  if (cntSlot < pc.knownOutsideCombatUsableTraitsTags.Count))
            //Spell sp = GetCurrentlySelectedSpell();
            Player pc = getCastingPlayer();

            //here insert
            sortTraitsForLevelUp(pc);

            //let's do in combat first
            if (isInCombat)
            {
                //+(tknPageIndex * slotsPerPage)
                //if (spellSlotIndex < pc.knownInCombatUsableTraitsTags.Count)
                if ((spellSlotIndex + (tknPageIndex * slotsPerPage)) < backupKnownInCombatUsableTraitsTags.Count)
                {
                    //Spell sp = gv.mod.getSpellByTag(pc.knownInCombatUsableTraitsTags[spellSlotIndex]);
                    Spell sp = gv.mod.getSpellByTag(backupKnownInCombatUsableTraitsTags[spellSlotIndex + (tknPageIndex * slotsPerPage)]);
                    //if (pc.knownSpellsTags.Contains(sp.tag))
                    //{
                    //if (inCombat) //all spells can be used in combat
                    //{
                    //if currently selected is usable say "Available to Cast" in lime

                    //enter check for workability based on entries in pcTags
                    bool traitWorksForThisPC = false;

                    // set up usablility lists for this spell (traitWorksOnlyWhen and traitWorksNeverWhen)
                    sp.traitWorksNeverWhen.Clear();
                    sp.traitWorksOnlyWhen.Clear();
                    //go through all trait tags of pc
                    foreach (string traitTag in pc.knownTraitsTags)
                    {
                        //go through all traits of gv.module
                        foreach (Trait t in gv.mod.moduleTraitsList)
                        {
                            //found a trait the pc has
                            if (t.tag.Equals(traitTag))
                            {
                                    //found out that our current spell is the associated spell of this trait
                                    if (t.associatedSpellTag.Equals(sp.tag))
                                    {
                                    //built the lists on runtime for our current spell from the trait's template
                                    foreach (LocalImmunityString ls in t.traitWorksOnlyWhen)
                                    {
                                        LocalImmunityString ls2 = ls.DeepCopy();
                                        sp.traitWorksOnlyWhen.Add(ls2);
                                    }

                                    foreach (LocalImmunityString ls in t.traitWorksNeverWhen)
                                    {
                                        LocalImmunityString ls2 = ls.DeepCopy();
                                        sp.traitWorksNeverWhen.Add(ls2);
                                    }
                                    //sp.traitWorksNeverWhen = t.traitWorksNeverWhen;
                                    //sp.traitWorksOnlyWhen = t.traitWorksOnlyWhen;
                                }
                            }
                        }
                    }


                    if (sp.traitWorksOnlyWhen.Count <= 0)
                    {
                        traitWorksForThisPC = true;
                    }

                    //note that the tratNeccessities are logically connected with OR the way it is setup
                    else
                        foreach (LocalImmunityString traitNeccessity in sp.traitWorksOnlyWhen)
                        {
                            foreach (string pcTag in pc.pcTags)
                            {
                                if (traitNeccessity.Value.Equals(pcTag))
                                {
                                    traitWorksForThisPC = true;
                                    break;
                                }
                            }
                        }

                    //one redFlag is enough to stop the trait from working, ie connected with OR, too
                    if (traitWorksForThisPC)
                    {
                        foreach (LocalImmunityString traitRedFlag in sp.traitWorksNeverWhen)
                        {
                            foreach (string pcTag in pc.pcTags)
                            {
                                if (traitRedFlag.Value.Equals(pcTag))
                                {
                                    traitWorksForThisPC = false;
                                    break;
                                }
                            }
                        }

                    }
 
                    if (traitWorksForThisPC)
                    {

                        bool swiftBlocked = false;
                        if (sp.isSwiftAction && gv.mod.swiftActionHasBeenUsedThisTurn)
                        {
                            swiftBlocked = true;
                        }

                        bool coolBlocked = false;
                        int coolDownTime = 0;
                        for (int i = 0; i < pc.coolingSpellsByTag.Count; i++)
                        {
                            if (pc.coolingSpellsByTag[i] == sp.tag)
                            {
                                coolBlocked = true;
                                coolDownTime = pc.coolDownTimes[i];
                                if (coolDownTime < sp.coolDownTime)
                                {
                                    coolDownTime++;
                                }
                            }
                        }

                        if (coolBlocked)
                        {
                            gv.DrawText("This is still cooling down for " + coolDownTime + " turn(s).", noticeX, noticeY, 1.0f, Color.Red);
                        }

                       else if (swiftBlocked)
                        {
                            gv.DrawText("Swift action already used this turn.", noticeX, noticeY, 1.0f, Color.Red);
                        }

                        else if ((pc.sp >= sp.costSP) && ((pc.hp - 1) >= sp.costHP) && !gv.mod.nonRepeatableFreeActionsUsedThisTurnBySpellTag.Contains(sp.tag))
                    {
                        //gv.mSheetTextPaint.setColor(Color.GREEN);
                        gv.DrawText("Available", noticeX, noticeY, 1.0f, Color.Lime);
                    }
                    else if (!gv.mod.nonRepeatableFreeActionsUsedThisTurnBySpellTag.Contains(sp.tag)) //if known but not enough spell points, "Insufficient SP to Cast" in yellow
                    {
                        //gv.mSheetTextPaint.setColor(Color.YELLOW);
                        gv.DrawText("Insufficient SP or HP", noticeX, noticeY, 1.0f, Color.Yellow);
                    }
                    else
                    {
                            gv.DrawText("This can only be used once per turn.", noticeX, noticeY, 1.0f, Color.Red);
                    }
                }
                    else
                    {
                        gv.DrawText("Specific requirements like e.g. worn equipment not met", noticeX, noticeY, 1.0f, Color.Red);
                    }

                   


                    //}
                    //not in combat so check if spell can be used on adventure maps
                    /*
                    else if ((sp.useableInSituation.Equals("Always")) || (sp.useableInSituation.Equals("OutOfCombat")))
                    {
                        //if currently selected is usable say "Available to Cast" in lime
                        if (pc.sp >= GetCurrentlySelectedSpell().costSP)
                        {
                            //gv.mSheetTextPaint.setColor(Color.GREEN);
                            gv.DrawText("Available to Cast", noticeX, noticeY, 1.0f, Color.Lime);
                        }
                        else //if known but not enough spell points, "Insufficient SP to Cast" in yellow
                        {
                            //gv.mSheetTextPaint.setColor(Color.YELLOW);
                            gv.DrawText("Insufficient SP", noticeX, noticeY, 1.0f, Color.Yellow);
                        }
                    }
                    else //can't be used on adventure map
                    {
                        //gv.mSheetTextPaint.setColor(Color.YELLOW);
                        gv.DrawText("Not Available Here", noticeX, noticeY, 1.0f, Color.Yellow);
                    }
                    */
                    //}
                    //else //spell not known
                    //{
                    //if unknown spell, "Spell Not Known Yet" in red
                    //gv.mSheetTextPaint.setColor(Color.RED);
                    //gv.DrawText(gv.mod.spellLabelSingular + " Not Known Yet", noticeX, noticeY, 1.0f, Color.Red);
                    //}
                }
            }

            //and now outside combat
            //copy from above
            else
            {
                //if (spellSlotIndex < pc.knownOutsideCombatUsableTraitsTags.Count)
                //{
                if ((spellSlotIndex + (tknPageIndex * slotsPerPage)) < backupKnownOutsideCombatUsableTraitsTags.Count)
                {
                    //Spell sp = gv.mod.getSpellByTag(pc.knownOutsideCombatUsableTraitsTags[spellSlotIndex]);
                    Spell sp = gv.mod.getSpellByTag(backupKnownOutsideCombatUsableTraitsTags[spellSlotIndex + (tknPageIndex * slotsPerPage)]);
                    //if (pc.knownSpellsTags.Contains(sp.tag))
                    //{
                    //if (inCombat) //all spells can be used in combat
                    //{
                    //if currently selected is usable say "Available to Cast" in lime

                    //enter check for workability based on entries in pcTags
                    bool traitWorksForThisPC = false;
                    // set up usablility lists for this spell (traitWorksOnlyWhen and traitWorksNeverWhen)
                    sp.traitWorksNeverWhen.Clear();
                    sp.traitWorksOnlyWhen.Clear();
                    //go through all trait tags of pc
                    foreach (string traitTag in pc.knownTraitsTags)
                    {
                        //go through all traits of gv.module
                        foreach (Trait t in gv.mod.moduleTraitsList)
                        {
                            //found a trait the pc has
                            if (t.tag.Equals(traitTag))
                            {
                                //found out that our current spell is the associated spell of this trait
                                if (t.associatedSpellTag.Equals(sp.tag))
                                {
                                    //built the lists on runtime for our current spell from the trait's template
                                    foreach (LocalImmunityString ls in t.traitWorksOnlyWhen)
                                    {
                                        LocalImmunityString ls2 = ls.DeepCopy();
                                        sp.traitWorksOnlyWhen.Add(ls2);
                                    }

                                    foreach (LocalImmunityString ls in t.traitWorksNeverWhen)
                                    {
                                        LocalImmunityString ls2 = ls.DeepCopy();
                                        sp.traitWorksNeverWhen.Add(ls2);
                                    }
                                    //sp.traitWorksNeverWhen = t.traitWorksNeverWhen;
                                    //sp.traitWorksOnlyWhen = t.traitWorksOnlyWhen;
                                }
                            }
                        }
                    }

                    if (sp.traitWorksOnlyWhen.Count <= 0)
                    {
                        traitWorksForThisPC = true;
                    }

                    //note that the tratNeccessities are logically connected with OR the way it is setup
                    else
                        foreach (LocalImmunityString traitNeccessity in sp.traitWorksOnlyWhen)
                        {
                            foreach (string pcTag in pc.pcTags)
                            {
                                if (traitNeccessity.Value.Equals(pcTag))
                                {
                                    traitWorksForThisPC = true;
                                    break;
                                }
                            }
                        }

                    //one redFlag is enough to stop the trait from working, ie connected with OR, too
                    if (traitWorksForThisPC)
                    {
                        foreach (LocalImmunityString traitRedFlag in sp.traitWorksNeverWhen)
                        {
                            foreach (string pcTag in pc.pcTags)
                            {
                                if (traitRedFlag.Value.Equals(pcTag))
                                {
                                    traitWorksForThisPC = false;
                                    break;
                                }
                            }
                        }

                    }

                    //eventually add damge bonus or multiplier for attacks from behind 
                    //eventually add max dex bonus allowed when wearing armor
                    if (traitWorksForThisPC)
                    {
                        if ((pc.sp >= sp.costSP) && ((pc.hp - 1) >= sp.costHP))
                    {
                        //gv.mSheetTextPaint.setColor(Color.GREEN);
                        gv.DrawText("Available", noticeX, noticeY, 1.0f, Color.Lime);
                    }
                    else //if known but not enough spell points, "Insufficient SP to Cast" in yellow
                    {
                        //gv.mSheetTextPaint.setColor(Color.YELLOW);
                        gv.DrawText("Insufficient SP or HP", noticeX, noticeY, 1.0f, Color.Yellow);
                    }
                }
                    else
                    {
                        gv.DrawText("Specific requirements like e.g. worn equipment not met", noticeX, noticeY, 1.0f, Color.Yellow);
                    }
                    //}
                    //not in combat so check if spell can be used on adventure maps
                    /*
                    else if ((sp.useableInSituation.Equals("Always")) || (sp.useableInSituation.Equals("OutOfCombat")))
                    {
                        //if currently selected is usable say "Available to Cast" in lime
                        if (pc.sp >= GetCurrentlySelectedSpell().costSP)
                        {
                            //gv.mSheetTextPaint.setColor(Color.GREEN);
                            gv.DrawText("Available to Cast", noticeX, noticeY, 1.0f, Color.Lime);
                        }
                        else //if known but not enough spell points, "Insufficient SP to Cast" in yellow
                        {
                            //gv.mSheetTextPaint.setColor(Color.YELLOW);
                            gv.DrawText("Insufficient SP", noticeX, noticeY, 1.0f, Color.Yellow);
                        }
                    }
                    else //can't be used on adventure map
                    {
                        //gv.mSheetTextPaint.setColor(Color.YELLOW);
                        gv.DrawText("Not Available Here", noticeX, noticeY, 1.0f, Color.Yellow);
                    }
                    */
                    //}
                    //else //spell not known
                    //{
                    //if unknown spell, "Spell Not Known Yet" in red
                    //gv.mSheetTextPaint.setColor(Color.RED);
                    //gv.DrawText(gv.mod.spellLabelSingular + " Not Known Yet", noticeX, noticeY, 1.0f, Color.Red);
                    //}
                }
            }

		    //DRAW ALL SPELL SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnSpellSlots)
		    {			
			    //Player pc = getCastingPlayer();						
			
			    if (cntSlot == spellSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}

                //code for turning buttons off
                //purpose
                if (isInCombat)
                {
                    if ((cntSlot + (tknPageIndex * slotsPerPage)) < backupKnownInCombatUsableTraitsTags.Count)
                    {
                        Spell sp = gv.mod.getSpellByTag(backupKnownInCombatUsableTraitsTags[cntSlot + (tknPageIndex * slotsPerPage)]);
                        //TraitAllowed ta = backupTraitsAllowed[cntSlot + (tknPageIndex * slotsPerPage)];
                        gv.cc.DisposeOfBitmap(ref btn.Img2);
                        btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        gv.cc.DisposeOfBitmap(ref btn.Img2Off);
                        btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        btn.btnState = buttonState.Normal;
                    }
                    else
                    {
                        gv.cc.DisposeOfBitmap(ref btn.Img);
                        btn.Img = gv.cc.LoadBitmap("btn_small_off");
                        btn.Img2 = null;
                        btn.Img2Off = null;
                        btn.Img3 = null;
                        btn.btnState = buttonState.Off;
                    }
                }

                //outside combat
                else
                {
                    if ((cntSlot + (tknPageIndex * slotsPerPage)) < backupKnownOutsideCombatUsableTraitsTags.Count)
                    {
                        Spell sp = gv.mod.getSpellByTag(backupKnownOutsideCombatUsableTraitsTags[cntSlot + (tknPageIndex * slotsPerPage)]);
                        //TraitAllowed ta = backupTraitsAllowed[cntSlot + (tknPageIndex * slotsPerPage)];
                        gv.cc.DisposeOfBitmap(ref btn.Img2);
                        btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                        gv.cc.DisposeOfBitmap(ref btn.Img2Off);
                        btn.Img2Off = gv.cc.LoadBitmap(sp.spellImage + "_off");
                        btn.btnState = buttonState.Normal;
                    }
                    else
                    {
                        gv.cc.DisposeOfBitmap(ref btn.Img);
                        btn.Img = gv.cc.LoadBitmap("btn_small_off");
                        btn.Img2 = null;
                        btn.Img2Off = null;
                        btn.Img3 = null;
                        btn.btnState = buttonState.Off;
                    }
                }
			    		
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;
            //adjust like above, differentiating between combat/adventure map		
            //if (isSelectedSpellSlotInKnownSpellsRange())
            //{

            //Spell sp = GetCurrentlySelectedSpell();
            if (isInCombat)
            {
                //sortTraitsForLevelUp(pc);
                if ((spellSlotIndex + (tknPageIndex * slotsPerPage)) < backupKnownInCombatUsableTraitsTags.Count)
                {
                    //newSpellTag_10948
                    Spell sp = gv.mod.getSpellByTag(backupKnownInCombatUsableTraitsTags[spellSlotIndex + (tknPageIndex * slotsPerPage)]);
                    //SpellAllowed sa = getCastingPlayer().playerClass.getSpellAllowedByTag(sp.tag);
                    //string textToSpan = "<u>Description</u>" + "<BR>";
                    string textToSpan = "<b><big>" + sp.name + "</big></b><BR>";
                    if (sp.isSwiftAction && !sp.usesTurnToActivate)
                    {
                        textToSpan += "Swift action" + "<BR>";
                    }
                    else if (sp.onlyOncePerTurn && !sp.usesTurnToActivate)
                    {
                        textToSpan += "Free action, not repeatable" + "<BR>";
                    }
                    else if (!sp.onlyOncePerTurn && !sp.usesTurnToActivate)
                    {
                        textToSpan += "Free action, repeatable" + "<BR>";
                    }
                    else if (sp.castTimeInTurns > 0)
                    {
                        textToSpan += "Takes " + sp.castTimeInTurns + " full turn(s)" + "<BR>";
                    }
                    if (sp.coolDownTime > 0)
                    {
                        textToSpan += "Cool down time: " + sp.coolDownTime + " turn(s)" + "<BR>";
                    }
                    textToSpan += "SP Cost: " + sp.costSP + "<BR>";
                    textToSpan += "HP Cost: " + sp.costHP + "<BR>";
                    textToSpan += "Target Range: " + sp.range + "<BR>";
                    textToSpan += "Area of Effect Radius: " + sp.aoeRadius + "<BR>";
                    if (sp.spellTargetType.Contains("Enemy"))
                    {
                        textToSpan += "Affects only enemies" + "<BR>";
                    }
                    if (sp.spellTargetType.Contains("Friend"))
                    {
                        textToSpan += "Affects only friends" + "<BR>";
                    }
                    if (sp.spellTargetType.Contains("PointLocation"))
                    {
                        textToSpan += "Affects all targets in area" + "<BR>";
                    }
                    if (sp.spellTargetType.Contains("Self"))
                    {
                        textToSpan += "Affects only self" + "<BR>";
                    }
                    //SpellAllowed sa = getCastingPlayer().playerClass.getSpellAllowedByTag(tag);
                    //if (sa != null)
                    //{
                        //return sa.atWhatLevelIsAvailable;
                    //}

                    //textToSpan += "Available at Level: " + getLevelAvailable(sp.tag) + "<BR>";
                    textToSpan += "<BR>";
                    textToSpan += sp.description;

                    description.tbXloc = 11 * gv.squareSize;
                    description.tbYloc = 2 * gv.squareSize;
                    description.tbWidth = pW * 40;
                    description.tbHeight = pH * 80;
                    description.logLinesList.Clear();
                    description.AddHtmlTextToLog(textToSpan);
                    description.onDrawLogBox();
                }
            }
            else
            {
                //sortTraitsForLevelUp(pc);
                if ((spellSlotIndex + (tknPageIndex * slotsPerPage)) < backupKnownOutsideCombatUsableTraitsTags.Count)
                {
                    Spell sp = gv.mod.getSpellByTag(backupKnownOutsideCombatUsableTraitsTags[spellSlotIndex + (tknPageIndex * slotsPerPage)]);
                    string textToSpan = "<u>Description</u>" + "<BR>";
                    textToSpan += "<b><i><big>" + sp.name + "</big></i></b><BR>";
                    if (sp.isSwiftAction && !sp.usesTurnToActivate)
                    {
                        textToSpan += "Swift action" + "<BR>";
                    }
                    else if (sp.onlyOncePerTurn && !sp.usesTurnToActivate)
                    {
                        textToSpan += "Free action, not repeatable" + "<BR>";
                    }
                    else if (!sp.onlyOncePerTurn && !sp.usesTurnToActivate)
                    {
                        textToSpan += "Free action, repeatable" + "<BR>";
                    }
                    else if (sp.castTimeInTurns > 0)
                    {
                        textToSpan += "Takes " + sp.castTimeInTurns + " full turn(s)" + "<BR>";
                    }
                    if (sp.coolDownTime > 0)
                    {
                        textToSpan += "Cool down time: " + sp.coolDownTime + " turn(s)" + "<BR>";
                    }
                    textToSpan += "SP Cost: " + sp.costSP + "<BR>";
                    textToSpan += "HP Cost: " + sp.costHP + "<BR>";
                    textToSpan += "Target Range: " + sp.range + "<BR>";
                    textToSpan += "Area of Effect Radius: " + sp.aoeRadius + "<BR>";
                    //textToSpan += "Available at Level: " + getLevelAvailable(sp.tag) + "<BR>";
                    textToSpan += "<BR>";
                    textToSpan += sp.description;

                    description.tbXloc = 11 * gv.squareSize;
                    description.tbYloc = 2 * gv.squareSize;
                    description.tbWidth = pW * 40;
                    description.tbHeight = pH * 80;
                    description.logLinesList.Clear();
                    description.AddHtmlTextToLog(textToSpan);
                    description.onDrawLogBox();
                }
            }

            btnHelp.Draw();	
		    btnExit.Draw();	
		    btnSelect.Draw();
            btnTokensLeft.Draw();
            btnTokensRight.Draw();
            btnPageIndex.Draw();
        }
        public void onTouchCastSelector(MouseEventArgs e, MouseEventType.EventType eventType, bool inCombat)
	    {
            try
            {
                btnHelp.glowOn = false;
                btnExit.glowOn = false;
                btnSelect.glowOn = false;
                btnTokensLeft.glowOn = false;
                btnTokensRight.glowOn = false;
                btnPageIndex.glowOn = false;

                switch (eventType)
                {
                    case MouseEventType.EventType.MouseDown:
                    case MouseEventType.EventType.MouseMove:
                        int x = (int)e.X;
                        int y = (int)e.Y;
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
                        else if (btnTokensLeft.getImpact(x, y))
                        {
                            btnTokensLeft.glowOn = true;
                        }
                        else if (btnTokensRight.getImpact(x, y))
                        {
                            btnTokensRight.glowOn = true;
                        }
                        else if (btnPageIndex.getImpact(x, y))
                        {
                            btnPageIndex.glowOn = true;
                        }
                        break;

                    case MouseEventType.EventType.MouseUp:
                        x = (int)e.X;
                        y = (int)e.Y;

                        btnHelp.glowOn = false;
                        //btnInfo.glowOn = false;
                        btnExit.glowOn = false;
                        btnSelect.glowOn = false;
                        btnTokensLeft.glowOn = false;
                        btnTokensRight.glowOn = false;
                        btnPageIndex.glowOn = false;

                        for (int j = 0; j < slotsPerPage; j++)
                        {
                            if (btnSpellSlots[j].getImpact(x, y))
                            {
                                spellSlotIndex = j;
                            }
                        }
                        if (btnTokensLeft.getImpact(x, y))
                        {
                            if (tknPageIndex > 0)
                            {
                                tknPageIndex--;
                                btnPageIndex.Text = (tknPageIndex + 1) + "/" + maxPages;
                            }
                        }
                        else if (btnTokensRight.getImpact(x, y))
                        {
                            if (tknPageIndex < maxPages)
                            {
                                tknPageIndex++;
                                btnPageIndex.Text = (tknPageIndex + 1) + "/" + maxPages;
                            }
                        }


                        else if (btnHelp.getImpact(x, y))
                        {
                            tutorialMessageCastingScreen();
                        }
                        else if (btnSelect.getImpact(x, y))
                        {
                            //check later whether this is properly addjusted
                            doSelectedSpell(inCombat);
                        }
                        else if (btnExit.getImpact(x, y))
                        {
                            if (inCombat)
                            {
                                if (gv.screenCombat.canMove)
                                {
                                    gv.screenCombat.currentCombatMode = "move";
                                }
                                else
                                {
                                    gv.screenCombat.currentCombatMode = "attack";
                                }
                                gv.screenType = "combat";
                                doCleanUp();
                            }
                            else
                            {
                                gv.screenType = "main";
                                doCleanUp();
                            }
                        }
                        break;
                }
            }
            catch
            { }
	    }
    
        public void doCleanUp()
	    {
    	    btnSpellSlots.Clear();
    	    btnHelp = null;
    	    btnSelect = null;
    	    btnExit = null;
            btnTokensLeft = null;
            btnTokensRight = null;
            btnPageIndex = null;
                
	    }

        public void doSpellTarget(Player pc, Player target)
        {
            try
            {
                gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, pc, target, !isInCombat, true);
                gv.screenType = "main";
                doCleanUp();
            }
            catch (Exception ex)
            {
                gv.sf.MessageBoxHtml("error with Pc Selector screen: " + ex.ToString());
                gv.errorLog(ex.ToString());
            }
        }

        public void doSelectedSpell(bool inCombat)
	    {
            //if (isSelectedSpellSlotInKnownSpellsRange())
            //{
            //only allow to cast spells that you know and are usable on this map
            //if (getCastingPlayer().knownSpellsTags.Contains(GetCurrentlySelectedSpell().tag))
            //{
            //if (inCombat) //Combat Map
            //{
            Player pc = getCastingPlayer();

            if (inCombat)
            {
                if ((spellSlotIndex + (tknPageIndex * slotsPerPage)) < backupKnownInCombatUsableTraitsTags.Count)
                {
                    Spell sp = gv.mod.getSpellByTag(backupKnownInCombatUsableTraitsTags[spellSlotIndex + (tknPageIndex * slotsPerPage)]);

                    //enter check for workability based on entries in pcTags
                    bool traitWorksForThisPC = false;

                    // set up usablility lists for this spell (traitWorksOnlyWhen and traitWorksNeverWhen)
                    sp.traitWorksNeverWhen.Clear();
                    sp.traitWorksOnlyWhen.Clear();
                    //go through all trait tags of pc
                    foreach (string traitTag in pc.knownTraitsTags)
                    {
                        //go through all traits of gv.module
                        foreach (Trait t in gv.mod.moduleTraitsList)
                        {
                            //found a trait the pc has
                            if (t.tag.Equals(traitTag))
                            {
                                //found out that our current spell is the associated spell of this trait
                                if (t.associatedSpellTag.Equals(sp.tag))
                                {
                                    //built the lists on runtime for our current spell from the trait's template
                                    foreach (LocalImmunityString ls in t.traitWorksOnlyWhen)
                                    {
                                        LocalImmunityString ls2 = ls.DeepCopy();
                                        sp.traitWorksOnlyWhen.Add(ls2);
                                    }

                                    foreach (LocalImmunityString ls in t.traitWorksNeverWhen)
                                    {
                                        LocalImmunityString ls2 = ls.DeepCopy();
                                        sp.traitWorksNeverWhen.Add(ls2);
                                    }
                                    //sp.traitWorksNeverWhen = t.traitWorksNeverWhen;
                                    //sp.traitWorksOnlyWhen = t.traitWorksOnlyWhen;
                                }
                            }
                        }
                    }

                    if (sp.traitWorksOnlyWhen.Count <= 0)
                    {
                        traitWorksForThisPC = true;
                    }

                    //note that the tratNeccessities are logically connected with OR the way it is setup
                    else
                        foreach (LocalImmunityString traitNeccessity in sp.traitWorksOnlyWhen)
                        {
                            foreach (string pcTag in pc.pcTags)
                            {
                                if (traitNeccessity.Value.Equals(pcTag))
                                {
                                    traitWorksForThisPC = true;
                                    break;
                                }
                            }
                        }

                    //one redFlag is enough to stop the trait from working, ie connected with OR, too
                    if (traitWorksForThisPC)
                    {
                        foreach (LocalImmunityString traitRedFlag in sp.traitWorksNeverWhen)
                        {
                            foreach (string pcTag in pc.pcTags)
                            {
                                if (traitRedFlag.Value.Equals(pcTag))
                                {
                                    traitWorksForThisPC = false;
                                    break;
                                }
                            }
                        }

                    }

                    //eventually add damge bonus or multiplier for attacks from behind 
                    //eventually add max dex bonus allowed when wearing armor
                    if (traitWorksForThisPC)
                    {
                        bool swiftBlocked = false;
                        if (sp.isSwiftAction && gv.mod.swiftActionHasBeenUsedThisTurn)
                        {
                            swiftBlocked = true;
                        }

                        bool coolBlocked = false;
                        for (int i = 0; i < pc.coolingSpellsByTag.Count; i++)
                        {
                            if (pc.coolingSpellsByTag[i] == GetCurrentlySelectedSpell().tag)
                            {
                                coolBlocked = true;
                            }
                        }

                        if ((pc.sp >= sp.costSP) && (pc.hp > sp.costHP) && (!gv.mod.nonRepeatableFreeActionsUsedThisTurnBySpellTag.Contains(sp.tag)) && !swiftBlocked && !coolBlocked)
                        {
                            /*
                            if (sp.onlyOncePerTurn)
                            {
                                gv.mod.nonRepeatableFreeActionsUsedThisTurnBySpellTag.Add(sp.tag);
                            }
                            if (sp.isSwiftAction)
                            {
                                gv.mod.swiftActionHasBeenUsedThisTurn = true;
                            }
                            if (sp.coolDownTime > 0)
                            {
                                pc.coolingSpellsByTag.Add(sp.tag);
                                pc.coolDownTimes.Add(sp.coolDownTime);
                            }
                            */

                            gv.cc.currentSelectedSpell = sp;
                            gv.screenType = "combat";
                            gv.screenCombat.currentCombatMode = "cast";
                            doCleanUp();
                        }
                        else
                        {
                            //Toast.makeText(gv.gameContext, "Not Enough SP for that spell", Toast.LENGTH_SHORT).show();
                        }
                    }
                }
            }

            else //Adventure Map
            {
                //only cast if useable on adventure maps
                //if ((GetCurrentlySelectedSpell().useableInSituation.Equals("Always")) || (GetCurrentlySelectedSpell().useableInSituation.Equals("OutOfCombat")))
                //{
                if (spellSlotIndex + (tknPageIndex * slotsPerPage) < backupKnownOutsideCombatUsableTraitsTags.Count)
                {
                    Spell sp = gv.mod.getSpellByTag(backupKnownOutsideCombatUsableTraitsTags[spellSlotIndex + (tknPageIndex * slotsPerPage)]);

                    //pcTags
                    //enter check for workability based on entries in pcTags
                    bool traitWorksForThisPC = false;
                    // set up usablility lists for this spell (traitWorksOnlyWhen and traitWorksNeverWhen)
                    sp.traitWorksNeverWhen.Clear();
                    sp.traitWorksOnlyWhen.Clear();
                    //go through all trait tags of pc
                    foreach (string traitTag in pc.knownTraitsTags)
                    {
                        //go through all traits of gv.module
                        foreach (Trait t in gv.mod.moduleTraitsList)
                        {
                            //found a trait the pc has
                            if (t.tag.Equals(traitTag))
                            {
                                //found out that our current spell is the associated spell of this trait
                                if (t.associatedSpellTag.Equals(sp.tag))
                                {
                                    //built the lists on runtime for our current spell from the trait's template
                                    foreach (LocalImmunityString ls in t.traitWorksOnlyWhen)
                                    {
                                        LocalImmunityString ls2 = ls.DeepCopy();
                                        sp.traitWorksOnlyWhen.Add(ls2);
                                    }

                                    foreach (LocalImmunityString ls in t.traitWorksNeverWhen)
                                    {
                                        LocalImmunityString ls2 = ls.DeepCopy();
                                        sp.traitWorksNeverWhen.Add(ls2);
                                    }
                                    //sp.traitWorksNeverWhen = t.traitWorksNeverWhen;
                                    //sp.traitWorksOnlyWhen = t.traitWorksOnlyWhen;
                                }
                            }
                        }
                    }

                    if (sp.traitWorksOnlyWhen.Count <= 0)
                    {
                        traitWorksForThisPC = true;
                    }

                    //note that the tratNeccessities are logically connected with OR the way it is setup
                    else
                        foreach (LocalImmunityString traitNeccessity in sp.traitWorksOnlyWhen)
                        {
                            foreach (string pcTag in pc.pcTags)
                            {
                                if (traitNeccessity.Value.Equals(pcTag))
                                {
                                    traitWorksForThisPC = true;
                                    break;
                                }
                            }
                        }

                    //one redFlag is enough to stop the trait from working, ie connected with OR, too
                    if (traitWorksForThisPC)
                    {
                        foreach (LocalImmunityString traitRedFlag in sp.traitWorksNeverWhen)
                        {
                            foreach (string pcTag in pc.pcTags)
                            {
                                if (traitRedFlag.Value.Equals(pcTag))
                                {
                                    traitWorksForThisPC = false;
                                    break;
                                }
                            }
                        }
                    }

                    //eventually add damge bonus or multiplier for attacks from behind 
                    //eventually add max dex bonus allowed when wearing armor
                    if (traitWorksForThisPC)
                    {
                        if ((pc.sp >= sp.costSP) && (pc.hp > sp.costHP))
                        {
                            gv.cc.currentSelectedSpell = sp;


                            //if target is SELF then just do doSpellTarget(self) 
                            if (gv.cc.currentSelectedSpell.spellTargetType.Equals("Self"))
                            {
                                doSpellTarget(getCastingPlayer(), getCastingPlayer());
                            }

                            //********************************************
                            else
                            {

                                //ask for target
                                // selected to USE ITEM

                                List<string> pcNames = new List<string>();
                                pcNames.Add("cancel");
                                foreach (Player p in gv.mod.playerList)
                                {
                                    pcNames.Add(p.name);
                                }

                                //If only one PC, do not show select PC dialog...just go to cast selector
                                if (gv.mod.playerList.Count == 1)
                                {
                                    try
                                    {
                                        Player target = gv.mod.playerList[0];
                                        gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, target, target, inCombat, true);
                                        gv.screenType = "main";
                                        doCleanUp();
                                        return;
                                    }
                                    catch (Exception ex)
                                    {
                                        gv.errorLog(ex.ToString());
                                    }
                                }

                                using (ItemListSelector pcSel = new ItemListSelector(gv, pcNames, gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.traitLabelSingular + " Target"))
                                {
                                    pcSel.ShowDialog();
                                    if (pcSel.selectedIndex > 0)
                                    {
                                        try
                                        {
                                            Player target = gv.mod.playerList[pcSel.selectedIndex - 1];
                                            gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, pc, target, inCombat, true);
                                            gv.screenType = "main";
                                            doCleanUp();
                                        }
                                        catch (Exception ex)
                                        {
                                            IBMessageBox.Show(gv, "error with Pc Selector screen: " + ex.ToString());
                                            gv.errorLog(ex.ToString());
                                        }
                                    }
                                    else if (pcSel.selectedIndex == 0) // selected "cancel"
                                    {
                                        //do nothing
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Toast.makeText(gv.gameContext, "Not Enough SP for that spell", Toast.LENGTH_SHORT).show();
                        }
                    }
                }
            }
		}

        //not used
        public Spell GetCurrentlySelectedSpell()
	    {
    	    SpellAllowed sa = getCastingPlayer().playerClass.spellsAllowed[spellSlotIndex];
		    return gv.mod.getSpellByTag(sa.tag);
	    }

        //no used
	    public bool isSelectedSpellSlotInKnownSpellsRange()
	    {
		    return spellSlotIndex < getCastingPlayer().playerClass.spellsAllowed.Count;
	    }	

	    public int getLevelAvailable(String tag)
	    {
		    SpellAllowed sa = getCastingPlayer().playerClass.getSpellAllowedByTag(tag);
		    if (sa != null)
		    {
			    return sa.atWhatLevelIsAvailable;
		    }
		    return 0;
	    }
	    public Player getCastingPlayer()
	    {
		    return gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex];
	    }
	    public void tutorialMessageCastingScreen()
        {
		    gv.sf.MessageBoxHtml(this.stringMessageCastSelector);	
        }
    }
}
