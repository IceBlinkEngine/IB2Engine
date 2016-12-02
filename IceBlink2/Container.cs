using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace IceBlink2
{   
    public class Container
    {
        public string containerTag = ""; //container tag
        public List<ItemRefs> containerItemRefs = new List<ItemRefs>();        
	    public List<ItemRefs> initialContainerItemRefs = new List<ItemRefs>();

        public Container()
        {
        }

        public bool containsItemWithResRef(String resref)
	    {
		    foreach (ItemRefs i in this.containerItemRefs)
		    {
			    if (i.resref.Equals(resref)) { return true; }
		    }
		    return false;
	    }
        public bool containsInitialItemWithResRef(String resref)
	    {
		    foreach (ItemRefs i in this.initialContainerItemRefs)
		    {
			    if (i.resref.Equals(resref)) { return true; }
		    }
		    return false;
	    }

        public override string ToString()
        {
            return containerTag;
        }
        public Container DeepCopy()
        {
            Container other = (Container)this.MemberwiseClone();
            other.containerItemRefs = new List<ItemRefs>();
            for (int i = 0; i < this.containerItemRefs.Count; i++)
            {
                other.containerItemRefs.Add(this.containerItemRefs[i].DeepCopy());
            }
            other.initialContainerItemRefs = new List<ItemRefs>();
            for (int i = 0; i < this.initialContainerItemRefs.Count; i++)
            {
                other.initialContainerItemRefs.Add(this.initialContainerItemRefs[i].DeepCopy());
            }

            return other;
        }
    }
}
