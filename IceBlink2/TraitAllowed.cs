using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class TraitAllowed 
    {
	    public string name = "";
	    public string tag = "";
	    public int atWhatLevelIsAvailable = 0;
	    public bool automaticallyLearned = false;
	    public bool allow = true;
        public string useableInSituation = "Always"; //InCombat, OutOfCombat, Always, Passive
        public string associatedSpellTag = "none";
        public List<string> traitWorksOnlyWhen = new List<string>();
        public List<string> traitWorksNeverWhen = new List<string>();

        public TraitAllowed()
	    {
		
	    }
	
	    public TraitAllowed DeepCopy()
	    {
		    TraitAllowed copy = new TraitAllowed();
		    copy.name = this.name;
		    copy.tag = this.tag;
		    copy.atWhatLevelIsAvailable = this.atWhatLevelIsAvailable;
		    copy.automaticallyLearned = this.automaticallyLearned;
		    copy.allow = this.allow;
            copy.useableInSituation = this.useableInSituation;
            copy.associatedSpellTag = this.associatedSpellTag;
            copy.traitWorksOnlyWhen = new List<string>();
            foreach (string s in this.traitWorksOnlyWhen)
            {
                copy.traitWorksOnlyWhen.Add(s);
            }

            copy.traitWorksNeverWhen = new List<string>();
            foreach (string s in this.traitWorksNeverWhen)
            {
                copy.traitWorksNeverWhen.Add(s);
            }

            return copy;
	    }
    }
}
