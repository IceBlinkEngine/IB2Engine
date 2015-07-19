using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class ScreenPcCreation 
    {
	    public Module mod;
	    public GameView gv;

        //private IbbButton ctrlUpArrow = null;
        //private IbbButton ctrlDownArrow = null;
        //private IbbButton ctrlLeftArrow = null;
        //private IbbButton ctrlRightArrow = null;
        private IbbButton btnName = null;
        private IbbButton btnRace = null;
        private IbbButton btnClass = null;
        private IbbButton btnGender = null;
        private IbbPortrait btnPortrait = null;
        private IbbButton btnToken = null;

	    private IbbButton btnPlayerGuideOnPcCreation = null;
	    private IbbButton btnBeginnerGuideOnPcCreation = null;
	    private IbbButton btnRollStats = null;
	    private IbbButton btnFinished = null;
	    private IbbButton btnAbort = null;
        
        private Bitmap blankItemSlot;
	    private int pcCreationIndex = 0;
	    private int pcTokenSelectionIndex = 0;
        private int pcPortraitSelectionIndex = 0;
	    private int pcRaceSelectionIndex = 0;
	    private int pcClassSelectionIndex = 0;
	    public List<string> playerTokenList = new List<string>();
        public List<string> playerPortraitList = new List<string>();
        public List<Race> playerRaces = new List<Race>();
        public Player pc;
	
	    public ScreenPcCreation(Module m, GameView g)
	    {
		    mod = m;
		    gv = g;
            blankItemSlot = gv.cc.LoadBitmap("item_slot");
		    LoadPlayerBitmapList();
            LoadPlayerPortraitList();            
            CreateRaceList();
		    resetPC();
            setControlsStart();
	    }
	
	    public void resetPC()
	    {
		    pc = gv.cc.LoadPlayer(gv.mod.defaultPlayerFilename);
		    pc.token = gv.cc.LoadBitmap(pc.tokenFilename);
            pc.portrait = gv.cc.LoadBitmap(pc.portraitFilename);
		    pc.playerClass = mod.getPlayerClass(pc.classTag);
		    pc.race = this.getAllowedRace(pc.raceTag);
		    pc.name = "CharacterName";
            pc.tag = "characterName";
		    pcCreationIndex = 0;
	    }
        public void CreateRaceList()
        {
            //Create Race List
            playerRaces.Clear();
            foreach (Race rc in mod.moduleRacesList)
            {
                if (rc.UsableByPlayer)
                {
                    Race newRace = rc.DeepCopy();
                    newRace.classesAllowed.Clear();
                    foreach (string s in rc.classesAllowed)
                    {
                        PlayerClass plc = mod.getPlayerClass(s);
                        if ((plc != null) && (plc.UsableByPlayer))
                        {
                            newRace.classesAllowed.Add(s);
                        }
                    }
                    playerRaces.Add(newRace);
                }
            }
        }
        public Race getAllowedRace(string tag)
        {
            foreach (Race r in this.playerRaces)
            {
                if (r.tag.Equals(tag)) return r;
            }
            return null;
        }
	    public void LoadPlayerBitmapList()
	    {            
		    playerTokenList.Clear();	
		    try
		    {
                //Load from module folder first
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\pctokens"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\pctokens", "*.png");
                    //directory.mkdirs(); 
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if (filename.EndsWith("_pc.png"))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                playerTokenList.Add(fileNameWithOutExt);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }			    			    
		    }
		    catch (Exception ex)
    	    {
                MessageBox.Show(ex.ToString());
    	    }
		    try
		    {
			    //Load from PlayerTokens folder last
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\PlayerTokens"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\PlayerTokens", "*.png");
                    //directory.mkdirs(); 
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if (filename.EndsWith("_pc.png"))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                if (!playerTokenList.Contains(fileNameWithOutExt))
                                {
                                    playerTokenList.Add(fileNameWithOutExt);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
		    }
		    catch (Exception ex)
    	    {
                MessageBox.Show(ex.ToString());
    	    }
	    }
        public void LoadPlayerPortraitList()
        {
            playerPortraitList.Clear();
            try
            {
                //Load from module folder first
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\portraits"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\portraits", "*.png");
                    //directory.mkdirs(); 
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if ((filename.EndsWith(".png")) || (filename.EndsWith(".PNG")))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                playerPortraitList.Add(fileNameWithOutExt);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            try
            {
                //Load from PlayerTokens folder last
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\PlayerPortraits"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\PlayerPortraits", "*.png");
                    //directory.mkdirs(); 
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if ((filename.EndsWith(".png")) || (filename.EndsWith(".PNG")))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                if (!playerPortraitList.Contains(fileNameWithOutExt))
                                {
                                    playerPortraitList.Add(fileNameWithOutExt);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
	
	    public void setControlsStart()
	    {		
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;
            int center = gv.screenWidth / 2;

            if (btnPortrait == null)
            {
                btnPortrait = new IbbPortrait(gv, 1.0f);
                btnPortrait.ImgBG = gv.cc.LoadBitmap("item_slot");
                btnPortrait.Glow = gv.cc.LoadBitmap("btn_small_glow");
                btnPortrait.X = 10 * gv.squareSize;
                btnPortrait.Y = 1 * gv.squareSize + pH * 2;
                btnPortrait.Height = (int)(pc.portrait.Height * gv.screenDensity);
                btnPortrait.Width = (int)(pc.portrait.Width * gv.screenDensity);
            }
            if (btnToken == null)
            {
                btnToken = new IbbButton(gv, 1.0f);
                btnToken.Img = gv.cc.LoadBitmap("item_slot");
                btnToken.Img2 = gv.cc.LoadBitmap(pc.tokenFilename);
                btnToken.Glow = gv.cc.LoadBitmap("btn_small_glow");
                btnToken.X = 12 * gv.squareSize;
                btnToken.Y = 1 * gv.squareSize + pH * 2;
                btnToken.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnToken.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnName == null)
            {
                btnName = new IbbButton(gv, 1.0f);
                btnName.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnName.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnName.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
                btnName.Y = 0 * gv.squareSize + gv.squareSize / 2;
                btnName.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnName.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnRace == null)
            {
                btnRace = new IbbButton(gv, 1.0f);
                btnRace.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnRace.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnRace.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
                btnRace.Y = 1 * gv.squareSize + gv.squareSize / 2;
                btnRace.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRace.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnGender == null)
            {
                btnGender = new IbbButton(gv, 1.0f);
                btnGender.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnGender.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnGender.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
                btnGender.Y = 2 * gv.squareSize + gv.squareSize / 2;
                btnGender.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnGender.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnClass == null)
            {
                btnClass = new IbbButton(gv, 1.0f);
                btnClass.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnClass.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnClass.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
                btnClass.Y = 3 * gv.squareSize + gv.squareSize / 2;
                btnClass.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnClass.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
		    if (btnPlayerGuideOnPcCreation == null)
		    {
			    btnPlayerGuideOnPcCreation = new IbbButton(gv, 1.0f);	
			    btnPlayerGuideOnPcCreation.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnPlayerGuideOnPcCreation.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnPlayerGuideOnPcCreation.Text = "Player's Guide";
                btnPlayerGuideOnPcCreation.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
			    btnPlayerGuideOnPcCreation.Y = 7 * gv.squareSize;
                btnPlayerGuideOnPcCreation.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPlayerGuideOnPcCreation.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnBeginnerGuideOnPcCreation == null)
		    {
			    btnBeginnerGuideOnPcCreation = new IbbButton(gv, 1.0f);	
			    btnBeginnerGuideOnPcCreation.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnBeginnerGuideOnPcCreation.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnBeginnerGuideOnPcCreation.Text = "Beginner's Guide";
                btnBeginnerGuideOnPcCreation.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
			    btnBeginnerGuideOnPcCreation.Y = 8 * gv.squareSize + pH;
                btnBeginnerGuideOnPcCreation.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnBeginnerGuideOnPcCreation.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnRollStats == null)
		    {
			    btnRollStats = new IbbButton(gv, 1.0f);	
			    btnRollStats.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnRollStats.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnRollStats.Text = "Roll Stats";
                btnRollStats.X = center + pW * 1;
                btnRollStats.Y = 7 * gv.squareSize;
                btnRollStats.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRollStats.Width = (int)(gv.ibbwidthL * gv.screenDensity);
		    }
		    if (btnFinished == null)
		    {
			    btnFinished = new IbbButton(gv, 1.0f);	
			    btnFinished.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnFinished.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnFinished.Text = "Finished";
                btnFinished.X = center + pW * 1;
			    btnFinished.Y = 8 * gv.squareSize + pH;
                btnFinished.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnFinished.Width = (int)(gv.ibbwidthL * gv.screenDensity);
		    }
		    if (btnAbort == null)
		    {
			    btnAbort = new IbbButton(gv, 0.8f);	
			    btnAbort.Text = "Abort";
			    btnAbort.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
			    btnAbort.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnAbort.X = 8 * gv.squareSize + padW * 1 + gv.oXshift;
			    btnAbort.Y = 9 * gv.squareSize + pH * 2;
                btnAbort.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAbort.Width = (int)(gv.ibbwidthR * gv.screenDensity);			
		    }		    
	    }

	    public void redrawPcCreation()
        {
            //Player pc = mod.playerList.get(0);
    	    gv.sf.UpdateStats(pc);
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);

            int locX = 6 * gv.squareSize;
            //int textH = (int)gv.mSheetTextPaint.getTextSize();
    	    //int spacing = (int)gv.mSheetTextPaint.getTextSize() + pH;            
            int textH = (int)gv.cc.MeasureString("GetHeight", gv.drawFontReg, gv.Width).Height;
            int spacing = textH;
            int locY = 4 * gv.squareSize + gv.squareSize / 4;
    	    int tabX = pW * 50;
    	    int tabX2 = pW * 50;
    	    int leftStartY = pH * 20;
    	    int tokenStartX = locX + (textH * 5);
    	    int tokenStartY = pH * 5 + (spacing/2);
            int portraitStartX = 12 * gv.squareSize + (textH * 5);
            int portraitStartY = pH * 5 + (spacing / 2);
    	    int tokenRectPad = pW * 1;
    	
		    //Page Title
		    gv.DrawText("CREATE CHARACTER", pW * 40, pH * 1);
						    
            Color color = Color.White;
            
            gv.DrawText("STR: " + pc.baseStr + " + " + (pc.strength - pc.baseStr) + " = " + pc.strength, locX, locY += spacing);
            gv.DrawText("AC: " + pc.AC, tabX2, locY);
            gv.DrawText("DEX: " + pc.baseDex + " + " + (pc.dexterity - pc.baseDex) + " = " + pc.dexterity, locX, locY += spacing);
            gv.DrawText("HP: " + pc.hp + "/" + pc.hpMax, tabX2, locY);
            gv.DrawText("CON:  " + pc.baseCon + " + " + (pc.constitution - pc.baseCon) + " = " + pc.constitution, locX, locY += spacing);
            gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, tabX2, locY);
            gv.DrawText("INT:  " + pc.baseInt + " + " + (pc.intelligence - pc.baseInt) + " = " + pc.intelligence, locX, locY += spacing);
            gv.DrawText("BAB: " + pc.baseAttBonus, tabX2, locY);
            gv.DrawText("WIS: " + pc.baseWis + " + " + (pc.wisdom - pc.baseWis) + " = " + pc.wisdom, locX, locY += spacing);
            gv.DrawText("CHA: " + pc.baseCha + " + " + (pc.charisma - pc.baseCha) + " = " + pc.charisma, locX, locY += spacing);

            //Description
		    string textToSpan = "";
            if (pcCreationIndex == 2) 
            { 
        	    textToSpan = "Description:" + Environment.NewLine;
                //textToSpan = "<u>Description</u>" + "<BR>";
        	    textToSpan += pc.race.description;
            }
            else if (pcCreationIndex == 4) 
            {
                textToSpan = "Description:" + Environment.NewLine;
        	    textToSpan += pc.playerClass.description;
            }
            int yLoc = 3 * gv.squareSize;
            IbRect rect = new IbRect(tabX, yLoc, pW * 35, pH * 50);
            gv.DrawText(textToSpan, rect, 1.0f, Color.White);

            btnPortrait.Img = pc.portrait;
            btnPortrait.Draw();
            btnToken.Draw();
            btnName.Text = pc.name;
            btnName.Draw();
            btnRace.Text = pc.race.name;
            btnRace.Draw();
            if (pc.isMale)
            {
                btnGender.Text = "Male";
            }
            else
            {
                btnGender.Text = "Female";
            }
            btnGender.Draw();
            btnClass.Text = pc.playerClass.name;
            btnClass.Draw();
            
            btnRollStats.Draw();
		    btnFinished.Draw();
		    gv.cc.btnHelp.Draw();
		    btnAbort.Draw();
		    btnPlayerGuideOnPcCreation.Draw();
		    btnBeginnerGuideOnPcCreation.Draw();
        }    
        public void onTouchPcCreation(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
    	    //Player pc = mod.playerList.get(0);
    	
    	    //ctrlUpArrow.glowOn = false;
    	    //ctrlDownArrow.glowOn = false;
    	    //ctrlLeftArrow.glowOn = false;
    	    //ctrlRightArrow.glowOn = false;
		    btnRollStats.glowOn = false;
		    btnFinished.glowOn = false;	
		    btnAbort.glowOn = false;	
		    gv.cc.btnHelp.glowOn = false;
		    btnPlayerGuideOnPcCreation.glowOn = false;
		    btnBeginnerGuideOnPcCreation.glowOn = false;
		
		    //int eventAction = event.getAction();
		    switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) e.X;
			    int y = (int) e.Y;
			    /*if (ctrlUpArrow.getImpact(x, y))
			    {
				    ctrlUpArrow.glowOn = true;
			    }
			    else if (ctrlDownArrow.getImpact(x, y))
			    {
				    ctrlDownArrow.glowOn = true;
			    }
			    else if (ctrlLeftArrow.getImpact(x, y))
			    {
				    ctrlLeftArrow.glowOn = true;
			    }
			    else if (ctrlRightArrow.getImpact(x, y))
			    {
				    ctrlRightArrow.glowOn = true;
			    }*/	
			    if (btnRollStats.getImpact(x, y))
			    {
				    btnRollStats.glowOn = true;
			    }
			    else if (btnFinished.getImpact(x, y))
			    {
				    btnFinished.glowOn = true;			
			    }
			    else if (btnAbort.getImpact(x, y))
			    {
				    btnAbort.glowOn = true;			
			    }
			    else if (gv.cc.btnHelp.getImpact(x, y))
			    {
				    gv.cc.btnHelp.glowOn = true;
			    }
			    else if (btnPlayerGuideOnPcCreation.getImpact(x, y))
			    {
				    btnPlayerGuideOnPcCreation.glowOn = true;
			    }
			    else if (btnBeginnerGuideOnPcCreation.getImpact(x, y))
			    {
				    btnBeginnerGuideOnPcCreation.glowOn = true;
			    }
			    break;	
			
		    case MouseEventType.EventType.MouseUp:
                x = (int)e.X;
                y = (int)e.Y;
			
			    //ctrlUpArrow.glowOn = false;
			    //ctrlDownArrow.glowOn = false;
			    //ctrlLeftArrow.glowOn = false;
			    //ctrlRightArrow.glowOn = false;
			    btnRollStats.glowOn = false;
			    btnFinished.glowOn = false;	
			    btnAbort.glowOn = false;
			    gv.cc.btnHelp.glowOn = false;
			    btnPlayerGuideOnPcCreation.glowOn = false;
			    btnBeginnerGuideOnPcCreation.glowOn = false;
			
                if (btnName.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    pcCreationIndex = 1;
                    changePcName();
			    }
                else if (btnRace.getImpact(x, y))
                {
                    gv.PlaySound("btn_click");
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    pcCreationIndex = 2; 
                    pcRaceSelectionIndex++;
                    if (pcRaceSelectionIndex >= this.playerRaces.Count)
                    {
                        pcRaceSelectionIndex = 0;
                    }
                    pc.race = playerRaces[pcRaceSelectionIndex];
                    pc.raceTag = pc.race.tag;
                    resetClassSelection(pc);
                    gv.sf.UpdateStats(pc);
                    pc.hp = pc.hpMax;
                    pc.sp = pc.spMax;
                }
                else if (btnGender.getImpact(x, y))
                {
                    gv.PlaySound("btn_click");
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    pcCreationIndex = 3; 
                    if (pc.isMale)
                    {
                        pc.isMale = false;
                    }
                    else
                    {
                        pc.isMale = true;
                    }
                }
                else if (btnClass.getImpact(x, y))
                {
                    gv.PlaySound("btn_click");
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    pcCreationIndex = 4;
                    pcClassSelectionIndex++;
                    if (pcClassSelectionIndex >= pc.race.classesAllowed.Count)
                    {
                        pcClassSelectionIndex = 0;
                    }
                    pc.playerClass = mod.getPlayerClass(pc.race.classesAllowed[pcClassSelectionIndex]);
                    pc.classTag = pc.playerClass.tag;
                    gv.sf.UpdateStats(pc);
                    pc.hp = pc.hpMax;
                    pc.sp = pc.spMax;
                }
                else if (btnPortrait.getImpact(x, y))
                {
                    //pass items to selector
                    gv.screenType = "portraitSelector";
                    gv.screenPortraitSelector.resetPortraitSelector("pcCreation", pc);
                }
                else if (btnToken.getImpact(x, y))
                {
                    gv.screenType = "tokenSelector";
                    gv.screenTokenSelector.resetTokenSelector("pcCreation", pc);
                    /*if (pcTokenSelectionIndex < playerTokenList.Count - 1)
                    {
                        pcTokenSelectionIndex++;
                        tokenLoad(pc);
                    }
                    else
                    {
                        pcTokenSelectionIndex = 0;
                        tokenLoad(pc);	
                    }*/
                }

			    /*if (ctrlUpArrow.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (pcCreationIndex > 0)
				    {
					    pcCreationIndex--;
				    }
			    }
			    else if (ctrlDownArrow.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (pcCreationIndex < 4)
				    {
					    pcCreationIndex++;
				    }
			    }
			    else if (ctrlLeftArrow.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    switch (pcCreationIndex)
				    {
					    case 0:
						    if (pcTokenSelectionIndex > 0)
						    {
							    pcTokenSelectionIndex--;
							    tokenLoad(pc);	
						    }											
						    break;
					    case 1:
						    changePcName();
						    break;
					    case 2:
						    if (pcRaceSelectionIndex > 0)
						    {
							    pcRaceSelectionIndex--;
							    //pc.race = mod.moduleRacesList.get(pcRaceSelectionIndex);
                                pc.race = playerRaces[pcRaceSelectionIndex];
							    pc.raceTag = pc.race.tag;
							    //changeClassIfDwarf(pc);
							    resetClassSelection(pc);
							    gv.sf.UpdateStats(pc);
							    pc.hp = pc.hpMax;
							    pc.sp = pc.spMax;
						    }						
						    break;
					    case 3:
						    if (pc.isMale)
						    {
							    pc.isMale = false;
						    }
						    else
						    {
							    pc.isMale = true;
						    }
						    break;
					    case 4:
						    //do stuff
						    if (pcClassSelectionIndex > 0)
						    {
							    pcClassSelectionIndex--;
							    //if raceIndex = 1 (dwarf) then class index is now 0 (human)
							    //if (pcRaceSelectionIndex == 1) {pcClassSelectionIndex = 0;}
							    pc.playerClass = mod.getPlayerClass(pc.race.classesAllowed[pcClassSelectionIndex]);
                                //pc.playerClass = mod.modulePlayerClassList.get(pcClassSelectionIndex);
							    pc.classTag = pc.playerClass.tag;
							    gv.sf.UpdateStats(pc);
					    	    pc.hp = pc.hpMax;
							    pc.sp = pc.spMax;
						    }
						    break;
				    }
			    }
                else if (ctrlRightArrow.getImpact(x, y))
                {
                    gv.PlaySound("btn_click");
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    switch (pcCreationIndex)
                    {
                        case 0:
                            if (pcTokenSelectionIndex < playerTokenList.Count - 1)
                            {
                                pcTokenSelectionIndex++;
                                tokenLoad(pc);
                            }
                            break;
                        case 1:
                            changePcName();
                            break;
                        case 2:
                            //if (pcRaceSelectionIndex < mod.moduleRacesList.size()-1)
                            if (pcRaceSelectionIndex < this.playerRaces.Count - 1)
                            {
                                pcRaceSelectionIndex++;
                                //pc.race = mod.moduleRacesList.get(pcRaceSelectionIndex);
                                pc.race = playerRaces[pcRaceSelectionIndex];
                                pc.raceTag = pc.race.tag;
                                //changeClassIfDwarf(pc);
                                resetClassSelection(pc);
                                gv.sf.UpdateStats(pc);
                                pc.hp = pc.hpMax;
                                pc.sp = pc.spMax;
                            }
                            break;
                        case 3:
                            if (pc.isMale)
                            {
                                pc.isMale = false;
                            }
                            else
                            {
                                pc.isMale = true;
                            }
                            break;
                        case 4:
                            //do stuff
                            if (pcClassSelectionIndex < pc.race.classesAllowed.Count - 1)
                            {
                                pcClassSelectionIndex++;
                                //if raceIndex = 1 (dwarf) then class index is now 1 (cleric)
                                //if (pcRaceSelectionIndex == 1) {pcClassSelectionIndex = 1;}
                                pc.playerClass = mod.getPlayerClass(pc.race.classesAllowed[pcClassSelectionIndex]);
                                //pc.playerClass = mod.modulePlayerClassList.get(pcClassSelectionIndex);
                                pc.classTag = pc.playerClass.tag;
                                gv.sf.UpdateStats(pc);
                                pc.hp = pc.hpMax;
                                pc.sp = pc.spMax;
                            }
                            break;
                    }
                }*/
                else if (btnRollStats.getImpact(x, y))
                {
                    gv.PlaySound("btn_click");
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    reRollStats(pc);
                }
                else if (btnFinished.getImpact(x, y))
                {
                    gv.PlaySound("btn_click");
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}

                    //if automatically learned traits or spells add them
                    foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                    {
                        if ((ta.automaticallyLearned) && (ta.atWhatLevelIsAvailable == pc.classLevel))
                        {
                            pc.knownTraitsTags.Add(ta.tag);
                        }
                    }
                    foreach (SpellAllowed sa in pc.playerClass.spellsAllowed)
                    {
                        if ((sa.automaticallyLearned) && (sa.atWhatLevelIsAvailable == pc.classLevel))
                        {
                            pc.knownSpellsTags.Add(sa.tag);
                        }
                    }

                    //check to see if have any traits to learn
                    List<string> traitTagsList = new List<string>();
                    traitTagsList = pc.getTraitsToLearn(gv.mod);

                    //check to see if have any spells to learn
                    List<string> spellTagsList = new List<string>();
                    spellTagsList = pc.getSpellsToLearn();

                    if (traitTagsList.Count > 0)
                    {
                        gv.screenTraitLevelUp.resetPC(pc);
                        gv.screenType = "learnTraitCreation";
                    }

                    else if (spellTagsList.Count > 0)
                    {
                        gv.screenSpellLevelUp.resetPC(pc);
                        gv.screenType = "learnSpellCreation";
                    }
                    else
                    {
                        //no spells or traits to learn
                        //save character, add them to the pcList of screenPartyBuild, and go back to build screen
                        this.SaveCharacter(pc);
                        gv.screenPartyBuild.pcList.Add(pc);
                        gv.screenType = "partyBuild";

                        /* old stuff, keep for now
                        gv.cc.tutorialMessageMainMap();
                        gv.screenType = "main";
                        gv.cc.doUpdate();*/
                    }
                }
                else if (btnAbort.getImpact(x, y))
                {
                    gv.PlaySound("btn_click");
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}				
                    gv.screenType = "partyBuild";
                }
                else if (gv.cc.btnHelp.getImpact(x, y))
                {
                    gv.PlaySound("btn_click");
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    gv.cc.tutorialPcCreation();
                }
                else if (btnPlayerGuideOnPcCreation.getImpact(x, y))
                {
                    gv.PlaySound("btn_click");
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    //doPlayersGuideDialog();
                    gv.cc.tutorialPlayersGuide();
                }
                else if (btnBeginnerGuideOnPcCreation.getImpact(x, y))
                {
                    gv.PlaySound("btn_click");
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    //doPlayersGuideDialog();
                    gv.cc.tutorialBeginnersGuide();
                }                
			    break;
		    }
	    }
        
        public void tokenLoad(Player p)
        {
    	    //p.tokenFilename = playerTokenList[pcTokenSelectionIndex];
    	    p.token = gv.cc.LoadBitmap(p.tokenFilename);
            btnToken.Img2 = p.token;
        }
        public void portraitLoad(Player p)
        {
            //p.portraitFilename = playerPortraitList[pcPortraitSelectionIndex];
            p.portrait = gv.cc.LoadBitmap(p.portraitFilename);
            btnPortrait.Img = p.portrait;
        }
        public void changePcName()
        {
            using (TextInputDialog itSel = new TextInputDialog(gv, "Choose a Name for this PC."))
            {
                itSel.IceBlinkButtonClose.Visible = true;
                itSel.IceBlinkButtonClose.Enabled = true;
                itSel.textInput = "Type Name Here";

                var ret = itSel.ShowDialog();

                if (ret == DialogResult.OK)
                {
                    if (itSel.textInput.Length > 0)
                    {
                        pc.name = itSel.textInput;
                        pc.tag = itSel.textInput.ToLower();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Entering a blank name is not allowed", Toast.LENGTH_SHORT).show();
                    }
                }
            }
    	    /*AlertDialog.Builder builder = new AlertDialog.Builder(gv.gameContext);
    	    builder.setTitle("Enter Character's Name");

    	    // Set up the input
    	    final EditText input = new EditText(gv.gameContext);
    	    // Specify the type of input expected
    	    input.setInputType(InputType.TYPE_CLASS_TEXT);
    	    builder.setView(input);

    	    // Set up the buttons
    	    builder.setPositiveButton("OK", new DialogInterface.OnClickListener() 
    	    { 
    	        @Override
    	        public void onClick(DialogInterface dialog, int which) 
    	        {
    	    	    if (input.getText().toString().length() > 0)
    	    	    {
    	    		    //Player pc = mod.playerList.get(0);
    	    		    pc.name = input.getText().toString();
    	    		    pc.tag = pc.name.toLowerCase(Locale.ENGLISH);
    	    	    }
    	    	    else
    	    	    {
    	    		    Toast.makeText(gv.gameContext, "Entering a blank name is not allowed", Toast.LENGTH_SHORT).show();
    	    	    }
    	        }
    	    });
    	
    	    builder.setNegativeButton("Cancel", new DialogInterface.OnClickListener() 
    	    {
    	        @Override
    	        public void onClick(DialogInterface dialog, int which) 
    	        {
    	            dialog.cancel();
    	        }
    	    });
    	
    	    builder.show();
            */
        }
        public void reRollStats(Player p)
        {
    	    p.baseStr = 6 + gv.sf.RandInt(12);
    	    p.baseDex = 6 + gv.sf.RandInt(12);
    	    p.baseInt = 6 + gv.sf.RandInt(12);
    	    p.baseCha = 6 + gv.sf.RandInt(12);
            p.baseCon = 6 + gv.sf.RandInt(12);
            p.baseWis = 6 + gv.sf.RandInt(12);
    	    gv.sf.UpdateStats(p);
    	    p.hp = p.hpMax;
		    p.sp = p.spMax;		
        }
        public void resetClassSelection(Player p)
        {
    	    pcClassSelectionIndex = 0;
		    p.playerClass = mod.getPlayerClass(p.race.classesAllowed[pcClassSelectionIndex]);
		    p.classTag = p.playerClass.tag;
		    gv.sf.UpdateStats(p);
    	    p.hp = p.hpMax;
		    p.sp = p.spMax;
        }
        public void SaveCharacter(Player p)
	    {
            string filename = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\characters\\" + pc.tag + ".json";
            gv.cc.MakeDirectoryIfDoesntExist(filename);
            string json = JsonConvert.SerializeObject(pc, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(json.ToString());
            }
	    }        
    }
}
