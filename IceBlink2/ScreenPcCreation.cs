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
        //public gv.module gv.mod;
        public GameView gv;

        private IbbButton btnName = null;
        private IbbButton btnRace = null;
        private IbbButton btnClass = null;
        private IbbButton btnGender = null;
        private IbbButton btnStrMinus = null;
        private IbbButton btnStrPlus = null;
        private IbbButton btnStr = null;
        private IbbButton btnDexMinus = null;
        private IbbButton btnDex = null;
        private IbbButton btnDexPlus = null;
        private IbbButton btnConMinus = null;
        private IbbButton btnCon = null;
        private IbbButton btnConPlus = null;
        private IbbButton btnIntMinus = null;
        private IbbButton btnInt = null;
        private IbbButton btnIntPlus = null;
        private IbbButton btnWisMinus = null;
        private IbbButton btnWis = null;
        private IbbButton btnWisPlus = null;
        private IbbButton btnChaMinus = null;
        private IbbButton btnCha = null;
        private IbbButton btnChaPlus = null;
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
            //gv.mod = m;
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
            gv.mod.counterPointsToDistributeLeft = gv.mod.pointPoolSize;
            pc = gv.cc.LoadPlayer(gv.mod.defaultPlayerFilename);
            pc.token = gv.cc.LoadBitmap(pc.tokenFilename);
            pc.portrait = gv.cc.LoadBitmap(pc.portraitFilename);
            pc.playerClass = gv.mod.getPlayerClass(pc.classTag);
            pc.race = this.getAllowedRace(pc.raceTag);
            pc.name = "CharacterName";
            pc.tag = "characterName";
            pcCreationIndex = 0;
            if (gv.mod.useManualPointDistribution)
            {
                gv.mod.counterPointsToDistributeLeft = gv.mod.pointPoolSize;
                pc.baseStr = gv.mod.attributeBaseValue;
                pc.baseDex = gv.mod.attributeBaseValue;
                pc.baseInt = gv.mod.attributeBaseValue;
                pc.baseCha = gv.mod.attributeBaseValue;
                pc.baseCon = gv.mod.attributeBaseValue;
                pc.baseWis = gv.mod.attributeBaseValue;
                gv.sf.UpdateStats(pc);
                pc.hp = pc.hpMax;
                pc.sp = pc.spMax;
            }
            else
            {
                if (gv.mod.useHybridRollPointDistribution)
                {
                    gv.mod.counterPointsToDistributeLeft = 0;
                }
                reRollStats(pc);
            }
        }
        public void CreateRaceList()
        {
            //Create Race List
            playerRaces.Clear();
            foreach (Race rc in gv.mod.moduleRacesList)
            {
                if (rc.UsableByPlayer)
                {
                    Race newRace = rc.DeepCopy();
                    newRace.classesAllowed.Clear();
                    foreach (string s in rc.classesAllowed)
                    {
                        PlayerClass plc = gv.mod.getPlayerClass(s);
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
                //Load from gv.module folder first
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
                //Load from gv.module folder first
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

            if (btnStrMinus == null)
            {
                btnStrMinus = new IbbButton(gv, 1.0f);
                btnStrMinus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnStrMinus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnStrMinus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 3*gv.squareSize - 3*pW;
                btnStrMinus.Y = 2 * gv.squareSize + gv.squareSize / 2;
                btnStrMinus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnStrMinus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (checkPhysical("Str"))
                {
                    btnStrMinus.Text = "- (" + calculateAttributeChangeCost("Str", false).ToString() + ")";
                }
                else
                {
                    btnStrMinus.Text = "NA";
                }
            }

            if (btnDexMinus == null)
            {
                btnDexMinus = new IbbButton(gv, 1.0f);
                btnDexMinus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnDexMinus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnDexMinus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 3 * gv.squareSize - 3 * pW;
                btnDexMinus.Y = 3 * gv.squareSize + gv.squareSize / 2 + pH;
                btnDexMinus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnDexMinus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (checkPhysical("Dex"))
                {
                    btnDexMinus.Text = "- (" + calculateAttributeChangeCost("Dex", false).ToString() + ")";
                }
                else
                {
                    btnDexMinus.Text = "NA";
                }
            }

            if (btnDex == null)
            {
                btnDex = new IbbButton(gv, 1.0f);
                btnDex.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnDex.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnDex.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 2 * gv.squareSize - 2 * pW;
                btnDex.Y = 3 * gv.squareSize + gv.squareSize / 2 + pH;
                btnDex.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnDex.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                btnDex.Text = "Dex";
            }

            if (btnDexPlus == null)
            {
                btnDexPlus = new IbbButton(gv, 1.0f);
                btnDexPlus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnDexPlus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnDexPlus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 1 * gv.squareSize - 1 * pW;
                btnDexPlus.Y = 3 * gv.squareSize + gv.squareSize / 2 + pH;
                btnDexPlus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnDexPlus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (pc.baseDex < gv.mod.attributeMaxValue)
                {
                    btnDexPlus.Text = "+ (" + calculateAttributeChangeCost("Dex", true).ToString() + ")";
                }
                else
                {
                    btnDexPlus.Text = "NA";
                }
            }

            if (btnConMinus == null)
            {
                btnConMinus = new IbbButton(gv, 1.0f);
                btnConMinus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnConMinus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnConMinus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 3 * gv.squareSize - 3 * pW;
                btnConMinus.Y = 4 * gv.squareSize + gv.squareSize / 2 + 2*pH;
                btnConMinus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnConMinus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (checkPhysical("Con"))
                {
                    btnConMinus.Text = "- (" + calculateAttributeChangeCost("Con", false).ToString() + ")";
                }
                else
                {
                    btnConMinus.Text = "NA";
                }
            }

            if (btnCon == null)
            {
                btnCon = new IbbButton(gv, 1.0f);
                btnCon.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnCon.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnCon.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 2 * gv.squareSize - 2 * pW;
                btnCon.Y = 4 * gv.squareSize + gv.squareSize / 2 + 2 * pH;
                btnCon.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnCon.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                btnCon.Text = "Con";
            }

            if (btnConPlus == null)
            {
                btnConPlus = new IbbButton(gv, 1.0f);
                btnConPlus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnConPlus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnConPlus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 1 * gv.squareSize - 1 * pW;
                btnConPlus.Y = 4 * gv.squareSize + gv.squareSize / 2 + 2 * pH;
                btnConPlus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnConPlus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (pc.baseCon < gv.mod.attributeMaxValue)
                {
                    btnConPlus.Text = "+ (" + calculateAttributeChangeCost("Con", true).ToString() + ")";
                }
                else
                {
                    btnConPlus.Text = "NA";
                }
            }

            if (btnIntMinus == null)
            {
                btnIntMinus = new IbbButton(gv, 1.0f);
                btnIntMinus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnIntMinus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnIntMinus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 3 * gv.squareSize - 3 * pW;
                btnIntMinus.Y = 5 * gv.squareSize + gv.squareSize / 2 + 3 * pH;
                btnIntMinus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnIntMinus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (checkMental("Int"))
                {
                    btnIntMinus.Text = "- (" + calculateAttributeChangeCost("Int", false).ToString() + ")";
                }
                else
                {
                    btnIntMinus.Text = "NA";
                }
            }

            if (btnInt == null)
            {
                btnInt = new IbbButton(gv, 1.0f);
                btnInt.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnInt.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnInt.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 2 * gv.squareSize - 2 * pW;
                btnInt.Y = 5 * gv.squareSize + gv.squareSize / 2 + 3 * pH;
                btnInt.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInt.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                btnInt.Text = "Int";
            }


            if (btnIntPlus == null)
            {
                btnIntPlus = new IbbButton(gv, 1.0f);
                btnIntPlus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnIntPlus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnIntPlus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 1 * gv.squareSize - 1 * pW;
                btnIntPlus.Y = 5 * gv.squareSize + gv.squareSize / 2 + 3 * pH;
                btnIntPlus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnIntPlus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (pc.baseInt < gv.mod.attributeMaxValue)
                {
                    btnIntPlus.Text = "+ (" + calculateAttributeChangeCost("Int", true).ToString() + ")";
                }
                else
                {
                    btnIntPlus.Text = "NA";
                }
            }

            if (btnWisMinus == null)
            {
                btnWisMinus = new IbbButton(gv, 1.0f);
                btnWisMinus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnWisMinus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnWisMinus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 3 * gv.squareSize - 3 * pW;
                btnWisMinus.Y = 6 * gv.squareSize + gv.squareSize / 2 + 4 * pH;
                btnWisMinus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnWisMinus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (checkMental("Wis"))
                {
                    btnWisMinus.Text = "- (" + calculateAttributeChangeCost("Wis", false).ToString() + ")";
                }
                else
                {
                    btnWisMinus.Text = "NA";
                }
            }

            if (btnWis == null)
            {
                btnWis = new IbbButton(gv, 1.0f);
                btnWis.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnWis.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnWis.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 2 * gv.squareSize - 2 * pW;
                btnWis.Y = 6 * gv.squareSize + gv.squareSize / 2 + 4 * pH;
                btnWis.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnWis.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                btnWis.Text = "Wis";
            }

            if (btnWisPlus == null)
            {
                btnWisPlus = new IbbButton(gv, 1.0f);
                btnWisPlus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnWisPlus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnWisPlus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 1 * gv.squareSize - 1 * pW;
                btnWisPlus.Y = 6 * gv.squareSize + gv.squareSize / 2 + 4 * pH;
                btnWisPlus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnWisPlus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (pc.baseWis < gv.mod.attributeMaxValue)
                {
                    btnWisPlus.Text = "+ (" + calculateAttributeChangeCost("Wis", true).ToString() + ")";
                }
                else
                {
                    btnWisPlus.Text = "NA";
                }
            }

            if (btnChaMinus == null)
            {
                btnChaMinus = new IbbButton(gv, 1.0f);
                btnChaMinus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnChaMinus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnChaMinus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 3 * gv.squareSize - 3 * pW;
                btnChaMinus.Y = 7 * gv.squareSize + gv.squareSize / 2 + 5 * pH;
                btnChaMinus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnChaMinus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (checkMental("Cha"))
                {
                    btnChaMinus.Text = "- (" + calculateAttributeChangeCost("Cha", false).ToString() + ")";
                }
                else
                {
                    btnChaMinus.Text = "NA";
                }
            }

            if (btnCha == null)
            {
                btnCha = new IbbButton(gv, 1.0f);
                btnCha.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnCha.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnCha.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 2 * gv.squareSize - 2 * pW;
                btnCha.Y = 7 * gv.squareSize + gv.squareSize / 2 + 5 * pH;
                btnCha.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnCha.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                btnCha.Text = "Cha";
            }

            if (btnChaPlus == null)
            {
                btnChaPlus = new IbbButton(gv, 1.0f);
                btnChaPlus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnChaPlus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnChaPlus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 1 * gv.squareSize - 1 * pW;
                btnChaPlus.Y = 7 * gv.squareSize + gv.squareSize / 2 + 5 * pH;
                btnChaPlus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnChaPlus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (pc.baseCha < gv.mod.attributeMaxValue)
                {
                    btnChaPlus.Text = "+ (" + calculateAttributeChangeCost("Cha", true).ToString() + ")";
                }
                else
                {
                    btnChaPlus.Text = "NA";
                }
            }

            if (btnStrPlus == null)
            {
                btnStrPlus = new IbbButton(gv, 1.0f);
                btnStrPlus.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnStrPlus.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnStrPlus.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 1 * gv.squareSize - 1 * pW;
                btnStrPlus.Y = 2 * gv.squareSize + gv.squareSize / 2;
                btnStrPlus.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnStrPlus.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                if (pc.baseStr < gv.mod.attributeMaxValue)
                {
                    btnStrPlus.Text = "+ (" + calculateAttributeChangeCost("Str", true).ToString() + ")";
                }
                else
                {
                    btnStrPlus.Text = "NA";
                }
            }

            if (btnStr == null)
            {
                btnStr = new IbbButton(gv, 1.0f);
                btnStr.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large);
                btnStr.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(gv.getResources(), R.drawable.btn_large_glow);
                btnStr.X = center - (int)(gv.ibbwidthL * gv.screenDensity) - pW * 1 - 2 * gv.squareSize - 2 * pW;
                btnStr.Y = 2 * gv.squareSize + gv.squareSize / 2;
                btnStr.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnStr.Width = (int)(gv.ibbwidthR * gv.screenDensity);
                btnStr.Text = "Str";
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
                btnAbort.Text = "Return";
                btnAbort.Img = gv.cc.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnAbort.Glow = gv.cc.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnAbort.X = 8 * gv.squareSize + padW * 1 + gv.oXshift;
                btnAbort.Y = 9 * gv.squareSize + pH * 2;
                btnAbort.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnAbort.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
        }

        public int calculateAttributeChangeCost (string attributeName, bool raise)
        {
            int cost = 1;
            if (!raise)
            {
                if (attributeName == "Str")
                {
                    if (pc.baseStr < gv.mod.twoPointThreshold)
                    {
                        cost = 1;
                    }
                    else if (pc.baseStr < gv.mod.threePointThreshold)
                    {
                        cost = 2;
                    }
                    else if (pc.baseStr < gv.mod.fourPointThreshold)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
                if (attributeName == "Dex")
                {
                    if (pc.baseDex < gv.mod.twoPointThreshold)
                    {
                        cost = 1;
                    }
                    else if (pc.baseDex < gv.mod.threePointThreshold)
                    {
                        cost = 2;
                    }
                    else if (pc.baseDex < gv.mod.fourPointThreshold)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
                if (attributeName == "Con")
                {
                    if (pc.baseCon < gv.mod.twoPointThreshold)
                    {
                        cost = 1;
                    }
                    else if (pc.baseCon < gv.mod.threePointThreshold)
                    {
                        cost = 2;
                    }
                    else if (pc.baseCon < gv.mod.fourPointThreshold)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
                if (attributeName == "Int")
                {
                    if (pc.baseInt < gv.mod.twoPointThreshold)
                    {
                        cost = 1;
                    }
                    else if (pc.baseInt < gv.mod.threePointThreshold)
                    {
                        cost = 2;
                    }
                    else if (pc.baseInt < gv.mod.fourPointThreshold)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
                if (attributeName == "Wis")
                {
                    if (pc.baseWis < gv.mod.twoPointThreshold)
                    {
                        cost = 1;
                    }
                    else if (pc.baseWis < gv.mod.threePointThreshold)
                    {
                        cost = 2;
                    }
                    else if (pc.baseWis < gv.mod.fourPointThreshold)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
                if (attributeName == "Cha")
                {
                    if (pc.baseCha < gv.mod.twoPointThreshold)
                    {
                        cost = 1;
                    }
                    else if (pc.baseCha < gv.mod.threePointThreshold)
                    {
                        cost = 2;
                    }
                    else if (pc.baseCha < gv.mod.fourPointThreshold)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
            }
            //raise
            else
            {
                if (attributeName == "Str")
                {
                    if (pc.baseStr < gv.mod.twoPointThreshold-1)
                    {
                        cost = 1;
                    }
                    else if (pc.baseStr < gv.mod.threePointThreshold-1)
                    {
                        cost = 2;
                    }
                    else if (pc.baseStr < gv.mod.fourPointThreshold-1)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
                if (attributeName == "Dex")
                {
                    if (pc.baseDex < gv.mod.twoPointThreshold-1)
                    {
                        cost = 1;
                    }
                    else if (pc.baseDex < gv.mod.threePointThreshold-1)
                    {
                        cost = 2;
                    }
                    else if (pc.baseDex < gv.mod.fourPointThreshold-1)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
                if (attributeName == "Con")
                {
                    if (pc.baseCon < gv.mod.twoPointThreshold-1)
                    {
                        cost = 1;
                    }
                    else if (pc.baseCon < gv.mod.threePointThreshold-1)
                    {
                        cost = 2;
                    }
                    else if (pc.baseCon < gv.mod.fourPointThreshold-1)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
                if (attributeName == "Int")
                {
                    if (pc.baseInt < gv.mod.twoPointThreshold-1)
                    {
                        cost = 1;
                    }
                    else if (pc.baseInt < gv.mod.threePointThreshold-1)
                    {
                        cost = 2;
                    }
                    else if (pc.baseInt < gv.mod.fourPointThreshold-1)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
                if (attributeName == "Wis")
                {
                    if (pc.baseWis < gv.mod.twoPointThreshold-1)
                    {
                        cost = 1;
                    }
                    else if (pc.baseWis < gv.mod.threePointThreshold-1)
                    {
                        cost = 2;
                    }
                    else if (pc.baseWis < gv.mod.fourPointThreshold-1)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
                if (attributeName == "Cha")
                {
                    if (pc.baseCha < gv.mod.twoPointThreshold-1)
                    {
                        cost = 1;
                    }
                    else if (pc.baseCha < gv.mod.threePointThreshold-1)
                    {
                        cost = 2;
                    }
                    else if (pc.baseCha < gv.mod.fourPointThreshold-1)
                    {
                        cost = 3;
                    }
                    else
                    {
                        cost = 4;
                    }
                }
            }

            return cost;
        }

        public void redrawPcCreation()
        {
            //Player pc = gv.mod.playerList.get(0);
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
            if (gv.mod.ArmorClassAscending) { actext = pc.AC; }
            else { actext = 20 - pc.AC; }
            if (gv.mod.useOrbitronFont == true)
            {
                if (gv.mod.useManualPointDistribution)
                {
                    int backupLocY = locY;
                    string infoText2 = "Points available: " + gv.mod.counterPointsToDistributeLeft;
                    gv.DrawText(infoText2, locX + pW - 3 * gv.squareSize - 2 * pW, locY = (spacing) + locY - 2 * gv.squareSize - 4 * pH);
                    locY = backupLocY;

                    //string infoText = ", Points available: " + gv.mod.counterPointsToDistributeLeft;
                    string infoText = "Min: " + gv.mod.attributeMinValue;
                    infoText += ", Max: " + gv.mod.attributeMaxValue;
                    if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed < 3)
                    {
                        infoText += ", Physical below " + gv.mod.attributeBaseValue + " allowed: " + gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed;
                    }
                    if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed < 3)
                    {
                        infoText += ", Mental below " + gv.mod.attributeBaseValue + " allowed: " + gv.mod.numberOfMentalAtttributesBelowBaseAllowed;
                    }

                    gv.DrawText(infoText, locX + pW, locY += (spacing));
                }
                else if (gv.mod.useHybridRollPointDistribution)
                {
                    int backupLocY = locY;
                    string infoText2 = "Points available: " + gv.mod.counterPointsToDistributeLeft;
                    gv.DrawText(infoText2, locX + pW - 3 * gv.squareSize - 2 * pW, locY = (spacing) + locY - 2 * gv.squareSize - 4 * pH);
                    locY = backupLocY;

                    string infoText = "";
                    if (gv.mod.use3d6 && gv.mod.useLuck)
                    {
                        infoText += "Rolling: 3d6, Luck is high for those who need it";
                    }
                    else if (gv.mod.use3d6 && !gv.mod.useLuck)
                    {
                        infoText += "Rolling: 3d6";
                    }
                    else if (!gv.mod.use3d6 && gv.mod.useLuck)
                    {
                        infoText += "Rolling: 6 + d12, Luck is high for those who need it";
                    }
                    else if (!gv.mod.use3d6 && !gv.mod.useLuck)
                    {
                        infoText += "Rolling: 6 + d12";

                    }
                    //infoText += "Points available: " + gv.mod.counterPointsToDistributeLeft;
                    infoText += ", Min: " + gv.mod.attributeMinValue;
                    infoText += ", Max: " + gv.mod.attributeMaxValue;
                    /*
                    if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed < 3)
                    {
                        infoText += ", Physical below " + gv.mod.attributeBaseValue + " allowed: " + gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed;
                    }
                    if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed < 3)
                    {
                        infoText += ", Mental below " + gv.mod.attributeBaseValue + " allowed: " + gv.mod.numberOfMentalAtttributesBelowBaseAllowed;
                    }
                    */

                    gv.DrawText(infoText, locX + pW, locY += (spacing));
                }
                else if (gv.mod.use3d6 && gv.mod.useLuck)
                {
                    gv.DrawText("Rolling: 3d6, Luck is high for those who need it", locX + pW, locY += (spacing));
                }
                else if (gv.mod.use3d6 && !gv.mod.useLuck)
                {
                    gv.DrawText("Rolling: 3d6", locX + pW, locY += (spacing));
                }
                else if (!gv.mod.use3d6 && gv.mod.useLuck)
                {
                    gv.DrawText("Rolling: 6 + d12, Luck is high for those who need it", locX + pW, locY += (spacing));
                }
                else if (!gv.mod.use3d6 && !gv.mod.useLuck)
                {
                    gv.DrawText("Rolling: 6 + d12", locX + pW, locY += (spacing));

                }
                //STR              
                gv.DrawText("STR:", locX + pW, locY += (spacing * 2));
                gv.DrawText(pc.baseStr.ToString(), locX + 3 * pW * 2, locY);
                if (pc.strength - pc.baseStr >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                int racial = pc.strength - pc.baseStr;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.strength.ToString(), locX + 13 * pW, locY);
                if (((pc.strength - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.strength - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.strength - 10) / 2) + ")", locX + 15 * pW, locY);
                }

                gv.DrawText("AC: " + actext, tabX2, locY2 += (spacing * 3));
                gv.DrawText("BAB: " + pc.baseAttBonus + ", Melee to hit/damage: " + (pc.baseAttBonus + ((pc.strength - 10) / 2)) + "/" + ((pc.strength - 10) / 2) + ", Ranged to hit: " + (pc.baseAttBonus + ((pc.dexterity - 10) / 2)), tabX2, locY2 += spacing);
                //DEX             
                gv.DrawText("DEX:", locX + pW, locY += (spacing));
                gv.DrawText(pc.baseDex.ToString(), locX + 3 * pW * 2, locY);
                if (pc.dexterity - pc.baseDex >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                racial = pc.dexterity - pc.baseDex;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.dexterity.ToString(), locX + 13 * pW, locY);
                if (((pc.dexterity - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.dexterity - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.dexterity - 10) / 2) + ")", locX + 15 * pW, locY);
                }

                gv.DrawText("HP: " + pc.hp + "/" + pc.hpMax, tabX2, locY2 += spacing);
                //CON             
                gv.DrawText("CON:", locX + pW, locY += (spacing));
                gv.DrawText(pc.baseCon.ToString(), locX + 3 * pW * 2, locY);
                if (pc.constitution - pc.baseCon >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                racial = pc.constitution - pc.baseCon;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.constitution.ToString(), locX + 13 * pW, locY);
                if (((pc.constitution - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.constitution - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.constitution - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, tabX2, locY2 += spacing);
                //INT             
                gv.DrawText("INT:", locX + pW, locY += (spacing));
                gv.DrawText(pc.baseInt.ToString(), locX + 3 * pW * 2, locY);
                if (pc.intelligence - pc.baseInt >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                racial = pc.intelligence - pc.baseInt;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.intelligence.ToString(), locX + 13 * pW, locY);
                if (((pc.intelligence - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.intelligence - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.intelligence - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                gv.DrawText("FORT: " + pc.fortitude + ", Acid: " + pc.damageTypeResistanceTotalAcid + "%" + ", Cold: " + pc.damageTypeResistanceTotalCold + "%" + ", Normal: " + pc.damageTypeResistanceTotalNormal + "%", tabX2, locY2 += spacing);
                gv.DrawText("REF:   " + pc.reflex + ", Electricity: " + pc.damageTypeResistanceTotalElectricity + "%" + ", Fire: " + pc.damageTypeResistanceTotalFire + "%", tabX2, locY2 += spacing);
                gv.DrawText("WILL: " + pc.will + ", Magic: " + pc.damageTypeResistanceTotalMagic + "%" + ", Poison: " + pc.damageTypeResistanceTotalPoison + "%", tabX2, locY2 += spacing);
                //WIS             
                gv.DrawText("WIS:", locX + pW, locY += (spacing));
                gv.DrawText(pc.baseWis.ToString(), locX + 3 * pW * 2, locY);
                if (pc.wisdom - pc.baseWis >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                racial = pc.wisdom - pc.baseWis;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.wisdom.ToString(), locX + 13 * pW, locY);
                if (((pc.wisdom - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.wisdom - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.wisdom - 10) / 2) + ")", locX + 15 * pW, locY);
                }

                //CHA             
                gv.DrawText("CHA:", locX + pW, locY += (spacing));
                gv.DrawText(pc.baseCha.ToString(), locX + 3 * pW * 2, locY);
                if (pc.charisma - pc.baseCha >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                racial = pc.charisma - pc.baseCha;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.charisma.ToString(), locX + 13 * pW, locY);
                if (((pc.charisma - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.charisma - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.charisma - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                if (gv.mod.useLuck)
                {
                    if (((pc.luck - 10) / 2) > 0)
                    {
                        gv.DrawText("LCK:  " + pc.baseLuck + " + " + (pc.luck - pc.baseLuck) + " = " + pc.luck, locX + pW, locY += spacing);
                    }
                    else
                    {
                        gv.DrawText("LCK:  " + pc.baseLuck + " + " + (pc.luck - pc.baseLuck) + " = " + pc.luck, locX + pW, locY += spacing);

                    }
                }
            }
            
            else
            {
                if (gv.mod.useManualPointDistribution)
                {
                    int backupLocY = locY;
                    string infoText2 = "Points available: " + gv.mod.counterPointsToDistributeLeft;
                    gv.DrawText(infoText2, locX + pW - 3 * gv.squareSize - 2 * pW, locY = (spacing) + locY - 2 * gv.squareSize - 4 * pH);
                    locY = backupLocY;

                    //string infoText = "Points available: " + gv.mod.counterPointsToDistributeLeft;
                    string infoText = "Min: " + gv.mod.attributeMinValue;
                    infoText += ", Max: " + gv.mod.attributeMaxValue;
                    if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed < 3)
                    {
                        infoText += ", Physical below " + gv.mod.attributeBaseValue + ": " + gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed;
                    }
                    if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed < 3)
                    {
                        infoText += ", Mental below " + gv.mod.attributeBaseValue + ": " + gv.mod.numberOfMentalAtttributesBelowBaseAllowed;
                    }

                    gv.DrawText(infoText, locX + pW, locY += (spacing));
                }
                else if (gv.mod.useHybridRollPointDistribution)
                {
                    int backupLocY = locY;
                    string infoText2 = "Points available: " + gv.mod.counterPointsToDistributeLeft;
                    gv.DrawText(infoText2, locX + pW - 3*gv.squareSize - 2*pW, locY = (spacing) + locY - 2*gv.squareSize - 4*pH);
                    locY = backupLocY;

                    string infoText = "";
                    if (gv.mod.use3d6 && gv.mod.useLuck)
                    { 
                        infoText += "Rolling: 3d6, Luck is high for those who need it";
                    }
                    else if (gv.mod.use3d6 && !gv.mod.useLuck)
                    {
                        infoText += "Rolling: 3d6";
                    }
                    else if (!gv.mod.use3d6 && gv.mod.useLuck)
                    {
                        infoText += "Rolling: 6 + d12, Luck is high for those who need it";
                    }
                    else if (!gv.mod.use3d6 && !gv.mod.useLuck)
                    {
                        infoText += "Rolling: 6 + d12";

                    }
                    //infoText += ", Points available: " + gv.mod.counterPointsToDistributeLeft;
                    infoText += ", Min: " + gv.mod.attributeMinValue;
                    infoText += ", Max: " + gv.mod.attributeMaxValue;
                    /*
                    if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed < 3)
                    {
                        infoText += ", Physical below " + gv.mod.attributeBaseValue + ": " + gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed;
                    }
                    if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed < 3)
                    {
                        infoText += ", Mental below " + gv.mod.attributeBaseValue + ": " + gv.mod.numberOfMentalAtttributesBelowBaseAllowed;
                    }
                    */

                    gv.DrawText(infoText, locX + pW, locY += (spacing));
                }
                else if (gv.mod.use3d6 && gv.mod.useLuck)
                {
                    gv.DrawText("Rolling: 3d6, Luck is high for those who need it", locX + pW, locY += (spacing));
                }
                else if (gv.mod.use3d6 && !gv.mod.useLuck)
                {
                    gv.DrawText("Rolling: 3d6", locX + pW, locY += (spacing));
                }
                else if (!gv.mod.use3d6 && gv.mod.useLuck)
                {
                    gv.DrawText("Rolling: 6 + d12, Luck is high for those who need it", locX + pW, locY += (spacing));
                }
                else if (!gv.mod.use3d6 && !gv.mod.useLuck)
                {
                    gv.DrawText("Rolling: 6 + d12", locX + pW, locY += (spacing));

                }


                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx
                
                //STR              
                    gv.DrawText("STR:" , locX + pW, locY += (spacing * 2));
                    gv.DrawText(pc.baseStr.ToString(), locX + 3*pW*2, locY);
                    if (pc.strength - pc.baseStr >= 0)
                    {
                        gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                    }
                    else
                    {
                        gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                    }
                    int racial = pc.strength - pc.baseStr;
                    if (racial < 0)
                    {
                        racial *= -1;
                    }
                    gv.DrawText(racial.ToString(), locX + 5*pW*2, locY);
                    gv.DrawText(" = ", locX + 11*pW, locY);
                    gv.DrawText(pc.strength.ToString(), locX + 13*pW, locY);
                    if (((pc.strength - 10) / 2) > 0)
                    {
                        gv.DrawText(" (+" + ((pc.strength - 10) / 2) + ")", locX + 15 * pW, locY);
                    }
                    else
                    {
                        gv.DrawText(" (" + ((pc.strength - 10) / 2) + ")", locX + 15 * pW, locY);
                    }

                gv.DrawText("AC: " + actext, tabX2, locY2 += (spacing * 3));
                gv.DrawText("BAB: " + pc.baseAttBonus + ", Melee to hit/damage: " + (pc.baseAttBonus + ((pc.strength - 10) / 2)) + "/" + ((pc.strength - 10) / 2) + ", Ranged to hit: " + (pc.baseAttBonus + ((pc.dexterity - 10) / 2)), tabX2, locY2 += spacing);
                //DEX             
                gv.DrawText("DEX:", locX + pW, locY += (spacing));
                gv.DrawText(pc.baseDex.ToString(), locX + 3 * pW * 2, locY);
                if (pc.dexterity - pc.baseDex >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                racial = pc.dexterity - pc.baseDex;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.dexterity.ToString(), locX + 13 * pW, locY);
                if (((pc.dexterity - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.dexterity - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.dexterity - 10) / 2) + ")", locX + 15 * pW, locY);
                }

                pc.hp = pc.hpMax;
                gv.DrawText("HP: " + pc.hp + "/" + pc.hpMax, tabX2, locY2 += spacing);
                //CON             
                gv.DrawText("CON:", locX + pW, locY += (spacing));
                gv.DrawText(pc.baseCon.ToString(), locX + 3 * pW * 2, locY);
                if (pc.constitution - pc.baseCon >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                racial = pc.constitution - pc.baseCon;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.constitution.ToString(), locX + 13 * pW, locY);
                if (((pc.constitution - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.constitution - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.constitution - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                pc.sp = pc.spMax;
                gv.DrawText("SP: " + pc.sp + "/" + pc.spMax, tabX2, locY2 += spacing);
                //INT             
                gv.DrawText("INT:", locX + pW, locY += (spacing));
                gv.DrawText(pc.baseInt.ToString(), locX + 3 * pW * 2, locY);
                if (pc.intelligence - pc.baseInt >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                racial = pc.intelligence - pc.baseInt;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.intelligence.ToString(), locX + 13 * pW, locY);
                if (((pc.intelligence - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.intelligence - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.intelligence - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                gv.DrawText("FORT: " + pc.fortitude + ", Acid: " + pc.damageTypeResistanceTotalAcid + "%" + ", Cold: " + pc.damageTypeResistanceTotalCold + "%" + ", Normal: " + pc.damageTypeResistanceTotalNormal + "%", tabX2, locY2 += spacing);
                gv.DrawText("REF:   " + pc.reflex + ", Electricity: " + pc.damageTypeResistanceTotalElectricity + "%" + ", Fire: " + pc.damageTypeResistanceTotalFire + "%", tabX2, locY2 += spacing);
                gv.DrawText("WILL: " + pc.will + ", Magic: " + pc.damageTypeResistanceTotalMagic + "%" + ", Poison: " + pc.damageTypeResistanceTotalPoison + "%", tabX2, locY2 += spacing);
                //WIS             
                gv.DrawText("WIS:", locX + pW, locY += (spacing));
                gv.DrawText(pc.baseWis.ToString(), locX + 3 * pW * 2, locY);
                if (pc.wisdom - pc.baseWis >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                racial = pc.wisdom - pc.baseWis;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.wisdom.ToString(), locX + 13 * pW, locY);
                if (((pc.wisdom - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.wisdom - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.wisdom - 10) / 2) + ")", locX + 15 * pW, locY);
                }

                //CHA             
                gv.DrawText("CHA:", locX + pW, locY += (spacing));
                gv.DrawText(pc.baseCha.ToString(), locX + 3 * pW * 2, locY);
                if (pc.charisma - pc.baseCha >= 0)
                {
                    gv.DrawText(" + ", locX + 4 * pW * 2, locY);
                }
                else
                {
                    gv.DrawText(" - ", locX + 4 * pW * 2, locY);
                }
                racial = pc.charisma - pc.baseCha;
                if (racial < 0)
                {
                    racial *= -1;
                }
                gv.DrawText(racial.ToString(), locX + 5 * pW * 2, locY);
                gv.DrawText(" = ", locX + 11 * pW, locY);
                gv.DrawText(pc.charisma.ToString(), locX + 13 * pW, locY);
                if (((pc.charisma - 10) / 2) > 0)
                {
                    gv.DrawText(" (+" + ((pc.charisma - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                else
                {
                    gv.DrawText(" (" + ((pc.charisma - 10) / 2) + ")", locX + 15 * pW, locY);
                }
                if (gv.mod.useLuck)
                {
                    if (((pc.luck - 10) / 2) > 0)
                    {
                        gv.DrawText("LCK:  " + pc.baseLuck + " + " + (pc.luck - pc.baseLuck) + " = " + pc.luck, locX + pW, locY += spacing);
                    }
                    else
                    {
                        gv.DrawText("LCK:  " + pc.baseLuck + " + " + (pc.luck - pc.baseLuck) + " = " + pc.luck, locX + pW, locY += spacing);

                    }
                }

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX


                /*
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
                if (gv.mod.useLuck)
                {
                    gv.DrawText("LCK:  " + pc.baseLuck + " + " + (pc.luck - pc.baseLuck) + " = " + pc.luck, locX + pW, locY += spacing);
                }
                */

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
            if (gv.mod.useManualPointDistribution || gv.mod.useHybridRollPointDistribution)
            {
                if (checkPhysical("Str"))
                {
                    btnStrMinus.Text = "- (" + calculateAttributeChangeCost("Str", false).ToString() + ")";
                }
                else
                {
                    btnStrMinus.Text = "NA";
                }
                btnStrMinus.Draw();
                btnStr.Draw();
                if (pc.baseStr < gv.mod.attributeMaxValue)
                {
                    btnStrPlus.Text = "+ (" + calculateAttributeChangeCost("Str", true).ToString() + ")";
                }
                else
                {
                    btnStrPlus.Text = "NA";
                }
                btnStrPlus.Draw();
                if (checkPhysical("Dex"))
                {
                    btnDexMinus.Text = "- (" + calculateAttributeChangeCost("Dex", false).ToString() + ")";
                }
                else
                {
                    btnDexMinus.Text = "NA";
                }
                btnDexMinus.Draw();

                btnDex.Draw();
                if (pc.baseDex < gv.mod.attributeMaxValue)
                {
                    btnDexPlus.Text = "+ (" + calculateAttributeChangeCost("Dex", true).ToString() + ")";
                }
                else
                {
                    btnDexPlus.Text = "NA";
                }
                btnDexPlus.Draw();
                if (checkPhysical("Con"))
                {
                    btnConMinus.Text = "- (" + calculateAttributeChangeCost("Con", false).ToString() + ")";
                }
                else
                {
                    btnConMinus.Text = "NA";
                }
                btnConMinus.Draw();
                btnCon.Draw();
                if (pc.baseCon < gv.mod.attributeMaxValue)
                {
                    btnConPlus.Text = "+ (" + calculateAttributeChangeCost("Con", true).ToString() + ")";
                }
                else
                {
                    btnConPlus.Text = "NA";
                }
                btnConPlus.Draw();
                if (checkMental("Int"))
                {
                    btnIntMinus.Text = "- (" + calculateAttributeChangeCost("Int", false).ToString() + ")";
                }
                else
                {
                    btnIntMinus.Text = "NA";
                }
                btnIntMinus.Draw();
                btnInt.Draw();
                if (pc.baseInt < gv.mod.attributeMaxValue)
                {
                    btnIntPlus.Text = "+ (" + calculateAttributeChangeCost("Int", true).ToString() + ")";
                }
                else
                {
                    btnIntPlus.Text = "NA";
                }
                btnIntPlus.Draw();
                if (checkMental("Wis"))
                {
                    btnWisMinus.Text = "- (" + calculateAttributeChangeCost("Wis", false).ToString() + ")";
                }
                else
                {
                    btnWisMinus.Text = "NA";
                }
                btnWisMinus.Draw();
                btnWis.Draw();
                if (pc.baseWis < gv.mod.attributeMaxValue)
                {
                    btnWisPlus.Text = "+ (" + calculateAttributeChangeCost("Wis", true).ToString() + ")";
                }
                else
                {
                    btnWisPlus.Text = "NA";
                }
                btnWisPlus.Draw();
                if (checkMental("Cha"))
                {
                    btnChaMinus.Text = "- (" + calculateAttributeChangeCost("Cha", false).ToString() + ")";
                }
                else
                {
                    btnChaMinus.Text = "NA";
                }
                btnChaMinus.Draw();
                btnCha.Draw();
                if (pc.baseCha < gv.mod.attributeMaxValue)
                {
                    btnChaPlus.Text = "+ (" + calculateAttributeChangeCost("Cha", true).ToString() + ")";
                }
                else
                {
                    btnChaPlus.Text = "NA";
                }
                btnChaPlus.Draw();
            }
            btnClass.Text = pc.playerClass.name;
            btnClass.Draw();
            if (!gv.mod.useManualPointDistribution)
            {
                btnRollStats.Draw();
            }
            btnFinished.Draw();
            gv.cc.btnHelp.Draw();
            btnAbort.Draw();
            btnPlayerGuideOnPcCreation.Draw();
            btnBeginnerGuideOnPcCreation.Draw();
        }
        public void onTouchPcCreation(MouseEventArgs e, MouseEventType.EventType eventType)
        {
            try
            {
                btnRollStats.glowOn = false;
                btnFinished.glowOn = false;
                btnAbort.glowOn = false;
                gv.cc.btnHelp.glowOn = false;
                btnPlayerGuideOnPcCreation.glowOn = false;
                btnBeginnerGuideOnPcCreation.glowOn = false;
                btnClass.glowOn = false;
                btnRace.glowOn = false;
                btnStrMinus.glowOn = false;
                btnStrPlus.glowOn = false;
                btnStr.glowOn = false;
                btnDexMinus.glowOn = false;
                btnDex.glowOn = false;
                btnDexPlus.glowOn = false;
                btnConMinus.glowOn = false;
                btnCon.glowOn = false;
                btnConPlus.glowOn = false;
                btnIntMinus.glowOn = false;
                btnInt.glowOn = false;
                btnIntPlus.glowOn = false;
                btnWisMinus.glowOn = false;
                btnWis.glowOn = false;
                btnWisPlus.glowOn = false;
                btnChaMinus.glowOn = false;
                btnCha.glowOn = false;
                btnChaPlus.glowOn = false;
                btnGender.glowOn = false;
                btnName.glowOn = false;

                switch (eventType)
                {
                    case MouseEventType.EventType.MouseDown:
                    case MouseEventType.EventType.MouseMove:
                        int x = (int)e.X;
                        int y = (int)e.Y;
                        if (!gv.mod.useManualPointDistribution)
                        {
                                if (btnRollStats.getImpact(x, y))
                                {
                                    btnRollStats.glowOn = true;
                                }   
                        }
                        if (btnFinished.getImpact(x, y))
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

                        if (gv.mod.useManualPointDistribution)
                        {
                            if (btnStrMinus.getImpact(x, y))
                            {
                                btnStrMinus.glowOn = true;
                            }
                            else if (btnStrPlus.getImpact(x, y))
                            {
                                btnStrPlus.glowOn = true;
                            }
                            else if (btnStr.getImpact(x, y))
                            {
                                btnStr.glowOn = true;
                            }
                            else if (btnDexMinus.getImpact(x, y))
                            {
                                btnDexMinus.glowOn = true;
                            }
                            else if (btnDex.getImpact(x, y))
                            {
                                btnDex.glowOn = true;
                            }
                            else if (btnDexPlus.getImpact(x, y))
                            {
                                btnDexPlus.glowOn = true;
                            }
                            else if (btnConMinus.getImpact(x, y))
                            {
                                btnConMinus.glowOn = true;
                            }
                            else if (btnCon.getImpact(x, y))
                            {
                                btnCon.glowOn = true;
                            }
                            else if (btnConPlus.getImpact(x, y))
                            {
                                btnConPlus.glowOn = true;
                            }
                            else if (btnIntMinus.getImpact(x, y))
                            {
                                btnIntMinus.glowOn = true;
                            }
                            else if (btnInt.getImpact(x, y))
                            {
                                btnInt.glowOn = true;
                            }
                            else if (btnIntPlus.getImpact(x, y))
                            {
                                btnIntPlus.glowOn = true;
                            }
                            else if (btnWisMinus.getImpact(x, y))
                            {
                                btnWisMinus.glowOn = true;
                            }
                            else if (btnWis.getImpact(x, y))
                            {
                                btnWis.glowOn = true;
                            }
                            else if (btnWisPlus.getImpact(x, y))
                            {
                                btnWisPlus.glowOn = true;
                            }
                            else if (btnChaMinus.getImpact(x, y))
                            {
                                btnChaMinus.glowOn = true;
                            }
                            else if (btnCha.getImpact(x, y))
                            {
                                btnCha.glowOn = true;
                            }
                            else if (btnChaPlus.getImpact(x, y))
                            {
                                btnChaPlus.glowOn = true;
                            }
                        }
                        if (btnName.getImpact(x, y))
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
                        btnStrMinus.glowOn = false;
                        btnStr.glowOn = false;
                        btnStrPlus.glowOn = false;
                        btnDexMinus.glowOn = false;
                        btnDex.glowOn = false;
                        btnDexPlus.glowOn = false;
                        btnConMinus.glowOn = false;
                        btnCon.glowOn = false;
                        btnConPlus.glowOn = false;
                        btnIntMinus.glowOn = false;
                        btnInt.glowOn = false;
                        btnIntPlus.glowOn = false;
                        btnWisMinus.glowOn = false;
                        btnWis.glowOn = false;
                        btnWisPlus.glowOn = false;
                        btnChaMinus.glowOn = false;
                        btnCha.glowOn = false;
                        btnChaPlus.glowOn = false;
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
                        if ( gv.mod.useManualPointDistribution || gv.mod.useHybridRollPointDistribution)
                        {
                            if (btnStrMinus.getImpact(x, y))
                            {
                                lowerAttribute("Str");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnStr.getImpact(x, y))
                            {
                                gv.sf.MessageBoxHtml(gv.cc.stringStrength);
                                gv.PlaySound("btn_click");
                            }
                            else if (btnStrPlus.getImpact(x, y))
                            {
                                raiseAttribute("Str");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnDexMinus.getImpact(x, y))
                            {
                                lowerAttribute("Dex");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnDex.getImpact(x, y))
                            {
                                gv.sf.MessageBoxHtml(gv.cc.stringDexterity);
                                gv.PlaySound("btn_click");
                            }
                            else if (btnDexPlus.getImpact(x, y))
                            {
                                raiseAttribute("Dex");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnConMinus.getImpact(x, y))
                            {
                                lowerAttribute("Con");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnCon.getImpact(x, y))
                            {
                                gv.sf.MessageBoxHtml(gv.cc.stringConstitution);
                                gv.PlaySound("btn_click");
                            }
                            else if (btnConPlus.getImpact(x, y))
                            {
                                raiseAttribute("Con");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnIntMinus.getImpact(x, y))
                            {
                                lowerAttribute("Int");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnInt.getImpact(x, y))
                            {
                                gv.sf.MessageBoxHtml(gv.cc.stringIntelligence);
                                gv.PlaySound("btn_click");
                            }
                            else if (btnIntPlus.getImpact(x, y))
                            {
                                raiseAttribute("Int");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnWisMinus.getImpact(x, y))
                            {
                                lowerAttribute("Wis");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnWis.getImpact(x, y))
                            {
                                gv.sf.MessageBoxHtml(gv.cc.stringWisdom);
                                gv.PlaySound("btn_click");
                            }
                            else if (btnWisPlus.getImpact(x, y))
                            {
                                raiseAttribute("Wis");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnChaMinus.getImpact(x, y))
                            {
                                lowerAttribute("Cha");
                                gv.PlaySound("btn_click");
                            }
                            else if (btnCha.getImpact(x, y))
                            {
                                gv.sf.MessageBoxHtml(gv.cc.stringCharisma);
                                gv.PlaySound("btn_click");
                            }
                            else if (btnChaPlus.getImpact(x, y))
                            {
                                raiseAttribute("Cha");
                                gv.PlaySound("btn_click");
                            }
                        }
                        if (btnClass.getImpact(x, y))
                        {
                            gv.PlaySound("btn_click");
                            pcCreationIndex = 4;
                            pcClassSelectionIndex++;
                            if (pcClassSelectionIndex >= pc.race.classesAllowed.Count)
                            {
                                pcClassSelectionIndex = 0;
                            }
                            pc.playerClass = gv.mod.getPlayerClass(pc.race.classesAllowed[pcClassSelectionIndex]);
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

                        if (!gv.mod.useManualPointDistribution)
                        {
                            if (btnRollStats.getImpact(x, y))
                            {
                                gv.PlaySound("btn_click");
                                reRollStats(pc);
                            }
                            
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
                                                                /*
                                                                bool traitWorksForThisPC = false;
                                                                //add conditional requirements
                                                                if (t.traitWorksOnlyWhen.Count <= 0)
                                                                {
                                                                    traitWorksForThisPC = true;
                                                                }

                                                                //note that the tratNeccessities are logically connected with OR the way it is setup
                                                                else
                                                                    foreach (LocalImmunityString traitNeccessity in t.traitWorksOnlyWhen)
                                                                    {
                                                                        foreach (string pcTag in pc.pcTags)
                                                                        {
                                                                            if (traitNeccessity.Value.Equals(pcTag))
                                                                            {
                                                                                traitWorksForThisPC = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }

                                                                //one redFlag is enough to stop the trait from working, ie connected with OR, too
                                                                if (traitWorksForThisPC)
                                                                {
                                                                    foreach (LocalImmunityString traitRedFlag in t.traitWorksNeverWhen)
                                                                    {
                                                                        foreach (string pcTag in pc.pcTags)
                                                                        {
                                                                            if (traitRedFlag.Value.Equals(pcTag))
                                                                            {
                                                                                traitWorksForThisPC = false;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                */
                                                                //end of conditional code
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
                                                                        pc.hp += ef.modifyCon / 2;
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

                                if ((traitTagsList.Count > 0) && (pc.playerClass.traitsToLearnAtLevelTable[pc.classLevel] > 0) && pc.getTraitsToLearn(gv.mod).Count > 0)
                                {
                                    gv.screenTraitLevelUp.resetPC(false, pc);
                                    gv.screenType = "learnTraitCreation";
                                }

                                else if ((spellTagsList.Count > 0) && (pc.playerClass.spellsToLearnAtLevelTable[pc.classLevel] > 0) && pc.getSpellsToLearn().Count > 0)
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

        public bool checkMental(string attribute)
        {
            if (attribute == "Int" && pc.baseInt > gv.mod.attributeMinValue)
            {
                if (gv.mod.useHybridRollPointDistribution)
                {
                    return true;
                }
                if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed == 2)
                {
                    if (pc.baseCha >= gv.mod.attributeBaseValue || pc.baseWis >= gv.mod.attributeBaseValue || pc.baseInt > gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed == 1)
                {
                    if ((pc.baseCha >= gv.mod.attributeBaseValue && pc.baseWis >= gv.mod.attributeBaseValue) || (pc.baseCha >= gv.mod.attributeBaseValue && pc.baseInt > gv.mod.attributeBaseValue) || (pc.baseWis >= gv.mod.attributeBaseValue && pc.baseInt > gv.mod.attributeBaseValue))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed == 0)
                {
                    if (pc.baseCha >= gv.mod.attributeBaseValue && pc.baseWis >= gv.mod.attributeBaseValue && pc.baseInt > gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            if (attribute == "Wis" && pc.baseWis > gv.mod.attributeMinValue)
            {
                if (gv.mod.useHybridRollPointDistribution)
                {
                    return true;
                }
                if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed == 2)
                {
                    if (pc.baseCha >= gv.mod.attributeBaseValue || pc.baseWis > gv.mod.attributeBaseValue || pc.baseInt >= gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed == 1)
                {
                    if ((pc.baseCha >= gv.mod.attributeBaseValue && pc.baseWis > gv.mod.attributeBaseValue) || (pc.baseCha >= gv.mod.attributeBaseValue && pc.baseInt >= gv.mod.attributeBaseValue) || (pc.baseWis > gv.mod.attributeBaseValue && pc.baseInt >= gv.mod.attributeBaseValue))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed == 0)
                {
                    if (pc.baseCha >= gv.mod.attributeBaseValue && pc.baseWis > gv.mod.attributeBaseValue && pc.baseInt >= gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            if (attribute == "Cha" && pc.baseCha > gv.mod.attributeMinValue)
            {
                if (gv.mod.useHybridRollPointDistribution)
                {
                    return true;
                }
                if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed == 2)
                {
                    if (pc.baseCha > gv.mod.attributeBaseValue || pc.baseWis >= gv.mod.attributeBaseValue || pc.baseInt >= gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed == 1)
                {
                    if ((pc.baseCha > gv.mod.attributeBaseValue && pc.baseWis >= gv.mod.attributeBaseValue) || (pc.baseCha > gv.mod.attributeBaseValue && pc.baseInt >= gv.mod.attributeBaseValue) || (pc.baseWis >= gv.mod.attributeBaseValue && pc.baseInt >= gv.mod.attributeBaseValue))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (gv.mod.numberOfMentalAtttributesBelowBaseAllowed == 0)
                {
                    if (pc.baseCha > gv.mod.attributeBaseValue && pc.baseWis >= gv.mod.attributeBaseValue && pc.baseInt >= gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public bool checkPhysical (string attribute)
        {
            if (attribute == "Str" && pc.baseStr > gv.mod.attributeMinValue)
            {
                if (gv.mod.useHybridRollPointDistribution)
                {
                    return true;
                }
                if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed == 2)
                {
                    if (pc.baseStr > gv.mod.attributeBaseValue || pc.baseDex >= gv.mod.attributeBaseValue || pc.baseCon >= gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //todo
                else if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed == 1)
                {
                    if ((pc.baseStr > gv.mod.attributeBaseValue && pc.baseDex >= gv.mod.attributeBaseValue) || (pc.baseStr > gv.mod.attributeBaseValue && pc.baseCon >= gv.mod.attributeBaseValue) || (pc.baseDex >= gv.mod.attributeBaseValue && pc.baseCon >= gv.mod.attributeBaseValue))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed == 0)
                {
                    if (pc.baseStr > gv.mod.attributeBaseValue && pc.baseDex >= gv.mod.attributeBaseValue && pc.baseCon >= gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            if (attribute == "Dex" && pc.baseDex > gv.mod.attributeMinValue)
            {
                if (gv.mod.useHybridRollPointDistribution)
                {
                    return true;
                }
                if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed == 2)
                {
                    if (pc.baseStr >= gv.mod.attributeBaseValue || pc.baseDex > gv.mod.attributeBaseValue || pc.baseCon >= gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //todo
                else if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed == 1)
                {
                    if ((pc.baseStr >= gv.mod.attributeBaseValue && pc.baseDex > gv.mod.attributeBaseValue) || (pc.baseStr >= gv.mod.attributeBaseValue && pc.baseCon >= gv.mod.attributeBaseValue) || (pc.baseDex > gv.mod.attributeBaseValue && pc.baseCon >= gv.mod.attributeBaseValue))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed == 0)
                {
                    if (pc.baseStr >= gv.mod.attributeBaseValue && pc.baseDex > gv.mod.attributeBaseValue && pc.baseCon >= gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            }

            if (attribute == "Con" && pc.baseCon > gv.mod.attributeMinValue)
            {
                if (gv.mod.useHybridRollPointDistribution)
                {
                    return true;
                }
                if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed == 2)
                {
                    if (pc.baseStr >= gv.mod.attributeBaseValue || pc.baseDex >= gv.mod.attributeBaseValue || pc.baseCon > gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //todo
                else if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed == 1)
                {
                    if ((pc.baseStr >= gv.mod.attributeBaseValue && pc.baseDex >= gv.mod.attributeBaseValue) || (pc.baseStr >= gv.mod.attributeBaseValue && pc.baseCon > gv.mod.attributeBaseValue) || (pc.baseDex >= gv.mod.attributeBaseValue && pc.baseCon > gv.mod.attributeBaseValue))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (gv.mod.numberOfPhysicalAtttributesBelowBaseAllowed == 0)
                {
                    if (pc.baseStr >= gv.mod.attributeBaseValue && pc.baseDex >= gv.mod.attributeBaseValue && pc.baseCon > gv.mod.attributeBaseValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public void lowerAttribute(string attribute)
        {
            if (attribute == "Str")
            {
                if (checkPhysical(attribute))
                {
                    
                    gv.mod.counterPointsToDistributeLeft += calculateAttributeChangeCost("Str", false);
                    pc.baseStr--;
                }
            }

            if (attribute == "Dex")
            {
                if (checkPhysical(attribute))
                {

                    gv.mod.counterPointsToDistributeLeft += calculateAttributeChangeCost("Dex", false);
                    pc.baseDex--;
                }
            }

            if (attribute == "Con")
            {
                if (checkPhysical(attribute))
                {
                    
                    gv.mod.counterPointsToDistributeLeft += calculateAttributeChangeCost("Con", false);
                    pc.baseCon--;
                }
            }

            if (attribute == "Int")
            {
                if (checkMental(attribute))
                {
                  
                    gv.mod.counterPointsToDistributeLeft += calculateAttributeChangeCost("Int", false);
                    pc.baseInt--;
                }
            }

            if (attribute == "Wis")
            {
                if (checkMental(attribute))
                {
                    
                    gv.mod.counterPointsToDistributeLeft += calculateAttributeChangeCost("Wis", false);
                    pc.baseWis--;
                }
            }

            if (attribute == "Cha")
            {
                if (checkMental(attribute))
                {
                  
                    gv.mod.counterPointsToDistributeLeft += calculateAttributeChangeCost("Cha", false);
                    pc.baseCha--;
                }
            }
            gv.sf.UpdateStats(pc);
        }

        public void raiseAttribute(string attribute)
        {
            if (attribute == "Str")
            {
                if (pc.baseStr < gv.mod.attributeMaxValue)
                {
                    if (gv.mod.counterPointsToDistributeLeft >= calculateAttributeChangeCost("Str", true))
                    {
                        gv.mod.counterPointsToDistributeLeft -= calculateAttributeChangeCost("Str", true);
                        pc.baseStr++;
                    }
                }
            }

            if (attribute == "Dex")
            {
                if (pc.baseDex < gv.mod.attributeMaxValue)
                {
                    if (gv.mod.counterPointsToDistributeLeft >= calculateAttributeChangeCost("Dex", true))
                    {
                    
                        gv.mod.counterPointsToDistributeLeft -= calculateAttributeChangeCost("Dex", true);
                        pc.baseDex++;
                    }
                }
            }

            if (attribute == "Con")
            {
                if (pc.baseCon < gv.mod.attributeMaxValue)
                {
                    if (gv.mod.counterPointsToDistributeLeft >= calculateAttributeChangeCost("Con", true))
                    {
                        
                        gv.mod.counterPointsToDistributeLeft -= calculateAttributeChangeCost("Con", true);
                        pc.baseCon++;
                    }
                }
            }

            if (attribute == "Int")
            {
                if (pc.baseInt < gv.mod.attributeMaxValue)
                {
                    if (gv.mod.counterPointsToDistributeLeft >= calculateAttributeChangeCost("Int", true))
                    {
                       
                        gv.mod.counterPointsToDistributeLeft -= calculateAttributeChangeCost("Int", true);
                        pc.baseInt++;
                    }
                }
            }

            if (attribute == "Wis")
            {
                if (pc.baseWis < gv.mod.attributeMaxValue)
                {
                    if (gv.mod.counterPointsToDistributeLeft >= calculateAttributeChangeCost("Wis", true))
                    {
                       
                        gv.mod.counterPointsToDistributeLeft -= calculateAttributeChangeCost("Wis", true);
                        pc.baseWis++;
                    }
                }
            }

            if (attribute == "Cha")
            {
                if (pc.baseCha < gv.mod.attributeMaxValue)
                {
                    if (gv.mod.counterPointsToDistributeLeft >= calculateAttributeChangeCost("Cha", true))
                    {
                       
                        gv.mod.counterPointsToDistributeLeft -= calculateAttributeChangeCost("Cha", true);
                        pc.baseCha++;
                    }
                }
            }
            gv.sf.UpdateStats(pc);
        }

        public void reRollStats(Player p)
        {
            gv.mod.counterPointsToDistributeLeft = 0;
            if (gv.mod.use3d6 == true)
            {
                p.baseStr = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseDex = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseInt = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseCha = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseCon = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);
                p.baseWis = gv.sf.RandInt(6) + gv.sf.RandInt(6) + gv.sf.RandInt(6);

                if (gv.mod.useHybridRollPointDistribution)
                {
                    if (p.baseStr < gv.mod.attributeMinValue)
                    {
                        p.baseStr = gv.mod.attributeMinValue;
                    }
                    if (p.baseStr > gv.mod.attributeMaxValue)
                    {
                        p.baseStr = gv.mod.attributeMaxValue;
                    }

                    if (p.baseDex < gv.mod.attributeMinValue)
                    {
                        p.baseDex = gv.mod.attributeMinValue;
                    }
                    if (p.baseDex > gv.mod.attributeMaxValue)
                    {
                        p.baseDex = gv.mod.attributeMaxValue;
                    }

                    if (p.baseCon < gv.mod.attributeMinValue)
                    {
                        p.baseCon = gv.mod.attributeMinValue;
                    }
                    if (p.baseCon > gv.mod.attributeMaxValue)
                    {
                        p.baseCon = gv.mod.attributeMaxValue;
                    }

                    if (p.baseInt < gv.mod.attributeMinValue)
                    {
                        p.baseInt = gv.mod.attributeMinValue;
                    }
                    if (p.baseInt > gv.mod.attributeMaxValue)
                    {
                        p.baseInt = gv.mod.attributeMaxValue;
                    }

                    if (p.baseWis < gv.mod.attributeMinValue)
                    {
                        p.baseWis = gv.mod.attributeMinValue;
                    }
                    if (p.baseWis > gv.mod.attributeMaxValue)
                    {
                        p.baseWis = gv.mod.attributeMaxValue;
                    }

                    if (p.baseCha < gv.mod.attributeMinValue)
                    {
                        p.baseCha = gv.mod.attributeMinValue;
                    }
                    if (p.baseCha > gv.mod.attributeMaxValue)
                    {
                        p.baseCha = gv.mod.attributeMaxValue;
                    }
                }
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
            p.playerClass = gv.mod.getPlayerClass(p.race.classesAllowed[pcClassSelectionIndex]);
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
