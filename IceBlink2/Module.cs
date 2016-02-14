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
        //public GameView gv;
        public string moduleName = "none";
        public string moduleLabelName = "none";
        public int moduleVersion = 1;
        public string saveName = "empty";
        public string uniqueSessionIdNumberTag = "";
        public string defaultPlayerFilename = "drin.json";
        public bool mustUsePreMadePC = false;
        public int numberOfPlayerMadePcsAllowed = 1;
        public int MaxPartySize = 6;
        public string moduleDescription = "";
        public string moduleCredits = "";
        public int nextIdNumber = 100;
        public int WorldTime = 0;
        public int TimePerRound = 6;
        public bool debugMode = false;
        public bool allowSave = true;
        public bool useLuck = false;
        public bool hideRoster = false;
        public bool use3d6 = false;
        public bool useUIBackground = false;
        public string fontName = "Metamorphous";
        public string fontFilename = "Metamorphous-Regular.ttf";
        public float fontD2DScaleMultiplier = 1.0f;
        public int logNumberOfLines = 20;
        public string spellLabelSingular = "Spell";
        public string spellLabelPlural = "Spells";
        public string goldLabelSingular = "Gold";
        public string goldLabelPlural = "Gold";
        public float diagonalMoveCost = 1.0f;
        public int nonAllowedDiagonalSquareX = -1;
        public int nonAllowedDiagonalSquareY = -1;
        public bool ArmorClassAscending = true;
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
        public List<string> moduleAreasList = new List<string>();
        public List<string> moduleConvosList = new List<string>();
        public List<Area> moduleAreasObjects = new List<Area>();
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
        public int combatAnimationSpeed = 100;
        public float soundVolume = 1.0f;
        //public string OnHeartBeatLogicTree = "none";
        //public string OnHeartBeatParms = "";
        public string OnHeartBeatIBScript = "none";
        public string OnHeartBeatIBScriptParms = "";
        public bool showInteractionState = false;
        public bool avoidInteraction = false;
        public bool useRealTimeTimer = false;
        public bool useSmoothMovement = false;
        public bool useCombatSmoothMovement = false;
        public bool useAllTileSystem = false;
        public int realTimeTimerLengthInMilliSeconds = 1500;
        public int attackFromBehindToHitModifier = 2;
        public int attackFromBehindDamageModifier = 0;
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

        public string currentWeatherName = "";
        public int currentWeatherDuration = 0;
        public string longEntryWeathersList = "";
        public string longExitWeathersList = "";
        public bool useFirstPartOfWeatherScript = true;
        public float howLongWeatherHasRun = 0;
        public float fullScreenEffectOpacityWeather = 1;

        public List<string> weatherSoundList = new List<string>();
        public string weatherSoundsName1 = "";
        public string weatherSoundsName2 = "";
        public string weatherSoundsName3 = "";

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

        public int maintainWeatherFromLastAreaTimer = 0;
        //public bool secondUpdateAfterTransition = false;
        //public bool blockTrigger = false;
        //public bool blockTriggerMovingProp = false;
        public bool doConvo = true;
        public int noTriggerLocX = -1;
        public int noTriggerLocY = -1;
        public bool firstTriggerCall = true;
        public bool isRecursiveCall = false;

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
        public void setCurrentArea(string filename, GameView gv)
        {
            try
            {
                foreach (Area area in this.moduleAreasObjects)
                {
                    if (area.Filename.Equals(filename))
                    {
                        this.currentArea = area;
                        gv.cc.DisposeOfBitmap(ref gv.cc.bmpMap);
                        gv.cc.bmpMap = gv.cc.LoadBitmap(this.currentArea.ImageFileName);
                        //TODO gv.cc.LoadTileBitmapList();
                        foreach (Prop p in this.currentArea.Props)
                        {
                            gv.cc.DisposeOfBitmap(ref p.token);
                            p.token = gv.cc.LoadBitmap(p.ImageFileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
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
                if (pc.name.Equals(tag))
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
