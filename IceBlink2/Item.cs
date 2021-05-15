using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
//using IceBlink;
using System.Drawing.Design;
//using System.Design;
using System.ComponentModel.Design;
using Newtonsoft.Json;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class Item 
    {
	    public string name = "none"; //item name  
        public bool isRation = false;
        public bool isLightSource = false;
        public string itemImage = "blank";
        [JsonIgnore]
	    public Bitmap token;
        public string ArmorWeightType = "Light"; //Light, Medium, Heavy 
        public string tag = "none"; //item unique tag name    
        public string resref = "none"; //item unique tag name    
        public string desc = ""; //item short detailed description
        public string descFull = ""; //item full detailed description
        public string useableInSituation = "Always"; //InCombat, OutOfCombat, Always, Passive
        public string projectileSpriteFilename = "none"; //sprite to use for projectiles
        public string spriteEndingFilename = "none"; //sprite to use for end effect of projectiles
        public string itemOnUseSound = "none"; //Filename of sound to play when the item is used (no extension)
        public string itemEndSound = "none"; //Filename of sound to play upon end use of item like explosion from thrown grenade (no extension)
        public string category = "Armor"; //catergory type (Armor, Ranged, Melee, General, Ring, Shield, Ammo, Gloves)
	    public bool plotItem = false;
	    public int value = 0; //cost in credits
	    public int quantity = 1; //useful for stacking and ammo
	    public int groupSizeForSellingStackableItems = 1;
	    public int charges = 0; //useful for items like wand of mage bolts
        public string ammoType = "none"; //typically arrow, stone, bolt 
	    public bool twoHanded = false; //true if item requires the use of two hands
        public bool isLightWeapon = false; //true if it is a light weapon
        public bool isFinesseWeapon = false; //if true, use dex modifier for attack roll, else use strength modifier
        public int threatRange = 20;
        public int criticalMultiplier = 2;
        public bool canNotBeUnequipped = false;
        public bool isStackable = false;
        public bool automaticallyHitsTarget = false; //does not require a successful to hit roll, always hits target (ex. mage bolt wand)
        public int attackBonus = 0; //attack bonus
	    public int attackRange = 1; //attack range
	    public int AreaOfEffect = 0; //AoE radius
        public AreaOfEffectShape aoeShape = AreaOfEffectShape.Circle;
        public string spellTag = "none";
        public int damageNumDice = 1; //number of dice to roll for damage
	    public int damageDie = 2; //type of dice to roll for damage
	    public int damageAdder = 0; //the adder like 2d4+1 where "1" is the adder
	    public int armorBonus = 0; //armor bonus
	    public int maxDexBonus = 99; //maximum Dexterity bonus allowed with this armor
        public int modifierMaxHP = 0;
        public int modifierMaxSP = 0;
        public int requiredSTR = 0;
        public int requiredDEX = 0;
        public int requiredCON = 0;
        public int requiredINT = 0;
        public int requiredWIS = 0;
        public int requiredCHA = 0;
        public int attributeBonusModifierStr = 0;
	    public int attributeBonusModifierDex = 0;
	    public int attributeBonusModifierInt = 0;
	    public int attributeBonusModifierCha = 0;
        public int attributeBonusModifierCon = 0;
        public int attributeBonusModifierWis = 0;
        public int attributeBonusModifierLuk = 0;

        public int maxStrengthBonusAllowedForWeapon = 100;
        public int spCostPerAttack = 0;
        public int hpCostPerAttack = 0;

        public int savingThrowModifierReflex = 0;
	    public int savingThrowModifierFortitude = 0;
	    public int savingThrowModifierWill = 0;
        public int MovementPointModifier = 0;
        public int spRegenPerRoundInCombat = 0;
        public int hpRegenPerRoundInCombat = 0;
        public int minutesPerSpRegenOutsideCombat = 0;
        public int minutesPerHpRegenOutsideCombat = 0;
        public string onScoringHit = "none";
        public string onScoringHitParms = "";
        public string onUseItem = "none";
        public string onWhileEquipped = "none";
        public string onUseItemIBScript = "none";
        public string onUseItemIBScriptParms = "";
        public bool destroyItemAfterOnUseItemIBScript = false;
        public bool destroyItemAfterOnUseItemScript = true;
        public string onScoringHitCastSpellTag = "none";
        public bool onScoringHitCastOnSelf = false;
        public string onUseItemCastSpellTag = "none";
        public bool destroyItemAfterOnUseItemCastSpell = false;
        public int levelOfItemForCastSpell = 1;
        public bool usePlayerClassLevelForOnUseItemCastSpell = false;
        public int damageTypeResistanceValueAcid = 0;
	    public int damageTypeResistanceValueCold = 0;
	    public int damageTypeResistanceValueNormal = 0;
	    public int damageTypeResistanceValueElectricity = 0;
	    public int damageTypeResistanceValueFire = 0;
	    public int damageTypeResistanceValueMagic = 0;
	    public int damageTypeResistanceValuePoison = 0;
        public int requiredLevel = 0;
        public string typeOfDamage = "Normal"; //Normal,Acid,Cold,Electricity,Fire,Magic,Poison
        public List<LocalImmunityString> entriesForPcTags = new List<LocalImmunityString>();

        public string requiredTrait = "none";
        public string requiredRace = "none";
        public string restrictedRace = "none";

        public bool canNotBeChangedInCombat = false;

        public bool endTurnAfterEquipping = true;

        public int hpRegenTimer = 0;
        public int spRegenTimer = 0;

        public int additionalAttacks = 0;

        public string tagOfTraitInfluenced = "none";
        public int traitSkillRollModifier = 0;

        //Not yet implemented
        public string labelForCastAction = "none";
        public string labelForSpellsButtonInCombat = "SPELL";

        public bool onlyUseableWhenEquipped = false;

        public Item()
	    {
		
	    }
	    public Item DeepCopy()
	    {
		    Item copy = new Item();
		    copy.name = this.name;
            copy.onlyUseableWhenEquipped = this.onlyUseableWhenEquipped;
            copy.endTurnAfterEquipping = this.endTurnAfterEquipping;
            copy.isRation = this.isRation;
            copy.isLightSource = this.isLightSource;
            copy.ArmorWeightType = this.ArmorWeightType;
		    copy.itemImage = this.itemImage;
		    copy.tag = this.tag;
		    copy.resref = this.resref;
		    copy.desc = this.desc;
		    copy.descFull = this.descFull;
		    copy.useableInSituation = this.useableInSituation;
		    copy.projectileSpriteFilename = this.projectileSpriteFilename;	
		    copy.spriteEndingFilename = this.spriteEndingFilename;
		    copy.itemOnUseSound = this.itemOnUseSound;
		    copy.category = this.category;
		    copy.plotItem = this.plotItem;
		    copy.value = this.value;
            copy.requiredLevel = this.requiredLevel;
            copy.requiredSTR = this.requiredSTR;
            copy.requiredDEX = this.requiredDEX;
            copy.requiredCON = this.requiredCON;
            copy.requiredINT = this.requiredINT;
            copy.requiredWIS = this.requiredWIS;
            copy.requiredCHA = this.requiredCHA;
            copy.requiredTrait = this.requiredTrait;
            copy.requiredRace = this.requiredRace;
            copy.restrictedRace = this.restrictedRace;
            copy.maxStrengthBonusAllowedForWeapon = this.maxStrengthBonusAllowedForWeapon;
            copy.quantity = this.quantity;
		    copy.groupSizeForSellingStackableItems = this.groupSizeForSellingStackableItems;
		    copy.charges = this.charges;
		    copy.ammoType = this.ammoType;
		    copy.twoHanded = this.twoHanded;
            copy.isLightWeapon = this.isLightWeapon;
            copy.isFinesseWeapon = this.isFinesseWeapon;
            copy.threatRange = this.threatRange;
            copy.criticalMultiplier = this.criticalMultiplier;
            copy.canNotBeUnequipped = this.canNotBeUnequipped; 
		    copy.isStackable = this.isStackable;
            copy.automaticallyHitsTarget = this.automaticallyHitsTarget;
		    copy.attackBonus = this.attackBonus;
		    copy.attackRange = this.attackRange;
		    copy.AreaOfEffect = this.AreaOfEffect;
            copy.aoeShape = this.aoeShape;
            copy.canNotBeChangedInCombat = this.canNotBeChangedInCombat;
            copy.spellTag = this.spellTag;
            copy.damageNumDice = this.damageNumDice;
		    copy.damageDie = this.damageDie;
		    copy.damageAdder = this.damageAdder;
            copy.additionalAttacks = this.additionalAttacks;
            copy.armorBonus = this.armorBonus;
		    copy.maxDexBonus = this.maxDexBonus;
            copy.modifierMaxHP = this.modifierMaxHP;
            copy.modifierMaxSP = this.modifierMaxSP;
            copy.attributeBonusModifierStr = this.attributeBonusModifierStr;
		    copy.attributeBonusModifierDex = this.attributeBonusModifierDex;
		    copy.attributeBonusModifierInt = this.attributeBonusModifierInt;
		    copy.attributeBonusModifierCha = this.attributeBonusModifierCha;
            copy.attributeBonusModifierCon = this.attributeBonusModifierCon;
            copy.attributeBonusModifierWis = this.attributeBonusModifierWis;
            copy.attributeBonusModifierLuk = this.attributeBonusModifierLuk;
		    copy.savingThrowModifierReflex = this.savingThrowModifierReflex;
		    copy.savingThrowModifierFortitude = this.savingThrowModifierFortitude;
		    copy.savingThrowModifierWill = this.savingThrowModifierWill;
            copy.MovementPointModifier = this.MovementPointModifier;
            copy.spRegenPerRoundInCombat = this.spRegenPerRoundInCombat;
            copy.hpRegenPerRoundInCombat = this.hpRegenPerRoundInCombat;
            copy.minutesPerSpRegenOutsideCombat = this.minutesPerSpRegenOutsideCombat;
            copy.minutesPerHpRegenOutsideCombat = this.minutesPerHpRegenOutsideCombat;
		    copy.onScoringHit = this.onScoringHit;
		    copy.onScoringHitParms = this.onScoringHitParms;
            copy.tagOfTraitInfluenced = this.tagOfTraitInfluenced;
            copy.traitSkillRollModifier = this.traitSkillRollModifier;
            //test
            copy.onUseItem = this.onUseItem;
		    copy.onWhileEquipped = this.onWhileEquipped;
		    copy.onUseItemIBScript = this.onUseItemIBScript;
            copy.onUseItemIBScriptParms = this.onUseItemIBScriptParms;
            copy.destroyItemAfterOnUseItemIBScript = this.destroyItemAfterOnUseItemIBScript;
            copy.destroyItemAfterOnUseItemScript = this.destroyItemAfterOnUseItemScript;
            copy.onScoringHitCastSpellTag = this.onScoringHitCastSpellTag;
            copy.onScoringHitCastOnSelf = this.onScoringHitCastOnSelf;
            copy.onUseItemCastSpellTag = this.onUseItemCastSpellTag;
            copy.destroyItemAfterOnUseItemCastSpell = this.destroyItemAfterOnUseItemCastSpell;
            copy.levelOfItemForCastSpell = this.levelOfItemForCastSpell;
            copy.usePlayerClassLevelForOnUseItemCastSpell = this.usePlayerClassLevelForOnUseItemCastSpell;
            copy.damageTypeResistanceValueAcid = this.damageTypeResistanceValueAcid;
		    copy.damageTypeResistanceValueNormal = this.damageTypeResistanceValueNormal;
		    copy.damageTypeResistanceValueCold = this.damageTypeResistanceValueCold;
		    copy.damageTypeResistanceValueElectricity = this.damageTypeResistanceValueElectricity;
		    copy.damageTypeResistanceValueFire = this.damageTypeResistanceValueFire;
		    copy.damageTypeResistanceValueMagic = this.damageTypeResistanceValueMagic;
		    copy.damageTypeResistanceValuePoison = this.damageTypeResistanceValuePoison;
		    copy.typeOfDamage = this.typeOfDamage;
            copy.labelForCastAction = this.labelForCastAction;
            copy.labelForSpellsButtonInCombat = this.labelForSpellsButtonInCombat;
            copy.entriesForPcTags = new List<LocalImmunityString>();
            foreach (LocalImmunityString s in this.entriesForPcTags)
            {
                copy.entriesForPcTags.Add(s);
            }
            copy.hpRegenTimer = this.hpRegenTimer;
            copy.spRegenTimer = this.spRegenTimer;
            return copy;
	    }
    }
}
