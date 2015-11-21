﻿using System;
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
        public List<FloatyTextByPixel> floatyTextByPixelPool = new List<FloatyTextByPixel>();
        public int mapStartLocXinPixels;
        public int movementDelayInMiliseconds = 100;
        private long timeStamp = 0;
        private bool finishedMove = true;
        public Bitmap minimap = null;
        public Bitmap fullScreenEffect1 = null;
        public Bitmap fullScreenEffect2 = null;
        public Bitmap fullScreenEffect3 = null;

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
            int padW = gv.squareSize / 6;
            int hotkeyShift = 0;
            if (gv.useLargeLayout)
            {
                hotkeyShift = 1;
            }


            if (btnWait == null)
            {
                btnWait = new IbbButton(gv, 0.8f);
                btnWait.Text = "WAIT";
                btnWait.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnWait.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                //btnWait.X = 17 * gv.squareSize - 3*gv.oXshift;
                //btnWait.Y = 8 * gv.squareSize + pH * 2;
                btnWait.X = gv.cc.pnlArrows.LocX + 1 * gv.squareSize + gv.squareSize / 2;
                btnWait.Y = gv.cc.pnlArrows.LocY + 1 * gv.squareSize + gv.pS;
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
                //btnParty.X = 7 * gv.squareSize + padW * 0 + gv.oXshift;
                //btnParty.Y = 9 * gv.squareSize + +(int)(1.75 * pH);
                btnParty.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 0) * gv.squareSize;
                btnParty.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
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
                //btnJournal.X = 9 * gv.squareSize + padW * 0 + gv.oXshift;
                //btnJournal.Y = 9 * gv.squareSize + +(int)(1.75 * pH);
                btnJournal.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 2) * gv.squareSize;
                btnJournal.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
                btnJournal.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnJournal.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnSettings == null)
            {
                btnSettings = new IbbButton(gv, 1.0f);
                btnSettings.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnSettings.Img2 = gv.cc.LoadBitmap("btnsettings"); // BitmapFactory.decodeResource(getResources(), R.drawable.btnsettings);
                btnSettings.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                //btnSettings.X = 10 * gv.squareSize + padW * 0 + gv.oXshift;
                //btnSettings.Y = 9 * gv.squareSize + +(int)(1.75 * pH);
                btnSettings.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 3) * gv.squareSize;
                btnSettings.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
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
                //btnCastOnMainMap.X = 11 * gv.squareSize + padW * 0 + gv.oXshift;
                //btnCastOnMainMap.Y = 9 * gv.squareSize + +(int)(1.75 * pH);
                btnCastOnMainMap.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 4) * gv.squareSize;
                btnCastOnMainMap.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
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
                //btnSave.X = 12 * gv.squareSize + padW * 0 + gv.oXshift;
                //btnSave.Y = 9 * gv.squareSize + +(int)(1.75 * pH);
                btnSave.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 5) * gv.squareSize;
                btnSave.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
                btnSave.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSave.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
        }
        public void setToggleButtonsStart()
        {
            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padW = gv.squareSize / 6;

            if (tglFullParty == null)
            {
                tglFullParty = new IbbToggleButton(gv);
                tglFullParty.ImgOn = gv.cc.LoadBitmap("tgl_fullparty_on");
                tglFullParty.ImgOff = gv.cc.LoadBitmap("tgl_fullparty_off");
                //tglFullParty.X = 0 * gv.squareSize + gv.oXshift + (gv.squareSize / 2);
                //tglFullParty.Y = 9 * (gv.squareSize) + (gv.squareSize / 2);
                tglFullParty.X = gv.cc.pnlToggles.LocX + 1 * gv.squareSize + gv.squareSize / 4;
                tglFullParty.Y = gv.cc.pnlToggles.LocY + 0 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglFullParty.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglFullParty.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
                tglFullParty.toggleOn = true;
            }
            if (tglMiniMap == null)
            {
                tglMiniMap = new IbbToggleButton(gv);
                tglMiniMap.ImgOn = gv.cc.LoadBitmap("tgl_minimap_on");
                tglMiniMap.ImgOff = gv.cc.LoadBitmap("tgl_minimap_off");
                //tglMiniMap.X = 4 * gv.squareSize + gv.oXshift + (gv.squareSize / 2);
                //tglMiniMap.Y = 9 * (gv.squareSize) + (gv.squareSize / 2);
                tglMiniMap.X = gv.cc.pnlToggles.LocX + 2 * gv.squareSize + gv.squareSize / 4;
                tglMiniMap.Y = gv.cc.pnlToggles.LocY + 2 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglMiniMap.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglMiniMap.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
                tglMiniMap.toggleOn = false;
            }
            if (tglGrid == null)
            {
                tglGrid = new IbbToggleButton(gv);
                tglGrid.ImgOn = gv.cc.LoadBitmap("tgl_grid_on");
                tglGrid.ImgOff = gv.cc.LoadBitmap("tgl_grid_off");
                //tglGrid.X = 1 * gv.squareSize + gv.oXshift + (gv.squareSize / 2);
                //tglGrid.Y = 9 * (gv.squareSize) + (gv.squareSize / 2);
                tglGrid.X = gv.cc.pnlToggles.LocX + 1 * gv.squareSize + gv.squareSize / 4;
                tglGrid.Y = gv.cc.pnlToggles.LocY + 1 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglGrid.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglGrid.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
                tglGrid.toggleOn = true;
            }
            if (tglClock == null)
            {
                tglClock = new IbbToggleButton(gv);
                tglClock.ImgOn = gv.cc.LoadBitmap("tgl_clock_on");
                tglClock.ImgOff = gv.cc.LoadBitmap("tgl_clock_off");
                //tglClock.X = 2 * gv.squareSize + gv.oXshift + (gv.squareSize / 2);
                //tglClock.Y = 9 * (gv.squareSize) + (gv.squareSize / 2);
                tglClock.X = gv.cc.pnlToggles.LocX + 3 * gv.squareSize + gv.squareSize / 4;
                tglClock.Y = gv.cc.pnlToggles.LocY + 2 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglClock.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglClock.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
                tglClock.toggleOn = true;
            }
            if (tglInteractionState == null)
            {
                tglInteractionState = new IbbToggleButton(gv);
                tglInteractionState.ImgOn = gv.cc.LoadBitmap("tgl_state_on");
                tglInteractionState.ImgOff = gv.cc.LoadBitmap("tgl_state_off");
                //tglInteractionState.X = 1 * gv.squareSize + gv.oXshift + (gv.squareSize / 2);
                //tglInteractionState.Y = 8 * (gv.squareSize) + (gv.squareSize / 2);
                tglInteractionState.X = gv.cc.pnlToggles.LocX + 3 * gv.squareSize + gv.squareSize / 4;
                tglInteractionState.Y = gv.cc.pnlToggles.LocY + 0 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglInteractionState.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglInteractionState.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
                tglInteractionState.toggleOn = false;
            }
            if (tglAvoidConversation == null)
            {
                tglAvoidConversation = new IbbToggleButton(gv);
                tglAvoidConversation.ImgOn = gv.cc.LoadBitmap("tgl_avoidConvo_on");
                tglAvoidConversation.ImgOff = gv.cc.LoadBitmap("tgl_avoidConvo_off");
                //tglAvoidConversation.X = 0 * gv.squareSize + gv.oXshift + (gv.squareSize / 2);
                //tglAvoidConversation.Y = 8 * (gv.squareSize) + (gv.squareSize / 2);
                tglAvoidConversation.X = gv.cc.pnlToggles.LocX + 2 * gv.squareSize + gv.squareSize / 4;
                tglAvoidConversation.Y = gv.cc.pnlToggles.LocY + 0 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglAvoidConversation.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglAvoidConversation.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
                tglAvoidConversation.toggleOn = false;
            }
        }

        //MAIN SCREEN
        public void resetMiniMapBitmap()
        {            
            int minimapSquareSizeInPixels = 4 * gv.squareSize / mod.currentArea.MapSizeX;
            int drawW = minimapSquareSizeInPixels * mod.currentArea.MapSizeX;
            int drawH = minimapSquareSizeInPixels * mod.currentArea.MapSizeY;
            using (System.Drawing.Bitmap surface = new System.Drawing.Bitmap(drawW, drawH))
            {
                using (Graphics device = Graphics.FromImage(surface))
                {
                    //draw background image first
                    if ((!mod.currentArea.ImageFileName.Equals("none")) && (gv.cc.bmpMap != null))
                    {
                        System.Drawing.Bitmap bg = gv.cc.LoadBitmapGDI(mod.currentArea.ImageFileName);
                        Rectangle srcBG = new Rectangle(0, 0, bg.Width, bg.Height);
                        Rectangle dstBG = new Rectangle(mod.currentArea.backgroundImageStartLocX * minimapSquareSizeInPixels, 
                                                        mod.currentArea.backgroundImageStartLocY * minimapSquareSizeInPixels, 
                                                        minimapSquareSizeInPixels * (bg.Width / 50), 
                                                        minimapSquareSizeInPixels * (bg.Height / 50));
                        device.DrawImage(bg, dstBG, srcBG, GraphicsUnit.Pixel);
                        bg.Dispose();
                        bg = null;
                    }
                    #region Draw Layer 1
                    for (int x = 0; x < mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < mod.currentArea.MapSizeY; y++)
                        {
                            Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                            Rectangle src = new Rectangle(0, 0, gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Height);
                            float scalerX = gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Width / 100;
                            float scalerY = gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Height / 100;
                            int brX = (int)(minimapSquareSizeInPixels * scalerX);
                            int brY = (int)(minimapSquareSizeInPixels * scalerY);
                            Rectangle dst = new Rectangle(x * minimapSquareSizeInPixels, y * minimapSquareSizeInPixels, brX, brY);
                            device.DrawImage(gv.cc.tileGDIBitmapList[tile.Layer1Filename], dst, src, GraphicsUnit.Pixel);
                        }
                    }
                    #endregion
                    #region Draw Layer 2
                    for (int x = 0; x < mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < mod.currentArea.MapSizeY; y++)
                        {
                            Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                            Rectangle src = new Rectangle(0, 0, gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Height);
                            float scalerX = gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Width / 100;
                            float scalerY = gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Height / 100;
                            int brX = (int)(minimapSquareSizeInPixels * scalerX);
                            int brY = (int)(minimapSquareSizeInPixels * scalerY);
                            Rectangle dst = new Rectangle(x * minimapSquareSizeInPixels, y * minimapSquareSizeInPixels, brX, brY);
                            device.DrawImage(gv.cc.tileGDIBitmapList[tile.Layer2Filename], dst, src, GraphicsUnit.Pixel);
                        }
                    }
                    #endregion
                    #region Draw Layer 3
                    for (int x = 0; x < mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < mod.currentArea.MapSizeY; y++)
                        {
                            Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                            Rectangle src = new Rectangle(0, 0, gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Height);
                            float scalerX = gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Width / 100;
                            float scalerY = gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Height / 100;
                            int brX = (int)(minimapSquareSizeInPixels * scalerX);
                            int brY = (int)(minimapSquareSizeInPixels * scalerY);
                            Rectangle dst = new Rectangle(x * minimapSquareSizeInPixels, y * minimapSquareSizeInPixels, brX, brY);
                            device.DrawImage(gv.cc.tileGDIBitmapList[tile.Layer3Filename], dst, src, GraphicsUnit.Pixel);
                        }
                    }
                    #endregion
                    #region Draw Layer 4
                    for (int x = 0; x < mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < mod.currentArea.MapSizeY; y++)
                        {
                            Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                            Rectangle src = new Rectangle(0, 0, gv.cc.tileBitmapList[tile.Layer4Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer4Filename].PixelSize.Height);
                            float scalerX = gv.cc.tileBitmapList[tile.Layer4Filename].PixelSize.Width / 100;
                            float scalerY = gv.cc.tileBitmapList[tile.Layer4Filename].PixelSize.Height / 100;
                            int brX = (int)(minimapSquareSizeInPixels * scalerX);
                            int brY = (int)(minimapSquareSizeInPixels * scalerY);
                            Rectangle dst = new Rectangle(x * minimapSquareSizeInPixels, y * minimapSquareSizeInPixels, brX, brY);
                            device.DrawImage(gv.cc.tileGDIBitmapList[tile.Layer4Filename], dst, src, GraphicsUnit.Pixel);
                        }
                    }
                    #endregion
                    #region Draw Layer 5
                    for (int x = 0; x < mod.currentArea.MapSizeX; x++)
                    {
                        for (int y = 0; y < mod.currentArea.MapSizeY; y++)
                        {
                            Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                            Rectangle src = new Rectangle(0, 0, gv.cc.tileBitmapList[tile.Layer5Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer5Filename].PixelSize.Height);
                            float scalerX = gv.cc.tileBitmapList[tile.Layer5Filename].PixelSize.Width / 100;
                            float scalerY = gv.cc.tileBitmapList[tile.Layer5Filename].PixelSize.Height / 100;
                            int brX = (int)(minimapSquareSizeInPixels * scalerX);
                            int brY = (int)(minimapSquareSizeInPixels * scalerY);
                            Rectangle dst = new Rectangle(x * minimapSquareSizeInPixels, y * minimapSquareSizeInPixels, brX, brY);
                            device.DrawImage(gv.cc.tileGDIBitmapList[tile.Layer5Filename], dst, src, GraphicsUnit.Pixel);
                        }
                    }
                    #endregion
                    minimap = gv.cc.ConvertGDIBitmapToD2D((System.Drawing.Bitmap)surface.Clone());
                }
            }
        }
        public void redrawMain()
        {
       
            setExplored();
            if (!mod.currentArea.areaDark)
            {
                if ((!mod.currentArea.ImageFileName.Equals("none")) && (gv.cc.bmpMap != null))
                {
                    drawMap();
                }
                drawWorldMap();
                /*if (mod.currentArea.IsWorldMap)
                {
                    drawWorldMap();
                }
                else
                {
                    drawMap();
                }*/
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
                bool hideOverlayNeeded = false;
                if (mod.currentArea.UseDayNightCycle)
                {
                    drawOverlayTints();
                    hideOverlayNeeded = true;
                }

                //off for now, later :-)
                //if (mod.currentArea.useWeather)
                //{
                    //drawOverlayWeather();
                    //hideOverlayNeeded = true;
                //}

                if (hideOverlayNeeded)
                {
                    drawBlackTilesOverTints();
                    hideOverlayNeeded = false;
                }
                drawFogOfWar();
            }            
            drawFloatyTextPool();
            if (gv.mod.useSmoothMovement)
            {
                drawFloatyTextByPixelPool();
            }

            if (tglClock.toggleOn)
            {
                drawMainMapClockText();
            }
            if (mod.useUIBackground)
            {
                drawPanels();
            }
            drawFullScreenEffects();
            gv.drawLog();
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
            int minX = mod.PlayerLocationX - gv.playerOffset - 2; //using -2 in case a large tile (3x3) needs to start off the visible map space to be seen
            if (minX < 0) { minX = 0; }
            int minY = mod.PlayerLocationY - gv.playerOffset - 2; //using -2 in case a large tile (3x3) needs to start off the visible map space to be seen
            if (minY < 0) { minY = 0; }

            int maxX = mod.PlayerLocationX + gv.playerOffset + 1;
            if (maxX > this.mod.currentArea.MapSizeX) { maxX = this.mod.currentArea.MapSizeX; }
            int maxY = mod.PlayerLocationY + gv.playerOffset + 1;
            if (maxY > this.mod.currentArea.MapSizeY) { maxY = this.mod.currentArea.MapSizeY; }

            #region Draw Layer 1
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                    int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
                    int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
                    float scalerX = gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Width / 100;
                    float scalerY = gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Height / 100;
                    int brX = (int)(gv.squareSize * scalerX);
                    int brY = (int)(gv.squareSize * scalerY);
                                        
                    try
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Height);
                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                        gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer1Filename], src, dst);
                    }
                    catch { }
                }
            }
            #endregion
            #region Draw Layer 2
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                    int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
                    int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
                    float scalerX = gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Width / 100;
                    float scalerY = gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Height / 100;
                    int brX = (int)(gv.squareSize * scalerX);
                    int brY = (int)(gv.squareSize * scalerY);

                    try
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer2Filename].PixelSize.Height);
                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                        gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer2Filename], src, dst);
                    }
                    catch { }
                }
            }
            #endregion
            #region Draw Layer 3
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                    int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
                    int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
                    float scalerX = gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Width / 100;
                    float scalerY = gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Height / 100;
                    int brX = (int)(gv.squareSize * scalerX);
                    int brY = (int)(gv.squareSize * scalerY);

                    try
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer3Filename].PixelSize.Height);
                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                        gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer3Filename], src, dst);
                    }
                    catch { }
                }
            }
            #endregion
            #region Draw Layer 4
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                    int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
                    int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
                    float scalerX = gv.cc.tileBitmapList[tile.Layer4Filename].PixelSize.Width / 100;
                    float scalerY = gv.cc.tileBitmapList[tile.Layer4Filename].PixelSize.Height / 100;
                    int brX = (int)(gv.squareSize * scalerX);
                    int brY = (int)(gv.squareSize * scalerY);

                    try
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.tileBitmapList[tile.Layer4Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer4Filename].PixelSize.Height);
                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                        gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer4Filename], src, dst);
                    }
                    catch { }
                }
            }
            #endregion
            #region Draw Layer 5
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                    int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
                    int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
                    float scalerX = gv.cc.tileBitmapList[tile.Layer5Filename].PixelSize.Width / 100;
                    float scalerY = gv.cc.tileBitmapList[tile.Layer5Filename].PixelSize.Height / 100;
                    int brX = (int)(gv.squareSize * scalerX);
                    int brY = (int)(gv.squareSize * scalerY);

                    try
                    {
                        IbRect src = new IbRect(0, 0, gv.cc.tileBitmapList[tile.Layer5Filename].PixelSize.Width, gv.cc.tileBitmapList[tile.Layer5Filename].PixelSize.Height);
                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                        gv.DrawBitmap(gv.cc.tileBitmapList[tile.Layer5Filename], src, dst);
                    }
                    catch { }
                }
            }
            #endregion

            #region Draw Black Squares
            //draw black squares to make sure and hide any large tiles that have over drawn outside the visible map area
            int mapStartLocationInSquares = 6;
            int mapSizeInSquares = gv.playerOffset + gv.playerOffset + 1;
            int mapRightEndSquare = mapStartLocationInSquares + mapSizeInSquares;
            if (!gv.useLargeLayout) { mapStartLocationInSquares = 4; }
            IbRect srcBlackTile = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);

            //draw left side squares
            for (int x = mapStartLocationInSquares - 2; x < mapStartLocationInSquares; x++)
            {
                for (int y = 0; y < mapSizeInSquares; y++)
                {                    
                    IbRect dst = new IbRect(x * gv.squareSize + gv.oXshift, y * gv.squareSize, gv.squareSize, gv.squareSize);
                    gv.DrawBitmap(gv.cc.black_tile, srcBlackTile, dst);
                }
            }
            //draw right side squares
            for (int x = mapRightEndSquare; x < mapRightEndSquare + 2; x++)
            {
                for (int y = 0; y < mapSizeInSquares; y++)
                {
                    IbRect dst = new IbRect(x * gv.squareSize + gv.oXshift, y * gv.squareSize, gv.squareSize, gv.squareSize);
                    gv.DrawBitmap(gv.cc.black_tile, srcBlackTile, dst);
                }
            }
            //draw top squares
            for (int x = mapStartLocationInSquares - 2; x < mapRightEndSquare + 2; x++)
            {
                IbRect dst = new IbRect(x * gv.squareSize + gv.oXshift, -1 * gv.squareSize, gv.squareSize, gv.squareSize);
                gv.DrawBitmap(gv.cc.black_tile, srcBlackTile, dst);
            }
            //draw bottom squares
            for (int x = mapStartLocationInSquares - 2; x < mapRightEndSquare + 2; x++)
            {
                for (int y = mapSizeInSquares; y < mapSizeInSquares + 2; y++)
                {
                    IbRect dst = new IbRect(x * gv.squareSize + gv.oXshift, y * gv.squareSize, gv.squareSize, gv.squareSize);
                    gv.DrawBitmap(gv.cc.black_tile, srcBlackTile, dst);
                }
            }
            //draw black tiles over large tiles when party is near edges of map
            drawBlackTilesOverTints();
            #endregion
        }


        public void drawFullScreenEffects()
        {
            
            int minX = mod.PlayerLocationX - gv.playerOffset; 
            if (minX < 0) { minX = 0; }
            int minY = mod.PlayerLocationY - gv.playerOffset;
            if (minY < 0) { minY = 0; }

            int maxX = mod.PlayerLocationX + gv.playerOffset + 1;
            if (maxX > this.mod.currentArea.MapSizeX) { maxX = this.mod.currentArea.MapSizeX; }
            int maxY = mod.PlayerLocationY + gv.playerOffset + 1;
            if (maxY > this.mod.currentArea.MapSizeY) { maxY = this.mod.currentArea.MapSizeY; }

            //using three layers, so one could e.g. combine rain, fog and occassional lightning
            //or green fog, red alarm lights and stars flying by on the outer side of the space ship hull 
            #region Draw full screen layer 1
            if (gv.mod.currentArea.useFullScreenEffectLayer1)
            {
                gv.cc.DisposeOfBitmap(ref fullScreenEffect1);
                //use weather system per area specific later on
                //utilizing weather type defined by area weather settings
                //add check for square specific punch hole that prevents drawing weather, e.g. house inside or spaceship interior
                fullScreenEffect1 = gv.cc.LoadBitmap(gv.mod.currentArea.fullScreenEffectLayerName1);
                gv.mod.fullScreenAnimationFrameCounter1 += 1;
                if (gv.mod.fullScreenAnimationFrameCounter1 > (60 / (gv.mod.currentArea.fullScreenAnimationSpeed1 * gv.mod.allAnimationSpeedMultiplier)))
                {
                    gv.mod.fullScreenAnimationFrameCounter1 = 0;
                }
                //assuming a square shaped source here
                float sizeOfWholeSource = fullScreenEffect1.PixelSize.Width;
                //takes about two seconds for an animation to run completely through on my laptop with this default set up, all speeds at 1
                float numberOfPixToMovePerCounterIncrease = sizeOfWholeSource / (60 / (gv.mod.currentArea.fullScreenAnimationSpeed1 * gv.mod.allAnimationSpeedMultiplier));
                float pixShiftOnThisFrame = gv.mod.fullScreenAnimationFrameCounter1 * numberOfPixToMovePerCounterIncrease;

                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];

                        if (!tile.blockFullScreenEffectLayer1)
                        {
                            int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
                            int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;

                            float scalerX = gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Width / 100;
                            float scalerY = gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Height / 100;
                            int brX = (int)(gv.squareSize * scalerX);
                            int brY = (int)(gv.squareSize * scalerY);

                            float numberOfPictureParts = gv.playerOffset * 2 + 1;

                            //code section for handling right and bottom border of area
                            int modX = x;
                            int modY = y;
                            if ((mod.PlayerLocationX + 4) == this.mod.currentArea.MapSizeX)
                            {
                                modX += 1;
                            }
                            if ((mod.PlayerLocationX + 3) == this.mod.currentArea.MapSizeX)
                            {
                                modX += 2;
                            }
                            if ((mod.PlayerLocationX + 2) == this.mod.currentArea.MapSizeX)
                            {
                                modX += 3;
                            }
                            if ((mod.PlayerLocationX + 1) == this.mod.currentArea.MapSizeX)
                            {
                                modX += 4;
                            }


                            if ((mod.PlayerLocationY + 4) == this.mod.currentArea.MapSizeY)
                            {
                                modY += 1;
                            }
                            if ((mod.PlayerLocationY + 3) == this.mod.currentArea.MapSizeY)
                            {
                                modY += 2;
                            }
                            if ((mod.PlayerLocationY + 2) == this.mod.currentArea.MapSizeY)
                            {
                                modY += 3;
                            }
                            if ((mod.PlayerLocationY + 1) == this.mod.currentArea.MapSizeY)
                            {
                                modY += 4;
                            }

                            //get the correct chunk on source
                            float floatSourceChunkCoordX = 0;
                            float floatSourceChunkCoordY = 0;

                            //scroll down
                            if (gv.mod.currentArea.fullScreenAnimationMovePattern1 == "down")
                            {
                                floatSourceChunkCoordX = ((float)(modX - minX) / numberOfPictureParts) * sizeOfWholeSource;
                                floatSourceChunkCoordY = ((float)(modY - minY) / numberOfPictureParts) * sizeOfWholeSource - (pixShiftOnThisFrame);
                                if (floatSourceChunkCoordY < 0)
                                {
                                    floatSourceChunkCoordY = sizeOfWholeSource + floatSourceChunkCoordY;
                                }

                                if ((floatSourceChunkCoordY + (sizeOfWholeSource / numberOfPictureParts)) > sizeOfWholeSource)
                                {
                                    //need to use parts from two source chunks and draw them onto the dst square

                                    //first render the existing part of chunk1 (ie cut off blank space) 
                                    //to the part of dst that is closer to scroll direction
                                    float availableLength = sizeOfWholeSource - floatSourceChunkCoordY;
                                    float dstScaler = availableLength / (sizeOfWholeSource / numberOfPictureParts);

                                    int srcCoordY2 = (int)floatSourceChunkCoordY;
                                    int srcCoordX2 = (int)floatSourceChunkCoordX;
                                    int sizeOfSourceChunk2 = (int)(sizeOfWholeSource / numberOfPictureParts);

                                    try
                                    {
                                        IbRect src = new IbRect(srcCoordX2, srcCoordY2, sizeOfSourceChunk2, (int)availableLength);
                                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, (int)(brY * dstScaler));
                                        gv.DrawBitmap(fullScreenEffect1, src, dst);
                                    }
                                    catch { }

                                    //now lets draw the escond part of dst, using a part of chunk2
                                    srcCoordY2 = 0;
                                    srcCoordX2 = (int)floatSourceChunkCoordX;
                                    availableLength = (sizeOfWholeSource / numberOfPictureParts) - availableLength;

                                    try
                                    {
                                        IbRect src = new IbRect(srcCoordX2, srcCoordY2, sizeOfSourceChunk2, (int)availableLength);
                                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY + (int)(brY * dstScaler), brX, brY - (int)(brY * dstScaler));
                                        gv.DrawBitmap(fullScreenEffect1, src, dst);
                                    }
                                    catch { }

                                    //this continue makes sure we dont do the normal drawing below
                                    continue;
                                }
                            }

                            //scroll up
                            if (gv.mod.currentArea.fullScreenAnimationMovePattern1 == "up")
                            {
                                floatSourceChunkCoordX = ((float)(modX - minX) / numberOfPictureParts) * sizeOfWholeSource;
                                floatSourceChunkCoordY = ((float)(modY - minY) / numberOfPictureParts) * sizeOfWholeSource + (pixShiftOnThisFrame);
                                if (floatSourceChunkCoordY > sizeOfWholeSource)
                                {
                                    floatSourceChunkCoordY = floatSourceChunkCoordY - sizeOfWholeSource;
                                }

                                if ((floatSourceChunkCoordY + (sizeOfWholeSource / numberOfPictureParts)) > sizeOfWholeSource)
                                {
                                    //need to use parts from two source chunks and draw them onto the dst square

                                    //first render the existing part of chunk1 (ie cut off blank space) 
                                    //to the part of dst that is closer to scroll direction
                                    float availableLength = sizeOfWholeSource - floatSourceChunkCoordY;
                                    float dstScaler = availableLength / (sizeOfWholeSource / numberOfPictureParts);

                                    int srcCoordY2 = (int)floatSourceChunkCoordY;
                                    int srcCoordX2 = (int)floatSourceChunkCoordX;
                                    int sizeOfSourceChunk2 = (int)(sizeOfWholeSource / numberOfPictureParts);

                                    try
                                    {
                                        IbRect src = new IbRect(srcCoordX2, srcCoordY2, sizeOfSourceChunk2, (int)availableLength);
                                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, (int)(brY * dstScaler));
                                        gv.DrawBitmap(fullScreenEffect1, src, dst);
                                    }
                                    catch { }

                                    //now lets draw the escond part of dst, using a part of chunk2
                                    srcCoordY2 = 0;
                                    srcCoordX2 = (int)floatSourceChunkCoordX;
                                    availableLength = (sizeOfWholeSource / numberOfPictureParts) - availableLength;

                                    try
                                    {
                                        IbRect src = new IbRect(srcCoordX2, srcCoordY2, sizeOfSourceChunk2, (int)availableLength);
                                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY + (int)(brY * dstScaler), brX, brY - (int)(brY * dstScaler));
                                        gv.DrawBitmap(fullScreenEffect1, src, dst);
                                    }
                                    catch { }

                                    //this continue makes sure we dont do the normal drawing below
                                    continue;
                                }
                            }

                            //scroll right
                            if (gv.mod.currentArea.fullScreenAnimationMovePattern1 == "right")
                            {
                                floatSourceChunkCoordX = ((float)(modX - minX) / numberOfPictureParts) * sizeOfWholeSource - (pixShiftOnThisFrame);
                                floatSourceChunkCoordY = ((float)(modY - minY) / numberOfPictureParts) * sizeOfWholeSource;
                                if (floatSourceChunkCoordX < 0)
                                {
                                    floatSourceChunkCoordX = sizeOfWholeSource + floatSourceChunkCoordX;
                                }
                                if ((floatSourceChunkCoordX + (sizeOfWholeSource / numberOfPictureParts)) > sizeOfWholeSource)
                                {
                                    //need to use parts from two source chunks and draw them onto the dst square

                                    //first render the existing part of chunk1 (ie cut off blank space) 
                                    //to the part of dst that is closer to scroll direction
                                    float availableLength = sizeOfWholeSource - floatSourceChunkCoordX;
                                    float dstScaler = availableLength / (sizeOfWholeSource / numberOfPictureParts);

                                    int srcCoordY2 = (int)floatSourceChunkCoordY;
                                    int srcCoordX2 = (int)floatSourceChunkCoordX;
                                    int sizeOfSourceChunk2 = (int)(sizeOfWholeSource / numberOfPictureParts);

                                    try
                                    {
                                        IbRect src = new IbRect(srcCoordX2, srcCoordY2, (int)availableLength, sizeOfSourceChunk2);
                                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, (int)(brX * dstScaler), brY);
                                        gv.DrawBitmap(fullScreenEffect1, src, dst);
                                    }
                                    catch { }

                                    //now lets draw the escond part of dst, using a part of chunk2
                                    srcCoordY2 = (int)floatSourceChunkCoordY;
                                    srcCoordX2 = 0;
                                    availableLength = (sizeOfWholeSource / numberOfPictureParts) - availableLength;

                                    try
                                    {
                                        IbRect src = new IbRect(srcCoordX2, srcCoordY2, (int)availableLength, sizeOfSourceChunk2);
                                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels + (int)(brX * dstScaler), tlY, brX - (int)(brX * dstScaler), brY);
                                        gv.DrawBitmap(fullScreenEffect1, src, dst);
                                    }
                                    catch { }

                                    //this continue makes sure we dont do the normal drawing below
                                    continue;
                                }
                            }

                            //scroll left
                            if (gv.mod.currentArea.fullScreenAnimationMovePattern1 == "left")
                            {
                                floatSourceChunkCoordX = ((float)(modX - minX) / numberOfPictureParts) * sizeOfWholeSource + (pixShiftOnThisFrame);
                                floatSourceChunkCoordY = ((float)(modY - minY) / numberOfPictureParts) * sizeOfWholeSource;
                                if (floatSourceChunkCoordX > sizeOfWholeSource)
                                {
                                    floatSourceChunkCoordX = floatSourceChunkCoordX - sizeOfWholeSource;
                                }
                                if ((floatSourceChunkCoordX + (sizeOfWholeSource / numberOfPictureParts) ) > sizeOfWholeSource)
                                {
                                    //need to use parts from two source chunks and draw them onto the dst square

                                    //first render the existing part of chunk1 (ie cut off blank space) 
                                    //to the part of dst that is closer to scroll direction
                                    float availableLength = sizeOfWholeSource - floatSourceChunkCoordX;
                                    float dstScaler = availableLength / (sizeOfWholeSource / numberOfPictureParts);

                                    int srcCoordY2 = (int)floatSourceChunkCoordY;
                                    int srcCoordX2 = (int)floatSourceChunkCoordX;
                                    int sizeOfSourceChunk2 = (int)(sizeOfWholeSource / numberOfPictureParts);

                                    try
                                    {
                                        IbRect src = new IbRect(srcCoordX2, srcCoordY2, (int)availableLength, sizeOfSourceChunk2);
                                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, (int)(brX * dstScaler), brY);
                                        gv.DrawBitmap(fullScreenEffect1, src, dst);
                                    }
                                    catch { }

                                    //now lets draw the escond part of dst, using a part of chunk2
                                    srcCoordY2 = (int)floatSourceChunkCoordY;
                                    srcCoordX2 = 0;
                                    availableLength = (sizeOfWholeSource / numberOfPictureParts) - availableLength;

                                    try
                                    {
                                        IbRect src = new IbRect(srcCoordX2, srcCoordY2, (int)availableLength, sizeOfSourceChunk2);
                                        IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels + (int)(brX * dstScaler), tlY, brX - (int)(brX * dstScaler), brY);
                                        gv.DrawBitmap(fullScreenEffect1, src, dst);
                                    }
                                    catch { }

                                    //this continue makes sure we dont do the normal drawing below
                                    continue;
                                }
                            }

                            //use individual animation frames
                            //using umberOfFrames and delayBetweenFrames as properties of the layer in area
                            //WIP
                            if (gv.mod.currentArea.fullScreenAnimationMovePattern1 == "individual")
                            {
                                floatSourceChunkCoordX = ((float)(modX - minX) / numberOfPictureParts) * sizeOfWholeSource + (pixShiftOnThisFrame);
                                floatSourceChunkCoordY = ((float)(modY - minY) / numberOfPictureParts) * sizeOfWholeSource;
                                if (floatSourceChunkCoordX > sizeOfWholeSource)
                                {
                                    floatSourceChunkCoordX = floatSourceChunkCoordX - sizeOfWholeSource;
                                }
                            }

                            int srcCoordY = (int)floatSourceChunkCoordY;
                            int srcCoordX = (int)floatSourceChunkCoordX;
                            int sizeOfSourceChunk = (int)(sizeOfWholeSource / numberOfPictureParts);

                            try
                            {
                                IbRect src = new IbRect(srcCoordX, srcCoordY, sizeOfSourceChunk, sizeOfSourceChunk);
                                IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                                gv.DrawBitmap(fullScreenEffect1, src, dst);
                            }
                            catch { }
                        }
                    }
                }
            }
            #endregion
            #region Draw full screen layer 2
            if (gv.mod.currentArea.useFullScreenEffectLayer2)
            {
                //new SharpDX.Direct2D1.Bitmap
                gv.cc.DisposeOfBitmap(ref fullScreenEffect2);
                //use weather system per area specific later on
                //use animation frame number 1 to 10 specific later on, utilizing weather type defined by area weather settings
                //add check for square spcific punch hole that prevents drawing weather, e.g. house inside or spaceship interior
                fullScreenEffect2 = gv.cc.LoadBitmap(gv.mod.currentArea.fullScreenEffectLayerName2);

                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                        int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
                        int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;
                        
                        float scalerX = gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Width / 100;
                        float scalerY = gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Height / 100;
                        int brX = (int)(gv.squareSize * scalerX);
                        int brY = (int)(gv.squareSize * scalerY);
                        
                        float numberOfPictureParts = gv.playerOffset * 2 + 1;
                        float sizeOfWholeSource = fullScreenEffect2.PixelSize.Width;

                        //code sction for handling right and bottom border of area
                        int modX = x;
                        int modY = y;
                        if ((mod.PlayerLocationX + 4) == this.mod.currentArea.MapSizeX)
                        {
                            modX += 1;
                        }
                        if ((mod.PlayerLocationX + 3) == this.mod.currentArea.MapSizeX)
                        {
                            modX += 2;
                        }
                        if ((mod.PlayerLocationX + 2) == this.mod.currentArea.MapSizeX)
                        { 
                            modX += 3;
                        }
                        if ((mod.PlayerLocationX + 1) == this.mod.currentArea.MapSizeX)
                        {
                            modX += 4;
                        }


                        if ((mod.PlayerLocationY + 4) == this.mod.currentArea.MapSizeY)
                        {
                            modY += 1;
                        }
                        if ((mod.PlayerLocationY + 3) == this.mod.currentArea.MapSizeY)
                        {
                            modY += 2;
                        }
                        if ((mod.PlayerLocationY + 2) == this.mod.currentArea.MapSizeY)
                        {
                            modY += 3;
                        }
                        if ((mod.PlayerLocationY + 1) == this.mod.currentArea.MapSizeY)
                        {
                            modY += 4;
                        }

                        //get the correct chunk on source
                        float floatSourceChunkCoordX = ((float)(modX - minX) / numberOfPictureParts) * sizeOfWholeSource;
                        float floatSourceChunkCoordY = ((float)(modY - minY) / numberOfPictureParts) * sizeOfWholeSource;
                        int srcCoordY = (int)floatSourceChunkCoordY;
                        int srcCoordX = (int)floatSourceChunkCoordX;
                        int sizeOfSourceChunk = (int)(sizeOfWholeSource / numberOfPictureParts);

                        try
                        {
                            IbRect src = new IbRect(srcCoordX, srcCoordY, sizeOfSourceChunk, sizeOfSourceChunk);
                            IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                            gv.DrawBitmap(fullScreenEffect2, src, dst);
                        }
                        catch { }
                    }
                }
            }
            #endregion

            #region Draw full screen layer 3
            if (gv.mod.currentArea.useFullScreenEffectLayer3)
            {
                //new SharpDX.Direct2D1.Bitmap
                gv.cc.DisposeOfBitmap(ref fullScreenEffect3);
                //use weather system per area specific later on
                //use animation frame number 1 to 10 specific later on, utilizing weather type defined by area weather settings
                //add check for square spcific punch hole that prevents drawing weather, e.g. house inside or spaceship interior
                fullScreenEffect3 = gv.cc.LoadBitmap(gv.mod.currentArea.fullScreenEffectLayerName3);

                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        Tile tile = mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x];
                        int tlX = (x - mod.PlayerLocationX + gv.playerOffset) * gv.squareSize;
                        int tlY = (y - mod.PlayerLocationY + gv.playerOffset) * gv.squareSize;

                        float scalerX = gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Width / 100;
                        float scalerY = gv.cc.tileBitmapList[tile.Layer1Filename].PixelSize.Height / 100;
                        int brX = (int)(gv.squareSize * scalerX);
                        int brY = (int)(gv.squareSize * scalerY);

                        float numberOfPictureParts = gv.playerOffset * 2 + 1;
                        float sizeOfWholeSource = fullScreenEffect3.PixelSize.Width;

                        //code sction for handling right and bottom border of area
                        int modX = x;
                        int modY = y;
                        if ((mod.PlayerLocationX + 4) == this.mod.currentArea.MapSizeX)
                        {
                            modX += 1;
                        }
                        if ((mod.PlayerLocationX + 3) == this.mod.currentArea.MapSizeX)
                        {
                            modX += 2;
                        }
                        if ((mod.PlayerLocationX + 2) == this.mod.currentArea.MapSizeX)
                        {
                            modX += 3;
                        }
                        if ((mod.PlayerLocationX + 1) == this.mod.currentArea.MapSizeX)
                        {
                            modX += 4;
                        }


                        if ((mod.PlayerLocationY + 4) == this.mod.currentArea.MapSizeY)
                        {
                            modY += 1;
                        }
                        if ((mod.PlayerLocationY + 3) == this.mod.currentArea.MapSizeY)
                        {
                            modY += 2;
                        }
                        if ((mod.PlayerLocationY + 2) == this.mod.currentArea.MapSizeY)
                        {
                            modY += 3;
                        }
                        if ((mod.PlayerLocationY + 1) == this.mod.currentArea.MapSizeY)
                        {
                            modY += 4;
                        }

                        //get the correct chunk on source
                        float floatSourceChunkCoordX = ((float)(modX - minX) / numberOfPictureParts) * sizeOfWholeSource;
                        float floatSourceChunkCoordY = ((float)(modY - minY) / numberOfPictureParts) * sizeOfWholeSource;
                        int srcCoordY = (int)floatSourceChunkCoordY;
                        int srcCoordX = (int)floatSourceChunkCoordX;
                        int sizeOfSourceChunk = (int)(sizeOfWholeSource / numberOfPictureParts);

                        try
                        {
                            IbRect src = new IbRect(srcCoordX, srcCoordY, sizeOfSourceChunk, sizeOfSourceChunk);
                            IbRect dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                            gv.DrawBitmap(fullScreenEffect3, src, dst);
                        }
                        catch { }
                    }
                }
            }
            #endregion
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
                        int dstW = (int)(((float)p.token.PixelSize.Width / (float)gv.squareSizeInPixels) * (float)gv.squareSize);
                        int dstH = (int)(((float)p.token.PixelSize.Height / (float)gv.squareSizeInPixels) * (float)gv.squareSize);
                        int dstXshift = (dstW - gv.squareSize) / 2;
                        int dstYshift = (dstH - gv.squareSize) / 2;
                        IbRect src = new IbRect(0, 0, p.token.PixelSize.Width, p.token.PixelSize.Width);
                        IbRect dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels - dstXshift, y - dstYshift, dstW, dstH);

                        if (gv.mod.currentArea.useSuperTinyProps)
                        {
                            dst = new IbRect((int)p.currentPixelPositionX + (int)(gv.squareSize * 3 / 8) - dstXshift, (int)p.currentPixelPositionY + (int)(gv.squareSize * 3 / 8) - dstYshift, (int)(dstW / 4), (int)(dstH / 4));
                        }
                        else if (gv.mod.currentArea.useMiniProps)
                        {
                            dst = new IbRect((int)p.currentPixelPositionX + (int)(gv.squareSize / 4) - dstXshift, (int)p.currentPixelPositionY + (int)(gv.squareSize / 4) - dstYshift, (int)(dstW / 2), (int)(dstH / 2));
                        }

                        gv.DrawBitmap(p.token, src, dst, !p.PropFacingLeft, false);

                        if (mod.showInteractionState == true)
                        {
                            if (!p.EncounterWhenOnPartySquare.Equals("none"))
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("encounter_indicator");
                                src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                gv.cc.DisposeOfBitmap(ref interactionStateIndicator); 
                                continue;
                            }

                            if (p.unavoidableConversation)
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                                src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                continue;
                            }

                            if (!p.ConversationWhenOnPartySquare.Equals("none"))
                            {
                                Bitmap interactionStateIndicator = gv.cc.LoadBitmap("optional_conversation_indicator");
                                src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                gv.DrawBitmap(interactionStateIndicator, src, dst);
                                gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                continue;
                            }
                        }
                    }
                }
            }
        }
        public void drawMovingProps()
        {
            if (mod.useSmoothMovement == true)
            {
                foreach (Prop p in mod.currentArea.Props)
                {
                    if ((p.isShown) && (p.isMover))
                    {
                        if ((p.LocationX + 1 >= mod.PlayerLocationX - gv.playerOffset) && (p.LocationX - 1 <= mod.PlayerLocationX + gv.playerOffset)
                            && (p.LocationY + 1 >= mod.PlayerLocationY - gv.playerOffset) && (p.LocationY - 1 <= mod.PlayerLocationY + gv.playerOffset))
                        {
                            IbRect src = new IbRect(0, 0, p.token.PixelSize.Width, p.token.PixelSize.Width);
                            if (p.destinationPixelPositionXList.Count > 0)
                            {
                                if ((p.destinationPixelPositionXList[0] >= (p.currentPixelPositionX - 0)) && (p.destinationPixelPositionXList[0] <= (p.currentPixelPositionX + 0)))
                                {
                                    if (p.destinationPixelPositionYList[0] > p.currentPixelPositionY)
                                    {
                                        p.currentPixelPositionY += (gv.floatPixMovedPerTick * p.pixelMoveSpeed);
                                        if (p.currentPixelPositionY >= p.destinationPixelPositionYList[0])
                                        {
                                            p.currentPixelPositionY = p.destinationPixelPositionYList[0];
                                            p.destinationPixelPositionYList.RemoveAt(0);
                                            p.destinationPixelPositionXList.RemoveAt(0);
                                            
                                        }
                                    }
                                    else
                                    {
                                        p.currentPixelPositionY -= (gv.floatPixMovedPerTick * p.pixelMoveSpeed);
                                        if (p.currentPixelPositionY <= p.destinationPixelPositionYList[0])
                                        {
                                            p.currentPixelPositionY = p.destinationPixelPositionYList[0];
                                            p.destinationPixelPositionYList.RemoveAt(0);
                                            p.destinationPixelPositionXList.RemoveAt(0);
                                        }

                                    }
                                }
                                else if ((p.destinationPixelPositionYList[0] >= (p.currentPixelPositionY - 0)) && (p.destinationPixelPositionYList[0] <= (p.currentPixelPositionY + 0)))
                                {
                                    {
                                        if (p.destinationPixelPositionXList[0] > p.currentPixelPositionX)
                                        {
                                            p.currentPixelPositionX += (gv.floatPixMovedPerTick * p.pixelMoveSpeed);
                                            if (p.currentPixelPositionX >= p.destinationPixelPositionXList[0])
                                            {
                                                p.currentPixelPositionX = p.destinationPixelPositionXList[0];
                                                p.destinationPixelPositionXList.RemoveAt(0);
                                                p.destinationPixelPositionYList.RemoveAt(0);
                                            }
                                        }
                                        else
                                        {
                                            p.currentPixelPositionX -= (gv.floatPixMovedPerTick * p.pixelMoveSpeed);
                                            if (p.currentPixelPositionX <= p.destinationPixelPositionXList[0])
                                            {
                                                p.currentPixelPositionX = p.destinationPixelPositionXList[0];
                                                p.destinationPixelPositionXList.RemoveAt(0);
                                                p.destinationPixelPositionYList.RemoveAt(0);
                                            }                                   
                                        }
                                    }
                                }

                            }//end, set dst

                            int playerPositionXInPix = 0;
                            int playerPositionYInPix = 0;

                            if (p.destinationPixelPositionXList.Count <= 0)
                            {
                                p.destinationPixelPositionXList.Clear();
                                p.destinationPixelPositionXList = new List<int>();
                                p.destinationPixelPositionYList.Clear();
                                p.destinationPixelPositionYList = new List<int>();

                                //set the currentPixel position of the props
                                int xOffSetInSquares = p.LocationX - gv.mod.PlayerLocationX;
                                int yOffSetInSquares = p.LocationY - gv.mod.PlayerLocationY;
                                 playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffset * gv.squareSize);
                                playerPositionYInPix = gv.playerOffset * gv.squareSize;

                                p.currentPixelPositionX = playerPositionXInPix + (xOffSetInSquares * gv.squareSize);
                                p.currentPixelPositionY = playerPositionYInPix + (yOffSetInSquares * gv.squareSize);
                            }


                            playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffset * gv.squareSize);
                            playerPositionYInPix = gv.playerOffset * gv.squareSize + gv.oYshift;

                            float floatConvertedToSquareDistanceX = (p.currentPixelPositionX - playerPositionXInPix) / gv.squareSize;
                            int ConvertedToSquareDistanceX = (int)Math.Ceiling(floatConvertedToSquareDistanceX);

                            float floatConvertedToSquareDistanceY = (p.currentPixelPositionY - playerPositionYInPix) / gv.squareSize;
                            int ConvertedToSquareDistanceY = (int)Math.Ceiling(floatConvertedToSquareDistanceY);

                            int SquareThatPixIsOnX = mod.PlayerLocationX + ConvertedToSquareDistanceX;
                            int SquareThatPixIsOnY = mod.PlayerLocationY + ConvertedToSquareDistanceY;

                            int tileNumberOfPropSquare = SquareThatPixIsOnX + (SquareThatPixIsOnY * gv.mod.currentArea.MapSizeX);

                            //cast the pix position to int in order to draw it at nearly exact loc
                            int pixDistanceOfPropToPlayerX = ((int)p.currentPixelPositionX - playerPositionXInPix);
                            if (pixDistanceOfPropToPlayerX < 0)
                            {
                                pixDistanceOfPropToPlayerX *= -1;
                            }
                            int pixDistanceOfPropToPlayerY = ((int)p.currentPixelPositionY - playerPositionYInPix);
                            if (pixDistanceOfPropToPlayerY < 0)
                            {
                                pixDistanceOfPropToPlayerY *= -1;
                            }

                            if ((pixDistanceOfPropToPlayerX <= ((gv.playerOffset + 1) * gv.squareSize)) && (pixDistanceOfPropToPlayerY <= ((gv.playerOffset + 1) * gv.squareSize)))
                            {
                                int dstW = (int)(((float)p.token.PixelSize.Width / (float)gv.squareSizeInPixels) * (float)gv.squareSize);
                                int dstH = (int)(((float)p.token.PixelSize.Height / (float)gv.squareSizeInPixels) * (float)gv.squareSize);
                                int dstXshift = (dstW - gv.squareSize) / 2;
                                int dstYshift = (dstH - gv.squareSize) / 2;
                                IbRect dst = new IbRect((int)p.currentPixelPositionX - dstXshift, (int)p.currentPixelPositionY - dstYshift, dstW, dstH);

                                if (gv.mod.currentArea.useSuperTinyProps)
                                {
                                    dst = new IbRect((int)p.currentPixelPositionX + (int)(gv.squareSize * 3 /8) - dstXshift, (int)p.currentPixelPositionY + (int)(gv.squareSize * 3 / 8) - dstYshift, (int)(dstW / 4), (int)(dstH / 4));
                                }
                                else if (gv.mod.currentArea.useMiniProps)
                                {
                                    dst = new IbRect((int)p.currentPixelPositionX + (int)(gv.squareSize / 4) - dstXshift, (int)p.currentPixelPositionY + (int)(gv.squareSize / 4) - dstYshift, (int)(dstW / 2), (int)(dstH / 2));
                                }

                                gv.DrawBitmap(p.token, src, dst);

                                if (mod.showInteractionState == true)
                                {
                                    if (!p.EncounterWhenOnPartySquare.Equals("none"))
                                    {
                                        Bitmap interactionStateIndicator = gv.cc.LoadBitmap("encounter_indicator");
                                        src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                        gv.DrawBitmap(interactionStateIndicator, src, dst);
                                        gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                        continue;
                                    }

                                    if (p.unavoidableConversation)
                                    {
                                        Bitmap interactionStateIndicator = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                                        src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                        gv.DrawBitmap(interactionStateIndicator, src, dst);
                                        gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                        continue;
                                    }

                                    if (!p.ConversationWhenOnPartySquare.Equals("none"))
                                    {
                                        Bitmap interactionStateIndicator = gv.cc.LoadBitmap("optional_conversation_indicator");
                                        src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                        gv.DrawBitmap(interactionStateIndicator, src, dst);
                                        gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                        continue;
                                    }
                                }
                            }

                        }
                    }
                }
                for (int i = 0; i < mod.currentArea.Tiles.Count; i++)
                {

                    float floatPositionY = i / mod.currentArea.MapSizeX;
                    int positionY = (int)Math.Floor(floatPositionY);
                    int positionX = i % mod.currentArea.MapSizeY;
                    int dist = 0;
                    int deltaX = (int)Math.Abs((positionX - mod.PlayerLocationX));
                    int deltaY = (int)Math.Abs((positionY - mod.PlayerLocationY));
                    if (deltaX > deltaY)
                    {
                        dist = deltaX;
                    }
                    else
                    {
                        dist = deltaY;
                    }
                    if ((dist == (gv.playerOffset + 1)) || (dist == (gv.playerOffset + 2)))
                    {
                        int squareInPixelsX = ((positionX - mod.PlayerLocationX) * gv.squareSize) + gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffset * gv.squareSize);
                        int squareInPixelsY = ((positionY - mod.PlayerLocationY) * gv.squareSize) + (gv.playerOffset * gv.squareSize);
                        IbRect src2 = new IbRect(0, 0, gv.squareSize, gv.squareSize);
                        IbRect dst2 = new IbRect(squareInPixelsX, squareInPixelsY, gv.squareSize, gv.squareSize);
                        gv.DrawBitmap(gv.cc.black_tile, src2, dst2);
                    }
                }

            }
            else
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
                            int dstW = (int)(((float)p.token.PixelSize.Width / (float)gv.squareSizeInPixels) * (float)gv.squareSize);
                            int dstH = (int)(((float)p.token.PixelSize.Height / (float)gv.squareSizeInPixels) * (float)gv.squareSize);
                            int dstXshift = (dstW - gv.squareSize) / 2;
                            int dstYshift = (dstH - gv.squareSize) / 2;
                            IbRect src = new IbRect(0, 0, p.token.PixelSize.Width, p.token.PixelSize.Width);
                            IbRect dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels - dstXshift, y - dstYshift, dstW, dstH);
                            gv.DrawBitmap(p.token, src, dst);

                            if (mod.showInteractionState)
                            {
                                if (!p.EncounterWhenOnPartySquare.Equals("none"))
                                {
                                    Bitmap interactionStateIndicator = gv.cc.LoadBitmap("encounter_indicator");
                                    src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                    gv.DrawBitmap(interactionStateIndicator, src, dst);
                                    gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                    continue;
                                }

                                if (p.unavoidableConversation)
                                {
                                    Bitmap interactionStateIndicator = gv.cc.LoadBitmap("mandatory_conversation_indicator");
                                    src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                    gv.DrawBitmap(interactionStateIndicator, src, dst);
                                    gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                    continue;
                                }

                                if (!p.ConversationWhenOnPartySquare.Equals("none"))
                                {
                                    Bitmap interactionStateIndicator = gv.cc.LoadBitmap("optional_conversation_indicator");
                                    src = new IbRect(0, 0, interactionStateIndicator.PixelSize.Width, interactionStateIndicator.PixelSize.Height);
                                    gv.DrawBitmap(interactionStateIndicator, src, dst);
                                    gv.cc.DisposeOfBitmap(ref interactionStateIndicator);
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void drawMiniMap()
        {
            //if ((mod.currentArea.IsWorldMap) && (tglMiniMap.toggleOn))
            if (tglMiniMap.toggleOn)
            {
                int pW = (int)((float)gv.screenWidth / 100.0f);
                int pH = (int)((float)gv.screenHeight / 100.0f);
                int shift = pW;
                //Bitmap minimap = gv.cc.LoadBitmap(mod.currentArea.Filename);

                //minimap should be 4 squares wide
                int minimapSquareSizeInPixels = 4 * gv.squareSize / mod.currentArea.MapSizeX;
                int drawW = minimapSquareSizeInPixels * mod.currentArea.MapSizeX;
                int drawH = minimapSquareSizeInPixels * mod.currentArea.MapSizeY;

                //int mapSqrX = minimap.PixelSize.Width / 5;
                //int mapSqrY = minimap.PixelSize.Height / 5;
                //int drawW = mapSqrX * pW / 2;
                //int drawH = mapSqrY * pW / 2;
                /*TODO
                    //draw a dark border
                    Paint pnt = new Paint();
                    pnt.setColor(Color.DKGRAY);
                    pnt.setStrokeWidth(pW * 2);
                    pnt.setStyle(Paint.Style.STROKE);	
                    canvas.drawRect(new Rect(gv.oXshift, pH, gv.oXshift + drawW + pW, pH + drawH + pW), pnt);
                */
                //draw minimap
                if (minimap == null) { resetMiniMapBitmap(); }
                IbRect src = new IbRect(0, 0, minimap.PixelSize.Width, minimap.PixelSize.Height);
                IbRect dst = new IbRect(pW, pH, drawW, drawH);
                gv.DrawBitmap(minimap, src, dst);

                //draw Fog of War
                for (int x = 0; x < this.mod.currentArea.MapSizeX; x++)                
                {
                    for (int y = 0; y < this.mod.currentArea.MapSizeY; y++)                    
                    {
                        int xx = x * minimapSquareSizeInPixels;
                        int yy = y * minimapSquareSizeInPixels;
                        src = new IbRect(0, 0, gv.cc.black_tile.PixelSize.Width, gv.cc.black_tile.PixelSize.Height);
                        dst = new IbRect(pW + xx, pH + yy, minimapSquareSizeInPixels, minimapSquareSizeInPixels);
                        if (!mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].Visible)
                        {
                            gv.DrawBitmap(gv.cc.black_tile, src, dst);
                        }
                    }
                }
                                
	            //draw a location marker square RED
                int x2 = mod.PlayerLocationX * minimapSquareSizeInPixels;
                int y2 = mod.PlayerLocationY * minimapSquareSizeInPixels;
                src = new IbRect(0, 0, gv.cc.pc_dead.PixelSize.Width, gv.cc.pc_dead.PixelSize.Height);
                dst = new IbRect(pW + x2, pH + y2, minimapSquareSizeInPixels, minimapSquareSizeInPixels);
                gv.DrawBitmap(gv.cc.pc_dead, src, dst);	            
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
            if (mod.currentArea.useMiniProps)
            {
                shift = (int)shift / 2;
            }
            else if (mod.currentArea.useSuperTinyProps)
            {
                shift = (int)shift / 4;
            }
                IbRect src = new IbRect(0, 0, mod.playerList[mod.selectedPartyLeader].token.PixelSize.Width, mod.playerList[mod.selectedPartyLeader].token.PixelSize.Width);
            IbRect dst = new IbRect(x + gv.oXshift + mapStartLocXinPixels, y, gv.squareSize, gv.squareSize);
            if (mod.showPartyToken)
            {

                if (mod.currentArea.useMiniProps)
                {
                    dst.Top += (int)(gv.squareSize * 1 / 8);
                    if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                    {
                        dst.Left += (int)(gv.squareSize / 4);
                    }
                    else
                    {
                        dst.Left -= (int)(gv.squareSize / 4);
                    }
                    dst.Height -= (int)(dst.Height / 2);
                    dst.Width -= (int)(dst.Width / 2);

                    /*dst.Top += (int)(gv.squareSize / 4);
                    dst.Left += (int)(gv.squareSize / 4);
                    dst.Height -= (int)(dst.Height / 2);
                    dst.Width -= (int)(dst.Width / 2);*/
                }
                else if (mod.currentArea.useSuperTinyProps)
                {
                    dst.Top += (int)(gv.squareSize * 1 / 8);
                    if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                    {
                        dst.Left += (int)(gv.squareSize * 3 / 8);
                    }
                    else
                    {
                        dst.Left -= (int)(gv.squareSize * 3 / 8);
                    }
                    dst.Height -= (int)(dst.Height * 3 / 4);
                    dst.Width -= (int)(dst.Width * 3 / 4);

                    /*dst.Top += (int)(gv.squareSize * 3 / 8);
                    dst.Left += (int)(gv.squareSize * 3 / 8);
                    dst.Height -= (int)(dst.Height / 4);
                    dst.Width -= (int)(dst.Width / 4);*/
                }

                gv.DrawBitmap(mod.partyTokenBitmap, src, dst, !mod.playerList[0].combatFacingLeft, false);
            }
            else
            {
                if ((tglFullParty.toggleOn) && (mod.playerList.Count > 1))
                {
                    if (mod.playerList[0].combatFacingLeft == true)
                    {
                        gv.oXshift = gv.oXshift + shift / 2;
                    }
                    else
                    {
                        gv.oXshift = gv.oXshift - shift / 2;
                    }
                    //gv.squareSize = gv.squareSize * 2 / 3;
                    int reducedSquareSize = gv.squareSize * 2 / 3;
                    for (int i = mod.playerList.Count - 1; i >= 0; i--)
                    {
                        if ((i == 0) && (i != mod.selectedPartyLeader))
                        {
                            dst = new IbRect(x + gv.oXshift + shift + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);

                            if (mod.currentArea.useMiniProps)
                            {
                                dst.Top += (int)(gv.squareSize * 1 / 8);
                                if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                {
                                    dst.Left += (int)(gv.squareSize / 4);
                                }
                                else
                                {
                                    dst.Left -= (int)(gv.squareSize / 4);
                                }
                                dst.Height -= (int)(dst.Height / 2);
                                dst.Width -= (int)(dst.Width / 2);
                            }
                            else if (mod.currentArea.useSuperTinyProps)
                            {
                                dst.Top += (int)(gv.squareSize * 1 / 8);
                                if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                {
                                    dst.Left += (int)(gv.squareSize * 3 / 8);
                                }
                                else
                                {
                                    dst.Left -= (int)(gv.squareSize * 3 / 8);
                                }
                                dst.Height -= (int)(dst.Height * 3 / 4);
                                dst.Width -= (int)(dst.Width * 3 / 4);
                            }

                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
                        }
                        if ((i == 1) && (i != mod.selectedPartyLeader))
                        {
                            dst = new IbRect(x + gv.oXshift - shift + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);

                            if (mod.currentArea.useMiniProps)
                            {
                                dst.Top += (int)(gv.squareSize * 1 / 8);
                                if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                {
                                    dst.Left += (int)(gv.squareSize / 4);
                                }
                                else
                                {
                                    dst.Left -= (int)(gv.squareSize / 4);
                                }
                                dst.Height -= (int)(dst.Height / 2);
                                dst.Width -= (int)(dst.Width / 2);
                            }

                            else if (mod.currentArea.useSuperTinyProps)
                            {
                                dst.Top += (int)(gv.squareSize * 1 / 8);
                                if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                {
                                    dst.Left += (int)(gv.squareSize * 3 / 8);
                                }
                                else
                                {
                                    dst.Left -= (int)(gv.squareSize * 3 / 8);
                                }
                                dst.Height -= (int)(dst.Height * 3 / 4);
                                dst.Width -= (int)(dst.Width * 3 / 4);
                            }

                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
                        }
                        if ((i == 2) && (i != mod.selectedPartyLeader))
                        {
                            if (mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x + gv.oXshift - (shift) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }

                            if (mod.currentArea.useMiniProps)
                            {
                                    dst.Top += (int)(gv.squareSize * 1 / 8);
                                    if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                    {
                                        dst.Left += (int)(gv.squareSize / 4);
                                    }
                                    else
                                    {
                                        dst.Left -= (int)(gv.squareSize / 4);
                                    }
                                    dst.Height -= (int)(dst.Height / 2);
                                    dst.Width -= (int)(dst.Width / 2);
                                }
                            else if (mod.currentArea.useSuperTinyProps)
                            {
                                dst.Top += (int)(gv.squareSize * 1 / 8);
                                if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                {
                                    dst.Left += (int)(gv.squareSize * 3 / 8);
                                }
                                else
                                {
                                    dst.Left -= (int)(gv.squareSize * 3 / 8);
                                }
                                dst.Height -= (int)(dst.Height * 3 / 4);
                                dst.Width -= (int)(dst.Width * 3 / 4);
                            }

                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
                        }
                        if ((i == 3) && (i != mod.selectedPartyLeader))
                        {

                            if (mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 2)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }

                            if (mod.currentArea.useMiniProps)
                            {
                                    dst.Top += (int)(gv.squareSize * 1 / 8);
                                    if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                    {
                                        dst.Left += (int)(gv.squareSize / 4);
                                    }
                                    else
                                    {
                                        dst.Left -= (int)(gv.squareSize / 4);
                                    }
                                    dst.Height -= (int)(dst.Height / 2);
                                    dst.Width -= (int)(dst.Width / 2);
                                }
                            else if (mod.currentArea.useSuperTinyProps)
                            {
                                dst.Top += (int)(gv.squareSize * 1 / 8);
                                if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                {
                                    dst.Left += (int)(gv.squareSize * 3 / 8);
                                }
                                else
                                {
                                    dst.Left -= (int)(gv.squareSize * 3 / 8);
                                }
                                dst.Height -= (int)(dst.Height * 3 / 4);
                                dst.Width -= (int)(dst.Width * 3 / 4);
                            }

                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
                        }
                        if ((i == 4) && (i != mod.selectedPartyLeader))
                        {
                            if (mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 2)
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 3)
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 175 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }

                            if (mod.currentArea.useMiniProps)
                            {
                                    dst.Top += (int)(gv.squareSize * 1 / 8);
                                    if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                    {
                                        dst.Left += (int)(gv.squareSize / 4);
                                    }
                                    else
                                    {
                                        dst.Left -= (int)(gv.squareSize / 4);
                                    }
                                    dst.Height -= (int)(dst.Height / 2);
                                    dst.Width -= (int)(dst.Width / 2);
                                }
                            else if (mod.currentArea.useSuperTinyProps)
                            {
                                dst.Top += (int)(gv.squareSize * 1 / 8);
                                if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                {
                                    dst.Left += (int)(gv.squareSize * 3 / 8);
                                }
                                else
                                {
                                    dst.Left -= (int)(gv.squareSize * 3 / 8);
                                }
                                dst.Height -= (int)(dst.Height * 3 / 4);
                                dst.Width -= (int)(dst.Width * 3 / 4);
                            }

                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
                        }

                        if ((i == 5) && (i != mod.selectedPartyLeader))
                        {
                            if (mod.selectedPartyLeader == 0)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 1)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 2)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 3)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else if (mod.selectedPartyLeader == 4)
                            {
                                dst = new IbRect(x + gv.oXshift + (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }
                            else
                            {
                                dst = new IbRect(x + gv.oXshift - (shift * 250 / 100) + mapStartLocXinPixels, y + reducedSquareSize * 47 / 100, reducedSquareSize, reducedSquareSize);
                            }

                            if (mod.currentArea.useMiniProps)
                            {
                                    dst.Top += (int)(gv.squareSize * 1 / 8);
                                    if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                    {
                                        dst.Left += (int)(gv.squareSize / 4);
                                    }
                                    else
                                    {
                                        dst.Left -= (int)(gv.squareSize / 4);
                                    }
                                    dst.Height -= (int)(dst.Height / 2);
                                    dst.Width -= (int)(dst.Width / 2);
                                }
                            else if (mod.currentArea.useSuperTinyProps)
                            {
                                dst.Top += (int)(gv.squareSize * 1 / 8);
                                if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                                {
                                    dst.Left += (int)(gv.squareSize * 3 / 8);
                                }
                                else
                                {
                                    dst.Left -= (int)(gv.squareSize * 3 / 8);
                                }
                                dst.Height -= (int)(dst.Height * 3 / 4);
                                dst.Width -= (int)(dst.Width * 3 / 4);
                            }

                            gv.DrawBitmap(mod.playerList[i].token, src, dst, !mod.playerList[i].combatFacingLeft, false);
                        }
                    }
                    //gv.squareSize = gv.squareSize * 3 / 2;

                    if (mod.playerList[0].combatFacingLeft == true)
                    {
                        gv.oXshift = gv.oXshift - shift / 2;
                    }
                    else
                    {
                        gv.oXshift = gv.oXshift + shift / 2;
                        //if (mod.currentArea.useMiniProps)
                        //{
                        //gv.oXshift -= gv.squareSize;
                        //}
                        //else if (mod.currentArea.useSuperTinyProps)
                        //{

                        //}

                    }

                    //gv.oXshift = gv.oXshift + shift / 2;
                }
                //always draw party leader on top
                int storeShift = shift;
                shift = 0;
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
                    //gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
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
                    //gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
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
                    //gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
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
                    //gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
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
                    //gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
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
                    //gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
                }

                if (mod.currentArea.useMiniProps)
                {
                    dst.Top += (int)(gv.squareSize / 4);
                    if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                    {
                        dst.Left += (int)(gv.squareSize / 4);
                    }
                    else
                    {
                        dst.Left -= (int)(gv.squareSize / 4);
                    }
                    dst.Height -= (int)(dst.Height / 2);
                    dst.Width -= (int)(dst.Width / 2);
                }
                else if (mod.currentArea.useSuperTinyProps)
                {
                    dst.Top += (int)(gv.squareSize * 3 / 8);
                    if (mod.playerList[mod.selectedPartyLeader].combatFacingLeft == true)
                    {
                        dst.Left += (int)(gv.squareSize * 3 /8);
                    }
                    else
                    {
                        dst.Left -= (int)(gv.squareSize * 3 / 8);
                    }
                    dst.Height -= (int)(dst.Height * 3 / 4);
                    dst.Width -= (int)(dst.Width * 3 / 4);
                }
                gv.DrawBitmap(mod.playerList[mod.selectedPartyLeader].token, src, dst, !mod.playerList[mod.selectedPartyLeader].combatFacingLeft, false);
                shift = storeShift;
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

            for (int x = minX; x < maxX; x++)
            {
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

        //not used for now; later :-)
        /*public void drawOverlayWeather()
        {
            //memo to self: in second step do animation by drawing two partial rectangles of same source that change size with time, upper and lower rect, and cast to same target dst, but shifted
            //the source picture must be identical top and bottom lines, other wise we will see a clear dividing line
            //idea that one source bitmap can be used all itself to simulate scrolling down if called in shifting chunks
            //part that scrolls out of lower screen border appears again at top screen border
            //second memo to self: in game settings implement several speed settings for animation speed (pixel move per call multiplier) so that players can adjust prop anim and weatehr anim speed themselves
            //third memo to self: descripe current weather type next to current time in the game ui
            IbRect src = new IbRect(0, 0, gv.cc.tint_rain.PixelSize.Width, gv.cc.tint_rain.PixelSize.Height);
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

        }*/
        
        public void drawMainMapClockText()
        {
            int timeofday = mod.WorldTime % (24 * 60);
            int hour = timeofday / 60;
            int minute = timeofday % 60;
            string sMinute = minute + "";
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

            for (int x = minX; x < maxX; x++)
            {
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
                drawColumnOfBlack(1);
            }
            if (mod.PlayerLocationX < 2)
            {
                drawColumnOfBlack(2);
            }
            if (mod.PlayerLocationX < 1)
            {
                drawColumnOfBlack(3);
            }
            //at top edge
            if (mod.PlayerLocationY < 4)
            {
                drawRowOfBlack(0);
            }
            if (mod.PlayerLocationY < 3)
            {
                drawRowOfBlack(1);
            }
            if (mod.PlayerLocationY < 2)
            {
                drawRowOfBlack(2);
            }
            if (mod.PlayerLocationY < 1)
            {
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
            }
            if (mod.PlayerLocationX > mod.currentArea.MapSizeX - 3)
            {
                drawColumnOfBlack(6);
            }
            if (mod.PlayerLocationX > mod.currentArea.MapSizeX - 2)
            {
                drawColumnOfBlack(5);
            }

            //at bottom edge
            if (mod.PlayerLocationY > mod.currentArea.MapSizeY - 5)
            {
                drawRowOfBlack(8);
            }
            if (mod.PlayerLocationY > mod.currentArea.MapSizeY - 4)
            {
                drawRowOfBlack(7);
            }
            if (mod.PlayerLocationY > mod.currentArea.MapSizeY - 3)
            {
                drawRowOfBlack(6);
            }
            if (mod.PlayerLocationY > mod.currentArea.MapSizeY - 2)
            {
                drawRowOfBlack(5);
            }            
        }
        public void drawPanels()
        {
            gv.cc.pnlLog.Draw();
            gv.cc.pnlToggles.Draw();
            gv.cc.pnlPortraits.Draw();
            gv.cc.pnlArrows.Draw();
            gv.cc.pnlHotkeys.Draw();
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
            //if (mod.currentArea.IsWorldMap)
            //{
                tglMiniMap.Draw();
            //}
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

                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            gv.DrawText(ft.value, new IbRect(xLoc + x + gv.oXshift + mapStartLocXinPixels, yLoc + y + txtH, gv.squareSize * 2, 1000), 0.8f, Color.Black);
                        }
                    }
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
                }
            }
        }

        public void drawFloatyTextByPixelPool()
        {
            if (floatyTextByPixelPool.Count > 0)
            {
                int txtH = (int)gv.drawFontRegHeight;
                int pH = (int)((float)gv.screenHeight / 200.0f);

                foreach (FloatyTextByPixel ft in floatyTextByPixelPool)
                {

                    int playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffset * gv.squareSize);
                    int playerPositionYInPix = gv.playerOffset * gv.squareSize + gv.oYshift;

                    float floatConvertedToSquareDistanceX = (ft.floatyCarrier2.currentPixelPositionX - playerPositionXInPix) / gv.squareSize;
                    int ConvertedToSquareDistanceX = (int)Math.Ceiling(floatConvertedToSquareDistanceX);

                    float floatConvertedToSquareDistanceY = (ft.floatyCarrier2.currentPixelPositionY - playerPositionYInPix) / gv.squareSize;
                    int ConvertedToSquareDistanceY = (int)Math.Ceiling(floatConvertedToSquareDistanceY);

                    int SquareThatPixIsOnX = mod.PlayerLocationX + ConvertedToSquareDistanceX;
                    int SquareThatPixIsOnY = mod.PlayerLocationY + ConvertedToSquareDistanceY;


                    if (gv.cc.getDistance(new Coordinate (SquareThatPixIsOnX, SquareThatPixIsOnY), new Coordinate(mod.PlayerLastLocationX, mod.PlayerLocationY)) > 3)
                    {
                        continue; //out of range from view so skip drawing floaty message
                    }

                    //location.X should be the the props actual map location in squares (not screen location)
                    int xLoc = (int)(ft.floatyCarrier2.currentPixelPositionX);
                    int yLoc = (int)(ft.floatyCarrier2.currentPixelPositionY) - (pH * ft.z);

                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            gv.DrawText(ft.value, new IbRect(xLoc + x, yLoc + y + txtH, gv.squareSize * 2, 1000), 0.8f, Color.Black);
                        }
                    }
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
                    gv.DrawText(ft.value, new IbRect(xLoc, yLoc + txtH, gv.squareSize * 2, 1000), 0.8f, colr);
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
                
        public void doFloatyTextLoop()
        {
            gv.postDelayed("doFloatyTextMainMap", 100);
        }
        public void doFloatyTextByPixelLoop()
        {
            gv.postDelayed("doFloatyTextMainMap", 100);
        }
        public void addFloatyText(int sqrX, int sqrY, String value, String color, int length)
        {
            floatyTextPool.Add(new FloatyText(sqrX, sqrY, value, color, length));
        }

        public void addFloatyText(Prop floatyCarrier, String value, String color, int length)
        {
            floatyTextByPixelPool.Add(new FloatyTextByPixel (floatyCarrier, value, color, length));
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

            switch (eventType)
            {
                case MouseEventType.EventType.MouseDown:
                case MouseEventType.EventType.MouseMove:
                    int x = (int)e.X;
                    int y = (int)e.Y;

                    //Draw Floaty Text On Mouse Over Prop
                    int gridx = (int)e.X / gv.squareSize;
                    int gridy = (int)e.Y / gv.squareSize;
                    int actualX = mod.PlayerLocationX + (gridx - gv.playerOffset);
                    int actualY = mod.PlayerLocationY + (gridy - gv.playerOffset);
                    gv.cc.floatyText = "";
                    if (IsTouchInMapWindow(gridx, gridy))
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
                    x = (int)e.X;
                    y = (int)e.Y;
                    int gridX = (int)e.X / gv.squareSize;
                    int gridY = (int)e.Y / gv.squareSize;
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
                            gv.cc.addLogText("yellow", "Hide info about interaction state of NPC and creatures (encounter = red, mandatory conversation = orange and optional conversation = green");
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
                            gv.cc.addLogText("yellow", "In a hurry: Party is avoiding all conversations that are not mandatory");
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
                            gv.cc.addLogText("lime", "Music Off, SoundFX Off");
                        }
                        else
                        {
                            gv.cc.tglSound.toggleOn = true;
                            mod.playMusic = true;
                            mod.playSoundFx = true;
                            gv.screenCombat.tglSoundFx.toggleOn = true;
                            gv.startMusic();
                            gv.startAmbient();
                            gv.cc.addLogText("lime", "Music On, SoundFX On");
                        }
                    }
                    if (tglFullParty.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        if (tglFullParty.toggleOn)
                        {
                            tglFullParty.toggleOn = false;
                            gv.cc.addLogText("lime", "Show Party Leader");
                        }
                        else
                        {
                            tglFullParty.toggleOn = true;
                            gv.cc.addLogText("lime", "Show Full Party");
                        }
                    }
                    //if ((tglMiniMap.getImpact(x, y)) && (mod.currentArea.IsWorldMap))
                    if (tglMiniMap.getImpact(x, y))
                    {
                        //if (mod.playButtonSounds) {gv.playSoundEffect(android.view.SoundEffectConstants.CLICK);}
                        //if (mod.playButtonHaptic) {gv.performHapticFeedback(android.view.HapticFeedbackConstants.VIRTUAL_KEY);}
                        if (tglMiniMap.toggleOn)
                        {
                            tglMiniMap.toggleOn = false;
                            gv.cc.addLogText("lime", "Hide Mini Map");
                        }
                        else
                        {
                            tglMiniMap.toggleOn = true;
                            gv.cc.addLogText("lime", "Show Mini Map");
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
                        gv.screenInventory.resetInventory();
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
                                gv.errorLog(ex.ToString());
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
                                    gv.errorLog(ex.ToString());
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
                gv.screenInventory.resetInventory();
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
                        gv.errorLog(ex.ToString());
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
                            gv.errorLog(ex.ToString());
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
        public bool IsTouchInMapWindow(int sqrX, int sqrY)
        {
            //all input coordinates are in Screen Location, not Map Location
            if ((sqrX < 6) || (sqrY < 0))
            {
                return false;
            }
            if ((sqrX >= 15) || (sqrY >= 9))
            {
                return false;
            }
            return true;
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
                gv.cc.DisposeOfBitmap(ref btnParty.Img2);
                btnParty.Img2 = gv.cc.LoadBitmap("btnpartyplus");
            }
            else
            {
                gv.cc.DisposeOfBitmap(ref btnParty.Img2);
                btnParty.Img2 = gv.cc.LoadBitmap("btnparty");
            }
        }
    }
}
