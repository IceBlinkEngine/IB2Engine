using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenSpellLevelUp 
    {
	    //private gv.module gv.mod;
	    private GameView gv;

        public int spellToLearnIndex = 1;
        public int castingPlayerIndex = 0;
	    private int spellSlotIndex = 0;
	    private int slotsPerPage = 20;

        //added(1)
        private int maxPages = 20;
        private int tknPageIndex = 0;

        private List<IbbButton> btnSpellSlots = new List<IbbButton>();

        //added(3)
        private IbbButton btnTokensLeft = null;
        private IbbButton btnTokensRight = null;
        private IbbButton btnPageIndex = null;

        private IbbButton btnHelp = null;
	    private IbbButton btnSelect = null;
	    private IbbButton btnExit = null;
	    List<string> spellsToLearnTagsList = new List<string>();
	    public Player pc;
        public bool infoOnly = false; //set to true when called for info only
        public bool isInCombat = false;
        private string stringMessageSpellLevelUp = "";
        private IbbHtmlTextBox description;

        List<SpellAllowed> backupSpellsAllowed = new List<SpellAllowed>();

        public ScreenSpellLevelUp(Module m, GameView g) 
	    {
		    //gv.mod = m;
		    gv = g;
		    setControlsStart();
		    pc = new Player();
		    stringMessageSpellLevelUp = gv.cc.loadTextToString("data/MessageSpellLevelUp.txt");
	    }
	
	    public void resetPC(bool info_only, Player p, bool inCombat)
	    {
		    pc = p;
            infoOnly = info_only;
            isInCombat = inCombat;
            spellToLearnIndex = 1;
        }

        public void sortSpellsForLevelUp(Player pc)
        {
            //clear 
            backupSpellsAllowed.Clear();
            List<string> spellsForLearningTags = new List<string>();
            List<SpellAllowed> spellsForLearning = new List<SpellAllowed>();

            if (!infoOnly)
            {
                //add the unknown available traits first
                spellsForLearningTags = pc.getSpellsToLearn();
                foreach (string s in spellsForLearningTags)
                {
                    foreach (SpellAllowed ta in pc.playerClass.spellsAllowed)
                    {
                        if (ta.tag == s)
                        {
                            spellsForLearning.Add(ta);
                        }
                    }
                }

                //sort the unknwon, available traits
                int levelCounter = 0;
                while (spellsForLearning.Count > 0)
                {
                    for (int i = spellsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == spellsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupSpellsAllowed.Add(spellsForLearning[i]);
                            spellsForLearning.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }

                //add the unkown, not yet available traits
                foreach (SpellAllowed ta in pc.playerClass.spellsAllowed)
                {
                    bool notKnownYet = true;

                    //not hidden
                    if (!ta.allow || ta.needsSpecificTrainingToLearn)
                    {
                        //do not show the "hidden" traits that require special learning here
                        notKnownYet = false;
                    }

                    //not available
                    if (ta.atWhatLevelIsAvailable <= pc.classLevel)
                    {
                        //Spell tr = gv.mod.getSpellByTag(ta.tag);
                        notKnownYet = false;
                    }
                        //not available(attribues, prequisite trait)
                        //attributes
                        /*
                        if (checkAttributeRequirementsOfTrait(pc, tr))
                        {
                            //prerequisite traits
                            if (!tr.prerequisiteTrait.Equals("none"))
                            {
                                //requires prereq so check if you have it
                                if (pc.knownTraitsTags.Contains(tr.prerequisiteTrait) || pc.learningTraitsTags.Contains(tr.prerequisiteTrait))
                                {
                                    notKnownYet = false;
                                }
                            }
                            else
                            {
                                notKnownYet = false;
                            }
                        }
                        */
                        //not known
                        foreach (string s in pc.knownSpellsTags)
                        {
                            if (s == ta.tag)
                            {
                                notKnownYet = false;
                            }
                        }
                        //not just learned
                        foreach (string s in pc.learningSpellsTags)
                        {
                            if (s == ta.tag)
                            {
                                notKnownYet = false;
                            }
                        }

                    //not already replaced
                    foreach (string s in pc.replacedTraitsOrSpellsByTag)
                    {
                        if (s == ta.tag)
                        {
                            notKnownYet = false;
                        }
                    }


                    if (notKnownYet)
                    {
                        //add
                        spellsForLearning.Add(ta);
                    }
                }

                //sort the unknwon, not yet available traits
                levelCounter = 0;
                while (spellsForLearning.Count > 0)
                {
                    for (int i = spellsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == spellsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupSpellsAllowed.Add(spellsForLearning[i]);
                            spellsForLearning.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }

                //add the known traits
                foreach (string s in pc.knownSpellsTags)
                {
                    foreach (SpellAllowed ta in pc.playerClass.spellsAllowed)
                    {
                        if (ta.tag == s)
                        {
                            spellsForLearning.Add(ta);
                        }
                    }
                }

                foreach (string s in pc.learningSpellsTags)
                {
                    foreach (SpellAllowed ta in pc.playerClass.spellsAllowed)
                    {
                        if (ta.tag == s)
                        {
                            spellsForLearning.Add(ta);
                        }
                    }
                }

                //sort the known traits
                levelCounter = 0;
                while (spellsForLearning.Count > 0)
                {
                    for (int i = spellsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == spellsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupSpellsAllowed.Add(spellsForLearning[i]);
                            spellsForLearning.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }

            }
            //info only
            //todo: adjust like above, sigh
            else
            {

                //add the known spells
                foreach (string s in pc.knownSpellsTags)
                {
                    foreach (SpellAllowed ta in pc.playerClass.spellsAllowed)
                    {
                        if (ta.tag == s)
                        {
                            spellsForLearning.Add(ta);
                        }
                    }
                }

                /*
                foreach (string s in pc.learningTraitsTags)
                {
                    foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                    {
                        if (ta.tag == s)
                        {
                            traitsForLearning.Add(ta);
                        }
                    }
                }
                */

                //sort the known traits
                int levelCounter = 0;
                while (spellsForLearning.Count > 0)
                {
                    for (int i = spellsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == spellsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupSpellsAllowed.Add(spellsForLearning[i]);
                            spellsForLearning.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }

                //add the unknown available traits first
                spellsForLearningTags = pc.getSpellsToLearn();
                foreach (string s in spellsForLearningTags)
                {
                    foreach (SpellAllowed ta in pc.playerClass.spellsAllowed)
                    {
                        if (ta.tag == s)
                        {
                            spellsForLearning.Add(ta);
                        }
                    }
                }

                //sort the unknwon, available traits
                levelCounter = 0;
                while (spellsForLearning.Count > 0)
                {
                    for (int i = spellsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == spellsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupSpellsAllowed.Add(spellsForLearning[i]);
                            spellsForLearning.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }


                //add the unkown, not yet available traits
                foreach (SpellAllowed ta in pc.playerClass.spellsAllowed)
                {
                    bool notKnownYet = true;

                    //is hidden
                    if (!ta.allow || ta.needsSpecificTrainingToLearn)
                    {
                        //do not show the "hidden" traits that require special learning here
                        notKnownYet = false;
                    }

                    //is available
                    if (ta.atWhatLevelIsAvailable <= pc.classLevel)
                    {
                        notKnownYet = false;
                    }
                        /*
                        Trait tr = gv.mod.getTraitByTag(ta.tag);
                        if (checkAttributeRequirementsOfTrait(pc, tr))
                        {
                            //prerequisite traits
                            if (!tr.prerequisiteTrait.Equals("none"))
                            {
                                //requires prereq so check if you have it
                                if (pc.knownTraitsTags.Contains(tr.prerequisiteTrait))
                                {
                                    notKnownYet = false;
                                }
                            }
                            else
                            {
                                notKnownYet = false;
                            }
                        }
                        */

                        //is known
                        foreach (string s in pc.knownSpellsTags)
                        {
                            if (s == ta.tag)
                            {
                                notKnownYet = false;
                            }
                        }

                    //not already replaced
                    foreach (string s in pc.replacedTraitsOrSpellsByTag)
                    {
                        if (s == ta.tag)
                        {
                            notKnownYet = false;
                        }
                    }


                    if (notKnownYet)
                    {
                        //add
                        spellsForLearning.Add(ta);
                    }
                }

                //sort the unknwon, not yet available traits
                levelCounter = 0;
                while (spellsForLearning.Count > 0)
                {
                    for (int i = spellsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == spellsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupSpellsAllowed.Add(spellsForLearning[i]);
                            spellsForLearning.RemoveAt(i);
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

            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;

            //added
            if (btnTokensLeft == null)
            {
                btnTokensLeft = new IbbButton(gv, 1.0f);
                btnTokensLeft.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnTokensLeft.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
                btnTokensLeft.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnTokensLeft.X = (int)(5 * gv.squareSize) + (3 * pW);
                btnTokensLeft.Y = (1 * gv.squareSize);
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
                btnPageIndex.Y = (1 * gv.squareSize);
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
                btnTokensRight.Y = (1 * gv.squareSize);
                btnTokensRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTokensRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }

            if (btnSelect == null)
		    {
			    btnSelect = new IbbButton(gv, 0.8f);	
			    btnSelect.Text = "LEARN SELECTED CHOICE";
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
			    btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    int x = y % 5;
			    int yy = y / 5;
			    btnNew.X = ((x + 4) * gv.squareSize) + (padW * (x+1)) + gv.oXshift;
			    btnNew.Y = (2 + yy) * gv.squareSize + (padW * yy + padW);

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);	
			
			    btnSpellSlots.Add(btnNew);
		    }			
	    }
	
	    //CAST SELECTOR SCREEN (COMBAT and MAIN)
        public void redrawSpellLevelUp(bool inPcCreation)
        {
            Player pc = getCastingPlayer();
            btnSelect.Text = "LEARN SELECTED " + gv.mod.getPlayerClass(getCastingPlayer().classTag).spellLabelPlural;
            spellsToLearnTagsList.Clear();
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
                int maxNumber = 0;
                if (gv.mod.getPlayerClass(pc.classTag).spellsToLearnAtLevelTable[pc.classLevel] > pc.getSpellsToLearn(gv.mod).Count)
                {
                    maxNumber = pc.getSpellsToLearn(gv.mod).Count;
                    maxNumber += spellToLearnIndex - 1;
                }
                else
                {
                    maxNumber = gv.mod.getPlayerClass(pc.classTag).traitsToLearnAtLevelTable[pc.classLevel];
                }
                gv.DrawText("Select Choice Nr. " + spellToLearnIndex + " of " + maxNumber + " Choice(s) to Learn", noticeX, pH * 1, 1.0f, Color.Gray);

                //gv.DrawText("Select One " + gv.mod.getPlayerClass(pc.classTag).spellLabelSingular + " to Learn", noticeX, pH * 1, 1.0f, Color.Gray);
                //gv.DrawText("Select " + spellToLearnIndex + " of " + gv.mod.getPlayerClass(pc.classTag).spellsToLearnAtLevelTable[getCastingPlayer().classLevel] + " " + gv.mod.getPlayerClass(pc.classTag).spellLabelPlural + " to Learn", noticeX, pH * 1, 1.0f, Color.Gray);
                //gv.DrawText(getCastingPlayer().name + " SP: " + getCastingPlayer().sp + "/" + getCastingPlayer().spMax, pW * 50, pH * 1, 1.0f, Color.Yellow);
                //gv.DrawText(getCastingPlayer().name + " HP: " + getCastingPlayer().hp + "/" + getCastingPlayer().hpMax, pW * 50, pH * 1, 1.0f, Color.Yellow);


                //DRAW NOTIFICATIONS
                if (isSelectedSpellSlotInKnownSpellsRange())
                {
                    Spell sp = GetCurrentlySelectedSpell();
                    //Player pc = getCastingPlayer();

                    //check to see if already known
                    //if (pc.knownSpellsTags.Contains(sp.tag))
                    if ((pc.knownSpellsTags.Contains(sp.tag)) || (pc.learningSpellsTags.Contains(sp.tag)))
                    {
                        //say that you already know this one
                        gv.DrawText("Already Known", noticeX, noticeY, 1.0f, Color.Yellow);
                    }
                    else //spell not known
                    {
                        //check if available to learn
                        if (isAvailableToLearn(sp.tag))
                        {
                            gv.DrawText("Available to Learn", noticeX, noticeY, 1.0f, Color.Lime);
                        }
                        else //not available yet
                        {
                            gv.DrawText(gv.mod.getPlayerClass(pc.classTag).spellLabelSingular + " Not Available to Learn Yet", noticeX, noticeY, 1.0f, Color.Red);
                        }
                    }
                }
            }	
		
		    //DRAW ALL SPELL SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnSpellSlots)
		    {			
			    ///Player pc = getCastingPlayer();						
			
			    if (cntSlot == spellSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}

                //here insert
                sortSpellsForLevelUp(pc);

                //show only spells for the PC class
                if ((cntSlot + (tknPageIndex * slotsPerPage)) < backupSpellsAllowed.Count)
			    {
				    SpellAllowed sa = backupSpellsAllowed[cntSlot + (tknPageIndex * slotsPerPage)];
				    Spell sp = gv.mod.getSpellByTag(sa.tag);

                    if (infoOnly)
                    {
                        if (pc.knownSpellsTags.Contains(sp.tag)) //check to see if already known, if so turn on button
                        {
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small");
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                            gv.cc.DisposeOfBitmap(ref btn.Img3);
                            btn.Img3 = null;
                            //gv.cc.DisposeOfBitmap(ref btn.Img3);
                            //btn.Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                        }
                        else //spell not known yet
                        {
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small_off");
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(sp.spellImage + "_off");
                            gv.cc.DisposeOfBitmap(ref btn.Img3);
                            btn.Img3 = null;

                            if (isAvailableToLearn(sp.tag))
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img3);
                                btn.Img3 = gv.cc.LoadBitmap("yellow_frame");
                            }
                            else
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img3);
                                btn.Img3 = gv.cc.LoadBitmap("red_frame");
                            }
                        }
                    }
                    else
                    {
                        if (pc.knownSpellsTags.Contains(sp.tag)) //check to see if already known, if so turn off button
                        {
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(sp.spellImage + "_off");
                            gv.cc.DisposeOfBitmap(ref btn.Img3);
                            btn.Img3 = gv.cc.LoadBitmap("yellow_frame");
                        }
                        else //spell not known yet
                        {
                            if (isAvailableToLearn(sp.tag)) //if available to learn, turn on button
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);	
                                gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = gv.cc.LoadBitmap(sp.spellImage);
                                gv.cc.DisposeOfBitmap(ref btn.Img3);
                                btn.Img3 = null;
                            }
                            else //not available to learn, turn off button
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
                                gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = gv.cc.LoadBitmap(sp.spellImage + "_off");
                                gv.cc.DisposeOfBitmap(ref btn.Img3);
                                btn.Img3 = gv.cc.LoadBitmap("red_frame");
                            }
                        }
                    }				
			    }
			    else //slot is not in spells allowed index range
			    {
                    gv.cc.DisposeOfBitmap(ref btn.Img);
                    btn.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
				    btn.Img2 = null;
                    btn.Img3 = null;
                }			
			    btn.Draw();
			    cntSlot++;
		    }

            //DRAW DESCRIPTION BOX
            locY = tabStartY;
            if (isSelectedSpellSlotInKnownSpellsRange())
            {
                Spell sp = GetCurrentlySelectedSpell();
                
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
                textToSpan += "Available at Level: " + getLevelAvailable(sp.tag) + "<BR>";
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

            if (infoOnly)
            {
                btnSelect.Text = "RETURN";
                btnSelect.Draw();
                btnTokensLeft.Draw();
                btnTokensRight.Draw();
                btnPageIndex.Draw();
            }
            else
            {
                btnSelect.Text = "LEARN SELECTED " + gv.mod.getPlayerClass(pc.classTag).spellLabelSingular.ToUpper();
                btnHelp.Draw();
                btnExit.Draw();
                btnSelect.Draw();
                btnTokensLeft.Draw();
                btnTokensRight.Draw();
                btnPageIndex.Draw();
            }
        }
        public void onTouchSpellLevelUp(MouseEventArgs e, MouseEventType.EventType eventType, bool inPcCreation, bool inCombat)
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

                        btnTokensLeft.glowOn = false;
                        btnTokensRight.glowOn = false;
                        btnHelp.glowOn = false;
                        btnExit.glowOn = false;
                        btnSelect.glowOn = false;

                        for (int j = 0; j < slotsPerPage; j++)
                        {
                            if (btnSpellSlots[j].getImpact(x, y))
                            {
                                gv.PlaySound("btn_click");
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
                            if (!infoOnly)
                            {
                                gv.PlaySound("btn_click");
                                tutorialMessageCastingScreen();
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
                                doSelectedSpellToLearn(inPcCreation);
                            }
                        }
                        else if (btnExit.getImpact(x, y))// NOT USED  ACTUALLY (double check, thought this was used?)
                        {
                            if (!infoOnly)
                            {
                                gv.screenParty.traitGained = "";
                                gv.screenParty.spellGained = "";
                                pc.learningTraitsTags.Clear();
                                pc.learningEffects.Clear();
                                pc.learningSpellsTags.Clear();
                                spellToLearnIndex = 1;
                                gv.PlaySound("btn_click");
                                if (inPcCreation)
                                {
                                    gv.screenType = "pcCreation";
                                }
                                else //differentiate for combat, use incombat
                                {
                                    if (inCombat)
                                    {
                                        pc.classLevel--;
                                        gv.screenType = "combatParty";
                                    }
                                    else
                                    {
                                        pc.classLevel--;
                                        gv.screenType = "party";
                                    }
                                }
                            }
                            else //differentiate for combat, use incombat
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
                        }
                        break;
                }
            }
            catch { }
	    }
    
        public void doSelectedSpellToLearn(bool inPcCreation)
        {
    	    if (isSelectedSpellSlotInKnownSpellsRange())
		    {
			    Spell sp = GetCurrentlySelectedSpell();
                if (isAvailableToLearn(sp.tag))
                {
                    Player pc = getCastingPlayer();
                    //pc.knownSpellsTags.Add(sp.tag);

                    pc.learningSpellsTags.Add(sp.tag);

                    //***************************************************
                    //get the trait tor replace (if existent)
                    Spell temp = new Spell();
                    foreach (Spell s in gv.mod.moduleSpellsList)
                    {
                        if (s.tag == sp.spellToReplaceByTag)
                        {
                            temp = s.DeepCopy();
                        }
                    }
                    /*
                    //adding spell to replace mechanism: known spells
                    for (int i = pc.knownSpellsTags.Count - 1; i >= 0; i--)
                    {
                        if (pc.knownSpellsTags[i] == sp.spellToReplaceByTag)
                        {
                            pc.knownSpellsTags.RemoveAt(i);
                        }
                    }
                    */

                    /*
                    for (int i = pc.knownInCombatUsableSpellsTags.Count - 1; i >= 0; i--)
                    {
                        if (pc.knownInCombatUsableTraitsTags[i] == tr.traitToReplaceByTag)
                        {
                            pc.knownInCombatUsableTraitsTags.RemoveAt(i);
                        }

                        if (pc.knownInCombatUsableTraitsTags[i] == temp.associatedSpellTag)
                        {
                            pc.knownInCombatUsableTraitsTags.RemoveAt(i);
                        }
                    }

                    for (int i = pc.knownOutsideCombatUsableTraitsTags.Count - 1; i >= 0; i--)
                    {
                        if (pc.knownOutsideCombatUsableTraitsTags[i] == tr.traitToReplaceByTag)
                        {
                            pc.knownOutsideCombatUsableTraitsTags.RemoveAt(i);
                        }
                        if (pc.knownOutsideCombatUsableTraitsTags[i] == temp.associatedSpellTag)
                        {
                            pc.knownOutsideCombatUsableTraitsTags.RemoveAt(i);
                        }
                    }

                    for (int i = pc.knownUsableTraitsTags.Count - 1; i >= 0; i--)
                    {
                        if (pc.knownUsableTraitsTags[i] == tr.traitToReplaceByTag)
                        {
                            pc.knownUsableTraitsTags.RemoveAt(i);
                        }
                        if (pc.knownUsableTraitsTags[i] == temp.associatedSpellTag)
                        {
                            pc.knownUsableTraitsTags.RemoveAt(i);
                        }
                    }
                    */
                    /*
                    //adding trait to replace mechanism: learing traits list (just added)
                    for (int i = pc.learningSpellsTags.Count - 1; i >= 0; i--)
                    {
                        if (pc.learningSpellsTags[i] == sp.spellToReplaceByTag)
                        {
                            pc.learningSpellsTags.RemoveAt(i);
                        }
                    }
                    */

                    //***************************************************
                    sortSpellsForLevelUp(pc);
                    /*
                    if (inPcCreation)
				    {
					    gv.screenPcCreation.SaveCharacter(pc);
			    	    gv.screenPartyBuild.pcList.Add(pc);
			    	    gv.screenType = "partyBuild";
				    }
				    else
				    {
					    gv.screenType = "party";
					    gv.screenParty.spellGained += sp.name + ", ";
					    gv.screenParty.doLevelUpSummary();
				    }
                    */
                    //check to see if there are more spells to learn at this level  
                    spellToLearnIndex++;
                    int maxNumber = 0;
                    if (gv.mod.getPlayerClass(pc.classTag).spellsToLearnAtLevelTable[pc.classLevel] > pc.getSpellsToLearn(gv.mod).Count)
                    {
                        maxNumber = pc.getSpellsToLearn(gv.mod).Count + 1;
                    }
                    else
                    {
                        maxNumber = gv.mod.getPlayerClass(pc.classTag).spellsToLearnAtLevelTable[pc.classLevel];
                    }

                   
                    if (spellToLearnIndex <= maxNumber)
                    {
                        gv.screenParty.spellGained += sp.name + ", ";
                    }
                    else //finished learning all spells available for this level  
                    {
                        if (inPcCreation)
                        {
                            //foreach (string s in pc.learningTraitsTags)
                            //{
                            for (int counter = pc.learningTraitsTags.Count - 1; counter >= 0; counter--)
                            {
                                pc.knownTraitsTags.Add(pc.learningTraitsTags[counter]);
                                pc.numberOfTraitUsesLeftForToday.Add(gv.mod.getTraitByTag(pc.learningTraitsTags[counter]).numberOfUsesPerDay[pc.classLevel]);
                                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                                //adding trait-effect system code here
                                //TODO: must get trait by ts string
                                Trait tr = new Trait();
                                foreach (Trait t in gv.mod.moduleTraitsList)
                                {
                                    if (t.tag == pc.learningTraitsTags[counter])
                                    {
                                        tr = t;
                                        break;
                                    }
                                }


                                //********************************************
                                #region replacement code traits
                                //get the trait tor replace (if existent)
                                Trait temp2 = new Trait();
                                foreach (Trait t in gv.mod.moduleTraitsList)
                                {
                                    if (t.tag == tr.traitToReplaceByTag)
                                    {
                                        temp2 = t.DeepCopy();
                                    }
                                }
                                if ((tr.traitToReplaceByTag != "none") && (tr.traitToReplaceByTag != ""))
                                {
                                    pc.replacedTraitsOrSpellsByTag.Add(tr.traitToReplaceByTag);
                                }
                                if (tr.traitToReplaceByTag != tr.prerequisiteTrait)
                                {
                                    string replacedTag = tr.traitToReplaceByTag;
                                    for (int j = gv.mod.moduleTraitsList.Count - 1; j >= 0; j--)
                                    {
                                        if (gv.mod.moduleTraitsList[j].prerequisiteTrait == replacedTag)
                                        {
                                            if (!pc.replacedTraitsOrSpellsByTag.Contains(replacedTag))
                                            {
                                                pc.replacedTraitsOrSpellsByTag.Add(gv.mod.moduleTraitsList[j].tag);
                                                replacedTag = gv.mod.moduleTraitsList[j].tag;
                                                j = gv.mod.moduleTraitsList.Count - 1;
                                            }
                                        }
                                    }
                                }

                                //adding trait to replace mechanism: known traits
                                for (int i = pc.knownTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        //TODO: remove connected permannent effects
                                        //Peter
                                        for (int j = pc.effectsList.Count - 1; j >= 0; j--)
                                        {
                                            foreach (EffectTagForDropDownList etfddl in temp2.traitEffectTagList)
                                            {
                                                if (pc.effectsList[j].tag == etfddl.tag)
                                                {
                                                    if (pc.effectsList[j].isPermanent)
                                                    {
                                                        pc.effectsList.RemoveAt(j);
                                                    }
                                                }
                                            }
                                        }
                                        //pc.replacedTraitsOrSpellsByTag.Add(tr.traitToReplaceByTag);
                                        pc.knownTraitsTags.RemoveAt(i);
                                        pc.numberOfTraitUsesLeftForToday.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.knownInCombatUsableTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownInCombatUsableTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        pc.knownInCombatUsableTraitsTags.RemoveAt(i);
                                    }

                                    if (pc.knownInCombatUsableTraitsTags[i] == temp2.associatedSpellTag)
                                    {
                                        pc.knownInCombatUsableTraitsTags.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.knownOutsideCombatUsableTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownOutsideCombatUsableTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        pc.knownOutsideCombatUsableTraitsTags.RemoveAt(i);
                                    }
                                    if (pc.knownOutsideCombatUsableTraitsTags[i] == temp2.associatedSpellTag)
                                    {
                                        pc.knownOutsideCombatUsableTraitsTags.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.knownUsableTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownUsableTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        pc.knownUsableTraitsTags.RemoveAt(i);
                                    }
                                    if (pc.knownUsableTraitsTags[i] == temp2.associatedSpellTag)
                                    {
                                        pc.knownUsableTraitsTags.RemoveAt(i);
                                    }
                                }


                                //adding trait to replace mechanism: learing traits list (just added)
                                for (int i = pc.learningTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.learningTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        //TODO: remove connected permannent effects
                                        //Peter
                                        for (int j = pc.effectsList.Count - 1; j >= 0; j--)
                                        {
                                            foreach (EffectTagForDropDownList etfddl in temp2.traitEffectTagList)
                                            {
                                                if (pc.effectsList[j].tag == etfddl.tag)
                                                {
                                                    if (pc.effectsList[j].isPermanent)
                                                    {
                                                        pc.effectsList.RemoveAt(j);
                                                    }
                                                }
                                            }
                                        }
                                        //pc.replacedTraitsOrSpellsByTag.Add(tr.traitToReplaceByTag);
                                        pc.learningTraitsTags.RemoveAt(i);
                                    }
                                }
                                #endregion

                                //********************************************

                                //add trait/effect system here: usable traits
                                if (!tr.associatedSpellTag.Equals("none"))
                                {
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
                                }

                                //add permanent effects of trait to effect list of this pc
                                foreach (EffectTagForDropDownList efTag in tr.traitEffectTagList)
                                {//1
                                    foreach (Effect ef in gv.mod.moduleEffectsList)
                                    {//2
                                        if (ef.tag == efTag.tag)
                                        {//3
                                            if (ef.isPermanent)
                                            {//4
                                                bool doesNotExistAlfready = true;
                                                foreach (Effect ef2 in pc.effectsList)
                                                {//5
                                                    if (ef2.tag == ef.tag)
                                                    {//6
                                                        doesNotExistAlfready = false;
                                                        break;
                                                    }//6
                                                }//5

                                                if (doesNotExistAlfready)
                                                {//5
                                                    pc.effectsList.Add(ef);
                                                    gv.sf.UpdateStats(pc);
                                                    if (ef.modifyHpMax != 0)
                                                    {//6
                                                        pc.hp += ef.modifyHpMax;
                                                        if (pc.hp < 1)
                                                        {//7
                                                            pc.hp = 1;
                                                        }//7
                                                        if (pc.hp > pc.hpMax)
                                                        {
                                                            pc.hp = pc.hpMax;
                                                        }
                                                    }//6

                                                    if (ef.modifyCon != 0)
                                                    {//6
                                                        pc.hp += ef.modifyCon / 2;
                                                        if (pc.hp < 1)
                                                        {//7
                                                            pc.hp = 1;
                                                        }//7
                                                        if (pc.hp > pc.hpMax)
                                                        {
                                                            pc.hp = pc.hpMax;
                                                        }
                                                    }//6

                                                    if (ef.modifySpMax != 0)
                                                    {
                                                        pc.sp += ef.modifySpMax;
                                                        if (pc.sp < 1)
                                                        {
                                                            pc.sp = 1;
                                                        }
                                                        if (pc.sp > pc.spMax)
                                                        {
                                                            pc.sp = pc.spMax;
                                                        }
                                                    }

                                                    if (ef.modifyStr != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("strength"))
                                                        {
                                                            pc.sp += ef.modifyStr / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyDex != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("dexterity"))
                                                        {
                                                            pc.sp += ef.modifyDex / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyCon != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("constitution"))
                                                        {
                                                            pc.sp += ef.modifyCon / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyCha != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("charisma"))
                                                        {
                                                            pc.sp += ef.modifyCha / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyInt != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("intelligence"))
                                                        {
                                                            pc.sp += ef.modifyInt / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyWis != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("wisdom"))
                                                        {
                                                            pc.sp += ef.modifyWis / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }
                                                }//5
                                            }//4
                                        }//3
                                    }//2
                                }//1
                            //}//2 (is 1 actually)

                            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                            }//end of the graet for loo for traits


                            //pc.learningTraitsTags.Clear();

                            //TODO: replace mechanism for spells
                            for (int counter = pc.learningSpellsTags.Count - 1; counter >= 0; counter--)
                            {
                                pc.knownSpellsTags.Add(pc.learningSpellsTags[counter]);

                               //Spell replacement code
                                foreach (Spell s in gv.mod.moduleSpellsList)
                                {
                                    if (s.tag == pc.learningSpellsTags[counter])
                                    {
                                        sp = s;
                                        break;
                                    }
                                }
                                if ((sp.spellToReplaceByTag != "none") && (sp.spellToReplaceByTag != ""))
                                {
                                    pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                                }

                                for (int i = pc.knownSpellsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownSpellsTags[i] == sp.spellToReplaceByTag)
                                    {
                                        //pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                                        pc.knownSpellsTags.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.learningSpellsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.learningSpellsTags[i] == sp.spellToReplaceByTag)
                                    {
                                        //pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                                        pc.learningSpellsTags.RemoveAt(i);
                                    }
                                }
                            }
                            
                            pc.learningTraitsTags.Clear();
                            pc.learningEffects.Clear();
                            pc.learningSpellsTags.Clear();

                            gv.screenPcCreation.SaveCharacter(pc);
                            gv.screenPartyBuild.pcList.Add(pc);
                            gv.screenType = "partyBuild";
                         }
                         else  
                         {

                            //todo; add the same as above here
                            //****************************************************
                            for (int counter = pc.learningTraitsTags.Count - 1; counter >= 0; counter--)
                            {
                                pc.knownTraitsTags.Add(pc.learningTraitsTags[counter]);
                                pc.numberOfTraitUsesLeftForToday.Add(gv.mod.getTraitByTag(pc.learningTraitsTags[counter]).numberOfUsesPerDay[pc.classLevel]);
                                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                                //adding trait-effect system code here
                                //TODO: must get trait by ts string
                                Trait tr = new Trait();
                                foreach (Trait t in gv.mod.moduleTraitsList)
                                {
                                    if (t.tag == pc.learningTraitsTags[counter])
                                    {
                                        tr = t;
                                        break;
                                    }
                                }


                                //********************************************
                                #region replacement code traits
                                //get the trait tor replace (if existent)
                                Trait temp2 = new Trait();
                                foreach (Trait t in gv.mod.moduleTraitsList)
                                {
                                    if (t.tag == tr.traitToReplaceByTag)
                                    {
                                        temp2 = t.DeepCopy();
                                    }
                                }
                                if ((tr.traitToReplaceByTag != "none") && (tr.traitToReplaceByTag != ""))
                                {
                                    pc.replacedTraitsOrSpellsByTag.Add(tr.traitToReplaceByTag);
                                }
                                if (tr.traitToReplaceByTag != tr.prerequisiteTrait)
                                {
                                    string replacedTag = tr.traitToReplaceByTag;
                                    for (int j = gv.mod.moduleTraitsList.Count - 1; j >= 0; j--)
                                    {
                                        if (gv.mod.moduleTraitsList[j].prerequisiteTrait == replacedTag)
                                        {
                                            if (!pc.replacedTraitsOrSpellsByTag.Contains(replacedTag))
                                            {
                                                pc.replacedTraitsOrSpellsByTag.Add(gv.mod.moduleTraitsList[j].tag);
                                                replacedTag = gv.mod.moduleTraitsList[j].tag;
                                                j = gv.mod.moduleTraitsList.Count - 1;
                                            }
                                        }
                                    }
                                }

                                //adding trait to replace mechanism: known traits
                                for (int i = pc.knownTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        //TODO: remove connected permannent effects
                                        //Peter
                                        for (int j = pc.effectsList.Count - 1; j >= 0; j--)
                                        {
                                            foreach (EffectTagForDropDownList etfddl in temp2.traitEffectTagList)
                                            {
                                                if (pc.effectsList[j].tag == etfddl.tag)
                                                {
                                                    if (pc.effectsList[j].isPermanent)
                                                    {
                                                        pc.effectsList.RemoveAt(j);
                                                    }
                                                }
                                            }
                                        }
                                        //pc.replacedTraitsOrSpellsByTag.Add(tr.traitToReplaceByTag);
                                        pc.knownTraitsTags.RemoveAt(i);
                                        pc.numberOfTraitUsesLeftForToday.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.knownInCombatUsableTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownInCombatUsableTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        pc.knownInCombatUsableTraitsTags.RemoveAt(i);
                                    }

                                    if (pc.knownInCombatUsableTraitsTags[i] == temp2.associatedSpellTag)
                                    {
                                        pc.knownInCombatUsableTraitsTags.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.knownOutsideCombatUsableTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownOutsideCombatUsableTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        pc.knownOutsideCombatUsableTraitsTags.RemoveAt(i);
                                    }
                                    if (pc.knownOutsideCombatUsableTraitsTags[i] == temp2.associatedSpellTag)
                                    {
                                        pc.knownOutsideCombatUsableTraitsTags.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.knownUsableTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownUsableTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        pc.knownUsableTraitsTags.RemoveAt(i);
                                    }
                                    if (pc.knownUsableTraitsTags[i] == temp2.associatedSpellTag)
                                    {
                                        pc.knownUsableTraitsTags.RemoveAt(i);
                                    }
                                }


                                //adding trait to replace mechanism: learing traits list (just added)
                                for (int i = pc.learningTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.learningTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        //TODO: remove connected permannent effects
                                        //Peter
                                        for (int j = pc.effectsList.Count - 1; j >= 0; j--)
                                        {
                                            foreach (EffectTagForDropDownList etfddl in temp2.traitEffectTagList)
                                            {
                                                if (pc.effectsList[j].tag == etfddl.tag)
                                                {
                                                    if (pc.effectsList[j].isPermanent)
                                                    {
                                                        pc.effectsList.RemoveAt(j);
                                                    }
                                                }
                                            }
                                        }
                                        //pc.replacedTraitsOrSpellsByTag.Add(tr.traitToReplaceByTag);
                                        pc.learningTraitsTags.RemoveAt(i);
                                    }
                                }
                                #endregion

                                //********************************************

                                //add trait/effect system here: usable traits
                                if (!tr.associatedSpellTag.Equals("none"))
                                {
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
                                }

                                //add permanent effects of trait to effect list of this pc
                                foreach (EffectTagForDropDownList efTag in tr.traitEffectTagList)
                                {//1
                                    foreach (Effect ef in gv.mod.moduleEffectsList)
                                    {//2
                                        if (ef.tag == efTag.tag)
                                        {//3
                                            if (ef.isPermanent)
                                            {//4
                                                bool doesNotExistAlfready = true;
                                                foreach (Effect ef2 in pc.effectsList)
                                                {//5
                                                    if (ef2.tag == ef.tag)
                                                    {//6
                                                        doesNotExistAlfready = false;
                                                        break;
                                                    }//6
                                                }//5

                                                if (doesNotExistAlfready)
                                                {//5
                                                    pc.effectsList.Add(ef);
                                                    gv.sf.UpdateStats(pc);
                                                    if (ef.modifyHpMax != 0)
                                                    {//6
                                                        pc.hp += ef.modifyHpMax;
                                                        if (pc.hp < 1)
                                                        {//7
                                                            pc.hp = 1;
                                                        }//7
                                                        if (pc.hp > pc.hpMax)
                                                        {
                                                            pc.hp = pc.hpMax;
                                                        }
                                                    }//6

                                                    if (ef.modifyCon != 0)
                                                    {//6
                                                        pc.hp += ef.modifyCon / 2;
                                                        if (pc.hp < 1)
                                                        {//7
                                                            pc.hp = 1;
                                                        }//7
                                                        if (pc.hp > pc.hpMax)
                                                        {
                                                            pc.hp = pc.hpMax;
                                                        }
                                                    }//6

                                                    if (ef.modifySpMax != 0)
                                                    {
                                                        pc.sp += ef.modifySpMax;
                                                        if (pc.sp < 1)
                                                        {
                                                            pc.sp = 1;
                                                        }
                                                        if (pc.sp > pc.spMax)
                                                        {
                                                            pc.sp = pc.spMax;
                                                        }
                                                    }

                                                    if (ef.modifyStr != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("strength"))
                                                        {
                                                            pc.sp += ef.modifyStr / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyDex != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("dexterity"))
                                                        {
                                                            pc.sp += ef.modifyDex / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyCon != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("constitution"))
                                                        {
                                                            pc.sp += ef.modifyCon / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyCha != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("charisma"))
                                                        {
                                                            pc.sp += ef.modifyCha / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyInt != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("intelligence"))
                                                        {
                                                            pc.sp += ef.modifyInt / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyWis != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("wisdom"))
                                                        {
                                                            pc.sp += ef.modifyWis / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }
                                                }//5
                                            }//4
                                        }//3
                                    }//2
                                }//1
                                 //}//2 (is 1 actually)

                                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                            }//end of the graet for loo for traits


                            //pc.learningTraitsTags.Clear();

                            //TODO: replace mechanism for spells
                            for (int counter = pc.learningSpellsTags.Count - 1; counter >= 0; counter--)
                            {
                                pc.knownSpellsTags.Add(pc.learningSpellsTags[counter]);

                                //Spell replacement code
                                foreach (Spell s in gv.mod.moduleSpellsList)
                                {
                                    if (s.tag == pc.learningSpellsTags[counter])
                                    {
                                        sp = s;
                                        break;
                                    }
                                }
                                if ((sp.spellToReplaceByTag != "none") && (sp.spellToReplaceByTag != ""))
                                {
                                    pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                                }

                                for (int i = pc.knownSpellsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownSpellsTags[i] == sp.spellToReplaceByTag)
                                    {
                                        //pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                                        pc.knownSpellsTags.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.learningSpellsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.learningSpellsTags[i] == sp.spellToReplaceByTag)
                                    {
                                        //pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                                        pc.learningSpellsTags.RemoveAt(i);
                                    }
                                }
                            }

                            //****************************************************

                            pc.learningTraitsTags.Clear();
                            pc.learningEffects.Clear();
                            pc.learningSpellsTags.Clear();

                            pc.classLevel--;
                            pc.LevelUp();
                            gv.sf.UpdateStats(pc);
                            gv.screenType = "party";
                            gv.screenParty.spellGained += sp.name + ", ";
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
    	    if (spellsToLearnTagsList.Contains(spellTag))
    	    {
    		    return true;
    	    }
    	    return false;
        }
    
        public void fillToLearnList()
        {
    	    spellsToLearnTagsList = getCastingPlayer().getSpellsToLearn();	    
        }
    
        public Spell GetCurrentlySelectedSpell()
	    {
            //SpellAllowed sa = getCastingPlayer().playerClass.spellsAllowed[spellSlotIndex];
            //return gv.mod.getSpellByTag(sa.tag);
            sortSpellsForLevelUp(pc);
            //TraitAllowed ta = pc.playerClass.traitsAllowed[traitSlotIndex + (tknPageIndex * slotsPerPage)];
            SpellAllowed ta = backupSpellsAllowed[spellSlotIndex + (tknPageIndex * slotsPerPage)];
            return gv.mod.getSpellByTag(ta.tag);
        }
	    public bool isSelectedSpellSlotInKnownSpellsRange()
	    {
            //return spellSlotIndex < getCastingPlayer().playerClass.spellsAllowed.Count;
            sortSpellsForLevelUp(pc);
            return (spellSlotIndex + (tknPageIndex * slotsPerPage)) < backupSpellsAllowed.Count;
        }	
	    public int getLevelAvailable(string tag)
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
		    return pc;
	    }
	    public void tutorialMessageCastingScreen()
        {
		    gv.sf.MessageBoxHtml(this.stringMessageSpellLevelUp);	
        }

    }
}
