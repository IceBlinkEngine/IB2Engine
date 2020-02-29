using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;

namespace IceBlink2
{
    public class IB2Panel
    {
        //this class is handled differently than Android version
        [JsonIgnore]
        public GameView gv;
        public string tag = "";
        public string backgroundImageFilename = "";
        public bool hiding = false;
        public bool showing = false;
        public int shownLocX = 0;
        public int shownLocY = 0;
        public int currentLocX = 0;
        public int currentLocY = 0;
        public int hiddenLocX = 0;
        public int hiddenLocY = 0;
        public int hidingXIncrement = 0;
        public int hidingYIncrement = 0;
        public int Width = 0;
        public int Height = 0;
        public List<IB2Button> buttonList = new List<IB2Button>();
        public List<IB2ToggleButton> toggleList = new List<IB2ToggleButton>();
        public List<IB2Portrait> portraitList = new List<IB2Portrait>();
        public List<IB2HtmlLogBox> logList = new List<IB2HtmlLogBox>();

        public IB2Panel()
        {
            
        }

        public IB2Panel(GameView g)
        {
            gv = g;
            currentLocX = shownLocX;
            currentLocY = shownLocY;
        }

        public void setupIB2Panel(GameView g)
        {
            gv = g;
            currentLocX = shownLocX;
            currentLocY = shownLocY;

            //try to do aspect ratio adjustemnets here
            //to do
            //default is 16:9 (=1,77777777...)
            //16:10 would be 1.6, ie increase y coordinate (move panels down)
            //4:3 would be 1.3333333333,ie increase y coordinate (move panels down)

            float yAdjustmentFactor = 0;
            float width = gv.screenWidth;
            float height = gv.screenHeight;
            float currentAspectRatio = width / height;
            yAdjustmentFactor = (1920f / 1080f) / currentAspectRatio;

            currentLocY = (int)(currentLocY * yAdjustmentFactor);

            foreach (IB2Button btn in buttonList)
            {
                btn.setupIB2Button(gv);
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                btn.setupIB2ToggleButton(gv);
            }
            foreach (IB2Portrait btn in portraitList)
            { 
                btn.setupIB2Portrait(gv);
            }
            foreach (IB2HtmlLogBox log in logList)
            {
                log.setupIB2HtmlLogBox(gv);
            }
        }

        public void setHover(int x, int y)
        {
            //iterate over all controls and set glow on/off
            foreach (IB2Button btn in buttonList)
            {
                btn.setHover(this, x, y);
            }            
            foreach (IB2Portrait btn in portraitList)
            {
                btn.setHover(this, x, y);
            }
        }

        public string getImpact(int x, int y)
        {
            //iterate over all controls and get impact
            foreach (IB2Button btn in buttonList)
            {
                if (btn.getImpact(this, x, y))
                {
                    return btn.tag;
                }
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                if (btn.getImpact(this, x, y))
                {
                    return btn.tag;
                }
            }
            foreach (IB2Portrait btn in portraitList)
            {
                if (btn.getImpact(this, x, y))
                {
                    return btn.tag;
                }
            }
            foreach (IB2HtmlLogBox log in logList)
            {
                //log.onDrawLogBox();
            }
            return "";
        }

        public void Draw()
        {
            if (!gv.mod.useMinimalisticUI)
            {
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
                IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
                
                if ((this.tag != "InitiativePanel") && (this.tag != "logPanel"))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                }
                
                    if ((this.tag.Contains("logPanel")) && (gv.screenCombat.showIniBar) && (gv.screenType.Equals("combat")))
                    {
                        dst = new IbRect((int)(currentLocX * gv.screenDensity) + gv.pS, (int)(currentLocY * gv.screenDensity) + gv.squareSize + 4*gv.pS, (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity - gv.squareSize - 4*gv.pS));
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                    }
                    else if (this.tag.Contains("logPanel") && (gv.screenType.Equals("combat")))
                    {
                        dst = new IbRect((int)(currentLocX * gv.screenDensity) + gv.pS, (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                    }
                    else if (this.tag.Contains("logPanel"))
                    {
                        dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                    }
                    

            }
            //IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
            //IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
            //gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);

            //iterate over all controls and draw
            //backwerk
            //currentLocX = hiddenLocX;
            bool stopDrawing = false;

            if (this.hidingXIncrement != 0 && this.currentLocX == this.hiddenLocX)
            {
                stopDrawing = true;
            }

            if (this.hidingYIncrement != 0 && this.currentLocY == this.hiddenLocY)
            {
                stopDrawing = true;
            }

            if (!stopDrawing)
            { 
                 foreach (IB2Button btn in buttonList)
                {
                    //if ((btn.X > -gv.squareSize && btn.Y > -gv.squareSize) || (btn.X < gv.screenWidth + gv.squareSize && btn.Y < gv.screenHeight + gv.squareSize))
                    //{
                    if (!gv.mod.currentArea.isOverviewMap)
                    {
                        btn.Draw(this);
                    }
                    else
                    {
                        if (btn.tag  == "btnOwnZoneMap" || btn.tag == "btnMotherZoneMap" || btn.tag == "btnGrandMotherZoneMap")
                        {
                            btn.Draw(this);
                        }
                    }
                    //}
                }
                foreach (IB2ToggleButton btn in toggleList)
                {
                    //if ((btn.X > -gv.squareSize && btn.Y > -gv.squareSize) || (btn.X < gv.screenWidth + gv.squareSize && btn.Y < gv.screenHeight + gv.squareSize))
                    //{
                    if (!gv.mod.currentArea.isOverviewMap)
                    {
                        btn.Draw(this);
                    }
                    //}
                }
                foreach (IB2Portrait btn in portraitList)
                {
                    //if ((btn.X > -gv.squareSize && btn.Y > -gv.squareSize) || (btn.X < gv.screenWidth + gv.squareSize && btn.Y < gv.screenHeight + gv.squareSize))
                    //{
                    if (!gv.mod.currentArea.isOverviewMap)
                    {
                        btn.Draw(this);
                    }
                    //}
                }
            }

            if (!gv.mod.useMinimalisticUI)
            {
                foreach (IB2HtmlLogBox log in logList)
                {
                    log.onDrawLogBox(this);
                }
            }
        }

        public void DrawLogBackground()
        {
            //if (gv.screenType.Equals("main") )
            //{
            //IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
            //IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity - 3 * gv.pS), (int)(Width * gv.screenDensity + 2 * gv.pS), (int)(Height * gv.screenDensity - gv.squareSize + 7 * gv.pS));
            //gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.8f * gv.mod.logOpacity);
            //}
            //else
            //{
            if (gv.mod.logOpacity > 0)
            {
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
                IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity + gv.oXshift - 2.5 * gv.pS), (int)(currentLocY * gv.screenDensity) - 3 * gv.pS, (int)(Width * gv.screenDensity + 6.5 * gv.pS), (int)(Height * gv.screenDensity - 1 * gv.squareSize + 12 * gv.pS - 0 * gv.pS));

                if (gv.mod.useComplexCoordinateSystem && gv.screenType != "combat")
                {
                    dst = new IbRect((int)(currentLocX * gv.screenDensity + gv.oXshift - 2.5 * gv.pS), (int)(currentLocY * gv.screenDensity) + 5 * gv.pS, (int)(Width * gv.screenDensity + 6.5 * gv.pS), (int)(Height * gv.screenDensity - 1 * gv.squareSize + 12 * gv.pS - 2 * gv.pS));
                }
                if (gv.cc.floatyTextActorInfoName != "")
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 1f * gv.mod.logOpacity, true);
                }
                else
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.575f * gv.mod.logOpacity, true);
                }
                //}
                int txtH = (int)gv.drawFontRegHeight;
                gv.cc.floatyTextLocInfo.X = gv.squareSize / 2;
                gv.cc.floatyTextLocInfo.Y = gv.squareSize * 1;
                bool isPlayer = false;
                foreach (Player p in gv.mod.playerList)
                {
                    if (p.name == gv.cc.floatyTextActorInfoName)
                    {
                        isPlayer = true;
                        break;
                    }
                }
                bool isCreature = false;
                foreach (Creature c in gv.mod.currentEncounter.encounterCreatureList)
                {
                    if (c.cr_name == gv.cc.floatyTextActorInfoName)
                    {
                        isCreature = true;
                        break;
                    }
                }
                bool isTrigger = false;
                if (gv.cc.floatyTextActorInfoName == "Trigger Square")
                {
                        isTrigger = true;
                }
                
                //isTrigger, Trigger Square

                if (gv.cc.drawInfoText && gv.screenCombat.isPlayerTurn)
                {
                    if (isPlayer)
                    {
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 3 * txtH, 0.9f, Color.Lime);
                        if (!gv.cc.inEffectMode)
                        {
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAC, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 5 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoMoveOrder, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 5 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoInitiative, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 5 * txtH, 0.9f, Color.White); 
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoHP, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSP, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoNumberOfAttacks, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoToHit, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAmmo, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAttackType, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 10 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAttackRange, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 10 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamage, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y +12 * txtH, 0.9f, Color.White);
                            if (gv.cc.floatyTextActorInfoDamageType == "Normal")
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.White);
                            }
                            if (gv.cc.floatyTextActorInfoDamageType == "Poison")
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.LimeGreen);
                            }
                            if (gv.cc.floatyTextActorInfoDamageType == "Magic")
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.Gold);
                            }
                            if (gv.cc.floatyTextActorInfoDamageType == "Fire")
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.OrangeRed);
                            }
                            if (gv.cc.floatyTextActorInfoDamageType == "Cold")
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.CornflowerBlue);
                            }
                            if (gv.cc.floatyTextActorInfoDamageType == "Acid")
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.Purple);
                            }
                            if (gv.cc.floatyTextActorInfoDamageType == "Electricity")
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.Turquoise);
                            }

                            //gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoWeaponTags, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnScoringHitSpellName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 14 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 16 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves2, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 16 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves3, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 16 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 18 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances2, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 18 * txtH, 0.9f, Color.Gold);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances3, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 18 * txtH, 0.9f, Color.LimeGreen);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances4, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 19 * txtH, 0.9f, Color.OrangeRed);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances5, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 19 * txtH, 0.9f, Color.CornflowerBlue);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances6, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 20 * txtH, 0.9f, Color.Purple);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances7, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 20 * txtH, 0.9f, Color.Turquoise);

                            gv.DrawTextOutlined("Press RMB to show current", gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 26 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined("temporary effects", gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 27 * txtH, 0.9f, Color.White);


                            //gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnScoringHitSpellNameSelf, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 1.0f, Color.White);

                        }

                    }
                    //this draw creature info
                    if (isCreature)
                    {
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 3 * txtH, 0.9f, Color.Red);
                        if (!gv.cc.inEffectMode)
                        {
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAC, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 4 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoMoveOrder, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 4 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoInitiative, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 4 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoHP, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 5 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSP, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 5 * txtH, 0.9f, Color.White);

                            //cut here if trait != noen and roll failed
                            // unveilStatInfoTraitDC
                            bool showStats = false;
                            string traitName = "none";
                            int traitDC = 0;

                            foreach (Creature c in gv.mod.currentEncounter.encounterCreatureList)
                            {
                                if (c.cr_name == gv.cc.floatyTextActorInfoName)
                                {
                                    if (c.unveilStatInfoTraitTag == "none" || c.unveilStatInfoTraitTag == "None" || c.unveilStatInfoTraitTag == "" || c.unveilStatInfoTraitTag == null)
                                    {
                                        showStats = true;
                                    }
                                    else
                                    {
                                        traitName = c.unveilStatInfoTraitTag;
                                        traitDC = c.unveilStatInfoTraitDC;
                                        if (gv.sf.CheckPassSkill(-2, c.unveilStatInfoTraitTag, c.unveilStatInfoTraitDC, true, true))
                                        {
                                            showStats = true;
                                        }

                                    }
                                }
                            }
                            if (showStats)
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoNumberOfAttacks, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoToHit, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAmmo, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAttackType, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAttackRange, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamage, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.White);

                                //todo color coding
                                if (gv.cc.floatyTextActorInfoDamageType == "Normal")
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.White);
                                }
                                if (gv.cc.floatyTextActorInfoDamageType == "Poison")
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.LimeGreen);
                                }
                                if (gv.cc.floatyTextActorInfoDamageType == "Magic")
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.Gold);
                                }
                                if (gv.cc.floatyTextActorInfoDamageType == "Fire")
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.OrangeRed);
                                }
                                if (gv.cc.floatyTextActorInfoDamageType == "Cold")
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.CornflowerBlue);
                                }
                                if (gv.cc.floatyTextActorInfoDamageType == "Acid")
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.Purple);
                                }
                                if (gv.cc.floatyTextActorInfoDamageType == "Electricity")
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.Turquoise);
                                }




                                //gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnScoringHitSpellName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 10 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves2, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves3, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances2, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, Color.Gold);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances3, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, Color.LimeGreen);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances4, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 14 * txtH, 0.9f, Color.OrangeRed);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances5, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 14 * txtH, 0.9f, Color.CornflowerBlue);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances6, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 15 * txtH, 0.9f, Color.Purple);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances7, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 15 * txtH, 0.9f, Color.Turquoise);
                                //17

                                /*
                                 public bool showType = true;
          public bool showHitBy = true;
          public bool showRegen = true;
          public bool showDeathScript = true;
          public bool showAI = true;
          public bool showSpells = true;
          */


                                if (gv.mod.showType)
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoCreatureTags, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 17 * txtH, 0.9f, Color.White);
                                }
                                if (gv.mod.showHitBy)
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoHitBy, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 18 * txtH, 0.9f, Color.White);
                                }
                                if (gv.mod.showRegen)
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoRegenerationHP, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 19 * txtH, 0.9f, Color.White);
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoRegenerationSP, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 19 * txtH, 0.9f, Color.White);
                                }
                                if (gv.mod.showDeathScript)
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnDeathScriptName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 20 * txtH, 0.9f, Color.White);
                                }
                                if (gv.mod.showAI)
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAIType, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 21 * txtH, 0.9f, Color.White);
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAIAffinityForCasting, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 22 * txtH, 0.9f, Color.White);
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoInjuryThreshold, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 22 * txtH, 0.9f, Color.White);
                                }
                                if (gv.mod.showSpells)
                                {
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSpellsKnown1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 23 * txtH, 0.9f, Color.White);
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSpellsKnown2, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 24 * txtH, 0.9f, Color.White);
                                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSpellsKnown3, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 25 * txtH, 0.9f, Color.White);
                                }

                                ///gv.cc.floatyTextActorInfoRegenerationHP = "";
                                ///gv.cc.floatyTextActorInfoRegenerationSP = "";
                                //gv.cc.floatyTextActorInfoSpellsKnown1 = "";
                                //gv.cc.floatyTextActorInfoSpellsKnown2 = "";
                                //gv.cc.floatyTextActorInfoSpellsKnown3 = "";
                                //gv.cc.floatyTextActorInfoAIType = "";
                                //gv.cc.floatyTextActorInfoAIAffinityForCasting = "";//0 to 100
                                ///gv.cc.floatyTextActorInfoCreatureTags = "";//used for immunities, special weaknesses, eg "undead" are affected by turn spells and immunne to paralyze...
                                ///gv.cc.floatyTextActorInfoOnDeathScriptName = "";

                                //25
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoRMB1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 26 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoRMB2, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 27 * txtH, 0.9f, Color.White);
                            }
                            //show stats was false
                            else
                            {
                                //draw info for checked trait and required dc
                                gv.DrawTextOutlined(traitName + " too low (best in party)", gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined("At least skill level " + traitDC + " required", gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoRMB1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 26 * txtH, 0.9f, Color.White);
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoRMB2, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 27 * txtH, 0.9f, Color.White);

                            }
                        }

                    }

                    if (isTrigger)
                    {
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 3 * txtH, 0.9f, Color.Yellow);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoText, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 5 * txtH, 0.9f, Color.White);

                        if (gv.cc.floatyTextActorInfoEnabledState.Contains("Enabled"))
                        {
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoEnabledState, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, Color.Lime);
                        }
                        else
                        {
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoEnabledState, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, Color.Red);
                        }
                        if (gv.cc.floatyTextActorInfoEnableTrait != "none" && gv.cc.floatyTextActorInfoEnableTrait != "None" && gv.cc.floatyTextActorInfoEnableTrait != "" && gv.cc.floatyTextActorInfoEnableTrait != null)
                        {
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoEnableTrait, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoEnableDC, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.White);
                        }
                        else if (gv.cc.floatyTextActorInfoDisableTrait != "none" && gv.cc.floatyTextActorInfoDisableTrait != "None" && gv.cc.floatyTextActorInfoDisableTrait != "" && gv.cc.floatyTextActorInfoDisableTrait != null)
                        {
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDisableTrait, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 0.9f, Color.White);
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDisableDC, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.White);
                        }

                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoWorksFor, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 10 * txtH, 0.9f, Color.White);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoCharges, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 11* txtH, 0.9f, Color.White);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoEveryStep, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y +12 * txtH, 0.9f, Color.White);

                        if (gv.cc.floatyTextActorInfoVanishInXTurns != "" && gv.cc.floatyTextActorInfoVanishInXTurns != "none" && gv.cc.floatyTextActorInfoVanishInXTurns != "None")
                        {
                            if (gv.cc.floatyTextActorInfoVanishInXTurns.Contains("Vanishes in") || gv.cc.floatyTextActorInfoVanishInXTurns.Contains("Vanishes on"))
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoVanishInXTurns, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, Color.Yellow);
                            }
                            else
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoVanishInXTurns, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, Color.White);
                            }
                        }
                        else
                        {
                            if (gv.cc.floatyTextActorInfoAppearInXTurns.Contains("Enabled in") || gv.cc.floatyTextActorInfoAppearInXTurns.Contains("Enabled on"))
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAppearInXTurns, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, Color.Yellow);
                            }
                            else
                            {
                                gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAppearInXTurns, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, Color.White);
                            }
                            //gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAppearInXTurns, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.White);
                        }
                        if (gv.cc.floatyTextActorInfoChangeWalkableState.Contains("Affects walkable"))
                        {
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoChangeWalkableState, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 14 * txtH, 0.9f, Color.Yellow);
                        }
                        else
                        {
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoChangeWalkableState, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 14 * txtH, 0.9f, Color.White);
                        }
                        

                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSpellName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 16 * txtH, 0.9f, Color.White);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnlyWhileOnSquare, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 17 * txtH, 0.9f, Color.White);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnlyCasterLevel, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 18 * txtH, 0.9f, Color.White);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSpellName2, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 20 * txtH, 0.9f, Color.White);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnlyWhileOnSquare2, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 21 * txtH, 0.9f, Color.White);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnlyCasterLevel2, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 22 * txtH, 0.9f, Color.White);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSpellName3, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 24 * txtH, 0.9f, Color.White);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnlyWhileOnSquare3, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 25 * txtH, 0.9f, Color.White);
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnlyCasterLevel3, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 26 * txtH, 0.9f, Color.White);


                    }
                    /*
                    public string floatyTextActorInfoText = "";
        public string floatyTextActorInfoWorksFor = "";
        public string floatyTextActorInfoCharges = "";
        public string floatyTextActorInfoEveryStep = "";
        public string floatyTextActorInfoSpellName = "";//get via tag
        public string floatyTextActorInfoOnlyWhileOnSquare = "";
        public string floatyTextActorInfoOnlyCasterLevel = "";
        public string floatyTextActorInfoSpellName2 = "";//get via tag
        public string floatyTextActorInfoOnlyWhileOnSquare2 = "";
        public string floatyTextActorInfoOnlyCasterLevel2 = "";
        public string floatyTextActorInfoSpellName3 = "";//get via tag
        public string floatyTextActorInfoOnlyWhileOnSquare3 = "";
        public string floatyTextActorInfoOnlyCasterLevel3 = "";
        */


                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 4 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects2, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 6 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects3, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects4, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 10 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects5, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects6, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 14 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects7, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 16 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects8, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 18 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects9, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 20 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects10, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 22 * txtH, 0.9f, Color.White);

                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects1custom, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 5 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects2custom, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects3custom, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects4custom, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 11 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects5custom, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects6custom, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 15 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects7custom, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 17 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects8custom, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 19 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects9custom, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 21 * txtH, 0.9f, Color.White);
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects10custom, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 23 * txtH, 0.9f, Color.White);
                }



                foreach (IB2HtmlLogBox log in logList)
                {
                    if (gv.cc.floatyTextActorInfoName == "")
                    {
                        log.onDrawLogBox(this);
                    } 
                }
            }

            //foreach (IB2ToggleButton btn in toggleList)
            //{
                //btn.Draw(this);
            //}
            /*
            src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
            dst = new IbRect(gv.pS, (int)((gv.playerOffsetY*2+1 -2) * gv.squareSize + 2*gv.pS), (int)(5 * gv.squareSize), (int)(1 * gv.squareSize - 2*gv.pS ));
            gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
            */

            /*
            //iterate over all controls and draw
            foreach (IB2Button btn in buttonList)
            {
                btn.Draw(this);
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                btn.Draw(this);
            }
            foreach (IB2Portrait btn in portraitList)
            {
                btn.Draw(this);
            }
            */

        }

        public void Update(int elapsed)
        {
            //animate hiding panel
            if (hiding)
            {
                currentLocX += (hidingXIncrement * elapsed/2);
                currentLocY += (hidingYIncrement * elapsed/2);
                //hiding left and passed
                if ((hidingXIncrement < 0) && (currentLocX < hiddenLocX))
                {
                    currentLocX = hiddenLocX;
                    hiding = false;
                }
                //hiding right and passed
                if ((hidingXIncrement > 0) && (currentLocX > hiddenLocX))
                {
                    currentLocX = hiddenLocX;
                    hiding = false;
                }
                //hiding down and passed
                if ((hidingYIncrement > 0) && (currentLocY > hiddenLocY))
                {
                    currentLocY = hiddenLocY;
                    hiding = false;
                }
                //hiding up and passed
                if ((hidingYIncrement < 0) && (currentLocY < hiddenLocY))
                {
                    currentLocY = hiddenLocY;
                    hiding = false;
                }
            }
            else if (showing)
            {
                currentLocX -= (hidingXIncrement * elapsed);
                currentLocY -= (hidingYIncrement * elapsed);
                //showing right and passed
                if ((hidingXIncrement < 0) && (currentLocX > shownLocX))
                {
                    currentLocX = shownLocX;
                    showing = false;
                }
                //showing left and passed
                if ((hidingXIncrement > 0) && (currentLocX < shownLocX))
                {
                    currentLocX = shownLocX;
                    showing = false;
                }
                //showing up and passed
                if ((hidingYIncrement > 0) && (currentLocY < shownLocY))
                {
                    currentLocY = shownLocY;
                    showing = false;
                }
                //showing down and passed
                if ((hidingYIncrement < 0) && (currentLocY > shownLocY))
                {
                    currentLocY = shownLocY;
                    showing = false;
                }
            }
        }
    }
}
