using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class ScreenJournal 
    {

	    public Module mod;
	    public GameView gv;
	    private int journalScreenQuestIndex = 0;
	    private int journalScreenEntryIndex = 0;	
	    private Bitmap journalBack;
	    private IbbButton btnReturnJournal = null;
	    public IbbButton ctrlUpArrow = null;
	    public IbbButton ctrlDownArrow = null;
	    public IbbButton ctrlLeftArrow = null;
	    public IbbButton ctrlRightArrow = null;
        private IbbHtmlTextBox description;
	
	    public ScreenJournal(Module m, GameView g)
	    {
		    mod = m;
		    gv = g;
		    //journalBack = gv.cc.LoadBitmap("journalback"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.journalback);
		    //setControlsStart();
	    }
	    public void setControlsStart()
	    {		
		    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;		
		    int xShift = gv.squareSize/2;
		    int yShift = gv.squareSize/2;

            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;

		    if (ctrlUpArrow == null)
		    {
			    ctrlUpArrow = new IbbButton(gv, 1.0f);
			    ctrlUpArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlUpArrow.Img2 = gv.cc.LoadBitmap("ctrl_up_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_up_arrow);
			    ctrlUpArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlUpArrow.X = 12 * gv.squareSize;
			    ctrlUpArrow.Y = 1 * gv.squareSize + pH * 2;
			    ctrlUpArrow.Height = (int)(50 * gv.screenDensity);
			    ctrlUpArrow.Width = (int)(50 * gv.screenDensity);
		    }
		    if (ctrlDownArrow == null)
		    {
			    ctrlDownArrow = new IbbButton(gv, 1.0f);	
			    ctrlDownArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlDownArrow.Img2 = gv.cc.LoadBitmap("ctrl_down_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_down_arrow);
			    ctrlDownArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlDownArrow.X = 12 * gv.squareSize;
			    ctrlDownArrow.Y = 2 * gv.squareSize + pH * 3;
			    ctrlDownArrow.Height = (int)(50 * gv.screenDensity);
			    ctrlDownArrow.Width = (int)(50 * gv.screenDensity);
		    }
		    if (ctrlLeftArrow == null)
		    {
			    ctrlLeftArrow = new IbbButton(gv, 1.0f);
			    ctrlLeftArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlLeftArrow.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    ctrlLeftArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlLeftArrow.X = 10 * gv.squareSize + xShift;
			    ctrlLeftArrow.Y = pH * 34;
			    ctrlLeftArrow.Height = (int)(50 * gv.screenDensity);
			    ctrlLeftArrow.Width = (int)(50 * gv.screenDensity);
		    }
		    if (ctrlRightArrow == null)
		    {
			    ctrlRightArrow = new IbbButton(gv, 1.0f);
			    ctrlRightArrow.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    ctrlRightArrow.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    ctrlRightArrow.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.arrow_glow);
			    ctrlRightArrow.X = 11 * gv.squareSize + pW * 2 + xShift;
			    ctrlRightArrow.Y = pH * 34;
			    ctrlRightArrow.Height = (int)(50 * gv.screenDensity);
			    ctrlRightArrow.Width = (int)(50 * gv.screenDensity);			
		    }				
		    if (btnReturnJournal == null)
		    {
			    btnReturnJournal = new IbbButton(gv, 1.2f);	
			    btnReturnJournal.Text = "RETURN";
			    btnReturnJournal.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnReturnJournal.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnReturnJournal.X = (gv.screenWidth / 2) - (int)(170.0f * gv.screenDensity / 2.0f);
			    btnReturnJournal.Y = 10 * gv.squareSize + pH * 2;
			    btnReturnJournal.Height = (int)(50 * gv.screenDensity);
			    btnReturnJournal.Width = (int)(170 * gv.screenDensity);			
		    }
		
	    }

        public void redrawJournal()
        {
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = pH * 5;
            int locX = 5 * gv.squareSize + pW * 14;
            int textH = (int)gv.cc.MeasureString("GetHeight", gv.drawFontReg, gv.Width).Height;
            int spacing = textH;
    	    //int spacing = (int)gv.mSheetTextPaint.getTextSize() + pH;
    	    int leftStartY = pH * 4;
    	    int tabStartY = pH * 40;
    	
    	    //DRAW BACKGROUND COLOR
            //gv.gCanvas.Clear(Color.DarkGray);
    	    //canvas.drawColor(Color.DKGRAY);
    	
    	    //IF BACKGROUND IS NULL, LOAD IMAGE
    	    if (journalBack == null)
    	    {
    		    journalBack = gv.cc.LoadBitmap("journalback");
    	    }
    	    //IF BUTTONS ARE NULL, LOAD BUTTONS
    	    if (btnReturnJournal == null)
    	    {
    		    setControlsStart();
    	    }
    	
    	    //DRAW BACKGROUND IMAGE
    	    IbRect src = new IbRect(0, 0, journalBack.Width, journalBack.Height);
            IbRect dst = new IbRect(7 * gv.squareSize, 0, 7 * gv.squareSize, 10 * gv.squareSize);
            gv.DrawBitmap(journalBack, src, dst);
        
            //MAKE SURE NO OUT OF INDEX ERRORS
    	    if (mod.partyJournalQuests.Count > 0)
    	    {
	    	    if (journalScreenQuestIndex >= mod.partyJournalQuests.Count)
	    	    {
	    		    journalScreenQuestIndex = 0;
	    	    }    	
	    	    if (journalScreenEntryIndex >= mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count)
	    	    {
	    		    journalScreenEntryIndex = 0;
	    	    }
    	    }
			
    	    //DRAW QUESTS
            Color color = Color.Black;
		    //gv.mSheetTextPaint.setColor(Color.BLACK);
		    gv.DrawText("Active Quests:", locX, locY += leftStartY, 1.0f, color);
		    gv.DrawText("--------------", locX, locY += spacing, 1.0f, color);
		    if (mod.partyJournalQuests.Count > 0)
    	    {
			    int cnt = 0;
			    foreach (JournalQuest jq in mod.partyJournalQuests)
			    {
                    if (journalScreenQuestIndex == cnt) { color = Color.Lime; }
				    else { color = Color.Black; }	
                    gv.DrawText(jq.Name, locX, locY += spacing, 1.0f, color);
				    cnt++;
			    }
    	    }
		
		    //DRAW QUEST ENTRIES
		    locY = tabStartY;
		    //gv.mSheetTextPaint.setColor(Color.BLACK);
		    gv.DrawText("Quest Entry:", locX, locY, 1.0f, Color.Black);
		    gv.DrawText("--------------", locX, locY += spacing, 1.0f, Color.Black);	
		    if (mod.partyJournalQuests.Count > 0)
    	    {
                //Description
                string textToSpan = "<font color='black'><i><b>" + mod.partyJournalQuests[journalScreenQuestIndex].Entries[journalScreenEntryIndex].EntryTitle + "</b></i></font><br>";
                textToSpan += mod.partyJournalQuests[journalScreenQuestIndex].Entries[journalScreenEntryIndex].EntryText;
	                            
                int yLoc = pH * 18;
                //IbRect rect = new IbRect(locX, locY, pW * 79, pH * 50);
                //gv.DrawText(textToSpan, rect, 1.0f, Color.White);

                description.tbXloc = locX;
                description.tbYloc = locY + spacing;
                description.tbWidth = pW * 30;
                description.tbHeight = pH * 50;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.brush.Color = Color.Black;
                description.onDrawLogBox(gv.gCanvas);
    	    }
		
		    //DRAW INSTRUCTIONS
            //string text = "Up-Down: view quests. <BR>"
        	//	    + "Left-Right: view different entries of selected quest.";
            //drawJournalInstructions(canvas, text);
        
            //DRAW ALL CONTROLS
		    ctrlUpArrow.Draw();
		    ctrlDownArrow.Draw();
		    ctrlLeftArrow.Draw();
		    ctrlRightArrow.Draw();
		    btnReturnJournal.Draw();
        }
    
	    public void onTouchJournal(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
		    ctrlUpArrow.glowOn = false;
		    ctrlDownArrow.glowOn = false;
		    ctrlLeftArrow.glowOn = false;
		    ctrlRightArrow.glowOn = false;
		    btnReturnJournal.glowOn = false;
		
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
			    else if (btnReturnJournal.getImpact(x, y))
			    {
				    btnReturnJournal.glowOn = true;
			    }
			
			    break;
			
		    case MouseEventType.EventType.MouseUp:
			    x = (int) e.X;
			    y = (int) e.Y;
			
			    ctrlUpArrow.glowOn = false;
			    ctrlDownArrow.glowOn = false;
			    ctrlLeftArrow.glowOn = false;
			    ctrlRightArrow.glowOn = false;
			    btnReturnJournal.glowOn = false;
			
			    if (ctrlUpArrow.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (journalScreenQuestIndex > 0)
				    {
					    journalScreenQuestIndex--;
					    journalScreenEntryIndex = mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count - 1;
				    }
			    }
			    else if (ctrlDownArrow.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (journalScreenQuestIndex < mod.partyJournalQuests.Count - 1)
				    {
					    journalScreenQuestIndex++;
					    journalScreenEntryIndex = mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count - 1;
				    }
			    }
			    else if (ctrlLeftArrow.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (journalScreenEntryIndex > 0)
				    {
					    journalScreenEntryIndex--;
				    }
			    }
			    else if (ctrlRightArrow.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (journalScreenEntryIndex < mod.partyJournalQuests[journalScreenQuestIndex].Entries.Count - 1)
				    {
					    journalScreenEntryIndex++;
				    }
			    }	
			    else if (btnReturnJournal.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.screenType = "main";
				    journalBack = null;
				    btnReturnJournal = null;
				    ctrlUpArrow = null;
				    ctrlDownArrow = null;
				    ctrlLeftArrow = null;
				    ctrlRightArrow = null;
			    }			
			    break;		
		    }
	    }
	    public void drawJournalInstructions(string text)
        {
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int startX = pW * 3;
		    int startY = (int)(8.0f * gv.squareSize);
		/*TODO
		    TextPaint tp = new TextPaint();
            tp.setColor(Color.YELLOW);
            tp.setTextSize(gv.mUiTextPaint.getTextSize() * 0.85f);
            tp.setTextAlign(Align.LEFT);
            tp.setAntiAlias(true);
            tp.setTypeface(gv.uiFont);
        
            String textToSpan = text;        
            Spanned htmlText = Html.fromHtml(textToSpan);
            StaticLayout sl = new StaticLayout(htmlText, tp, pW * 50, Layout.Alignment.ALIGN_NORMAL, 1, 0, false);
            canvas.translate(startX, startY);
            sl.draw(canvas);
            canvas.translate(-startX, -startY);
            */
        }
    }
}
