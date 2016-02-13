using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class JournalEntry 
    {
	    public string EntryTitle = "newTitle";
	    public string EntryText = "quest entry text";
	    public string Tag = "tag";
        public int EntryId = 0;
        public bool EndPoint = false;
    
	    public JournalEntry()
	    {
		
	    }
	
	    public JournalEntry DeepCopy()
        {
            JournalEntry copy = new JournalEntry();
		    copy.EntryTitle = this.EntryTitle;
		    copy.EntryText = this.EntryText;
		    copy.Tag = this.Tag;
            copy.EntryId = this.EntryId;
		    copy.EndPoint = this.EndPoint;		
		    return copy;
        }
    }
}
