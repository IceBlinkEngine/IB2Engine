using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class SpellAllowed 
    {
	    public string name = "";
	    public string tag = "";
	    public int atWhatLevelIsAvailable = 0;
	    public bool automaticallyLearned = false;
	    public bool allow = true;
	
	    public SpellAllowed()
	    {
		
	    }
	
	    public SpellAllowed DeepCopy()
	    {
		    SpellAllowed copy = new SpellAllowed();
		    copy.name = this.name;
		    copy.tag = this.tag;
		    copy.atWhatLevelIsAvailable = this.atWhatLevelIsAvailable;
		    copy.automaticallyLearned = this.automaticallyLearned;
		    copy.allow = this.allow;		
		    return copy;
	    }
    }
}
