using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using Newtonsoft.Json;

namespace IceBlink2
{
    public class Effect 
    {

        public int onSquarePersistenceBonus = 0;
        public bool triggeredEachStepToo = false;

        public int modifyShopBuyBackPrice = 0;//is in  
        public int modifyShopSellPrice = 0;//is in

        public string name = "newEffect";
	    public string tag = "newEffectTag";
	    public string tagOfSender = "senderTag";//not used so far (could be used eg for rmeocing effects immediately once teh effect cretaor dies) 
        public int classLevelOfSender = 0;
        public string description = "";
	    public string spriteFilename = "held";
        public string squareIndicatorFilename = "fx_webbed";
        public int durationInUnits = 0;//this is in seconds
        public int durationOnSquareInUnits = 0;//this is in seconds; this is used to determine hwo the effect can linger on a square
        public int currentDurationInUnits = 0;//likely redundant, used as a check against zero tohugh!
	    public int startingTimeInUnits = 0;//is in for player and creature

        public int babModifier = 0; //for Creatures modifies cr_att, for PCs modifies baseAttBonus
        public int babModifierForRangedAttack = 0;//is in for player only
        public int babModifierForMeleeAttack = 0;//is in for player only
        public int damageModifierForMeleeAttack = 0;//is in for player only  
        public int damageModifierForRangedAttack = 0;//is in for player only
        public int acModifier = 0;////is in for player and creature

        public bool isStackableEffect = false;//is in for player and creature
        public bool isStackableDuration = false;//is in for player and creature

        //used for update stats is not relevant in new system
        public bool usedForUpdateStats = false;

	    public string effectLetter = "A";//not implemented yet
	    public string effectLetterColor = "White";//not implemented yet
	    public string effectScript = "efGeneric";//is in (for special or old spells)

        public string saveCheckType = "none"; //none, reflex, will, fortitude
        public int saveCheckDC = 10;
        public int combatLocX = 0; //used in combat for effects on squares
        public int combatLocY = 0; //used in combat for effects on squares

        public List<LocalImmunityString> affectOnlyList = new List<LocalImmunityString>();//works for creature (and pc)
        public List<LocalImmunityString> affectNeverList = new List<LocalImmunityString>();//works for creature (and pc)
        public bool endEffectWhenCarrierTakesDamage = false;//works for creature (and pc)

        public bool isPermanent = false;//for pc only

        public bool repeatTerminalSaveEachRound = false;//works for creature (and pc)
        public bool saveOnlyHalvesDamage = false;//works for creature (and pc)

        //DAMAGE (hp)
        public bool doDamage = false;
        public string damType = "Normal"; //Normal,Acid,Cold,Electricity,Fire,Magic,Poison
        //(for reference) Attack: AdB+C for every D levels after level E up to F levels total
        public int damNumOfDice = 0; //(A)how many dice to roll
        public int damDie = 0; //(B)type of die to roll such as 4 sided or 10 sided, etc.
        public int damAdder = 0; //(C)integer adder to total damage such as the "1" in 2d4+1
        public int damAttacksEveryNLevels = 0; //(D)
        public int damAttacksAfterLevelN = 0; //(E)
        public int damAttacksUpToNLevelsTotal = 0; //(F)
        //(for reference) NumOfAttacks: A of these attacks for every B levels after level C up to D attacks total
        public int damNumberOfAttacks = 0; //(A)
        public int damNumberOfAttacksForEveryNLevels = 0; //(B)
        public int damNumberOfAttacksAfterLevelN = 0; //(C)
        public int damNumberOfAttacksUpToNAttacksTotal = 0; //(D)

        //HEAL (hp)
        public bool doHeal = false;
        public bool healHP = true; //if true, heals HP. If false, heals SP
        public string healType = "Organic"; //Organic (living things), NonOrganic (robots, constructs)
        //(for reference) HealActions: AdB+C for every D levels after level E up to F levels total
        public int healNumOfDice = 0; //(A)how many dice to roll
        public int healDie = 0; //(B)type of die to roll such as 4 sided or 10 sided, etc.
        public int healAdder = 0; //(C)integer adder to total damage such as the "1" in 2d4+1
        public int healActionsEveryNLevels = 0; //(D)
        public int healActionsAfterLevelN = 0; //(E)
        public int healActionsUpToNLevelsTotal = 0; //(F)

        //BUFF and DEBUFF
        public bool doBuff = false;//should be redundant, use effect duration greater 0 instead
        public bool doDeBuff = false;//should be redundant, use effect duration greater 0 instead
        public string statusType = "none"; //none, Held, Immobile, Invisible, Silenced, etc.
        public int modifyFortitude = 0;//works for creature (and pc)
        public int modifyWill = 0;//works for creature (and pc)
        public int modifyReflex = 0;//works for creature (and pc)


        //For PC only
        public int modifyStr = 0;//not intended for creature
        public int modifyDex = 0;//not intended for creature
        public int modifyInt = 0;//not intended for creature
        public int modifyCha = 0;//not intended for creature
        public int modifyCon = 0;//not intended for creature
        public int modifyWis = 0;//not intended for creature
        public int modifyLuk = 0;//not intended for creature
        //end PC only
        public int modifyMoveDistance = 0;//works for creature (and pc)
        public int modifyHpMax = 0;//implemeneted for cretaure (is in for pc)
        public int modifySpMax = 0;//not used by creature (is in for pc)
        public int modifySp = 0;//not used at all (doHeal does this, when switched to affect sp via healHP = false)
        public int modifyHpInCombat = 0;////implemeneted for creature and pc (it is regeneration)  
        public int modifySpInCombat = 0;//implemeneted for creature and pc (it is regeneration)

        //resitnces work for both pc and creatures
        public int modifyDamageTypeResistanceAcid = 0;
        public int modifyDamageTypeResistanceCold = 0;
        public int modifyDamageTypeResistanceNormal = 0;
        public int modifyDamageTypeResistanceElectricity = 0;
        public int modifyDamageTypeResistanceFire = 0;
        public int modifyDamageTypeResistanceMagic = 0;
        public int modifyDamageTypeResistancePoison = 0;

        public int modifyNumberOfMeleeAttacks = 0;//implemeneted for creature and pc  
        public int modifyNumberOfRangedAttacks = 0;//implemeneted for creature and pc  

        //cleave and sweep is for pc only
        public int modifyNumberOfEnemiesAttackedOnCleave = 0; //(melee only) cleave attacks are only made if previous attacked enemy goes down.  
        public int modifyNumberOfEnemiesAttackedOnSweepAttack = 0; //(melee only) sweep attack simultaneously attacks multiple enemies in range 
        
        //is for pc only, too 
        public bool useDexterityForMeleeAttackModifierIfGreaterThanStrength = false;  
        public bool useDexterityForMeleeDamageModifierIfGreaterThanStrength = false;
        public bool negateAttackPenaltyForAdjacentEnemyWithRangedAttack = false;
        public bool allowCastingWithoutTriggeringAoO = false;
        public bool allowCastingWithoutRiskOfInterruption = false;

        public bool useEvasion = false; //not used at all so far

        public List<LocalImmunityString> traitWorksOnlyWhen = new List<LocalImmunityString>(); // is in; items have entries in entriesForPcTags; these entriesForPcTags are checked against by traitWorksOnlyWhen
        public List<LocalImmunityString> traitWorksNeverWhen = new List<LocalImmunityString>(); //is in; items have entries in entriesForPcTags; these entriesForPcTags are checked against by traitWorksOnlyWhen

        public Effect()
	    {
		
	    }
	    public Effect DeepCopy()
	    {
		    Effect copy = new Effect();
            copy.onSquarePersistenceBonus = this.onSquarePersistenceBonus;
            copy.squareIndicatorFilename = this.squareIndicatorFilename;
            copy.durationOnSquareInUnits = this.durationOnSquareInUnits;
            copy.triggeredEachStepToo = this.triggeredEachStepToo;
            copy.allowCastingWithoutRiskOfInterruption = this.allowCastingWithoutRiskOfInterruption;
            copy.allowCastingWithoutTriggeringAoO = this.allowCastingWithoutTriggeringAoO;
		    copy.name = this.name;
		    copy.tag = this.tag;
            //copy.shortText = this.shortText;
		    copy.tagOfSender = this.tagOfSender;
            copy.classLevelOfSender = this.classLevelOfSender;
		    copy.description = this.description;
		    copy.spriteFilename = this.spriteFilename;	
		    copy.durationInUnits = this.durationInUnits;
            copy.currentDurationInUnits = this.currentDurationInUnits;
		    copy.startingTimeInUnits = this.startingTimeInUnits;
		    copy.babModifier = this.babModifier;
		    copy.acModifier = this.acModifier;
            copy.babModifierForRangedAttack = this.babModifierForRangedAttack;
            copy.babModifierForMeleeAttack = this.babModifierForMeleeAttack;
            copy.damageModifierForMeleeAttack = this.damageModifierForMeleeAttack;
            copy.damageModifierForRangedAttack = this.damageModifierForRangedAttack;

            copy.isStackableEffect = this.isStackableEffect;
		    copy.isStackableDuration = this.isStackableDuration;
		    copy.usedForUpdateStats = this.usedForUpdateStats;
		    copy.effectLetter = this.effectLetter;
		    copy.effectLetterColor = this.effectLetterColor;
		    copy.effectScript = this.effectScript;
            copy.saveCheckType = this.saveCheckType;
            copy.saveCheckDC = this.saveCheckDC;
            copy.combatLocX = this.combatLocX;
            copy.combatLocY = this.combatLocY;
            copy.doBuff = this.doBuff;
            copy.doDamage = this.doDamage;
            copy.doDeBuff = this.doDeBuff;
            copy.doHeal = this.doHeal;
            copy.healHP = this.healHP;
            copy.damType = this.damType;
            copy.damNumOfDice = this.damNumOfDice;
            copy.damDie = this.damDie;
            copy.damAdder = this.damAdder;
            copy.damAttacksEveryNLevels = this.damAttacksEveryNLevels;
            copy.damAttacksAfterLevelN = this.damAttacksAfterLevelN;
            copy.damAttacksUpToNLevelsTotal = this.damAttacksUpToNLevelsTotal;
            copy.damNumberOfAttacks = this.damNumberOfAttacks;
            copy.damNumberOfAttacksForEveryNLevels = this.damNumberOfAttacksForEveryNLevels;
            copy.damNumberOfAttacksAfterLevelN = this.damNumberOfAttacksAfterLevelN;
            copy.damNumberOfAttacksUpToNAttacksTotal = this.damNumberOfAttacksUpToNAttacksTotal;
            copy.healType = this.healType;
            copy.healNumOfDice = this.healNumOfDice;
            copy.healDie = this.healDie;
            copy.healAdder = this.healAdder;
            copy.healActionsEveryNLevels = this.healActionsEveryNLevels;
            copy.healActionsAfterLevelN = this.healActionsAfterLevelN;
            copy.healActionsUpToNLevelsTotal = this.healActionsUpToNLevelsTotal;
            copy.statusType = this.statusType;
            copy.modifyFortitude = this.modifyFortitude;
            copy.modifyWill = this.modifyWill;
            copy.modifyReflex = this.modifyReflex;
            copy.modifyStr = this.modifyStr;
            copy.modifyDex = this.modifyDex;
            copy.modifyInt = this.modifyInt;
            copy.modifyCha = this.modifyCha;
            copy.modifyCon = this.modifyCon;
            copy.modifyWis = this.modifyWis;
            copy.modifyLuk = this.modifyLuk;
            copy.modifyMoveDistance = this.modifyMoveDistance;
            copy.modifyHpMax = this.modifyHpMax;
            copy.modifySpMax = this.modifySpMax;
            copy.modifySp = this.modifySp;
            copy.modifyHpInCombat = this.modifyHpInCombat;
            copy.modifySpInCombat = this.modifySpInCombat;

            copy.modifyDamageTypeResistanceAcid = this.modifyDamageTypeResistanceAcid;
            copy.modifyDamageTypeResistanceCold = this.modifyDamageTypeResistanceCold;
            copy.modifyDamageTypeResistanceNormal = this.modifyDamageTypeResistanceNormal;
            copy.modifyDamageTypeResistanceElectricity = this.modifyDamageTypeResistanceElectricity;
            copy.modifyDamageTypeResistanceFire = this.modifyDamageTypeResistanceFire;
            copy.modifyDamageTypeResistanceMagic = this.modifyDamageTypeResistanceMagic;
            copy.modifyDamageTypeResistancePoison = this.modifyDamageTypeResistancePoison;
            copy.modifyNumberOfMeleeAttacks = this.modifyNumberOfMeleeAttacks;
            copy.modifyNumberOfRangedAttacks = this.modifyNumberOfRangedAttacks;
            copy.modifyNumberOfEnemiesAttackedOnCleave = this.modifyNumberOfEnemiesAttackedOnCleave;
            copy.modifyNumberOfEnemiesAttackedOnSweepAttack = this.modifyNumberOfEnemiesAttackedOnSweepAttack;
            copy.useDexterityForMeleeAttackModifierIfGreaterThanStrength = this.useDexterityForMeleeAttackModifierIfGreaterThanStrength;
            copy.useDexterityForMeleeDamageModifierIfGreaterThanStrength = this.useDexterityForMeleeDamageModifierIfGreaterThanStrength;
            copy.negateAttackPenaltyForAdjacentEnemyWithRangedAttack = this.negateAttackPenaltyForAdjacentEnemyWithRangedAttack;
            copy.useEvasion = this.useEvasion;
            copy.modifyShopBuyBackPrice = this.modifyShopBuyBackPrice;
            copy.modifyShopSellPrice = this.modifyShopSellPrice;


            copy.isPermanent = this.isPermanent;
            copy.endEffectWhenCarrierTakesDamage = this.endEffectWhenCarrierTakesDamage;
            copy.repeatTerminalSaveEachRound = this.repeatTerminalSaveEachRound;
            copy.saveOnlyHalvesDamage = this.saveOnlyHalvesDamage;

            copy.affectOnlyList = new List<LocalImmunityString>();
            foreach (LocalImmunityString s in this.affectOnlyList)
            {
                copy.affectOnlyList.Add(s);
            }

            copy.affectNeverList = new List<LocalImmunityString>();
            foreach (LocalImmunityString s in this.affectNeverList)
            {
                copy.affectNeverList.Add(s);
            }

            copy.traitWorksOnlyWhen = new List<LocalImmunityString>();
            foreach (LocalImmunityString s in this.traitWorksOnlyWhen)
            {
                copy.traitWorksOnlyWhen.Add(s);
            }

            copy.traitWorksNeverWhen = new List<LocalImmunityString>();
            foreach (LocalImmunityString s in this.traitWorksNeverWhen)
            {
                copy.traitWorksNeverWhen.Add(s);
            }


            return copy;
	    }
    }
}
