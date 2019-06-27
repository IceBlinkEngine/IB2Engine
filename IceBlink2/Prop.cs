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
        //A. Concept

        //I.minable tile/digging (eg some ore vein or collapsed tunnels):

        //mining/digging is simply done by bumping (walking) into a minable/diggable prop
        //preparation in toolset: tile layer 1 is ground floor, tile layer 2 is material on top 
        //when minning/digging layer 2 is turned blank, revealing layer 1
        //this way authors can inspect their tunnel network simply by setting show layer to 2 to off in toolset
        //place the tile with rock to mine/dig one level higher than ground level (for shadow play in the to be revealed tunnels)
        //when mining/digging the height level of the mined square is lowered to party height level
        //prop default graphic is likely some mostly transparent green indicator for breakability/mining (customizable, might also be a very subtle crack overlay if authors want players to spot the breakbales actively)
        //this prop graphic is (as all props) drawn on top the layer 2 for the top material (see above)
        //(optional) stages 1 to 3 graphics could be intensifying cracked lines, drawn on top the tile 
        //(optional) debris graphic could be some stone rubble
        //(optional) sound file to play when bumping into the prop
        //(optional) sound file to play when the object breaks 

        //II.pure breakbale objects (eg a statue, a crate):

        //height and tile layer 2 graphic are not affcted when bumping into a pure breakable object
        //the rest (stage graphcis, debris) work the same as for minable tiles/digging above (though they will likely be individualized by authors for the affected objects)

        //B.Mechanics in code:

        //I.Toolset

        //stage counter is set to number of stages upon placement of the prop in toolset
        //design as decribed above (using layer one as ground floor, hidden by layer 2 to 5 above)
        //new properties added into new section of prop's properties

        //II.Engine

        //1.minable tile/digging (eg some ore vein or collapsed tunnels):

        //a. upon success (all requirements met: skill level, required item):

        //if stage counter number higher than 0
        //exchange prop graphic to stage graphic with the same number as stage counter number
        //reduce stage counter number by 1
        //play sound file for bumping into prop
        //messaging only to log

        //if stage counter number 0
        //exchange prop graphic against debris graphic (if existent) or blank out prop
        //adjust height of target tile to height level of tile the party is on right now
        //set all tile layer graphics higher than 1 to blank (revealing the ground floor)
        //play sound file for breaking the object
        //set isDiggableIndicatorProp to false for this prop
        //give out item connected if existent (eg ore)
        //messaging only to log

        //b. upon failure
        //messaging to log and as floaty

        public bool justJumpedBetweenAreas = false;

        public int relocX = 0;
        public int relocY = 0;

        public string lastAreaFilenameOfProp = "none";
        public bool isPausing = false;
        public int pauseCounter = 0;

        public bool skipNavigationThisTurn = false;

        public float propMovingHalfSpeedMulti = 1;
        public float timeSinceLastFloaty = 0;


        //make copy
        public float currentWalkingSpeed = 0;
        public float breathAnimationDelayCounter = 0;
        public bool showBreathingFrame = false;
        public float walkAnimationDelayCounter = 0;
        public bool showWalkingFrame = false;
        public float idleAnimationDelayCounter = 0;
        public bool showIdlingFrame = false;
        public float hurdle = 10f;

        public bool isCurrentlyScrolling = false;

        public bool isPureBreakableProp = false;
        //public bool isDiggableIndicatorProp = false;
        public string requiredItemInInventory = "none"; //like eg pick axes of varying qualities
        public string breakableTraitTag = "none";
        public int breakableDC = 10;
        public int numberOfStages = 0;
        public string debrisGraphic = "none";
        public string stageGraphic1 = "none";
        public string stageGraphic2 = "none";
        public string stageGraphic3 = "none";
        public string resRefOfItemGained = "none";
        public string nameOfSoundFileBump = "none";
        public string nameOfSoundFileBreak = "break";

        public int counterStages = 0;
        public bool isBroken = false;

        public bool isLever = false;
        public bool isOn = false;
        public string nameOfBitmapON = "none";
        public string nameOfBitmapOFF = "none";
        public string keyOFGlobalIntToChange = "none";
        public int valueOfGlobalIntOFF = 0;
        public int valueOfGlobalIntON = 0;

        //also need to add gaPushObjectScript as nezry point bump registration)
        //pushable props
        //1.pushable grid properties (04f - STEP: Pushable grid)
        //public bool isGridForPushableObject = false;
        //public bool showPushableGridOutline = true;
        public bool gridHasTimeLimit = false;
        public int turnsBeforeGridResets = 0;
        public int timerTurnsBeforeGridResets = 0;
        public bool gridIsCompleted = false;
        public bool completionStateCanBeLostAgain = false;
        public bool pushableGridCanBeResetViaHotkey = true;
        public bool pushableGridCanBeResetEvenAfterCompletion = false;
        public int partyDefaultPushableGridPositionX = 0;
        public int partyDefaultPushableGridPositionY = 0;
        //public string drawPartyDirection = "none";
        public string partyDefaultDrawDirection = "down";
        public bool allPushableGridTargetPositionsAreShared = true;

        public string keyOfGlobalIntToChangeUponPushableGridCompletion = "none";
        public int valueOfGlobalIntToChangeUponPushableGridCompletion = 0;
        public bool lockGridOnCompletion = false;
        public bool removeGridOnCompletion = false;
        public string messageOnCompletion = "none";

        public string keyOfGlobalIntToChangeUponPushableGridFailure = "none";
        public int valueOfGlobalIntToChangeUponPushableGridFailure = 0;
        public bool lockGridOnFailure = false;
        public bool removeGridOnFailure = false;
        public string messageOnFailure = "none";
        public bool pushableGridIsResetOnEachFailure = false;

        //2.pushable object properties (04f - STEP: Pushable object)
        public bool isPushable = false;
        public string pushableGridTriggerTag = "none";
        public int pushableStartPositionX = 0;
        public int pushableStartPositionY = 0;
        public int pushableTargetPositionX = 0;
        public int pushableTargetPositionY = 0;
        public string pushableTraitTag = "strongman";
        public int pushableDC = 10;

        public bool isClimbable = false;
        public string climbDirection = "north"; //north, east, south, west 
        public int climbDC = 0;
        public string climbTrait = "athlete";
        public bool moved2 = false;
        public bool showSneakThroughSymbol = false;
        public int challengeLevelAssignedForEncounterInConvo = 0; 
        public bool alwaysFlagAsEncounter = false;
        public string ingameShownEncName = "none";
        //script firing situations:

        //trap: step on/bump, no skill roll for firing it, can be disarmed
        //none: step on/bump, no skill roll for firing it, cannot be disarmed
        //hiddenInfo: search, skill roll required, cannot be disarmed

        //doors are props with collision
        //all ACTIVE door props make their own square los blocking
        //all must carry a gaOpenObject script that checks for a customizable tratit (eg Pick Lock) or even requires a customized key item, that is optinally rmeoved upon contact with door 
        //if an active door prop has a gaOpenObject script attached with scriptname "unlocked", it will (1) show an opened floaty on contact, (2) be set to non-active, (3) have collission set to off and (4) optionally change its sprite
        //a door with a gaOpenObject script attached other than "unlocked", will do the same only on successful gaOpenObject script call
        //on a failed call it will display a red floaty showing the requirement(s): key name and/or trait eith DC

        //chests, chasms, climbale obstacles, destructible walls and pushable objects will work the same way (via gc scripts) 

        //place in trap section, using the script options there 
        public bool isDoor = false;
        //png when opened
        public string differentSpriteWhenOpen = "none";
        //for ia pushable gridconnection
        public string keyOfFirstGlobalIntThatControllsDoor = "none";
        public int valueOfFirstGlobalIntThatOpensDoor = 1;
        public string keyOfSecondGlobalIntThatControllsDoor = "none";
        public int valueOfSecondGlobalIntThatOpensDoor = 1;
        public string keyOfThirdGlobalIntThatControllsDoor = "none";
        public int valueOfThirdGlobalIntThatOpensDoor = 1;
        public string keyOfFourthGlobalIntThatControllsDoor = "none";
        public int valueOfFourthGlobalIntThatOpensDoor = 1;
        public string keyOfFifthGlobalIntThatControllsDoor = "none";
        public int valueOfFifthGlobalIntThatOpensDoor = 1;

        public bool isContainer = false;
        public string containerTag = "none";
        public string keyOfFirstGlobalIntThatControllsChest = "none";
        public int valueOfFirstGlobalIntThatOpensChest = 1;
        public string keyOfSecondGlobalIntThatControllsChest = "none";
        public int valueOfSecondGlobalIntThatOpensChest = 1;
        public string keyOfThirdGlobalIntThatControllsChest = "none";
        public int valueOfThirdGlobalIntThatOpensChest = 1;
        public string keyOfFourthGlobalIntThatControllsChest = "none";
        public int valueOfFourthGlobalIntThatOpensChest = 1;
        public string keyOfFifthGlobalIntThatControllsChest = "none";
        public int valueOfFifthGlobalIntThatOpensChest = 1;

        public bool isHiddenInfo = false;
        public string floatyAndLogText = "none";
        public string conversationName = "none";
        public string boxText = "none";
        public int infoDC = 15;
        public string infoTraitTag = "mechanics";
        public bool showOnlyOnce = false;
        public string globalStringKey = "none";
        public string globalStringValue = "none";

        public bool isTrapMain = false;
        //public bool canBeDisarmed = true;
        public int trapDC = 15;
        public string trapTraitTag = "mechanics";        
        public string scriptFilename = "none";
        public string parm1 = "none";
        public string parm2 = "none";
        public string parm3 = "none";
        public string parm4 = "none";
        public bool onlyOnce = false;
        public string scriptActivationFloaty = "none";
        public string scriptActivationLogEntry = "none";

        public bool isSecretDoor = false;
        public int secretDoorDC = 15;
        public string secretDoorTraitTag = "mechanics";
         
        public bool wasKilled = false;

        public bool stealthSkipsPropTriggers = false;
        public bool isStealthed = false;

        public int movementSpeed = -1; //-1 denotes a prop taht moves at default speed 50% chance of zero moves, 0% chance of double moves
                                       //otherwise, use positive vlues will compred to party speed; party speed is detemrioned by its leader (5 per trait level + DEX + items)
                                       //10 feels like a normal party speed with someone who has bit expertise in it, 20 is quite fast alreday, 30 is really fast
        public int spotEnemy = -1;
        public int stealth = -1;

        public bool alwaysDrawNormalSize = false;

        public bool encounterPropTriggerOnEveryStep = false;

        //new properties for faction and respawn system
        //respawn:
        public int respawnTimeInHours = -1; //-1 meaning false, respawn time is in hours
        public int maxNumberOfRespawns = -1;//-1 meaning no limit to the number of respawns
        //public bool spawnOnNearbySquaresIfOccupied = false; //if false, the respawn will be delayed until target square is free
        public int respawnTimeInMinutesPassedAlready = 0; //internal property, not for toolset
        public int numberOfRespawnsThatHappenedAlready = 0;//internal property, not for toolset
        public string nameAsMaster = "none";// blank meaning this prop is master of none
        public string thisPropsMaster = "none"; //blank means this prop has no master; refers to nameAsMaster of another prop
        public bool instantDeathOnMasterDeath = false; //if true,the propsis immediately placed on List<Prop> propsWaitingForRespawn of this module on its master's death
        public string keyOfGlobalVarToSetTo1OnDeathOrInactivity = "none";//set to zero again on respawn
        public string spawnArea = "";
        public int spawnLocationX = 0;
        public int spawnLocationY = 0;
        public int spawnLocationZ = 0;

        //faction:
        public string factionTag = "none";
        public int requiredFactionStrength = 0;
        public int maxFactionStrength = -1;
        public int worthForOwnFaction = 0;
        public string otherFactionAffectedOnDeath1 = "none";
        public int effectOnOtherFactionOnDeath1 = 0;
        public string otherFactionAffectedOnDeath2 = "none";
        public int effectOnOtherFactionOnDeath2 = 0;
        public string otherFactionAffectedOnDeath3 = "none";
        public int effectOnOtherFactionOnDeath3 = 0;
        public string otherFactionAffectedOnDeath4 = "none";
        public int effectOnOtherFactionOnDeath4 = 0;
        public bool pendingFactionStrengthEffectReversal = false;

        //gcCheck
        public string firstGcScriptName = "none";
        public string firstGcParm1 = "none";
        public string firstGcParm2 = "none";
        public string firstGcParm3 = "none";
        public string firstGcParm4 = "none";
        public bool firstCheckForConditionFail = false;

        public string secondGcScriptName = "none";
        public string secondGcParm1 = "none";
        public string secondGcParm2 = "none";
        public string secondGcParm3 = "none";
        public string secondGcParm4 = "none";
        public bool secondCheckForConditionFail = false;

        public string thirdGcScriptName = "none";
        public string thirdGcParm1 = "none";
        public string thirdGcParm2 = "none";
        public string thirdGcParm3 = "none";
        public string thirdGcParm4 = "none";
        public bool thirdCheckForConditionFail = false;

        public bool allConditionsMustBeTrue = true;


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
        //[JsonIgnore]
	    public bool isCurrentlyChasing = false;
	    public int ChaserDetectRangeRadius = 2;
	    public int ChaserGiveUpChasingRangeRadius = 3;
	    public int ChaserChaseDuration = 24;
        //[JsonIgnore]
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

        public string permanentText = "none";



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

            //TODO
            copy.justJumpedBetweenAreas = justJumpedBetweenAreas;
            copy.relocX = relocX;
            copy.relocY = relocY;
            copy.lastAreaFilenameOfProp = lastAreaFilenameOfProp;
            copy.isPausing = isPausing;
            copy.pauseCounter = pauseCounter;
        copy.skipNavigationThisTurn = skipNavigationThisTurn;
        copy.propMovingHalfSpeedMulti = propMovingHalfSpeedMulti;
        copy.timeSinceLastFloaty = timeSinceLastFloaty;

        copy.currentWalkingSpeed = currentWalkingSpeed;
        copy.breathAnimationDelayCounter = breathAnimationDelayCounter;
        copy.showBreathingFrame = showBreathingFrame;
        copy.walkAnimationDelayCounter = walkAnimationDelayCounter;
        copy.showWalkingFrame = showWalkingFrame;
        copy.idleAnimationDelayCounter = idleAnimationDelayCounter;
        copy.showIdlingFrame = showIdlingFrame;
        copy.hurdle = hurdle;

        copy.isBroken = isBroken;
        copy.isPureBreakableProp = isPureBreakableProp;
        //copy.isDiggableIndicatorProp = isDiggableIndicatorProp;
        copy.requiredItemInInventory = requiredItemInInventory; //like eg pick axes of varying qualities
        copy.breakableTraitTag = breakableTraitTag;
        copy.breakableDC = breakableDC;
        copy.numberOfStages = numberOfStages;
        copy.debrisGraphic = debrisGraphic;
        copy.stageGraphic1 = stageGraphic1;
        copy.stageGraphic2 = stageGraphic2;
        copy.stageGraphic3 = stageGraphic3;
        copy.resRefOfItemGained = resRefOfItemGained;
        copy.nameOfSoundFileBump = nameOfSoundFileBump;
        copy.nameOfSoundFileBreak = nameOfSoundFileBreak;

        copy.counterStages = counterStages;


            copy.gridHasTimeLimit = gridHasTimeLimit;
            copy.turnsBeforeGridResets = turnsBeforeGridResets;
            copy.timerTurnsBeforeGridResets = timerTurnsBeforeGridResets;
            copy.gridIsCompleted = gridIsCompleted;
            copy.isLever = isLever;
            copy.isOn = isOn;
            copy.nameOfBitmapON = nameOfBitmapON;
            copy.nameOfBitmapOFF = nameOfBitmapOFF;
            copy.keyOFGlobalIntToChange = keyOFGlobalIntToChange;
            copy.valueOfGlobalIntOFF = valueOfGlobalIntOFF;
            copy.valueOfGlobalIntON = valueOfGlobalIntON;

            //1.pushable grid properties (04f - STEP: Pushable grid)
            //copy.isGridForPushableObject = isGridForPushableObject;
            //copy.showPushableGridOutline = showPushableGridOutline;
            copy.completionStateCanBeLostAgain = completionStateCanBeLostAgain;
            copy.keyOfFirstGlobalIntThatControllsDoor = keyOfFirstGlobalIntThatControllsDoor;
            copy.valueOfFirstGlobalIntThatOpensDoor = valueOfFirstGlobalIntThatOpensDoor;
            copy.keyOfSecondGlobalIntThatControllsDoor = keyOfSecondGlobalIntThatControllsDoor;
            copy.valueOfSecondGlobalIntThatOpensDoor = valueOfSecondGlobalIntThatOpensDoor;
            copy.keyOfThirdGlobalIntThatControllsDoor = keyOfThirdGlobalIntThatControllsDoor;
            copy.valueOfThirdGlobalIntThatOpensDoor = valueOfThirdGlobalIntThatOpensDoor;
            copy.keyOfFourthGlobalIntThatControllsDoor = keyOfFourthGlobalIntThatControllsDoor;
            copy.valueOfFourthGlobalIntThatOpensDoor = valueOfFourthGlobalIntThatOpensDoor;
            copy.keyOfFifthGlobalIntThatControllsDoor = keyOfFifthGlobalIntThatControllsDoor;
            copy.valueOfFifthGlobalIntThatOpensDoor = valueOfFifthGlobalIntThatOpensDoor;

            copy.keyOfFirstGlobalIntThatControllsChest = keyOfFirstGlobalIntThatControllsChest;
            copy.valueOfFirstGlobalIntThatOpensChest = valueOfFirstGlobalIntThatOpensChest;

            copy.keyOfSecondGlobalIntThatControllsChest = keyOfSecondGlobalIntThatControllsChest;
            copy.valueOfSecondGlobalIntThatOpensChest = valueOfSecondGlobalIntThatOpensChest;
            copy.keyOfThirdGlobalIntThatControllsChest = keyOfThirdGlobalIntThatControllsChest;
            copy.valueOfThirdGlobalIntThatOpensChest = valueOfThirdGlobalIntThatOpensChest;
            copy.keyOfFourthGlobalIntThatControllsChest = keyOfFourthGlobalIntThatControllsChest;
            copy.valueOfFourthGlobalIntThatOpensChest = valueOfFourthGlobalIntThatOpensChest;
            copy.keyOfFifthGlobalIntThatControllsChest = keyOfFifthGlobalIntThatControllsChest;
            copy.valueOfFifthGlobalIntThatOpensChest = valueOfFifthGlobalIntThatOpensChest;

            copy.partyDefaultDrawDirection = partyDefaultDrawDirection;
            copy.pushableGridCanBeResetViaHotkey = pushableGridCanBeResetViaHotkey;
            copy.pushableGridCanBeResetEvenAfterCompletion = pushableGridCanBeResetEvenAfterCompletion;
            copy.partyDefaultPushableGridPositionX = partyDefaultPushableGridPositionX;
            copy.partyDefaultPushableGridPositionY = partyDefaultPushableGridPositionY;
            copy.allPushableGridTargetPositionsAreShared = allPushableGridTargetPositionsAreShared;
            copy.keyOfGlobalIntToChangeUponPushableGridCompletion = keyOfGlobalIntToChangeUponPushableGridCompletion;
            copy.valueOfGlobalIntToChangeUponPushableGridCompletion = valueOfGlobalIntToChangeUponPushableGridCompletion;
            copy.keyOfGlobalIntToChangeUponPushableGridFailure = keyOfGlobalIntToChangeUponPushableGridFailure;
            copy.valueOfGlobalIntToChangeUponPushableGridFailure = valueOfGlobalIntToChangeUponPushableGridFailure;
            copy.pushableGridIsResetOnEachFailure = pushableGridIsResetOnEachFailure;
            copy.lockGridOnCompletion = lockGridOnCompletion;
            copy.removeGridOnCompletion = removeGridOnCompletion;
            copy.messageOnCompletion = messageOnCompletion;
            copy.lockGridOnFailure = lockGridOnFailure;
            copy.removeGridOnFailure = removeGridOnFailure;
            copy.messageOnFailure = messageOnFailure;

            //2.pushable object properties (04f - STEP: Pushable object)
            copy.isPushable = isPushable;
            copy.pushableGridTriggerTag = pushableGridTriggerTag;
            copy.pushableStartPositionX = pushableStartPositionX;
            copy.pushableStartPositionY = pushableStartPositionY;
            copy.pushableTargetPositionX = pushableTargetPositionX;
            copy.pushableTargetPositionY = pushableTargetPositionY;
            copy.pushableTraitTag = pushableTraitTag;
            copy.pushableDC = pushableDC;

            copy.isClimbable = this.isClimbable;
            copy.climbDirection = this.climbDirection;
            copy.climbDC = this.climbDC;
            copy.climbTrait = this.climbTrait;
            copy.moved2 = this.moved2;
            copy.showSneakThroughSymbol = this.showSneakThroughSymbol;
            copy.challengeLevelAssignedForEncounterInConvo = this.challengeLevelAssignedForEncounterInConvo;
            copy.alwaysFlagAsEncounter = this.alwaysFlagAsEncounter;
            copy.ingameShownEncName = this.ingameShownEncName;
            copy.isDoor = this.isDoor;
            copy.differentSpriteWhenOpen = this.differentSpriteWhenOpen;

            copy.isContainer = this.isContainer;
            copy.containerTag = this.containerTag;

            copy.isHiddenInfo = this.isHiddenInfo;
            copy.floatyAndLogText = this.floatyAndLogText;
            copy.conversationName = this.conversationName;
            copy.boxText = this.boxText;
            copy.infoDC = this.infoDC;
            copy.infoTraitTag = this.infoTraitTag;
            copy.showOnlyOnce = this.showOnlyOnce;
            copy.globalStringKey = this.globalStringKey;
            copy.globalStringValue = this.globalStringValue;

            copy.scriptFilename = this.scriptFilename;
            copy.parm1 = this.parm1;
            copy.parm2 = this.parm2;
            copy.parm3 = this.parm3;
            copy.parm4 = this.parm4;
            copy.onlyOnce = this.onlyOnce;
            copy.scriptActivationFloaty = this.scriptActivationFloaty;
            copy.scriptActivationLogEntry = this.scriptActivationLogEntry;
             
            copy.isTrapMain = this.isTrapMain;
            copy.trapDC = this.trapDC;
            copy.trapTraitTag = this.trapTraitTag;

            copy.isSecretDoor = this.isSecretDoor;
            copy.secretDoorDC = this.secretDoorDC;
            copy.secretDoorTraitTag = this.secretDoorTraitTag;


            //copy.secretDoorDirectionEW = this.secretDoorDirectionEW;
            copy.wasKilled = this.wasKilled;
            copy.stealthSkipsPropTriggers = this.stealthSkipsPropTriggers;
            copy.isStealthed = this.isStealthed;
            //copy.token = this.token;
            copy.spotEnemy = this.spotEnemy;
            copy.stealth = this.stealth;
            copy.movementSpeed = this.movementSpeed;
            copy.permanentText = this.permanentText;
            copy.alwaysDrawNormalSize = this.alwaysDrawNormalSize;
            copy.encounterPropTriggerOnEveryStep = this.encounterPropTriggerOnEveryStep;
            copy.pendingFactionStrengthEffectReversal = this.pendingFactionStrengthEffectReversal;
            copy.spawnLocationX = this.spawnLocationX;
            copy.spawnLocationY = this.spawnLocationY;
            copy.spawnLocationZ = this.spawnLocationZ;
            copy.spawnArea = this.spawnArea;
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
                //List<BarkString> blAT = new List<BarkString>();
                //List<BarkString> blON = new List<BarkString>();
                foreach (BarkString bAT in coor.BarkStringsAtWayPoint)
                {
                    c.BarkStringsAtWayPoint.Add(bAT.DeepCopy());
                }
                foreach (BarkString bON in coor.BarkStringsOnTheWayToNextWayPoint)
                {
                    c.BarkStringsOnTheWayToNextWayPoint.Add(bON.DeepCopy());
                }
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

            //faction and respawn systems
        copy.respawnTimeInHours = this.respawnTimeInHours; //-1 meaning false, respawn time is in hours
        copy.maxNumberOfRespawns = this.maxNumberOfRespawns;//-1 meaning no limit to the number of respawns
        //copy.spawnOnNearbySquaresIfOccupied = this.spawnOnNearbySquaresIfOccupied; //if false, the respawn will be delayed until target square is free
        copy.respawnTimeInMinutesPassedAlready = this.respawnTimeInMinutesPassedAlready; //internal property, not for toolset
        copy.numberOfRespawnsThatHappenedAlready = this.numberOfRespawnsThatHappenedAlready;//internal property, not for toolset
        copy.nameAsMaster = this.nameAsMaster;// blank meaning this prop is master of none
        copy.thisPropsMaster = this.thisPropsMaster; //blank means this prop has no master; refers to nameAsMaster of another prop
        copy.instantDeathOnMasterDeath = this.instantDeathOnMasterDeath; //if true,the propsis immediately placed on List<Prop> propsWaitingForRespawn of this module on its master's death
        copy.keyOfGlobalVarToSetTo1OnDeathOrInactivity = this.keyOfGlobalVarToSetTo1OnDeathOrInactivity;//set to zero again when living and active

        //faction:
        copy.factionTag = this.factionTag;
        copy.requiredFactionStrength = this.requiredFactionStrength;
        copy.maxFactionStrength = this.maxFactionStrength;
        copy.worthForOwnFaction = this.worthForOwnFaction;
        copy.otherFactionAffectedOnDeath1 = this.otherFactionAffectedOnDeath1;
        copy.effectOnOtherFactionOnDeath1 = this.effectOnOtherFactionOnDeath1;
        copy.otherFactionAffectedOnDeath2 = this.otherFactionAffectedOnDeath2;
        copy.effectOnOtherFactionOnDeath2 = this.effectOnOtherFactionOnDeath2;
        copy.otherFactionAffectedOnDeath3 = this.otherFactionAffectedOnDeath3;
        copy.effectOnOtherFactionOnDeath3 = this.effectOnOtherFactionOnDeath3;
        copy.otherFactionAffectedOnDeath4 = this.otherFactionAffectedOnDeath4;
        copy.effectOnOtherFactionOnDeath4 = this.effectOnOtherFactionOnDeath4;

        //gcCheck
        copy.firstGcScriptName = this.firstGcScriptName;
        copy.firstGcParm1 = this.firstGcParm1;
        copy.firstGcParm2 = this.firstGcParm2;
        copy.firstGcParm3 = this.firstGcParm3;
        copy.firstGcParm4 = this.firstGcParm4;
        copy.firstCheckForConditionFail = this.firstCheckForConditionFail;

            copy.secondGcScriptName = this.secondGcScriptName;
            copy.secondGcParm1 = this.secondGcParm1;
            copy.secondGcParm2 = this.secondGcParm2;
            copy.secondGcParm3 = this.secondGcParm3;
            copy.secondGcParm4 = this.secondGcParm4;
            copy.secondCheckForConditionFail = this.secondCheckForConditionFail;

            copy.thirdGcScriptName = this.thirdGcScriptName;
            copy.thirdGcParm1 = this.thirdGcParm1;
            copy.thirdGcParm2 = this.thirdGcParm2;
            copy.thirdGcParm3 = this.thirdGcParm3;
            copy.thirdGcParm4 = this.thirdGcParm4;
            copy.thirdCheckForConditionFail = this.thirdCheckForConditionFail;


        copy.allConditionsMustBeTrue = this.allConditionsMustBeTrue;

            return copy;
        }
    }
}
