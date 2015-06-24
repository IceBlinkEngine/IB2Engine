using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class ScreenTitle 
    {
	    public Module mod;
	    public GameView gv;
	
	    private IbbButton btnNewGame = null;
	    private IbbButton btnLoadSavedGame = null;
	    private IbbButton btnPlayerGuide = null;
	    private IbbButton btnBeginnerGuide = null;
	    private IbbButton btnAbout = null;	
	
	    public ScreenTitle(Module m, GameView g)
	    {
		    mod = m;
		    gv = g;
		    setControlsStart();
	    }
	
	    public void setControlsStart()
	    {
            int pH = (int)((float)gv.screenHeight / 100.0f);

    	    if (btnNewGame == null)
		    {
			    btnNewGame = new IbbButton(gv, 1.0f);	
			    btnNewGame.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnNewGame.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnNewGame.Text = "New Game";
                btnNewGame.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnNewGame.Y = (4 * gv.squareSize) + (2 * pH);
                btnNewGame.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNewGame.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnLoadSavedGame == null)
		    {
			    btnLoadSavedGame = new IbbButton(gv, 1.0f);	
			    btnLoadSavedGame.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnLoadSavedGame.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnLoadSavedGame.Text = "Load Saved Game";
                btnLoadSavedGame.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnLoadSavedGame.Y = (5 * gv.squareSize) + (4 * pH);
                btnLoadSavedGame.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnLoadSavedGame.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnPlayerGuide == null)
		    {
			    btnPlayerGuide = new IbbButton(gv, 1.0f);	
			    btnPlayerGuide.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnPlayerGuide.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnPlayerGuide.Text = "Player's Guide";
                btnPlayerGuide.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnPlayerGuide.Y = (6 * gv.squareSize) + (6 * pH);
                btnPlayerGuide.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPlayerGuide.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }		
		    if (btnBeginnerGuide == null)
		    {
			    btnBeginnerGuide = new IbbButton(gv, 1.0f);	
			    btnBeginnerGuide.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnBeginnerGuide.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnBeginnerGuide.Text = "Beginner's Guide";
                btnBeginnerGuide.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnBeginnerGuide.Y = (7 * gv.squareSize) + (8 * pH);
                btnBeginnerGuide.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnBeginnerGuide.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
		    if (btnAbout == null)
		    {
			    btnAbout = new IbbButton(gv, 1.0f);	
			    btnAbout.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnAbout.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
			    btnAbout.Text = "Credits";
                btnAbout.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnAbout.Y = (8 * gv.squareSize) + (10 * pH);
                btnAbout.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAbout.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }		
	    }

	    //TITLE SCREEN  
        public void redrawTitle()
        {            
    	    //canvas.drawColor(Color.BLACK); 

            //DRAW TITLE SCREEN
            IbRect src = new IbRect(0, 0, gv.cc.title.Width, gv.cc.title.Height);
            IbRect dst = new IbRect((gv.screenWidth / 2) - (gv.squareSize * 4), 0, gv.squareSize * 8, gv.squareSize * 4);
            
            gv.DrawBitmap(gv.cc.title, src, dst);
            /*TODO
    	    //Draw This Module's Version Number
    	    int xLoc = (gv.screenWidth / 2) - (int)(gv.mSheetTextPaint.getTextSize());
    	    int pH = (int)((float)gv.screenHeight / 100.0f);
    	    gv.mSheetTextPaint.setColor(Color.LTGRAY);
		    canvas.drawText("v" + mod.moduleVersion, xLoc, (11 * gv.squareSize) + (pH * 4), gv.mSheetTextPaint);
            */
            drawTitleControls();
        }
        public void drawTitleControls()
	    {    	
		    btnNewGame.Draw();		
		    btnLoadSavedGame.Draw();		
		    btnPlayerGuide.Draw();
		    btnBeginnerGuide.Draw();
            //btnMoreGames.Draw();            
		    btnAbout.Draw();
	    }
        public void onTouchTitle(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
    	    btnNewGame.glowOn = false;
		    btnLoadSavedGame.glowOn = false;
		    //btnIab.glowOn = false;
		    btnPlayerGuide.glowOn = false;
		    btnBeginnerGuide.glowOn = false;				
		    //btnMoreGames.glowOn = false;
		    btnAbout.glowOn = false;	
		
		    //int eventAction = event.getAction();
		    switch (eventType)
		    {
		    case MouseEventType.EventType.MouseUp:
                int x = (int)e.X;
                int y = (int)e.Y;
				
			    btnNewGame.glowOn = false;
			    //btnMoreGames.glowOn = false;
			    btnLoadSavedGame.glowOn = false;
			    btnAbout.glowOn = false;
			    btnPlayerGuide.glowOn = false;
			    btnBeginnerGuide.glowOn = false;	
			    //btnIab.glowOn = false;
			
			    if (btnNewGame.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    //gv.TrackerSendScreenView("NewGame");
                    //mod.uniqueSessionIdNumberTag = gv.sf.RandInt(1000000) + "";
				    if (gv.mod.mustUsePreMadePC)
				    {
					    //no spell selection offered
					    gv.cc.tutorialMessageMainMap();
			    	    gv.screenType = "main";
			    	    gv.cc.doUpdate();
				    }
				    else
				    {
					    gv.screenType = "partyBuild";
					    gv.screenPartyBuild.loadPlayerList();
				    }
			    }
			    else if (btnLoadSavedGame.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    //gv.TrackerSendScreenView("LoadSavedGame");
				    if (gv.cc.slot5.Equals(""))
				    {
					    //Toast.makeText(gv.gameContext, "Still Loading Data... try again in a second", Toast.LENGTH_SHORT).show();
				    }
				    else
				    {
					    gv.cc.doLoadSaveGameDialog();
				    }
			    }
			    else if (btnPlayerGuide.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.cc.tutorialPlayersGuide();
			    }
			    else if (btnBeginnerGuide.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.cc.tutorialBeginnersGuide();
			    }
			    else if (btnAbout.getImpact(x, y))
			    {
                    gv.PlaySound("btn_click");
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.cc.doAboutDialog();
			    }						
			    break;

            case MouseEventType.EventType.MouseDown:
            case MouseEventType.EventType.MouseMove:
                x = (int)e.X;
                y = (int)e.Y;
				
			    if (btnNewGame.getImpact(x, y))
			    {
                    btnNewGame.glowOn = true;
			    }
			    else if (btnLoadSavedGame.getImpact(x, y))
			    {
				    btnLoadSavedGame.glowOn = true;
			    }
			    else if (btnAbout.getImpact(x, y))
			    {
				    btnAbout.glowOn = true;
			    }
			    else if (btnPlayerGuide.getImpact(x, y))
			    {
				    btnPlayerGuide.glowOn = true;
			    }
			    else if (btnBeginnerGuide.getImpact(x, y))
			    {
				    btnBeginnerGuide.glowOn = true;	
			    }
			    break;		
		    }
	    }
    
    
    }
}
