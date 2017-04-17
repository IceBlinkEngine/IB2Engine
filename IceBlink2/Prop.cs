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
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class Prop 
    {
        public bool isUnderBridge = false;

        public string OnEnterSquareScript = "none";  
        public string OnEnterSquareScriptParm1 = "none";  
        public string OnEnterSquareScriptParm2 = "none";
        public string OnEnterSquareScriptParm3 = "none";  
        public string OnEnterSquareScriptParm4 = "none";  
        public bool canBeTriggeredByPc = true;  
        public bool canBeTriggeredByCreature = true;  
        public int numberOfScriptCallsRemaining = 999;

        public bool allowLastLocationUpdate = false;

        public int LocationX = 0;
	    public int LocationY = 0;
        public int LocationZ = 0;
        public int lastLocationX = 0;
        public int lastLocationY = 0;
        public int priorLastLocationX = 0;
        public int priorLastLocationY = 0;
        public int lastLocationZ = 0;
        public string ImageFileName = "blank";
        public bool isLight = false;
        //[JsonIgnore]
	    public bool PropFacingLeft = true;
	    public string MouseOverText = "none";
	    public bool HasCollision = false;
	    public bool isShown = true;
	    public bool isActive = true;
	    public string PropTag = "newProp";
	    public string PropCategoryName = "newCategory";
	    public string ConversationWhenOnPartySquare = "none";
	    public string EncounterWhenOnPartySquare = "none";
	    public bool DeletePropWhenThisEncounterIsWon = true;
        [JsonIgnore]
	    public Bitmap token;
	    public List<LocalInt> PropLocalInts = new List<LocalInt>();
	    public List<LocalString> PropLocalStrings = new List<LocalString>();
	    //All THE PROJECT LIVING WORLD STUFF
	    public int PostLocationX = 0;
	    public int PostLocationY = 0;
	    public List<WayPoint> WayPointList = new List<WayPoint>();
	    public int WayPointListCurrentIndex = 0;
	    public bool isMover = false;
	    public int ChanceToMove2Squares = 0;
	    public int ChanceToMove0Squares = 0;
	    public string MoverType = "post"; //post, random, patrol, daily, weekly, monthly, yearly
        //[JsonIgnore]
        //Removed save ignore for currentMoveToTarget,, othrisw all patroling props head towards 0,0 after loading a save
	    public Coordinate CurrentMoveToTarget = new Coordinate(0,0);
	    public bool isChaser = false;
        [JsonIgnore]
	    public bool isCurrentlyChasing = false;
	    public int ChaserDetectRangeRadius = 2;
	    public int ChaserGiveUpChasingRangeRadius = 3;
	    public int ChaserChaseDuration = 24;
        [JsonIgnore]
	    public int ChaserStartChasingTime = 0;
	    public int RandomMoverRadius = 5;
	    public bool ReturningToPost = false;
        //public string OnHeartBeatLogicTree = "none";
        //public string OnHeartBeatParms = "";
        public string OnHeartBeatIBScript = "none";
        public string OnHeartBeatIBScriptParms = "";
        public bool passOneMove = false;
        public int randomMoverTimerForNextTarget = 0;
        public int lengthOfLastPath = 0;
        public bool unavoidableConversation = false;
        public List<int> destinationPixelPositionXList = new List<int>();
        public List<int> destinationPixelPositionYList = new List<int>();
        //Turned the 2 ints below to floats for storing pix coordinates with higher precision
        public float currentPixelPositionX = 0;
        public float currentPixelPositionY = 0;
        public int pixelMoveSpeed = 1;
        //public int drawAnchorX = 0;
        //public int drawAnchorY = 0;
        //public bool startingOnVisibleSquare = true;
        public bool wasTriggeredLastUpdate = false;
        public bool blockTrigger = false;
        public bool hasHalo = false;
        public float focalIntensity = 1f;
        public float ringIntensity = 1f;

        public float roamDistanceX = 0;
        public float roamDistanceY = 0;
        public float straightLineDistanceX = 0;
        public float straightLineDistanceY = 0;
        public bool goDown = false;
        public bool goRight = false;
        public float inactiveTimer = 0;

        //prop animation variables
        public int maxNumberOfFrames = 1;
        public int currentFrameNumber = 0;
        public float animationDelayCounter = 0;
        public float updateTicksNeededTillNextFrame = 0;
        public float chanceToTriggerAnimationCycle = 100;
        public bool animationComplete = false;
        public float normalizedTime = 0;
        public int propFrameHeight = 100;
        public int sizeFactor = 100;
        public bool doOnce = false;
        public bool animationIsActive = true;
        public bool hiddenWhenComplete = false;
        public bool hiddenWhenNotActive = false;
        public bool drawAnimatedProp = true;
        public int numberOfCyclesNeededForCompletion = 1;
        public int cycleCounter = 0;
        public int framesNeededForFullFadeInOut = 0; 
        public int totalFramesInWholeLoopCounter = 0;
        public float opacity = 1;
        public bool inverseAnimationDirection = false;
        public bool randomAnimationDirectionEachCall = false; 

        public string OnEnterSquareIBScript = "none";  
        public string OnEnterSquareIBScriptParms = "";
        public bool isTrap = false;  
        public int trapDCforDisableCheck = 10;



        public Prop()
        {
    	
        }
    
        public void initializeProp()
        {
    	    CurrentMoveToTarget = new Coordinate(this.LocationX, this.LocationY);
        }
    
        public Prop DeepCopy()
        {
    	    Prop copy = new Prop();
            copy.allowLastLocationUpdate = this.allowLastLocationUpdate;
            copy.isLight = this.isLight;
            copy.inverseAnimationDirection = this.inverseAnimationDirection;
            copy.randomAnimationDirectionEachCall = this.randomAnimationDirectionEachCall;
            copy.hasHalo = this.hasHalo;
            copy.focalIntensity = this.focalIntensity;
            copy.ringIntensity = this.ringIntensity;
            copy.LocationX = this.LocationX;
		    copy.LocationY = this.LocationY;
            copy.LocationZ = this.LocationZ;
            copy.blockTrigger = this.blockTrigger;
            copy.wasTriggeredLastUpdate = this.wasTriggeredLastUpdate;
            copy.lastLocationX = this.lastLocationX;
            copy.lastLocationY = this.lastLocationY;
            copy.priorLastLocationX = this.lastLocationX;
            copy.priorLastLocationY = this.lastLocationY;
            copy.lastLocationZ = this.lastLocationZ;
            copy.ImageFileName = this.ImageFileName;
		    copy.PropFacingLeft = this.PropFacingLeft;
		    copy.MouseOverText = this.MouseOverText;
		    copy.HasCollision = this.HasCollision;
		    copy.isShown = this.isShown;
		    copy.isActive = this.isActive;
		    copy.PropTag = this.PropTag;
		    copy.PropCategoryName = this.PropCategoryName;
		    copy.ConversationWhenOnPartySquare = this.ConversationWhenOnPartySquare;
		    copy.EncounterWhenOnPartySquare = this.EncounterWhenOnPartySquare;
		    copy.DeletePropWhenThisEncounterIsWon = this.DeletePropWhenThisEncounterIsWon;
            //copy.PropLocalInts = new List<LocalInt>();
            //copy.OnEnterSquareIBScriptParms = this.OnEnterSquareIBScriptParms;
            copy.OnEnterSquareScript = this.OnEnterSquareScript;
            copy.OnEnterSquareScriptParm1 = this.OnEnterSquareScriptParm1;
            copy.OnEnterSquareScriptParm2 = this.OnEnterSquareScriptParm1;
            copy.OnEnterSquareScriptParm3 = this.OnEnterSquareScriptParm1;
            copy.OnEnterSquareScriptParm4 = this.OnEnterSquareScriptParm1;
            copy.canBeTriggeredByPc = this.canBeTriggeredByPc;
            copy.canBeTriggeredByCreature = this.canBeTriggeredByCreature;
            copy.numberOfScriptCallsRemaining = this.numberOfScriptCallsRemaining;
            copy.PropLocalInts = new List<LocalInt>();
            foreach (LocalInt l in this.PropLocalInts)
            {
                LocalInt Lint = new LocalInt();
                Lint.Key = l.Key;
                Lint.Value = l.Value;
                copy.PropLocalInts.Add(Lint);
            }        
            copy.PropLocalStrings = new List<LocalString>();
            foreach (LocalString l in this.PropLocalStrings)
            {
                LocalString Lstr = new LocalString();
                Lstr.Key = l.Key;
                Lstr.Value = l.Value;
                copy.PropLocalStrings.Add(Lstr);
            }
            //PROJECT LIVING WORLD STUFF
            copy.PostLocationX = this.PostLocationX;
            copy.PostLocationY = this.PostLocationY;
            copy.WayPointList = new List<WayPoint>();
            foreach (WayPoint coor in this.WayPointList)
            {
        	    WayPoint c = new WayPoint();
                c.X = coor.X;
                c.Y = coor.Y;
                c.areaName = coor.areaName;
                c.departureTime = coor.departureTime;
                copy.WayPointList.Add(c);
            }
            copy.WayPointListCurrentIndex = this.WayPointListCurrentIndex;
            copy.isMover = this.isMover;
            copy.ChanceToMove2Squares = this.ChanceToMove2Squares;
		    copy.ChanceToMove0Squares = this.ChanceToMove0Squares;
		    copy.MoverType = this.MoverType;
		    copy.CurrentMoveToTarget = new Coordinate(this.CurrentMoveToTarget.X, this.CurrentMoveToTarget.Y);
		    copy.isChaser = this.isChaser;
		    copy.isCurrentlyChasing = this.isCurrentlyChasing;
		    copy.ChaserDetectRangeRadius = this.ChaserDetectRangeRadius;
		    copy.ChaserGiveUpChasingRangeRadius = this.ChaserGiveUpChasingRangeRadius;
		    copy.ChaserChaseDuration = this.ChaserChaseDuration;
		    copy.ChaserStartChasingTime = this.ChaserStartChasingTime;
		    copy.RandomMoverRadius = this.RandomMoverRadius;
            //copy.OnHeartBeatLogicTree = this.OnHeartBeatLogicTree;
            //copy.OnHeartBeatParms = this.OnHeartBeatParms;
            copy.OnHeartBeatIBScript = this.OnHeartBeatIBScript;
            copy.OnHeartBeatIBScriptParms = this.OnHeartBeatIBScriptParms;
            copy.passOneMove = this.passOneMove;
            copy.randomMoverTimerForNextTarget = this.randomMoverTimerForNextTarget;
            copy.lengthOfLastPath = this.lengthOfLastPath;
            copy.unavoidableConversation = this.unavoidableConversation;
            copy.destinationPixelPositionXList = this.destinationPixelPositionXList;
            copy.destinationPixelPositionYList = this.destinationPixelPositionYList;
            copy.currentPixelPositionX = this.currentPixelPositionX;
            copy.currentPixelPositionY = this.currentPixelPositionY;
            copy.pixelMoveSpeed = this.pixelMoveSpeed;
            //copy.drawAnchorX = this.drawAnchorX;
            //copy.drawAnchorY = this.drawAnchorY;
            //copy.startingOnVisibleSquare = this.startingOnVisibleSquare;

            copy.roamDistanceX = this.roamDistanceX;
            copy.roamDistanceY = this.roamDistanceY;
            copy.straightLineDistanceX = this.straightLineDistanceX;
            copy.straightLineDistanceY = this.straightLineDistanceY;
            copy.goDown = this.goDown;
            copy.goRight = this.goRight;
            copy.inactiveTimer = this.inactiveTimer;

            copy.isUnderBridge = this.isUnderBridge;

             //prop animation variables
            copy.maxNumberOfFrames = this.maxNumberOfFrames;
            copy.currentFrameNumber = this.currentFrameNumber;
            copy.animationDelayCounter = this.animationDelayCounter;
            copy.updateTicksNeededTillNextFrame = this.updateTicksNeededTillNextFrame;
            copy.chanceToTriggerAnimationCycle = this.chanceToTriggerAnimationCycle;
            copy.animationComplete = this.animationComplete;
            copy.normalizedTime = this.normalizedTime;
            copy.propFrameHeight = this.propFrameHeight;
            copy.hiddenWhenComplete = this.hiddenWhenComplete;
            copy.sizeFactor = this.sizeFactor;
            copy.doOnce = this.doOnce;
            copy.animationIsActive = this.animationIsActive;
            copy.hiddenWhenNotActive = this.hiddenWhenNotActive;
            copy.drawAnimatedProp = this.drawAnimatedProp;
            copy.numberOfCyclesNeededForCompletion = this.numberOfCyclesNeededForCompletion;
            copy.cycleCounter = this.cycleCounter;
            copy.framesNeededForFullFadeInOut = this.framesNeededForFullFadeInOut;
            copy.totalFramesInWholeLoopCounter = this.totalFramesInWholeLoopCounter;
            copy.opacity = this.opacity;

            copy.OnEnterSquareIBScript = this.OnEnterSquareIBScript;
            copy.OnEnterSquareIBScriptParms = this.OnEnterSquareIBScriptParms;
            copy.isTrap = this.isTrap;
            copy.trapDCforDisableCheck = this.trapDCforDisableCheck;


            return copy;
        }
    }
}
