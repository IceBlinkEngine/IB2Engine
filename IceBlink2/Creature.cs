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
	    public string cr_tokenFilename = "blank.png";
        [JsonIgnore]
	    public Bitmap token;
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
        //public string onDeathLogicTree = "none";
        //public string onDeathParms = "";
        public string onDeathIBScript = "none";
        public string onDeathIBScriptParms = ""; 
	    public List<string> knownSpellsTags = new List<string>();
	    public List<Effect> cr_effectsList = new List<Effect>();
	    public List<LocalInt> CreatureLocalInts = new List<LocalInt>();
	    public List<LocalString> CreatureLocalStrings = new List<LocalString>();
	    
	    public Creature()
	    {
		
	    }
	
	    public Creature DeepCopy()
	    {
		    Creature copy = new Creature();
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
            //copy.onDeathLogicTree = this.onDeathLogicTree;
            //copy.onDeathParms = this.onDeathParms;
            copy.onDeathIBScript = this.onDeathIBScript;
            copy.onDeathIBScriptParms = this.onDeathIBScriptParms;
		    copy.cr_effectsList = new List<Effect>();
		
		    copy.knownSpellsTags = new List<string>();
            foreach (string s in this.knownSpellsTags)
            {
                copy.knownSpellsTags.Add(s);
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
	    public void AddEffectByObject(Effect effect, int startTime)
        {
            Effect ef = effect.DeepCopy();
            ef.startingTimeInUnits = startTime; //mod.WorldTime;
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
                    e.durationInUnits += ef.durationInUnits;
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
                    e.startingTimeInUnits = startTime;
                    e.currentDurationInUnits = 0;
                }
            }
        }	
    }  
}
