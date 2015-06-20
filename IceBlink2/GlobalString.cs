using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class GlobalString 
    {
	    public string Key = "";
	    public string Value = "";
    	
	    public GlobalString()
	    {
		
	    }
	
	    public GlobalString DeepCopy()
        {
		    GlobalString copy = new GlobalString();
		    copy.Key = this.Key;
		    copy.Value = this.Value;
		    return copy;
        }
    }
}
