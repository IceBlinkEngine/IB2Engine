using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class LocalImmunityString 
    {
	    public string Value = "";
    	
	    public LocalImmunityString()
	    {
		
	    }
	
	    public LocalImmunityString DeepCopy()
        {
		    LocalImmunityString copy = new LocalImmunityString();
		    copy.Value = this.Value;
		    return copy;
        }
    }
}
