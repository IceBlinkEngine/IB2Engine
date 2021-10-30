using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class ScreenTokenSelector
    {
        //public gv.module gv.mod;
	    public GameView gv;
        public Player pc;
	    private int tknPageIndex = 0;
	    private int tknSlotIndex = 0;
	    private int slotsPerPage = 20;
        private int maxPages = 20;
	    private List<IbbButton> btnTokenSlot = new List<IbbButton>();
	    private IbbButton btnTokensLeft = null;
	    private IbbButton btnTokensRight = null;
	    private IbbButton btnPageIndex = null;
	    private IbbButton btnAction = null;
        private IbbButton btnExit = null;
        public string callingScreen = "pcCreation"; //party, pcCreation
        public List<string> playerTokenList = new List<string>();

        public ScreenTokenSelector(Module m, GameView g)
	    {
		    //gv.mod = m;
		    gv = g;
	    }

        public void resetTokenSelector(string callingScreenToReturnTo, Player p)
        {
            pc = p;
            callingScreen = callingScreenToReturnTo;
            LoadPlayerTokenList();
        }

        public void LoadPlayerTokenList()
        {
            playerTokenList.Clear();
            try
            {
                //Load from gv.module folder first
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
                            if (filename.EndsWith(".png"))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                playerTokenList.Add(fileNameWithOutExt);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            gv.errorLog(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                gv.errorLog(ex.ToString());
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
                            gv.errorLog(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                gv.errorLog(ex.ToString());
            }
        }

	    public void setControlsStart()
	    {			
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;

            if (btnTokensLeft == null)
		    {
			    btnTokensLeft = new IbbButton(gv, 1.0f);
			    btnTokensLeft.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnTokensLeft.Img2 = gv.cc.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_left_arrow);
			    btnTokensLeft.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnTokensLeft.X = 8 * gv.squareSize;
			    btnTokensLeft.Y = (1 * gv.squareSize) - (pH * 2);
                btnTokensLeft.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTokensLeft.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnPageIndex == null)
		    {
			    btnPageIndex = new IbbButton(gv, 1.0f);
			    btnPageIndex.Img = gv.cc.LoadBitmap("btn_small_off"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_off);
			    btnPageIndex.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnPageIndex.Text = "1/10";
			    btnPageIndex.X = 9 * gv.squareSize;
			    btnPageIndex.Y = (1 * gv.squareSize) - (pH * 2);
                btnPageIndex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPageIndex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnTokensRight == null)
		    {
			    btnTokensRight = new IbbButton(gv, 1.0f);
			    btnTokensRight.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
			    btnTokensRight.Img2 = gv.cc.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.ctrl_right_arrow);
			    btnTokensRight.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			    btnTokensRight.X = 10 * gv.squareSize;
			    btnTokensRight.Y = (1 * gv.squareSize) - (pH * 2);
                btnTokensRight.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTokensRight.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		
		    if (btnAction == null)
		    {
			    btnAction = new IbbButton(gv, 1.0f);
                btnAction.Text = "USE SELECTED";
                btnAction.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
			    btnAction.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnAction.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f) - (gv.squareSize * 4);
			    btnAction.Y = 9 * gv.squareSize + pH * 2;
                btnAction.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAction.Width = (int)(gv.ibbwidthL * gv.screenDensity);			
		    }
            if (btnExit == null)
            {
                btnExit = new IbbButton(gv, 1.0f);
                btnExit.Text = "Return";
                btnExit.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnExit.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnExit.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnExit.Y = 9 * gv.squareSize + pH * 2;
                btnExit.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnExit.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
		    for (int y = 0; y < slotsPerPage; y++)
		    {
			    IbbButton btnNew = new IbbButton(gv, 1.0f);
                gv.cc.DisposeOfBitmap(ref btnNew.Img);
                btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
                gv.cc.DisposeOfBitmap(ref btnNew.Glow);
                btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
			
			    if (y < 5)
			    {
				    btnNew.X = ((y + 2 + 4) * gv.squareSize) + (padW * (y + 1)) + gv.oXshift;
				    btnNew.Y = 2 * gv.squareSize;
			    }
			    else if ((y >=5 ) && (y < 10))
			    {
				    btnNew.X = ((y - 5 + 2 + 4) * gv.squareSize) + (padW * ((y - 5) + 1)) + gv.oXshift;
				    btnNew.Y = 3 * gv.squareSize + padW;
			    }
                else if ((y >= 10) && (y < 15))
                {
                    btnNew.X = ((y - 10 + 2 + 4) * gv.squareSize) + (padW * ((y - 10) + 1)) + gv.oXshift;
                    btnNew.Y = 4 * gv.squareSize + (padW * 2);
                }
                else
                {
                    btnNew.X = ((y - 15 + 2 + 4) * gv.squareSize) + (padW * ((y - 15) + 1)) + gv.oXshift;
                    btnNew.Y = 5 * gv.squareSize + (padW * 3);
                }

                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);
			
			    btnTokenSlot.Add(btnNew);
		    }			
	    }
	
	    //INVENTORY SCREEN (COMBAT and MAIN)
        public void redrawTokenSelector()
        {
    	    //IF CONTROLS ARE NULL, CREATE THEM
    	    if (btnAction == null)
    	    {
    		    setControlsStart();
    	    }
    	
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		
    	    int locY = 0;
    	    int locX = pW * 4;
    	    int textH = (int)gv.drawFontRegHeight;
            int spacing = textH;
            int tabX = pW * 4;
    	    int tabX2 = 5 * gv.squareSize + pW * 2;
    	    int leftStartY = pH * 4;
    	    int tabStartY = 5 * gv.squareSize + pW * 10;
    	
            //DRAW TEXT		
		    locY = (pH * 2);
		    gv.DrawText("Token Selection", locX + (gv.squareSize * 8), locY);
		    
		    //DRAW LEFT/RIGHT ARROWS and PAGE INDEX
		    btnPageIndex.Draw();
		    btnTokensLeft.Draw();
		    btnTokensRight.Draw();		
		
		    //DRAW ALL INVENTORY SLOTS		
		    int cntSlot = 0;
		    foreach (IbbButton btn in btnTokenSlot)
		    {
			    if (cntSlot == tknSlotIndex) {btn.glowOn = true;}
			    else {btn.glowOn = false;}
			    if ((cntSlot + (tknPageIndex * slotsPerPage)) < playerTokenList.Count)
			    {
                    gv.cc.DisposeOfBitmap(ref btn.Img2);
                    btn.Img2 = gv.cc.LoadBitmap(playerTokenList[cntSlot + (tknPageIndex * slotsPerPage)]);
			    }
			    else
			    {
				    btn.Img2 = null;
			    }
			    btn.Draw();
			    cntSlot++;
		    }		
		    
		    btnAction.Draw();
            btnExit.Draw();
        }
        public void onTouchTokenSelector(MouseEventArgs e, MouseEventType.EventType eventType)
	    {
            try
            {
                if (btnTokensLeft == null) { return; }                
                btnTokensLeft.glowOn = false;
                btnTokensRight.glowOn = false;
                btnAction.glowOn = false;
                btnExit.glowOn = false;

                switch (eventType)
                {
                    case MouseEventType.EventType.MouseDown:
                    case MouseEventType.EventType.MouseMove:
                        int x = (int)e.X;
                        int y = (int)e.Y;
                        if (btnTokensLeft.getImpact(x, y))
                        {
                            btnTokensLeft.glowOn = true;
                        }
                        else if (btnTokensRight.getImpact(x, y))
                        {
                            btnTokensRight.glowOn = true;
                        }
                        else if (btnAction.getImpact(x, y))
                        {
                            btnAction.glowOn = true;
                        }
                        else if (btnExit.getImpact(x, y))
                        {
                            btnExit.glowOn = true;
                        }
                        break;

                    case MouseEventType.EventType.MouseUp:
                        x = (int)e.X;
                        y = (int)e.Y;

                        btnTokensLeft.glowOn = false;
                        btnTokensRight.glowOn = false;
                        btnAction.glowOn = false;
                        btnExit.glowOn = false;

                        for (int j = 0; j < slotsPerPage; j++)
                        {
                            if (btnTokenSlot[j].getImpact(x, y))
                            {
                                if (tknSlotIndex == j)
                                {
                                    //return to calling screen
                                    if (callingScreen.Equals("party"))
                                    {
                                        gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex].tokenFilename = playerTokenList[GetIndex()];
                                        gv.screenType = "party";
                                        gv.screenParty.tokenLoad(gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex]);
                                    }
                                    else if (callingScreen.Equals("pcCreation"))
                                    {
                                        //set PC token filename to the currently selected image
                                        gv.screenPcCreation.pc.tokenFilename = playerTokenList[GetIndex()];
                                        gv.screenType = "pcCreation";
                                        gv.screenPcCreation.tokenLoad(gv.screenPcCreation.pc);
                                    }
                                    doCleanUp();
                                }
                                tknSlotIndex = j;
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
                        else if (btnAction.getImpact(x, y))
                        {
                            //return to calling screen
                            if (callingScreen.Equals("party"))
                            {
                                gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex].tokenFilename = playerTokenList[GetIndex()];
                                gv.screenType = "party";
                                gv.screenParty.tokenLoad(gv.screenParty.gv.mod.playerList[gv.cc.partyScreenPcIndex]);
                            }
                            else if (callingScreen.Equals("pcCreation"))
                            {
                                //set PC portrait filename to the currently selected image
                                gv.screenPcCreation.pc.tokenFilename = playerTokenList[GetIndex()];
                                gv.screenType = "pcCreation";
                                gv.screenPcCreation.tokenLoad(gv.screenPcCreation.pc);
                            }
                            doCleanUp();
                        }
                        else if (btnExit.getImpact(x, y))
                        {
                            //do nothing, return to calling screen
                            if (callingScreen.Equals("party"))
                            {
                                gv.screenType = "party";
                            }
                            else if (callingScreen.Equals("pcCreation"))
                            {
                                gv.screenType = "pcCreation";
                            }
                            doCleanUp();
                        }
                        break;
                }
            }
            catch
            { }
	    }
        public void doCleanUp()
	    {
		    btnTokenSlot.Clear();
		    btnTokensLeft = null;
		    btnTokensRight = null;
		    btnPageIndex = null;
		    btnAction = null;
            btnExit = null;
	    }
	
	    public int GetIndex()
	    {
		    return tknSlotIndex + (tknPageIndex * slotsPerPage);
	    }	
	    public bool isSelectedPtrSlotInPortraitListRange()
	    {
            return GetIndex() < playerTokenList.Count;
	    }
    }
}
