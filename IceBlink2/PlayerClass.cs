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
//using IceBlink;

namespace IceBlink2
{
    public class PlayerClass 
    {
	    public string name = "newClass"; //item name
	    public string tag = "newClassTag"; //item unique tag name
        public bool UsableByPlayer = true;
	    public string description = "";
        public string labelForCastAction = "CAST";
        public string labelForSpellsButtonInCombat = "SPELL";
        public string labelForUseTraitAction = "USE";
        public string labelForUseTraitButtonInCombat = "TRAIT";
        public int startingHP = 10;
	    public int startingSP = 20;
        public string modifierFromSPRelevantAttribute = "intelligence";
        public int hpPerLevelUp = 10;
	    public int spPerLevelUp = 20;
	    public int hpRegenTimeNeeded = 0;
	    public int spRegenTimeNeeded = 0;
        public string spellLabelSingular = "Spell";  
        public string spellLabelPlural = "Spells";
 
        public int[] baseFortitudeAtLevel = new int[]{0, 2, 3, 3, 4, 4, 5, 5, 6};
	    public int[] baseWillAtLevel = new int[]{0, 0, 0, 1, 1, 1, 2, 2, 2};
	    public int[] baseReflexAtLevel = new int[]{0, 0, 0, 1, 1, 1, 2, 2, 2};
        public int[] babTable = new int[]{0,1,2,3,4,5,6,7,8,9};	
	    public int[] xpTable = new int[]{0,200,400,800,1600,3200,6500,12500,25000,50000};
	    public List<ItemRefs> itemsAllowed = new List<ItemRefs>();
	    public List<TraitAllowed> traitsAllowed = new List<TraitAllowed>();
	    public List<SpellAllowed> spellsAllowed = new List<SpellAllowed>();
	
	    public PlayerClass()
	    {
		
	    }
	
	    public bool containsItemRefsWithResRef(String resref)
	    {
		    foreach (ItemRefs i in this.itemsAllowed)
		    {
			    if (i.resref.Equals(resref)) { return true; }
		    }
		    return false;
	    }
	
	    public SpellAllowed getSpellAllowedByTag(String tag)
	    {
		    foreach (SpellAllowed sa in spellsAllowed)
		    {
			    if (sa.tag.Equals(tag))
			    {
				    return sa;
			    }
		    }
		    return null;
	    }
	    public TraitAllowed getTraitAllowedByTag(String tag)
	    {
		    foreach (TraitAllowed ta in traitsAllowed)
		    {
			    if (ta.tag.Equals(tag))
			    {
				    return ta;
			    }
		    }
		    return null;
	    }
	
	    public PlayerClass DeepCopy()
	    {
		    PlayerClass copy = new PlayerClass();
		    copy.name = this.name;
		    copy.tag = this.tag;
            copy.UsableByPlayer = this.UsableByPlayer;
		    copy.description = this.description;
		    copy.startingHP = this.startingHP;
		    copy.startingSP = this.startingSP;	
		    copy.hpPerLevelUp = this.hpPerLevelUp;
		    copy.spPerLevelUp = this.spPerLevelUp;
            copy.spellLabelSingular = this.spellLabelSingular;
            copy.spellLabelPlural = this.spellLabelPlural;

            copy.baseFortitudeAtLevel = (int[])this.baseFortitudeAtLevel.Clone();
		    copy.baseWillAtLevel = (int[])this.baseWillAtLevel.Clone();
		    copy.baseReflexAtLevel = (int[])this.baseReflexAtLevel.Clone();
		    copy.babTable = (int[])this.babTable.Clone();	
		    copy.xpTable = (int[])this.xpTable.Clone();
            copy.itemsAllowed = new List<ItemRefs>();
            copy.modifierFromSPRelevantAttribute = this.modifierFromSPRelevantAttribute;

            copy.labelForCastAction = this.labelForCastAction;
            copy.labelForSpellsButtonInCombat = this.labelForSpellsButtonInCombat;
            copy.labelForUseTraitAction = this.labelForUseTraitAction;
            copy.labelForUseTraitButtonInCombat = this.labelForUseTraitButtonInCombat;

            foreach (ItemRefs s in this.itemsAllowed)
            {
                copy.itemsAllowed.Add(s.DeepCopy());
            }
            copy.spellsAllowed = new List<SpellAllowed>();
            foreach (SpellAllowed sa in this.spellsAllowed)
            {
                copy.spellsAllowed.Add(sa.DeepCopy());
            }
            copy.traitsAllowed = new List<TraitAllowed>();
            foreach (TraitAllowed ta in this.traitsAllowed)
            {
                copy.traitsAllowed.Add(ta.DeepCopy());
            }
		    return copy;
	    }
    }
}
