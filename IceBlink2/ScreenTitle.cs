using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class ScreenTitle 
    {
	    //public gv.module gv.mod;
	    public GameView gv;
	
	    private IbbButton btnNewGame = null;
	    private IbbButton btnLoadSavedGame = null;
	    private IbbButton btnPlayerGuide = null;
	    private IbbButton btnBeginnerGuide = null;
	    private IbbButton btnAbout = null;
        private IbbButton btnExit = null;

        public ScreenTitle(Module m, GameView g)
	    {
		    //gv.mod = m;
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
                btnNewGame.Y = (1 * gv.squareSize) + (2 * pH);
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
                btnLoadSavedGame.Y = (2 * gv.squareSize) + (4 * pH);
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
                btnPlayerGuide.Y = (3 * gv.squareSize) + (6 * pH);
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
                btnBeginnerGuide.Y = (4 * gv.squareSize) + (8 * pH);
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
                btnAbout.Y = (5 * gv.squareSize) + (10 * pH);
                btnAbout.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAbout.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
            if (btnExit == null)
            {
                btnExit = new IbbButton(gv, 1.0f);
                btnExit.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnExit.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnExit.Text = "Exit";
                btnExit.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnExit.Y = (6 * gv.squareSize) + (12 * pH);
                btnExit.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnExit.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
        }

	    //TITLE SCREEN  
        public void redrawTitle()
        {            
    	    //DRAW TITLE SCREEN
            float dstHeight = ((float)gv.screenWidth / (float)gv.cc.title.PixelSize.Width) * (float)gv.cc.title.PixelSize.Height;
            //do narration with image setup    	
            IbRect src = new IbRect(0, 0, gv.cc.title.PixelSize.Width, gv.cc.title.PixelSize.Height);
            IbRect dst = new IbRect(0, 0, gv.screenWidth, (int)dstHeight);
            gv.DrawBitmap(gv.cc.title, src, dst);

            //Draw This gv.module's Version Number
            int xLoc = (gv.screenWidth / 2) - 4;
            int pH = (int)((float)gv.screenHeight / 100.0f);
            gv.DrawText("v" + gv.mod.moduleVersion, xLoc, (8 * gv.squareSize) + (pH * 4));
            
            drawTitleControls();
        }
        public void drawTitleControls()
	    {    	
		    btnNewGame.Draw();		
		    btnLoadSavedGame.Draw();		
		    btnPlayerGuide.Draw();
		    btnBeginnerGuide.Draw();           
		    btnAbout.Draw();
            btnExit.Draw();
        }
        public void onTouchTitle(MouseEventArgs e, MouseEventType.EventType eventType)
        {
            try
            {
                btnNewGame.glowOn = false;
                btnLoadSavedGame.glowOn = false;
                btnPlayerGuide.glowOn = false;
                btnBeginnerGuide.glowOn = false;
                btnAbout.glowOn = false;
                btnExit.glowOn = false;

                switch (eventType)
                {
                    case MouseEventType.EventType.MouseUp:
                        int x = (int)e.X;
                        int y = (int)e.Y;

                        btnNewGame.glowOn = false;
                        btnLoadSavedGame.glowOn = false;
                        btnAbout.glowOn = false;
                        btnExit.glowOn = false;
                        btnPlayerGuide.glowOn = false;
                        btnBeginnerGuide.glowOn = false;

                        if (btnNewGame.getImpact(x, y))
                        {
                            gv.PlaySound("btn_click");
                            if (gv.mod.mustUsePreMadePC)
                            {
                                //no spell selection offered
                                gv.log.tagStack.Clear();
                                gv.log.logLinesList.Clear();
                                gv.log.currentTopLineIndex = 0;
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
                            gv.cc.tutorialPlayersGuide();
                        }
                        else if (btnBeginnerGuide.getImpact(x, y))
                        {
                            gv.PlaySound("btn_click");
                            gv.cc.tutorialBeginnersGuide();
                        }
                        else if (btnAbout.getImpact(x, y))
                        {
                            gv.PlaySound("btn_click");
                            gv.cc.doAboutDialog();
                        }
                        else if (btnExit.getImpact(x, y))
                        {
                            gv.PlaySound("btn_click");
                            gv.Close();
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
                        else if (btnExit.getImpact(x, y))
                        {
                            btnExit.glowOn = true;
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
            catch
            { }
        }
    }
}
