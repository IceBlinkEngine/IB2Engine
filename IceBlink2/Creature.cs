using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;
using Bitmap = SharpDX.Direct2D1.Bitmap;
//using IceBlink;

namespace IceBlink2
{
    public class Creature 
    {

        [JsonIgnore]
        public List<String> tagsOfEffectsToRemoveOnMove = new List<String>();

        public float maxTurnTimeCounter = 0;

        public bool showNormalFrame = false;//0
        public bool showAttackingFrame = false;//1
        public bool showWalkingFrame = false;//2
        public bool showIdlingFrame = false;//3
        public bool showBreathingFrame = false;//4


        public float walkAnimationDelayCounter = 0;
        public float idleAnimationDelayCounter = 0;
        public float breathAnimationDelayCounter = 0;

        public float hurdle = 10f;
        public int currentFrameNumber = 0;

        public string cr_tokenFilename = "blank.png";

        public string factionTag = "none";

        [JsonIgnore]
	    public Bitmap token;
        public float roamDistanceX = 0;
        public int percentChanceToCastSpell = 100; //for GeneralCaster, this is the % chance that they will try and cast a spell on each turn (100 = 100%, 80 = 80%, etc.)    	    
        public float roamDistanceY = 0;
        public float straightLineDistanceX = 0;
        public float straightLineDistanceY = 0;
        public bool goDown = false;
        public bool goRight = false;
        public float inactiveTimer = 0;

        public bool combatFacingLeft = true;
        public int combatFacing = 4; //numpad directions (7,8,9,4,6,1,2,3)
	    public int combatLocX = 0;
	    public int combatLocY = 0;
        public int moveDistance = 5;
        public int initiativeBonus = 0;
        public int moveOrder = 0;
	    public string cr_name = "newCreature";	
	    public string cr_tag = "newTag";
	    public string cr_resref = "newResRef";
	    public string cr_desc = ""; //detailed description
	    public int cr_level = 1;
	    public int hp = 10;
	    public int hpMax = 10;
	    public int sp = 50;
        public int spMax = 50;
        public int cr_XP = 10;
	    public int AC = 10;
	    public string cr_status = "Alive"; //Alive, Dead, Held
	    public int cr_att = 0;
	    public int cr_attRange = 1;
	    public int cr_damageNumDice = 1; //number of dice to roll for damage
	    public int cr_damageDie = 4; //type of dice to roll for damage
	    public int cr_damageAdder = 0;
	    public string cr_category = "Melee"; //catergory type (Ranged, Melee)
	    public string cr_projSpriteFilename = "none"; //sprite filename including .spt
	    public string cr_spriteEndingFilename = "none"; //sprite to use for end effect of projectiles
	    public string cr_attackSound = "none"; //Filename of sound to play when the creature attacks (no extension)
	    public int cr_numberOfAttacks = 1;
	    public string cr_ai = "BasicAttacker";
	    public int fortitude = 0;
	    public int will = 0;
	    public int reflex = 0; 
	    public int damageTypeResistanceValueAcid = 0;
	    public int damageTypeResistanceValueNormal = 0;
	    public int damageTypeResistanceValueCold = 0;
	    public int damageTypeResistanceValueElectricity = 0;
	    public int damageTypeResistanceValueFire = 0;
	    public int damageTypeResistanceValueMagic = 0;
	    public int damageTypeResistanceValuePoison = 0;
	    public string cr_typeOfDamage = "Normal"; //Normal,Acid,Cold,Electricity,Fire,Magic,Poison
	    public string onScoringHit = "none";
	    public string onScoringHitParms = "none";
        public string onScoringHitCastSpellTag = "none";
        public string onDeathIBScript = "none";
        public string onDeathIBScriptParms = ""; 
	    public List<string> knownSpellsTags = new List<string>();
        public List<LocalInt> castChances = new List<LocalInt>();

        public List<Effect> cr_effectsList = new List<Effect>();
	    public List<LocalInt> CreatureLocalInts = new List<LocalInt>();
	    public List<LocalString> CreatureLocalStrings = new List<LocalString>();
        public List<int> destinationPixelPositionXList = new List<int>();
        public List<int> destinationPixelPositionYList = new List<int>();
        //Turned the 2 ints below to floats for storing pix coordinates with higher precision
        public float currentPixelPositionX = 0;
        public float currentPixelPositionY = 0;
        public int pixelMoveSpeed = 1;
        public Coordinate newCoor = new Coordinate(-1,-1);
        public float glideAdderX = 0;
        public float glideAdderY = 0;
        public int hpLastTurn = -1;
        public int hpRegenerationPerRound = 0;
        public int spRegenerationPerRound = 0;

        //creature size system
        //1=normal, 2=wide, 3=tall, 4=large
        //normal is 100px width, 100px height each frame (1x1 squares in battle)
        //wide is 200px width, 100px height each frame (2x1 squares in battle)
        //tall is is 100px width, 200px height each frame (1x2 squares in battle)
        //large is 200px width, 200px height each frame (2x2 squares in battle)
        public int creatureSize = 1;
        public List<Coordinate> tokenCoveredSquares = new List<Coordinate>();

        //The two below are not yet implemented 
        public string labelForCastAction = "none";
        public string labelForSpellsButtonInCombat = "SPELL";

        public int stayDurationInTurns = 100000;

        public string targetPcTag = "none";

        public int percentRequirementOfTargetInjuryForHealSpells = 50;
        public int percentRequirementOfTargetSPLossForRestoreSPSpells = 50;

        public Creature()
	    {
		
	    }
	
	    public Creature DeepCopy()
	    {
		    Creature copy = new Creature();

            copy.maxTurnTimeCounter = maxTurnTimeCounter;
            foreach (String et in this.tagsOfEffectsToRemoveOnMove)
            {
                copy.tagsOfEffectsToRemoveOnMove.Add(et);
            }
             copy.showNormalFrame = showNormalFrame;//0
            copy.showAttackingFrame = showAttackingFrame;//1

            copy.showWalkingFrame = showWalkingFrame;//2
            copy.showIdlingFrame = showIdlingFrame;//3
            copy.showBreathingFrame = showBreathingFrame;//4

            copy.walkAnimationDelayCounter = walkAnimationDelayCounter;
            copy.idleAnimationDelayCounter = idleAnimationDelayCounter;
            copy.breathAnimationDelayCounter = breathAnimationDelayCounter;

            copy.hurdle = hurdle;
            copy.currentFrameNumber = currentFrameNumber;

            copy.factionTag = this.factionTag;
            copy.percentRequirementOfTargetInjuryForHealSpells = this.percentRequirementOfTargetInjuryForHealSpells;
            copy.percentRequirementOfTargetSPLossForRestoreSPSpells = this.percentRequirementOfTargetInjuryForHealSpells;
            copy.targetPcTag = this.targetPcTag;
            copy.stayDurationInTurns = this.stayDurationInTurns;
            copy.percentChanceToCastSpell = this.percentChanceToCastSpell;
            copy.roamDistanceX = this.roamDistanceX;
            copy.roamDistanceY = this.roamDistanceY;
            copy.straightLineDistanceX = this.straightLineDistanceX;
            copy.straightLineDistanceY = this.straightLineDistanceY;
            copy.goDown = this.goDown;
            copy.goRight = this.goRight;
            copy.inactiveTimer = this.inactiveTimer; ;
            copy.cr_tokenFilename = this.cr_tokenFilename;
		    copy.combatFacingLeft = this.combatFacingLeft;
            copy.combatFacing = this.combatFacing;
		    copy.combatLocX = this.combatLocX;
		    copy.combatLocY = this.combatLocY;
            copy.moveDistance = this.moveDistance;
            copy.initiativeBonus = this.initiativeBonus;
		    copy.cr_name = this.cr_name;	
		    copy.cr_tag = this.cr_tag;
		    copy.cr_resref = this.cr_resref;
		    copy.cr_desc = this.cr_desc;
		    copy.cr_level = this.cr_level;
		    copy.hp = this.hp;
		    copy.hpMax = this.hpMax;
            
            //idea is that creatures are generated at start of each batte by copying template, ie they always start with maxSP in full height (= initial sp)
            copy.spMax = this.sp;

            copy.sp = this.sp;
		    copy.cr_XP = this.cr_XP;
		    copy.AC = this.AC;
		    copy.cr_status = this.cr_status;
		    copy.cr_att = this.cr_att;
		    copy.cr_attRange = this.cr_attRange;
		    copy.cr_damageNumDice = this.cr_damageNumDice;
		    copy.cr_damageDie = this.cr_damageDie;
		    copy.cr_damageAdder = this.cr_damageAdder;
		    copy.cr_category = this.cr_category;
		    copy.cr_projSpriteFilename = this.cr_projSpriteFilename;
		    copy.cr_spriteEndingFilename = this.cr_spriteEndingFilename;
		    copy.cr_attackSound = this.cr_attackSound;
		    copy.cr_numberOfAttacks = this.cr_numberOfAttacks;
		    copy.cr_ai = this.cr_ai;
		    copy.fortitude = this.fortitude;
		    copy.will = this.will;
		    copy.reflex = this.reflex;
		    copy.damageTypeResistanceValueAcid = this.damageTypeResistanceValueAcid;
		    copy.damageTypeResistanceValueNormal = this.damageTypeResistanceValueNormal;
		    copy.damageTypeResistanceValueCold = this.damageTypeResistanceValueCold;
		    copy.damageTypeResistanceValueElectricity = this.damageTypeResistanceValueElectricity;
		    copy.damageTypeResistanceValueFire = this.damageTypeResistanceValueFire;
		    copy.damageTypeResistanceValueMagic = this.damageTypeResistanceValueMagic;
		    copy.damageTypeResistanceValuePoison = this.damageTypeResistanceValuePoison;
		    copy.cr_typeOfDamage = this.cr_typeOfDamage;
		    copy.onScoringHit = this.onScoringHit;
		    copy.onScoringHitParms = this.onScoringHitParms;
            copy.onScoringHitCastSpellTag = this.onScoringHitCastSpellTag;
            copy.onDeathIBScript = this.onDeathIBScript;
            copy.onDeathIBScriptParms = this.onDeathIBScriptParms;
		    copy.cr_effectsList = new List<Effect>();
            copy.newCoor = this.newCoor;
            copy.glideAdderX = this.glideAdderX;
            copy.glideAdderY = this.glideAdderY;
            copy.hpLastTurn = this.hpLastTurn;
            copy.labelForCastAction = this.labelForCastAction;
            copy.labelForSpellsButtonInCombat = this.labelForSpellsButtonInCombat;
            copy.creatureSize = this.creatureSize;
            copy.spRegenerationPerRound = this.spRegenerationPerRound;
            copy.hpRegenerationPerRound = this.hpRegenerationPerRound;

            copy.knownSpellsTags = new List<string>();
            foreach (string s in this.knownSpellsTags)
            {
                copy.knownSpellsTags.Add(s);
            }

            copy.castChances = new List<LocalInt>();
            foreach (LocalInt l in this.castChances)
            {
                LocalInt Lint = new LocalInt();
                Lint.Key = l.Key;
                Lint.Value = l.Value;
                copy.castChances.Add(Lint);
                //copy.castChances.Add(s);
            }

            /*
            bool useCastChances = true;
            foreach (LocalInt l in this.castChances)

            if (copy.castChances.Count < 1)
            {
                foreach (string s in copy.knownSpellsTags)
                {
                    copy.castChances.Add(100);
                }
            }
            */

            //public List<Coordinate> tokenCoveredSquares = new List<Coordinate>();
            copy.tokenCoveredSquares = new List<Coordinate>();
            foreach (Coordinate c in this.tokenCoveredSquares)
            {
                copy.tokenCoveredSquares.Add(c);
            }

            copy.CreatureLocalInts = new List<LocalInt>();
            foreach (LocalInt l in this.CreatureLocalInts)
            {
                LocalInt Lint = new LocalInt();
                Lint.Key = l.Key;
                Lint.Value = l.Value;
                copy.CreatureLocalInts.Add(Lint);
            }
        
            copy.CreatureLocalStrings = new List<LocalString>();
            foreach (LocalString l in this.CreatureLocalStrings)
            {
                LocalString Lstr = new LocalString();
                Lstr.Key = l.Key;
                Lstr.Value = l.Value;
                copy.CreatureLocalStrings.Add(Lstr);
            }
		    return copy;
	    }

        public bool isHeld()
        {
            foreach (Effect ef in this.cr_effectsList)
            {
                if (ef.statusType.Equals("Held"))
                {
                    return true;
                }
            }
            /*
            if (this.cr_status == "Held")
            {
                return true;
            }
            */

            return false;
        }
        public bool isImmobile()
        {
            foreach (Effect ef in this.cr_effectsList)
            {
                if (ef.statusType.Equals("Immobile"))
                {
                    return true;
                }
            }
            return false;
        }
        public bool isInvisible()
        {
            foreach (Effect ef in this.cr_effectsList)
            {
                if (ef.statusType.Equals("Invisible"))
                {
                    return true;
                }
            }
            return false;
        }
        public bool isSilenced()
        {
            foreach (Effect ef in this.cr_effectsList)
            {
                if (ef.statusType.Equals("Silenced"))
                {
                    return true;
                }
            }
            return false;
        }
        //*****************************************************


        public int getDamageTypeResistanceValueMagic()
         {  
             int modifier = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
             {  
                 if (ef.isStackableEffect)  
                 {  
                     modifier += ef.modifyDamageTypeResistanceMagic;  
                 }  
                 else  
                 {  
                     if ((ef.modifyDamageTypeResistanceMagic != 0) && (ef.modifyDamageTypeResistanceMagic > highestNonStackable))  
                     {  
                         highestNonStackable = ef.modifyDamageTypeResistanceMagic;  
                     }  
                 }  
             }  
             if (highestNonStackable > -99) { modifier = highestNonStackable; }  
   
             int returnValue = this.damageTypeResistanceValueMagic + modifier;  
   
             if (returnValue > 100) //check is necessary so creature could never receive negative damage (heal) due to greater than 100 resistance.  
             {  
                 returnValue = 100;  
             }  
             return returnValue;  
         }  
         public int getDamageTypeResistanceValueAcid()
         {  
             int modifier = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
             {
                    if (ef.isStackableEffect)
                       {
         modifier += ef.modifyDamageTypeResistanceAcid;
                        }
                   else  
                 {
                            if ((ef.modifyDamageTypeResistanceAcid != 0) && (ef.modifyDamageTypeResistanceAcid > highestNonStackable))
                                {
             highestNonStackable = ef.modifyDamageTypeResistanceAcid;
                                }
                        }
                }  
             if (highestNonStackable > -99) { modifier = highestNonStackable; }  
   
             int returnValue = this.damageTypeResistanceValueAcid + modifier;  
   
             if (returnValue > 100) //check is necessary so creature could never receive negative damage (heal) due to greater than 100 resistance.  
             {
     returnValue = 100;
                }  
             return returnValue;  
         }  
         public int getDamageTypeResistanceValueNormal()
         {  
             int modifier = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
             {
                    if (ef.isStackableEffect)
                        {
         modifier += ef.modifyDamageTypeResistanceNormal;
                        }
                    else  
                 {
                            if ((ef.modifyDamageTypeResistanceNormal != 0) && (ef.modifyDamageTypeResistanceNormal > highestNonStackable))
                                {
             highestNonStackable = ef.modifyDamageTypeResistanceNormal;
                                }
                        }
                }  
             if (highestNonStackable > -99) { modifier = highestNonStackable; }  
   
             int returnValue = this.damageTypeResistanceValueNormal + modifier;  
   
             if (returnValue > 100) //check is necessary so creature could never receive negative damage (heal) due to greater than 100 resistance.  
             {
     returnValue = 100;
                }  
             return returnValue;  
         }  
         public int getDamageTypeResistanceValueCold()
         {  
             int modifier = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
             {
                    if (ef.isStackableEffect)
                        {
         modifier += ef.modifyDamageTypeResistanceCold;
                        }
                    else  
                 {
                            if ((ef.modifyDamageTypeResistanceCold != 0) && (ef.modifyDamageTypeResistanceCold > highestNonStackable))
                                {
             highestNonStackable = ef.modifyDamageTypeResistanceCold;
                                }
                        }
                }  
             if (highestNonStackable > -99) { modifier = highestNonStackable; }  
   
             int returnValue = this.damageTypeResistanceValueCold + modifier;  
   
             if (returnValue > 100) //check is necessary so creature could never receive negative damage (heal) due to greater than 100 resistance.  
             {
     returnValue = 100;
                }  
             return returnValue;  
         }  
         public int getDamageTypeResistanceValueElectricity()
         {  
             int modifier = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
             {
                    if (ef.isStackableEffect)
                        {
         modifier += ef.modifyDamageTypeResistanceElectricity;
                        }
                    else  
                 {
                            if ((ef.modifyDamageTypeResistanceElectricity != 0) && (ef.modifyDamageTypeResistanceElectricity > highestNonStackable))
                                {
             highestNonStackable = ef.modifyDamageTypeResistanceElectricity;
                                }
                        }
                }  
             if (highestNonStackable > -99) { modifier = highestNonStackable; }  
   
             int returnValue = this.damageTypeResistanceValueElectricity + modifier;  
   
             if (returnValue > 100) //check is necessary so creature could never receive negative damage (heal) due to greater than 100 resistance.  
             {
     returnValue = 100;
                }  
             return returnValue;  
         }  
         public int getDamageTypeResistanceValueFire()
         {  
             int modifier = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
             {
                    if (ef.isStackableEffect)
                        {
         modifier += ef.modifyDamageTypeResistanceFire;
                        }
                    else  
                 {
                            if ((ef.modifyDamageTypeResistanceFire != 0) && (ef.modifyDamageTypeResistanceFire > highestNonStackable))
                                {
             highestNonStackable = ef.modifyDamageTypeResistanceFire;
                                }
                       }
                }  
             if (highestNonStackable > -99) { modifier = highestNonStackable; }  
   
             int returnValue = this.damageTypeResistanceValueFire + modifier;  
   
             if (returnValue > 100) //check is necessary so creature could never receive negative damage (heal) due to greater than 100 resistance.  
             {
     returnValue = 100;
                }  
             return returnValue;  
         }  
         public int getDamageTypeResistanceValuePoison()
         {  
             int modifier = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
             {
                    if (ef.isStackableEffect)
                        {
         modifier += ef.modifyDamageTypeResistancePoison;
                       }
                    else  
                 {
                           if ((ef.modifyDamageTypeResistancePoison != 0) && (ef.modifyDamageTypeResistancePoison > highestNonStackable))
                                {
             highestNonStackable = ef.modifyDamageTypeResistancePoison;
                                }
                        }
                }  
             if (highestNonStackable > -99) { modifier = highestNonStackable; }  
   
             int returnValue = this.damageTypeResistanceValuePoison + modifier;  
   
             if (returnValue > 100) //check is necessary so creature could never receive negative damage (heal) due to greater than 100 resistance.  
             {
     returnValue = 100;
                }  
             return returnValue;  
         }  
         public int getMoveDistance()
         {  
             int moveBonuses = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
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
             if (highestNonStackable > -99) { moveBonuses = highestNonStackable; }  
   
             int moveDist = this.moveDistance + moveBonuses;  
             if (moveDist < 0) { moveDist = 0; }  
             return moveDist;  
         }  
         public int getAc()
         {  
             int adder = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
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
             if (highestNonStackable > -99) { adder = highestNonStackable; }  
             int ac = this.AC + adder;  
             return ac;  
         }  
         public int getAttackBonus()
         {  
             int adder = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
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
             if (highestNonStackable > -99) { adder = highestNonStackable; }  
             int att = this.cr_att + adder;  
             return att;  
         } 
        
        public int getMaxHPModifier()
        {
            int adder = 0;
            int highestNonStackable = -99;
            foreach (Effect ef in this.cr_effectsList)
            {
                if (ef.isStackableEffect)
                {
                    adder += ef.modifyHpMax;
                }
                else
                {
                    if ((ef.modifyHpMax != 0) && (ef.modifyHpMax > highestNonStackable))
                    {
                        highestNonStackable = ef.modifyHpMax;
                    }
                }
            }
            if (highestNonStackable > -99) { adder = highestNonStackable; }
            return adder;
        } 

         public int getNumberOfAttacks()
         {  
             if (this.cr_category.Equals("Melee"))  
             {
     int numOfAdditionalPositiveMeleeAttacks = 0;
     int numOfAdditionalPositiveStackableMeleeAttacks = 0;
     int numOfAdditionalNegativeMeleeAttacks = 0;
     int numOfAdditionalNegativeStackableMeleeAttacks = 0;
                    //go through each effect and see if has a buff type like rapidshot, use largest, not cumulative  
                    foreach (Effect ef in this.cr_effectsList)
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
     int numAtt = this.cr_numberOfAttacks + numOfAdditionalAttacks;
                    if (numAtt < 0)
                        {
                            return 0;
                        }
                    return numAtt;
                }  
             else //Ranged attacks  
             {
     int numOfAdditionalPositiveRangedAttacks = 0;
     int numOfAdditionalPositiveStackableRangedAttacks = 0;
     int numOfAdditionalNegativeRangedAttacks = 0;
     int numOfAdditionalNegativeStackableRangedAttacks = 0;
                    //go through each effect and see if has a buff type like rapidshot, use largest, not cumulative  
                    foreach (Effect ef in this.cr_effectsList)
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
     int numAtt = this.cr_numberOfAttacks + numOfAdditionalAttacks;
                    if (numAtt < 0)
                        {
                            return 0;
                        }
                    return numAtt;
                }  
         }  
         public int getFortitude()
         {  
             int savBonuses = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
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
             if (highestNonStackable > -99) { savBonuses = highestNonStackable; }  
             int fort = this.fortitude + savBonuses;  
             return fort;  
         }  
         public int getWill()
         {  
             int savBonuses = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
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
             if (highestNonStackable > -99) { savBonuses = highestNonStackable; }  
             int wil = this.will + savBonuses;  
             return wil;  
         }  
         public int getReflex()
         {  
             int savBonuses = 0;  
             int highestNonStackable = -99;  
             foreach (Effect ef in this.cr_effectsList)  
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
             if (highestNonStackable > -99) { savBonuses = highestNonStackable; }  
             int reflx = this.reflex + savBonuses;  
             return reflx;  
         }  
   
        //*****************************************************
        public Effect getEffectByTag(string tag)
        {
            foreach (Effect ef in this.cr_effectsList)
            {
                if (ef.tag.Equals(tag)) return ef;
            }
            return null;
        }
	    public bool IsInEffectList(string effectTag)
        {
            foreach (Effect ef in this.cr_effectsList)
            {
                if (ef.tag.Equals(effectTag))
                {
                    return true;
                }
            }
            return false;
        }
        public void AddEffect(Effect ef)
        {
            this.cr_effectsList.Add(ef);
        }
        public void AddEffectByObject(Effect effect, int classLevel)
        {
            if (!effect.isPermanent)
            {
                Effect ef = effect.DeepCopy();
                ef.classLevelOfSender = classLevel;
                //ef.startingTimeInUnits = startTime; //mod.WorldTime;
                //stackable effect and duration (just add effect to list)
                if (ef.isStackableEffect)
                {
                    //add to the list
                    AddEffect(ef);
                }
                //stackable duration (add to list if not there, if there add to duration)
                else if ((!ef.isStackableEffect) && (ef.isStackableDuration))
                {
                    if (!IsInEffectList(ef.tag)) //Not in list so add to list
                    {
                        AddEffect(ef);
                    }
                    else //is in list so add durations together
                    {
                        Effect e = this.getEffectByTag(ef.tag);
                        if (!e.isPermanent)
                        {
                            e.durationInUnits += ef.durationInUnits;
                            if (classLevel > e.classLevelOfSender)
                            {
                                e.classLevelOfSender = classLevel;
                            }
                        }
                    }
                }
                //none stackable (add to list if not there)
                else if ((!ef.isStackableEffect) && (!ef.isStackableDuration))
                {
                    if (!IsInEffectList(ef.tag)) //Not in list so add to list
                    {
                        AddEffect(ef);
                    }
                    else //is in list so reset duration
                    {
                        Effect e = this.getEffectByTag(ef.tag);
                        if (!e.isPermanent)
                        {
                            e.durationInUnits = ef.durationInUnits;
                            if (classLevel > e.classLevelOfSender)
                            {
                                e.classLevelOfSender = classLevel;
                            }
                        }
                        //e.startingTimeInUnits = startTime;
                        //e.currentDurationInUnits = 0;
                    }
                }
            }
        }
    }  
}
