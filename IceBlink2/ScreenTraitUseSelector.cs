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
	    private Module mod;
	    private GameView gv;
	
	    public int castingPlayerIndex = 0;
	    private int spellSlotIndex = 0;
	    private int slotsPerPage = 20;
	    private List<IbbButton> btnSpellSlots = new List<IbbButton>();
	    private IbbButton btnHelp = null;
	    private IbbButton btnSelect = null;
	    private IbbButton btnExit = null;
	    private string stringMessageCastSelector = "";
        private IbbHtmlTextBox description;
        public bool isInCombat = false;
	
	    public ScreenTraitUseSelector(Module m, GameView g) 
	    {
		    mod = m;
		    gv = g;

            //Maybe add own text for traits later or generalize text in MessageCastSelector
            //stringMessageCastSelector = gv.cc.loadTextToString("data/MessageCastSelector.txt");
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
                //add player class based label for "use" here, same in ScreenCastSelector later	
                
                btnSelect.Text = gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitAction + " SELECTED " + gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitButtoninCombat;
                
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
			    btnNew.Y = (1 + yy) * gv.squareSize + (padW * yy);

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

                    //show only traits known by this pc and usable in combat
                    if (cntSlot < pc.knownInCombatUsableTraitsTags.Count)
                    {
                        //TraitAllowed sa = pc.knownInCombatUsableTraitsTags[cntSlot];
                        //Trait sp = mod.getTraitByTag(sa.tag);

                        //TraitAllowed sa = pc.knownInCombatUsableTraitsTags[cntSlot];

                        //the system assumes that traits have an associatedSpellTag property nd that these associted spell tags are written into knownInCombatUsableTraitsTags
                        Spell sp = mod.getSpellByTag(pc.knownInCombatUsableTraitsTags[cntSlot]);

                        //best sue same image for trait and spell, traitImage and spellImage should align then
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

                    //show only traits known by this pc and usable in combat
                    if (cntSlot < pc.knownOutsideCombatUsableTraitsTags.Count)
                    {
                        //TraitAllowed sa = pc.knownInCombatUsableTraitsTags[cntSlot];
                        //Trait sp = mod.getTraitByTag(sa.tag);

                        //TraitAllowed sa = pc.knownInCombatUsableTraitsTags[cntSlot];
                        //Spell sp = mod.getSpellByTag(pc.knownOutsideCombatUsableTraitsTags[cntSlot]);
                        
                        //the system assumes that traits have an associatedSpellTag property nd that these associted spell tags are written into knownInCombatUsableTraitsTags
                        Spell sp = mod.getSpellByTag(pc.knownOutsideCombatUsableTraitsTags[cntSlot]);

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
            //btnSelect.Text = gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitAction + " SELECTED " + gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitButtoninCombat;

            gv.DrawText("Select a " + gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitButtoninCombat.ToLower() + " to " + gv.mod.playerList[gv.screenCastSelector.castingPlayerIndex].playerClass.labelForUseTraitAction.ToLower(), noticeX, pH * 3);
		    //gv.mSheetTextPaint.setColor(Color.YELLOW);
		    gv.DrawText(getCastingPlayer().name + " SP: " + getCastingPlayer().sp + "/" + getCastingPlayer().spMax, pW * 55, leftStartY);
            gv.DrawText(getCastingPlayer().name + " HP: " + getCastingPlayer().hp + "/" + getCastingPlayer().hpMax, pW * 55, leftStartY + (int)(gv.squareSize/3));

            //DRAW NOTIFICATIONS
            //spellSlotIndex < getCastingPlayer().playerClass.spellsAllowed.Count;
            //if (cntSlot <  if (cntSlot < pc.knownOutsideCombatUsableTraitsTags.Count))
            //Spell sp = GetCurrentlySelectedSpell();
            Player pc = getCastingPlayer();

            //let's do in combat first
            if (isInCombat)
            {
                if (spellSlotIndex < pc.knownInCombatUsableTraitsTags.Count)
                {
                    Spell sp = mod.getSpellByTag(pc.knownInCombatUsableTraitsTags[spellSlotIndex]);
                    //if (pc.knownSpellsTags.Contains(sp.tag))
                    //{
                        //if (inCombat) //all spells can be used in combat
                        //{
                            //if currently selected is usable say "Available to Cast" in lime
                            if ((pc.sp >= GetCurrentlySelectedSpell().costSP) && ((pc.hp-1) >= GetCurrentlySelectedSpell().costHP))
                            {
                                //gv.mSheetTextPaint.setColor(Color.GREEN);
                                gv.DrawText("Available", noticeX, noticeY, 1.0f, Color.Lime);
                            }
                            else //if known but not enough spell points, "Insufficient SP to Cast" in yellow
                            {
                                //gv.mSheetTextPaint.setColor(Color.YELLOW);
                                gv.DrawText("Insufficient SP or HP", noticeX, noticeY, 1.0f, Color.Yellow);
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
                        //gv.DrawText(mod.spellLabelSingular + " Not Known Yet", noticeX, noticeY, 1.0f, Color.Red);
                    //}
                }
            }

            //and now outside combat
            //copy from above
            else
            {
                if (spellSlotIndex < pc.knownOutsideCombatUsableTraitsTags.Count)
                {
                    Spell sp = mod.getSpellByTag(pc.knownOutsideCombatUsableTraitsTags[spellSlotIndex]);
                    //if (pc.knownSpellsTags.Contains(sp.tag))
                    //{
                    //if (inCombat) //all spells can be used in combat
                    //{
                    //if currently selected is usable say "Available to Cast" in lime
                    if ((pc.sp >= GetCurrentlySelectedSpell().costSP) && ((pc.hp-1) >= GetCurrentlySelectedSpell().costHP))
                    {
                        //gv.mSheetTextPaint.setColor(Color.GREEN);
                        gv.DrawText("Available", noticeX, noticeY, 1.0f, Color.Lime);
                    }
                    else //if known but not enough spell points, "Insufficient SP to Cast" in yellow
                    {
                        //gv.mSheetTextPaint.setColor(Color.YELLOW);
                        gv.DrawText("Insufficient SP or HP", noticeX, noticeY, 1.0f, Color.Yellow);
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
                    //gv.DrawText(mod.spellLabelSingular + " Not Known Yet", noticeX, noticeY, 1.0f, Color.Red);
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
                if (spellSlotIndex < pc.knownInCombatUsableTraitsTags.Count)
                {
                    Spell sp = mod.getSpellByTag(pc.knownInCombatUsableTraitsTags[spellSlotIndex]);
                    //SpellAllowed sa = getCastingPlayer().playerClass.getSpellAllowedByTag(sp.tag);
                    string textToSpan = "<u>Description</u>" + "<BR>";
                    textToSpan += "<b><i><big>" + sp.name + "</big></i></b><BR>";
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
                    description.tbYloc = 1 * gv.squareSize;
                    description.tbWidth = pW * 40;
                    description.tbHeight = pH * 80;
                    description.logLinesList.Clear();
                    description.AddHtmlTextToLog(textToSpan);
                    description.onDrawLogBox();
                }
            }
            else
            {
                if (spellSlotIndex < pc.knownOutsideCombatUsableTraitsTags.Count)
                {
                    Spell sp = mod.getSpellByTag(pc.knownOutsideCombatUsableTraitsTags[spellSlotIndex]);
                    string textToSpan = "<u>Description</u>" + "<BR>";
                    textToSpan += "<b><i><big>" + sp.name + "</big></i></b><BR>";
                    textToSpan += "SP Cost: " + sp.costSP + "<BR>";
                    textToSpan += "HP Cost: " + sp.costHP + "<BR>";
                    textToSpan += "Target Range: " + sp.range + "<BR>";
                    textToSpan += "Area of Effect Radius: " + sp.aoeRadius + "<BR>";
                    //textToSpan += "Available at Level: " + getLevelAvailable(sp.tag) + "<BR>";
                    textToSpan += "<BR>";
                    textToSpan += sp.description;

                    description.tbXloc = 11 * gv.squareSize;
                    description.tbYloc = 1 * gv.squareSize;
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
        }
        public void onTouchCastSelector(MouseEventArgs e, MouseEventType.EventType eventType, bool inCombat)
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
			    x = (int) e.X;
			    y = (int) e.Y;
			
			    btnHelp.glowOn = false;
			    //btnInfo.glowOn = false;
			    btnExit.glowOn = false;
			    btnSelect.glowOn = false;
			
			    for (int j = 0; j < slotsPerPage; j++)
			    {
				    if (btnSpellSlots[j].getImpact(x, y))
				    {
					    spellSlotIndex = j;
				    }
			    }
			    if (btnHelp.getImpact(x, y))
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
    
        public void doCleanUp()
	    {
    	    btnSpellSlots.Clear();
    	    btnHelp = null;
    	    btnSelect = null;
    	    btnExit = null;
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
                if (spellSlotIndex < pc.knownInCombatUsableTraitsTags.Count)
                {
                    Spell sp = mod.getSpellByTag(pc.knownInCombatUsableTraitsTags[spellSlotIndex]);

                    if ((pc.sp >= sp.costSP) && (pc.hp > sp.costHP))
                    {
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

            else //Adventure Map
            {
                //only cast if useable on adventure maps
                //if ((GetCurrentlySelectedSpell().useableInSituation.Equals("Always")) || (GetCurrentlySelectedSpell().useableInSituation.Equals("OutOfCombat")))
                //{
                if (spellSlotIndex < pc.knownOutsideCombatUsableTraitsTags.Count)
                {
                    Spell sp = mod.getSpellByTag(pc.knownOutsideCombatUsableTraitsTags[spellSlotIndex]);

                    if ((pc.sp >= sp.costSP) && (pc.hp > sp.costHP))
                    {
                        gv.cc.currentSelectedSpell = sp;
                        //ask for target
                        // selected to USE ITEM

                        List<string> pcNames = new List<string>();
                        pcNames.Add("cancel");
                        foreach (Player p in mod.playerList)
                        {
                            pcNames.Add(p.name);
                        }

                        //If only one PC, do not show select PC dialog...just go to cast selector
                        if (mod.playerList.Count == 1)
                        {
                            try
                            {
                                Player target = mod.playerList[0];
                                gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, target, target);
                                gv.screenType = "main";
                                doCleanUp();
                                return;
                            }
                            catch (Exception ex)
                            {
                                gv.errorLog(ex.ToString());
                            }
                        }

                        using (ItemListSelector pcSel = new ItemListSelector(gv, pcNames, mod.spellLabelSingular + " Target"))
                        {
                            pcSel.ShowDialog();
                            if (pcSel.selectedIndex > 0)
                            {
                                try
                                {
                                    Player target = mod.playerList[pcSel.selectedIndex - 1];
                                    gv.cc.doSpellBasedOnScriptOrEffectTag(gv.cc.currentSelectedSpell, pc, target);
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
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Not Enough SP for that spell", Toast.LENGTH_SHORT).show();
                    }
        
                }
            }
		}          
    
        public Spell GetCurrentlySelectedSpell()
	    {
    	    SpellAllowed sa = getCastingPlayer().playerClass.spellsAllowed[spellSlotIndex];
		    return mod.getSpellByTag(sa.tag);
	    }
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
		    return mod.playerList[castingPlayerIndex];
	    }
	    public void tutorialMessageCastingScreen()
        {
		    gv.sf.MessageBoxHtml(this.stringMessageCastSelector);	
        }
    }
}
