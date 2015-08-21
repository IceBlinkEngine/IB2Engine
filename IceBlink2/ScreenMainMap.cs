using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenMainMap 
    {
	    public Module mod;
	    public GameView gv;
	
	    private IbbButton btnParty = null;
	    private IbbButton btnJournal = null;
	    private IbbButton btnSettings = null;
	    private IbbButton btnSave = null;
	    private IbbButton btnCastOnMainMap = null;
	    private IbbButton btnWait = null;
	    public IbbToggleButton tglFullParty = null;
	    public IbbToggleButton tglMiniMap = null;
	    public IbbToggleButton tglGrid = null;
        public IbbToggleButton tglInteractionState = null;
        public IbbToggleButton tglAvoidConversation = null;
	    public IbbToggleButton tglClock = null;
        public List<FloatyText> floatyTextPool = new List<FloatyText>();
        public int mapStartLocXinPixels;

        public int movementDelayInMiliseconds = 100;
        private long timeStamp = 0;
        private bool finishedMove = true;
        
	
	    public ScreenMainMap(Module m, GameView g)
	    {
		    mod = m;
		    gv = g;
            mapStartLocXinPixels = 6 * gv.squareSize;
		    setControlsStart();
            setToggleButtonsStart();
	    }
	
	    public void setControlsStart()
	    {
		    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;
		    //int shift = squareSize / 2;
		
				
		    if (btnWait == null)
		    {
			    btnWait = new IbbButton(gv, 0.8f);	
			    btnWait.Text = "WAIT";
			    btnWait.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
			    btnWait.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnWait.X = 17 * gv.squareSize;
			    btnWait.Y = 8 * gv.squareSize + pH * 2;
                btnWait.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnWait.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnParty == null)
		    {
			    btnParty = new IbbButton(gv, 0.8f);
                btnParty.HotKey = "P";
			    btnParty.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
			    btnParty.Img2 = gv.cc.LoadBitmap("btnparty"); // BitmapFactory.decodeResource(getResources(), R.drawable.btnparty);
			    btnParty.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnParty.X = 6 * gv.squareSize + padW * 0 + gv.oXshift;
			    btnParty.Y = 9 * gv.squareSize + pH;
                btnParty.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnParty.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }		
		    if (btnJournal == null)
		    {
			    btnJournal = new IbbButton(gv, 0.8f);
                btnJournal.HotKey = "J";
			    btnJournal.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
			    btnJournal.Img2 = gv.cc.LoadBitmap("btnjournal"); // BitmapFactory.decodeResource(getResources(), R.drawable.btnjournal);
			    btnJournal.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnJournal.X = 8 * gv.squareSize + padW * 0 + gv.oXshift;
			    btnJournal.Y = 9 * gv.squareSize + pH;
                btnJournal.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnJournal.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnSettings == null)
		    {
			    btnSettings = new IbbButton(gv, 1.0f);
			    btnSettings.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
			    btnSettings.Img2 = gv.cc.LoadBitmap("btnsettings"); // BitmapFactory.decodeResource(getResources(), R.drawable.btnsettings);
			    btnSettings.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnSettings.X = 9 * gv.squareSize + padW * 0 + gv.oXshift;
			    btnSettings.Y = 9 * gv.squareSize + pH;
                btnSettings.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSettings.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
		    if (btnCastOnMainMap == null)
		    {
			    btnCastOnMainMap = new IbbButton(gv, 0.8f);
                btnCastOnMainMap.HotKey = "C";
			    btnCastOnMainMap.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
			    btnCastOnMainMap.Img2 = gv.cc.LoadBitmap("btnspell"); // BitmapFactory.decodeResource(getResources(), R.drawable.btnspell);
			    btnCastOnMainMap.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnCastOnMainMap.X = 10 * gv.squareSize + padW * 0 + gv.oXshift;
			    btnCastOnMainMap.Y = 9 * gv.squareSize + pH;
                btnCastOnMainMap.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnCastOnMainMap.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }	
		    if (btnSave == null)
		    {
			    btnSave = new IbbButton(gv, 0.8f);
			    btnSave.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnSave.ImgOff = gv.cc.LoadBitmap("btn_small_off");
			    btnSave.Img2 = gv.cc.LoadBitmap("btndisk"); // BitmapFactory.decodeResource(getResources(), R.drawable.btndisk);
			    btnSave.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
			    btnSave.X = 11 * gv.squareSize + padW * 0 + gv.oXshift;
			    btnSave.Y = 9 * gv.squareSize + pH;
                btnSave.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSave.Width = (int)(gv.ibbwidthR * gv.screenDensity);
		    }
	    }
	    public void setToggleButtonsStart()
        {
    	    int pW = (int)((float)gv.screenWidth / 100.0f);
		    int pH = (int)((float)gv.screenHeight / 100.0f);
		    int padW = gv.squareSize/6;
					
		    if (tglFullParty == null)
		    {
			    tglFullParty = new IbbToggleButton(gv);
			    tglFullParty.ImgOn = gv.cc.LoadBitmap("tgl_fullparty_on");
			    tglFullParty.ImgOff = gv.cc.LoadBitmap("tgl_fullparty_off");
			    tglFullParty.X = 0 * gv.squareSize + gv.oXshift + (gv.squareSize/2);
			    tglFullParty.Y = 9 * (gv.squareSize) + (gv.squareSize/2);
                tglFullParty.Height = (int)(gv.ibbheight/2 * gv.screenDensity);
                tglFullParty.Width = (int)(gv.ibbwidthR/2 * gv.screenDensity);
			    tglFullParty.toggleOn = true;
		    }
		    if (tglMiniMap == null)
		    {
			    tglMiniMap = new IbbToggleButton(gv);
			    tglMiniMap.ImgOn = gv.cc.LoadBitmap("tgl_minimap_on");
			    tglMiniMap.ImgOff = gv.cc.LoadBitmap("tgl_minimap_off");
			    tglMiniMap.X = 4 * gv.squareSize + gv.oXshift + (gv.squareSize/2);
			    tglMiniMap.Y = 9 * (gv.squareSize) + (gv.squareSize/2);
                tglMiniMap.Height = (int)(gv.ibbheight/2 * gv.screenDensity);
                tglMiniMap.Width = (int)(gv.ibbwidthR/2 * gv.screenDensity);
			    tglMiniMap.toggleOn = false;
		    }
		    if (tglGrid == null)
		    {
			    tglGrid = new IbbToggleButton(gv);
			    tglGrid.ImgOn = gv.cc.LoadBitmap("tgl_grid_on");
			    tglGrid.ImgOff = gv.cc.LoadBitmap("tgl_grid_off");
			    tglGrid.X = 1 * gv.squareSize + gv.oXshift + (gv.squareSize/2);
			    tglGrid.Y = 9 * (gv.squareSize) + (gv.squareSize/2);
                tglGrid.Height = (int)(gv.ibbheight/2 * gv.screenDensity);
                tglGrid.Width = (int)(gv.ibbwidthR/2 * gv.screenDensity);
			    tglGrid.toggleOn = true;
		    }
		    if (tglClock == null)
		    {
			    tglClock = new IbbToggleButton(gv);
			    tglClock.ImgOn = gv.cc.LoadBitmap("tgl_clock_on");
			    tglClock.ImgOff = gv.cc.LoadBitmap("tgl_clock_off");
			    tglClock.X = 2 * gv.squareSize + gv.oXshift + (gv.squareSize/2);
			    tglClock.Y = 9 * (gv.squareSize) + (gv.squareSize/2);
                tglClock.Height = (int)(gv.ibbheight/2 * gv.screenDensity);
                tglClock.Width = (int)(gv.ibbwidthR/2 * gv.screenDensity);
			    tglClock.toggleOn = true;
		    }
            if (tglInteractionState == null)
            {
                tglInteractionState = new IbbToggleButton(gv);
                tglInteractionState.ImgOn = gv.cc.LoadBitmap("tgl_state_on");
                tglInteractionState.ImgOff = gv.cc.LoadBitmap("tgl_state_off");
                tglInteractionState.X = 1 * gv.squareSize + gv.oXshift + (gv.squareSize / 2);
                tglInteractionState.Y = 8 * (gv.squareSize) + (gv.squareSize / 2);
                tglInteractionState.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglInteractionState.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
                tglInteractionState.toggleOn = false;
            }
            if (tglAvoidConversation == null)
            {
                tglAvoidConversation = new IbbToggleButton(gv);
                tglAvoidConversation.ImgOn = gv.cc.LoadBitmap("tgl_avoidConvo_on");
                tglAvoidConversation.ImgOff = gv.cc.LoadBitmap("tgl_avoidConvo_off");
                tglAvoidConversation.X = 0 * gv.squareSize + gv.oXshift + (gv.squareSize / 2);
                tglAvoidConversation.Y = 8 * (gv.squareSize) + (gv.squareSize / 2);
                tglAvoidConversation.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglAvoidConversation.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
                tglAvoidConversation.toggleOn = false;
            }


        }

	    //MAIN SCREEN
        public void redrawMain()
        {
            setExplored();
    	    gv.drawLog();
    	    //gv.cc.drawBlackMap();
    	    if (!mod.currentArea.areaDark)
		    {
	    	    if (mod.currentArea.IsWorldMap)
	    	    {
	    		    drawWorldMap();
	    	    }
	    	    else
	    	    {
	    		    drawMap();
	    	    }
	            drawProps();
	            if (mod.map_showGrid)
	            {
	        	    tglGrid.toggleOn = true;
	        	    drawGrid();
	            }
	            else
	            {
	        	    tglGrid.toggleOn = false;
	            }
		    }
            drawPlayer(); 
            if (!mod.currentArea.areaDark)
		    {
        	    drawMovingProps();
		    }
            drawMainMapFloatyText();     
            if (!mod.currentArea.areaDark)
		    {
                if (mod.currentArea.UseDayNightCycle)
                {
                    drawOverlayTints();
                    drawBlackTilesOverTints();
                }
        	    drawFogOfWar();
		    }
            drawFloatyTextPool();
            if (tglClock.toggleOn)
            {
        	    drawMainMapClockText();
            }
            drawControls();
            drawMiniMap();
            drawPortraits();
        }
        public void drawPortraits()
        {
            if (mod.playerList.Count > 0)
            {
                gv.cc.ptrPc0.Img = mod.playerList[0].portrait;
                gv.cc.ptrPc0.TextHP = mod.playerList[0].hp + "/" + mod.playerList[0].hpMax;
                gv.cc.ptrPc0.TextSP = mod.playerList[0].sp + "/" + mod.playerList[0].spMax;
                if (gv.mod.selectedPartyLeader == 0) { gv.cc.ptrPc0.glowOn = true; }
                else { gv.cc.ptrPc0.glowOn = false; }
                gv.cc.ptrPc0.Draw();
            }
            if (mod.playerList.Count > 1)
            {
                gv.cc.ptrPc1.Img = mod.playerList[1].portrait;
                gv.cc.ptrPc1.TextHP = mod.playerList[1].hp + "/" + mod.playerList[1].hpMax;
                gv.cc.ptrPc1.TextSP = mod.playerList[1].sp + "/" + mod.playerList[1].spMax;
                if (gv.mod.selectedPartyLeader == 1) { gv.cc.ptrPc1.glowOn = true; }
                else { gv.cc.ptrPc1.glowOn = false; }
                gv.cc.ptrPc1.Draw();
            }
            if (mod.playerList.Count > 2)
            {
                gv.cc.ptrPc2.Img = mod.playerList[2].portrait;
                gv.cc.ptrPc2.TextHP = mod.playerList[2].hp + "/" + mod.playerList[2].hpMax;
                gv.cc.ptrPc2.TextSP = mod.playerList[2].sp + "/" + mod.playerList[2].spMax;
                if (gv.mod.selectedPartyLeader == 2) { gv.cc.ptrPc2.glowOn = true; }
                else { gv.cc.ptrPc2.glowOn = false; }
                gv.cc.ptrPc2.Draw();
            }
            if (mod.playerList.Count > 3)
            {
                gv.cc.ptrPc3.Img = mod.playerList[3].portrait;
                gv.cc.ptrPc3.TextHP = mod.playerList[3].hp + "/" + mod.playerList[3].hpMax;
                gv.cc.ptrPc3.TextSP = mod.playerList[3].sp + "/" + mod.playerList[3].spMax;
                if (gv.mod.selectedPartyLeader == 3) { gv.cc.ptrPc3.glowOn = true; }
                else { gv.cc.ptrPc3.glowOn = false; }
                gv.cc.ptrPc3.Draw();
            }
            if (mod.playerList.Count > 4)
            {
                gv.cc.ptrPc4.Img = mod.playerList[4].portrait;
                gv.cc.ptrPc4.TextHP = mod.playerList[4].hp + "/" + mod.playerList[4].hpMax;
                gv.cc.ptrPc4.TextSP = mod.playerList[4].sp + "/" + mod.playerList[4].spMax;
                if (gv.mod.selectedPartyLeader == 4) { gv.cc.ptrPc4.glowOn = true; }
                else { gv.cc.ptrPc4.glowOn = false; }
                gv.cc.ptrPc4.Draw();
            }
            if (mod.playerList.Count > 5)
            {
                gv.cc.ptrPc5.Img = mod.playerList[5].portrait;
                gv.cc.ptrPc5.TextHP = mod.playerList[5].hp + "/" + mod.playerList[5].hpMax;
                gv.cc.ptrPc5.TextSP = mod.playerList[5].sp + "/" + mod.playerList[5].spMax;
                if (gv.mod.selectedPartyLeader == 5) { gv.cc.ptrPc5.glowOn = true; }
                else { gv.cc.ptrPc5.glowOn = false; }
                gv.cc.ptrPc5.Draw();
            }
        }
        public void drawWorldMap()
        {
    	    int minX = mod.PlayerLocationX - gv.playerOffset;
		    if (minX < 0) { minX = 0; }
		    int minY = mod.PlayerLocationY - gv.playerOffset;
		    if (minY < 0) { minY = 0; }
		
		    int maxX = mod.PlayerLocationX + gv.playerOffset + 1;
		    if (maxX > this.mod.currentArea.MapSizeX) { maxX = this.mod.currentArea.MapSizeX; }
		    int maxY = mod.PlayerLocationY + gv.playerOffset + 1;
		    if (maxY > this.mod.currentArea.MapSizeY) { maxY = this.mod.currentArea.MapSizeY; }
		
		    //for (int x = 0; x < this.mod.currentArea.MapSizeX; x++)
		    for (int x = minX; x < maxX; x++)
            {
			    //for (int y = 0; y < this.mod.currentArea.MapSizeY; y++)
                for (int y = minY; y < maxY; y++)
                {
            	    int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
            	    int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
            	    int brX = gv.squareSize;
            	    int brY = gv.squareSize;

            	    Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];

                    IbRect src1 = new IbRect(0, 0, gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Height);
                    IbRect src2 = new IbRect(0, 0, gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Height);
                    IbRect src3 = new IbRect(0, 0, gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Height);
                    IbRect src4 = new IbRect(0, 0, gv.cc.tileBitmapList[tile.Layer4Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer4Filename].PixelSize.Height);
                    IbRect src5 = new IbRect(0, 0, gv.cc.tileBitmapList[tile.Layer5Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer5Filename].PixelSize.Height);
                    IbRect dst = new IbRect(tlX + gv.oXshift + ((gv.playerOffset + 2) * gv.squareSize), tlY, brX, brY);
                
                    gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer1Filename], src1, dst);
                    gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer2Filename], src2, dst);
                    gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer3Filename], src3, dst);
                    gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer4Filename], src4, dst);
                    gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer5Filename], src5, dst);
                }
            }
        }
	    public void drawMap()
	    {
		    int srcUX = 0, srcUY = 0, srcDX = 0, srcDY = 0;
		    int dstUX = 0, dstUY = 0, dstDX = 0, dstDY = 0;
            int bmpWidth = gv.cc.bmpMap.PixelSize.Width;
            int bmpHeight = gv.cc.bmpMap.PixelSize.Height;
            int mapSquareSize = 50;
		    if (mod.PlayerLocationX < gv.playerOffset) //at left edge of map
		    {
			    srcUX = 0;
                srcDX = ((mod.PlayerLocationX + 1) * mapSquareSize) + (gv.playerOffset * mapSquareSize);
			    dstUX = (gv.playerOffset * gv.squareSize) - (mod.PlayerLocationX * gv.squareSize);
			    dstDX = dstUX + (int)(srcDX * 2 * gv.screenDensity);
		    }
            else if ((mod.PlayerLocationX >= gv.playerOffset) && (mod.PlayerLocationX < (bmpWidth / mapSquareSize) - gv.playerOffset))
		    {
                srcUX = (mod.PlayerLocationX * mapSquareSize) - (gv.playerOffset * mapSquareSize);
                srcDX = srcUX + (mapSquareSize * ((gv.playerOffset * 2) + 1));
			    dstUX = 0;
			    dstDX = gv.squareSize * ((gv.playerOffset * 2) + 1);
		    }
		    else //mod.PlayerLocationX >= width - 3  //at right edge of map
		    {
                srcUX = (mod.PlayerLocationX * mapSquareSize) - (gv.playerOffset * mapSquareSize);
			    srcDX = bmpWidth;
			    dstUX = 0;
			    dstDX = (int)(srcDX * 2 * gv.screenDensity) - (int)(srcUX * 2 * gv.screenDensity);		
		    }

		    if (mod.PlayerLocationY < gv.playerOffset) //at top of map
		    {
			    srcUY = 0;
                srcDY = ((mod.PlayerLocationY + 1) * mapSquareSize) + (gv.playerOffset * mapSquareSize);
			    dstUY = (gv.playerOffset * gv.squareSize) - (mod.PlayerLocationY * gv.squareSize);
			    dstDY = dstUY + (int)(srcDY * 2 * gv.screenDensity);
		    }
            else if ((mod.PlayerLocationY >= gv.playerOffset) && (mod.PlayerLocationY < (bmpHeight / mapSquareSize) - gv.playerOffset))
		    {
                srcUY = (mod.PlayerLocationY * mapSquareSize) - (gv.playerOffset * mapSquareSize);
                srcDY = srcUY + (mapSquareSize * ((gv.playerOffset * 2) + 1));
			    dstUY = 0;
			    dstDY = gv.squareSize * ((gv.playerOffset * 2) + 1);
		    }
		    else //mod.PlayerLocationY >= height - 3  //at bottom of map
		    {
                srcUY = (mod.PlayerLocationY * mapSquareSize) - (gv.playerOffset * mapSquareSize);
			    srcDY = bmpHeight;
			    dstUY = 0;
			    dstDY = (int)(srcDY * 2 * gv.screenDensity) - (int)(srcUY * 2 * gv.screenDensity);
		    }
		    
            IbRect src = new IbRect(srcUX, srcUY, srcDX - srcUX, srcDY - srcUY);
            IbRect dst = new IbRect(dstUX + gv.oXshift + mapStartLocXinPixels, dstUY, dstDX - dstUX, dstDY - dstUY); 
            gv.DrawBitmap(gv.cc.bmpMap, src, dst);
	    }
	    public void drawProps()
	    {
		    foreach (Prop p in mod.currentArea.Props)
		    {
			    if ((p.isShown) && (!p.isMover))
			    {
				    if ((p.LocationX >= mod.PlayerLocationX - gv.playerOffset) && (p.LocationX <= mod.PlayerLocationX + gv.playerOffset)
					    && (p.LocationY >= mod.PlayerLocationY - gv.playerOffset) && (p.LocationY <= mod.PlayerLocationY + gv.playerOffset))
				    {
					    //prop X - playerX
					    int x = ((p.LocationX - mod.PlayerLocationX) * gv.squareSize) + (gv.playerOffset * gv.squareSize);
					    int y = ((p.LocationY - mod.PlayerLocationY) * gv.squareSize) + (gv.playerOffset * gv.squareSize);
					    //Rect src = new Rect(0, 0, squareSizeInPixels, squareSizeInPixels);
                        IbRect src = new IbRect(0, 0, p.token.PixelSize.Width, p.token.PixelSize.Width);
                        IbRect dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
			            gv.DrawBitmap(p.token, src, dst, !p.PropFacingLeft, false);

                        if (mod.showInteractionState == true)
                        {
                            if (p.EncounterWhenOnPartySquare != "none")
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("encounter_indicator");
                                src = new IbRect(0, 0, p.token.PixelSize.Width / 2, p.token.PixelSize.Width / 2);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                continue;
                            }

                            if (p.unavoidableConversation == true)
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                                src = new IbRect(0, 0, p.token.PixelSize.Width / 2, p.token.PixelSize.Width / 2);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                continue;
                            }

                            if (p.ConversationWhenOnPartySquare != "none")
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("optional_conversation_indicator");
                                src = new IbRect(0, 0, p.token.PixelSize.Width / 2, p.token.PixelSize.Width / 2);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                continue;
                            }
                        }
				    }
			    }
		    }
	    }
	    public void drawMovingProps()
	    {
		    foreach (Prop p in mod.currentArea.Props)
		    {
			    if ((p.isShown) && (p.isMover))
			    {
				    if ((p.LocationX >= mod.PlayerLocationX - gv.playerOffset) && (p.LocationX <= mod.PlayerLocationX + gv.playerOffset)
					    && (p.LocationY >= mod.PlayerLocationY - gv.playerOffset) && (p.LocationY <= mod.PlayerLocationY + gv.playerOffset))
				    {
					    //prop X - playerX
					    int x = ((p.LocationX - mod.PlayerLocationX) * gv.squareSize) + (gv.playerOffset * gv.squareSize);
					    int y = ((p.LocationY - mod.PlayerLocationY) * gv.squareSize) + (gv.playerOffset * gv.squareSize);
					    //Rect src = new Rect(0, 0, squareSizeInPixels, squareSizeInPixels);
                        IbRect src = new IbRect(0, 0, p.token.PixelSize.Width, p.token.PixelSize.Width);
                        IbRect dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
			            gv.DrawBitmap(p.token, src, dst);

                        if(mod.showInteractionState == true)
                        {
                        if (p.EncounterWhenOnPartySquare != "none")
                        {
                            Bitmap interactionStateIndicator = gv.cc.LoadBitmap("encounter_indicator");
                            src = new IbRect(0, 0, 50, 50);
                            gv.DrawBitmap(interactionStateIndicator, src, dst);
                            continue;
                        }

                        if (p.unavoidableConversation == true)
                        {
                            Bitmap interactionStateIndicator = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                            src = new IbRect(0, 0, 50, 50);
                            gv.DrawBitmap(interactionStateIndicator, src, dst);
                            continue;
                        }
                        
                        if (p.ConversationWhenOnPartySquare != "none")
                        {
                            Bitmap interactionStateIndicator = gv.cc.LoadBitmap("optional_conversation_indicator");
                            src = new IbRect(0, 0, 50, 50);
                            gv.DrawBitmap(interactionStateIndicator, src, dst);
                            continue;
                        }
                        }
				    }
			    }
		    }
	    }
	    public void drawMiniMap()
	    {
		    if ((mod.currentArea.IsWorldMap) && (tglMiniMap.toggleOn))
		    {
			    int pW = (int)((float)gv.screenWidth / 100.0f);
			    int pH = (int)((float)gv.screenHeight / 100.0f);
			    int shift = pW;
			    Bitmap minimap = gv.cc.LoadBitmap(mod.currentArea.Filename);

                int mapSqrX = minimap.PixelSize.Width / 5;
                int mapSqrY = minimap.PixelSize.Height / 5;
			    int drawW = mapSqrX * pW/2;
			    int drawH = mapSqrY * pW/2;
			/*TODO
			    //draw a dark border
	            Paint pnt = new Paint();
			    pnt.setColor(Color.DKGRAY);
			    pnt.setStrokeWidth(pW * 2);
			    pnt.setStyle(Paint.Style.STROKE);	
			    canvas.drawRect(new Rect(gv.oXshift, pH, gv.oXshift + drawW + pW, pH + drawH + pW), pnt);
			*/
			    //draw minimap
                IbRect src = new IbRect(0, 0, minimap.PixelSize.Width, minimap.PixelSize.Height);
                IbRect dst = new IbRect(gv.oXshift + shift + mapStartLocXinPixels, pH + shift, drawW, drawH);
	            gv.DrawBitmap(minimap, src, dst);
                /*TODO
                //draw Fog of War
                if (mod.currentArea.UseMiniMapFogOfWar)
                {
                    pnt = new Paint();
                    pnt.setColor(Color.BLACK);
                    pnt.setStrokeWidth(pW);
                    pnt.setStyle(Paint.Style.FILL);
                    for (int x = 0; x < mod.currentArea.MapSizeX; x++) {
                        for (int y = 0; y < mod.currentArea.MapSizeY; y++) {
                            int xx = x * pW / 2 + shift;
                            int yy = y * pW / 2 + shift;
                            if (!mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].Visible) 
                            {
                                canvas.drawRect(new Rect(xx + gv.oXshift, yy + pH, xx + pW / 2 + gv.oXshift, yy + pH + pW / 2), pnt);
                            }
                        }
                    }
                }

	            //draw a location marker square RED
	            pnt = new Paint();
			    pnt.setColor(Color.RED);
			    pnt.setStrokeWidth(pW);
			    pnt.setStyle(Paint.Style.FILL);	
			    int x = mod.PlayerLocationX * pW/2 + shift;
			    int y = mod.PlayerLocationY * pW/2 + shift;
			    canvas.drawRect(new Rect(x + gv.oXshift, y + pH, x + pW/2 + gv.oXshift, y + pH + pW/2), pnt);
                */
		    }
	    }
	
	    public void drawPlayer()
	    {
            if (mod.selectedPartyLeader >= mod.playerList.Count)
            {
                mod.selectedPartyLeader = 0;
            }
		    int x = gv.playerOffset * gv.squareSize;
		    int y = gv.playerOffset * gv.squareSize;
		    int shift = gv.squareSize / 3;
            IbRect src = new IbRect(0, 0, mod.playerList[mod.selectedPartyLeader].token.PixelSize.Width, mod.playerList[mod.selectedPartyLeader].token.PixelSize.Width);
            IbRect dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize); 
            if (mod.showPartyToken)
            {
                gv.DrawBitmap(mod.partyTokenBitmap, src, dst, !mod.playerList[0].combatFacingLeft, false);
            }
            else
            {
	            if (tglFullParty.toggleOn)
	            {
		            for (int i = mod.playerList.Count - 1; i >= 0; i--)
		            {		
		        	    if (i == 0)
		        	    {
                            dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
		        	    }
		        	    if (i == 1)
		        	    {
                            dst = new IbRect(x + gv.oXshift + shift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
		        	    }
		        	    if (i == 2)
		        	    {
                            dst = new IbRect(x + gv.oXshift - shift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
		        	    }
		        	    if (i == 3)
		        	    {
                            dst = new IbRect(x + gv.oXshift + (shift * 2) + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
		        	    }
		        	    if (i == 4)
		        	    {
                            dst = new IbRect(x + gv.oXshift - (shift * 2) + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
		        	    }
		        	    if (i == 5)
		        	    {
                            dst = new IbRect(x + gv.oXshift - (shift * 3) + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
		        	    }
		            }
	            }
	            //always draw party leader on top
	            if (mod.selectedPartyLeader == 0)
	    	    {
	        	    if (tglFullParty.toggleOn)
	        	    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize); 
	        	    }
	        	    else
	        	    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
	        	    }
                    gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
	    	    }
	            else if (mod.selectedPartyLeader == 1)
	    	    {
	        	    if (tglFullParty.toggleOn)
	        	    {
                        dst = new IbRect(x + gv.oXshift + shift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize); 
	        	    }
	        	    else
	        	    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
	        	    }
                    gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
	    	    }
	            else if (mod.selectedPartyLeader == 2)
	    	    {
	        	    if (tglFullParty.toggleOn)
	        	    {
                        dst = new IbRect(x + gv.oXshift - shift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize); 
	        	    }
	        	    else
	        	    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
	        	    }
                    gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
	    	    }
	            else if (mod.selectedPartyLeader == 3)
	    	    {
	        	    if (tglFullParty.toggleOn)
	        	    {
                        dst = new IbRect(x + gv.oXshift + (shift * 2) + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize); 
	        	    }
	        	    else
	        	    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
	        	    }
                    gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
	    	    }
	            else if (mod.selectedPartyLeader == 4)
	    	    {
	        	    if (tglFullParty.toggleOn)
	        	    {
                        dst = new IbRect(x + gv.oXshift - (shift * 2) + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize); 
	        	    }
	        	    else
	        	    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
	        	    }
                    gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
	    	    }
	            else if (mod.selectedPartyLeader == 5)
	    	    {
	        	    if (tglFullParty.toggleOn)
	        	    {
                        dst = new IbRect(x + gv.oXshift - (shift * 3) + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize); 
	        	    }
	        	    else
	        	    {
                        dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
	        	    }
                    gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
	    	    }
            }
	    }
	    public void drawGrid()
	    {
		    int minX = mod.PlayerLocationX - gv.playerOffset;
		    if (minX < 0) { minX = 0; }
		    int minY = mod.PlayerLocationY - gv.playerOffset;
		    if (minY < 0) { minY = 0; }
		
		    int maxX = mod.PlayerLocationX + gv.playerOffset + 1;
		    if (maxX > this.mod.currentArea.MapSizeX) { maxX = this.mod.currentArea.MapSizeX; }
		    int maxY = mod.PlayerLocationY + gv.playerOffset + 1;
		    if (maxY > this.mod.currentArea.MapSizeY) { maxY = this.mod.currentArea.MapSizeY; }
		
		    //for (int x = 0; x < this.mod.currentArea.MapSizeX; x++)
		    for (int x = minX; x < maxX; x++)
            {
			    //for (int y = 0; y < this.mod.currentArea.MapSizeY; y++)
                for (int y = minY; y < maxY; y++)
                {
            	    int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
            	    int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
            	    int brX = gv.squareSize;
            	    int brY = gv.squareSize;
                    IbRect src = new IbRect(0, 0, gv.cc.walkBlocked.PixelSize.Width, gv.cc.walkBlocked.PixelSize.Height);
                    IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                    if (mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].LoSBlocked)
                    {
                	    gv.DrawBitmap(gv.cc.losBlocked, src, dst);
                	}
                    if (mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].Walkable != true)
                    {
                	    gv.DrawBitmap(gv.cc.walkBlocked, src, dst);
                    }
                    else
                    {
                	    gv.DrawBitmap(gv.cc.walkPass, src, dst);
                    }                
                }
            }  
	    }
	    public void drawMainMapFloatyText()
	    {
            /*TODO
		    int txtH = (int)gv.floatyTextPaint.getTextSize();
		
		    gv.floatyTextPaint.setStyle(Paint.Style.FILL);
		    gv.floatyTextPaint.setColor(Color.BLACK);
		    for (int x = -2; x <= 2; x++)
		    {
			    for (int y = -2; y <= 2; y++)
			    {
				    canvas.drawText(gv.cc.floatyText, gv.cc.floatyTextLoc.X + gv.oXshift + x, gv.cc.floatyTextLoc.Y + txtH + y, gv.floatyTextPaint);				
			    }
		    }		
		    gv.floatyTextPaint.setStyle(Paint.Style.FILL);
		    gv.floatyTextPaint.setColor(Color.WHITE);
		    canvas.drawText(gv.cc.floatyText, gv.cc.floatyTextLoc.X + gv.oXshift, gv.cc.floatyTextLoc.Y + txtH, gv.floatyTextPaint);	
	        */
	    }
	    public void drawOverlayTints()
	    {
            IbRect src = new IbRect(0, 0, gv.cc.tint_sunset.PixelSize.Width, gv.cc.tint_sunset.PixelSize.Height);
            IbRect dst = new IbRect(gv.oXshift + mapStartLocXinPixels, 0, (gv.squareSize * 9), (gv.squareSize * 9));
		    int dawn = 5 * 60;
		    int sunrise = 6 * 60;
		    int day = 7 * 60;
		    int sunset = 17 * 60;
		    int dusk = 18 * 60;
		    int night = 20 * 60;
		    int time = gv.mod.WorldTime % 1440;
		    if ((time >= dawn) && (time < sunrise))
		    {
			    gv.DrawBitmap(gv.cc.tint_dawn, src, dst);
		    }
		    else if ((time >= sunrise) && (time < day))
		    {
			    gv.DrawBitmap(gv.cc.tint_sunrise, src, dst);
		    }
		    else if ((time >= day) && (time < sunset))
		    {
			    //no tint for day
		    }
		    else if ((time >= sunset) && (time < dusk))
		    {
			    gv.DrawBitmap(gv.cc.tint_sunset, src, dst);
		    }
		    else if ((time >= dusk) && (time < night))
		    {
			    gv.DrawBitmap(gv.cc.tint_dusk, src, dst);
		    }
		    else if ((time >= night) || (time < dawn))
		    {
			    gv.DrawBitmap(gv.cc.tint_night, src, dst);
		    }
        
	    }
	    public void drawMainMapClockText()
	    {
		    int timeofday = mod.WorldTime % (24 * 60);
		    int hour = timeofday / 60;
		    int minute = timeofday % 60;
		    String sMinute = minute + "";
		    if (minute < 10)
		    {
			    sMinute = "0" + minute;
		    }

            int txtH = (int)gv.drawFontRegHeight;
		
		    for (int x = -2; x <= 2; x++)
		    {
			    for (int y = -2; y <= 2; y++)
			    {
                    gv.DrawText(hour + ":" + sMinute, new IbRect(gv.oXshift + x + mapStartLocXinPixels, 9 * gv.squareSize - txtH + y, 100, 100), 1.0f, Color.Black);				
			    }
		    }
            gv.DrawText(hour + ":" + sMinute, new IbRect(gv.oXshift + mapStartLocXinPixels, 9 * gv.squareSize - txtH, 100, 100), 1.0f, Color.White);		
            
	    }
	    public void drawFogOfWar()
	    {
		    int minX = mod.PlayerLocationX - gv.playerOffset;
		    if (minX < 0) { minX = 0; }
		    int minY = mod.PlayerLocationY - gv.playerOffset;
		    if (minY < 0) { minY = 0; }
		
		    int maxX = mod.PlayerLocationX + gv.playerOffset + 1;
		    if (maxX > this.mod.currentArea.MapSizeX) { maxX = this.mod.currentArea.MapSizeX; }
		    int maxY = mod.PlayerLocationY + gv.playerOffset + 1;
		    if (maxY > this.mod.currentArea.MapSizeY) { maxY = this.mod.currentArea.MapSizeY; }
		
		    //for (int x = 0; x < this.mod.currentArea.MapSizeX; x++)
		    for (int x = minX; x < maxX; x++)
            {
			    //for (int y = 0; y < this.mod.currentArea.MapSizeY; y++)
                for (int y = minY; y < maxY; y++)
                {
            	    int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
            	    int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
            	    int brX = gv.squareSize;
            	    int brY = gv.squareSize;
                    IbRect src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                    IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                    if (!mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].Visible)
                    {
                	    gv.DrawBitmap(gv.cc.black_tile, src, dst);
                    }
                }
            }  
	    }
        public void drawBlackTilesOverTints()
        {
            //at left edge
            if (mod.PlayerLocationX < 4)
            {
                drawColumnOfBlack(0);
            }
            if (mod.PlayerLocationX < 3)
            {
                drawColumnOfBlack(0);
                drawColumnOfBlack(1);
            }
            if (mod.PlayerLocationX < 2)
            {
                drawColumnOfBlack(1);
                drawColumnOfBlack(2);
            }
            if (mod.PlayerLocationX < 1)
            {
                drawColumnOfBlack(2);
                drawColumnOfBlack(3);
            }
            //at top edge
            if (mod.PlayerLocationY < 4)
            {
                drawRowOfBlack(0);
            }
            if (mod.PlayerLocationY < 3)
            {
                drawRowOfBlack(0);
                drawRowOfBlack(1);
            }
            if (mod.PlayerLocationY < 2)
            {
                drawRowOfBlack(1);
                drawRowOfBlack(2);
            }
            if (mod.PlayerLocationY < 1)
            {
                drawRowOfBlack(2);
                drawRowOfBlack(3);
            }
            
            //at right edge
            if (mod.PlayerLocationX > mod.currentArea.MapSizeX - 5)
            {
                drawColumnOfBlack(8);
            }

            if (mod.PlayerLocationX > mod.currentArea.MapSizeX - 4)
            {
                drawColumnOfBlack(7);
                drawColumnOfBlack(8);
            }
            if (mod.PlayerLocationX > mod.currentArea.MapSizeX - 3)
            {

                drawColumnOfBlack(6);
                drawColumnOfBlack(7);
                drawColumnOfBlack(8);
            }
            if (mod.PlayerLocationX > mod.currentArea.MapSizeX - 2)
            {

                drawColumnOfBlack(5);
                drawColumnOfBlack(6);
                drawColumnOfBlack(7);
                drawColumnOfBlack(8);
            }
            
            //at bottom edge
            if (mod.PlayerLocationY > mod.currentArea.MapSizeY - 5)
            {
                drawRowOfBlack(8);
                //drawRowOfBlack(9);
                //drawRowOfBlack(10);
            }

            if (mod.PlayerLocationY > mod.currentArea.MapSizeY - 4)
            {
               
                drawRowOfBlack(7);
                drawRowOfBlack(8);
                //drawRowOfBlack(9);
                //drawRowOfBlack(10);
            }
            if (mod.PlayerLocationY > mod.currentArea.MapSizeY - 3)
            {
                
                drawRowOfBlack(6);
                drawRowOfBlack(7);
                drawRowOfBlack(8);
                //drawRowOfBlack(9);
                //drawRowOfBlack(10);
            }
            if (mod.PlayerLocationY > mod.currentArea.MapSizeY - 2)
            {
                
                drawRowOfBlack(5);
                drawRowOfBlack(6);
                drawRowOfBlack(7);
                drawRowOfBlack(8);
                //drawRowOfBlack(9);
                //drawRowOfBlack(10);
            }
            //if player location is 1 draw two row or col
            //if player location is 2 draw one row or col
            //if player location is mapsize, draw three row or col
            //if player location is mapsize - 1, draw two row or col
            //if player location is mapsize - 2, draw one row or col
            int minX = mod.PlayerLocationX - gv.playerOffset;
            if (minX < 0) { minX = 0; }
            int minY = mod.PlayerLocationY - gv.playerOffset;
            if (minY < 0) { minY = 0; }

            int maxX = mod.PlayerLocationX + gv.playerOffset + 1;
            if (maxX > this.mod.currentArea.MapSizeX) { maxX = this.mod.currentArea.MapSizeX; }
            int maxY = mod.PlayerLocationY + gv.playerOffset + 1;
            if (maxY > this.mod.currentArea.MapSizeY) { maxY = this.mod.currentArea.MapSizeY; }
        }
	    public void drawControls()
	    {
		    gv.cc.ctrlUpArrow.Draw();
		    gv.cc.ctrlDownArrow.Draw();
		    gv.cc.ctrlLeftArrow.Draw();
		    gv.cc.ctrlRightArrow.Draw();
		    btnWait.Draw();
		    gv.cc.tglSound.Draw();
		    tglFullParty.Draw();
		    if (mod.currentArea.IsWorldMap)
		    {
			    tglMiniMap.Draw();
		    }
		    tglGrid.Draw();
            tglInteractionState.Draw();
            tglAvoidConversation.Draw();
		    tglClock.Draw();
		
		    //check for levelup available and switch button image
		    checkLevelUpAvailable();
				
		    btnParty.Draw();
		    gv.cc.btnInventory.Draw();
		    btnJournal.Draw();
		    btnSettings.Draw();
            if (mod.allowSave)
            {
                btnSave.btnState = buttonState.Normal;
            }
            else
            {
                btnSave.btnState = buttonState.Off;
            }
            btnSave.Draw();
		    btnCastOnMainMap.Draw();			
	    }
	    public void drawFloatyTextPoolOld()
	    {            
		    if (floatyTextPool.Count > 0)
		    {
			    int txtH = (int)gv.drawFontRegHeight;
			    int pH = (int)((float)gv.screenHeight / 200.0f);
					
			    foreach (FloatyText ft in floatyTextPool)
			    {
				    if (gv.cc.getDistance(ft.location, new Coordinate(mod.PlayerLastLocationX, mod.PlayerLocationY)) > 3)
				    {
					    continue; //out of range from view so skip drawing floaty message
				    }
				    List<string> wrapList = this.wrapList(ft.value, 12);
				
				    for (int i = 0; i < wrapList.Count; i++)
				    {
					    //location.X should be the the props actual map location in squares (not screen location)
					    int xLoc = (ft.location.X + gv.playerOffset - mod.PlayerLocationX) * gv.squareSize;
					    int yLoc = ((ft.location.Y + gv.playerOffset - mod.PlayerLocationY) * gv.squareSize) - (pH * ft.z) + (i * txtH);
					
					    //gv.floatyTextPaint.setStyle(Paint.Style.FILL);
					    //gv.floatyTextPaint.setColor(Color.BLACK);
					    for (int x = -2; x <= 2; x++)
					    {
						    for (int y = -2; y <= 2; y++)
						    {
							    gv.DrawText(wrapList[i], xLoc + x + gv.oXshift, yLoc + y + txtH, 1.0f, Color.Black);
						    }
					    }
					    //gv.floatyTextPaint.setStyle(Paint.Style.FILL);
                        Color colr = Color.Yellow;
					    if (ft.color.Equals("yellow"))
					    {
                            colr = Color.Yellow;
					    }
					    else if (ft.color.Equals("blue"))
					    {
                            colr = Color.Blue;
					    }
					    else if (ft.color.Equals("green"))
					    {
                            colr = Color.Lime;
					    }
					    else if (ft.color.Equals("red"))
					    {
                            colr = Color.Red;
					    }
					    else
					    {
                            colr = Color.White;
					    }
					    gv.DrawText(wrapList[i], xLoc + gv.oXshift, yLoc + txtH, 1.0f, colr);
				    }
			    }
		    }            
	    }
        public void drawFloatyTextPool()
        {
            if (floatyTextPool.Count > 0)
            {
                int txtH = (int)gv.drawFontRegHeight;
                int pH = (int)((float)gv.screenHeight / 200.0f);

                foreach (FloatyText ft in floatyTextPool)
                {
                    if (gv.cc.getDistance(ft.location, new Coordinate(mod.PlayerLastLocationX, mod.PlayerLocationY)) > 3)
                    {
                        continue; //out of range from view so skip drawing floaty message
                    }
                    
                        //location.X should be the the props actual map location in squares (not screen location)
                        int xLoc = (ft.location.X + gv.playerOffset - mod.PlayerLocationX) * gv.squareSize;
                        int yLoc = ((ft.location.Y + gv.playerOffset - mod.PlayerLocationY) * gv.squareSize) - (pH * ft.z);

                        //gv.floatyTextPaint.setStyle(Paint.Style.FILL);
                        //gv.floatyTextPaint.setColor(Color.BLACK);
                        for (int x = -2; x <= 2; x++)
                        {
                            for (int y = -2; y <= 2; y++)
                            {
                                gv.DrawText(ft.value, new IbRect(xLoc + x + gv.oXshift + mapStartLocXinPixels, yLoc + y + txtH, gv.squareSize * 2, 1000), 0.8f, Color.Black);
                            }
                        }
                        //gv.floatyTextPaint.setStyle(Paint.Style.FILL);
                        Color colr = Color.Yellow;
                        if (ft.color.Equals("yellow"))
                        {
                            colr = Color.Yellow;
                        }
                        else if (ft.color.Equals("blue"))
                        {
                            colr = Color.Blue;
                        }
                        else if (ft.color.Equals("green"))
                        {
                            colr = Color.Lime;
                        }
                        else if (ft.color.Equals("red"))
                        {
                            colr = Color.Red;
                        }
                        else
                        {
                            colr = Color.White;
                        }
                        gv.DrawText(ft.value, new IbRect(xLoc + gv.oXshift + mapStartLocXinPixels, yLoc + txtH, gv.squareSize * 2, 1000), 0.8f, colr);
                        //gv.DrawText(wrapList[i], xLoc + gv.oXshift, yLoc + txtH, 1.0f, colr);
                    
                }
            }
        }

        public void drawColumnOfBlack(int col)
        {
            for (int y = 0; y < 9; y++)
            {
                int tlX = col * gv.squareSize;
                int tlY = y * gv.squareSize;
                int brX = gv.squareSize;
                int brY = gv.squareSize;
                IbRect src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                gv.DrawBitmap(gv.cc.black_tile, src, dst);
            }
        }
        public void drawRowOfBlack(int row)
        {
            for (int x = 0; x < 9; x++)
            {
                int tlX = x * gv.squareSize;
                int tlY = row * gv.squareSize;
                int brX = gv.squareSize;
                int brY = gv.squareSize;
                IbRect src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                gv.DrawBitmap(gv.cc.black_tile, src, dst);
            }
        }
	
	    /*public Runnable doFloatyText = new Runnable()
	    {
		    @Override
		    public void run()
		    {
			    gv.invalidate();			
	    	    if (floatyTextPool.size() > 0)
			    {
				    for (int i = floatyTextPool.size() - 1; i >= 0; i--)
	                {
	                    if (floatyTextPool.get(i).timer > floatyTextPool.get(i).timerLength)
	                    {
	                	    floatyTextPool.remove(i);
	                    }
	                    else
	                    {
	                	    floatyTextPool.get(i).z++; //increase float height multiplier
	                	    floatyTextPool.get(i).timer += 400;
	                    }
	                }
				    doFloatyTextLoop();
			    }
		    }
	    };*/
	
	    public void doFloatyTextLoop()
	    {
		    gv.postDelayed("doFloatyTextMainMap", 100);
	    }
	    public void addFloatyText(int sqrX, int sqrY, String value, String color, int length)
	    {
		    floatyTextPool.Add(new FloatyText(sqrX, sqrY, value, color, length));
	    }
	
	    public void onTouchMain(MouseEventArgs e, MouseEventType.EventType eventType)
	    {	
		    gv.cc.ctrlUpArrow.glowOn = false;
		    gv.cc.ctrlDownArrow.glowOn = false;
		    gv.cc.ctrlLeftArrow.glowOn = false;
		    gv.cc.ctrlRightArrow.glowOn = false;
		    btnParty.glowOn = false;
		    gv.cc.btnInventory.glowOn = false;
		    btnJournal.glowOn = false;
		    btnSettings.glowOn = false;
		    btnSave.glowOn = false;
		    btnCastOnMainMap.glowOn = false;
		    btnWait.glowOn = false;
		
		    //TODOgv.cc.onTouchLog();
		    //int eventAction = event.getAction();
		    switch (eventType)
		    {
		    case MouseEventType.EventType.MouseDown:
		    case MouseEventType.EventType.MouseMove:
			    int x = (int) e.X;
			    int y = (int) e.Y;

			    //Draw Floaty Text On Mouse Over Prop
			    int gridx = (int) e.X / gv.squareSize;
			    int gridy = (int) e.Y / gv.squareSize;
			    int actualX = mod.PlayerLocationX + (gridx - gv.playerOffset);
			    int actualY = mod.PlayerLocationY + (gridy - gv.playerOffset);
			    gv.cc.floatyText = "";
			    if (gridy < 7)
			    {
				    foreach (Prop p in mod.currentArea.Props)
				    {
					    if ((p.LocationX == actualX) && (p.LocationY == actualY))
					    {
						    if (!p.MouseOverText.Equals("none"))
						    {
							    gv.cc.floatyText = p.MouseOverText;					
							    gv.cc.floatyTextLoc = new Coordinate(gridx * gv.squareSize, gridy * gv.squareSize);
						    }					
					    }
				    }	
			    }
			
			
			    if (gv.cc.ctrlUpArrow.getImpact(x, y))
			    {
				    gv.cc.ctrlUpArrow.glowOn = true;
			    }
			    else if (gv.cc.ctrlDownArrow.getImpact(x, y))
			    {
				    gv.cc.ctrlDownArrow.glowOn = true;
			    }
			    else if (gv.cc.ctrlLeftArrow.getImpact(x, y))
			    {
				    gv.cc.ctrlLeftArrow.glowOn = true;
			    }
			    else if (gv.cc.ctrlRightArrow.getImpact(x, y))
			    {
				    gv.cc.ctrlRightArrow.glowOn = true;
			    }	
			    else if (btnParty.getImpact(x, y))
			    {
				    btnParty.glowOn = true;
			    }
			    else if (gv.cc.btnInventory.getImpact(x, y))
			    {
				    gv.cc.btnInventory.glowOn = true;
			    }
			    else if (btnJournal.getImpact(x, y))
			    {
				    btnJournal.glowOn = true;
			    }
			    else if (btnSettings.getImpact(x, y))
			    {
				    btnSettings.glowOn = true;
			    }	
			    else if (btnSave.getImpact(x, y))
			    {
                    if (mod.allowSave)
                    {
                        btnSave.glowOn = true;
                    }
			    }
			    else if (btnCastOnMainMap.getImpact(x, y))
			    {
				    btnCastOnMainMap.glowOn = true;							
			    }
			    else if (btnWait.getImpact(x, y))
			    {
				    btnWait.glowOn = true;							
			    }
			    break;
			
		    case MouseEventType.EventType.MouseUp:
			    x = (int) e.X;
			    y = (int) e.Y;
			    int gridX = (int) e.X / gv.squareSize;
			    int gridY = (int) e.Y / gv.squareSize;
			    int actualx = mod.PlayerLocationX + (gridX - gv.playerOffset);
			    int actualy = mod.PlayerLocationY + (gridY - gv.playerOffset);		
			
			
			    gv.cc.ctrlUpArrow.glowOn = false;
			    gv.cc.ctrlDownArrow.glowOn = false;
			    gv.cc.ctrlLeftArrow.glowOn = false;
			    gv.cc.ctrlRightArrow.glowOn = false;
			    btnParty.glowOn = false;
			    gv.cc.btnInventory.glowOn = false;
			    btnJournal.glowOn = false;
			    btnSettings.glowOn = false;
			    btnSave.glowOn = false;
			    btnCastOnMainMap.glowOn = false;
			    btnWait.glowOn = false;
			
			    if (tglGrid.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (tglGrid.toggleOn)
				    {
					    tglGrid.toggleOn = false;
					    mod.map_showGrid = false;					
				    }
				    else
				    {
					    tglGrid.toggleOn = true;
					    mod.map_showGrid = true;
				    }
			    }

                if (tglInteractionState.getImpact(x, y))
                {
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    if (tglInteractionState.toggleOn)
                    {
                        tglInteractionState.toggleOn = false;
                        mod.showInteractionState = false;
                        gv.cc.addLogText("lime", "No info about interaction state of NPC and creatures (encounter = red, mandatory conversation = orange and optional conversation = green");
                    }
                    else
                    {
                        tglInteractionState.toggleOn = true;
                        mod.showInteractionState = true;
                        gv.cc.addLogText("lime", "Show info about interaction state of NPC and creatures (encounter = red, mandatory conversation = orange and optional conversation = green");
                    }
                }

                if (tglAvoidConversation.getImpact(x, y))
                {
                    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    if (tglAvoidConversation.toggleOn)
                    {
                        tglAvoidConversation.toggleOn = false;
                        mod.avoidInteraction = false;
                        gv.cc.addLogText("lime", "Normal move mode: party does all possible conversations");
                    }
                    else
                    {
                        tglAvoidConversation.toggleOn = true;
                        mod.avoidInteraction = true;
                        gv.cc.addLogText("lime", "In a hurry: Party is avoiding all conversations that are not mandatory");
                    }
                }


			    if (tglClock.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (tglClock.toggleOn)
				    {
					    tglClock.toggleOn = false;					
				    }
				    else
				    {
					    tglClock.toggleOn = true;
				    }
			    }
			    if (gv.cc.tglSound.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (gv.cc.tglSound.toggleOn)
				    {
					    gv.cc.tglSound.toggleOn = false;
					    mod.playMusic = false;
                        mod.playSoundFx = false;
                        gv.screenCombat.tglSoundFx.toggleOn = false;
					    gv.stopMusic();
					    gv.stopAmbient();
					    gv.cc.addLogText("lime","Music Off, SoundFX Off");
				    }
				    else
				    {
					    gv.cc.tglSound.toggleOn = true;
					    mod.playMusic = true;
                        mod.playSoundFx = true;
                        gv.screenCombat.tglSoundFx.toggleOn = true;
					    gv.startMusic();
					    gv.startAmbient();
					    gv.cc.addLogText("lime","Music On, SoundFX On");
				    }
			    }
			    if (tglFullParty.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (tglFullParty.toggleOn)
				    {
					    tglFullParty.toggleOn = false;
					    gv.cc.addLogText("lime","Show Party Leader");
				    }
				    else
				    {
					    tglFullParty.toggleOn = true;
					    gv.cc.addLogText("lime","Show Full Party");
				    }
			    }
			    if ((tglMiniMap.getImpact(x, y)) && (mod.currentArea.IsWorldMap))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (tglMiniMap.toggleOn)
				    {
					    tglMiniMap.toggleOn = false;
					    gv.cc.addLogText("lime","Hide Mini Map");
				    }
				    else
				    {
					    tglMiniMap.toggleOn = true;
					    gv.cc.addLogText("lime","Show Mini Map");
				    }
			    }
			    if ((gv.cc.ctrlUpArrow.getImpact(x, y)) || ((mod.PlayerLocationX == actualx) && ((mod.PlayerLocationY - 1) == actualy)))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (mod.PlayerLocationY > 0)
				    {
					    if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY - 1) == false)
					    {
						    mod.PlayerLastLocationX = mod.PlayerLocationX;
						    mod.PlayerLastLocationY = mod.PlayerLocationY;
						    mod.PlayerLocationY--;
						    gv.cc.doUpdate();
					    }
				    }
			    }
			    else if ((gv.cc.ctrlDownArrow.getImpact(x, y)) || ((mod.PlayerLocationX == actualx) && ((mod.PlayerLocationY + 1) == actualy)))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    int mapheight = mod.currentArea.MapSizeY;
				    if (mod.PlayerLocationY < (mapheight - 1))
				    {
					    if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY + 1) == false)
					    {
						    mod.PlayerLastLocationX = mod.PlayerLocationX;
						    mod.PlayerLastLocationY = mod.PlayerLocationY;
						    mod.PlayerLocationY++;	
						    gv.cc.doUpdate();
					    }
				    }
			    }
			    else if ((gv.cc.ctrlLeftArrow.getImpact(x, y)) || (((mod.PlayerLocationX - 1) == actualx) && (mod.PlayerLocationY == actualy)))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    if (mod.PlayerLocationX > 0)
				    {
					    if (mod.currentArea.GetBlocked(mod.PlayerLocationX - 1, mod.PlayerLocationY) == false)
					    {
						    mod.PlayerLastLocationX = mod.PlayerLocationX;
						    mod.PlayerLastLocationY = mod.PlayerLocationY;
						    mod.PlayerLocationX--;
						    if (!mod.playerList[0].combatFacingLeft)
						    {
//TODO							    //mod.partyTokenBitmap = gv.cc.flip(mod.partyTokenBitmap);
						    }
						    foreach (Player pc in mod.playerList)
						    {
							    if (!pc.combatFacingLeft)
							    {
//TODO								    //pc.token = gv.cc.flip(pc.token);
								    pc.combatFacingLeft = true;
							    }
						    }
						    gv.cc.doUpdate();
					    }
				    }
			    }
			    else if ((gv.cc.ctrlRightArrow.getImpact(x, y)) || (((mod.PlayerLocationX + 1) == actualx) && (mod.PlayerLocationY == actualy)))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    int mapwidth = mod.currentArea.MapSizeX;
				    if (mod.PlayerLocationX < (mapwidth - 1))
				    {
					    if (mod.currentArea.GetBlocked(mod.PlayerLocationX + 1, mod.PlayerLocationY) == false)
					    {
						    mod.PlayerLastLocationX = mod.PlayerLocationX;
						    mod.PlayerLastLocationY = mod.PlayerLocationY;
						    mod.PlayerLocationX++;
						    if (mod.playerList[0].combatFacingLeft)
						    {
//TODO							    mod.partyTokenBitmap = gv.cc.flip(mod.partyTokenBitmap);
						    }
						    foreach (Player pc in mod.playerList)
						    {
							    if (pc.combatFacingLeft)
							    {
//TODO								    pc.token = gv.cc.flip(pc.token);
								    pc.combatFacingLeft = false;
							    }
						    }						
						    gv.cc.doUpdate();
					    }
				    }
			    }	
			    else if (btnParty.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    /*int cntPCs = 0;
				    foreach (IbbButton btn in gv.screenParty.btnPartyIndex)
				    {
					    if (cntPCs < mod.playerList.Count)
					    {
						    btn.Img2 = gv.cc.LoadBitmap(mod.playerList[cntPCs].tokenFilename);						
					    }
					    cntPCs++;
				    }*/
                    gv.screenParty.resetPartyScreen();
				    gv.screenType = "party";
				    gv.cc.tutorialMessageParty(false);
			    }
                else if ((gv.cc.ptrPc0.getImpact(x, y)) && (mod.playerList.Count > 0))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        mod.selectedPartyLeader = 0;
                        gv.cc.partyScreenPcIndex = 0;
                        gv.screenParty.resetPartyScreen();
                        gv.screenType = "party";
                        gv.cc.tutorialMessageParty(false);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        mod.selectedPartyLeader = 0;
                        gv.cc.partyScreenPcIndex = 0;
                    }
                }
                else if ((gv.cc.ptrPc1.getImpact(x, y)) && (mod.playerList.Count > 1))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        mod.selectedPartyLeader = 1;
                        gv.cc.partyScreenPcIndex = 1;
                        gv.screenParty.resetPartyScreen();
                        gv.screenType = "party";
                        gv.cc.tutorialMessageParty(false);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        mod.selectedPartyLeader = 1;
                        gv.cc.partyScreenPcIndex = 1;
                    }
                }
                else if ((gv.cc.ptrPc2.getImpact(x, y)) && (mod.playerList.Count > 2))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        mod.selectedPartyLeader = 2;
                        gv.cc.partyScreenPcIndex = 2;
                        gv.screenParty.resetPartyScreen();
                        gv.screenType = "party";
                        gv.cc.tutorialMessageParty(false);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        mod.selectedPartyLeader = 2;
                        gv.cc.partyScreenPcIndex = 2;
                    }
                }
                else if ((gv.cc.ptrPc3.getImpact(x, y)) && (mod.playerList.Count > 3))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        mod.selectedPartyLeader = 3;
                        gv.cc.partyScreenPcIndex = 3;
                        gv.screenParty.resetPartyScreen();
                        gv.screenType = "party";
                        gv.cc.tutorialMessageParty(false);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        mod.selectedPartyLeader = 3;
                        gv.cc.partyScreenPcIndex = 3;
                    }
                }
                else if ((gv.cc.ptrPc4.getImpact(x, y)) && (mod.playerList.Count > 4))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        mod.selectedPartyLeader = 4;
                        gv.cc.partyScreenPcIndex = 4;
                        gv.screenParty.resetPartyScreen();
                        gv.screenType = "party";
                        gv.cc.tutorialMessageParty(false);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        mod.selectedPartyLeader = 4;
                        gv.cc.partyScreenPcIndex = 4;
                    }
                }
                else if ((gv.cc.ptrPc5.getImpact(x, y)) && (mod.playerList.Count > 5))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        mod.selectedPartyLeader = 5;
                        gv.cc.partyScreenPcIndex = 5;
                        gv.screenParty.resetPartyScreen();
                        gv.screenType = "party";
                        gv.cc.tutorialMessageParty(false);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        mod.selectedPartyLeader = 5;
                        gv.cc.partyScreenPcIndex = 5;
                    }
                }
                else if (gv.cc.btnInventory.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.screenType = "inventory";
				    gv.cc.tutorialMessageInventory(false);
			    }
			    else if (btnJournal.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.screenType = "journal";
			    }
			    else if (btnSettings.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.cc.doSettingsDialogs();
			    }	
			    else if (btnSave.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                    if (mod.allowSave)
                    {
                        gv.cc.doSavesDialog();
                    }
			    }
			    else if (btnWait.getImpact(x, y))
			    {
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				    gv.cc.doUpdate();
			    }
			    else if (btnCastOnMainMap.getImpact(x, y))
			    {
                    
				    //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
				    //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
				
				    List<string> pcNames = new List<string>();
				    List<int> pcIndex = new List<int>();
                    pcNames.Add("cancel");

                    int cnt = 0;
                    foreach (Player p in mod.playerList)
                    {
                	    if (hasMainMapTypeSpell(p))
                	    {
                		    pcNames.Add(p.name);
                		    pcIndex.Add(cnt);
                	    }
                	    cnt++;
                    }
                
                    //If only one PC, do not show select PC dialog...just go to cast selector
                    if (pcIndex.Count == 1)
            	    {
        			    try
                        {
        				    gv.screenCastSelector.castingPlayerIndex = pcIndex[0];
                            gv.screenCombat.spellSelectorIndex = 0;
        				    gv.screenType = "mainMapCast";
        				    return;
                        }
                        catch (Exception ex)
                        {
                            //print error
                            IBMessageBox.Show(gv, "error with Pc Selector screen: " + ex.ToString());
                    	    return;
                        }        	                            	        	                        
            	    }

                    using (ItemListSelector pcSel = new ItemListSelector(gv, pcNames, "Select Caster"))
                    {                        
                        pcSel.ShowDialog();

                        if (pcSel.selectedIndex > 0)
                        {
                            try
                            {
                                gv.screenCastSelector.castingPlayerIndex = pcIndex[pcSel.selectedIndex - 1]; // pcIndex.get(item - 1);
                                gv.screenCombat.spellSelectorIndex = 0;
                                gv.screenType = "mainMapCast";
                            }
                            catch (Exception ex)
                            {
                                IBMessageBox.Show(gv, "error with Pc Selector screen: " + ex.ToString());
                                //print error
                            }
                        }
                        else if (pcSel.selectedIndex == 0) // selected "cancel"
                        {
                            //do nothing
                        }
                    }                    				
			    }
			    break;	
		    }
	    }
        public void onKeyUp(Keys keyData)
        {
            if ((moveDelay()) && (finishedMove))
            {
                //game.lastPlayerLocation = new Point(game.playerPosition.X, game.playerPosition.Y);
                //int mapwidth = game.currentArea.MapSizeInSquares.Width;
                //int mapheight = game.currentArea.MapSizeInSquares.Height;
                if (keyData == Keys.Left | keyData == Keys.D4 | keyData == Keys.NumPad4)
                {
                    moveLeft();
                    //return true; //for the active control to see the keypress, return false 
                }
                else if (keyData == Keys.Right | keyData == Keys.D6 | keyData == Keys.NumPad6)
                {
                    moveRight();
                    //return true; //for the active control to see the keypress, return false 
                }
                else if (keyData == Keys.Up | keyData == Keys.D8 | keyData == Keys.NumPad8)
                {
                    moveUp();
                    //return true; //for the active control to see the keypress, return false 
                }
                else if (keyData == Keys.Down | keyData == Keys.D2 | keyData == Keys.NumPad2)
                {
                    moveDown();
                    //return true; //for the active control to see the keypress, return false 
                }
                else { }
            }
            if (keyData == Keys.Q)
            {
                if (mod.allowSave)
                {
                    gv.cc.QuickSave();
                    gv.cc.addLogText("lime", "Quicksave Completed");
                }
                else
                {
                    gv.cc.addLogText("red", "No save allowed at this time.");
                }
            }
            if (keyData == Keys.D)
            {
                if (gv.mod.debugMode)
                {
                    gv.mod.debugMode = false;
                    gv.cc.addLogText("lime", "DebugMode Turned Off");
                }
                else
                {
                    gv.mod.debugMode = true;
                    gv.cc.addLogText("lime", "DebugMode Turned On");
                }
            }
            if (keyData == Keys.I)
            {
                gv.screenType = "inventory";
                gv.cc.tutorialMessageInventory(false);
            }
            if (keyData == Keys.J)
            {
                gv.screenType = "journal";
            }
            if (keyData == Keys.P)
            {
                /*int cntPCs = 0;
                foreach (IbbButton btn in gv.screenParty.btnPartyIndex)
                {
                    if (cntPCs < mod.playerList.Count)
                    {
                        btn.Img2 = gv.cc.LoadBitmap(mod.playerList[cntPCs].tokenFilename);
                    }
                    cntPCs++;
                }*/
                gv.screenParty.resetPartyScreen();
                gv.screenType = "party";
                gv.cc.tutorialMessageParty(false);
            }
            if (keyData == Keys.C)
            {
                List<string> pcNames = new List<string>();
                List<int> pcIndex = new List<int>();
                pcNames.Add("cancel");

                int cnt = 0;
                foreach (Player p in mod.playerList)
                {
                    if (hasMainMapTypeSpell(p))
                    {
                        pcNames.Add(p.name);
                        pcIndex.Add(cnt);
                    }
                    cnt++;
                }

                //If only one PC, do not show select PC dialog...just go to cast selector
                if (pcIndex.Count == 1)
                {
                    try
                    {
                        gv.screenCastSelector.castingPlayerIndex = pcIndex[0];
                        gv.screenCombat.spellSelectorIndex = 0;
                        gv.screenType = "mainMapCast";
                        return;
                    }
                    catch (Exception ex)
                    {
                        //print error
                        IBMessageBox.Show(gv, "error with Pc Selector screen: " + ex.ToString());
                        return;
                    }
                }

                using (ItemListSelector pcSel = new ItemListSelector(gv, pcNames, "Select Caster"))
                {
                    pcSel.ShowDialog();

                    if (pcSel.selectedIndex > 0)
                    {
                        try
                        {
                            gv.screenCastSelector.castingPlayerIndex = pcIndex[pcSel.selectedIndex - 1]; // pcIndex.get(item - 1);
                            gv.screenCombat.spellSelectorIndex = 0;
                            gv.screenType = "mainMapCast";
                        }
                        catch (Exception ex)
                        {
                            IBMessageBox.Show(gv, "error with Pc Selector screen: " + ex.ToString());
                            //print error
                        }
                    }
                    else if (pcSel.selectedIndex == 0) // selected "cancel"
                    {
                        //do nothing
                    }
                } 
            }
        }
        private bool moveDelay()
        {
            long elapsed = DateTime.Now.Ticks - timeStamp;
            if (elapsed > 10000 * movementDelayInMiliseconds) //10,000 ticks in 1 ms
            {
                timeStamp = DateTime.Now.Ticks;
                return true;
            }
            return false;
        }
        private void moveLeft()
        {
            if (mod.PlayerLocationX > 0)
            {
                if (mod.currentArea.GetBlocked(mod.PlayerLocationX - 1, mod.PlayerLocationY) == false)
                {
                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                    mod.PlayerLocationX--;
                    if (!mod.playerList[0].combatFacingLeft)
                    {
//TODO                        mod.partyTokenBitmap = gv.cc.flip(mod.partyTokenBitmap);
                    }
                    foreach (Player pc in mod.playerList)
                    {
                        if (!pc.combatFacingLeft)
                        {
//TODO                            pc.token = gv.cc.flip(pc.token);
                            pc.combatFacingLeft = true;
                        }
                    }
                    gv.cc.doUpdate();
                }
            }
        }
        private void moveRight()
        {
            int mapwidth = mod.currentArea.MapSizeX;
            if (mod.PlayerLocationX < (mapwidth - 1))
            {
                if (mod.currentArea.GetBlocked(mod.PlayerLocationX + 1, mod.PlayerLocationY) == false)
                {
                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                    mod.PlayerLocationX++;
                    if (mod.playerList[0].combatFacingLeft)
                    {
//TODO                        mod.partyTokenBitmap = gv.cc.flip(mod.partyTokenBitmap);
                    }
                    foreach (Player pc in mod.playerList)
                    {
                        if (pc.combatFacingLeft)
                        {
//TODO                            pc.token = gv.cc.flip(pc.token);
                            pc.combatFacingLeft = false;
                        }
                    }
                    gv.cc.doUpdate();
                }
            }
        }
        private void moveUp()
        {
            if (mod.PlayerLocationY > 0)
            {
                if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY - 1) == false)
                {
                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                    mod.PlayerLocationY--;
                    gv.cc.doUpdate();
                }
            }
        }
        private void moveDown()
        {
            int mapheight = mod.currentArea.MapSizeY;
            if (mod.PlayerLocationY < (mapheight - 1))
            {
                if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY + 1) == false)
                {
                    mod.PlayerLastLocationX = mod.PlayerLocationX;
                    mod.PlayerLastLocationY = mod.PlayerLocationY;
                    mod.PlayerLocationY++;
                    gv.cc.doUpdate();
                }
            }
        }
	    public List<string> wrapList(string str, int wrapLength) 
	    {
		     if (str == null) 
		     {
		         return null;
		     }
		     if (wrapLength < 1) 
		     {
		         wrapLength = 1;
		     }
		     int inputLineLength = str.Length;
		     int offset = 0;
		     List<string> returnList = new List<string>();
		 
		     while ((inputLineLength - offset) > wrapLength) 
		     {
		         if (str.ElementAt(offset) == ' ') 
		         {
		             offset++;
		             continue;
		         }
		     
		         int spaceToWrapAt = str.LastIndexOf(' ', wrapLength + offset);
		 
                 if (spaceToWrapAt >= offset) 
                 {
		             // normal case
            	     returnList.Add(str.Substring(offset, spaceToWrapAt));
		             offset = spaceToWrapAt + 1;
		         } 
		         else 
		         {
		    	     // do not wrap really long word, just extend beyond limit
		    	     spaceToWrapAt = str.IndexOf(' ', wrapLength + offset);
		             if (spaceToWrapAt >= 0) 
		             {
		        	     returnList.Add(str.Substring(offset, spaceToWrapAt));
		                 offset = spaceToWrapAt + 1;
		             } 
		             else 
		             {
		        	     returnList.Add(str.Substring(offset));
		                 offset = inputLineLength;
		             }
		         }
		     }
		 
             // Whatever is left in line is short enough to just pass through
		     returnList.Add(str.Substring(offset));
		     return returnList;
	    }
	    private void setExplored()
        {
    	    int minX = mod.PlayerLocationX - 1;
		    if (minX < 0) { minX = 0; }
		    int minY = mod.PlayerLocationY - 1;
		    if (minY < 0) { minY = 0; }
		
		    int maxX = mod.PlayerLocationX + 1;
		    if (maxX > this.mod.currentArea.MapSizeX - 1) { maxX = this.mod.currentArea.MapSizeX - 1; }
		    int maxY = mod.PlayerLocationY + 1;
		    if (maxY > this.mod.currentArea.MapSizeY - 1) { maxY = this.mod.currentArea.MapSizeY - 1; }
		
		    for (int xx = minX; xx <= maxX; xx++)
            {
			    for (int yy = minY; yy <= maxY; yy++)
                {
            	    mod.currentArea.Tiles[yy * mod.currentArea.MapSizeX + xx].Visible = true;                
                }
            }
				
		    //check left
    	    int x = mod.PlayerLocationX - 1;
    	    int y = mod.PlayerLocationY;
    	    if ((x - 1 >= 0) && (!mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].LoSBlocked))
		    {
    		    if (y > 0)
    		    {
    			    mod.currentArea.Tiles[(y - 1) * mod.currentArea.MapSizeX + (x - 1)].Visible = true;
    		    }
    		    mod.currentArea.Tiles[(y + 0) * mod.currentArea.MapSizeX + (x - 1)].Visible = true;
    		    if (y < this.mod.currentArea.MapSizeY - 1)
    		    {
    			    mod.currentArea.Tiles[(y + 1) * mod.currentArea.MapSizeX + (x - 1)].Visible = true;
    		    }
		    }
    	    //check right
    	    x = mod.PlayerLocationX + 1;
    	    y = mod.PlayerLocationY;
    	    if ((x + 1 < this.mod.currentArea.MapSizeX) && (!mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].LoSBlocked))
		    {
    		    if (y > 0)
    		    {
    			    mod.currentArea.Tiles[(y - 1) * mod.currentArea.MapSizeX + (x + 1)].Visible = true;
    		    }
    		    mod.currentArea.Tiles[(y + 0) * mod.currentArea.MapSizeX + (x + 1)].Visible = true;
    		    if (y < this.mod.currentArea.MapSizeY - 1)
    		    {
    			    mod.currentArea.Tiles[(y + 1) * mod.currentArea.MapSizeX + (x + 1)].Visible = true;
    		    }
		    }
    	    //check up
    	    x = mod.PlayerLocationX;
    	    y = mod.PlayerLocationY - 1;
    	    if ((y - 1 >= 0) && (!mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].LoSBlocked))
		    {
    		    if (x < this.mod.currentArea.MapSizeX - 1)
    		    {
    			    mod.currentArea.Tiles[(y - 1) * mod.currentArea.MapSizeX + (x + 1)].Visible = true;
    		    }
    		    mod.currentArea.Tiles[(y - 1) * mod.currentArea.MapSizeX + (x + 0)].Visible = true;
    		    if (x > 0)
    		    {
    		        mod.currentArea.Tiles[(y - 1) * mod.currentArea.MapSizeX + (x - 1)].Visible = true;
    		    }
		    }
    	    //check down
    	    x = mod.PlayerLocationX;
    	    y = mod.PlayerLocationY + 1;
    	    if ((y + 1 < this.mod.currentArea.MapSizeY) && (!mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].LoSBlocked))
		    {
    		    if (x < this.mod.currentArea.MapSizeX - 1)
    		    {
    			    mod.currentArea.Tiles[(y + 1) * mod.currentArea.MapSizeX + (x + 1)].Visible = true;
    		    }
    		    mod.currentArea.Tiles[(y + 1) * mod.currentArea.MapSizeX + (x + 0)].Visible = true;
    		    if (x > 0)
    		    {
    			    mod.currentArea.Tiles[(y + 1) * mod.currentArea.MapSizeX + (x - 1)].Visible = true;
    		    }
		    }
        }
        public bool IsLineOfSightForEachCorner(Coordinate s, Coordinate e)
        {
            int spacer = 5;
            Coordinate start = new Coordinate((s.X * gv.squareSize) + (gv.squareSize / 2), (s.Y * gv.squareSize) + (gv.squareSize / 2));
            // top left
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize - spacer, e.Y * gv.squareSize - spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + spacer, e.Y * gv.squareSize - spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize - spacer, e.Y * gv.squareSize + spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + spacer, e.Y * gv.squareSize + spacer), e)) { return true; } 
            // top right
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + gv.squareSize - spacer, e.Y * gv.squareSize - spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + gv.squareSize + spacer, e.Y * gv.squareSize - spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + gv.squareSize - spacer, e.Y * gv.squareSize + spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + gv.squareSize + spacer, e.Y * gv.squareSize + spacer), e)) { return true; }
            // bottom left
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize - spacer, e.Y * gv.squareSize + gv.squareSize - spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + spacer, e.Y * gv.squareSize + gv.squareSize - spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize - spacer, e.Y * gv.squareSize + gv.squareSize + spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + spacer, e.Y * gv.squareSize + gv.squareSize + spacer), e)) { return true; }
            // bottom right
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + gv.squareSize - spacer, e.Y * gv.squareSize + gv.squareSize - spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + gv.squareSize + spacer, e.Y * gv.squareSize + gv.squareSize - spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + gv.squareSize - spacer, e.Y * gv.squareSize + gv.squareSize + spacer), e)) { return true; }
            if (IsVisibleLineOfSight(start, new Coordinate(e.X * gv.squareSize + gv.squareSize + spacer, e.Y * gv.squareSize + gv.squareSize + spacer), e)) { return true; }
                   
            return false;
        }
        public bool IsVisibleLineOfSight(Coordinate s, Coordinate e, Coordinate endSquare)
        {
            // Bresenham Line algorithm
            // Creates a line from Begin to End starting at (x0,y0) and ending at (x1,y1)
            // where x0 less than x1 and y0 less than y1
            // AND line is less steep than it is wide (dx less than dy)    
            //Point start = new Point((s.X * _squareSize) + (_squareSize / 2), (s.Y * _squareSize) + (_squareSize / 2));
            //Point end = new Point((e.X * _squareSize) + (_squareSize / 2), (e.Y * _squareSize) + (_squareSize / 2));
    	    Coordinate start = s;
    	    Coordinate end = e;
            int deltax = Math.Abs(end.X - start.X);
            int deltay = Math.Abs(end.Y - start.Y);
            int ystep = 10;
            int xstep = 10;
            int gridx = 0;
            int gridy = 0;
            int gridXdelayed = s.X;
            int gridYdelayed = s.Y;

            //#region low angle version
            if (deltax > deltay) //Low Angle line
            {
        	    Coordinate nextPoint = start;
                int error = deltax / 2;

                if (end.Y < start.Y) { ystep = -1 * ystep; } //down and right or left

                if (end.X > start.X) //down and right
                {
                    for (int x = start.X; x <= end.X; x += xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        gridx = nextPoint.X / gv.squareSize;
                        gridy = nextPoint.Y / gv.squareSize;
                    
                        if ((mod.currentArea.Tiles[gridy * mod.currentArea.MapSizeX + gridx].LoSBlocked) || (new Coordinate(gridXdelayed, gridYdelayed) == endSquare))
                        {
                            return false;
                        }
                        gridXdelayed = gridx;
                        gridYdelayed = gridy;
                    }
                }
                else //down and left
                {
                    for (int x = start.X; x >= end.X; x -= xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        gridx = nextPoint.X / gv.squareSize;
                        gridy = nextPoint.Y / gv.squareSize;
                        if ((mod.currentArea.Tiles[gridy * mod.currentArea.MapSizeX + gridx].LoSBlocked) || (new Coordinate(gridXdelayed, gridYdelayed) == endSquare))
                        {
                            return false;
                        }
                        gridXdelayed = gridx;
                        gridYdelayed = gridy;
                    }
                }
            }
            //#endregion

            //#region steep version
            else //Low Angle line
            {
        	    Coordinate nextPoint = start;
                int error = deltay / 2;

                if (end.X < start.X) { xstep = -1 * xstep; } //up and right or left

                if (end.Y > start.Y) //up and right
                {
                    for (int y = start.Y; y <= end.Y; y += ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        gridx = nextPoint.X / gv.squareSize;
                        gridy = nextPoint.Y / gv.squareSize;
                        if ((mod.currentArea.Tiles[gridy * mod.currentArea.MapSizeX + gridx].LoSBlocked) || (new Coordinate(gridXdelayed, gridYdelayed) == endSquare))
                        {
                            return false;
                        }
                        gridXdelayed = gridx;
                        gridYdelayed = gridy;
                    }
                }
                else //up and right
                {
                    for (int y = start.Y; y >= end.Y; y -= ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        gridx = nextPoint.X / gv.squareSize;
                        gridy = nextPoint.Y / gv.squareSize;
                        if ((mod.currentArea.Tiles[gridy * mod.currentArea.MapSizeX + gridx].LoSBlocked) || (new Coordinate(gridXdelayed, gridYdelayed) == endSquare))
                        {
                            return false;
                        }
                        gridXdelayed = gridx;
                        gridYdelayed = gridy;
                    }
                }
            }
            //#endregion

            return true;
        }
        public bool hasMainMapTypeSpell(Player pc)
	    {
		    foreach (string s in pc.knownSpellsTags)
            {
			    Spell sp = mod.getSpellByTag(s);
			    if ((sp.useableInSituation.Equals("Always")) || (sp.useableInSituation.Equals("OutOfCombat")))
			    {
                    return true;
                }
            }
            return false;
	    }
        public void checkLevelUpAvailable()
	    {
		    bool levelup = false;
		    foreach (Player pc in mod.playerList)
		    {
			    if (pc.IsReadyToAdvanceLevel())
			    {
				    levelup = true;
			    }
		    }
		    if (levelup)
		    {
			    btnParty.Img2 = gv.cc.LoadBitmap("btnpartyplus");
		    }
		    else
		    {
			    btnParty.Img2 = gv.cc.LoadBitmap("btnparty");
		    }
	    }
    }
}
