using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class JournalQuest 
    {
	    public string Name = "newCategory";
	    public string Tag = "tag";
	    public List<JournalEntry> Entries = new List<JournalEntry>();
    
	    public JournalQuest()
	    {
		
	    }
	
	    public JournalEntry getJournalEntryByTag(String tag)
        {
            foreach (JournalEntry it in Entries)
            {
                if (it.Tag.Equals(tag)) return it;
            }
            return null;
        }
	
	    public JournalQuest DeepCopy()
        {
            JournalQuest copy = new JournalQuest();
		    copy.Name = this.Name;
		    copy.Tag = this.Tag;
		    copy.Entries = new List<JournalEntry>();
		    foreach (JournalEntry jent in this.Entries)
            {
                JournalEntry j = jent.DeepCopy();
                copy.Entries.Add(j);
            }
		    return copy;
        }
    }
}
