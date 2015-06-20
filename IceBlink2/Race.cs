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
    public class Race 
    {
	    public string name = "newRace"; //item name
	    public string tag = "newRaceTag"; //item unique tag name
        public bool UsableByPlayer = true;
	    public string description = "";
	    public int strMod = 0;
	    public int dexMod = 0;
	    public int intMod = 0;
	    public int chaMod = 0;
	    public int damageTypeResistanceValueAcid = 0;
	    public int damageTypeResistanceValueNormal = 0;
	    public int damageTypeResistanceValueCold = 0;
	    public int damageTypeResistanceValueElectricity = 0;
	    public int damageTypeResistanceValueFire = 0;
	    public int damageTypeResistanceValueMagic = 0;
	    public int damageTypeResistanceValuePoison = 0;
	    public List<string> classesAllowed = new List<string>();
	
	    public Race()
	    {
		
	    }
	
	    public Race DeepCopy()
	    {
		    Race copy = new Race();
		    copy.name = this.name;
		    copy.tag = this.tag;
            copy.UsableByPlayer = this.UsableByPlayer;
		    copy.description = this.description;
		    copy.strMod = this.strMod;
		    copy.dexMod = this.dexMod;	
		    copy.intMod = this.intMod;
		    copy.chaMod = this.chaMod;
		    copy.damageTypeResistanceValueAcid = this.damageTypeResistanceValueAcid;
		    copy.damageTypeResistanceValueNormal = this.damageTypeResistanceValueNormal;
		    copy.damageTypeResistanceValueCold = this.damageTypeResistanceValueCold;
		    copy.damageTypeResistanceValueElectricity = this.damageTypeResistanceValueElectricity;
		    copy.damageTypeResistanceValueFire = this.damageTypeResistanceValueFire;
		    copy.damageTypeResistanceValueMagic = this.damageTypeResistanceValueMagic;
		    copy.damageTypeResistanceValuePoison = this.damageTypeResistanceValuePoison;
		    copy.classesAllowed = new List<string>();
            foreach (string s in this.classesAllowed)
            {
                copy.classesAllowed.Add(s);
            }
		    return copy;
	    }
    }
}
