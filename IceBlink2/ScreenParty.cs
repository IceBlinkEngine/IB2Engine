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
    public class ScreenParty
    {
        //public gv.module gv.mod;
        public GameView gv;
        private IbbHtmlTextBox description;
        private IbbHtmlTextBox attackAndDamageInfo;
         
        public List<IbbButton> btnPartyIndex = new List<IbbButton>();
        private IbbPortrait btnPortrait = null;
        private IbbButton btnToken = null;
        private IbbButton btnHead = null;
        private IbbButton btnNeck = null;
        private IbbButton btnGloves = null;
        private IbbButton btnBody = null;
        private IbbButton btnMainHand = null;
        private IbbButton btnOffHand = null;
        private IbbButton btnRing = null;
        private IbbButton btnRing2 = null;
        private IbbButton btnFeet = null;
        private IbbButton btnAmmo = null;
        private IbbButton btnHelp = null;
        private IbbButton btnInfo = null;
        private IbbButton btnReturn = null;
        private IbbButton btnLevelUp = null;
        private IbbButton btnPartyRoster = null;
        private IbbButton btnSpells = null;
        private IbbButton btnTraits = null;
        private IbbButton btnEffects = null;
        private IbbButton btnOthers = null;
        //private bool dialogOpen = false;
        public string traitGained = "";
        public string spellGained = "";


        public ScreenParty(Module m, GameView g)
        {
            //gv.mod = m;
            gv = g;
            setControlsStart();
            description = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            description.showBoxBorder = false;
            attackAndDamageInfo = new IbbHtmlTextBox(gv, 320, 100, 500, 300);
            attackAndDamageInfo.showBoxBorder = false;
        }

        public void setControlsStart()
        {
            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padW = gv.squareSize / 6;

            if (btnPortrait == null)
            {
                btnPortrait = new IbbPortrait(gv, 1.0f);
                btnPortrait.ImgBG = gv.cc.LoadBitmap("item_slot");
                btnPortrait.Glow = gv.cc.LoadBitmap("btn_small_glow");
                btnPortrait.X = 2 * gv.squareSize - (pW * 2);
                btnPortrait.Y = 1 * gv.squareSize + pH * 2;
                btnPortrait.Height = (int)(gv.ibpheight * gv.screenDensity);
                btnPortrait.Width = (int)(gv.ibpwidth * gv.screenDensity);
            }
            if (btnToken == null)
            {
                btnToken = new IbbButton(gv, 1.0f);
                btnToken.Img = gv.cc.LoadBitmap("item_slot");
                //btnToken.Img2 = gv.cc.LoadBitmap(pc.tokenFilename);
                btnToken.Glow = gv.cc.LoadBitmap("btn_small_glow");
                btnToken.X = 2 * gv.squareSize - (pW * 2);
                btnToken.Y = 3 * gv.squareSize + pH * 2;
                btnToken.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnToken.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnSpells == null)
            {
                btnSpells = new IbbButton(gv, 0.6f);
                if ((gv.mod.playerList.Count > 0) && (gv.cc.partyScreenPcIndex < gv.mod.playerList.Count))
                {
                    btnSpells.Text = gv.mod.playerList[gv.cc.partyScreenPcIndex].playerClass.spellLabelPlural.ToUpper();
                }
                else
                {
                    btnSpells.Text = gv.mod.spellLabelPlural.ToUpper();
                }
                btnSpells.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnSpells.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnSpells.X = 6 * gv.squareSize + padW * 1 + gv.oXshift + (int)(gv.squareSize * 0.75f);
                //btnSpells.Y = 10 * gv.squareSize + pH * 2; See OnDraw for Y
                btnSpells.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnSpells.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnTraits == null)
            {
                btnTraits = new IbbButton(gv, 0.6f);
                if ((gv.mod.playerList.Count > 0) && (gv.cc.partyScreenPcIndex < gv.mod.playerList.Count))
                {
                    btnTraits.Text = gv.mod.playerList[gv.cc.partyScreenPcIndex].playerClass.traitLabelPlural.ToUpper();
                }
                else
                {
                    btnTraits.Text = gv.mod.traitsLabelPlural.ToUpper();
                }
                //btnTraits.Text = gv.mod.playerList[gv.cc.partyScreenPcIndex].playerClass.traitLabelPlural.ToUpper();
                btnTraits.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnTraits.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnTraits.X = 7 * gv.squareSize + padW * 2 + gv.oXshift + (int)(gv.squareSize * 0.75f);
                //btnSpells.Y = 10 * gv.squareSize + pH * 2; See OnDraw for Y
                btnTraits.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnTraits.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnEffects == null)
            {
                btnEffects = new IbbButton(gv, 0.6f);
                btnEffects.Text = "EFFECTS";
                btnEffects.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnEffects.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnEffects.X = 8 * gv.squareSize + padW * 3 + gv.oXshift + (int)(gv.squareSize * 0.75f);
                //btnSpells.Y = 10 * gv.squareSize + pH * 2; See OnDraw for Y
                btnEffects.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnEffects.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnOthers == null)
            {
                btnOthers = new IbbButton(gv, 0.6f);
                btnOthers.Text = "OTHERS";
                btnOthers.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnOthers.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnOthers.X = 9 * gv.squareSize + padW * 4 + gv.oXshift + (int)(gv.squareSize * 0.75f);
                //btnSpells.Y = 10 * gv.squareSize + pH * 2; See OnDraw for Y
                btnOthers.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnOthers.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnPartyRoster == null)
            {
                btnPartyRoster = new IbbButton(gv, 0.6f);
                btnPartyRoster.Text = "ROSTER";
                btnPartyRoster.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnPartyRoster.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                ///btnPartyRoster.X = 13 * gv.squareSize;
                btnPartyRoster.X = (gv.screenWidth / 2) + (int)((gv.ibbwidthL / 2) * gv.screenDensity) + (int)(gv.squareSize * 0.5);
                btnPartyRoster.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPartyRoster.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnHelp == null)
            {
                btnHelp = new IbbButton(gv, 0.8f);
                btnHelp.Text = "HELP";
                btnHelp.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnHelp.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                //btnHelp.X = 5 * gv.squareSize + padW * 1 + gv.oXshift;
                btnHelp.Y = 9 * gv.squareSize + pH * 2;
                btnHelp.X = (gv.screenWidth / 2) - (int)((gv.ibbwidthL / 2) * gv.screenDensity) - (int)(gv.squareSize * 1.5);
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnInfo == null)
            {
                btnInfo = new IbbButton(gv, 0.8f);
                btnInfo.Text = "INFO";
                btnInfo.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small);
                btnInfo.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnInfo.X = 10 * gv.squareSize + (padW * 5) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                //btnInfo.X = (11 * gv.squareSize) - padW + (int)(gv.squareSize * 0.75f);
                //btnInfo.Y = 9 * gv.squareSize + pH * 2;
                //btnInfo.Y = 7 * gv.squareSize + (int)(pH * 2.5f);

                btnInfo.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInfo.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnReturn == null)
            {
                btnReturn = new IbbButton(gv, 1.2f);
                btnReturn.Text = "RETURN";
                btnReturn.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnReturn.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnReturn.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnReturn.Y = 9 * gv.squareSize + pH * 2;
                btnReturn.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturn.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnLevelUp == null)
            {
                btnLevelUp = new IbbButton(gv, 1.2f);
                btnLevelUp.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnLevelUp.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnLevelUp.Text = "Level Up";
                //btnLevelUp.X = 5 * gv.squareSize + padW * 1 + gv.oXshift;
                //btnLevelUp.Y = 8 * gv.squareSize - pH * 2;
                btnLevelUp.X = 10 * gv.squareSize + (padW * (7)) + gv.oXshift;
                btnLevelUp.Y = pH * 2; ;
                btnLevelUp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnLevelUp.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }

            if (btnMainHand == null)
            {
                btnMainHand = new IbbButton(gv, 1.0f);
                btnMainHand.Img = gv.cc.LoadBitmap("item_slot_mainhand"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_mainhand);
                btnMainHand.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnMainHand.X = 6 * gv.squareSize + (padW * 1) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                btnMainHand.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnMainHand.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnMainHand.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnHead == null)
            {
                btnHead = new IbbButton(gv, 1.0f);
                btnHead.Img = gv.cc.LoadBitmap("item_slot_head"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_head);
                btnHead.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnHead.X = 8 * gv.squareSize + (padW * 3) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                btnHead.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnHead.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHead.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnGloves == null)
            {
                btnGloves = new IbbButton(gv, 1.0f);
                btnGloves.Img = gv.cc.LoadBitmap("item_slot_gloves"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_neck);
                btnGloves.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);

                btnGloves.X = 9 * gv.squareSize + (padW * 4) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                //btnGloves.X = 8 * gv.squareSize + (padW * 5) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                btnGloves.Y = 2 * gv.squareSize + (padW * 2); //not used, see onDraw function

                //btnGloves.X = 10 * gv.squareSize + (padW * 5) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                //btnGloves.Y = 2 * gv.squareSize + padW; //not used, see onDraw function
                // btnGloves.X = 10 * gv.squareSize + (padW * 5) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                //btnInfo.X = (11 * gv.squareSize) - padW + (int)(gv.squareSize * 0.75f);
                //btnInfo.Y = 9 * gv.squareSize + pH * 2;
                //btnGloves.Y = 6 * gv.squareSize + (int)(pH * 2.5f);
                //btnGloves.X = 10 * gv.squareSize + (padW * 3) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                //btnGloves.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnGloves.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnGloves.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnNeck == null)
            {
                btnNeck = new IbbButton(gv, 1.0f);
                btnNeck.Img = gv.cc.LoadBitmap("item_slot_neck"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_neck);
                btnNeck.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnNeck.X = 7 * gv.squareSize + (padW * 2) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                btnNeck.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnNeck.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNeck.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnOffHand == null)
            {
                btnOffHand = new IbbButton(gv, 1.0f);
                btnOffHand.Img = gv.cc.LoadBitmap("item_slot_offhand"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_offhand);
                btnOffHand.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnOffHand.X = 9 * gv.squareSize + (padW * 4) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                btnOffHand.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnOffHand.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnOffHand.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnRing == null)
            {
                btnRing = new IbbButton(gv, 1.0f);
                btnRing.Img = gv.cc.LoadBitmap("item_slot_ring"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_ring);
                btnRing.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnRing.X = 6 * gv.squareSize + (padW * 1) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                btnRing.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnRing.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRing.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnBody == null)
            {
                btnBody = new IbbButton(gv, 1.0f);
                btnBody.Img = gv.cc.LoadBitmap("item_slot_body"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_body);
                btnBody.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnBody.X = 8 * gv.squareSize + (padW * 3) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                btnBody.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnBody.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnBody.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnFeet == null)
            {
                btnFeet = new IbbButton(gv, 1.0f);
                btnFeet.Img = gv.cc.LoadBitmap("item_slot_feet"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_feet);
                btnFeet.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnFeet.X = 7 * gv.squareSize + (padW * 2) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                btnFeet.Y = 1 * gv.squareSize; //not used, see onDraw function
                btnFeet.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnFeet.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnRing2 == null)
            {
                btnRing2 = new IbbButton(gv, 1.0f);
                btnRing2.Img = gv.cc.LoadBitmap("item_slot_ring"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_ring);
                btnRing2.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnRing2.X = 10 * gv.squareSize + (padW * 5) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                btnRing2.Y = 2 * gv.squareSize + padW; //not used, see onDraw function
                btnRing2.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRing2.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnAmmo == null)
            {
                btnAmmo = new IbbButton(gv, 1.0f);
                btnAmmo.Img = gv.cc.LoadBitmap("item_slot_ammo"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot_ring);
                btnAmmo.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnAmmo.X = 10 * gv.squareSize + (padW * 5) + gv.oXshift + (int)(gv.squareSize * 0.75f);
                btnAmmo.Y = 2 * gv.squareSize + padW; //not used, see onDraw function
                btnAmmo.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAmmo.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }

            for (int x = 0; x < 6; x++)
            {
                IbbButton btnNew = new IbbButton(gv, 1.0f);
                btnNew.Img = gv.cc.LoadBitmap("item_slot"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.item_slot);
                btnNew.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_small_glow);
                btnNew.X = ((x + 3) * gv.squareSize) + (padW * (x + 1)) + gv.oXshift;
                btnNew.Y = pH * 2;
                btnNew.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnNew.Width = (int)(gv.ibbwidthR * gv.screenDensity);

                btnPartyIndex.Add(btnNew);
            }
        }

        public void resetPartyScreen()
        {
            int cntPCs = 0;
            foreach (IbbButton btn in btnPartyIndex)
            {
                if (cntPCs < gv.mod.playerList.Count)
                {
                    if (!gv.mod.playerList[cntPCs].isTemporaryAllyForThisEncounterOnly)
                    {
                        gv.cc.DisposeOfBitmap(ref btn.Img2);
                        btn.Img2 = gv.cc.LoadBitmap(gv.mod.playerList[cntPCs].tokenFilename);
                    }
                }
                cntPCs++;
            }
            resetTokenAndPortrait();
        }
        public void resetTokenAndPortrait()
        {
            btnToken.Img2 = gv.mod.playerList[gv.cc.partyScreenPcIndex].token;
            btnPortrait.Img = gv.mod.playerList[gv.cc.partyScreenPcIndex].portrait;
        }

        public void redrawParty()
        {
            if (gv.cc.partyScreenPcIndex >= gv.mod.playerList.Count)
            {
                int i = 0;
                while (gv.mod.playerList[i].isTemporaryAllyForThisEncounterOnly)
                {
                    i++;
                }
                gv.cc.partyScreenPcIndex = i;
            }
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
            gv.sf.UpdateStats(pc);
            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padH = gv.squareSize / 6;
            float locY = 0;
            int locX = 3 * gv.squareSize + (pW * 1) + gv.oXshift;
            int tabX = 7 * gv.squareSize + (pW * 0) - (3 * gv.oXshift);
            int tabX2 = 10 * gv.squareSize + (pW * 0) - gv.oXshift;
            //int textH = (int)gv.cc.MeasureString("GetHeight", gv.drawFontReg, gv.Width).Height;
            float textH = gv.drawFontRegHeight;
            float spacing = textH + pH;
            int leftStartY = btnPartyIndex[0].Y + btnPartyIndex[0].Height + (pH * 2);

            //DRAW EACH PC BUTTON
            int cntPCs = 0;
            foreach (IbbButton btn in btnPartyIndex)
            {
                if (cntPCs < gv.mod.playerList.Count)
                {
                    if (cntPCs == gv.cc.partyScreenPcIndex) { btn.glowOn = true; }
                    else { btn.glowOn = false; }
                    btn.Draw();
                }
                cntPCs++;
            }
            //DRAW TOKEN AND PORTRAIT
            btnPortrait.Draw();
            btnToken.Draw();

            //DRAW LEFT STATS
            //name            
            gv.DrawText(pc.name, locX, locY += leftStartY, 1.0f, Color.White);

            //race
            gv.DrawText(gv.mod.raceLabel +": " + gv.mod.getRace(pc.raceTag).name, locX, locY += spacing, 1.0f, Color.White);

            //gender
            if (pc.isMale)
            {
                gv.DrawText("Gender: Male", locX, locY += spacing, 1.0f, Color.White);
            }
            else
            {
                gv.DrawText("Gender: Female", locX, locY += spacing, 1.0f, Color.White);
            }

            //class
            gv.DrawText("Class: " + gv.mod.getPlayerClass(pc.classTag).name, locX, locY += spacing, 1.0f, Color.White);
            gv.DrawText("Level: " + pc.classLevel, locX, locY += spacing, 1.0f, Color.White);
            gv.DrawText("XP: " + pc.XP + "/" + pc.XPNeeded, locX, locY += spacing, 1.0f, Color.White);
            //gv.DrawText("---------------", locX, locY += spacing, 1.0f, Color.White);

            //LOCATE STATS INFO BUTTONS
            locY += spacing;
                        
            int bottomLocY = 7 * gv.squareSize + pH * 5;
            btnSpells.Y = (int)bottomLocY;
            btnTraits.Y = (int)bottomLocY;
            btnEffects.Y = (int)bottomLocY;
            btnOthers.Y = (int)bottomLocY;
            btnInfo.Y = (int)bottomLocY;
            btnPartyRoster.Y = 9 * gv.squareSize + pH * 2;

            //LOCATE EQUIPMENT SLOTS
            int startSlotsY = (int)locY + 2 * gv.squareSize + padH + (pH * 2) - gv.squareSize;
            btnHead.Y = startSlotsY;
            btnNeck.Y = startSlotsY;
            //btnGloves.Y = startSlotsY;
            btnMainHand.Y = startSlotsY;
            btnOffHand.Y = startSlotsY;
            btnAmmo.Y = startSlotsY;
            int startSlotsY2 = startSlotsY + gv.squareSize + padH + (pH * 2);
            btnRing.Y = startSlotsY2;
            btnRing2.Y = startSlotsY2;
            btnBody.Y = startSlotsY2;
            btnFeet.Y = startSlotsY2;
            btnGloves.Y = startSlotsY2;

            //DRAW RIGHT STATS
            int actext = 0;
            //float locY2 = 4 * gv.squareSize + gv.squareSize / 4;
            if (gv.mod.ArmorClassAscending) { actext = pc.AC; }
            else { actext = 20 - pc.AC; }
            locY = 0;
            gv.DrawText("STR:  " + pc.baseStr + " + " + (pc.strength - pc.baseStr) + " = " + pc.strength + " (" + ((pc.strength - 10) / 2) + ")", tabX, locY += leftStartY);
            gv.DrawText("AC: " + actext, tabX2, locY);
            //gv.DrawText("BAB: " + pc.baseAttBonus + ", Melee to hit/damage: " + (pc.baseAttBonus + ((pc.strength - 10) / 2)) + "/" + ((pc.strength - 10) / 2) + ", Ranged to hit: " + (pc.baseAttBonus + ((pc.dexterity - 10) / 2)), tabX2, locY += spacing);
            gv.DrawText("DEX: " + pc.baseDex + " + " + (pc.dexterity - pc.baseDex) + " = " + pc.dexterity + " (" + ((pc.dexterity - 10) / 2) + ")", tabX, locY += spacing);
            gv.DrawText("HP: " + pc.hp + "/" + pc.hpMax, tabX2, locY);
            gv.DrawText("CON: " + pc.baseCon + " + " + (pc.constitution - pc.baseCon) + " = " + pc.constitution + " (" + ((pc.constitution - 10) / 2) + ")", tabX, locY += spacing);
            gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, tabX2, locY);
            //gv.DrawText("BAB: " + pc.baseAttBonus + ", Melee to hit/damage: " + (pc.baseAttBonus + ((pc.strength - 10) / 2)) + "/" + ((pc.strength - 10) / 2) + ", Ranged to hit: " + (pc.baseAttBonus + ((pc.dexterity - 10) / 2)), tabX2, locY += spacing);
            gv.DrawText("BAB: " + pc.baseAttBonus + ", Melee to hit/damage: " + (pc.baseAttBonus + gv.sf.CalcPcMeleeAttackAttributeModifier(pc)) + "/" + (((pc.strength - 10) / 2) + gv.sf.CalcPcMeleeDamageModifier(pc)) + ", Ranged to hit: " + (pc.baseAttBonus + ((pc.dexterity - 10) / 2) + gv.sf.CalcPcRangedAttackModifier(pc)), tabX2, locY += spacing);
            //gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, tabX2, locY);
            gv.DrawText("INT:  " + pc.baseInt + " + " + (pc.intelligence - pc.baseInt) + " = " + pc.intelligence + " (" + ((pc.intelligence - 10) / 2) + ")", tabX, locY);
            gv.DrawText("FORT: " + pc.fortitude + ", Acid: " + pc.damageTypeResistanceTotalAcid + "%" + ", Cold: " + pc.damageTypeResistanceTotalCold + "%" + ", Normal: " + pc.damageTypeResistanceTotalNormal + "%", tabX2, locY += spacing);
            gv.DrawText("REF:   " + pc.reflex + ", Electricity: " + pc.damageTypeResistanceTotalElectricity + "%" + ", Fire: " + pc.damageTypeResistanceTotalFire + "%", tabX2, locY += spacing);
            gv.DrawText("WILL: " + pc.will + ", Magic: " + pc.damageTypeResistanceTotalMagic + "%" + ", Poison: " + pc.damageTypeResistanceTotalPoison + "%", tabX2, locY += spacing);
            gv.DrawText("WIS:  " + pc.baseWis + " + " + (pc.wisdom - pc.baseWis) + " = " + pc.wisdom + " (" + ((pc.wisdom - 10) / 2) + ")", tabX, locY -= (spacing * 2));
            gv.DrawText("CHA: " + pc.baseCha + " + " + (pc.charisma - pc.baseCha) + " = " + pc.charisma + " (" + ((pc.charisma - 10) / 2) + ")", tabX, locY += spacing);
            if (gv.mod.useLuck == true)
            {
                gv.DrawText("LCK:  " + pc.baseLuck + " + " + (pc.luck - pc.baseLuck) + " = " + pc.luck, tabX, locY += spacing);
            }

            //DRAW LEVEL UP BUTTON
            if (gv.mod.playerList[gv.cc.partyScreenPcIndex].IsReadyToAdvanceLevel())
            {
                btnLevelUp.Draw();
            }

            if (gv.cc.partyItemSlotIndex == 0) { btnMainHand.glowOn = true; }
            else { btnMainHand.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 1) { btnHead.glowOn = true; }
            else { btnHead.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 2) { btnGloves.glowOn = true; }
            else { btnGloves.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 2) { btnNeck.glowOn = true; }
            else { btnNeck.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 3) { btnOffHand.glowOn = true; }
            else { btnOffHand.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 4) { btnRing.glowOn = true; }
            else { btnRing.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 5) { btnBody.glowOn = true; }
            else { btnBody.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 6) { btnFeet.glowOn = true; }
            else { btnFeet.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 7) { btnRing2.glowOn = true; }
            else { btnRing2.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 8) { btnAmmo.glowOn = true; }
            else { btnAmmo.glowOn = false; }
            if (gv.cc.partyItemSlotIndex == 9) { btnGloves.glowOn = true; }
            else { btnGloves.glowOn = false; }

            gv.cc.DisposeOfBitmap(ref btnMainHand.Img2);
            btnMainHand.Img2 = gv.cc.LoadBitmap(gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).itemImage);
            gv.cc.DisposeOfBitmap(ref btnHead.Img2);
            btnHead.Img2 = gv.cc.LoadBitmap(gv.mod.getItemByResRefForInfo(pc.HeadRefs.resref).itemImage);
            gv.cc.DisposeOfBitmap(ref btnGloves.Img2);
            btnGloves.Img2 = gv.cc.LoadBitmap(gv.mod.getItemByResRefForInfo(pc.GlovesRefs.resref).itemImage);
            gv.cc.DisposeOfBitmap(ref btnNeck.Img2);
            btnNeck.Img2 = gv.cc.LoadBitmap(gv.mod.getItemByResRefForInfo(pc.NeckRefs.resref).itemImage);
            gv.cc.DisposeOfBitmap(ref btnOffHand.Img2);
            btnOffHand.Img2 = gv.cc.LoadBitmap(gv.mod.getItemByResRefForInfo(pc.OffHandRefs.resref).itemImage);
            gv.cc.DisposeOfBitmap(ref btnRing.Img2);
            btnRing.Img2 = gv.cc.LoadBitmap(gv.mod.getItemByResRefForInfo(pc.RingRefs.resref).itemImage);
            gv.cc.DisposeOfBitmap(ref btnBody.Img2);
            btnBody.Img2 = gv.cc.LoadBitmap(gv.mod.getItemByResRefForInfo(pc.BodyRefs.resref).itemImage);
            gv.cc.DisposeOfBitmap(ref btnFeet.Img2);
            btnFeet.Img2 = gv.cc.LoadBitmap(gv.mod.getItemByResRefForInfo(pc.FeetRefs.resref).itemImage);
            gv.cc.DisposeOfBitmap(ref btnRing2.Img2);
            btnRing2.Img2 = gv.cc.LoadBitmap(gv.mod.getItemByResRefForInfo(pc.Ring2Refs.resref).itemImage);
            gv.cc.DisposeOfBitmap(ref btnAmmo.Img2);
            btnAmmo.Img2 = gv.cc.LoadBitmap(gv.mod.getItemByResRefForInfo(pc.AmmoRefs.resref).itemImage);

            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            /*
            if ((it.onUseItemCastSpellTag == "none" || it.onUseItemCastSpellTag == "") && (it.onUseItemIBScript == "none" || it.onUseItemIBScript == "") && (it.onUseItem == "none" || it.onUseItem == ""))
            {
                if (itr.quantity > 1)
                {
                    btnInventorySlot[i].Quantity = itr.quantity + "";
                }
                else
                {
                    btnInventorySlot[i].Quantity = "";
                }
            }
            else if (itr.quantity != 1)
            {
                if (itr.quantity > 1)
                {
                    btnInventorySlot[i].Quantity = (itr.quantity - 1) + "";
                }
                else
                {
                    btnInventorySlot[i].Quantity = "0";
                }
            }
            */
            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            //bockauf
            //Ammo
            ItemRefs itr = gv.mod.getItemRefsInInventoryByResRef(pc.AmmoRefs.resref);
          
            if (itr != null)
            {
                //Item itQ = gv.mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                btnAmmo.Quantity = itr.quantity + "";
            }
            else
            {
                btnAmmo.Quantity = "";
            }

            //MainHand
            itr = pc.MainHandRefs;
            if (itr != null)
            {
                Item itQ = gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
                if ((itQ.onUseItemCastSpellTag == "none" || itQ.onUseItemCastSpellTag == "") && (itQ.onUseItemIBScript == "none" || itQ.onUseItemIBScript == "") && (itQ.onUseItem == "none" || itQ.onUseItem == ""))
                {
                    if (itr.quantity > 1)
                    {
                        btnMainHand.Quantity = itr.quantity + "";
                        btnMainHand.btnOfChargedItem = false;
                    }
                    else
                    {
                        btnMainHand.Quantity = "";
                        btnMainHand.btnOfChargedItem = false;
                    }
                }
                else if (itr.quantity != 1)
                {
                    if (itr.quantity > 1)
                    {
                        btnMainHand.Quantity = (itr.quantity - 1) + "";
                        if (!itQ.isStackable)
                        {
                            btnMainHand.btnOfChargedItem = true;
                        }
                        //eg potion
                        else
                        {
                            btnMainHand.btnOfChargedItem = false;
                        }
                    }
                    else
                    {
                        btnMainHand.Quantity = "0";
                        btnMainHand.btnOfChargedItem = true;
                    }
                }

                else
                {
                    btnMainHand.Quantity = "";
                    btnMainHand.btnOfChargedItem = false;
                }
            }


            //offhand, todo
            //MainHand
            itr = pc.OffHandRefs;
            if (itr != null)
            {
                Item itQ = gv.mod.getItemByResRefForInfo(pc.OffHandRefs.resref);
                if ((itQ.onUseItemCastSpellTag == "none" || itQ.onUseItemCastSpellTag == "") && (itQ.onUseItemIBScript == "none" || itQ.onUseItemIBScript == "") && (itQ.onUseItem == "none" || itQ.onUseItem == ""))
                {
                    if (itr.quantity > 1)
                    {
                        btnOffHand.Quantity = itr.quantity + "";
                        btnOffHand.btnOfChargedItem = false;
                    }
                    else
                    {
                        btnOffHand.Quantity = "";
                        btnOffHand.btnOfChargedItem = false;
                    }
                }
                else if (itr.quantity != 1)
                {
                    if (itr.quantity > 1)
                    {
                        btnOffHand.Quantity = (itr.quantity - 1) + "";
                        if (!itQ.isStackable)
                        {
                            btnOffHand.btnOfChargedItem = true;
                        }
                        //eg potion
                        else
                        {
                            btnOffHand.btnOfChargedItem = false;
                        }
                    }
                    else
                    {
                        btnOffHand.Quantity = "0";
                        btnOffHand.btnOfChargedItem = true;
                    }
                }
                else
                {
                    btnOffHand.Quantity = "";
                    btnOffHand.btnOfChargedItem = false;
                }
            }

            //Head
            itr =  pc.HeadRefs;
            if (itr != null)
            {
                Item itQ = gv.mod.getItemByResRefForInfo(pc.HeadRefs.resref);
                if ((itQ.onUseItemCastSpellTag == "none" || itQ.onUseItemCastSpellTag == "") && (itQ.onUseItemIBScript == "none" || itQ.onUseItemIBScript == "") && (itQ.onUseItem == "none" || itQ.onUseItem == ""))
                {
                    if (itr.quantity > 1)
                    {
                        btnHead.Quantity = itr.quantity + "";
                        btnHead.btnOfChargedItem = false;
                    }
                    else
                    {
                        btnHead.Quantity = "";
                        btnHead.btnOfChargedItem = false;
                    }
                }
                else if (itr.quantity != 1)
                {
                    if (itr.quantity > 1)
                    {
                        btnHead.Quantity = (itr.quantity - 1) + "";
                        if (!itQ.isStackable)
                        {
                            btnHead.btnOfChargedItem = true;
                        }
                        //eg potion
                        else
                        {
                            btnHead.btnOfChargedItem = false;
                        }
                    }
                    else
                    {
                        btnHead.Quantity = "0";
                        btnHead.btnOfChargedItem = true;
                    }
                }

                else
                {
                    btnHead.Quantity = "";
                    btnHead.btnOfChargedItem = false;
                }
            }

            //Body
            itr = pc.BodyRefs;
            if (itr != null)
            {
                Item itQ = gv.mod.getItemByResRefForInfo(pc.BodyRefs.resref);
                if ((itQ.onUseItemCastSpellTag == "none" || itQ.onUseItemCastSpellTag == "") && (itQ.onUseItemIBScript == "none" || itQ.onUseItemIBScript == "") && (itQ.onUseItem == "none" || itQ.onUseItem == ""))
                {
                    if (itr.quantity > 1)
                    {
                        btnBody.Quantity = itr.quantity + "";
                        btnBody.btnOfChargedItem = false;
                    }
                    else
                    {
                        btnBody.Quantity = "";
                        btnBody.btnOfChargedItem = false;
                    }
                }
                else if (itr.quantity != 1)
                {
                    if (itr.quantity > 1)
                    {
                        btnBody.Quantity = (itr.quantity - 1) + "";
                        if (!itQ.isStackable)
                        {
                            btnBody.btnOfChargedItem = true;
                        }
                        //eg potion
                        else
                        {
                            btnBody.btnOfChargedItem = false;
                        }
                    }
                    else
                    {
                        btnBody.Quantity = "0";
                        btnBody.btnOfChargedItem = true;
                    }
                }

                else
                {
                    btnBody.Quantity = "";
                    btnBody.btnOfChargedItem = false;
                }
            }

            //Gloves
            itr = pc.GlovesRefs;
            if (itr != null)
            {
                Item itQ = gv.mod.getItemByResRefForInfo(pc.GlovesRefs.resref);
                if ((itQ.onUseItemCastSpellTag == "none" || itQ.onUseItemCastSpellTag == "") && (itQ.onUseItemIBScript == "none" || itQ.onUseItemIBScript == "") && (itQ.onUseItem == "none" || itQ.onUseItem == ""))
                {
                    if (itr.quantity > 1)
                    {
                        btnGloves.Quantity = itr.quantity + "";
                        btnGloves.btnOfChargedItem = false;
                    }
                    else
                    {
                        btnGloves.Quantity = "";
                        btnGloves.btnOfChargedItem = false;
                    }
                }
                else if (itr.quantity != 1)
                {
                    if (itr.quantity > 1)
                    {
                        btnGloves.Quantity = (itr.quantity - 1) + "";
                        if (!itQ.isStackable)
                        {
                            btnGloves.btnOfChargedItem = true;
                        }
                        //eg potion
                        else
                        {
                            btnGloves.btnOfChargedItem = false;
                        }
                    }
                    else
                    {
                        btnGloves.Quantity = "0";
                        btnGloves.btnOfChargedItem = true;
                    }
                }

                else
                {
                    btnGloves.Quantity = "";
                    btnGloves.btnOfChargedItem = false;
                }
            }

            //Neck
            itr = pc.NeckRefs;
            if (itr != null)
            {
                Item itQ = gv.mod.getItemByResRefForInfo(pc.NeckRefs.resref);
                if ((itQ.onUseItemCastSpellTag == "none" || itQ.onUseItemCastSpellTag == "") && (itQ.onUseItemIBScript == "none" || itQ.onUseItemIBScript == "") && (itQ.onUseItem == "none" || itQ.onUseItem == ""))
                {
                    if (itr.quantity > 1)
                    {
                        btnNeck.Quantity = itr.quantity + "";
                        btnNeck.btnOfChargedItem = false;
                    }
                    else
                    {
                        btnNeck.Quantity = "";
                        btnNeck.btnOfChargedItem = false;
                    }
                }
                else if (itr.quantity != 1)
                {
                    if (itr.quantity > 1)
                    {
                        btnNeck.Quantity = (itr.quantity - 1) + "";
                        if (!itQ.isStackable)
                        {
                            btnNeck.btnOfChargedItem = true;
                        }
                        //eg potion
                        else
                        {
                            btnNeck.btnOfChargedItem = false;
                        }
                    }
                    else
                    {
                        btnNeck.Quantity = "0";
                        btnNeck.btnOfChargedItem = true;
                    }
                }

                else
                {
                    btnNeck.Quantity = "";
                    btnNeck.btnOfChargedItem = false;
                }
            }

            //Feet
            itr = pc.FeetRefs;
            if (itr != null)
            {
                Item itQ = gv.mod.getItemByResRefForInfo(pc.FeetRefs.resref);
                if ((itQ.onUseItemCastSpellTag == "none" || itQ.onUseItemCastSpellTag == "") && (itQ.onUseItemIBScript == "none" || itQ.onUseItemIBScript == "") && (itQ.onUseItem == "none" || itQ.onUseItem == ""))
                {
                    if (itr.quantity > 1)
                    {
                        btnFeet.Quantity = itr.quantity + "";
                        btnFeet.btnOfChargedItem = false;
                    }
                    else
                    {
                        btnFeet.Quantity = "";
                        btnFeet.btnOfChargedItem = false;
                    }
                }
                else if (itr.quantity != 1)
                {
                    if (itr.quantity > 1)
                    {
                        btnFeet.Quantity = (itr.quantity - 1) + "";
                        if (!itQ.isStackable)
                        {
                            btnFeet.btnOfChargedItem = true;
                        }
                        //eg potion
                        else
                        {
                            btnFeet.btnOfChargedItem = false;
                        }
                    }
                    else
                    {
                        btnFeet.Quantity = "0";
                        btnFeet.btnOfChargedItem = true;
                    }
                }

                else
                {
                    btnFeet.Quantity = "";
                    btnFeet.btnOfChargedItem = false;
                }
            }

            //Ring1
            itr = pc.RingRefs;
            if (itr != null)
            {
                Item itQ = gv.mod.getItemByResRefForInfo(pc.RingRefs.resref);
                if ((itQ.onUseItemCastSpellTag == "none" || itQ.onUseItemCastSpellTag == "") && (itQ.onUseItemIBScript == "none" || itQ.onUseItemIBScript == "") && (itQ.onUseItem == "none" || itQ.onUseItem == ""))
                {
                    if (itr.quantity > 1)
                    {
                        btnRing.Quantity = itr.quantity + "";
                        btnRing.btnOfChargedItem = false;
                    }
                    else
                    {
                        btnRing.Quantity = "";
                        btnRing.btnOfChargedItem = false;
                    }
                }
                else if (itr.quantity != 1)
                {
                    if (itr.quantity > 1)
                    {
                        btnRing.Quantity = (itr.quantity - 1) + "";
                        if (!itQ.isStackable)
                        {
                            btnRing.btnOfChargedItem = true;
                        }
                        //eg potion
                        else
                        {
                            btnRing.btnOfChargedItem = false;
                        }
                    }
                    else
                    {
                        btnRing.Quantity = "0";
                        btnRing.btnOfChargedItem = true;
                    }
                }

                else
                {
                    btnRing.Quantity = "";
                    btnRing.btnOfChargedItem = false;
                }
            }

            //Ring2
            itr = pc.Ring2Refs;
            if (itr != null)
            {
                Item itQ = gv.mod.getItemByResRefForInfo(pc.Ring2Refs.resref);
                if ((itQ.onUseItemCastSpellTag == "none" || itQ.onUseItemCastSpellTag == "") && (itQ.onUseItemIBScript == "none" || itQ.onUseItemIBScript == "") && (itQ.onUseItem == "none" || itQ.onUseItem == ""))
                {
                    if (itr.quantity > 1)
                    {
                        btnRing2.Quantity = itr.quantity + "";
                        btnRing2.btnOfChargedItem = false;
                    }
                    else
                    {
                        btnRing2.Quantity = "";
                        btnRing2.btnOfChargedItem = false;
                    }
                }
                else if (itr.quantity != 1)
                {
                    if (itr.quantity > 1)
                    {
                        btnRing2.Quantity = (itr.quantity - 1) + "";
                        if (!itQ.isStackable)
                        {
                            btnRing2.btnOfChargedItem = true;
                        }
                        //eg potion
                        else
                        {
                            btnRing2.btnOfChargedItem = false;
                        }
                    }
                    else
                    {
                        btnRing2.Quantity = "0";
                        btnRing2.btnOfChargedItem = true;

                    }
                }

                else
                {
                    btnRing2.Quantity = "";
                    btnRing2.btnOfChargedItem = false;
                }
            }

            btnMainHand.Draw();
            btnHead.Draw();
            btnGloves.Draw();
            btnNeck.Draw();
            btnOffHand.Draw();
            btnRing.Draw();
            btnBody.Draw();
            btnFeet.Draw();
            btnRing2.Draw();
            btnAmmo.Draw();
            btnSpells.Draw();
            btnTraits.Draw();
            btnEffects.Draw();
            btnOthers.Draw();
            if (gv.mod.hideRoster == false)
            {
                btnPartyRoster.Draw();
            }
            
            //DRAW DESCRIPTION BOX
            Item it = new Item();
            if (gv.cc.partyItemSlotIndex == 0) { it = gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 1) { it = gv.mod.getItemByResRefForInfo(pc.HeadRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 9) { it = gv.mod.getItemByResRefForInfo(pc.GlovesRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 2) { it = gv.mod.getItemByResRefForInfo(pc.NeckRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 3) { it = gv.mod.getItemByResRefForInfo(pc.OffHandRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 4) { it = gv.mod.getItemByResRefForInfo(pc.RingRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 5) { it = gv.mod.getItemByResRefForInfo(pc.BodyRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 6) { it = gv.mod.getItemByResRefForInfo(pc.FeetRefs.resref); }
            else if (gv.cc.partyItemSlotIndex == 7) { it = gv.mod.getItemByResRefForInfo(pc.Ring2Refs.resref); }
            else if (gv.cc.partyItemSlotIndex == 8) { it = gv.mod.getItemByResRefForInfo(pc.AmmoRefs.resref); }

            /*
            //Description
            string textToSpan = "";
            textToSpan = "<u>Description</u>" + "<BR>";
            textToSpan += "<b><i><big>" + it.name + "</big></i></b><BR>";
            if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
            {
                textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<BR>";
                textToSpan += "Attack Bonus: " + it.attackBonus + "<BR>";
                textToSpan += "Attack Range: " + it.attackRange + "<BR>";
                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
                textToSpan += "Tap 'INFO' for Full Description<BR>";
            }
            else if (!it.category.Equals("General"))
            {
                textToSpan += "AC Bonus: " + it.armorBonus + "<BR>";
                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";
                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
                textToSpan += "Tap 'INFO' for Full Description<BR>";
            }
            */
            string textToSpan = gv.cc.buildItemInfoText(it, 0);


            locY = btnBody.Y + btnBody.Height + (pH * 2);

            int xLoc = (11 * gv.squareSize) + (pW * 5) + gv.oXshift + (int)(gv.squareSize * 0.75f);
            int yLoc = startSlotsY - pH;
            int width = pW * 80;
            int height = pH * 50;
            DrawTextLayout(description, textToSpan, xLoc, yLoc, width, height);

            btnHelp.Draw();
            btnInfo.Draw();
            btnReturn.Draw();

            //Current attack and damage box

            //1. get number of attacks with melee or ranged (numAtt)
            int numAtt = 1;
            numAtt = gv.sf.CalcNumberOfAttacks(pc);
            if (numAtt < 1)
            {
                numAtt = 1;
            }
            /*
            if ((gv.sf.hasTrait(pc, "twoAttack")) && (gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee")))
            {
                numAtt = 2;
            }
            if ((gv.sf.hasTrait(pc, "rapidshot")) && (gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged")))
            {
                numAtt = 2;
            }
            if ((gv.sf.hasTrait(pc, "rapidshot2")) && (gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Ranged")))
            {
                numAtt = 3;
            }
            */

            //2. calculate attack modifier with current weapon (attackMod)
            int attackMod = 0;
            int modifier = 0;
            /*
            if ((gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee"))
                    || (gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).name.Equals("none"))
                    || (gv.mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))
                    */

            if (gv.sf.isMeleeAttack(pc))
            {
                /*
                modifier = (pc.strength - 10) / 2;
                //if has critical strike trait use dexterity for attack modifier in melee if greater than strength modifier
                if (pc.knownTraitsTags.Contains("criticalstrike"))
                {
                    int modifierDex = (pc.dexterity - 10) / 2;
                    if (modifierDex > modifier)
                    {
                        modifier = (pc.dexterity - 10) / 2;
                    }
                }
                */
                modifier = gv.sf.CalcPcMeleeAttackAttributeModifier(pc);
            }
            else //ranged weapon used
            {
                modifier = (pc.dexterity - 10) / 2;

                //if (gv.sf.hasTrait(pc, "preciseshot2"))
                int preciseShotAdder = 0;
                preciseShotAdder = gv.sf.CalcPcRangedAttackModifier(pc);
                if (preciseShotAdder > 0)
                {
                    modifier += preciseShotAdder;
                }

                //else
                //{
                    if (gv.sf.hasTrait(pc, "preciseshot2"))
                    {
                        modifier += 2;
                    }
                    else if (gv.sf.hasTrait(pc, "preciseshot"))
                    {
                        modifier++;
                    }
                //}
                Item it2 = gv.mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                if (it2 != null)
                {
                    modifier += gv.mod.getItemByResRefForInfo(pc.AmmoRefs.resref).attackBonus;
                }
            }

            attackMod = modifier + pc.baseAttBonus + gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attackBonus;

            //3. Calculate damage with current weapon (numberOfDiceRolled, typeOfDieRolled, dammodifier)  
            int numberOfDiceRolled = 0;
            int typeOfDieRolled = 0;
            int dammodifier = 0;
            string damageType = gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).typeOfDamage;

            if (gv.sf.isMeleeAttack(pc))
            {
                /*
                dammodifier = (pc.strength - 10) / 2;
                //if has critical strike trait use dexterity for damage modifier in melee if greater than strength modifier
                if (gv.sf.hasTrait(pc, "criticalstrike"))
                {
                    int dammodifierDex = (pc.dexterity - 10) / 4;
                    if (dammodifierDex > dammodifier)
                    {
                        dammodifier = (pc.dexterity - 10) / 2;
                    }
                }
                */
                dammodifier = gv.sf.CalcPcMeleeDamageAttributeModifier(pc);
            }
            else //ranged weapon used
            {
                dammodifier = 0;
                int preciseShotAdder = 0;
                preciseShotAdder = gv.sf.CalcPcRangedDamageModifier(pc);
                if (preciseShotAdder > 0)
                {
                    //dammodifier += 2;
                    dammodifier += preciseShotAdder;
                }
                else
                {
                    if (gv.sf.hasTrait(pc, "preciseshot2"))
                    {
                        dammodifier += 2;
                    }
                    else if (gv.sf.hasTrait(pc, "preciseshot"))
                    {
                        dammodifier++;
                    }

                }
                Item it3 = gv.mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
                if (it3 != null)
                {
                    dammodifier += gv.mod.getItemByResRefForInfo(pc.AmmoRefs.resref).damageAdder;
                    damageType = gv.mod.getItemByResRefForInfo(pc.AmmoRefs.resref).typeOfDamage;
                }

            }

            dammodifier += gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageAdder;
            numberOfDiceRolled = gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageNumDice;
            typeOfDieRolled = gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageDie;

            //4. Draw TextBox with info from abvoe about attack and damage
            //Description
            string textToSpan2 = "";
            textToSpan2 = "<b>Current Attack and Damage</b>" + "<BR>";
            textToSpan2 += "Number of attacks: " + numAtt + "<BR>";
            //Item it2 = gv.mod.getItemByResRefForInfo(pc.AmmoRefs.resref);
            if (gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attackRange > 1)
            {
                textToSpan2 += "Attack range: " + gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attackRange + "<BR>";
            }
            textToSpan2 += "Attack bonus: " + attackMod + "<BR>";
            if (numberOfDiceRolled > 0)
            {
                textToSpan2 += "Damage: " + numberOfDiceRolled + "d" + typeOfDieRolled + "+" + dammodifier + "<BR>";
            }
            else
            {
                textToSpan2 += "Damage: " + dammodifier + "<BR>";
            }
            textToSpan2 += "Damage type: " + damageType + "<BR>";

            /*
               locY = btnBody.Y + btnBody.Height + (pH * 2);

            int xLoc = (11 * gv.squareSize) + (pW * 5) + gv.oXshift + (int)(gv.squareSize * 0.75f);
            int yLoc = startSlotsY - pH;
            int width = pW * 80;
            int height = pH * 50;
            DrawTextLayout(description, textToSpan, xLoc, yLoc, width, height);

            */


            locY = btnBody.Y + btnBody.Height + (pH * 2);

            xLoc = (1 * gv.squareSize) + (pW * 5) + gv.oXshift + (int)(gv.squareSize * 0.75f);
            yLoc = startSlotsY - pH;
            width = pW * 80;
            height = pH * 50;
            DrawTextLayout(attackAndDamageInfo, textToSpan2, xLoc, yLoc, width, height);
            //antidings
        }
        public void DrawTextLayout(IbbHtmlTextBox tb, string text, int xLoc, int yLoc, int width, int height)
        {
            tb.tbXloc = xLoc;
            tb.tbYloc = yLoc;
            tb.tbWidth = width;
            tb.tbHeight = height;
            tb.logLinesList.Clear();
            tb.AddHtmlTextToLog(text);
            tb.onDrawLogBox();                       
        }
        public void onTouchParty(MouseEventArgs e, MouseEventType.EventType eventType, bool inCombat)
        {
            try
            {
                btnLevelUp.glowOn = false;
                btnPartyRoster.glowOn = false;
                btnHelp.glowOn = false;
                btnInfo.glowOn = false;
                btnReturn.glowOn = false;
                btnSpells.glowOn = false;
                btnTraits.glowOn = false;
                btnEffects.glowOn = false;
                btnOthers.glowOn = false;

                //int eventAction = event.getAction();
                switch (eventType)
                {
                    case MouseEventType.EventType.MouseDown:
                    case MouseEventType.EventType.MouseMove:
                        int x = (int)e.X;
                        int y = (int)e.Y;

                        if (btnLevelUp.getImpact(x, y))
                        {
                            btnLevelUp.glowOn = true;
                        }
                        else if (btnPartyRoster.getImpact(x, y))
                        {
                            if (gv.mod.hideRoster == false)
                            {
                                btnPartyRoster.glowOn = true;
                            }
                        }
                        else if (btnHelp.getImpact(x, y))
                        {
                            btnHelp.glowOn = true;
                        }
                        else if (btnInfo.getImpact(x, y))
                        {
                            btnInfo.glowOn = true;
                        }
                        else if (btnReturn.getImpact(x, y))
                        {
                            btnReturn.glowOn = true;
                        }
                        else if (btnSpells.getImpact(x, y))
                        {
                            btnSpells.glowOn = true;
                        }
                        else if (btnTraits.getImpact(x, y))
                        {
                            btnTraits.glowOn = true;
                        }
                        else if (btnEffects.getImpact(x, y))
                        {
                            btnEffects.glowOn = true;
                        }
                        else if (btnOthers.getImpact(x, y))
                        {
                            btnOthers.glowOn = true;
                        }
                        break;

                    case MouseEventType.EventType.MouseUp:
                        x = (int)e.X;
                        y = (int)e.Y;

                        btnLevelUp.glowOn = false;
                        btnPartyRoster.glowOn = false;
                        btnHelp.glowOn = false;
                        btnInfo.glowOn = false;
                        btnReturn.glowOn = false;
                        btnSpells.glowOn = false;
                        btnTraits.glowOn = false;
                        btnEffects.glowOn = false;
                        btnOthers.glowOn = false;

                        Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];

                        if (btnPortrait.getImpact(x, y))
                        {
                            if (!inCombat)
                            {
                                //pass items to selector
                                gv.screenType = "portraitSelector";
                                gv.screenPortraitSelector.resetPortraitSelector("party", pc);
                            }
                        }
                        else if (btnToken.getImpact(x, y))
                        {
                            if (!inCombat)
                            {
                                gv.screenType = "tokenSelector";
                                gv.screenTokenSelector.resetTokenSelector("party", pc);
                            }
                        }
                        else if (btnSpells.getImpact(x, y))
                        {
                            if (inCombat)
                            {
                                gv.screenSpellLevelUp.resetPC(true, pc, true);
                                gv.screenType = "learnSpellLevelUpCombat";
                            }
                            else
                            {
                                gv.screenSpellLevelUp.resetPC(true, pc, false);
                                gv.screenType = "learnSpellLevelUp";
                            }
                        }
                        else if (btnTraits.getImpact(x, y))
                        {
                            if (inCombat)
                            {
                                gv.screenTraitLevelUp.resetPC(true, pc);
                                gv.screenType = "learnTraitLevelUpCombat";
                            }
                            else
                            {
                                gv.screenTraitLevelUp.resetPC(true, pc);
                                gv.screenType = "learnTraitLevelUp";
                            }
                        }
                        else if (btnEffects.getImpact(x, y))
                        {
                            string allEffects = "";
                            foreach (Effect ef in pc.effectsList)
                            {
                                if (!ef.isPermanent)
                                {
                                    int left = ef.durationInUnits;
                                    allEffects += ef.name + " (" + left + ")" + "<br>";
                                }
                            }
                            gv.sf.MessageBoxHtml("<big><b>CURRENT EFFECTS</b></big><br><b><small>(#) denotes effect time left</small></b><br><br>" + allEffects);
                        }
                        else if (btnOthers.getImpact(x, y))
                        {
                            gv.sf.MessageBoxHtml("<big><b><u>SAVING THROW modifierS</u></b></big><br>" +
                                    "Fortitude: " + pc.fortitude + "<br>" +
                                    "Will: " + pc.will + "<br>" +
                                    "Reflex: " + pc.reflex + "<br><br>" +
                                    "<big><b><u>RESISTANCES (%)</u></b></big><br>" +
                                    "Acid: " + pc.damageTypeResistanceTotalAcid + "<br>" +
                                    "Cold: " + pc.damageTypeResistanceTotalCold + "<br>" +
                                    "Normal: " + pc.damageTypeResistanceTotalNormal + "<br>" +
                                    "Electricity: " + pc.damageTypeResistanceTotalElectricity + "<br>" +
                                    "Fire: " + pc.damageTypeResistanceTotalFire + "<br>" +
                                    "Magic: " + pc.damageTypeResistanceTotalMagic + "<br>" +
                                    "Poison: " + pc.damageTypeResistanceTotalPoison + "<br>"
                                    );
                        }
                        else if (btnMainHand.getImpact(x, y))
                        {
                            if (gv.cc.partyItemSlotIndex == 0)
                            {
                                //lipgloss
                                switchEquipment(inCombat);
                            }
                            gv.cc.partyItemSlotIndex = 0;
                        }
                        else if (btnHead.getImpact(x, y))
                        {
                            //if (inCombat)
                            //{
                                //gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                                //return;
                            //}
                            if (gv.cc.partyItemSlotIndex == 1)
                            {
                                switchEquipment(inCombat);
                            }
                            gv.cc.partyItemSlotIndex = 1;
                        }
                        else if (btnGloves.getImpact(x, y))
                        {
                            //if (inCombat)
                            //{
                            //gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                            //return;
                            //}
                            if (gv.cc.partyItemSlotIndex == 9)
                            {
                                switchEquipment(inCombat);
                            }
                            gv.cc.partyItemSlotIndex = 9;
                        }
                        else if (btnNeck.getImpact(x, y))
                        {
                            //if (inCombat)
                            //{
                                //gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                                //return;
                            //}
                            if (gv.cc.partyItemSlotIndex == 2)
                            {
                                switchEquipment(inCombat);
                            }
                            gv.cc.partyItemSlotIndex = 2;
                        }
                        else if (btnOffHand.getImpact(x, y))
                        {
                            if (gv.cc.partyItemSlotIndex == 3)
                            {
                                switchEquipment(inCombat);
                            }
                            gv.cc.partyItemSlotIndex = 3;
                        }
                        else if (btnRing.getImpact(x, y))
                        {
                            //if (inCombat)
                            //{
                                //gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                                //return;
                            //}
                            if (gv.cc.partyItemSlotIndex == 4)
                            {
                                switchEquipment(inCombat);
                            }
                            gv.cc.partyItemSlotIndex = 4;
                        }
                        else if (btnBody.getImpact(x, y))
                        {
                            //if (inCombat)
                            //{
                                //gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                                //return;
                            //}
                            if (gv.cc.partyItemSlotIndex == 5)
                            {
                                switchEquipment(inCombat);
                            }
                            gv.cc.partyItemSlotIndex = 5;
                        }
                        else if (btnFeet.getImpact(x, y))
                        {
                            //if (inCombat)
                            //{
                                //gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                                //return;
                            //}
                            if (gv.cc.partyItemSlotIndex == 6)
                            {
                                switchEquipment(inCombat);
                            }
                            gv.cc.partyItemSlotIndex = 6;
                        }
                        else if (btnRing2.getImpact(x, y))
                        {
                            //if (inCombat)
                            //{
                                //gv.sf.MessageBoxHtml("Can't equip/unequip this item in combat.");
                                //return;
                            //}
                            if (gv.cc.partyItemSlotIndex == 7)
                            {
                                switchEquipment(inCombat);
                            }
                            gv.cc.partyItemSlotIndex = 7;
                        }
                        else if (btnAmmo.getImpact(x, y))
                        {
                            if (gv.cc.partyItemSlotIndex == 8)
                            {
                                switchEquipment(inCombat);
                            }
                            gv.cc.partyItemSlotIndex = 8;
                        }

                        else if (btnLevelUp.getImpact(x, y))
                        {
                            if (inCombat)
                            {
                                gv.sf.MessageBoxHtml("Can't Level up during combat.");
                                return;
                            }
                            if (gv.mod.playerList[gv.cc.partyScreenPcIndex].IsReadyToAdvanceLevel())
                            {
                                if (gv.mod.playerList[gv.cc.partyScreenPcIndex].isDead())
                                {
                                    //Toast.makeText(gv.gameContext, "Can't Level Up a Dead Character", Toast.LENGTH_SHORT).show();
                                }
                                else
                                {
                                    doLevelUp();
                                }
                            }
                        }
                        else if (btnHelp.getImpact(x, y))
                        {
                            tutorialMessageParty(true);
                        }
                        else if (btnInfo.getImpact(x, y))
                        {
                            Item it = new Item();
                            if (gv.cc.partyItemSlotIndex == 0) { it = gv.mod.getItemByResRef(pc.MainHandRefs.resref); }
                            else if (gv.cc.partyItemSlotIndex == 1) { it = gv.mod.getItemByResRef(pc.HeadRefs.resref); }
                            else if (gv.cc.partyItemSlotIndex == 9) { it = gv.mod.getItemByResRef(pc.GlovesRefs.resref); }
                            else if (gv.cc.partyItemSlotIndex == 2) { it = gv.mod.getItemByResRef(pc.NeckRefs.resref); }
                            else if (gv.cc.partyItemSlotIndex == 3) { it = gv.mod.getItemByResRef(pc.OffHandRefs.resref); }
                            else if (gv.cc.partyItemSlotIndex == 4) { it = gv.mod.getItemByResRef(pc.RingRefs.resref); }
                            else if (gv.cc.partyItemSlotIndex == 5) { it = gv.mod.getItemByResRef(pc.BodyRefs.resref); }
                            else if (gv.cc.partyItemSlotIndex == 6) { it = gv.mod.getItemByResRef(pc.FeetRefs.resref); }
                            else if (gv.cc.partyItemSlotIndex == 7) { it = gv.mod.getItemByResRef(pc.Ring2Refs.resref); }
                            else if (gv.cc.partyItemSlotIndex == 8) { it = gv.mod.getItemByResRef(pc.AmmoRefs.resref); }
                            if (it != null)
                            {
                                gv.cc.buildItemInfoText(it, -100);
                            }
                        }
                        else if (btnReturn.getImpact(x, y))
                        {
                            if (inCombat)
                            {
                                if (gv.screenCombat.canMove)
                                {
                                    gv.screenCombat.currentCombatMode = "move";
                                }
                                else
                                {
                                    gv.screenCombat.currentCombatMode = "attack";
                                }
                                gv.screenType = "combat";
                            }
                            else
                            {
                                gv.screenType = "main";
                            }
                        }
                        else if (btnPartyRoster.getImpact(x, y))
                        {
                            if (!inCombat)
                            {
                                if (gv.mod.hideRoster == false)
                                {
                                    gv.screenType = "partyRoster";
                                }
                            }
                        }
                        if (!inCombat)
                        {
                            for (int j = 0; j < gv.mod.playerList.Count; j++)
                            {
                                if (btnPartyIndex[j].getImpact(x, y))
                                {
                                    gv.mod.selectedPartyLeader = j;
                                    gv.screenMainMap.updateTraitsPanel();
                                    gv.cc.addLogText("lime", gv.mod.playerList[gv.mod.selectedPartyLeader].name + " is Party Leader");
                                    if (gv.cc.partyScreenPcIndex == gv.mod.selectedPartyLeader)
                                    {
                                        doInterPartyConvo(); //not used in The Raventhal
                                    }
                                    gv.cc.partyScreenPcIndex = j;
                                    resetTokenAndPortrait();
                                }
                            }
                        }
                        break;
                }
            }
            catch { }
        }
        public void tokenLoad(Player p)
        {
            p.token = gv.cc.LoadBitmap(p.tokenFilename);
            btnToken.Img2 = p.token;
        }
        public void portraitLoad(Player p)
        {
            p.portrait = gv.cc.LoadBitmap(p.portraitFilename);
            btnPortrait.Img = p.portrait;
        }
        public String isUseableBy(Item it)
        {
            string strg = "";
            foreach (PlayerClass cls in gv.mod.modulePlayerClassList)
            {
                string firstLetter = cls.name.Substring(0, 1);
                foreach (ItemRefs stg in cls.itemsAllowed)
                {
                    if (stg.resref.Equals(it.resref))
                    {
                        strg += firstLetter + ", ";
                    }
                }
            }
            return strg;
        }
        public void doInterPartyConvo()
        {
            if (gv.cc.partyScreenPcIndex == 0)
            {
                return;
            }
            if (gv.cc.partyScreenPcIndex >= gv.mod.playerList.Count)
            {
                return;
            }
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
            gv.cc.doConversationBasedOnTag(pc.name);
        }

        public bool canNotBeUnequipped()
        {
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
            if (gv.cc.partyItemSlotIndex == 0) { return pc.MainHandRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 1) { return pc.HeadRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 9) { return pc.GlovesRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 2) { return pc.NeckRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 3) { return pc.OffHandRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 4) { return pc.RingRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 5) { return pc.BodyRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 6) { return pc.FeetRefs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 7) { return pc.Ring2Refs.canNotBeUnequipped; }
            else if (gv.cc.partyItemSlotIndex == 8) { return pc.AmmoRefs.canNotBeUnequipped; }
            return false;
        }

        public bool canNotBeChangedInCombat()
        {
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];

            if (gv.cc.partyItemSlotIndex == 0)
            {
                Item it = gv.mod.getItemByResRef(pc.MainHandRefs.resref);
                if (it == null) { return false; };
                return it.canNotBeChangedInCombat;
            }
            else if (gv.cc.partyItemSlotIndex == 1)
            {
                Item it = gv.mod.getItemByResRef(pc.HeadRefs.resref);
                if (it == null) { return false; };
                return it.canNotBeChangedInCombat;
            }
            else if (gv.cc.partyItemSlotIndex == 9)
            {
                Item it = gv.mod.getItemByResRef(pc.GlovesRefs.resref);
                if (it == null) { return false; };
                return it.canNotBeChangedInCombat;
            }
            else if (gv.cc.partyItemSlotIndex == 2)
            {
                Item it = gv.mod.getItemByResRef(pc.NeckRefs.resref);
                if (it == null) { return false; };
                return it.canNotBeChangedInCombat;
            }
            else if (gv.cc.partyItemSlotIndex == 3)
            {
                Item it = gv.mod.getItemByResRef(pc.OffHandRefs.resref);
                if (it == null) { return false; };
                return it.canNotBeChangedInCombat;
            }
            else if (gv.cc.partyItemSlotIndex == 4)
            {
                Item it = gv.mod.getItemByResRef(pc.RingRefs.resref);
                if (it == null) { return false; };
                return it.canNotBeChangedInCombat;
            }
            else if (gv.cc.partyItemSlotIndex == 5)
            {
                Item it = gv.mod.getItemByResRef(pc.BodyRefs.resref);
                if (it == null) { return false; };
                return it.canNotBeChangedInCombat;
            }
            else if (gv.cc.partyItemSlotIndex == 6)
            {
                Item it = gv.mod.getItemByResRef(pc.FeetRefs.resref);
                if (it == null) { return false; };
                return it.canNotBeChangedInCombat;
            }
            else if (gv.cc.partyItemSlotIndex == 7)
            {
                Item it = gv.mod.getItemByResRef(pc.Ring2Refs.resref);
                if (it == null) { return false; };
                return it.canNotBeChangedInCombat;
            }
            else if (gv.cc.partyItemSlotIndex == 8)
            {
                Item it = gv.mod.getItemByResRef(pc.AmmoRefs.resref);
                if (it == null) { return false; };
                return it.canNotBeChangedInCombat;
            }
            return false;
        }

        public void switchEquipment(bool inCombat)
        {
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
            if (gv.cc.partyItemSlotIndex == 3)
            {
                if (gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref).twoHanded)
                {
                    gv.sf.MessageBoxHtml("Can't equip an item in off-hand while using a two-handed weapon. Unequip the two-handed weapon from the main-hand first.");
                    return;
                }
            }

            //check to see if ammo can be used by MainHand weapon
            if (gv.cc.partyItemSlotIndex == 8)
            {
                Item itMH = gv.mod.getItemByResRef(pc.MainHandRefs.resref);
                if ((!itMH.category.Equals("Ranged")) || (itMH.ammoType.Equals("none")))
                {
                    gv.sf.MessageBoxHtml("Can't use ammo with the weapon currently equipped in your main-hand.");
                    return;
                }
            }

            //check to see if item can not be unequipped
            if (canNotBeUnequipped())
            {
                gv.sf.MessageBoxHtml("Can't unequip this item...PC specific item or a cursed item.");
                return;
            }

            //check to see if item can not be unequipped
            if (inCombat)
            {
                if (canNotBeChangedInCombat())
                {
                    gv.sf.MessageBoxHtml("Can't change this item during combat.");
                    return;
                }
            }

            List<ItemRefs> allowedItems = new List<ItemRefs>();

            //add any other allowed items to the allowed list
            foreach (ItemRefs itRef in gv.mod.partyInventoryRefsList)
            {
                Item it = gv.mod.getItemByResRef(itRef.resref);

                if (inCombat)
                {
                    if (it.canNotBeChangedInCombat)
                    {
                        continue;
                    }
                }

                if ((it.requiredLevel <= pc.classLevel) &&(gv.cc.checkRequirmentsMet(pc, it)))
                {
                    if (gv.cc.partyItemSlotIndex == 0)
                    {
                        if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
                        {
                            if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                            {
                                allowedItems.Add(itRef);
                            }
                        }
                    }
                    else if ((it.category.Equals("Head")) && (gv.cc.partyItemSlotIndex == 1))
                    {
                        if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                        {
                            allowedItems.Add(itRef);
                        }
                    }
                    else if ((it.category.Equals("Gloves")) && (gv.cc.partyItemSlotIndex == 9))
                    {
                        if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                        {
                            allowedItems.Add(itRef);
                        }
                    }
                    else if ((it.category.Equals("Neck")) && (gv.cc.partyItemSlotIndex == 2))
                    {
                        if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                        {
                            allowedItems.Add(itRef);
                        }
                    }
                    else if ((it.category.Equals("Shield")) && (gv.cc.partyItemSlotIndex == 3))
                    {
                        if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                        {
                            allowedItems.Add(itRef);
                        }
                    }
                    else if ((it.category.Equals("Ring")) && (gv.cc.partyItemSlotIndex == 4))
                    {
                        if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                        {
                            allowedItems.Add(itRef);
                        }
                    }
                    else if ((it.category.Equals("Armor")) && (gv.cc.partyItemSlotIndex == 5))
                    {
                        if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                        {
                            allowedItems.Add(itRef);
                        }
                    }
                    else if ((it.category.Equals("Feet")) && (gv.cc.partyItemSlotIndex == 6))
                    {
                        if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                        {
                            allowedItems.Add(itRef);
                        }
                    }
                    else if ((it.category.Equals("Ring")) && (gv.cc.partyItemSlotIndex == 7))
                    {
                        if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                        {
                            allowedItems.Add(itRef);
                        }
                    }
                    else if ((it.category.Equals("Ammo")) && (gv.cc.partyItemSlotIndex == 8))
                    {
                        if (pc.playerClass.containsItemRefsWithResRef(itRef.resref))
                        {
                            Item itMH = gv.mod.getItemByResRef(pc.MainHandRefs.resref);
                            if ((itMH.category.Equals("Ranged")) && (!itMH.ammoType.Equals("none")) && (it.ammoType.Equals(itMH.ammoType)))
                            {
                                allowedItems.Add(itRef);
                            }
                        }
                    }
                }
            }

            //pass items to selector
            gv.screenType = "itemSelector";
            if (inCombat)
            {
                gv.screenItemSelector.resetItemSelector(allowedItems, "equip", "combatParty");
            }
            else
            {
                gv.screenItemSelector.resetItemSelector(allowedItems, "equip", "party");
            }
        }

        public void doLevelUp()
        {
            List<string> actionList = new List<string> { "Cancel", "LEVEL UP" };

            using (ItemListSelector itSel = new ItemListSelector(gv, actionList, "Level Up Action"))
            {
                itSel.IceBlinkButtonClose.Enabled = true;
                itSel.IceBlinkButtonClose.Visible = true;
                itSel.setupAll(gv);
                var ret = itSel.ShowDialog();
                if (itSel.selectedIndex == 0) // selected to Cancel
                {
                    //do nothing
                }
                else if (itSel.selectedIndex == 1) // selected to LEVEL UP
                {
                    Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
                    //LEVEL UP ALL STATS AND UPDATE STATS
                    //moving the level to later in the process (after all is done, with no exit button clicked)
                    //pc.LevelUp();
                    gv.sf.UpdateStats(pc);
                    pc.classLevel++;

                    traitGained = "Trait Gained: ";
                    spellGained = "Spell Gained: ";

                    //sambal
                    //brainstorming how to do this correctly
                    //likely always call the level up screen, even when only automatics are learned
                    //in case manually chosable traits or spells exist, we only add to temp lists, also for EFFECTS, here and then calll the trait/spell screens
                    //in case, no manually choosabels exist we can leave it as is
                    //add automatics only temporarily there, with temp effects (like manually chosen traits/spells)
                    //but then the screen must finish resolving wthout need to click anything
                    //maybe add aditional parm to register which kind of call comes?

                    //check to see if have any traits to learn
                    List<string> traitTagsList = new List<string>();
                    traitTagsList = pc.getTraitsToLearn(gv.mod);

                    //check to see if have any spells to learn available
                    List<string> spellTagsList = new List<string>();
                    spellTagsList = pc.getSpellsToLearn();

                    //no manually selectable spells or traits
                    //kölnsüd
                    //pc.playerClass.spellsToLearnAtLevelTable[pc.classLevel] > 0
                    if ((traitTagsList.Count == 0 && spellTagsList.Count == 0) || (pc.playerClass.spellsToLearnAtLevelTable[pc.classLevel] == 0 && pc.playerClass.traitsToLearnAtLevelTable[pc.classLevel] == 0))
                    {
                        //if automatically learned traits or spells add them
                        foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                        {
                            if ((ta.automaticallyLearned) && (ta.atWhatLevelIsAvailable == pc.classLevel))
                            {
                                traitGained += ta.name + ", ";
                                pc.knownTraitsTags.Add(ta.tag);
                                //public string useableInSituation = "Always"; //InCombat, OutOfCombat, Always, Passive
                                if (!ta.associatedSpellTag.Equals("none"))
                                {
                                    if (ta.useableInSituation.Contains("Always"))
                                    {
                                        pc.knownUsableTraitsTags.Add(ta.associatedSpellTag);
                                        pc.knownOutsideCombatUsableTraitsTags.Add(ta.associatedSpellTag);
                                        pc.knownInCombatUsableTraitsTags.Add(ta.associatedSpellTag);
                                    }
                                    if (ta.useableInSituation.Contains("OutOfCombat"))
                                    {
                                        pc.knownUsableTraitsTags.Add(ta.associatedSpellTag);
                                        pc.knownOutsideCombatUsableTraitsTags.Add(ta.associatedSpellTag);
                                    }
                                    if (ta.useableInSituation.Contains("InCombat"))
                                    {
                                        pc.knownUsableTraitsTags.Add(ta.associatedSpellTag);
                                        pc.knownInCombatUsableTraitsTags.Add(ta.associatedSpellTag);
                                    }
                                }

                                
                                //must add replacement code (for spells, too)
                                //********************************************
                                #region replacement code traits

                                //get the trait gained
                                Trait tr = new Trait();
                                tr = gv.mod.getTraitByTag(ta.tag);

                                //get the trait to replace (if existent)
                                Trait temp2 = new Trait();
                                foreach (Trait t in gv.mod.moduleTraitsList)
                                {
                                    if (t.tag == tr.traitToReplaceByTag)
                                    {
                                        temp2 = t.DeepCopy();
                                    }
                                }

                                //adding to replaced list even if never taken (allow alternativley exclusive traits)
                                if ((tr.traitToReplaceByTag != "none") && (tr.traitToReplaceByTag != ""))
                                {
                                    pc.replacedTraitsOrSpellsByTag.Add(tr.traitToReplaceByTag);
                                }
                                if (tr.traitToReplaceByTag != tr.prerequisiteTrait)
                                {
                                    string replacedTag = tr.traitToReplaceByTag;
                                    for (int j = gv.mod.moduleTraitsList.Count - 1; j >= 0; j--)
                                    {
                                        if (gv.mod.moduleTraitsList[j].prerequisiteTrait == replacedTag)
                                        {
                                            if (!pc.replacedTraitsOrSpellsByTag.Contains(replacedTag))
                                            {
                                                pc.replacedTraitsOrSpellsByTag.Add(gv.mod.moduleTraitsList[j].tag);
                                                replacedTag = gv.mod.moduleTraitsList[j].tag;
                                                j = gv.mod.moduleTraitsList.Count - 1;
                                            }
                                        }
                                    }
                                }

                                //adding trait to replace mechanism: known traits
                                for (int i = pc.knownTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        //TODO: remove connected permannent effects
                                        //Peter
                                        for (int j = pc.effectsList.Count - 1; j >= 0; j--)
                                        {
                                            foreach (EffectTagForDropDownList etfddl in temp2.traitEffectTagList)
                                            {
                                                if (pc.effectsList[j].tag == etfddl.tag)
                                                {
                                                    if (pc.effectsList[j].isPermanent)
                                                    {
                                                        pc.effectsList.RemoveAt(j);
                                                    }
                                                }
                                            }
                                        }
                                        //pc.replacedTraitsOrSpellsByTag.Add(tr.traitToReplaceByTag);
                                        pc.knownTraitsTags.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.knownInCombatUsableTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownInCombatUsableTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        pc.knownInCombatUsableTraitsTags.RemoveAt(i);
                                    }

                                    if (pc.knownInCombatUsableTraitsTags[i] == temp2.associatedSpellTag)
                                    {
                                        pc.knownInCombatUsableTraitsTags.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.knownOutsideCombatUsableTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownOutsideCombatUsableTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        pc.knownOutsideCombatUsableTraitsTags.RemoveAt(i);
                                    }
                                    if (pc.knownOutsideCombatUsableTraitsTags[i] == temp2.associatedSpellTag)
                                    {
                                        pc.knownOutsideCombatUsableTraitsTags.RemoveAt(i);
                                    }
                                }

                                for (int i = pc.knownUsableTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownUsableTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        pc.knownUsableTraitsTags.RemoveAt(i);
                                    }
                                    if (pc.knownUsableTraitsTags[i] == temp2.associatedSpellTag)
                                    {
                                        pc.knownUsableTraitsTags.RemoveAt(i);
                                    }
                                }


                                //adding trait to replace mechanism: learing traits list (just added)
                                for (int i = pc.learningTraitsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.learningTraitsTags[i] == tr.traitToReplaceByTag)
                                    {
                                        //TODO: remove connected permannent effects
                                        //Peter
                                        for (int j = pc.effectsList.Count - 1; j >= 0; j--)
                                        {
                                            foreach (EffectTagForDropDownList etfddl in temp2.traitEffectTagList)
                                            {
                                                if (pc.effectsList[j].tag == etfddl.tag)
                                                {
                                                    if (pc.effectsList[j].isPermanent)
                                                    {
                                                        pc.effectsList.RemoveAt(j);
                                                    }
                                                }
                                            }
                                        }
                                        //pc.replacedTraitsOrSpellsByTag.Add(tr.traitToReplaceByTag);
                                        pc.learningTraitsTags.RemoveAt(i);
                                    }
                                }
                                #endregion

                                //********************************************
                                ////must add code for adding effects!
                                //*********************************************
                                //add permanent effects of trait to effect list of this pc
                                foreach (EffectTagForDropDownList efTag in tr.traitEffectTagList)
                                {//1
                                    foreach (Effect ef in gv.mod.moduleEffectsList)
                                    {//2
                                        if (ef.tag == efTag.tag)
                                        {//3
                                            if (ef.isPermanent)
                                            {//4
                                                bool doesNotExistAlfready = true;
                                                foreach (Effect ef2 in pc.effectsList)
                                                {//5
                                                    if (ef2.tag == ef.tag)
                                                    {//6
                                                        doesNotExistAlfready = false;
                                                        break;
                                                    }//6
                                                }//5

                                                if (doesNotExistAlfready)
                                                {//6
                                                    pc.effectsList.Add(ef);
                                                    gv.sf.UpdateStats(pc);
                                                    if (ef.modifyHpMax != 0)
                                                    {//7
                                                        pc.hp += ef.modifyHpMax;
                                                        if (pc.hp < 1)
                                                        {//8
                                                            pc.hp = 1;
                                                        }//8
                                                        if (pc.hp > pc.hpMax)
                                                        {
                                                            pc.hp = pc.hpMax;
                                                        }
                                                    }//7

                                                    if (ef.modifyCon != 0)
                                                    {//7
                                                        pc.hp += ef.modifyCon / 2;
                                                        if (pc.hp < 1)
                                                        {//8
                                                            pc.hp = 1;
                                                        }//8
                                                        if (pc.hp > pc.hpMax)
                                                        {
                                                            pc.hp = pc.hpMax;
                                                        }
                                                    }//7

                                                    if (ef.modifySpMax != 0)
                                                    {
                                                        pc.sp += ef.modifySpMax;
                                                        if (pc.sp < 1)
                                                        {
                                                            pc.sp = 1;
                                                        }
                                                        if (pc.sp > pc.spMax)
                                                        {
                                                            pc.sp = pc.spMax;
                                                        }
                                                    }

                                                    if (ef.modifyStr != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("strength"))
                                                        {
                                                            pc.sp += ef.modifyStr / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyDex != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("dexterity"))
                                                        {
                                                            pc.sp += ef.modifyDex / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyCon != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("constitution"))
                                                        {
                                                            pc.sp += ef.modifyCon / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyCha != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("charisma"))
                                                        {
                                                            pc.sp += ef.modifyCha / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyInt != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("intelligence"))
                                                        {
                                                            pc.sp += ef.modifyInt / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }

                                                    if (ef.modifyWis != 0)
                                                    {
                                                        if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("wisdom"))
                                                        {
                                                            pc.sp += ef.modifyWis / 2;
                                                            if (pc.sp < 1)
                                                            {
                                                                pc.sp = 1;
                                                            }
                                                            if (pc.sp > pc.spMax)
                                                            {
                                                                pc.sp = pc.spMax;
                                                            }
                                                        }
                                                    }
                                                }//5
                                            }//4
                                        }//3
                                    }//2
                                }//1 

                                //**********************************************

                            }
                        }

                        foreach (SpellAllowed sa in pc.playerClass.spellsAllowed)
                        {
                            if ((sa.automaticallyLearned) && (sa.atWhatLevelIsAvailable == pc.classLevel))
                            {
                                spellGained += sa.name + ", ";
                                pc.knownSpellsTags.Add(sa.tag);
                                
                                #region replacement code spells
                                //get the spell gained
                                Spell sp = new Spell();
                                sp = gv.mod.getSpellByTag(sp.tag);
                                if ((sp.spellToReplaceByTag != "none") && (sp.spellToReplaceByTag != ""))
                                {
                                    pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                                }
                                
                                //adding trait to replace mechanism: known traits
                                for (int i = pc.knownSpellsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.knownSpellsTags[i] == sp.spellToReplaceByTag)
                                    {
                                        //pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                                        pc.knownSpellsTags.RemoveAt(i);
                                    }
                                }

                                //adding trait to replace mechanism: learing traits list (just added)
                                for (int i = pc.learningSpellsTags.Count - 1; i >= 0; i--)
                                {
                                    if (pc.learningSpellsTags[i] == sp.spellToReplaceByTag)
                                    {
                                        //pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                                        pc.learningSpellsTags.RemoveAt(i);
                                    }
                                }
                                #endregion
                            }
                        }
                        pc.classLevel--;
                        pc.LevelUp();
                        gv.sf.UpdateStats(pc);
                        doLevelUpSummary();
                    }
                    //found: at least one screen needs to be called

                    else if (traitTagsList.Count > 0 && pc.playerClass.traitsToLearnAtLevelTable[pc.classLevel] > 0 && pc.getTraitsToLearn(gv.mod).Count > 0)
                    {
                        //add automatically gained spells and traits (inlcuding effects) to the tempoarray lists already here
                        //no replacements at this stage though (these come when done on the manual screens) 

                        //traits
                        foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                        {
                            if ((ta.automaticallyLearned) && (ta.atWhatLevelIsAvailable == pc.classLevel))
                            {
                                Trait tr = new Trait();
                                tr = gv.mod.getTraitByTag(ta.tag);

                                pc.learningTraitsTags.Add(tr.tag);

                                foreach (EffectTagForDropDownList etfddl in tr.traitEffectTagList)
                                {
                                    foreach (Effect e in gv.mod.moduleEffectsList)
                                    {
                                        if (e.tag == etfddl.tag)
                                        {
                                            if (e.isPermanent)
                                            {
                                                pc.learningEffects.Add(e);
                                            }
                                        }
                                    }
                                }
                                
                                //}
                                /*
                                if (!ta.associatedSpellTag.Equals("none"))
                                {
                                    if (ta.useableInSituation.Contains("Always"))
                                    {
                                        pc.knownUsableTraitsTags.Add(ta.associatedSpellTag);
                                        pc.knownOutsideCombatUsableTraitsTags.Add(ta.associatedSpellTag);
                                        pc.knownInCombatUsableTraitsTags.Add(ta.associatedSpellTag);
                                    }
                                    if (ta.useableInSituation.Contains("OutOfCombat"))
                                    {
                                        pc.knownUsableTraitsTags.Add(ta.associatedSpellTag);
                                        pc.knownOutsideCombatUsableTraitsTags.Add(ta.associatedSpellTag);
                                    }
                                    if (ta.useableInSituation.Contains("InCombat"))
                                    {
                                        pc.knownUsableTraitsTags.Add(ta.associatedSpellTag);
                                        pc.knownInCombatUsableTraitsTags.Add(ta.associatedSpellTag);
                                    }
                                }
                                */
                            }
                        }

                        //spells
                        foreach (SpellAllowed sa in pc.playerClass.spellsAllowed)
                        {
                            if ((sa.automaticallyLearned) && (sa.atWhatLevelIsAvailable == pc.classLevel))
                            {
                                Spell sp = new Spell();
                                sp = gv.mod.getSpellByTag(sa.tag);

                                pc.learningSpellsTags.Add(sp.tag);
                            }
                        }

                        gv.screenTraitLevelUp.resetPC(false, pc);
                        gv.screenType = "learnTraitLevelUp";
                    }
                    else if (spellTagsList.Count > 0 && pc.playerClass.spellsToLearnAtLevelTable[pc.classLevel] > 0)
                    {
                        //add automaticall gained spells to the tempoarray lists already here
                        //no replacements at this stage though

                        //spells
                        foreach (SpellAllowed sa in pc.playerClass.spellsAllowed)
                        {
                            if ((sa.automaticallyLearned) && (sa.atWhatLevelIsAvailable == pc.classLevel))
                            {
                                Spell sp = new Spell();
                                sp = gv.mod.getSpellByTag(sa.tag);

                                pc.learningSpellsTags.Add(sp.tag);
                            }
                        }

                        gv.screenSpellLevelUp.resetPC(false, pc, false);
                        gv.screenType = "learnSpellLevelUp";
                    }
                    else //no spells or traits to learn
                    {
                        pc.classLevel--;
                        pc.LevelUp();
                        gv.sf.UpdateStats(pc);
                        doLevelUpSummary();
                    }
                }
            }
        }
        public void doLevelUpSummary()
        {
            Player pc = gv.mod.playerList[gv.cc.partyScreenPcIndex];
            int babGained = pc.playerClass.babTable[pc.classLevel] - pc.playerClass.babTable[pc.classLevel - 1];

            string text = pc.name + " has gained:<br>";
            if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("intelligence"))
            {
                text += "HP: +" + (pc.playerClass.hpPerLevelUp + ((pc.constitution - 10) / 2)) + "<br>"
                   + "SP: +" + (pc.playerClass.spPerLevelUp + ((pc.intelligence - 10) / 2)) + "<br>"
                   + "BAB: +" + babGained + "<br>"
                   + traitGained + "<br>"
                   + spellGained;
            }
            if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("wisdom"))
            {
                text += "HP: +" + (pc.playerClass.hpPerLevelUp + ((pc.constitution - 10) / 2)) + "<br>"
                   + "SP: +" + (pc.playerClass.spPerLevelUp + ((pc.wisdom - 10) / 2)) + "<br>"
                   + "BAB: +" + babGained + "<br>"
                   + traitGained + "<br>"
                   + spellGained;
            }
            if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("charisma"))
            {
                text += "HP: +" + (pc.playerClass.hpPerLevelUp + ((pc.constitution - 10) / 2)) + "<br>"
                   + "SP: +" + (pc.playerClass.spPerLevelUp + ((pc.charisma - 10) / 2)) + "<br>"
                   + "BAB: +" + babGained + "<br>"
                   + traitGained + "<br>"
                   + spellGained;
            }
            if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("strength"))
            {
                text += "HP: +" + (pc.playerClass.hpPerLevelUp + ((pc.constitution - 10) / 2)) + "<br>"
                   + "SP: +" + (pc.playerClass.spPerLevelUp + ((pc.strength - 10) / 2)) + "<br>"
                   + "BAB: +" + babGained + "<br>"
                   + traitGained + "<br>"
                   + spellGained;
            }
            if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("dexterity"))
            {
                text += "HP: +" + (pc.playerClass.hpPerLevelUp + ((pc.constitution - 10) / 2)) + "<br>"
                   + "SP: +" + (pc.playerClass.spPerLevelUp + ((pc.dexterity - 10) / 2)) + "<br>"
                   + "BAB: +" + babGained + "<br>"
                   + traitGained + "<br>"
                   + spellGained;
            }
            if (pc.playerClass.modifierFromSPRelevantAttribute.Equals("constitution"))
            {
                text += "HP: +" + (pc.playerClass.hpPerLevelUp + ((pc.constitution - 10) / 2)) + "<br>"
                   + "SP: +" + (pc.playerClass.spPerLevelUp + ((pc.constitution - 10) / 2)) + "<br>"
                   + "BAB: +" + babGained + "<br>"
                   + traitGained + "<br>"
                   + spellGained;
            }

            gv.sf.MessageBoxHtml(text);
        }
        public void tutorialMessageParty(bool helpCall)
        {
            if ((gv.mod.showTutorialParty) || (helpCall))
            {
                gv.sf.MessageBoxHtml(gv.cc.stringMessageParty);
                gv.mod.showTutorialParty = false;
            }
        }
    }
}
