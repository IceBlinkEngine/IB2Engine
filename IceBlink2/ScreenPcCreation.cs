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

        private IbbButton ctrlUpArrow = null;
        private IbbButton ctrlDownArrow = null;
        private IbbButton ctrlLeftArrow = null;
        private IbbButton ctrlRightArrow = null;
	    private IbbButton btnPlayerGuideOnPcCreation = null;
	    private IbbButton btnBeginnerGuideOnPcCreation = null;
	    private IbbButton btnRollStats = null;
	    private IbbButton btnFinished = null;
	    private IbbButton btnAbort = null;
        private Bitmap blankItemSlot;
	    private int pcCreationIndex = 0;
	    private int pcTokenSelectionIndex = 0;
	    private int pcRaceSelectionIndex = 0;
	    private int pcClassSelectionIndex = 0;
	    public List<string> playerTokenList = new List<string>();
        public List<Race> playerRaces = new List<Race>();
        private Player pc;
	
	    public ScreenPcCreation(Module m, GameView g)
	    {
		    mod = m;
		    gv = g;
            blankItemSlot = gv.cc.LoadBitmap("item_slot");
		    setControlsStart();
		    LoadPlayerBitmapList();
            CreateRaceList();
		    resetPC();
	    }
	
	    public void resetPC()
	    {
		    pc = gv.cc.LoadPlayer(gv.mod.defaultPlayerFilename);
		    pc.token = gv.cc.LoadBitmap(pc.tokenFilename);
		    pc.playerClass = mod.getPlayerClass(pc.classTag);
		    pc.race = this.getAllowedRace(pc.raceTag);
		    pc.name = "ChangeThis";
		    pc.tag = "changethis";
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
	
	    public void setControlsStart()
	    {		
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;
            int center = gv.screenWidth / 2;

            if (ctrlUpArrow == null)
            {
                ctrlUpArrow = new IbbButton(gv, 1.0f);
                ctrlUpArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlUpArrow.Img2 = gv.cc.LoadBitmap("ctrl_up_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_up_arrow);
                ctrlUpArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlUpArrow.X = 12 * gv.squareSize;
                ctrlUpArrow.Y = 7 * gv.squareSize + pH * 2;
                ctrlUpArrow.Height = (int)(50 * gv.screenDensity);
                ctrlUpArrow.Width = (int)(50 * gv.screenDensity);
            }
            if (ctrlLeftArrow == null)
            {
                ctrlLeftArrow = new IbbButton(gv, 1.0f);
                ctrlLeftArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlLeftArrow.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_left_arrow);
                ctrlLeftArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlLeftArrow.X = 11 * gv.squareSize;
                ctrlLeftArrow.Y = 8 * gv.squareSize + pH * 2;
                ctrlLeftArrow.Height = (int)(50 * gv.screenDensity);
                ctrlLeftArrow.Width = (int)(50 * gv.screenDensity);
            }
            if (ctrlRightArrow == null)
            {
                ctrlRightArrow = new IbbButton(gv, 1.0f);
                ctrlRightArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlRightArrow.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_right_arrow);
                ctrlRightArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlRightArrow.X = 13 * gv.squareSize;
                ctrlRightArrow.Y = 8 * gv.squareSize + pH * 2;
                ctrlRightArrow.Height = (int)(50 * gv.screenDensity);
                ctrlRightArrow.Width = (int)(50 * gv.screenDensity);
            }
            if (ctrlDownArrow == null)
            {
                ctrlDownArrow = new IbbButton(gv, 1.0f);
                ctrlDownArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlDownArrow.Img2 = gv.cc.LoadBitmap("ctrl_down_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_down_arrow);
                ctrlDownArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlDownArrow.X = 12 * gv.squareSize;
                ctrlDownArrow.Y = 9 * gv.squareSize + pH * 2;
                ctrlDownArrow.Height = (int)(50 * gv.screenDensity);
                ctrlDownArrow.Width = (int)(50 * gv.screenDensity);
            }
		    if (btnPlayerGuideOnPcCreation == null)
		    {
			    btnPlayerGuideOnPcCreation = new IbbButton(gv, 1.0f);	
			    btnPlayerGuideOnPcCreation.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnPlayerGuideOnPcCreation.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnPlayerGuideOnPcCreation.Text = "Player's Guide";
                btnPlayerGuideOnPcCreation.X = center - (int)(170 * gv.screenDensity) - pW * 1;
			    btnPlayerGuideOnPcCreation.Y = 7 * gv.squareSize;
			    btnPlayerGuideOnPcCreation.Height = (int)(50 * gv.screenDensity);
			    btnPlayerGuideOnPcCreation.Width = (int)(170 * gv.screenDensity);			
		    }
		    if (btnBeginnerGuideOnPcCreation == null)
		    {
			    btnBeginnerGuideOnPcCreation = new IbbButton(gv, 1.0f);	
			    btnBeginnerGuideOnPcCreation.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnBeginnerGuideOnPcCreation.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnBeginnerGuideOnPcCreation.Text = "Beginner's Guide";
                btnBeginnerGuideOnPcCreation.X = center - (int)(170 * gv.screenDensity) - pW * 1;
			    btnBeginnerGuideOnPcCreation.Y = 8 * gv.squareSize + pH;
			    btnBeginnerGuideOnPcCreation.Height = (int)(50 * gv.screenDensity);
			    btnBeginnerGuideOnPcCreation.Width = (int)(170 * gv.screenDensity);			
		    }
		    if (btnRollStats == null)
		    {
			    btnRollStats = new IbbButton(gv, 1.0f);	
			    btnRollStats.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnRollStats.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnRollStats.Text = "Roll Stats";
                btnRollStats.X = center - (int)(170 * gv.screenDensity) - pW * 1;
			    btnRollStats.Y = 6 * gv.squareSize - pH;
			    btnRollStats.Height = (int)(50 * gv.screenDensity);
                btnRollStats.Width = (int)(170 * gv.screenDensity);
		    }
		    if (btnFinished == null)
		    {
			    btnFinished = new IbbButton(gv, 1.0f);	
			    btnFinished.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnFinished.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnFinished.Text = "Finished";
                btnFinished.X = center + pW * 1;
			    btnFinished.Y = 6 * gv.squareSize - pH;
			    btnFinished.Height = (int)(50 * gv.screenDensity);
                btnFinished.Width = (int)(170 * gv.screenDensity);
		    }
		    if (btnAbort == null)
		    {
			    btnAbort = new IbbButton(gv, 0.8f);	
			    btnAbort.Text = "Abort";
			    btnAbort.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
			    btnAbort.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnAbort.X = 4 * gv.squareSize + padW * 1 + gv.oXshift;
			    btnAbort.Y = 10 * gv.squareSize + pH * 2;
			    btnAbort.Height = (int)(50 * gv.screenDensity);
			    btnAbort.Width = (int)(50 * gv.screenDensity);			
		    }		    
	    }

	    public void redrawPcCreation()
        {
            //gv.BackColor = Color.DimGray;
    	    //Player pc = mod.playerList.get(0);
    	    gv.sf.UpdateStats(pc);
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);

            int locX = 6 * gv.squareSize;
            //int textH = (int)gv.mSheetTextPaint.getTextSize();
    	    //int spacing = (int)gv.mSheetTextPaint.getTextSize() + pH;            
            int textH = (int)gv.cc.MeasureString("GetHeight", gv.drawFontReg, gv.Width).Height;
            int spacing = textH;
    	    int locY = 0;
    	    int tabX = pW * 50;
    	    int tabX2 = pW * 50;
    	    int leftStartY = pH * 20;
    	    int tokenStartX = locX + (textH * 5);
    	    int tokenStartY = pH * 5 + (spacing/2);
    	    int tokenRectPad = pW * 1;
    	
		    //canvas.drawColor(Color.DKGRAY);
		
		    //Page Title
		    //gv.mSheetTextPaint.setColor(Color.WHITE);
		    //canvas.drawText("CREATE CHARACTER", pW * 31, pH * 3, gv.mSheetTextPaint);
            gv.DrawText("CREATE CHARACTER", pW * 40, pH * 3);
				
		    //select token
		    //gv.mSheetTextPaint.setColor(Color.YELLOW);	
		    //canvas.drawText("Left/Right to Change", tabX2, tokenStartY + (gv.squareSize / 2), gv.mSheetTextPaint);
            gv.DrawText("Left/Right to Change", tabX2, tokenStartY + (gv.squareSize / 2), 1.0f, Color.Yellow);

            Color color = Color.White;
            if (pcCreationIndex == 0) { color = Color.Lime; }
            else { color = Color.White; }
            gv.DrawText("Image:", locX, tokenStartY + (gv.squareSize / 2), 1.0f, color);
		    //canvas.drawText("Image:", locX, tokenStartY + (gv.squareSize / 2), gv.mSheetTextPaint);
		
		    IbRect src = new IbRect(0, 0, pc.token.Width, pc.token.Width);
		    IbRect dst = new IbRect(tokenStartX, tokenStartY, gv.squareSize, gv.squareSize);
            gv.DrawBitmap(blankItemSlot, src, dst);
		    gv.DrawBitmap(pc.token, src, dst);	
		    if (pcCreationIndex == 0)
		    {                
			    IbRect dst2 = new IbRect(tokenStartX - tokenRectPad/2, tokenStartY - tokenRectPad/2, tokenRectPad + gv.squareSize, tokenRectPad + gv.squareSize);
                gv.DrawRoundRectangle(dst2, 10, Color.Lime, 3);			    
		    }
		
		    //name
            if (pcCreationIndex == 1) { color = Color.Lime; }
            else { color = Color.White; }
            gv.DrawText("Name: " + pc.name, locX, locY += leftStartY, 1.0f, color);

		    //if (pcCreationIndex == 1) { gv.mSheetTextPaint.setColor(Color.GREEN); }
		    //else { gv.mSheetTextPaint.setColor(Color.WHITE); }
		    //canvas.drawText("Name: " + pc.name, locX, locY += leftStartY, gv.mSheetTextPaint);
		
		    //race
            if (pcCreationIndex == 2) { color = Color.Lime; }
            else { color = Color.White; }
            gv.DrawText("Race: " + pc.race.name, locX, locY += spacing, 1.0f, color);

		    //if (pcCreationIndex == 2) { gv.mSheetTextPaint.setColor(Color.GREEN); }
		    //else { gv.mSheetTextPaint.setColor(Color.WHITE); }
		    //canvas.drawText("Race: " + pc.race.name, locX, locY += spacing, gv.mSheetTextPaint);
		
		    //gender
            if (pcCreationIndex == 3) { color = Color.Lime; }
            else { color = Color.White; }
            

		    //if (pcCreationIndex == 3) { gv.mSheetTextPaint.setColor(Color.GREEN); }
		    //else { gv.mSheetTextPaint.setColor(Color.WHITE); }
		    if (pc.isMale)
		    {
                gv.DrawText("Gender: Male", locX, locY += spacing, 1.0f, color);
			    //canvas.drawText("Gender: Male", locX, locY += spacing, gv.mSheetTextPaint);
		    }
		    else
		    {
                gv.DrawText("Gender: Female", locX, locY += spacing, 1.0f, color);
			    //canvas.drawText("Gender: Female", locX, locY += spacing, gv.mSheetTextPaint);
		    }
		
		    //class
            if (pcCreationIndex == 4) { color = Color.Lime; }
            else { color = Color.White; }
            gv.DrawText("Class: " + pc.playerClass.name, locX, locY += spacing, 1.0f, color);

		    gv.DrawText("STR: " + pc.baseStr + " + " + (pc.strength - pc.baseStr) + " = " + pc.strength, locX, locY += spacing);
            gv.DrawText("AC: " + pc.AC, tabX2, locY);
            gv.DrawText("DEX: " + pc.baseDex + " + " + (pc.dexterity - pc.baseDex) + " = " + pc.dexterity, locX, locY += spacing);
            gv.DrawText("HP: " + pc.hp + "/" + pc.hpMax, tabX2, locY);
            gv.DrawText("INT:  " + pc.baseInt + " + " + (pc.intelligence - pc.baseInt) + " = " + pc.intelligence, locX, locY += spacing);
            gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, tabX2, locY);
            gv.DrawText("CHA: " + pc.baseCha + " + " + (pc.charisma - pc.baseCha) + " = " + pc.charisma, locX, locY += spacing);
            gv.DrawText("BAB: " + pc.baseAttBonus, tabX2, locY);

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
            int yLoc = pH * 18;
            IbRect rect = new IbRect(tabX, yLoc, pW * 35, pH * 50);
            gv.DrawText(textToSpan, rect, 1.0f, Color.White);
            
            ctrlUpArrow.Draw();
    	    ctrlDownArrow.Draw();
    	    ctrlLeftArrow.Draw();
    	    ctrlRightArrow.Draw();
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
    	
    	    ctrlUpArrow.glowOn = false;
    	    ctrlDownArrow.glowOn = false;
    	    ctrlLeftArrow.glowOn = false;
    	    ctrlRightArrow.glowOn = false;
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
			    if (ctrlUpArrow.getImpact(x, y))
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
			    }	
			    else if (btnRollStats.getImpact(x, y))
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
			
			    ctrlUpArrow.glowOn = false;
			    ctrlDownArrow.glowOn = false;
			    ctrlLeftArrow.glowOn = false;
			    ctrlRightArrow.glowOn = false;
			    btnRollStats.glowOn = false;
			    btnFinished.glowOn = false;	
			    btnAbort.glowOn = false;
			    gv.cc.btnHelp.glowOn = false;
			    btnPlayerGuideOnPcCreation.glowOn = false;
			    btnBeginnerGuideOnPcCreation.glowOn = false;
			
			    if (ctrlUpArrow.getImpact(x, y))
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
                            if (pcRaceSelectionIndex < this.playerRaces.Count-1)
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
						    if (pcClassSelectionIndex < pc.race.classesAllowed.Count-1)
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
			    }	
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
    	    p.tokenFilename = playerTokenList[pcTokenSelectionIndex];
    	    p.token = gv.cc.LoadBitmap(p.tokenFilename);
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
