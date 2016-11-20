using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class ScreenPcCreation
    {
        public Module mod;
        public GameView gv;

        private IbbButton btnName = null;
        private IbbButton btnRace = null;
        private IbbButton btnClass = null;
        private IbbButton btnGender = null;
        private IbbPortrait btnPortrait = null;
        private IbbButton btnToken = null;

        private IbbButton btnPlayerGuideOnPcCreation = null;
        private IbbButton btnBeginnerGuideOnPcCreation = null;
        private IbbButton btnRollStats = null;
        private IbbButton btnFinished = null;
        private IbbButton btnAbort = null;

        private Bitmap blankItemSlot;
        private int pcCreationIndex = 0;
        //private int pcTokenSelectionIndex = 0;
        //private int pcPortraitSelectionIndex = 0;
        private int pcRaceSelectionIndex = 0;
        private int pcClassSelectionIndex = 0;
        public List<string> playerTokenList = new List<string>();
        public List<string> playerPortraitList = new List<string>();
        public List<Race> playerRaces = new List<Race>();
        public Player pc;

        public ScreenPcCreation(Module m, GameView g)
        {
            mod = m;
            gv = g;
            blankItemSlot = gv.cc.LoadBitmap("item_slot");
            LoadPlayerBitmapList();
            LoadPlayerPortraitList();
            CreateRaceList();
            resetPC();
            setControlsStart();

        }

        public void resetPC()
        {
            pc = gv.cc.LoadPlayer(gv.mod.defaultPlayerFilename);
            pc.token = gv.cc.LoadBitmap(pc.tokenFilename);
            pc.portrait = gv.cc.LoadBitmap(pc.portraitFilename);
            pc.playerClass = mod.getPlayerClass(pc.classTag);
            pc.race = this.getAllowedRace(pc.raceTag);
            pc.name = "CharacterName";
            pc.tag = "characterName";
            pcCreationIndex = 0;
            reRollStats(pc);
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
        public void LoadPlayerPortraitList()
        {
            playerPortraitList.Clear();
            try
            {
                //Load from module folder first
                string[] files;
                if (Directory.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\portraits"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\portraits", "*.png");
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if ((filename.EndsWith(".png")) || (filename.EndsWith(".PNG")))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                playerPortraitList.Add(fileNameWithOutExt);
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
                if (Directory.Exists(gv.mainDirectory + "\\PlayerPortraits"))
                {
                    files = Directory.GetFiles(gv.mainDirectory + "\\PlayerPortraits", "*.png");
                    foreach (string file in files)
                    {
                        try
                        {
                            string filename = Path.GetFileName(file);
                            if ((filename.EndsWith(".png")) || (filename.EndsWith(".PNG")))
                            {
                                string fileNameWithOutExt = Path.GetFileNameWithoutExtension(file);
                                if (!playerPortraitList.Contains(fileNameWithOutExt))
                                {
                                    playerPortraitList.Add(fileNameWithOutExt);
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
            int padW = gv.squareSize / 6;
            int center = gv.screenWidth / 2;

            if (btnPortrait == null)
            {
                btnPortrait = new IbbPortrait(gv, 1.0f);
                btnPortrait.ImgBG = gv.cc.LoadBitmap("item_slot");
                btnPortrait.Glow = gv.cc.LoadBitmap("btn_small_glow");
                btnPortrait.X = 10 * gv.squareSize;
                btnPortrait.Y = 1 * gv.squareSize + pH * 2;
                btnPortrait.Height = (int)(pc.portrait.PixelSize.Height * gv.screenDensity);
                btnPortrait.Width = (int)(pc.portrait.PixelSize.Width * gv.screenDensity);
            }
            if (btnToken == null)
            {
                btnToken = new IbbButton(gv, 1.0f);
                btnToken.Img = gv.cc.LoadBitmap("item_slot");
                btnToken.Img2 = gv.cc.LoadBitmap(pc.tokenFilename);
                btnToken.Glow = gv.cc.LoadBitmap("btn_small_glow");
                btnToken.X = 12 * gv.squareSize;
                btnToken.Y = 1 * gv.squareSize + pH * 2;
                btnToken.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnToken.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnName == null)
            {
                btnName = new IbbButton(gv, 1.0f);
                btnName.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnName.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnName.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
                btnName.Y = 0 * gv.squareSize + gv.squareSize / 2;
                btnName.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnName.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnRace == null)
            {
                btnRace = new IbbButton(gv, 1.0f);
                btnRace.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnRace.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnRace.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
                btnRace.Y = 1 * gv.squareSize + gv.squareSize / 2;
                btnRace.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRace.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnGender == null)
            {
                btnGender = new IbbButton(gv, 1.0f);
                btnGender.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnGender.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnGender.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
                btnGender.Y = 2 * gv.squareSize + gv.squareSize / 2;
                btnGender.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnGender.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnClass == null)
            {
                btnClass = new IbbButton(gv, 1.0f);
                btnClass.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnClass.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnClass.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
                btnClass.Y = 3 * gv.squareSize + gv.squareSize / 2;
                btnClass.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnClass.Width = (int)(gv.ibbwidthL * gv.screenDensity);
                
            }
            if (btnPlayerGuideOnPcCreation == null)
            {
                btnPlayerGuideOnPcCreation = new IbbButton(gv, 1.0f);
                btnPlayerGuideOnPcCreation.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnPlayerGuideOnPcCreation.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnPlayerGuideOnPcCreation.Text = "Player's Guide";
                btnPlayerGuideOnPcCreation.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
                btnPlayerGuideOnPcCreation.Y = 7 * gv.squareSize;
                btnPlayerGuideOnPcCreation.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnPlayerGuideOnPcCreation.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnBeginnerGuideOnPcCreation == null)
            {
                btnBeginnerGuideOnPcCreation = new IbbButton(gv, 1.0f);
                btnBeginnerGuideOnPcCreation.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnBeginnerGuideOnPcCreation.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnBeginnerGuideOnPcCreation.Text = "Beginner's Guide";
                btnBeginnerGuideOnPcCreation.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1;
                btnBeginnerGuideOnPcCreation.Y = 8 * gv.squareSize + pH;
                btnBeginnerGuideOnPcCreation.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnBeginnerGuideOnPcCreation.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnRollStats == null)
            {
                btnRollStats = new IbbButton(gv, 1.0f);
                btnRollStats.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnRollStats.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnRollStats.Text = "Roll Stats";
                btnRollStats.X = center + pW * 1;
                btnRollStats.Y = 7 * gv.squareSize;
                btnRollStats.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnRollStats.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnFinished == null)
            {
                btnFinished = new IbbButton(gv, 1.0f);
                btnFinished.Img = gv.cc.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnFinished.Glow = gv.cc.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnFinished.Text = "Finished";
                btnFinished.X = center + pW * 1;
                btnFinished.Y = 8 * gv.squareSize + pH;
                btnFinished.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnFinished.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }
            if (btnAbort == null)
            {
                btnAbort = new IbbButton(gv, 0.8f);
                btnAbort.Text = "Abort";
                btnAbort.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnAbort.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnAbort.X = 8 * gv.squareSize + padW * 1 + gv.oXshift;
                btnAbort.Y = 9 * gv.squareSize + pH * 2;
                btnAbort.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAbort.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
        }

        public void redrawPcCreation()
        {
            //Player pc = mod.playerList.get(0);
            gv.sf.UpdateStats(pc);

            int pW = (int)((float)gv.screenWidth / 100.0f);
            int pH = (int)((float)gv.screenHeight / 100.0f);

            int locX = 6 * gv.squareSize;
            int textH = (int)gv.drawFontRegHeight;
            int spacing = textH;
            int locY = 4 * gv.squareSize + gv.squareSize / 4;
            int locY2 = 4 * gv.squareSize + gv.squareSize / 4;
            int tabX = pW * 50;
            int tabX2 = pW * 50 + gv.squareSize - pW;
            int leftStartY = pH * 20;
            int tokenStartX = locX + (textH * 5);
            int tokenStartY = pH * 5 + (spacing / 2);
            int portraitStartX = 12 * gv.squareSize + (textH * 5);
            int portraitStartY = pH * 5 + (spacing / 2);
            int tokenRectPad = pW * 1;

            //Page Title
            gv.DrawText("CREATE CHARACTER", pW * 40 + gv.squareSize, pH * 1);

            Color color = Color.White;

            int actext = 0;
            if (mod.ArmorClassAscending) { actext = pc.AC; }
            else { actext = 20 - pc.AC; }
            if (mod.useOrbitronFont == true)
            {
                if (mod.use3d6 && mod.useLuck)
                {
                    gv.DrawText("Rolling: 3d6, Luck is high for those who need it", locX + pW, locY += (spacing));
                }
                else if (mod.use3d6 && !mod.useLuck)
                {
                    gv.DrawText("Rolling: 3d6", locX + pW, locY += (spacing));
                }
                else if (!mod.use3d6 && mod.useLuck)
                {
                    gv.DrawText("Rolling: 6 + d12, Luck is high for those who need it", locX + pW, locY += (spacing));
                }
                else if (!mod.use3d6 && !mod.useLuck)
                {
                    gv.DrawText("Rolling: 6 + d12", locX + pW, locY += (spacing));

                }
                gv.DrawText("STR:   " + pc.baseStr + " + " + (pc.strength - pc.baseStr) + " = " + pc.strength + " (" + ((pc.strength - 10) / 2) + ")", locX + pW, locY += (spacing * 2));
                gv.DrawText("AC: " + actext, tabX2, locY2 += (spacing * 3));
                gv.DrawText("BAB: " + pc.baseAttBonus + ", Melee to hit/damage: " + (pc.baseAttBonus + ((pc.strength - 10) / 2)) + "/" + ((pc.strength - 10) / 2) + ", Ranged to hit: " + (pc.baseAttBonus + ((pc.dexterity - 10) / 2)), tabX2, locY2 += spacing);
                gv.DrawText("DEX:  " + pc.baseDex + " + " + (pc.dexterity - pc.baseDex) + " = " + pc.dexterity + " (" + ((pc.dexterity - 10) / 2) + ")", locX + pW, locY += spacing);
                gv.DrawText("HP: " + pc.hp + "/" + pc.hpMax, tabX2, locY2 += spacing);
                gv.DrawText("CON: " + pc.baseCon + " + " + (pc.constitution - pc.baseCon) + " = " + pc.constitution + " (" + ((pc.constitution - 10) / 2) + ")", locX + pW, locY += spacing);
                gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, tabX2, locY2 += spacing);
                gv.DrawText("INT:   " + pc.baseInt + " + " + (pc.intelligence - pc.baseInt) + " = " + pc.intelligence + " (" + ((pc.intelligence - 10) / 2) + ")", locX + pW, locY += spacing);
                gv.DrawText("FORT: " + pc.fortitude + ", Acid: " + pc.damageTypeResistanceTotalAcid + "%" + ", Cold: " + pc.damageTypeResistanceTotalCold + "%" + ", Normal: " + pc.damageTypeResistanceTotalNormal + "%", tabX2, locY2 += spacing);
                gv.DrawText("REF:   " + pc.reflex + ", Electricity: " + pc.damageTypeResistanceTotalElectricity + "%" + ", Fire: " + pc.damageTypeResistanceTotalFire + "%", tabX2, locY2 += spacing);
                gv.DrawText("WILL: " + pc.will + ", Magic: " + pc.damageTypeResistanceTotalMagic + "%" + ", Poison: " + pc.damageTypeResistanceTotalPoison + "%", tabX2, locY2 += spacing);
                gv.DrawText("WIS:  " + pc.baseWis + " + " + (pc.wisdom - pc.baseWis) + " = " + pc.wisdom + " (" + ((pc.wisdom - 10) / 2) + ")", locX + pW, locY += spacing);
                gv.DrawText("CHA: " + pc.baseCha + " + " + (pc.charisma - pc.baseCha) + " = " + pc.charisma + " (" + ((pc.charisma - 10) / 2) + ")", locX + pW, locY += spacing);
                if (mod.useLuck)
                {
                    gv.DrawText("LCK:  " + pc.baseLuck + " + " + (pc.luck - pc.baseLuck) + " = " + pc.luck, locX + pW, locY += spacing);
                }
            }
            else
            {

                if (mod.use3d6 && mod.useLuck)
                {
                    gv.DrawText("Rolling: 3d6, Luck is high for those who need it", locX + pW, locY += (spacing));
                }
                else if (mod.use3d6 && !mod.useLuck)
                {
                    gv.DrawText("Rolling: 3d6", locX + pW, locY += (spacing));
                }
                else if (!mod.use3d6 && mod.useLuck)
                {
                    gv.DrawText("Rolling: 6 + d12, Luck is high for those who need it", locX + pW, locY += (spacing));
                }
                else if (!mod.use3d6 && !mod.useLuck)
                {
                    gv.DrawText("Rolling: 6 + d12", locX + pW, locY += (spacing));

                }
                gv.DrawText("STR:  " + pc.baseStr + " + " + (pc.strength - pc.baseStr) + " = " + pc.strength + " (" + ((pc.strength - 10) / 2) + ")", locX + pW, locY += (spacing * 2));
                gv.DrawText("AC: " + actext, tabX2, locY2 += (spacing * 3));
                gv.DrawText("BAB: " + pc.baseAttBonus + ", Melee to hit/damage: " + (pc.baseAttBonus + ((pc.strength - 10) / 2)) + "/" + ((pc.strength - 10) / 2) + ", Ranged to hit: " + (pc.baseAttBonus + ((pc.dexterity - 10) / 2)), tabX2, locY2 += spacing);
                gv.DrawText("DEX: " + pc.baseDex + " + " + (pc.dexterity - pc.baseDex) + " = " + pc.dexterity + " (" + ((pc.dexterity - 10) / 2) + ")", locX + pW, locY += spacing);
                gv.DrawText("HP: " + pc.hp + "/" + pc.hpMax, tabX2, locY2 += spacing);
                gv.DrawText("CON: " + pc.baseCon + " + " + (pc.constitution - pc.baseCon) + " = " + pc.constitution + " (" + ((pc.constitution - 10) / 2) + ")", locX + pW, locY += spacing);
                gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, tabX2, locY2 += spacing);
                gv.DrawText("INT:  " + pc.baseInt + " + " + (pc.intelligence - pc.baseInt) + " = " + pc.intelligence + " (" + ((pc.intelligence - 10) / 2) + ")", locX + pW, locY += spacing);
                gv.DrawText("FORT: " + pc.fortitude + ", Acid: " + pc.damageTypeResistanceTotalAcid + "%" + ", Cold: " + pc.damageTypeResistanceTotalCold + "%" + ", Normal: " + pc.damageTypeResistanceTotalNormal + "%", tabX2, locY2 += spacing);
                gv.DrawText("REF:   " + pc.reflex + ", Electricity: " + pc.damageTypeResistanceTotalElectricity + "%" + ", Fire: " + pc.damageTypeResistanceTotalFire + "%", tabX2, locY2 += spacing);
                gv.DrawText("WILL: " + pc.will + ", Magic: " + pc.damageTypeResistanceTotalMagic + "%" + ", Poison: " + pc.damageTypeResistanceTotalPoison + "%", tabX2, locY2 += spacing);
                gv.DrawText("WIS:  " + pc.baseWis + " + " + (pc.wisdom - pc.baseWis) + " = " + pc.wisdom + " (" + ((pc.wisdom - 10) / 2) + ")", locX + pW, locY += spacing);
                gv.DrawText("CHA: " + pc.baseCha + " + " + (pc.charisma - pc.baseCha) + " = " + pc.charisma + " (" + ((pc.charisma - 10) / 2) + ")", locX + pW, locY += spacing);
                if (mod.useLuck)
                {
                    gv.DrawText("LCK:  " + pc.baseLuck + " + " + (pc.luck - pc.baseLuck) + " = " + pc.luck, locX + pW, locY += spacing);
                }

            }
            //Description
            string textToSpan = "";
            if (pcCreationIndex == 2)
            {
                textToSpan = "Description:" + Environment.NewLine;
                textToSpan += pc.race.description;
            }
            else if (pcCreationIndex == 4)
            {
                textToSpan = "Description:" + Environment.NewLine;
                textToSpan += pc.playerClass.description;
            }
            int yLoc = 3 * gv.squareSize;
            IbRect rect = new IbRect(tabX + gv.squareSize - pW, yLoc, pW * 35, pH * 50);
            gv.DrawText(textToSpan, rect, 1.0f, Color.White);

            btnPortrait.Img = pc.portrait;
            btnPortrait.Draw();
            btnToken.Draw();
            btnName.Text = pc.name;
            btnName.Draw();
            btnRace.Text = pc.race.name;
            btnRace.Draw();
            if (pc.isMale)
            {
                btnGender.Text = "Male";
            }
            else
            {
                btnGender.Text = "Female";
            }
            btnGender.Draw();
            btnClass.Text = pc.playerClass.name;
            btnClass.Draw();

            btnRollStats.Draw();
            btnFinished.Draw();
            gv.cc.btnHelp.Draw();
            btnAbort.Draw();
            btnPlayerGuideOnPcCreation.Draw();
            btnBeginnerGuideOnPcCreation.Draw();
        }
        public void onTouchPcCreation(MouseEventArgs e, MouseEventType.EventType eventType)
        {
            btnRollStats.glowOn = false;
            btnFinished.glowOn = false;
            btnAbort.glowOn = false;
            gv.cc.btnHelp.glowOn = false;
            btnPlayerGuideOnPcCreation.glowOn = false;
            btnBeginnerGuideOnPcCreation.glowOn = false;
            btnClass.glowOn = false;
            btnRace.glowOn = false;
            btnGender.glowOn = false;
            btnName.glowOn = false;

            switch (eventType)
            {
                case MouseEventType.EventType.MouseDown:
                case MouseEventType.EventType.MouseMove:
                    int x = (int)e.X;
                    int y = (int)e.Y;
                    if (btnRollStats.getImpact(x, y))
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
                    else if (btnClass.getImpact(x, y))
                    {
                        btnClass.glowOn = true;
                    }
                    else if (btnGender.getImpact(x, y))
                    {
                        btnGender.glowOn = true;
                    }
                    else if (btnName.getImpact(x, y))
                    {
                        btnName.glowOn = true;
                    }
                    else if (btnRace.getImpact(x, y))
                    {
                        btnRace.glowOn = true;
                    }
                    break;

                case MouseEventType.EventType.MouseUp:
                    x = (int)e.X;
                    y = (int)e.Y;

                    btnRollStats.glowOn = false;
                    btnFinished.glowOn = false;
                    btnAbort.glowOn = false;
                    gv.cc.btnHelp.glowOn = false;
                    btnPlayerGuideOnPcCreation.glowOn = false;
                    btnBeginnerGuideOnPcCreation.glowOn = false;
                    btnClass.glowOn = false;
                    btnRace.glowOn = false;
                    btnGender.glowOn = false;
                    btnName.glowOn = false;

                    if (btnName.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        pcCreationIndex = 1;
                        changePcName();
                    }
                    else if (btnRace.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        pcCreationIndex = 2;
                        pcRaceSelectionIndex++;
                        if (pcRaceSelectionIndex >= this.playerRaces.Count)
                        {
                            pcRaceSelectionIndex = 0;
                        }
                        pc.race = playerRaces[pcRaceSelectionIndex];
                        pc.raceTag = pc.race.tag;
                        resetClassSelection(pc);
                        gv.sf.UpdateStats(pc);
                        pc.hp = pc.hpMax;
                        pc.sp = pc.spMax;
                    }
                    else if (btnGender.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        pcCreationIndex = 3;
                        if (pc.isMale)
                        {
                            pc.isMale = false;
                        }
                        else
                        {
                            pc.isMale = true;
                        }
                    }
                    else if (btnClass.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        pcCreationIndex = 4;
                        pcClassSelectionIndex++;
                        if (pcClassSelectionIndex >= pc.race.classesAllowed.Count)
                        {
                            pcClassSelectionIndex = 0;
                        }
                        pc.playerClass = mod.getPlayerClass(pc.race.classesAllowed[pcClassSelectionIndex]);
                        pc.classTag = pc.playerClass.tag;
                        gv.sf.UpdateStats(pc);
                        pc.hp = pc.hpMax;
                        pc.sp = pc.spMax;
                    }
                    else if (btnPortrait.getImpact(x, y))
                    {
                        //pass items to selector
                        gv.screenType = "portraitSelector";
                        gv.screenPortraitSelector.resetPortraitSelector("pcCreation", pc);
                    }
                    else if (btnToken.getImpact(x, y))
                    {
                        gv.screenType = "tokenSelector";
                        gv.screenTokenSelector.resetTokenSelector("pcCreation", pc);
                    }

                    else if (btnRollStats.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        reRollStats(pc);
                    }
                    else if (btnFinished.getImpact(x, y))
                    {
                        //hurghxxx
                        if ((pc.name != "CharacterName") && (pc.name != ""))
                        {
                            gv.PlaySound("btn_click");
                            
                            //if automatically learned traits or spells add them
                            foreach (TraitAllowed ta in pc.playerClass.traitsAllowed)
                            {
                                if ((ta.automaticallyLearned) && (ta.atWhatLevelIsAvailable == pc.classLevel || ta.atWhatLevelIsAvailable == 0))
                                {
                                    pc.knownTraitsTags.Add(ta.tag);
                                    //code for eventually adding permamnet effect
                                    foreach (Trait t in gv.mod.moduleTraitsList)
                                    {
                                        if (t.tag == ta.tag)
                                        {
                                            //add permanent effects of trait to effect list of this pc
                                            foreach (EffectTagForDropDownList efTag in t.traitEffectTagList)
                                            {
                                                foreach (Effect ef in gv.mod.moduleEffectsList)
                                                {
                                                    if (ef.tag == efTag.tag)
                                                    {
                                                        if (ef.isPermanent)
                                                        {
                                                            bool doesNotExistAlfready = true;
                                                            foreach (Effect ef2 in pc.effectsList)
                                                            {
                                                                if (ef2.tag == ef.tag)
                                                                {
                                                                    doesNotExistAlfready = false;
                                                                    break;
                                                                }
                                                            }

                                                            if (doesNotExistAlfready)
                                                            {
                                                                pc.effectsList.Add(ef);
                                                                gv.sf.UpdateStats(pc);

                                                                if (ef.modifyHpMax != 0)
                                                                {
                                                                    pc.hp += ef.modifyHpMax;
                                                                    if (pc.hp < 1)
                                                                    {
                                                                        pc.hp = 1;
                                                                    }
                                                                    if (pc.hp > pc.hpMax)
                                                                    {
                                                                        pc.hp = pc.hpMax;
                                                                    }
                                                                }

                                                                if (ef.modifyCon != 0)
                                                                {
                                                                    pc.hp += ef.modifyCon/2;
                                                                    if (pc.hp < 1)
                                                                    {
                                                                        pc.hp = 1;
                                                                    }
                                                                    if (pc.hp > pc.hpMax)
                                                                    {
                                                                        pc.hp = pc.hpMax;
                                                                    }
                                                                }

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
                                                                        pc.sp += ef.modifyStr/2;
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
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

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

                                }
                            }
                            foreach (SpellAllowed sa in pc.playerClass.spellsAllowed)
                            {
                                if ((sa.automaticallyLearned) && (sa.atWhatLevelIsAvailable == pc.classLevel || sa.atWhatLevelIsAvailable == 0))
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
                                gv.screenTraitLevelUp.resetPC(false, pc);
                                gv.screenType = "learnTraitCreation";
                            }

                            else if (spellTagsList.Count > 0)
                            {
                                gv.screenSpellLevelUp.resetPC(false, pc, false);
                                gv.screenType = "learnSpellCreation";
                            }
                            else
                            {
                                //no spells or traits to learn
                                //save character, add them to the pcList of screenPartyBuild, and go back to build screen
                                this.SaveCharacter(pc);
                                gv.screenPartyBuild.pcList.Add(pc);
                                gv.screenType = "partyBuild";
                            }
                        }
                        else
                        {
                            gv.sf.MessageBoxHtml("Name cannot be CharacterName or blank, please choose a different one.");
                        }
                    }
                    else if (btnAbort.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        gv.screenType = "partyBuild";
                    }
                    else if (gv.cc.btnHelp.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        gv.cc.tutorialPcCreation();

                        //gv.sf.MessageBoxHtml(this.stringMessageMainMap);
                    }
                    else if (btnPlayerGuideOnPcCreation.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        gv.cc.tutorialPlayersGuide();
                    }
                    else if (btnBeginnerGuideOnPcCreation.getImpact(x, y))
                    {
                        gv.PlaySound("btn_click");
                        gv.cc.tutorialBeginnersGuide();
                    }
                    break;
            }
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
        public void changePcName()
        {
            using (TextInputDialog itSel = new TextInputDialog(gv, "Choose a unique Name for this PC."))
            {
                itSel.IceBlinkButtonClose.Visible = true;
                itSel.IceBlinkButtonClose.Enabled = true;
                itSel.textInput = "Type unique Name Here";

                var ret = itSel.ShowDialog();

                if (ret == DialogResult.OK)
                {
                    if (itSel.textInput.Length > 0)
                    {
                        pc.name = itSel.textInput;
                        pc.tag = itSel.textInput.ToLower();
                        bool foundNameConflict = false;
                        foreach (Player p in gv.mod.playerList)
                        {
                            if ((p.name == pc.name) || (p.tag == pc.tag))
                            {
                                gv.sf.MessageBoxHtml("This name already exists, please choose a different one.");
                                pc.name = "";
                                pc.tag = "";
                                itSel.textInput = "Type unique Name Here";
                                foundNameConflict = true;
                                break;
                            }
                        }
                        if (foundNameConflict == false)
                        {
                            foreach (Player p in gv.screenPartyBuild.pcList)
                            {
                                if ((p.name == pc.name) || (p.tag == pc.tag))
                                {
                                    gv.sf.MessageBoxHtml("This name already exists, please choose a different one.");
                                    pc.name = "";
                                    pc.tag = "";
                                    itSel.textInput = "Type unique Name Here";
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Entering a blank name is not allowed", Toast.LENGTH_SHORT).show();
                    }
                }
            }
        }
        public void reRollStats(Player p)
        {
            if (mod.use3d6 == true)
            {
                p.baseStr = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseDex = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseInt = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseCha = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseCon = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseWis = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                int sumOfAttributeBoni = ((p.baseStr - 10) / 2) + ((p.baseDex - 10) / 2) + ((p.baseCon - 10) / 2) + ((p.baseInt - 10) / 2) + ((p.baseWis - 10) / 2) + ((p.baseCha - 10) / 2);
                if (sumOfAttributeBoni > 6)
                {
                    p.baseLuck = 10 - (sumOfAttributeBoni - 6);
                }
                else
                {
                    p.baseLuck = 10 + (6 - sumOfAttributeBoni);
                }
                //p.baseLuck = (int)(14 - ((p.baseStr - 10) / 2) - ((p.baseDex - 10) / 2) - ((p.baseCon - 10) / 2) - ((p.baseInt - 10) / 2) - ((p.baseWis - 10) / 2) - ((p.baseCha - 10) / 2));
                if (p.baseLuck < 3)
                {
                    p.baseLuck = 3;
                }
            }
            else
            {
                p.baseStr = 6 + gv.sf.RandInt(12);
                p.baseDex = 6 + gv.sf.RandInt(12);
                p.baseInt = 6 + gv.sf.RandInt(12);
                p.baseCha = 6 + gv.sf.RandInt(12);
                p.baseCon = 6 + gv.sf.RandInt(12);
                p.baseWis = 6 + gv.sf.RandInt(12);
                int sumOfAttributeBoni = ((p.baseStr - 10) / 2) + ((p.baseDex - 10) / 2) + ((p.baseCon - 10) / 2) + ((p.baseInt - 10) / 2) + ((p.baseWis - 10) / 2) + ((p.baseCha - 10) / 2);
                if (sumOfAttributeBoni > 6)
                {
                    p.baseLuck = 10 - (sumOfAttributeBoni - 6);
                }
                else
                {
                    p.baseLuck = 10 + (6 - sumOfAttributeBoni);
                }
                //p.baseLuck = (int)(14 - ((p.baseStr - 10) / 2) - ((p.baseDex - 10) / 2) - ((p.baseCon - 10) / 2) - ((p.baseInt - 10) / 2) - ((p.baseWis - 10) / 2) - ((p.baseCha - 10) / 2));
                if (p.baseLuck < 3)
                {
                    p.baseLuck = 3;
                }
            }

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
