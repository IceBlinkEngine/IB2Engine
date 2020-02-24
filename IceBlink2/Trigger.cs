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
        public string vanishInXTurns = "none";
        public int vanishCounter = 0;
        public string appearInXTurns = "none";
        public int appearCounter = 0;
        public bool changeWalkableStateOnEnabledStateChange = false;
        //below is likely only needed in toolset
        public string tagOfPRopToPlaceOnTop = "none";

        public bool chkTrigHidden = false;
        public string txtTrigFindingTraitTag = "none";
        public string txtTrigFindingDC = "none";
        public string txtTrigSpawningTraitTag = "none";
        public string txtTrigSpawningDC = "none";
        public string txtTrigDespawningTraitTag = "none";
        public string txtTrigDespawningDC = "none";
        public string txtTrigDisablingTraitTag = "none";
        public string txtTrigDisablingDC = "none";
        public string txtTrigEnablingTraitTag = "none";
        public string txtTrigEnablingDC = "none";
        public bool chkTrigEnableOnFinding = false;

        public string bumpTriggerDirection = "none"; //can be fromSouth,fromNorth, fromEast, fromWest, none
        public string mouseOverText = "none";

        //public string traitTagForSpawning = "none";
        //public int DCForSpawning = 0;
        //public string traitTagForFinding = "none";
        //public int DCForFinding = 0;
        //public bool onlyFunctionalWhenFound = false;

        public bool connectedDiscovery = false;
        public string triggerImage = "none";
        public bool encounterTriggerOnEveryStep = false;

        public bool isBumpTrigger = false;

        public string TriggerTag = "newTrigger"; //must be unique
	    public bool Enabled = true;
	    public bool DoOnceOnly = false;
        public bool requiresActiveSearch = false;
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
        public bool event2RequiresTrueReturnCheck = false;
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
        public bool event3RequiresFalseReturnCheck = false;
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

        public bool isLinkToMaster = false;
        public string tagOfLinkedMaster = "none";
        public string tagOfLink = "none";
        public int transitionToMasterRotationCounter = 1;

        public Trigger DeepCopy()
        {
            Trigger copy = new Trigger();
            copy = (Trigger)this.MemberwiseClone();
            return copy;
        }

        public Trigger()
        {
    	


        }
    }
}
