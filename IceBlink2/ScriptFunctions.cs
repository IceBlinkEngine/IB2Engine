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
        public int spCnt = 0;
        Random rand;

        public ScriptFunctions(Module m, GameView g)
        {
            mod = m;
            gv = g;
            rand = new Random();
        }

        public void MessageBox(string message)
        {
            /*TODO
		    AlertDialog.Builder dlgAlert  = new AlertDialog.Builder(gv.gameContext);
		    dlgAlert.setMessage(message);
		    //dlgAlert.setTitle("App Title");
		    dlgAlert.setPositiveButton("Ok",
			        new DialogInterface.OnClickListener() {
			            public void onClick(DialogInterface dialog, int which) {
			              //dismiss the dialog  
			            }
			        });
		    dlgAlert.setCancelable(true);
		    dlgAlert.create().show();
            */
        }

        public void ShowFullDescription(Item it)
        {
            string textToSpan = "<u>Description</u>" + "<BR>";
            textToSpan += "<b><i><big>" + it.name + "</big></i></b><BR>";
            if ((it.category.Equals("Melee")) || (it.category.Equals("Ranged")))
            {
                textToSpan += "Damage: " + it.damageNumDice + "d" + it.damageDie + "+" + it.damageAdder + "<BR>";
                textToSpan += "Attack Bonus: " + it.attackBonus + "<BR>";
                textToSpan += "Attack Range: " + it.attackRange + "<BR>";
                textToSpan += "Useable By: " + isUseableBy(it) + "<BR>";
                textToSpan += "Two-Handed Weapon: ";
                if (it.twoHanded) { textToSpan += "Yes<BR>"; }
                else { textToSpan += "No<BR>"; }
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
            MessageBoxHtml(textToSpan);
        }
        public string isUseableBy(Item it)
        {
            string strg = "";
            foreach (PlayerClass cls in mod.modulePlayerClassList)
            {
                string firstLetter = cls.name.Substring(0, 1);
                foreach (ItemRefs ia in cls.itemsAllowed)
                {
                    string stg = ia.resref;
                    if (stg.Equals(it.resref))
                    {
                        strg += firstLetter + ", ";
                    }
                }
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
                int x = 0;
            }
            /*
            AlertDialog.Builder dlgAlert  = new AlertDialog.Builder(gv.gameContext);
		    dlgAlert.setMessage(Html.fromHtml(message));
		    //dlgAlert.setTitle("App Title");
		    dlgAlert.setPositiveButton("Ok",
			        new DialogInterface.OnClickListener() 
				    {
			            public void onClick(DialogInterface dialog, int which) 
			            {
			              //dismiss the dialog  
			            }
			        });
		    dlgAlert.setCancelable(true);
		    dlgAlert.create().show();
            */
        }

        /// <summary>
        /// Used for generating a random number between some number and some other number (max must be >= min).
        /// </summary>
        /// <param name="min"> The minimum value that can be found (ex. Random(5, 9); will return a number between 5-9 so 5, 6, 7, 8 or 9 are possible results).</param>
        /// <param name="max"> The maximum value that can be found (ex. Random(5, 9); will return a number between 5-9 so 5, 6, 7, 8 or 9 are possible results).</param>
        public int RandInt(int min, int max)
        {
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
                        GiveXP(parm1);
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
                    else if (filename.Equals("gaForceRestAndRaiseDead.cs"))
                    {
                        itForceRestAndRaiseDead();
                    }
                    else if (filename.Equals("gaMovePartyToLastLocation.cs"))
                    {
                        gv.mod.PlayerLocationX = gv.mod.PlayerLastLocationX;
                        gv.mod.PlayerLocationY = gv.mod.PlayerLastLocationY;
                    }
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
                    else if (filename.Equals("gaRiddle.cs"))
                    {
                        riddle();
                    }
                    else if (filename.Equals("gaDamageWithoutItem.cs"))
                    {
                        int parm1 = Convert.ToInt32(p1);
                        DamageWithoutItem(parm1, p2);
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
                        int parm2 = Convert.ToInt32(p2);
                        int parm3 = Convert.ToInt32(p3);
                        if (gv.mod.currentArea.Filename.Equals(p1))
                        {
                            gv.mod.PlayerLocationX = parm2;
                            gv.mod.PlayerLocationY = parm3;
                        }
                        else
                        {
                            gv.cc.doTransitionBasedOnAreaLocation(p1, parm2, parm3);
                        }
                    }
                    else if (filename.Equals("gaAddPartyMember.cs"))
                    {
                        AddCharacterToParty(p1);
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
                    else if (filename.Equals("gaPlaySound.cs"))
                    {
                        gv.PlaySound(p1);
                    }
                    else if (filename.Equals("gaKillAllCreatures.cs"))
                    {
                        gv.mod.currentEncounter.encounterCreatureList.Clear();
                        gv.mod.currentEncounter.encounterCreatureRefsList.Clear();
                        gv.screenCombat.checkEndEncounter();
                    }
                    else if (filename.Equals("gaOpenShopByTag.cs"))
                    {
                        gv.screenShop.currentShopTag = p1;
                        gv.screenShop.currentShop = gv.mod.getShopByTag(p1);
                        gv.screenType = "shop";
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
                }
                catch (Exception ex)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        this.MessageBoxHtml("Failed to run script (" + filename + "): " + ex.ToString());
                    }
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
                        if ((p1.Equals("")) || (p1.Equals("-1")))
                        {
                            parm1 = gv.mod.selectedPartyLeader;
                        }
                        else
                        {
                            parm1 = Convert.ToInt32(p1);
                        }
                        gv.mod.returnCheck = CheckHasTrait(parm1, p2);
                    }
                    else if (filename.Equals("gcPassSkillCheck.cs"))
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
                        gv.mod.returnCheck = CheckPassSkill(parm1, p2, parm3);
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
                }
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
                    else if (filename.Equals("ogGetPartyRosterSize.cs"))
                    {
                        String val = gv.mod.partyRosterList.Count + "";
                        SetGlobalInt(prm1, val);
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
                        String val = mod.WorldTime + "";
                        SetGlobalInt(prm2, val);
                    }
                }
                catch (Exception ex)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        this.MessageBoxHtml("Failed to run script (" + filename + "): " + ex.ToString());
                    }
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
                            prp2.LocationX = Convert.ToInt32(p3);
                            prp2.LocationY = Convert.ToInt32(p4);
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

                    else if (filename.Equals("osAddCreatureToCurrentEncounter.cs"))
                    {
                        //p1 is the resref of the summoned cretaure (use one from a blueprint in the toolset's creature blueprints section)
                        //p2 will be teh su mmon duartion (NOT IMPLEMENTED YET)
                        //p3 x location of the summon in current encounter (will be automatically adjusted to nearest location if the spot is already occupied or non-walkable)
                        //p4 y location of the summon in current encounter (will be automatically adjusted to nearest location if the spot is already occupied or non-walkable)

                        //this.MessageBoxHtml("Add creature script ran");
                        foreach (Creature c in gv.mod.moduleCreaturesList)
                        {
                            if (c.cr_resref == p1)
                            {
                                //fetch the data for our creature by making a blueprint(object) copy
                                Creature copy = c.DeepCopy();
                                //crucial for loading the creature token
                                copy.token = gv.cc.LoadBitmap(copy.cr_tokenFilename);
                                
                                //Automaically create a unique tag
                                gv.sf.SetGlobalInt("summonCounter", "++");
                                string tagCounter = gv.sf.GetGlobalInt("summonCounter").ToString();
                                copy.cr_tag = "SummonTag" + tagCounter;
                                this.MessageBoxHtml("Summon tag is: " + copy.cr_tag);

                                //find correct summon spot, replace with nearest location if neccessary
                                copy.combatLocX = Convert.ToInt32(p3);// x destination intended for the summon
                                copy.combatLocY = Convert.ToInt32(p4);// y destination intended for the summon
                                bool changeSummonLocation = false;// used as switch for cycling through all tiles in case the originally intended spot was occupied/not-walkable
                                int targetTile = copy.combatLocY * gv.mod.currentEncounter.MapSizeX + copy.combatLocX;//the index of the original target spot in the encounter's tiles list
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
                                
                                //first check: check walkable
                                if (gv.mod.currentEncounter.encounterTiles[targetTile].Walkable == false)
                                {
                                    changeSummonLocation = true;
                                    //this.MessageBoxHtml("Target square not walkable");
                                }

                                //second check: check ossupied by creature (only necceessary if walkable)
                                if (changeSummonLocation == false)
                                {
                                    foreach (Creature cr in gv.mod.currentEncounter.encounterCreatureList)
                                    {
                                        if ((cr.combatLocX == copy.combatLocX) && (cr.combatLocY == copy.combatLocY))
                                        {
                                            changeSummonLocation = true;
                                            //this.MessageBoxHtml("Target square occupied by other cretaure");
                                            break;
                                        }
                                    }
                                }

                                //third check: check occupied by pc (only necceessary if walkable and not occupied by creature)
                                if (changeSummonLocation == false)
                                {
                                    foreach (Player pc in gv.mod.playerList)
                                    {
                                        if ((pc.combatLocX == copy.combatLocX) && (pc.combatLocY == copy.combatLocY))
                                        {
                                            changeSummonLocation = true;
                                            //this.MessageBoxHtml("Target square occupied by pc");
                                            break;
                                        }
                                    }
                                }

                                //target square was already occupied/non-walkable, so all other tiles are searched for the NEAREST FREE tile to switch the summon location to
                                if (changeSummonLocation == true)
                                {
                                    //this.MessageBoxHtml("Changed the summon location because target spot was occupied");
                                    //FIRST PART: get all FREE tiles in the current encounter
                                    for (int i = 0; i < gv.mod.currentEncounter.encounterTiles.Count; i++)
                                    {
                                        //get the x and y location of current tile by calculation derived from index number, assuming that counting starts at top left corner of a map (0x, 0y)
                                        //and that each horizintal x-line is counted first, then counting next horizonal x-line starting from the left again
                                        tileIsFree = true;
                                        //Note: When e.g. MapsizeY is 7, the y values range from 0 to 6
                                        tileLocX = i % gv.mod.currentEncounter.MapSizeY;
                                        //Note: ensure rounding down here 
                                        floatTileLocY = i / gv.mod.currentEncounter.MapSizeX;
                                        tileLocY = (int)Math.Floor(floatTileLocY);

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
                                        tileLocX = freeTilesByIndex[i] % gv.mod.currentEncounter.MapSizeY;
                                        floatTileLocY = freeTilesByIndex[i] / gv.mod.currentEncounter.MapSizeX;
                                        tileLocY = (int)Math.Floor(floatTileLocY);

                                        //get distance between the current free tile and the originally intended summon location
                                        deltaX = (int)Math.Abs((tileLocX - copy.combatLocX));
                                        deltaY = (int)Math.Abs((tileLocY - copy.combatLocY));
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
                                        tileLocX = nearestTileByIndex % gv.mod.currentEncounter.MapSizeY;
                                        floatTileLocY = nearestTileByIndex / gv.mod.currentEncounter.MapSizeX;
                                        tileLocY = (int)Math.Floor(floatTileLocY);

                                        copy.combatLocX = tileLocX;
                                        copy.combatLocY = tileLocY;
                                    }

                                }

                                //just check whether a free squre does exist at all; if not, do not complete the summon
                                if ((nearestTileByIndex != -1) || (changeSummonLocation == false))
                                {
                                    //finally add creature
                                    mod.currentEncounter.encounterCreatureList.Add(copy);
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
            mod.partyGold -= amount;
            if (mod.partyGold < 0)
            {
                mod.partyGold = 0;
            }
            gv.cc.addLogText("<font color='yellow'>" + "The party loses " + amount + " Gold" + "</font>" + "<BR>");
        }
        public void GiveItem(string resref, int quantity)
        {
            Item newItem = mod.getItemByResRef(resref);
            for (int i = 0; i < quantity; i++)
            {
                ItemRefs ir = mod.createItemRefsFromItem(newItem);
                mod.partyInventoryRefsList.Add(ir);
            }
            gv.cc.addLogText("<font color='yellow'>" + "The party gains " + quantity + " " + newItem.name + "</font><BR>");
        }
        public void RemoveItemFromInventory(ItemRefs itRef, int quantity)
        {
            //decrement item quantity
            itRef.quantity -= quantity;
            //if item quantity <= 0, remove item from inventory
            if (itRef.quantity < 1)
            {
                gv.mod.partyInventoryRefsList.Remove(itRef);
            }
        }
        public void GiveXP(int amount)
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
                Player newPc = gv.cc.LoadPlayer(filename); //ex: filename = "ezzbel.json"
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
                mod.playerList.Remove(pc);
                mod.selectedPartyLeader = 0;
                gv.cc.partyScreenPcIndex = 0;
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to remove character from party" + "</font><BR>");
                }
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
                gv.cc.partyScreenPcIndex = 0;
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to remove character from party" + "</font><BR>");
                }
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
                gv.cc.partyScreenPcIndex = 0;
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to remove character from party" + "</font><BR>");
                }
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
                gv.mod.partyTokenBitmap = gv.cc.LoadBitmap(gv.mod.partyTokenFilename);
                if (!mod.playerList[0].combatFacingLeft)
                {
                    mod.partyTokenBitmap = gv.cc.flip(mod.partyTokenBitmap);
                }
                gv.mod.showPartyToken = enable;
            }
            catch (Exception ex)
            {
                if (mod.debugMode) //SD_20131102
                {
                    gv.cc.addLogText("<font color='yellow'>" + "failed to switch party token" + "</font><BR>");
                }
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
        public bool CheckPassSkill(int PCIndex, string tag, int dc)
        {
            string foundLargest = "none";
            int largest = 0;
            foreach (string s in mod.playerList[PCIndex].knownTraitsTags)
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
                        int i = Convert.ToInt32(c);
                        if (i > largest)
                        {
                            largest = i;
                            foundLargest = s;
                        }
                    }
                }
            }
            if (!foundLargest.Equals("none"))
            {
                //PC has trait skill so do calculation check
                Trait tr = mod.getTraitByTag(foundLargest);
                int skillMod = tr.skillModifier;
                int attMod = 0;
                if (tr.skillModifierAttribute.Equals("str"))
                {
                    attMod = (mod.playerList[PCIndex].strength - 10) / 2;
                }
                else if (tr.skillModifierAttribute.Equals("dex"))
                {
                    attMod = (mod.playerList[PCIndex].dexterity - 10) / 2;
                }
                else if (tr.skillModifierAttribute.Equals("int"))
                {
                    attMod = (mod.playerList[PCIndex].intelligence - 10) / 2;
                }
                else if (tr.skillModifierAttribute.Equals("cha"))
                {
                    attMod = (mod.playerList[PCIndex].charisma - 10) / 2;
                }
                else if (tr.skillModifierAttribute.Equals("con"))
                {
                    attMod = (mod.playerList[PCIndex].constitution - 10) / 2;
                }
                else if (tr.skillModifierAttribute.Equals("wis"))
                {
                    attMod = (mod.playerList[PCIndex].wisdom - 10) / 2;
                }
                int roll = gv.sf.RandInt(20);
                if (roll + attMod + skillMod >= dc)
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'> skill check(" + tag + "): " + roll + "+" + attMod + "+" + skillMod + ">=" + dc + "</font><BR>");
                    }
                    return true;
                }
                else
                {
                    if (mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<font color='yellow'> skill check: " + roll + "+" + attMod + "+" + skillMod + "<" + dc + "</font><BR>");
                    }
                    return false;
                }
            }
            return false;
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
                            if (jem.EndPoint)
                            {
                                mod.partyJournalCompleted.Add(jcp);
                                mod.partyJournalQuests.Remove(jcp);
                            }
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
                            if (jem.EndPoint)
                            {
                                mod.partyJournalCompleted.Add(jcp);
                                mod.partyJournalQuests.Remove(jcp);
                            }
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
                        if (jem.EndPoint)
                        {
                            mod.partyJournalCompleted.Add(jcp2);
                            mod.partyJournalQuests.Remove(jcp2);
                        }
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
            pc.fortitude = pc.baseFortitude + CalcSavingThrowModifiersFortitude(pc) + (pc.constitution - 10) / 2; //SD_20131127
            pc.will = pc.baseWill + CalcSavingThrowModifiersWill(pc) + (pc.intelligence - 10) / 2; //SD_20131127
            pc.reflex = pc.baseReflex + CalcSavingThrowModifiersReflex(pc) + (pc.dexterity - 10) / 2; //SD_20131127
            pc.strength = pc.baseStr + pc.race.strMod + CalcAttributeModifierStr(pc); //SD_20131127
            pc.dexterity = pc.baseDex + pc.race.dexMod + CalcAttributeModifierDex(pc); //SD_20131127
            pc.intelligence = pc.baseInt + pc.race.intMod + CalcAttributeModifierInt(pc); //SD_20131127
            pc.charisma = pc.baseCha + pc.race.chaMod + CalcAttributeModifierCha(pc); //SD_20131127
            pc.wisdom = pc.baseWis + pc.race.wisMod + CalcAttributeModifierWis(pc); //SD_20131127
            pc.constitution = pc.baseCon + pc.race.conMod + CalcAttributeModifierCon(pc); //SD_20131127
            //SD_20131124 Start
            pc.damageTypeResistanceTotalAcid = pc.race.damageTypeResistanceValueAcid + CalcAcidModifiers(pc);
            if (pc.damageTypeResistanceTotalAcid > 100) { pc.damageTypeResistanceTotalAcid = 100; }
            pc.damageTypeResistanceTotalNormal = pc.race.damageTypeResistanceValueNormal + CalcNormalModifiers(pc);
            if (pc.damageTypeResistanceTotalNormal > 100) { pc.damageTypeResistanceTotalNormal = 100; }
            pc.damageTypeResistanceTotalCold = pc.race.damageTypeResistanceValueCold + CalcAcidModifiers(pc);
            if (pc.damageTypeResistanceTotalCold > 100) { pc.damageTypeResistanceTotalCold = 100; }
            pc.damageTypeResistanceTotalElectricity = pc.race.damageTypeResistanceValueElectricity + CalcElectricityModifiers(pc);
            if (pc.damageTypeResistanceTotalElectricity > 100) { pc.damageTypeResistanceTotalElectricity = 100; }
            pc.damageTypeResistanceTotalFire = pc.race.damageTypeResistanceValueFire + CalcFireModifiers(pc);
            if (pc.damageTypeResistanceTotalFire > 100) { pc.damageTypeResistanceTotalFire = 100; }
            pc.damageTypeResistanceTotalMagic = pc.race.damageTypeResistanceValueMagic + CalcMagicModifiers(pc);
            if (pc.damageTypeResistanceTotalMagic > 100) { pc.damageTypeResistanceTotalMagic = 100; }
            pc.damageTypeResistanceTotalPoison = pc.race.damageTypeResistanceValuePoison + CalcPoisonModifiers(pc);
            if (pc.damageTypeResistanceTotalPoison > 100) { pc.damageTypeResistanceTotalPoison = 100; }
            //SD_20131124 End
            //pc.BaseAttBonus = (int)((double)pc.ClassLevel * pc.Class.BabMultiplier) + CalcBABAdders(pc);
            if (pc.playerClass.babTable.Length > 0)//SD_20131115
            {
                pc.baseAttBonus = pc.playerClass.babTable[pc.classLevel] + CalcBABAdders(pc); //SD_20131115
            }

            int cMod = (pc.constitution - 10) / 2;
            int iMod = (pc.intelligence - 10) / 2;
            pc.spMax = pc.playerClass.startingSP + iMod + ((pc.classLevel - 1) * (pc.playerClass.spPerLevelUp + iMod));
            pc.hpMax = pc.playerClass.startingHP + cMod + ((pc.classLevel - 1) * (pc.playerClass.hpPerLevelUp + cMod));

            pc.XPNeeded = pc.playerClass.xpTable[pc.classLevel];

            int dMod = (pc.dexterity - 10) / 2;
            int maxDex = CalcMaxDexBonus(pc);
            if (dMod > maxDex) { dMod = maxDex; }
            int armBonus = 0;
            int acMods = 0;
            armBonus = CalcArmorBonuses(pc);
            acMods = CalcACModifiers(pc);
            pc.AC = pc.ACBase + dMod + armBonus + acMods;
            if (mod.getItemByResRefForInfo(pc.BodyRefs.resref).ArmorWeightType.Equals("Light"))
            {
                pc.moveDistance = pc.race.MoveDistanceLightArmor + CalcMovementBonuses(pc);
            }
            else //medium or heavy SD_20131116
            {
                pc.moveDistance = pc.race.MoveDistanceMediumHeavyArmor + CalcMovementBonuses(pc);
            }
            foreach (Effect ef in pc.effectsList)
            {
                if (ef.usedForUpdateStats)
                {
                    doUpdateStatsEffectScript(pc, ef.effectScript);
                }
            }
            RunAllItemWhileEquippedScripts(pc);
            if (pc.hp > pc.hpMax) { pc.hp = pc.hpMax; } //SD_20131201
            if (pc.sp > pc.spMax) { pc.sp = pc.spMax; } //SD_20131201
            if ((pc.hp > 0) && (pc.charStatus.Equals("Dead")))
            {
                pc.charStatus = "Alive";
            }
        }
        public void doUpdateStatsEffectScript(Player pc, string scriptName)
        {
            if (scriptName.Equals("efHeld"))
            {
                //efHeld(src, currentDurationInUnits, durationInUnits);
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
        public int CalcSavingThrowModifiersReflex(Player pc)
        {
            int savBonuses = 0;
            savBonuses += mod.getItemByResRefForInfo(pc.BodyRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.MainHandRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.OffHandRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.RingRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.HeadRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).savingThrowModifierReflex;
            savBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).savingThrowModifierReflex;
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
            savBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).savingThrowModifierFortitude;
            savBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).savingThrowModifierFortitude;
            savBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).savingThrowModifierFortitude;
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
            savBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).savingThrowModifierWill;
            savBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).savingThrowModifierWill;
            savBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).savingThrowModifierWill;
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
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierStr;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierStr;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierStr;
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
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierDex;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierDex;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierDex;
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
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierInt;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierInt;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierInt;
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
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierCha;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierCha;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierCha;
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
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierCon;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierCon;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierCon;
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
            attBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).attributeBonusModifierWis;
            attBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).attributeBonusModifierWis;
            attBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).attributeBonusModifierWis;
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
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueAcid;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueAcid;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueAcid;
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
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueNormal;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueNormal;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueNormal;
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
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueCold;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueCold;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueCold;
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
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueElectricity;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueElectricity;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueElectricity;
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
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueFire;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueFire;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueFire;
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
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValueMagic;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValueMagic;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValueMagic;
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
            md += mod.getItemByResRefForInfo(pc.NeckRefs.resref).damageTypeResistanceValuePoison;
            md += mod.getItemByResRefForInfo(pc.FeetRefs.resref).damageTypeResistanceValuePoison;
            md += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).damageTypeResistanceValuePoison;
            return md;
        }
        public int CalcBABAdders(Player pc)
        {
            int adder = 0;
            foreach (Effect ef in pc.effectsList)
            {
                adder += ef.babModifier;
            }
            return adder;
        }
        public int CalcACModifiers(Player pc)
        {
            int adder = 0;
            foreach (Effect ef in pc.effectsList)
            {
                adder += ef.acModifier;
            }
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
            moveBonuses += mod.getItemByResRefForInfo(pc.NeckRefs.resref).MovementPointModifier;
            moveBonuses += mod.getItemByResRefForInfo(pc.FeetRefs.resref).MovementPointModifier;
            moveBonuses += mod.getItemByResRefForInfo(pc.Ring2Refs.resref).MovementPointModifier;
            return moveBonuses;
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
                //IBMessageBox.Show(sf.gm, "failed running OnWhileEquipped scripts during UpdateStats()... see debug.txt");
                //sf.gm.errorLog(ex.ToString());
            }
        }
        public bool hasTrait(Player pc, string tag)
        {
            return pc.knownTraitsTags.Contains(tag);
        }

        //DEFAULT SCRIPTS
        public void dsWorldTime()
        {
            //mod.WorldTime +=  mod.TimePerRound;
            mod.WorldTime += mod.currentArea.TimePerSquare;
            //Code: Bleed to death at -20 hp
            spCnt++;
            foreach (Player pc in mod.playerList)
            {
                //check to see if allow HP to regen
                if (mod.getPlayerClass(pc.classTag).hpRegenTimeNeeded > 0)
                {
                    if (pc.hp > 0) //do not regen if dead
                    {
                        pc.hpRegenTimePassedCounter += mod.TimePerRound;
                        if (pc.hpRegenTimePassedCounter >= mod.getPlayerClass(pc.classTag).hpRegenTimeNeeded)
                        {
                            pc.hp++;
                            if (pc.hp > pc.hpMax)
                            {
                                pc.hp = pc.hpMax;
                            }
                            pc.hpRegenTimePassedCounter = 0;
                            gv.cc.addLogText("<font color='lime'>" + pc.name + " regen 1hp</font><br>");
                        }
                    }
                }
                //check to see if allow SP to regen
                if (mod.getPlayerClass(pc.classTag).spRegenTimeNeeded > 0)
                {
                    pc.spRegenTimePassedCounter += mod.TimePerRound;
                    if (pc.spRegenTimePassedCounter >= mod.getPlayerClass(pc.classTag).spRegenTimeNeeded)
                    {
                        pc.sp++;
                        if (pc.sp > pc.spMax) { pc.sp = pc.spMax; }
                        pc.spRegenTimePassedCounter = 0;
                        gv.cc.addLogText("<font color='lime'>" + pc.name + " regen 1sp</font><br>");
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
                        gv.cc.addLogText("<font color='red'>" + pc.name + " has DIED!" + "</font><BR>");
                    }
                }
            }
        }
        public void RunAllItemRegenerations(Player pc)
        {
            try
            {
                if (mod.getItemByResRefForInfo(pc.BodyRefs.resref).roundsPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.BodyRefs.resref).roundsPerSpRegenOutsideCombat);
                }
                if (mod.getItemByResRefForInfo(pc.BodyRefs.resref).roundsPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.BodyRefs.resref).roundsPerHpRegenOutsideCombat);
                }

                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).roundsPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.MainHandRefs.resref).roundsPerSpRegenOutsideCombat);
                }
                if (mod.getItemByResRefForInfo(pc.MainHandRefs.resref).roundsPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.MainHandRefs.resref).roundsPerHpRegenOutsideCombat);
                }

                if (mod.getItemByResRefForInfo(pc.OffHandRefs.resref).roundsPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.OffHandRefs.resref).roundsPerSpRegenOutsideCombat);
                }
                if (mod.getItemByResRefForInfo(pc.OffHandRefs.resref).roundsPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.OffHandRefs.resref).roundsPerHpRegenOutsideCombat);
                }

                if (mod.getItemByResRefForInfo(pc.RingRefs.resref).roundsPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.RingRefs.resref).roundsPerSpRegenOutsideCombat);
                }
                if (mod.getItemByResRefForInfo(pc.RingRefs.resref).roundsPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.RingRefs.resref).roundsPerHpRegenOutsideCombat);
                }

                if (mod.getItemByResRefForInfo(pc.HeadRefs.resref).roundsPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.HeadRefs.resref).roundsPerSpRegenOutsideCombat);
                }
                if (mod.getItemByResRefForInfo(pc.HeadRefs.resref).roundsPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.HeadRefs.resref).roundsPerHpRegenOutsideCombat);
                }

                if (mod.getItemByResRefForInfo(pc.NeckRefs.resref).roundsPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.NeckRefs.resref).roundsPerSpRegenOutsideCombat);
                }
                if (mod.getItemByResRefForInfo(pc.NeckRefs.resref).roundsPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.NeckRefs.resref).roundsPerHpRegenOutsideCombat);
                }

                if (mod.getItemByResRefForInfo(pc.FeetRefs.resref).roundsPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.FeetRefs.resref).roundsPerSpRegenOutsideCombat);
                }
                if (mod.getItemByResRefForInfo(pc.FeetRefs.resref).roundsPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.FeetRefs.resref).roundsPerHpRegenOutsideCombat);
                }

                if (mod.getItemByResRefForInfo(pc.Ring2Refs.resref).roundsPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.Ring2Refs.resref).roundsPerSpRegenOutsideCombat);
                }
                if (mod.getItemByResRefForInfo(pc.Ring2Refs.resref).roundsPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.Ring2Refs.resref).roundsPerHpRegenOutsideCombat);
                }

                if (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).roundsPerSpRegenOutsideCombat > 0)
                {
                    doRegenSp(pc, mod.getItemByResRefForInfo(pc.AmmoRefs.resref).roundsPerSpRegenOutsideCombat);
                }
                if (mod.getItemByResRefForInfo(pc.AmmoRefs.resref).roundsPerHpRegenOutsideCombat > 0)
                {
                    doRegenHp(pc, mod.getItemByResRefForInfo(pc.AmmoRefs.resref).roundsPerHpRegenOutsideCombat);
                }
            }
            catch (Exception ex)
            {
                //IBMessageBox.Show(sf.gm, "failed running OnWhileEquipped scripts during UpdateStats()... see debug.txt");
                //sf.gm.errorLog(ex.ToString());
            }
        }
        public void doRegenSp(Player pc, int rounds)
        {
            if (mod.WorldTime % (rounds * 6) == 0)
            {
                pc.sp++;
                if (pc.sp > pc.spMax) { pc.sp = pc.spMax; }
                //gv.cc.addLogText("<font color='lime'>" + pc.name + " regen 1sp</font><br>");
            }
        }
        public void doRegenHp(Player pc, int rounds)
        {
            if (mod.WorldTime % (rounds * 6) == 0)
            {
                pc.hp++;
                if (pc.hp > pc.hpMax) { pc.hp = pc.hpMax; }
                //gv.cc.addLogText("<font color='lime'>" + pc.name + " regen 1hp</font><br>");
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
                if ((pc.hp > 0) && (pc.charStatus.Equals("Dead")))
                {
                    pc.charStatus = "Alive";
                }
                MessageBox(pc.name + " gains " + healAmount + " HPs, now has " + pc.hp + "/" + pc.hpMax + "HPs");
                gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + healAmount + " HPs" + "</font><BR>");
            }
        }
        public void itForceRest()
        {
            foreach (Player pc in mod.playerList)
            {
                if (pc.hp > -20)
                {
                    pc.hp = pc.hpMax;
                    pc.sp = pc.spMax;
                }
            }
            MessageBox("Party safely rests until completely healed.");
            gv.cc.addLogText("<font color='lime'>" + "Party safely rests until completely healed." + "</font><BR>");
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
            MessageBox("Party safely rests until completely healed and the dead are raised.");
            gv.cc.addLogText("<font color='lime'>" + "Party safely rests until completely healed and the dead are raised." + "</font><BR>");
        }
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
        public void efHeld(object src, int parm1, int parm2)
        {
            //int parm1 = Integer.parseInt(p1); // parm1 = CurrentDurationInUnits (how many rounds have passed)
            //int parm2 = Integer.parseInt(p2); // parm2 = DurationInUnits (how long it lasts)

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                gv.cc.addLogText("<font color='yellow'>" + source.name + " is held, " + parm1 + " out" + "</font>" + "<BR>");
                gv.cc.addLogText("<font color='yellow'>" + " of " + parm2 + " seconds" + "</font>" + "<BR>");
                if (parm1 >= parm2)
                {
                    if (source.hp > 0)
                    {
                        source.charStatus = "Alive";
                    }
                    else
                    {
                        source.charStatus = "Dead";
                    }
                    gv.cc.addLogText("<font color='yellow'>" + source.name + " is no longer" + "</font>" + "<BR>");
                    gv.cc.addLogText("<font color='yellow'>" + " being held" + "</font>" + "<BR>");
                }
                else
                {
                    source.charStatus = "Held";
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                gv.cc.addLogText("<font color='yellow'>" + source.cr_name + " is held, " + parm1 + " out" + "</font>" + "<BR>");
                gv.cc.addLogText("<font color='yellow'>" + " of " + parm2 + " seconds" + "</font>" + "<BR>");
                if (parm1 >= parm2)
                {
                    source.cr_status = "Alive";
                    gv.cc.addLogText("<font color='yellow'>" + source.cr_name + " is no longer" + "</font>" + "<BR>");
                    gv.cc.addLogText("<font color='yellow'>" + " being held" + "</font>" + "<BR>");
                }
                else
                {
                    source.cr_status = "Held";
                }
            }
            else // don't know who cast this spell
            {
                //MessageBox.Show("Invalid script owner, not a Creature of PC");
                return;
            }
        }
        public void efSleep(object src, int parm1, int parm2)
        {
            //int parm1 = Integer.parseInt(p1); // parm1 = CurrentDurationInUnits (how many rounds have passed)
            //int parm2 = Integer.parseInt(p2); // parm2 = DurationInUnits (how long it lasts)

            if (src is Player) //player casting
            {
                Player source = (Player)src;
                gv.cc.addLogText("<font color='yellow'>" + source.name + " is sleeping, " + "</font><BR>");
                gv.cc.addLogText("<font color='yellow'>" + parm1 + " out of " + parm2 + " seconds" + "</font><BR>");
                if (parm1 >= parm2)
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
                    source.charStatus = "Held";
                }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                gv.cc.addLogText("<font color='yellow'>" + source.cr_name + " is sleeping, " + "</font><BR>");
                gv.cc.addLogText("<font color='yellow'>" + parm1 + " out of " + parm2 + " seconds" + "</font><BR>");
                if (parm1 >= parm2)
                {
                    source.cr_status = "Alive";
                    gv.cc.addLogText("<font color='yellow'>" + source.cr_name + " wakes up from sleep spell" + "</font><BR>");
                }
                else
                {
                    source.cr_status = "Held";
                }
            }
            else // don't know who cast this spell
            {
                //MessageBox.Show("Invalid script owner, not a Creature of PC");
                return;
            }
        }
        public void efRegenMinor(object src, int parm1, int parm2)
        {
            //int parm1 = Integer.parseInt(p1); // parm1 = CurrentDurationInUnits (how many rounds have passed)
            //int parm2 = Integer.parseInt(p2); // parm2 = DurationInUnits (how long it lasts)

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
                    if ((source.hp > 0) && (source.charStatus.Equals("Dead")))
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
        public void efPoisoned(object src, int parm1, int parm2, int damMax)
        {
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

        //SPELLS WIZARD
        public void spMageBolt(object src, object trg)
        {
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
                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + target.cr_name + "</font><BR>");
                        try
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
                                    catch (Exception e)
                                    {
                                        //e.printStackTrace();
                                    }
                                }
                            }

                            //mod.currentEncounter.encounterCreatureList.remove(target);
                        }
                        catch (Exception e)
                        {
                            //e.printStackTrace();
                        }
                    }
                }
                //Do floaty text damage
                gv.cc.floatyTextOn = true;
                gv.cc.addFloatyText(new Coordinate(target.combatLocX, target.combatLocY), damageTotal + "");
                //TODOgv.postDelayed(gv.screenCombat.doFloatyText, 100);

                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
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
                        gv.cc.addLogText("<font color='red'>" + target.name + " drops unconcious!" + "</font><BR>");
                        target.charStatus = "Dead";
                    }
                }
                //Do floaty text damage
                gv.cc.floatyTextOn = true;
                gv.cc.addFloatyText(new Coordinate(target.combatLocX, target.combatLocY), damageTotal + "");
                //TODOgv.postDelayed(gv.screenCombat.doFloatyText, 100);

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
        public void spSleep(object src, object trg)
        {
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Coordinate target = (Coordinate)trg;

                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    // if in range of radius of x and radius of y
                    if ((crt.combatLocX >= target.X - gv.cc.currentSelectedSpell.aoeRadius) && (crt.combatLocX <= target.X + gv.cc.currentSelectedSpell.aoeRadius))
                    {
                        if ((crt.combatLocY >= target.Y - gv.cc.currentSelectedSpell.aoeRadius) && (crt.combatLocY <= target.Y + gv.cc.currentSelectedSpell.aoeRadius))
                        {
                            int saveChkRoll = RandInt(20);
                            int saveChk = saveChkRoll + crt.will;
                            int DC = 13;
                            if (saveChk >= DC) //passed save check
                            {
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " avoids the sleep spell" + "</font><BR>");
                                if (mod.debugMode)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.will + " >= " + DC + "</font><BR>");
                                }
                            }
                            else //failed check
                            {
                                gv.cc.addLogText("<font color='red'>" + crt.cr_name + " is held by a sleep spell" + "</font><BR>");
                                crt.cr_status = "Held";
                                Effect ef = mod.getEffectByTag("sleep");
                                crt.AddEffectByObject(ef, mod.WorldTime);
                            }
                        }
                    }
                }
                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Coordinate target = (Coordinate)trg;

                foreach (Player pc in mod.playerList)
                {
                    // if in range of radius of x and radius of y
                    if ((pc.combatLocX >= target.X - SpellToCast.aoeRadius) && (pc.combatLocX <= target.X + SpellToCast.aoeRadius))
                    {
                        if ((pc.combatLocY >= target.Y - SpellToCast.aoeRadius) && (pc.combatLocY <= target.Y + SpellToCast.aoeRadius))
                        {
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
                                pc.AddEffectByObject(ef, mod.WorldTime);
                            }
                        }
                    }
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
        public void spMageArmor(object src, object trg)
        {
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
                ef.durationInUnits = numberOfRounds * 6;
                gv.cc.addLogText("<font color='lime'>" + "Mage Armor is applied on " + target.name + "<BR>");
                gv.cc.addLogText("<font color='lime'>" + " for " + numberOfRounds + " round(s)" + "</font><BR>");
                target.AddEffectByObject(ef, mod.WorldTime);
                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Creature target = (Creature)trg;

                int numberOfRounds = (source.cr_level * 20); //20 rounds per level
                Effect ef = mod.getEffectByTag("mageArmor").DeepCopy();
                ef.durationInUnits = numberOfRounds * 6;
                gv.cc.addLogText("<font color='lime'>" + "Mage Armor is applied on " + target.cr_name + "<BR>");
                gv.cc.addLogText("<font color='lime'>" + " for " + numberOfRounds + " round(s)" + "</font><BR>");
                target.AddEffectByObject(ef, mod.WorldTime);
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
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Player target = (Player)trg;

                Effect ef = mod.getEffectByTag("minorRegen");
                gv.cc.addLogText("<font color='lime'>" + "Minor Regeneration is applied on " + target.name + "</font><BR>");
                target.AddEffectByObject(ef, mod.WorldTime);
                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Creature target = (Creature)trg;

                Effect ef = mod.getEffectByTag("minorRegen");
                gv.cc.addLogText("<font color='lime'>" + "Minor Regeneration is applied on " + target.cr_name + "</font><BR>");
                target.AddEffectByObject(ef, mod.WorldTime);
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
        public void spWeb(object src, object trg)
        {
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Coordinate target = (Coordinate)trg;

                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    // if in range of radius of x and radius of y
                    if ((crt.combatLocX >= target.X - gv.cc.currentSelectedSpell.aoeRadius) && (crt.combatLocX <= target.X + gv.cc.currentSelectedSpell.aoeRadius))
                    {
                        if ((crt.combatLocY >= target.Y - gv.cc.currentSelectedSpell.aoeRadius) && (crt.combatLocY <= target.Y + gv.cc.currentSelectedSpell.aoeRadius))
                        {
                            int saveChkRoll = RandInt(20);
                            int saveChk = saveChkRoll + crt.reflex;
                            int DC = 13;
                            if (saveChk >= DC) //passed save check
                            {
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " avoids the web spell" + "</font><BR>");
                                if (mod.debugMode)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.reflex + " >= " + DC + "</font><BR>");
                                }
                            }
                            else //failed check
                            {
                                gv.cc.addLogText("<font color='red'>" + crt.cr_name + " is held by a web spell" + "</font><BR>");
                                crt.cr_status = "Held";
                                Effect ef = mod.getEffectByTag("web");
                                crt.AddEffectByObject(ef, mod.WorldTime);
                            }
                        }
                    }
                }
                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Coordinate target = (Coordinate)trg;

                foreach (Player pc in mod.playerList)
                {
                    // if in range of radius of x and radius of y
                    if ((pc.combatLocX >= target.X - SpellToCast.aoeRadius) && (pc.combatLocX <= target.X + SpellToCast.aoeRadius))
                    {
                        if ((pc.combatLocY >= target.Y - SpellToCast.aoeRadius) && (pc.combatLocY <= target.Y + SpellToCast.aoeRadius))
                        {
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
                                pc.AddEffectByObject(ef, mod.WorldTime);
                            }
                        }
                    }
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
        public void spIceStorm(object src, object trg)
        {
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Coordinate target = (Coordinate)trg;

                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    // if in range of radius of x and radius of y
                    if ((crt.combatLocX >= target.X - gv.cc.currentSelectedSpell.aoeRadius) && (crt.combatLocX <= target.X + gv.cc.currentSelectedSpell.aoeRadius))
                    {
                        if ((crt.combatLocY >= target.Y - gv.cc.currentSelectedSpell.aoeRadius) && (crt.combatLocY <= target.Y + gv.cc.currentSelectedSpell.aoeRadius))
                        {
                            float resist = (float)(1f - ((float)crt.damageTypeResistanceValueCold / 100f));
                            float damage = source.classLevel * RandInt(3);
                            int iceDam = (int)(damage * resist);

                            int saveChkRoll = RandInt(20);
                            int saveChk = saveChkRoll + crt.reflex;
                            int DC = 13;
                            if (saveChk >= DC) //passed save check
                            {
                                iceDam = iceDam / 2;
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " evades most of the Ice Storm spell" + "</font><BR>");
                                if (mod.debugMode)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.reflex + " >= " + DC + "</font><BR>");
                                }
                            }
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                            + " iceDam = " + iceDam + "</font>" +
                                            "<BR>");
                            }
                            gv.cc.addLogText("<font color='aqua'>" + source.name + "</font>" +
                                    "<font color='white'>" + " attacks " + "</font>" +
                                    "<font color='silver'>" + crt.cr_name + "</font>" +
                                    "<BR>");
                            gv.cc.addLogText("<font color='white'>" + "Ice Storm (" + "</font>");
                            gv.cc.addLogText("<font color='lime'>" + iceDam + "</font>" +
                                    "<font color='white'>" + " damage)" + "</font>" +
                                    "<BR>");
                            crt.hp -= iceDam;
                            if (crt.hp <= 0)
                            {
                                gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                            }
                            //Do floaty text damage
                            gv.cc.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), iceDam + "");
                            //gv.postDelayed(gv.doFloatyText, 100);
                        }
                    }
                }
                for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
                {
                    if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                    {
                        try
                        {
                            //do OnDeath LOGIC TREE
                            //REMOVEgv.cc.doLogicTreeBasedOnTag(mod.currentEncounter.encounterCreatureList[x].onDeathLogicTree, mod.currentEncounter.encounterCreatureList[x].onDeathParms);
                            //do OnDeath IBScript
                            gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                            mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                            mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                        }
                        catch (Exception e)
                        {
                            //e.printStackTrace();
                        }
                    }
                }

                foreach (Player pc in mod.playerList)
                {
                    // if in range of radius of x and radius of y
                    if ((pc.combatLocX >= target.X - gv.cc.currentSelectedSpell.aoeRadius) && (pc.combatLocX <= target.X + gv.cc.currentSelectedSpell.aoeRadius))
                    {
                        if ((pc.combatLocY >= target.Y - gv.cc.currentSelectedSpell.aoeRadius) && (pc.combatLocY <= target.Y + gv.cc.currentSelectedSpell.aoeRadius))
                        {
                            float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalCold / 100f));
                            float damage = source.classLevel * RandInt(3);
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
                                gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                            + " iceDam = " + iceDam + "</font>" +
                                            "<BR>");
                            }
                            gv.cc.addLogText("<font color='aqua'>" + source.name + "</font>" +
                                    "<font color='white'>" + " attacks " + "</font>" +
                                    "<font color='silver'>" + pc.name + "</font>" +
                                    "<BR>");
                            gv.cc.addLogText("<font color='white'>" + "Ice Storm (" + "</font>");
                            gv.cc.addLogText("<font color='lime'>" + iceDam + "</font>" +
                                    "<font color='white'>" + " damage)" + "</font>" +
                                    "<BR>");
                            pc.hp -= iceDam;
                            if (pc.hp <= 0)
                            {
                                gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                                pc.charStatus = "Dead";
                            }
                            //Do floaty text damage
                            gv.cc.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), iceDam + "");
                            //gv.postDelayed(gv.doFloatyText, 100);
                        }
                    }
                }
                //Do floaty text damage
                //TODOgv.postDelayed(gv.screenCombat.doFloatyText, 100);

                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Coordinate target = (Coordinate)trg;

                foreach (Player pc in mod.playerList)
                {
                    // if in range of radius of x and radius of y
                    if ((pc.combatLocX >= target.X - SpellToCast.aoeRadius) && (pc.combatLocX <= target.X + SpellToCast.aoeRadius))
                    {
                        if ((pc.combatLocY >= target.Y - SpellToCast.aoeRadius) && (pc.combatLocY <= target.Y + SpellToCast.aoeRadius))
                        {
                            float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalCold / 100f));
                            float damage = source.cr_level * RandInt(3);
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
                                gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                            + " iceDam = " + iceDam + "</font>" +
                                            "<BR>");
                            }
                            gv.cc.addLogText("<font color='aqua'>" + source.cr_name + "</font>" +
                                    "<font color='white'>" + " attacks " + "</font>" +
                                    "<font color='silver'>" + pc.name + "</font>" +
                                    "<BR>");
                            gv.cc.addLogText("<font color='white'>" + "Ice Storm (" + "</font>");
                            gv.cc.addLogText("<font color='lime'>" + iceDam + "</font>" +
                                    "<font color='white'>" + " damage)" + "</font>" +
                                    "<BR>");
                            pc.hp -= iceDam;
                            if (pc.hp <= 0)
                            {
                                gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                                pc.charStatus = "Dead";
                            }
                            //Do floaty text damage
                            gv.cc.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), iceDam + "");
                            //gv.postDelayed(gv.doFloatyText, 100);
                        }
                    }
                }
                //Do floaty text damage
                //TODOgv.postDelayed(gv.screenCombat.doFloatyText, 100);

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
        public void spFireball(object src, object trg)
        {
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Coordinate target = (Coordinate)trg;

                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    // if in range of radius of x and radius of y
                    if ((crt.combatLocX >= target.X - gv.cc.currentSelectedSpell.aoeRadius) && (crt.combatLocX <= target.X + gv.cc.currentSelectedSpell.aoeRadius))
                    {
                        if ((crt.combatLocY >= target.Y - gv.cc.currentSelectedSpell.aoeRadius) && (crt.combatLocY <= target.Y + gv.cc.currentSelectedSpell.aoeRadius))
                        {
                            float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                            float damage = source.classLevel * RandInt(6);
                            int fireDam = (int)(damage * resist);

                            int saveChkRoll = RandInt(20);
                            int saveChk = saveChkRoll + crt.reflex;
                            int DC = 13;
                            if (saveChk >= DC) //passed save check
                            {
                                fireDam = fireDam / 2;
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " evades most of the Fireball spell" + "</font><BR>");
                                if (mod.debugMode)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.reflex + " >= " + DC + "</font><BR>");
                                }
                            }
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                            + " fireDam = " + fireDam + "</font>" +
                                            "<BR>");
                            }
                            gv.cc.addLogText("<font color='aqua'>" + source.name + "</font>" +
                                    "<font color='white'>" + " attacks " + "</font>" +
                                    "<font color='silver'>" + crt.cr_name + "</font>" +
                                    "<BR>");
                            gv.cc.addLogText("<font color='white'>" + "Fireball (" + "</font>");
                            gv.cc.addLogText("<font color='lime'>" + fireDam + "</font>" +
                                    "<font color='white'>" + " damage)" + "</font>" +
                                    "<BR>");
                            crt.hp -= fireDam;
                            if (crt.hp <= 0)
                            {
                                gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                            }
                            //Do floaty text damage
                            gv.cc.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), fireDam + "");
                            //gv.postDelayed(gv.doFloatyText, 100);
                        }
                    }
                }
                for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
                {
                    if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                    {
                        try
                        {
                            //do OnDeath LOGIC TREE
                            //REMOVEgv.cc.doLogicTreeBasedOnTag(mod.currentEncounter.encounterCreatureList[x].onDeathLogicTree, mod.currentEncounter.encounterCreatureList[x].onDeathParms);
                            //do OnDeath IBScript
                            gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                            mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                            mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                        }
                        catch (Exception e)
                        {
                            //e.printStackTrace();
                        }
                    }
                }
                foreach (Player pc in mod.playerList)
                {
                    // if in range of radius of x and radius of y
                    if ((pc.combatLocX >= target.X - gv.cc.currentSelectedSpell.aoeRadius) && (pc.combatLocX <= target.X + gv.cc.currentSelectedSpell.aoeRadius))
                    {
                        if ((pc.combatLocY >= target.Y - gv.cc.currentSelectedSpell.aoeRadius) && (pc.combatLocY <= target.Y + gv.cc.currentSelectedSpell.aoeRadius))
                        {
                            float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f));
                            float damage = source.classLevel * RandInt(6);
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
                                gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                            + " fireDam = " + fireDam + "</font>" +
                                            "<BR>");
                            }
                            gv.cc.addLogText("<font color='aqua'>" + source.name + "</font>" +
                                    "<font color='white'>" + " attacks " + "</font>" +
                                    "<font color='silver'>" + pc.name + "</font>" +
                                    "<BR>");
                            gv.cc.addLogText("<font color='white'>" + "Fireball (" + "</font>");
                            gv.cc.addLogText("<font color='lime'>" + fireDam + "</font>" +
                                    "<font color='white'>" + " damage)" + "</font>" +
                                    "<BR>");
                            pc.hp -= fireDam;
                            if (pc.hp <= 0)
                            {
                                gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                                pc.charStatus = "Dead";
                            }
                            //Do floaty text damage
                            gv.cc.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), fireDam + "");
                            //gv.postDelayed(gv.doFloatyText, 100);
                        }
                    }
                }
                //Do floaty text damage
                //TODOgv.postDelayed(gv.screenCombat.doFloatyText, 100);

                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Coordinate target = (Coordinate)trg;

                foreach (Player pc in mod.playerList)
                {
                    // if in range of radius of x and radius of y
                    if ((pc.combatLocX >= target.X - SpellToCast.aoeRadius) && (pc.combatLocX <= target.X + SpellToCast.aoeRadius))
                    {
                        if ((pc.combatLocY >= target.Y - SpellToCast.aoeRadius) && (pc.combatLocY <= target.Y + SpellToCast.aoeRadius))
                        {
                            float resist = (float)(1f - ((float)pc.damageTypeResistanceTotalFire / 100f));
                            float damage = source.cr_level * RandInt(6);
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
                                gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                            + " fireDam = " + fireDam + "</font>" +
                                            "<BR>");
                            }
                            gv.cc.addLogText("<font color='aqua'>" + source.cr_name + "</font>" +
                                    "<font color='white'>" + " attacks " + "</font>" +
                                    "<font color='silver'>" + pc.name + "</font>" +
                                    "<BR>");
                            gv.cc.addLogText("<font color='white'>" + "Fireball (" + "</font>");
                            gv.cc.addLogText("<font color='lime'>" + fireDam + "</font>" +
                                    "<font color='white'>" + " damage)" + "</font>" +
                                    "<BR>");
                            pc.hp -= fireDam;
                            if (pc.hp <= 0)
                            {
                                gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                                pc.charStatus = "Dead";
                            }
                            //Do floaty text damage
                            gv.cc.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), fireDam + "");
                            //gv.postDelayed(gv.doFloatyText, 100);
                        }
                    }
                }
                //Do floaty text damage
                //TODOgv.postDelayed(gv.screenCombat.doFloatyText, 100);

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

        //SPELLS CLERIC
        public void spHeal(object src, object trg, int healAmount)
        {
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
                    if ((target.hp > 0) && (target.charStatus.Equals("Dead")))
                    {
                        target.charStatus = "Alive";
                    }
                    //MessageBox(pc.name + " gains " + healAmount + " HPs");
                    gv.cc.addLogText("<font color='lime'>" + target.name + " gains " + healAmount + " HPs" + "</font><BR>");
                }
                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
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
                        if ((pc.hp > 0) && (pc.charStatus.Equals("Dead")))
                        {
                            pc.charStatus = "Alive";
                        }
                        //MessageBox(pc.name + " gains " + healAmount + " HPs");
                        gv.cc.addLogText("<font color='lime'>" + pc.name + " gains " + healAmount + " HPs" + "</font><BR>");
                    }
                }
                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
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
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                //Player target = (Player)trg;

                foreach (Player pc in mod.playerList)
                {
                    int numberOfRounds = (source.classLevel * 5); //5 rounds per level
                    Effect ef = mod.getEffectByTag("bless").DeepCopy();
                    ef.durationInUnits = numberOfRounds * 6;
                    gv.cc.addLogText("<font color='lime'>" + "Bless is applied on " + pc.name
                            + " for " + numberOfRounds + " round(s)" + "</font>" +
                            "<BR>");
                    pc.AddEffectByObject(ef, mod.WorldTime);
                }
                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                //Creature target = (Creature)trg;

                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    int numberOfRounds = (source.cr_level * 5); //5 rounds per level
                    Effect ef = mod.getEffectByTag("bless").DeepCopy();
                    ef.durationInUnits = numberOfRounds * 6;
                    gv.cc.addLogText("<font color='lime'>" + "Bless is applied on " + crt.cr_name
                            + " for " + numberOfRounds + " round(s)" + "</font>" +
                            "<BR>");
                    crt.AddEffectByObject(ef, mod.WorldTime);
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
                        gv.cc.addLogText("<font color='lime'>" + "You killed the " + target.cr_name + "</font><BR>");
                        try
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
                                    catch (Exception e)
                                    {
                                        //e.printStackTrace();
                                    }
                                }
                            }
                            mod.currentEncounter.encounterCreatureList.Remove(target);
                        }
                        catch (Exception e)
                        {
                            //e.printStackTrace();
                        }
                    }
                }
                //Do floaty text damage
                gv.cc.floatyTextOn = true;
                gv.cc.addFloatyText(new Coordinate(target.combatLocX, target.combatLocY), damageTotal + "");
                //TODOgv.postDelayed(gv.screenCombat.doFloatyText, 100);

                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
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
                        gv.cc.addLogText("<font color='red'>" + target.name + " drops unconcious!" + "</font><BR>");
                        target.charStatus = "Dead";
                    }
                }
                //Do floaty text damage
                gv.cc.floatyTextOn = true;
                gv.cc.addFloatyText(new Coordinate(target.combatLocX, target.combatLocY), damageTotal + "");
                //TODOgv.postDelayed(gv.screenCombat.doFloatyText, 100);

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
        public void spBlastOfLight(Object src, Object trg)
        {
            if (src is Player) //player casting
            {
                Player source = (Player)src;
                Coordinate target = (Coordinate)trg;

                foreach (Creature crt in mod.currentEncounter.encounterCreatureList)
                {
                    // if in range of radius of x and radius of y
                    if ((crt.combatLocX >= target.X - gv.cc.currentSelectedSpell.aoeRadius) && (crt.combatLocX <= target.X + gv.cc.currentSelectedSpell.aoeRadius))
                    {
                        if ((crt.combatLocY >= target.Y - gv.cc.currentSelectedSpell.aoeRadius) && (crt.combatLocY <= target.Y + gv.cc.currentSelectedSpell.aoeRadius))
                        {
                            float resist = (float)(1f - ((float)crt.damageTypeResistanceValueFire / 100f));
                            float damage = 2 * RandInt(6);
                            int fireDam = (int)(damage * resist);

                            int saveChkRoll = RandInt(20);
                            int saveChk = saveChkRoll + crt.reflex;
                            int DC = 13;
                            if (saveChk >= DC) //passed save check
                            {
                                fireDam = fireDam / 2;
                                gv.cc.addLogText("<font color='yellow'>" + crt.cr_name + " evades most of the Blast of Light spell" + "</font><BR>");
                                if (mod.debugMode)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + saveChkRoll + " + " + crt.reflex + " >= " + DC + "</font><BR>");
                                }
                            }
                            if (mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                            + " fireDam = " + fireDam + "</font>" +
                                            "<BR>");
                            }
                            gv.cc.addLogText("<font color='aqua'>" + source.name + "</font>" +
                                    "<font color='white'>" + " attacks " + "</font>" +
                                    "<font color='silver'>" + crt.cr_name + "</font>" +
                                    "<BR>");
                            gv.cc.addLogText("<font color='white'>" + "Blast of Light (" + "</font>");
                            gv.cc.addLogText("<font color='lime'>" + fireDam + "</font>" +
                                    "<font color='white'>" + " damage)" + "</font>" +
                                    "<BR>");
                            crt.hp -= fireDam;
                            if (crt.hp <= 0)
                            {
                                gv.cc.addLogText("<font color='lime'>" + "You killed the " + crt.cr_name + "</font><BR>");
                            }
                            //Do floaty text damage
                            gv.cc.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(crt.combatLocX, crt.combatLocY), fireDam + "");
                            //gv.postDelayed(gv.doFloatyText, 100);
                        }
                    }
                }
                for (int x = mod.currentEncounter.encounterCreatureList.Count - 1; x >= 0; x--)
                {
                    if (mod.currentEncounter.encounterCreatureList[x].hp <= 0)
                    {
                        try
                        {
                            //do OnDeath LOGIC TREE
                            //REMOVEgv.cc.doLogicTreeBasedOnTag(mod.currentEncounter.encounterCreatureList[x].onDeathLogicTree, mod.currentEncounter.encounterCreatureList[x].onDeathParms);
                            //do OnDeath IBScript
                            gv.cc.doIBScriptBasedOnFilename(mod.currentEncounter.encounterCreatureList[x].onDeathIBScript, mod.currentEncounter.encounterCreatureList[x].onDeathIBScriptParms);
                            mod.currentEncounter.encounterCreatureList.RemoveAt(x);
                            mod.currentEncounter.encounterCreatureRefsList.RemoveAt(x);
                        }
                        catch (Exception e)
                        {
                            //e.printStackTrace();
                        }
                    }
                }
                foreach (Player pc in mod.playerList)
                {
                    // if in range of radius of x and radius of y
                    if ((pc.combatLocX >= target.X - gv.cc.currentSelectedSpell.aoeRadius) && (pc.combatLocX <= target.X + gv.cc.currentSelectedSpell.aoeRadius))
                    {
                        if ((pc.combatLocY >= target.Y - gv.cc.currentSelectedSpell.aoeRadius) && (pc.combatLocY <= target.Y + gv.cc.currentSelectedSpell.aoeRadius))
                        {
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
                                gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                            + " fireDam = " + fireDam + "</font>" +
                                            "<BR>");
                            }
                            gv.cc.addLogText("<font color='aqua'>" + source.name + "</font>" +
                                    "<font color='white'>" + " attacks " + "</font>" +
                                    "<font color='silver'>" + pc.name + "</font>" +
                                    "<BR>");
                            gv.cc.addLogText("<font color='white'>" + "Blast of Light (" + "</font>");
                            gv.cc.addLogText("<font color='lime'>" + fireDam + "</font>" +
                                    "<font color='white'>" + " damage)" + "</font>" +
                                    "<BR>");
                            pc.hp -= fireDam;
                            if (pc.hp <= 0)
                            {
                                gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                                pc.charStatus = "Dead";
                            }
                            //Do floaty text damage
                            gv.cc.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), fireDam + "");
                            //gv.postDelayed(gv.doFloatyText, 100);
                        }
                    }
                }
                //Do floaty text damage
                //TODOgv.postDelayed(gv.screenCombat.doFloatyText, 100);

                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
            }
            else if (src is Creature) //creature casting
            {
                Creature source = (Creature)src;
                Coordinate target = (Coordinate)trg;

                foreach (Player pc in mod.playerList)
                {
                    // if in range of radius of x and radius of y
                    if ((pc.combatLocX >= target.X - SpellToCast.aoeRadius) && (pc.combatLocX <= target.X + SpellToCast.aoeRadius))
                    {
                        if ((pc.combatLocY >= target.Y - SpellToCast.aoeRadius) && (pc.combatLocY <= target.Y + SpellToCast.aoeRadius))
                        {
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
                                gv.cc.addLogText("<font color='yellow'>" + "resist = " + resist + " damage = " + damage
                                            + " fireDam = " + fireDam + "</font>" +
                                            "<BR>");
                            }
                            gv.cc.addLogText("<font color='aqua'>" + source.cr_name + "</font>" +
                                    "<font color='white'>" + " attacks " + "</font>" +
                                    "<font color='silver'>" + pc.name + "</font>" +
                                    "<BR>");
                            gv.cc.addLogText("<font color='white'>" + "Blast of Light (" + "</font>");
                            gv.cc.addLogText("<font color='lime'>" + fireDam + "</font>" +
                                    "<font color='white'>" + " damage)" + "</font>" +
                                    "<BR>");
                            pc.hp -= fireDam;
                            if (pc.hp <= 0)
                            {
                                gv.cc.addLogText("<font color='red'>" + pc.name + " drops unconcious!" + "</font><BR>");
                                pc.charStatus = "Dead";
                            }
                            //Do floaty text damage
                            gv.cc.floatyTextOn = true;
                            gv.cc.addFloatyText(new Coordinate(pc.combatLocX, pc.combatLocY), fireDam + "");
                            //gv.postDelayed(gv.doFloatyText, 100);
                        }
                    }
                }
                //Do floaty text damage
                //TODOgv.postDelayed(gv.screenCombat.doFloatyText, 100);

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
        public void spHold(Object src, Object trg)
        {
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
                    target.AddEffectByObject(ef, mod.WorldTime);
                }
                source.sp -= gv.cc.currentSelectedSpell.costSP;
                if (source.sp < 0) { source.sp = 0; }
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
                    target.AddEffectByObject(ef, mod.WorldTime);
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
