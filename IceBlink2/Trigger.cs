using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Newtonsoft.Json;

namespace IceBlink2
{
    public class Trigger 
    {
	    public string TriggerTag = "newTrigger"; //must be unique
	    public bool Enabled = true;
	    public bool DoOnceOnly = false;
        public bool conversationCannotBeAvoided = true;
	    public List<Coordinate> TriggerSquaresList = new List<Coordinate>();

	    public bool EnabledEvent1 = true;
	    public bool DoOnceOnlyEvent1 = false;
	    public string Event1Type = "none"; // container, transition, conversation, encounter, script
	    public string Event1FilenameOrTag = "none";
	    public int Event1TransPointX = 0;
	    public int Event1TransPointY = 0;
	    public string Event1Parm1 = "none";
	    public string Event1Parm2 = "none";
	    public string Event1Parm3 = "none";
	    public string Event1Parm4 = "none";

	    public bool EnabledEvent2 = true;
	    public bool DoOnceOnlyEvent2 = false;
	    public string Event2Type = "none";
	    public string Event2FilenameOrTag = "none";
	    public int Event2TransPointX = 0;
	    public int Event2TransPointY = 0;
	    public string Event2Parm1 = "none";
	    public string Event2Parm2 = "none";
	    public string Event2Parm3 = "none";
	    public string Event2Parm4 = "none";

	    public bool EnabledEvent3 = true;
	    public bool DoOnceOnlyEvent3 = false;
	    public string Event3Type = "none";
	    public string Event3FilenameOrTag = "none";
	    public int Event3TransPointX = 0;
	    public int Event3TransPointY = 0;
	    public string Event3Parm1 = "none";
	    public string Event3Parm2 = "none";
	    public string Event3Parm3 = "none";
	    public string Event3Parm4 = "none";

        public int numberOfScriptCallsRemaining = 999;  
        public bool canBeTriggeredByPc = true;  
        public bool canBeTriggeredByCreature = true;


        public Trigger()
        {
    	
        }
    }
}
