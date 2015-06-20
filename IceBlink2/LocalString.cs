using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class LocalString 
    {
	    public string Key = "";
	    public string Value = "";
    	
	    public LocalString()
	    {
		
	    }
	
	    public LocalString DeepCopy()
        {
		    LocalString copy = new LocalString();
		    copy.Key = this.Key;
		    copy.Value = this.Value;
		    return copy;
        }
    }
}
