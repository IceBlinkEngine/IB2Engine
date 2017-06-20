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
    public class Trait 
    {
	    public string name = "newTrait";
	    public string tag = "newTraitTag";
	    public string traitImage = "sp_magebolt";
	    public string description = "";
	    public string prerequisiteTrait = "none";
	    public int skillModifier = 0;
	    public string skillModifierAttribute = "str";
	    public string useableInSituation = "Always"; //InCombat, OutOfCombat, Always, Passive
        public string associatedSpellTag = "none";
        //note: uses turn to activate is defined on spell level
	    public string spriteFilename = "none";
	    public string spriteEndingFilename = "none";
	    public int costSP = 10;	
	    public string traitTargetType = "Enemy"; //Self, Enemy, Friend, PointLocation
	    public string traitEffectType = "Damage"; //Damage, Heal, Buff, Debuff
	    public int aoeRadius = 1;
	    public int range = 2;	
	    public string traitScript = "none";
        public List<EffectTagForDropDownList> traitEffectTagList = new List<EffectTagForDropDownList>();
        public List<LocalImmunityString> traitWorksOnlyWhen = new List<LocalImmunityString>();
        public List<LocalImmunityString> traitWorksNeverWhen = new List<LocalImmunityString>();
        public int requiredStrength = 0;
        public int requiredDexterity = 0;
        public int requiredConstitution = 0;
        public int requiredIntelligence = 0;
        public int requiredWisdom = 0;
        public int requiredCharisma = 0;
        public string traitToReplaceByTag = "none";

        public Trait()
	    {
		
	    }
	
	    public Trait DeepCopy()
	    {
		    Trait copy = new Trait();
            copy.traitToReplaceByTag = this.traitToReplaceByTag;
		    copy.name = this.name;
		    copy.tag = this.tag;
		    copy.traitImage = this.traitImage;
		    copy.description = this.description;
		    copy.prerequisiteTrait = this.prerequisiteTrait;
		    copy.skillModifier = this.skillModifier;
		    copy.skillModifierAttribute = this.skillModifierAttribute;
		    copy.useableInSituation = this.useableInSituation;
		    copy.spriteFilename = this.spriteFilename;	
		    copy.spriteEndingFilename = this.spriteEndingFilename;
		    copy.costSP = this.costSP;
		    copy.traitTargetType = this.traitTargetType;
		    copy.traitEffectType = this.traitEffectType;
		    copy.aoeRadius = this.aoeRadius;
		    copy.range = this.range;
		    copy.traitScript = this.traitScript;
            copy.associatedSpellTag = this.associatedSpellTag;

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

            copy.traitEffectTagList = new List<EffectTagForDropDownList>();
            foreach (EffectTagForDropDownList s in this.traitEffectTagList)
            {
                copy.traitEffectTagList.Add(s);
            }

            return copy;
	    }
    }
}
