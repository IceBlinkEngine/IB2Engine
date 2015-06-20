using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class ScreenLauncher 
    {
	    private Module mod;
	    private GameView gv;
	
	    private IbbButton btnLeft = null;
	    private IbbButton btnRight = null;
	    private IbbButton btnModuleName = null;
        private IbbHtmlTextBox description;
	    private List<Module> moduleList = new List<Module>();
	    private List<Bitmap> titleList = new List<Bitmap>();
	    private int moduleIndex = 0;
	
	
	    public ScreenLauncher(Module m, GameView g) 
	    {
		    mod = m;
		    gv = g;
		    setControlsStart();
            int pH = (int)((float)gv.screenHeight / 100.0f);
            description = new IbbHtmlTextBox(gv, 320, (6 * gv.squareSize) + (pH * 2), 500, 300);
            description.showBoxBorder = false;
	    }
	
        public void loadModuleFiles()
        {
            string[] files;

            files = Directory.GetFiles(gv.mainDirectory + "\\modules", "*.mod", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileName(file) != "NewModule.mod")
                {
                    // Process each file
                    Module mod = gv.cc.LoadModule(file, true);
                    if (mod == null)
                    {
                        gv.sf.MessageBox("returned a null module");
                    }
                    moduleList.Add(mod);
                    titleList.Add(gv.cc.LoadBitmap("title", mod));
                }
            }
        }
        	    	
	    public void setControlsStart()
	    {		
		    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
            int wideX = (gv.screenWidth / 2) - (int)(170 * gv.screenDensity / 2);
            int smallLeftX = wideX - (int)(50 * gv.screenDensity);
            int smallRightX = wideX + (int)(170 * gv.screenDensity);
		    int padW = gv.squareSize/6;
		
		    if (btnLeft == null)
		    {
			    btnLeft = new IbbButton(gv, 1.0f);
			    btnLeft.Img = gv.cc.LoadBitmap("btn_small");
			    btnLeft.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow");
			    btnLeft.Glow = gv.cc.LoadBitmap("btn_small_glow");
			    btnLeft.X = smallLeftX;
                btnLeft.Y = (5 * gv.squareSize) - (pH * 2);
			    btnLeft.Height = (int)(50 * gv.screenDensity);
			    btnLeft.Width = (int)(50 * gv.screenDensity);
		    }
		    if (btnModuleName == null)
		    {
			    btnModuleName = new IbbButton(gv, 1.0f);
			    btnModuleName.Img = gv.cc.LoadBitmap("btn_large");
			    btnModuleName.Glow = gv.cc.LoadBitmap("btn_large_glow");
			    btnModuleName.Text = "";
                btnModuleName.X = wideX;
			    btnModuleName.Y = (5 * gv.squareSize) - (pH * 2);
			    btnModuleName.Height = (int)(50 * gv.screenDensity);
			    btnModuleName.Width = (int)(170 * gv.screenDensity);
		    }
		    if (btnRight == null)
		    {
			    btnRight = new IbbButton(gv, 1.0f);
			    btnRight.Img = gv.cc.LoadBitmap("btn_small");
			    btnRight.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow");
			    btnRight.Glow = gv.cc.LoadBitmap("btn_small_glow");
			    btnRight.X = smallRightX;
			    btnRight.Y = (5 * gv.squareSize) - (pH * 2);
			    btnRight.Height = (int)(50 * gv.screenDensity);
			    btnRight.Width = (int)(50 * gv.screenDensity);
		    }	
	    }

	    //TITLE SCREEN  
        public void redrawLauncher()
        {
            //gv.gCanvas.Clear(Color.Black);
            //DRAW TITLE SCREEN
    	    if ((titleList.Count > 0) && (moduleIndex < titleList.Count))
		    {
    		    float dstHeight = ((float)gv.screenWidth / (float)titleList[moduleIndex].Width) * (float)titleList[moduleIndex].Height;
	            IbRect src = new IbRect(0, 0, titleList[moduleIndex].Width, titleList[moduleIndex].Height);
	            IbRect dst = new IbRect((gv.screenWidth/2)-200, gv.squareSize/2, gv.squareSize*8, gv.squareSize*4);
                gv.DrawBitmap(titleList[moduleIndex], src, dst);
		    }
            	
    	    //DRAW DESCRIPTION BOX
            //int pW = (int)((float)gv.screenWidth / 100.0f);
		    //int pH = (int)((float)gv.screenHeight / 100.0f);
            //int locY = btnModuleName.Y + btnModuleName.Height + pH * 2;
            //int tabX = pW * 4;
        
		    if ((moduleList.Count > 0) && (moduleIndex < moduleList.Count))
		    {
                string textToSpan = "<u>Module Description</u>" + "<br><br>";
                textToSpan += "<b><i><big>" + moduleList[moduleIndex].moduleLabelName + "</big></i></b><br>";
                textToSpan += moduleList[moduleIndex].moduleDescription;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.onDrawLogBox(gv.gCanvas);
                /* need to figure out how to print text to screen with word wrap
                 * 
                 * 
			    TextPaint tp = new TextPaint();
	            tp.setColor(Color.WHITE);
	            tp.setTextSize(gv.mSheetTextPaint.getTextSize());
	            tp.setTextAlign(Align.LEFT);
	            tp.setAntiAlias(true);
	            tp.setTypeface(gv.uiFont);	        
	            String textToSpan = "<u>Module Description</u>" + "<BR><BR>";
	            textToSpan += "<b><i><big>" + moduleList.get(moduleIndex).moduleLabelName + "</big></i></b><BR><BR>";
	            textToSpan += moduleList.get(moduleIndex).moduleDescription;	        	
	            Spanned htmlText = Html.fromHtml(textToSpan);
	            StaticLayout sl = new StaticLayout(htmlText, tp, pW * 94, Layout.Alignment.ALIGN_NORMAL, 1, 0, false);
	            //locY += spacing;
	            canvas.translate(tabX, locY);
	            sl.draw(canvas);
	            canvas.translate(-tabX, -locY);
                */
                //string text = "Module Description" + Environment.NewLine + moduleList[moduleIndex].moduleLabelName + Environment.NewLine + moduleList[moduleIndex].moduleDescription;
                //gv.DrawText(text, new IbRect(300, (6 * gv.squareSize) + (pH * 2), 400, 600), 1.0f, Color.White);
			    btnModuleName.Text = moduleList[moduleIndex].moduleLabelName;
	    	    drawLauncherControls();
		    }
        }
        public void drawLauncherControls()
	    {    	
		    btnLeft.Draw();		
		    btnRight.Draw();
		    btnModuleName.Draw();
		    //btnPlay.Draw();
	    }
        public void onTouchLauncher(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
    	    btnLeft.glowOn = false;
    	    btnRight.glowOn = false;
    	    //btnPlay.glowOn = false;	
    	    btnModuleName.glowOn = false;
		
		    //int eventAction = event.getAction();
		    switch (eventType)
		    {
		    case MouseEventType.EventType.MouseUp:
			    int x = (int) e.X;
			    int y = (int) e.Y;
				
			    btnLeft.glowOn = false;
	    	    btnRight.glowOn = false;
	    	    //btnPlay.glowOn = false;	
	    	    btnModuleName.glowOn = false;
	    	    //btnPlay.Text = "PLAY SELECTED";
			
	    	    if (btnLeft.getImpact(x, y))
			    {
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (moduleIndex > 0)
				    {
					    moduleIndex--;
					    btnModuleName.Text = moduleList[moduleIndex].moduleName;
				    }
			    }
			    else if (btnRight.getImpact(x, y))
			    {
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (moduleIndex < moduleList.Count-1)
				    {
					    moduleIndex++;
					    btnModuleName.Text = moduleList[moduleIndex].moduleName;
				    }
			    }	    	
			    /*else if (btnPlay.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.mod = moduleList[moduleIndex];
				    gv.resetGame();
				    gv.cc.LoadSaveListItems();
				    gv.screenType = "title";
                    //gv.TrackerSendScreenView("Launch:" + gv.mod.moduleLabelName);
			    }*/
			    else if (btnModuleName.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.mod = moduleList[moduleIndex];
				    gv.resetGame();
				    gv.cc.LoadSaveListItems();
				    gv.screenType = "title";
                    //gv.TrackerSendScreenView("Launch:" + gv.mod.moduleLabelName);
			    }
			    break;
		
		    case MouseEventType.EventType.MouseMove:
		    case MouseEventType.EventType.MouseDown:
			    x = (int) e.X;
			    y = (int) e.Y;
				
			    if (btnLeft.getImpact(x, y))
			    {
                    btnLeft.glowOn = true;
			    }
			    else if (btnRight.getImpact(x, y))
			    {
				    btnRight.glowOn = true;
			    }
			    /*else if (btnPlay.getImpact(x, y))
			    {
				    btnPlay.glowOn = true;
				    btnPlay.Text = "LOADING....";
			    }*/
			    else if (btnModuleName.getImpact(x, y))
			    {
				    btnModuleName.glowOn = true;
				    //btnPlay.glowOn = true;
				    //btnPlay.Text = "LOADING....";
			    }
			    break;		
		    }
	    }
    }
}
