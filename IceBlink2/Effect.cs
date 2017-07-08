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
        public int modifyShopBuyBackPrice = 0;  
        public int modifyShopSellPrice = 0;

        public string name = "newEffect";
	    public string tag = "newEffectTag";
	    public string tagOfSender = "senderTag";
        public int classLevelOfSender = 0;
        public string description = "";
	    public string spriteFilename = "held";
	    public int durationInUnits = 0;
        public int currentDurationInUnits = 0;
	    public int startingTimeInUnits = 0;
	    public int babModifier = 0; //for Creatures modifies cr_att, for PCs modifies baseAttBonus
        public int babModifierForRangedAttack = 0;
        public int babModifierForMeleeAttack = 0;
        public int damageModifierForMeleeAttack = 0;  
        public int damageModifierForRangedAttack = 0;

        public int acModifier = 0;
        public bool isStackableEffect = false;
	    public bool isStackableDuration = false;
	    public bool usedForUpdateStats = false;
	    public string effectLetter = "A";
	    public string effectLetterColor = "White";
	    public string effectScript = "none";
        public string saveCheckType = "none"; //none, reflex, will, fortitude
        public int saveCheckDC = 10;
        public int combatLocX = 0; //used in combat for effects on squares
        public int combatLocY = 0; //used in combat for effects on squares
        public List<LocalImmunityString> affectOnlyList = new List<LocalImmunityString>();
        public List<LocalImmunityString> affectNeverList = new List<LocalImmunityString>();
        public bool endEffectWhenCarrierTakesDamage = false;
        public bool isPermanent = false;

        //the two below yet need implementation in engine code
        public bool repeatTerminalSaveEachRound = false;
        public bool saveOnlyHalvesDamage = false;

        //* = marks properties that only work on PCs, not Creatures
        //The below modifiers will be cumulative over each round of the Effects duration if usedForUpdateStats = false

        //if you want the effect to be cumulative such as damage per round due to poison, set usedForUpdateStats = false
        //if you want the effect to NOT be cumulative such as AC bonus, set usedForUpdateStats = true

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
        public bool doBuff = false;
        public bool doDeBuff = false;
        public string statusType = "none"; //none, Held, Immobile, Invisible, Silenced, etc.
        public int modifyFortitude = 0;
        public int modifyWill = 0;
        public int modifyReflex = 0;
        //For PC only
        public int modifyStr = 0;
        public int modifyDex = 0;
        public int modifyInt = 0;
        public int modifyCha = 0;
        public int modifyCon = 0;
        public int modifyWis = 0;
        public int modifyLuk = 0;
        //end PC only
        public int modifyMoveDistance = 0;
        public int modifyHpMax = 0;
        public int modifySpMax = 0;
        public int modifySp = 0;
        public int modifyHpInCombat = 0;  
        public int modifySpInCombat = 0;

        public int modifyDamageTypeResistanceAcid = 0;
        public int modifyDamageTypeResistanceCold = 0;
        public int modifyDamageTypeResistanceNormal = 0;
        public int modifyDamageTypeResistanceElectricity = 0;
        public int modifyDamageTypeResistanceFire = 0;
        public int modifyDamageTypeResistanceMagic = 0;
        public int modifyDamageTypeResistancePoison = 0;
        public int modifyNumberOfMeleeAttacks = 0;
        public int modifyNumberOfRangedAttacks = 0;

        public int modifyNumberOfEnemiesAttackedOnCleave = 0; //(melee only) cleave attacks are only made if previous attacked enemy goes down.  
        public int modifyNumberOfEnemiesAttackedOnSweepAttack = 0; //(melee only) sweep attack simultaneously attacks multiple enemies in range  
        public bool useDexterityForMeleeAttackModifierIfGreaterThanStrength = false;  
        public bool useDexterityForMeleeDamageModifierIfGreaterThanStrength = false;  
        public bool negateAttackPenaltyForAdjacentEnemyWithRangedAttack = false;  
        public bool useEvasion = false; //

        public bool allowCastingWithoutTriggeringAoO = false;
        public bool allowCastingWithoutRiskOfInterruption = false;

        public List<LocalImmunityString> traitWorksOnlyWhen = new List<LocalImmunityString>();
        public List<LocalImmunityString> traitWorksNeverWhen = new List<LocalImmunityString>();

        public Effect()
	    {
		
	    }
	    public Effect DeepCopy()
	    {
		    Effect copy = new Effect();
            copy.allowCastingWithoutRiskOfInterruption = this.allowCastingWithoutRiskOfInterruption;
            copy.allowCastingWithoutTriggeringAoO = this.allowCastingWithoutTriggeringAoO;
		    copy.name = this.name;
		    copy.tag = this.tag;
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
