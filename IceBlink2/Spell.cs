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
    public class Spell 
    {
	    public string name = "newSpell";
	    public string tag = "newSpellTag";
	    public string spellImage = "sp_magebolt";
	    public string description = "";
	    public string useableInSituation = "Always"; //InCombat, OutOfCombat, Always, Passive
	    public string spriteFilename = "none";
	    public string spriteEndingFilename = "none";
	    public string spellStartSound = "none";
        public string spellEndSound = "none";
	    public int costSP = 10;
        public int costHP = 10;	
	    public string spellTargetType = "Enemy"; //Self, Enemy, Friend, PointLocation
	    public string spellEffectType = "Damage"; //Damage, Heal, Buff, Debuff
        public AreaOfEffectShape aoeShape = AreaOfEffectShape.Circle;
	    public int aoeRadius = 1;
	    public int range = 2;	
	    public string spellScript = "none";
        public string spellEffectTag = "none";
        public List<EffectTagForDropDownList> spellEffectTagList = new List<EffectTagForDropDownList>();
        public List<EffectTagForDropDownList> removeEffectTagList = new List<EffectTagForDropDownList>();
    
	    public Spell()
	    {
		
	    }
	
	    public Spell DeepCopy()
	    {
		    Spell copy = new Spell();
		    copy.name = this.name;
		    copy.tag = this.tag;
		    copy.spellImage = this.spellImage;
		    copy.description = this.description;
		    copy.useableInSituation = this.useableInSituation;
		    copy.spriteFilename = this.spriteFilename;	
		    copy.spriteEndingFilename = this.spriteEndingFilename;
		    copy.spellStartSound = this.spellStartSound;
		    copy.spellEndSound = this.spellEndSound;
		    copy.costSP = this.costSP;
            copy.costHP = this.costHP;
            copy.spellTargetType = this.spellTargetType;
		    copy.spellEffectType = this.spellEffectType;
            copy.aoeShape = this.aoeShape;
		    copy.aoeRadius = this.aoeRadius;
		    copy.range = this.range;
		    copy.spellScript = this.spellScript;
            copy.spellEffectTag = this.spellEffectTag;

            //copy.spellEffectTagList = this.spellEffectTagList;
            copy.spellEffectTagList = new List<EffectTagForDropDownList>();
            foreach (EffectTagForDropDownList s in this.spellEffectTagList)
            {
                copy.spellEffectTagList.Add(s);
            }

            copy.removeEffectTagList = new List<EffectTagForDropDownList>();
            foreach (EffectTagForDropDownList s in this.removeEffectTagList)
            {
                copy.removeEffectTagList.Add(s);
            }

            return copy;
	    }
    }
}
