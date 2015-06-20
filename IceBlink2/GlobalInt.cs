using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class GlobalInt 
    {
	    public string Key = "";
	    public int Value = 0;
    	
	    public GlobalInt()
	    {
		
	    }
	
	    public GlobalInt DeepCopy()
        {
		    GlobalInt copy = new GlobalInt();
		    copy.Key = this.Key;
		    copy.Value = this.Value;
		    return copy;
        }
    }
}
