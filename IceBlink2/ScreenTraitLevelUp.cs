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

	    //private gv.module gv.mod;
	    private GameView gv;
	
	    private int traitSlotIndex = 0;
        private int traitToLearnIndex = 1;
        private int slotsPerPage = 20;
        
        //added(1)
        private int maxPages = 20;
        private int tknPageIndex = 0;

        private List<IbbButton> btnTraitSlots = new List<IbbButton>();

        //added(3)
        private IbbButton btnTokensLeft = null;
        private IbbButton btnTokensRight = null;
        private IbbButton btnPageIndex = null;

        private IbbButton btnHelp = null;
	    private IbbButton btnSelect = null;
	    private IbbButton btnExit = null;
	    List<string> traitsToLearnTagsList = new List<string>();
	    private Player pc;
        public bool infoOnly = false; //set to true when called for info only
        private string stringMessageTraitLevelUp = "";
        private IbbHtmlTextBox description;

        List<TraitAllowed> backupTraitsAllowed = new List<TraitAllowed>();


        public ScreenTraitLevelUp(Module m, GameView g) 
	    {
		    //gv.mod = m;
		    gv = g;
		    setControlsStart();
		    pc = new Player();
		    stringMessageTraitLevelUp = gv.cc.loadTextToString("data/MessageTraitLevelUp.txt");
	    }
	
	    public void resetPC(bool info_only, Player p)
	    {
		    pc = p;
            infoOnly = info_only;
            traitToLearnIndex = 1;
        }

        public void sortTraitsForLevelUp(Player pc)
        {
                //clear 
                backupTraitsAllowed.Clear();
                List<string> traitsForLearningTags = new List<string>();
                List<TraitAllowed> traitsForLearning = new List<TraitAllowed>();

            if (!infoOnly)
            {
                //add the unknown available traits first
                traitsForLearningTags = pc.getTraitsToLearn(gv.mod);
                foreach (string s in traitsForLearningTags)
                {
                    foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                    {
                        if (ta.tag == s)
                        {
                            traitsForLearning.Add(ta);
                        }
                    }
                }

                //sort the unknwon, available traits
                int levelCounter = 0;
                while (traitsForLearning.Count > 0)
                {
                    for (int i = traitsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == traitsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupTraitsAllowed.Add(traitsForLearning[i]);
                            traitsForLearning.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }

                //add the unkown, not yet available traits
                foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                {
                    bool notKnownYet = true;

                    //not hidden
                    if (!ta.allow)
                    {
                        //do not show the "hidden" traits that require special learning here
                        notKnownYet = false;
                    }

                    //not available
                    if (ta.atWhatLevelIsAvailable <= pc.classLevel)
                    {
                        Trait tr = gv.mod.getTraitByTag(ta.tag);
                        //not available(attribues, prequisite trait)
                        //attributes
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
                        //not known
                        foreach (string s in pc.knownTraitsTags)
                        {
                            if (s == ta.tag)
                            {
                                notKnownYet = false;
                            }
                        }
                        //not just learned
                        foreach (string s in pc.learningTraitsTags)
                        {
                            if (s == ta.tag)
                            {
                                notKnownYet = false;
                            }
                        }
                    }

                    if (notKnownYet)
                    {
                        //add
                        traitsForLearning.Add(ta);
                    }
                }

                //sort the unknwon, not yet available traits
                levelCounter = 0;
                while (traitsForLearning.Count > 0)
                {
                    for (int i = traitsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == traitsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupTraitsAllowed.Add(traitsForLearning[i]);
                            traitsForLearning.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }

                //add the known traits
                foreach (string s in pc.knownTraitsTags)
                {
                    foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                    {
                        if (ta.tag == s)
                        {
                            traitsForLearning.Add(ta);
                        }
                    }
                }

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

                //sort the known traits
                levelCounter = 0;
                while (traitsForLearning.Count > 0)
                {
                    for (int i = traitsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == traitsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupTraitsAllowed.Add(traitsForLearning[i]);
                            traitsForLearning.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }

            }
            //info only
            //todo: adjust like above, sigh
            else
            {

                //add the known traits
                foreach (string s in pc.knownTraitsTags)
                {
                    foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                    {
                        if (ta.tag == s)
                        {
                            traitsForLearning.Add(ta);
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
                while (traitsForLearning.Count > 0)
                {
                    for (int i = traitsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == traitsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupTraitsAllowed.Add(traitsForLearning[i]);
                            traitsForLearning.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }

                //add the unknown available traits first
                traitsForLearningTags = pc.getTraitsToLearn(gv.mod);
                foreach (string s in traitsForLearningTags)
                {
                    foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                    {
                        if (ta.tag == s)
                        {
                            traitsForLearning.Add(ta);
                        }
                    }
                }

                //sort the unknwon, available traits
                levelCounter = 0;
                while (traitsForLearning.Count > 0)
                {
                    for (int i = traitsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == traitsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupTraitsAllowed.Add(traitsForLearning[i]);
                            traitsForLearning.RemoveAt(i);
                        }
                    }
                    levelCounter++;
                }


                //add the unkown, not yet available traits
                foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                {
                    bool notKnownYet = true;

                    //not hidden
                    if (!ta.allow)
                    {
                        //do not show the "hidden" traits that require special learning here
                        notKnownYet = false;
                    }

                    //not available
                    if (ta.atWhatLevelIsAvailable <= pc.classLevel)
                    {
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
                        //not known
                        foreach (string s in pc.knownTraitsTags)
                        {
                            if (s == ta.tag)
                            {
                                notKnownYet = false;
                            }
                        }
                    }

                    if (notKnownYet)
                    {
                        //add
                        traitsForLearning.Add(ta);
                    }
                }

                //sort the unknwon, not yet available traits
                levelCounter = 0;
                while (traitsForLearning.Count > 0)
                {
                    for (int i = traitsForLearning.Count - 1; i >= 0; i--)
                    {
                        if (levelCounter == traitsForLearning[i].atWhatLevelIsAvailable)
                        {
                            backupTraitsAllowed.Add(traitsForLearning[i]);
                            traitsForLearning.RemoveAt(i);
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
            //description = new IbbHtmlTextBox(gv, 3*gv.squareSize + 2*pW, 2*gv.squareSize, gv.squareSize*5, gv.squareSize*10);
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
                gv.cc.DisposeOfBitmap(ref btnNew.Img);
                btnNew.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                gv.cc.DisposeOfBitmap(ref btnNew.Glow);
                btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    int x = y % 5;
			    int yy = y / 5;
			    btnNew.X = ((x + 4) * gv.squareSize) + (padW * (x+1)) + gv.oXshift;
			    btnNew.Y = (2 + yy) * gv.squareSize + (padW * yy + padW);

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);
			    btnTraitSlots.Add(btnNew);
		    }			
	    }

        //new method for checking attribute requiremnts of traits
        public bool checkAttributeRequirementsOfTrait (Player pc, Trait t)
        {
            gv.sf.UpdateStats(pc);

            if (pc.strength < t.requiredStrength)
            {
                return false;
            }
            if (pc.dexterity < t.requiredDexterity)
            {
                return false;
            }
            if (pc.constitution < t.requiredConstitution)
            {
                return false;
            }
            if (pc.intelligence < t.requiredIntelligence)
            {
                return false;
            }
            if (pc.wisdom < t.requiredWisdom)
            {
                return false;
            }
            if (pc.charisma < t.requiredCharisma)
            {
                return false;
            }

            return true;
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
                //gv.DrawText("Select One Trait to Learn", noticeX, pH * 1, 1.0f, Color.Gray);
                gv.DrawText("Select " + traitToLearnIndex + " of " + gv.mod.getPlayerClass(pc.classTag).traitsToLearnAtLevelTable[pc.classLevel] + " Choices to Learn", noticeX, pH * 1, 1.0f, Color.Gray);

                //DRAW NOTIFICATIONS
                if (isSelectedTraitSlotInKnownTraitsRange())
                {
                    Trait tr = GetCurrentlySelectedTrait();
                    
                    //check to see if already known
                    //if (pc.knownTraitsTags.Contains(tr.tag))
                    if ((pc.knownTraitsTags.Contains(tr.tag)) || (pc.learningTraitsTags.Contains(tr.tag)))
                    {
                        //say that you already know this one
                        gv.DrawText("Already Known", noticeX, noticeY, 1.0f, Color.Yellow);
                    }
                    else //trait not known
                    {
                        //checking attribute requiremnts of trait
                        bool attributeRequirementsMet = checkAttributeRequirementsOfTrait(pc, tr);
                        
                        //check if available to learn
                        if (isAvailableToLearn(tr.tag) && attributeRequirementsMet)
                        {
                            gv.DrawText("Available to Learn", noticeX, noticeY, 1.0f, Color.Lime);
                        }
                        else //not available yet
                        {
                            if (attributeRequirementsMet)
                            {
                                gv.DrawText(pc.playerClass.traitLabelSingular + " Not Available to Learn Yet", noticeX, noticeY, 1.0f, Color.Red);
                            }
                            else 
                            {
                                gv.DrawText("Attribute requirements not met", noticeX, noticeY, 1.0f, Color.Red);
                            }
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

                //added
                //if ((cntSlot + (tknPageIndex * slotsPerPage)) < playerTokenList.Count)
                //{
                //}
                //show only traits for the PC class
                
                //here insert
                sortTraitsForLevelUp(pc);

                //if ((cntSlot +(tknPageIndex * slotsPerPage)) < pc.playerClass.traitsAllowed.Count)
                if ((cntSlot + (tknPageIndex * slotsPerPage)) < backupTraitsAllowed.Count)
                {
                    //TraitAllowed ta = pc.playerClass.traitsAllowed[cntSlot + (tknPageIndex * slotsPerPage)];
                    TraitAllowed ta = backupTraitsAllowed[cntSlot + (tknPageIndex * slotsPerPage)];
                    Trait tr = gv.mod.getTraitByTag(ta.tag);

                    if (infoOnly)
                    {
                        if (pc.knownTraitsTags.Contains(tr.tag)) //check to see if already known, if so turn on button
                        {
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small");
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(tr.traitImage);
                            btn.Img3 = null;

                            //gv.cc.DisposeOfBitmap(ref btn.Img3);
                            //btn.Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                        }
                        else //trait not known yet
                        {
                            /*
                            gv.cc.DisposeOfBitmap(ref btn.Img);
                            btn.Img = gv.cc.LoadBitmap("btn_small_off");
                            gv.cc.DisposeOfBitmap(ref btn.Img2);
                            btn.Img2 = gv.cc.LoadBitmap(tr.traitImage + "_off");
                            btn.Img3 = null;
                            */
                            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                            //checking attribute requiremnts of trait
                            bool attributeRequirementsMet = checkAttributeRequirementsOfTrait(pc, tr);
                            //if (tr.tag == "bluff")
                            //{
                            //int hghg = 0;
                            //}
                            if (isAvailableToLearn(tr.tag) && attributeRequirementsMet) //if available to learn, turn on button
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = gv.cc.LoadBitmap("btn_small_off");
                                gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = gv.cc.LoadBitmap(tr.traitImage + "_off");
                                gv.cc.DisposeOfBitmap(ref btn.Img3);
                                btn.Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                            }
                            else //not available to learn, turn off button
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = gv.cc.LoadBitmap("btn_small_off");
                                gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = gv.cc.LoadBitmap(tr.traitImage + "_off");
                                gv.cc.DisposeOfBitmap(ref btn.Img3);
                                btn.Img3 = gv.cc.LoadBitmap("encounter_indicator");
                            }

                            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
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
                            gv.cc.DisposeOfBitmap(ref btn.Img3);
                            btn.Img3 = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                        }
                        else //trait not known yet
                        {
                            //checking attribute requiremnts of trait
                            bool attributeRequirementsMet = checkAttributeRequirementsOfTrait(pc, tr);
                            //if (tr.tag == "bluff")
                            //{
                                //int hghg = 0;
                            //}
                            if (isAvailableToLearn(tr.tag) && attributeRequirementsMet) //if available to learn, turn on button
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = gv.cc.LoadBitmap("btn_small");
                                gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = gv.cc.LoadBitmap(tr.traitImage);
                                btn.Img3 = null;
                            }
                            else //not available to learn, turn off button
                            {
                                gv.cc.DisposeOfBitmap(ref btn.Img);
                                btn.Img = gv.cc.LoadBitmap("btn_small_off");
                                gv.cc.DisposeOfBitmap(ref btn.Img2);
                                btn.Img2 = gv.cc.LoadBitmap(tr.traitImage + "_off");
                                gv.cc.DisposeOfBitmap(ref btn.Img3);
                                btn.Img3 = gv.cc.LoadBitmap("encounter_indicator");
                            }
                        }
                    }				
			    }
			    else //slot is not in traits allowed index range
			    {
                    gv.cc.DisposeOfBitmap(ref btn.Img);
                    btn.Img = gv.cc.LoadBitmap("btn_small_off"); 
				    btn.Img2 = null;
                    btn.Img3 = null;
			    }			
			    btn.Draw();
			    cntSlot++;
		    }
		
		    //DRAW DESCRIPTION BOX
		    locY = tabStartY;		
		    if (isSelectedTraitSlotInKnownTraitsRange())
		    {
                Trait tr = GetCurrentlySelectedTrait();
                //string textToSpan = "<u>Description</u>" + "<BR>" + "<BR>";
                string textToSpan = "<b><big>" + tr.name + "</big></b><BR>";
                textToSpan += "Available at Level: " + getLevelAvailable(tr.tag) + "<BR>";
         
                if (tr.prerequisiteTrait != "none")
                {
                    foreach (Trait t in gv.mod.moduleTraitsList)
                    {
                        if (t.tag == tr.prerequisiteTrait)
                        {
                            textToSpan += "Requires: " + t.name + "<BR>";
                            break;
                        }
                    }
                }

                if (tr.requiredStrength > 0)
                {
                    textToSpan += "Required STR: " + tr.requiredStrength + "<BR>"; 
                }
                if (tr.requiredDexterity > 0)
                {
                    textToSpan += "Required DEX: " + tr.requiredDexterity + "<BR>";
                }
                if (tr.requiredConstitution > 0)
                {
                    textToSpan += "Required CON: " + tr.requiredConstitution + "<BR>";
                }
                if (tr.requiredIntelligence > 0)
                {
                    textToSpan += "Required INT: " + tr.requiredIntelligence + "<BR>";
                }
                if (tr.requiredWisdom > 0)
                {
                    textToSpan += "Required WIS: " + tr.requiredWisdom + "<BR>";
                }
                if (tr.requiredCharisma > 0)
                {
                    textToSpan += "Required CHA: " + tr.requiredCharisma + "<BR>";
                }
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
                btnTokensLeft.Draw();
                btnTokensRight.Draw(); 
                btnPageIndex.Draw();
        //todo draw other buttons
        //zeke2
    }
            else
            {
                btnSelect.Text = "LEARN SELECTED " + pc.playerClass.traitLabelSingular.ToUpper();
                btnHelp.Draw();
                btnExit.Draw();
                btnSelect.Draw();
                //todo draw other buttons
                btnTokensLeft.Draw();
                btnTokensRight.Draw();
                btnPageIndex.Draw();
            }
        }
        public void onTouchTraitLevelUp(MouseEventArgs e, MouseEventType.EventType eventType, bool inPcCreation, bool inCombat)
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
                            if (btnTraitSlots[j].getImpact(x, y))
                            {
                                gv.PlaySound("btn_click");
                                traitSlotIndex = j;
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
                                gv.screenParty.traitGained = "";
                                gv.screenParty.spellGained = "";
                                pc.learningTraitsTags.Clear();
                                pc.learningEffects.Clear();
                                pc.learningSpellsTags.Clear();
                                traitToLearnIndex = 1;
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
            catch
            { }
	    }
    
        public void doSelectedTraitToLearn(bool inPcCreation)
        {
    	    if (isSelectedTraitSlotInKnownTraitsRange())
		    {
			    Trait tr = GetCurrentlySelectedTrait();

                //checking attribute requiremnts of trait
                bool attributeRequirementsMet = checkAttributeRequirementsOfTrait(pc, tr);

			    if (isAvailableToLearn(tr.tag) && attributeRequirementsMet)
			    {
                    //add trait
                    //pc.knownTraitsTags.Add(tr.tag);
                    pc.learningTraitsTags.Add(tr.tag);
                    sortTraitsForLevelUp(pc);
                    foreach (EffectTagForDropDownList etfddl in tr.traitEffectTagList)
                    {
                        foreach (Effect e in gv.mod.moduleEffectsList)
                        {
                            if (e.tag == etfddl.tag)
                            {
                                if (e.isPermanent)
                                {
                                    pc.learningEffects.Add(e);
                                }
                            }
                        }
                    }
                    //public string useableInSituation = "Always"; //InCombat, OutOfCombat, Always, Passive
                    //note: might have to do this on exit
                    /*
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
                    */

                    /*
                    //add permanent effects of trait to effect list of this pc
                    foreach (EffectTagForDropDownList efTag in tr.traitEffectTagList)
                    {//1
                        foreach (Effect ef in gv.gv.mod.gv.moduleEffectsList)
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
                                    {//6
                                        pc.effectsList.Add(ef);
                                        gv.sf.UpdateStats(pc);
                                        if (ef.gv.modifyHpMax != 0)
                                        {//7
                                            pc.hp += ef.gv.modifyHpMax;
                                            if (pc.hp < 1)
                                            {//8
                                                pc.hp = 1;
                                            }//8
                                            if (pc.hp > pc.hpMax)
                                            {
                                                pc.hp = pc.hpMax;
                                            }
                                        }//7

                                        if (ef.gv.modifyCon != 0)
                                        {//7
                                            pc.hp += ef.gv.modifyCon / 2;
                                            if (pc.hp < 1)
                                            {//8
                                                pc.hp = 1;
                                            }//8
                                            if (pc.hp > pc.hpMax)
                                            {
                                                pc.hp = pc.hpMax;
                                            }
                                        }//7

                                        if (ef.gv.modifySpMax != 0)
                                        {
                                            pc.sp += ef.gv.modifySpMax;
                                            if (pc.sp < 1)
                                            {
                                                pc.sp = 1;
                                            }
                                            if (pc.sp > pc.spMax)
                                            {
                                                pc.sp = pc.spMax;
                                            }
                                        }

                                        if (ef.gv.modifyStr != 0)
                                        {
                                            if (pc.playerClass.gv.modifierFromSPRelevantAttribute.Equals("strength"))
                                            {
                                                pc.sp += ef.gv.modifyStr / 2;
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

                                        if (ef.gv.modifyDex != 0)
                                        {
                                            if (pc.playerClass.gv.modifierFromSPRelevantAttribute.Equals("dexterity"))
                                            {
                                                pc.sp += ef.gv.modifyDex / 2;
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

                                        if (ef.gv.modifyCon != 0)
                                        {
                                            if (pc.playerClass.gv.modifierFromSPRelevantAttribute.Equals("constitution"))
                                            {
                                                pc.sp += ef.gv.modifyCon / 2;
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

                                        if (ef.gv.modifyCha != 0)
                                        {
                                            if (pc.playerClass.gv.modifierFromSPRelevantAttribute.Equals("charisma"))
                                            {
                                                pc.sp += ef.gv.modifyCha / 2;
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

                                        if (ef.gv.modifyInt != 0)
                                        {
                                            if (pc.playerClass.gv.modifierFromSPRelevantAttribute.Equals("intelligence"))
                                            {
                                                pc.sp += ef.gv.modifyInt / 2;
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

                                        if (ef.gv.modifyWis != 0)
                                        {
                                            if (pc.playerClass.gv.modifierFromSPRelevantAttribute.Equals("wisdom"))
                                            {
                                                pc.sp += ef.gv.modifyWis / 2;
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
                                    }//6
                                }//5
                            }//4
                        }//3
                    }//2 (is 1 actually)
                    */

                    /*
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
                    */

                    //check to see if there are more traits to learn at this level  
                    traitToLearnIndex++;
                    if (traitToLearnIndex <= gv.mod.getPlayerClass(pc.classTag).traitsToLearnAtLevelTable[pc.classLevel])
                    {
                        gv.screenParty.traitGained += tr.name + ", ";
                    }
                    else //finished learning all traits available for this level  
                    {
                        //else if in creation go back to partybuild				  
                        if (inPcCreation)
                        {
                            //if there are spells to learn go to spell screen next  
                            List < string > spellTagsList = new List<string>();
                            spellTagsList = pc.getSpellsToLearn();
                            if (spellTagsList.Count > 0)
                            {
                                gv.screenSpellLevelUp.resetPC(false, pc, false);
                                gv.screenType = "learnSpellCreation";
                            }
                            else //no spells to learn  
                            {
                                //save character, add them to the pcList of screenPartyBuild, and go back to build screen
                                foreach (string s in pc.learningTraitsTags)
                                {
                                    pc.knownTraitsTags.Add(s);
                                    //TODO: must get trait by ts string
                                    foreach (Trait t in gv.mod.moduleTraitsList)
                                    {
                                        if (t.tag == s)
                                        {
                                            tr = t;
                                            break;
                                        }
                                    }

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
                                    {//6
                                        pc.effectsList.Add(ef);
                                        gv.sf.UpdateStats(pc);
                                        if (ef.modifyHpMax != 0)
                                        {//7
                                            pc.hp += ef.modifyHpMax;
                                            if (pc.hp < 1)
                                            {//8
                                                pc.hp = 1;
                                            }//8
                                            if (pc.hp > pc.hpMax)
                                            {
                                                pc.hp = pc.hpMax;
                                            }
                                        }//7

                                        if (ef.modifyCon != 0)
                                        {//7
                                            pc.hp += ef.modifyCon / 2;
                                            if (pc.hp < 1)
                                            {//8
                                                pc.hp = 1;
                                            }//8
                                            if (pc.hp > pc.hpMax)
                                            {
                                                pc.hp = pc.hpMax;
                                            }
                                        }//7

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

                                }
                                //note2
                                gv.screenPcCreation.SaveCharacter(pc);
                                gv.screenPartyBuild.pcList.Add(pc);
                                gv.screenType = "partyBuild";
                             }
                         }
                         else  
                         {
                            //if there are spells to learn go to spell screen next  
                            List < string > spellTagsList = new List<string>();
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
                }
                else
			    {
				    gv.sf.MessageBox("Can't learn that trait, try another or exit");
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
            sortTraitsForLevelUp(pc);
            //TraitAllowed ta = pc.playerClass.traitsAllowed[traitSlotIndex + (tknPageIndex * slotsPerPage)];
            TraitAllowed ta = backupTraitsAllowed[traitSlotIndex + (tknPageIndex * slotsPerPage)];
            return gv.mod.getTraitByTag(ta.tag);
	    }
	    public bool isSelectedTraitSlotInKnownTraitsRange()
	    {
            //return (traitSlotIndex + (tknPageIndex * slotsPerPage)) < pc.playerClass.traitsAllowed.Count;
            sortTraitsForLevelUp(pc);
            return (traitSlotIndex + (tknPageIndex * slotsPerPage)) < backupTraitsAllowed.Count;
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
