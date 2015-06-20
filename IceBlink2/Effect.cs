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
    public class Effect 
    {
	    public string name = "newEffect";
	    public string tag = "newEffectTag";
	    public string tagOfSender = "senderTag";
	    public string description = "";
	    public string spriteFilename = "held";
	    public int durationInUnits = 0;
	    public int currentDurationInUnits = 0;
	    public int startingTimeInUnits = 0;
	    public int babModifier = 0;
	    public int acModifier = 0;
	    public bool isStackableEffect = false;
	    public bool isStackableDuration = false;
	    public bool usedForUpdateStats = false;
	    public string effectLetter = "A";
	    public string effectLetterColor = "White";
	    public string effectScript = "none";
	
	    public Effect()
	    {
		
	    }
	    public Effect DeepCopy()
	    {
		    Effect copy = new Effect();
		    copy.name = this.name;
		    copy.tag = this.tag;
		    copy.tagOfSender = this.tagOfSender;
		    copy.description = this.description;
		    copy.spriteFilename = this.spriteFilename;	
		    copy.durationInUnits = this.durationInUnits;
		    copy.currentDurationInUnits = this.currentDurationInUnits;
		    copy.startingTimeInUnits = this.startingTimeInUnits;
		    copy.babModifier = this.babModifier;
		    copy.acModifier = this.acModifier;
		    copy.isStackableEffect = this.isStackableEffect;
		    copy.isStackableDuration = this.isStackableDuration;
		    copy.usedForUpdateStats = this.usedForUpdateStats;
		    copy.effectLetter = this.effectLetter;
		    copy.effectLetterColor = this.effectLetterColor;
		    copy.effectScript = this.effectScript;		
		    return copy;
	    }
    }
}
