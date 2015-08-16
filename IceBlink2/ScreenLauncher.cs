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
            description = new IbbHtmlTextBox(gv);
            description.tbXloc = 0 * gv.squareSize + gv.oXshift;
            description.tbYloc = 6 * gv.squareSize + gv.oYshift;
            description.tbWidth = 16 * gv.squareSize;
            description.tbHeight = 6 * gv.squareSize;
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
            int wideX = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2);
            int smallLeftX = wideX - (int)(gv.ibbwidthR * gv.screenDensity);
            int smallRightX = wideX + (int)(gv.ibbwidthL * gv.screenDensity);
		    int padW = gv.squareSize/6;
		
		    if (btnLeft == null)
		    {
			    btnLeft = new IbbButton(gv, 1.0f);
			    btnLeft.Img = gv.cc.LoadBitmap("btn_small");
			    btnLeft.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow");
			    btnLeft.Glow = gv.cc.LoadBitmap("btn_small_glow");
			    btnLeft.X = smallLeftX;
                btnLeft.Y = (5 * gv.squareSize) - (pH * 2);
                btnLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnModuleName == null)
		    {
			    btnModuleName = new IbbButton(gv, 1.0f);
			    btnModuleName.Img = gv.cc.LoadBitmap("btn_large");
			    btnModuleName.Glow = gv.cc.LoadBitmap("btn_large_glow");
			    btnModuleName.Text = "";
                btnModuleName.X = wideX;
			    btnModuleName.Y = (5 * gv.squareSize) - (pH * 2);
                btnModuleName.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnModuleName.Width = (int)(gv.ibbwidthL * gv.screenDensity);
		    }
		    if (btnRight == null)
		    {
			    btnRight = new IbbButton(gv, 1.0f);
			    btnRight.Img = gv.cc.LoadBitmap("btn_small");
			    btnRight.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow");
			    btnRight.Glow = gv.cc.LoadBitmap("btn_small_glow");
			    btnRight.X = smallRightX;
			    btnRight.Y = (5 * gv.squareSize) - (pH * 2);
                btnRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }	
	    }

	    //TITLE SCREEN  
        public void redrawLauncher()
        {
            //DRAW TITLE SCREEN
    	    if ((titleList.Count > 0) && (moduleIndex < titleList.Count))
		    {
    		    IbRect src = new IbRect(0, 0, titleList[moduleIndex].Width, titleList[moduleIndex].Height);
                IbRect dst = new IbRect((gv.screenWidth / 2) - (gv.squareSize * 4), 0, gv.squareSize * 8, gv.squareSize * 4);
                gv.DrawBitmap(titleList[moduleIndex], src, dst);
		    }
            	
    	    //DRAW DESCRIPTION BOX
            if ((moduleList.Count > 0) && (moduleIndex < moduleList.Count))
		    {
                string textToSpan = "<u>Module Description</u>" + "<br>";
                //textToSpan += "<b><i><big>" + moduleList[moduleIndex].moduleLabelName + "</big></i></b><br>";
                textToSpan += moduleList[moduleIndex].moduleDescription;
                description.logLinesList.Clear();
                description.AddHtmlTextToLog(textToSpan);
                description.tbXloc = 4 * gv.squareSize + gv.oXshift;
                description.tbYloc = 6 * gv.squareSize + gv.oYshift;
                description.tbWidth = 12 * gv.squareSize;
                description.tbHeight = 6 * gv.squareSize;
                description.onDrawLogBox();
                
                btnModuleName.Text = moduleList[moduleIndex].moduleLabelName;
	    	    drawLauncherControls();
		    }
        }
        public void drawLauncherControls()
	    {    	
		    btnLeft.Draw();		
		    btnRight.Draw();
		    btnModuleName.Draw();
	    }
        public void onTouchLauncher(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
    	    btnLeft.glowOn = false;
    	    btnRight.glowOn = false;	
    	    btnModuleName.glowOn = false;
		
		    switch (eventType)
		    {
		        case MouseEventType.EventType.MouseUp:
			        int x = (int) e.X;
			        int y = (int) e.Y;
				
			        btnLeft.glowOn = false;
	    	        btnRight.glowOn = false;	
	    	        btnModuleName.glowOn = false;
			
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
			        else if (btnModuleName.getImpact(x, y))
			        {
				        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				        gv.mod = moduleList[moduleIndex];
				        gv.resetGame();
				        gv.cc.LoadSaveListItems();
				        gv.screenType = "title";
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
			        else if (btnModuleName.getImpact(x, y))
			        {
				        btnModuleName.glowOn = true;
			        }
			        break;		
		    }
	    }
    }
}
