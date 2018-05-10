using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IceBlink2
{
    public class ScriptFunctions
    {
        public Module mod;
        public string ActionToTake = "Attack";  //Attack, Cast, Move 
        public Spell SpellToCast = null;        //Spell that the creature is casting, make sure to null out after use
        public Prop ThisProp = null;            //Prop that is calling the current script, convo, or encounter when using 'thisProp' in script or convo, make sure to null out after use
        public Creature ThisCreature = null;    //Creature that is calling the current script, when using 'thisCreature' in script, make sure to null out after use
        public object CombatTarget = null;
        public GameView gv;
        //public int spCnt = 0;
        public Random rand;
        public List<object> AoeTargetsList = new List<object>();
        public List<Coordinate> AoeSquaresList = new List<Coordinate>();

        public ScriptFunctions(Module m, GameView g)
        {
            mod = m;
            gv = g;
            rand = new Random();
        }

        public void MessageBox(string message)
        {
            MessageBoxHtml(message);
        }

        public void ShowFullDescription(Item it)
        {
            //greatitemproject
            //string textToSpan = "<u>Description</u>" + "<BR>";
            string textToSpan = "<b><big>" + it.name + "</big></b><BR>";
            //if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
            //{
            if (it.category == "Melee" || it.category == "Ranged")
            {
                if (it.damageNumDice != 0 || it.damageAdder != 0)
                {
                    if (it.damageAdder != 0 && it.damageNumDice != 0)
                    {
                        textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<BR>";
                    }
                    else if (it.damageAdder == 0 && it.damageNumDice != 0)
                    {
                        textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "<BR>";
                    }
                    else if (it.damageAdder != 0 && it.damageNumDice == 0)
                    {
                        textToSpan += "Damage: " + it.damageAdder + "<BR>";
                    }
                }
            }
                if (it.attackBonus != 0)
                {
                    textToSpan += "Attack Modifier: " + it.attackBonus + "<BR>";
                }
                if (it.attackRange > 1)
                {
                    textToSpan += "Attack Range: " + it.attackRange + "<BR>";
                }

                if (it.AreaOfEffect > 0)
                {
                    textToSpan += "Area of Effect radius/length: " + it.AreaOfEffect + "<BR>";
                    textToSpan += "Area of Effect shape: " + it.aoeShape + "<BR>";
                }

                if (it.typeOfDamage != "Normal")
                {
                    textToSpan += "Type of Damage: " + it.typeOfDamage + "<BR>";
                }

                if ((it.ammoType != "none") && (it.category != "Ammo"))
                {
                    string ammoName = "none";
                    foreach (Item itA in gv.mod.moduleItemsList)
                    {
                        if (itA.tag == it.ammoType)
                        {
                            ammoName = itA.name;
                        }
                    }
                    textToSpan += "Required Ammo: " + ammoName + "<BR>";
                }
                if (it.armorBonus != 0)
                {
                    textToSpan += "AC Modifier: " + it.armorBonus + "<BR>";
                }

                if (it.twoHanded)
                {
                    textToSpan += "Two handed: " + it.twoHanded + "<BR>";
                }
             
                if (it.category == "Armor")
                {
                    textToSpan += "Armor type: " + it.ArmorWeightType + "<BR>";
                }

                if (it.maxDexBonus != 99)
                {
                    textToSpan += "Max dexterity bonus: " + it.maxDexBonus + "<BR>";
                }

                if (it.automaticallyHitsTarget)
                {
                    textToSpan += "Always hits: " + it.automaticallyHitsTarget + "<BR>";
                }

                if (it.canNotBeChangedInCombat)
                {
                    textToSpan += "Not changeable in combat: " + it.canNotBeChangedInCombat + "<BR>";
                }

                if (it.canNotBeUnequipped)
                {
                    textToSpan += "Can never be changed: " + it.canNotBeUnequipped + "<BR>";
                }

                if (!it.endTurnAfterEquipping)
                {
                    textToSpan += "Changing is free action: " + it.endTurnAfterEquipping + "<BR>";
                }

              
                if (it.onUseItemCastSpellTag != "none" || it.onUseItemIBScript != "none" || it.onUseItem != "none" )
                {
                    textToSpan += "Allows USE action: true" + "<BR>";
                    if (it.destroyItemAfterOnUseItemCastSpell || it.destroyItemAfterOnUseItemIBScript || it.destroyItemAfterOnUseItemScript)
                    {
                        textToSpan += "Item is destroyed after use (of last charge if charged): true" + "<BR>";
                    }
                }

                if (it.onUseItemCastSpellTag != "none")
                {
                    string spellName = "none";
                    foreach (Spell sp in gv.mod.moduleSpellsList)
                    {
                        if (sp.tag == it.onUseItemCastSpellTag)
                        {
                            spellName = sp.name;
                            break;
                        }
                    }

                    textToSpan += "Spell to cast on use: " + spellName + "<BR>";
                    textToSpan += "Item on use caster level: " + it.levelOfItemForCastSpell + "<BR>";
                }

                if (it.onlyUseableWhenEquipped)
                {
                    textToSpan += "Must be equipped to use: " + it.onlyUseableWhenEquipped + "<BR>";
                }

                if (it.useableInSituation != "Passive" && it.useableInSituation != "Always")
                {
                    if (it.useableInSituation == "InCombat")
                    {
                        textToSpan += "Only useable in combat: true" + "<BR>";
                    }

                    else if (it.useableInSituation == "OutOfCombat")
                    {
                        textToSpan += "Only useable out of combat: true" + "<BR>";
                    }
                }

                if (it.onScoringHitCastSpellTag != "none")
                {
                    textToSpan += "Special effect on hit: true" + "<BR>";
                }
                

                if (it.entriesForPcTags.Count > 0)
                {
                    string pcTags = "";
                    foreach (LocalImmunityString ls in it.entriesForPcTags)
                    {
                        pcTags += ls.Value + ", ";
                    }
                    textToSpan += "Item perks: " + pcTags + "<BR>";
                }

                if (it.isRation)
                {
                    textToSpan += "Is ration: " + it.isRation + "<BR>";
                }

                if (it.isLightSource)
                {
                    textToSpan += "Is light source: " + it.isLightSource + "<BR>";
                }

                if (it.attributeBonusModifierStr != 0)
                {
                    textToSpan += "STR modifier: " + it.attributeBonusModifierStr + "<BR>";
                }

                if (it.attributeBonusModifierDex != 0)
                {
                    textToSpan += "DEX modifier: " + it.attributeBonusModifierDex + "<BR>";
                }

                if (it.attributeBonusModifierCon != 0)
                {
                    textToSpan += "CON modifier: " + it.attributeBonusModifierCon + "<BR>";
                }

                if (it.attributeBonusModifierInt != 0)
                {
                    textToSpan += "INT modifier: " + it.attributeBonusModifierInt + "<BR>";
                }

                if (it.attributeBonusModifierWis != 0)
                {
                    textToSpan += "WIS modifier: " + it.attributeBonusModifierWis + "<BR>";
                }

                if (it.attributeBonusModifierCha != 0)
                {
                    textToSpan += "CHA modifier: " + it.attributeBonusModifierCha + "<BR>";
                }

                if (it.hpRegenPerRoundInCombat != 0)
                {
                    textToSpan += "HP reg per round in combat: " + it.hpRegenPerRoundInCombat + "<BR>"; 
                }

                if (it.spRegenPerRoundInCombat != 0)
                {
                    textToSpan += "SP reg per round in combat: " + it.spRegenPerRoundInCombat + "<BR>";
                }

                if (it.minutesPerHpRegenOutsideCombat != 0)
                {
                    textToSpan += "+1 HP outside combat every: " + it.minutesPerHpRegenOutsideCombat + " minutes" + "<BR>";
                }

                if (it.minutesPerSpRegenOutsideCombat != 0)
                {
                    textToSpan += "+1 SP outside combat every: " + it.minutesPerSpRegenOutsideCombat + " minutes" + "<BR>";
                }

                if (it.MovementPointModifier != 0)
                {
                    textToSpan += "Effect on movement points: " + it.MovementPointModifier + "<BR>";
                }

                if (it.savingThrowModifierFortitude != 0)
                {
                    textToSpan += "Fortitude save modifier: " + it.savingThrowModifierFortitude + "<BR>";
                }

                if (it.savingThrowModifierReflex != 0)
                {
                    textToSpan += "Reflex save modifier: " + it.savingThrowModifierReflex + "<BR>";
                }

                if (it.savingThrowModifierFortitude != 0)
                {
                    textToSpan += "Will save modifier: " + it.savingThrowModifierWill + "<BR>";
                }

                if (it.damageTypeResistanceValueNormal != 0)
                {
                    textToSpan += "Resistance physical modifier: " + it.damageTypeResistanceValueNormal + "<BR>";
                }

                if (it.damageTypeResistanceValueAcid != 0)
                {
                    textToSpan += "Resistance acid modifier: " + it.damageTypeResistanceValueAcid + "<BR>";
                }

            if (it.damageTypeResistanceValueElectricity != 0)
            {
                textToSpan += "Resistance electricity modifier: " + it.damageTypeResistanceValueElectricity + "<BR>";
            }

            if (it.damageTypeResistanceValueFire != 0)
            {
                textToSpan += "Resistance fire modifier: " + it.damageTypeResistanceValueFire + "<BR>";
            }

            if (it.damageTypeResistanceValueCold != 0)
            {
                textToSpan += "Resistance cold modifier: " + it.damageTypeResistanceValueCold + "<BR>";
            }

            if (it.damageTypeResistanceValuePoison != 0)
            {
                textToSpan += "Resistance poison modifier: " + it.damageTypeResistanceValuePoison + "<BR>";
            }

            if (it.damageTypeResistanceValueMagic != 0)
            {
                textToSpan += "Resistance magic modifier: " + it.damageTypeResistanceValueMagic + "<BR>";
            }

            if (it.onUseItemCastSpellTag != "none" || it.onUseItemIBScript != "none" || it.onUseItem != "none" || it.category != "General")
            {
                textToSpan += "Allowed for classes: " + isUseableBy(it) + "<BR>";
            }

            textToSpan += "Value: " + it.value + "<BR>";

            //rückwärts

                /*
                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
                textToSpan += "Two-Handed Weapon: ";
                if (it.twoHanded) { textToSpan += "Yes<BR>"; }
                else { textToSpan += "No<BR>"; }
                */


                textToSpan += "<BR>";

                if (!it.descFull.Equals(""))
                {
                    textToSpan += it.descFull;
                }
                else
                {
                    textToSpan += it.desc;
                }
            //}
        /*
            else if (!it.category.Equals("General"))
            {
                textToSpan += "AC Bonus: " + it.armorBonus + "<BR>";
                textToSpan += "Max Dex Bonus: " + it.maxDexBonus + "<BR>";
                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
                textToSpan += "<BR>";
                if (!it.descFull.Equals(""))
                {
                    textToSpan += it.descFull;
                }
                else
                {
                    textToSpan += it.desc;
                }
            }
            else if (it.category.Equals("General"))
            {
                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
                textToSpan += "<BR>";
                if (!it.descFull.Equals(""))
                {
                    textToSpan += it.descFull;
                }
                else
                {
                    textToSpan += it.desc;
                }
            }
            */
            MessageBoxHtml(textToSpan);
        }
        public string isUseableBy(Item it)
        {
            string strg = "";
            foreach (PlayerClass cls in mod.modulePlayerClassList)
            {
                string firstLetters = cls.name.Substring(0, 2);
                foreach (ItemRefs ia in cls.itemsAllowed)
                {
                    string stg = ia.resref;
                    if (stg.Equals(it.resref))
                    {
                        strg += firstLetters + ", ";
                    }
                }
            }
            if (strg =="")
            {
                strg = "All";
            }
            return strg;
        }

        public void MessageBoxHtml(string message)
        {
            //<b> Bold
            //<i> Italics
            //<u> Underline
            //<big> Big
            //<small> Small
            //<font> Font face and color
            //<br> Linefeed
            try
            {
                using (IBHtmlMessageBox hmb = new IBHtmlMessageBox(gv, message))
                {
                    var result = hmb.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        //MessageBox.Show("selected OK");
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        //MessageBox.Show("selected Cancel");
                    }
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }
        }

        /// <summary>
        /// Used for generating a random number between some number and some other number (max must be >= min).
        /// </summary>
        /// <param name="min"> The minimum value that can be found (ex. Random(5, 9); will return a number between 5-9 so 5, 6, 7, 8 or 9 are possible results).</param>
        /// <param name="max"> The maximum value that can be found (ex. Random(5, 9); will return a number between 5-9 so 5, 6, 7, 8 or 9 are possible results).</param>
        public int RandInt(int min, int max)
        {
            if (min > max)
            {
                max = min;
            }
            //A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue.
            return rand.Next(min, max + 1);
        }
        /// <summary>
        /// Used for generating a random number between 1 and some number.
        /// </summary>
        /// <param name="max">The maximum value that can be used (ex. Random(5); will return a number between 1-5 so 1, 2, 3, 4 or 5 are possible results).</param>
        /// <returns>"returns"</returns>
        public int RandInt(int max)
        {
            return RandInt(1, max);
        }
        public int RandDiceRoll(int numberOfDice, int numberOfSidesOnDie)
        {
            int roll = 0;
            for (int x = 0; x < numberOfDice; x++)
            {
                roll += RandInt(numberOfSidesOnDie);
            }
            return roll;
        }

        public void gaController(string filename, string prm1, string prm2, string prm3, string prm4)
        {
            if (!filename.Equals("none"))
            {
                try
                {
                    //go through each parm1-4 and replace if GlobalInt variable, GlobalString variable or rand(3-16)
                    string p1 = replaceParameter(prm1);
                    string p2 = replaceParameter(prm2);
                    string p3 = replaceParameter(prm3);
                    string p4 = replaceParameter(prm4);

                    if (filename.Equals("gaSetGlobalInt.cs"))
                    {
                        SetGlobalInt(prm1, p2);
                    }
                    else if (filename.Equals("gaModifyFactionStrength.cs"))
                    {
                        //p1 is faction tag
                        //prm2 is operator
                        //p3 is amount of modification
                        ModifyFactionStrength(p1, prm2, p3);
                    }
                    else if (filename.Equals("gaModifyFactionGrowthRate.cs"))
                    {
                        //p1 is faction tag
                        //prm2 is operator
                        //p3 is amount of modification
                        ModifyFactionGrowthRate(p1, prm2, p3);
                    }
                    else if (filename.Equals("gaRechargeSingleItem.cs"))
                    {
                        RechargeSingleItem(prm1);
                    }
                    else if (filename.Equals("gaMovePartyToLastLocation.cs"))
                    {
                        MovePartyToLastLocation(prm1);
                    }
                    else if (filename.Equals("gaRechargeAllItemsOfAType.cs"))
                    {
                        RechargeAllItemsOfAType(prm1);
                    }
                    else if (filename.Equals("gaRechargeAllItems.cs"))
                    {
                        RechargeAllItems();
                    }
                    else if (filename.Equals("gaSetLocalInt.cs"))
                    {
                        //check to see if prm1 is thisprop or thisarea
                        if (prm1.Equals("thisprop"))
                        {
                            //find the prop at this location
                            prm1 = mod.currentArea.getPropByLocation(mod.PlayerLocationX, mod.PlayerLocationY).PropTag;
                        }
                        else if (prm1.Equals("thisarea"))
                        {
                            //use the currentArea
                            prm1 = mod.currentArea.Filename;
                        }
                        SetLocalInt(prm1, prm2, p3);
                    }
                    else if (filename.Equals("gaSetGlobalString.cs"))
                    {
                        SetGlobalString(prm1, p2);
                    }
                    else if (filename.Equals("gaSetLocalString.cs"))
                    {
                        //check to see if prm1 is thisprop or thisarea
                        if (prm1.Equals("thisprop"))
                        {
                            //find the prop at this location
                            prm1 = mod.currentArea.getPropByLocation(mod.PlayerLocationX, mod.PlayerLocationY).PropTag;
                        }
                        else if (prm1.Equals("thisarea"))
                        {
                            //use the currentArea
                            prm1 = mod.currentArea.Filename;
                        }
                        SetLocalString(prm1, prm2, p3);
                    }
                    else if (filename.Equals("gaGetLocalInt.cs"))
                    {
                        //check to see if prm1 is thisprop or thisarea
                        if (prm1.Equals("thisprop"))
                        {
                            //find the prop at this location
                            prm1 = mod.currentArea.getPropByLocation(mod.PlayerLocationX, mod.PlayerLocationY).PropTag;
                        }
                        else if (prm1.Equals("thisarea"))
                        {
                            //use the currentArea
                            prm1 = mod.currentArea.Filename;
                        }
                        int i = GetLocalInt(prm1, prm2);
                        SetGlobalInt(prm3, i + "");
                    }
                    else if (filename.Equals("gaGetLocalString.cs"))
                    {
                        //check to see if prm1 is thisprop or thisarea
                        if (prm1.Equals("thisprop"))
                        {
                            //find the prop at this location
                            prm1 = mod.currentArea.getPropByLocation(mod.PlayerLocationX, mod.PlayerLocationY).PropTag;
                        }
                        else if (prm1.Equals("thisarea"))
                        {
                            //use the currentArea
                            prm1 = mod.currentArea.Filename;
                        }
                        String s = GetLocalString(prm1, prm2);
                        SetGlobalString(prm3, s);
                    }
                    else if (filename.Equals("gaTransformGlobalInt.cs"))
                    {
                        TransformGlobalInt(p1, prm2, p3, prm4);
                    }
                    else if (filename.Equals("gaTransformGlobalString.cs"))
                    {
                        TransformGlobalString(p1, p2, prm3);
                    }
                    else if (filename.Equals("gaGiveItem.cs"))
                    {
                        int parm2 = Convert.ToInt32(p2);
                        GiveItem(p1, parm2);
                    }
                    else if (filename.Equals("gaGiveXP.cs"))
                    {
                        int parm1 = Convert.ToInt32(p1);
                        GiveXP(parm1, p2);
                    }
                    else if (filename.Equals("gaCastSpell.cs"))
                    {
                        //unterhose
                        //int parm1 = Convert.ToInt32(p2);
                        //GiveXP(parm1, p2);
                        //public void doSpellCalledFromScript(Spell spell, Player player, int casterLevel, string logTextForCastingAction)
                        castSpell(p1,p2,p3,p4);
                    }
                    else if (filename.Equals("gaGiveGold.cs"))
                    {
                        int parm1 = Convert.ToInt32(p1);
                        GiveFunds(parm1);
                    }
                    else if (filename.Equals("gaTakeGold.cs"))
                    {
                        int parm1 = Convert.ToInt32(p1);
                        TakeFunds(parm1);
                    }
                    else if (filename.Equals("gaForceRest.cs"))
                    {
                        itForceRest();
                    }
                    else if (filename.Equals("gaForceRestNoRations.cs"))
                    {
                        itForceRestNoRations();
                    }  
                    else if (filename.Equals("gaForceRestAndRaiseDead.cs"))
                    {
                        itForceRestAndRaiseDead();
                    }
                    else if (filename.Equals("gaForceRestAndRaiseDeadRequireRations.cs"))
                    {
                        itForceRestAndRaiseDeadRequireRations();
                    }
                    /*
                    else if (filename.Equals("gaMovePartyToLastLocation.cs"))
                    {
                        gv.mod.PlayerLocationX = gv.mod.PlayerLastLocationX;
                        gv.mod.PlayerLocationY = gv.mod.PlayerLastLocationY;
                    }
                    */
                    else if (filename.Equals("gaTakeItem.cs"))
                    {
                        int parm2 = Convert.ToInt32(p2);
                        TakeItem(p1, parm2);
                    }
                    else if (filename.Equals("gaPartyDamage.cs"))
                    {
                        int parm1 = Convert.ToInt32(p1);
                        ApplyPartyDamage(parm1);
                    }
                    else if (filename.Equals("gaSinglePCDamage.cs"))
                    {
                        int parm1 = Convert.ToInt32(p1);
                        ApplySinglePCDamage(parm1, p2);
                    }
                    else if (filename.Equals("gaRiddle.cs"))
                    {
                        riddle();
                    }
                    else if (filename.Equals("gaDamageWithoutItem.cs"))
                    {
                        int parm1 = Convert.ToInt32(p1);
                        DamageWithoutItem(parm1, p2);
                    }
                    else if (filename.Equals("gaDoPartyLight.cs"))
                    {
                        //p1: light name
                        //p2: light color
                        //p3: focal halo intesity multiplier
                        //p4: ring halo intensity multiplier

                        if (gv.mod.partyLightName == p1)
                        {
                            if (gv.mod.partyLightOn)
                            {
                                gv.mod.partyLightOn = false;
                                gv.mod.partyLightColor = "";
                            }
                            else
                            {
                                gv.mod.partyLightOn = true;
                                if (p2 != "" && p2 != "none")
                                {
                                    gv.mod.partyLightColor = p2;
                                }
                            }
                        }
                        else
                        {
                            gv.mod.partyLightName = p1;
                            gv.mod.partyLightOn = true;

                            bool newLight = true;
                            for (int i = 0; i < gv.mod.partyLightEnergyName.Count; i++)
                            {
                                if (gv.mod.partyLightName == gv.mod.partyLightEnergyName[i])
                                {
                                    newLight = false;
                                    break;
                                }
                            }

                            if (newLight)
                            {
                                gv.mod.partyLightEnergyName.Add(gv.mod.partyLightName);
                                gv.mod.partyLightEnergyUnitsLeft.Add(gv.mod.durationInStepsOfPartyLightItems);
                                gv.mod.currentLightUnitsLeft = gv.mod.durationInStepsOfPartyLightItems; 
                            }
                            if (p2 != "" && p2 != "none")
                            {
                                gv.mod.partyLightColor = p2;
                            }
                        }

                        float parm3 = (float)Convert.ToDouble(p3);
                        float parm4 = (float)Convert.ToDouble(p4);

                        gv.mod.partyFocalHaloIntensity = parm3;
                        gv.mod.partyRingHaloIntensity = parm4;
                        gv.cc.doUpdate();
                        gv.screenType = ("main");
                    }
                    else if (filename.Equals("gaRemovePropByTag.cs"))
                    {
                        RemovePropByTag(p1);
                    }
                    else if (filename.Equals("gaRemovePropByIndex.cs"))
                    {
                        int parm1 = Convert.ToInt32(p1);
                        RemovePropByIndex(parm1, prm2);
                    }
                    else if (filename.Equals("gaTransitionPartyToMapLocation.cs"))
                    {
                        if (gv.mod.justTransitioned == false)
                        {
                            int parm2 = Convert.ToInt32(p2);
                            int parm3 = Convert.ToInt32(p3);
                            if (gv.mod.currentArea.Filename.Equals(p1))
                            {
                                gv.mod.PlayerLocationX = parm2;
                                gv.mod.PlayerLocationY = parm3;
                                //transi2
                                gv.mod.justTransitioned = true;
                                gv.mod.justTransitioned2 = true;
                                gv.mod.arrivalSquareX = gv.mod.PlayerLocationX;
                                gv.mod.arrivalSquareY = gv.mod.PlayerLocationY;
                            }
                            else
                            {
                                gv.cc.doTransitionBasedOnAreaLocation(p1, parm2, parm3);
                            }
                        }
                    }
                    else if (filename.Equals("gaAddPartyMember.cs"))
                    {
                        AddCharacterToParty(p1);
                    }
                    //this is a script that adds temporary pc
                    //via firing AddTemporaryAllyForThisEncounter method
                    else if (filename.Equals("gaAddTemporaryAllyForThisEncounter.cs"))
                    {
                        //thunfisch
                        int x = Convert.ToInt32(p2);//x (parm2)
                        int y = Convert.ToInt32(p3);//y (parm3)
                        int parm4 = Convert.ToInt32(p4);//duration

                        //add code for checking whether the  creature can be spawned on x,y location
                        //****************************************************************
                        //****************************************************************

                        //Creature source = (Creature)src;
                        Coordinate target = new Coordinate();
                        target.X = x;
                        target.Y = y;
                        bool foundPlace = true;

                            if (!IsSquareOpen(target))
                            {
                                foundPlace = false;
                            }
                       

                       

                        //try to find a nearby square
                        if (foundPlace)
                        {
                            AddTemporaryAllyForThisEncounter(p1, x, y, parm4);
                        }
                        else
                        {
                            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                            //find correct summon spot, replace with nearest location if neccessary  

                            bool changeSummonLocation = false;// used as switch for cycling through all tiles in case the originally intended spot was occupied/not-walkable  
                            int targetTile = target.Y * gv.mod.currentEncounter.MapSizeX + target.X;//the index of the original target spot in the encounter's tiles list  
                            List<int> freeTilesByIndex = new List<int>();// a new list used to store the indices of all free tiles in the enocunter  
                            int tileLocX = 0;//just temporary storage in for locations of tiles  
                            int tileLocY = 0;//just temporary storage in for locations of tiles  
                            double floatTileLocY = 0;//was uncertain about rounding and conversion details, therefore need this one (see below)  
                            bool tileIsFree = true;//identify a tile suited as new summon loaction  
                            int nearestTileByIndex = -1;//store the nearest tile by index; as the relevant loop runs this will be replaced several times likely with ever nearer tiles  
                            int dist = 0;//distance between the orignally intended summon location and a free tile  
                            int lowestDist = 10000;//this storest the lowest ditance found while the loop runs  
                            int deltaX = 0;//temporary value used for distance calculation   
                            int deltaY = 0;//temporary value used for distance calculation   

                            
                            changeSummonLocation = true;
                            Coordinate target2 = new Coordinate();
                            //target square was already occupied/non-walkable, so all other tiles are searched for the NEAREST FREE tile to switch the summon location to  
                            if (changeSummonLocation == true)
                            {
                                //FIRST PART: get all FREE tiles in the current encounter  
                                for (int i = 0; i < gv.mod.currentEncounter.encounterTiles.Count; i++)
                                {
                                    //get the x and y location of current tile by calculation derived from index number, assuming that counting starts at top left corner of a map (0x, 0y)  
                                    //and that each horizintal x-line is counted first, then counting next horizonal x-line starting from the left again  
                                    tileIsFree = true;
                                    //Note: When e.g. MapsizeY is 7, the y values range from 0 to 6  
                                    //MODULO
                                    tileLocX = i % gv.mod.currentEncounter.MapSizeX;
                                    //Note: ensure rounding down here   
                                    floatTileLocY = i / gv.mod.currentEncounter.MapSizeX;
                                    tileLocY = (int)Math.Floor(floatTileLocY);
                                    target2.X = tileLocX;
                                    target2.Y = tileLocY;

                                    //code for large summons goes here, see above
                                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                                    foundPlace = true;

                                        if (!IsSquareOpen(target2))
                                        {
                                            foundPlace = false;
                                        }

                                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                                    if (foundPlace)
                                    {
                                        tileIsFree = true;
                                    }
                                    else
                                    {
                                        tileIsFree = false;
                                    }
                                   
                                    //this writes all free tiles into a fresh list; please note that the values of the elements of this new list are our relevant index values  
                                    //therefore it's not the index (which doesnt correalte to locations) in this list that's relevant, but the value of the element at that index  
                                    if (tileIsFree == true)
                                    {
                                        freeTilesByIndex.Add(i);
                                    }
                                }

                                //SECOND PART: find the free tile NEAREST to originally intended summon location  
                                for (int i = 0; i < freeTilesByIndex.Count; i++)
                                {
                                    dist = 0;

                                    //get location x and y of the tile stored at the index number i, i.e. get the value of elment indexed with i and transform to x and y location  
                                    tileLocX = freeTilesByIndex[i] % gv.mod.currentEncounter.MapSizeX;
                                    floatTileLocY = freeTilesByIndex[i] / gv.mod.currentEncounter.MapSizeX;
                                    tileLocY = (int)Math.Floor(floatTileLocY);

                                    //get distance between the current free tile and the originally intended summon location  
                                    deltaX = (int)Math.Abs((tileLocX - target.X));
                                    deltaY = (int)Math.Abs((tileLocY - target.Y));
                                    if (deltaX > deltaY)
                                    {
                                        dist = deltaX;
                                    }
                                    else
                                    {
                                        dist = deltaY;
                                    }

                                    //filter out the nearest tile by remembering it and its distance for further comparison while the loop runs through all free tiles  
                                    if (dist < lowestDist)
                                    {
                                        lowestDist = dist;
                                        nearestTileByIndex = freeTilesByIndex[i];
                                    }
                                }

                                if (nearestTileByIndex != -1)
                                {
                                    //get the nearest tile's x and y location and use it as creature summon coordinates  
                                    tileLocX = nearestTileByIndex % gv.mod.currentEncounter.MapSizeX;
                                    floatTileLocY = nearestTileByIndex / gv.mod.currentEncounter.MapSizeX;
                                    tileLocY = (int)Math.Floor(floatTileLocY);

                                    target.X = tileLocX;
                                    target.Y = tileLocY;
                                }

                            }

                            //just check whether a free squre does exist at all; if not, do not complete the summon  
                            if ((nearestTileByIndex != -1) || (changeSummonLocation == false))
                            {
                                AddTemporaryAllyForThisEncounter(p1, target.X, target.Y, parm4);
                            }
                            else
                            {
                                gv.cc.addLogText("<yl>" + p1 + " fails to appear as no space is available</yl><BR>");
                            }
                        }
                    }
                    else if (filename.Equals("gaRemovePartyMember.cs"))
                    {
                        RemoveCharacterFromParty(prm1, p2);
                    }
                    else if (filename.Equals("gaMovePartyMemberToRoster.cs"))
                    {
                        MoveCharacterToRoster(prm1, p2);
                    }
                    else if (filename.Equals("gaMoveRosterMemberToParty.cs"))
                    {
                        MoveCharacterToPartyFromRoster(prm1, p2);
                    }
                    else if (filename.Equals("gaEnableDisableTriggerEvent.cs"))
                    {
                        EnableDisableTriggerEvent(p1, p2, p3);
                    }
                    else if (filename.Equals("gaEnableDisableTrigger.cs"))
                    {
                        EnableDisableTrigger(p1, p2);
                    }
                    else if (filename.Equals("gaTogglePartyToken.cs"))
                    {
                        TogglePartyToken(p1, p2);
                    }
                    else if (filename.Equals("gaEnableDisableTriggerAtCurrentLocation.cs"))
                    {
                        EnableDisableTriggerAtCurrentLocation(p1);
                    }
                    else if (filename.Equals("gaEnableDisableTriggerEventAtCurrentLocation.cs"))
                    {
                        EnableDisableTriggerEventAtCurrentLocation(prm1, prm2);
                    }
                    else if (filename.Equals("gaAddJournalEntryByTag.cs"))
                    {
                        AddJournalEntry(prm1, prm2);
                    }
                    else if (filename.Equals("gaEndGame.cs"))
                    {
                        gv.resetGame();
                        gv.screenType = "title";
                    }
                    else if (filename.Equals("gaRoster.cs"))
                    {
                        gv.screenType = "partyRoster";
                    }
                    else if (filename.Equals("gaPlaySound.cs"))
                    {
                        gv.PlaySound(p1);
                    }
                    else if (filename.Equals("gaKillAllCreatures.cs"))
                    {
                        gv.mod.currentEncounter.encounterCreatureList.Clear();
                        //alllow thereticlly to repeat encounter
                        //check condition, totDO
                        if (!gv.mod.currentEncounter.isRepeatable)
                        {
                            gv.mod.currentEncounter.encounterCreatureRefsList.Clear();
                        }
                        gv.screenCombat.checkEndEncounter();
                    }
                    else if (filename.Equals("gaOpenShopByTag.cs"))
                    {
                        gv.screenShop.currentShopTag = p1;
                        gv.screenShop.currentShop = gv.mod.getShopByTag(p1);
                        gv.screenType = "shop";
                    }
                    else if (filename.Equals("gaModifiyShopBuyBackPercentage.cs"))
                    {
                        ModifyBuyBack(p1, prm2, p3);
                    }
                    else if (filename.Equals("gaModifiyShopSellPercentage.cs"))
                    {
                        ModifySellPrice(p1, prm2, p3);
                    }  

                    else if (filename.Equals("gaGetPlayerIndexThatIsUsingItem.cs"))
                    {
                        //String val = gv.cc.currentPlayerIndexUsingItem + "";
                        string val = gv.mod.indexOfPCtoLastUseItem + "";
                        SetGlobalInt(prm1, val);
                    }
                    else if (filename.Equals("gaWriteTextToLog.cs"))
                    {
                        gv.cc.addLogText(prm2, p1);
                    }
                    else if (filename.Equals("gaWriteHtmlTextToLog.cs"))
                    {
                        gv.cc.addLogText(p1);
                    }
                    else if (filename.Equals("gaShowMessageBox.cs"))
                    {
                        this.MessageBoxHtml(p1);
                    }
                    else if (filename.Equals("gaShowFloatyTextOnMainMap.cs"))
                    {
                        int parm3 = Convert.ToInt32(p3);
                        int parm4 = Convert.ToInt32(p4);
                        gv.screenMainMap.addFloatyText(parm3, parm4, p1, p2, 4000);
                    }
                    else if (filename.Equals("gaDoConversationByName.cs"))
                    {
                        gv.cc.doConversationBasedOnTag(p1);
                    }
                    else if (filename.Equals("gaDoEncounterByTag.cs"))
                    {
                        gv.cc.doEncounterBasedOnTag(p1);
                    }
                    else if (filename.Equals("gaCheckForItemToggleLights.cs"))
                    {
                        int parm2 = Convert.ToInt32(p2);
                        if (CheckForItem(p1, parm2))
                        {
                            gv.mod.currentArea.areaDark = false;
                        }
                        else
                        {
                            gv.mod.currentArea.areaDark = true;
                            gv.screenMainMap.addFloatyText(mod.PlayerLocationX, mod.PlayerLastLocationY, "need light!", "white", 4000);
                        }
                    }
                    else if (filename.Equals("gaToggleAreaSquareLoSBlocking.cs"))
                    {
                        int x = Convert.ToInt32(p1);
                        int y = Convert.ToInt32(p2);
                        bool enable = Boolean.Parse(p3);
                        gv.mod.currentArea.Tiles[y * gv.mod.currentArea.MapSizeX + x].LoSBlocked = enable;
                        Coordinate coord = new Coordinate();
                        coord.X = x;
                        coord.Y = y;
                        if (enable)
                        {
                            if (gv.mod.currentArea.toggledSquaresLoS != null)
                            {
                                gv.mod.currentArea.toggledSquaresLoS.Add(coord);
                            }
                        }
                        else
                        {
                            if (gv.mod.currentArea.toggledSquaresLoSFalse != null)
                            {
                                gv.mod.currentArea.toggledSquaresLoSFalse.Add(coord);
                            }
                        }

                        //floaty in case tile gets transparent
                        if ((p4 != null && p4 != "none" && p4 != "") && (!enable))
                        {
                            //gv.cc.addFloatyText(new Coordinate(coord.X, coord.Y), p4, "green");
                            gv.screenMainMap.addFloatyText(coord.X, coord.Y, p4, "green", 4000);
                        }

                    }
                    else if (filename.Equals("gaToggleAreaSquareWalkable.cs"))
                    {
                        int x = Convert.ToInt32(p1);
                        int y = Convert.ToInt32(p2);
                        bool enable = Boolean.Parse(p3);
                        gv.mod.currentArea.Tiles[y * gv.mod.currentArea.MapSizeX + x].Walkable = enable;
                        Coordinate coord = new Coordinate();
                        coord.X = x;
                        coord.Y = y;
                        if (enable)
                        {
                            if (gv.mod.currentArea.toggledSquaresWalkable != null)
                            {
                                gv.mod.currentArea.toggledSquaresWalkable.Add(coord);
                            }
                        }
                        else
                        {
                            if (gv.mod.currentArea.toggledSquaresWalkableFalse != null)
                            {
                                gv.mod.currentArea.toggledSquaresWalkableFalse.Add(coord);
                            }
                        }

                        //floaty in case tile becomes walkable
                        if ((p4 != null && p4 != "none" && p4 != "") && (enable))
                        {
                            //gv.cc.addFloatyText(new Coordinate(coord.X, coord.Y), p4, "green");
                            gv.screenMainMap.addFloatyText(coord.X, coord.Y, p4, "green", 4000);
                        }
                    }
                    else if (filename.Equals("gaToggleAreaSquareIsSecretPassage.cs"))
                    {
                        int x = Convert.ToInt32(p1);
                        int y = Convert.ToInt32(p2);
                        bool enable = Boolean.Parse(p3);
                        gv.mod.currentArea.Tiles[y * gv.mod.currentArea.MapSizeX + x].isSecretPassage = enable;
                        Coordinate coord = new Coordinate();
                        coord.X = x;
                        coord.Y = y;
                        if (enable)
                        {
                            if (gv.mod.currentArea.toggledSquaresIsSecretPassage != null)
                            {
                                gv.mod.currentArea.toggledSquaresIsSecretPassage.Add(coord);
                            }
                        }
                        else
                        {
                            if (gv.mod.currentArea.toggledSquaresIsSecretPassageFalse != null)
                            {
                                gv.mod.currentArea.toggledSquaresIsSecretPassageFalse.Add(coord);
                            }
                        }
                        //floaty in case passage becomes open/existent
                        if ((p4 != null && p4 != "none" && p4 != "") && (enable))
                        {
                            //gv.cc.addFloatyText(new Coordinate(coord.X, coord.Y), p4, "green");
                            gv.screenMainMap.addFloatyText(coord.X, coord.Y, p4, "green", 4000);
                        }
                    }
                    else if (filename.Equals("gaPropOrTriggerCastSpellOnThisSquare.cs"))
                    {
                        gv.screenCombat.doPropOrTriggerCastSpell(p1);
                    }

                    else if (filename.Equals("gaAddCreatureToCurrentEncounter.cs") || filename.Equals("osAddCreatureToCurrentEncounter.cs"))
                    {
                        //steak
                        //****************************************************************
                        //****************************************************************
                        //Creature source = (Creature)src;
                        Coordinate target = new Coordinate();
                        target.X = Convert.ToInt32(p2);
                        target.Y = Convert.ToInt32(p3);

                        bool foundPlace = true;

                        //holla
                        //we must determine the size of the summoned creature
                        Creature summon = new Creature();
                        foreach (Creature c in gv.mod.moduleCreaturesList)
                        {
                            if (c.cr_resref == p1)
                            {
                                summon.creatureSize = c.creatureSize;
                            }
                        }

                        Coordinate plusX = new Coordinate();
                        plusX.X = target.X + 1;
                        plusX.Y = target.Y;
                        Coordinate plusY = new Coordinate();
                        plusY.X = target.X;
                        plusY.Y = target.Y + 1;
                        Coordinate plusXandY = new Coordinate();
                        plusXandY.X = target.X + 1;
                        plusXandY.Y = target.Y + 1;

                        if (summon.creatureSize == 1)
                        {
                            if (!IsSquareOpen(target))
                            {
                                foundPlace = false;
                            }
                        }

                        if (summon.creatureSize == 2)
                        {

                            if (!IsSquareOpen(target))
                            {
                                foundPlace = false;
                            }

                            if (plusX.X < gv.mod.currentEncounter.MapSizeX)
                            {
                                if (!IsSquareOpen(plusX))
                                {
                                    foundPlace = false;
                                }
                            }
                            else
                            {
                                foundPlace = false;
                            }
                        }

                        if (summon.creatureSize == 3)
                        {

                            if (!IsSquareOpen(target))
                            {
                                foundPlace = false;
                            }

                            if (plusY.Y < gv.mod.currentEncounter.MapSizeY)
                            {
                                if (!IsSquareOpen(plusY))
                                {
                                    foundPlace = false;
                                }
                            }
                            else
                            {
                                foundPlace = false;
                            }
                        }

                        if (summon.creatureSize == 4)
                        {

                            if (!IsSquareOpen(target))
                            {
                                foundPlace = false;
                            }

                            if (plusX.X < gv.mod.currentEncounter.MapSizeX)
                            {
                                if (!IsSquareOpen(plusX))
                                {
                                    foundPlace = false;
                                }
                            }
                            else
                            {
                                foundPlace = false;
                            }

                            if (plusY.Y < gv.mod.currentEncounter.MapSizeY)
                            {
                                if (!IsSquareOpen(plusY))
                                {
                                    foundPlace = false;
                                }
                            }
                            else
                            {
                                foundPlace = false;
                            }

                            if (plusXandY.X < gv.mod.currentEncounter.MapSizeX && plusXandY.Y < gv.mod.currentEncounter.MapSizeY)
                            {
                                if (!IsSquareOpen(plusXandY))
                                {
                                    foundPlace = false;
                                }
                            }
                            else
                            {
                                foundPlace = false;
                            }
                        }

                        //try to find a nearby square
                        if (foundPlace)
                        {
                            AddCreatureToCurrentEncounter(p1, target.X.ToString(), target.Y.ToString(), p4);
                        }
                        else
                        {
                            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                            //find correct summon spot, replace with nearest location if neccessary  

                            bool changeSummonLocation = false;// used as switch for cycling through all tiles in case the originally intended spot was occupied/not-walkable  
                            int targetTile = target.Y * gv.mod.currentEncounter.MapSizeX + target.X;//the index of the original target spot in the encounter's tiles list  
                            List<int> freeTilesByIndex = new List<int>();// a new list used to store the indices of all free tiles in the enocunter  
                            int tileLocX = 0;//just temporary storage in for locations of tiles  
                            int tileLocY = 0;//just temporary storage in for locations of tiles  
                            double floatTileLocY = 0;//was uncertain about rounding and conversion details, therefore need this one (see below)  
                            bool tileIsFree = true;//identify a tile suited as new summon loaction  
                            int nearestTileByIndex = -1;//store the nearest tile by index; as the relevant loop runs this will be replaced several times likely with ever nearer tiles  
                            int dist = 0;//distance between the orignally intended summon location and a free tile  
                            int lowestDist = 10000;//this storest the lowest ditance found while the loop runs  
                            int deltaX = 0;//temporary value used for distance calculation   
                            int deltaY = 0;//temporary value used for distance calculation   

                            //Check whether the target tile is free (then it's not neccessary to loop through any other tiles)  
                            //three checks are done in the following: walkable, occupied by creature, occupied by pc  

                            //TODO: for oversized cretaures
                            //which squares will the cretaure cover

                            //first check: check walkable  
                            //if (gv.mod.currentEncounter.encounterTiles[targetTile].Walkable == false)
                            /*
                            if (gv.mod.currentEncounter.encounterTiles[targetTile].Walkable == false)
                            {
                                changeSummonLocation = true;
                            }

                            //second check: check occupied by creature (only necceessary if walkable)  
                            if (changeSummonLocation == false)
                            {
                                foreach (Creature cr in gv.mod.currentEncounter.encounterCreatureList)
                                {
                                    if ((cr.combatLocX == target.X) && (cr.combatLocY == target.Y))
                                    {
                                        changeSummonLocation = true;
                                        break;
                                    }
                                }
                            }

                            //third check: check occupied by pc (only necceessary if walkable and not occupied by creature)  
                            if (changeSummonLocation == false)
                            {
                                foreach (Player pc in gv.mod.playerList)
                                {
                                    if ((pc.combatLocX == target.X) && (pc.combatLocY == target.Y))
                                    {
                                        changeSummonLocation = true;
                                        break;
                                    }
                                }
                            }
                            */
                            changeSummonLocation = true;
                            Coordinate target2 = new Coordinate();
                            //target square was already occupied/non-walkable, so all other tiles are searched for the NEAREST FREE tile to switch the summon location to  
                            if (changeSummonLocation == true)
                            {
                                //FIRST PART: get all FREE tiles in the current encounter  
                                for (int i = 0; i < gv.mod.currentEncounter.encounterTiles.Count; i++)
                                {
                                    //get the x and y location of current tile by calculation derived from index number, assuming that counting starts at top left corner of a map (0x, 0y)  
                                    //and that each horizintal x-line is counted first, then counting next horizonal x-line starting from the left again  
                                    tileIsFree = true;
                                    //Note: When e.g. MapsizeY is 7, the y values range from 0 to 6  
                                    //MODULO
                                    tileLocX = i % gv.mod.currentEncounter.MapSizeX;
                                    //Note: ensure rounding down here   
                                    floatTileLocY = i / gv.mod.currentEncounter.MapSizeX;
                                    tileLocY = (int)Math.Floor(floatTileLocY);
                                    target2.X = tileLocX;
                                    target2.Y = tileLocY;

                                    //code for large summons goes here, see above
                                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                                    plusX.X = target2.X + 1;
                                    plusX.Y = target2.Y;
                                    plusY.X = target2.X;
                                    plusY.Y = target2.Y + 1;
                                    plusXandY.X = target2.X + 1;
                                    plusXandY.Y = target2.Y + 1;

                                    foundPlace = true;

                                    if (summon.creatureSize == 1)
                                    {
                                        if (!IsSquareOpen(target2))
                                        {
                                            foundPlace = false;
                                        }
                                    }

                                    if (summon.creatureSize == 2)
                                    {

                                        if (!IsSquareOpen(target2))
                                        {
                                            foundPlace = false;
                                        }

                                        if (plusX.X < gv.mod.currentEncounter.MapSizeX)
                                        {
                                            if (!IsSquareOpen(plusX))
                                            {
                                                foundPlace = false;
                                            }
                                        }
                                        else
                                        {
                                            foundPlace = false;
                                        }
                                    }

                                    if (summon.creatureSize == 3)
                                    {

                                        if (!IsSquareOpen(target2))
                                        {
                                            foundPlace = false;
                                        }

                                        if (plusY.Y < gv.mod.currentEncounter.MapSizeY)
                                        {
                                            if (!IsSquareOpen(plusY))
                                            {
                                                foundPlace = false;
                                            }
                                        }
                                        else
                                        {
                                            foundPlace = false;
                                        }
                                    }

                                    if (summon.creatureSize == 4)
                                    {

                                        if (!IsSquareOpen(target2))
                                        {
                                            foundPlace = false;
                                        }

                                        if (plusX.X < gv.mod.currentEncounter.MapSizeX)
                                        {
                                            if (!IsSquareOpen(plusX))
                                            {
                                                foundPlace = false;
                                            }
                                        }
                                        else
                                        {
                                            foundPlace = false;
                                        }

                                        if (plusY.Y < gv.mod.currentEncounter.MapSizeY)
                                        {
                                            if (!IsSquareOpen(plusY))
                                            {
                                                foundPlace = false;
                                            }
                                        }
                                        else
                                        {
                                            foundPlace = false;
                                        }

                                        if (plusXandY.X < gv.mod.currentEncounter.MapSizeX && plusXandY.Y < gv.mod.currentEncounter.MapSizeY)
                                        {
                                            if (!IsSquareOpen(plusXandY))
                                            {
                                                foundPlace = false;
                                            }
                                        }
                                        else
                                        {
                                            foundPlace = false;
                                        }
                                    }


                                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                                    if (foundPlace)
                                    {
                                        tileIsFree = true;
                                    }
                                    else
                                    {
                                        tileIsFree = false;
                                    }
                                    /*
                                        //look at content of currently checked tile, again with three checks for walkable, occupied by creature, occupied by pc  
                                        //walkbale check  
                                        if (gv.mod.currentEncounter.encounterTiles[i].Walkable == false)
                                    {
                                        tileIsFree = false;
                                    }

                                    //creature occupied check  
                                    if (tileIsFree == true)
                                    {
                                        foreach (Creature cr in gv.mod.currentEncounter.encounterCreatureList)
                                        {
                                            if ((cr.combatLocX == tileLocX) && (cr.combatLocY == tileLocY))
                                            {
                                                tileIsFree = false;
                                                break;
                                            }
                                        }
                                    }

                                    //pc occupied check  
                                    if (tileIsFree == true)
                                    {
                                        foreach (Player pc in gv.mod.playerList)
                                        {
                                            if ((pc.combatLocX == tileLocX) && (pc.combatLocY == tileLocY))
                                            {
                                                tileIsFree = false;
                                                break;
                                            }
                                        }
                                    }
                                    */

                                    //this writes all free tiles into a fresh list; please note that the values of the elements of this new list are our relevant index values  
                                    //therefore it's not the index (which doesnt correalte to locations) in this list that's relevant, but the value of the element at that index  
                                    if (tileIsFree == true)
                                    {
                                        freeTilesByIndex.Add(i);
                                    }
                                }

                                //SECOND PART: find the free tile NEAREST to originally intended summon location  
                                for (int i = 0; i < freeTilesByIndex.Count; i++)
                                {
                                    dist = 0;

                                    //get location x and y of the tile stored at the index number i, i.e. get the value of elment indexed with i and transform to x and y location  
                                    tileLocX = freeTilesByIndex[i] % gv.mod.currentEncounter.MapSizeX;
                                    floatTileLocY = freeTilesByIndex[i] / gv.mod.currentEncounter.MapSizeX;
                                    tileLocY = (int)Math.Floor(floatTileLocY);

                                    //get distance between the current free tile and the originally intended summon location  
                                    deltaX = (int)Math.Abs((tileLocX - target.X));
                                    deltaY = (int)Math.Abs((tileLocY - target.Y));
                                    if (deltaX > deltaY)
                                    {
                                        dist = deltaX;
                                    }
                                    else
                                    {
                                        dist = deltaY;
                                    }

                                    //filter out the nearest tile by remembering it and its distance for further comparison while the loop runs through all free tiles  
                                    if (dist < lowestDist)
                                    {
                                        lowestDist = dist;
                                        nearestTileByIndex = freeTilesByIndex[i];
                                    }
                                }

                                if (nearestTileByIndex != -1)
                                {
                                    //get the nearest tile's x and y location and use it as creature summon coordinates  
                                    tileLocX = nearestTileByIndex % gv.mod.currentEncounter.MapSizeX;
                                    floatTileLocY = nearestTileByIndex / gv.mod.currentEncounter.MapSizeX;
                                    tileLocY = (int)Math.Floor(floatTileLocY);

                                    target.X = tileLocX;
                                    target.Y = tileLocY;
                                }

                            }

                            //just check whether a free squre does exist at all; if not, do not complete the summon  
                            if ((nearestTileByIndex != -1) || (changeSummonLocation == false))
                            {
                                AddCreatureToCurrentEncounter(p1, target.X.ToString(), target.Y.ToString(), p4);
                            }
                            else
                            {
                                gv.cc.addLogText("<yl>" + "Creature fails to appear, no valid space.</yl><BR>");
                            }

                            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        this.MessageBoxHtml("Failed to run script (" + filename + "): " + ex.ToString());
                    }
                    gv.errorLog(ex.ToString());
                }
            }
        }
        public void gcController(string filename, string prm1, string prm2, string prm3, string prm4)
        {
            if (!filename.Equals("none"))
            {
                try
                {
                    //go through each parm1-4 and replace if GlobalInt variable, GlobalString variable or rand(3-16)
                    string p1 = replaceParameter(prm1);
                    string p2 = replaceParameter(prm2);
                    string p3 = replaceParameter(prm3);
                    string p4 = replaceParameter(prm4);

                    if (filename.Equals("gcCheckGlobalInt.cs"))
                    {
                        int parm3 = Convert.ToInt32(p3);
                        gv.mod.returnCheck = CheckGlobalInt(prm1, prm2, parm3);
                    }
                    else if (filename.Equals("gcCheckIsInDarkness.cs"))
                    { 
                        gv.mod.returnCheck = CheckIsInDarkness(p1, p2);
                    }
                    else if (filename.Equals("gcCheckIsInHoursWindowDaily.cs"))
                    {
                        gv.mod.returnCheck = CheckIsInHoursWindowDaily(p1, p2);
                    }
                    else if (filename.Equals("gcCheckIsInDaysWindowWeekly.cs"))
                    {
                        gv.mod.returnCheck = CheckIsInDaysWindowWeekly(p1, p2);
                    }
                    else if (filename.Equals("gcCheckIsInWeeksWindowMonthly.cs"))
                    {
                        gv.mod.returnCheck = CheckIsInWeeksWindowMonthly(p1, p2);
                    }
                    else if (filename.Equals("gcCheckIsInMonthsWindowYearly.cs"))
                    {
                        gv.mod.returnCheck = CheckIsInMonthsWindowYearly(p1, p2);
                    }
                    else if (filename.Equals("gcCheckIsInFactionStrengthWindow.cs"))
                    {
                        int parm2 = Convert.ToInt32(p2);
                        int parm3 = Convert.ToInt32(p3);
                        gv.mod.returnCheck = CheckIsInFactionStrengthWindow(prm1, parm2, parm3);
                    }
                    else if (filename.Equals("gcCheckLocalInt.cs"))
                    {
                        //check to see if prm1 is thisprop or thisarea
                        if (prm1.Equals("thisprop"))
                        {
                            //find the prop at this location
                            prm1 = mod.currentArea.getPropByLocation(mod.PlayerLocationX, mod.PlayerLocationY).PropTag;
                        }
                        else if (prm1.Equals("thisarea"))
                        {
                            //use the currentArea
                            prm1 = mod.currentArea.Filename;
                        }
                        int parm4 = Convert.ToInt32(p4);
                        gv.mod.returnCheck = CheckLocalInt(prm1, prm2, prm3, parm4);
                    }
                    /*
                    else if (filename.Equals("gcCheckIsInDarkness.cs"))
                    {
                        //check to see if prm1 is thisprop or thisarea
                        if (prm1.Equals("thisprop"))
                        {
                            //find the prop at this location
                            prm1 = mod.currentArea.getPropByLocation(mod.PlayerLocationX, mod.PlayerLocationY).PropTag;
                        }
                        else if (prm1.Equals("thisarea"))
                        {
                            //use the currentArea
                            prm1 = mod.currentArea.Filename;
                        }
                        int parm4 = Convert.ToInt32(p4);
                        gv.mod.returnCheck = CheckLocalInt(prm1, prm2, prm3, parm4);
                    }
                    */
                    else if (filename.Equals("gcCheckGlobalString.cs"))
                    {
                        gv.mod.returnCheck = CheckGlobalString(prm1, p2);
                    }
                    else if (filename.Equals("gcCheckLocalString.cs"))
                    {
                        //check to see if prm1 is thisprop or thisarea
                        if (prm1.Equals("thisprop"))
                        {
                            //find the prop at this location
                            prm1 = mod.currentArea.getPropByLocation(mod.PlayerLocationX, mod.PlayerLocationY).PropTag;
                        }
                        else if (prm1.Equals("thisarea"))
                        {
                            //use the currentArea
                            prm1 = mod.currentArea.Filename;
                        }
                        gv.mod.returnCheck = CheckLocalString(prm1, prm2, p3);
                    }
                    else if (filename.Equals("gcCheckJournalEntryByTag.cs"))
                    {
                        int parm3 = Convert.ToInt32(p4);
                        gv.mod.returnCheck = CheckJournalEntry(prm1, prm2, parm3);
                    }
                    else if (filename.Equals("gcCheckForGold.cs"))
                    {
                        int parm1 = Convert.ToInt32(p1);
                        if (parm1 <= gv.mod.partyGold)
                        {
                            gv.mod.returnCheck = true;
                        }
                        else
                        {
                            gv.mod.returnCheck = false;
                        }
                    }
                    else if (filename.Equals("gcCheckAttribute.cs"))
                    {
                        int parm4 = Convert.ToInt32(p4);
                        int parm1 = 0;
                        if ((p1.Equals("")) || (p1.Equals("-1")))
                        {
                            parm1 = gv.mod.selectedPartyLeader;
                        }
                        else
                        {
                            parm1 = Convert.ToInt32(p1);
                        }
                        gv.mod.returnCheck = CheckAttribute(parm1, p2, p3, parm4);
                    }
                    else if (filename.Equals("gcCheckIsRace.cs"))
                    {
                        int parm1 = 0;
                        if ((p1.Equals("")) || (p1.Equals("-1")))
                        {
                            parm1 = gv.mod.selectedPartyLeader;
                        }
                        else
                        {
                            parm1 = Convert.ToInt32(p1);
                        }
                        gv.mod.returnCheck = CheckIsRace(parm1, p2);
                    }
                    else if (filename.Equals("gcCheckHasTrait.cs"))
                    {
                       
                        int parm1 = 0;
                        if (p1.Equals("") || p1.Equals("-1") || p1.Equals("leader") || p1.Equals("Leader"))
                        {
                            parm1 = gv.mod.selectedPartyLeader;
                        }
                        //highest
                        else if (p1.Equals("-2") || p1.Equals("highest") || p1.Equals("Highest"))
                        {
                            parm1 = -2;
                        }
                        //lowest
                        else if (p1.Equals("-3") || p1.Equals("lowest") || p1.Equals("Lowest"))
                        {
                            parm1 = -3;
                        }
                        //average
                        else if (p1.Equals("-4") || p1.Equals("average") || p1.Equals("Average"))
                        {
                            parm1 = -4;
                        }
                        //allMustSucceed
                        else if (p1.Equals("-5") || p1.Equals("allMustSucceed") || p1.Equals("AllMustSucceed"))
                        {
                            parm1 = -5;
                        }
                        //oneMustSucceed
                        else if (p1.Equals("-6") || p1.Equals("oneMustSucceed") || p1.Equals("OneMustSucceed"))
                        {
                            parm1 = -6;
                        }
                        //directly selected
                        else
                        {
                            parm1 = Convert.ToInt32(p1);
                            if (parm1 < 0 || parm1 > 5)
                            {
                                parm1 = gv.mod.selectedPartyLeader;
                            }
                        }
                        gv.mod.returnCheck = CheckHasTrait(parm1, p2);
                    }
                    else if (filename.Equals("gcCheckHasSpell.cs"))
                    {
                        int parm1 = 0;
                        //leader
                        if ((p1.Equals("")) || (p1.Equals("-1")))
                        {
                            parm1 = gv.mod.selectedPartyLeader;
                        }
                        //directly selected
                        else 
                        {
                            parm1 = Convert.ToInt32(p1);
                            if (parm1 < 0 || parm1 > 5)
                            {
                                parm1 = gv.mod.selectedPartyLeader;
                            }
                        }
                        gv.mod.returnCheck = CheckHasSpell(parm1, p2);
                    }
                    else if (filename.Equals("gcPassSkillCheck.cs"))
                    {
                        int parm1 = 0;
                        string traitMethod = "";
                        foreach (Trait t in gv.mod.moduleTraitsList)
                        {
                            if (t.tag == p2)
                            {
                                traitMethod = t.methodOfChecking;
                            }
                        }
                        
                        if (p1 == null)
                        {
                            if (traitMethod.Equals("-1") || traitMethod.Equals("leader") || traitMethod.Equals("Leader"))
                            {
                                parm1 = gv.mod.selectedPartyLeader;
                            }
                            else if (traitMethod.Equals("-2") || traitMethod.Equals("highest") || traitMethod.Equals("Highest"))
                            {
                                parm1 = -2;
                            }
                            else if (traitMethod.Equals("-3") || traitMethod.Equals("lowest") || traitMethod.Equals("Lowest"))
                            {
                                parm1 = -3;
                            }
                            else if (traitMethod.Equals("-4") || traitMethod.Equals("average") || traitMethod.Equals("Average"))
                            {
                                parm1 = -4;
                            }
                            else if (traitMethod.Equals("-5") || traitMethod.Equals("allMustSucceed") || traitMethod.Equals("AllMustSucceed"))
                            {
                                parm1 = -5;
                            }
                            else if (traitMethod.Equals("-6") || traitMethod.Equals("oneMustSucceed") || traitMethod.Equals("OneMustSucceed"))
                            {
                                parm1 = -6;
                            }
                        }
                        else if (p1.Equals(""))
                        {
                            if (traitMethod.Equals("-1") || traitMethod.Equals("leader") || traitMethod.Equals("Leader"))
                            {
                                parm1 = gv.mod.selectedPartyLeader;
                            }
                            else if (traitMethod.Equals("-2") || traitMethod.Equals("highest") || traitMethod.Equals("Highest"))
                            {
                                parm1 = -2;
                            }
                            else if (traitMethod.Equals("-3") || traitMethod.Equals("lowest") || traitMethod.Equals("Lowest"))
                            {
                                parm1 = -3;
                            }
                            else if (traitMethod.Equals("-4") || traitMethod.Equals("average") || traitMethod.Equals("Average"))
                            {
                                parm1 = -4;
                            }
                            else if (traitMethod.Equals("-5") || traitMethod.Equals("allMustSucceed") || traitMethod.Equals("AllMustSucceed"))
                            {
                                parm1 = -5;
                            }
                            else if (traitMethod.Equals("-6") || traitMethod.Equals("oneMustSucceed") || traitMethod.Equals("OneMustSucceed"))
                            {
                                parm1 = -6;
                            }
                        }
                        else if (p1.Equals("-1") || p1.Equals("leader")  || p1.Equals("Leader") )
                        {
                            parm1 = gv.mod.selectedPartyLeader;
                        }
                        //highest
                        else if ( p1.Equals("-2") || p1.Equals("highest") || p1.Equals("Highest")) 
                        {
                            parm1 = -2;
                        }
                        //lowest
                        else if ( p1.Equals("-3") || p1.Equals("lowest") || p1.Equals("Lowest")) 
                        {
                            parm1 = -3;
                        }
                        //average
                        else if (p1.Equals("-4") || p1.Equals("average") || p1.Equals("Average"))
                        {
                            parm1 = -4;
                        }
                        //allMustSucceed
                        else if ( p1.Equals("-5") || p1.Equals("allMustSucceed") || p1.Equals("AllMustSucceed")) 
                        {
                            parm1 = -5;
                        }
                        //oneMustSucceed
                        else if (p1.Equals("-6") || p1.Equals("oneMustSucceed") || p1.Equals("OneMustSucceed"))
                        {
                            parm1 = -6;
                        }
                        //directly selected
                        else
                        {
                            parm1 = Convert.ToInt32(p1);
                            if (parm1 < 0 || parm1 > 5)
                            {
                                parm1 = gv.mod.selectedPartyLeader;
                            }
                        }
                        int parm3 = Convert.ToInt32(p3);

                        bool useRollTen = false;
                        if (p4 != null)
                        {
                            if ((p4.Equals("false")) || (p4.Equals("False")) || (p4.Equals("")))
                            {
                                useRollTen = false;
                            }
                            else if ((p4.Equals("true")) || (p4.Equals("True")) || (p4.Equals("10")))
                            {
                                useRollTen = true;
                            }
                        }
                        gv.mod.returnCheck = CheckPassSkill(parm1, p2, parm3, useRollTen, false);
                    }
                    else if (filename.Equals("gcPassSkillCheckSilent.cs"))
                    {
                        int parm1 = 0;
                        string traitMethod = "";
                        foreach (Trait t in gv.mod.moduleTraitsList)
                        {
                            if (t.tag == p2)
                            {
                                traitMethod = t.methodOfChecking;
                            }
                        }

                        if (p1 == null)
                        {
                            if (traitMethod.Equals("-1") || traitMethod.Equals("leader") || traitMethod.Equals("Leader"))
                            {
                                parm1 = gv.mod.selectedPartyLeader;
                            }
                            else if (traitMethod.Equals("-2") || traitMethod.Equals("highest") || traitMethod.Equals("Highest"))
                            {
                                parm1 = -2;
                            }
                            else if (traitMethod.Equals("-3") || traitMethod.Equals("lowest") || traitMethod.Equals("Lowest"))
                            {
                                parm1 = -3;
                            }
                            else if (traitMethod.Equals("-4") || traitMethod.Equals("average") || traitMethod.Equals("Average"))
                            {
                                parm1 = -4;
                            }
                            else if (traitMethod.Equals("-5") || traitMethod.Equals("allMustSucceed") || traitMethod.Equals("AllMustSucceed"))
                            {
                                parm1 = -5;
                            }
                            else if (traitMethod.Equals("-6") || traitMethod.Equals("oneMustSucceed") || traitMethod.Equals("OneMustSucceed"))
                            {
                                parm1 = -6;
                            }
                        }
                        else if (p1.Equals(""))
                        {
                            if (traitMethod.Equals("-1") || traitMethod.Equals("leader") || traitMethod.Equals("Leader"))
                            {
                                parm1 = gv.mod.selectedPartyLeader;
                            }
                            else if (traitMethod.Equals("-2") || traitMethod.Equals("highest") || traitMethod.Equals("Highest"))
                            {
                                parm1 = -2;
                            }
                            else if (traitMethod.Equals("-3") || traitMethod.Equals("lowest") || traitMethod.Equals("Lowest"))
                            {
                                parm1 = -3;
                            }
                            else if (traitMethod.Equals("-4") || traitMethod.Equals("average") || traitMethod.Equals("Average"))
                            {
                                parm1 = -4;
                            }
                            else if (traitMethod.Equals("-5") || traitMethod.Equals("allMustSucceed") || traitMethod.Equals("AllMustSucceed"))
                            {
                                parm1 = -5;
                            }
                            else if (traitMethod.Equals("-6") || traitMethod.Equals("oneMustSucceed") || traitMethod.Equals("OneMustSucceed"))
                            {
                                parm1 = -6;
                            }
                        }
                        else if (p1.Equals("-1") || p1.Equals("leader") || p1.Equals("Leader"))
                        {
                            parm1 = gv.mod.selectedPartyLeader;
                        }
                        //highest
                        else if (p1.Equals("-2") || p1.Equals("highest") || p1.Equals("Highest"))
                        {
                            parm1 = -2;
                        }
                        //lowest
                        else if (p1.Equals("-3") || p1.Equals("lowest") || p1.Equals("Lowest"))
                        {
                            parm1 = -3;
                        }
                        //average
                        else if (p1.Equals("-4") || p1.Equals("average") || p1.Equals("Average"))
                        {
                            parm1 = -4;
                        }
                        //allMustSucceed
                        else if (p1.Equals("-5") || p1.Equals("allMustSucceed") || p1.Equals("AllMustSucceed"))
                        {
                            parm1 = -5;
                        }
                        //oneMustSucceed
                        else if (p1.Equals("-6") || p1.Equals("oneMustSucceed") || p1.Equals("OneMustSucceed"))
                        {
                            parm1 = -6;
                        }
                        //directly selected
                        else
                        {
                            parm1 = Convert.ToInt32(p1);
                            if (parm1 < 0 || parm1 > 5)
                            {
                                parm1 = gv.mod.selectedPartyLeader;
                            }
                        }
                        int parm3 = Convert.ToInt32(p3);

                        bool useRollTen = false;
                        if (p4 != null)
                        {
                            if ((p4.Equals("false")) || (p4.Equals("False")) || (p4.Equals("")))
                            {
                                useRollTen = false;
                            }
                            else if ((p4.Equals("true")) || (p4.Equals("True")) || (p4.Equals("10")))
                            {
                                useRollTen = true;
                            }
                        }
                        gv.mod.returnCheck = CheckPassSkill(parm1, p2, parm3, useRollTen, true);
                    }
                    else if (filename.Equals("gcCheckIsClassLevel.cs"))
                    {
                        int parm1 = 0;
                        if ((p1.Equals("")) || (p1.Equals("-1")))
                        {
                            parm1 = gv.mod.selectedPartyLeader;
                        }
                        else
                        {
                            parm1 = Convert.ToInt32(p1);
                        }
                        int parm3 = Convert.ToInt32(p3);
                        gv.mod.returnCheck = this.CheckIsClassLevel(parm1, p2, parm3);
                    }
                    else if (filename.Equals("gcCheckIsMale.cs"))
                    {
                        int parm1 = 0;
                        if ((p1.Equals("")) || (p1.Equals("-1")))
                        {
                            parm1 = gv.mod.selectedPartyLeader;
                        }
                        else
                        {
                            parm1 = Convert.ToInt32(p1);
                        }
                        gv.mod.returnCheck = CheckIsMale(parm1);
                    }
                    else if (filename.Equals("gcCheckPcInPartyByName.cs"))
                    {
                        gv.mod.returnCheck = false;
                        foreach (Player pc in gv.mod.playerList)
                        {
                            if (pc.name.Equals(p1))
                            {
                                gv.mod.returnCheck = true;
                            }
                        }
                    }
                    else if (filename.Equals("gcCheckSelectedPcName.cs"))
                    {
                        gv.mod.returnCheck = false;
                        if (gv.mod.playerList[gv.mod.selectedPartyLeader].name.Equals(p1))
                        {
                            gv.mod.returnCheck = true;
                        }
                    }
                    else if (filename.Equals("gcCheckForItem.cs"))
                    {
                        int parm2 = Convert.ToInt32(p2);
                        gv.mod.returnCheck = CheckForItem(p1, parm2);
                    }
                    else if (filename.Equals("gcCheckPartyDistance.cs"))
                    {
                        int parm2 = Convert.ToInt32(p2);
                        gv.mod.returnCheck = CheckPartyDistance(p1, parm2);
                    }
                    else if (filename.Equals("gcCheckPropIsShownByTag.cs"))
                    {
                        Prop prp = gv.mod.currentArea.getPropByTag(prm1);
                        if (prp != null)
                        {
                            gv.mod.returnCheck = prp.isShown;
                        }
                        else
                        {
                            gv.mod.returnCheck = false;
                            if (mod.debugMode) //SD_20131102
                            {
                                gv.cc.addLogText("<font color='yellow'>didn't find prop in this area, returning 'false' for isShown</font><BR>");
                            }
                        }
                    }
                    else if (filename.Equals("gcCheckProp.cs"))
                    {
                        gv.mod.returnCheck = CheckProp(p1, p2, prm3);
                    }
                    else if (filename.Equals("gcRand1of.cs"))
                    {
                        mod.returnCheck = false;
                        int parm1 = Convert.ToInt32(p1);
                        int rnd = gv.sf.RandInt(parm1);
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>Rand = " + rnd + "</font><BR>");
                        }
                        if (rnd == 1)
                        {
                            mod.returnCheck = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        this.MessageBoxHtml("Failed to run script (" + filename + "): " + ex.ToString());
                    }
                    gv.errorLog(ex.ToString());
                }
            }
        }

        public void ModifyBuyBack(string shoptag, string opertr, string value)
        {  
             Shop shp = gv.mod.getShopByTag(shoptag);  
             if (shp == null)  
             {  
                 if (mod.debugMode) //SD_20131102  
                 {  
                     gv.cc.addLogText("<yl>Could not find Shop: " + shoptag + ", aborting</yl><BR>");  
                 }  
                 return;                  
             }  
             try  
             {  
                 if (opertr.Equals("+"))  
                 {  
                     shp.buybackModifier += Convert.ToInt32(value);  
                 }  
                 else if (opertr.Equals("-"))  
                 {  
                     shp.buybackModifier -= Convert.ToInt32(value);  
                 }  
                 /*
                 else if (opertr.Equals("/"))  
                 {  
                     shp.buybackPercent /= Convert.ToInt32(value);  
                 }  
                 else if (opertr.Equals("*"))  
                 {  
                     shp.buybackPercent *= Convert.ToInt32(value);  
                 } 
                 */ 
                 else  
                 {  
                     shp.buybackModifier = Convert.ToInt32(value);  
                 }  
             }  
             catch (Exception ex)  
             {  
                 if (mod.debugMode) //SD_20131102  
                 {  
                     gv.cc.addLogText("<yl>Error modifying shop buyback: " + ex.ToString() + ", aborting</yl><BR>");  
                 }  
                 return;  
             }  
        }

        public void ModifySellPrice(string shoptag, string opertr, string value)
         {  
             Shop shp = gv.mod.getShopByTag(shoptag);  
             if (shp == null)  
             {  
                if (mod.debugMode) //SD_20131102  
                 {  
                     gv.cc.addLogText("<yl>Could not find Shop: " + shoptag + ", aborting</yl><BR>");  
                 }  
                 return;  
             }  
             try  
             {  
                 if (opertr.Equals("+"))  
                 {  
                     shp.sellModifier += Convert.ToInt32(value);  
                 }  
                 else if (opertr.Equals("-"))  
                 {  
                     shp.sellModifier -= Convert.ToInt32(value);  
                 }  
                 /*
                 else if (opertr.Equals("/"))  
                 {  
                     shp.sellPercent /= Convert.ToInt32(value);  
                 }  
                 else if (opertr.Equals("*"))  
                 {  
                     shp.sellPercent *= Convert.ToInt32(value);  
                 } 
                 */ 
                 else  
                 {  
                     shp.sellModifier = Convert.ToInt32(value);  
                 }  
             }  
             catch (Exception ex)  
             {  
                 if (mod.debugMode) //SD_20131102  
                 {  
                     gv.cc.addLogText("<yl>Error modifying shop buyback: " + ex.ToString() + ", aborting</yl><BR>");  
                 }  
                 return;  
            }  
        } 


        public void ogController(string filename, string prm1, string prm2, string prm3, string prm4)
        {
            if (!filename.Equals("none"))
            {
                try
                {
                    //go through each parm1-4 and replace if GlobalInt variable, GlobalString variable or rand(3-16)
                    string p1 = replaceParameter(prm1);
                    string p2 = replaceParameter(prm2);
                    string p3 = replaceParameter(prm3);
                    string p4 = replaceParameter(prm4);

                    if (filename.Equals("ogGetPartySize.cs"))
                    {
                        String val = gv.mod.playerList.Count + "";
                        SetGlobalInt(prm1, val);
                    }
                    else if (filename.Equals("ogGetNumberOfChargesMissing.cs"))
                    {
                        int counter = 0;

                        if (prm1 == "all")
                        {
                            foreach (ItemRefs ir in gv.mod.partyInventoryRefsList)
                            {
                                Item item = mod.getItemByResRef(ir.resref);

                                if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none"))
                                {
                                    if (ir.quantity < item.quantity)
                                    {
                                        counter += (item.quantity - ir.quantity);
                                    }
                                }
                            }

                            foreach (Player pc in gv.mod.playerList)
                            {
                                Item item = mod.getItemByResRef(pc.BodyRefs.resref);
                                if (item != null)
                                {
                                    if ((pc.BodyRefs.quantity < item.quantity))
                                    {
                                        counter += (item.quantity - pc.BodyRefs.quantity);
                                    }
                                }

                                item = mod.getItemByResRef(pc.OffHandRefs.resref);
                                if (item != null)
                                {
                                    if ((pc.OffHandRefs.quantity < item.quantity))
                                    {
                                        counter += (item.quantity - pc.OffHandRefs.quantity);
                                    }
                                }

                                item = mod.getItemByResRef(pc.MainHandRefs.resref);
                                if (item != null)
                                {
                                    if ((pc.MainHandRefs.quantity < item.quantity))
                                    {
                                        counter += (item.quantity - pc.MainHandRefs.quantity);
                                    }
                                }

                                item = mod.getItemByResRef(pc.RingRefs.resref);
                                if (item != null)
                                {
                                    if ((pc.RingRefs.quantity < item.quantity))
                                    {
                                        counter += (item.quantity - pc.RingRefs.quantity);
                                    }
                                }

                                item = mod.getItemByResRef(pc.Ring2Refs.resref);
                                if (item != null)
                                {
                                    if ((pc.Ring2Refs.quantity < item.quantity))
                                    {
                                        counter += (item.quantity - pc.Ring2Refs.quantity);
                                    }
                                }

                                item = mod.getItemByResRef(pc.HeadRefs.resref);
                                if (item != null)
                                {
                                    if ((pc.HeadRefs.quantity < item.quantity))
                                    {
                                        counter += (item.quantity - pc.HeadRefs.quantity);
                                    }
                                }

                                item = mod.getItemByResRef(pc.GlovesRefs.resref);
                                if (item != null)
                                {
                                    if ((pc.GlovesRefs.quantity < item.quantity))
                                    {
                                        counter += (item.quantity - pc.GlovesRefs.quantity);
                                    }
                                }

                                item = mod.getItemByResRef(pc.NeckRefs.resref);
                                if (item != null)
                                {
                                    if ((pc.NeckRefs.quantity < item.quantity))
                                    {
                                        counter += (item.quantity - pc.NeckRefs.quantity);
                                    }
                                }

                                item = mod.getItemByResRef(pc.FeetRefs.resref);
                                if (item != null)
                                {
                                    if ((pc.FeetRefs.quantity < item.quantity))
                                    {
                                        counter += (item.quantity - pc.FeetRefs.quantity);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Item masterItem = mod.getItemByResRef(prm1);

                            foreach (ItemRefs ir in gv.mod.partyInventoryRefsList)
                            {
                                if (ir.resref == prm1)
                                {
                                    if (ir.quantity < masterItem.quantity)
                                    {
                                        counter += (masterItem.quantity - ir.quantity);
                                    }
                                }
                            }

                            foreach (Player pc in gv.mod.playerList)
                            {
                                if (pc.BodyRefs.resref.Equals(prm1) && (pc.BodyRefs.quantity < masterItem.quantity))
                                {
                                    counter += (masterItem.quantity - pc.BodyRefs.quantity);
                                }
                                else if (pc.OffHandRefs.resref.Equals(prm1) && (pc.OffHandRefs.quantity < masterItem.quantity))
                                {
                                    counter += (masterItem.quantity - pc.OffHandRefs.quantity);
                                }
                                else if (pc.MainHandRefs.resref.Equals(prm1) && (pc.MainHandRefs.quantity < masterItem.quantity))
                                {
                                    counter += (masterItem.quantity - pc.MainHandRefs.quantity);
                                }
                                else if (pc.RingRefs.resref.Equals(prm1) && (pc.RingRefs.quantity < masterItem.quantity))
                                {
                                    counter += (masterItem.quantity - pc.RingRefs.quantity);
                                }
                                else if (pc.Ring2Refs.resref.Equals(prm1) && (pc.Ring2Refs.quantity < masterItem.quantity))
                                {
                                    counter += (masterItem.quantity - pc.Ring2Refs.quantity);
                                }
                                else if (pc.HeadRefs.resref.Equals(prm1) && (pc.HeadRefs.quantity < masterItem.quantity))
                                {
                                    counter += (masterItem.quantity - pc.HeadRefs.quantity);
                                }
                                else if (pc.GlovesRefs.resref.Equals(prm1) && (pc.GlovesRefs.quantity < masterItem.quantity))
                                {
                                    counter += (masterItem.quantity - pc.GlovesRefs.quantity);
                                }
                                else if (pc.NeckRefs.resref.Equals(prm1) && (pc.NeckRefs.quantity < masterItem.quantity))
                                {
                                    counter += (masterItem.quantity - pc.NeckRefs.quantity);
                                }
                                else if (pc.FeetRefs.resref.Equals(prm1) && (pc.FeetRefs.quantity < masterItem.quantity))
                                {
                                    counter += (masterItem.quantity - pc.NeckRefs.quantity);
                                }
                            }
                        }
                        SetGlobalInt(prm2, counter.ToString());
                    }
                    else if (filename.Equals("ogGetPartyRosterSize.cs"))
                    {
                        String val = gv.mod.partyRosterList.Count + "";
                        SetGlobalInt(prm1, val);
                    }
                    else if (filename.Equals("ogGetFactionStrength.cs"))
                    {
                        foreach (Faction f in gv.mod.moduleFactionsList)
                        {
                            if (f.tag == p1)
                            {
                                SetGlobalInt(p1, f.strength.ToString());
                            }
                        }
                    }
                    else if (filename.Equals("ogGetFactionGrowthRate.cs"))
                    {
                        foreach (Faction f in gv.mod.moduleFactionsList)
                        {
                            if (f.tag == p1)
                            {
                                SetGlobalInt(p1 + "GrowthRate", f.amountOfFactionStrengthChangePerInterval.ToString());
                            }
                        }
                    }
                    else if (filename.Equals("ogGetNumberOfCreaturesInEncounter.cs"))
                    {
                        String val = gv.mod.currentEncounter.encounterCreatureList.Count + "";
                        SetGlobalInt(prm1, val);
                    }
                    else if (filename.Equals("ogGetPropListSize.cs"))
                    {
                        String val = gv.mod.currentArea.Props.Count + "";
                        SetGlobalInt(prm1, val);
                    }
                    else if (filename.Equals("ogGetCurrentPlayerIndexUsingItem.cs"))
                    {
                        //String val = gv.cc.currentPlayerIndexUsingItem + "";
                        string val = gv.mod.indexOfPCtoLastUseItem + "";
                        SetGlobalInt(prm1, val);
                    }
                    else if (filename.Equals("ogGetCreatureCombatLocation.cs"))
                    {
                        Creature crt = GetCreature(prm1, p2);
                        if (crt == null)
                        {
                            return;
                        }
                        if (crt != null)
                        {
                            String valX = crt.combatLocX + "";
                            String valY = crt.combatLocY + "";
                            SetGlobalInt(prm3, valX);
                            SetGlobalInt(prm4, valY);
                        }
                    }
                    else if (filename.Equals("ogGetPropLocation.cs"))
                    {
                        Prop prp = GetProp(prm1, p2);
                        if (prp == null)
                        {
                            return;
                        }
                        if (prp != null)
                        {
                            String valX = prp.LocationX + "";
                            String valY = prp.LocationY + "";
                            SetGlobalInt(prm3, valX);
                            SetGlobalInt(prm4, valY);
                        }
                    }
                    else if (filename.Equals("ogGetPcCombatLocation.cs"))
                    {
                        Player pc = gv.mod.playerList[0];
                        if ((prm1 != null) && (!prm1.Equals("")))
                        {
                            pc = gv.mod.getPlayerByName(prm1);
                        }
                        else if ((p2 != null) && (!p2.Equals("")))
                        {
                            int parm2 = Convert.ToInt32(p2);
                            pc = gv.mod.playerList[parm2];
                        }
                        if (pc != null)
                        {
                            String valX = pc.combatLocX + "";
                            String valY = pc.combatLocY + "";
                            SetGlobalInt(prm3, valX);
                            SetGlobalInt(prm4, valY);
                        }
                    }
                    else if (filename.Equals("ogGetCreatureHp.cs"))
                    {
                        Creature crt = GetCreature(prm1, p2);
                        if (crt == null)
                        {
                            return;
                        }
                        if (crt != null)
                        {
                            String val = crt.hp + "";
                            SetGlobalInt(prm3, val);
                        }
                    }
                    else if (filename.Equals("ogGetCreatureSp.cs"))
                    {
                        Creature crt = GetCreature(prm1, p2);
                        if (crt == null)
                        {
                            return;
                        }
                        if (crt != null)
                        {
                            String val = crt.sp + "";
                            SetGlobalInt(prm3, val);
                        }
                    }
                    else if (filename.Equals("ogGetPlayerHp.cs"))
                    {
                        this.GetPlayerHp(prm1, p2, prm3);
                    }
                    else if (filename.Equals("ogGetPlayerSp.cs"))
                    {
                        this.GetPlayerSp(prm1, p2, prm3);
                    }
                    else if (filename.Equals("ogGetPartyLocation.cs"))
                    {
                        String valX = gv.mod.PlayerLocationX + "";
                        String valY = gv.mod.PlayerLocationY + "";
                        String valName = gv.mod.currentArea.Filename;
                        SetGlobalInt(prm1, valX);
                        SetGlobalInt(prm2, valY);
                        SetGlobalString(prm3, valName);
                    }
                    else if (filename.Equals("ogGetWorldTime.cs"))
                    {
                        //this stores time in minutes into a global int

                        //p1 key of global int to store time into; raw number in minutes; the engine automatically extends the key entered here 
                        //by either DateInformation or AutomaticCountDown, 
                        //depending on p2 setting (eg you enter DeadLineMike and it becomes DeadLineMikeAutomaticCountDown, if p2 is set so) 
                        //p2 type of time stored: 
                        //DateInformation (current world time in minutes + p3 as aditional time in hours) or
                        //AutomaticCountDown (only p3 as time or the countdown in hours)
                        //all keys, fully spelled out like DeadLineMikeAutomaticCountDown, can be used via <> in convos and in journal,
                        //like typing in a convo <DeadLineMikeAutomaticCountDown> to display the remaining time to the player
                        //they are automatically converted into a well readable time format
                        //furthermore, you can manipulate them like any global int via scripts, decreasing or extending a countdown manually or shifting a point in time
                        //AutomaticCountDown types are automatically decreased by the engine as ingame time passes
                        //p3 either the length of the countDown in hours (AutomaticCountDown) or an amount of time in hours added on top of current world time (DateInformation type)

                        //turn the modifier into a number (it shows hours)
                        int timeModifier = 0;
                        if (p3 != "none" && p3 != null)
                        {
                            timeModifier = Convert.ToInt32(p3);
                        }

                        if (p2 == "AutomaticCountDown" || p2 == "automaticCountDown" || p2 == "AutomaticCountdown" || p2 == "automaticCountdown" || p2 == "automaticcountdown")
                        {
                            String val = (timeModifier * 60) + "";
                            SetGlobalInt((p1 + "AutomaticCountDown"), val);
                        }
                        else if (p2 == "DateInformation" || p2 == "Dateinformation" || p2 == "dateInformation" || p2 == "dateinformation")
                        {
                            String val = (mod.WorldTime + (timeModifier * 60)) + "";
                            SetGlobalInt(p1 + "DateInformation", val);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        this.MessageBoxHtml("Failed to run script (" + filename + "): " + ex.ToString());
                    }
                    gv.errorLog(ex.ToString());
                }
            }
        }
        public void osController(string filename, string prm1, string prm2, string prm3, string prm4)
        {
            if (!filename.Equals("none"))
            {
                try
                {
                    //go through each parm1-4 and replace if GlobalInt variable, GlobalString variable or rand(3-16)
                    string p1 = replaceParameter(prm1);
                    string p2 = replaceParameter(prm2);
                    string p3 = replaceParameter(prm3);
                    string p4 = replaceParameter(prm4);

                    if (filename.Equals("osSetPlayerHp.cs"))
                    {
                        SetPlayerHp(prm1, p2, prm3, p4);
                    }
                    else if (filename.Equals("osSetPlayerSp.cs"))
                    {
                        SetPlayerSp(prm1, p2, prm3, p4);
                    }
                    else if (filename.Equals("osAddSpellToPlayer.cs"))
                    {
                        AddSpellToPlayer(prm1, p2, prm3);
                    }
                    else if (filename.Equals("osAddTraitToPlayer.cs"))
                    {
                        AddTraitToPlayer(prm1, p2, prm3);
                    }
                    else if (filename.Equals("osAddAllowedItemToPlayerClass.cs"))
                    {
                        AddAllowedItemToPlayerClass(prm1, p2);
                    }
                    else if (filename.Equals("osRemoveAllowedItemFromPlayerClass.cs"))
                    {
                        RemoveAllowedItemFromPlayerClass(prm1, p2);
                    }
                    else if (filename.Equals("osSetPlayerBaseStr.cs"))
                    {
                        SetPlayerBaseAtt(prm1, p2, prm3, p4, "str");
                    }
                    else if (filename.Equals("osSetPlayerBaseDex.cs"))
                    {
                        SetPlayerBaseAtt(prm1, p2, prm3, p4, "dex");
                    }
                    else if (filename.Equals("osSetPlayerBaseInt.cs"))
                    {
                        SetPlayerBaseAtt(prm1, p2, prm3, p4, "int");
                    }
                    else if (filename.Equals("osSetPlayerBaseCha.cs"))
                    {
                        SetPlayerBaseAtt(prm1, p2, prm3, p4, "cha");
                    }
                    else if (filename.Equals("osSetCreatureSp.cs"))
                    {
                        SetCreatureSp(prm1, p2, prm3, p4);
                    }
                    else if (filename.Equals("osSetCreatureHp.cs"))
                    {
                        SetCreatureHp(prm1, p2, prm3, p4);
                    }
                    else if (filename.Equals("osSetCreatureCombatLocation.cs"))
                    {
                        Creature crt = GetCreature(prm1, p2);
                        if (crt == null)
                        {
                            return;
                        }
                        if (crt != null)
                        {
                            crt.combatLocX = Convert.ToInt32(p3);
                            crt.combatLocY = Convert.ToInt32(p4);
                        }
                    }
                    else if (filename.Equals("osSetPropLocation.cs"))
                    {
                        Prop prp = GetProp(prm1, p2);
                        if (prp == null)
                        {
                            return;
                        }
                        if (prp != null)
                        {
                            prp.LocationX = Convert.ToInt32(p3);
                            prp.LocationY = Convert.ToInt32(p4);
                        }
                    }

                    else if (filename.Equals("osSetPropLocationAnyArea.cs"))
                    {
                        Prop prp = GetPropByUniqueTag(p1);
                        int added = 0;
                        if (prp == null)
                        {
                            return;
                        }
                        if (prp != null)
                        {

                            Prop prp2 = prp.DeepCopy();
                            //prp2.lastLocationX = prp.LocationX;
                            //prp2.lastLocationY = prp.LocationY;
                            prp2.LocationX = Convert.ToInt32(p3);
                            prp2.LocationY = Convert.ToInt32(p4);
                            gv.cc.DisposeOfBitmap(ref prp2.token);
                            prp2.token = gv.cc.LoadBitmap(prp.ImageFileName);
                           
                            for (int i2 = 0; i2 < gv.mod.moduleAreasObjects.Count; i2++)
                            {
                                if (gv.mod.moduleAreasObjects[i2].Filename == p2)
                                {
                                    added = 1;
                                    gv.mod.moduleAreasObjects[i2].Props.Add(prp2);   
                                    break;
                                }
                            }
                              
                            if (added == 1)
                            {
                                for (int j2 = gv.mod.moduleAreasObjects.Count -1; j2 > -1; j2--)
                                {
                                    for (int k2 = gv.mod.moduleAreasObjects[j2].Props.Count -1; k2 > -1; k2--)
                                    {
                                        //prevent removing the prop from the area it was just addded to
                                        if (gv.mod.moduleAreasObjects[j2].Props[k2].PropTag.Equals(p1) && (gv.mod.moduleAreasObjects[j2].Filename != p2))
                                        {
                                            gv.mod.moduleAreasObjects[j2].Props.RemoveAt(k2);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    
                    else if (filename.Equals("osSetTriggerSingleLocation.cs"))
                    {
                        Trigger t = gv.mod.currentArea.getTriggerByTag(prm1);
                        if (t != null)
                        {
                            t.TriggerSquaresList.Clear();
                            Coordinate newCoor = new Coordinate(Convert.ToInt32(p2), Convert.ToInt32(p3));
                            t.TriggerSquaresList.Add(newCoor);
                        }
                    }
                    else if (filename.Equals("osSetPropIsShownByTag.cs"))
                    {
                        SetPropIsShown(prm1, prm2);
                    }
                    else if (filename.Equals("osSetPropIsMover.cs"))
                    {
                        SetPropIsMover(prm1, prm2);
                    }
                    else if (filename.Equals("osSetProp.cs"))
                    {
                        SetProp(p1, p2, prm3, prm4);
                    }
                    else if (filename.Equals("osSetWorldTime.cs"))
                    {
                        SetWorldTime(prm1, p2, prm3);
                    }
                }
                catch (Exception ex)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        this.MessageBoxHtml("Failed to run script (" + filename + "): " + ex.ToString());
                    }
                    gv.errorLog(ex.ToString());
                }
            }
            
        }

        public void AddCreatureToCurrentEncounter(string p1, string p2, string p3, string p4)
         {  
             //p1 is the resref of the added creature (use one from a blueprint in the toolset's creature blueprints section)  
             //p2 x location of the creature in current encounter (will be automatically adjusted to nearest location if the spot is already occupied or non-walkable)  
             //p3 y location of the creature in current encounter (will be automatically adjusted to nearest location if the spot is already occupied or non-walkable)  
             //p4 is the duration in turns that the creature will stay
   
             foreach (Creature c in gv.mod.moduleCreaturesList)  
             {  
                 if (c.cr_resref.Equals(p1))  
                 {  
                   //fetch the data for our creature by making a blueprint(object) copy  
                    Creature copy = c.DeepCopy();
                    copy.stayDurationInTurns = Convert.ToInt32(p4);

                    //test if still needed with IB2 later on
                    //crucial for loading the creature token
                    gv.cc.DisposeOfBitmap(ref copy.token);
                    copy.token = gv.cc.LoadBitmap(copy.cr_tokenFilename);

                    //Automaically create a unique tag                      
                    copy.cr_tag = "SummonTag" + mod.getNextIdNumber();  
                     if (mod.debugMode)  
                     {  
                         gv.cc.addLogText("<bu>Added Creature tag is: " + copy.cr_tag + "</bu>");  
                     }
                       
                    copy.moveOrder = gv.screenCombat.moveOrderList.Count;
                    copy.combatLocX = Convert.ToInt32(p2);
                    copy.combatLocY = Convert.ToInt32(p3);

                    //finally add creature  
                    mod.currentEncounter.encounterCreatureList.Add(copy);
                        //add to end of move order  
                        MoveOrder newMO = new MoveOrder();
                        newMO.PcOrCreature = copy;
                        newMO.rank = 1000;
                        gv.screenCombat.moveOrderList.Add(newMO);
                        //increment the number of initial move order objects
                        //note: check how ini bar system will interact with creatures added while battle is running  
                        gv.screenCombat.initialMoveOrderListSize++;
                        //add to encounter xp  
                        gv.screenCombat.encounterXP += copy.cr_XP;
                }  
             }  
        }  

        public string replaceParameter(string parm)
        {
            if (parm == null) { return parm; }
            //check to see if it is a GlobalInt
            foreach (GlobalInt g in gv.mod.moduleGlobalInts)
            {
                if (g.Key.Equals(parm))
                {
                    if (gv.mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>Replaced " + parm + " with " + g.Value + "</font><BR>");
                    }
                    return g.Value + "";
                }
            }
            //check to see if it is a GlobatString
            foreach (GlobalString g in gv.mod.moduleGlobalStrings)
            {
                if (g.Key.Equals(parm))
                {
                    if (gv.mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>Replaced " + parm + " with " + g.Value + "</font><BR>");
                    }
                    return g.Value;
                }
            }
            //check to see if it is a rand(3-16)
            if (parm.StartsWith("rand("))
            {
                string firstNum = parm.Split('(', '-')[1]; //.Substring(parm.IndexOf("(") + 1, parm.IndexOf("-"));
                string lastNum = parm.Split('-', ')')[1]; //.Substring(parm.IndexOf("-") + 1, parm.IndexOf(")"));
                int fNum = Convert.ToInt32(firstNum);
                int lNum = Convert.ToInt32(lastNum);
                int returnRand = gv.sf.RandInt(fNum, lNum);
                if (gv.mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>Replaced " + parm + " with " + returnRand + "</font><BR>");
                }
                return returnRand + "";
            }

            return parm; //did not find a replacement so send back original
        }

        //GLOBAL AND LOCAL INTS
        public void SetGlobalInt(string variableName, string val)
        {
            int value = 0;
            if (val.Equals("++"))
            {
                int currentValue = GetGlobalInt(variableName);
                if (currentValue == -1) //this is our first time using this variable so set to 1
                {
                    //SetGlobalInt(p1, 1);
                    value = 1;
                }
                else //we have the variable so increment by one
                {
                    currentValue++;
                    //sf.SetGlobalInt(p1, currentValue);
                    value = currentValue;
                }
            }
            else if (val.Equals("--"))
            {
                int currentValue = GetGlobalInt(variableName);
                if (currentValue == -1) //this is our first time using this variable so set to 0
                {
                    //sf.SetGlobalInt(p1, 0);
                    value = 0;
                }
                else //we have the variable so decrement by one
                {
                    currentValue--;
                    if (currentValue < 0) { currentValue = 0; }
                    //sf.SetGlobalInt(p1, currentValue);
                    value = currentValue;
                }
            }
            else
            {
                value = Convert.ToInt32(val);
                //int parm2 = Convert.ToInt32(p2);
                //sf.SetGlobalInt(p1, parm2);
            }
            int exists = 0;
            foreach (GlobalInt variable in mod.moduleGlobalInts)
            {
                if (variable.Key.Equals(variableName))
                {
                    variable.Value = value;
                    exists = 1;
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "setGlobal: " + variableName + " = " + value + "</font>" +
                                "<BR>");
                    }
                }
            }
            if (exists == 0)
            {
                GlobalInt newGlobal = new GlobalInt();
                newGlobal.Key = variableName;
                newGlobal.Value = value;
                mod.moduleGlobalInts.Add(newGlobal);
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "setGlobal: " + variableName + " = " + value + "</font>" +
                            "<BR>");
                }
            }
        }
        public void SetLocalInt(string objectTag, string variableName, string val)
        {
            int value = 0;
            if (val.Equals("++"))
            {
                int currentValue = GetLocalInt(objectTag, variableName);
                if (currentValue == -1) //this is our first time using this variable so set to 1
                {
                    value = 1;
                }
                else //we have the variable so increment by one
                {
                    currentValue++;
                    value = currentValue;
                }
            }
            else if (val.Equals("--"))
            {
                int currentValue = GetLocalInt(objectTag, variableName);
                if (currentValue == -1) //this is our first time using this variable so set to 0
                {
                    value = 0;
                }
                else //we have the variable so decrement by one
                {
                    currentValue--;
                    if (currentValue < 0) { currentValue = 0; }
                    value = currentValue;
                }
            }
            else
            {
                value = Convert.ToInt32(val);
            }

            //check creatures and areas and props
            foreach (Area a in mod.moduleAreasObjects)
            {
                if (a.Filename.Equals(objectTag))
                {
                    int exists = 0;
                    foreach (LocalInt variable in a.AreaLocalInts)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            variable.Value = value;
                            exists = 1;
                        }
                    }
                    if (exists == 0)
                    {
                        LocalInt newLocalInt = new LocalInt();
                        newLocalInt.Key = variableName;
                        newLocalInt.Value = value;
                        a.AreaLocalInts.Add(newLocalInt);
                    }
                    return;
                }
                else
                {
                    foreach (Prop p in a.Props)
                    {
                        if (p.PropTag.Equals(objectTag))
                        {
                            int exists = 0;
                            foreach (LocalInt variable in p.PropLocalInts)
                            {
                                if (variable.Key.Equals(variableName))
                                {
                                    variable.Value = value;
                                    exists = 1;
                                }
                            }
                            if (exists == 0)
                            {
                                LocalInt newLocalInt = new LocalInt();
                                newLocalInt.Key = variableName;
                                newLocalInt.Value = value;
                                p.PropLocalInts.Add(newLocalInt);
                            }
                            return;
                        }
                    }
                }
            }
            foreach (Creature cr in mod.currentEncounter.encounterCreatureList)
            {
                if (cr.cr_tag.Equals(objectTag))
                {
                    int exists = 0;
                    foreach (LocalInt variable in cr.CreatureLocalInts)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            variable.Value = value;
                            exists = 1;
                        }
                    }
                    if (exists == 0)
                    {
                        LocalInt newLocalInt = new LocalInt();
                        newLocalInt.Key = variableName;
                        newLocalInt.Value = value;
                        cr.CreatureLocalInts.Add(newLocalInt);
                    }
                    return;
                }
            }
            if (mod.debugMode)
            {
                gv.cc.addLogText("<font color='yellow'>" + objectTag + " setLocal: " + variableName + " = " + value + "</font><BR>");
            }
        }
        public int GetGlobalInt(string variableName)
        {
            foreach (GlobalInt variable in mod.moduleGlobalInts)
            {
                if (variable.Key.Equals(variableName))
                {
                    return variable.Value;
                }
            }
            if (mod.debugMode) //SD_20131102
            {
                gv.cc.addLogText("<font color='yellow'>" + "Couldn't find the tag specified...returning a value of -1" + "</font>" +
                        "<BR>");
            }
            return -1;
        }
        public int GetLocalInt(string objectTag, string variableName)
        {
            //check creatures, areas        
            foreach (Area a in mod.moduleAreasObjects)
            {
                if (a.Filename.Equals(objectTag))
                {
                    foreach (LocalInt variable in a.AreaLocalInts)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            return variable.Value;
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>Found the object, but couldn't find the tag specified...returning a value of -1</font><BR>");
                    }
                    return -1;
                }
                else
                {
                    foreach (Prop p in a.Props)
                    {
                        if (p.PropTag.Equals(objectTag))
                        {
                            foreach (LocalInt variable in p.PropLocalInts)
                            {
                                if (variable.Key.Equals(variableName))
                                {
                                    return variable.Value;
                                }
                            }
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>Found the object, but couldn't find the tag specified...returning a value of -1</font><BR>");
                            }
                            return -1;
                        }
                    }
                }
            }
            foreach (Creature cr in mod.currentEncounter.encounterCreatureList)
            {
                if (cr.cr_tag.Equals(objectTag))
                {
                    foreach (LocalInt variable in cr.CreatureLocalInts)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            return variable.Value;
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>Found the object, but couldn't find the tag specified...returning a value of -1</font><BR>");
                    }
                    return -1;
                }
            }
            if (mod.debugMode)
            {
                gv.cc.addLogText("<font color='yellow'>couldn't find the object with the tag specified (only Creatures and Areas), returning a value of -1</font><BR>");
            }
            return -1;
        }

        public bool CheckIsInHoursWindowDaily(string startHour, string endHour)
        {
            int start = Convert.ToInt32(startHour);
            int end = Convert.ToInt32(endHour);
            start *= 60;//convert into minutes
            end *= 60;//convert into minutes
            int time = gv.mod.WorldTime % 1440;//days begins at 0 minutes(0.00) goes to 1440 minutes (24.00), 0 to 24 for start/end
 
            if (start <= end)
            {
                if (start <= time && end >= time)
                {
                    return true;
                }
            }
            //we check for something like night, crossing the 24.00 line; from 23.00 to 24.00 is(23-24)
            else
            {
                if (start <= time || end >= time)
                {
                    return true;
                }
            } 

            return false;
        }

        public bool CheckIsInDaysWindowWeekly(string startDay, string endDay)
        {
            int start = Convert.ToInt32(startDay);
            int end = Convert.ToInt32(endDay);
            start *= 1440;//convert into minutes
            end *= 1440;//convert into minutes
            int time = gv.mod.WorldTime % (1440*7);//seven values (0 -7 ), monday (0-1), tuesday (1-2), wednesday (2-3), thursday (3-4), friday (4-5), saturday (5-6), sunday(6-7)

            if (start <= end)
            {
                if (start <= time && end >= time)
                {
                    return true;
                }
            }
            //we check for something like from Sunday(6) to Monday(1), whole sunday is eg (6-7)
            else
            {
                if (start <= time || end >= time)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckIsInWeeksWindowMonthly(string startWeek, string endWeek)
        {
            int start = Convert.ToInt32(startWeek);
            int end = Convert.ToInt32(endWeek);
            start *= (1440 * 7);//convert into minutes
            end *= (1440 * 7);//convert into minutes
            int time = gv.mod.WorldTime % (1440 * 7 * 4);//month begins at 0 and end at 4 (week1(0-1), week2(1-2), week3(2-3) and week4(3-4)

            if (start <= end)
            {
                if (start <= time && end >= time)
                {
                    return true;
                }
            }
            //we check for something like from fourth week(3) to first week(1), within week4 is eg 3-4
            else
            {
                if (start <= time || end >= time)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckIsInMonthsWindowYearly(string startMonth, string endMonth)
        {
            int start = Convert.ToInt32(startMonth);
            int end = Convert.ToInt32(endMonth);
            start *= (1440 * 7 * 4);//convert into minutes
            end *= (1440 * 7 * 4);//convert into minutes
            int time = gv.mod.WorldTime % (1440 * 7 * 4 * 12);//year begins at 0 and end at 12 ((0-1),(1-2),(2-3)...(11-12))

            if (start <= end)
            {
                if (start <= time && end >= time)
                {
                    return true;
                }
            }
            //we check for something like from december(11) to January(1), within december is eg (11-12)
            else
            {
                if (start <= time || end >= time)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckIsInDarkness(string identifier, string darkMode)
        {
            //the area does not use the light system, nothing is in darkness
            if (!gv.mod.currentArea.useLightSystem)
            {
                return false;
            }

            #region party
            else if (identifier == "party" || identifier == "Party")
           {
                bool isLit = false;
                foreach (bool litState in gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].isLit)
                {
                    if (litState)
                    {
                        isLit = true;
                        break;
                    }
                }

                //outdoor area, using day&night
                if (gv.mod.currentArea.UseDayNightCycle && gv.mod.currentArea.useLightSystem)
                {
                    //mode wil need utter dark, so this outdoor si always false
                    if (darkMode == "noLight" || darkMode == "NoLight")
                    {
                        return false;
                    }
                    // mode is cheking for ngith state
                    else
                    {
                        if (isLit)
                        {
                            return false;
                        }
                        //not lit
                        else
                        {
                            int dawn = 5 * 60;
                            int sunrise = 6 * 60;
                            int day = 7 * 60;
                            int sunset = 17 * 60;
                            int dusk = 18 * 60;
                            int night = 20 * 60;
                            int time = gv.mod.WorldTime % 1440;

                            if ((time >= dawn) && (time < sunrise))
                            {
                                return false;
                            }
                            else if ((time >= sunrise) && (time < day))
                            {
                                return false;
                            }
                            else if ((time >= day) && (time < sunset))
                            {
                                return false;
                            }
                            else if ((time >= sunset) && (time < dusk))
                            {
                                return false;
                            }
                            else if ((time >= dusk) && (time < night))
                            {
                                return false;
                            }
                            else if ((time >= night) || (time < dawn))
                            {
                                return true;
                            }

                        }//tile was not lit
                    }//mode was looking for night state
                }// end of outdoor, & lightSystem

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //inddor area, using light system
                else if (!gv.mod.currentArea.UseDayNightCycle && gv.mod.currentArea.useLightSystem)
                {
                    //mode will need outdoor night, so this indoor dark
                    if (darkMode == "night")
                    {
                        return false;
                    }
                    // mode is checking for indoor dark state
                    else
                    {
                        if (isLit)
                        {
                            return false;
                        }
                        //not lit
                        else
                        {
                            return true;
                        }//tile was not lit
                    }//mode was looking for indoor dark state
                }// end of indoor & lightSystem             
            }//end of "party" identifier
            #endregion

            #region partyLast
            else if (identifier == "partyLast" || identifier == "PartyLast")
            {
                bool isLit = false;
                foreach (bool litState in gv.mod.currentArea.Tiles[gv.mod.PlayerLastLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLastLocationX].isLit)
                {
                    if (litState)
                    {
                        isLit = true;
                        break;
                    }
                }

                //outdoor area, using day&night
                if (gv.mod.currentArea.UseDayNightCycle && gv.mod.currentArea.useLightSystem)
                {
                    //mode wil need utter dark, so this outdoor si always false
                    if (darkMode == "noLight" || darkMode == "NoLight")
                    {
                        return false;
                    }
                    // mode is cheking for ngith state
                    else
                    {
                        if (isLit)
                        {
                            return false;
                        }
                        //not lit
                        else
                        {
                            int dawn = 5 * 60;
                            int sunrise = 6 * 60;
                            int day = 7 * 60;
                            int sunset = 17 * 60;
                            int dusk = 18 * 60;
                            int night = 20 * 60;
                            int time = gv.mod.WorldTime % 1440;

                            if ((time >= dawn) && (time < sunrise))
                            {
                                return false;
                            }
                            else if ((time >= sunrise) && (time < day))
                            {
                                return false;
                            }
                            else if ((time >= day) && (time < sunset))
                            {
                                return false;
                            }
                            else if ((time >= sunset) && (time < dusk))
                            {
                                return false;
                            }
                            else if ((time >= dusk) && (time < night))
                            {
                                return false;
                            }
                            else if ((time >= night) || (time < dawn))
                            {
                                return true;
                            }

                        }//tile was not lit
                    }//mode was looking for night state
                }// end of outdoor, & lightSystem

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //inddor area, using light system
                else if (!gv.mod.currentArea.UseDayNightCycle && gv.mod.currentArea.useLightSystem)
                {
                    //mode will need outdoor night, so this indoor dark
                    if (darkMode == "night")
                    {
                        return false;
                    }
                    // mode is checking for indoor dark state
                    else
                    {
                        if (isLit)
                        {
                            return false;
                        }
                        //not lit
                        else
                        {
                            return true;
                        }//tile was not lit
                    }//mode was looking for indoor dark state
                }// end of indoor & lightSystem             
            }//end of "party" identifier
            #endregion

            #region thisProp
            else if (identifier == "thisProp" || identifier == "ThisProp")
            {
                bool isLit = false;
                Prop tempProp = new Prop();
                bool foundProp = false;
                foreach (Prop p in gv.mod.currentArea.Props)
                {
                    if (p.PropTag == gv.mod.currentPropTag)
                    {
                        tempProp = p;
                        foundProp = true;
                        break;
                    }
                }
                if (foundProp)
                {
                    foreach (bool litState in gv.mod.currentArea.Tiles[tempProp.LocationY * gv.mod.currentArea.MapSizeX + tempProp.LocationX].isLit)
                    {
                        if (litState)
                        {
                            isLit = true;
                            break;
                        }
                    }

                    //outdoor area, using day&night
                    if (gv.mod.currentArea.UseDayNightCycle && gv.mod.currentArea.useLightSystem)
                    {
                        //mode wil need utter dark, so this outdoor si always false
                        if (darkMode == "noLight" || darkMode == "NoLight")
                        {
                            return false;
                        }
                        // mode is cheking for ngith state
                        else
                        {
                            if (isLit)
                            {
                                return false;
                            }
                            //not lit
                            else
                            {
                                int dawn = 5 * 60;
                                int sunrise = 6 * 60;
                                int day = 7 * 60;
                                int sunset = 17 * 60;
                                int dusk = 18 * 60;
                                int night = 20 * 60;
                                int time = gv.mod.WorldTime % 1440;

                                if ((time >= dawn) && (time < sunrise))
                                {
                                    return false;
                                }
                                else if ((time >= sunrise) && (time < day))
                                {
                                    return false;
                                }
                                else if ((time >= day) && (time < sunset))
                                {
                                    return false;
                                }
                                else if ((time >= sunset) && (time < dusk))
                                {
                                    return false;
                                }
                                else if ((time >= dusk) && (time < night))
                                {
                                    return false;
                                }
                                else if ((time >= night) || (time < dawn))
                                {
                                    return true;
                                }

                            }//tile was not lit
                        }//mode was looking for night state
                    }// end of outdoor, & lightSystem

                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    //inddor area, using light system
                    else if (!gv.mod.currentArea.UseDayNightCycle && gv.mod.currentArea.useLightSystem)
                    {
                        //mode will need outdoor night, so this indoor dark
                        if (darkMode == "night")
                        {
                            return false;
                        }
                        // mode is checking for indoor dark state
                        else
                        {
                            if (isLit)
                            {
                                return false;
                            }
                            //not lit
                            else
                            {
                                return true;
                            }//tile was not lit
                        }//mode was looking for indoor dark state
                    }// end of indoor & lightSystem
                }//found prop             
            }//end of "party" identifier
            #endregion

            #region thisPropLast
            else if (identifier == "thisPropLast" || identifier == "ThisPropLast")
            {
                bool isLit = false;
                Prop tempProp = new Prop();
                foreach (Prop p in gv.mod.currentArea.Props)
                {
                    if (p.PropTag == gv.mod.currentPropTag)
                    {
                        tempProp = p;
                        break;
                    }
                }

                foreach (bool litState in gv.mod.currentArea.Tiles[tempProp.lastLocationY * gv.mod.currentArea.MapSizeX + tempProp.lastLocationX].isLit)
                {
                    if (litState)
                    {
                        isLit = true;
                        break;
                    }
                }

                //outdoor area, using day&night
                if (gv.mod.currentArea.UseDayNightCycle && gv.mod.currentArea.useLightSystem)
                {
                    //mode wil need utter dark, so this outdoor si always false
                    if (darkMode == "noLight" || darkMode == "NoLight")
                    {
                        return false;
                    }
                    // mode is cheking for ngith state
                    else
                    {
                        if (isLit)
                        {
                            return false;
                        }
                        //not lit
                        else
                        {
                            int dawn = 5 * 60;
                            int sunrise = 6 * 60;
                            int day = 7 * 60;
                            int sunset = 17 * 60;
                            int dusk = 18 * 60;
                            int night = 20 * 60;
                            int time = gv.mod.WorldTime % 1440;

                            if ((time >= dawn) && (time < sunrise))
                            {
                                return false;
                            }
                            else if ((time >= sunrise) && (time < day))
                            {
                                return false;
                            }
                            else if ((time >= day) && (time < sunset))
                            {
                                return false;
                            }
                            else if ((time >= sunset) && (time < dusk))
                            {
                                return false;
                            }
                            else if ((time >= dusk) && (time < night))
                            {
                                return false;
                            }
                            else if ((time >= night) || (time < dawn))
                            {
                                return true;
                            }

                        }//tile was not lit
                    }//mode was looking for night state
                }// end of outdoor, & lightSystem

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //inddor area, using light system
                else if (!gv.mod.currentArea.UseDayNightCycle && gv.mod.currentArea.useLightSystem)
                {
                    //mode will need outdoor night, so this indoor dark
                    if (darkMode == "night")
                    {
                        return false;
                    }
                    // mode is checking for indoor dark state
                    else
                    {
                        if (isLit)
                        {
                            return false;
                        }
                        //not lit
                        else
                        {
                            return true;
                        }//tile was not lit
                    }//mode was looking for indoor dark state
                }// end of indoor & lightSystem             
            }//end of "party" identifier
            #endregion

            #region PropByTag
            else
            {
                bool isLit = false;
                Prop tempProp = new Prop();
                bool foundProp = false;
                foreach (Prop p in gv.mod.currentArea.Props)
                {
                    if (p.PropTag == identifier)
                    {
                        tempProp = p;
                        foundProp = true;
                        break;
                    }
                }

                if (foundProp)
                {

                    foreach (bool litState in gv.mod.currentArea.Tiles[tempProp.LocationY * gv.mod.currentArea.MapSizeX + tempProp.LocationX].isLit)
                    {
                        if (litState)
                        {
                            isLit = true;
                            break;
                        }
                    }

                    //outdoor area, using day&night
                    if (gv.mod.currentArea.UseDayNightCycle && gv.mod.currentArea.useLightSystem)
                    {
                        //mode wil need utter dark, so this outdoor si always false
                        if (darkMode == "noLight" || darkMode == "NoLight")
                        {
                            return false;
                        }
                        // mode is cheking for ngith state
                        else
                        {
                            if (isLit)
                            {
                                return false;
                            }
                            //not lit
                            else
                            {
                                int dawn = 5 * 60;
                                int sunrise = 6 * 60;
                                int day = 7 * 60;
                                int sunset = 17 * 60;
                                int dusk = 18 * 60;
                                int night = 20 * 60;
                                int time = gv.mod.WorldTime % 1440;

                                if ((time >= dawn) && (time < sunrise))
                                {
                                    return false;
                                }
                                else if ((time >= sunrise) && (time < day))
                                {
                                    return false;
                                }
                                else if ((time >= day) && (time < sunset))
                                {
                                    return false;
                                }
                                else if ((time >= sunset) && (time < dusk))
                                {
                                    return false;
                                }
                                else if ((time >= dusk) && (time < night))
                                {
                                    return false;
                                }
                                else if ((time >= night) || (time < dawn))
                                {
                                    return true;
                                }

                            }//tile was not lit
                        }//mode was looking for night state
                    }// end of outdoor, & lightSystem

                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    //inddor area, using light system
                    else if (!gv.mod.currentArea.UseDayNightCycle && gv.mod.currentArea.useLightSystem)
                    {
                        //mode will need outdoor night, so this indoor dark
                        if (darkMode == "night")
                        {
                            return false;
                        }
                        // mode is checking for indoor dark state
                        else
                        {
                            if (isLit)
                            {
                                return false;
                            }
                            //not lit
                            else
                            {
                                return true;
                            }//tile was not lit
                        }//mode was looking for indoor dark state
                    }// end of indoor & lightSystem
                }//found a prop             
            }//end of "party" identifier
            #endregion

            return false;
        }


        public bool CheckIsInFactionStrengthWindow(string factionTag, int minFactionStrength,int maxFactionStrength)
        {
            foreach (Faction f in gv.mod.moduleFactionsList)
            {
                if (f.tag == factionTag)
                {
                    if (f.strength >= minFactionStrength && f.strength <= maxFactionStrength)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }


        public bool CheckGlobalInt(string variableName, string compare, int value)
        {
            if (mod.debugMode) //SD_20131102
            {
                gv.cc.addLogText("<font color='yellow'>" + "checkGlobal: " + variableName + " " + compare + " " + value + "</font>" +
                        "<BR>");
            }

            foreach (GlobalInt variable in mod.moduleGlobalInts)
            {
                if (variable.Key.Equals(variableName))
                {
                    if (compare.Equals("="))
                    {
                        if (variable.Value == value)
                        {
                            if (mod.debugMode) //SD_20131102
                            {
                                gv.cc.addLogText("<font color='yellow'>" + "foundGlobal: " + variable.Key + " == " + variable.Value + "</font>" +
                                        "<BR>");
                            }
                            return true;
                        }
                    }
                    else if (compare.Equals(">"))
                    {
                        if (variable.Value > value)
                        {
                            if (mod.debugMode) //SD_20131102
                            {
                                gv.cc.addLogText("<font color='yellow'>" + "foundGlobal: " + variable.Key + " > " + variable.Value + "</font>" +
                                        "<BR>");
                            }
                            return true;
                        }
                    }
                    else if (compare.Equals("<"))
                    {
                        if (variable.Value < value)
                        {
                            if (mod.debugMode) //SD_20131102
                            {
                                gv.cc.addLogText("<font color='yellow'>" + "foundGlobal: " + variable.Key + " < " + variable.Value + "</font>" +
                                        "<BR>");
                            }
                            return true;
                        }
                    }
                    else if (compare.Equals("!"))
                    {
                        if (variable.Value != value)
                        {
                            if (mod.debugMode) //SD_20131102
                            {
                                gv.cc.addLogText("<font color='yellow'>" + "foundGlobal: " + variable.Key + " != " + variable.Value + "</font>" +
                                        "<BR>");
                            }
                            return true;
                        }
                    }
                }
            }
            if (mod.debugMode) //SD_20131102
            {
                gv.cc.addLogText("<font color='yellow'>" + "returning false" + "</font>" +
                        "<BR>");
            }
            return false;
        }
        public bool CheckLocalInt(string objectTag, string variableName, string compare, int value)
        {
            if (mod.debugMode) //SD_20131102
            {
                gv.cc.addLogText("<font color='yellow'>" + "checkLocal: " + objectTag + " " + variableName + " " + compare + " " + value + "</font>" +
                        "<BR>");
            }
            //check creatures, PCs, Props, areas, items
            foreach (Area a in mod.moduleAreasObjects)
            {
                if (a.Filename.Equals(objectTag))
                {
                    foreach (LocalInt variable in a.AreaLocalInts)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            if (compare.Equals("="))
                            {
                                if (variable.Value == value)
                                {
                                    if (mod.debugMode) //SD_20131102
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " == " + variable.Value + "</font>" +
                                                "<BR>");
                                    }
                                    return true;
                                }
                            }
                            else if (compare.Equals(">"))
                            {
                                if (variable.Value > value)
                                {
                                    if (mod.debugMode) //SD_20131102
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " > " + variable.Value + "</font>" +
                                                "<BR>");
                                    }
                                    return true;
                                }
                            }
                            else if (compare.Equals("<"))
                            {
                                if (variable.Value < value)
                                {
                                    if (mod.debugMode) //SD_20131102
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " < " + variable.Value + "</font>" +
                                                "<BR>");
                                    }
                                    return true;
                                }
                            }
                            else if (compare.Equals("!"))
                            {
                                if (variable.Value != value)
                                {
                                    if (mod.debugMode) //SD_20131102
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " != " + variable.Value + "</font>" +
                                                "<BR>");
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }
                else
                {
                    foreach (Prop p in a.Props)
                    {
                        if (p.PropTag.Equals(objectTag))
                        {
                            foreach (LocalInt variable in p.PropLocalInts)
                            {
                                if (variable.Key.Equals(variableName))
                                {
                                    if (compare.Equals("="))
                                    {
                                        if (variable.Value == value)
                                        {
                                            if (mod.debugMode) //SD_20131102
                                            {
                                                gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " == " + variable.Value + "</font>" +
                                                        "<BR>");
                                            }
                                            return true;
                                        }
                                    }
                                    else if (compare.Equals(">"))
                                    {
                                        if (variable.Value > value)
                                        {
                                            if (mod.debugMode) //SD_20131102
                                            {
                                                gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " > " + variable.Value + "</font>" +
                                                        "<BR>");
                                            }
                                            return true;
                                        }
                                    }
                                    else if (compare.Equals("<"))
                                    {
                                        if (variable.Value < value)
                                        {
                                            if (mod.debugMode) //SD_20131102
                                            {
                                                gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " < " + variable.Value + "</font>" +
                                                        "<BR>");
                                            }
                                            return true;
                                        }
                                    }
                                    else if (compare.Equals("!"))
                                    {
                                        if (variable.Value != value)
                                        {
                                            if (mod.debugMode) //SD_20131102
                                            {
                                                gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " != " + variable.Value + "</font>" +
                                                        "<BR>");
                                            }
                                            return true;
                                        }
                                    }
                                }
                            }
                            return false;
                        }
                    }
                }
            }
            foreach (Creature cr in mod.currentEncounter.encounterCreatureList)
            {
                if (cr.cr_tag.Equals(objectTag))
                {
                    foreach (LocalInt variable in cr.CreatureLocalInts)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            if (compare.Equals("="))
                            {
                                if (variable.Value == value)
                                {
                                    if (mod.debugMode) //SD_20131102
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " == " + variable.Value + "</font>" +
                                                "<BR>");
                                    }
                                    return true;
                                }
                            }
                            else if (compare.Equals(">"))
                            {
                                if (variable.Value > value)
                                {
                                    if (mod.debugMode) //SD_20131102
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " > " + variable.Value + "</font>" +
                                                "<BR>");
                                    }
                                    return true;
                                }
                            }
                            else if (compare.Equals("<"))
                            {
                                if (variable.Value < value)
                                {
                                    if (mod.debugMode) //SD_20131102
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " < " + variable.Value + "</font>" +
                                                "<BR>");
                                    }
                                    return true;
                                }
                            }
                            else if (compare.Equals("!"))
                            {
                                if (variable.Value != value)
                                {
                                    if (mod.debugMode) //SD_20131102
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + "foundLocal: " + variable.Key + " != " + variable.Value + "</font>" +
                                                "<BR>");
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }
            }
            if (mod.debugMode)
            {
                gv.cc.addLogText("<font color='yellow'>couldn't find the object with the tag (tag: " + objectTag + ") specified (only Creatures or Areas)</font><BR>");
            }
            return false;
        }

        public void ModifyFactionStrength(string factionTag, string transformType, string amount)
        {
            int amountNumber = Convert.ToInt32(amount);
            foreach (Faction f in gv.mod.moduleFactionsList)
            {
                if (f.tag == factionTag)
                {
                    if (transformType.Equals("+"))
                    {
                        f.strength += amountNumber;
                    }
                    else if (transformType.Equals("-"))
                    {
                        f.strength -= amountNumber;
                    }
                    else if (transformType.Equals("/"))
                    {
                        f.strength /= amountNumber;
                    }
                    else if (transformType.Equals("%"))
                    {
                        f.strength %= amountNumber;
                    }
                    else if (transformType.Equals("*"))
                    {
                        f.strength *= amountNumber;
                    }
                    else if (transformType.Equals("="))
                    {
                        f.strength = amountNumber;
                    }
                }
            }
        }

        public void ModifyFactionGrowthRate(string factionTag, string transformType, string amount)
        {
            int amountNumber = Convert.ToInt32(amount);
            foreach (Faction f in gv.mod.moduleFactionsList)
            {
                if (f.tag == factionTag)
                {
                    if (transformType.Equals("+"))
                    {
                        f.amountOfFactionStrengthChangePerInterval += amountNumber;
                    }
                    else if (transformType.Equals("-"))
                    {
                        f.amountOfFactionStrengthChangePerInterval -= amountNumber;
                    }
                    else if (transformType.Equals("/"))
                    {
                        f.amountOfFactionStrengthChangePerInterval /= amountNumber;
                    }
                    else if (transformType.Equals("%"))
                    {
                        f.amountOfFactionStrengthChangePerInterval %= amountNumber;
                    }
                    else if (transformType.Equals("*"))
                    {
                        f.amountOfFactionStrengthChangePerInterval *= amountNumber;
                    }
                    else if (transformType.Equals("="))
                    {
                        f.amountOfFactionStrengthChangePerInterval = amountNumber;
                    }
                }
            }
        }


        public void TransformGlobalInt(string firstInt, string transformType, string secondInt, string variableName)
        {
            string val = "";
            int value = 0;
            int fInt = Convert.ToInt32(firstInt);
            int sInt = Convert.ToInt32(secondInt);

            if (transformType.Equals("+"))
            {
                value = fInt + sInt;
            }
            else if (transformType.Equals("-"))
            {
                value = fInt - sInt;
            }
            else if (transformType.Equals("/"))
            {
                value = fInt / sInt;
            }
            else if (transformType.Equals("%"))
            {
                value = fInt % sInt;
            }
            else if (transformType.Equals("*"))
            {
                value = fInt * sInt;
            }
            else
            {
                value = 0;
            }

            val = value + "";
            SetGlobalInt(variableName, val);
        }

        //GLOBAL AND LOCAL STRINGS
        public void SetGlobalString(string variableName, string val)
        {
            int exists = 0;
            foreach (GlobalString variable in mod.moduleGlobalStrings)
            {
                if (variable.Key.Equals(variableName))
                {
                    variable.Value = val;
                    exists = 1;
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "setGlobal: " + variableName + " = " + val + "</font>" +
                                "<BR>");
                    }
                }
            }
            if (exists == 0)
            {
                GlobalString newGlobal = new GlobalString();
                newGlobal.Key = variableName;
                newGlobal.Value = val;
                mod.moduleGlobalStrings.Add(newGlobal);
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "setGlobal: " + variableName + " = " + val + "</font>" +
                            "<BR>");
                }
            }
        }
        public void SetLocalString(string objectTag, string variableName, string value)
        {
            //check creatures and areas
            foreach (Area a in mod.moduleAreasObjects)
            {
                if (a.Filename.Equals(objectTag))
                {
                    int exists = 0;
                    foreach (LocalString variable in a.AreaLocalStrings)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            variable.Value = value;
                            exists = 1;
                        }
                    }
                    if (exists == 0)
                    {
                        LocalString newLocalInt = new LocalString();
                        newLocalInt.Key = variableName;
                        newLocalInt.Value = value;
                        a.AreaLocalStrings.Add(newLocalInt);
                    }
                    return;
                }
                else
                {
                    foreach (Prop p in a.Props)
                    {
                        if (p.PropTag.Equals(objectTag))
                        {
                            int exists = 0;
                            foreach (LocalString variable in p.PropLocalStrings)
                            {
                                if (variable.Key.Equals(variableName))
                                {
                                    variable.Value = value;
                                    exists = 1;
                                }
                            }
                            if (exists == 0)
                            {
                                LocalString newLocalInt = new LocalString();
                                newLocalInt.Key = variableName;
                                newLocalInt.Value = value;
                                p.PropLocalStrings.Add(newLocalInt);
                            }
                            return;
                        }
                    }
                }
            }
            foreach (Creature cr in mod.currentEncounter.encounterCreatureList)
            {
                if (cr.cr_tag.Equals(objectTag))
                {
                    int exists = 0;
                    foreach (LocalString variable in cr.CreatureLocalStrings)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            variable.Value = value;
                            exists = 1;
                        }
                    }
                    if (exists == 0)
                    {
                        LocalString newLocalInt = new LocalString();
                        newLocalInt.Key = variableName;
                        newLocalInt.Value = value;
                        cr.CreatureLocalStrings.Add(newLocalInt);
                    }
                    return;
                }
            }
            if (mod.debugMode)
            {
                gv.cc.addLogText("<font color='yellow'>" + objectTag + " setLocal: " + variableName + " = " + value + "</font><BR>");
            }
        }
        public string GetGlobalString(string variableName)
        {
            foreach (GlobalString variable in mod.moduleGlobalStrings)
            {
                if (variable.Key.Equals(variableName))
                {
                    return variable.Value;
                }
            }
            if (mod.debugMode) //SD_20131102
            {
                gv.cc.addLogText("<font color='yellow'>" + "Couldn't find the tag specified...returning a value of \"none\"" + "</font>" +
                        "<BR>");
            }
            return "none";
        }
        public string GetLocalString(string objectTag, string variableName)
        {
            //check creatures, areas        
            foreach (Area a in mod.moduleAreasObjects)
            {
                if (a.Filename.Equals(objectTag))
                {
                    foreach (LocalString variable in a.AreaLocalStrings)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            return variable.Value;
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>Found the object, but couldn't find the tag specified...returning a value of \"none\"</font><BR>");
                    }
                    return "none";
                }
                else
                {
                    foreach (Prop p in a.Props)
                    {
                        if (p.PropTag.Equals(objectTag))
                        {
                            foreach (LocalString variable in p.PropLocalStrings)
                            {
                                if (variable.Key.Equals(variableName))
                                {
                                    return variable.Value;
                                }
                            }
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>Found the object, but couldn't find the tag specified...returning a value of \"none\"</font><BR>");
                            }
                            return "none";
                        }
                    }
                }
            }
            foreach (Creature cr in mod.currentEncounter.encounterCreatureList)
            {
                if (cr.cr_tag.Equals(objectTag))
                {
                    foreach (LocalString variable in cr.CreatureLocalStrings)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            return variable.Value;
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>Found the object, but couldn't find the tag specified...returning a value of \"none\"</font><BR>");
                    }
                    return "none";
                }
            }
            if (mod.debugMode)
            {
                gv.cc.addLogText("<font color='yellow'>couldn't find the object with the tag specified (only Creatures and Areas), returning a value of \"none\"</font><BR>");
            }
            return "none";
        }
        public bool CheckGlobalString(string variableName, string value)
        {
            if (mod.debugMode)
            {
                gv.cc.addLogText("<font color='yellow'>" + "checkGlobal: " + variableName + " == " + value + "</font>" +
                        "<BR>");
            }
            foreach (GlobalString variable in mod.moduleGlobalStrings)
            {
                if (variable.Key.Equals(variableName))
                {
                    if (variable.Value.Equals(value))
                    {
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>foundGlobal: " + variable.Key + " == " + variable.Value + "</font><BR>");
                        }
                        return true;
                    }
                }
            }
            if (mod.debugMode)
            {
                gv.cc.addLogText("<font color='yellow'>returning false</font><BR>");
            }
            return false;
        }
        public bool CheckLocalString(string objectTag, string variableName, string value)
        {
            if (mod.debugMode) //SD_20131102
            {
                gv.cc.addLogText("<font color='yellow'>" + "checkLocal: " + objectTag + " " + variableName + " == " + value + "</font><BR>");
            }
            //check creatures, areas
            foreach (Area a in mod.moduleAreasObjects)
            {
                if (a.Filename.Equals(objectTag))
                {
                    foreach (LocalString variable in a.AreaLocalStrings)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            if (variable.Value.Equals(value))
                            {
                                if (mod.debugMode) //SD_20131102
                                {
                                    gv.cc.addLogText("<font color='yellow'>foundLocal: " + variable.Key + " == " + variable.Value + "</font><BR>");
                                }
                                return true;
                            }
                        }
                    }
                    return false;
                }
                else
                {
                    foreach (Prop p in a.Props)
                    {
                        if (p.PropTag.Equals(objectTag))
                        {
                            foreach (LocalString variable in p.PropLocalStrings)
                            {
                                if (variable.Key.Equals(variableName))
                                {
                                    if (variable.Value.Equals(value))
                                    {
                                        if (mod.debugMode) //SD_20131102
                                        {
                                            gv.cc.addLogText("<font color='yellow'>foundLocal: " + variable.Key + " == " + variable.Value + "</font><BR>");
                                        }
                                        return true;
                                    }
                                }
                            }
                            return false;
                        }
                    }
                }
            }
            foreach (Creature cr in mod.currentEncounter.encounterCreatureList)
            {
                if (cr.cr_tag.Equals(objectTag))
                {
                    foreach (LocalString variable in cr.CreatureLocalStrings)
                    {
                        if (variable.Key.Equals(variableName))
                        {
                            if (variable.Value.Equals(value))
                            {
                                if (mod.debugMode) //SD_20131102
                                {
                                    gv.cc.addLogText("<font color='yellow'>foundLocal: " + variable.Key + " == " + variable.Value + "</font><BR>");
                                }
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
            if (mod.debugMode)
            {
                gv.cc.addLogText("<font color='yellow'>couldn't find the object with the tag (tag: " + objectTag + ") specified (only Creatures or Areas)</font><BR>");
            }
            return false;
        }
        public void TransformGlobalString(string firstString, string secondString, string variableName)
        {
            firstString = firstString.Replace("\"", "");
            secondString = secondString.Replace("\"", "");
            string val = firstString + secondString;
            SetGlobalString(variableName, val);
        }

        public void GiveFunds(int amount)
        {
            mod.partyGold += amount;
            gv.cc.addLogText("<font color='yellow'>" + "The party receives " + amount + " Gold" + "</font>" + "<BR>");
        }
        public void TakeFunds(int amount)
        {
            if (mod.partyGold < amount)
            {
                amount = mod.partyGold;
                mod.partyGold = 0;
            }
            else
            {
                mod.partyGold -= amount;
            }
            gv.cc.addLogText("<font color='yellow'>" + "The party loses " + amount + " Gold" + "</font>" + "<BR>");
        }

        /*
        else if (filename.Equals("gaRechargeSingleItem.cs"))
                    {
                        RechargeSingleItem(prm1);
                    }
                    else if (filename.Equals("gaRechargeAllItemsOfAType.cs"))
                    {
                        RechargeAllItemsOfAType(prm1);
                    }
                    else if (filename.Equals("gaRechargeAllItems.cs"))
                    {
                        RechargeAllItems();
                    }
        */
        public void MovePartyToLastLocation(string message)
        {
            gv.mod.PlayerLocationX = gv.mod.PlayerLastLocationX;
            gv.mod.PlayerLocationY = gv.mod.PlayerLastLocationY;
            if (message != null)
            {
                if (message != "")
                {
                    gv.cc.addLogText("<font color='yellow'>" + message + "</font><BR>");
                }
            }
        }

        public void RechargeSingleItem(string resref)
        {
            //callister
            Item masterItem = mod.getItemByResRef(resref);
            bool itemFound = false;

            foreach (ItemRefs ir in gv.mod.partyInventoryRefsList)
            {
                if (ir.resref == resref)
                {
                    if (ir.quantity < masterItem.quantity)
                    {
                        ir.quantity = masterItem.quantity;
                        itemFound = true;
                        break;
                    }
                }
            }

            if (!itemFound)
            {
                foreach (Player pc in gv.mod.playerList)
                {
                    if (pc.BodyRefs.resref.Equals(resref) && (pc.BodyRefs.quantity < masterItem.quantity)) 
                    {
                        pc.BodyRefs.quantity = masterItem.quantity;
                        break;
                    }
                   else if (pc.OffHandRefs.resref.Equals(resref) && (pc.OffHandRefs.quantity < masterItem.quantity))
                    {
                        pc.OffHandRefs.quantity = masterItem.quantity;
                        break;
                    }
                    else if (pc.MainHandRefs.resref.Equals(resref) && (pc.MainHandRefs.quantity < masterItem.quantity))
                    {
                        pc.MainHandRefs.quantity = masterItem.quantity;
                        break;
                    }
                    else if (pc.RingRefs.resref.Equals(resref) && (pc.RingRefs.quantity < masterItem.quantity))
                    {
                        pc.RingRefs.quantity = masterItem.quantity;
                        break;
                    }
                    else if (pc.Ring2Refs.resref.Equals(resref) && (pc.Ring2Refs.quantity < masterItem.quantity))
                    {
                        pc.Ring2Refs.quantity = masterItem.quantity;
                        break;
                    }
                   else if (pc.HeadRefs.resref.Equals(resref) && (pc.HeadRefs.quantity < masterItem.quantity))
                    {
                        pc.HeadRefs.quantity = masterItem.quantity;
                        break;
                    }
                    else if (pc.GlovesRefs.resref.Equals(resref) && (pc.GlovesRefs.quantity < masterItem.quantity))
                    {
                        pc.GlovesRefs.quantity = masterItem.quantity;
                        break;
                    }
                    else if (pc.NeckRefs.resref.Equals(resref) && (pc.NeckRefs.quantity < masterItem.quantity))
                    {
                        pc.NeckRefs.quantity = masterItem.quantity;
                        break;
                    }
                    else if (pc.FeetRefs.resref.Equals(resref) && (pc.FeetRefs.quantity < masterItem.quantity))
                    {
                        pc.FeetRefs.quantity = masterItem.quantity;
                        break;
                    }
                }
            }

            gv.cc.addLogText("<font color='yellow'>" + "A(n) " + masterItem.name + " has been fully recharged." + "</font><BR>");
        }

        public void RechargeAllItemsOfAType(string resref)
        {
            Item masterItem = mod.getItemByResRef(resref);

            foreach (ItemRefs ir in gv.mod.partyInventoryRefsList)
            {
                if (ir.resref == resref)
                {
                    ir.quantity = masterItem.quantity;
                }
            }

                foreach (Player pc in gv.mod.playerList)
                {
                    if (pc.BodyRefs.resref.Equals(resref))
                    {
                        pc.BodyRefs.quantity = masterItem.quantity;
                    }
                    else if (pc.OffHandRefs.resref.Equals(resref))
                    {
                        pc.OffHandRefs.quantity = masterItem.quantity;
                    }
                    else if (pc.MainHandRefs.resref.Equals(resref))
                    {
                        pc.MainHandRefs.quantity = masterItem.quantity;
                    }
                    else if (pc.RingRefs.resref.Equals(resref))
                    {
                        pc.RingRefs.quantity = masterItem.quantity;
                    }
                    else if (pc.Ring2Refs.resref.Equals(resref))
                    {
                        pc.Ring2Refs.quantity = masterItem.quantity;
                    }
                    else if (pc.HeadRefs.resref.Equals(resref))
                    {
                        pc.HeadRefs.quantity = masterItem.quantity;
                    }
                    else if (pc.GlovesRefs.resref.Equals(resref))
                    {
                        pc.GlovesRefs.quantity = masterItem.quantity;
                    }
                    else if (pc.NeckRefs.resref.Equals(resref))
                    {
                        pc.NeckRefs.quantity = masterItem.quantity;
                    }
                    else if (pc.FeetRefs.resref.Equals(resref))
                    {
                        pc.FeetRefs.quantity = masterItem.quantity;
                    }
                }
            
            gv.cc.addLogText("<font color='yellow'>" + "All items of type " + masterItem.name + " have been fully recharged." + "</font><BR>");
        }


        public void RechargeAllItems()
        {
            foreach (ItemRefs ir in gv.mod.partyInventoryRefsList)
            {
                Item item = mod.getItemByResRef(ir.resref);

                //identify charged item
                if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none" ))
                {
                    ir.quantity = item.quantity;
                }
            }

            foreach (Player pc in gv.mod.playerList)
            {
                if (pc.BodyRefs.resref != "none")
                {
                    Item item = mod.getItemByResRef(pc.BodyRefs.resref);
                    //identify charged item
                    if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none"))
                    {
                        pc.BodyRefs.quantity = item.quantity;
                    }
                }
                else if (pc.OffHandRefs.resref != "none")
                {
                    Item item = mod.getItemByResRef(pc.OffHandRefs.resref);
                    //identify charged item
                    if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none"))
                    {
                        pc.OffHandRefs.quantity = item.quantity;
                    }
                }
                else if (pc.MainHandRefs.resref != "none")
                {
                    Item item = mod.getItemByResRef(pc.MainHandRefs.resref);
                    //identify charged item
                    if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none"))
                    {
                        pc.MainHandRefs.quantity = item.quantity;
                    }
                }
                else if (pc.RingRefs.resref != "none")
                {
                    Item item = mod.getItemByResRef(pc.RingRefs.resref);
                    //identify charged item
                    if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none"))
                    {
                        pc.RingRefs.quantity = item.quantity;
                    }
                }
                else if (pc.Ring2Refs.resref != "none")
                {
                    Item item = mod.getItemByResRef(pc.Ring2Refs.resref);
                    //identify charged item
                    if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none"))
                    {
                        pc.Ring2Refs.quantity = item.quantity;
                    }
                }
                else if (pc.HeadRefs.resref != "none")
                {
                    Item item = mod.getItemByResRef(pc.HeadRefs.resref);
                    //identify charged item
                    if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none"))
                    {
                        pc.HeadRefs.quantity = item.quantity;
                    }
                }
                else if (pc.GlovesRefs.resref != "none")
                {
                    Item item = mod.getItemByResRef(pc.GlovesRefs.resref);
                    //identify charged item
                    if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none"))
                    {
                        pc.GlovesRefs.quantity = item.quantity;
                    }
                }
                else if (pc.NeckRefs.resref != "none")
                {
                    Item item = mod.getItemByResRef(pc.NeckRefs.resref);
                    //identify charged item
                    if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none"))
                    {
                        pc.NeckRefs.quantity = item.quantity;
                    }
                }
                else if (pc.FeetRefs.resref != "none")
                {
                    Item item = mod.getItemByResRef(pc.FeetRefs.resref);
                    //identify charged item
                    if ((item.quantity > 1) && (item.onUseItemCastSpellTag != "none" || item.onUseItemIBScript != "none" || item.onUseItem != "none"))
                    {
                        pc.FeetRefs.quantity = item.quantity;
                    }
                }
            }
            gv.cc.addLogText("<font color='yellow'>" + "All rechargeable items have been fully recharged." + "</font><BR>");
        }

        public void GiveItem(string resref, int quantity)
        {
            Item newItem = mod.getItemByResRef(resref);
            for (int i = 0; i < quantity; i++)
            {
                ItemRefs ir = mod.createItemRefsFromItem(newItem);
                mod.partyInventoryRefsList.Add(ir);
            }
            gv.cc.addLogText("<font color='yellow'>" + "The party gains " + quantity + " " + newItem.name + "(s)" + "</font><BR>");
        }

        public void RemoveItemFromInventory(ItemRefs itRef, int quantity)
        {
            Item newItem = mod.getItemByResRef(itRef.resref);

            if (!newItem.isStackable)
            {
                gv.mod.partyInventoryRefsList.Remove(itRef);
            }
            else
            {
                //decrement item quantity
                itRef.quantity -= quantity;
                //if item quantity <= 0, remove item from inventory
                if (itRef.quantity < 1)
                {
                    gv.mod.partyInventoryRefsList.Remove(itRef);
                }
            }
        }

        public void GiveXP(int amount, string pcIdentifier)
        {
            
            if (pcIdentifier == "-1")
            {
                //musculus
                if (mod.playerList.Count > 0)
                {
                    int xpToGive = amount;
                    gv.mod.playerList[gv.mod.selectedPartyLeader].XP += xpToGive;
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[gv.mod.selectedPartyLeader].name + " has gained " + xpToGive + " XP" + "</font>" +
                            "<BR>");
                }
            }
            else if (pcIdentifier == "0")
            {
                //musculus
                if (mod.playerList.Count > 0)
                {
                    int xpToGive = amount;
                    gv.mod.playerList[0].XP += xpToGive;
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[0].name + " has gained " + xpToGive + " XP" + "</font>" +
                            "<BR>");
                }
            }
            else if (pcIdentifier == "1")
            {
                //musculus
                if (mod.playerList.Count > 1)
                {
                    int xpToGive = amount;
                    gv.mod.playerList[1].XP += xpToGive;
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[1].name + " has gained " + xpToGive + " XP" + "</font>" +
                            "<BR>");
                }
            }
            else if (pcIdentifier == "2")
            {
                //musculus
                if (mod.playerList.Count > 2)
                {
                    int xpToGive = amount;
                    gv.mod.playerList[2].XP += xpToGive;
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[2].name + " has gained " + xpToGive + " XP" + "</font>" +
                            "<BR>");
                }
            }
            else if (pcIdentifier == "3")
            {
                //musculus
                if (mod.playerList.Count > 3)
                {
                    int xpToGive = amount;
                    gv.mod.playerList[3].XP += xpToGive;
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[3].name + " has gained " + xpToGive + " XP" + "</font>" +
                            "<BR>");
                }
            }
            else if (pcIdentifier == "4")
            {
                //musculus
                if (mod.playerList.Count > 4)
                {
                    int xpToGive = amount;
                    gv.mod.playerList[4].XP += xpToGive;
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[4].name + " has gained " + xpToGive + " XP" + "</font>" +
                            "<BR>");
                }
            }
            else if (pcIdentifier == "5")
            {
                //musculus
                if (mod.playerList.Count > 5)
                {
                    int xpToGive = amount;
                    gv.mod.playerList[5].XP += xpToGive;
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[5].name + " has gained " + xpToGive + " XP" + "</font>" +
                            "<BR>");
                }
            }
            else
            {
                if (mod.playerList.Count > 0)
                {
                    int xpToGive = amount / mod.playerList.Count;
                    //give xp to each PC member...split the value given
                    foreach (Player givePcXp in mod.playerList)
                    {
                        givePcXp.XP += xpToGive;
                    }
                    gv.cc.addLogText("<font color='yellow'>" + "Each player has gained " + xpToGive + " XP" + "</font>" +
                            "<BR>");
                }
            }

                //musculus
                if (mod.playerList.Count > 0)
                {
                    foreach (Player p in gv.mod.playerList)
                    {
                        if (p.name == pcIdentifier)
                        {
                            int xpToGive = amount;
                            p.XP += xpToGive;
                            gv.cc.addLogText("<font color='yellow'>" + p.name + " has gained " + xpToGive + " XP" + "</font>" +
                                    "<BR>");
                        }
                    }
                }
        }
        public void TakeItem(string resref, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                bool FoundOne = false;
                int cnt = 0;
                foreach (ItemRefs itr in mod.partyInventoryRefsList)
                {
                    if (!FoundOne)
                    {
                        if (itr.resref.Equals(resref))
                        {
                            gv.sf.RemoveItemFromInventory(itr, 1);
                            FoundOne = true;
                            break;
                        }
                    }
                    cnt++;
                }
                cnt = 0;
                foreach (Player pc in mod.playerList)
                {
                    if (!FoundOne)
                    {
                        if (pc.BodyRefs.resref.Equals(resref))
                        {
                            mod.playerList[cnt].BodyRefs = new ItemRefs();
                            FoundOne = true;
                        }
                        if (pc.OffHandRefs.resref.Equals(resref))
                        {
                            mod.playerList[cnt].OffHandRefs = new ItemRefs();
                            FoundOne = true;
                        }
                        if (pc.MainHandRefs.resref.Equals(resref))
                        {
                            mod.playerList[cnt].MainHandRefs = new ItemRefs();
                            FoundOne = true;
                        }
                        if (pc.RingRefs.resref.Equals(resref))
                        {
                            mod.playerList[cnt].RingRefs = new ItemRefs();
                            FoundOne = true;
                        }
                        if (pc.Ring2Refs.resref.Equals(resref))
                        {
                            mod.playerList[cnt].Ring2Refs = new ItemRefs();
                            FoundOne = true;
                        }
                        if (pc.HeadRefs.resref.Equals(resref))
                        {
                            mod.playerList[cnt].HeadRefs = new ItemRefs();
                            FoundOne = true;
                        }
                        if (pc.GlovesRefs.resref.Equals(resref))
                        {
                            mod.playerList[cnt].GlovesRefs = new ItemRefs();
                            FoundOne = true;
                        }
                        if (pc.NeckRefs.resref.Equals(resref))
                        {
                            mod.playerList[cnt].NeckRefs = new ItemRefs();
                            FoundOne = true;
                        }
                        if (pc.FeetRefs.resref.Equals(resref))
                        {
                            mod.playerList[cnt].FeetRefs = new ItemRefs();
                            FoundOne = true;
                        }
                    }
                    cnt++;
                }
            }
        }
        public void AddCharacterToParty(string filename)
        {
            try
            {
                Player newPc = gv.cc.LoadPlayer(filename +".json"); //ex: filename = "ezzbel.json"
                newPc.token = gv.cc.LoadBitmap(newPc.tokenFilename);
                newPc.portrait = gv.cc.LoadBitmap(newPc.portraitFilename);
                newPc.playerClass = mod.getPlayerClass(newPc.classTag);
                newPc.race = mod.getRace(newPc.raceTag);
                //check to see if already in party before adding
                bool foundOne = false;
                foreach (Player pc in mod.playerList)
                {
                    if (newPc.tag.Equals(pc.tag))
                    {
                        foundOne = true;
                    }
                }
                if (!foundOne)
                {
                    mod.playerList.Add(newPc);
                    if (!newPc.AmmoRefs.resref.Equals("none"))
                    {
                        GiveItem(newPc.AmmoRefs.resref, 1);
                    }
                    //gv.TrackerSendEventOnePlayerInfo(newPc,"PartyAddCompanion:" + newPc.name);
                    gv.cc.addLogText("<font color='lime'>" + newPc.name + " joins the party</font><BR>");
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "This PC is already in the party" + "</font><BR>");
                    }
                }
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to load character from character folder" + "</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }

        //AddTemporaryAllyForThisEncounter(p1, p2, p3, p4);
        public void AddTemporaryAllyForThisEncounter(string filename, int locationX, int locationY, int stayDuration)
        {
            try
            {
                Player newPc = gv.cc.LoadPlayer(filename + ".json"); //ex: filename = "ezzbel.json"

                //new set of values for temporaries allies/summons
                newPc.isTemporaryAllyForThisEncounterOnly = true;
                newPc.stayDurationInTurns = stayDuration;
                newPc.combatLocX = locationX;
                newPc.combatLocY = locationY;

                newPc.token = gv.cc.LoadBitmap(newPc.tokenFilename);
                //newPc.portrait = gv.cc.LoadBitmap(newPc.portraitFilename);
                newPc.playerClass = mod.getPlayerClass(newPc.classTag);
                newPc.race = mod.getRace(newPc.raceTag);
                //check to see if already in party before adding
                //bool foundOne = false;
                foreach (Player pc in mod.playerList)
                {
                    if (newPc.tag.Equals(pc.tag))
                    {
                        newPc.tag += gv.mod.allyCounter;
                        gv.mod.allyCounter++;
                    }
                }
                //if (!foundOne)
                //{
                    mod.playerList.Add(newPc);
                    if (!newPc.AmmoRefs.resref.Equals("none"))
                    {
                        GiveItem(newPc.AmmoRefs.resref, 1);
                    }

                newPc.moveOrder = gv.screenCombat.moveOrderList.Count;
                //finally add creature  
                //mod.currentEncounter.encounterCreatureList.Add(copy);
                //add to end of move order  
                MoveOrder newMO = new MoveOrder();
                newMO.PcOrCreature = newPc;
                newMO.rank = 100;
                gv.screenCombat.moveOrderList.Add(newMO);
                //increment the number of initial move order objects
                //note: check how ini bar system will interact with creatures added while battle is running  
                gv.screenCombat.initialMoveOrderListSize++;
                //add to encounter xp  

                /*
                                MoveOrder newMO = new MoveOrder();
                                newMO.PcOrCreature = newPc;
                                newMO.rank = 1000 + gv.mod.allyCounter; 
                                gv.screenCombat.moveOrderList.Add(newMO);
                                //gv.TrackerSendEventOnePlayerInfo(newPc,"PartyAddCompanion:" + newPc.name);

                                gv.screenCombat.initialMoveOrderListSize = gv.screenCombat.moveOrderList.Count;
                                gv.screenCombat.moveOrderList = gv.screenCombat.moveOrderList.OrderByDescending(x => x.rank).ToList();

                                int cnt = 0;
                                foreach (MoveOrder m in gv.screenCombat.moveOrderList)
                                {
                                    if (m.PcOrCreature is Player)
                                    {
                                        Player pc = (Player)m.PcOrCreature;
                                        pc.moveOrder = cnt;
                                    }
                                    else
                                    {
                                        Creature crt = (Creature)m.PcOrCreature;
                                        crt.moveOrder = cnt;
                                    }
                                    cnt++;
                                }
                  */

                gv.cc.addLogText("<font color='lime'>" + newPc.name + " joins the party</font><BR>");

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                /*
                //draw form here
        public void calcualteMoveOrder()
        {
            moveOrderList.Clear();
            //creatureCounter2 = 0;
            //go through each PC and creature and make initiative roll
            foreach (Player pc in gv.mod.playerList)
            {

                int roll = gv.sf.RandInt(100) + (((pc.dexterity - 10) / 2) * 5);
                MoveOrder newMO = new MoveOrder();
                newMO.PcOrCreature = pc;
                newMO.rank = roll;
                moveOrderList.Add(newMO);


            }
            foreach (Creature crt in gv.mod.currentEncounter.encounterCreatureList)
            {


                int roll = gv.sf.RandInt(100) + (crt.initiativeBonus * 5);
                MoveOrder newMO = new MoveOrder();
                newMO.PcOrCreature = crt;
                newMO.rank = roll;
                moveOrderList.Add(newMO);

            }
            initialMoveOrderListSize = moveOrderList.Count;
            //sort PCs and creatures based on results
            moveOrderList = moveOrderList.OrderByDescending(x => x.rank).ToList();
            //assign moveOrder to PC and Creature property
            int cnt = 0;
            foreach (MoveOrder m in moveOrderList)
            {
                if (m.PcOrCreature is Player)
                {
                    Player pc = (Player)m.PcOrCreature;
                    pc.moveOrder = cnt;
                }
                else
                {
                    Creature crt = (Creature)m.PcOrCreature;
                    crt.moveOrder = cnt;
                }
                cnt++;
            }
        }
        */

        ///XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

        //}
        //else
        //{
        //if (mod.debugMode) //SD_20131102
        //{
        //gv.cc.addLogText("<font color='yellow'>" + "This PC is already in the party" + "</font><BR>");
        //}
        //}
    }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to load character from character folder" + "</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }





        public void RemoveCharacterFromParty(string PCtag, string index)
        {
            try
            {
                Player pc = gv.mod.playerList[0];
                if ((PCtag != null) && (!PCtag.Equals("")))
                {
                    pc = gv.mod.getPlayerByName(PCtag);
                    if (pc == null)
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            gv.cc.addLogText("<font color='yellow'>Could not find PC: " + PCtag + ", aborting</font><BR>");
                        }
                        return;
                    }
                }
                else if ((index != null) && (!index.Equals("")) && (!index.Equals("none")))
                {
                    int parm2 = Convert.ToInt32(index);
                    if ((parm2 >= 0) && (parm2 < gv.mod.playerList.Count))
                    {
                        pc = gv.mod.playerList[parm2];
                    }
                    else
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                        }
                        return;
                    }
                }
                mod.playerList.Remove(pc);
                mod.selectedPartyLeader = 0;
                gv.screenMainMap.updateTraitsPanel();
                gv.cc.partyScreenPcIndex = 0;
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to remove character from party" + "</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }
        public void MoveCharacterToRoster(string PCtag, string index)
        {
            try
            {
                Player pc = gv.mod.playerList[0];
                if ((PCtag != null) && (!PCtag.Equals("")))
                {
                    pc = gv.mod.getPlayerByName(PCtag);
                    if (pc == null)
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            gv.cc.addLogText("<font color='yellow'>Could not find PC: " + PCtag + ", aborting</font><BR>");
                        }
                        return;
                    }
                }
                else if ((index != null) && (!index.Equals("")))
                {
                    int parm2 = Convert.ToInt32(index);
                    if ((parm2 >= 0) && (parm2 < gv.mod.playerList.Count))
                    {
                        pc = gv.mod.playerList[parm2];
                    }
                    else
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                        }
                        return;
                    }
                }
                //remove selected from partyList and add to pcList
                if (mod.playerList.Count > 0)
                {
                    Player copyPC = pc.DeepCopy();
                    copyPC.token = gv.cc.LoadBitmap(copyPC.tokenFilename);
                    copyPC.playerClass = mod.getPlayerClass(copyPC.classTag);
                    copyPC.race = mod.getRace(copyPC.raceTag);
                    mod.partyRosterList.Add(copyPC);
                    mod.playerList.Remove(pc);
                }
                mod.selectedPartyLeader = 0;
                gv.screenMainMap.updateTraitsPanel();
                gv.cc.partyScreenPcIndex = 0;
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to remove character from party" + "</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }
        public void MoveCharacterToPartyFromRoster(string PCtag, string index)
        {
            try
            {
                if (gv.mod.partyRosterList.Count < 1)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Party Roster is empty, aborting</font><BR>");
                    }
                    return;
                }

                Player pc = null;
                if ((PCtag != null) && (!PCtag.Equals("")))
                {
                    foreach (Player plr in gv.mod.partyRosterList)
                    {
                        if (plr.name.Equals(PCtag))
                        {
                            pc = plr;
                        }
                    }
                    if (pc == null)
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            gv.cc.addLogText("<font color='yellow'>Could not find PC: " + PCtag + ", aborting</font><BR>");
                        }
                        return;
                    }
                }
                else if ((index != null) && (!index.Equals("")))
                {
                    int parm2 = Convert.ToInt32(index);
                    if ((parm2 >= 0) && (parm2 < gv.mod.partyRosterList.Count))
                    {
                        pc = gv.mod.partyRosterList[parm2];
                    }
                    else
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                        }
                        return;
                    }
                }
                if ((mod.partyRosterList.Count > 0) && (mod.playerList.Count < mod.MaxPartySize))
                {
                    Player copyPC = pc.DeepCopy();
                    copyPC.token = gv.cc.LoadBitmap(copyPC.tokenFilename);
                    copyPC.playerClass = mod.getPlayerClass(copyPC.classTag);
                    copyPC.race = mod.getRace(copyPC.raceTag);
                    mod.playerList.Add(copyPC);
                    mod.partyRosterList.Remove(pc);
                }
                mod.selectedPartyLeader = 0;
                gv.screenMainMap.updateTraitsPanel();
                gv.cc.partyScreenPcIndex = 0;
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to remove character from party" + "</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }
        public void EnableDisableTriggerEvent(string tag, string evNum, string enabl)
        {
            int eventNumber = Convert.ToInt32(evNum);
            bool enable = Boolean.Parse(enabl);

            try
            {
                foreach (Area ar in mod.moduleAreasObjects)
                {
                    Trigger trig = ar.getTriggerByTag(tag);
                    if (trig != null)
                    {
                        if (eventNumber == 1)
                        {
                            trig.EnabledEvent1 = enable;
                        }
                        else if (eventNumber == 2)
                        {
                            trig.EnabledEvent2 = enable;
                        }
                        else if (eventNumber == 3)
                        {
                            trig.EnabledEvent3 = enable;
                        }
                        return;
                    }
                }
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to find trigger in this area" + "</font><BR>");
                }
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to find trigger due to exception error" + "</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }
        public void EnableDisableTrigger(string tag, string enabl)
        {
            bool enable = Boolean.Parse(enabl);
            try
            {
                foreach (Area ar in mod.moduleAreasObjects)
                {
                    Trigger trig = ar.getTriggerByTag(tag);
                    if (trig != null)
                    {
                        trig.Enabled = enable;
                        return;
                    }
                }
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "can't find designated trigger tag in any area" + "</font><BR>");
                }
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to find trigger due to exception error" + "</font>" +
                            "<BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }
        public void EnableDisableTriggerAtCurrentLocation(string enabl)
        {
            bool enable = Boolean.Parse(enabl);
            try
            {
                Trigger trig = mod.currentArea.getTriggerByLocation(mod.PlayerLocationX, mod.PlayerLocationY);
                if (trig != null)
                {
                    trig.Enabled = enable;
                    return;
                }
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>can't find designated trigger at this location</font><BR>");
                }
            }
            catch (Exception ex)
            {
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>failed to find trigger due to exception error</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }
        public void EnableDisableTriggerEventAtCurrentLocation(string evNum, string enabl)
        {
            bool enable = Boolean.Parse(enabl);
            int eventNumber = Convert.ToInt32(evNum);

            try
            {
                Trigger trig = mod.currentArea.getTriggerByLocation(mod.PlayerLocationX, mod.PlayerLocationY);
                if (trig != null)
                {
                    if (eventNumber == 1)
                    {
                        trig.EnabledEvent1 = enable;
                    }
                    else if (eventNumber == 2)
                    {
                        trig.EnabledEvent2 = enable;
                    }
                    else if (eventNumber == 3)
                    {
                        trig.EnabledEvent3 = enable;
                    }
                    return;
                }
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>can't find designated trigger at this location</font><BR>");
                }
            }
            catch (Exception ex)
            {
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>failed to find trigger due to exception error</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }
        public void TogglePartyToken(string filename, string enabl)
        {
            bool enable = Boolean.Parse(enabl);
            try
            {
                if ((filename.Equals("none")) || (filename.Equals("")))
                {
                    //leave the token filename alone, use current filename
                }
                else
                {
                    gv.mod.partyTokenFilename = filename;
                }
                gv.cc.DisposeOfBitmap(ref gv.mod.partyTokenBitmap);
                gv.mod.partyTokenBitmap = gv.cc.LoadBitmap(gv.mod.partyTokenFilename);
                if (!mod.playerList[0].combatFacingLeft)
                {
//TODO                    mod.partyTokenBitmap = gv.cc.flip(mod.partyTokenBitmap);
                }
                gv.mod.showPartyToken = enable;
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to switch party token" + "</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }
        public void SetPropIsShown(string tag, string show)
        {
            bool shown = Boolean.Parse(show);
            Prop prp = gv.mod.currentArea.getPropByTag(tag);
            if (prp != null)
            {
                prp.isShown = shown;
            }
            else
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>didn't find prop in this area.</font><BR>");
                }
            }
        }
        public void SetPropIsMover(string tag, string mover)
        {
            bool ismover = Boolean.Parse(mover);
            Prop prp = gv.mod.currentArea.getPropByTag(tag);
            if (prp != null)
            {
                prp.isMover = ismover;
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>prop isMover toggled</font><BR>");
                }
            }
            else
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>didn't find prop in this area.</font><BR>");
                }
            }
        }
        public void RemovePropByTag(string tag)
        {
            try
            {
                foreach (Area ar in mod.moduleAreasObjects)
                {
                    Prop prp = ar.getPropByTag(tag);
                    if (prp != null)
                    {
                        ar.Props.Remove(prp);
                        if (mod.debugMode) //SD_20131102
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "removed prop: " + tag + "</font><BR>");
                        }
                        return;
                    }
                }
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "can't find prop " + tag + " in any area" + "</font><BR>");
                }
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to find prop due to exception error" + "</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }
        public void RemovePropByIndex(int index, string areafilename)
        {
            try
            {
                foreach (Area ar in mod.moduleAreasObjects)
                {
                    if (ar.Filename.Equals(areafilename))
                    {
                        if (index < ar.Props.Count)
                        {
                            if (mod.debugMode) //SD_20131102
                            {
                                gv.cc.addLogText("<font color='yellow'>" + "removed prop: " + ar.Props[index].PropTag + "</font><BR>");
                            }
                            ar.Props.RemoveAt(index);
                            return;
                        }
                    }
                }
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "can't find prop at index " + index + " in area " + areafilename + "</font><BR>");
                }
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to find prop due to exception error</font><BR>");
                }
                gv.errorLog(ex.ToString());
            }
        }
        public void GetPlayerHp(string tag, string index, string key)
        {
            Player pc = gv.mod.playerList[0];
            if ((tag != null) && (!tag.Equals("")))
            {
                pc = gv.mod.getPlayerByName(tag);
                if (pc == null)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Could not find PC: " + tag + ", aborting</font><BR>");
                    }
                    return;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int parm2 = Convert.ToInt32(index);
                if ((parm2 >= 0) && (parm2 < gv.mod.playerList.Count))
                {
                    pc = gv.mod.playerList[parm2];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                    }
                    return;
                }
            }
            this.SetGlobalInt(key, pc.hp + "");
        }
        public void GetPlayerSp(string tag, string index, string key)
        {
            Player pc = gv.mod.playerList[0];
            if ((tag != null) && (!tag.Equals("")))
            {
                pc = gv.mod.getPlayerByName(tag);
                if (pc == null)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Could not find PC: " + tag + ", aborting</font><BR>");
                    }
                    return;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int parm2 = Convert.ToInt32(index);
                if ((parm2 >= 0) && (parm2 < gv.mod.playerList.Count))
                {
                    pc = gv.mod.playerList[parm2];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                    }
                    return;
                }
            }
            this.SetGlobalInt(key, pc.sp + "");
        }
        public void SetPlayerHp(string tag, string index, string opertr, string value)
        {
            Player pc = gv.mod.playerList[0];
            if ((tag != null) && (!tag.Equals("")))
            {
                pc = gv.mod.getPlayerByName(tag);
                if (pc == null)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Could not find PC: " + tag + ", aborting</font><BR>");
                    }
                    return;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int parm2 = Convert.ToInt32(index);
                if (parm2 == -1)
                {
                    parm2 = gv.mod.selectedPartyLeader;
                }
                if ((parm2 >= 0) && (parm2 < gv.mod.playerList.Count))
                {
                    pc = gv.mod.playerList[parm2];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                    }
                    return;
                }
            }
            if (opertr.Equals("+"))
            {
                pc.hp += Convert.ToInt32(value);
            }
            else if (opertr.Equals("-"))
            {
                pc.hp -= Convert.ToInt32(value);
            }
            else if (opertr.Equals("/"))
            {
                pc.hp /= Convert.ToInt32(value);
            }
            else if (opertr.Equals("*"))
            {
                pc.hp *= Convert.ToInt32(value);
            }
            else
            {
                pc.hp = Convert.ToInt32(value);
            }
        }
        public void SetPlayerBaseAtt(string tag, string index, string opertr, string value, string att)
        {
            Player pc = gv.mod.playerList[0];
            if ((tag != null) && (!tag.Equals("")))
            {
                pc = gv.mod.getPlayerByName(tag);
                if (pc == null)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Could not find PC: " + tag + ", aborting</font><BR>");
                    }
                    return;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int parm2 = Convert.ToInt32(index);
                if (parm2 == -1)
                {
                    parm2 = gv.mod.selectedPartyLeader;
                }
                if ((parm2 >= 0) && (parm2 < gv.mod.playerList.Count))
                {
                    pc = gv.mod.playerList[parm2];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                    }
                    return;
                }
            }
            if (opertr.Equals("+"))
            {
                if (att.Equals("str"))
                {
                    pc.baseStr += Convert.ToInt32(value);
                }
                else if (att.Equals("dex"))
                {
                    pc.baseDex += Convert.ToInt32(value);
                }
                else if (att.Equals("int"))
                {
                    pc.baseInt += Convert.ToInt32(value);
                }
                else if (att.Equals("cha"))
                {
                    pc.baseCha += Convert.ToInt32(value);
                }
            }
            else if (opertr.Equals("-"))
            {
                if (att.Equals("str"))
                {
                    pc.baseStr -= Convert.ToInt32(value);
                }
                else if (att.Equals("dex"))
                {
                    pc.baseDex -= Convert.ToInt32(value);
                }
                else if (att.Equals("int"))
                {
                    pc.baseInt -= Convert.ToInt32(value);
                }
                else if (att.Equals("cha"))
                {
                    pc.baseCha -= Convert.ToInt32(value);
                }
            }
            else if (opertr.Equals("/"))
            {
                if (att.Equals("str"))
                {
                    pc.baseStr /= Convert.ToInt32(value);
                }
                else if (att.Equals("dex"))
                {
                    pc.baseDex /= Convert.ToInt32(value);
                }
                else if (att.Equals("int"))
                {
                    pc.baseInt /= Convert.ToInt32(value);
                }
                else if (att.Equals("cha"))
                {
                    pc.baseCha /= Convert.ToInt32(value);
                }
            }
            else if (opertr.Equals("*"))
            {
                if (att.Equals("str"))
                {
                    pc.baseStr *= Convert.ToInt32(value);
                }
                else if (att.Equals("dex"))
                {
                    pc.baseDex *= Convert.ToInt32(value);
                }
                else if (att.Equals("int"))
                {
                    pc.baseInt *= Convert.ToInt32(value);
                }
                else if (att.Equals("cha"))
                {
                    pc.baseCha *= Convert.ToInt32(value);
                }
            }
            else
            {
                if (att.Equals("str"))
                {
                    pc.baseStr = Convert.ToInt32(value);
                }
                else if (att.Equals("dex"))
                {
                    pc.baseDex = Convert.ToInt32(value);
                }
                else if (att.Equals("int"))
                {
                    pc.baseInt = Convert.ToInt32(value);
                }
                else if (att.Equals("cha"))
                {
                    pc.baseCha = Convert.ToInt32(value);
                }
            }
        }
        public void AddSpellToPlayer(string tag, string index, string SpellTag)
        {
            Player pc = gv.mod.playerList[0];
            if ((tag != null) && (!tag.Equals("")))
            {
                pc = gv.mod.getPlayerByName(tag);
                if (pc == null)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Could not find PC: " + tag + ", aborting</font><BR>");
                    }
                    return;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int parm2 = Convert.ToInt32(index);
                if (parm2 == -1)
                {
                    parm2 = gv.mod.selectedPartyLeader;
                }
                if ((parm2 >= 0) && (parm2 < gv.mod.playerList.Count))
                {
                    pc = gv.mod.playerList[parm2];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                    }
                    return;
                }
            }
            //get spell to add
            Spell sp = mod.getSpellByTag(SpellTag);
            if (sp != null)
            {
                pc.knownSpellsTags.Add(sp.tag);
                //Spell replacement code  
                if ((sp.spellToReplaceByTag != "none") && (sp.spellToReplaceByTag != ""))
                {
                    pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                }

                for (int i = pc.knownSpellsTags.Count - 1; i >= 0; i--)
                {
                    if (pc.knownSpellsTags[i] == sp.spellToReplaceByTag)
                    {
                        //pc.replacedTraitsOrSpellsByTag.Add(sp.spellToReplaceByTag);
                        pc.knownSpellsTags.RemoveAt(i);
                    }
                }
            }
            else if (mod.debugMode) //SD_20131102
            {
                gv.cc.addLogText("<font color='yellow'>Could not find Spell with tag: " + SpellTag + ", aborting</font><BR>");
            }
        }

        public void AddTraitToPlayer(string tag, string index, string TraitTag)
        {
            Player pc = gv.mod.playerList[0];
            if ((tag != null) && (!tag.Equals("")))
            {
                pc = gv.mod.getPlayerByName(tag);
                if (pc == null)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Could not find PC: " + tag + ", aborting</font><BR>");
                    }
                    return;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int parm2 = Convert.ToInt32(index);
                if (parm2 == -1)
                {
                    parm2 = gv.mod.selectedPartyLeader;
                }
                if ((parm2 >= 0) && (parm2 < gv.mod.playerList.Count))
                {
                    pc = gv.mod.playerList[parm2];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                    }
                    return;
                }
            }
            //get trait to add
            Trait tr = mod.getTraitByTag(TraitTag);
            if (tr != null)
            {
                pc.knownTraitsTags.Add(tr.tag);
                //public string useableInSituation = "Always"; //InCombat, OutOfCombat, Always, Passive

                //**************************************************************************
                #region replacement code traits
                //get the trait tor replace (if existent)
                Trait temp2 = new Trait();
                foreach (Trait t in gv.mod.moduleTraitsList)
                {
                    if (t.tag == tr.traitToReplaceByTag)
                    {
                        temp2 = t.DeepCopy();
                    }
                }
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


                //**************************************************************************

                if (!tr.associatedSpellTag.Equals("none"))
                {
                    if (tr.useableInSituation.Contains("Always"))
                    {
                        pc.knownUsableTraitsTags.Add(tr.associatedSpellTag);
                        pc.knownOutsideCombatUsableTraitsTags.Add(tr.associatedSpellTag);
                        pc.knownInCombatUsableTraitsTags.Add(tr.associatedSpellTag);
                    }
                    if (tr.useableInSituation.Contains("OutOfCombat"))
                    {
                        pc.knownUsableTraitsTags.Add(tr.associatedSpellTag);
                        pc.knownOutsideCombatUsableTraitsTags.Add(tr.associatedSpellTag);
                    }
                    if (tr.useableInSituation.Contains("InCombat"))
                    {
                        pc.knownUsableTraitsTags.Add(tr.associatedSpellTag);
                        pc.knownInCombatUsableTraitsTags.Add(tr.associatedSpellTag);
                    }
                }

                //effects
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

            }
            else if (mod.debugMode) //SD_20131102
            {
                gv.cc.addLogText("<font color='yellow'>Could not find Trait with tag: " + TraitTag + ", aborting</font><BR>");
            }
        }
        public void AddAllowedItemToPlayerClass(string tag, string resref)
        {
            PlayerClass pcl = mod.getPlayerClass(tag);
            if (pcl != null)
            {
                Item it = mod.getItemByResRef(resref);
                if (it != null)
                {
                    if (pcl.containsItemRefsWithResRef(resref))
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            gv.cc.addLogText("<font color='yellow'>Item is already allowed, aborting</font><BR>");
                        }
                        return;
                    }
                    else
                    {
                        ItemRefs ir = mod.createItemRefsFromItem(it);
                        pcl.itemsAllowed.Add(ir);
                    }
                }
            }
        }
        public void RemoveAllowedItemFromPlayerClass(string tag, string resref)
        {
            PlayerClass pcl = mod.getPlayerClass(tag);
            if (pcl != null)
            {
                Item it = mod.getItemByResRef(resref);
                if (it != null)
                {
                    foreach (ItemRefs itref in pcl.itemsAllowed)
                    {
                        if (itref.resref.Equals(resref))
                        {
                            pcl.itemsAllowed.Remove(itref);
                            return;
                        }
                    }
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Did't find Item to remove from known list, aborting</font><BR>");
                    }
                }
            }
        }
        public void SetPlayerSp(string tag, string index, string opertr, string value)
        {
            Player pc = gv.mod.playerList[0];
            if ((tag != null) && (!tag.Equals("")))
            {
                pc = gv.mod.getPlayerByName(tag);
                if (pc == null)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Could not find PC: " + tag + ", aborting</font><BR>");
                    }
                    return;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int parm2 = Convert.ToInt32(index);
                if (parm2 == -1)
                {
                    parm2 = gv.mod.selectedPartyLeader;
                }
                if ((parm2 >= 0) && (parm2 < gv.mod.playerList.Count))
                {
                    pc = gv.mod.playerList[parm2];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                    }
                    return;
                }
            }
            if (opertr.Equals("+"))
            {
                pc.sp += Convert.ToInt32(value);
            }
            else if (opertr.Equals("-"))
            {
                pc.sp -= Convert.ToInt32(value);
            }
            else if (opertr.Equals("/"))
            {
                pc.sp /= Convert.ToInt32(value);
            }
            else if (opertr.Equals("*"))
            {
                pc.sp *= Convert.ToInt32(value);
            }
            else
            {
                pc.sp = Convert.ToInt32(value);
            }
        }
        public void SetCreatureSp(string tag, string index, string opertr, string value)
        {
            Creature crt = GetCreature(tag, index);
            if (crt == null)
            {
                return;
            }
            if (opertr.Equals("+"))
            {
                crt.sp += Convert.ToInt32(value);
            }
            else if (opertr.Equals("-"))
            {
                crt.sp -= Convert.ToInt32(value);
            }
            else if (opertr.Equals("/"))
            {
                crt.sp /= Convert.ToInt32(value);
            }
            else if (opertr.Equals("*"))
            {
                crt.sp *= Convert.ToInt32(value);
            }
            else
            {
                crt.sp = Convert.ToInt32(value);
            }
        }
        public void SetCreatureHp(string tag, string index, string opertr, string value)
        {
            Creature crt = GetCreature(tag, index);
            if (crt == null)
            {
                return;
            }
            if (opertr.Equals("+"))
            {
                crt.hp += Convert.ToInt32(value);
            }
            else if (opertr.Equals("-"))
            {
                crt.hp -= Convert.ToInt32(value);
            }
            else if (opertr.Equals("/"))
            {
                crt.hp /= Convert.ToInt32(value);
            }
            else if (opertr.Equals("*"))
            {
                crt.hp *= Convert.ToInt32(value);
            }
            else
            {
                crt.hp = Convert.ToInt32(value);
            }
        }
        public void SetWorldTime(string opertr, string value, string multsix)
        {
            if (opertr.Equals("+"))
            {
                mod.WorldTime += Convert.ToInt32(value);
            }
            else if (opertr.Equals("-"))
            {
                mod.WorldTime -= Convert.ToInt32(value);
            }
            else
            {
                mod.WorldTime = Convert.ToInt32(value);
            }
            //round to nearest multiple of 6
            if (multsix.Equals("true"))
            {
                mod.WorldTime = (mod.WorldTime / 6) * 6;
            }
            if (mod.WorldTime < 0)
            {
                mod.WorldTime = 0;
            }
        }
        public void ApplyPartyDamage(int dam)
        {
            foreach (Player pc in mod.playerList)
            {
                gv.cc.addLogText("<font color='yellow'>" + pc.name + " takes " + dam + " damage" + "</font><BR>");
                pc.hp -= dam;
                if (pc.hp <= 0)
                {
                    gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                    pc.charStatus = "Dead";
                }
            }
        }

        public void ApplySinglePCDamage(int dam, string pcIdentifier)
        {
            //musculus
            //block for party leader, "-1"
            if (pcIdentifier == "-1")
            {         
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[gv.mod.selectedPartyLeader].name + " takes " + dam + " damage" + "</font><BR>");
                    gv.mod.playerList[gv.mod.selectedPartyLeader].hp -= dam;
                    if (gv.mod.playerList[gv.mod.selectedPartyLeader].hp <= 0)
                    {
                        gv.cc.addLogText("<font color='red'>" + gv.mod.playerList[gv.mod.selectedPartyLeader].name + " drops unconcious!" + "</font><BR>");
                        gv.mod.playerList[gv.mod.selectedPartyLeader].charStatus = "Dead";
                    }
            }
            else if (pcIdentifier == "0")
            {
                if (gv.mod.playerList.Count > 0)
                {
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[0].name + " takes " + dam + " damage" + "</font><BR>");
                    gv.mod.playerList[0].hp -= dam;
                    if (gv.mod.playerList[0].hp <= 0)
                    {
                        gv.cc.addLogText("<font color='red'>" + gv.mod.playerList[0].name + " drops unconcious!" + "</font><BR>");
                        gv.mod.playerList[0].charStatus = "Dead";
                    }
                }
            }
            else if (pcIdentifier == "1")
            {
                if (gv.mod.playerList.Count > 1)
                {
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[1].name + " takes " + dam + " damage" + "</font><BR>");
                    gv.mod.playerList[1].hp -= dam;
                    if (gv.mod.playerList[1].hp <= 0)
                    {
                        gv.cc.addLogText("<font color='red'>" + gv.mod.playerList[1].name + " drops unconcious!" + "</font><BR>");
                        gv.mod.playerList[1].charStatus = "Dead";
                    }
                }
            }
            else if (pcIdentifier == "2")
            {
                if (gv.mod.playerList.Count > 2)
                {
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[2].name + " takes " + dam + " damage" + "</font><BR>");
                    gv.mod.playerList[2].hp -= dam;
                    if (gv.mod.playerList[2].hp <= 0)
                    {
                        gv.cc.addLogText("<font color='red'>" + gv.mod.playerList[2].name + " drops unconcious!" + "</font><BR>");
                        gv.mod.playerList[2].charStatus = "Dead";
                    }
                }
            }
            else if (pcIdentifier == "3")
            {
                if (gv.mod.playerList.Count > 3)
                {
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[3].name + " takes " + dam + " damage" + "</font><BR>");
                    gv.mod.playerList[3].hp -= dam;
                    if (gv.mod.playerList[3].hp <= 0)
                    {
                        gv.cc.addLogText("<font color='red'>" + gv.mod.playerList[3].name + " drops unconcious!" + "</font><BR>");
                        gv.mod.playerList[3].charStatus = "Dead";
                    }
                }
            }
            else if (pcIdentifier == "4")
            {
                if (gv.mod.playerList.Count > 4)
                {
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[4].name + " takes " + dam + " damage" + "</font><BR>");
                    gv.mod.playerList[4].hp -= dam;
                    if (gv.mod.playerList[4].hp <= 0)
                    {
                        gv.cc.addLogText("<font color='red'>" + gv.mod.playerList[4].name + " drops unconcious!" + "</font><BR>");
                        gv.mod.playerList[4].charStatus = "Dead";
                    }
                }
            }
            else if (pcIdentifier == "5")
            {
                if (gv.mod.playerList.Count > 5)
                {
                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.playerList[5].name + " takes " + dam + " damage" + "</font><BR>");
                    gv.mod.playerList[5].hp -= dam;
                    if (gv.mod.playerList[5].hp <= 0)
                    {
                        gv.cc.addLogText("<font color='red'>" + gv.mod.playerList[5].name + " drops unconcious!" + "</font><BR>");
                        gv.mod.playerList[5].charStatus = "Dead";
                    }
                }
            }
            else
            {
                foreach (Player pc in mod.playerList)
                {
                    if (pc.name == pcIdentifier)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + pc.name + " takes " + dam + " damage" + "</font><BR>");
                        pc.hp -= dam;
                        if (pc.hp <= 0)
                        {
                            gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                            pc.charStatus = "Dead";
                        }
                    }
                }
            }
            
        }

        public void riddle()
        {
            /*TODO
            AlertDialog.Builder builder = new AlertDialog.Builder(gv.gameContext);
            builder.setTitle("Engraved you find the following:");
            builder.setMessage(Html.fromHtml("<i>\"First of the First King of Men only afew;<br>Second of the First Kingdom on lands anew;<br>Third of the First City in icy blue;<br>Fourth of the foe a friend he grew;<br>Right in the center of me and you.</i><br><br>Type in your answer below:"));

            // Set up the input
            final EditText input = new EditText(gv.gameContext);
            // Specify the type of input expected
            input.setInputType(InputType.TYPE_CLASS_TEXT);
            builder.setView(input);

            // Set up the buttons
            builder.setPositiveButton("Speak Answer", new DialogInterface.OnClickListener()
            {
                @Override
                public void onClick(DialogInterface dialog, int which)
                {
                    //if (input.getText().toString().length() > 0)
                    if ((input.getText().toString().equals("core") || input.getText().toString().equals("Core")))
                    {
                        EnableDisableTriggerEvent("riddleTrig", "1", "false");
                        EnableDisableTriggerEvent("riddleTrig", "2", "true");
                        MessageBox("That is correct...the chest opens");
                    }
                    else
                    {
                        MessageBox("Incorrect, try again");
                    }

                }
            });

            builder.setNegativeButton("Leave Chest Alone", new DialogInterface.OnClickListener()
            {
                @Override
                public void onClick(DialogInterface dialog, int which)
                {
                    dialog.cancel();
                }
            });

            builder.show();
            */
        }
        public void DamageWithoutItem(int damage, string itemTag)
        {
            bool itemfound = CheckForItem(itemTag, 1);
            if (itemfound)
            {
                //have item so 10% chance to damage
                if (RandInt(100) > 90)
                {
                    //do damage to all
                    gv.cc.addLogText("<font color='aqua'>drowning (-1hp ea)</font><br>");
                    foreach (Player pc in mod.playerList)
                    {
                        pc.hp -= damage;
                        if (pc.hp <= 0)
                        {
                            gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                            pc.charStatus = "Dead";
                        }
                    }
                }
            }
            else
            {
                //do not have item so 50% chance to damage
                if (RandInt(100) > 50)
                {
                    //do damage to all
                    gv.cc.addLogText("<font color='aqua'>drowning (-1hp ea)</font><br>");
                    foreach (Player pc in mod.playerList)
                    {
                        pc.hp -= damage;
                        if (pc.hp <= 0)
                        {
                            gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                            pc.charStatus = "Dead";
                        }
                    }
                }
            }
        }

        public bool CheckForItem(string resref, int quantity)
        {
            //check if item is on any of the party members
            if (mod.debugMode) //SD_20131102
            {
                gv.cc.addLogText("<font color='yellow'>" + "checkForItemResRef: " + resref + " quantity: " + quantity + "</font><BR>");
            }
            int numFound = 0;
            foreach (Player pc in mod.playerList)
            {
                if (pc.BodyRefs.resref.Equals(resref)) { numFound++; }
                if (pc.MainHandRefs.resref.Equals(resref)) { numFound++; }
                if (pc.RingRefs.resref.Equals(resref)) { numFound++; }
                if (pc.OffHandRefs.resref.Equals(resref)) { numFound++; }
                if (pc.HeadRefs.resref.Equals(resref)) { numFound++; }
                if (pc.GlovesRefs.resref.Equals(resref)) { numFound++; }
                if (pc.NeckRefs.resref.Equals(resref)) { numFound++; }
                if (pc.Ring2Refs.resref.Equals(resref)) { numFound++; }
                if (pc.FeetRefs.resref.Equals(resref)) { numFound++; }
            }
            foreach (ItemRefs item in mod.partyInventoryRefsList)
            {
                if (item.resref.Equals(resref)) { numFound += item.quantity; }
            }
            if (numFound >= quantity)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "found enough: " + resref + " numFound: " + numFound + "</font><BR>");
                }
                return true;
            }
            else
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "didn't find enough: " + resref + " numFound: " + numFound + "</font><BR>");
                }
                return false;
            }
        }
        public bool CheckIsRace(int PCIndex, string tag)
        {
            if (mod.playerList[PCIndex].race.tag.Equals(tag))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckPartyDistance(string tag, int distance)
        {
            Prop prp = GetProp(tag, "");
            if (prp == null)
            {
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>error finding prop in this area, returning 'false'</font><BR>");
                }
                return false;
            }
            
            int dist = 0;
            int deltaX = (int)Math.Abs((prp.LocationX - mod.PlayerLocationX));
            int deltaY = (int)Math.Abs((prp.LocationY - mod.PlayerLocationY));
            if (deltaX > deltaY)
            {
                dist = deltaX;
            }
            else
            {
                dist = deltaY;
            }
            if (mod.debugMode)
            {
                gv.cc.addLogText("<font color='yellow'>party distance from " + prp.PropTag + " is " + dist + "</font><BR>");
            }
            if (dist <= distance)
            {
                return true;
            }
            return false;
        }
        public bool CheckHasTrait(int PCIndex, string tag)
        {
            foreach (string s in mod.playerList[PCIndex].knownTraitsTags)
            {
                if (tag.Equals(s))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckHasSpell(int PCIndex, string tag)
        {
            foreach (string s in mod.playerList[PCIndex].knownSpellsTags)
            {
                if (tag.Equals(s))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckPassSkill(int PCIndex, string tag, int dc, bool useRollTen, bool isSilent)
        {
           
            int itemMod = 0;
            int skillMod = 0;
            int attMod = 0;
            Trait tr = new Trait();
            foreach (Player p in gv.mod.playerList)
            {
                p.powerOfThisPc = 0;
            }

            for (int i = 0; i <= gv.mod.playerList.Count-1; i++)
            {

                string foundLargest = "none";
                int largest = 0;
                foreach (string s in mod.playerList[i].knownTraitsTags)
                {
                    if (s.StartsWith(tag))
                    {
                        if (s.Equals(tag))
                        {
                            if (foundLargest.Equals("none"))
                            {
                                foundLargest = s;
                            }
                        }
                        else //get the number at the end 
                        {
                            string c = s.Substring(s.Length - 1, 1);
                            int j = Convert.ToInt32(c);
                            if (j > largest)
                            {
                                largest = j;
                                foundLargest = s;
                            }
                        }
                    }
                }

                skillMod = 0;
                //Trait tr = new Trait();
                if (!foundLargest.Equals("none"))
                {
                    //PC has trait skill so do calculation check
                    tr = mod.getTraitByTag(foundLargest);
                    skillMod = tr.skillModifier;
                }
                else
                {

                    foreach (Trait t in mod.moduleTraitsList)
                    {
                        if (t.tag.Contains(tag))
                        {
                            tr = mod.getTraitByTag(t.tag);
                            break;
                        }
                    }
                }

                attMod = 0;
                if (tr.skillModifierAttribute.Equals("str") || tr.skillModifierAttribute.Equals("strength") || tr.skillModifierAttribute.Equals("Str") || tr.skillModifierAttribute.Equals("Strength"))
                {
                    attMod = (mod.playerList[i].strength - 10) / 2;
                }
                else if (tr.skillModifierAttribute.Equals("dex") || tr.skillModifierAttribute.Equals("dexterity") || tr.skillModifierAttribute.Equals("Dex") || tr.skillModifierAttribute.Equals("Dexterity"))
                {
                    attMod = (mod.playerList[i].dexterity - 10) / 2;
                }
                else if (tr.skillModifierAttribute.Equals("int") || tr.skillModifierAttribute.Equals("intelligance") || tr.skillModifierAttribute.Equals("Int") || tr.skillModifierAttribute.Equals("Intelligence"))
                {
                    attMod = (mod.playerList[i].intelligence - 10) / 2;
                }
                else if (tr.skillModifierAttribute.Equals("cha") || tr.skillModifierAttribute.Equals("charisma") || tr.skillModifierAttribute.Equals("Cha") || tr.skillModifierAttribute.Equals("Charisma"))
                {
                    attMod = (mod.playerList[i].charisma - 10) / 2;
                }
                else if (tr.skillModifierAttribute.Equals("con") || tr.skillModifierAttribute.Equals("constitution") || tr.skillModifierAttribute.Equals("Con") || tr.skillModifierAttribute.Equals("Constitution"))
                {
                    attMod = (mod.playerList[i].constitution - 10) / 2;
                }
                else if (tr.skillModifierAttribute.Equals("wis") || tr.skillModifierAttribute.Equals("wisdom") || tr.skillModifierAttribute.Equals("Wis") || tr.skillModifierAttribute.Equals("Wisdom"))
                {
                    attMod = (mod.playerList[i].wisdom - 10) / 2;
                }

                itemMod = 0;

                if (mod.playerList[i].BodyRefs.resref != "none")
                {
                    Item itm = gv.mod.getItemByResRefForInfo(mod.playerList[i].BodyRefs.resref);
                    if (itm != null)
                    {
                        if (itm.tagOfTraitInfluenced.Contains(tag))
                        {
                            itemMod += itm.traitSkillRollModifier;
                        }
                    }
                }

                if (mod.playerList[i].RingRefs.resref != "none")
                {
                    Item itm = gv.mod.getItemByResRefForInfo(mod.playerList[i].RingRefs.resref);
                    if (itm != null)
                    {
                        if (itm.tagOfTraitInfluenced.Contains(tag))
                        {
                            itemMod += itm.traitSkillRollModifier;
                        }
                    }
                }

                if (mod.playerList[i].MainHandRefs.resref != "none")
                {
                    Item itm = gv.mod.getItemByResRefForInfo(mod.playerList[i].MainHandRefs.resref);
                    if (itm != null)
                    {
                        if (itm.tagOfTraitInfluenced.Contains(tag))
                        {
                            itemMod += itm.traitSkillRollModifier;
                        }
                    }
                }

                if (mod.playerList[i].OffHandRefs.resref != "none")
                {
                    Item itm = gv.mod.getItemByResRefForInfo(mod.playerList[i].OffHandRefs.resref);
                    if (itm != null)
                    {
                        if (itm.tagOfTraitInfluenced.Contains(tag))
                        {
                            itemMod += itm.traitSkillRollModifier;
                        }
                    }
                }

                if (mod.playerList[i].HeadRefs.resref != "none")
                {
                    Item itm = gv.mod.getItemByResRefForInfo(mod.playerList[i].HeadRefs.resref);
                    if (itm != null)
                    {
                        if (itm.tagOfTraitInfluenced.Contains(tag))
                        {
                            itemMod += itm.traitSkillRollModifier;
                        }
                    }
                }

                if (mod.playerList[i].GlovesRefs.resref != "none")
                {
                    Item itm = gv.mod.getItemByResRefForInfo(mod.playerList[i].GlovesRefs.resref);
                    if (itm != null)
                    {
                        if (itm.tagOfTraitInfluenced.Contains(tag))
                        {
                            itemMod += itm.traitSkillRollModifier;
                        }
                    }
                }

                if (mod.playerList[i].NeckRefs.resref != "none")
                {
                    Item itm = gv.mod.getItemByResRefForInfo(mod.playerList[i].NeckRefs.resref);
                    if (itm != null)
                    {
                        if (itm.tagOfTraitInfluenced.Contains(tag))
                        {
                            itemMod += itm.traitSkillRollModifier;
                        }
                    }
                }

                if (mod.playerList[i].Ring2Refs.resref != "none")
                {
                    Item itm = gv.mod.getItemByResRefForInfo(mod.playerList[i].Ring2Refs.resref);
                    if (itm != null)
                    {
                        if (itm.tagOfTraitInfluenced.Contains(tag))
                        {
                            itemMod += itm.traitSkillRollModifier;
                        }
                    }
                }

                if (mod.playerList[i].FeetRefs.resref != "none")
                {
                    Item itm = gv.mod.getItemByResRefForInfo(mod.playerList[i].FeetRefs.resref);
                    if (itm != null)
                    {
                        if (itm.tagOfTraitInfluenced.Contains(tag))
                        {
                            itemMod += itm.traitSkillRollModifier;
                        }
                    }
                }

                gv.mod.playerList[i].powerOfThisPc = attMod + skillMod + itemMod;
            }

            string playerName = "";
            int power = 0;

            //onlyone pc checks
            if (PCIndex >= -4)
            {
                //leader or directly selected
                if (PCIndex >= 0)
                {
                    playerName = gv.mod.playerList[PCIndex].name + " (selected character)";
                    power = gv.mod.playerList[PCIndex].powerOfThisPc;
                }

                //highest
                if (PCIndex == -2)
                {
                    int highestFound = -100;
                    foreach (Player p in gv.mod.playerList)
                    {
                        if (p.powerOfThisPc > highestFound)
                        {
                            playerName = p.name + " (best in group)";
                            power = p.powerOfThisPc;
                            highestFound = p.powerOfThisPc;
                        }
                    }
                }

                //lowest
                if (PCIndex == -3)
                {
                    int lowestFound = 10000;
                    foreach (Player p in gv.mod.playerList)
                    {
                        if (p.powerOfThisPc < lowestFound)
                        {
                            playerName = p.name + " (worst in group)";
                            power = p.powerOfThisPc;
                            lowestFound = p.powerOfThisPc;
                        }
                    }
                }

                //average
                if (PCIndex == -4)
                {
                    int sumOfPower = 0;
                    foreach (Player p in gv.mod.playerList)
                    {
                        sumOfPower += p.powerOfThisPc;
                    }
                    power = sumOfPower / gv.mod.playerList.Count;
                    playerName = "group average";
                }

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //all modifieirs build, lets roll
                int roll = gv.sf.RandInt(20);
                if (useRollTen)
                {
                    roll = 10;
                }
                //string power = (attMod + skillMod + itemMod).ToString();
                if (roll + power >= dc)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        //gv.cc.addLogText("<font color='yellow'> Skill check(" + tag + "): " + roll + "+" + attMod + "+" + skillMod + itemMod + ">=" + dc + "</font><BR>");
                        //gv.cc.addLogText("<font color='lime'> Static " + tr.name + " check of " + gv.mod.playerList[PCIndex] + " successful (" + roll + "+" + power + ">=" + dc + ")" + "</font><BR>");
                    }
                    if ((useRollTen) && (!isSilent))
                    {
                        gv.cc.addLogText("<font color='lime'> Static " + tr.name + " check of " + playerName + " successful (" + roll + "+" + power + ">=" + dc + ")" + "</font><BR>");
                    }
                    else if ((!useRollTen) && (!isSilent))
                    {
                        gv.cc.addLogText("<font color='lime'> Rolled " + tr.name + " check of " + playerName + " successful (" + roll + "+" + power + ">=" + dc + ")" + "</font><BR>");
                    }
                    return true;
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        //gv.cc.addLogText("<font color='yellow'> Skill check: " + roll + "+" + attMod + "+" + skillMod + itemMod + " < " + dc + "</font><BR>");
                    }
                    if ((useRollTen) && (!isSilent))
                    {
                        gv.cc.addLogText("<font color='red'> Static " + tr.name + " check of " + playerName + " failed (" + roll + "+" + power + " is less than " + dc + ")" + "</font><BR>");
                    }
                    else if ((!useRollTen) && (!isSilent))
                    {
                        gv.cc.addLogText("<font color='red'> Rolled " + tr.name + " check of " + playerName + " failed (" + roll + "+" + power + " is less than " + dc + ")" + "</font><BR>");
                    }
                    return false;
                }
            }
            //all pc must roll
            else
            {
                //allMustSucceed
                if (PCIndex == -5)
                {
                    int rollUsed = 0;
                    bool success = true;
                    foreach (Player p in gv.mod.playerList)
                    {
                        int roll = gv.sf.RandInt(20);
                        if (useRollTen)
                        {
                            roll = 10;
                        }
                        if (roll + p.powerOfThisPc < dc)
                        {
                            success = false;
                            playerName = p.name;
                            power = p.powerOfThisPc;
                            rollUsed = roll;
                            break;
                        }
                    }

                    if (success)
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            //gv.cc.addLogText("<font color='yellow'> Skill check(" + tag + "): " + roll + "+" + attMod + "+" + skillMod + itemMod + ">=" + dc + "</font><BR>");
                            //gv.cc.addLogText("<font color='lime'> Static " + tr.name + " check of " + gv.mod.playerList[PCIndex] + " successful (" + roll + "+" + power + ">=" + dc + ")" + "</font><BR>");
                        }
                        if ((useRollTen)&& (!isSilent))
                        {
                            gv.cc.addLogText("<font color='lime'> Static " + tr.name + " check (for not a single failure in group) was successful" + " (difficulty level was " + dc + ")" + "</font><BR>");
                        }
                        else if ((!useRollTen) && (!isSilent))
                        {
                            gv.cc.addLogText("<font color='lime'> Rolled " + tr.name + " check (for not a single failure in group) was successful" + " (difficulty level was " + dc + ")" + "</font><BR>");
                        }
                        return true;
                    }
                    else
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            //gv.cc.addLogText("<font color='yellow'> Skill check: " + roll + "+" + attMod + "+" + skillMod + itemMod + " < " + dc + "</font><BR>");
                        }
                        if ((useRollTen) && (!isSilent))
                        {
                            gv.cc.addLogText("<font color='red'> Static " + tr.name + " check (for not a single failure in group) was failed by " + playerName + " (" + rollUsed + "+" + power + " is less than" + dc + ")" + "</font><BR>");
                        }
                        else if ((!useRollTen) && (!isSilent))                        {
                            gv.cc.addLogText("<font color='red'> Rolled " + tr.name + " check (for not a single failure in group) was failed by " + playerName + " (" + rollUsed + "+" + power + " is less than" + dc + ")" + "</font><BR>");
                        }
                        return false;
                    }

                }

                //oneMustSucceed
                if (PCIndex == -6)
                {
                    bool success = false;
                    int rollUsed = 0;
                    foreach (Player p in gv.mod.playerList)
                    {
                        int roll = gv.sf.RandInt(20);
                        if (useRollTen)
                        {
                            roll = 10;
                        }
                        if (roll + p.powerOfThisPc >= dc)
                        {
                            success = true;
                            playerName = p.name;
                            power = p.powerOfThisPc;
                            rollUsed = roll;
                            break;
                        }
                    }
                    if (success)
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            //gv.cc.addLogText("<font color='yellow'> Skill check(" + tag + "): " + roll + "+" + attMod + "+" + skillMod + itemMod + ">=" + dc + "</font><BR>");
                            //gv.cc.addLogText("<font color='lime'> Static " + tr.name + " check of " + gv.mod.playerList[PCIndex] + " successful (" + roll + "+" + power + ">=" + dc + ")" + "</font><BR>");
                        }
                        if ((useRollTen) && (!isSilent))
                        {
                            gv.cc.addLogText("<font color='lime'> Static " + tr.name + " check (for one success in group) was successful for " + playerName + " (" + rollUsed + "+" + power + ">=" + dc + ")" + "</font><BR>");
                        }
                        else if ((!useRollTen) && (!isSilent))
                        {
                            gv.cc.addLogText("<font color='lime'> Rolled " + tr.name + " check (for one success in group) was successful for " + playerName + " (" + rollUsed + "+" + power + ">=" + dc + ")" + "</font><BR>");
                        }
                        return true;
                    }
                    else
                    {
                        if (mod.debugMode) //SD_20131102
                        {
                            //gv.cc.addLogText("<font color='yellow'> Skill check: " + roll + "+" + attMod + "+" + skillMod + itemMod + " < " + dc + "</font><BR>");
                        }
                        if ((useRollTen) && (!isSilent))
                        {
                            gv.cc.addLogText("<font color='red'> Static " + tr.name + " check (for one success in group) failed for everybody" + " (difficulty level was " + dc + ")" + "</font><BR>");
                        }
                        else if ((!useRollTen) && (!isSilent))
                        {
                            gv.cc.addLogText("<font color='red'> Rolled " + tr.name + " check (for one success in group) failed for everybody" + " (difficulty level was " + dc + ")" + "</font><BR>");
                        }
                        return false;
                    }
                }

                //just a catch, dont ever end here
                return false;
            }
        }

        public bool CheckIsMale(int PCIndex)
        {
            if (mod.playerList[PCIndex].isMale)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckIsClassLevel(int PCIndex, string tag, int level)
        {
            if (mod.playerList[PCIndex].playerClass.tag.Equals(tag))
            {
                if (mod.playerList[PCIndex].classLevel >= level)
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
                return false;
            }
        }
        public bool CheckFunds(int amount)
        {
            if (mod.partyGold >= amount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckAttribute(int PCIndex, string attribute, string compare, int value)
        {
            int pcAttValue = 0;
            if (attribute.Equals("str"))
            {
                pcAttValue = mod.playerList[PCIndex].strength;
            }
            else if (attribute.Equals("dex"))
            {
                pcAttValue = mod.playerList[PCIndex].dexterity;
            }
            else if (attribute.Equals("int"))
            {
                pcAttValue = mod.playerList[PCIndex].intelligence;
            }
            else if (attribute.Equals("cha"))
            {
                pcAttValue = mod.playerList[PCIndex].charisma;
            }
            else if (attribute.Equals("con"))
            {
                pcAttValue = mod.playerList[PCIndex].constitution;
            }
            else if (attribute.Equals("wis"))
            {
                pcAttValue = mod.playerList[PCIndex].wisdom;
            }
            else
            {
                return false;
            }

            if (compare.Equals("="))
            {
                if (pcAttValue == value)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "pcAttValue: " + pcAttValue + " == " + value + "</font><BR>");
                    }
                    return true;
                }
            }
            else if (compare.Equals(">"))
            {
                if (pcAttValue > value)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "pcAttValue: " + pcAttValue + " > " + value + "</font><BR>");
                    }
                    return true;
                }
            }
            else if (compare.Equals("<"))
            {
                if (pcAttValue < value)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "pcAttValue: " + pcAttValue + " < " + value + "</font><BR>");
                    }
                    return true;
                }
            }
            else if (compare.Equals("!"))
            {
                if (pcAttValue != value)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "pcAttValue: " + pcAttValue + " != " + value + "</font><BR>");
                    }
                    return true;
                }
            }
            return false;
        }
        public bool CheckProp(string tag, string index, string property)
        {
            if (gv.mod.currentArea.Props.Count < 1)
            {
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>didn't find prop in this area, returning 'false'</font><BR>");
                }
                return false;
            }

            Prop prp = gv.mod.currentArea.Props[0];
            if ((tag != null) && (!tag.Equals("")))
            {
                if (tag.Equals("thisProp"))
                {
                    prp = ThisProp;
                }
                else
                {
                    prp = gv.mod.currentArea.getPropByTag(tag);
                }
                if (prp == null)
                {
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>didn't find prop in this area (prop=null), returning 'false'</font><BR>");
                    }
                    return false;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int indx = Convert.ToInt32(index);
                if ((indx >= 0) && (indx < gv.mod.currentArea.Props.Count))
                {
                    prp = gv.mod.currentArea.Props[indx];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Prop index outside range of PropList size, returning 'false'</font><BR>");
                    }
                    return false;
                }
            }
            else
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>Do not recognize the Prop's tag or index, returning 'false'</font><BR>");
                }
            }

            if ((property.Equals("s")) || (property.Equals("S")) || (property.Equals("isShown")))
            {
                return prp.isShown;
            }
            else if ((property.Equals("m")) || (property.Equals("M")) || (property.Equals("isMover")))
            {
                return prp.isMover;
            }
            else if ((property.Equals("a")) || (property.Equals("A")) || (property.Equals("isActive")))
            {
                return prp.isActive;
            }
            else if ((property.Equals("c")) || (property.Equals("C")) || (property.Equals("isChaser")))
            {
                return prp.isChaser;
            }
            else if ((property.Equals("h")) || (property.Equals("H")) || (property.Equals("HasCollisions")))
            {
                return prp.HasCollision;
            }
            else
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>Do not recognize the property: " + property + ", returning 'false'</font><BR>");
                }
            }
            return false;
        }
        public void SetProp(string tag, string index, string property, string bln)
        {
            if (gv.mod.currentArea.Props.Count < 1)
            {
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>didn't find prop in this area, aborting SetProp</font><BR>");
                }
                return;
            }

            Prop prp = gv.mod.currentArea.Props[0];
            if ((tag != null) && (!tag.Equals("")))
            {
                if (tag.Equals("thisProp"))
                {
                    prp = ThisProp;
                }
                else
                {
                    prp = gv.mod.currentArea.getPropByTag(tag);
                }
                if (prp == null)
                {
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>didn't find prop in this area (prop=null), aborting SetProp</font><BR>");
                    }
                    return;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int indx = Convert.ToInt32(index);
                if ((indx >= 0) && (indx < gv.mod.currentArea.Props.Count))
                {
                    prp = gv.mod.currentArea.Props[indx];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Prop index outside range of PropList size, aborting SetProp</font><BR>");
                    }
                    return;
                }
            }
            else
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>Do not recognize the Prop's tag or index, aborting SetProp</font><BR>");
                }
            }

            bool setBool = Boolean.Parse(bln);

            if ((property.Equals("s")) || (property.Equals("S")) || (property.Equals("isShown")))
            {
                prp.isShown = setBool;
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>prop isShown set to " + bln + "</font><BR>");
                }
                return;
            }
            else if ((property.Equals("m")) || (property.Equals("M")) || (property.Equals("isMover")))
            {
                prp.isMover = setBool;
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>prop isMover set to " + bln + "</font><BR>");
                }
                return;
            }
            else if ((property.Equals("a")) || (property.Equals("A")) || (property.Equals("isActive")))
            {
                prp.isActive = setBool;
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>prop isActive set to " + bln + "</font><BR>");
                }
                return;
            }
            else if ((property.Equals("c")) || (property.Equals("C")) || (property.Equals("isChaser")))
            {
                prp.isChaser = setBool;
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>prop isChaser set to " + bln + "</font><BR>");
                }
                return;
            }
            else if ((property.Equals("h")) || (property.Equals("H")) || (property.Equals("HasCollisions")))
            {
                prp.HasCollision = setBool;
                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>prop HasCollisions set to " + bln + "</font><BR>");
                }
                return;
            }
            else
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>Do not recognize the property: " + property + ", aborting SetProp</font><BR>");
                }
            }
        }
        public Prop GetProp(string tag, string index)
        {
            Prop prp = null;
            if ((tag != null) && (!tag.Equals("")))
            {
                if (tag.Equals("thisProp"))
                {
                    prp = ThisProp;
                }
                else
                {
                    prp = gv.mod.currentArea.getPropByTag(tag);
                }
                if (prp == null)
                {
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>didn't find prop in this area (prop=null)</font><BR>");
                    }
                    return null;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int indx = Convert.ToInt32(index);
                if ((indx >= 0) && (indx < gv.mod.currentArea.Props.Count))
                {
                    prp = gv.mod.currentArea.Props[indx];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Prop index outside range of PropList size</font><BR>");
                    }
                    return null;
                }
            }
            else
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>Do not recognize the Prop's tag or index</font><BR>");
                }
            }
            return prp;
        }

        public Prop GetPropByUniqueTag(string tag)
        {
            Prop prp = null;

            if ((tag != null) && (!tag.Equals("")))
            {
                if (tag.Equals("thisProp"))
                {
                    prp = ThisProp;
                }
                else
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        for (int j = 0; j < gv.mod.moduleAreasObjects[i].Props.Count; j++)
                        {
                            if (gv.mod.moduleAreasObjects[i].Props[j].PropTag.Equals(tag))
                            {
                                prp = gv.mod.moduleAreasObjects[i].Props[j];
                                return prp;
                            }
                        }
                    }
                }
            }
            else return prp;
            return prp;
        }


        public Creature GetCreature(string tag, string index)
        {
            Creature crt = null;
            if ((tag != null) && (!tag.Equals("")))
            {
                if (tag.Equals("thisCreature"))
                {
                    crt = ThisCreature;
                }
                else
                {
                    crt = gv.screenCombat.GetCreatureByTag(tag);
                }
                if (crt == null)
                {
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>Could not find creature with tag: " + tag + ", aborting</font><BR>");
                    }
                    return null;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int parm2 = Convert.ToInt32(index);
                if ((parm2 >= 0) && (parm2 < gv.mod.currentEncounter.encounterCreatureList.Count))
                {
                    crt = gv.mod.currentEncounter.encounterCreatureList[parm2];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>index outside range of creatureList size, aborting</font><BR>");
                    }
                    return null;
                }
            }
            return crt;
        }

        public void castSpell(string spellString, string pcIdentifier, string casterLevelString, string logTextForCastingAction)
        {
            //unterhose

            //get spell section
            Spell spell = new Spell();
            foreach (Spell sp in gv.mod.moduleSpellsList)
            {
                if (sp.tag == spellString)
                {
                    spell = sp;
                    break;
                }
            }

            //turn casterLevelString to int
            int casterLevel = Convert.ToInt32(casterLevelString);

            //unterhose
            if (pcIdentifier == "party")
            {
                foreach (Player p in gv.mod.playerList)
                {
                    gv.cc.doSpellCalledFromScript(spell, p, casterLevel, logTextForCastingAction);
                }
            }
            else if (pcIdentifier == "-1")
            {
                gv.cc.doSpellCalledFromScript(spell, gv.mod.playerList[gv.mod.selectedPartyLeader], casterLevel, logTextForCastingAction);
            }
            else if (pcIdentifier == "0")
            {
                gv.cc.doSpellCalledFromScript(spell, gv.mod.playerList[0], casterLevel, logTextForCastingAction);
            }
            else if (pcIdentifier == "1")
            {
                gv.cc.doSpellCalledFromScript(spell, gv.mod.playerList[1], casterLevel, logTextForCastingAction);
            }
            else if (pcIdentifier == "2")
            {
                gv.cc.doSpellCalledFromScript(spell, gv.mod.playerList[2], casterLevel, logTextForCastingAction);
            }
            else if (pcIdentifier == "3")
            {
                gv.cc.doSpellCalledFromScript(spell, gv.mod.playerList[3], casterLevel, logTextForCastingAction);
            }
            else if (pcIdentifier == "4")
            {
                gv.cc.doSpellCalledFromScript(spell, gv.mod.playerList[4], casterLevel, logTextForCastingAction);
            }
            else if (pcIdentifier == "5")
            {
                gv.cc.doSpellCalledFromScript(spell, gv.mod.playerList[5], casterLevel, logTextForCastingAction);
            }
            else
            {
                foreach (Player pc in mod.playerList)
                {
                    if (pc.name == pcIdentifier)
                    {
                        gv.cc.doSpellCalledFromScript(spell, pc, casterLevel, logTextForCastingAction);
                        break;
                    }
                }
            }
        }


        public Player GetPlayer(string tag, string index)
        {
            Player pc = null;
            if ((tag != null) && (!tag.Equals("")))
            {
                pc = gv.mod.getPlayerByName(tag);
                if (pc == null)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>Could not find PC: " + tag + ", aborting</font><BR>");
                    }
                    return null;
                }
            }
            else if ((index != null) && (!index.Equals("")))
            {
                int parm2 = Convert.ToInt32(index);
                if (parm2 == -1)
                {
                    parm2 = gv.mod.selectedPartyLeader;
                }
                if ((parm2 >= 0) && (parm2 < gv.mod.playerList.Count))
                {
                    pc = gv.mod.playerList[parm2];
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'>index outside range of playerList size, aborting</font><BR>");
                    }
                    return null;
                }
            }
            return pc;
        }

        public bool CheckJournalEntry(string categoryTag, string compareOperator, int entryId)
        {
            foreach (JournalQuest quest in mod.partyJournalQuests)
            {
                if (quest.Tag.Equals(categoryTag))
                {
                    if (compareOperator.Equals("="))
                    {
                        foreach (JournalEntry entry in quest.Entries)
                        {
                            if (entry.EntryId == entryId)
                            {
                                return true;
                            }
                        }
                    }
                    else if (compareOperator.Equals(">"))
                    {
                        foreach (JournalEntry entry in quest.Entries)
                        {
                            if (entry.EntryId > entryId)
                            {
                                return true;
                            }
                        }
                    }
                    else if (compareOperator.Equals("<"))
                    {
                        foreach (JournalEntry entry in quest.Entries)
                        {
                            if (entry.EntryId >= entryId)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else if (compareOperator.Equals("!"))
                    {
                        foreach (JournalEntry entry in quest.Entries)
                        {
                            if (entry.EntryId == entryId)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }                
            }
            foreach (JournalQuest quest in mod.partyJournalCompleted)
            {
                if (quest.Tag.Equals(categoryTag))
                {
                    if (compareOperator.Equals("="))
                    {
                        foreach (JournalEntry entry in quest.Entries)
                        {
                            if (entry.EntryId == entryId)
                            {
                                return true;
                            }
                        }
                    }
                    else if (compareOperator.Equals(">"))
                    {
                        foreach (JournalEntry entry in quest.Entries)
                        {
                            if (entry.EntryId > entryId)
                            {
                                return true;
                            }
                        }
                    }
                    else if (compareOperator.Equals("<"))
                    {
                        foreach (JournalEntry entry in quest.Entries)
                        {
                            if (entry.EntryId >= entryId)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else if (compareOperator.Equals("!"))
                    {
                        foreach (JournalEntry entry in quest.Entries)
                        {
                            if (entry.EntryId == entryId)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        public void AddJournalEntry(string categoryTag, string entryTag)
        {
            JournalQuest jcm = mod.getJournalCategoryByTag(categoryTag);
            if (jcm != null)
            {
                JournalQuest jcp = mod.getPartyJournalActiveCategoryByTag(categoryTag);
                if (jcp != null) //an existing category, just add entry
                {
                    JournalEntry jem = jcm.getJournalEntryByTag(entryTag);
                    if (jem != null)
                    {
                        if (!entryAlreadyExists(jem.Tag))
                        {
                            jcp.Entries.Add(jem);
                            //if (jem.EndPoint)
                            //{
                                //mod.partyJournalCompleted.Add(jcp);
                                //mod.partyJournalQuests.Remove(jcp);
                            //}
                            //Toast.makeText(gv.gameContext, "Your journal has been updated with: " + jem.EntryTitle, Toast.LENGTH_LONG).show();
                            //gv.TrackerSendEvent("Journal", jcp.Name, jem.EntryTitle, 0l);
                            //gv.TrackerSendEventJournal(jcp.Name + " -- " + jem.EntryTitle);
                        }
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "module's journal entry wasn't found based on tag given", Toast.LENGTH_LONG).show();
                    }
                }
                else //a new category, add category and entry
                {
                    JournalQuest jcp2 = mod.getJournalCategoryByTag(categoryTag).DeepCopy();
                    //Toast.makeText(gv.gameContext, "player's journal category wasn't found based on tag given, creating category...", Toast.LENGTH_SHORT).show();
                    //MessageBox.Show("player's journal category wasn't found based on tag given, creating category...");
                    JournalEntry jem = jcm.getJournalEntryByTag(entryTag);
                    if (jem != null)
                    {
                        jcp2.Entries.Clear();
                        jcp2.Entries.Add(jem);
                        mod.partyJournalQuests.Add(jcp2);
                        //Toast.makeText(gv.gameContext, "Your journal has been updated with: " + jem.EntryTitle, Toast.LENGTH_LONG).show();
                        //gv.TrackerSendEvent("Journal", jcp2.Name, jem.EntryTitle, 0l);
                        //gv.TrackerSendEventJournal(jcp2.Name + " -- " + jem.EntryTitle);
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "module's journal entry wasn't found based on tag given", Toast.LENGTH_LONG).show();
                    }
                }
            }
            else
            {
                //Toast.makeText(gv.gameContext, "module's journal category wasn't found based on tag given", Toast.LENGTH_LONG).show();
            }
        }
        public void AddJournalEntryNoMessages(string categoryTag, string entryTag)
        {
            JournalQuest jcm = mod.getJournalCategoryByTag(categoryTag);
            if (jcm != null)
            {
                JournalQuest jcp = mod.getPartyJournalActiveCategoryByTag(categoryTag);
                if (jcp != null) //an existing category, just add entry
                {
                    JournalEntry jem = jcm.getJournalEntryByTag(entryTag);
                    if (jem != null)
                    {
                        if (!entryAlreadyExists(jem.Tag))
                        {
                            jcp.Entries.Add(jem);
                            //if (jem.EndPoint)
                            //{
                                //mod.partyJournalCompleted.Add(jcp);
                                //mod.partyJournalQuests.Remove(jcp);
                            //}
                            //Toast.makeText(gv.gameContext, "Your journal has been updated with: " + jem.EntryTitle, Toast.LENGTH_LONG).show();
                        }
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "module's journal entry wasn't found based on tag given", Toast.LENGTH_LONG).show();
                    }
                }
                else //a new category, add category and entry
                {
                    JournalQuest jcp2 = mod.getJournalCategoryByTag(categoryTag).DeepCopy();
                    //Toast.makeText(gv.gameContext, "player's journal category wasn't found based on tag given, creating category...", Toast.LENGTH_SHORT).show();
                    JournalEntry jem = jcm.getJournalEntryByTag(entryTag);
                    if (jem != null)
                    {
                        jcp2.Entries.Clear();
                        jcp2.Entries.Add(jem);
                        mod.partyJournalQuests.Add(jcp2);
                        //Toast.makeText(gv.gameContext, "Your journal has been updated with: " + jem.EntryTitle, Toast.LENGTH_LONG).show();
                        //if (jem.EndPoint)
                        //{
                            //mod.partyJournalCompleted.Add(jcp2);
                            //mod.partyJournalQuests.Remove(jcp2);
                        //}
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "module's journal entry wasn't found based on tag given", Toast.LENGTH_LONG).show();
                    }
                }
            }
            else
            {
                //Toast.makeText(gv.gameContext, "module's journal category wasn't found based on tag given", Toast.LENGTH_LONG).show();
            }
        }
        public bool entryAlreadyExists(string entryTag)
        {
            foreach (JournalQuest quest in mod.partyJournalQuests)
            {
                foreach (JournalEntry entry in quest.Entries)
                {
                    if (entry.Tag.Equals(entryTag))
                    {
                        return true;
                    }
                }
            }
            foreach (JournalQuest quest in mod.partyJournalCompleted)
            {
                foreach (JournalEntry entry in quest.Entries)
                {
                    if (entry.Tag.Equals(entryTag))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //UPDATE STATS
        public void UpdateStats(Player pc)
        {
            //used at level up, doPcTurn, open inventory, etc.
            ReCalcSavingThrowBases(pc); //SD_20131029

            pc.fortitude = pc.baseFortitude + CalcSavingThrowModifiersFortitude(pc) + (pc.constitution - 10) / 2 + gv.mod.poorVisionModifier; //SD_20131127
            pc.will = pc.baseWill + CalcSavingThrowModifiersWill(pc) + (pc.intelligence - 10) / 2 + gv.mod.poorVisionModifier; //SD_20131127
            pc.reflex = pc.baseReflex + CalcSavingThrowModifiersReflex(pc) + (pc.dexterity - 10) / 2 + gv.mod.poorVisionModifier; //SD_20131127
            pc.strength = pc.baseStr + pc.race.strMod + CalcAttributeModifierStr(pc); //SD_20131127
            pc.dexterity = pc.baseDex + pc.race.dexMod + CalcAttributeModifierDex(pc); //SD_20131127
            pc.intelligence = pc.baseInt + pc.race.intMod + CalcAttributeModifierInt(pc); //SD_20131127
            pc.charisma = pc.baseCha + pc.race.chaMod + CalcAttributeModifierCha(pc); //SD_20131127
            pc.wisdom = pc.baseWis + pc.race.wisMod + CalcAttributeModifierWis(pc); //SD_20131127
            pc.constitution = pc.baseCon + pc.race.conMod + CalcAttributeModifierCon(pc); //SD_20131127
            pc.luck = pc.baseLuck + pc.race.luckMod + CalcAttributeModifierLuk(pc);
            pc.damageTypeResistanceTotalAcid = pc.race.damageTypeResistanceValueAcid + CalcAcidModifiers(pc);
            if (pc.damageTypeResistanceTotalAcid > gv.mod.resistanceMaxValue) { pc.damageTypeResistanceTotalAcid = gv.mod.resistanceMaxValue; }
            pc.damageTypeResistanceTotalNormal = pc.race.damageTypeResistanceValueNormal + CalcNormalModifiers(pc);
            if (pc.damageTypeResistanceTotalNormal > gv.mod.resistanceMaxValue) { pc.damageTypeResistanceTotalNormal = gv.mod.resistanceMaxValue; }
            pc.damageTypeResistanceTotalCold = pc.race.damageTypeResistanceValueCold + CalcColdModifiers(pc);
            if (pc.damageTypeResistanceTotalCold > gv.mod.resistanceMaxValue) { pc.damageTypeResistanceTotalCold = gv.mod.resistanceMaxValue; }
            pc.damageTypeResistanceTotalElectricity = pc.race.damageTypeResistanceValueElectricity + CalcElectricityModifiers(pc);
            if (pc.damageTypeResistanceTotalElectricity > gv.mod.resistanceMaxValue) { pc.damageTypeResistanceTotalElectricity = gv.mod.resistanceMaxValue; }
            pc.damageTypeResistanceTotalFire = pc.race.damageTypeResistanceValueFire + CalcFireModifiers(pc);
            if (pc.damageTypeResistanceTotalFire > gv.mod.resistanceMaxValue) { pc.damageTypeResistanceTotalFire = gv.mod.resistanceMaxValue; }
            pc.damageTypeResistanceTotalMagic = pc.race.damageTypeResistanceValueMagic + CalcMagicModifiers(pc);
            if (pc.damageTypeResistanceTotalMagic > gv.mod.resistanceMaxValue) { pc.damageTypeResistanceTotalMagic = gv.mod.resistanceMaxValue; }
            pc.damageTypeResistanceTotalPoison = pc.race.damageTypeResistanceValuePoison + CalcPoisonModifiers(pc);
            if (pc.damageTypeResistanceTotalPoison > gv.mod.resistanceMaxValue) { pc.damageTypeResistanceTotalPoison = gv.mod.resistanceMaxValue; }
            
            if (pc.playerClass.babTable.Length > 0)//SD_20131115
            {
                pc.baseAttBonus = pc.playerClass.babTable[pc.classLevel] + CalcBABAdders(pc); //SD_20131115
            }

            int modifierFromSPRelevantAttribute = 0;
            foreach (PlayerClass pClass in gv.mod.modulePlayerClassList)
            {
                if (pc.classTag == pClass.tag)
                {
                    if (pClass.modifierFromSPRelevantAttribute.Contains("intelligence"))
                    {
                        modifierFromSPRelevantAttribute = (pc.intelligence -10) / 2;
                    }
                    if (pClass.modifierFromSPRelevantAttribute.Contains("wisdom"))
                    {
                        modifierFromSPRelevantAttribute = (pc.wisdom - 10) / 2;
                    }
                    if (pClass.modifierFromSPRelevantAttribute.Contains("charisma"))
                    {
                        modifierFromSPRelevantAttribute = (pc.charisma - 10) / 2;
                    }
                    if (pClass.modifierFromSPRelevantAttribute.Contains("constitution"))
                    {
                        modifierFromSPRelevantAttribute = (pc.constitution - 10) / 2;
                    }
                    if (pClass.modifierFromSPRelevantAttribute.Contains("strength"))
                    {
                        modifierFromSPRelevantAttribute = (pc.strength - 10) / 2;
                    }
                    if (pClass.modifierFromSPRelevantAttribute.Contains("dexterity"))
                    {
                        modifierFromSPRelevantAttribute = (pc.dexterity - 10) / 2;
                    }
                    if (pClass.modifierFromSPRelevantAttribute.Contains("luck"))
                    {
                        modifierFromSPRelevantAttribute = (pc.luck - 10) / 2;
                    }
                    break;
                }
            }

            int cMod = (pc.constitution - 10) / 2;
            int iMod = modifierFromSPRelevantAttribute;
            pc.spMax = pc.playerClass.startingSP + iMod + ((pc.classLevel - 1) * (pc.playerClass.spPerLevelUp + iMod)) + CalcAttributeModifierSpMax(pc) + CalcModifierMaxSP(pc);
            pc.hpMax = pc.playerClass.startingHP + cMod + ((pc.classLevel - 1) * (pc.playerClass.hpPerLevelUp + cMod)) + CalcAttributeModifierHpMax(pc) + CalcModifierMaxHP(pc);

            pc.XPNeeded = pc.playerClass.xpTable[pc.classLevel];

            int dMod = (pc.dexterity - 10) / 2;
            int maxDex = CalcMaxDexBonus(pc);
            if (dMod > maxDex) { dMod = maxDex; }
            int armBonus = 0;
            int acMods = 0;
            armBonus = CalcArmorBonuses(pc);
            acMods = CalcACModifiers(pc);
            pc.AC = pc.ACBase + dMod + armBonus + acMods + gv.mod.poorVisionModifier;
            if (mod.getItemByResRefForInfo(pc.BodyRefs.resref).ArmorWeightType.Equals("Light"))
            {
                pc.moveDistance = pc.race.MoveDistanceLightArmor + CalcMovementBonuses(pc);
            }
            else //medium or heavy SD_20131116
            {
                pc.moveDistance = pc.race.MoveDistanceMediumHeavyArmor + CalcMovementBonuses(pc);
            }
            RunAllItemWhileEquippedScripts(pc);
            if (pc.hp > pc.hpMax) { pc.hp = pc.hpMax; } //SD_20131201
            if (pc.sp > pc.spMax) { pc.sp = pc.spMax; } //SD_20131201
            if (pc.hp > 0)
            {
                pc.charStatus = "Alive";
            }
        }
        public void ReCalcSavingThrowBases(Player pc)
        {
            if (!pc.playerClass.name.Equals("newClass"))
            {
                pc.baseFortitude = pc.playerClass.baseFortitudeAtLevel[pc.classLevel];
                pc.baseReflex = pc.playerClass.baseReflexAtLevel[pc.classLevel];
                pc.baseWill = pc.playerClass.baseWillAtLevel[pc.classLevel];
            }
        }

        public bool isPassiveTraitApplied(Effect ef, Player pc)
        {
            //code block for cheking tag based trait requirements

            //we have an effect coming from a passive trait
            //for non permanent effects fed to it the method returns true in any case
            if (ef.isPermanent)
            {//1
                bool traitWorksForThisPC = false;

                // set up effects lists for traitWorksOnlyWhen and traitWorksNeverWhen
                ef.traitWorksNeverWhen.Clear();
                ef.traitWorksOnlyWhen.Clear();
                //go through all trait tags of pc
                foreach (string traitTag in pc.knownTraitsTags)
                {//2
                 //go through all traits of module
                    foreach (Trait t in gv.mod.moduleTraitsList)
                    {//3
                     //found a trait the pc has
                        if (t.tag.Equals(traitTag))
                        {//4
                         //go through effect tags for drop down list of this trait
                            foreach (EffectTagForDropDownList effectTag in t.traitEffectTagList)
                            {//5
                             //found out that our current effect ef stems from this trait of the pc
                                if (effectTag.tag.Equals(ef.tag))
                                {//6
                                 //built the lists on runtime for our current ef from the trait's template
                                    foreach (LocalImmunityString ls in t.traitWorksOnlyWhen)
                                    {//7
                                        LocalImmunityString ls2 = ls.DeepCopy();
                                        ef.traitWorksOnlyWhen.Add(ls2);
                                    }//7closed

                                    foreach (LocalImmunityString ls in t.traitWorksNeverWhen)
                                    {//7
                                        LocalImmunityString ls2 = ls.DeepCopy();
                                        ef.traitWorksNeverWhen.Add(ls2);
                                    }//7closed

                                    //ef.traitWorksNeverWhen = t.traitWorksNeverWhen;
                                    //ef.traitWorksOnlyWhen = t.traitWorksOnlyWhen;
                                }//6closed
                            }//5 closed
                        }//4closed
                    }//3closed
                }//2clsoed

                if (ef.traitWorksOnlyWhen.Count <= 0)
                {
                    traitWorksForThisPC = true;
                }

                //note that the tratNeccessities are logically connected with OR the way it is setup
                else
                    foreach (LocalImmunityString traitNeccessity in ef.traitWorksOnlyWhen)
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
                    foreach (LocalImmunityString traitRedFlag in ef.traitWorksNeverWhen)
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
                //note: work with  if (traitWorksForThisPC){} from here on

                if (traitWorksForThisPC)
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

        /*
        public int getSumOfSameTypeEffects (Effect ef, Player pc)
        {
            // or addEffectByObjet
            //find all stackable effects of this type and add them together
            foreach (Effect ef2 in pc.effectsList)
            {

            }
            //compare the result with each non-stackable effect of this type and return the highest
        }
        */
        /*
        public int CalcPcSpRegenInCombat(Player pc)
         {  
             
            int adder = 0;  
           
            //go through all traits and see if has passive HP regen type trait, use largest, not cumulative  
            foreach (string taTag in pc.knownTraitsTags)  
             {  
                 Trait ta = mod.getTraitByTag(taTag);  
                 foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
                 {  
                     Effect ef = mod.getEffectByTag(efTag.tag);  
                    
                     if (ef.modifySpInCombat > adder)  
                     {  
                         adder = ef.modifySpInCombat;  
                     }  
                 }  
             }  
             return adder;  
        }

        public int CalcPcHpRegenInCombat(Player pc)
        {

            int adder = 0;

            //go through all traits and see if has passive HP regen type trait, use largest, not cumulative  
            foreach (string taTag in pc.knownTraitsTags)
            {
                Trait ta = mod.getTraitByTag(taTag);
                foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)
                {
                    Effect ef = mod.getEffectByTag(efTag.tag);

                    if (ef.modifyHpInCombat > adder)
                    {
                        adder = ef.modifyHpInCombat;
                    }
                }
            }
            return adder;
        }
        */

        public int CalcAttributeModifierHpMax(Player pc)
        {
            int hpMaxBonuses = 0;
            /*
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).savingThrowModifierReflex;
            */
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        hpMaxBonuses += ef.modifyHpMax;
                    }
                    else
                    {
                        if ((ef.modifyHpMax != 0) && (ef.modifyHpMax > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyHpMax;
                        }
                    }
                }
            }
            if (highestNonStackable > hpMaxBonuses) { hpMaxBonuses = highestNonStackable; }
            return hpMaxBonuses;
        }

        public int CalcAttributeModifierSpMax(Player pc)
        {
            int spMaxBonuses = 0;
            /*
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).savingThrowModifierReflex;
            hpMaxBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).savingThrowModifierReflex;
            */
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        spMaxBonuses += ef.modifySpMax;
                    }
                    else
                    {
                        if ((ef.modifySpMax != 0) && (ef.modifySpMax > highestNonStackable))
                        {
                            highestNonStackable = ef.modifySpMax;
                        }
                    }
                }
            }
            if (highestNonStackable > spMaxBonuses) { spMaxBonuses = highestNonStackable; }
            return spMaxBonuses;
        }

        public int CalcSavingThrowModifiersReflex(Player pc)
        {
            int savBonuses = 0;
            savBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).savingThrowModifierReflex;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        savBonuses += ef.modifyReflex;
                    }
                    else
                    {
                        if ((ef.modifyReflex != 0) && (ef.modifyReflex > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyReflex;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { savBonuses = highestNonStackable; }
            return savBonuses;
        }
        public int CalcSavingThrowModifiersFortitude(Player pc)
        {
            int savBonuses = 0;
            savBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).savingThrowModifierFortitude;
            savBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).savingThrowModifierFortitude;
            savBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).savingThrowModifierFortitude;
            savBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).savingThrowModifierFortitude;
            savBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).savingThrowModifierFortitude;
            savBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).savingThrowModifierFortitude;
            savBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).savingThrowModifierFortitude;
            savBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).savingThrowModifierFortitude;
            savBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).savingThrowModifierFortitude;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        savBonuses += ef.modifyFortitude;
                    }
                    else
                    {
                        if ((ef.modifyFortitude != 0) && (ef.modifyFortitude > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyFortitude;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { savBonuses = highestNonStackable; }
            return savBonuses;
        }
        public int CalcSavingThrowModifiersWill(Player pc)
        {
            int savBonuses = 0;
            savBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).savingThrowModifierWill;
            savBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).savingThrowModifierWill;
            savBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).savingThrowModifierWill;
            savBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).savingThrowModifierWill;
            savBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).savingThrowModifierWill;
            savBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).savingThrowModifierWill;
            savBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).savingThrowModifierWill;
            savBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).savingThrowModifierWill;
            savBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).savingThrowModifierWill;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        savBonuses += ef.modifyWill;
                    }
                    else
                    {
                        if ((ef.modifyWill != 0) && (ef.modifyWill > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyWill;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { savBonuses = highestNonStackable; }
            return savBonuses;
        }

        public int CalcAttributeModifierStr(Player pc)
        {
            int attBonuses = 0;
            attBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).attributeBonusModifierStr;
            attBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attributeBonusModifierStr;
            attBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).attributeBonusModifierStr;
            attBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).attributeBonusModifierStr;
            attBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).attributeBonusModifierStr;
            attBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).attributeBonusModifierStr;
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierStr;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierStr;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierStr;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                        if (ef.isStackableEffect)
                        {
                            attBonuses += ef.modifyStr;
                        }
                        else
                        {
                            if ((ef.modifyStr != 0) && (ef.modifyStr > highestNonStackable))
                            {
                                highestNonStackable = ef.modifyStr;
                            }
                        }
                    }
            }
            if (highestNonStackable > -99) { attBonuses = highestNonStackable; }
            return attBonuses;
        }

        public int CalcModifierMaxHP(Player pc)
        {
            int attBonuses = 0;
            attBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).modifierMaxHP;
            attBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).modifierMaxHP;
            attBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).modifierMaxHP;
            attBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).modifierMaxHP;
            attBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).modifierMaxHP;
            attBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).modifierMaxHP;
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).modifierMaxHP;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).modifierMaxHP;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).modifierMaxHP;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        attBonuses += ef.modifyHpMax;
                    }
                    else
                    {
                        if ((ef.modifyHpMax != 0) && (ef.modifyHpMax > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyHpMax;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { attBonuses = highestNonStackable; }
            return attBonuses;
        }

        public int CalcModifierMaxSP(Player pc)
        {
            int attBonuses = 0;
            attBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).modifierMaxSP;
            attBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).modifierMaxSP;
            attBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).modifierMaxSP;
            attBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).modifierMaxSP;
            attBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).modifierMaxSP;
            attBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).modifierMaxSP;
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).modifierMaxSP;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).modifierMaxSP;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).modifierMaxSP;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        attBonuses += ef.modifySpMax;
                    }
                    else
                    {
                        if ((ef.modifySpMax != 0) && (ef.modifySpMax > highestNonStackable))
                        {
                            highestNonStackable = ef.modifySpMax;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { attBonuses = highestNonStackable; }
            return attBonuses;
        }

        public int CalcAttributeModifierDex(Player pc)
        {
            int attBonuses = 0;
            attBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).attributeBonusModifierDex;
            attBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attributeBonusModifierDex;
            attBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).attributeBonusModifierDex;
            attBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).attributeBonusModifierDex;
            attBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).attributeBonusModifierDex;
            attBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).attributeBonusModifierDex;
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierDex;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierDex;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierDex;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    
                        if (ef.isStackableEffect)
                        {
                            attBonuses += ef.modifyDex;
                        }
                        else
                        {
                            if ((ef.modifyDex != 0) && (ef.modifyDex > highestNonStackable))
                            {
                                highestNonStackable = ef.modifyDex;
                            }
                        }
                }
            }
            if (highestNonStackable > -99) { attBonuses = highestNonStackable; }
            return attBonuses;
        }
        public int CalcAttributeModifierInt(Player pc)
        {
            int attBonuses = 0;
            attBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).attributeBonusModifierInt;
            attBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attributeBonusModifierInt;
            attBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).attributeBonusModifierInt;
            attBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).attributeBonusModifierInt;
            attBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).attributeBonusModifierInt;
            attBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).attributeBonusModifierInt;
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierInt;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierInt;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierInt;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    //if (!ef.isPermanent)
                //{
                    if (ef.isStackableEffect)
                    {
                        attBonuses += ef.modifyInt;
                    }
                    else
                    {
                        if ((ef.modifyInt != 0) && (ef.modifyInt > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyInt;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { attBonuses = highestNonStackable; }
            return attBonuses;
        }
        public int CalcAttributeModifierCha(Player pc)
        {
            int attBonuses = 0;
            attBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).attributeBonusModifierCha;
            attBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attributeBonusModifierCha;
            attBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).attributeBonusModifierCha;
            attBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).attributeBonusModifierCha;
            attBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).attributeBonusModifierCha;
            attBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).attributeBonusModifierCha;
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierCha;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierCha;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierCha;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        attBonuses += ef.modifyCha;
                    }
                    else
                    {
                        if ((ef.modifyCha != 0) && (ef.modifyCha > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyCha;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { attBonuses = highestNonStackable; }
            return attBonuses;
        }
        public int CalcAttributeModifierCon(Player pc)
        {
            int attBonuses = 0;
            attBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).attributeBonusModifierCon;
            attBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attributeBonusModifierCon;
            attBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).attributeBonusModifierCon;
            attBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).attributeBonusModifierCon;
            attBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).attributeBonusModifierCon;
            attBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).attributeBonusModifierCon;
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierCon;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierCon;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierCon;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        attBonuses += ef.modifyCon;
                    }
                    else
                    {
                        if ((ef.modifyCon != 0) && (ef.modifyCon > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyCon;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { attBonuses = highestNonStackable; }
            return attBonuses;
        }
        public int CalcAttributeModifierWis(Player pc)
        {
            int attBonuses = 0;
            attBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).attributeBonusModifierWis;
            attBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attributeBonusModifierWis;
            attBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).attributeBonusModifierWis;
            attBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).attributeBonusModifierWis;
            attBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).attributeBonusModifierWis;
            attBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).attributeBonusModifierWis;
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierWis;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierWis;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierWis;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        attBonuses += ef.modifyWis;
                    }
                    else
                    {
                        if ((ef.modifyWis != 0) && (ef.modifyWis > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyWis;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { attBonuses = highestNonStackable; }
            return attBonuses;
        }
        public int CalcAttributeModifierLuk(Player pc)
        {
            int attBonuses = 0;
            attBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).attributeBonusModifierLuk;
            attBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).attributeBonusModifierLuk;
            attBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).attributeBonusModifierLuk;
            attBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).attributeBonusModifierLuk;
            attBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).attributeBonusModifierLuk;
            attBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).attributeBonusModifierLuk;
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierLuk;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierLuk;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierLuk;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        attBonuses += ef.modifyLuk;
                    }
                    else
                    {
                        if ((ef.modifyLuk != 0) && (ef.modifyLuk > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyLuk;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { attBonuses = highestNonStackable; }
            return attBonuses;
        }
        public int CalcAcidModifiers(Player pc)
        {
            int md = 0;
            md += mod.getItemByResRefForInfo(pc.BodyRefs.resref).damageTypeResistanceValueAcid;
            md += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageTypeResistanceValueAcid;
            md += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).damageTypeResistanceValueAcid;
            md += mod.getItemByResRefForInfo(pc.RingRefs.resref).damageTypeResistanceValueAcid;
            md += mod.getItemByResRefForInfo(pc.HeadRefs.resref).damageTypeResistanceValueAcid;
            md += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).damageTypeResistanceValueAcid;
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueAcid;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueAcid;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueAcid;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        md += ef.modifyDamageTypeResistanceAcid;
                    }
                    else
                    {
                        if ((ef.modifyDamageTypeResistanceAcid != 0) && (ef.modifyDamageTypeResistanceAcid > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyDamageTypeResistanceAcid;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { md = highestNonStackable; }
            return md;
        }
        public int CalcNormalModifiers(Player pc)
        {
            int md = 0;
            md += mod.getItemByResRefForInfo(pc.BodyRefs.resref).damageTypeResistanceValueNormal;
            md += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageTypeResistanceValueNormal;
            md += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).damageTypeResistanceValueNormal;
            md += mod.getItemByResRefForInfo(pc.RingRefs.resref).damageTypeResistanceValueNormal;
            md += mod.getItemByResRefForInfo(pc.HeadRefs.resref).damageTypeResistanceValueNormal;
            md += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).damageTypeResistanceValueNormal;
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueNormal;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueNormal;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueNormal;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        md += ef.modifyDamageTypeResistanceNormal;
                    }
                    else
                    {
                        if ((ef.modifyDamageTypeResistanceNormal != 0) && (ef.modifyDamageTypeResistanceNormal > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyDamageTypeResistanceNormal;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { md = highestNonStackable; }
            return md;
        }
        public int CalcColdModifiers(Player pc)
        {
            int md = 0;
            md += mod.getItemByResRefForInfo(pc.BodyRefs.resref).damageTypeResistanceValueCold;
            md += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageTypeResistanceValueCold;
            md += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).damageTypeResistanceValueCold;
            md += mod.getItemByResRefForInfo(pc.RingRefs.resref).damageTypeResistanceValueCold;
            md += mod.getItemByResRefForInfo(pc.HeadRefs.resref).damageTypeResistanceValueCold;
            md += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).damageTypeResistanceValueCold;
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueCold;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueCold;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueCold;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        md += ef.modifyDamageTypeResistanceCold;
                    }
                    else
                    {
                        if ((ef.modifyDamageTypeResistanceCold != 0) && (ef.modifyDamageTypeResistanceCold > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyDamageTypeResistanceCold;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { md = highestNonStackable; }
            return md;
        }
        public int CalcElectricityModifiers(Player pc)
        {
            int md = 0;
            md += mod.getItemByResRefForInfo(pc.BodyRefs.resref).damageTypeResistanceValueElectricity;
            md += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageTypeResistanceValueElectricity;
            md += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).damageTypeResistanceValueElectricity;
            md += mod.getItemByResRefForInfo(pc.RingRefs.resref).damageTypeResistanceValueElectricity;
            md += mod.getItemByResRefForInfo(pc.HeadRefs.resref).damageTypeResistanceValueElectricity;
            md += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).damageTypeResistanceValueElectricity;
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueElectricity;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueElectricity;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueElectricity;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        md += ef.modifyDamageTypeResistanceElectricity;
                    }
                    else
                    {
                        if ((ef.modifyDamageTypeResistanceElectricity != 0) && (ef.modifyDamageTypeResistanceElectricity > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyDamageTypeResistanceElectricity;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { md = highestNonStackable; }
            return md;
        }
        public int CalcFireModifiers(Player pc)
        {
            int md = 0;
            md += mod.getItemByResRefForInfo(pc.BodyRefs.resref).damageTypeResistanceValueFire;
            md += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageTypeResistanceValueFire;
            md += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).damageTypeResistanceValueFire;
            md += mod.getItemByResRefForInfo(pc.RingRefs.resref).damageTypeResistanceValueFire;
            md += mod.getItemByResRefForInfo(pc.HeadRefs.resref).damageTypeResistanceValueFire;
            md += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).damageTypeResistanceValueFire;
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueFire;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueFire;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueFire;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        md += ef.modifyDamageTypeResistanceFire;
                    }
                    else
                    {
                        if ((ef.modifyDamageTypeResistanceFire != 0) && (ef.modifyDamageTypeResistanceFire > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyDamageTypeResistanceFire;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { md = highestNonStackable; }
            return md;
        }
        public int CalcMagicModifiers(Player pc)
        {
            int md = 0;
            md += mod.getItemByResRefForInfo(pc.BodyRefs.resref).damageTypeResistanceValueMagic;
            md += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageTypeResistanceValueMagic;
            md += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).damageTypeResistanceValueMagic;
            md += mod.getItemByResRefForInfo(pc.RingRefs.resref).damageTypeResistanceValueMagic;
            md += mod.getItemByResRefForInfo(pc.HeadRefs.resref).damageTypeResistanceValueMagic;
            md += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).damageTypeResistanceValueMagic;
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueMagic;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueMagic;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueMagic;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        md += ef.modifyDamageTypeResistanceMagic;
                    }
                    else
                    {
                        if ((ef.modifyDamageTypeResistanceMagic != 0) && (ef.modifyDamageTypeResistanceMagic > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyDamageTypeResistanceMagic;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { md = highestNonStackable; }
            return md;
        }
        public int CalcPoisonModifiers(Player pc)
        {
            int md = 0;
            md += mod.getItemByResRefForInfo(pc.BodyRefs.resref).damageTypeResistanceValuePoison;
            md += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).damageTypeResistanceValuePoison;
            md += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).damageTypeResistanceValuePoison;
            md += mod.getItemByResRefForInfo(pc.RingRefs.resref).damageTypeResistanceValuePoison;
            md += mod.getItemByResRefForInfo(pc.HeadRefs.resref).damageTypeResistanceValuePoison;
            md += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).damageTypeResistanceValuePoison;
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValuePoison;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValuePoison;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValuePoison;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        md += ef.modifyDamageTypeResistancePoison;
                    }
                    else
                    {
                        if ((ef.modifyDamageTypeResistancePoison != 0) && (ef.modifyDamageTypeResistancePoison > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyDamageTypeResistancePoison;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { md = highestNonStackable; }
            return md;
        }
        public int CalcBABAdders(Player pc)
        {            
            int adder = 0;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        adder += ef.babModifier;
                    }
                    else
                    {
                        if ((ef.babModifier != 0) && (ef.babModifier > highestNonStackable))
                        {
                            highestNonStackable = ef.babModifier;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { adder = highestNonStackable; }
            return adder;
        }
        public int CalcACModifiers(Player pc)
        {
            int adder = 0;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        adder += ef.acModifier;
                    }
                    else
                    {
                        if ((ef.acModifier != 0) && (ef.acModifier > highestNonStackable))
                        {
                            highestNonStackable = ef.acModifier;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { adder = highestNonStackable; }
            return adder;
        }
        public int CalcArmorBonuses(Player pc)
        {
            int armBonuses = 0;
            armBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).armorBonus;
            armBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).armorBonus;
            armBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).armorBonus;
            armBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).armorBonus;
            armBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).armorBonus;
            armBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).armorBonus;
            armBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).armorBonus;
            armBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).armorBonus;
            armBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).armorBonus;
            return armBonuses;
        }
        public int CalcMaxDexBonus(Player pc)
        {
            int armMaxDexBonuses = 99;
            int mdb = mod.getItemByResRefForInfo(pc.BodyRefs.resref).maxDexBonus;
            if (mdb < armMaxDexBonuses) { armMaxDexBonuses = mdb; }
            mdb = mod.getItemByResRefForInfo(pc.MainHandRefs.resref).maxDexBonus;
            if (mdb < armMaxDexBonuses) { armMaxDexBonuses = mdb; }
            mdb = mod.getItemByResRefForInfo(pc.OffHandRefs.resref).maxDexBonus;
            if (mdb < armMaxDexBonuses) { armMaxDexBonuses = mdb; }
            mdb = mod.getItemByResRefForInfo(pc.RingRefs.resref).maxDexBonus;
            if (mdb < armMaxDexBonuses) { armMaxDexBonuses = mdb; }
            mdb = mod.getItemByResRefForInfo(pc.HeadRefs.resref).maxDexBonus;
            if (mdb < armMaxDexBonuses) { armMaxDexBonuses = mdb; }
            mdb = mod.getItemByResRefForInfo(pc.GlovesRefs.resref).maxDexBonus;
            if (mdb < armMaxDexBonuses) { armMaxDexBonuses = mdb; }
            mdb = mod.getItemByResRefForInfo(pc.NeckRefs.resref).maxDexBonus;
            if (mdb < armMaxDexBonuses) { armMaxDexBonuses = mdb; }
            mdb = mod.getItemByResRefForInfo(pc.FeetRefs.resref).maxDexBonus;
            if (mdb < armMaxDexBonuses) { armMaxDexBonuses = mdb; }
            mdb = mod.getItemByResRefForInfo(pc.Ring2Refs.resref).maxDexBonus;
            if (mdb < armMaxDexBonuses) { armMaxDexBonuses = mdb; }
            return armMaxDexBonuses;
        }
        public int CalcMovementBonuses(Player pc)
        {
            int moveBonuses = 0;
            moveBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).MovementPointModifier;
            moveBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).MovementPointModifier;
            moveBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).MovementPointModifier;
            moveBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).MovementPointModifier;
            moveBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).MovementPointModifier;
            moveBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).MovementPointModifier;
            moveBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).MovementPointModifier;
            moveBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).MovementPointModifier;
            moveBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).MovementPointModifier;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        moveBonuses += ef.modifyMoveDistance;
                    }
                    else
                    {
                        if ((ef.modifyMoveDistance != 0) && (ef.modifyMoveDistance > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyMoveDistance;
                        }
                    }
                }
            }
            if (highestNonStackable > -99) { moveBonuses = highestNonStackable; }
            return moveBonuses;
        }

        public int CalcNumberOfAttacks(Player pc)
         {
            int moveBonuses = 0;
            moveBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).additionalAttacks;
            moveBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).additionalAttacks;
            moveBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).additionalAttacks;
            moveBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).additionalAttacks;
            moveBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).additionalAttacks;
            moveBonuses += mod.getItemByResRefForInfo(pc.GlovesRefs.resref).additionalAttacks;
            moveBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).additionalAttacks;
            moveBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).additionalAttacks;
            moveBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).additionalAttacks;

            if (isMeleeAttack(pc))  
             {  
                 return CalcNumberOfMeleeAttacks(pc) + moveBonuses;  
             }  
             else  
             {  
                 return CalcNumberOfRangedAttacks(pc) + moveBonuses;  
             }  
         }

        public int CalcNumberOfMeleeAttacks(Player pc)
         {  
             int numOfAdditionalPositiveMeleeAttacks = 0;  
             int numOfAdditionalPositiveStackableMeleeAttacks = 0;  
             int numOfAdditionalNegativeMeleeAttacks = 0;  
             int numOfAdditionalNegativeStackableMeleeAttacks = 0;  
             /*
             //go through all traits and see if has passive rapidshot type trait, use largest, not cumulative  
             foreach (string taTag in pc.knownTraitsTags)  
             {  
                 Trait ta = mod.getTraitByTag(taTag);  
                 foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
                 {  
                     Effect ef = mod.getEffectByTag(efTag.tag);  
                     //replace non-stackable positive with highest value  
                     if ((ef.modifyNumberOfMeleeAttacks > numOfAdditionalPositiveMeleeAttacks) && (ef.isPermanent) && (!ef.isStackableEffect))  
                     {  
                         numOfAdditionalPositiveMeleeAttacks = ef.modifyNumberOfMeleeAttacks;  
                     }  
                     //replace non-stackable negative with lowest value  
                     if ((ef.modifyNumberOfMeleeAttacks<numOfAdditionalNegativeMeleeAttacks) && (ef.isPermanent) && (!ef.isStackableEffect))  
                     {  
                         numOfAdditionalNegativeMeleeAttacks = ef.modifyNumberOfMeleeAttacks;  
                     }  
                     //if isStackable positive then pile on  
                     if ((ef.modifyNumberOfMeleeAttacks > 0) && (ef.isPermanent) && (ef.isStackableEffect))  
                     {  
                         numOfAdditionalPositiveStackableMeleeAttacks += ef.modifyNumberOfMeleeAttacks;  
                     }  
                     //if isStackable negative then pile on  
                     if ((ef.modifyNumberOfMeleeAttacks< 0) && (ef.isPermanent) && (ef.isStackableEffect))  
                     {  
                         numOfAdditionalNegativeStackableMeleeAttacks += ef.modifyNumberOfMeleeAttacks;  
                     }  
                 }  
             }
             */  
             //go through each effect and see if has a buff type like rapidshot, use largest, not cumulative  
             foreach (Effect ef in pc.effectsList)  
             {
                if (isPassiveTraitApplied(ef, pc))
                {
                    //replace non-stackable positive with highest value  
                    if ((ef.modifyNumberOfMeleeAttacks > numOfAdditionalPositiveMeleeAttacks) && (!ef.isStackableEffect))
                    {
                        numOfAdditionalPositiveMeleeAttacks = ef.modifyNumberOfMeleeAttacks;
                    }
                    //replace non-stackable negative with lowest value  
                    if ((ef.modifyNumberOfMeleeAttacks < numOfAdditionalNegativeMeleeAttacks) && (!ef.isStackableEffect))
                    {
                        numOfAdditionalNegativeMeleeAttacks = ef.modifyNumberOfMeleeAttacks;
                    }
                    //if isStackable positive then pile on  
                    if ((ef.modifyNumberOfMeleeAttacks > 0) && (ef.isStackableEffect))
                    {
                        numOfAdditionalPositiveStackableMeleeAttacks += ef.modifyNumberOfMeleeAttacks;
                    }
                    //if isStackable negative then pile on  
                    if ((ef.modifyNumberOfMeleeAttacks < 0) && (ef.isStackableEffect))
                    {
                        numOfAdditionalNegativeStackableMeleeAttacks += ef.modifyNumberOfMeleeAttacks;
                    }
                }  
             }  
   
             int numOfPos = 0;  
             int numOfNeg = 0;  
             //check to see if stackable is greater than non-stackable and combine the highest positive and negative effect  
             if (numOfAdditionalPositiveMeleeAttacks > numOfAdditionalPositiveStackableMeleeAttacks)  
             {  
                 numOfPos = numOfAdditionalPositiveMeleeAttacks;  
             }  
             else  
             {  
                 numOfPos = numOfAdditionalPositiveStackableMeleeAttacks;  
             }  
             if (numOfAdditionalNegativeMeleeAttacks < numOfAdditionalNegativeStackableMeleeAttacks)  
             {  
                 numOfNeg = numOfAdditionalNegativeMeleeAttacks;  
             }  
             else  
             {  
                 numOfNeg = numOfAdditionalNegativeStackableMeleeAttacks;  
             }  
   
             int numOfAdditionalAttacks = numOfPos + numOfNeg;  
             if (numOfAdditionalAttacks != 0)  
             {  
                 return numOfAdditionalAttacks + 1;  
             }  
             else if (gv.sf.hasTrait(pc, "twoAttack"))  
             {  
                 return 2;  
             }  
             else  
             {
                return 1;  
             }  
       }

        public int CalcNumberOfRangedAttacks(Player pc)
         {  
             int numOfAdditionalPositiveRangedAttacks = 0;  
             int numOfAdditionalPositiveStackableRangedAttacks = 0;  
             int numOfAdditionalNegativeRangedAttacks = 0;  
             int numOfAdditionalNegativeStackableRangedAttacks = 0;  
        /*
 5021 +            //go through all traits and see if has passive rapidshot type trait, use largest, not cumulative  
 5022 +            foreach (string taTag in pc.knownTraitsTags)  
 5023 +            {  
 5024 +                Trait ta = mod.getTraitByTag(taTag);  
 5025 +                foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
 5026 +                {  
 5027 +                    Effect ef = mod.getEffectByTag(efTag.tag);  
 5028 +                    //replace non-stackable positive with highest value  
 5029 +                    if ((ef.modifyNumberOfRangedAttacks > numOfAdditionalPositiveRangedAttacks) && (ta.isPassive) && (!ef.isStackableEffect))  
 5030 +                    {  
 5031 +                        numOfAdditionalPositiveRangedAttacks = ef.modifyNumberOfRangedAttacks;  
 5032 +                    }  
 5033 +                    //replace non-stackable negative with lowest value  
 5034 +                    if ((ef.modifyNumberOfRangedAttacks<numOfAdditionalNegativeRangedAttacks) && (ta.isPassive) && (!ef.isStackableEffect))  
 5035 +                    {  
 5036 +                        numOfAdditionalNegativeRangedAttacks = ef.modifyNumberOfRangedAttacks;  
 5037 +                    }  
 5038 +                    //if isStackable positive then pile on  
 5039 +                    if ((ef.modifyNumberOfRangedAttacks > 0) && (ta.isPassive) && (ef.isStackableEffect))  
 5040 +                    {  
 5041 +                        numOfAdditionalPositiveStackableRangedAttacks += ef.modifyNumberOfRangedAttacks;  
 5042 +                    }  
 5043 +                    //if isStackable negative then pile on  
 5044 +                    if ((ef.modifyNumberOfRangedAttacks< 0) && (ta.isPassive) && (ef.isStackableEffect))  
 5045 +                    {  
 5046 +                        numOfAdditionalNegativeStackableRangedAttacks += ef.modifyNumberOfRangedAttacks;  
 5047 +                    }  
 5048 +                }  
 5049 +            }
 */  
             //go through each effect and see if has a buff type like rapidshot, use largest, not cumulative  
             foreach (Effect ef in pc.effectsList)  
             {
                if (isPassiveTraitApplied(ef, pc))
                {
                    //replace non-stackable positive with highest value  
                    if ((ef.modifyNumberOfRangedAttacks > numOfAdditionalPositiveRangedAttacks) && (!ef.isStackableEffect))
                    {
                        numOfAdditionalPositiveRangedAttacks = ef.modifyNumberOfRangedAttacks;
                    }
                    //replace non-stackable negative with lowest value  
                    if ((ef.modifyNumberOfRangedAttacks < numOfAdditionalNegativeRangedAttacks) && (!ef.isStackableEffect))
                    {
                        numOfAdditionalNegativeRangedAttacks = ef.modifyNumberOfRangedAttacks;
                    }
                    //if isStackable positive then pile on  
                    if ((ef.modifyNumberOfRangedAttacks > 0) && (ef.isStackableEffect))
                    {
                        numOfAdditionalPositiveStackableRangedAttacks += ef.modifyNumberOfRangedAttacks;
                    }
                    //if isStackable negative then pile on  
                    if ((ef.modifyNumberOfRangedAttacks < 0) && (ef.isStackableEffect))
                    {
                        numOfAdditionalNegativeStackableRangedAttacks += ef.modifyNumberOfRangedAttacks;
                    }
                }  
             }  
   
            int numOfPos = 0;  
             int numOfNeg = 0;  
             //check to see if stackable is greater than non-stackable and combine the highest positive and negative effect  
             if (numOfAdditionalPositiveRangedAttacks > numOfAdditionalPositiveStackableRangedAttacks)  
             {  
                 numOfPos = numOfAdditionalPositiveRangedAttacks;  
             }  
             else  
             {  
                 numOfPos = numOfAdditionalPositiveStackableRangedAttacks;  
             }  
             if (numOfAdditionalNegativeRangedAttacks < numOfAdditionalNegativeStackableRangedAttacks)  
             {  
                 numOfNeg = numOfAdditionalNegativeRangedAttacks;  
             }  
             else  
             {  
                 numOfNeg = numOfAdditionalNegativeStackableRangedAttacks;  
             }  
   
             int numOfAdditionalAttacks = numOfPos + numOfNeg;  
             if (numOfAdditionalAttacks != 0)  
             {  
                 return numOfAdditionalAttacks + 1;  
             }  
             else if (gv.sf.hasTrait(pc, "rapidshot2"))  
             {  
                 return 3;  
             }  
             else if (gv.sf.hasTrait(pc, "rapidshot"))  
             {  
                 return 2;  
             }  
             else  
             {  
                 return 1;  
             }  
         }

        public bool isMeleeAttack(Player pc)
         {  
            if ((mod.getItemByResRefForInfo(pc.MainHandRefs.resref).category.Equals("Melee"))  
                    || (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).name.Equals("none"))  
                     || (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).name.Equals("none")))  
             {  
                 return true;  
             }  
             return false;  
         }

        public int CalcNumberOfCleaveAttackTargets(Player pc)
        {  
             int cleaveAttTargets = 0;  
 /*
        5126 +            //go through all traits and see if has passive cleave type trait, use largest, not cumulative  
 5127 +            foreach (string taTag in pc.knownTraitsTags)  
 5128 +            {  
 5129 +                Trait ta = mod.getTraitByTag(taTag);  
 5130 +                foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
 5131 +                {  
 5132 +                    Effect ef = mod.getEffectByTag(efTag.tag);  
 5133 +                    if ((ef.modifyNumberOfEnemiesAttackedOnCleave > cleaveAttTargets) && (ta.isPassive))  
 5134 +                    {  
 5135 +                        cleaveAttTargets = ef.modifyNumberOfEnemiesAttackedOnCleave;  
 5136 +                    }  
 5137 +                }  
 5138 +            }
 */  
             //go through each effect and see if has a buff type like cleave, use largest, not cumulative  
             foreach (Effect ef in pc.effectsList)  
             {  
                 if (ef.modifyNumberOfEnemiesAttackedOnCleave > cleaveAttTargets)  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        cleaveAttTargets = ef.modifyNumberOfEnemiesAttackedOnCleave;
                    }
                 }  
             }  
             if (cleaveAttTargets > 0)  
             {  
                 return cleaveAttTargets;  
             }  
             else if (gv.sf.hasTrait(pc, "cleave"))  
             {  
                 return 1;  
             }  
             else  
             {  
                 return 0;  
             }  
         }

        public int CalcNumberOfSweepAttackTargets(Player pc)
        {  
             int sweepAttTargets = 0;  
 /*
 5163 +            //go through all traits and see if has passive sweep type trait, use largest, not cumulative  
 5164 +            foreach (string taTag in pc.knownTraitsTags)  
 5165 +            {  
 5166 +                Trait ta = mod.getTraitByTag(taTag);  
 5167 +                foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
 5168 +                {  
 5169 +                    Effect ef = mod.getEffectByTag(efTag.tag);  
 5170 +                    if ((ef.modifyNumberOfEnemiesAttackedOnSweepAttack > sweepAttTargets) && (ta.isPassive))  
 5171 +                    {  
 5172 +                        sweepAttTargets = ef.modifyNumberOfEnemiesAttackedOnSweepAttack;  
 5173 +                    }  
 5174 +                }  
 5175 +            }
 */  
             //go through each effect and see if has a buff/active type like sweep, use largest, not cumulative  
             foreach (Effect ef in pc.effectsList)  
             {  
                 if (ef.modifyNumberOfEnemiesAttackedOnSweepAttack > sweepAttTargets)  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        sweepAttTargets = ef.modifyNumberOfEnemiesAttackedOnSweepAttack;
                    }
                 }  
             }  
             if (sweepAttTargets > 0)  
             {  
                 return sweepAttTargets;  
             }              
             else  
             {  
                 return 0;  
             }  
         }
        public int CalcPcMeleeAttackAttributeModifier(Player pc)
        {  
             int modifier = (pc.strength - 10) / 2;  
             bool useDexModifier = false;  
            /*
 5197 +            //go through all traits and see if has passive criticalstrike type trait  
 5198 +            foreach (string taTag in pc.knownTraitsTags)  
 5199 +            {  
 5200 +                Trait ta = mod.getTraitByTag(taTag);  
 5201 +                foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
 5202 +                {  
 5203 +                    Effect ef = mod.getEffectByTag(efTag.tag);  
 5204 +                    if ((ef.useDexterityForMeleeAttackModifierIfGreaterThanStrength) && (ta.isPassive))  
 5205 +                    {  
 5206 +                        useDexModifier = true;  
 5207 +                    }  
 5208 +                }  
 5209 +            }
 */
   
             //go through each effect and see if has a buff type like criticalstrike  
             foreach (Effect ef in pc.effectsList)  
             {  
                 if (ef.useDexterityForMeleeAttackModifierIfGreaterThanStrength)  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        useDexModifier = true;
                    }
                 }  
             }  
             //if has critical strike trait use dexterity for attack modifier in melee if greater than strength modifier  
             if ((pc.knownTraitsTags.Contains("criticalstrike")) || (useDexModifier))  
             {  
                int modifierDex = (pc.dexterity - 10) / 2;  
                 if (modifierDex > modifier)  
                 {  
                     modifier = (pc.dexterity - 10) / 2;  
                 }  
             }

            //******************************************************
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        modifier += ef.babModifierForMeleeAttack;
                    }
                    else
                    {
                        if ((ef.babModifierForMeleeAttack != 0) && (ef.babModifierForMeleeAttack > highestNonStackable))
                        {
                            highestNonStackable = ef.babModifierForMeleeAttack;
                        }
                    }
                }
            }
            if (highestNonStackable > modifier) { modifier = highestNonStackable; }


            //*******************************************************

            return modifier;  
         }

        public int CalcPcMeleeDamageAttributeModifier(Player pc)
        {  
             int damModifier = (pc.strength - 10) / 2;  
             bool useDexModifier = false;
             Item it = gv.mod.getItemByResRefForInfo(pc.MainHandRefs.resref);
             if (damModifier > it.maxStrengthBonusAllowedForWeapon)
             {
                damModifier = it.maxStrengthBonusAllowedForWeapon;
             }

            /*
 5233 +            //go through all traits and see if has passive criticalstrike type trait  
 5234 +            foreach (string taTag in pc.knownTraitsTags)  
 5235 +            {  
 5236 +                Trait ta = mod.getTraitByTag(taTag);  
 5237 +                foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
 5238 +                {  
 5239 +                    Effect ef = mod.getEffectByTag(efTag.tag);  
 5240 +                    if ((ef.useDexterityForMeleeDamageModifierIfGreaterThanStrength) && (ta.isPassive))  
 5241 +                    {  
 5242 +                        useDexModifier = true;  
 5243 +                    }  
 5244 +                }  
 5245 +            } 
 */
            //go through each effect and see if has a buff type like criticalstrike  
            foreach (Effect ef in pc.effectsList)  
             {  
                 if (ef.useDexterityForMeleeDamageModifierIfGreaterThanStrength)  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        useDexModifier = true;
                    }
                 }  
             }  
            //if has critical strike trait use dexterity for damage modifier in melee if greater than strength modifier  
            if ((pc.knownTraitsTags.Contains("criticalstrike")) || (useDexModifier))  
            {  
                 int damModifierDex = (pc.dexterity - 10) / 2;  
                 if (damModifierDex > damModifier)  
                 {  
                     damModifier = damModifierDex;  
                 }  
             }

            //**********************************************************
            int highestNonStackable = -99;
            int additionalDamModifier = 0;

            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                       additionalDamModifier += ef.damageModifierForMeleeAttack;
                    }
                    else
                    {
                        if ((ef.damageModifierForMeleeAttack != 0) && (ef.damageModifierForMeleeAttack > highestNonStackable))
                        {
                            highestNonStackable = ef.damageModifierForMeleeAttack;
                        }
                    }
                }
            }
            if (highestNonStackable > additionalDamModifier) { additionalDamModifier = highestNonStackable; }

            //********************************************************
            return damModifier + additionalDamModifier;  
         }

        public bool canNegateAdjacentAttackPenalty(Player pc)
         {  
             bool cancelAttackPenalty = false;  
            /*
             //go through all traits and see if has passive pointblankshot type trait  
             foreach (string taTag in pc.knownTraitsTags)  
             {  
                 Trait ta = mod.getTraitByTag(taTag);  
                 foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
                  {  
                     Effect ef = mod.getEffectByTag(efTag.tag);  
                     if ((ef.negateAttackPenaltyForAdjacentEnemyWithRangedAttack) && (ta.isPassive))  
                     {  
                         cancelAttackPenalty = true;  
                     }  
                 }  
             }
             */  
             //go through each effect and see if has a buff type like pointblankshot  
             foreach (Effect ef in pc.effectsList)  
             {  
                 if (ef.negateAttackPenaltyForAdjacentEnemyWithRangedAttack)  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        cancelAttackPenalty = true;
                    }
                 }  
             }  
             if ((gv.sf.hasTrait(pc, "pointblankshot")) || (cancelAttackPenalty))  
             {  
                 return true;  
             }  
             return false;  
         }

        public int CalcPcRangedAttackModifier(Player pc)
         {  
             int preciseShotAdder = 0;
            //string label = "";
            /*  
             //go through all traits and see if has passive preciseshot type trait, use largest, not cumulative  
             foreach (string taTag in pc.knownTraitsTags)  
             {  
                 Trait ta = mod.getTraitByTag(taTag);  
                 foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
 5304 +                {  
 5305 +                    Effect ef = mod.getEffectByTag(efTag.tag);  
 5306 +                    if ((ef.babModifierForRangedAttack > preciseShotAdder) && (ta.isPassive))  
 5307 +                    {  
 5308 +                        preciseShotAdder = ef.babModifierForRangedAttack;  
 5309 +                        label = ta.name;  
 5310 +                    }  
 5311 +                }  
 5312 +            }
 */
            //go through each effect and see if has a buff type like preciseshot, use largest, not cumulative  
            /*
            foreach (Effect ef in pc.effectsList)  
             {  
                 if (ef.babModifierForRangedAttack > preciseShotAdder)  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        preciseShotAdder = ef.babModifierForRangedAttack;
                        label = ef.name;
                    }
                 }  
             }
             */

            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        preciseShotAdder += ef.babModifierForRangedAttack;
                    }
                    else
                    {
                        if ((ef.babModifierForRangedAttack != 0) && (ef.babModifierForRangedAttack > highestNonStackable))
                        {
                            highestNonStackable = ef.babModifierForRangedAttack;
                        }
                    }
                }
            }
            if (highestNonStackable > preciseShotAdder) { preciseShotAdder = highestNonStackable; }

            return preciseShotAdder;  
         }

        //not used
        public int CalcPcMeleeDamageModifier(Player pc)
         {  
             int adder = 0;
            //go through all traits and see if has passive preciseshot type trait, use largest, not cumulative  
            /*
5328 +            foreach (string taTag in pc.knownTraitsTags)  
5329 +            {  
5330 +                Trait ta = mod.getTraitByTag(taTag);  
5331 +                foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
5332 +                {  
5333 +                    Effect ef = mod.getEffectByTag(efTag.tag);  
5334 +                    if ((ef.damageModifierForMeleeAttack > adder) && (ta.isPassive))  
5335 +                    {  
5336 +                        adder = ef.damageModifierForMeleeAttack;  
5337 +                    }  
5338 +                }  
5339 +            }
*/
            //go through each effect and see if has a buff type like preciseshot, use largest, not cumulative  
            /*foreach (Effect ef in pc.effectsList)  
            {  
                if (ef.damageModifierForMeleeAttack > adder)  
                {
                   if (isPassiveTraitApplied(ef, pc))
                   {
                       adder = ef.damageModifierForMeleeAttack;
                   }
                }  
            } 
            */
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        adder += ef.damageModifierForMeleeAttack;
                    }
                    else
                    {
                        if ((ef.damageModifierForMeleeAttack != 0) && (ef.damageModifierForMeleeAttack > highestNonStackable))
                        {
                            highestNonStackable = ef.damageModifierForMeleeAttack;
                        }
                    }
                }
            }
            if (highestNonStackable > adder) { adder = highestNonStackable; }

            return adder;  
         }

        public int CalcPcRangedDamageModifier(Player pc)
         {  
            int adder = 0;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        adder += ef.damageModifierForRangedAttack;
                    }
                    else
                    {
                        if ((ef.damageModifierForRangedAttack != 0) && (ef.damageModifierForRangedAttack > highestNonStackable))
                        {
                            highestNonStackable = ef.damageModifierForRangedAttack;
                        }
                    }
                }
            }
            if (highestNonStackable > adder) { adder = highestNonStackable; }
            return adder;  
         }

        public int CalcPcHpRegenInCombat(Player pc)
         {
            int adder = 0;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        adder += ef.modifyHpInCombat;
                    }
                    else
                    {
                        if ((ef.modifyHpInCombat != 0) && (ef.modifyHpInCombat > highestNonStackable))
                        {
                            highestNonStackable = ef.modifyHpInCombat;
                        }
                    }
                }
            }
            if (highestNonStackable > adder) { adder = highestNonStackable; }
            return adder;
        }

        public int CalcPcSpRegenInCombat(Player pc)
         {
            int adder = 0;
            int highestNonStackable = -99;
            foreach (Effect ef in pc.effectsList)
            {
                if (isPassiveTraitApplied(ef, pc))
                {
                    if (ef.isStackableEffect)
                    {
                        adder += ef.modifySpInCombat;
                    }
                    else
                    {
                        if ((ef.modifySpInCombat != 0) && (ef.modifySpInCombat > highestNonStackable))
                        {
                            highestNonStackable = ef.modifySpInCombat;
                        }
                    }
                }
            }
            if (highestNonStackable > adder) { adder = highestNonStackable; }
            return adder;
        }

        public int CalcCrtSpRegenInCombat(Creature crt)
        {
            int adder = 0;
            int highestNonStackable = -99;
            foreach (Effect ef in crt.cr_effectsList)
            {
                    if (ef.isStackableEffect)
                    {
                        adder += ef.modifySpInCombat;
                    }
                    else
                    {
                        if ((ef.modifySpInCombat != 0) && (ef.modifySpInCombat > highestNonStackable))
                        {
                            highestNonStackable = ef.modifySpInCombat;
                        }
                    }
            }
            if (highestNonStackable > adder) { adder = highestNonStackable; }
            return adder;
        }

        public int CalcCrtHpRegenInCombat(Creature crt)
        {
            int adder = 0;
            int highestNonStackable = -99;
            foreach (Effect ef in crt.cr_effectsList)
            {
                if (ef.isStackableEffect)
                {
                    adder += ef.modifyHpInCombat;
                }
                else
                {
                    if ((ef.modifyHpInCombat != 0) && (ef.modifyHpInCombat > highestNonStackable))
                    {
                        highestNonStackable = ef.modifyHpInCombat;
                    }
                }
            }
            if (highestNonStackable > adder) { adder = highestNonStackable; }
            return adder;
        }

        public void RunAllItemWhileEquippedScripts(Player pc)
        {
            try
            {
                if (!mod.getItemByResRefForInfo(pc.BodyRefs.resref).onWhileEquipped.Equals("none"))
                {
                    gv.cc.doScriptBasedOnFilename(mod.getItemByResRefForInfo(pc.BodyRefs.resref).onWhileEquipped, "", "", "", "");
                }
                if (!mod.getItemByResRefForInfo(pc.MainHandRefs.resref).onWhileEquipped.Equals("none"))
                {
                    gv.cc.doScriptBasedOnFilename(mod.getItemByResRefForInfo(pc.MainHandRefs.resref).onWhileEquipped, "", "", "", "");
                }
                if (!mod.getItemByResRefForInfo(pc.OffHandRefs.resref).onWhileEquipped.Equals("none"))
                {
                    gv.cc.doScriptBasedOnFilename(mod.getItemByResRefForInfo(pc.OffHandRefs.resref).onWhileEquipped, "", "", "", "");
                }
                if (!mod.getItemByResRefForInfo(pc.RingRefs.resref).onWhileEquipped.Equals("none"))
                {
                    gv.cc.doScriptBasedOnFilename(mod.getItemByResRefForInfo(pc.RingRefs.resref).onWhileEquipped, "", "", "", "");
                }
                if (!mod.getItemByResRefForInfo(pc.HeadRefs.resref).onWhileEquipped.Equals("none"))
                {
                    gv.cc.doScriptBasedOnFilename(mod.getItemByResRefForInfo(pc.HeadRefs.resref).onWhileEquipped, "", "", "", "");
                }
                if (!mod.getItemByResRefForInfo(pc.GlovesRefs.resref).onWhileEquipped.Equals("none"))
                {
                    gv.cc.doScriptBasedOnFilename(mod.getItemByResRefForInfo(pc.GlovesRefs.resref).onWhileEquipped, "", "", "", "");
                }
                if (!mod.getItemByResRefForInfo(pc.NeckRefs.resref).onWhileEquipped.Equals("none"))
                {
                    gv.cc.doScriptBasedOnFilename(mod.getItemByResRefForInfo(pc.NeckRefs.resref).onWhileEquipped, "", "", "", "");
                }
                if (!mod.getItemByResRefForInfo(pc.FeetRefs.resref).onWhileEquipped.Equals("none"))
                {
                    gv.cc.doScriptBasedOnFilename(mod.getItemByResRefForInfo(pc.FeetRefs.resref).onWhileEquipped, "", "", "", "");
                }
                if (!mod.getItemByResRefForInfo(pc.Ring2Refs.resref).onWhileEquipped.Equals("none"))
                {
                    gv.cc.doScriptBasedOnFilename(mod.getItemByResRefForInfo(pc.Ring2Refs.resref).onWhileEquipped, "", "", "", "");
                }
                if (!mod.getItemByResRefForInfo(pc.AmmoRefs.resref).onWhileEquipped.Equals("none"))
                {
                    gv.cc.doScriptBasedOnFilename(mod.getItemByResRefForInfo(pc.AmmoRefs.resref).onWhileEquipped, "", "", "", "");
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }
        }
        public bool hasTrait(Player pc, string tag)
        {
            return pc.knownTraitsTags.Contains(tag);
        }

        //DEFAULT SCRIPTS
        public void dsWorldTime()
        {
            //world time is in minues
            //28 day a month
            //12 months a year (48 weeks a year)
            gv.mod.timeInThisYear = (gv.mod.WorldTime) % 483840;
           
            //note: our ranges strat at 0 here, while our usual displayed counting starts at 1
            gv.mod.currentYear = (gv.mod.WorldTime) / 483840;
            decimal.Round(gv.mod.currentYear, 0);
            gv.mod.currentMonth = ((gv.mod.timeInThisYear) / 40320);
            decimal.Round(gv.mod.currentMonth, 0);
            gv.mod.currentDay = ((gv.mod.timeInThisYear) / 1440);
            decimal.Round(gv.mod.currentDay, 0);
            gv.mod.currentWeekDay = (gv.mod.currentDay % 7);
            gv.mod.currentMonthDay = (gv.mod.currentDay % 28);

            //XXX
            if (gv.mod.currentMonth == 0)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfFirstMonth;
            }
            else if (gv.mod.currentMonth == 1)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfSecondMonth;
            }
            else if (gv.mod.currentMonth == 2)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfThirdMonth;
            }
            else if (gv.mod.currentMonth == 3)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfFourthMonth;
            }
            else if (gv.mod.currentMonth == 4)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfFifthMonth;
            }
            else if (gv.mod.currentMonth == 5)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfSixthMonth;
            }
            else if (gv.mod.currentMonth == 6)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfSeventhMonth;
            }
            else if (gv.mod.currentMonth == 7)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfEighthMonth;
            }
            else if (gv.mod.currentMonth == 8)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfNinthMonth;
            }
            else if (gv.mod.currentMonth == 9)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfTenthMonth;
            }
            else if (gv.mod.currentMonth == 10)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfEleventhMonth;
            }
            else if (gv.mod.currentMonth == 11)
            {
                gv.mod.monthNameToDisplay = gv.mod.nameOfTwelfthMonth;
            }

            gv.mod.monthDayCounterNumberToDisplay = (gv.mod.currentMonthDay + 1).ToString();
            if (gv.mod.currentMonthDay == 0)
            {
                gv.mod.monthDayCounterAddendumToDisplay = "st";
            }
            else if (gv.mod.currentMonthDay == 1)
            {
                gv.mod.monthDayCounterAddendumToDisplay = "nd";
            }
            else if (gv.mod.currentMonthDay == 2)
            {
                gv.mod.monthDayCounterAddendumToDisplay = "rd";
            }
            else if (gv.mod.currentMonthDay == 20)
            {
                gv.mod.monthDayCounterAddendumToDisplay = "st";
            }
            else if (gv.mod.currentMonthDay == 21)
            {
                gv.mod.monthDayCounterAddendumToDisplay = "nd";
            }
            else if (gv.mod.currentMonthDay == 22)
            {
                gv.mod.monthDayCounterAddendumToDisplay = "rd";
            }
            else
            {
                gv.mod.monthDayCounterAddendumToDisplay = "th";
            }

            if (gv.mod.currentWeekDay == 0)
            {
                gv.mod.weekDayNameToDisplay = gv.mod.nameOfFirstDayOfTheWeek;
            }
            else if (gv.mod.currentWeekDay == 1)
            {
                gv.mod.weekDayNameToDisplay = gv.mod.nameOfSecondDayOfTheWeek;
            }
            else if (gv.mod.currentWeekDay == 2)
            {
                gv.mod.weekDayNameToDisplay = gv.mod.nameOfThirdDayOfTheWeek;
            }
            else if (gv.mod.currentWeekDay == 3)
            {
                gv.mod.weekDayNameToDisplay = gv.mod.nameOfFourthDayOfTheWeek;
            }
            else if (gv.mod.currentWeekDay == 4)
            {
                gv.mod.weekDayNameToDisplay = gv.mod.nameOfFifthDayOfTheWeek;
            }
            else if (gv.mod.currentWeekDay == 5)
            {
                gv.mod.weekDayNameToDisplay = gv.mod.nameOfSixthDayOfTheWeek;
            }
            else if (gv.mod.currentWeekDay == 6)
            {
                gv.mod.weekDayNameToDisplay = gv.mod.nameOfSeventhDayOfTheWeek;
            }

            //XXX

            //assuming 28 days in 12 Months, ie 336 days a year
            //notation example: 13:17, Tuesday, 9th of March 1213

            /*
            public string weekDayNameToDisplay = "";
            public string monthDayCounterNumberToDisplay = "";
            public string monthDayCounterAddendumToDisplay = "";
            public string monthNameToDisplay = "";

            public string nameOfFirstDayOfTheWeek = "Monday";
            public string nameOfSecondDayOfTheWeek = "Tuesday";
            public string nameOfThirdDayOfTheWeek = "Wednesday";
            public string nameOfFourthDayOfTheWeek = "Thursday";
            public string nameOfFifthDayOfTheWeek = "Friday";
            public string nameOfSixthDayOfTheWeek = "Saturday";
            public string nameOfSeventhDayOfTheWeek = "Sunday";

            public string nameOfFirstMonth = "";
            public string nameOfSecondMonth = "";
            public string nameOfThirdMonth = "";
            public string nameOfFourthMonth = "";
            public string nameOfFifthMonth = "";
            public string nameOfSixthMonth = "";
            public string nameOfSeventhMonth = "";
            public string nameOfEighthMonth = "";
            public string nameOfNinthMonth = "";
            public string nameOfTenthMonth = "";
            public string nameOfElventhMonth = "";
            public string nameOfTwelfthMonth = "";

            public int currentYear = 0;
            //from 1 to 12
            public int currentMonth = 1;
            //from 1 to 336
            public int currentDay = 1;
            public int currentWeekDay = 1;
            public int currentMonthDay = 1;
            */

            //XXX
            mod.WorldTime += mod.currentArea.TimePerSquare;

            foreach (Prop p in gv.mod.propsWaitingForRespawn)
            {
                p.respawnTimeInMinutesPassedAlready += mod.currentArea.TimePerSquare;
            }

            foreach (Faction f in gv.mod.moduleFactionsList)
            {
                f.timePassedInThisInterval += mod.currentArea.TimePerSquare;
                if (f.timePassedInThisInterval >= (f.intervalOfFactionStrengthChangeInHours * 60))
                {
                    f.timePassedInThisInterval = 0;
                    f.strength += f.amountOfFactionStrengthChangePerInterval;
                }
                    if (f.strength >= f.factionStrengthRequiredForRank10)
                    {
                        f.rank = 10;
                    }
                    else if (f.strength >= f.factionStrengthRequiredForRank9)
                    {
                        f.rank = 9;
                    }
                    else if (f.strength >= f.factionStrengthRequiredForRank8)
                    {
                        f.rank = 8;
                    }
                    else if (f.strength >= f.factionStrengthRequiredForRank7)
                    {
                        f.rank = 7;
                    }
                    else if (f.strength >= f.factionStrengthRequiredForRank6)
                    {
                        f.rank = 6;
                    }
                    else if (f.strength >= f.factionStrengthRequiredForRank5)
                    {
                        f.rank = 5;
                    }
                    else if (f.strength >= f.factionStrengthRequiredForRank4)
                    {
                        f.rank = 4;
                    }
                    else if (f.strength >= f.factionStrengthRequiredForRank3)
                    {
                        f.rank = 3;
                    }
                    else if (f.strength >= f.factionStrengthRequiredForRank2)
                    {
                        f.rank = 2;
                    }
                    else
                    {
                        f.rank = 1;
                    }
                
            }

            //Code: Bleed to death at -20 hp
            //spCnt++;
            foreach (Player pc in mod.playerList)
            {
                //check to see if allow HP to regen
                if (mod.getPlayerClass(pc.classTag).hpRegenTimeNeeded > 0)
                {
                    if (pc.hp > -20 && pc.hp < pc.hpMax) //do not regen if truely dead
                    {
                        pc.hpRegenTimePassedCounter += mod.currentArea.TimePerSquare;
                        if (pc.hpRegenTimePassedCounter >= mod.getPlayerClass(pc.classTag).hpRegenTimeNeeded)
                        {
                            int hpGained = 0;
                            hpGained = pc.hpRegenTimePassedCounter / mod.getPlayerClass(pc.classTag).hpRegenTimeNeeded;
                            pc.hp += hpGained;
                            if (pc.hp > pc.hpMax)
                            {
                                pc.hp = pc.hpMax;
                            }
                            if ((pc.charStatus == "Dead") && (pc.hp > 0))
                            {
                                pc.charStatus = "Alive";
                            }
                            pc.hpRegenTimePassedCounter = 0;
                        }
                    }
                }
                //check to see if allow SP to regen
                if (mod.getPlayerClass(pc.classTag).spRegenTimeNeeded > 0)
                {
                    if (pc.hp > -20 && pc.sp < pc.spMax) //do not regen if truely dead
                    {
                        pc.spRegenTimePassedCounter += mod.currentArea.TimePerSquare;
                        if (pc.spRegenTimePassedCounter >= mod.getPlayerClass(pc.classTag).spRegenTimeNeeded)
                        {
                            int spGained = 0;
                            spGained = pc.spRegenTimePassedCounter / mod.getPlayerClass(pc.classTag).spRegenTimeNeeded;
                            pc.sp += spGained;
                            if (pc.sp > pc.spMax) { pc.sp = pc.spMax; }
                            pc.spRegenTimePassedCounter = 0;
                        }
                    }
                }
                //check all items to see if any are regeneration SP or HP type scripts that happen over intervals of time
                RunAllItemRegenerations(pc);

                if ((pc.hp <= 0) && (pc.hp > -20))
                {
                    pc.hp -= 1;
                    gv.cc.addLogText("<font color='red'>" + pc.name + " bleeds 1 HP, dead at -20 HP!" + "</font><BR>");
                    pc.charStatus = "Dead";
                    if (pc.hp <= -20)
                    {
                        pc.charStatus = "Dead";
                        gv.cc.addLogText("<font color='red'>" + pc.name + " has DIED!" + "</font><BR>");
                    }
                }
            }
        }
        public void RunAllItemRegenerations(Player pc)
        {
            try
            {
                if (mod.getItemByResRefForInfo(pc.BodyRefs.resref).minutesPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.BodyRefs.resref).minutesPerSpRegenOutsideCombat, pc.BodyRefs);
                }
                if (mod.getItemByResRefForInfo(pc.BodyRefs.resref).minutesPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.BodyRefs.resref).minutesPerHpRegenOutsideCombat, pc.BodyRefs);
                }

                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).minutesPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.MainHandRefs.resref).minutesPerSpRegenOutsideCombat, pc.MainHandRefs);
                }
                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).minutesPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.MainHandRefs.resref).minutesPerHpRegenOutsideCombat, pc.MainHandRefs);
                }

                if (mod.getItemByResRefForInfo(pc.OffHandRefs.resref).minutesPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.OffHandRefs.resref).minutesPerSpRegenOutsideCombat, pc.OffHandRefs);
                }
                if (mod.getItemByResRefForInfo(pc.OffHandRefs.resref).minutesPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.OffHandRefs.resref).minutesPerHpRegenOutsideCombat, pc.OffHandRefs);
                }

                if (mod.getItemByResRefForInfo(pc.RingRefs.resref).minutesPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.RingRefs.resref).minutesPerSpRegenOutsideCombat, pc.RingRefs);
                }
                if (mod.getItemByResRefForInfo(pc.RingRefs.resref).minutesPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.RingRefs.resref).minutesPerHpRegenOutsideCombat, pc.RingRefs);
                }

                if (mod.getItemByResRefForInfo(pc.HeadRefs.resref).minutesPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.HeadRefs.resref).minutesPerSpRegenOutsideCombat, pc.HeadRefs);
                }
                if (mod.getItemByResRefForInfo(pc.HeadRefs.resref).minutesPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.HeadRefs.resref).minutesPerHpRegenOutsideCombat, pc.HeadRefs);
                }

                if (mod.getItemByResRefForInfo(pc.GlovesRefs.resref).minutesPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.GlovesRefs.resref).minutesPerSpRegenOutsideCombat, pc.GlovesRefs);
                }
                if (mod.getItemByResRefForInfo(pc.GlovesRefs.resref).minutesPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.GlovesRefs.resref).minutesPerHpRegenOutsideCombat, pc.GlovesRefs);
                }


                if (mod.getItemByResRefForInfo(pc.NeckRefs.resref).minutesPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.NeckRefs.resref).minutesPerSpRegenOutsideCombat, pc.NeckRefs);
                }
                if (mod.getItemByResRefForInfo(pc.NeckRefs.resref).minutesPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.NeckRefs.resref).minutesPerHpRegenOutsideCombat, pc.NeckRefs);
                }

                if (mod.getItemByResRefForInfo(pc.FeetRefs.resref).minutesPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.FeetRefs.resref).minutesPerSpRegenOutsideCombat, pc.FeetRefs);
                }
                if (mod.getItemByResRefForInfo(pc.FeetRefs.resref).minutesPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.FeetRefs.resref).minutesPerHpRegenOutsideCombat, pc.FeetRefs);
                }

                if (mod.getItemByResRefForInfo(pc.Ring2Refs.resref).minutesPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.Ring2Refs.resref).minutesPerSpRegenOutsideCombat, pc.Ring2Refs);
                }
                if (mod.getItemByResRefForInfo(pc.Ring2Refs.resref).minutesPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.Ring2Refs.resref).minutesPerHpRegenOutsideCombat, pc.Ring2Refs);
                }

                if (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).minutesPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.AmmoRefs.resref).minutesPerSpRegenOutsideCombat, pc.AmmoRefs);
                }
                if (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).minutesPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.AmmoRefs.resref).minutesPerHpRegenOutsideCombat, pc.AmmoRefs);
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }
        }

        public void doRegenSp(Player pc, int minutesNeeded, ItemRefs itRef)
        {
            if ((minutesNeeded > 0) && (pc.hp > -20) && (pc.sp < pc.spMax))
            {
                itRef.spRegenTimer += mod.currentArea.TimePerSquare;
                if (itRef.spRegenTimer >= minutesNeeded)
                {
                    int spGained = 0;
                    spGained = itRef.spRegenTimer / minutesNeeded;
                    pc.sp += spGained;
                    if (pc.sp > pc.spMax) { pc.sp = pc.spMax; }
                    itRef.spRegenTimer = 0;
                }
            }
        }

        public void doRegenHp(Player pc, int minutesNeeded, ItemRefs itRef)
        {
            if ((minutesNeeded > 0) && (pc.hp > -20) && (pc.hp < pc.hpMax))
            {
                itRef.hpRegenTimer += mod.currentArea.TimePerSquare;
                if (itRef.hpRegenTimer >= minutesNeeded)
                {
                    int hpGained = 0;
                    hpGained = itRef.hpRegenTimer / minutesNeeded;
                    pc.hp += hpGained;
                    if (pc.hp > pc.hpMax) { pc.hp = pc.hpMax; }
                    itRef.hpRegenTimer = 0;
                }
            }
        }

        //ITEM ON USE
        public void itHeal(Player pc, Item it, int healAmount)
        {
            if (pc.hp <= -20)
            {
                MessageBox("Can't heal a dead character!");
                gv.cc.addLogText("<font color='red'>" + "Can't heal a dead character!" + "</font><BR>");
            }
            else
            {
                pc.hp += healAmount;
                if (pc.hp > pc.hpMax)
                {
                    pc.hp = pc.hpMax;
                }
                if (pc.hp > 0)
                {
                    pc.charStatus = "Alive";
                }
                MessageBox(pc.name + " gains " + healAmount + " HPs, now has " + pc.hp + "/" + pc.hpMax + "HPs");
                gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + healAmount + " HPs" + "</font><BR>");
            }
        }


        public void itForceRest()
        {
            if (gv.mod.useRationSystem)
            {
                if (gv.mod.numberOfRationsRemaining > 0)
                {
                    foreach (ItemRefs ir in gv.mod.partyInventoryRefsList)
                    {
                        if (ir.isRation)
                        {
                            ir.quantity--;
                            if (ir.quantity < 1)
                            {
                                gv.mod.partyInventoryRefsList.Remove(ir);
                            }
                            break;
                        }
                    }
                        
                    foreach (Player pc in mod.playerList)
                    {
                        if (pc.hp > -20)
                        {
                            pc.hp = pc.hpMax;
                            pc.sp = pc.spMax;
                        }
                    }
                    if (gv.mod.showRestMessagesInBox)
                    {
                        MessageBox(gv.mod.messageOnRest);
                    }
                    if (gv.mod.showRestMessagesInLog)
                    {
                        gv.cc.addLogText("<font color='lime'>" + gv.mod.messageOnRest + "</font><BR>");
                    }
                }
                else 
                { 
                    MessageBox("Party cannot rest without rations.");
                    gv.cc.addLogText("<font color='red'>" + "Party cannot rest without rations." + "</font><BR>");
                }
            }
            else
            {
                foreach (Player pc in mod.playerList)
                {
                    if (pc.hp > -20)
                    {
                        pc.hp = pc.hpMax;
                        pc.sp = pc.spMax;
                    }
                }
                if (gv.mod.showRestMessagesInBox)
                {
                    MessageBox(gv.mod.messageOnRest);
                }
                if (gv.mod.showRestMessagesInLog)
                {
                    gv.cc.addLogText("<font color='lime'>" + gv.mod.messageOnRest + "</font><BR>");
                }
            }
        }

        public void itForceRestNoRations()
        {  
             foreach (Player pc in mod.playerList)  
             {  
                 if (pc.hp > -20)  
                 {  
                     pc.hp = pc.hpMax;  
                     pc.sp = pc.spMax;  
                 }  
             }
            if (gv.mod.showRestMessagesInBox)
            {
                MessageBox(gv.mod.messageOnRest);
            }
            if (gv.mod.showRestMessagesInLog)
            {
                gv.cc.addLogText("<font color='lime'>" + gv.mod.messageOnRest + "</font><BR>");
            }
        }  


        public void itForceRestAndRaiseDead()
        {
            //MessageBox.Show("Heal Light Wounds");
            foreach (Player pc in mod.playerList)
            {
                pc.hp = pc.hpMax;
                pc.sp = pc.spMax;
                pc.charStatus = "Alive";
            }
            if (gv.mod.showRestMessagesInBox)
            {
                MessageBox(gv.mod.messageOnRestAndRaise);
            }
            if (gv.mod.showRestMessagesInLog)
            {
                gv.cc.addLogText("<font color='lime'>" + gv.mod.messageOnRestAndRaise + "</font><BR>");
            }
        }

        public void itForceRestAndRaiseDeadRequireRations()
        {
            if (gv.mod.useRationSystem)
            {
                if (gv.mod.numberOfRationsRemaining > 0)
                {
                    foreach (ItemRefs ir in gv.mod.partyInventoryRefsList)
                    {
                        if (ir.isRation)
                        {
                            ir.quantity--;
                            if (ir.quantity < 1)
                            {
                                gv.mod.partyInventoryRefsList.Remove(ir);
                            }
                            break;
                        }
                    }

                    foreach (Player pc in mod.playerList)
                    {
                        pc.hp = pc.hpMax;
                        pc.sp = pc.spMax;
                        pc.charStatus = "Alive";
                    }
                    if (gv.mod.showRestMessagesInBox)
                    {
                        MessageBox(gv.mod.messageOnRestAndRaise);
                    }
                    if (gv.mod.showRestMessagesInLog)
                    {
                        gv.cc.addLogText("<font color='lime'>" + gv.mod.messageOnRestAndRaise + "</font><BR>");
                    }
                }//not enough rations:
                else
                {
                    MessageBox("Party cannot rest without rations.");
                    gv.cc.addLogText("<font color='red'>" + "Party cannot rest without rations." + "</font><BR>");
                }

            }//no ration system:
            else
            {
                foreach (Player pc in mod.playerList)
                {
                    pc.hp = pc.hpMax;
                    pc.sp = pc.spMax;
                    pc.charStatus = "Alive";
                }
                if (gv.mod.showRestMessagesInBox)
                {
                    MessageBox(gv.mod.messageOnRestAndRaise);
                }
                if (gv.mod.showRestMessagesInLog)
                {
                    gv.cc.addLogText("<font color='lime'>" + gv.mod.messageOnRestAndRaise + "</font><BR>");
                }

            }

        }//method end

        public void itSpHeal(Player pc, Item it, int healAmount)
        {
            pc.sp += healAmount;
            if (pc.sp > pc.spMax)
            {
                pc.sp = pc.spMax;
            }
            MessageBox(pc.name + " regains " + healAmount + " SPs, now has " + pc.sp + "/" + pc.spMax + "SPs");
            gv.cc.addLogText("<font color='lime'>" + pc.name + " regains " + healAmount + " SPs" + "</font><BR>");
        }

        //Effects
        public void efGeneric(object src, Effect ef)
        {
            #region apply the modifiers for damage, heal, buffs and debuffs
            if (src is Creature)
            {
                Creature crt = (Creature)src;
                if ((ef.doDamage) && (ef.durationInUnits != ef.currentDurationInUnits))
                {
                    #region Do Damage
                    #region Get Resistances
                    float resist = 0;
                    /*
                    if (ef.damType.Equals("Normal")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueNormal / 100f)); }
                    else if (ef.damType.Equals("Acid")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueAcid / 100f)); }
                    else if (ef.damType.Equals("Cold")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueCold / 100f)); }
                    else if (ef.damType.Equals("Electricity")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueElectricity / 100f)); }
                    else if (ef.damType.Equals("Fire")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f)); }
                    else if (ef.damType.Equals("Magic")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueMagic / 100f)); }
                    else if (ef.damType.Equals("Poison")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValuePoison / 100f)); }
                    */
                    if (ef.damType.Equals("Normal")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueNormal() / 100f)); }
                    else if (ef.damType.Equals("Acid")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueAcid() / 100f)); }
                    else if (ef.damType.Equals("Cold")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueCold() / 100f)); }
                    else if (ef.damType.Equals("Electricity")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueElectricity() / 100f)); }
                    else if (ef.damType.Equals("Fire")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueFire() / 100f)); }
                    else if (ef.damType.Equals("Magic")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueMagic() / 100f)); }

                    #endregion
                    int damageTotal = 0;
                    #region Calculate Number of Attacks
                    //(for reference) NumOfAttacks: A of these attacks for every B levels after level C up to D attacks total                    
                    int numberOfAttacks = 0;
                    if (ef.damNumberOfAttacksForEveryNLevels == 0) //this effect is using a fixed amount of attacks
                    {
                        numberOfAttacks = ef.damNumberOfAttacks;
                    }
                    else //this effect is using a variable amount of attacks
                    {
                        //numberOfAttacks = (((classLevel - C) / B) + 1) * A;
                        numberOfAttacks = (((ef.classLevelOfSender - ef.damNumberOfAttacksAfterLevelN) / ef.damNumberOfAttacksForEveryNLevels) + 1) * ef.damNumberOfAttacks; //ex: 1 bolt for every 2 levels after level 1
                        if (numberOfAttacks > ef.damNumberOfAttacksUpToNAttacksTotal) { numberOfAttacks = ef.damNumberOfAttacksUpToNAttacksTotal; } //can't have more than a max amount of attacks
                    }                    
                    #endregion
                    //loop over number of attacks
                    for (int i = 0; i < numberOfAttacks; i++)
                    {
                        #region Calculate Damage
                        //(for reference) Attack: AdB+C for every D levels after level E up to F levels total
                        // damage += RandDieRoll(A,B) + C
                        //int damage = (int)((1 * RandInt(4) + 1) * resist);
                        int damage = 0;
                        if (ef.damAttacksEveryNLevels == 0) //this damage is not level based
                        {
                            damage = RandDiceRoll(ef.damNumOfDice, ef.damDie) + ef.damAdder;
                        }
                        else //this damage is level based
                        {
                            int numberOfDamAttacks = ((ef.classLevelOfSender - ef.damAttacksAfterLevelN) / ef.damAttacksEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                            if (numberOfDamAttacks > ef.damAttacksUpToNLevelsTotal) { numberOfDamAttacks = ef.damAttacksUpToNLevelsTotal; } //can't have more than a max amount of attacks
                            for (int j = 0; j < numberOfDamAttacks; j++)
                            {
                                damage += RandDiceRoll(ef.damNumOfDice, ef.damDie) + ef.damAdder;
                            }
                        }
                        #endregion
                        #region Do Calc Save and DC
                        int saveChkRoll = RandInt(20);
                        int saveChk = 0;
                        int DC = 0;
                        int saveChkAdder = 0;
                        if (ef.saveCheckType.Equals("will"))
                        {
                            saveChkAdder = crt.getWill();
                        }
                        else if (ef.saveCheckType.Equals("reflex"))
                        {
                            saveChkAdder = crt.getReflex();
                        }
                        else if (ef.saveCheckType.Equals("fortitude"))
                        {
                            saveChkAdder = crt.getFortitude();
                        }
                        else
                        {
                            saveChkAdder = -99;
                        }
                        saveChk = saveChkRoll + saveChkAdder;
                        DC = ef.saveCheckDC;
                        #endregion
                        if (saveChk >= DC) //passed save check (do half or avoid all?)
                        {
                            //**************************************
                            if (ef.saveOnlyHalvesDamage)
                            {
                                damage = damage / 2;
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " takes only half damage from " + ef.name + "</font><BR>");
                            }
                            else
                            {
                                damage = 0;
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " takes no damage from " + ef.name + "</font><BR>");
                            }
                        
                        //**************************************
                        //damage = damage / 2;
                        //gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " evades most of the " + ef.name + "</font><BR>");
                        if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + saveChkAdder + " >= " + DC + "</font><BR>"); }
                        }
                        if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + "</font><BR>"); }
                        int damageAndResist = (int)((float)damage * resist);
                        damageTotal += damageAndResist;
                        gv.cc.addLogText("<font color='silver'>" + crt.cr_name + "</font>" + "<font color='white'>" + " is damaged with " + ef.name 
                                        + " (" + "</font>" + "<font color='lime'>" + damageAndResist + "</font>" + "<font color='white'>" + " damage)</font><BR>");
                    }
                    crt.hp -= damageTotal;
                    if (crt.hp <= 0)
                    {
                        //gv.screenCombat.deathAnimationLocations.Add(new Coordinate(crt.combatLocX, crt.combatLocY));
                        foreach (Coordinate coor in crt.tokenCoveredSquares)
                        {
                            gv.screenCombat.deathAnimationLocations.Add(new Coordinate(coor.X, coor.Y));
                        }
                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), damageTotal + "");
                    #endregion
                }
                if ((ef.doHeal) && (ef.durationInUnits != ef.currentDurationInUnits))
                {
                    #region Do Heal
                    #region Calculate Heal
                    //(for reference) Heal: AdB+C for every D levels after level E up to F levels total
                    // heal += RandDieRoll(A,B) + C
                    int heal = 0;
                    if (ef.healActionsEveryNLevels == 0) //this heal is not level based
                    {
                        heal = RandDiceRoll(ef.healNumOfDice, ef.healDie) + ef.healAdder;
                    }
                    else //this heal is level based
                    {
                        int numberOfHealActions = ((ef.classLevelOfSender - ef.healActionsAfterLevelN) / ef.healActionsEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                        if (numberOfHealActions > ef.healActionsUpToNLevelsTotal) { numberOfHealActions = ef.healActionsUpToNLevelsTotal; } //can't have more than a max amount of actions
                        for (int j = 0; j < numberOfHealActions; j++)
                        {
                            heal += RandDiceRoll(ef.healNumOfDice, ef.healDie) + ef.healAdder;
                        }
                    }
                    #endregion
                    if (ef.healHP)
                    {
                        crt.hp += heal;
                        if (crt.hp > crt.hpMax)
                        {
                            crt.hp = crt.hpMax;
                        }
                        gv.cc.addLogText("<font color='lime'>" + crt.cr_name + " gains " + heal + " HPs" + "</font><BR>");
                        //Do floaty text heal
                        //gv.screenCombat.floatyTextOn = true;
                        gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), heal + "", "green");
                    }
                    else
                    {
                        crt.sp += heal;
                        if (crt.sp > crt.spMax)
                        {
                            crt.sp = crt.spMax;
                        }
                        gv.cc.addLogText("<font color='lime'>" + crt.cr_name + " gains " + heal + " SPs" + "</font><BR>");
                        //Do floaty text heal
                        //gv.screenCombat.floatyTextOn = true;
                        gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), heal + "", "green");
                    }
                    #endregion
                }
                if ((ef.doBuff) || (ef.durationInUnits > 0))
                {
                    if (!ef.isPermanent)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " has effect: " + ef.name + ", (" + (int)((ef.durationInUnits / gv.mod.TimePerRound) - 1) + " round(s) remain)</font><BR>");
                        if ((int)(ef.durationInUnits / gv.mod.TimePerRound) <= 1)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "This effect is removed on start of next turn of " + crt.cr_name + "</font><BR>");
                        }
                    }
                    //no need to do anything here as buffs are used in updateStats or during
                    //checks such as ef.addStatusType.Equals("Held") on Player or Creature class
                }
                if ((ef.doDeBuff) || (ef.durationInUnits > 0))
                {
                    if (!ef.isPermanent)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " has effect: " + ef.name + ", (" + (int)((ef.durationInUnits / gv.mod.TimePerRound) - 1) + " round(s) remain)</font><BR>");
                        if ((int)(ef.durationInUnits / gv.mod.TimePerRound) <= 1)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "This effect is removed on start of next turn of " + crt.cr_name + "</font><BR>");
                        }
                    }
                    //no need to do anything here as buffs are used in updateStats or during
                    //checks such as ef.addStatusType.Equals("Held") on Player or Creature class
                }
            }
            else //target is Player
            {
                Player pc = (Player)src;
                if ((ef.doDamage) && (ef.durationInUnits != ef.currentDurationInUnits))
                {
                    #region Do Damage
                    #region Get Resistances
                    float resistPc = 0;
                    if (ef.damType.Equals("Normal")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalNormal / 100f)); }
                    else if (ef.damType.Equals("Acid")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalAcid / 100f)); }
                    else if (ef.damType.Equals("Cold")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalCold / 100f)); }
                    else if (ef.damType.Equals("Electricity")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalElectricity / 100f)); }
                    else if (ef.damType.Equals("Fire")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f)); }
                    else if (ef.damType.Equals("Magic")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalMagic / 100f)); }
                    else if (ef.damType.Equals("Poison")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalPoison / 100f)); }
                    #endregion
                    int damageTotal = 0;
                    #region Calculate Number of Attacks
                    //(for reference) NumOfAttacks: A of these attacks for every B levels after level C up to D attacks total                    
                    int numberOfAttacks = 0;
                    if (ef.damNumberOfAttacksForEveryNLevels == 0) //this effect is using a fixed amount of attacks
                    {
                        numberOfAttacks = ef.damNumberOfAttacks;
                    }
                    else //this effect is using a variable amount of attacks
                    {
                        //numberOfAttacks = (((classLevel - C) / B) + 1) * A;
                        numberOfAttacks = (((ef.classLevelOfSender - ef.damNumberOfAttacksAfterLevelN) / ef.damNumberOfAttacksForEveryNLevels) + 1) * ef.damNumberOfAttacks; //ex: 1 bolt for every 2 levels after level 1
                    }
                    if (numberOfAttacks > ef.damNumberOfAttacksUpToNAttacksTotal) { numberOfAttacks = ef.damNumberOfAttacksUpToNAttacksTotal; } //can't have more than a max amount of attacks
                    #endregion
                    //loop over number of attacks
                    for (int i = 0; i < numberOfAttacks; i++)
                    {
                        #region Calculate Damage
                        //(for reference) Attack: AdB+C for every D levels after level E up to F levels total
                        // damage += RandDieRoll(A,B) + C
                        //int damage = (int)((1 * RandInt(4) + 1) * resist);
                        int damagePc = 0;
                        if (ef.damAttacksEveryNLevels == 0) //this damage is not level based
                        {
                            damagePc = RandDiceRoll(ef.damNumOfDice, ef.damDie) + ef.damAdder;
                        }
                        else //this damage is level based
                        {
                            int numberOfDamAttacks = ((ef.classLevelOfSender - ef.damAttacksAfterLevelN) / ef.damAttacksEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                            if (numberOfDamAttacks > ef.damAttacksUpToNLevelsTotal) { numberOfDamAttacks = ef.damAttacksUpToNLevelsTotal; } //can't have more than a max amount of attacks
                            for (int j = 0; j < numberOfDamAttacks; j++)
                            {
                                damagePc += RandDiceRoll(ef.damNumOfDice, ef.damDie) + ef.damAdder;
                            }
                        }
                        #endregion
                        #region Do Calc Save and DC
                        int saveChkRollPc = RandInt(20);
                        int saveChkPc = 0;
                        int DCPc = 0;
                        int saveChkAdder = 0;
                        if (ef.saveCheckType.Equals("will"))
                        {
                            saveChkAdder = pc.will;
                        }
                        else if (ef.saveCheckType.Equals("reflex"))
                        {
                            saveChkAdder = pc.reflex;
                        }
                        else if (ef.saveCheckType.Equals("fortitude"))
                        {
                            saveChkAdder = pc.fortitude;
                        }
                        else
                        {
                            saveChkAdder = -99;
                        }
                        saveChkPc = saveChkRollPc + saveChkAdder;
                        DCPc = ef.saveCheckDC;
                        #endregion
                        if (saveChkPc >= DCPc) //passed save check (do half or avoid all?)
                        {
                            if (ef.saveOnlyHalvesDamage)
                            {
                                damagePc = damagePc / 2;
                                gv.cc.addLogText("<font color='yellow'>" + pc.name + " takes only half damage from " + ef.name + "</font><BR>");
                            }
                            else
                            {
                                damagePc = 0;
                                gv.cc.addLogText("<font color='yellow'>" + pc.name + " takes no damage from " + ef.name + "</font><BR>");
                            }
                        
                        //damagePc = damagePc / 2;
                        //gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades most of the " + ef.name + "</font><BR>");
                        if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + saveChkRollPc + " + " + saveChkAdder + " >= " + DCPc + "</font><BR>"); }
                        }
                        if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + "resist = " + resistPc + " damage = " + damagePc + "</font><BR>"); }
                        int damageAndResist = (int)((float)damagePc * resistPc);
                        damageTotal += damageAndResist;
                        gv.cc.addLogText("<font color='silver'>"
                                        + pc.name + "</font>" + "<font color='white'>" + " is damaged with " + ef.name + " (" + "</font>" + "<font color='lime'>"
                                        + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    }
                    pc.hp -= damageTotal;
                    if (pc.hp <= 0)
                    {
                        if (pc.hp <= -20)
                        {
                            gv.screenCombat.deathAnimationLocations.Add(new Coordinate(pc.combatLocX, pc.combatLocY));
                        }
                        gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                        pc.charStatus = "Dead";
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), damageTotal + "");
                    #endregion
                }
                if ((ef.doHeal) && (ef.durationInUnits != ef.currentDurationInUnits))
                {
                    #region Do Heal
                    if (pc.hp <= -20)
                    {
                        //MessageBox("Can't heal a dead character!");
                        gv.cc.addLogText("<font color='red'>" + "Can't heal a dead character!" + "</font><BR>");
                    }
                    else
                    {
                        #region Calculate Heal
                        //(for reference) Heal: AdB+C for every D levels after level E up to F levels total
                        // heal += RandDieRoll(A,B) + C
                        int heal = 0;
                        if (ef.healActionsEveryNLevels == 0) //this heal is not level based
                        {
                            heal = RandDiceRoll(ef.healNumOfDice, ef.healDie) + ef.healAdder;
                        }
                        else //this heal is level based
                        {
                            int numberOfHealActions = ((ef.classLevelOfSender - ef.healActionsAfterLevelN) / ef.healActionsEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                            if (numberOfHealActions > ef.healActionsUpToNLevelsTotal) { numberOfHealActions = ef.healActionsUpToNLevelsTotal; } //can't have more than a max amount of actions
                            for (int j = 0; j < numberOfHealActions; j++)
                            {
                                heal += RandDiceRoll(ef.healNumOfDice, ef.healDie) + ef.healAdder;
                            }
                        }
                        #endregion
                        if (ef.healHP)
                        {
                            pc.hp += heal;
                            if (pc.hp > pc.hpMax)
                            {
                                pc.hp = pc.hpMax;
                            }
                            if (pc.hp > 0)
                            {
                                pc.charStatus = "Alive";
                            }
                            gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + heal + " HPs" + "</font><BR>");
                            //Do floaty text heal
                            //gv.screenCombat.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), heal + "", "green");
                        }
                        else
                        {
                            pc.sp += heal;
                            if (pc.sp > pc.spMax)
                            {
                                pc.sp = pc.spMax;
                            }
                            gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + heal + " SPs" + "</font><BR>");
                            //Do floaty text heal
                            //gv.screenCombat.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), heal + "", "green");
                        }
                    }
                    #endregion
                }
                if ((ef.doBuff) || (ef.durationInUnits > 0) || (ef.doDeBuff))
                {
                    if (!ef.isPermanent)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + pc.name + " has effect: " + ef.name + ", (" + (int)((ef.durationInUnits / gv.mod.TimePerRound) - 1) + " turn(s) remain)</font><BR>");
                        if ((int)(ef.durationInUnits / gv.mod.TimePerRound) <= 1)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "This effect is removed on start of next turn of " + pc.name + "</font><BR>");
                        }
                    }
                    UpdateStats(pc);
                    //no need to do anything here as buffs are used in updateStats or during
                    //checks such as ef.addStatusType.Equals("Held") on Player or Creature class
                }
                /*
                if ((ef.doDeBuff) || (ef.durationInUnits > 0))
                {
                    if (!ef.isPermanent)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + pc.name + " has effect: " + ef.name + ", (" + (int)((ef.durationInUnits / gv.mod.TimePerRound) - 1) + " turn(s) remain)</font><BR>");
                        if ((int)(ef.durationInUnits / gv.mod.TimePerRound) <= 1)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + "This effect is removed on start of next turn of " + pc.name + "</font><BR>");
                        }
                    }
                    //no need to do anything here as buffs are used in updateStats or during
                    //checks such as ef.addStatusType.Equals("Held") on Player or Creature class
                }
                */
            }
            #endregion

            #region remove dead creatures **not sure if we should do this here or not**           
            /*for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
            {
                if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                {
                    try
                    {
                        //do OnDeath IBScript
                        gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                        mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                        mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                    }
                    catch (Exception ex)
                    {
                        gv.errorLog(ex.ToString());
                    }
                }
            }*/
            #endregion

//            gv.postDelayed("doFloatyText", 100);
        }
        public void efHeld(object src, Effect ef)
        {
            //int parm1 = Integer.parseInt(p1); // parm1 = RemainingDurationInUnits (how many seconds remain)

            if (src is Player) //player casting
            {
                Player source = (Player)src;                
                if (ef.durationInUnits <= 0) //effect has expired
                {
                    if (source.hp > 0)
                    {
                        source.charStatus = "Alive";
                    }
                    else
                    {
                        source.charStatus = "Dead";
                    }
                    gv.cc.addLogText("<font color='yellow'>" + source.name + " is no longer</font><BR>");
                    gv.cc.addLogText("<font color='yellow'> being held</font><BR>");
                }
                else
                {
                    gv.cc.addLogText("<font color='yellow'>" + source.name + " is held, (" + ef.durationInUnits + " seconds remain)</font><BR>");
                    source.charStatus = "Held";
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;                
                if (ef.durationInUnits <= 0)
                {
                    source.cr_status = "Alive";
                    //ef.statusType = "none";
                    gv.cc.addLogText("<font color='yellow'>" + source.cr_name + " is no longer" + "</font>" + "<BR>");
                    gv.cc.addLogText("<font color='yellow'>" + " being held" + "</font>" + "<BR>");
                }
                else
                {
                    gv.cc.addLogText("<font color='yellow'>" + source.cr_name + " is held, (" + ef.durationInUnits + " seconds remain)</font><BR>");
                    source.cr_status = "Held";
                    //ef.statusType = "Held";
                }
            }
            else // don't know who cast this spell
            {
                //MessageBox.Show("Invalid script owner, not a Creature of PC");
                return;
            }
        }
        public void efSleep(object src, Effect ef)
        {
            //int parm1 = Integer.parseInt(p1); // parm1 = RemainingDurationInUnits (how many seconds remain)

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                if (ef.durationInUnits <= 0)
                {
                    if (source.hp > 0)
                    {
                        source.charStatus = "Alive";
                    }
                    else
                    {
                        source.charStatus = "Dead";
                    }
                    gv.cc.addLogText("<font color='yellow'>" + source.name + " wakes up from sleep spell" + "</font><BR>");
                }
                else
                {
                    gv.cc.addLogText("<font color='yellow'>" + source.name + " is sleeping, (" + ef.durationInUnits + " seconds remain)</font><BR>");
                    source.charStatus = "Held";
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                if (ef.durationInUnits <= 0)
                {
                    source.cr_status = "Alive";
                    //ef.statusType = "none";
                    gv.cc.addLogText("<font color='yellow'>" + source.cr_name + " wakes up from sleep spell" + "</font><BR>");
                }
                else
                {
                    gv.cc.addLogText("<font color='yellow'>" + source.cr_name + " is sleeping, (" + ef.durationInUnits + " seconds remain)</font><BR>");
                    source.cr_status = "Held";
                    //hurgh16
                    //ef = mod.getEffectByTag("hold");
                    //ef.statusType = "Held";
                    //target.AddEffectByObject(ef, source.cr_level);
                    //ef.statusType = "Held";
                }
            }
            else // don't know who cast this spell
            {
                //MessageBox.Show("Invalid script owner, not a Creature of PC");
                return;
            }
        }
        public void efRegenMinor(object src, Effect ef)
        {
            //int parm1 = Integer.parseInt(p1); // parm1 = RemainingDurationInUnits (how many seconds remain)

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                if (source.hp <= -20)
                {
                    //MessageBox("Can't heal a dead character!");
                    gv.cc.addLogText("<font color='red'>" + "Can't heal a dead character!" + "</font><BR>");
                }
                else
                {
                    source.hp += 2;
                    if (source.hp > source.hpMax)
                    {
                        source.hp = source.hpMax;
                    }
                    if (source.hp > 0)
                    {
                        source.charStatus = "Alive";
                    }
                    gv.cc.addLogText("<font color='lime'>" + source.name + " gains 2 HPs" + "</font><BR>");
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                source.hp += 2;
                if (source.hp > source.hpMax)
                {
                    source.hp = source.hpMax;
                }
                gv.cc.addLogText("<font color='lime'>" + source.cr_name + " gains 2 HPs" + "</font><BR>");
            }
            else // don't know who cast this spell
            {
                //MessageBox.Show("Invalid script owner, not a Creature of PC");
                return;
            }
        }
        public void efPoisoned(object src, Effect ef, int damMax)
        {
            //int parm1 = Integer.parseInt(p1); // parm1 = RemainingDurationInUnits (how many seconds remain)

            if (src is Player) //player casting
            {
                Player source = (Player)src;

                float resist = (float)(1f - ((float)source.damageTypeResistanceTotalPoison / 100f));
                float damage = 1 * RandInt(damMax);
                int poisonDam = (int)(damage * resist);

                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " poisonDam = " + poisonDam + "</font>" + "<BR>");
                }

                gv.cc.addLogText("<font color='lime'>" + source.name + " is poisoned for " + poisonDam + " hp" + "</font>" + "<BR>");
                source.hp -= poisonDam;
                if (source.hp <= 0)
                {
                    gv.cc.addLogText("<font color='red'>" + source.name + " drops unconcious!" + "</font>" + "<BR>");
                    source.charStatus = "Dead";
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;

                float resist = (float)(1f - ((float)source.damageTypeResistanceValuePoison / 100f));
                float damage = 1 * RandInt(damMax);
                int poisonDam = (int)(damage * resist);

                if (mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " poisonDam = " + poisonDam + "</font>" + "<BR>");
                }

                gv.cc.addLogText("<font color='lime'>" + source.cr_name + " is poisoned for " + poisonDam + " hp" + "</font>" + "<BR>");
                source.hp -= poisonDam;
                if (source.hp <= 0)
                {
                    gv.cc.addLogText("<font color='red'>" + source.cr_name + " has been killed!" + "</font>" + "<BR>");
                }
            }
            else // don't know who cast this spell
            {
                //MessageBox.Show("Invalid script owner, not a Creature of PC");
                return;
            }
        }

        public float GetDistanceF(int sX, int sY, int eX, int eY)
        {
            double y = Math.Abs(eY - sY);
            double x = Math.Abs(eX - sX);
            double dist = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            return (float)dist;
        }
        public void CreateAoeSquaresList(object src, object trg, AreaOfEffectShape shape, int aoeRadius)
        {
            AoeSquaresList.Clear();

            Coordinate target = new Coordinate(0,0);

            if (trg is Player)
            {
                Player pc = (Player)trg;
                target = new Coordinate(pc.combatLocX, pc.combatLocY);
            }
            else if (trg is Creature)
            {
                Creature crt = (Creature)trg;
                target = new Coordinate(crt.combatLocX, crt.combatLocY);
            }
            else if (trg is Coordinate)
            {
                target = (Coordinate)trg;
            }            

            //define AoE Radius
            int srcX = 0;
            int srcY = 0;
            if (src is Player)
            {
                Player pcs = (Player)src;
                srcX = pcs.combatLocX;
                srcY = pcs.combatLocY;
            }
            else if (src is Creature)
            {
                Creature crts = (Creature)src;
                srcX = crts.combatLocX;
                srcY = crts.combatLocY;
            }
            else if (src is Item) //item was used
            {
                Player pcs = mod.playerList[gv.screenCombat.currentPlayerIndex];
                srcX = pcs.combatLocX;
                srcY = pcs.combatLocY;
            }
            else if (src is Coordinate) //prop or trigger was used  
            {
                Coordinate coor = (Coordinate)src;
                srcX = coor.X;
                srcY = coor.Y;
            }

            //shape and radius
            #region Circle
            if (shape == AreaOfEffectShape.Circle)
            {
                for (int x = target.X - aoeRadius; x <= target.X + aoeRadius; x++)
                {
                    for (int y = target.Y - aoeRadius; y <= target.Y + aoeRadius; y++)
                    {
                        //TODO check for LoS from (target.X, target.Y) center location to (x,y)
                        AoeSquaresList.Add(new Coordinate(x, y));
                    }
                }                
            }
            #endregion
            #region Cone
            else if (shape == AreaOfEffectShape.Cone)
            {
                int signX = target.X - srcX;
                int signY = target.Y - srcY;
                int incY = 0;
                if (signY < 0) { incY = -1; }
                else { incY = 1; }
                int incX = 0;
                if (signX < 0) { incX = -1; }
                else { incX = 1; }

                //non-diagnols
                if ((signX == 0) || (signY == 0))
                {
                    if (signY == 0) //right or left
                    {
                        for (int x = 0; Math.Abs(x) <= aoeRadius; x += incX)
                        {
                            for (int y = -Math.Abs(x); y <= Math.Abs(x); y++)
                            {
                                float r = GetDistanceF(0, 0, x, y);
                                if (r <= aoeRadius)
                                {
                                    //TODO check for LoS from (target.X, target.Y) center location to (x,y)
                                    AoeSquaresList.Add(new Coordinate(x + target.X, y + target.Y));
                                }
                            }
                        }
                    }
                    else //up or down
                    {
                        for (int y = 0; Math.Abs(y) <= aoeRadius; y += incY)
                        {
                            for (int x = -Math.Abs(y); x <= Math.Abs(y); x++)
                            {
                                float r = GetDistanceF(0, 0, x, y);
                                if (r <= aoeRadius)
                                {
                                    //TODO check for LoS from (target.X, target.Y) center location to (x,y)
                                    AoeSquaresList.Add(new Coordinate(x + target.X, y + target.Y));
                                }
                            }
                        }
                    }
                }
                //diagnols
                else
                {
                    for (int x = 0; Math.Abs(x) <= aoeRadius; x += incX)
                    {
                        for (int y = 0; Math.Abs(y) <= aoeRadius; y += incY)
                        {
                            float r = GetDistanceF(0, 0, x, y);
                            if (r <= aoeRadius)
                            {
                                //TODO check for LoS from (target.X, target.Y) center location to (x,y)
                                AoeSquaresList.Add(new Coordinate(x + target.X, y + target.Y));
                            }
                        }
                    }
                }
            }
            #endregion
            #region Line
            else if (shape == AreaOfEffectShape.Line)
            {
                int rise = target.Y - srcY;
                int incY = 0;
                if (rise < 0) { incY = -1; }
                else { incY = 1; }
                if (rise == 0) { incY = 0; }
                int run = target.X - srcX;
                int incX = 0;
                if (run < 0) { incX = -1; }
                else { incX = 1; }
                if (run == 0) { incX = 0; }
                int slope = 1;
                if (Math.Abs(rise) > Math.Abs(run))
                {
                    if (run != 0)
                    {
                        slope = rise / run;
                    }
                }
                else
                {
                    if (rise != 0)
                    {
                        slope = run / rise;
                    }
                }
                int currentX = target.X;
                int currentY = target.Y;
                int riseCnt = 1;
                for (int i = 0; i < aoeRadius; i++)
                {
                    //TODO check for LoS from (target.X, target.Y) center location to (x,y)
                    AoeSquaresList.Add(new Coordinate(currentX, currentY));

                    //do the increments for the next location
                    if (Math.Abs(rise) > Math.Abs(run))
                    {
                        if (riseCnt < Math.Abs(slope)) //do rise increment only
                        {
                            currentY += incY;
                            riseCnt++;
                        }
                        else //do rise and run then reset riseCnt = 0
                        {
                            currentY += incY;
                            currentX += incX;
                            riseCnt = 1;
                        }
                    }
                    else
                    {
                        if (riseCnt < Math.Abs(slope)) //do rise increment only
                        {
                            currentX += incX;
                            riseCnt++;
                        }
                        else //do rise and run then reset riseCnt = 0
                        {
                            currentY += incY;
                            currentX += incX;
                            riseCnt = 1;
                        }
                    }
                }
            }
            #endregion
        }

        public List<Coordinate> CreateAoeSquaresListWithReturnValue(object src, object trg, AreaOfEffectShape shape, int aoeRadius)
        {
            AoeSquaresList.Clear();

            Coordinate target = new Coordinate(0, 0);

            if (trg is Player)
            {
                Player pc = (Player)trg;
                target = new Coordinate(pc.combatLocX, pc.combatLocY);
            }
            else if (trg is Creature)
            {
                Creature crt = (Creature)trg;
                target = new Coordinate(crt.combatLocX, crt.combatLocY);
            }
            else if (trg is Coordinate)
            {
                target = (Coordinate)trg;
            }

            //define AoE Radius
            int srcX = 0;
            int srcY = 0;
            if (src is Player)
            {
                Player pcs = (Player)src;
                srcX = pcs.combatLocX;
                srcY = pcs.combatLocY;
            }
            else if (src is Creature)
            {
                Creature crts = (Creature)src;
                srcX = crts.combatLocX;
                srcY = crts.combatLocY;
            }
            else if (src is Item) //item was used
            {
                Player pcs = mod.playerList[gv.screenCombat.currentPlayerIndex];
                srcX = pcs.combatLocX;
                srcY = pcs.combatLocY;
            }
            else if (src is Coordinate) //prop or trigger was used  
            {
                Coordinate coor = (Coordinate)src;
                srcX = coor.X;
                srcY = coor.Y;
            }

            //shape and radius
            #region Circle
            if (shape == AreaOfEffectShape.Circle)
            {
                for (int x = target.X - aoeRadius; x <= target.X + aoeRadius; x++)
                {
                    for (int y = target.Y - aoeRadius; y <= target.Y + aoeRadius; y++)
                    {
                        //TODO check for LoS from (target.X, target.Y) center location to (x,y)
                        AoeSquaresList.Add(new Coordinate(x, y));
                    }
                }
            }
            #endregion
            #region Cone
            else if (shape == AreaOfEffectShape.Cone)
            {
                int signX = target.X - srcX;
                int signY = target.Y - srcY;
                int incY = 0;
                if (signY < 0) { incY = -1; }
                else { incY = 1; }
                int incX = 0;
                if (signX < 0) { incX = -1; }
                else { incX = 1; }

                //non-diagnols
                if ((signX == 0) || (signY == 0))
                {
                    if (signY == 0) //right or left
                    {
                        for (int x = 0; Math.Abs(x) <= aoeRadius; x += incX)
                        {
                            for (int y = -Math.Abs(x); y <= Math.Abs(x); y++)
                            {
                                float r = GetDistanceF(0, 0, x, y);
                                if (r <= aoeRadius)
                                {
                                    //TODO check for LoS from (target.X, target.Y) center location to (x,y)
                                    AoeSquaresList.Add(new Coordinate(x + target.X, y + target.Y));
                                }
                            }
                        }
                    }
                    else //up or down
                    {
                        for (int y = 0; Math.Abs(y) <= aoeRadius; y += incY)
                        {
                            for (int x = -Math.Abs(y); x <= Math.Abs(y); x++)
                            {
                                float r = GetDistanceF(0, 0, x, y);
                                if (r <= aoeRadius)
                                {
                                    //TODO check for LoS from (target.X, target.Y) center location to (x,y)
                                    AoeSquaresList.Add(new Coordinate(x + target.X, y + target.Y));
                                }
                            }
                        }
                    }
                }
                //diagnols
                else
                {
                    for (int x = 0; Math.Abs(x) <= aoeRadius; x += incX)
                    {
                        for (int y = 0; Math.Abs(y) <= aoeRadius; y += incY)
                        {
                            float r = GetDistanceF(0, 0, x, y);
                            if (r <= aoeRadius)
                            {
                                //TODO check for LoS from (target.X, target.Y) center location to (x,y)
                                AoeSquaresList.Add(new Coordinate(x + target.X, y + target.Y));
                            }
                        }
                    }
                }
            }
            #endregion
            #region Line
            else if (shape == AreaOfEffectShape.Line)
            {
                int rise = target.Y - srcY;
                int incY = 0;
                if (rise < 0) { incY = -1; }
                else { incY = 1; }
                if (rise == 0) { incY = 0; }
                int run = target.X - srcX;
                int incX = 0;
                if (run < 0) { incX = -1; }
                else { incX = 1; }
                if (run == 0) { incX = 0; }
                int slope = 1;
                if (Math.Abs(rise) > Math.Abs(run))
                {
                    if (run != 0)
                    {
                        slope = rise / run;
                    }
                }
                else
                {
                    if (rise != 0)
                    {
                        slope = run / rise;
                    }
                }
                int currentX = target.X;
                int currentY = target.Y;
                int riseCnt = 1;
                for (int i = 0; i < aoeRadius; i++)
                {
                    //TODO check for LoS from (target.X, target.Y) center location to (x,y)
                    AoeSquaresList.Add(new Coordinate(currentX, currentY));

                    //do the increments for the next location
                    if (Math.Abs(rise) > Math.Abs(run))
                    {
                        if (riseCnt < Math.Abs(slope)) //do rise increment only
                        {
                            currentY += incY;
                            riseCnt++;
                        }
                        else //do rise and run then reset riseCnt = 0
                        {
                            currentY += incY;
                            currentX += incX;
                            riseCnt = 1;
                        }
                    }
                    else
                    {
                        if (riseCnt < Math.Abs(slope)) //do rise increment only
                        {
                            currentX += incX;
                            riseCnt++;
                        }
                        else //do rise and run then reset riseCnt = 0
                        {
                            currentY += incY;
                            currentX += incX;
                            riseCnt = 1;
                        }
                    }
                }
            }
            return AoeSquaresList;
            #endregion
        }


        public void CreateAoeTargetsList(object src, object trg, Spell thisSpell, bool usedForEffectSquares)
        {
            //europa4
            AoeTargetsList.Clear();

            int startX2 = 0;
            int startY2 = 0;
            if (src is Player)
            {
                startX2 = gv.screenCombat.targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
                startY2 = gv.screenCombat.targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);
            }
            else if (src is Item)
            {
                startX2 = gv.screenCombat.targetHighlightCenterLocation.X * gv.squareSize + (gv.squareSize / 2);
                startY2 = gv.screenCombat.targetHighlightCenterLocation.Y * gv.squareSize + (gv.squareSize / 2);
            }
            else if (src is Coordinate) //called from a prop or trigger  
            {
                startX2 = gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX * gv.squareSize + (gv.squareSize / 2);
                startY2 = gv.mod.currentEncounter.triggerScriptCalledFromSquareLocX * gv.squareSize + (gv.squareSize / 2);
            }  

            else if (src is Creature) //source is a Creature
            {
                if (trg is Player)
                {
                    Player pcs = (Player)trg;
                    startX2 = pcs.combatLocX * gv.squareSize + (gv.squareSize / 2);
                    startY2 = pcs.combatLocY * gv.squareSize + (gv.squareSize / 2);
                }
                else if (trg is Creature)
                {
                    Creature crts = (Creature)trg;
                    startX2 = crts.combatLocX * gv.squareSize + (gv.squareSize / 2);
                    startY2 = crts.combatLocY * gv.squareSize + (gv.squareSize / 2);
                }
                else if (trg is Coordinate)
                {
                    Coordinate pnt = (Coordinate)gv.sf.CombatTarget;
                    startX2 = pnt.X * gv.squareSize + (gv.squareSize / 2);
                    startY2 = pnt.Y * gv.squareSize + (gv.squareSize / 2);
                }
            }

            foreach (Coordinate coor in AoeSquaresList)
            {
                int endX2 = coor.X * gv.squareSize + (gv.squareSize / 2);
                int endY2 = coor.Y * gv.squareSize + (gv.squareSize / 2);

                if (usedForEffectSquares)
                {
                    AoeTargetsList.Add(new Coordinate(coor.X, coor.Y));
                }

                else if (gv.screenCombat.isVisibleLineOfSight(new Coordinate(startX2, startY2), new Coordinate(endX2, endY2)))
                {
                    foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                    {
                        //if in range of radius of x and radius of y
                        //if ((crt.combatLocX == coor.X) && (crt.combatLocY == coor.Y))
                        //if any part of creature is in range of radius of x and radius of y  
                        foreach (Coordinate crtCoor in crt.tokenCoveredSquares)
                        {
                            //player casts on creature
                            if ((src is Player) || (src is Item))
                            {
                                if ((thisSpell.spellTargetType.Equals("Enemy")) || (thisSpell.spellTargetType.Equals("PointLocation")) )
                                {
                                    //AoeTargetsList.Add(crt);
                                    if ((crtCoor.X == coor.X) && (crtCoor.Y == coor.Y))
                                    {
                                        AoeTargetsList.Add(crt);
                                    }
                                }
                            }
                            //creature casts on creature
                            else if (src is Creature)
                            {
                                if ((thisSpell.spellTargetType.Equals("Friend")) || (thisSpell.spellTargetType.Equals("PointLocation")))
                                {
                                    if ((crtCoor.X == coor.X) && (crtCoor.Y == coor.Y))
                                    {
                                        AoeTargetsList.Add(crt);
                                    }
                                }
                            }
                        }
                    }

                    foreach (Player pc in mod.playerList)
                    {
                        //if in range of radius of x and radius of y
                        if ((pc.combatLocX == coor.X) && (pc.combatLocY == coor.Y))
                        {
                            //player casts on player
                            if ((src is Player) || (src is Item))
                            {
                                if ((thisSpell.spellTargetType.Equals("Friend")) || (thisSpell.spellTargetType.Equals("PointLocation")))
                                {
                                    AoeTargetsList.Add(pc);
                                }
                            }
                            //creature casts on player
                            else if (src is Creature)
                            {
                                if ((thisSpell.spellTargetType.Equals("Enemy")) || (thisSpell.spellTargetType.Equals("PointLocation")))
                                {
                                    AoeTargetsList.Add(pc);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void spGeneric(Spell thisSpell, object src, object trg, bool outsideCombat, string logTextForCastAction)
        {
           
            //Effect thisSpellEffect = gv.mod.getEffectByTag(thisSpell.spellEffectTag);

            //set squares list
            //CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);

            //set target list
            //CreateAoeTargetsList(src, trg, thisSpell, false);
            if (outsideCombat)
            {
                AoeTargetsList.Clear();
                AoeTargetsList.Add(trg);
            } 
            else if (thisSpell.isUsedForCombatSquareEffect)
            {
            
                CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);
                CreateAoeTargetsList(src, trg, thisSpell, true);
            }
            else
            {
                CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);
                CreateAoeTargetsList(src, trg, thisSpell, false);
            }

            //Effect thisSpellEffect = gv.mod.getEffectByTag(thisSpell.spellEffectTag);

            #region Get casting source information
            int classLevel = 0;
            string sourceName = "";
            
            /*            
            if (thisSpellEffect == null)
            {
                gv.sf.MessageBoxHtml("EffectTag: " + thisSpell.spellEffectTag + " does not exist in this module. Abort spell cast.");
                return;
            }
            */
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                classLevel = source.classLevel;
                sourceName = source.name;
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= thisSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > thisSpell.costHP)
                    {
                        source.hp -= thisSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                classLevel = source.cr_level;
                sourceName = source.cr_name;
                source.sp -= thisSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
                //if (source.hp > thisSpell.costHP)
                //{
                    //source.hp -= thisSpell.costHP;
                //}
            }
            else if (src is Item) //item was used
            {
                Item source = (Item)src;
                if (source.usePlayerClassLevelForOnUseItemCastSpell)
                {
                    classLevel = gv.mod.playerList[gv.screenCombat.currentPlayerIndex].classLevel;
                }
                else
                {
                    classLevel = source.levelOfItemForCastSpell;
                }
                sourceName = source.name;
            }

            else if (src is Coordinate) //trigger or prop was used  
            {
                classLevel = 1;
                sourceName = "trigger";
            }

            #endregion

            //loop through all effects of spell from here
            //turn spellEffectTaag inot  a list of strings

            for (int k = 0; k < thisSpell.spellEffectTagList.Count; k++)
            {
                Effect thisSpellEffect = gv.mod.getEffectByTag(thisSpell.spellEffectTagList[k].tag);
                if (thisSpellEffect == null)
                {
                    gv.sf.MessageBoxHtml("EffectTag: " + thisSpell.spellEffectTagList[k].tag + " does not exist in this module. Abort spell cast.");
                    return;
                }

                #region Iterate over targets and apply the modifiers for damage, heal, buffs and debuffs
                if (thisSpell.isUsedForCombatSquareEffect)
                {
                
                    #region Iterate over squares and apply effect to them
                    int numberOfRounds = thisSpellEffect.durationInUnits / gv.mod.TimePerRound;
                    gv.cc.addLogText("<gn>" + thisSpellEffect.name + " is applied for " + numberOfRounds + " round(s)</gn><BR>");
                    foreach (object target in AoeTargetsList)
                    {
                        if (target is Coordinate)
                        {
                            Coordinate c = (Coordinate)target;
                            Effect e = thisSpellEffect.DeepCopy();
                            e.combatLocX = c.X;
                            e.combatLocY = c.Y;
                            gv.mod.currentEncounter.AddEffectByObject(e, classLevel);
                        }
                    }
                }
                #endregion
                
                else foreach (object target in AoeTargetsList)
                {
                    if (target is Creature)
                    {
                        Creature crt = (Creature)target;
                        bool skip = false;

                        //go through creature local vars and compare with this spellEffect's affectOnly and affectNever lists

                        //when finding a matching apply never, skip
                        foreach (LocalImmunityString s in thisSpellEffect.affectNeverList)
                        {
                            foreach (LocalString ls in crt.CreatureLocalStrings)
                            {
                                if (s.Value.Equals(ls.Value))
                                {
                                    skip = true;
                                    gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " is immune to " + thisSpellEffect.name + "</font><BR>");
                                    break;
                                }
                            }

                            if (skip)
                            {
                                break;
                            }
                        }

                        //when finding an entry in affectOnlyList, skip unless it matches
                        if (!skip)
                        {
                            if (thisSpellEffect.affectOnlyList.Count > 0)
                            {
                                skip = true;

                                foreach (LocalImmunityString s in thisSpellEffect.affectOnlyList)
                                {
                                    foreach (LocalString ls in crt.CreatureLocalStrings)
                                    {
                                        if (s.Value.Equals(ls.Value))
                                        {
                                            skip = false;
                                            break;
                                        }
                                    }
                                    if (skip)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (!skip)
                        {
                            if ((thisSpellEffect.doDamage) && (thisSpellEffect.durationInUnits == 0))
                            {
                                #region Do Damage
                                #region Get Resistances
                                float resist = 0;
                                 /*
                                if (thisSpellEffect.damType.Equals("Normal")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueNormal / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Acid")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueAcid / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Cold")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueCold / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Electricity")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueElectricity / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Fire")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Magic")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueMagic / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Poison")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValuePoison / 100f)); }
                                */

                                    if (thisSpellEffect.damType.Equals("Normal")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueNormal() / 100f)); }
                                    else if (thisSpellEffect.damType.Equals("Acid")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueAcid() / 100f)); }
                                    else if (thisSpellEffect.damType.Equals("Cold")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueCold() / 100f)); }
                                    else if (thisSpellEffect.damType.Equals("Electricity")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueElectricity() / 100f)); }
                                    else if (thisSpellEffect.damType.Equals("Fire")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueFire() / 100f)); }
                                    else if (thisSpellEffect.damType.Equals("Magic")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValueMagic() / 100f)); }
                                    else if (thisSpellEffect.damType.Equals("Poison")) { resist = (float)(1f - ((float)crt.getDamageTypeResistanceValuePoison() / 100f)); }

                                #endregion
                                    int damageTotal = 0;
                                #region Calculate Number of Attacks
                                //(for reference) NumOfAttacks: A of these attacks for every B levels after level C up to D attacks total                    
                                int numberOfAttacks = 0;
                                if (thisSpellEffect.damNumberOfAttacksForEveryNLevels == 0) //this effect is using a fixed amount of attacks
                                {
                                    numberOfAttacks = thisSpellEffect.damNumberOfAttacks;
                                }
                                else //this effect is using a variable amount of attacks
                                {
                                    //numberOfAttacks = (((classLevel - C) / B) + 1) * A;
                                    numberOfAttacks = (((classLevel - thisSpellEffect.damNumberOfAttacksAfterLevelN) / thisSpellEffect.damNumberOfAttacksForEveryNLevels) + 1) * thisSpellEffect.damNumberOfAttacks; //ex: 1 bolt for every 2 levels after level 1
                                    if (numberOfAttacks > thisSpellEffect.damNumberOfAttacksUpToNAttacksTotal) { numberOfAttacks = thisSpellEffect.damNumberOfAttacksUpToNAttacksTotal; } //can't have more than a max amount of attacks
                                }

                                #endregion
                                //loop over number of attacks
                                for (int i = 0; i < numberOfAttacks; i++)
                                {
                                    #region Calculate Damage
                                    //(for reference) Attack: AdB+C for every D levels after level E up to F levels total
                                    // damage += RandDieRoll(A,B) + C
                                    //int damage = (int)((1 * RandInt(4) + 1) * resist);
                                    int damage = 0;
                                    if (thisSpellEffect.damAttacksEveryNLevels == 0) //this damage is not level based
                                    {
                                        damage = RandDiceRoll(thisSpellEffect.damNumOfDice, thisSpellEffect.damDie) + thisSpellEffect.damAdder;
                                    }
                                    else //this damage is level based
                                    {
                                        int numberOfDamAttacks = ((classLevel - thisSpellEffect.damAttacksAfterLevelN) / thisSpellEffect.damAttacksEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                                        if (numberOfDamAttacks > thisSpellEffect.damAttacksUpToNLevelsTotal) { numberOfDamAttacks = thisSpellEffect.damAttacksUpToNLevelsTotal; } //can't have more than a max amount of attacks
                                        for (int j = 0; j < numberOfDamAttacks; j++)
                                        {
                                            damage += RandDiceRoll(thisSpellEffect.damNumOfDice, thisSpellEffect.damDie) + thisSpellEffect.damAdder;
                                        }
                                    }
                                    #endregion
                                    #region Do Calc Save and DC
                                    int saveChkRoll = RandInt(20);
                                    int saveChk = 0;
                                    int DC = 0;
                                    int saveChkAdder = 0;
                                    if (thisSpellEffect.saveCheckType.Equals("will"))
                                    {
                                        saveChkAdder = crt.getWill();
                                    }
                                    else if (thisSpellEffect.saveCheckType.Equals("reflex"))
                                    {
                                        saveChkAdder = crt.getReflex();
                                    }
                                    else if (thisSpellEffect.saveCheckType.Equals("fortitude"))
                                    {
                                        saveChkAdder = crt.getFortitude();
                                    }
                                    else
                                    {
                                        saveChkAdder = -99;
                                    }
                                    saveChk = saveChkRoll + saveChkAdder;
                                    DC = thisSpellEffect.saveCheckDC;
                                    #endregion
                                    //europa
                                    if (saveChk >= DC) //passed save check (do half or avoid all?)
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " makes successful " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRoll.ToString() + "+" + saveChkAdder + ">=" + DC.ToString() + ")" + "</font><BR>");
                                        if (thisSpellEffect.saveOnlyHalvesDamage)
                                        {
                                            damage = damage / 2;
                                            gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " takes only half damage from " + thisSpellEffect.name + "</font><BR>");
                                        }
                                        else
                                        {
                                            damage = 0;
                                            gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " takes no damage from " + thisSpellEffect.name + "</font><BR>");
                                        }
                                    }
                                    else //failed save check or no save check allowed
                                    {
                                        //failed save roll
                                        if (saveChkAdder > -99)
                                        {
                                            gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " failed " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRoll.ToString() + "+" + saveChkAdder + " < " + DC.ToString() + ")" + "</font><BR>");
                                        }
                                        else//no save roll allowed
                                        {
                                            gv.cc.addLogText("<font color='yellow'>" + "No saving roll allowed" + "</font><BR>");
                                        }
                                    }

                                    if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + saveChkAdder + " >= " + DC + "</font><BR>"); }
                                    if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + "</font><BR>"); }

                                    int damageAndResist = (int)((float)damage * resist);
                                    damageTotal += damageAndResist;
                                    //resistance exists
                                    if (resist < 1)
                                    {
                                        gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                        + crt.cr_name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                        + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "(-" + ((1 - resist) * 100f) + "% resistance)" + "</font><BR>");
                                    }
                                    //vulnerability exists
                                    else if (resist > 1)
                                    {
                                        gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                        + crt.cr_name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                        + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "(+" + ((resist - 1) * 100f) + "% vulnerability)" + "</font><BR>");
                                    }
                                    //neither resistance nor vulnerability
                                    else
                                    {
                                        gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                        + crt.cr_name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                        + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                                    }
                                }
                                crt.hp -= damageTotal;
                                if (crt.hp <= 0)
                                {
                                        //gv.screenCombat.deathAnimationLocations.Add(new Coordinate(crt.combatLocX, crt.combatLocY));
                                        //gv.screenCombat.deathAnimationLocations.Add(new Coordinate(crt.combatLocX, crt.combatLocY));  
                                        foreach (Coordinate coor in crt.tokenCoveredSquares)
                                        {
                                            gv.screenCombat.deathAnimationLocations.Add(new Coordinate(coor.X, coor.Y));
                                        }
                                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                                }
                                //Do floaty text damage
                                //gv.screenCombat.floatyTextOn = true;
                                gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), damageTotal + "");
                                #endregion
                            }
                            if ((thisSpellEffect.doHeal) && (thisSpellEffect.durationInUnits == 0))
                            {
                                    //this will be checked whiel building the AOETargetsList
                                    //if (src is Player) //PCs shouldn't heal creatures  
                                    //{
                                        //continue;
                                    //}

                                    #region Do Heal
                                    #region Calculate Heal
                                    //(for reference) Heal: AdB+C for every D levels after level E up to F levels total
                                    // heal += RandDieRoll(A,B) + C
                                    int heal = 0;
                                if (thisSpellEffect.healActionsEveryNLevels == 0) //this heal is not level based
                                {
                                    heal = RandDiceRoll(thisSpellEffect.healNumOfDice, thisSpellEffect.healDie) + thisSpellEffect.healAdder;
                                }
                                else //this heal is level based
                                {
                                    int numberOfHealActions = ((classLevel - thisSpellEffect.healActionsAfterLevelN) / thisSpellEffect.healActionsEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                                    if (numberOfHealActions > thisSpellEffect.healActionsUpToNLevelsTotal) { numberOfHealActions = thisSpellEffect.healActionsUpToNLevelsTotal; } //can't have more than a max amount of actions
                                    for (int j = 0; j < numberOfHealActions; j++)
                                    {
                                        heal += RandDiceRoll(thisSpellEffect.healNumOfDice, thisSpellEffect.healDie) + thisSpellEffect.healAdder;
                                    }
                                }
                                #endregion
                                //crt.hp += heal;
                                //if (crt.hp > crt.hpMax)
                                if (thisSpellEffect.healHP)
                                {
                                        //crt.hp = crt.hpMax;
                                        crt.hp += heal;
                                        if (crt.hp > crt.hpMax)
                                        {
                                            crt.hp = crt.hpMax;
                                        }
                                        gv.cc.addLogText("<font color='lime'>" + crt.cr_name + " gains " + heal + " HPs" + "</font><BR>");
                                        gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), heal + "", "green");

                                 }
                                    //gv.cc.addLogText("<font color='lime'>" + crt.cr_name + " gains " + heal + " HPs" + "</font><BR>");
                                 else
                                 {
                                        crt.sp += heal;
                                        if (crt.sp > crt.spMax)
                                        {
                                            crt.sp = crt.spMax;
                                        }
                                        gv.cc.addLogText("<font color='lime'>" + crt.cr_name + " gains " + heal + " SPs" + "</font><BR>");
                                        gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), heal + "", "yellow");
                                 }

                                    //Do floaty text heal
                                    //gv.screenCombat.floatyTextOn = true;
                                    //gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), heal + "", "green");
                                #endregion
                            }

                            /*
                            if (thisSpellEffect.doBuff)
                            {
                                #region Do Buff
                                int numberOfRounds = thisSpellEffect.durationInUnits / gv.mod.TimePerRound;
                                gv.cc.addLogText("<font color='lime'>" + thisSpellEffect.name + " is applied on " + crt.cr_name + " for " + numberOfRounds + " round(s)</font><BR>");
                                crt.AddEffectByObject(thisSpellEffect, classLevel);
                                #endregion
                            }
                            */

                            ///trying to keep old spells compatible, in the long run likely just rely on duration > 0
                            if ((thisSpellEffect.doBuff) || (thisSpellEffect.doDeBuff) || (thisSpellEffect.durationInUnits > 0))
                            {
                                #region (Try to) add to effect list of target
                                #region Do Calc Save and DC
                                int saveChkRoll = RandInt(20);
                                int saveChk = 0;
                                int DC = 0;
                                int saveChkAdder = 0;
                                if (thisSpellEffect.saveCheckType.Equals("will"))
                                {
                                    saveChkAdder = crt.getWill();
                                }
                                else if (thisSpellEffect.saveCheckType.Equals("reflex"))
                                {
                                    saveChkAdder = crt.getReflex();
                                }
                                else if (thisSpellEffect.saveCheckType.Equals("fortitude"))
                                {
                                    saveChkAdder = crt.getFortitude();
                                }
                                else
                                {
                                    saveChkAdder = -99;
                                }
                                saveChk = saveChkRoll + saveChkAdder;
                                DC = thisSpellEffect.saveCheckDC;
                                #endregion
                                //europa
                                if (saveChk >= DC) //passed save check
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " makes successful " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRoll.ToString() + "+" + saveChkAdder + ">=" + DC.ToString() + ")" + " and avoids " + thisSpellEffect.name + " </font><BR>");
                                    //gv.cc.addLogText("<font color='yellow'>" + "(" + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRoll.ToString() + "+" + saveChkAdder + ">=" + DC.ToString() + ")" + " and avoids the longer lasting effect of" + thisSpellEffect.name + " </font><BR>");
                                    //gv.cc.addLogText("<font color='yellow'>" + "(" + saveChkRoll.ToString() + "+" + saveChkAdder + " < " + DC.ToString() + ")" + "</font><BR>");
                                    //gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " avoids the " + thisSpellEffect.name + " effect.</font><BR>");
                                }
                                else//failed save roll or no roll allowed
                                {
                                    //failed save roll
                                    if (saveChkAdder > -99)
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " failed " + thisSpellEffect.saveCheckType + " saving roll for " + thisSpellEffect.name + "(" + saveChkRoll.ToString() + "+" + saveChkAdder + " < " + DC.ToString() + ")" + "</font><BR>");
                                    }
                                    //else//no save roll allowed
                                    //{
                                    //gv.cc.addLogText("<font color='yellow'>" + "No save roll against longer lasting effect of " + thisSpellEffect.name + " allowed" + "</font><BR>");
                                    //}
                                    int numberOfRounds = thisSpellEffect.durationInUnits / gv.mod.TimePerRound;
                                    gv.cc.addLogText("<font color='lime'>" + thisSpellEffect.name + " is applied on " + crt.cr_name + " for " + numberOfRounds + " round(s)</font><BR>");
                                    crt.AddEffectByObject(thisSpellEffect, classLevel);
                                }
                                #endregion
                            }
                            if (thisSpell.removeEffectTagList.Count > 0)
                            {
                                #region remove effects  
                                foreach (EffectTagForDropDownList efTag in thisSpell.removeEffectTagList)
                                {
                                    for (int x = crt.cr_effectsList.Count - 1; x >= 0; x--)
                                    {
                                        if (crt.cr_effectsList[x].tag.Equals(efTag.tag))
                                        {
                                            try
                                            {
                                                crt.cr_effectsList.RemoveAt(x);
                                            }
                                            catch (Exception ex)
                                            {
                                                gv.errorLog(ex.ToString());
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    else //target is Player
                    {
                        //europa
                        Player pc = (Player)target;

                        bool skip = false;

                        //go through creature local vars and compare with this spellEffect's affectOnly and affectNever lists

                        //when finding a matching apply never, skip
                        foreach (LocalImmunityString s in thisSpellEffect.affectNeverList)
                        {
                            foreach (string ls in pc.knownTraitsTags)
                            {
                                if (s.Value.Equals(ls))
                                {
                                    skip = true;
                                    gv.cc.addLogText("<font color='yellow'>" + pc.name + " is immune to " + thisSpellEffect.name + "</font><BR>");
                                    break;
                                }
                            }

                            if (skip)
                            {
                                break;
                            }
                        }

                        //when finding an entry in affectOnlyList, skip unless it matches
                        if (!skip)
                        {
                            if (thisSpellEffect.affectOnlyList.Count > 0)
                            {
                                skip = true;

                                foreach (LocalImmunityString s in thisSpellEffect.affectOnlyList)
                                {
                                    foreach (string ls in pc.knownTraitsTags)
                                    {
                                        if (s.Value.Equals(ls))
                                        {
                                            skip = false;
                                            break;
                                        }
                                    }
                                    if (skip)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (!skip)
                        {
                            if ((thisSpellEffect.doDamage) && (thisSpellEffect.durationInUnits == 0))
                            {
                                #region Do Damage
                                #region Get Resistances
                                float resistPc = 0;
                                if (thisSpellEffect.damType.Equals("Normal")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalNormal / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Acid")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalAcid / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Cold")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalCold / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Electricity")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalElectricity / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Fire")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Magic")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalMagic / 100f)); }
                                else if (thisSpellEffect.damType.Equals("Poison")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalPoison / 100f)); }
                                #endregion
                                int damageTotal = 0;
                                #region Calculate Number of Attacks
                                //(for reference) NumOfAttacks: A of these attacks for every B levels after level C up to D attacks total                    
                                int numberOfAttacks = 0;
                                if (thisSpellEffect.damNumberOfAttacksForEveryNLevels == 0) //this effect is using a fixed amount of attacks
                                {
                                    numberOfAttacks = thisSpellEffect.damNumberOfAttacks;
                                }
                                else //this effect is using a variable amount of attacks
                                {
                                    //numberOfAttacks = (((classLevel - C) / B) + 1) * A;
                                    numberOfAttacks = (((classLevel - thisSpellEffect.damNumberOfAttacksAfterLevelN) / thisSpellEffect.damNumberOfAttacksForEveryNLevels) + 1) * thisSpellEffect.damNumberOfAttacks; //ex: 1 bolt for every 2 levels after level 1
                                }
                                if (numberOfAttacks > thisSpellEffect.damNumberOfAttacksUpToNAttacksTotal) { numberOfAttacks = thisSpellEffect.damNumberOfAttacksUpToNAttacksTotal; } //can't have more than a max amount of attacks
                                #endregion
                                //loop over number of attacks
                                for (int i = 0; i < numberOfAttacks; i++)
                                {
                                    #region Calculate Damage
                                    //(for reference) Attack: AdB+C for every D levels after level E up to F levels total
                                    // damage += RandDieRoll(A,B) + C
                                    //int damage = (int)((1 * RandInt(4) + 1) * resist);
                                    int damagePc = 0;
                                    if (thisSpellEffect.damAttacksEveryNLevels == 0) //this damage is not level based
                                    {
                                        damagePc = RandDiceRoll(thisSpellEffect.damNumOfDice, thisSpellEffect.damDie) + thisSpellEffect.damAdder;
                                    }
                                    else //this damage is level based
                                    {
                                        int numberOfDamAttacks = ((classLevel - thisSpellEffect.damAttacksAfterLevelN) / thisSpellEffect.damAttacksEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                                        if (numberOfDamAttacks > thisSpellEffect.damAttacksUpToNLevelsTotal) { numberOfDamAttacks = thisSpellEffect.damAttacksUpToNLevelsTotal; } //can't have more than a max amount of attacks
                                        for (int j = 0; j < numberOfDamAttacks; j++)
                                        {
                                            damagePc += RandDiceRoll(thisSpellEffect.damNumOfDice, thisSpellEffect.damDie) + thisSpellEffect.damAdder;
                                        }
                                    }
                                    #endregion
                                    #region Do Calc Save and DC
                                    int saveChkRollPc = RandInt(20);
                                    int saveChkPc = 0;
                                    int DCPc = 0;
                                    int saveChkAdder = 0;
                                    if (thisSpellEffect.saveCheckType.Equals("will"))
                                    {
                                        saveChkAdder = pc.will;
                                    }
                                    else if (thisSpellEffect.saveCheckType.Equals("reflex"))
                                    {
                                        saveChkAdder = pc.reflex;
                                    }
                                    else if (thisSpellEffect.saveCheckType.Equals("fortitude"))
                                    {
                                        saveChkAdder = pc.fortitude;
                                    }
                                    else
                                    {
                                        saveChkAdder = -99;
                                    }
                                    saveChkPc = saveChkRollPc + saveChkAdder;
                                    DCPc = thisSpellEffect.saveCheckDC;
                                    #endregion

                                    if (saveChkPc >= DCPc) //passed save check (do half or avoid all?)
                                    {

                                            if (thisSpellEffect.saveOnlyHalvesDamage)
                                            {
                                                damagePc = damagePc / 2;
                                                gv.cc.addLogText("<font color='yellow'>" + pc.name + " takes only half damage from " + thisSpellEffect.name + "</font><BR>");
                                            }
                                            else
                                            {
                                                damagePc = 0;
                                                gv.cc.addLogText("<font color='yellow'>" + pc.name + " takes no damage from " + thisSpellEffect.name + "</font><BR>");
                                            }
                                            //gv.cc.addLogText("<font color='yellow'>" + pc.name + " makes successful " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRollPc.ToString() + "+" + saveChkAdder + ">=" + DCPc.ToString() + ")" + "</font><BR>");
                                            //damagePc = damagePc / 2;
                                            //gv.cc.addLogText("<font color='yellow'>" + pc.name + " takes only half damage from " + thisSpellEffect.name + "</font><BR>");
                                    }
                                    else //failed save check or no save check allowed
                                    {
                                        //failed save roll
                                        if (saveChkAdder > -99)
                                        {
                                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " failed " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRollPc.ToString() + "+" + saveChkAdder + " < " + DCPc.ToString() + ")" + "</font><BR>");
                                        }
                                        else//no save roll allowed
                                        {
                                            gv.cc.addLogText("<font color='yellow'>" + "No saving roll allowed" + "</font><BR>");
                                        }
                                    }

                                    if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + saveChkRollPc + " + " + saveChkAdder + " >= " + DCPc + "</font><BR>"); }
                                    if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + "resist = " + resistPc + " damage = " + damagePc + "</font><BR>"); }

                                    int damageAndResist = (int)((float)damagePc * resistPc);
                                    damageTotal += damageAndResist;
                                    //europa
                                    //resistance exists
                                    if (resistPc < 1)
                                    {
                                        gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                        + pc.name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                        + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "(-" + ((1 - resistPc) * 100f) + "% resistance)" + "</font><BR>");
                                    }
                                    //vulnerability exists
                                    else if (resistPc > 1)
                                    {
                                        gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                        + pc.name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                        + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "(+" + ((resistPc - 1) * 100f) + "% vulnerability)" + "</font><BR>");
                                    }
                                    //neither resistance nor vulnerability
                                    else
                                    {
                                        gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                        + pc.name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                        + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                                    }
                                }

                                pc.hp -= damageTotal;
                                if (pc.hp <= 0)
                                {
                                    if (pc.hp <= -20)
                                    {
                                        gv.screenCombat.deathAnimationLocations.Add(new Coordinate(pc.combatLocX, pc.combatLocY));
                                    }
                                    gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                                    pc.charStatus = "Dead";
                                }
                                //Do floaty text damage
                                //gv.screenCombat.floatyTextOn = true;
                                gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), damageTotal + "");
                                #endregion
                            }
                            if ((thisSpellEffect.doHeal) && (thisSpellEffect.durationInUnits == 0))
                            {
                                #region Do Heal
                                if (pc.hp <= -20)
                                {
                                    //MessageBox("Can't heal a dead character!");
                                    gv.cc.addLogText("<font color='red'>" + "Can't heal a dead character!" + "</font><BR>");
                                }
                                else
                                {
                                    #region Calculate Heal
                                    //(for reference) Heal: AdB+C for every D levels after level E up to F levels total
                                    // heal += RandDieRoll(A,B) + C
                                    int heal = 0;
                                    if (thisSpellEffect.healActionsEveryNLevels == 0) //this heal is not level based
                                    {
                                        heal = RandDiceRoll(thisSpellEffect.healNumOfDice, thisSpellEffect.healDie) + thisSpellEffect.healAdder;
                                    }
                                    else //this heal is level based
                                    {
                                        int numberOfHealActions = ((classLevel - thisSpellEffect.healActionsAfterLevelN) / thisSpellEffect.healActionsEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                                        if (numberOfHealActions > thisSpellEffect.healActionsUpToNLevelsTotal) { numberOfHealActions = thisSpellEffect.healActionsUpToNLevelsTotal; } //can't have more than a max amount of actions
                                        for (int j = 0; j < numberOfHealActions; j++)
                                        {
                                            heal += RandDiceRoll(thisSpellEffect.healNumOfDice, thisSpellEffect.healDie) + thisSpellEffect.healAdder;
                                        }
                                    }
                                        #endregion
                                        //pc.hp += heal;
                                        //if (pc.hp > pc.hpMax)
                                    if (thisSpellEffect.healHP)
                                    {
                                            //pc.hp = pc.hpMax;
                                            pc.hp += heal;
                                            if (pc.hp > pc.hpMax)
                                            {
                                                pc.hp = pc.hpMax;
                                            }
                                            if (pc.hp > 0)
                                            {
                                                pc.charStatus = "Alive";
                                            }
                                            gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + heal + " HPs" + "</font><BR>");
                                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), heal + "", "green");

                                   }
                                   else
                                    {
                                            //pc.charStatus = "Alive";
                                            pc.sp += heal;
                                            if (pc.sp > pc.spMax)
                                            {
                                                pc.sp = pc.spMax;
                                            }
                                            gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + heal + " SPs" + "</font><BR>");
                                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), heal + "", "yellow");

                                        }
                                        //gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + heal + " HPs" + "</font><BR>");
                                    //Do floaty text heal
                                    //gv.screenCombat.floatyTextOn = true;
                                    //gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), heal + "", "green");
                                }
                                #endregion
                            }
                            /*
                            if (thisSpellEffect.doBuff)
                            {
                                #region Do Buff
                                int numberOfRounds = thisSpellEffect.durationInUnits / gv.mod.TimePerRound;
                                gv.cc.addLogText("<font color='lime'>" + thisSpellEffect.name + " is applied on " + pc.name + " for " + numberOfRounds + " round(s)</font><BR>");
                                pc.AddEffectByObject(thisSpellEffect, classLevel);
                                #endregion
                            }
                            */

                            //europa2
                            //trying to keep this compatible with old spells, in the long run only duration units should be relevant? 
                            if ((thisSpellEffect.doDeBuff) || (thisSpellEffect.doBuff) || (thisSpellEffect.durationInUnits > 0))
                            {
                                #region (Try to) add to target's effect list
                                #region Do Calc Save and DC
                                int saveChkRoll = RandInt(20);
                                int saveChk = 0;
                                int DC = 0;
                                int saveChkAdder = 0;
                                if (thisSpellEffect.saveCheckType.Equals("will"))
                                {
                                    saveChkAdder = pc.will;
                                }
                                else if (thisSpellEffect.saveCheckType.Equals("reflex"))
                                {
                                    saveChkAdder = pc.reflex;
                                }
                                else if (thisSpellEffect.saveCheckType.Equals("fortitude"))
                                {
                                    saveChkAdder = pc.fortitude;
                                }
                                else
                                {
                                    saveChkAdder = -99;
                                }
                                saveChk = saveChkRoll + saveChkAdder;
                                DC = thisSpellEffect.saveCheckDC;
                                #endregion
                                if (saveChk >= DC) //passed save check
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + pc.name + " makes successful " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRoll.ToString() + "+" + saveChkAdder + ">=" + DC.ToString() + ")" + " and avoids " + thisSpellEffect.name + " </font><BR>");
                                }
                                else//failed save roll or no roll allowed
                                {
                                    //failed save roll
                                    if (saveChkAdder > -99)
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + pc.name + " failed " + thisSpellEffect.saveCheckType + " saving roll for " + thisSpellEffect.name + "(" + saveChkRoll.ToString() + "+" + saveChkAdder + " < " + DC.ToString() + ")" + "</font><BR>");
                                        //gv.cc.addLogText("<font color='yellow'>" + pc.name + " failed " + thisSpellEffect.saveCheckType + " saving roll against " + thisSpellEffect.name + "</font><BR>");
                                        //gv.cc.addLogText("<font color='yellow'>" + "(" + saveChkRoll.ToString() + "+" + saveChkAdder.ToString() + "<" + DC.ToString() + ")" + "</font><BR>");
                                    }
                                    else//no save roll allowed
                                    {
                                        //gv.cc.addLogText("<font color='yellow'>" + "No saving roll allowed against longer lasting effect of " + thisSpellEffect.name + "</font><BR>");
                                    }
                                    int numberOfRounds = thisSpellEffect.durationInUnits / gv.mod.TimePerRound;
                                    //gv.cc.addLogText("<font color='lime'>" + thisSpellEffect.name + " is applied on " + pc.name + " for " + numberOfRounds + " round(s)</font><BR>");
                                    pc.AddEffectByObject(thisSpellEffect, classLevel);
                                    gv.cc.doEffectScript(pc, thisSpellEffect);

                                    }
                                #endregion
                            }
                            if (thisSpell.removeEffectTagList.Count > 0)
                            {
                                #region remove effects  
                                foreach (EffectTagForDropDownList efTag in thisSpell.removeEffectTagList)
                                {
                                    for (int x = pc.effectsList.Count - 1; x >= 0; x--)
                                    {
                                        if (pc.effectsList[x].tag.Equals(efTag.tag))
                                        {
                                            try
                                            {
                                                pc.effectsList.RemoveAt(x);
                                            }
                                            catch (Exception ex)
                                            {
                                                gv.errorLog(ex.ToString());
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                #endregion

                #region remove dead creatures            
                /*for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
                {
                    if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                    {
                        try
                        {
                            //do OnDeath IBScript
                            gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                            mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                            mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                        }
                        catch (Exception ex)
                        {
                            gv.errorLog(ex.ToString());
                        }
                    }
                }*/
                #endregion

                //            gv.postDelayed("doFloatyText", 100);
            }
        }

        public void spGenericUsingOldSingleEffectTag(Spell thisSpell, object src, object trg, bool outsideCombat)
        {
            //set squares list
            //CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);

            if (outsideCombat)
            {
                AoeTargetsList.Clear();
                AoeTargetsList.Add(trg);
            }
            else if (thisSpell.isUsedForCombatSquareEffect)
            {
                CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);
                CreateAoeTargetsList(src, trg, thisSpell, true);
            }
            else
            {
                CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);
                CreateAoeTargetsList(src, trg, thisSpell, false);
            }

            //set target list
            //CreateAoeTargetsList(src, trg, thisSpell, false);

            //Effect thisSpellEffect = gv.mod.getEffectByTag(thisSpell.spellEffectTag);

            #region Get casting source information
            int classLevel = 0;
            string sourceName = "";
            /*            
            if (thisSpellEffect == null)
            {
                gv.sf.MessageBoxHtml("EffectTag: " + thisSpell.spellEffectTag + " does not exist in this module. Abort spell cast.");
                return;
            }
            */
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                classLevel = source.classLevel;
                sourceName = source.name;
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= thisSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > thisSpell.costHP)
                    {
                        source.hp -= thisSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                classLevel = source.cr_level;
                sourceName = source.cr_name;
                source.sp -= thisSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Item) //item was used
            {
                Item source = (Item)src;
                classLevel = source.levelOfItemForCastSpell;
                sourceName = source.name;
            }
            #endregion

            //loop through all effects of spell from here
            //turn spellEffectTaag inot  a list of strings

            //for (int k = 0; k < thisSpell.spellEffectTagList.Count; k++)
            //{
                Effect thisSpellEffect = gv.mod.getEffectByTag(thisSpell.spellEffectTag);
                if (thisSpellEffect == null)
                {
                    gv.sf.MessageBoxHtml("EffectTag: " + thisSpell.spellEffectTag + " does not exist in this module. Abort spell cast.");
                    return;
                }

                #region Iterate over targets and apply the modifiers for damage, heal, buffs and debuffs
                foreach (object target in AoeTargetsList)
                {
                if (target is Creature)
                {
                    Creature crt = (Creature)target;
                    bool skip = false;

                    //go through creature local vars and compare with this spellEffect's affectOnly and affectNever lists

                    //when finding a matching apply never, skip
                    foreach (LocalImmunityString s in thisSpellEffect.affectNeverList)
                    {
                        foreach (LocalString ls in crt.CreatureLocalStrings)
                        {
                            if (s.Value.Equals(ls.Value))
                            {
                                skip = true;
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " is immune to " + thisSpellEffect.name + "</font><BR>");
                                break;
                            }
                        }

                        if (skip)
                        {
                            break;
                        }
                    }

                    //when finding an entry in affectOnlyList, skip unless it matches
                    if (!skip)
                    {
                        if (thisSpellEffect.affectOnlyList.Count > 0)
                        {
                            skip = true;

                            foreach (LocalImmunityString s in thisSpellEffect.affectOnlyList)
                            {
                                foreach (LocalString ls in crt.CreatureLocalStrings)
                                {
                                    if (s.Value.Equals(ls.Value))
                                    {
                                        skip = false;
                                        break;
                                    }
                                }
                                if (skip)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (!skip)
                    {
                        if ((thisSpellEffect.doDamage) && (thisSpellEffect.durationInUnits == 0))
                    {
                        #region Do Damage
                        #region Get Resistances
                        float resist = 0;
                        if (thisSpellEffect.damType.Equals("Normal")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueNormal / 100f)); }
                        else if (thisSpellEffect.damType.Equals("Acid")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueAcid / 100f)); }
                        else if (thisSpellEffect.damType.Equals("Cold")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueCold / 100f)); }
                        else if (thisSpellEffect.damType.Equals("Electricity")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueElectricity / 100f)); }
                        else if (thisSpellEffect.damType.Equals("Fire")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f)); }
                        else if (thisSpellEffect.damType.Equals("Magic")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValueMagic / 100f)); }
                        else if (thisSpellEffect.damType.Equals("Poison")) { resist = (float)(1f - ((float)crt.damageTypeResistanceValuePoison / 100f)); }
                        #endregion
                        int damageTotal = 0;
                        #region Calculate Number of Attacks
                        //(for reference) NumOfAttacks: A of these attacks for every B levels after level C up to D attacks total                    
                        int numberOfAttacks = 0;
                        if (thisSpellEffect.damNumberOfAttacksForEveryNLevels == 0) //this effect is using a fixed amount of attacks
                        {
                            numberOfAttacks = thisSpellEffect.damNumberOfAttacks;
                        }
                        else //this effect is using a variable amount of attacks
                        {
                            //numberOfAttacks = (((classLevel - C) / B) + 1) * A;
                            numberOfAttacks = (((classLevel - thisSpellEffect.damNumberOfAttacksAfterLevelN) / thisSpellEffect.damNumberOfAttacksForEveryNLevels) + 1) * thisSpellEffect.damNumberOfAttacks; //ex: 1 bolt for every 2 levels after level 1
                            if (numberOfAttacks > thisSpellEffect.damNumberOfAttacksUpToNAttacksTotal) { numberOfAttacks = thisSpellEffect.damNumberOfAttacksUpToNAttacksTotal; } //can't have more than a max amount of attacks
                        }

                        #endregion
                        //loop over number of attacks
                        for (int i = 0; i < numberOfAttacks; i++)
                        {
                            #region Calculate Damage
                            //(for reference) Attack: AdB+C for every D levels after level E up to F levels total
                            // damage += RandDieRoll(A,B) + C
                            //int damage = (int)((1 * RandInt(4) + 1) * resist);
                            int damage = 0;
                            if (thisSpellEffect.damAttacksEveryNLevels == 0) //this damage is not level based
                            {
                                damage = RandDiceRoll(thisSpellEffect.damNumOfDice, thisSpellEffect.damDie) + thisSpellEffect.damAdder;
                            }
                            else //this damage is level based
                            {
                                int numberOfDamAttacks = ((classLevel - thisSpellEffect.damAttacksAfterLevelN) / thisSpellEffect.damAttacksEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                                if (numberOfDamAttacks > thisSpellEffect.damAttacksUpToNLevelsTotal) { numberOfDamAttacks = thisSpellEffect.damAttacksUpToNLevelsTotal; } //can't have more than a max amount of attacks
                                for (int j = 0; j < numberOfDamAttacks; j++)
                                {
                                    damage += RandDiceRoll(thisSpellEffect.damNumOfDice, thisSpellEffect.damDie) + thisSpellEffect.damAdder;
                                }
                            }
                            #endregion
                            #region Do Calc Save and DC
                            int saveChkRoll = RandInt(20);
                            int saveChk = 0;
                            int DC = 0;
                            int saveChkAdder = 0;
                            if (thisSpellEffect.saveCheckType.Equals("will"))
                            {
                                saveChkAdder = crt.getWill();
                            }
                            else if (thisSpellEffect.saveCheckType.Equals("reflex"))
                            {
                                saveChkAdder = crt.getReflex();
                            }
                            else if (thisSpellEffect.saveCheckType.Equals("fortitude"))
                            {
                                saveChkAdder = crt.getFortitude();
                            }
                            else
                            {
                                saveChkAdder = -99;
                            }
                            saveChk = saveChkRoll + saveChkAdder;
                            DC = thisSpellEffect.saveCheckDC;
                            #endregion
                            //europa
                            if (saveChk >= DC) //passed save check (do half or avoid all?)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " makes successful " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRoll.ToString() + "+" + saveChkAdder + ">=" + DC.ToString() + ")" + "</font><BR>");
                                    if (thisSpellEffect.saveOnlyHalvesDamage)
                                    {
                                        damage = damage / 2;
                                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " takes only half damage from " + thisSpellEffect.name + "</font><BR>");
                                    }
                                    else
                                    {
                                        damage = 0;
                                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " takes no damage from " + thisSpellEffect.name + "</font><BR>");
                                    }
                                
                                //damage = damage / 2;
                                //gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " takes only half damage from " + thisSpellEffect.name + "</font><BR>");
                            }
                            else //failed save check or no save check allowed
                            {
                                //failed save roll
                                if (saveChkAdder > -99)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " failed " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRoll.ToString() + "+" + saveChkAdder + " < " + DC.ToString() + ")" + "</font><BR>");
                                }
                                else//no save roll allowed
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + "No saving roll allowed" + "</font><BR>");
                                }
                            }

                            if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + saveChkAdder + " >= " + DC + "</font><BR>"); }
                            if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + "</font><BR>"); }

                            int damageAndResist = (int)((float)damage * resist);
                            damageTotal += damageAndResist;
                            //resistance exists
                            if (resist < 1)
                            {
                                gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                + crt.cr_name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "(-" + ((1 - resist) * 100f) + "% resistance)" + "</font><BR>");
                            }
                            //vulnerability exists
                            else if (resist > 1)
                            {
                                gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                + crt.cr_name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "(+" + ((resist - 1) * 100f) + "% vulnerability)" + "</font><BR>");
                            }
                            //neither resistance nor vulnerability
                            else
                            {
                                gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                + crt.cr_name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                            }
                        }
                        crt.hp -= damageTotal;
                        if (crt.hp <= 0)
                        {
                                //gv.screenCombat.deathAnimationLocations.Add(new Coordinate(crt.combatLocX, crt.combatLocY));
                                foreach (Coordinate coor in crt.tokenCoveredSquares)
                                {
                                    gv.screenCombat.deathAnimationLocations.Add(new Coordinate(coor.X, coor.Y));
                                }
                                gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                        }
                        //Do floaty text damage
                        //gv.screenCombat.floatyTextOn = true;
                        gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), damageTotal + "");
                        #endregion
                    }
                    if ((thisSpellEffect.doHeal) && (thisSpellEffect.durationInUnits == 0))
                    {
                        #region Do Heal
                        #region Calculate Heal
                        //(for reference) Heal: AdB+C for every D levels after level E up to F levels total
                        // heal += RandDieRoll(A,B) + C
                        int heal = 0;
                        if (thisSpellEffect.healActionsEveryNLevels == 0) //this heal is not level based
                        {
                            heal = RandDiceRoll(thisSpellEffect.healNumOfDice, thisSpellEffect.healDie) + thisSpellEffect.healAdder;
                        }
                        else //this heal is level based
                        {
                            int numberOfHealActions = ((classLevel - thisSpellEffect.healActionsAfterLevelN) / thisSpellEffect.healActionsEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                            if (numberOfHealActions > thisSpellEffect.healActionsUpToNLevelsTotal) { numberOfHealActions = thisSpellEffect.healActionsUpToNLevelsTotal; } //can't have more than a max amount of actions
                            for (int j = 0; j < numberOfHealActions; j++)
                            {
                                heal += RandDiceRoll(thisSpellEffect.healNumOfDice, thisSpellEffect.healDie) + thisSpellEffect.healAdder;
                            }
                        }
                            #endregion

                            if (thisSpellEffect.healHP)
                            {
                                crt.hp += heal;
                                if (crt.hp > crt.hpMax)
                                {
                                    crt.hp = crt.hpMax;
                                }
                                gv.cc.addLogText("<font color='lime'>" + crt.cr_name + " gains " + heal + " HPs" + "</font><BR>");
                                //Do floaty text heal
                                //gv.screenCombat.floatyTextOn = true;
                                gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), heal + "", "green");
                            }
                            else
                            {
                                crt.sp += heal;
                                gv.cc.addLogText("<font color='lime'>" + crt.cr_name + " gains " + heal + " SPs" + "</font><BR>");
                                //Do floaty text heal
                                //gv.screenCombat.floatyTextOn = true;
                                gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), heal + "", "green");
                            }
                        #endregion
                    }

                    /*
                    if (thisSpellEffect.doBuff)
                    {
                        #region Do Buff
                        int numberOfRounds = thisSpellEffect.durationInUnits / gv.mod.TimePerRound;
                        gv.cc.addLogText("<font color='lime'>" + thisSpellEffect.name + " is applied on " + crt.cr_name + " for " + numberOfRounds + " round(s)</font><BR>");
                        crt.AddEffectByObject(thisSpellEffect, classLevel);
                        #endregion
                    }
                    */

                    ///trying to keep old spells compatible, in the long run likely just rely on duration > 0
                    if ((thisSpellEffect.doDeBuff) || (thisSpellEffect.doDeBuff) || (thisSpellEffect.durationInUnits > 0))
                    {
                        #region (Try to) add to effect list of target
                        #region Do Calc Save and DC
                        int saveChkRoll = RandInt(20);
                        int saveChk = 0;
                        int DC = 0;
                        int saveChkAdder = 0;
                        if (thisSpellEffect.saveCheckType.Equals("will"))
                        {
                            saveChkAdder = crt.getWill();
                        }
                        else if (thisSpellEffect.saveCheckType.Equals("reflex"))
                        {
                            saveChkAdder = crt.getReflex();
                        }
                        else if (thisSpellEffect.saveCheckType.Equals("fortitude"))
                        {
                            saveChkAdder = crt.getFortitude();
                        }
                        else
                        {
                            saveChkAdder = -99;
                        }
                        saveChk = saveChkRoll + saveChkAdder;
                        DC = thisSpellEffect.saveCheckDC;
                        #endregion
                        //europa
                        if (saveChk >= DC) //passed save check
                        {
                            gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " makes successful " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRoll.ToString() + "+" + saveChkAdder + ">=" + DC.ToString() + ")" + " and avoids " + thisSpellEffect.name + " </font><BR>");
                            //gv.cc.addLogText("<font color='yellow'>" + "(" + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRoll.ToString() + "+" + saveChkAdder + ">=" + DC.ToString() + ")" + " and avoids the longer lasting effect of" + thisSpellEffect.name + " </font><BR>");
                            //gv.cc.addLogText("<font color='yellow'>" + "(" + saveChkRoll.ToString() + "+" + saveChkAdder + " < " + DC.ToString() + ")" + "</font><BR>");
                            //gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " avoids the " + thisSpellEffect.name + " effect.</font><BR>");
                        }
                        else//failed save roll or no roll allowed
                        {
                            //failed save roll
                            if (saveChkAdder > -99)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " failed " + thisSpellEffect.saveCheckType + " saving roll for " + thisSpellEffect.name + "(" + saveChkRoll.ToString() + "+" + saveChkAdder + " < " + DC.ToString() + ")" + "</font><BR>");
                            }
                            //else//no save roll allowed
                            //{
                            //gv.cc.addLogText("<font color='yellow'>" + "No save roll against longer lasting effect of " + thisSpellEffect.name + " allowed" + "</font><BR>");
                            //}
                            int numberOfRounds = thisSpellEffect.durationInUnits / gv.mod.TimePerRound;
                            gv.cc.addLogText("<font color='lime'>" + thisSpellEffect.name + " is applied on " + crt.cr_name + " for " + numberOfRounds + " round(s)</font><BR>");
                            crt.AddEffectByObject(thisSpellEffect, classLevel);
                            //gv.cc.doEffectScript(pc, thisSpellEffect);
                            }
                        #endregion
                    }
                    if (thisSpell.removeEffectTagList.Count > 0)
                    {
                            #region remove effects  
                            foreach (EffectTagForDropDownList efTag in thisSpell.removeEffectTagList)
                            {
                                for (int x = crt.cr_effectsList.Count - 1; x >= 0; x--)
                                {
                                    if (crt.cr_effectsList[x].tag.Equals(efTag.tag))
                                    {
                                        try
                                        {
                                            crt.cr_effectsList.RemoveAt(x);
                                        }
                                        catch (Exception ex)
                                        {
                                            gv.errorLog(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                else //target is Player
                {
                    //europa
                    Player pc = (Player)target;
                    bool skip = false;

                    //go through creature local vars and compare with this spellEffect's affectOnly and affectNever lists

                    //when finding a matching apply never, skip
                    foreach (LocalImmunityString s in thisSpellEffect.affectNeverList)
                    {
                        foreach (string ls in pc.knownTraitsTags)
                        {
                            if (s.Value.Equals(ls))
                            {
                                skip = true;
                                gv.cc.addLogText("<font color='yellow'>" + pc.name + " is immune to " + thisSpellEffect.name + "</font><BR>");
                                break;
                            }
                        }

                        if (skip)
                        {
                            break;
                        }
                    }

                    //when finding an entry in affectOnlyList, skip unless it matches
                    if (!skip)
                    {
                        if (thisSpellEffect.affectOnlyList.Count > 0)
                        {
                            skip = true;

                            foreach (LocalImmunityString s in thisSpellEffect.affectOnlyList)
                            {
                                foreach (string ls in pc.knownTraitsTags)
                                {
                                    if (s.Value.Equals(ls))
                                    {
                                        skip = false;
                                        break;
                                    }
                                }
                                if (skip)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (!skip)
                    {
                        if ((thisSpellEffect.doDamage) && (thisSpellEffect.durationInUnits == 0))
                        {
                            #region Do Damage
                            #region Get Resistances
                            float resistPc = 0;
                            if (thisSpellEffect.damType.Equals("Normal")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalNormal / 100f)); }
                            else if (thisSpellEffect.damType.Equals("Acid")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalAcid / 100f)); }
                            else if (thisSpellEffect.damType.Equals("Cold")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalCold / 100f)); }
                            else if (thisSpellEffect.damType.Equals("Electricity")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalElectricity / 100f)); }
                            else if (thisSpellEffect.damType.Equals("Fire")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f)); }
                            else if (thisSpellEffect.damType.Equals("Magic")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalMagic / 100f)); }
                            else if (thisSpellEffect.damType.Equals("Poison")) { resistPc = (float)(1f - ((float)pc.damageTypeResistanceTotalPoison / 100f)); }
                            #endregion
                            int damageTotal = 0;
                            #region Calculate Number of Attacks
                            //(for reference) NumOfAttacks: A of these attacks for every B levels after level C up to D attacks total                    
                            int numberOfAttacks = 0;
                            if (thisSpellEffect.damNumberOfAttacksForEveryNLevels == 0) //this effect is using a fixed amount of attacks
                            {
                                numberOfAttacks = thisSpellEffect.damNumberOfAttacks;
                            }
                            else //this effect is using a variable amount of attacks
                            {
                                //numberOfAttacks = (((classLevel - C) / B) + 1) * A;
                                numberOfAttacks = (((classLevel - thisSpellEffect.damNumberOfAttacksAfterLevelN) / thisSpellEffect.damNumberOfAttacksForEveryNLevels) + 1) * thisSpellEffect.damNumberOfAttacks; //ex: 1 bolt for every 2 levels after level 1
                            }
                            if (numberOfAttacks > thisSpellEffect.damNumberOfAttacksUpToNAttacksTotal) { numberOfAttacks = thisSpellEffect.damNumberOfAttacksUpToNAttacksTotal; } //can't have more than a max amount of attacks
                            #endregion
                            //loop over number of attacks
                            for (int i = 0; i < numberOfAttacks; i++)
                            {
                                #region Calculate Damage
                                //(for reference) Attack: AdB+C for every D levels after level E up to F levels total
                                // damage += RandDieRoll(A,B) + C
                                //int damage = (int)((1 * RandInt(4) + 1) * resist);
                                int damagePc = 0;
                                if (thisSpellEffect.damAttacksEveryNLevels == 0) //this damage is not level based
                                {
                                    damagePc = RandDiceRoll(thisSpellEffect.damNumOfDice, thisSpellEffect.damDie) + thisSpellEffect.damAdder;
                                }
                                else //this damage is level based
                                {
                                    int numberOfDamAttacks = ((classLevel - thisSpellEffect.damAttacksAfterLevelN) / thisSpellEffect.damAttacksEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                                    if (numberOfDamAttacks > thisSpellEffect.damAttacksUpToNLevelsTotal) { numberOfDamAttacks = thisSpellEffect.damAttacksUpToNLevelsTotal; } //can't have more than a max amount of attacks
                                    for (int j = 0; j < numberOfDamAttacks; j++)
                                    {
                                        damagePc += RandDiceRoll(thisSpellEffect.damNumOfDice, thisSpellEffect.damDie) + thisSpellEffect.damAdder;
                                    }
                                }
                                #endregion
                                #region Do Calc Save and DC
                                int saveChkRollPc = RandInt(20);
                                int saveChkPc = 0;
                                int DCPc = 0;
                                int saveChkAdder = 0;
                                if (thisSpellEffect.saveCheckType.Equals("will"))
                                {
                                    saveChkAdder = pc.will;
                                }
                                else if (thisSpellEffect.saveCheckType.Equals("reflex"))
                                {
                                    saveChkAdder = pc.reflex;
                                }
                                else if (thisSpellEffect.saveCheckType.Equals("fortitude"))
                                {
                                    saveChkAdder = pc.fortitude;
                                }
                                else
                                {
                                    saveChkAdder = -99;
                                }
                                saveChkPc = saveChkRollPc + saveChkAdder;
                                DCPc = thisSpellEffect.saveCheckDC;
                                #endregion

                                if (saveChkPc >= DCPc) //passed save check (do half or avoid all?)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + pc.name + " makes successful " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRollPc.ToString() + "+" + saveChkAdder + ">=" + DCPc.ToString() + ")" + "</font><BR>");
                                    if (thisSpellEffect.saveOnlyHalvesDamage)
                                    {
                                        damagePc = damagePc / 2;
                                        gv.cc.addLogText("<font color='yellow'>" + pc.name + " takes only half damage from " + thisSpellEffect.name + "</font><BR>");
                                    }
                                    else
                                    {
                                        damagePc = 0;
                                        gv.cc.addLogText("<font color='yellow'>" + pc.name + " takes no damage from " + thisSpellEffect.name + "</font><BR>");
                                    }

                                    //damagePc = damagePc / 2;
                                    //gv.cc.addLogText("<font color='yellow'>" + pc.name + " takes only half damage from " + thisSpellEffect.name + "</font><BR>");
                                }
                                else //failed save check or no save check allowed
                                {
                                    //failed save roll
                                    if (saveChkAdder > -99)
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + pc.name + " failed " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRollPc.ToString() + "+" + saveChkAdder + " < " + DCPc.ToString() + ")" + "</font><BR>");
                                    }
                                    else//no save roll allowed
                                    {
                                        gv.cc.addLogText("<font color='yellow'>" + "No saving roll allowed" + "</font><BR>");
                                    }
                                }

                                if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + saveChkRollPc + " + " + saveChkAdder + " >= " + DCPc + "</font><BR>"); }
                                if (mod.debugMode) { gv.cc.addLogText("<font color='yellow'>" + "resist = " + resistPc + " damage = " + damagePc + "</font><BR>"); }

                                int damageAndResist = (int)((float)damagePc * resistPc);
                                damageTotal += damageAndResist;
                                //europa
                                //resistance exists
                                if (resistPc < 1)
                                {
                                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                    + pc.name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                    + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "(-" + ((1 - resistPc) * 100f) + "% resistance)" + "</font><BR>");
                                }
                                //vulnerability exists
                                else if (resistPc > 1)
                                {
                                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                    + pc.name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                    + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "(+" + ((resistPc - 1) * 100f) + "% vulnerability)" + "</font><BR>");
                                }
                                //neither resistance nor vulnerability
                                else
                                {
                                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " damages " + "</font>" + "<font color='silver'>"
                                                    + pc.name + "</font>" + "<font color='white'>" + "with " + thisSpellEffect.name + " (" + "</font>" + "<font color='lime'>"
                                                    + damageAndResist + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                                }
                            }

                            pc.hp -= damageTotal;
                            if (pc.hp <= 0)
                            {
                                if (pc.hp <= -20)
                                {
                                    gv.screenCombat.deathAnimationLocations.Add(new Coordinate(pc.combatLocX, pc.combatLocY));
                                }
                                gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                                pc.charStatus = "Dead";
                            }
                            //Do floaty text damage
                            //gv.screenCombat.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), damageTotal + "");
                            #endregion
                        }
                        if ((thisSpellEffect.doHeal) && (thisSpellEffect.durationInUnits == 0))
                        {
                            #region Do Heal
                            if (pc.hp <= -20)
                            {
                                //MessageBox("Can't heal a dead character!");
                                gv.cc.addLogText("<font color='red'>" + "Can't heal a dead character!" + "</font><BR>");
                            }
                            else
                            {
                                #region Calculate Heal
                                //(for reference) Heal: AdB+C for every D levels after level E up to F levels total
                                // heal += RandDieRoll(A,B) + C
                                int heal = 0;
                                if (thisSpellEffect.healActionsEveryNLevels == 0) //this heal is not level based
                                {
                                    heal = RandDiceRoll(thisSpellEffect.healNumOfDice, thisSpellEffect.healDie) + thisSpellEffect.healAdder;
                                }
                                else //this heal is level based
                                {
                                    int numberOfHealActions = ((classLevel - thisSpellEffect.healActionsAfterLevelN) / thisSpellEffect.healActionsEveryNLevels) + 1; //ex: 1 bolt for every 2 levels after level 1
                                    if (numberOfHealActions > thisSpellEffect.healActionsUpToNLevelsTotal) { numberOfHealActions = thisSpellEffect.healActionsUpToNLevelsTotal; } //can't have more than a max amount of actions
                                    for (int j = 0; j < numberOfHealActions; j++)
                                    {
                                        heal += RandDiceRoll(thisSpellEffect.healNumOfDice, thisSpellEffect.healDie) + thisSpellEffect.healAdder;
                                    }
                                }
                                #endregion
                                if (thisSpellEffect.healHP)
                                {
                                    pc.hp += heal;
                                    if (pc.hp > pc.hpMax)
                                    {
                                        pc.hp = pc.hpMax;
                                    }
                                    if (pc.hp > 0)
                                    {
                                        pc.charStatus = "Alive";
                                    }
                                    gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + heal + " HPs" + "</font><BR>");
                                    //Do floaty text heal
                                    //gv.screenCombat.floatyTextOn = true;
                                    gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), heal + "", "green");
                                }
                                else
                                {
                                    pc.sp += heal;
                                    if (pc.sp > pc.spMax)
                                    {
                                        pc.sp = pc.spMax;
                                    }
                                    gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + heal + " SPs" + "</font><BR>");
                                    //Do floaty text heal
                                    //gv.screenCombat.floatyTextOn = true;
                                    gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), heal + "", "green");
                                }
                            }
                            #endregion
                        }
                        /*
                        if (thisSpellEffect.doBuff)
                        {
                            #region Do Buff
                            int numberOfRounds = thisSpellEffect.durationInUnits / gv.mod.TimePerRound;
                            gv.cc.addLogText("<font color='lime'>" + thisSpellEffect.name + " is applied on " + pc.name + " for " + numberOfRounds + " round(s)</font><BR>");
                            pc.AddEffectByObject(thisSpellEffect, classLevel);
                            #endregion
                        }
                        */

                        //europa2
                        //trying to keep this compatible with old spells, in the long run only duration units should be relevant? 
                        if ((thisSpellEffect.doDeBuff) || (thisSpellEffect.doBuff) || (thisSpellEffect.durationInUnits > 0))
                        {
                            #region (Try to) add to target's effect list
                            #region Do Calc Save and DC
                            int saveChkRoll = RandInt(20);
                            int saveChk = 0;
                            int DC = 0;
                            int saveChkAdder = 0;
                            if (thisSpellEffect.saveCheckType.Equals("will"))
                            {
                                saveChkAdder = pc.will;
                            }
                            else if (thisSpellEffect.saveCheckType.Equals("reflex"))
                            {
                                saveChkAdder = pc.reflex;
                            }
                            else if (thisSpellEffect.saveCheckType.Equals("fortitude"))
                            {
                                saveChkAdder = pc.fortitude;
                            }
                            else
                            {
                                saveChkAdder = -99;
                            }
                            saveChk = saveChkRoll + saveChkAdder;
                            DC = thisSpellEffect.saveCheckDC;
                            #endregion
                            if (saveChk >= DC) //passed save check
                            {
                                gv.cc.addLogText("<font color='yellow'>" + pc.name + " makes successful " + thisSpellEffect.saveCheckType + " saving roll (" + saveChkRoll.ToString() + "+" + saveChkAdder + ">=" + DC.ToString() + ")" + " and avoids " + thisSpellEffect.name + " </font><BR>");
                            }
                            else//failed save roll or no roll allowed
                            {
                                //failed save roll
                                if (saveChkAdder > -99)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + pc.name + " failed " + thisSpellEffect.saveCheckType + " saving roll for " + thisSpellEffect.name + "(" + saveChkRoll.ToString() + "+" + saveChkAdder + " < " + DC.ToString() + ")" + "</font><BR>");
                                    //gv.cc.addLogText("<font color='yellow'>" + pc.name + " failed " + thisSpellEffect.saveCheckType + " saving roll against " + thisSpellEffect.name + "</font><BR>");
                                    //gv.cc.addLogText("<font color='yellow'>" + "(" + saveChkRoll.ToString() + "+" + saveChkAdder.ToString() + "<" + DC.ToString() + ")" + "</font><BR>");
                                }
                                else//no save roll allowed
                                {
                                    //gv.cc.addLogText("<font color='yellow'>" + "No saving roll allowed against longer lasting effect of " + thisSpellEffect.name + "</font><BR>");
                                }
                                //int numberOfRounds = thisSpellEffect.durationInUnits / gv.mod.TimePerRound;
                                //gv.cc.addLogText("<font color='lime'>" + thisSpellEffect.name + " is applied on " + pc.name + " for " + numberOfRounds + " round(s)</font><BR>");
                                pc.AddEffectByObject(thisSpellEffect, classLevel);
                                gv.cc.doEffectScript(pc, thisSpellEffect);
                            }
                            #endregion
                        }
                        if (thisSpell.removeEffectTagList.Count > 0)
                        {
                            #region remove effects  
                            foreach (EffectTagForDropDownList efTag in thisSpell.removeEffectTagList)
                            {
                                for (int x = pc.effectsList.Count - 1; x >= 0; x--)
                                {
                                    if (pc.effectsList[x].tag.Equals(efTag.tag))
                                    {
                                        try
                                        {
                                            pc.effectsList.RemoveAt(x);
                                        }
                                        catch (Exception ex)
                                        {
                                            gv.errorLog(ex.ToString());
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                 }
                }
                #endregion

                #region remove dead creatures            
                /*for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
                {
                    if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                    {
                        try
                        {
                            //do OnDeath IBScript
                            gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                            mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                            mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                        }
                        catch (Exception ex)
                        {
                            gv.errorLog(ex.ToString());
                        }
                    }
                }*/
                #endregion

                //            gv.postDelayed("doFloatyText", 100);
        }

        public void trRemoveTrap(object src, object trg)
        {  
           if (src is Player) //player casting  
            {  
                Player source = (Player)src;  
                Coordinate target = (Coordinate)trg;  
  
                 foreach(Prop prp in gv.mod.currentEncounter.propsList)  
                 {  
                    if ((prp.LocationX == target.X) && (prp.LocationY == target.Y))  
                    {  
                         if (prp.isTrap)  
                         {  
                             gv.mod.currentEncounter.propsList.Remove(prp);  
                             gv.cc.addLogText("<gn>" + source.name + " removed trap</gn><BR>");  
                             gv.cc.addFloatyText(new Coordinate(target.X, target.X), "trap removed", "green");  
                             return;  
                         }  
                     }  
                 }  
             }              
        }  

        //SPELLS WIZARD
        public void spDimensionDoor(object src, object trg)
         {  
             if (src is Player) //player casting  
             {  
                 Player source = (Player)src;  
                 Coordinate target = (Coordinate)trg;  
   
                 if (IsSquareOpen(target))  
                 {  
                     gv.cc.addLogText("<gn>" + source.name + " teleports to another location</gn><BR>");  
                     source.combatLocX = target.X;  
                     source.combatLocY = target.Y;
                    if (!source.thisCastIsFreeOfCost)
                    {
                        source.sp -= gv.cc.currentSelectedSpell.costSP;
                        if (source.sp < 0) { source.sp = 0; }

                        if (source.hp > gv.cc.currentSelectedSpell.costHP)
                        {
                            source.hp -= gv.cc.currentSelectedSpell.costHP;
                        }
                    }
                 }  
                 else  
                 {  
                     gv.cc.addLogText("<yl>" + source.name + " fails to teleport, square is already occupied or not valid</yl><BR>");  
                 }  
            }  
             else if (src is Creature) //creature casting  
             {  
                 Creature source = (Creature)src;  
                 Coordinate target = (Coordinate)trg;  
  
                 if (IsSquareOpen(target))  
                 {  
                     gv.cc.addLogText("<gn>" + source.cr_name + " teleports to another location</gn><BR>");  
                     source.combatLocX = target.X;  
                     source.combatLocY = target.Y;  
                     source.sp -= SpellToCast.costSP;  
                     if (source.sp< 0) { source.sp = 0; }  
                 }  
                 else  
                 {  
                     gv.cc.addLogText("<yl>" + source.cr_name + " fails to teleport, square is already occupied or not valid</yl><BR>");  
                 }  
             }  
         }

        public void spSummonAlly(object src, object trg, Spell sp)
        {
            if (src is Player) //player casting  
            {
                Player source = (Player)src;
                Coordinate target = (Coordinate)trg;

                if (IsSquareOpen(target))
                {
                    gv.cc.addLogText("<gn>" + source.name + " calls for a " + sp.spellScriptParm1 + "</gn><BR>");
                    //hersel
                    int parm2 = Convert.ToInt32(sp.spellScriptParm2);
                    gv.sf.AddTemporaryAllyForThisEncounter(sp.spellScriptParm1, target.X, target.Y, parm2);
                    if (!source.thisCastIsFreeOfCost)
                    {
                        source.sp -= gv.cc.currentSelectedSpell.costSP;
                        if (source.sp < 0) { source.sp = 0; }

                        if (source.hp > gv.cc.currentSelectedSpell.costHP)
                        {
                            source.hp -= gv.cc.currentSelectedSpell.costHP;
                        }
                    }
                }
                else
                {
                    gv.cc.addLogText("<yl>" + source.name + " fails to call ally, square is already occupied or not valid</yl><BR>");
                }
            }
            //gaAddCreatureToCurrentEncounter.cs
            //AddCreatureToCurrentEncounter(p1, p2, p3, p4);

            else if (src is Creature) //creature casting  
            {
                Creature source = (Creature)src;
                Coordinate target = (Coordinate)trg;
                bool foundPlace = true;

                //holla
                //we must determine the size of the summoned creature
                Creature summon = new Creature();
                foreach (Creature c in gv.mod.moduleCreaturesList)
                {
                    if (c.cr_resref == sp.spellScriptParm1)
                    {
                        summon.creatureSize = c.creatureSize;
                    }
                }

                Coordinate plusX = new Coordinate();
                plusX.X = target.X + 1;
                plusX.Y = target.Y;
                Coordinate plusY = new Coordinate();
                plusY.X = target.X;
                plusY.Y = target.Y + 1;
                Coordinate plusXandY = new Coordinate();
                plusXandY.X = target.X + 1;
                plusXandY.Y = target.Y + 1;

                if (summon.creatureSize == 1)
                {
                    if (!IsSquareOpen(target))
                    {
                        foundPlace = false;
                    }
                }

                if (summon.creatureSize == 2)
                {
                   
                    if (!IsSquareOpen(target))
                    {
                        foundPlace = false;
                    }

                    if (plusX.X < gv.mod.currentEncounter.MapSizeX)
                    {
                        if (!IsSquareOpen(plusX))
                        {
                            foundPlace = false;
                        }
                    }
                    else
                    {
                        foundPlace = false;
                    }
                }

                if (summon.creatureSize == 3)
                {

                    if (!IsSquareOpen(target))
                    {
                        foundPlace = false;
                    }

                    if (plusY.Y < gv.mod.currentEncounter.MapSizeY)
                    {
                        if (!IsSquareOpen(plusY))
                        {
                            foundPlace = false;
                        }
                    }
                    else
                    {
                        foundPlace = false;
                    }
                }

                if (summon.creatureSize == 4)
                {

                    if (!IsSquareOpen(target))
                    {
                        foundPlace = false;
                    }

                    if (plusX.X < gv.mod.currentEncounter.MapSizeX)
                    {
                        if (!IsSquareOpen(plusX))
                        {
                            foundPlace = false;
                        }
                    }
                    else
                    {
                        foundPlace = false;
                    }

                    if (plusY.Y < gv.mod.currentEncounter.MapSizeY)
                    {
                        if (!IsSquareOpen(plusY))
                        {
                            foundPlace = false;
                        }
                    }
                    else
                    {
                        foundPlace = false;
                    }

                    if (plusXandY.X < gv.mod.currentEncounter.MapSizeX && plusXandY.Y < gv.mod.currentEncounter.MapSizeY)
                    {
                        if (!IsSquareOpen(plusXandY))
                        {
                            foundPlace = false;
                        }
                    }
                    else
                    {
                        foundPlace = false;
                    }
                }

                //try to find a nearby square
                if (foundPlace)
                {
                    gv.cc.addLogText("<gn>" + source.cr_name + " calls for a " + sp.spellScriptParm1 + "</gn><BR>");
                    //p1 is the resref of the added creature(use one from a blueprint in the toolset's creature blueprints section)  
                    //p2 is the duration in turns that the creature will stay
                    gv.sf.AddCreatureToCurrentEncounter(sp.spellScriptParm1, target.X.ToString(), target.Y.ToString(), sp.spellScriptParm2);
                    source.hp -= SpellToCast.costHP;
                    if (source.hp < 0) { source.hp = 1; }
                    source.sp -= SpellToCast.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                }
                else
                {
                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    //find correct summon spot, replace with nearest location if neccessary  
              
                    bool changeSummonLocation = false;// used as switch for cycling through all tiles in case the originally intended spot was occupied/not-walkable  
                    int targetTile = target.Y * gv.mod.currentEncounter.MapSizeX + target.X;//the index of the original target spot in the encounter's tiles list  
                    List<int> freeTilesByIndex = new List<int>();// a new list used to store the indices of all free tiles in the enocunter  
                    int tileLocX = 0;//just temporary storage in for locations of tiles  
                    int tileLocY = 0;//just temporary storage in for locations of tiles  
                    double floatTileLocY = 0;//was uncertain about rounding and conversion details, therefore need this one (see below)  
                    bool tileIsFree = true;//identify a tile suited as new summon loaction  
                    int nearestTileByIndex = -1;//store the nearest tile by index; as the relevant loop runs this will be replaced several times likely with ever nearer tiles  
                    int dist = 0;//distance between the orignally intended summon location and a free tile  
                    int lowestDist = 10000;//this storest the lowest ditance found while the loop runs  
                    int deltaX = 0;//temporary value used for distance calculation   
                    int deltaY = 0;//temporary value used for distance calculation   

                    //Check whether the target tile is free (then it's not neccessary to loop through any other tiles)  
                    //three checks are done in the following: walkable, occupied by creature, occupied by pc  

                    //TODO: for oversized cretaures
                    //which squares will the cretaure cover

                    //first check: check walkable  
                    //if (gv.mod.currentEncounter.encounterTiles[targetTile].Walkable == false)
                    /*
                    if (gv.mod.currentEncounter.encounterTiles[targetTile].Walkable == false)
                    {
                        changeSummonLocation = true;
                    }

                    //second check: check occupied by creature (only necceessary if walkable)  
                    if (changeSummonLocation == false)
                    {
                        foreach (Creature cr in gv.mod.currentEncounter.encounterCreatureList)
                        {
                            if ((cr.combatLocX == target.X) && (cr.combatLocY == target.Y))
                            {
                                changeSummonLocation = true;
                                break;
                            }
                        }
                    }

                    //third check: check occupied by pc (only necceessary if walkable and not occupied by creature)  
                    if (changeSummonLocation == false)
                    {
                        foreach (Player pc in gv.mod.playerList)
                        {
                            if ((pc.combatLocX == target.X) && (pc.combatLocY == target.Y))
                            {
                                changeSummonLocation = true;
                                break;
                            }
                        }
                    }
                    */
                    changeSummonLocation = true;
                    Coordinate target2 = new Coordinate();
                    //target square was already occupied/non-walkable, so all other tiles are searched for the NEAREST FREE tile to switch the summon location to  
                    if (changeSummonLocation == true)
                    {
                        //FIRST PART: get all FREE tiles in the current encounter  
                        for (int i = 0; i < gv.mod.currentEncounter.encounterTiles.Count; i++)
                        {
                            //get the x and y location of current tile by calculation derived from index number, assuming that counting starts at top left corner of a map (0x, 0y)  
                            //and that each horizintal x-line is counted first, then counting next horizonal x-line starting from the left again  
                            tileIsFree = true;
                            //Note: When e.g. MapsizeY is 7, the y values range from 0 to 6  
                            //MODULO
                            tileLocX = i % gv.mod.currentEncounter.MapSizeX;
                            //Note: ensure rounding down here   
                            floatTileLocY = i / gv.mod.currentEncounter.MapSizeX;
                            tileLocY = (int)Math.Floor(floatTileLocY);
                            target2.X = tileLocX;
                            target2.Y = tileLocY;

                            //code for large summons goes here, see above
                            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                            plusX.X = target2.X + 1;
                            plusX.Y = target2.Y;
                            plusY.X = target2.X;
                            plusY.Y = target2.Y + 1;
                            plusXandY.X = target2.X + 1;
                            plusXandY.Y = target2.Y + 1;

                            foundPlace = true;

                            if (summon.creatureSize == 1)
                            {
                                if (!IsSquareOpen(target2))
                                {
                                    foundPlace = false;
                                }
                            }

                            if (summon.creatureSize == 2)
                            {

                                if (!IsSquareOpen(target2))
                                {
                                    foundPlace = false;
                                }

                                if (plusX.X < gv.mod.currentEncounter.MapSizeX)
                                {
                                    if (!IsSquareOpen(plusX))
                                    {
                                        foundPlace = false;
                                    }
                                }
                                else
                                {
                                    foundPlace = false;
                                }
                            }

                            if (summon.creatureSize == 3)
                            {

                                if (!IsSquareOpen(target2))
                                {
                                    foundPlace = false;
                                }

                                if (plusY.Y < gv.mod.currentEncounter.MapSizeY)
                                {
                                    if (!IsSquareOpen(plusY))
                                    {
                                        foundPlace = false;
                                    }
                                }
                                else
                                {
                                    foundPlace = false;
                                }
                            }

                            if (summon.creatureSize == 4)
                            {

                                if (!IsSquareOpen(target2))
                                {
                                    foundPlace = false;
                                }

                                if (plusX.X < gv.mod.currentEncounter.MapSizeX)
                                {
                                    if (!IsSquareOpen(plusX))
                                    {
                                        foundPlace = false;
                                    }
                                }
                                else
                                {
                                    foundPlace = false;
                                }

                                if (plusY.Y < gv.mod.currentEncounter.MapSizeY)
                                {
                                    if (!IsSquareOpen(plusY))
                                    {
                                        foundPlace = false;
                                    }
                                }
                                else
                                {
                                    foundPlace = false;
                                }

                                if (plusXandY.X < gv.mod.currentEncounter.MapSizeX && plusXandY.Y < gv.mod.currentEncounter.MapSizeY)
                                {
                                    if (!IsSquareOpen(plusXandY))
                                    {
                                        foundPlace = false;
                                    }
                                }
                                else
                                {
                                    foundPlace = false;
                                }
                            }


                            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                            if (foundPlace)
                            {
                                tileIsFree = true;
                            }
                            else
                            {
                                tileIsFree = false;
                            }
                            /*
                                //look at content of currently checked tile, again with three checks for walkable, occupied by creature, occupied by pc  
                                //walkbale check  
                                if (gv.mod.currentEncounter.encounterTiles[i].Walkable == false)
                            {
                                tileIsFree = false;
                            }

                            //creature occupied check  
                            if (tileIsFree == true)
                            {
                                foreach (Creature cr in gv.mod.currentEncounter.encounterCreatureList)
                                {
                                    if ((cr.combatLocX == tileLocX) && (cr.combatLocY == tileLocY))
                                    {
                                        tileIsFree = false;
                                        break;
                                    }
                                }
                            }

                            //pc occupied check  
                            if (tileIsFree == true)
                            {
                                foreach (Player pc in gv.mod.playerList)
                                {
                                    if ((pc.combatLocX == tileLocX) && (pc.combatLocY == tileLocY))
                                    {
                                        tileIsFree = false;
                                        break;
                                    }
                                }
                            }
                            */

                            //this writes all free tiles into a fresh list; please note that the values of the elements of this new list are our relevant index values  
                            //therefore it's not the index (which doesnt correalte to locations) in this list that's relevant, but the value of the element at that index  
                            if (tileIsFree == true)
                            {
                                freeTilesByIndex.Add(i);
                            }
                        }

                        //SECOND PART: find the free tile NEAREST to originally intended summon location  
                        for (int i = 0; i < freeTilesByIndex.Count; i++)
                        {
                            dist = 0;

                            //get location x and y of the tile stored at the index number i, i.e. get the value of elment indexed with i and transform to x and y location  
                            tileLocX = freeTilesByIndex[i] % gv.mod.currentEncounter.MapSizeX;
                            floatTileLocY = freeTilesByIndex[i] / gv.mod.currentEncounter.MapSizeX;
                            tileLocY = (int)Math.Floor(floatTileLocY);

                            //get distance between the current free tile and the originally intended summon location  
                            deltaX = (int)Math.Abs((tileLocX - target.X));
                            deltaY = (int)Math.Abs((tileLocY - target.Y));
                            if (deltaX > deltaY)
                            {
                                dist = deltaX;
                            }
                            else
                            {
                                dist = deltaY;
                            }

                            //filter out the nearest tile by remembering it and its distance for further comparison while the loop runs through all free tiles  
                            if (dist < lowestDist)
                            {
                                lowestDist = dist;
                                nearestTileByIndex = freeTilesByIndex[i];
                            }
                        }

                        if (nearestTileByIndex != -1)
                        {
                            //get the nearest tile's x and y location and use it as creature summon coordinates  
                            tileLocX = nearestTileByIndex % gv.mod.currentEncounter.MapSizeX;
                            floatTileLocY = nearestTileByIndex / gv.mod.currentEncounter.MapSizeX;
                            tileLocY = (int)Math.Floor(floatTileLocY);

                            target.X = tileLocX;
                            target.Y = tileLocY;
                        }

                    }

                    //just check whether a free squre does exist at all; if not, do not complete the summon  
                    if ((nearestTileByIndex != -1) || (changeSummonLocation == false))
                    {
                        gv.cc.addLogText("<gn>" + source.cr_name + " calls for a " + sp.spellScriptParm1 + "</gn><BR>");
                        //p1 is the resref of the added creature(use one from a blueprint in the toolset's creature blueprints section)  
                        //p2 is the duration in turns that the creature will stay
                        gv.sf.AddCreatureToCurrentEncounter(sp.spellScriptParm1, target.X.ToString(), target.Y.ToString(), sp.spellScriptParm2);
                        source.hp -= SpellToCast.costHP;
                        if (source.hp < 0) { source.hp = 1; }
                        source.sp -= SpellToCast.costSP;
                        if (source.sp < 0) { source.sp = 0; }
                    }
                    else
                    {
                        gv.cc.addLogText("<yl>" + source.cr_name + " fails to call ally, square is already occupied or not valid</yl><BR>");
                    }

                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                }
            }
           
        }

        public int CalcShopBuyBackModifier(Player pc)
         {  
             int positiveMod = 0;  
             int positiveStackableMod = 0;  
             int negativeMod = 0;  
             int negativeStackableMod = 0; 
            /* 
             //go through all traits and see if has passive rapidshot type trait, use largest, not cumulative  
             foreach (string taTag in pc.knownTraitsTags)  
             {  
                 Trait ta = mod.getTraitByTag(taTag);  
                 foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
                 {  
                     Effect ef = mod.getEffectByTag(efTag.tag);  
                     //replace non-stackable positive with highest value  
                     if ((ef.modifyShopBuyBackPrice > positiveMod) && (ef.isPermanent) && (!ef.isStackableEffect))  
                     {
                        if (isPassiveTraitApplied(ef, pc))
                        {
                            positiveMod = ef.modifyShopBuyBackPrice;
                        }
                     }  
                     //replace non-stackable negative with lowest value  
                     if ((ef.modifyShopBuyBackPrice<negativeMod) && (ef.isPermanent && (!ef.isStackableEffect))  
                     {
                        if (isPassiveTraitApplied(ef, pc))
                        {
                            negativeMod = ef.modifyShopBuyBackPrice;
                        }
                     }  
                     //if isStackable positive then pile on  
                     if ((ef.modifyShopBuyBackPrice > 0) && (ef.isPermanent) && (ef.isStackableEffect))  
                     {
                        if (isPassiveTraitApplied(ef, pc))
                        {
                            positiveStackableMod += ef.modifyShopBuyBackPrice;
                        }
                     }  
                     //if isStackable negative then pile on  
                     if ((ef.modifyShopBuyBackPrice< 0) && (ef.isPermanent) && (ef.isStackableEffect))  
                     {
                        if (isPassiveTraitApplied(ef, pc))
                        {
                            negativeStackableMod += ef.modifyShopBuyBackPrice;
                        }
                     }  
                 }  
             }
             */
               
             //go through each effect and see if has a buff type like rapidshot, use largest, not cumulative  
             foreach (Effect ef in pc.effectsList)  
             {  
                 //replace non-stackable positive with highest value  
                 if ((ef.modifyShopBuyBackPrice > positiveMod) && (!ef.isStackableEffect))  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        positiveMod = ef.modifyShopBuyBackPrice;
                    }
                 }  
                 //replace non-stackable negative with lowest value  
                 if ((ef.modifyShopBuyBackPrice<negativeMod) && (!ef.isStackableEffect))  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        negativeMod = ef.modifyShopBuyBackPrice;
                    }
                 }  
                 //if isStackable positive then pile on  
                 if ((ef.modifyShopBuyBackPrice > 0) && (ef.isStackableEffect))  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        positiveStackableMod += ef.modifyShopBuyBackPrice;
                    }
                 }  
                 //if isStackable negative then pile on  
                 if ((ef.modifyShopBuyBackPrice< 0) && (ef.isStackableEffect))  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        negativeStackableMod += ef.modifyShopBuyBackPrice;
                    }
                 }  
             }  
   
             int numOfPos = 0;  
             int numOfNeg = 0;  
             //check to see if stackable is greater than non-stackable and combine the highest positive and negative effect  
             if (positiveMod > positiveStackableMod)  
             {  
                 numOfPos = positiveMod;  
             }  
             else  
             {  
                 numOfPos = positiveStackableMod;  
             }  
             if (negativeMod<negativeStackableMod)  
             {  
                 numOfNeg = negativeMod;  
             }  
             else  
             {  
                 numOfNeg = negativeStackableMod;  
             }  
   
             return numOfPos + numOfNeg;  
         }

        public int CalcShopSellModifier(Player pc)
         {  
             int positiveMod = 0;  
             int positiveStackableMod = 0;  
             int negativeMod = 0;  
             int negativeStackableMod = 0; 
            /* 
             //go through all traits and see if has passive rapidshot type trait, use largest, not cumulative  
             foreach (string taTag in pc.knownTraitsTags)  
             {  
                 Trait ta = mod.getTraitByTag(taTag);  
                 foreach (EffectTagForDropDownList efTag in ta.traitEffectTagList)  
                 {  
                     Effect ef = mod.getEffectByTag(efTag.tag);  
                     //replace non-stackable positive with highest value  
                     if ((ef.modifyShopSellPrice > positiveMod) && (ta.isPassive) && (!ef.isStackableEffect))  
                     {  
                         positiveMod = ef.modifyShopSellPrice;  
                     }  
                     //replace non-stackable negative with lowest value  
                     if ((ef.modifyShopSellPrice<negativeMod) && (ta.isPassive) && (!ef.isStackableEffect))  
                     {  
                         negativeMod = ef.modifyShopSellPrice;  
                     }  
                     //if isStackable positive then pile on  
                     if ((ef.modifyShopSellPrice > 0) && (ta.isPassive) && (ef.isStackableEffect))  
                     {  
                         positiveStackableMod += ef.modifyShopSellPrice;  
                     }  
                     //if isStackable negative then pile on  
                     if ((ef.modifyShopSellPrice< 0) && (ta.isPassive) && (ef.isStackableEffect))  
                     {  
                         negativeStackableMod += ef.modifyShopSellPrice;  
                     }  
                 }  
             }
             */  
             //go through each effect and see if has a buff type like rapidshot, use largest, not cumulative  
             foreach (Effect ef in pc.effectsList)  
             {  
                 //replace non-stackable positive with highest value  
                 if ((ef.modifyShopSellPrice > positiveMod) && (!ef.isStackableEffect))  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        positiveMod = ef.modifyShopSellPrice;
                    }
                 }  
                 //replace non-stackable negative with lowest value  
                 if ((ef.modifyShopSellPrice<negativeMod) && (!ef.isStackableEffect))  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        negativeMod = ef.modifyShopSellPrice;
                    }
                 }  
                 //if isStackable positive then pile on  
                 if ((ef.modifyShopSellPrice > 0) && (ef.isStackableEffect))  
                 {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        positiveStackableMod += ef.modifyShopSellPrice;
                    }
                 }  
                 //if isStackable negative then pile on  
                 if ((ef.modifyShopSellPrice< 0) && (ef.isStackableEffect))
                {
                    if (isPassiveTraitApplied(ef, pc))
                    {
                        negativeStackableMod += ef.modifyShopSellPrice;
                    }
                 }  
             }  
   
             int numOfPos = 0;  
             int numOfNeg = 0;  
            //check to see if stackable is greater than non-stackable and combine the highest positive and negative effect  
             if (positiveMod > positiveStackableMod)  
             {  
                 numOfPos = positiveMod;  
             }  
             else  
             {  
                 numOfPos = positiveStackableMod;  
             }  
             if (negativeMod<negativeStackableMod)  
             {  
                 numOfNeg = negativeMod;  
             }  
             else  
             {  
                 numOfNeg = negativeStackableMod;  
             }  
   
             return numOfPos + numOfNeg;  
         }  



         public bool IsSquareOpen(Coordinate target)
         {  
             if (!gv.mod.currentEncounter.encounterTiles[target.Y * mod.currentEncounter.MapSizeX + target.X].Walkable)  
             {  
                 return false;  
             }  
             foreach (Player pc in gv.mod.playerList)  
             {  
                 if ((pc.combatLocX == target.X) && (pc.combatLocY == target.Y))  
                 {  
                     if (pc.isAlive())  
                     {  
                         return false;  
                     }  
                 }  
             }  
             foreach (Creature crt in gv.mod.currentEncounter.encounterCreatureList)  
             {  
                 if ((crt.combatLocX == target.X) && (crt.combatLocY == target.Y))  
                 {  
                     return false;  
                 }  

                 //check additional squares occupied by oversized creatures
                if (crt.creatureSize == 2)
                {
                    if ((crt.combatLocX + 1 == target.X) && (crt.combatLocY == target.Y))
                    {
                        return false;
                    }
                }

                if (crt.creatureSize == 3)
                {
                    if ((crt.combatLocX == target.X) && (crt.combatLocY + 1 == target.Y))
                    {
                        return false;
                    }
                }

                if (crt.creatureSize == 4)
                {
                    if ((crt.combatLocX + 1 == target.X) && (crt.combatLocY == target.Y))
                    {
                        return false;
                    }

                    if ((crt.combatLocX == target.X) && (crt.combatLocY + 1 == target.Y))
                    {
                        return false;
                    }

                    if ((crt.combatLocX + 1 == target.X) && (crt.combatLocY + 1 == target.Y))
                    {
                        return false;
                    }
                }
                }  
             return true;  
         }   

        public void spFlameFingers(object src, object trg, Spell thisSpell)
        {
            //set squares list
            CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);
            
            //set target list
            CreateAoeTargetsList(src, trg, thisSpell, false);
            
            //get casting source information
            int classLevel = 0;
            string sourceName = "";
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                classLevel = source.classLevel;
                sourceName = source.name;
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > thisSpell.costHP)
                    {
                        source.hp -= thisSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                classLevel = source.cr_level;
                sourceName = source.cr_name;
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
              else if (src is Item) //item was used
            {
                Item source = (Item)src;
                classLevel = source.levelOfItemForCastSpell;
                sourceName = source.name;
            }

            else if (src is Coordinate) //trigger or prop was used  
            {
                classLevel = 1;
                sourceName = "trigger";
            }

            //iterate over targets and do damage
            foreach (object target in AoeTargetsList)
            {
                if (target is Creature)
                {
                    Creature crt = (Creature)target;
                    float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                    float damage = classLevel * RandInt(3);
                    int fireDam = (int)(damage * resist);
                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + crt.getReflex();
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        fireDam = fireDam / 2;
                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " evades most of the Flame Fingers spell" + "</font><BR>");
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.getReflex() + " >= " + DC + "</font><BR>");
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " fireDam = " + fireDam + "</font><BR>");
                    }
                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " scorches " + "</font>" + "<font color='silver'>" + crt.cr_name + "</font><BR>");
                    gv.cc.addLogText("<font color='white'>" + "with Flame Fingers (" + "</font>" + "<font color='lime'>" + fireDam + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    crt.hp -= fireDam;
                    if (crt.hp <= 0)
                    {
                        gv.screenCombat.deathAnimationLocations.Add(new Coordinate(crt.combatLocX, crt.combatLocY));
                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), fireDam + "");                    
                }
                else //target is Player
                {
                    Player pc = (Player)target;
                    float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f));
                    float damage = classLevel * RandInt(3);
                    int fireDam = (int)(damage * resist);
                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + pc.reflex;
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        if (this.hasTrait(pc, "evasion"))
                        {
                            fireDam = 0;
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades all of the Flame Fingers spell" + "</font><BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                            }
                        }
                        else
                        {
                            fireDam = fireDam / 2;
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades most of the Flame Fingers spell" + "</font><BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                            }
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " fireDam = " + fireDam + "</font><BR>");
                    }
                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " scorches " + "</font>" + "<font color='silver'>" + pc.name + "</font><BR>");
                    gv.cc.addLogText("<font color='white'>" + "with Flame Fingers (" + "</font>" + "<font color='lime'>" + fireDam + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    pc.hp -= fireDam;
                    if (pc.hp <= 0)
                    {
                        if (pc.hp <= -20)
                        {
                            gv.screenCombat.deathAnimationLocations.Add(new Coordinate(pc.combatLocX, pc.combatLocY));
                        }
                        gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                        pc.charStatus = "Dead";
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), fireDam + "");
                }
            }
            
            //remove dead creatures            
            /*for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
            {
                if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                {
                    try
                    {
                        //do OnDeath IBScript
                        gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                        mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                        mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                    }
                    catch (Exception ex)
                    {
                        gv.errorLog(ex.ToString());
                    }
                }
            }*/
//            gv.postDelayed("doFloatyText", 100);
        }
        public void spMageBolt(object src, object trg)
        {
            //clear squares list
            gv.sf.AoeSquaresList.Clear();

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Creature target = (Creature)trg;

                int damageTotal = 0;
                int numberOfBolts = ((source.classLevel - 1) / 2) + 1; //1 bolt for every 2 levels after level 1
                if (numberOfBolts > 5) { numberOfBolts = 5; } //can not have more than 5 bolts
                for (int i = 0; i < numberOfBolts; i++)
                {
                    int damage = 1 * RandInt(4) + 1;
                    target.hp = target.hp - damage;
                    damageTotal += damage;
                    gv.cc.addLogText("<font color='aqua'>" + source.name + "</font>" +
                            "<font color='white'>" + " attacks " + "</font>" +
                            "<font color='silver'>" + target.cr_name + "</font>" +
                            "<BR>");
                    gv.cc.addLogText("<font color='white'>" + "Mage Bolt HITS (" + "</font>" +
                            "<font color='lime'>" + damage + "</font>" +
                            "<font color='white'>" + " damage)" + "</font>" +
                            "<BR>");
                    if (target.hp <= 0)
                    {
                        gv.screenCombat.deathAnimationLocations.Add(new Coordinate(target.combatLocX, target.combatLocY));
                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + target.cr_name + "</font><BR>");
                        /*try
                        {
                            for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
                            {
                                if (mod.currentEncounter.encounterCreatureList[x] == target)
                                {
                                    try
                                    {
                                        //do OnDeath LOGIC TREE
                                        //REMOVEgv.cc.doLogicTreeBasedOnTag(target.onDeathLogicTree, target.onDeathParms);
                                        //do OnDeath IBScript
                                        gv.cc.doIBScriptBasedOnFilename(target.onDeathIBScript, target.onDeathIBScriptParms);
                                        mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                                        mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                                    }
                                    catch (Exception ex)
                                    {
                                        gv.errorLog(ex.ToString());
                                    }
                                }
                            }

                            //mod.currentEncounter.encounterCreatureList.remove(target);
                        }
                        catch (Exception ex)
                        {
                            gv.errorLog(ex.ToString());
                        }*/
                    }
                }
                //Do floaty text damage
                //gv.screenCombat.floatyTextOn = true;
                gv.cc.addFloatyText(new Coordinate(target.combatLocX, target.combatLocY), damageTotal + "");
                //                gv.postDelayed("doFloatyText", 100);
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Player target = (Player)trg;

                int damageTotal = 0;
                int numberOfBolts = ((source.cr_level - 1) / 2) + 1; //1 bolt for every 2 levels after level 1
                if (numberOfBolts > 5) { numberOfBolts = 5; } //can not have more than 5 bolts
                for (int i = 0; i < numberOfBolts; i++)
                {
                    int damage = 1 * RandInt(4) + 1;

                    target.hp = target.hp - damage;
                    damageTotal += damage;
                    gv.cc.addLogText("<font color='silver'>" + source.cr_name + "</font>" +
                            "<font color='white'>" + " attacks " + "</font>" +
                            "<font color='aqua'>" + target.name + "</font>" +
                            "<BR>");
                    gv.cc.addLogText("<font color='white'>" + "Mage Bolt HITS (" + "</font>" +
                            "<font color='red'>" + damage + "</font>" +
                            "<font color='white'>" + " damage)" + "</font>" +
                            "<BR>");
                    if (target.hp <= 0)
                    {
                        if (target.hp <= -20)
                        {
                            gv.screenCombat.deathAnimationLocations.Add(new Coordinate(target.combatLocX, target.combatLocY));
                        }
                        gv.cc.addLogText("<font color='red'>" + target.name + " drops unconcious!" + "</font><BR>");
                        target.charStatus = "Dead";
                    }
                }
                //Do floaty text damage
                //gv.screenCombat.floatyTextOn = true;
                gv.cc.addFloatyText(new Coordinate(target.combatLocX, target.combatLocY), damageTotal + "");
//                gv.postDelayed("doFloatyText", 100);

                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
        }
        public void spSleep(object src, object trg, Spell thisSpell)
        {
            //set squares list
            CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);

            //set target list
            CreateAoeTargetsList(src, trg, thisSpell, false);

            //get casting source information
            int classLevel = 0;
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                classLevel = source.classLevel;
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else //creature casting
            {
                Creature source = (Creature)src;
                classLevel = source.cr_level;
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }

            //iterate over targets and do damage
            foreach (object target in AoeTargetsList)
            {
                if (target is Creature)
                {
                    Creature crt = (Creature)target;
                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + crt.getWill();
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " avoids the sleep spell" + "</font><BR>");
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.getWill() + " >= " + DC + "</font><BR>");
                        }
                    }
                    else //failed check
                    {
                        gv.cc.addLogText("<font color='red'>" + crt.cr_name + " is held by a sleep spell" + "</font><BR>");
                        crt.cr_status = "Held";
                        Effect ef = mod.getEffectByTag("sleep");
                        ef.statusType = "Held";
                        crt.AddEffectByObject(ef, classLevel);

                    }
                }
                else //target is Player
                {
                    Player pc = (Player)target;
                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + pc.will;
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        gv.cc.addLogText("<font color='yellow'>" + pc.name + " avoids the sleep spell" + "</font><BR>");
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.will + " >= " + DC + "</font><BR>");
                        }
                    }
                    else //failed check
                    {
                        gv.cc.addLogText("<font color='red'>" + pc.name + " is held by a sleep spell" + "</font><BR>");
                        pc.charStatus = "Held";
                        Effect ef = mod.getEffectByTag("sleep");
                        ef.statusType = "Held";
                        pc.AddEffectByObject(ef, classLevel);
                    }
                }
            }
        }
        public void spMageArmor(object src, object trg)
        {
            //clear squares list
            gv.sf.AoeSquaresList.Clear();

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Player target = (Player)trg;

                if (source != target)
                {
                    MessageBox("Mage Armor can only be applied to the caster...casting aborted");
                    return;
                }

                int numberOfRounds = (source.classLevel * 20); //20 rounds per level
                Effect ef = mod.getEffectByTag("mageArmor").DeepCopy();
                ef.durationInUnits = numberOfRounds * gv.mod.TimePerRound;
                gv.cc.addLogText("<font color='lime'>" + "Mage Armor is applied on " + target.name + "<BR>");
                gv.cc.addLogText("<font color='lime'>" + " for " + numberOfRounds + " round(s)" + "</font><BR>");
                target.AddEffectByObject(ef, source.classLevel);
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Creature target = (Creature)trg;

                int numberOfRounds = (source.cr_level * 20); //20 rounds per level
                Effect ef = mod.getEffectByTag("mageArmor").DeepCopy();
                ef.durationInUnits = numberOfRounds * gv.mod.TimePerRound;
                gv.cc.addLogText("<font color='lime'>" + "Mage Armor is applied on " + target.cr_name + "<BR>");
                gv.cc.addLogText("<font color='lime'>" + " for " + numberOfRounds + " round(s)" + "</font><BR>");
                target.AddEffectByObject(ef, source.cr_level);
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Coordinate)
            {
                //Toast.makeText(gv.gameContext, "source is not a PC or Creature", Toast.LENGTH_SHORT).show();
            }
            else
            {
                //Toast.makeText(gv.gameContext, "don't recognize source type", Toast.LENGTH_SHORT).show();			
            }
        }
        public void spMinorRegen(object src, object trg)
        {
            //clear squares list
            gv.sf.AoeSquaresList.Clear();

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Player target = (Player)trg;

                Effect ef = mod.getEffectByTag("minorRegen");
                gv.cc.addLogText("<font color='lime'>" + "Minor Regeneration is applied on " + target.name + "</font><BR>");
                target.AddEffectByObject(ef, source.classLevel);
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Creature target = (Creature)trg;

                Effect ef = mod.getEffectByTag("minorRegen");
                gv.cc.addLogText("<font color='lime'>" + "Minor Regeneration is applied on " + target.cr_name + "</font><BR>");
                target.AddEffectByObject(ef, source.cr_level);
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Coordinate)
            {
                //Toast.makeText(gv.gameContext, "source is not a PC or Creature", Toast.LENGTH_SHORT).show();
            }
            else
            {
                //Toast.makeText(gv.gameContext, "don't recognize source type", Toast.LENGTH_SHORT).show();			
            }
        }
        public void spWeb(object src, object trg, Spell thisSpell)
        {
            //set squares list
            CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);

            //set target list
            CreateAoeTargetsList(src, trg, thisSpell, false);

            //get casting source information
            int classLevel = 0;
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                classLevel = source.classLevel;
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else //creature casting
            {
                Creature source = (Creature)src;
                classLevel = source.cr_level;
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }

            //iterate over targets and do damage
            foreach (object target in AoeTargetsList)
            {
                if (target is Creature)
                {
                    Creature crt = (Creature)target;
                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + crt.getReflex();
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " avoids the web spell" + "</font><BR>");
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.getReflex() + " >= " + DC + "</font><BR>");
                        }
                    }
                    else //failed check
                    {
                        gv.cc.addLogText("<font color='red'>" + crt.cr_name + " is held by a web spell" + "</font><BR>");
                        crt.cr_status = "Held";
                        //hurgh15
                        Effect ef = mod.getEffectByTag("web");
                        ef.statusType = "Held";
                        crt.AddEffectByObject(ef, classLevel);
                    }
                }
                else //target is Player
                {
                    Player pc = (Player)target;
                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + pc.reflex;
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        gv.cc.addLogText("<font color='yellow'>" + pc.name + " avoids the web spell" + "</font><BR>");
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                        }
                    }
                    else //failed check
                    {
                        gv.cc.addLogText("<font color='red'>" + pc.name + " is held by a web spell" + "</font><BR>");
                        pc.charStatus = "Held";
                        Effect ef = mod.getEffectByTag("web");
                        ef.statusType = "Held";
                        pc.AddEffectByObject(ef, classLevel);
                    }
                }
            }
        }
        public void spIceStorm(object src, object trg, Spell thisSpell)
        {
            //set squares list
            CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);

            //set target list
            CreateAoeTargetsList(src, trg, thisSpell, false);

            //get casting source information
            int classLevel = 0;
            string sourceName = "";
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                classLevel = source.classLevel;
                sourceName = source.name;
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else //creature casting
            {
                Creature source = (Creature)src;
                classLevel = source.cr_level;
                sourceName = source.cr_name;
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }

            //iterate over targets and do damage
            foreach (object target in AoeTargetsList)
            {
                if (target is Creature)
                {
                    Creature crt = (Creature)target;
                    float resist = (float)(1f - ((float)crt.damageTypeResistanceValueCold / 100f));
                    float damage = classLevel * RandInt(3);
                    int iceDam = (int)(damage * resist);

                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + crt.getReflex();
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        iceDam = iceDam / 2;
                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " evades most of the Ice Storm spell" + "</font><BR>");
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.getReflex() + " >= " + DC + "</font><BR>");
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " iceDam = " + iceDam + "</font><BR>");
                    }
                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " attacks " + "</font>" + "<font color='silver'>" + crt.cr_name + "</font><BR>");
                    gv.cc.addLogText("<font color='white'>" + "Ice Storm (" + "</font>" + "<font color='lime'>" + iceDam + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    crt.hp -= iceDam;
                    if (crt.hp <= 0)
                    {
                        gv.screenCombat.deathAnimationLocations.Add(new Coordinate(crt.combatLocX, crt.combatLocY));
                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), iceDam + "");
                }
                else //target is Player
                {
                    Player pc = (Player)target;
                    float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalCold / 100f));
                    float damage = classLevel * RandInt(3);
                    int iceDam = (int)(damage * resist);

                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + pc.reflex;
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        if (this.hasTrait(pc, "evasion"))
                        {
                            iceDam = 0;
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades all of the Ice Storm spell" + "</font><BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                            }
                        }
                        else
                        {
                            iceDam = iceDam / 2;
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades most of the Ice Storm spell" + "</font><BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                            }
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " iceDam = " + iceDam + "</font><BR>");
                    }
                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " attacks " + "</font>" + "<font color='silver'>" + pc.name + "</font><BR>");
                    gv.cc.addLogText("<font color='white'>" + "Ice Storm (" + "</font>" + "<font color='lime'>" + iceDam + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    pc.hp -= iceDam;
                    if (pc.hp <= 0)
                    {
                        if (pc.hp <= -20)
                        {
                            gv.screenCombat.deathAnimationLocations.Add(new Coordinate(pc.combatLocX, pc.combatLocY));
                        }
                        gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                        pc.charStatus = "Dead";
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), iceDam + "");                    
                }
            }

            //remove dead creatures            
            /*for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
            {
                if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                {
                    try
                    {
                        //do OnDeath IBScript
                        gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                        mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                        mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                    }
                    catch (Exception ex)
                    {
                        gv.errorLog(ex.ToString());
                    }
                }
            }*/
//            gv.postDelayed("doFloatyText", 100);
        }
        public void spFireball(object src, object trg, Spell thisSpell)
        {
            //set squares list
            CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);

            //set target list
            CreateAoeTargetsList(src, trg, thisSpell, false);

            //get casting source information
            int classLevel = 0;
            string sourceName = "";
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                classLevel = source.classLevel;
                sourceName = source.name;
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                classLevel = source.cr_level;
                sourceName = source.cr_name;
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }

            else if (src is Item) //item was used
            {
                Item source = (Item)src;
                classLevel = source.levelOfItemForCastSpell;
                sourceName = source.name;
            }

            else if (src is Coordinate) //trigger or prop was used  
            {
                classLevel = 1;
                sourceName = "trigger";
            }

            //iterate over targets and do damage
            foreach (object target in AoeTargetsList)
            {
                if (target is Creature)
                {
                    Creature crt = (Creature)target;
                    float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                    float damage = classLevel * RandInt(6);
                    int fireDam = (int)(damage * resist);

                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + crt.getReflex();
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        fireDam = fireDam / 2;
                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " evades most of the Fireball spell" + "</font><BR>");
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.getReflex() + " >= " + DC + "</font><BR>");
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " fireDam = " + fireDam + "</font><BR>");
                    }
                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " attacks " + "</font>" + "<font color='silver'>" + crt.cr_name + "</font><BR>");
                    gv.cc.addLogText("<font color='white'>" + "Fireball (" + "</font>" + "<font color='lime'>" + fireDam + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    crt.hp -= fireDam;
                    if (crt.hp <= 0)
                    {
                        gv.screenCombat.deathAnimationLocations.Add(new Coordinate(crt.combatLocX, crt.combatLocY));
                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), fireDam + "");
                }
                else //target is Player
                {
                    Player pc = (Player)target;
                    float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f));
                    float damage = classLevel * RandInt(6);
                    int fireDam = (int)(damage * resist);

                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + pc.reflex;
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        if (this.hasTrait(pc, "evasion"))
                        {
                            fireDam = 0;
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades all of the Fireball spell" + "</font><BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                            }
                        }
                        else
                        {
                            fireDam = fireDam / 2;
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades most of the Fireball spell" + "</font><BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                            }
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " fireDam = " + fireDam + "</font><BR>");
                    }
                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " attacks " + "</font>" + "<font color='silver'>" + pc.name + "</font><BR>");
                    gv.cc.addLogText("<font color='white'>" + "Fireball (" + "</font>" + "<font color='lime'>" + fireDam + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    pc.hp -= fireDam;
                    if (pc.hp <= 0)
                    {
                        if (pc.hp <= -20)
                        {
                            gv.screenCombat.deathAnimationLocations.Add(new Coordinate(pc.combatLocX, pc.combatLocY));
                        }
                        gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                        pc.charStatus = "Dead";
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), fireDam + "");
                }
            }

            //remove dead creatures            
            /*for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
            {
                if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                {
                    try
                    {
                        //do OnDeath IBScript
                        gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                        mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                        mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                    }
                    catch (Exception ex)
                    {
                        gv.errorLog(ex.ToString());
                    }
                }
            }*/
//            gv.postDelayed("doFloatyText", 100);
        }
        public void spLightning(object src, object trg, Spell thisSpell)
        {
            //set squares list
            CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);

            //set target list
            CreateAoeTargetsList(src, trg, thisSpell, false);

            //get casting source information
            int classLevel = 0;
            string sourceName = "";
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                classLevel = source.classLevel;
                sourceName = source.name;
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else //creature casting
            {
                Creature source = (Creature)src;
                classLevel = source.cr_level;
                sourceName = source.cr_name;
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }

            //iterate over targets and do damage
            foreach (object target in AoeTargetsList)
            {
                if (target is Creature)
                {
                    Creature crt = (Creature)target;
                    float resist = (float)(1f - ((float)crt.damageTypeResistanceValueElectricity / 100f));
                    float damage = classLevel * RandInt(6);
                    int elecDam = (int)(damage * resist);

                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + crt.getReflex();
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        elecDam = elecDam / 2;
                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " evades most of the Lightning spell" + "</font><BR>");
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.getReflex() + " >= " + DC + "</font><BR>");
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " elecDam = " + elecDam + "</font><BR>");
                    }
                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " attacks " + "</font>" + "<font color='silver'>" + crt.cr_name + "</font><BR>");
                    gv.cc.addLogText("<font color='white'>" + "Lightning (" + "</font>" + "<font color='lime'>" + elecDam + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    crt.hp -= elecDam;
                    if (crt.hp <= 0)
                    {
                        gv.screenCombat.deathAnimationLocations.Add(new Coordinate(crt.combatLocX, crt.combatLocY));
                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), elecDam + "");
                }
                else //target is Player
                {
                    Player pc = (Player)target;
                    float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalElectricity / 100f));
                    float damage = classLevel * RandInt(6);
                    int elecDam = (int)(damage * resist);

                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + pc.reflex;
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        if (this.hasTrait(pc, "evasion"))
                        {
                            elecDam = 0;
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades all of the Lightning spell" + "</font><BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                            }
                        }
                        else
                        {
                            elecDam = elecDam / 2;
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades most of the Lightning spell" + "</font><BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                            }
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " elecDam = " + elecDam + "</font><BR>");
                    }
                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " attacks " + "</font>" + "<font color='silver'>" + pc.name + "</font><BR>");
                    gv.cc.addLogText("<font color='white'>" + "Lightning (" + "</font>" + "<font color='lime'>" + elecDam + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    pc.hp -= elecDam;
                    if (pc.hp <= 0)
                    {
                        if (pc.hp <= -20)
                        {
                            gv.screenCombat.deathAnimationLocations.Add(new Coordinate(pc.combatLocX, pc.combatLocY));
                        }
                        gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                        pc.charStatus = "Dead";
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), elecDam + "");
                }
            }

            //remove dead creatures            
            /*for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
            {
                if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                {
                    try
                    {
                        //do OnDeath IBScript
                        gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                        mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                        mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                    }
                    catch (Exception ex)
                    {
                        gv.errorLog(ex.ToString());
                    }
                }
            }*/
//            gv.postDelayed("doFloatyText", 100);
        }

        //SPELLS CLERIC
        public void spHeal(object src, object trg, int healAmount)
        {
            //clear squares list
            gv.sf.AoeSquaresList.Clear();

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Player target = (Player)trg;

                if (target.hp <= -20)
                {
                    //MessageBox("Can't heal a dead character!");
                    gv.cc.addLogText("<font color='red'>" + "Can't heal a dead character!" + "</font><BR>");
                }
                else
                {
                    target.hp += healAmount;
                    if (target.hp > target.hpMax)
                    {
                        target.hp = target.hpMax;
                    }
                    if (target.hp > 0)
                    {
                        target.charStatus = "Alive";
                    }
                    //MessageBox(pc.name + " gains " + healAmount + " HPs");
                    gv.cc.addLogText("<font color='lime'>" + target.name + " gains " + healAmount + " HPs" + "</font><BR>");
                }
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            if (src is Item) //player casting
            {
                Player target = (Player)trg;

                if (target.hp <= -20)
                {
                    //MessageBox("Can't heal a dead character!");
                    gv.cc.addLogText("<font color='red'>" + "Can't heal a dead character!" + "</font><BR>");
                }
                else
                {
                    target.hp += healAmount;
                    if (target.hp > target.hpMax)
                    {
                        target.hp = target.hpMax;
                    }
                    if (target.hp > 0)
                    {
                        target.charStatus = "Alive";
                    }
                    //MessageBox(pc.name + " gains " + healAmount + " HPs");
                    MessageBoxHtml(target.name + " gains " + healAmount + " HPs, now has " + target.hp + "/" + target.hpMax + "HPs");
                    gv.cc.addLogText("<font color='lime'>" + target.name + " gains " + healAmount + " HPs" + "</font><BR>");
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Creature target = (Creature)trg;

                target.hp += healAmount;
                if (target.hp > target.hpMax)
                {
                    target.hp = target.hpMax;
                }
                gv.cc.addLogText("<font color='lime'>" + target.cr_name + " gains " + healAmount + " HPs" + "</font><BR>");
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Coordinate)
            {
                //Toast.makeText(gv.gameContext, "target is not a PC or Creature", Toast.LENGTH_SHORT).show();
            }
            else
            {
                //Toast.makeText(gv.gameContext, "don't recognize target type", Toast.LENGTH_SHORT).show();			
            }
        }
        public void spMassHeal(object src, object trg, int healAmount)
        {
            //clear squares list
            gv.sf.AoeSquaresList.Clear();

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                //Player target = (Player)trg;

                foreach (Player pc in mod.playerList)
                {
                    if (pc.hp <= -20)
                    {
                        gv.cc.addLogText("<font color='red'>" + "Can't heal a dead character!" + "</font><BR>");
                    }
                    else
                    {
                        pc.hp += healAmount;
                        if (pc.hp > pc.hpMax)
                        {
                            pc.hp = pc.hpMax;
                        }
                        if (pc.hp > 0)
                        {
                            pc.charStatus = "Alive";
                        }
                        //MessageBox(pc.name + " gains " + healAmount + " HPs");
                        gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + healAmount + " HPs" + "</font><BR>");
                    }
                }
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                //Creature target = (Creature)trg;

                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    crt.hp += healAmount;
                    if (crt.hp > crt.hpMax)
                    {
                        crt.hp = crt.hpMax;
                    }
                    gv.cc.addLogText("<font color='lime'>" + crt.cr_name + " gains " + healAmount + " HPs" + "</font><BR>");
                }
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Coordinate)
            {
                //Toast.makeText(gv.gameContext, "target is not a PC or Creature", Toast.LENGTH_SHORT).show();
            }
            else
            {
                //Toast.makeText(gv.gameContext, "don't recognize target type", Toast.LENGTH_SHORT).show();			
            }
        }
        public void spBless(object src, object trg)
        {
            //clear squares list
            gv.sf.AoeSquaresList.Clear();

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                //Player target = (Player)trg;

                foreach (Player pc in mod.playerList)
                {
                    int numberOfRounds = (source.classLevel * 5); //5 rounds per level
                    Effect ef = mod.getEffectByTag("bless").DeepCopy();
                    ef.durationInUnits = numberOfRounds * gv.mod.TimePerRound;
                    gv.cc.addLogText("<font color='lime'>" + "Bless is applied on " + pc.name
                            + " for " + numberOfRounds + " round(s)" + "</font>" +
                            "<BR>");
                    pc.AddEffectByObject(ef, source.classLevel);
                }
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                //Creature target = (Creature)trg;

                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    int numberOfRounds = (source.cr_level * 5); //5 rounds per level
                    Effect ef = mod.getEffectByTag("bless").DeepCopy();
                    ef.durationInUnits = numberOfRounds * gv.mod.TimePerRound;
                    gv.cc.addLogText("<font color='lime'>" + "Bless is applied on " + crt.cr_name
                            + " for " + numberOfRounds + " round(s)" + "</font>" +
                            "<BR>");
                    crt.AddEffectByObject(ef, source.cr_level);
                }
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Coordinate)
            {
                //Toast.makeText(gv.gameContext, "source is not a PC or Creature", Toast.LENGTH_SHORT).show();
            }
            else
            {
                //Toast.makeText(gv.gameContext, "don't recognize source type", Toast.LENGTH_SHORT).show();			
            }
        }
        public void spMagicStone(object src, object trg)
        {
            //clear squares list
            gv.sf.AoeSquaresList.Clear();

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Creature target = (Creature)trg;

                int damageTotal = 0;
                int numberOfBolts = ((source.classLevel - 1) / 2) + 1; //1 stone for every 2 levels after level 1
                if (numberOfBolts > 3) { numberOfBolts = 3; } //can not have more than 3 stones
                for (int i = 0; i < numberOfBolts; i++)
                {
                    float resist = (float)(1f - ((float)target.damageTypeResistanceValueMagic / 100f));
                    float damage = 1 * RandInt(3) + 1;
                    int stoneDam = (int)(damage * resist);
                    //int damage = 1 * RandInt(3) + 1;                
                    target.hp = target.hp - stoneDam;
                    damageTotal += stoneDam;
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                    + " stoneDam = " + stoneDam + "</font>" +
                                    "<BR>");
                    }

                    gv.cc.addLogText("<font color='aqua'>" + source.name + "</font>" +
                            "<font color='white'>" + " attacks " + "</font>" +
                            "<font color='silver'>" + target.cr_name + "</font>" +
                            "<BR>");
                    gv.cc.addLogText("<font color='white'>" + "Magic Stone (" + "</font>");
                    gv.cc.addLogText("<font color='lime'>" + stoneDam + "</font>" +
                            "<font color='white'>" + " damage)" + "</font>" +
                            "<BR>");
                    if (target.hp <= 0)
                    {
                        gv.screenCombat.deathAnimationLocations.Add(new Coordinate(target.combatLocX, target.combatLocY));
                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + target.cr_name + "</font><BR>");
                        /*try
                        {
                            for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
                            {
                                if (mod.currentEncounter.encounterCreatureList[x] == target)
                                {
                                    try
                                    {
                                        //do OnDeath LOGIC TREE
                                        //REMOVEgv.cc.doLogicTreeBasedOnTag(target.onDeathLogicTree, target.onDeathParms);
                                        //do OnDeath IBScript
                                        gv.cc.doIBScriptBasedOnFilename(target.onDeathIBScript, target.onDeathIBScriptParms);
                                        mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                                        mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                                    }
                                    catch (Exception ex)
                                    {
                                        gv.errorLog(ex.ToString());
                                    }
                                }
                            }
                            mod.currentEncounter.encounterCreatureList.Remove(target);
                        }
                        catch (Exception ex)
                        {
                            gv.errorLog(ex.ToString());
                        }*/
                    }
                }
                //Do floaty text damage
                //gv.screenCombat.floatyTextOn = true;
                gv.cc.addFloatyText(new Coordinate(target.combatLocX, target.combatLocY), damageTotal + "");
                //                gv.postDelayed("doFloatyText", 100);
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Player target = (Player)trg;

                int damageTotal = 0;
                int numberOfBolts = ((source.cr_level - 1) / 2) + 1; //1 bolt for every 2 levels after level 1
                if (numberOfBolts > 3) { numberOfBolts = 3; } //can not have more than 5 bolts
                for (int i = 0; i < numberOfBolts; i++)
                {
                    float resist = (float)(1f - ((float)target.damageTypeResistanceTotalMagic / 100f));
                    float damage = 1 * RandInt(3) + 1;
                    int stoneDam = (int)(damage * resist);
                    //int damage = 1 * RandInt(4) + 1;
                    damageTotal += stoneDam;
                    target.hp = target.hp - stoneDam;
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                    + " stoneDam = " + stoneDam + "</font>" +
                                    "<BR>");
                    }
                    gv.cc.addLogText("<font color='silver'>" + source.cr_name + "</font>" +
                            "<font color='white'>" + " attacks " + "</font>" +
                            "<font color='aqua'>" + target.name + "</font>" +
                            "<BR>");
                    gv.cc.addLogText("<font color='white'>" + "Magic Stone (" + "</font>");
                    gv.cc.addLogText("<font color='lime'>" + stoneDam + "</font>" +
                            "<font color='white'>" + " damage)" + "</font>" +
                            "<BR>");
                    if (target.hp <= 0)
                    {
                        if (target.hp <= -20)
                        {
                            gv.screenCombat.deathAnimationLocations.Add(new Coordinate(target.combatLocX, target.combatLocY));
                        }
                        gv.cc.addLogText("<font color='red'>" + target.name + " drops unconcious!" + "</font><BR>");
                        target.charStatus = "Dead";
                    }
                }
                //Do floaty text damage
                //gv.screenCombat.floatyTextOn = true;
                gv.cc.addFloatyText(new Coordinate(target.combatLocX, target.combatLocY), damageTotal + "");
//                gv.postDelayed("doFloatyText", 100);

                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Coordinate)
            {
                //Toast.makeText(gv.gameContext, "target is not a PC or Creature", Toast.LENGTH_SHORT).show();
            }
            else
            {
                //Toast.makeText(gv.gameContext, "don't recognize target type", Toast.LENGTH_SHORT).show();			
            }
        }
        public void spBlastOfLight(object src, object trg, Spell thisSpell)
        {
            //set squares list
            CreateAoeSquaresList(src, trg, thisSpell.aoeShape, thisSpell.aoeRadius);

            //set target list
            CreateAoeTargetsList(src, trg, thisSpell, false);

            //get casting source information
            int classLevel = 0;
            string sourceName = "";
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                classLevel = source.classLevel;
                sourceName = source.name;
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else //creature casting
            {
                Creature source = (Creature)src;
                classLevel = source.cr_level;
                sourceName = source.cr_name;
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }

            //iterate over targets and do damage
            foreach (object target in AoeTargetsList)
            {
                if (target is Creature)
                {
                    Creature crt = (Creature)target;
                    float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                    float damage = 2 * RandInt(6);
                    int fireDam = (int)(damage * resist);

                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + crt.getReflex();
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        fireDam = fireDam / 2;
                        gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " evades most of the Blast of Light spell" + "</font><BR>");
                        if (mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.getReflex() + " >= " + DC + "</font><BR>");
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " fireDam = " + fireDam + "</font><BR>");
                    }
                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " attacks " + "</font>" + "<font color='silver'>" + crt.cr_name + "</font><BR>");
                    gv.cc.addLogText("<font color='white'>" + "Blast of Light (" + "</font>" + "<font color='lime'>" + fireDam + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    crt.hp -= fireDam;
                    if (crt.hp <= 0)
                    {
                        gv.screenCombat.deathAnimationLocations.Add(new Coordinate(crt.combatLocX, crt.combatLocY));
                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), fireDam + "");
                }
                else //target is Player
                {
                    Player pc = (Player)target;
                    float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f));
                    float damage = 2 * RandInt(6);
                    int fireDam = (int)(damage * resist);

                    int saveChkRoll = RandInt(20);
                    int saveChk = saveChkRoll + pc.reflex;
                    int DC = 13;
                    if (saveChk >= DC) //passed save check
                    {
                        if (this.hasTrait(pc, "evasion"))
                        {
                            fireDam = 0;
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades all of the Blast of Light spell" + "</font><BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                            }
                        }
                        else
                        {
                            fireDam = fireDam / 2;
                            gv.cc.addLogText("<font color='yellow'>" + pc.name + " evades most of the Blast of Light spell" + "</font><BR>");
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + pc.reflex + " >= " + DC + "</font><BR>");
                            }
                        }
                    }
                    if (mod.debugMode)
                    {
                        gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage + " fireDam = " + fireDam + "</font><BR>");
                    }
                    gv.cc.addLogText("<font color='aqua'>" + sourceName + "</font>" + "<font color='white'>" + " attacks " + "</font>" + "<font color='silver'>" + pc.name + "</font><BR>");
                    gv.cc.addLogText("<font color='white'>" + "Blast of Light (" + "</font>" + "<font color='lime'>" + fireDam + "</font>" + "<font color='white'>" + " damage)" + "</font><BR>");
                    pc.hp -= fireDam;
                    if (pc.hp <= 0)
                    {
                        if (pc.hp <= -20)
                        {
                            gv.screenCombat.deathAnimationLocations.Add(new Coordinate(pc.combatLocX, pc.combatLocY));
                        }
                        gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                        pc.charStatus = "Dead";
                    }
                    //Do floaty text damage
                    //gv.screenCombat.floatyTextOn = true;
                    gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), fireDam + "");                    
                }
            }

            //remove dead creatures            
            /*for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
            {
                if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                {
                    try
                    {
                        //do OnDeath IBScript
                        gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                        mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                        mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                    }
                    catch (Exception ex)
                    {
                        gv.errorLog(ex.ToString());
                    }
                }
            }*/
//            gv.postDelayed("doFloatyText", 100);
        }
        public void spHold(object src, object trg)
        {
            //clear squares list
            gv.sf.AoeSquaresList.Clear();

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Creature target = (Creature)trg;

                int saveChkRoll = RandInt(20);
                //int saveChk = saveChkRoll + target.Will;
                int saveChk = saveChkRoll;
                int DC = 16;
                if (saveChk >= DC) //passed save check
                {
                    gv.cc.addLogText("<font color='yellow'>" + target.cr_name + " avoids the hold spell" + "</font><BR>");
                }
                else
                {
                    gv.cc.addLogText("<font color='red'>" + target.cr_name + " is held by a hold spell" + "</font><BR>");
                    target.cr_status = "Held";
                    Effect ef = mod.getEffectByTag("hold");
                    ef.statusType = "Held";
                    target.AddEffectByObject(ef, source.classLevel);
                }
                if (!source.thisCastIsFreeOfCost)
                {
                    source.sp -= gv.cc.currentSelectedSpell.costSP;
                    if (source.sp < 0) { source.sp = 0; }
                    if (source.hp > gv.cc.currentSelectedSpell.costHP)
                    {
                        source.hp -= gv.cc.currentSelectedSpell.costHP;
                    }
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Player target = (Player)trg;

                int saveChkRoll = RandInt(20);
                //int saveChk = saveChkRoll + target.Will;
                int saveChk = saveChkRoll;
                int DC = 16;
                if (saveChk >= DC) //passed save check
                {
                    gv.cc.addLogText("<font color='yellow'>" + target.name + " avoids the hold spell" + "</font><BR>");
                }
                else
                {
                    gv.cc.addLogText("<font color='red'>" + target.name + " is held by a hold spell" + "</font><BR>");
                    target.charStatus = "Held";
                    Effect ef = mod.getEffectByTag("hold");
                    ef.statusType = "Held";
                    target.AddEffectByObject(ef, source.cr_level);
                }
                source.sp -= SpellToCast.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Coordinate)
            {
                //Toast.makeText(gv.gameContext, "target is not a PC or Creature", Toast.LENGTH_SHORT).show();
            }
            else
            {
                //Toast.makeText(gv.gameContext, "don't recognize target type", Toast.LENGTH_SHORT).show();			
            }
        }
    }
}
