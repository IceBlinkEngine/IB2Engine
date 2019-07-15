using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using Newtonsoft.Json;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class Module
    {

        public int findNewPointCounter = 0;

        public bool partyJustCameFromNeighbouringArea = false;
        public bool isBreathingWorld = true;

        public bool newPropMoveSystem = false;
        public bool justWentSouth = false;
        public bool justWentNorth = false;
        public bool justWentEast = false;
        public bool justWentWest = false;

        //leader
        public float breathAnimationDelayCounter = 0;
        public bool showBreathingFrame = false;
        public float walkAnimationDelayCounter = 0;
        public bool showWalkingFrame = false;
        public float idleAnimationDelayCounter = 0;
        public bool showIdlingFrame = false;
        public float hurdle = 10f;

        //0
        public float breathAnimationDelayCounter0 = 0;
        public bool showBreathingFrame0 = false;
        public float walkAnimationDelayCounter0 = 0;
        public bool showWalkingFrame0 = false;
        public float idleAnimationDelayCounter0 = 0;
        public bool showIdlingFrame0 = false;
        public float hurdle0 = 10f;

        //1
        public float breathAnimationDelayCounter1 = 0;
        public bool showBreathingFrame1 = false;
        public float walkAnimationDelayCounter1 = 0;
        public bool showWalkingFrame1 = false;
        public float idleAnimationDelayCounter1 = 0;
        public bool showIdlingFrame1 = false;
        public float hurdle1 = 10f;

        //2
        public float breathAnimationDelayCounter2 = 0;
        public bool showBreathingFrame2 = false;
        public float walkAnimationDelayCounter2 = 0;
        public bool showWalkingFrame2 = false;
        public float idleAnimationDelayCounter2 = 0;
        public bool showIdlingFrame2 = false;
        public float hurdle2 = 10f;

        //3
        public float breathAnimationDelayCounter3 = 0;
        public bool showBreathingFrame3 = false;
        public float walkAnimationDelayCounter3 = 0;
        public bool showWalkingFrame3 = false;
        public float idleAnimationDelayCounter3 = 0;
        public bool showIdlingFrame3 = false;
        public float hurdle3 = 10f;

        //4
        public float breathAnimationDelayCounter4 = 0;
        public bool showBreathingFrame4 = false;
        public float walkAnimationDelayCounter4 = 0;
        public bool showWalkingFrame4 = false;
        public float idleAnimationDelayCounter4 = 0;
        public bool showIdlingFrame4 = false;
        public float hurdle4 = 10f;

        //5
        public float breathAnimationDelayCounter5 = 0;
        public bool showBreathingFrame5 = false;
        public float walkAnimationDelayCounter5 = 0;
        public bool showWalkingFrame5 = false;
        public float idleAnimationDelayCounter5 = 0;
        public bool showIdlingFrame5 = false;
        public float hurdle5 = 10f;


        //walking
        //public float hurdle = 10f;

        public int breathAnimationFrequency = 10;
        public int idleAnimationFrequency = 10;




     

        public float sprintModifier = 1.0f; 
        public int oldTailPositionX = 0;
        public int oldTailPositionY = 0;
        public string tailScrollDirection = "none"; //up, down, left, right

        public bool doNotStartScrolling = false;
        public bool keyUpPressedAgain = true;
        //public bool mouseUpPressedAgain = true;

        public bool blockUpKey = false;
        public bool blockDownKey = false;
        public bool blockLeftKey = false;
        public bool blockRightKey = false;

        public bool calledByWaiting = false;
        public float scrollModeSpeed = 1.1f;
        public long elapsed2 = 0;
        public int keyPressCounter = 0;
        public List<float> distances = new List<float>();
        public int stopScrollCounter = 0;
        //public Keys LastPressKeyData = new Keys();
        public string lastPressedKey = "none";
        public string frozenPressedKey = "none";
        public float lastScrollStep = 10;
        public float scrollingSpeedReduction = 1f;
        public bool doTriggerInspiteOfScrolling = true;

        public bool comningFromBattle = false;
        public float preRenderDistance = 0;
        public float postUpdateAdjustmentDistance = 0;

        public float scrollingOverhang = 0;
        public float scrollingOverhang2 = 0;

        public bool blockMainKeyboard = false;
        public bool wasJustCalled = false;

        public bool doThisScrollingsLightShift = true;
        public float nightTimeDarknessOpacity = 0.6f;

        public int counterUpMoves = 0;
        public int uCounter = 0;
        public bool useScrollingSystem = true;
        public float scrollingTimer = 100; //runs from 100 to 0
        public bool isScrollingNow = false;
        public string scrollingDirection = "up"; //up, right, down, left
        public float scrollingSpeed = 4.0f;//default 4f, lower is faster

        public bool useFastRender = false;
        public bool isInitialParticleWave = false;
        public bool wasSuccessfulPush = false;
        public bool showPortrtaitsThisUpdate = false;
        public string permanentPartyText = "none";
        public bool playFootstepSound = true;
        public bool hudIsShown = true;
        public float nightTimeDarkness = 0.65f;
        public bool stopMoves = false;
        //public bool noHaloForParty = false;
        //public bool noHaloAddToParty = false;
        public string drawPartyDirection = "none"; //left, right, up, down
        public string tagOfStealthMainTrait = "shadow";
        public string tagOfStealthCombatTrait = "stealth";
        public string tagOfMovementSpeedTrait = "traveller";
        public string tagOfSpotEnemyTrait = "lookout";
        public string tagOfDisarmTrapCombatTrait = "disabledevice";


        public bool useFlatFootedSystem = true; 

        public bool partyIsSearching = false;
        public int timePerStepAfterSpeedCalc = 6; //in minutes

        public int partySpeed = 100; //default speed is 100; max is 199, min is 1
        public int vehicleAdditionalSpeed = 0;//ís added to party speed, always caps at 199 though (min 1 in case of negative additional vehicle speed)
        public int absoluteVehicleSpeed = 0; //The his replaces the normal party speed, use values from 1 to 199;


        public string oldPartyTokenFilename = "none";
        public bool oldPartyTokenEnabledState = false;

        public bool currentlyOnOwnZone = false;
        public bool currentlyOnMotherZone = false;
        public bool currentlyOnGrandMotherZone = false;


        public bool overviewOwnZoneMapExists = false;
        public bool overviewMotherZoneMapExists = false;
        public bool overviewGrandMotherZoneMapExists = false;

        public bool showOverviewButtonOwnZoneMap = false;
        public bool showOverviewButtonMotherZoneMap = false;
        public bool showOverviewButtonGrandMotherZoneMap = false;

        public string filenameOfOwnZoneMap = "none";
        public string filenameOfMotherZoneMap = "none";
        public string filenameOfGrandMotherZoneMap = "none";

        public string ingameNameOfOwnZoneMap = "none";
        public string ingameNameOfMotherZoneMap = "none";
        public string ingameNameOfGrandMotherZoneMap = "none";

        public string overviewReturnAreaName = "none";
        public int overviewReturnLocationX = 0;
        public int overviewReturnLocationY = 0; 

        public bool allowIntraPartyConvos = false;

        public bool useLightSystem = true; 

        public bool useComplexCoordinateSystem = true;

        public bool useAlternativeSpeechBubbleSystem = true;

        public bool realTimeTimerStopped = false;

        public int poorVisionModifier = 0;

        public int nightFightModifier = -4;
        public int darkFightModifier = -8; 

        public bool alreadyDeleted = false; 

        public string currentPropTag = "none";

        public bool noRimLights = false;
        public bool blendOutTooHighAndTooDeepTiles = false;

        public bool activeSearchDoneThisMove = false;
        public bool activeSearchSPCostPaidByByLeaderOnly = true;
        public int activeSearchSPCost = 1;

        public List<string> addedItemsRefs = new List<string>();

        public bool encounterSingleImageAutoScale = true;
        public bool useMinimalisticUI = true;
        public bool useManualCombatCam = true;
        public bool useCombatSmoothMovement = true;
        public float fogOfWarOpacity = 0.9525f;
        public bool spritesUnderOverlays = true;

        public int creatureCounterSubstractor = 0;
        public int moveOrderOfCreatureThatIsBeforeBandChange = 0;
        public bool enteredFirstTime = false;
        public int indexOfCurrentArea = -1;
        public int indexOfNorthernNeighbour = -1;
        public int indexOfSouthernNeighbour = -1;
        public int indexOfEasternNeighbour = -1;
        public int indexOfWesternNeighbour = -1;
        public int indexOfNorthEasternNeighbour = -1;
        public int indexOfNorthWesternNeighbour = -1;
        public int indexOfSouthEasternNeighbour = -1;
        public int indexOfSouthWesternNeighbour = -1;

        public int seamlessModififierMinX = 0;
        public int seamlessModififierMaxX = 0;
        public int seamlessModififierMinY = 0;
        public int seamlessModififierMaxY = 0;

        public int resistanceMaxValue = 85;

        //public float fullScreenEffectOpacityWeatherOld = 0;
        public string oldWeatherName = "";
        public float maintainWeatherFromLastAreaTimer = 0;
        public bool blockCloudCreation = false;
        public bool isFoggy = false;
        public bool blockFogCreation = false;
        public bool isSnowing = false;
        public bool isLightning = false;
        public bool isSandstorm = false;
        public float logOpacity = 1f;
        public int logFadeCounter = 120;
        public bool hideInterfaceNextMove = false;

        public float pixDistanceToBorderWest = 0;
        public float pixDistanceToBorderEast = 0;
        public float pixDistanceToBorderNorth = 0;
        public float pixDistanceToBorderSouth = 0;
        public string windDirection = "";
        public bool isRaining = false;
        public bool isCloudy = false;
        public float sandStormDirectionX = 1f;
        public float sandStormDirectionY = 1f;
        public string sandStormBlowingTo = "";

        public bool breakActiveSearch = false;
      
        public string moduleName = "none";
        public string moduleLabelName = "none";
        public int moduleVersion = 1;
        public string saveName = "empty";
        public string uniqueSessionIdNumberTag = "";
        public string defaultPlayerFilename = "drin.json";
        public bool mustUsePreMadePC = false;
        public int numberOfPlayerMadePcsAllowed = 1;
        public int numberOfPlayerMadePcsRequired = 1;
        public int MaxPartySize = 6;
        //public int requiredPartySize = 0;
        public string moduleDescription = "";
        public string moduleCredits = "";
        public int nextIdNumber = 100;
        public int WorldTime = 0;
        public int lastWorldTime = 0;
        public int TimePerRound = 6;
        public bool debugMode = false;
        public bool allowSave = true;
        public bool useLuck = false;
        public bool hideRoster = false;
        public bool use3d6 = false;

        //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        public bool useHybridRollPointDistribution = false;
        public bool useManualPointDistribution = true;
        public int attributeBaseValue = 10;
        public int attributeMinValue = 6;
        public int attributeMaxValue = 18;
        public int pointPoolSize = 15;
        public int twoPointThreshold = 14;
        public int threePointThreshold = 16;
        public int fourPointThreshold = 18;
        public int numberOfMentalAtttributesBelowBaseAllowed = 2;
        public int numberOfPhysicalAtttributesBelowBaseAllowed = 2;

        public int counterMentalAttributesBelowTen = 0;
        public int counterPhysicalAttributesBelowTen = 0;
        public int counterPointsToDistributeLeft = 0;
        
        //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

        public bool useUIBackground = true;
        public string fontName = "Metamorphous";
        public string fontFilename = "Metamorphous-Regular.ttf";
        public float fontD2DScaleMultiplier = 1.0f;
        public int logNumberOfLines = 20;
        public string spellLabelSingular = "Spell";
        public string spellLabelPlural = "Spells";
        public string traitLabelSingular = "Trait";
        public string traitsLabelPlural = "Traits";
        public string goldLabelSingular = "Gold";
        public string goldLabelPlural = "Gold";
        public string raceLabel = "Race";
        public float diagonalMoveCost = 1.0f;
        public int nonAllowedDiagonalSquareX = -1;
        public int nonAllowedDiagonalSquareY = -1;
        public bool ArmorClassAscending = true;
        public bool calledByRealTimeTimer = false;
        public int numberOfRationsRemaining = 0;
        public int maxNumberOfRationsAllowed = 7;
        public bool hungerIsLethal = true;
        public float maxHPandSPPercentageLostOnHunger = 20;
        public bool showRestMessagesInLog = true;
        public bool showRestMessagesInBox = true;
        public string messageOnRest = "Party safely rests until completely healed.";
        public string messageOnRestAndRaise = "Party safely rests until completely healed (bringing back the - at least presumedly - dead as well).";
        public int maxNumberOfLightSourcesAllowed = 7;
        public int minutesSinceLastRationConsumed = 0;
        //public Keys KeyDebug = new Keys();
        [JsonIgnore]
        public List<Item> moduleItemsList = new List<Item>();
        
        public List<Encounter> moduleEncountersList = new List<Encounter>();
        
        public List<Container> moduleContainersList = new List<Container>();
        public List<Shop> moduleShopsList = new List<Shop>();
        [JsonIgnore]
        public List<Creature> moduleCreaturesList = new List<Creature>();
        [JsonIgnore]
        public List<JournalQuest> moduleJournal = new List<JournalQuest>();
        [JsonIgnore]
        public List<PlayerClass> modulePlayerClassList = new List<PlayerClass>();
        [JsonIgnore]
        public List<Race> moduleRacesList = new List<Race>();
        [JsonIgnore]
        public List<Spell> moduleSpellsList = new List<Spell>();
        [JsonIgnore]
        public List<Trait> moduleTraitsList = new List<Trait>();
        [JsonIgnore]
        public List<Effect> moduleEffectsList = new List<Effect>();
        [JsonIgnore]
        public List<string> nonRepeatableFreeActionsUsedThisTurnBySpellTag = new List<string>();
        [JsonIgnore]
        public bool swiftActionHasBeenUsedThisTurn = false;
        public List<Faction> moduleFactionsList = new List<Faction>();

        public List<Prop> propsWaitingForRespawn = new List<Prop>();

        public List<string> moduleAreasList = new List<string>();
        
        public List<string> moduleConvosList = new List<string>();
        
        public List<Area> moduleAreasObjects = new List<Area>();

        public int lastYadjustment = 0;
        public int lastXadjustment = 0;

        public List<GlobalInt> moduleGlobalInts = new List<GlobalInt>();
        public List<GlobalString> moduleGlobalStrings = new List<GlobalString>();
        public List<ConvoSavedValues> moduleConvoSavedValuesList = new List<ConvoSavedValues>();
        public string startingArea = "";
        public int startingPlayerPositionX = 0;
        public int startingPlayerPositionY = 0;
        public int PlayerLocationX = 4;
        public int PlayerLocationY = 1;
        public int PlayerLastLocationX = 4;
        public int PlayerLastLocationY = 1;
        //public bool PalyerIsUnderBridge = false;
        [JsonIgnore]
        public bool PlayerFacingLeft = true;
        public Area currentArea = new Area();
        [JsonIgnore]
        public Encounter currentEncounter = new Encounter();
        public int partyGold = 0;
        public bool showPartyToken = false;
        public string partyTokenFilename = "prp_party";
        [JsonIgnore]
        public Bitmap partyTokenBitmap;
        public List<Player> playerList = new List<Player>();
        public List<Player> partyRosterList = new List<Player>();
        public List<ItemRefs> partyInventoryRefsList = new List<ItemRefs>();
        public List<JournalQuest> partyJournalQuests = new List<JournalQuest>();
        public List<JournalQuest> partyJournalCompleted = new List<JournalQuest>();
        public string partyJournalNotes = "";
        public bool hideZeroPowerTraits = false;
        public int selectedPartyLeader = 0;
        [JsonIgnore]
        public bool returnCheck = false;
        [JsonIgnore]
        public bool addPCScriptFired = false;
        [JsonIgnore]
        public bool uncheckConvo = false;
        [JsonIgnore]
        public bool removeCreature = false;
        [JsonIgnore]
        public bool deleteItemUsedScript = false;
        [JsonIgnore]
        public int indexOfPCtoLastUseItem = 0;
        public bool com_showGrid = false;
        public bool map_showGrid = false;
        public bool playMusic = false;
        public bool playSoundFx = false;
        public bool playButtonSounds = false;
        public bool playButtonHaptic = false;
        public bool showTutorialParty = true;
        public bool showTutorialInventory = true;
        public bool showTutorialCombat = true;
        public bool showAutosaveMessage = true;
        public bool allowAutosave = true;
        public int combatAnimationSpeed = 50;
        public bool useRationSystem = true;
        public bool fastMode = false;
        public int attackAnimationSpeed = 100;
        public float soundVolume = 1.0f;
        public string OnHeartBeatIBScript = "none";
        public string OnHeartBeatIBScriptParms = "";
        public bool showInteractionState = false;
        public bool avoidInteraction = false;
        public bool useRealTimeTimer = false;
        public bool useSmoothMovement = true;
        public bool useAllTileSystem = true;
        public int realTimeTimerLengthInMilliSeconds = 1500;
        public int attackFromBehindToHitModifier = 2;
        public int attackFromBehindDamageModifier = 0;
        public bool EncounterOfTurnDone = false;
        public bool useOrbitronFont = false;
        public bool justTransitioned = false;
        public bool justTransitioned2 = false;
        public int arrivalSquareX = 1000000;
        public int arrivalSquareY = 1000000;
        //public bool isRecursiveDoTriggerCall = false;
        //public bool isRecursiveDoTriggerCallMovingProp = false;
        //public int fullScreenAnimationFrameCounter1 = 0;
        //public int fullScreenAnimationFrameCounter2 = 0;
        //public int fullScreenAnimationFrameCounter3 = 0;
        //public int fullScreenAnimationFrameCounter4 = 0;
        //public int fullScreenAnimationFrameCounter5 = 0;
        //public int fullScreenAnimationFrameCounter6 = 0;

        //this one can be set from ingame by player eventually (options menu), higher is fast
        //is used for smooth moving props and full screen animation, each on main map, right now
        //later on maybe also use for animated props, like camp fires flickering
        public float allAnimationSpeedMultiplier = 0.45f;

        //listOfEntryWeatherNames (a list of strings containing entry weather names in exact order)
        //listOfEntryWeatherChances (a list of ints containing entry weather chances in exact order)
        //listOfEntryWeatherDurations (a list of ints ontaining durations in exact same order as the entry weather list)
        //listOfExitWeatherName (a list of strings containing exit weather names in exact order)
        //listOfEntryWeatherChances (a list of ints containing exit weather chances in exact order)
        //listOfExitWeatherDurations (a list of durations in exact same order as the exit weather list)
        public List<string> listOfEntryWeatherNames = new List<string>();
        public List<int> listOfEntryWeatherChances = new List<int>();
        public List<int> listOfEntryWeatherDurations = new List<int>();
        public List<string> listOfExitWeatherNames = new List<string>();
        public List<int> listOfExitWeatherChances = new List<int>();
        public List<int> listOfExitWeatherDurations = new List<int>();

        public bool useScriptsForWeather = false;
        public List<Weather> moduleWeathersList = new List<Weather>();
        public List<WeatherEffect> moduleWeatherEffectsList = new List<WeatherEffect>();

        public string currentWeatherName = "";
        public int currentWeatherDuration = 0;
        public string longEntryWeathersList = "";
        public string longExitWeathersList = "";
        public bool useFirstPartOfWeatherScript = true;
        public float howLongWeatherHasRun = 0;
        public float fullScreenEffectOpacityWeather = 1;

        public string weatherSoundsName1 = "";
        public string weatherSoundsName2 = "";
        public string weatherSoundsName3 = "";

        //assuming 28 days in 12 Months, ie 336 days a year
        //notation example: 13:17, Tuesday, 9th of March 1213

        public string weekDayNameToDisplay = "";
        public string monthDayCounterNumberToDisplay = "";
        public string monthDayCounterAddendumToDisplay = "";
        public string monthNameToDisplay = "";

        public string nameOfFirstDayOfTheWeek = "Monday";
        public string nameOfSecondDayOfTheWeek = "Tuesday";
        public string nameOfThirdDayOfTheWeek = "Wednesday";
        public string nameOfFourthDayOfTheWeek = "Thursday";
        public string nameOfFifthDayOfTheWeek = "Friday";
        public string nameOfSixthDayOfTheWeek = "Saturday";
        public string nameOfSeventhDayOfTheWeek = "Sunday";

        public string nameOfFirstMonth = "January";
        public string nameOfSecondMonth = "February";
        public string nameOfThirdMonth = "March";
        public string nameOfFourthMonth = "April";
        public string nameOfFifthMonth = "May";
        public string nameOfSixthMonth = "June";
        public string nameOfSeventhMonth = "July";
        public string nameOfEighthMonth = "August";
        public string nameOfNinthMonth = "September";
        public string nameOfTenthMonth = "October";
        public string nameOfEleventhMonth = "November";
        public string nameOfTwelfthMonth = "December";

        public int timeInThisYear = 0;

        public int currentYear = 0;
        //from 1 to 12
        public int currentMonth = 1;
        //from 1 to 336
        public int currentDay = 1;
        public int currentWeekDay = 1;
        public int currentMonthDay = 1; 



        public bool useWeatherSound = true;
        public bool resetWeatherSound = false;

        public int borderAreaSize = 0;
        public bool allowImmediateRetransition = false;
        [JsonIgnore]
        public List<Bitmap> loadedTileBitmaps = new List<Bitmap>();
        public List<String> loadedTileBitmapsNames = new List<String>();
        [JsonIgnore]
        public List<System.Drawing.Bitmap> loadedMinimapTileBitmaps = new List<System.Drawing.Bitmap>();
        public List<String> loadedMinimapTileBitmapsNames = new List<String>();
        
        public string partyLightColor = "yellow";
        public float partyRingHaloIntensity = 1f;
        public float partyFocalHaloIntensity = 1f;
        public bool partyLightOn = false;
        public string partyLightName = "";
        public List<String> partyLightEnergyName = new List<String>();
        public List<int> partyLightEnergyUnitsLeft = new List<int>();
        public int currentLightUnitsLeft = 0;
        public int durationInStepsOfPartyLightItems = 250;

        public bool useMathGridFade = false;

        public float overrideDelayCounter1 = 0;
        public float cycleCounter1 = 0;
        public float fullScreenAnimationFrameCounter1 = 0;
        public float changeCounter1 = 0;
        public float changeFrameCounter1 = 0;
        public float fullScreenAnimationSpeedX1 = 0; 
        public float fullScreenAnimationSpeedY1 = 0;
        public float fullScreenAnimationFrameCounterX1 = 0;
        public float fullScreenAnimationFrameCounterY1 = 0;

        public float overrideDelayCounter2 = 0;
        public float cycleCounter2 = 0;
        public float fullScreenAnimationFrameCounter2 = 0;
        public float changeCounter2 = 0;
        public float changeFrameCounter2 = 0;
        public float fullScreenAnimationSpeedX2 = 0;
        public float fullScreenAnimationSpeedY2 = 0;
        public float fullScreenAnimationFrameCounterX2 = 0;
        public float fullScreenAnimationFrameCounterY2 = 0;

        public float overrideDelayCounter3 = 0;
        public float cycleCounter3 = 0;
        public float fullScreenAnimationFrameCounter3 = 0;
        public float changeCounter3 = 0;
        public float changeFrameCounter3 = 0;
        public float fullScreenAnimationSpeedX3 = 0;
        public float fullScreenAnimationSpeedY3 = 0;
        public float fullScreenAnimationFrameCounterX3 = 0;
        public float fullScreenAnimationFrameCounterY3 = 0;

        public float overrideDelayCounter4 = 0;
        public float cycleCounter4 = 0;
        public float fullScreenAnimationFrameCounter4 = 0;
        public float changeCounter4 = 0;
        public float changeFrameCounter4 = 0;
        public float fullScreenAnimationSpeedX4 = 0;
        public float fullScreenAnimationSpeedY4 = 0;
        public float fullScreenAnimationFrameCounterX4 = 0;
        public float fullScreenAnimationFrameCounterY4 = 0;

        public float overrideDelayCounter5 = 0;
        public float cycleCounter5 = 0;
        public float fullScreenAnimationFrameCounter5 = 0;
        public float changeCounter5 = 0;
        public float changeFrameCounter5 = 0;
        public float fullScreenAnimationSpeedX5 = 0;
        public float fullScreenAnimationSpeedY5 = 0;
        public float fullScreenAnimationFrameCounterX5 = 0;
        public float fullScreenAnimationFrameCounterY5 = 0;

        public float overrideDelayCounter6 = 0;
        public float cycleCounter6 = 0;
        public float fullScreenAnimationFrameCounter6 = 0;
        public float changeCounter6 = 0;
        public float changeFrameCounter6 = 0;
        public float fullScreenAnimationSpeedX6 = 0;
        public float fullScreenAnimationSpeedY6 = 0;
        public float fullScreenAnimationFrameCounterX6 = 0;
        public float fullScreenAnimationFrameCounterY6 = 0;

        public float overrideDelayCounter7 = 0;
        public float cycleCounter7 = 0;
        public float fullScreenAnimationFrameCounter7 = 0;
        public float changeCounter7 = 0;
        public float changeFrameCounter7 = 0;
        public float fullScreenAnimationSpeedX7 = 0;
        public float fullScreenAnimationSpeedY7 = 0;
        public float fullScreenAnimationFrameCounterX7 = 0;
        public float fullScreenAnimationFrameCounterY7 = 0;

        public float overrideDelayCounter8 = 0;
        public float cycleCounter8 = 0;
        public float fullScreenAnimationFrameCounter8 = 0;
        public float changeCounter8 = 0;
        public float changeFrameCounter8 = 0;
        public float fullScreenAnimationSpeedX8 = 0;
        public float fullScreenAnimationSpeedY8 = 0;
        public float fullScreenAnimationFrameCounterX8 = 0;
        public float fullScreenAnimationFrameCounterY8 = 0;

        public float overrideDelayCounter9 = 0;
        public float cycleCounter9 = 0;
        public float fullScreenAnimationFrameCounter9 = 0;
        public float changeCounter9 = 0;
        public float changeFrameCounter9 = 0;
        public float fullScreenAnimationSpeedX9 = 0;
        public float fullScreenAnimationSpeedY9 = 0;
        public float fullScreenAnimationFrameCounterX9 = 0;
        public float fullScreenAnimationFrameCounterY9 = 0;

        public float overrideDelayCounter10 = 0;
        public float cycleCounter10 = 0;
        public float fullScreenAnimationFrameCounter10 = 0;
        public float changeCounter10 = 0;
        public float changeFrameCounter10 = 0;
        public float fullScreenAnimationSpeedX10 = 0;
        public float fullScreenAnimationSpeedY10 = 0;
        public float fullScreenAnimationFrameCounterX10 = 0;
        public float fullScreenAnimationFrameCounterY10 = 0;

        //public int maintainWeatherFromLastAreaTimer = 0;
        //public bool secondUpdateAfterTransition = false;
        //public bool blockTrigger = false;
        //public bool blockTriggerMovingProp = false;
        public bool doConvo = true;
        public int noTriggerLocX = -1;
        public int noTriggerLocY = -1;
        public bool firstTriggerCall = true;
        public bool isRecursiveCall = false;

        public int allyCounter = 0;

        public Coordinate alternativeEnd = new Coordinate();

        public Module()
        {

        }
        public void loadAreas(GameView gv)
        {
            foreach (string areaName in this.moduleAreasList)
            {
                try
                {
                    using (StreamReader file = File.OpenText(gv.mainDirectory + "\\modules\\" + this.moduleName + "\\areas\\" + areaName + ".lvl"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Area newArea = (Area)serializer.Deserialize(file, typeof(Area));
                        foreach (Prop p in newArea.Props)
                        {
                            p.initializeProp();
                        }
                        moduleAreasObjects.Add(newArea);
                    }
                }
                catch (Exception ex)
                {
                    gv.errorLog(ex.ToString());
                }
            }
        }
        public bool setCurrentArea(string filename, GameView gv)
        {
            try
            {
                foreach (Area area in this.moduleAreasObjects)
                {
                    if (area.Filename.Equals(filename))
                    {
                        this.currentArea = area;
                        if (!gv.mod.useAllTileSystem)
                        {
                            gv.cc.DisposeOfBitmap(ref gv.cc.bmpMap);
                            gv.cc.bmpMap = gv.cc.LoadBitmap(this.currentArea.ImageFileName);
                         
                            //TODO gv.cc.LoadTileBitmapList();
                        
                            foreach (Prop p in this.currentArea.Props)
                            {
                                gv.cc.DisposeOfBitmap(ref p.token);
                                p.token = gv.cc.LoadBitmap(p.ImageFileName);
                            }

                        }
                        
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
                return false;
            }

        }

        public int getNextIdNumber()
        {
            this.nextIdNumber++;
            return this.nextIdNumber;
        }
        public Player getPlayerByName(string tag)
        {
            foreach (Player pc in this.playerList)
            {
                if (string.Equals(tag, pc.name, StringComparison.CurrentCultureIgnoreCase))
                ///if (pc.name.Equals(tag))
                {
                    return pc;
                }
            }
            return null;
        }
        public Item getItemByTag(string tag)
        {
            foreach (Item it in this.moduleItemsList)
            {
                if (it.tag.Equals(tag)) return it;
            }
            return null;
        }
        public Item getItemByResRef(string resref)
        {
            foreach (Item it in this.moduleItemsList)
            {
                if (it.resref.Equals(resref)) return it;
            }
            return null;
        }
        public ItemRefs getItemRefsInInventoryByResRef(string resref)
        {
            foreach (ItemRefs itr in this.partyInventoryRefsList)
            {
                if (itr.resref.Equals(resref)) return itr;
            }
            return null;
        }
        public Item getItemByResRefForInfo(string resref)
        {
            foreach (Item it in this.moduleItemsList)
            {
                if (it.resref.Equals(resref)) return it;
            }
            return new Item();
        }
        public ItemRefs createItemRefsFromItem(Item it)
        {
            ItemRefs newIR = new ItemRefs();
            newIR.tag = it.tag + "_" + this.getNextIdNumber();
            newIR.name = it.name;
            newIR.resref = it.resref;
            newIR.canNotBeUnequipped = it.canNotBeUnequipped;
            newIR.quantity = it.quantity;
            newIR.isRation = it.isRation;
            newIR.isLightSource = it.isLightSource;
            newIR.hpRegenTimer = it.hpRegenTimer;
            newIR.spRegenTimer = it.spRegenTimer;
            return newIR;
        }
        public Container getContainerByTag(string tag)
        {
            foreach (Container it in this.moduleContainersList)
            {
                if (it.containerTag.Equals(tag)) return it;
            }
            return null;
        }
        public Shop getShopByTag(string tag)
        {
            foreach (Shop s in this.moduleShopsList)
            {
                if (s.shopTag.Equals(tag)) return s;
            }
            return null;
        }
        public Encounter getEncounter(string name)
        {
            foreach (Encounter e in this.moduleEncountersList)
            {
                if (e.encounterName.Equals(name)) return e;
            }
            return null;
        }
        public Creature getCreatureInCurrentEncounterByTag(string tag)
        {
            foreach (Creature crt in this.currentEncounter.encounterCreatureList)
            {
                if (crt.cr_tag.Equals(tag)) return crt;
            }
            return null;
        }
        public Spell getSpellByTag(string tag)
        {
            foreach (Spell s in this.moduleSpellsList)
            {
                if (s.tag.Equals(tag)) return s;
            }
            return null;
        }
        public Trait getTraitByTag(string tag)
        {
            foreach (Trait t in this.moduleTraitsList)
            {
                if (t.tag.Equals(tag)) return t;
            }
            return null;
        }
        public Weather getWeatherByTag(string tag)
        {
            foreach (Weather t in this.moduleWeathersList)
            {
                if (t.tag.Equals(tag)) return t;
            }
            return null;
        }
        public WeatherEffect getWeatherEffectByTag(string tag)
        {
            foreach (WeatherEffect t in this.moduleWeatherEffectsList)
            {
                if (t.tag.Equals(tag)) return t;
            }
            return null;
        }
        public WeatherEffect getWeatherEffectByName(string tag)
        {
            foreach (WeatherEffect t in this.moduleWeatherEffectsList)
            {
                if (t.name.Equals(tag)) return t;
            }
            return null;
        }
        public Effect getEffectByTag(string tag)
        {
            foreach (Effect ef in this.moduleEffectsList)
            {
                if (ef.tag.Equals(tag)) return ef;
            }
            return null;
        }
        public PlayerClass getPlayerClass(string tag)
        {
            foreach (PlayerClass p in this.modulePlayerClassList)
            {
                if (p.tag.Equals(tag)) return p;
            }
            return null;
        }
        public Race getRace(string tag)
        {
            foreach (Race r in this.moduleRacesList)
            {
                if (r.tag.Equals(tag)) return r;
            }
            return null;
        }
        public JournalQuest getJournalCategoryByTag(string tag)
        {
            foreach (JournalQuest it in this.moduleJournal)
            {
                if (it.Tag.Equals(tag)) return it;
            }
            return null;
        }
        public JournalQuest getPartyJournalActiveCategoryByTag(string tag)
        {
            foreach (JournalQuest it in this.partyJournalQuests)
            {
                if (it.Tag.Equals(tag)) return it;
            }
            return null;
        }
        public JournalQuest getPartyJournalCompletedCategoryByTag(string tag)
        {
            foreach (JournalQuest it in this.partyJournalCompleted)
            {
                if (it.Tag.Equals(tag)) return it;
            }
            return null;
        }
    }
}
