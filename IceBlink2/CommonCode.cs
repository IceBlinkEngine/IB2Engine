using Newtonsoft.Json;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using System;
//using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
//using System.Threading;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class CommonCode
    {
        //this class is handled differently than Android version
        public GameView gv;

        public float weatherSoundMultiplier = 2.7f;
        public bool blockSecondPropTriggersCall = false;
        public List<FloatyText> floatyTextList = new List<FloatyText>();
        public List<Tile> tilesOfThisLightSource = new List<Tile>();
        public int floatyTextCounter = 0;
        public bool floatyTextOn = false;

        public IbbPanel pnlLog = null;
        public IbbPanel pnlToggles = null;
        public IbbPanel pnlPortraits = null;
        public IbbPanel pnlHotkeys = null;
        public IbbPanel pnlArrows = null;
        public IbbButton btnReturn = null;
        public IbbButton ctrlUpArrow = null;
        public IbbButton ctrlDownArrow = null;
        public IbbButton ctrlLeftArrow = null;
        public IbbButton ctrlRightArrow = null;
        public IbbButton ctrlUpRightArrow = null;
        public IbbButton ctrlDownLeftArrow = null;
        public IbbButton ctrlUpLeftArrow = null;
        public IbbButton ctrlDownRightArrow = null;
        public IbbButton btnInventory = null;
        public IbbButton btnHelp = null;
        public IbbToggleButton tglSound = null;
        public IbbPortrait ptrPc0 = null;
        public IbbPortrait ptrPc1 = null;
        public IbbPortrait ptrPc2 = null;
        public IbbPortrait ptrPc3 = null;
        public IbbPortrait ptrPc4 = null;
        public IbbPortrait ptrPc5 = null;

        public int partyScreenPcIndex = 0;
        public int partyItemSlotIndex = 0;
        public string currentMusic = "";
        public string slotA = "Autosave";
        public string slot0 = "Quicksave";
        public string slot1 = "";
        public string slot2 = "";
        public string slot3 = "";
        public string slot4 = "";
        public string slot5 = "";

        public Bitmap title;
        public Bitmap bmpMap;
        public Bitmap walkPass;
        public Bitmap btnIni;
        public Bitmap btnIniGlow;
        public Bitmap walkBlocked;
        public Bitmap losBlocked;
        public Bitmap hitSymbol;
        public Bitmap missSymbol;
        public Bitmap highlight_green;
        public Bitmap highlight_red;
        public Bitmap black_tile;
        public Bitmap black_tile_NE;
        public Bitmap black_tile_NW;
        public Bitmap black_tile_SE;
        public Bitmap black_tile_SW;

        public Bitmap black_tile2;
        public Bitmap turn_marker;
        public Bitmap pc_dead;
        public Bitmap pc_stealth;
        public Bitmap offScreen;
        public Bitmap offScreen5;
        public Bitmap offScreen6;
        public Bitmap offScreen7;
        public Bitmap black_tile4;
        public Bitmap black_tile5;
        public Bitmap offScreenTrans;
        public Bitmap death_fx;
        public Bitmap tint_dawn;
        public Bitmap tint_sunrise;
        public Bitmap tint_sunset;
        public Bitmap tint_dusk;
        public Bitmap tint_night;
        public Bitmap night_tile_NW;
        public Bitmap night_tile_NE;
        public Bitmap night_tile_SW;
        public Bitmap night_tile_SE;

        public Bitmap light_torch;
        public Bitmap prp_lightYellow;
        public Bitmap prp_lightGreen;
        public Bitmap prp_lightRed;
        public Bitmap light_torchOLD;
        public Bitmap ui_bg_fullscreen;
        public Bitmap ui_portrait_frame;
        public Bitmap facing1;
        public Bitmap facing2;
        public Bitmap facing3;
        public Bitmap facing4;
        public Bitmap facing6;
        public Bitmap facing7;
        public Bitmap facing8;
        public Bitmap facing9;
        //off for now
        //public Bitmap tint_rain;

        public Dictionary<string, Bitmap> tileBitmapList = new Dictionary<string, Bitmap>();
        public Dictionary<string, Bitmap> commonBitmapList = new Dictionary<string, Bitmap>();
        public Dictionary<string, System.Drawing.Bitmap> tileGDIBitmapList = new Dictionary<string, System.Drawing.Bitmap>();

        public Spell currentSelectedSpell = new Spell();
        public string floatyText = "";
        public string floatyText2 = "";
        public string floatyText3 = "";
        public Coordinate floatyTextLoc = new Coordinate();
        public int creatureIndex = 0;
        public bool calledConvoFromProp = false;
        public bool calledEncounterFromProp = false;
        public int currentPlayerIndexUsingItem = 0;

        public string stringBeginnersGuide = "";
        public string stringPlayersGuide = "";
        public string stringPcCreation = "";
        public string stringMessageCombat = "";
        public string stringMessageInventory = "";
        public string stringMessageParty = "";
        public string stringMessageMainMap = "";

        public bool doOnEnterAreaUpdate = false;
        public bool recursiveCall = false;
        int fallBackSquareX = 0;
        int fallBackSquareY = 0;

        public CommonCode(GameView g)
        {
            gv = g;
        }

        //LOAD FILES
        public void LoadTestParty()
        {
            gv.sf.AddCharacterToParty(gv.mod.defaultPlayerFilename); //drin.json is default
            gv.mod.partyTokenFilename = "prp_party";
            gv.mod.partyTokenBitmap = this.LoadBitmap(gv.mod.partyTokenFilename);
        }
        public Player LoadPlayer(string filename)
        {
            Player toReturn = null;

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\" + filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                toReturn = (Player)serializer.Deserialize(file, typeof(Player));
            }
            return toReturn;
        }
        public void LoadCurrentConvo(string filename)
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\dialog\\" + filename + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.screenConvo.currentConvo = (Convo)serializer.Deserialize(file, typeof(Convo));
            }
        }
        public void AutoSave()
        {
            string filename = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\autosave.json";
            MakeDirectoryIfDoesntExist(filename);
            string json = JsonConvert.SerializeObject(gv.mod, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(json.ToString());
            }
        }
        public void QuickSave()
        {
            string filename = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\quicksave.json";
            MakeDirectoryIfDoesntExist(filename);
            string json = JsonConvert.SerializeObject(gv.mod, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(json.ToString());
            }
        }
        public void SaveGame(string filename)
        {
            string filepath = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename;
            MakeDirectoryIfDoesntExist(filepath);
            string json = JsonConvert.SerializeObject(gv.mod, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.Write(json.ToString());
            }
        }
        public void SaveGameInfo(string filename)
        {
            ModuleInfo newModInfo = new ModuleInfo();
            newModInfo.saveName = gv.mod.saveName;

            string filepath = gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename;
            MakeDirectoryIfDoesntExist(filepath);
            string json = JsonConvert.SerializeObject(newModInfo, Newtonsoft.Json.Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.Write(json.ToString());
            }
        }
        public void doSavesDialog()
        {
            List<string> saveList = new List<string> { slot0, slot1, slot2, slot3, slot4, slot5 };

            using (ItemListSelector itSel = new ItemListSelector(gv, saveList, "Choose a slot to save game."))
            {
                itSel.IceBlinkButtonClose.Enabled = true;
                itSel.IceBlinkButtonClose.Visible = true;
                itSel.setupAll(gv);
                var ret = itSel.ShowDialog();

                if (itSel.selectedIndex == 0)
                {
                    try
                    {
                        QuickSave();
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
                else if (itSel.selectedIndex == 1)
                {
                    Player pc = gv.mod.playerList[0];
                    gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                    slot1 = gv.mod.saveName;
                    try
                    {
                        SaveGame("slot1.json");
                        SaveGameInfo("slot1info.json");
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
                else if (itSel.selectedIndex == 2)
                {
                    Player pc = gv.mod.playerList[0];
                    gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                    slot2 = gv.mod.saveName;
                    try
                    {
                        SaveGame("slot2.json");
                        SaveGameInfo("slot2info.json");
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
                else if (itSel.selectedIndex == 3)
                {
                    Player pc = gv.mod.playerList[0];
                    gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                    slot3 = gv.mod.saveName;
                    try
                    {
                        SaveGame("slot3.json");
                        SaveGameInfo("slot3info.json");
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
                else if (itSel.selectedIndex == 4)
                {
                    Player pc = gv.mod.playerList[0];
                    gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                    slot4 = gv.mod.saveName;
                    try
                    {
                        SaveGame("slot4.json");
                        SaveGameInfo("slot4info.json");
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
                else if (itSel.selectedIndex == 5)
                {
                    Player pc = gv.mod.playerList[0];
                    gv.mod.saveName = pc.name + ", Level:" + pc.classLevel + ", XP:" + pc.XP + ", WorldTime:" + gv.mod.WorldTime;
                    slot5 = gv.mod.saveName;
                    try
                    {
                        SaveGame("slot5.json");
                        SaveGameInfo("slot5info.json");
                    }
                    catch (Exception ex)
                    {
                        gv.sf.MessageBox("Failed to Save: Not enough free memory(RAM) on device, try and free up some memory and try again.");
                        gv.errorLog(ex.ToString());
                    }
                }
            }
        }
        public void doLoadSaveGameDialog()
        {
            List<string> saveList = new List<string> { slotA, slot0, slot1, slot2, slot3, slot4, slot5 };

            using (ItemListSelector itSel = new ItemListSelector(gv, saveList, "Choose a Saved Game to Load."))
            {
                itSel.IceBlinkButtonClose.Visible = true;
                itSel.IceBlinkButtonClose.Enabled = true;
                itSel.ShowDialog();

                if (itSel.selectedIndex == 0)
                {
                    bool result = LoadSave("autosave.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 1)
                {
                    bool result = LoadSave("quicksave.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 2)
                {
                    bool result = LoadSave("slot1.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 3)
                {
                    bool result = LoadSave("slot2.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 4)
                {
                    bool result = LoadSave("slot3.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 5)
                {
                    bool result = LoadSave("slot4.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
                else if (itSel.selectedIndex == 6)
                {
                    bool result = LoadSave("slot5.json");
                    if (result)
                    {
                        gv.screenType = "main";
                        doUpdate();
                    }
                    else
                    {
                        //Toast.makeText(gv.gameContext, "Save file not found", Toast.LENGTH_SHORT).show();
                    }
                }
            }
        }
        public ModuleInfo LoadModuleInfo(string filename)
        {
            ModuleInfo m = new ModuleInfo();
            try
            {
                using (StreamReader file = File.OpenText(gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    m = (ModuleInfo)serializer.Deserialize(file, typeof(ModuleInfo));
                }
            }
            catch { }
            return m;
        }
        public void LoadSaveListItems()
        {
            slot1 = LoadModuleInfo("slot1info.json").saveName;
            slot2 = LoadModuleInfo("slot2info.json").saveName;
            slot3 = LoadModuleInfo("slot3info.json").saveName;
            slot4 = LoadModuleInfo("slot4info.json").saveName;
            slot5 = LoadModuleInfo("slot5info.json").saveName;
        }

        public bool LoadSave(string filename)
        {
            //  load a new module (actually already have a new module at this point from launch screen		
            //  load the saved game module
            Module saveMod = LoadSaveGameModule(filename);
            if (saveMod == null) { return false; }
            //  replace parts of new module with parts of saved game module
            //
            // U = update from save file	 
            //
            //module file
            //{
            //U  "saveName": "Drin, Level:1, XP:150, WorldTime:24", (use all save)
            gv.mod.saveName = saveMod.saveName;
            //U  "playerList": [], (use all save)  Update PCs later further down
            gv.mod.playerList = new List<Player>();
            foreach (Player pc in saveMod.playerList)
            {
                gv.mod.playerList.Add(pc.DeepCopy());
            }
            setMainPc();
            //U  "partyRosterList": [], (use all save)  Update PCs later further down
            gv.mod.partyRosterList = new List<Player>();
            foreach (Player pc in saveMod.partyRosterList)
            {
                gv.mod.partyRosterList.Add(pc.DeepCopy());
            }
            //U  "partyJournalQuests": [], (use tags from save to get all from new)
            gv.mod.partyJournalQuests.Clear();
            foreach (JournalQuest jq in saveMod.partyJournalQuests)
            {
                foreach (JournalEntry je in jq.Entries)
                {
                    gv.sf.AddJournalEntryNoMessages(jq.Tag, je.Tag);
                }
            }
            //U  "partyJournalNotes": "", (use all save)
            gv.mod.partyJournalNotes = saveMod.partyJournalNotes;
            //U  "partyJournalCompleted": [], (use tags from save to get all from new)
            // NOT CURRENTLY USED
            //U  "partyInventoryTagList": [], (use all save) update Items later on down
            gv.mod.partyInventoryRefsList.Clear();
            foreach (ItemRefs s in saveMod.partyInventoryRefsList)
            {
                gv.mod.partyInventoryRefsList.Add(s.DeepCopy());
            }
            //U  "moduleShopsList": [], (have an original shop items tags list and the current tags list to see what to add or delete from the save tags list)
            this.updateShops(saveMod);
            //  "moduleName": "Lanterna2", Don't need to update
            //  "moduleAreasList": [], Don't need to update
            //U  "moduleAreasObjects": [],
            //                (triggers: use save trigger "enabled" value to update new)
            //                (tiles: use save "visible" to update new)
            //                (props: have an original props tags list and the current tags list to see what to add or delete from the save tags list)		               
            this.updateAreas(saveMod);
            //
            //U  "currentArea": {},
            gv.mod.setCurrentArea(saveMod.currentArea.Filename, gv);
            //U  "moduleContainersList": [], (have an original containers items tags list and the current tags list to see what to add or delete from the save tags list)
            this.updateContainers(saveMod);
            //U  "moduleConvoSavedValuesList": [], (use all save)
            gv.mod.moduleConvoSavedValuesList.Clear();
            foreach (ConvoSavedValues csv in saveMod.moduleConvoSavedValuesList)
            {
                gv.mod.moduleConvoSavedValuesList.Add(csv.DeepCopy());
            }
            //  "moduleConvosList": [], Don't need to update
            //U  "moduleEncountersList": [], (use new except delete those completed already in save)
            foreach (Encounter enc in saveMod.moduleEncountersList)
            {
                if (enc.encounterCreatureRefsList.Count <= 0)
                {
                    //if the encounter was completed in the saveMod then clear all creatures in the newMod
                    Encounter e = gv.mod.getEncounter(enc.encounterName);
                    e.encounterCreatureList.Clear();
                    e.encounterCreatureRefsList.Clear();
                }
            }
            //U  "moduleGlobalInts": [], (use all save)
            gv.mod.moduleGlobalInts.Clear();
            foreach (GlobalInt g in saveMod.moduleGlobalInts)
            {
                gv.mod.moduleGlobalInts.Add(g.DeepCopy());
            }
            //U  "moduleGlobalStrings": [], (use all save)
            gv.mod.moduleGlobalStrings.Clear();
            foreach (GlobalString g in saveMod.moduleGlobalStrings)
            {
                gv.mod.moduleGlobalStrings.Add(g.DeepCopy());
            }
            //  "moduleLabelName": "Lanterna 2 (demo)", don't need to update
            //U  "combatAnimationSpeed": 100, (use all save)
            gv.mod.combatAnimationSpeed = saveMod.combatAnimationSpeed;
            //gv.mod.combatAnimationSpeed = 100;

            //U  "partyGold": 70, (use all save)
            gv.mod.partyGold = saveMod.partyGold;
            //U  "com_showGrid": false, (use all save)
            gv.mod.com_showGrid = saveMod.com_showGrid;
            gv.mod.map_showGrid = saveMod.map_showGrid;
            /*gv.mod.sendProgressReport = saveMod.sendProgressReport;
            if (saveMod.uniqueSessionIdNumberTag.Equals(""))
            {
                gv.mod.uniqueSessionIdNumberTag = gv.sf.RandInt(1000000) + "";
            }
            else
            {
                gv.mod.uniqueSessionIdNumberTag = saveMod.uniqueSessionIdNumberTag;
            }*/
            //U  "allowAutosave": true, (use all save)
            gv.mod.allowAutosave = saveMod.allowAutosave;
            //U  "WorldTime": 24, (use all save)
            gv.mod.WorldTime = saveMod.WorldTime;
            gv.mod.nextIdNumber = saveMod.nextIdNumber;
            //U  "PlayerLocationY": 2, (use all save)
            gv.mod.PlayerLocationY = saveMod.PlayerLocationY;
            //U  "PlayerLocationX": 1, (use all save)
            gv.mod.PlayerLocationX = saveMod.PlayerLocationX;
            //U  "playButtonHaptic": false, (use all save)
            gv.mod.playButtonHaptic = saveMod.playButtonHaptic;
            //U  "playButtonSounds": false, (use all save)
            gv.mod.playButtonSounds = saveMod.playButtonSounds;
            //U  "playMusic": false, (use all save)
            gv.mod.playMusic = saveMod.playMusic;
            //U  "playSoundFx": false, (use all save)
            gv.mod.playSoundFx = saveMod.playSoundFx;
            //U  "PlayerLastLocationY": 1, (use all save)
            gv.mod.PlayerLastLocationY = saveMod.PlayerLastLocationY;
            //U  "PlayerLastLocationX": 2, (use all save)
            gv.mod.PlayerLastLocationX = saveMod.PlayerLastLocationX;
            //U  "selectedPartyLeader": 0, (use all save)
            gv.mod.selectedPartyLeader = saveMod.selectedPartyLeader;
            //U  "showAutosaveMessage": true, (use all save)
            gv.mod.showAutosaveMessage = saveMod.showAutosaveMessage;
            //U  "showPartyToken": false, (use all save)
            gv.mod.showPartyToken = saveMod.showPartyToken;
            //U  "showTutorialCombat": true, (use all save)
            gv.mod.showTutorialCombat = saveMod.showTutorialCombat;
            //U  "showTutorialInventory": true, (use all save)
            gv.mod.showTutorialInventory = saveMod.showTutorialInventory;
            //U  "showTutorialParty": true, (use all save)
            gv.mod.showTutorialParty = saveMod.showTutorialParty;
            //U  "soundVolume": 1.0, (use all save)
            gv.mod.soundVolume = saveMod.soundVolume;
            //U  "startingPlayerPositionX": 4, (use all save)
            gv.mod.startingPlayerPositionX = saveMod.startingPlayerPositionX;
            //U  "startingPlayerPositionY": 1 (use all save)
            gv.mod.startingPlayerPositionY = saveMod.startingPlayerPositionY;
            //gv.mod.OnHeartBeatLogicTree = saveMod.OnHeartBeatLogicTree;
            //gv.mod.OnHeartBeatParms = saveMod.OnHeartBeatParms;
            gv.mod.OnHeartBeatIBScript = saveMod.OnHeartBeatIBScript;
            gv.mod.OnHeartBeatIBScriptParms = saveMod.OnHeartBeatIBScriptParms;
            //}

            LoadRaces();
            LoadPlayerClasses();
            LoadItems();
            //no load of containers
            //no load of shops
            LoadEffects();
            LoadSpells();
            LoadTraits();
            LoadWeathers();
            LoadWeatherEffects();
            LoadCreatures();
            //no load of encounters
            LoadJournal();
            //LoadTileBitmapList();
            gv.initializeSounds();

            gv.mod.partyTokenFilename = "prp_party";
            gv.mod.partyTokenBitmap = this.LoadBitmap(gv.mod.partyTokenFilename);

            this.updatePlayers();
            this.updatePartyRosterPlayers();

            gv.createScreens();
            gv.screenMainMap.resetMiniMapBitmap();
            return true;
        }
        public Module LoadSaveGameModule(string filename)
        {
            Module toReturn = null;

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(gv.mainDirectory + "\\saves\\" + gv.mod.moduleName + "\\" + filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                toReturn = (Module)serializer.Deserialize(file, typeof(Module));
            }
            return toReturn;
        }
        public void updateContainers(Module saveMod)
        {
            foreach (Container saveCnt in saveMod.moduleContainersList)
            {
                //fill container with items that are still in the saved game 
                Container updatedCont = gv.mod.getContainerByTag(saveCnt.containerTag);
                if (updatedCont != null)
                {
                    //this container in the save also exists in the newMod so clear it out and add everything in the save
                    updatedCont.containerItemRefs.Clear();
                    foreach (ItemRefs it in saveCnt.containerItemRefs)
                    {
                        //check to see if item resref in save game container still exists in toolset
                        Item newItem = gv.mod.getItemByResRef(it.resref);
                        if (newItem != null)
                        {
                            updatedCont.containerItemRefs.Add(it.DeepCopy());
                        }
                    }
                    //compare lists and add items that are new
                    foreach (ItemRefs itemRef in updatedCont.initialContainerItemRefs)
                    {
                        //check to see if item in toolset does not exist in save initial list so it is new and add it
                        if (!saveCnt.containsInitialItemWithResRef(itemRef.resref))
                        {
                            //item is not in the saved game initial container item list so add it to the container
                            //check to see if item resref in save game container still exists in toolset
                            Item newItem = gv.mod.getItemByResRef(itemRef.resref);
                            if (newItem != null)
                            {
                                updatedCont.containerItemRefs.Add(itemRef.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
        public void updateShops(Module saveMod)
        {
            foreach (Shop saveShp in saveMod.moduleShopsList)
            {
                Shop updatedShop = gv.mod.getShopByTag(saveShp.shopTag);
                if (updatedShop != null)
                {
                    //this shop in the save also exists in the newMod so clear it out and add everything in the save
                    updatedShop.shopItemRefs.Clear();
                    foreach (ItemRefs it in saveShp.shopItemRefs)
                    {
                        Item newItem = gv.mod.getItemByResRef(it.resref);
                        if (newItem != null)
                        {
                            updatedShop.shopItemRefs.Add(it.DeepCopy());
                        }
                    }
                    //compare lists and add items that are new
                    foreach (ItemRefs itemRef in updatedShop.initialShopItemRefs)
                    {
                        if (!saveShp.containsInitialItemWithResRef(itemRef.resref))
                        {
                            //item is not in the saved game initial container item list so add it to the container
                            Item newItem = gv.mod.getItemByResRef(itemRef.resref);
                            if (newItem != null)
                            {
                                updatedShop.shopItemRefs.Add(itemRef.DeepCopy());
                            }
                        }
                    }
                }
            }
        }
        public void updateAreas(Module saveMod)
        {
            foreach (Area ar in gv.mod.moduleAreasObjects)
            {
                foreach (Area sar in saveMod.moduleAreasObjects)
                {
                    if (sar.Filename.Equals(ar.Filename)) //sar is saved game, ar is new game from toolset version
                    {
                        //locals
                        ar.AreaLocalInts.Clear();
                        foreach (LocalInt l in sar.AreaLocalInts)
                        {
                            ar.AreaLocalInts.Add(l.DeepCopy());
                        }
                        ar.AreaLocalStrings.Clear();
                        foreach (LocalString l in sar.AreaLocalStrings)
                        {
                            ar.AreaLocalStrings.Add(l.DeepCopy());
                        }

                        //tiles
                        for (int index = 0; index < ar.Tiles.Count; index++)
                        {
                            ar.Tiles[index].Visible = sar.Tiles[index].Visible;
                        }

                        //props
                        //start at the end of the newMod prop list and work up
                        //if the prop tag is found in the save game, update it
                        //else if not found in saved game, but exists in the 
                        //saved game initial list (the toolset version of the prop list), remove prop
                        //else leave it alone
                        for (int index = ar.Props.Count - 1; index >= 0; index--)
                        {
                            Prop prp = ar.Props[index];
                            bool foundOne = false;
                            foreach (Prop sprp in sar.Props) //sprp is the saved game prop
                            {
                                if (prp.PropTag.Equals(sprp.PropTag))
                                {
                                    foundOne = true; //the prop tag exists in the saved game
                                    //replace the one in the toolset with the one from the saved game
                                    ar.Props.RemoveAt(index);
                                    ar.Props.Add(sprp.DeepCopy());
                                    //prp = sprp.DeepCopy();
                                    break;
                                }
                            }
                            if (!foundOne) //didn't find the prop tag in the saved game
                            {
                                if (sar.InitialAreaPropTagsList.Contains(prp.PropTag))
                                {
                                    //was once on the map, but was deleted so remove from the newMod prop list
                                    ar.Props.RemoveAt(index);
                                }
                                else
                                {
                                    //is new to the mod so leave it alone, don't remove from the prop list
                                }
                            }
                        }
                        //triggers
                        foreach (Trigger tr in ar.Triggers)
                        {
                            foreach (Trigger str in sar.Triggers)
                            {
                                if (tr.TriggerTag.Equals(str.TriggerTag))
                                {
                                    tr.Enabled = str.Enabled;
                                    tr.EnabledEvent1 = str.EnabledEvent1;
                                    tr.EnabledEvent2 = str.EnabledEvent2;
                                    tr.EnabledEvent3 = str.EnabledEvent3;
                                    //may want to copy the trigger's squares list from the save game if builders can modify the list with scripts
                                }
                            }
                        }
                    }
                }
            }
        }
        public void updatePlayers()
        {
            //load player Bitmap, race, class, known spells, equipped items
            foreach (Player pc in gv.mod.playerList)
            {
                try { pc.token = LoadBitmap(pc.tokenFilename); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.portrait = LoadBitmap(pc.portraitFilename); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.race = gv.mod.getRace(pc.raceTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.playerClass = gv.mod.getPlayerClass(pc.classTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                //may not need this(deleted already) as it is not used anywhere, only knownspellstags is used
            }
        }
        public void updatePartyRosterPlayers()
        {
            //load player Bitmap, race, class, known spells, equipped items
            foreach (Player pc in gv.mod.partyRosterList)
            {
                try { pc.token = LoadBitmap(pc.tokenFilename); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.portrait = LoadBitmap(pc.portraitFilename); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.race = gv.mod.getRace(pc.raceTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                try { pc.playerClass = gv.mod.getPlayerClass(pc.classTag).DeepCopy(); }
                catch (Exception ex) { gv.errorLog(ex.ToString()); }
                //may not need this as it is not used anywhere, only knownspellstags is used
            }
        }
        public void setMainPc()
        {
            foreach (Player pc in gv.mod.playerList)
            {
                if (pc.mainPc)
                {
                    return;
                }
            }
            if (gv.mod.playerList.Count > 0)
            {
                gv.mod.playerList[0].mainPc = true;
            }
        }
        public Module LoadModule(string folderAndFilename, bool fullPath)
        {
            Module toReturn = null;
            if (fullPath)
            {
                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(folderAndFilename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    toReturn = (Module)serializer.Deserialize(file, typeof(Module));
                }
            }
            else
            {
                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(GetModulePath() + "\\" + folderAndFilename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    toReturn = (Module)serializer.Deserialize(file, typeof(Module));
                }
            }
            return toReturn;
        }
        public void LoadRaces()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\races.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleRacesList = (List<Race>)serializer.Deserialize(file, typeof(List<Race>));
            }
        }
        public void LoadPlayerClasses()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\playerClasses.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.modulePlayerClassList = (List<PlayerClass>)serializer.Deserialize(file, typeof(List<PlayerClass>));
            }
        }
        public void LoadItems()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\items.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleItemsList = (List<Item>)serializer.Deserialize(file, typeof(List<Item>));
            }
        }
        public void LoadContainers()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\containers.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleContainersList = (List<Container>)serializer.Deserialize(file, typeof(List<Container>));
            }
        }
        public void LoadShops()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\shops.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleShopsList = (List<Shop>)serializer.Deserialize(file, typeof(List<Shop>));
            }
        }
        public void LoadJournal()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\journal.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleJournal = (List<JournalQuest>)serializer.Deserialize(file, typeof(List<JournalQuest>));
            }
        }
        public void LoadEffects()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\effects.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleEffectsList = (List<Effect>)serializer.Deserialize(file, typeof(List<Effect>));
            }
        }
        public void LoadSpells()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\spells.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleSpellsList = (List<Spell>)serializer.Deserialize(file, typeof(List<Spell>));
            }
        }
        public void LoadTraits()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\traits.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleTraitsList = (List<Trait>)serializer.Deserialize(file, typeof(List<Trait>));
            }
        }

        public void LoadWeathers()
        {
            try
            {
                using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\weathers.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    gv.mod.moduleWeathersList = (List<Weather>)serializer.Deserialize(file, typeof(List<Weather>));
                    int i = gv.mod.moduleWeathersList.Count;
                    int j = 2;
                }
            }
            catch
            {
                gv.mod.moduleWeathersList = new List<Weather>();
            }
        }

        public void LoadWeatherEffects()
        {
            try
            {
                using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\weatherEffects.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    gv.mod.moduleWeatherEffectsList = (List<WeatherEffect>)serializer.Deserialize(file, typeof(List<WeatherEffect>));
                }
            }
            catch
            {
                gv.mod.moduleWeatherEffectsList = new List<WeatherEffect>();
            }
        }

        public void LoadCreatures()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\creatures.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleCreaturesList = (List<Creature>)serializer.Deserialize(file, typeof(List<Creature>));
            }
        }
        public void LoadEncounters()
        {
            using (StreamReader file = File.OpenText(GetModulePath() + "\\data\\encounters.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                gv.mod.moduleEncountersList = (List<Encounter>)serializer.Deserialize(file, typeof(List<Encounter>));
            }
        }

        public string GetModulePath()
        {
            return gv.mainDirectory + "\\modules\\" + gv.mod.moduleName;
        }

        //GENERAL
        public void nullOutControls()
        {
            btnReturn = null;
            ctrlUpArrow = null;
            ctrlDownArrow = null;
            ctrlLeftArrow = null;
            ctrlRightArrow = null;
            ctrlUpRightArrow = null;
            ctrlDownLeftArrow = null;
            ctrlUpLeftArrow = null;
            ctrlDownRightArrow = null;
            btnInventory = null;
            btnHelp = null;
            tglSound = null;
        }        
        public void setPanelsStart()
        {
            int mapSize = gv.playerOffsetY + gv.playerOffsetY + 1;
            int topPanelHeight = mapSize - 2;
            int leftPanelWidth = gv.squaresInWidth - mapSize - 4;

            if (pnlLog == null)
            {
                pnlLog = new IbbPanel(gv);
                pnlLog.ImgBG = this.LoadBitmap("ui_bg_log");
                pnlLog.LocX = gv.oXshift - gv.pS;
                pnlLog.LocY = 0;
                pnlLog.Height = topPanelHeight * gv.squareSize;
                pnlLog.Width = leftPanelWidth * gv.squareSize;
                gv.log.tbWidth = leftPanelWidth * gv.squareSize - gv.pS - gv.pS;
                gv.log.tbHeight = topPanelHeight * gv.squareSize - gv.pS - gv.pS;
            }
            if (pnlToggles == null)
            {
                pnlToggles = new IbbPanel(gv);
                pnlToggles.ImgBG = this.LoadBitmap("ui_bg_toggles");
                pnlToggles.LocX = gv.oXshift - gv.pS;
                pnlToggles.LocY = topPanelHeight * gv.squareSize + gv.pS;
                pnlToggles.Height = 3 * gv.squareSize + gv.pS + gv.pS;
                pnlToggles.Width = leftPanelWidth * gv.squareSize;
            }
            if (pnlPortraits == null)
            {
                pnlPortraits = new IbbPanel(gv);
                pnlPortraits.ImgBG = this.LoadBitmap("ui_bg_portraits");
                pnlPortraits.LocX = (leftPanelWidth + mapSize) * gv.squareSize + gv.oXshift + gv.pS;
                pnlPortraits.LocY = 0 * gv.squareSize;
                pnlPortraits.Height = topPanelHeight * gv.squareSize;
                pnlPortraits.Width = 4 * gv.squareSize;
            }
            if (pnlArrows == null)
            {
                pnlArrows = new IbbPanel(gv);
                pnlArrows.ImgBG = this.LoadBitmap("ui_bg_arrows");
                pnlArrows.LocX = (leftPanelWidth + mapSize) * gv.squareSize + gv.oXshift + gv.pS;
                pnlArrows.LocY = topPanelHeight * gv.squareSize + gv.pS;
                pnlArrows.Height = 3 * gv.squareSize + gv.pS + gv.pS;
                pnlArrows.Width = 4 * gv.squareSize;
            }
            if (pnlHotkeys == null)
            {
                pnlHotkeys = new IbbPanel(gv);
                pnlHotkeys.ImgBG = this.LoadBitmap("ui_bg_hotkeys");
                pnlHotkeys.LocX = leftPanelWidth * gv.squareSize + gv.oXshift;
                pnlHotkeys.LocY = mapSize * gv.squareSize + gv.pS;
                pnlHotkeys.Height = 1 * gv.squareSize + gv.pS + gv.pS;
                pnlHotkeys.Width = mapSize * gv.squareSize;
            }
        }
        public void setControlsStart()
        {
            int pH = (int)((float)gv.screenHeight / 100.0f);
            int padW = gv.squareSize / 6;
            int hotkeyShift = 0;
            if (gv.useLargeLayout)
            {
                hotkeyShift = 1;
            }

            if (ctrlUpArrow == null)
            {
                ctrlUpArrow = new IbbButton(gv, 1.0f);
                ctrlUpArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlUpArrow.Img2 = this.LoadBitmap("ctrl_up_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_up_arrow);
                ctrlUpArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlUpArrow.X = gv.cc.pnlArrows.LocX + 1 * gv.squareSize + gv.squareSize / 2;
                ctrlUpArrow.Y = gv.cc.pnlArrows.LocY + 0 * gv.squareSize + gv.pS;
                ctrlUpArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlUpArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlLeftArrow == null)
            {
                ctrlLeftArrow = new IbbButton(gv, 1.0f);
                ctrlLeftArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlLeftArrow.Img2 = this.LoadBitmap("ctrl_left_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_left_arrow);
                ctrlLeftArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlLeftArrow.X = gv.cc.pnlArrows.LocX + 0 * gv.squareSize + gv.squareSize / 2;
                ctrlLeftArrow.Y = gv.cc.pnlArrows.LocY + 1 * gv.squareSize + gv.pS;
                ctrlLeftArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlLeftArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlRightArrow == null)
            {
                ctrlRightArrow = new IbbButton(gv, 1.0f);
                ctrlRightArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlRightArrow.Img2 = this.LoadBitmap("ctrl_right_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_right_arrow);
                ctrlRightArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlRightArrow.X = gv.cc.pnlArrows.LocX + 2 * gv.squareSize + gv.squareSize / 2;
                ctrlRightArrow.Y = gv.cc.pnlArrows.LocY + 1 * gv.squareSize + gv.pS;
                ctrlRightArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlRightArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlDownArrow == null)
            {
                ctrlDownArrow = new IbbButton(gv, 1.0f);
                ctrlDownArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlDownArrow.Img2 = this.LoadBitmap("ctrl_down_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_down_arrow);
                ctrlDownArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlDownArrow.X = gv.cc.pnlArrows.LocX + 1 * gv.squareSize + gv.squareSize / 2;
                ctrlDownArrow.Y = gv.cc.pnlArrows.LocY + 2 * gv.squareSize + gv.pS;
                ctrlDownArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlDownArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlUpRightArrow == null)
            {
                ctrlUpRightArrow = new IbbButton(gv, 1.0f);
                ctrlUpRightArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlUpRightArrow.Img2 = this.LoadBitmap("ctrl_up_right_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_up_right_arrow);
                ctrlUpRightArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlUpRightArrow.X = gv.cc.pnlArrows.LocX + 2 * gv.squareSize + gv.squareSize / 2;
                ctrlUpRightArrow.Y = gv.cc.pnlArrows.LocY + 0 * gv.squareSize + gv.pS;
                ctrlUpRightArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlUpRightArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlUpLeftArrow == null)
            {
                ctrlUpLeftArrow = new IbbButton(gv, 1.0f);
                ctrlUpLeftArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlUpLeftArrow.Img2 = this.LoadBitmap("ctrl_up_left_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_up_left_arrow);
                ctrlUpLeftArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlUpLeftArrow.X = gv.cc.pnlArrows.LocX + 0 * gv.squareSize + gv.squareSize / 2;
                ctrlUpLeftArrow.Y = gv.cc.pnlArrows.LocY + 0 * gv.squareSize + gv.pS;
                ctrlUpLeftArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlUpLeftArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlDownRightArrow == null)
            {
                ctrlDownRightArrow = new IbbButton(gv, 1.0f);
                ctrlDownRightArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlDownRightArrow.Img2 = this.LoadBitmap("ctrl_down_right_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_down_right_arrow);
                ctrlDownRightArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlDownRightArrow.X = gv.cc.pnlArrows.LocX + 2 * gv.squareSize + gv.squareSize / 2;
                ctrlDownRightArrow.Y = gv.cc.pnlArrows.LocY + 2 * gv.squareSize + gv.pS;
                ctrlDownRightArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlDownRightArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (ctrlDownLeftArrow == null)
            {
                ctrlDownLeftArrow = new IbbButton(gv, 1.0f);
                ctrlDownLeftArrow.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                ctrlDownLeftArrow.Img2 = this.LoadBitmap("ctrl_down_left_arrow"); // BitmapFactory.decodeResource(getResources(), R.drawable.ctrl_down_left_arrow);
                ctrlDownLeftArrow.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.arrow_glow);
                ctrlDownLeftArrow.X = gv.cc.pnlArrows.LocX + 0 * gv.squareSize + gv.squareSize / 2;
                ctrlDownLeftArrow.Y = gv.cc.pnlArrows.LocY + 2 * gv.squareSize + gv.pS;
                ctrlDownLeftArrow.Height = (int)(gv.ibbheight * gv.screenDensity);
                ctrlDownLeftArrow.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnInventory == null)
            {
                btnInventory = new IbbButton(gv, 0.8f);
                btnInventory.HotKey = "I";
                btnInventory.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnInventory.Img2 = this.LoadBitmap("btninventory"); // BitmapFactory.decodeResource(getResources(), R.drawable.btninventory);
                btnInventory.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnInventory.X = gv.cc.pnlHotkeys.LocX + (hotkeyShift + 1) * gv.squareSize;
                btnInventory.Y = gv.cc.pnlHotkeys.LocY + 0 * gv.squareSize + gv.pS;
                btnInventory.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnInventory.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnHelp == null)
            {
                btnHelp = new IbbButton(gv, 0.8f);
                btnHelp.Text = "HELP";
                btnHelp.Img = this.LoadBitmap("btn_small"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small);
                btnHelp.Glow = this.LoadBitmap("btn_small_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_small_glow);
                btnHelp.X = 6 * gv.squareSize + padW * 1 + gv.oXshift;
                btnHelp.Y = 9 * gv.squareSize + pH * 2;
                btnHelp.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnHelp.Width = (int)(gv.ibbwidthR * gv.screenDensity);
            }
            if (btnReturn == null)
            {
                btnReturn = new IbbButton(gv, 1.0f);
                btnReturn.Text = "RETURN";
                btnReturn.Img = this.LoadBitmap("btn_large"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_large);
                btnReturn.Glow = this.LoadBitmap("btn_large_glow"); // BitmapFactory.decodeResource(getResources(), R.drawable.btn_large_glow);
                btnReturn.X = (gv.screenWidth / 2) - (int)(gv.ibbwidthL * gv.screenDensity / 2.0f);
                btnReturn.Y = 10 * gv.squareSize + pH * 2;
                btnReturn.Height = (int)(gv.ibbheight * gv.screenDensity);
                btnReturn.Width = (int)(gv.ibbwidthL * gv.screenDensity);
            }

        }
        public void setPortraitsStart()
        {
            float scale = 1.0f;
            int tabX1 = gv.cc.pnlPortraits.LocX + 0 * gv.squareSize + gv.squareSize / 2 + gv.pS;
            int tabX2 = gv.cc.pnlPortraits.LocX + 2 * gv.squareSize + gv.pS;
            int tabY1 = gv.cc.pnlPortraits.LocY + 0 * gv.squareSize + gv.squareSize / 2;
            int tabY2 = gv.cc.pnlPortraits.LocY + 2 * gv.squareSize + gv.squareSize / 2;
            int tabY3 = gv.cc.pnlPortraits.LocY + 4 * gv.squareSize + gv.squareSize / 2;
                        
            if (!gv.useLargeLayout)
            {
                scale = 0.75f;
                tabY1 = gv.cc.pnlPortraits.LocY + 0 * gv.squareSize + gv.squareSize / 2;
                tabY2 = gv.cc.pnlPortraits.LocY + 2 * gv.squareSize;
                tabY3 = gv.cc.pnlPortraits.LocY + 3 * gv.squareSize + gv.squareSize / 2;
            }
            int ptrHeight = (int)(gv.ibpheight * gv.screenDensity * scale);
            int ptrWidth = (int)(gv.ibpwidth * gv.screenDensity * scale);

            if (ptrPc0 == null)
            {
                ptrPc0 = new IbbPortrait(gv, 0.8f);
                ptrPc0.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc0.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc0.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc0.X = tabX1;
                ptrPc0.Y = tabY1;
                ptrPc0.Height = ptrHeight;
                ptrPc0.Width = ptrWidth;
            }
            if (ptrPc1 == null)
            {
                ptrPc1 = new IbbPortrait(gv, 0.8f);
                ptrPc1.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc1.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc1.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc1.X = tabX2;
                ptrPc1.Y = tabY1;
                ptrPc1.Height = ptrHeight;
                ptrPc1.Width = ptrWidth;
            }
            if (ptrPc2 == null)
            {
                ptrPc2 = new IbbPortrait(gv, 0.8f);
                ptrPc2.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc2.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc2.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc2.X = tabX1;
                ptrPc2.Y = tabY2;
                ptrPc2.Height = ptrHeight;
                ptrPc2.Width = ptrWidth;
            }
            if (ptrPc3 == null)
            {
                ptrPc3 = new IbbPortrait(gv, 0.8f);
                ptrPc3.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc3.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc3.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc3.X = tabX2;
                ptrPc3.Y = tabY2;
                ptrPc3.Height = ptrHeight;
                ptrPc3.Width = ptrWidth;
            }
            if (ptrPc4 == null)
            {
                ptrPc4 = new IbbPortrait(gv, 0.8f);
                ptrPc4.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc4.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc4.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc4.X = tabX1;
                ptrPc4.Y = tabY3;
                ptrPc4.Height = ptrHeight;
                ptrPc4.Width = ptrWidth;
            }
            if (ptrPc5 == null)
            {
                ptrPc5 = new IbbPortrait(gv, 0.8f);
                ptrPc5.ImgBG = gv.cc.LoadBitmap("item_slot");
                ptrPc5.Glow = gv.cc.LoadBitmap("btn_ptr_glow");
                ptrPc5.ImgLU = gv.cc.LoadBitmap("btnLevelUpPlus");
                ptrPc5.X = tabX2;
                ptrPc5.Y = tabY3;
                ptrPc5.Height = ptrHeight;
                ptrPc5.Width = ptrWidth;
            }
        }
        public void setToggleButtonsStart()
        {
            if (tglSound == null)
            {
                tglSound = new IbbToggleButton(gv);
                tglSound.ImgOn = this.LoadBitmap("tgl_music_on"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sound_on);
                tglSound.ImgOff = this.LoadBitmap("tgl_music_off"); // BitmapFactory.decodeResource(getResources(), R.drawable.tgl_sound_off);
                tglSound.X = gv.cc.pnlToggles.LocX + 2 * gv.squareSize + gv.squareSize / 4;
                tglSound.Y = gv.cc.pnlToggles.LocY + 1 * gv.squareSize + gv.squareSize / 4 + gv.pS;
                tglSound.Height = (int)(gv.ibbheight / 2 * gv.screenDensity);
                tglSound.Width = (int)(gv.ibbwidthR / 2 * gv.screenDensity);
            }
        }

        //TUTORIAL MESSAGES
        public void tutorialMessageMainMap()
        {
            gv.sf.MessageBoxHtml(this.stringMessageMainMap);
        }
        public void tutorialMessageParty(bool helpCall)
        {
            if ((gv.mod.showTutorialParty) || (helpCall))
            {
                gv.sf.MessageBoxHtml(this.stringMessageParty);
                gv.mod.showTutorialParty = false;
            }
        }
        public void tutorialMessageInventory(bool helpCall)
        {
            if ((gv.mod.showTutorialInventory) || (helpCall))
            {
                gv.sf.MessageBoxHtml(this.stringMessageInventory);
                gv.mod.showTutorialInventory = false;
            }
        }
        public void tutorialMessageCombat(bool helpCall)
        {
            if ((gv.mod.showTutorialCombat) || (helpCall))
            {
                gv.sf.MessageBoxHtml(this.stringMessageCombat);
                gv.mod.showTutorialCombat = false;
            }
        }
        public void tutorialPcCreation()
        {
            gv.sf.MessageBoxHtml(this.stringPcCreation);
        }
        public void tutorialPlayersGuide()
        {
            gv.sf.MessageBoxHtml(this.stringPlayersGuide);
        }
        public void tutorialBeginnersGuide()
        {
            gv.sf.MessageBoxHtml(this.stringBeginnersGuide);
        }

        public void addLogText(string color, string text)
        {
            if (color.Equals("red"))
            {
                gv.log.AddHtmlTextToLog("<font color='red'>" + text + "</font>");
            }
            else if (color.Equals("lime"))
            {
                gv.log.AddHtmlTextToLog("<font color='lime'>" + text + "</font>");
            }
            else if (color.Equals("yellow"))
            {
                gv.log.AddHtmlTextToLog("<font color='yellow'>" + text + "</font>");
            }
            else if (color.Equals("teal"))
            {
                gv.log.AddHtmlTextToLog("<font color='teal'>" + text + "</font>");
            }
            else if (color.Equals("blue"))
            {
                gv.log.AddHtmlTextToLog("<font color='blue'>" + text + "</font>");
            }
            else if (color.Equals("fuchsia"))
            {
                gv.log.AddHtmlTextToLog("<font color='fuchsia'>" + text + "</font>");
            }
            else
            {
                gv.log.AddHtmlTextToLog("<font color='white'>" + text + "</font>");
            }
            /*
            <?xml version="1.0" encoding="utf-8"?>
            <resources>
             <color name="white">#FFFFFF</color>
             <color name="yellow">#FFFF00</color>
             <color name="fuchsia">#FF00FF</color>
             <color name="red">#FF0000</color>
             <color name="silver">#C0C0C0</color>
             <color name="gray">#808080</color>
             <color name="olive">#808000</color>
             <color name="purple">#800080</color>
             <color name="maroon">#800000</color>
             <color name="aqua">#00FFFF</color>
             <color name="lime">#00FF00</color>
             <color name="teal">#008080</color>
             <color name="green">#008000</color>
             <color name="blue">#0000FF</color>
             <color name="navy">#000080</color>
             <color name="black">#000000</color>
            </resources>
            */
        }
        public void addLogText(string text)
        {
            gv.log.AddHtmlTextToLog(text);		
        }
        public void addFloatyText(Coordinate coorInSquares, string value)
        {
            int txtH = (int)gv.drawFontRegHeight;
            int x = ((coorInSquares.X * gv.squareSize) + (gv.squareSize / 2) + gv.oXshift) - (txtH / 2);
            int y = ((coorInSquares.Y * gv.squareSize) + (gv.squareSize / 2) + txtH) - (txtH / 2);
            Coordinate coor = new Coordinate(x, y);
            floatyTextList.Add(new FloatyText(coor, value));
        }
        public void addFloatyText(Coordinate coorInSquares, string value, string color)
        {
            int txtH = (int)gv.drawFontRegHeight;
            int x = ((coorInSquares.X * gv.squareSize) + (gv.squareSize / 2) + gv.oXshift) - (txtH / 2);
            int y = ((coorInSquares.Y * gv.squareSize) + (gv.squareSize / 2) + txtH) - (txtH / 2);
            Coordinate coor = new Coordinate(x, y);
            floatyTextList.Add(new FloatyText(coor, value, color));
        }
        public void addFloatyText(Coordinate coorInSquares, string value, int shiftUp)
        {
            int txtH = (int)gv.drawFontRegHeight;
            int x = ((coorInSquares.X * gv.squareSize) + (gv.squareSize / 2) + gv.oXshift) - (txtH / 2);
            int y = ((coorInSquares.Y * gv.squareSize) + (gv.squareSize / 2) + txtH) - (txtH / 2) - shiftUp;
            Coordinate coor = new Coordinate(x, y);
            floatyTextList.Add(new FloatyText(coor, value));
        }

        public void doSettingsDialogs()
        {
            /*TODO
		    try
            {
			    final CharSequence[] items = {"Play Music","Button Sounds","Button Vibrate","DebugMode","Autosave Upon Exit","Show Message When Autosaving"};
	        
        	    //final Container container = mod.getContainerByTag(tag);
        	    final boolean[] checkedSettings = new boolean[6]; 
        	    if (gv.mod.playMusic)
                {
            	    checkedSettings[0] = true;
                }
                if (gv.mod.playButtonSounds)
                {
            	    checkedSettings[1] = true;
                }
                if (gv.mod.playButtonHaptic)
                {
            	    checkedSettings[2] = true;
                }
                if (gv.mod.debugMode)
                {
            	    checkedSettings[3] = true;
                }     
                if (gv.mod.allowAutosave)
                {
            	    checkedSettings[4] = true;
                }
                if (gv.mod.showAutosaveMessage)
                {
            	    checkedSettings[5] = true;
                }
        	    //set initial states for checkedSettings for loop
        	    //final List<String> containerItems = new ArrayList<String>();
                      
                //final CharSequence[] items = containerItems.toArray(new CharSequence[containerItems.size()]);
                // Creating and Building the Dialog 
                AlertDialog.Builder builder = new AlertDialog.Builder(gv.gameContext);
                builder.setTitle("Settings and Guides");
                builder.setMultiChoiceItems(items, checkedSettings, new DialogInterface.OnMultiChoiceClickListener() {
				
				    @Override
				    public void onClick(DialogInterface dialog, int which, boolean isChecked) {
					    switch (which)
					    {
					    case 0:
						    if (gv.mod.playMusic)
	                        {
							    gv.stopMusic();
							    gv.stopAmbient();
							    gv.mod.playMusic = false;
	    					    addLogText("lime","Music Off");
	                        }
	                        else
	                        {
	                    	    gv.startMusic();
	                    	    gv.startAmbient();
	                    	    gv.mod.playMusic = true;
	                    	    addLogText("lime","Music On");
	                        }					
						    break;
						
					    case 1:
						    if (gv.mod.playButtonSounds)
	                        {
							    gv.mod.playButtonSounds = false;
	    					    addLogText("lime","Button Sounds Off");
	                        }
	                        else
	                        {
	                    	    gv.mod.playButtonSounds = true;
	                    	    addLogText("lime","Button Sounds On");
	                        }					
						    break;	
						
					    case 2:
						    if (gv.mod.playButtonHaptic)
	                        {
							    gv.mod.playButtonHaptic = false;
	    					    addLogText("lime","Button Vibrate Off");
	                        }
	                        else
	                        {
	                    	    gv.mod.playButtonHaptic = true;
	                    	    addLogText("lime","Button Vibrate On");
	                        }
						    break;
						
					    case 3:
						    if (gv.mod.debugMode)
	                        {
							    gv.mod.debugMode = false;
	    					    addLogText("lime","DebugMode Off");
	                        }
	                        else
	                        {
	                    	    gv.mod.debugMode = true;
	                    	    addLogText("lime","DebugMode On");
	                        }
						    break;
					
					    case 4:
						    if (gv.mod.allowAutosave)
	                        {
							    gv.mod.allowAutosave = false;
	    					    addLogText("lime","Autosave Off");
	                        }
	                        else
	                        {
	                    	    gv.mod.allowAutosave = true;
	                    	    addLogText("lime","Autosave On");
	                        }
						    break;
						
					    case 5:
						    if (gv.mod.showAutosaveMessage)
	                        {
							    gv.mod.showAutosaveMessage = false;
	    					    addLogText("lime","Autosave Message Off");
	                        }
	                        else
	                        {
	                    	    gv.mod.showAutosaveMessage = true;
	                    	    addLogText("lime","Autosave Message On");
	                        }
						    break;
					    }
				    }
			    });
            
                // Set the action buttons
                builder.setPositiveButton("Player's Guide", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int id) 
                    {           
                	    tutorialPlayersGuide();
                        // user clicked OK, so save the mSelectedItems results somewhere
                        // here we are trying to retrieve the selected items indices
                        //ActionDialog.dismiss();
                        //invalidate();
                    }
                });
             
                builder.setNeutralButton("OK", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int id) 
                    {                     
                        // user clicked OK, so save the mSelectedItems results somewhere
                        // here we are trying to retrieve the selected items indices
                	    gv.ActionDialog.dismiss();
                	    gv.invalidate();
                    }
                });
            
                builder.setNegativeButton("Beginner's Guide", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int id) 
                    {
                	    tutorialBeginnersGuide();
                    }
                });
            
                gv.ActionDialog = builder.create();
                gv.ActionDialog.show();
            }
            catch (Exception ex)
            {
                //IBMessageBox.Show(game, "failed to open conversation with tag: " + tag);
            }
            */
        }

        public void doAboutDialog()
        {
            gv.sf.MessageBoxHtml(gv.mod.moduleCredits);
        }

        public void SwitchToNextAvailablePartyLeader()
        {
            int idx = 0;
            foreach (Player pc in gv.mod.playerList)
            {
                if (!pc.isDead())
                {
                    gv.mod.selectedPartyLeader = idx;
                    return;
                }
                idx++;
            }
        }

        public void doUpdate()
        {
            setToBorderPixDistancesMainMap();
            if (gv.mod.useAllTileSystem)
            {
                resetLightAndDarkness();
            }
            #region tile loading on demand
            if (gv.mod.useAllTileSystem)
            {
                //addLogText("yellow", "Number of tiles, before cull:" + gv.mod.loadedTileBitmaps.Count.ToString());
                /*
                if ((gv.mod.PlayerLastLocationX == gv.mod.PlayerLocationX) && (gv.mod.PlayerLastLocationY == gv.mod.PlayerLocationY))
                {
                    gv.mod.blockTrigger = true;
                }
                else
                {
                    gv.mod.blockTrigger = false;
                }
                */
                //cull all down if too high value is reached (last resort)
                if (gv.mod.loadedTileBitmaps.Count > 400)
                {
                    try
                    {
                        foreach (Bitmap bm in gv.mod.loadedTileBitmaps)
                        {
                            bm.Dispose();
                        }

                        //these two lists keep an exact order so each bitmap stored in one corrsponds with a name in the other
                        gv.mod.loadedTileBitmaps.Clear();
                        gv.mod.loadedTileBitmapsNames.Clear();
                    }
                    catch
                    {

                    }

                }

                if (gv.mod.loadedTileBitmaps.Count > 250)
                {
                    //addLogText("yellow", "Disposing tiles.");
                    int cullNumber = ((gv.mod.loadedTileBitmaps.Count / 10) + 2);
                    try
                    {
                        if (gv.mod.loadedTileBitmaps != null)
                        {
                            //remove 12 entries per move, 3 more than usual 9 squares uncovered
                            for (int i = 0; i < cullNumber; i++)
                            {
                                gv.mod.loadedTileBitmaps[i].Dispose();
                                gv.mod.loadedTileBitmaps.RemoveAt(i);
                                gv.mod.loadedTileBitmapsNames.RemoveAt(i);

                                //addLogText("red", "Removal Counter is:" + i.ToString());

                            }

                        }

                        //these two lists keep an exact order so each bitmap stored in one corresponds with a name in the other
                    }
                    catch
                    {
                        //addLogText("red", "caught error");
                    }
                }

                //addLogText("red", "number of tiles in cache, after cull:" + gv.mod.loadedTileBitmaps.Count);
                //normal cleanup while moving
            }
            #endregion

            //reset the timer interval, important for synching with party move
            if (gv.mod.useRealTimeTimer == true)
            {
                //gv.realTimeTimer.Stop();
                //gv.realTimeTimer.Start();
            }

            //in case whole party is unconscious and bleeding, end the game (outside combat here)
            bool endGame = true;
            foreach (Player pc in gv.mod.playerList)
            {
                if (pc.hp >= 0)
                {
                    endGame = false;
                    break;
                }
            }

            if (endGame == true)
            {
                gv.resetGame();
                gv.screenType = "title";
                IBMessageBox.Show(gv, "Everybody is unconscious and bleeding - your party has been defeated!");
                return;
            }

            #region handling chances for full screen animation effects, edit: for all 10 channels now
            if ((gv.mod.currentArea.fullScreenEffectLayerIsActive1 == false) && (gv.mod.currentArea.numberOfCyclesPerOccurence1 != 0))
            {
                if (gv.sf.RandInt(100) < gv.mod.currentArea.fullScreenEffectChanceToOccur1)
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 == 1)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 == 2)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 == 3)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 == 4)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 == 5)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 == 6)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 == 7)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 == 8)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 == 9)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 == 10)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    }
                }
            }
            if ((gv.mod.currentArea.fullScreenEffectLayerIsActive2 == false) && (gv.mod.currentArea.numberOfCyclesPerOccurence2 != 0))
            {
                if (gv.sf.RandInt(100) < gv.mod.currentArea.fullScreenEffectChanceToOccur2)
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 == 1)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 == 2)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 == 3)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 == 4)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 == 5)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 == 6)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 == 7)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 == 8)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 == 9)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 == 10)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    }
                }
            }
            if ((gv.mod.currentArea.fullScreenEffectLayerIsActive3 == false) && (gv.mod.currentArea.numberOfCyclesPerOccurence3 != 0))
            {
                if (gv.sf.RandInt(100) < gv.mod.currentArea.fullScreenEffectChanceToOccur3)
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 == 1)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 == 2)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 == 3)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 == 4)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 == 5)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 == 6)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 == 7)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 == 8)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 == 9)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 == 10)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    }
                }
            }
            if ((gv.mod.currentArea.fullScreenEffectLayerIsActive4 == false) && (gv.mod.currentArea.numberOfCyclesPerOccurence4 != 0))
            {
                if (gv.sf.RandInt(100) < gv.mod.currentArea.fullScreenEffectChanceToOccur4)
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 == 1)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 == 2)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 == 3)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 == 4)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 == 5)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 == 6)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 == 7)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 == 8)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 == 9)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 == 10)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    }

                }
            }
            if ((gv.mod.currentArea.fullScreenEffectLayerIsActive5 == false) && (gv.mod.currentArea.numberOfCyclesPerOccurence5 != 0))
            {
                if (gv.sf.RandInt(100) < gv.mod.currentArea.fullScreenEffectChanceToOccur5)
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 == 1)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 == 2)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 == 3)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 == 4)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 == 5)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 == 6)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 == 7)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 == 8)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 == 9)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 == 10)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    }
                }
            }
            if ((gv.mod.currentArea.fullScreenEffectLayerIsActive6 == false) && (gv.mod.currentArea.numberOfCyclesPerOccurence6 != 0))
            {
                if (gv.sf.RandInt(100) < gv.mod.currentArea.fullScreenEffectChanceToOccur6)
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 == 1)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 == 2)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 == 3)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 == 4)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 == 5)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 == 6)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 == 7)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 == 8)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 == 9)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 == 10)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    }
                }
            }
            if ((gv.mod.currentArea.fullScreenEffectLayerIsActive7 == false) && (gv.mod.currentArea.numberOfCyclesPerOccurence7 != 0))
            {
                if (gv.sf.RandInt(100) < gv.mod.currentArea.fullScreenEffectChanceToOccur7)
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 == 1)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 == 2)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 == 3)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 == 4)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 == 5)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 == 6)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 == 7)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 == 8)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 == 9)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 == 10)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    }
                }
            }
            if ((gv.mod.currentArea.fullScreenEffectLayerIsActive8 == false) && (gv.mod.currentArea.numberOfCyclesPerOccurence8 != 0))
            {
                if (gv.sf.RandInt(100) < gv.mod.currentArea.fullScreenEffectChanceToOccur8)
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 == 1)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 == 2)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 == 3)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 == 4)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 == 5)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 == 6)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 == 7)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 == 8)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 == 9)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 == 10)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    }
                }
            }
            if ((gv.mod.currentArea.fullScreenEffectLayerIsActive9 == false) && (gv.mod.currentArea.numberOfCyclesPerOccurence9 != 0))
            {
                if (gv.sf.RandInt(100) < gv.mod.currentArea.fullScreenEffectChanceToOccur9)
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 == 1)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 == 2)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 == 3)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 == 4)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 == 5)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 == 6)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 == 7)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 == 8)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 == 9)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 == 10)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    }
                }
            }
            if ((gv.mod.currentArea.fullScreenEffectLayerIsActive10 == false) && (gv.mod.currentArea.numberOfCyclesPerOccurence10 != 0))
            {
                if (gv.sf.RandInt(100) < gv.mod.currentArea.fullScreenEffectChanceToOccur10)
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 == 1)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 == 2)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 == 3)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 == 4)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 == 5)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 == 6)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 == 7)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 == 8)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 == 9)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    }
                    if (gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 == 10)
                    {
                        gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    }
                }
            }
            #endregion
           
            //CLEAN UP START SCREENS IF DONE WITH THEM
            if (gv.screenLauncher != null)
            {
                gv.screenLauncher = null;
                gv.screenPartyBuild = null;
                gv.screenPcCreation = null;
                gv.screenTitle = null;
            }

            if ((gv.mod.PlayerLocationX != gv.mod.arrivalSquareX) || (gv.mod.PlayerLocationY != gv.mod.arrivalSquareY))
            {
                gv.mod.justTransitioned = false;
                gv.mod.justTransitioned2 = false;
                gv.mod.arrivalSquareX = 1000000;
                gv.mod.arrivalSquareY = 1000000;
            }
       

            gv.sf.dsWorldTime();
            //IBScript Module heartbeat
            gv.cc.doIBScriptBasedOnFilename(gv.mod.OnHeartBeatIBScript, gv.mod.OnHeartBeatIBScriptParms);
            //IBScript Area heartbeat
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentArea.OnHeartBeatIBScript, gv.mod.currentArea.OnHeartBeatIBScriptParms);
            //apply effects
            applyEffects();
            //do Prop heartbeat
            doPropHeartBeat();
            //script hook for the weather script (channels 5 to 10)
            if (gv.mod.currentArea.areaWeatherName != "")
            {
                doWeather();
            }
            //script hook for full screen effects on channels 1 to 4 (also called in doTransitionBasedOnAreaLocation)
            doChannelScripts();
            //do weather sounds
            if (gv.mod.currentArea.areaWeatherName != "")
            {
                doWeatherSound();
            }

            blockSecondPropTriggersCall = false;
            //do Conversation and/or Encounter if on Prop (check before props move)
            gv.triggerPropIndex = 0;
            gv.triggerIndex = 0;
            doPropTriggers();

            //move any props that are active and only if they are not on the party location
            doPropMoves();

            //set illumination level of tiles based on party and prop position
            if (gv.mod.useAllTileSystem)
            {
                doIllumination();
            }
            //do Conversation and/or Encounter if on Prop (check after props move)
            if (!blockSecondPropTriggersCall)
            {
                gv.triggerPropIndex = 0;
                gv.triggerIndex = 0;
                doPropTriggers();
            }

            //move any props that are active and only if they are not on the party location
            //doPropMoves();

            //check for levelup available and switch button image
            checkLevelUpAvailable(); //move this to on update and use a plus overlay in top left
            adjustSpriteMainMapPositionToMakeItMoveIdependentlyFromPlayer();
        }

        public void resetLightAndDarkness()
        {

            //if (gv.mod.arrivalSquareX != 1000000)
            //{
                //gv.mod.PlayerLocationX = gv.mod.arrivalSquareX;
                //gv.mod.PlayerLocationY = gv.mod.arrivalSquareY;
            //}

            //XXXXXXXXXXXXXXXXXXXXXXXXX
            int indexOfNorthernNeighbour = -1;
            int indexOfSouthernNeighbour = -1;
            int indexOfEasternNeighbour = -1;
            int indexOfWesternNeighbour = -1;
            int indexOfNorthEasternNeighbour = -1;
            int indexOfNorthWesternNeighbour = -1;
            int indexOfSouthEasternNeighbour = -1;
            int indexOfSouthWesternNeighbour = -1;

            int seamlessModififierMinX = 0;
            int seamlessModififierMaxX = 0;
            int seamlessModififierMinY = 0;
            int seamlessModififierMaxY = 0;

            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

            if ((gv.mod.currentArea.northernNeighbourArea != ""))
            {
                for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.northernNeighbourArea)
                    {
                        indexOfNorthernNeighbour = i;
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].easternNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].easternNeighbourArea)
                        {
                            indexOfNorthEasternNeighbour = i;
                        }
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].westernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].westernNeighbourArea)
                        {
                            indexOfNorthWesternNeighbour = i;
                        }
                    }
                }
            }

            if ((gv.mod.currentArea.southernNeighbourArea != ""))
            {
                for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.southernNeighbourArea)
                    {
                        indexOfSouthernNeighbour = i;
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].easternNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].easternNeighbourArea)
                        {
                            indexOfSouthEasternNeighbour = i;
                        }
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].westernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].westernNeighbourArea)
                        {
                            indexOfSouthWesternNeighbour = i;
                        }
                    }
                }
            }

            if ((gv.mod.currentArea.westernNeighbourArea != ""))
            {
                for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.westernNeighbourArea)
                    {
                        indexOfWesternNeighbour = i;
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfWesternNeighbour].northernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfWesternNeighbour].northernNeighbourArea)
                        {
                            indexOfNorthWesternNeighbour = i;
                        }
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfWesternNeighbour].southernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfWesternNeighbour].southernNeighbourArea)
                        {
                            indexOfSouthWesternNeighbour = i;
                        }
                    }
                }
            }

            if ((gv.mod.currentArea.easternNeighbourArea != ""))
            {
                for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.easternNeighbourArea)
                    {
                        indexOfEasternNeighbour = i;
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfEasternNeighbour].northernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfEasternNeighbour].northernNeighbourArea)
                        {
                            indexOfNorthEasternNeighbour = i;
                        }
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfEasternNeighbour].southernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfEasternNeighbour].southernNeighbourArea)
                        {
                            indexOfSouthEasternNeighbour = i;
                        }
                    }
                }
            }

            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

            if ((gv.mod.currentArea.northernNeighbourArea != "") && (gv.mod.PlayerLocationY < gv.playerOffsetY))
            {
                seamlessModififierMinY = gv.playerOffsetY - gv.mod.PlayerLocationY;
            }

            if ((gv.mod.currentArea.southernNeighbourArea != "") && (gv.mod.PlayerLocationY > (gv.mod.currentArea.MapSizeY - 1 - gv.playerOffsetY)))
            {
                seamlessModififierMaxY = gv.mod.PlayerLocationY - (gv.mod.currentArea.MapSizeY - 1) + gv.playerOffsetY;
            }

            if ((gv.mod.currentArea.westernNeighbourArea != "") && (gv.mod.PlayerLocationX < gv.playerOffsetX))
            {
                seamlessModififierMinX = gv.playerOffsetX - gv.mod.PlayerLocationX;
            }

            if ((gv.mod.currentArea.easternNeighbourArea != "") && (gv.mod.PlayerLocationX > (gv.mod.currentArea.MapSizeX - 1 - gv.playerOffsetX)))
            {
                seamlessModififierMaxX = gv.mod.PlayerLocationX - (gv.mod.currentArea.MapSizeX - 1) + gv.playerOffsetX;
            }


            //this block draws teh saquares immediately around the player

            int minX = gv.mod.PlayerLocationX - gv.playerOffsetX;
            if (minX < -seamlessModififierMinX) { minX = -seamlessModififierMinX; }
            int minY = gv.mod.PlayerLocationY - gv.playerOffsetY;
            if (minY < -seamlessModififierMinY) { minY = -seamlessModififierMinY; }

            int maxX = gv.mod.PlayerLocationX + gv.playerOffsetX;
            if (maxX > this.gv.mod.currentArea.MapSizeX - 1 + seamlessModififierMaxX) { maxX = this.gv.mod.currentArea.MapSizeX - 1 + seamlessModififierMaxX; }
            int maxY = gv.mod.PlayerLocationY + gv.playerOffsetY;
            if (maxY > this.gv.mod.currentArea.MapSizeY - 1 + seamlessModififierMaxY) { maxY = this.gv.mod.currentArea.MapSizeY - 1 + seamlessModififierMaxY; }

            for (int xx = minX; xx <= maxX; xx++)
            {
                for (int yy = minY; yy <= maxY; yy++)
                {
                    //YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
                    bool situationFound = false;
                    bool drawTile = true;
                    int index = -1;
                    Tile tile = new Tile();

                    //nine situations where a tile can be:
                    //tile on north-western map (diagonal situation)
                    if ((xx < 0) && (yy < 0) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfNorthWesternNeighbour != -1)
                        {
                            int transformedX = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeX + xx;
                            int transformedY = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeY + yy;
                            tile = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeX + transformedX];
                            index = indexOfNorthWesternNeighbour;
                        }
                        else
                        {
                            drawTile = false;
                        }
                    }
                    //tile on south-westernmap (diagonal situation)
                    if ((xx < 0) && (yy > (gv.mod.currentArea.MapSizeY - 1)) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfSouthWesternNeighbour != -1)
                        {
                            int transformedX = gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].MapSizeX + xx;
                            int transformedY = yy - gv.mod.currentArea.MapSizeY;
                            tile = gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].MapSizeX + transformedX];
                            index = indexOfSouthWesternNeighbour;
                        }
                        else
                        {
                            drawTile = false;
                        }
                    }
                    //tile on south-easternmap (diagonal situation)
                    if ((xx > (gv.mod.currentArea.MapSizeX - 1)) && (yy > (gv.mod.currentArea.MapSizeY - 1)) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfSouthEasternNeighbour != -1)
                        {
                            int transformedX = xx - gv.mod.currentArea.MapSizeX;
                            int transformedY = yy - gv.mod.currentArea.MapSizeY;
                            tile = gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].MapSizeX + transformedX];
                            index = indexOfSouthEasternNeighbour;
                        }
                        else
                        {
                            drawTile = false;
                        }
                    }
                    //tile on north-easternmap (diagonal situation)
                    if ((xx > (gv.mod.currentArea.MapSizeX - 1)) && (yy < 0) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfNorthEasternNeighbour != -1)
                        {
                            int transformedX = xx - gv.mod.currentArea.MapSizeX;
                            int transformedY = gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].MapSizeY + yy;
                            tile = gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].MapSizeX + transformedX];
                            index = indexOfNorthEasternNeighbour;
                        }
                        else
                        {
                            drawTile = false;
                        }
                    }
                    //tile on western map
                    if ((xx < 0) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfWesternNeighbour != -1)
                        {
                            int transformedX = gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + xx;
                            int transformedY = yy;
                            tile = gv.mod.moduleAreasObjects[indexOfWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + transformedX];
                            index = indexOfWesternNeighbour;
                        }
                        else
                        {
                            drawTile = false;
                        }
                    }
                    //tile on southern map
                    if ((yy > (gv.mod.currentArea.MapSizeY - 1)) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfSouthernNeighbour != -1)
                        {
                            int transformedX = xx;
                            int transformedY = yy - gv.mod.currentArea.MapSizeY;
                            tile = gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].MapSizeX + transformedX];
                            index = indexOfSouthernNeighbour;
                        }
                        else
                        {
                            drawTile = false;
                        }
                    }
                    //tile on eastern map
                    if ((xx > (gv.mod.currentArea.MapSizeX - 1)) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfEasternNeighbour != -1)
                        {
                            int transformedX = xx - gv.mod.currentArea.MapSizeX;
                            int transformedY = yy;
                            tile = gv.mod.moduleAreasObjects[indexOfEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour].MapSizeX + transformedX];
                            index = indexOfEasternNeighbour;
                        }
                        else
                        {
                            drawTile = false;
                        }
                    }
                    //tile on northern map
                    if ((yy < 0) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfNorthernNeighbour != -1)
                        {
                            int transformedX = xx;
                            int transformedY = gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeY + yy;
                            tile = gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeX + transformedX];
                            index = indexOfNorthernNeighbour;
                        }
                        else
                        {
                            drawTile = false;
                        }
                    }
                    //tile is on current map
                    if (!situationFound)
                    {
                        tile = gv.mod.currentArea.Tiles[yy * gv.mod.currentArea.MapSizeX + xx];
                        //mod.currentArea.Tiles[yy * mod.currentArea.MapSizeX + xx].Visible = true;
                    }

                    if (drawTile)
                    {
                        //tile.Visible = true;
                        tile.lightRadius = 0;
                        tile.isCentreOfLightCircle = false;
                        tile.isOtherPartOfLightCircle = false;
                        tile.hasHalo = false;
                        tile.isFocalPoint = false;
                        tile.tilePositionInLitArea.Clear();
                        tile.tileLightSourceTag.Clear();
                        tile.isLit.Clear();
                        tile.priority.Clear();
                        tile.lightSourceCoordinate.Clear();
                    }

                    //YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
                    //mod.currentArea.Tiles[yy * mod.currentArea.MapSizeX + xx].Visible = true;
                }
            }

        }

        public void doIllumination()
        {
            /*
            int backupPlayerLocationX = gv.mod.PlayerLocationX;
            int backupPlayerLocationY = gv.mod.PlayerLocationY;

            if (gv.mod.justTransitioned)
            {
                gv.mod.PlayerLocationX = gv.mod.arrivalSquareX;
                gv.mod.PlayerLocationY = gv.mod.arrivalSquareY;
            }
            */

            //XXXXXXXXXXXXXXXXXXXXXXXXX
            int indexOfNorthernNeighbour = -1;
            int indexOfSouthernNeighbour = -1;
            int indexOfEasternNeighbour = -1;
            int indexOfWesternNeighbour = -1;
            int indexOfNorthEasternNeighbour = -1;
            int indexOfNorthWesternNeighbour = -1;
            int indexOfSouthEasternNeighbour = -1;
            int indexOfSouthWesternNeighbour = -1;
            int indexOfCurrentArea = -1;

            int seamlessModififierMinX = 0;
            int seamlessModififierMaxX = 0;
            int seamlessModififierMinY = 0;
            int seamlessModififierMaxY = 0;

            

            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            
            for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
            {
                if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.Filename)
                {
                    indexOfCurrentArea = i;
                }
            }


            if ((gv.mod.currentArea.northernNeighbourArea != ""))
            {
                for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.northernNeighbourArea)
                    {
                        indexOfNorthernNeighbour = i;
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].easternNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].easternNeighbourArea)
                        {
                            indexOfNorthEasternNeighbour = i;
                        }
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].westernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].westernNeighbourArea)
                        {
                            indexOfNorthWesternNeighbour = i;
                        }
                    }
                }
            }

            if ((gv.mod.currentArea.southernNeighbourArea != ""))
            {
                for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.southernNeighbourArea)
                    {
                        indexOfSouthernNeighbour = i;
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].easternNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].easternNeighbourArea)
                        {
                            indexOfSouthEasternNeighbour = i;
                        }
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].westernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].westernNeighbourArea)
                        {
                            indexOfSouthWesternNeighbour = i;
                        }
                    }
                }
            }

            if ((gv.mod.currentArea.westernNeighbourArea != ""))
            {
                for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.westernNeighbourArea)
                    {
                        indexOfWesternNeighbour = i;
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfWesternNeighbour].northernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfWesternNeighbour].northernNeighbourArea)
                        {
                            indexOfNorthWesternNeighbour = i;
                        }
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfWesternNeighbour].southernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfWesternNeighbour].southernNeighbourArea)
                        {
                            indexOfSouthWesternNeighbour = i;
                        }
                    }
                }
            }

            if ((gv.mod.currentArea.easternNeighbourArea != ""))
            {
                for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.easternNeighbourArea)
                    {
                        indexOfEasternNeighbour = i;
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfEasternNeighbour].northernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfEasternNeighbour].northernNeighbourArea)
                        {
                            indexOfNorthEasternNeighbour = i;
                        }
                    }
                }

                if (gv.mod.moduleAreasObjects[indexOfEasternNeighbour].southernNeighbourArea != "")
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfEasternNeighbour].southernNeighbourArea)
                        {
                            indexOfSouthEasternNeighbour = i;
                        }
                    }
                }
            }

            if ((gv.mod.currentArea.northernNeighbourArea != "") && (gv.mod.PlayerLocationY < gv.playerOffsetY))
            {
                seamlessModififierMinY = gv.playerOffsetY - gv.mod.PlayerLocationY;
            }

            if ((gv.mod.currentArea.southernNeighbourArea != "") && (gv.mod.PlayerLocationY > (gv.mod.currentArea.MapSizeY - 1 - gv.playerOffsetY)))
            {
                seamlessModififierMaxY = gv.mod.PlayerLocationY - (gv.mod.currentArea.MapSizeY - 1) + gv.playerOffsetY;
            }

            if ((gv.mod.currentArea.westernNeighbourArea != "") && (gv.mod.PlayerLocationX < gv.playerOffsetX))
            {
                seamlessModififierMinX = gv.playerOffsetX - gv.mod.PlayerLocationX;
            }

            if ((gv.mod.currentArea.easternNeighbourArea != "") && (gv.mod.PlayerLocationX > (gv.mod.currentArea.MapSizeX - 1 - gv.playerOffsetX)))
            {
                seamlessModififierMaxX = gv.mod.PlayerLocationX - (gv.mod.currentArea.MapSizeX - 1) + gv.playerOffsetX;
            }

            //set up indices for iterating through light emitting props from neighbouring maps
            List<int> relevantIndices = new List<int>();

            //current area
            relevantIndices.Add(indexOfCurrentArea);

            //northwest
            if ((seamlessModififierMinX > 0) && (seamlessModififierMinY > 0) && (indexOfNorthWesternNeighbour != -1))
            {
                relevantIndices.Add(indexOfNorthWesternNeighbour);
            }
            //northeast
            if ((seamlessModififierMaxX > 0) && (seamlessModififierMinY > 0) && (indexOfNorthEasternNeighbour != -1))
            {
                relevantIndices.Add(indexOfNorthEasternNeighbour);
            }
            //southwest
            if ((seamlessModififierMinX > 0) && (seamlessModififierMaxY > 0) && (indexOfSouthWesternNeighbour != -1))
            {
                relevantIndices.Add(indexOfSouthWesternNeighbour);
            }
            //southeast
            if ((seamlessModififierMaxX > 0) && (seamlessModififierMaxY > 0) && (indexOfSouthEasternNeighbour != -1))
            {
                relevantIndices.Add(indexOfSouthEasternNeighbour);
            }
            //north
            if ((seamlessModififierMinY > 0) && (indexOfNorthernNeighbour != -1))
            {
                relevantIndices.Add(indexOfNorthernNeighbour);
            }
            //south
            if ((seamlessModififierMaxY > 0) && (indexOfSouthernNeighbour != -1))
            {
                relevantIndices.Add(indexOfSouthernNeighbour);
            }
            //west
            if ((seamlessModififierMinX > 0) && (indexOfWesternNeighbour != -1))
            {
                relevantIndices.Add(indexOfWesternNeighbour);
            }
            //east
            if ((seamlessModififierMaxX > 0) && (indexOfEasternNeighbour != -1))
            {
                relevantIndices.Add(indexOfEasternNeighbour);
            }

            /*
            for (int i = 0; i < relevantIndices.Count; i++)
            {//2

                int backupLocationX = -1;
                int backupLocationY = -1;

                foreach (Prop p in gv.mod.moduleAreasObjects[relevantIndices[i]].Props)
                {//3
                    //if (p.isLight)
                    //{
                        backupLocationX = p.LocationX;
                        backupLocationY = p.LocationY;

                        try
                        {

                            //1. centre of light (later in radius)
                            //continue here, good road
                            //must add radius of light, color bmp and flicker rate here later (read from prop and written into the affected tiles)
                            //assign light source tag to all squres belonging together here
                            //have alist with alreaddy drawn lightsource tile tags in draw clss rfrehed on eahc draw call
                            //hurgh 4444
                            gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].isCentreOfLightCircle = true;
                            //gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].flickerDelayCounter += gv.elapsed / 1000f * 30f;
                            //float testi = gv.elapsed;
                            if (p.LocationY + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].isCentreOfLightCircle = true;
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].tilePositionInLitArea = "";
                            }
                            if (p.LocationY + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1) && (p.LocationX - 1 >= 0))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isCentreOfLightCircle = true;
                            }
                            if (p.LocationY + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1) && (p.LocationX + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1)))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isCentreOfLightCircle = true;
                            }
                            if (p.LocationY - 1 >= 0)
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].isCentreOfLightCircle = true;
                            }
                            if ((p.LocationY - 1 >= 0) && (p.LocationX - 1 >= 0))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isCentreOfLightCircle = true;
                            }
                            if ((p.LocationY - 1 >= 0) && (p.LocationX + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1)))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isCentreOfLightCircle = true;
                            }
                            if (p.LocationX - 1 >= 0)
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isCentreOfLightCircle = true;
                            }
                            if (p.LocationX + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isCentreOfLightCircle = true;
                            }

                            gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].lightRadius = 1;


                            //2. rim of light, in deeper shades (later in radius)

                            //gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].isCentreOfLightCircle = true;
                            //gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].flickerDelayCounter += gv.elapsed / 1000f * 30f;
                            //float testi = gv.elapsed;
                            //NW,N1,N2,N3,NE,E1,E2,E3,SE,S1,S2,S3,SW,W1,W2,W3


                            //SW
                            if ((p.LocationY + 2 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1)) && (p.LocationX - 2 >= 0))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 2) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 2].isOtherPartOfLightCircle = true;
                            }

                            //S1
                            if ((p.LocationY + 2 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1)) && (p.LocationX - 1 >= 0))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 2) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                            }

                            //S2
                            if (p.LocationY + 2 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 2) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].isOtherPartOfLightCircle = true;
                            }

                            //S3
                            if ((p.LocationY + 2 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1)) && (p.LocationX + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1)))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 2) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                            }

                            //SE
                            if ((p.LocationY + 2 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1)) && (p.LocationX + 2 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1)))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 2) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 2].isOtherPartOfLightCircle = true;
                            }

                            //E1
                            if ((p.LocationY - 1 >= 0) && p.LocationX + 2 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 2].isOtherPartOfLightCircle = true;
                            }

                            //E2
                            if (p.LocationX + 2 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 2].isOtherPartOfLightCircle = true;
                            }

                            //E3
                            if ((p.LocationY + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1)) && p.LocationX + 2 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 2].isOtherPartOfLightCircle = true;
                            }

                            //NE
                            if ((p.LocationY - 2 >= 0) && (p.LocationX + 2 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1)))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 2) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 2].isOtherPartOfLightCircle = true;
                            }

                            //N1
                            if ((p.LocationY - 2 >= 0) && (p.LocationX - 1 >= 0))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 2) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                            }

                            //N2
                            if (p.LocationY - 2 >= 0)
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 2) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].isOtherPartOfLightCircle = true;
                            }

                            //N3
                            if ((p.LocationY - 2 >= 0) && (p.LocationX + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1)))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 2) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                            }

                            //NW
                            if ((p.LocationY - 2 >= 0) && (p.LocationX - 2 >= 0))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 2) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 2].isOtherPartOfLightCircle = true;
                            }

                            //W1
                            if ((p.LocationX - 2 >= 0) && (p.LocationY - 1 >= 0))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 2].isOtherPartOfLightCircle = true;
                            }

                            //W2
                            if (p.LocationX - 2 >= 0)
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 2].isOtherPartOfLightCircle = true;
                            }

                            //W3
                            if ((p.LocationX - 2 >= 0) && (p.LocationY + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1)))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 2].isOtherPartOfLightCircle = true;
                            }





                        //XXX

                        if (p.LocationY + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1) && (p.LocationX - 1 >= 0))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                            }
                            if (p.LocationY + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1) && (p.LocationX + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1)))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                            }
                            if (p.LocationY - 1 >= 0)
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].isOtherPartOfLightCircle = true;
                            }
                            if ((p.LocationY - 1 >= 0) && (p.LocationX - 1 >= 0))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                            }
                            if ((p.LocationY - 1 >= 0) && (p.LocationX + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1)))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY - 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                            }
                            if (p.LocationX - 1 >= 0)
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                            }
                            if (p.LocationX + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX - 1))
                            {
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                            }
                        }
                        catch
                        { }
                    //}

                    //XXXXXXXXXXXXXXXXXXXXXXXXX
                    /*
                    if (tile.isCentreOfLightCircle)
                    {

                        tile.flickerDelayCounter += elapsed / 1000f * 30f;
                        dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels - (tile.lightRadius * gv.squareSize), tlY - (tile.lightRadius * gv.squareSize), brX * (1 + tile.lightRadius * 2), brY * (1 + tile.lightRadius * 2));
                        //tile.flicker = 0;
                        if (tile.flickerDelayCounter > 2)
                        {
                            if (tile.flickerRise)
                            {
                                //tile.affectedByFlickerAlready = true;
                                int decider = gv.sf.RandInt(2);
                                if (decider == 1)
                                {
                                    tile.flicker++;
                                }
                                else
                                {
                                    tile.flicker++;
                                    tile.flicker++;
                                }
                            }
                            else
                            {
                                //tile.affectedByFlickerAlready = true;
                                int decider = gv.sf.RandInt(2);
                                if (decider == 1)
                                {
                                    tile.flicker--;
                                }
                                else
                                {
                                    tile.flicker--;
                                    tile.flicker--;
                                }
                            }
                            if (tile.flicker >= 25)
                            {
                                tile.flickerRise = false;
                            }
                            if (tile.flicker <= 0)
                            {
                                tile.flickerRise = true;
                            }

                            tile.flickerDelayCounter = 0;
                        }
                        gv.DrawBitmap(gv.cc.hitSymbol, src, dst, 0, false, 1.0f - tile.flicker / 100f);
                        gv.DrawBitmap(gv.cc.black_tile, src, dst, 0, false, tile.flicker / 100f);
                        if (!tile.Visible)
                        {
                            dst = new IbRect(tlX + gv.oXshift + mapStartLocXinPixels, tlY, brX, brY);
                            gv.DrawBitmap(gv.cc.offScreen, src, dst, 0, false, 0.9f);
                        }
                    }
                    */

                    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX



                    /*
                    //XXXXXXXXXXXXXXXXXXX
                    bool situationFound = false;

                    //northwest
                    if (indexOfNorthWesternNeighbour != -1)
                    {
                        if (gv.mod.moduleAreasObjects[relevantIndices[i]].Filename.Contains(gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].Filename))
                        //if ((seamlessModififierMinX > 0) && (seamlessModififierMinY > 0) && !situationFound)
                        {
                            try
                            {
                                //continue here, good road
                                //hurgh 4444
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].isCentreOfLightCircle = true;
                                if (p.LocationY + 1 <= (gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY - 1))
                                {
                                    gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY + 1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].isOtherPartOfLightCircle = true;
                                }
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY+1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY+1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY-1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].isOtherPartOfLightCircle = true;
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY-1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[(p.LocationY-1) * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;

                                gv.mod.moduleAreasObjects[relevantIndices[i]].Tiles[p.LocationY * gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX + p.LocationX].lightRadius = 1;
                            }
                            catch
                            { }
                            //situationFound = true;
                            //p.LocationX = p.LocationX - gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX;
                            //p.LocationY = p.LocationY - gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY;

                        }
                    }

                    //northeast
                    if (indexOfNorthEasternNeighbour != -1)
                    {
                        if (gv.mod.moduleAreasObjects[relevantIndices[i]].Filename.Contains(gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].Filename))

                        //if ((seamlessModififierMaxX > 0) && (seamlessModififierMinY > 0) && !situationFound)
                        {
                            situationFound = true;
                            p.LocationX = p.LocationX + gv.mod.currentArea.MapSizeX;
                            p.LocationY = p.LocationY - gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY;

                        }
                    }

                    //southwest
                    if (indexOfSouthWesternNeighbour != -1)
                    {
                        if (gv.mod.moduleAreasObjects[relevantIndices[i]].Filename.Contains(gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].Filename))

                        //if ((seamlessModififierMinX > 0) && (seamlessModififierMaxY > 0) && !situationFound)
                        {
                            situationFound = true;
                            p.LocationX = p.LocationX - gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX;
                            p.LocationY = p.LocationY + gv.mod.currentArea.MapSizeY;

                        }
                    }

                    //southeast
                    if (indexOfSouthEasternNeighbour != -1)
                    {
                        if (gv.mod.moduleAreasObjects[relevantIndices[i]].Filename.Contains(gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].Filename))

                        //if ((seamlessModififierMaxX > 0) && (seamlessModififierMaxY > 0) && !situationFound)
                        {
                            situationFound = true;
                            p.LocationX = p.LocationX + gv.mod.currentArea.MapSizeX;
                            p.LocationY = p.LocationY + gv.mod.currentArea.MapSizeY;

                        }
                    }

                    //north
                    if (indexOfNorthernNeighbour != -1)
                    {
                        if (gv.mod.moduleAreasObjects[relevantIndices[i]].Filename.Contains(gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].Filename))

                        //if ((seamlessModififierMinY > 0) && !situationFound)
                        {
                            situationFound = true;
                            p.LocationY = p.LocationY - gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeY;

                        }
                    }

                    //south
                    if (indexOfSouthernNeighbour != -1)
                    {
                        if (gv.mod.moduleAreasObjects[relevantIndices[i]].Filename.Contains(gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].Filename))

                        //if ((seamlessModififierMaxY > 0) && !situationFound)
                        {
                            situationFound = true;
                            p.LocationY = p.LocationY + gv.mod.currentArea.MapSizeY;

                        }
                    }

                    //west
                    if (indexOfWesternNeighbour != -1)
                    {
                        if (gv.mod.moduleAreasObjects[relevantIndices[i]].Filename.Contains(gv.mod.moduleAreasObjects[indexOfWesternNeighbour].Filename))

                        //if ((seamlessModififierMinX > 0) && !situationFound)
                        {
                            situationFound = true;
                            p.LocationX = p.LocationX - gv.mod.moduleAreasObjects[relevantIndices[i]].MapSizeX;
                        }
                    }

                    //east
                    if (indexOfEasternNeighbour != -1)
                    {
                        if (gv.mod.moduleAreasObjects[relevantIndices[i]].Filename.Contains(gv.mod.moduleAreasObjects[indexOfEasternNeighbour].Filename))

                        //if ((seamlessModififierMaxX > 0) && !situationFound)
                        {
                            situationFound = true;
                            p.LocationX = p.LocationX + gv.mod.currentArea.MapSizeX;
                        }
                    }

                    //hurgh555
                    try
                    {
                        gv.mod.currentArea.Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX].isCentreOfLightCircle = true;
                        gv.mod.currentArea.Tiles[(p.LocationY + 1) * gv.mod.currentArea.MapSizeX + p.LocationX].isOtherPartOfLightCircle = true;
                        gv.mod.currentArea.Tiles[(p.LocationY + 1) * gv.mod.currentArea.MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                        gv.mod.currentArea.Tiles[(p.LocationY + 1) * gv.mod.currentArea.MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                        gv.mod.currentArea.Tiles[(p.LocationY - 1) * gv.mod.currentArea.MapSizeX + p.LocationX].isOtherPartOfLightCircle = true;
                        gv.mod.currentArea.Tiles[(p.LocationY - 1) * gv.mod.currentArea.MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                        gv.mod.currentArea.Tiles[(p.LocationY - 1) * gv.mod.currentArea.MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                        gv.mod.currentArea.Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                        gv.mod.currentArea.Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                        gv.mod.currentArea.Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX].lightRadius = 1;
                    }
                    catch
                    { }

                    
                }
            }
    */
            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            //illuminate around props, new

            for (int h = 0; h < relevantIndices.Count; h++)
            {//2

                int indexOfNorthernNeighbour2 = -1;
                int indexOfSouthernNeighbour2 = -1;
                int indexOfEasternNeighbour2 = -1;
                int indexOfWesternNeighbour2 = -1;
                int indexOfNorthEasternNeighbour2 = -1;
                int indexOfNorthWesternNeighbour2 = -1;
                int indexOfSouthEasternNeighbour2 = -1;
                int indexOfSouthWesternNeighbour2 = -1;
                int indexOfCurrentArea2 = -1;

                //get neighbours of the check map
                for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[relevantIndices[h]].Filename)
                    {
                        indexOfCurrentArea2 = i;
                    }
                }


                if ((gv.mod.moduleAreasObjects[relevantIndices[h]].northernNeighbourArea != ""))
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[relevantIndices[h]].northernNeighbourArea)
                        {
                            indexOfNorthernNeighbour2 = i;
                        }
                    }

                    if (gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].easternNeighbourArea != "")
                    {
                        for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                        {
                            if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].easternNeighbourArea)
                            {
                                indexOfNorthEasternNeighbour2 = i;
                            }
                        }
                    }

                    if (gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].westernNeighbourArea != "")
                    {
                        for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                        {
                            if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].westernNeighbourArea)
                            {
                                indexOfNorthWesternNeighbour2 = i;
                            }
                        }
                    }
                }

                if ((gv.mod.moduleAreasObjects[relevantIndices[h]].southernNeighbourArea != ""))
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[relevantIndices[h]].southernNeighbourArea)
                        {
                            indexOfSouthernNeighbour2 = i;
                        }
                    }

                    if (gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].easternNeighbourArea != "")
                    {
                        for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                        {
                            if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].easternNeighbourArea)
                            {
                                indexOfSouthEasternNeighbour2 = i;
                            }
                        }
                    }

                    if (gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].westernNeighbourArea != "")
                    {
                        for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                        {
                            if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].westernNeighbourArea)
                            {
                                indexOfSouthWesternNeighbour2 = i;
                            }
                        }
                    }
                }

                if ((gv.mod.moduleAreasObjects[relevantIndices[h]].westernNeighbourArea != ""))
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[relevantIndices[h]].westernNeighbourArea)
                        {
                            indexOfWesternNeighbour2 = i;
                        }
                    }

                    if (gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].northernNeighbourArea != "")
                    {
                        for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                        {
                            if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].northernNeighbourArea)
                            {
                                indexOfNorthWesternNeighbour2 = i;
                            }
                        }
                    }

                    if (gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].southernNeighbourArea != "")
                    {
                        for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                        {
                            if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].southernNeighbourArea)
                            {
                                indexOfSouthWesternNeighbour2 = i;
                            }
                        }
                    }
                }

                if ((gv.mod.moduleAreasObjects[relevantIndices[h]].easternNeighbourArea != ""))
                {
                    for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                    {
                        if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[relevantIndices[h]].easternNeighbourArea)
                        {
                            indexOfEasternNeighbour2 = i;
                        }
                    }

                    if (gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].northernNeighbourArea != "")
                    {
                        for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                        {
                            if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].northernNeighbourArea)
                            {
                                indexOfNorthEasternNeighbour2 = i;
                            }
                        }
                    }

                    if (gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].southernNeighbourArea != "")
                    {
                        for (int i = 0; i < gv.mod.moduleAreasObjects.Count; i++)
                        {
                            if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].southernNeighbourArea)
                            {
                                indexOfSouthEasternNeighbour2 = i;
                            }
                        }
                    }
                }

                int backupLocationX = -1;
                int backupLocationY = -1;

                foreach (Prop p in gv.mod.moduleAreasObjects[relevantIndices[h]].Props)
                {//3
                    if (p.isLight)
                    {
                        //hurgh27
                        //List<Tile> tilesOfThisLightSource = new List<Tile>();
                        tilesOfThisLightSource.Clear();
                        int minX2 = p.LocationX - 2;
                        //if (minX2 < -seamlessModififierMinX) { minX2 = -seamlessModififierMinX; }
                        int minY2 = p.LocationY - 2;
                        //if (minY2 < -seamlessModififierMinY) { minY2 = -seamlessModififierMinY; }

                        int maxX2 = p.LocationX + 2;
                        //if (maxX2 > this.gv.mod.currentArea.MapSizeX - 1 + seamlessModififierMaxX) { maxX2 = this.gv.mod.currentArea.MapSizeX - 1 + seamlessModififierMaxX; }
                        int maxY2 = p.LocationY + 2;
                        //if (maxY2 > this.gv.mod.currentArea.MapSizeY - 1 + seamlessModififierMaxY) { maxY2 = this.gv.mod.currentArea.MapSizeY - 1 + seamlessModififierMaxY; }

                        Coordinate propCoord = new Coordinate();
                        propCoord.X = p.LocationX;
                        propCoord.Y = p.LocationY;

                        //these 25 squares are added in a list, column per column, starting from 2 sqaures left of center
                        //1 | 6 | 11 | 16 | 21
                        //2 | 7 | 12 | 17 | 22
                        //3 | 8 | 13 | 18 | 23
                        //4 | 9 | 14 | 19 | 24
                        //5 | 10 | 15 | 20 | 25
                        for (int xx = minX2; xx <= maxX2; xx++)
                        {
                            for (int yy = minY2; yy <= maxY2; yy++)
                            {
                                //YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
                                bool situationFound = false;
                                bool drawTile = true;
                                int index = -1;
                                Tile tile = new Tile();
                                bool addedTileToListAlready = false;

                                //nine plus sixteen situations where a tile can be:
                                //tile on north-western map (diagonal situation)
                                if ((xx == -1) && (yy == -1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfNorthWesternNeighbour2 != -1)
                                    {
                                        int transformedX = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeX + xx;
                                        int transformedY = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeY + yy;
                                        gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeX + transformedX].isCentreOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on north-western map (diagonal situation), level 2
                                else if ((xx <= -1) && (yy <= -1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfNorthWesternNeighbour2 != -1)
                                    {
                                        int transformedX = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeX + xx;
                                        int transformedY = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeY + yy;
                                        gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on south-westernmap (diagonal situation)
                                if ((xx == -1) && (yy == (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfSouthWesternNeighbour2 != -1)
                                    {
                                        int transformedX = gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].MapSizeX + xx;
                                        int transformedY = yy - gv.mod.currentArea.MapSizeY;
                                        gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].MapSizeX + transformedX].isCentreOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on south-westernmap (diagonal situation), level 2
                                else if ((xx <= -1) && (yy >= (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfSouthWesternNeighbour2 != -1)
                                    {
                                        int transformedX = gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].MapSizeX + xx;
                                        int transformedY = yy - gv.mod.currentArea.MapSizeY;
                                        gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on south-easternmap (diagonal situation)
                                if ((xx == (gv.mod.currentArea.MapSizeX - 1) + 1) && (yy == (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfSouthEasternNeighbour2 != -1)
                                    {
                                        int transformedX = xx - gv.mod.currentArea.MapSizeX;
                                        int transformedY = yy - gv.mod.currentArea.MapSizeY;
                                        gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].MapSizeX + transformedX].isCentreOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on south-easternmap (diagonal situation). level 2
                                else if ((xx >= (gv.mod.currentArea.MapSizeX - 1) + 1) && (yy >= (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfSouthEasternNeighbour2 != -1)
                                    {
                                        int transformedX = xx - gv.mod.currentArea.MapSizeX;
                                        int transformedY = yy - gv.mod.currentArea.MapSizeY;
                                        gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on north-easternmap (diagonal situation)
                                if ((xx == (gv.mod.currentArea.MapSizeX - 1) + 1) && (yy == -1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfNorthEasternNeighbour2 != -1)
                                    {
                                        int transformedX = xx - gv.mod.currentArea.MapSizeX;
                                        int transformedY = gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].MapSizeY + yy;
                                        gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].MapSizeX + transformedX].isCentreOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on north-easternmap (diagonal situation), level 2
                                else if ((xx >= (gv.mod.currentArea.MapSizeX - 1) + 1) && (yy <= -1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfNorthEasternNeighbour2 != -1)
                                    {
                                        int transformedX = xx - gv.mod.currentArea.MapSizeX;
                                        int transformedY = gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].MapSizeY + yy;
                                        gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on western map
                                if ((xx == -1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfWesternNeighbour2 != -1)
                                    {
                                        int transformedX = gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + xx;
                                        int transformedY = yy;
                                        if (yy < 0)
                                        {
                                            //check
                                            transformedY = p.LocationY + yy;
                                        }
                                        if (yy > (gv.mod.currentArea.MapSizeY - 1))
                                        {
                                            //check
                                            transformedY = yy - p.LocationY;
                                        }
                                        if ((p.LocationX == 0) && (yy != p.LocationY + 2) && (yy != p.LocationY - 2))
                                        {
                                            gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + transformedX].isCentreOfLightCircle = true;
                                        }
                                        else
                                        {
                                            gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        }
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on western map, level 2
                                else if ((xx < -1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfWesternNeighbour2 != -1)
                                    {
                                        int transformedX = gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + xx;
                                        int transformedY = yy;
                                        if (yy < 0)
                                        {
                                            //check
                                            transformedY = p.LocationY + yy;
                                        }
                                        if (yy > (gv.mod.currentArea.MapSizeY - 1))
                                        {
                                            //check
                                            transformedY = yy - p.LocationY;
                                        }

                                        gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }



                                //tile on southern map
                                if ((yy == (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfSouthernNeighbour2 != -1)
                                    {
                                        int transformedX = xx;
                                        if (xx < 0)
                                        {
                                            //check
                                            transformedX = p.LocationX + xx;
                                        }
                                        if (xx > (gv.mod.currentArea.MapSizeX - 1))
                                        {
                                            //check
                                            transformedX = xx - p.LocationX;
                                        }
                                        int transformedY = yy - gv.mod.currentArea.MapSizeY;
                                        if ((p.LocationY == (gv.mod.currentArea.MapSizeY - 1)) && (xx != p.LocationX + 2) && (xx != p.LocationX - 2))
                                        {
                                            gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].MapSizeX + transformedX].isCentreOfLightCircle = true;
                                        }
                                        else
                                        {
                                            gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        }
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on southern map, level 2
                                else if ((yy > (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfSouthernNeighbour2 != -1)
                                    {
                                        int transformedX = xx;
                                        if (xx < 0)
                                        {
                                            //check
                                            transformedX = p.LocationX + xx;
                                        }
                                        if (xx > (gv.mod.currentArea.MapSizeX - 1))
                                        {
                                            //check
                                            transformedX = xx - p.LocationX;
                                        }
                                        int transformedY = yy - gv.mod.currentArea.MapSizeY;
                                        gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on eastern map
                                if ((xx == (gv.mod.currentArea.MapSizeX - 1) + 1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfEasternNeighbour2 != -1)
                                    {
                                        int transformedX = xx - gv.mod.currentArea.MapSizeX;
                                        int transformedY = yy;
                                        if (yy < 0)
                                        {
                                            //check
                                            transformedY = p.LocationY + yy;
                                        }
                                        if (yy > (gv.mod.currentArea.MapSizeY - 1))
                                        {
                                            //check
                                            transformedY = yy - p.LocationY;
                                        }
                                        if ((p.LocationX == (gv.mod.currentArea.MapSizeX - 1)) && (yy != p.LocationY + 2) && (yy != p.LocationY - 2))
                                        {
                                            gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].MapSizeX + transformedX].isCentreOfLightCircle = true;
                                        }
                                        else
                                        {
                                            gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        }
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on eastern map, level 2
                                else if ((xx > (gv.mod.currentArea.MapSizeX - 1) + 1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfEasternNeighbour2 != -1)
                                    {
                                        int transformedX = xx - gv.mod.currentArea.MapSizeX;
                                        int transformedY = yy;
                                        if (yy < 0)
                                        {
                                            //check
                                            transformedY = p.LocationY + yy;
                                        }
                                        if (yy > (gv.mod.currentArea.MapSizeY - 1))
                                        {
                                            //check
                                            transformedY = yy - p.LocationY;
                                        }
                                        gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on northern map
                                if ((yy == -1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfNorthernNeighbour2 != -1)
                                    {
                                        int transformedX = xx;
                                        if (xx < 0)
                                        {
                                            //check
                                            transformedX = p.LocationX + xx;
                                        }
                                        if (xx > (gv.mod.currentArea.MapSizeX - 1))
                                        {
                                            //check
                                            transformedX = xx - p.LocationX;
                                        }
                                        int transformedY = gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeY + yy;
                                        if ((p.LocationY == 0) && (xx != p.LocationX + 2) && (xx != p.LocationX - 2))
                                        {
                                            gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeX + transformedX].isCentreOfLightCircle = true;
                                        }
                                        else
                                        {
                                            gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        }
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }

                                //tile on northern map, level 2
                                else if ((yy < -1) && (!situationFound))
                                {
                                    situationFound = true;
                                    if (indexOfNorthernNeighbour2 != -1)
                                    {
                                        int transformedX = xx;
                                        if (xx < 0)
                                        {
                                            //check
                                            transformedX = p.LocationX + xx;
                                        }
                                        if (xx > (gv.mod.currentArea.MapSizeX - 1))
                                        {
                                            //check
                                            transformedX = xx - p.LocationX;
                                        }
                                        int transformedY = gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeY + yy;
                                        gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeX + transformedX]);
                                        gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeX + transformedX].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour2].MapSizeX + transformedX].lightSourceCoordinate.Add(propCoord);
                                        addedTileToListAlready = true;
                                    }
                                }
                                //tile is on current map
                                //adjust for relevant index map of prop as well as prop square instead of party square
                                if (!situationFound)
                                {
                                    Coordinate illuSquare = new Coordinate(xx, yy);
                                    Coordinate propSquare = new Coordinate(p.LocationX, p.LocationY);
                                    if ((xx >= 0) && (yy >= 0) && (xx <= gv.mod.moduleAreasObjects[relevantIndices[h]].MapSizeX - 1) && (yy <= gv.mod.moduleAreasObjects[relevantIndices[h]].MapSizeY - 1))
                                    {
                                        addedTileToListAlready = true;
                                        tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[relevantIndices[h]].Tiles[yy * gv.mod.moduleAreasObjects[relevantIndices[h]].MapSizeX + xx]);
                                        gv.mod.moduleAreasObjects[relevantIndices[h]].Tiles[yy * gv.mod.moduleAreasObjects[relevantIndices[h]].MapSizeX + xx].tileLightSourceTag.Add(p.PropTag);
                                        gv.mod.moduleAreasObjects[relevantIndices[h]].Tiles[yy * gv.mod.moduleAreasObjects[relevantIndices[h]].MapSizeX + xx].lightSourceCoordinate.Add(propCoord);
                                        //note: this also added the focal point

                                        if (getDistance(illuSquare, propSquare) <= 1)
                                        {
                                            gv.mod.moduleAreasObjects[relevantIndices[h]].Tiles[yy * gv.mod.moduleAreasObjects[relevantIndices[h]].MapSizeX + xx].isCentreOfLightCircle = true;
                                            //mod.currentArea.Tiles[yy * mod.currentArea.MapSizeX + xx].Visible = true;
                                        }
                                        else
                                        {
                                            gv.mod.moduleAreasObjects[relevantIndices[h]].Tiles[yy * gv.mod.moduleAreasObjects[relevantIndices[h]].MapSizeX + xx].isOtherPartOfLightCircle = true;
                                        }
                                    }
                                }

                                if (!addedTileToListAlready)
                                {
                                    //add placebo entry into list as this tile cannot be addresed without error
                                    //addign keeps the supposed order of th tiles in the lighting scheme intact
                                    Tile placeboTile = new Tile();
                                    tilesOfThisLightSource.Add(placeboTile);
                                }
                            }
                        }

                        //current prop position is centre of light
                        gv.mod.moduleAreasObjects[relevantIndices[h]].Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX].isOtherPartOfLightCircle = false;
                        gv.mod.moduleAreasObjects[relevantIndices[h]].Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX].isCentreOfLightCircle = false;
                        gv.mod.moduleAreasObjects[relevantIndices[h]].Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX].isFocalPoint = true;
                        gv.mod.moduleAreasObjects[relevantIndices[h]].Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX].hasHalo = p.hasHalo;
                        //gv.mod.moduleAreasObjects[relevantIndices[h]].Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX].lightRadius = 1;

                        //all tiles around the currnt prop are in list now, go through list to set up LoS effect (to use for draw routine later)
                        //TODOXXX
                        //TODO rset isLit values and props they belogn to for tile son clearillumination call of doupdate, done
                        //these 25 squares are added in a list, column per column, starting from 2 sqaures left of center
                        //0 | 5 | 10 | 15 | 20
                        //1 | 6 | 11 | 16 | 21
                        //2 | 7 | 12 | 17 | 22
                        //3 | 8 | 13 | 18 | 23
                        //4 | 9 | 14 | 19 | 24

                        //priority number:
                        //0: not lit (ie hidden by LoS)
                        //1: N0, N4, S0, S4
                        //2: N1,N3,S1,S3,E1,E3,W1,W3
                        //3: N2,S2,E2,W2
                        //4: NE,NW,SE,SW
                        //5: N,S,E,W
                        //6: center

                        for (int tCount = 0; tCount <= 24; tCount++)
                        {
                            if (tCount == 0)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N0");
                                if (!tilesOfThisLightSource[6].LoSBlocked)
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(1);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 5)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N1");
                                if ((!tilesOfThisLightSource[6].LoSBlocked) && (!tilesOfThisLightSource[11].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(2);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 10)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N2");
                                if ((!tilesOfThisLightSource[11].LoSBlocked) && (!tilesOfThisLightSource[11].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(3);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 15)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N3");
                                if ((!tilesOfThisLightSource[11].LoSBlocked) && (!tilesOfThisLightSource[16].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(2);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 20)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N4");
                                if ((!tilesOfThisLightSource[16].LoSBlocked) && (!tilesOfThisLightSource[16].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(1);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 21)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("E1");
                                if ((!tilesOfThisLightSource[16].LoSBlocked) && (!tilesOfThisLightSource[17].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(2);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 22)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("E2");
                                if ((!tilesOfThisLightSource[17].LoSBlocked) && (!tilesOfThisLightSource[17].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(3);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                    continue;
                                }
                            }
                            else if (tCount == 23)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("E3");
                                if ((!tilesOfThisLightSource[17].LoSBlocked) && (!tilesOfThisLightSource[18].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(2);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 24)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S4");
                                if ((!tilesOfThisLightSource[18].LoSBlocked) && (!tilesOfThisLightSource[18].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(1);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 19)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S3");
                                if ((!tilesOfThisLightSource[13].LoSBlocked) && (!tilesOfThisLightSource[18].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(2);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 14)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S2");
                                if ((!tilesOfThisLightSource[13].LoSBlocked) && (!tilesOfThisLightSource[13].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(3);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 9)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S1");
                                if ((!tilesOfThisLightSource[8].LoSBlocked) && (!tilesOfThisLightSource[13].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(2);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 4)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S0");
                                if ((!tilesOfThisLightSource[8].LoSBlocked) && (!tilesOfThisLightSource[8].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(1);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 3)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("W3");
                                if ((!tilesOfThisLightSource[7].LoSBlocked) && (!tilesOfThisLightSource[8].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(2);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 2)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("W2");
                                if ((!tilesOfThisLightSource[7].LoSBlocked) && (!tilesOfThisLightSource[7].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(3);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 1)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("W1");
                                if ((!tilesOfThisLightSource[6].LoSBlocked) && (!tilesOfThisLightSource[7].LoSBlocked))
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(true);
                                    tilesOfThisLightSource[tCount].priority.Add(2);
                                }
                                else
                                {
                                    tilesOfThisLightSource[tCount].isLit.Add(false);
                                    tilesOfThisLightSource[tCount].priority.Add(0);
                                }
                                continue;
                            }
                            else if (tCount == 6)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("NW");
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(4);
                                continue;
                            }
                            else if (tCount == 11)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N");
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(5);
                                continue;
                            }
                            else if (tCount == 16)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("NE");
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(4);
                                continue;
                            }
                            else if (tCount == 17)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("E");
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(5);
                                continue;
                            }
                            else if (tCount == 18)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("SE");
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(4);
                                continue;
                            }
                            else if (tCount == 13)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S");
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(5);
                                continue;
                            }
                            else if (tCount == 8)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("SW");
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(4);
                                continue;
                            }
                            else if (tCount == 7)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("W");
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(5);
                                continue;
                            }
                            else if (tCount == 12)
                            {
                                tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("Center");
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(6);
                                continue;
                            }
                        }
                    }
                }
            }

            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

            //illuminate around the party
            tilesOfThisLightSource.Clear();
            int minX = gv.mod.PlayerLocationX - 2;
            //if (minX < -seamlessModififierMinX) { minX = -seamlessModififierMinX; }
            int minY = gv.mod.PlayerLocationY - 2;
            //if (minY < -seamlessModififierMinY) { minY = -seamlessModififierMinY; }

            int maxX = gv.mod.PlayerLocationX + 2;
            //if (maxX > this.gv.mod.currentArea.MapSizeX - 1 + seamlessModififierMaxX) { maxX = this.gv.mod.currentArea.MapSizeX - 1 + seamlessModififierMaxX; }
            int maxY = gv.mod.PlayerLocationY + 2;
            //if (maxY > this.gv.mod.currentArea.MapSizeY - 1 + seamlessModififierMaxY) { maxY = this.gv.mod.currentArea.MapSizeY - 1 + seamlessModififierMaxY; }

            /*
            int indexOfNorthernNeighbour2 = -1;
            int indexOfSouthernNeighbour2 = -1;
            int indexOfEasternNeighbour2 = -1;
            int indexOfWesternNeighbour2 = -1;
            int indexOfNorthEasternNeighbour2 = -1;
            int indexOfNorthWesternNeighbour2 = -1;
            int indexOfSouthEasternNeighbour2 = -1;
            int indexOfSouthWesternNeighbour2 = -1;
            int indexOfCurrentArea2 = -1;
            */

            for (int xx = minX; xx <= maxX; xx++)
            {
                for (int yy = minY; yy <= maxY; yy++)
                {
                    //YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
                    bool situationFound = false;
                    bool addedTileToListAlready = false;
                    bool drawTile = true;
                    int index = -1;
                    Tile tile = new Tile();

                    //nine plus sixteen situations where a tile can be:
                    //tile on north-western map (diagonal situation)
                    if ((xx == -1) && (yy == -1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfNorthWesternNeighbour != -1)
                        {
                            int transformedX = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeX + xx;
                            int transformedY = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeY + yy;
                            gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeX + transformedX].isCentreOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on north-western map (diagonal situation), level 2
                    else if ((xx <= -1) && (yy <= -1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfNorthWesternNeighbour != -1)
                        {
                            int transformedX = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeX + xx;
                            int transformedY = gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeY + yy;
                            gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthWesternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on south-westernmap (diagonal situation)
                    if ((xx == -1) && (yy == (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfSouthWesternNeighbour != -1)
                        {
                            int transformedX = gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].MapSizeX + xx;
                            int transformedY = yy - gv.mod.currentArea.MapSizeY;
                            gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].MapSizeX + transformedX].isCentreOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on south-westernmap (diagonal situation), level 2
                    else if ((xx <= -1) && (yy >= (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfSouthWesternNeighbour != -1)
                        {
                            int transformedX = gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].MapSizeX + xx;
                            int transformedY = yy - gv.mod.currentArea.MapSizeY;
                            gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthWesternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on south-easternmap (diagonal situation)
                    if ((xx == (gv.mod.currentArea.MapSizeX - 1) + 1) && (yy == (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfSouthEasternNeighbour != -1)
                        {
                            int transformedX = xx - gv.mod.currentArea.MapSizeX;
                            int transformedY = yy - gv.mod.currentArea.MapSizeY;
                            gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].MapSizeX + transformedX].isCentreOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on south-easternmap (diagonal situation). level 2
                    else if ((xx >= (gv.mod.currentArea.MapSizeX - 1) + 1) && (yy >= (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfSouthEasternNeighbour != -1)
                        {
                            int transformedX = xx - gv.mod.currentArea.MapSizeX;
                            int transformedY = yy - gv.mod.currentArea.MapSizeY;
                            gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthEasternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on north-easternmap (diagonal situation)
                    if ((xx == (gv.mod.currentArea.MapSizeX - 1) + 1) && (yy == -1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfNorthEasternNeighbour != -1)
                        {
                            int transformedX = xx - gv.mod.currentArea.MapSizeX;
                            int transformedY = gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].MapSizeY + yy;
                            gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].MapSizeX + transformedX].isCentreOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on north-easternmap (diagonal situation), level 2
                    else if ((xx >= (gv.mod.currentArea.MapSizeX - 1) + 1) && (yy <= -1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfNorthEasternNeighbour != -1)
                        {
                            int transformedX = xx - gv.mod.currentArea.MapSizeX;
                            int transformedY = gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].MapSizeY + yy;
                            gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthEasternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on western map
                    if ((xx == -1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfWesternNeighbour != -1)
                        {
                            int transformedX = gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + xx;
                            int transformedY = yy;
                            if (yy < 0)
                            {
                                //check
                                transformedY = gv.mod.PlayerLocationY + yy;
                            }
                            if (yy > (gv.mod.currentArea.MapSizeY - 1))
                            {
                                //check
                                transformedY = yy - gv.mod.PlayerLocationY;
                            }
                            if ( (gv.mod.PlayerLocationX == 0) && (yy != gv.mod.PlayerLocationY + 2) && (yy != gv.mod.PlayerLocationY - 2))
                            {
                                gv.mod.moduleAreasObjects[indexOfWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + transformedX].isCentreOfLightCircle = true;
                            }
                            else
                            {
                                gv.mod.moduleAreasObjects[indexOfWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            }
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on western map, level 2
                    else if ((xx < -1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfWesternNeighbour != -1)
                        {
                            int transformedX = gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + xx;
                            int transformedY = yy;
                            if (yy < 0)
                            {
                                //check
                                transformedY = gv.mod.PlayerLocationY + yy;
                            }
                            if (yy > (gv.mod.currentArea.MapSizeY-1))
                            {
                                //check
                                transformedY = yy - gv.mod.PlayerLocationY;
                            }

                            gv.mod.moduleAreasObjects[indexOfWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfWesternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfWesternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }



                    //tile on southern map
                    if ((yy == (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfSouthernNeighbour != -1)
                        {
                            int transformedX = xx;
                            if (xx < 0)
                            {
                                //check
                                transformedX = gv.mod.PlayerLocationX + xx;
                            }
                            if (xx > (gv.mod.currentArea.MapSizeX - 1))
                            {
                                //check
                                transformedX = xx - gv.mod.PlayerLocationX;
                            }
                            int transformedY = yy - gv.mod.currentArea.MapSizeY;
                            if ((gv.mod.PlayerLocationY == (gv.mod.currentArea.MapSizeY - 1)) && (xx != gv.mod.PlayerLocationX + 2) && (xx != gv.mod.PlayerLocationX - 2))
                            {
                                gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].MapSizeX + transformedX].isCentreOfLightCircle = true;
                            }
                            else
                            {
                                gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            }
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on southern map, level 2
                    else if ((yy > (gv.mod.currentArea.MapSizeY - 1) + 1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfSouthernNeighbour != -1)
                        {
                            int transformedX = xx;
                            if (xx < 0)
                            {
                                //check
                                transformedX = gv.mod.PlayerLocationX + xx;
                            }
                            if (xx > (gv.mod.currentArea.MapSizeX - 1))
                            {
                                //check
                                transformedX = xx - gv.mod.PlayerLocationX;
                            }
                            int transformedY = yy - gv.mod.currentArea.MapSizeY;
                            gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfSouthernNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }



                    //tile on eastern map
                    if ((xx == (gv.mod.currentArea.MapSizeX - 1) + 1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfEasternNeighbour != -1)
                        {
                            int transformedX = xx - gv.mod.currentArea.MapSizeX;
                            int transformedY = yy;
                            if (yy < 0)
                            {
                                //check
                                transformedY = gv.mod.PlayerLocationY + yy;
                            }
                            if (yy > (gv.mod.currentArea.MapSizeY - 1))
                            {
                                //check
                                transformedY = yy - gv.mod.PlayerLocationY;
                            }
                            if ((gv.mod.PlayerLocationX == (gv.mod.currentArea.MapSizeX-1)) && (yy != gv.mod.PlayerLocationY + 2) && (yy != gv.mod.PlayerLocationY - 2))
                            {
                                gv.mod.moduleAreasObjects[indexOfEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour].MapSizeX + transformedX].isCentreOfLightCircle = true;
                            }
                            else
                            {
                                gv.mod.moduleAreasObjects[indexOfEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            }
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on eastern map, level 2
                    else if ((xx > (gv.mod.currentArea.MapSizeX - 1) + 1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfEasternNeighbour != -1)
                        {
                            int transformedX = xx - gv.mod.currentArea.MapSizeX;
                            int transformedY = yy;
                            if (yy < 0)
                            {
                                //check
                                transformedY = gv.mod.PlayerLocationY + yy;
                            }
                            if (yy > (gv.mod.currentArea.MapSizeY - 1))
                            {
                                //check
                                transformedY = yy - gv.mod.PlayerLocationY;
                            }
                            gv.mod.moduleAreasObjects[indexOfEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfEasternNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfEasternNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on northern map
                    if ((yy == -1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfNorthernNeighbour != -1)
                        {
                            int transformedX = xx;
                            if (xx < 0)
                            {
                                //check
                                transformedX = gv.mod.PlayerLocationX + xx;
                            }
                            if (xx > (gv.mod.currentArea.MapSizeX - 1))
                            {
                                //check
                                transformedX = xx - gv.mod.PlayerLocationX;
                            }
                            int transformedY = gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeY + yy;
                            if ( (gv.mod.PlayerLocationY == 0) && (xx != gv.mod.PlayerLocationX + 2) && (xx != gv.mod.PlayerLocationX - 2) )
                            {
                                gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeX + transformedX].isCentreOfLightCircle = true;
                            }
                            else
                            {
                                gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            }
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }

                    //tile on northern map, level 2
                    else if ((yy < -1) && (!situationFound))
                    {
                        situationFound = true;
                        if (indexOfNorthernNeighbour != -1)
                        {
                            int transformedX = xx;
                            if (xx < 0)
                            {
                                //check
                                transformedX = gv.mod.PlayerLocationX + xx;
                            }
                            if (xx > (gv.mod.currentArea.MapSizeX - 1))
                            {
                                //check
                                transformedX = xx - gv.mod.PlayerLocationX;
                            }
                            int transformedY = gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeY + yy;
                            gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeX + transformedX].isOtherPartOfLightCircle = true;
                            tilesOfThisLightSource.Add(gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeX + transformedX]);
                            gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].Tiles[transformedY * gv.mod.moduleAreasObjects[indexOfNorthernNeighbour].MapSizeX + transformedX].tileLightSourceTag.Add("party");
                            addedTileToListAlready = true;
                        }
                    }
                    //tile is on current map
                    if (!situationFound)
                    {
                        Coordinate illuSquare = new Coordinate(xx,yy);
                        Coordinate partySquare = new Coordinate(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY);

                        if (getDistance(illuSquare, partySquare) <= 1)
                        {
                            gv.mod.currentArea.Tiles[yy * gv.mod.currentArea.MapSizeX + xx].isCentreOfLightCircle = true;
                            //mod.currentArea.Tiles[yy * mod.currentArea.MapSizeX + xx].Visible = true;
                        }
                        else
                        {
                            gv.mod.currentArea.Tiles[yy * gv.mod.currentArea.MapSizeX + xx].isOtherPartOfLightCircle = true;
                        }
                        tilesOfThisLightSource.Add(gv.mod.currentArea.Tiles[yy * gv.mod.currentArea.MapSizeX + xx]);
                        gv.mod.currentArea.Tiles[yy * gv.mod.currentArea.MapSizeX + xx].tileLightSourceTag.Add("party");
                        addedTileToListAlready = true;
                    }

                    //placebo catch
                    if (!addedTileToListAlready)
                    {
                        //add placebo entry into list as this tile cannot be addresed without error
                        //addign keeps the supposed order of th tiles in the lighting scheme intact
                        Tile placeboTile = new Tile();
                        tilesOfThisLightSource.Add(placeboTile);
                    }
                }
            }

             
        //current player position is centre of light
            gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].isOtherPartOfLightCircle = false;
            gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].isCentreOfLightCircle = false;
            gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].isFocalPoint = true;
            gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].hasHalo = true;
            gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].lightRadius = 1;
            tilesOfThisLightSource.Add(gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX]);
            gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].tileLightSourceTag.Add("party");


            //0 | 5 | 10 | 15 | 20
            //1 | 6 | 11 | 16 | 21
            //2 | 7 | 12 | 17 | 22
            //3 | 8 | 13 | 18 | 23
            //4 | 9 | 14 | 19 | 24
            //tilesOfThisLightSource.Clear();

            //priority number:
            //0: not lit (ie hidden by LoS)
            //1: N0, N4, S0, S4
            //2: N1,N3,S1,S3,E1,E3,W1,W3
            //3: N2,S2,E2,W2
            //4: NE,NW,SE,SW
            //5: N,S,E,W
            //6: center

            for (int tCount = 0; tCount <= 24; tCount++)
            {
                    if (tCount == 0)
                    {
                        tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N0");
                        if (!tilesOfThisLightSource[6].LoSBlocked)
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(1);
                        }
                        else
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(false);
                            tilesOfThisLightSource[tCount].priority.Add(0);
                        }
                        continue;
                    }
                    else if (tCount == 5)
                    {
                        tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N1");
                        if ((!tilesOfThisLightSource[6].LoSBlocked) && (!tilesOfThisLightSource[11].LoSBlocked))
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(2);
                        }
                        else
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(false);
                            tilesOfThisLightSource[tCount].priority.Add(0);
                        }
                        continue;
                    }
                    else if (tCount == 10)
                    {
                        tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N2");
                        if ((!tilesOfThisLightSource[11].LoSBlocked) && (!tilesOfThisLightSource[11].LoSBlocked))
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(3);
                        }
                        else
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(false);
                            tilesOfThisLightSource[tCount].priority.Add(0);
                        }
                        continue;
                    }
                    else if (tCount == 15)
                    {
                        tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N3");
                        if ((!tilesOfThisLightSource[11].LoSBlocked) && (!tilesOfThisLightSource[16].LoSBlocked))
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(2);
                        }
                        else
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(false);
                            tilesOfThisLightSource[tCount].priority.Add(0);
                        }
                        continue;
                    }
                    else if (tCount == 20)
                    {
                        tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N4");
                        if ((!tilesOfThisLightSource[16].LoSBlocked) && (!tilesOfThisLightSource[16].LoSBlocked))
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(1);
                        }
                        else
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(false);
                            tilesOfThisLightSource[tCount].priority.Add(0);
                        }
                        continue;
                    }
                    else if (tCount == 21)
                    {
                        tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("E1");
                        if ((!tilesOfThisLightSource[16].LoSBlocked) && (!tilesOfThisLightSource[17].LoSBlocked))
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(2);
                        }
                        else
                        {
                            tilesOfThisLightSource[tCount].isLit.Add(false);
                            tilesOfThisLightSource[tCount].priority.Add(0);
                        }
                        continue;
                    }
                    else if (tCount == 22)
                    {
                        tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("E2");
                    if ((!tilesOfThisLightSource[17].LoSBlocked) && (!tilesOfThisLightSource[17].LoSBlocked))
                    {
                        tilesOfThisLightSource[tCount].isLit.Add(true);
                        tilesOfThisLightSource[tCount].priority.Add(3);
                    }
                    else
                    {
                        tilesOfThisLightSource[tCount].isLit.Add(false);
                        tilesOfThisLightSource[tCount].priority.Add(0);
                        continue;
                    }
                        }
                        else if (tCount == 23)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("E3");
                            if ((!tilesOfThisLightSource[17].LoSBlocked) && (!tilesOfThisLightSource[18].LoSBlocked))
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(2);
                            }
                            else
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(false);
                                tilesOfThisLightSource[tCount].priority.Add(0);
                            }
                            continue;
                        }
                        else if (tCount == 24)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S4");
                            if ((!tilesOfThisLightSource[18].LoSBlocked) && (!tilesOfThisLightSource[18].LoSBlocked))
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(1);
                            }
                            else
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(false);
                                tilesOfThisLightSource[tCount].priority.Add(0);
                            }
                            continue;
                        }
                        else if (tCount == 19)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S3");
                            if ((!tilesOfThisLightSource[13].LoSBlocked) && (!tilesOfThisLightSource[18].LoSBlocked))
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(2);
                            }
                            else
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(false);
                                tilesOfThisLightSource[tCount].priority.Add(0);
                            }
                            continue;
                        }
                        else if (tCount == 14)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S2");
                            if ((!tilesOfThisLightSource[13].LoSBlocked) && (!tilesOfThisLightSource[13].LoSBlocked))
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(3);
                            }
                            else
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(false);
                                tilesOfThisLightSource[tCount].priority.Add(0);
                            }
                            continue;
                        }
                        else if (tCount == 9)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S1");
                            if ((!tilesOfThisLightSource[8].LoSBlocked) && (!tilesOfThisLightSource[13].LoSBlocked))
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(2);
                            }
                            else
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(false);
                                tilesOfThisLightSource[tCount].priority.Add(0);
                            }
                            continue;
                        }
                        else if (tCount == 4)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S0");
                            if ((!tilesOfThisLightSource[8].LoSBlocked) && (!tilesOfThisLightSource[8].LoSBlocked))
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(1);
                            }
                            else
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(false);
                                tilesOfThisLightSource[tCount].priority.Add(0);
                            }
                            continue;
                        }
                        else if (tCount == 3)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("W3");
                            if ((!tilesOfThisLightSource[7].LoSBlocked) && (!tilesOfThisLightSource[8].LoSBlocked))
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(2);
                            }
                            else
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(false);
                                tilesOfThisLightSource[tCount].priority.Add(0);
                            }
                            continue;
                        }
                        else if (tCount == 2)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("W2");
                            if ((!tilesOfThisLightSource[7].LoSBlocked) && (!tilesOfThisLightSource[7].LoSBlocked))
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(3);
                            }
                            else
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(false);
                                tilesOfThisLightSource[tCount].priority.Add(0);
                            }
                            continue;
                        }
                        else if (tCount == 1)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("W1");
                            if ((!tilesOfThisLightSource[6].LoSBlocked) && (!tilesOfThisLightSource[7].LoSBlocked))
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(true);
                                tilesOfThisLightSource[tCount].priority.Add(2);
                            }
                            else
                            {
                                tilesOfThisLightSource[tCount].isLit.Add(false);
                                tilesOfThisLightSource[tCount].priority.Add(0);
                            }
                            continue;
                        }
                        else if (tCount == 6)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("NW");
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(4);
                            continue;
                        }
                        else if (tCount == 11)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("N");
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(5);
                            continue;
                        }
                        else if (tCount == 16)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("NE");
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(4);
                            continue;
                        }
                        else if (tCount == 17)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("E");
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(5);
                            continue;
                        }
                        else if (tCount == 18)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("SE");
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(4);
                            continue;
                        }
                        else if (tCount == 13)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("S");
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(5);
                            continue;
                        }
                        else if (tCount == 8)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("SW");
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(4);
                            continue;
                        }
                        else if (tCount == 7)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("W");
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(5);
                            continue;
                        }
                        else if (tCount == 12)
                        {
                            tilesOfThisLightSource[tCount].tilePositionInLitArea.Add("Center");
                            tilesOfThisLightSource[tCount].isLit.Add(true);
                            tilesOfThisLightSource[tCount].priority.Add(6);
                            continue;
                        }
                    }
        
    //addedTileToListAlready = true;

    /*
    if (gv.mod.justTransitioned)
    {
        gv.mod.PlayerLocationX = backupPlayerLocationX;
        gv.mod.PlayerLocationY = backupPlayerLocationY;
    }
    */
}


        public void doIlluminationOld()
        {
            //or mayhapdo within prop moves?
            foreach (Prop p in gv.mod.currentArea.Props)
            {
                try
                {
                    gv.mod.currentArea.Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX].isCentreOfLightCircle = true;
                    gv.mod.currentArea.Tiles[(p.LocationY + 1) * gv.mod.currentArea.MapSizeX + p.LocationX].isOtherPartOfLightCircle = true;
                    gv.mod.currentArea.Tiles[(p.LocationY + 1) * gv.mod.currentArea.MapSizeX + p.LocationX -1].isOtherPartOfLightCircle = true;
                    gv.mod.currentArea.Tiles[(p.LocationY + 1) * gv.mod.currentArea.MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                    gv.mod.currentArea.Tiles[(p.LocationY - 1) * gv.mod.currentArea.MapSizeX + p.LocationX].isOtherPartOfLightCircle = true;
                    gv.mod.currentArea.Tiles[(p.LocationY - 1) * gv.mod.currentArea.MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                    gv.mod.currentArea.Tiles[(p.LocationY - 1) * gv.mod.currentArea.MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                    gv.mod.currentArea.Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX + 1].isOtherPartOfLightCircle = true;
                    gv.mod.currentArea.Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX - 1].isOtherPartOfLightCircle = true;
                    gv.mod.currentArea.Tiles[p.LocationY * gv.mod.currentArea.MapSizeX + p.LocationX].lightRadius = 1;
                }
                catch
                { }
            }
            try
            {
                gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].isCentreOfLightCircle = true;
                if (gv.mod.PlayerLocationY + 1 < gv.mod.currentArea.MapSizeY)
                {
                    gv.mod.currentArea.Tiles[(gv.mod.PlayerLocationY + 1) * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].isOtherPartOfLightCircle = true;
                }
                gv.mod.currentArea.Tiles[(gv.mod.PlayerLocationY + 1) * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX - 1].isOtherPartOfLightCircle = true;
                gv.mod.currentArea.Tiles[(gv.mod.PlayerLocationY + 1) * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX + 1].isOtherPartOfLightCircle = true;
                gv.mod.currentArea.Tiles[(gv.mod.PlayerLocationY - 1) * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].isOtherPartOfLightCircle = true;
                gv.mod.currentArea.Tiles[(gv.mod.PlayerLocationY - 1) * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX + 1].isOtherPartOfLightCircle = true;
                gv.mod.currentArea.Tiles[(gv.mod.PlayerLocationY - 1) * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX - 1].isOtherPartOfLightCircle = true;
                gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX + 1].isOtherPartOfLightCircle = true;
                gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX - 1].isOtherPartOfLightCircle = true;
                gv.mod.currentArea.Tiles[gv.mod.PlayerLocationY * gv.mod.currentArea.MapSizeX + gv.mod.PlayerLocationX].lightRadius = 1;
            }
            catch
            { }
        }

        public void adjustSpriteMainMapPositionToMakeItMoveIdependentlyFromPlayer()
        {
            float horizontalAdjustment = 0;
            float verticalAdjustment = 0;

            if ((gv.mod.PlayerLocationX != gv.mod.PlayerLastLocationX) || (gv.mod.PlayerLocationY != gv.mod.PlayerLastLocationY))
            {
                //moved east
                if (gv.mod.PlayerLocationX == gv.mod.PlayerLastLocationX + 1)
                {
                    horizontalAdjustment = -gv.squareSize;
                }

                //moved west
                if (gv.mod.PlayerLocationX == gv.mod.PlayerLastLocationX - 1)
                {
                    horizontalAdjustment = gv.squareSize;
                }

                //moved south
                if (gv.mod.PlayerLocationY == gv.mod.PlayerLastLocationY + 1)
                {
                    verticalAdjustment = -gv.squareSize;
                }

                //moved north
                if (gv.mod.PlayerLocationY == gv.mod.PlayerLastLocationY - 1)
                {
                    verticalAdjustment = gv.squareSize;
                }
            }

            if ((horizontalAdjustment != 0) || (verticalAdjustment != 0))
            {
                foreach (Sprite spr in gv.screenMainMap.spriteList)
                {
                    if (spr.movesIndependentlyFromPlayerPosition)
                    {
                        spr.position.X += horizontalAdjustment;
                        spr.position.Y += verticalAdjustment;
                    }
                }
            }
        }


        public void setToBorderPixDistancesMainMap()
        {
            gv.mod.pixDistanceToBorderWest = gv.playerOffsetX;
            gv.mod.pixDistanceToBorderEast = gv.playerOffsetX;
            gv.mod.pixDistanceToBorderNorth = gv.playerOffsetY;
            gv.mod.pixDistanceToBorderSouth = gv.playerOffsetY;

            //near eastern border
            if ((gv.mod.currentArea.MapSizeX - 1 - gv.mod.PlayerLocationX < gv.playerOffsetX) && (gv.mod.currentArea.easternNeighbourArea.Contains("none") || string.IsNullOrEmpty(gv.mod.currentArea.easternNeighbourArea)))
            {
                gv.mod.pixDistanceToBorderEast = gv.mod.currentArea.MapSizeX - 1 - gv.mod.PlayerLocationX;
            }

            //near western border
            if ((gv.mod.PlayerLocationX < gv.playerOffsetX) && (gv.mod.currentArea.westernNeighbourArea.Contains("none") || string.IsNullOrEmpty(gv.mod.currentArea.westernNeighbourArea)))
            {
                gv.mod.pixDistanceToBorderWest = gv.mod.PlayerLocationX;
            }

            //near southern border
            if ((gv.mod.currentArea.MapSizeY - 1 - gv.mod.PlayerLocationY < gv.playerOffsetY) && (gv.mod.currentArea.southernNeighbourArea.Contains("none") || string.IsNullOrEmpty(gv.mod.currentArea.southernNeighbourArea)))
            {
                gv.mod.pixDistanceToBorderSouth = gv.mod.currentArea.MapSizeY - 1 - gv.mod.PlayerLocationY;
            }

            //near northern border
            if ((gv.mod.PlayerLocationY < gv.playerOffsetY) && (gv.mod.currentArea.northernNeighbourArea.Contains("none") || string.IsNullOrEmpty(gv.mod.currentArea.northernNeighbourArea)))
            {
                gv.mod.pixDistanceToBorderNorth = gv.mod.PlayerLocationY;
            }

            //note this is measured form the outer rims (the rim next to the border we check for) of the player's current square this way
            gv.mod.pixDistanceToBorderEast *= gv.squareSize;
            gv.mod.pixDistanceToBorderWest *= gv.squareSize;
            gv.mod.pixDistanceToBorderNorth *= gv.squareSize;
            gv.mod.pixDistanceToBorderSouth *= gv.squareSize;
        }

        public void transformSpritePixelPositionOnContactWithVisibleMainMapBorders(Sprite spr, float outOffVisibleMapAllowedPercentage, bool movesOnGlobe, bool movesAsBumper, float bumpAngleVarianceInDegrees)
        {
            setToBorderPixDistancesMainMap();

            float scaler = 0;
            if (spr.scaleX >= spr.scaleY)
            {
                scaler = spr.scaleX;
            }
            else
            {
                scaler = spr.scaleY;
            }

            float scaledSpriteDimension = spr.frameHeight * scaler;
            //if (!spr.movementMethod.Contains("fog"))
            //{
                float capSize = 0;
                if (gv.screenWidth > gv.screenHeight)
                {
                    capSize = gv.screenHeight;
                }
                else
                {
                    capSize = gv.screenWidth;
                }
                if (scaledSpriteDimension > capSize)
                {
                    scaledSpriteDimension = capSize;
                }
            //}

            float visibleMapWidth = (2 * gv.playerOffsetX + 1) * gv.squareSize - ((gv.playerOffsetX * gv.squareSize) - gv.mod.pixDistanceToBorderWest) - ((gv.playerOffsetX * gv.squareSize) - gv.mod.pixDistanceToBorderEast);
            float visibleMapHeight = (2 * gv.playerOffsetY + 1) * gv.squareSize - ((gv.playerOffsetY * gv.squareSize) - gv.mod.pixDistanceToBorderNorth) - ((gv.playerOffsetY * gv.squareSize) - gv.mod.pixDistanceToBorderSouth);

            float extraDistanceBeforeReappearanceOrBumpWest = (gv.playerOffsetX * gv.squareSize) - gv.mod.pixDistanceToBorderEast;
            float extraDistanceBeforeReappearanceOrBumpEast = (gv.playerOffsetX * gv.squareSize) - gv.mod.pixDistanceToBorderWest;
            float extraDistanceBeforeReappearanceOrBumpSouth = (gv.playerOffsetY * gv.squareSize) - gv.mod.pixDistanceToBorderNorth;
            float extraDistanceBeforeReappearanceOrBumpNorth = (gv.playerOffsetY * gv.squareSize) - gv.mod.pixDistanceToBorderSouth;

            //nullign these valus as I think they are not really needed (must test more)
            extraDistanceBeforeReappearanceOrBumpWest = 0;
            extraDistanceBeforeReappearanceOrBumpEast = 0;
            extraDistanceBeforeReappearanceOrBumpSouth = 0;
            extraDistanceBeforeReappearanceOrBumpNorth = 0;
            
            bool isLeavingEast = false;
            bool isLeavingWest = false;
            bool isLeavingNorth = false;
            bool isLeavingSouth = false;

            if (spr.position.X - (scaledSpriteDimension * outOffVisibleMapAllowedPercentage) + scaledSpriteDimension - extraDistanceBeforeReappearanceOrBumpEast > (gv.screenWidth / 2 + gv.mod.pixDistanceToBorderEast))
            {
                isLeavingEast = true;
            }
            if (spr.position.X + (scaledSpriteDimension * outOffVisibleMapAllowedPercentage) + extraDistanceBeforeReappearanceOrBumpWest < (gv.screenWidth / 2 - gv.mod.pixDistanceToBorderWest))
            {
                isLeavingWest = true;
            }
            if (spr.position.Y + (scaledSpriteDimension * outOffVisibleMapAllowedPercentage) + extraDistanceBeforeReappearanceOrBumpNorth < (gv.screenHeight / 2 - gv.mod.pixDistanceToBorderNorth))
            {
                isLeavingNorth = true;
            }
            if (spr.position.Y - (scaledSpriteDimension * outOffVisibleMapAllowedPercentage) + scaledSpriteDimension - extraDistanceBeforeReappearanceOrBumpSouth > (gv.screenHeight / 2 + gv.mod.pixDistanceToBorderSouth))
            {
                isLeavingSouth = true;
            }

            if (isLeavingEast || isLeavingWest || isLeavingNorth || isLeavingSouth)
            {
                //like clouds
                if (spr.movementMethod.Contains("clouds"))
                {
                    float randomizerVertical = gv.sf.RandInt(gv.screenHeight / 2);
                    float randomizerHorizontal = gv.sf.RandInt(gv.screenWidth / 2);
                    float directionDecider = gv.sf.RandInt(2);
                    if (directionDecider == 1)
                    {
                        randomizerVertical = randomizerVertical * -1;
                    }
                    directionDecider = gv.sf.RandInt(2);
                    if (directionDecider == 1)
                    {
                        randomizerHorizontal = randomizerHorizontal * -1;
                    }

                    float diagonalRandomizer = gv.sf.RandInt(gv.screenHeight / 2);
                    float axisDecider = gv.sf.RandInt(2);
                    float diagonalRandomizerHorizontal = 0;
                    float diagonalRandomizerVertical = 0;
                    if (axisDecider == 1)
                    {
                        diagonalRandomizerHorizontal = diagonalRandomizer;
                    }
                    else
                    {
                        diagonalRandomizerVertical = diagonalRandomizer;
                    }
                    if (directionDecider == 1)
                    {
                        diagonalRandomizerHorizontal = diagonalRandomizerHorizontal * -1;
                        diagonalRandomizerVertical = diagonalRandomizerVertical * -1;
                    }

                    if (gv.mod.windDirection.Contains("North"))
                    {
                        spr.position.X = gv.screenWidth / 2 - scaledSpriteDimension / 2 + randomizerHorizontal;
                        spr.position.Y = gv.screenHeight / 2 + gv.mod.pixDistanceToBorderSouth - 1;
                    }
                    if (gv.mod.windDirection.Contains("NE"))
                    {
                        spr.position.X = gv.screenWidth / 2 - gv.mod.pixDistanceToBorderWest - scaledSpriteDimension + 1 + diagonalRandomizerHorizontal;
                        spr.position.Y = gv.screenHeight / 2 + gv.mod.pixDistanceToBorderSouth - 1 - diagonalRandomizerVertical;
                    }
                    if (gv.mod.windDirection.Contains("East"))
                    {
                        spr.position.X = gv.screenWidth / 2 - gv.mod.pixDistanceToBorderWest - scaledSpriteDimension + 1;
                        spr.position.Y = gv.screenHeight / 2 - scaledSpriteDimension / 2 + randomizerVertical;
                    }
                    if (gv.mod.windDirection.Contains("SE"))
                    {
                        spr.position.X = gv.screenWidth / 2 - gv.mod.pixDistanceToBorderWest - scaledSpriteDimension + 1 + diagonalRandomizerHorizontal;
                        spr.position.Y = gv.screenHeight / 2 - gv.mod.pixDistanceToBorderNorth - scaledSpriteDimension + 1 + diagonalRandomizerVertical;
                    }
                    if (gv.mod.windDirection.Contains("South"))
                    {
                        spr.position.X = gv.screenWidth / 2 - scaledSpriteDimension / 2 + randomizerHorizontal;
                        spr.position.Y = gv.screenHeight / 2 - gv.mod.pixDistanceToBorderNorth - scaledSpriteDimension + 1;
                    }
                    if (gv.mod.windDirection.Contains("SW"))
                    {
                        spr.position.X = gv.screenWidth / 2 + gv.mod.pixDistanceToBorderEast - 1 - diagonalRandomizerHorizontal;
                        spr.position.Y = gv.screenHeight / 2 - gv.mod.pixDistanceToBorderNorth - scaledSpriteDimension + 1 + diagonalRandomizerVertical;
                    }
                    if (gv.mod.windDirection.Contains("West"))
                    {
                        spr.position.X = gv.screenWidth / 2 + gv.mod.pixDistanceToBorderEast - 1;
                        spr.position.Y = gv.screenHeight / 2 - scaledSpriteDimension / 2 + randomizerVertical;
                    }
                    if (gv.mod.windDirection.Contains("NW"))
                    {
                        spr.position.X = gv.screenWidth / 2 + gv.mod.pixDistanceToBorderEast - 1 - diagonalRandomizerHorizontal;
                        spr.position.Y = gv.screenHeight / 2 + gv.mod.pixDistanceToBorderSouth - 1 - diagonalRandomizerVertical;
                    }
                }

                //like fog
                if (movesAsBumper)
                {
                    spr.velocity.X = -spr.velocity.X;
                    spr.velocity.Y = -spr.velocity.Y;
                    spr.position.X += 2*spr.velocity.X;
                    spr.position.Y += 2*spr.velocity.Y;
                    //to do: modifiy angle of speed vector randomly after bump 
                }
            }
        }

        public void checkLevelUpAvailable()
        {            
            if (gv.mod.playerList.Count > 0)
            {
                if (gv.mod.playerList[0].IsReadyToAdvanceLevel()) { gv.cc.ptrPc0.levelUpOn = true; }
                else { gv.cc.ptrPc0.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 1)
            {
                if (gv.mod.playerList[1].IsReadyToAdvanceLevel()) { gv.cc.ptrPc1.levelUpOn = true; }
                else { gv.cc.ptrPc1.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 2)
            {
                if (gv.mod.playerList[2].IsReadyToAdvanceLevel()) { gv.cc.ptrPc2.levelUpOn = true; }
                else { gv.cc.ptrPc2.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 3)
            {
                if (gv.mod.playerList[3].IsReadyToAdvanceLevel()) { gv.cc.ptrPc3.levelUpOn = true; }
                else { gv.cc.ptrPc3.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 4)
            {
                if (gv.mod.playerList[4].IsReadyToAdvanceLevel()) { gv.cc.ptrPc4.levelUpOn = true; }
                else { gv.cc.ptrPc4.levelUpOn = false; }
            }
            if (gv.mod.playerList.Count > 5)
            {
                if (gv.mod.playerList[5].IsReadyToAdvanceLevel()) { gv.cc.ptrPc5.levelUpOn = true; }
                else { gv.cc.ptrPc5.levelUpOn = false; }
            }
        }

        public void doWeatherSound()
        {
            #region weatherSounds
            //Note that in doTransitionBasedOnAreaLocation() method another weather code part is located
            //the whole system uses three sound channels, ie three instances of mediaplayer (defined in gameview, set to loop there):
            //sound channel 1 (weatherSounds1 media player) is for different degreees of rain effects
            //sound channel 2 (weatherSounds2 media player) is for different degreees of wind(cloud) and sandStorm effects
            //sound channel 3 (weatherSounds3 media player) is for the lightning effect 
            //requires a switch on module levelset to true as well as the ingame toggle for music&sound on
            if ((gv.mod.useWeatherSound) && (gv.mod.playMusic))
            {
                //soundName is used to store the relevant name on the different checks for wind, sandStorm, rain and lightning
                string soundName = "";

                //flags for noticing wether a weather soune effect is supposed to stop (corresponds with turn off code at the end of this weather region)
                bool isRaining = false;
                bool isWindy = false;

                //set up rain sound
                //the idea is that the mp3 files have same name as the defining part of the weather layers name
                //e.g. the channelname/.png heavyRainLayerA(.png) correponds with heavyRain(.mp3)
                //check for heavyRain
                if ((gv.mod.currentWeatherName.Contains("heavyRain")) || (gv.mod.currentWeatherName.Contains("HeavyRain")))
                {
                    //store that rain is still running and that the sound channel for rain, ie sound channel1, shall not be stopped 
                    isRaining = true;
                    //gv.weatherSounds1.settings.volume = (int)(23 * weatherSoundMultiplier);
                    gv.weatherSounds1.settings.volume = (int)(18 * weatherSoundMultiplier);
                    if ((gv.mod.weatherSoundsName1 != "heavyRain") && (gv.mod.weatherSoundsName1 != "HeavyRain"))
                    {
                        gv.mod.weatherSoundsName1 = "heavyRain";
                        soundName = gv.mod.weatherSoundsName1;

                        if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                        }
                        else
                        {
                            gv.weatherSounds1.URL = "";
                        }
                        if ((gv.weatherSounds1.URL != "") && (gv.weatherSounds1 != null))
                        {
                            gv.weatherSounds1.controls.play();
                        }
                    }

                }

                //check for lightRain
                else if ((gv.mod.currentWeatherName.Contains("lightRain")) || (gv.mod.currentWeatherName.Contains("LightRain")))
                {
                    isRaining = true;
                    //gv.weatherSounds1.settings.volume = (int)(45 * weatherSoundMultiplier);
                    gv.weatherSounds1.settings.volume = (int)(35 * weatherSoundMultiplier);
                    if ((gv.mod.weatherSoundsName1 != "lightRain") && (gv.mod.weatherSoundsName1 != "lightRain"))
                    {
                        gv.mod.weatherSoundsName1 = "lightRain";
                        soundName = gv.mod.weatherSoundsName1;

                        if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                        }
                        else
                        {
                            gv.weatherSounds1.URL = "";
                        }
                        if ((gv.weatherSounds1.URL != "") && (gv.weatherSounds1 != null))
                        {
                            gv.weatherSounds1.controls.play();
                        }
                    }

                }

                //check for "normal" rain
                else if ((gv.mod.currentWeatherName.Contains("rain")) || (gv.mod.currentWeatherName.Contains("Rain")))
                {
                    isRaining = true;
                    //gv.weatherSounds1.settings.volume = (int)(55 * weatherSoundMultiplier);
                    gv.weatherSounds1.settings.volume = (int)(45 * weatherSoundMultiplier);
                    if ((gv.mod.weatherSoundsName1 != "rain") && (gv.mod.weatherSoundsName1 != "Rain"))
                    {
                        gv.mod.weatherSoundsName1 = "rain";
                        soundName = gv.mod.weatherSoundsName1;

                        if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds1.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                        }
                        else
                        {
                            gv.weatherSounds1.URL = "";
                        }
                        if ((gv.weatherSounds1.URL != "") && (gv.weatherSounds1 != null))
                        {
                            gv.weatherSounds1.controls.play();
                        }
                    }

                }

                //set up wind sound
                //set up heavy wind

                if ((gv.mod.currentWeatherName.Contains("heavyCloud")) || (gv.mod.currentWeatherName.Contains("HeavyCloud")))
                {
                    isWindy = true;
                    gv.weatherSounds2.settings.volume = (int)(55 * weatherSoundMultiplier);
                    if (((gv.mod.weatherSoundsName2 != "heavyCloud") && (gv.mod.weatherSoundsName2 != "HeavyCloud")) || (gv.mod.resetWeatherSound))
                    {
                        /*
                        if (gv.mod.resetWeatherSound)
                        {
                            gv.mod.resetWeatherSound = false;
                        }
                        */
                        gv.mod.weatherSoundsName2 = "heavyCloud";
                        soundName = gv.mod.weatherSoundsName2;

                        if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                        }
                        else
                        {
                            gv.weatherSounds2.URL = "";
                        }
                        if ((gv.weatherSounds2.URL != "") && (gv.weatherSounds2 != null))
                        {
                            gv.weatherSounds2.controls.play();
                        }
                    }

                }

                //set up light winds
                else if ((gv.mod.currentWeatherName.Contains("lightCloud")) || (gv.mod.currentWeatherName.Contains("LightCloud")))
                {
                    isWindy = true;
                    gv.weatherSounds2.settings.volume = (int)(23 * weatherSoundMultiplier);
                    if (((gv.mod.weatherSoundsName2 != "lightCloud") && (gv.mod.weatherSoundsName2 != "LightCloud")) || (gv.mod.resetWeatherSound))
                    {
                        /*
                        if (gv.mod.resetWeatherSound)
                        {
                            gv.mod.resetWeatherSound = false;
                        }
                        */

                        gv.mod.weatherSoundsName2 = "lightCloud";
                        soundName = gv.mod.weatherSoundsName2;

                        if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                        }
                        else
                        {
                            gv.weatherSounds2.URL = "";
                        }
                        if ((gv.weatherSounds2.URL != "") && (gv.weatherSounds2 != null))
                        {
                            gv.weatherSounds2.controls.play();
                        }
                    }

                }

                //set up "normal" winds
                else if ((gv.mod.currentWeatherName.Contains("cloud")) || (gv.mod.currentWeatherName.Contains("Cloud")))
                {
                    isWindy = true;
                    gv.weatherSounds2.settings.volume = (int)(30 * weatherSoundMultiplier);
                    if (((gv.mod.weatherSoundsName2 != "cloud") && (gv.mod.weatherSoundsName2 != "Cloud")) || (gv.mod.resetWeatherSound))
                    {
                        /*
                        if (gv.mod.resetWeatherSound)
                        {
                            gv.mod.resetWeatherSound = false;
                        }
                        */

                        gv.mod.weatherSoundsName2 = "cloud";
                        soundName = gv.mod.weatherSoundsName2;

                        if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                        }
                        else
                        {
                            gv.weatherSounds2.URL = "";
                        }
                        if ((gv.weatherSounds2.URL != "") && (gv.weatherSounds2 != null))
                        {
                            gv.weatherSounds2.controls.play();
                        }
                    }
                }

                //set up light sandstorm
                //hurgh13
                else if ((gv.mod.currentWeatherName.Contains("lightSandStorm")) || (gv.mod.currentWeatherName.Contains("LightSandStorm")))
                {
                    isWindy = true;
                    gv.weatherSounds2.settings.volume = (int)(25 * weatherSoundMultiplier);
                    if (gv.mod.weatherSoundsName2 != "lightSandstorm")
                    {
                        gv.mod.weatherSoundsName2 = "lightSandstorm";
                        soundName = gv.mod.weatherSoundsName2;

                        if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                        }
                        else
                        {
                            gv.weatherSounds2.URL = "";
                        }
                        if ((gv.weatherSounds2.URL != "") && (gv.weatherSounds2 != null))
                        {
                            gv.weatherSounds2.controls.play();
                        }
                    }

                }

                //set up heavy sandstorm
                else if ((gv.mod.currentWeatherName.Contains("heavySandStorm")) || (gv.mod.currentWeatherName.Contains("HeavySandStorm")))
                {
                    isWindy = true;
                    gv.weatherSounds2.settings.volume = (int)(70 * weatherSoundMultiplier);
                    if (gv.mod.weatherSoundsName2 != "heavySandstorm")
                    {
                        gv.mod.weatherSoundsName2 = "heavySandstorm";
                        soundName = gv.mod.weatherSoundsName2;

                        if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                        }
                        else
                        {
                            gv.weatherSounds2.URL = "";
                        }
                        if ((gv.weatherSounds2.URL != "") && (gv.weatherSounds2 != null))
                        {
                            gv.weatherSounds2.controls.play();
                        }
                    }
                }

                //set up "normal" sandstorm
                else if ((gv.mod.currentWeatherName.Contains("sandStorm")) || (gv.mod.currentWeatherName.Contains("SandStorm")))
                {
                    isWindy = true;
                    gv.weatherSounds2.settings.volume = (int)(35 * weatherSoundMultiplier);
                    if (gv.mod.weatherSoundsName2 != "sandstorm")
                    {
                        gv.mod.weatherSoundsName2 = "sandstorm";
                        soundName = gv.mod.weatherSoundsName2;
                        //gv.weatherSounds1.controls.stop();

                        if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds2.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                        }
                        else
                        {
                            gv.weatherSounds2.URL = "";
                        }
                        if ((gv.weatherSounds2.URL != "") && (gv.weatherSounds2 != null))
                        {
                            gv.weatherSounds2.controls.play();
                        }
                    }
                }

                /*
                //set up lightning
                if ((gv.mod.currentWeatherName.Contains("lightning")) || (gv.mod.currentWeatherName.Contains("Lightning")))
                {
                    isLightning = true;
                    gv.weatherSounds3.settings.volume = (int)(50 * weatherSoundMultiplier);
                    if (gv.mod.weatherSoundsName3 != "lightning")
                    {
                        gv.mod.weatherSoundsName3 = "lightning";
                        soundName = gv.mod.weatherSoundsName3;

                        if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                        {
                            gv.weatherSounds3.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds3.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                        {
                            gv.weatherSounds3.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                        }
                        else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                        {
                            gv.weatherSounds3.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                        }
                        else
                        {
                            gv.weatherSounds3.URL = "";
                        }
                        if ((gv.weatherSounds3.URL != "") && (gv.weatherSounds3 != null))
                        {
                            gv.weatherSounds3.controls.play();
                        }
                    }
                }
                */

                //mute the not used channels
                if (isRaining == false)
                {
                    //if ((gv.weatherSounds1.URL != "") && (gv.weatherSounds1 != null))
                    //{
                    gv.weatherSounds1.controls.stop();
                    //}
                }
                else
                {
                    //if ((gv.weatherSounds1.URL != "") && (gv.weatherSounds1 != null))
                    //{
                    gv.weatherSounds1.controls.play();
                    //}
                }

                if (isWindy == false)
                {
                    //if ((gv.weatherSounds2.URL != "") && (gv.weatherSounds2 != null))
                    //{
                    gv.weatherSounds2.controls.stop();
                    //}
                }
                else
                {
                    //if ((gv.weatherSounds2.URL != "") && (gv.weatherSounds2 != null))
                    //{
                    gv.weatherSounds2.controls.play();
                    //}
                }
                /*
                if (isLightning == false)
                {
                    //if ((gv.weatherSounds3.URL != "") && (gv.weatherSounds3 != null))
                    //{
                    gv.weatherSounds3.controls.stop();
                    //}
                }
                else
                {
                    //if ((gv.weatherSounds3.URL != "") && (gv.weatherSounds3 != null))
                    //{
                    gv.weatherSounds3.controls.play();
                    //}
                }
                */
                
                if (!gv.mod.playMusic || gv.mod.currentArea.areaWeatherName == "" || gv.mod.currentArea.areaWeatherName == "none")
                {
                    gv.weatherSounds1.controls.stop();
                    gv.weatherSounds2.controls.stop();
                    gv.weatherSounds3.controls.stop();
                }
                #endregion
            }
        }
        public void doPropHeartBeat()
        {
            foreach (Prop prp in gv.mod.currentArea.Props)
            { 
                gv.sf.ThisProp = prp;
                //IBScript Prop heartbeat
                gv.cc.doIBScriptBasedOnFilename(prp.OnHeartBeatIBScript, prp.OnHeartBeatIBScriptParms);
                gv.sf.ThisProp = null;
            }
        }

        public void doChannelScripts()
        {
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentArea.effectChannelScript1, "fullScreenEffectScript");
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentArea.effectChannelScript2, "fullScreenEffectScript");
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentArea.effectChannelScript3, "fullScreenEffectScript");
            gv.cc.doIBScriptBasedOnFilename(gv.mod.currentArea.effectChannelScript4, "fullScreenEffectScript");

        }

        public void doWeather()
        {
            //clear the old weather lists
            gv.mod.listOfEntryWeatherNames.Clear();
            gv.mod.listOfEntryWeatherChances.Clear();
            gv.mod.listOfEntryWeatherDurations.Clear();
            gv.mod.listOfExitWeatherNames.Clear();
            gv.mod.listOfExitWeatherChances.Clear();
            gv.mod.listOfExitWeatherDurations.Clear();

            //if ((gv.mod.currentArea.areaWeatherName != "") && (gv.mod.currentArea.areaWeatherName != "none"))
            //{
                //fill the weather lists again with fresh data from the long lists
                //note: this is done on each update call, mostly overwriting samey data
                //iterate through the module's list of weather objects
                for (int i = 0; i < gv.mod.moduleWeathersList.Count; i++)
                {
                    if (gv.mod.currentArea.areaWeatherName == gv.mod.moduleWeathersList[i].name)
                    {
                        for (int j = 0; j < gv.mod.moduleWeathersList[i].weatherTypeLists.Count; j++)
                        {
                            if (gv.mod.moduleWeathersList[i].weatherTypeLists[j].name == "entryList")
                            {
                                for (int k = 0; k < gv.mod.moduleWeathersList[i].weatherTypeLists[j].weatherTypeListItems.Count; k++)
                                {
                                    gv.mod.listOfEntryWeatherNames.Add(gv.mod.moduleWeathersList[i].weatherTypeLists[j].weatherTypeListItems[k].weatherEffectName);
                                    gv.mod.listOfEntryWeatherChances.Add(gv.mod.moduleWeathersList[i].weatherTypeLists[j].weatherTypeListItems[k].chance);
                                    gv.mod.listOfEntryWeatherDurations.Add(gv.mod.moduleWeathersList[i].weatherTypeLists[j].weatherTypeListItems[k].duration);
                                }
                                break;
                            }
                        }
                    }
                }

                #region check if current weather exists in this area
                //check whether the current weather is still in the entry list of current area (which should contain all weathers possible in an area)
                bool doesCurrentWeatherExistHere = false;
                foreach (string weatherName in gv.mod.listOfEntryWeatherNames)
                {
                    if (weatherName == gv.mod.currentWeatherName)
                    {
                        doesCurrentWeatherExistHere = true;
                        break;
                    }
                }

                //there is a current weather, the area allows weather and the particular current weather does not exist in this area anyhow
                if ((gv.mod.currentWeatherName != "") && (gv.mod.currentArea.areaWeatherName != "") && (!doesCurrentWeatherExistHere))
                {
                    //maintain weather from previous map for a few update calls (if new area allos weather at all)
                    //otherwise area borders will become weather borders which is bad for the seamlessness illusion 
                    if (gv.mod.justTransitioned2 == true)
                    {
                        gv.mod.oldWeatherName = gv.mod.currentWeatherName;
                        if (gv.mod.useRealTimeTimer == true)
                        {
                            gv.mod.maintainWeatherFromLastAreaTimer = gv.sf.RandInt(30) + 30;
                        }
                        else
                        {
                            gv.mod.maintainWeatherFromLastAreaTimer = gv.sf.RandInt(15) + 15;
                        }
                        gv.mod.justTransitioned2 = false;

                        //make sure the run out time is not longer than the anyhow remaining time of the old weather
                        if ((gv.mod.currentWeatherDuration > gv.mod.maintainWeatherFromLastAreaTimer))
                        {
                            gv.mod.currentWeatherDuration = (int)gv.mod.maintainWeatherFromLastAreaTimer;
                        }
                    }
                }
                #endregion

                //if (gv.mod.currentArea.areaWeatherName == "")
                //{
                //gv.mod.currentWeatherDuration = 0;
                //}

                //initialize a fresh weather
                //hurgh9             
                if ((gv.mod.currentWeatherName == "") && (gv.mod.oldWeatherName == "") && (gv.mod.currentArea.areaWeatherName != ""))
                {
                    //determine random number between 1 and 100 for choosing entry weather type
                    int rollRandom = gv.sf.RandInt(100);
                    int addedChances = 0;

                    for (int i = 0; i < gv.mod.listOfEntryWeatherChances.Count; i++)
                    {
                        addedChances += gv.mod.listOfEntryWeatherChances[i];
                        if (rollRandom <= addedChances)
                        {
                            gv.mod.currentWeatherName = gv.mod.listOfEntryWeatherNames[i];
                            if (gv.mod.currentWeatherName.Contains("Sandstorm") || gv.mod.currentWeatherName.Contains("sandStorm") || gv.mod.currentWeatherName.Contains("sandstorm") || gv.mod.currentWeatherName.Contains("SandStorm"))
                            {
                                gv.mod.sandStormDirectionX = (float)(gv.sf.RandInt(100) + 150) / 500f;
                                gv.mod.sandStormDirectionY = (float)(gv.sf.RandInt(100) + 150) / 500f;
                                int deciderX = gv.sf.RandInt(2);
                                if (deciderX == 1)
                                {
                                    gv.mod.sandStormDirectionX = gv.mod.sandStormDirectionX * -1;
                                }
                                int deciderY = gv.sf.RandInt(2);
                                if (deciderY == 1)
                                {
                                    gv.mod.sandStormDirectionY = gv.mod.sandStormDirectionY * -1;
                                }

                                if ((deciderX == 1) && (deciderY == 1))
                                {
                                    gv.mod.sandStormBlowingTo = "NW";
                                }
                                if ((deciderX == 1) && (deciderY == 2))
                                {
                                    gv.mod.sandStormBlowingTo = "SW";
                                }
                                if ((deciderX == 2) && (deciderY == 1))
                                {
                                    gv.mod.sandStormBlowingTo = "NE";
                                }
                                if ((deciderX == 2) && (deciderY == 2))
                                {
                                    gv.mod.sandStormBlowingTo = "SE";
                                }
                            }

                            gv.mod.currentWeatherDuration = gv.mod.listOfEntryWeatherDurations[i];
                            float rollRandom2 = gv.sf.RandInt(100);
                            if (gv.mod.useRealTimeTimer == true)
                            {
                                gv.mod.currentWeatherDuration = (int)(gv.mod.currentWeatherDuration * ((200f + rollRandom2) / 100f));
                            }
                            else
                            {
                                gv.mod.currentWeatherDuration = (int)(gv.mod.currentWeatherDuration * ((100f + rollRandom2) / 100f));
                            }

                            doesCurrentWeatherExistHere = true;
                            gv.mod.howLongWeatherHasRun = 0;
                            gv.mod.fullScreenEffectOpacityWeather = 0;
                            //gv.mod.currentArea.rememberedWeatherName = gv.mod.currentWeatherName;
                            //gv.mod.currentArea.rememberedWeatherDuration = gv.mod.currentWeatherDuration;
                            break;
                        }
                    }
                }

                //reduce duration by 1, more for areas that consume more time per step, also for fade out
                gv.mod.currentWeatherDuration -= (1 * gv.mod.currentArea.weatherDurationMultiplierForScale);

                //fade in counter
                gv.mod.howLongWeatherHasRun += (1 * gv.mod.currentArea.weatherDurationMultiplierForScale);

                float changeThreshold = (5 * gv.mod.currentArea.weatherDurationMultiplierForScale);

                //Fade in
                if (gv.mod.howLongWeatherHasRun <= changeThreshold)
                {
                    gv.mod.fullScreenEffectOpacityWeather = 1f * (gv.mod.howLongWeatherHasRun / changeThreshold);
                }

                //Fade out
                if (gv.mod.currentWeatherDuration <= changeThreshold)
                {
                    gv.mod.fullScreenEffectOpacityWeather = 1f * (gv.mod.currentWeatherDuration / changeThreshold);
                }

                #region weather duration has ended, setup new weather
                //weather duration has ended 
                if (gv.mod.currentWeatherDuration <= 0)
                {
                    gv.mod.oldWeatherName = "";
                    gv.mod.currentWeatherName = "";

                    gv.weatherSounds1.controls.stop();
                    gv.weatherSounds2.controls.stop();
                    gv.weatherSounds3.controls.stop();

                    gv.mod.howLongWeatherHasRun = 0;
                    gv.mod.fullScreenEffectOpacityWeather = 0;

                    //iterate through the module's list of weather objects
                    for (int i = 0; i < gv.mod.moduleWeathersList.Count; i++)
                    {
                        if (gv.mod.currentArea.areaWeatherName == gv.mod.moduleWeathersList[i].name)
                        {
                            for (int j = 0; j < gv.mod.moduleWeathersList[i].weatherTypeLists.Count; j++)
                            {
                                if (gv.mod.moduleWeathersList[i].weatherTypeLists[j].name == gv.mod.currentWeatherName)
                                {
                                    for (int k = 0; k < gv.mod.moduleWeathersList[i].weatherTypeLists[j].weatherTypeListItems.Count; k++)
                                    {
                                        gv.mod.listOfExitWeatherNames.Add(gv.mod.moduleWeathersList[i].weatherTypeLists[j].weatherTypeListItems[k].weatherEffectName);
                                        gv.mod.listOfExitWeatherChances.Add(gv.mod.moduleWeathersList[i].weatherTypeLists[j].weatherTypeListItems[k].chance);
                                        gv.mod.listOfExitWeatherDurations.Add(gv.mod.moduleWeathersList[i].weatherTypeLists[j].weatherTypeListItems[k].duration);
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    //determine random number between 1 and 100 for choosing entry weather type
                    int rollRandom = gv.sf.RandInt(100);
                    int addedChances = 0;
                    bool foundWeather = false;

                    for (int i = 0; i < gv.mod.listOfExitWeatherChances.Count; i++)
                    {
                        addedChances += gv.mod.listOfExitWeatherChances[i];
                        if (rollRandom <= addedChances)
                        {
                            gv.mod.currentWeatherName = gv.mod.listOfExitWeatherNames[i];
                            if (gv.mod.currentWeatherName.Contains("Sandstorm") || gv.mod.currentWeatherName.Contains("sandStorm") || gv.mod.currentWeatherName.Contains("sandstorm") || gv.mod.currentWeatherName.Contains("SandStorm"))
                            {
                                gv.mod.sandStormDirectionX = (float)(gv.sf.RandInt(100) + 150) / 500f;
                                gv.mod.sandStormDirectionY = (float)(gv.sf.RandInt(100) + 150) / 500f;
                                int deciderX = gv.sf.RandInt(2);
                                if (deciderX == 1)
                                {
                                    gv.mod.sandStormDirectionX = gv.mod.sandStormDirectionX * -1;
                                }
                                int deciderY = gv.sf.RandInt(2);
                                if (deciderY == 1)
                                {
                                    gv.mod.sandStormDirectionY = gv.mod.sandStormDirectionY * -1;
                                }

                                if ((deciderX == 1) && (deciderY == 1))
                                {
                                    gv.mod.sandStormBlowingTo = "NW";
                                }
                                if ((deciderX == 1) && (deciderY == 2))
                                {
                                    gv.mod.sandStormBlowingTo = "SW";
                                }
                                if ((deciderX == 2) && (deciderY == 1))
                                {
                                    gv.mod.sandStormBlowingTo = "NE";
                                }
                                if ((deciderX == 2) && (deciderY == 2))
                                {
                                    gv.mod.sandStormBlowingTo = "SE";
                                }


                            }
                            gv.mod.currentWeatherDuration = gv.mod.listOfExitWeatherDurations[i];
                            float rollRandom2 = gv.sf.RandInt(100);
                            gv.mod.currentWeatherDuration = (int)(gv.mod.currentWeatherDuration * ((50f + rollRandom2) / 100f));
                            doesCurrentWeatherExistHere = true;
                            foundWeather = true;
                            break;
                        }
                    }

                    //fail safe catch, take first weather from the respective exit list
                    if ((foundWeather == false) && (gv.mod.listOfExitWeatherChances.Count > 0))
                    {
                        gv.mod.currentWeatherName = gv.mod.listOfExitWeatherNames[0];
                        gv.mod.currentWeatherDuration = gv.mod.listOfExitWeatherDurations[0];
                        float rollRandom2 = gv.sf.RandInt(100);
                        gv.mod.currentWeatherDuration = (int)(gv.mod.currentWeatherDuration * ((50f + rollRandom2) / 100f));
                        doesCurrentWeatherExistHere = true;
                        foundWeather = true;
                    }

                }
                #endregion

                gv.mod.isRaining = false;
                gv.mod.isCloudy = false;
                gv.mod.isFoggy = false;
                gv.mod.isSnowing = false;
                gv.mod.isLightning = false;
                gv.mod.isSandstorm = false;


                if (gv.mod.currentArea.areaWeatherName != "")
                {
                    if (gv.mod.currentWeatherName.Contains("lightRain") || gv.mod.currentWeatherName.Contains("LightRain"))
                    {
                        gv.mod.isRaining = true;
                        gv.rainType = "lightRain";
                    }
                    else if (gv.mod.currentWeatherName.Contains("heavyRain") || gv.mod.currentWeatherName.Contains("HeavyRain"))
                    {
                        gv.mod.isRaining = true;
                        gv.rainType = "heavyRain";
                    }
                    else if (gv.mod.currentWeatherName.Contains("rain") || gv.mod.currentWeatherName.Contains("Rain"))
                    {
                        gv.mod.isRaining = true;
                        gv.rainType = "rain";
                    }

                    if (gv.mod.currentWeatherName.Contains("lightSnow") || gv.mod.currentWeatherName.Contains("LightSnow"))
                    {
                        gv.mod.isSnowing = true;
                        gv.snowType = "lightSnow";
                    }
                    else if (gv.mod.currentWeatherName.Contains("heavySnow") || gv.mod.currentWeatherName.Contains("HeavySnow"))
                    {
                        gv.mod.isSnowing = true;
                        gv.snowType = "heavySnow";
                    }
                    else if (gv.mod.currentWeatherName.Contains("snow") || gv.mod.currentWeatherName.Contains("Snow"))
                    {
                        gv.mod.isSnowing = true;
                        gv.snowType = "snow";
                    }


                    if (gv.mod.currentWeatherName.Contains("lightSandStorm") || gv.mod.currentWeatherName.Contains("LightSandStorm"))
                    {
                        gv.mod.isSandstorm = true;
                        gv.sandstormType = "lightSandStorm";
                    }
                    else if (gv.mod.currentWeatherName.Contains("heavySandStorm") || gv.mod.currentWeatherName.Contains("HeavySandStorm"))
                    {
                        gv.mod.isSandstorm = true;
                        gv.sandstormType = "heavySandStorm";
                    }
                    else if (gv.mod.currentWeatherName.Contains("sandStorm") || gv.mod.currentWeatherName.Contains("SandStorm"))
                    {
                        gv.mod.isSandstorm = true;
                        gv.sandstormType = "sandStorm";
                    }

                    if (gv.mod.currentWeatherName.Contains("heavyClouds") || gv.mod.currentWeatherName.Contains("HeavyClouds"))
                    {
                        gv.mod.isCloudy = true;
                        gv.cloudType = "heavyCloud";
                    }
                    else if (gv.mod.currentWeatherName.Contains("lightClouds") || gv.mod.currentWeatherName.Contains("LightClouds"))
                    {
                        gv.mod.isCloudy = true;
                        gv.cloudType = "lightCloud";
                    }
                    else if (gv.mod.currentWeatherName.Contains("clouds") || gv.mod.currentWeatherName.Contains("Clouds"))
                    {
                        gv.mod.isCloudy = true;
                        gv.cloudType = "cloud";
                    }

                    if (gv.mod.currentWeatherName.Contains("heavyFog") || gv.mod.currentWeatherName.Contains("HeavyFog"))
                    {
                        gv.mod.isFoggy = true;
                        gv.fogType = "heavyFog";
                    }
                    else if (gv.mod.currentWeatherName.Contains("lightFog") || gv.mod.currentWeatherName.Contains("LightFog"))
                    {
                        gv.mod.isFoggy = true;
                        gv.fogType = "lightFog";
                    }
                    else if (gv.mod.currentWeatherName.Contains("fog") || gv.mod.currentWeatherName.Contains("Fog"))
                    {
                        gv.mod.isFoggy = true;
                        gv.fogType = "fog";
                    }

                    if (gv.mod.currentWeatherName.Contains("lightning") || gv.mod.currentWeatherName.Contains("Lightning"))
                    {
                        gv.mod.isLightning = true;
                    }
                }

                if (gv.mod.debugMode)
                {
                    gv.cc.addLogText("lime", gv.mod.currentWeatherName);
                }

                //hurgh7
                //gv.mod.currentWeatherName = "sandStorm";
                //gv.mod.isSandstorm = true;
            }
        //}
               
        public void SetUpEntryLists(string str)
        {
            //this method expects input data in the following format
            //(SunnyWithClouds),Chance:[30],Duration:{123};(RainyWithFog),Chance:[50],Duration:{123},...
            // the important stuff here are the brackets, so you could also write actually:
            //(SunnyWithClouds)[30]{123}(RainyWithFog)[50]{123}...
            //it will store the three different items in brackets to three different lists in the same order
            string retString = "";
            bool startRecording = false;

            foreach (char c in str)
            {
                if ((!startRecording) && (c == '('))
                {
                    startRecording = true;
                    continue;
                }

                if (c == ')')
                {
                    gv.mod.listOfEntryWeatherNames.Add(retString);
                    retString = "";
                    startRecording = false;
                }

                if (startRecording)
                {
                    retString += c;
                }
            }

            foreach (char c in str)
            {
                if ((!startRecording) && (c == '['))
                {
                    startRecording = true;
                    continue;
                }

                if (c == ']')
                {
                    int chance = Convert.ToInt32(retString);
                    gv.mod.listOfEntryWeatherChances.Add(chance);
                    retString = "";
                    startRecording = false;
                }

                if (startRecording)
                {
                    retString += c;
                }
            }

            foreach (char c in str)
            {
                if ((!startRecording) && (c == '{'))
                {
                    startRecording = true;
                    continue;
                }

                if (c == '}')
                {
                    int duration = Convert.ToInt32(retString);
                    gv.mod.listOfEntryWeatherDurations.Add(duration);
                    retString = "";
                    startRecording = false;
                }

                if (startRecording)
                {
                    retString += c;
                }
            }

        }

        public void SetUpExitLists(string str)
        {
            //this method expects input data in the following format
            //(SunnyWithClouds),Chance:[30],Duration:{123};(RainyWithFog),Chance:[50],Duration:{123},...
            // the important stuff here are the brackets, so you could also write actually:
            //(SunnyWithClouds)[30]{123}(RainyWithFog)[50]{123}...
            //it will store the three different items in brackets to three different lists in the same order

            string retString = "";
            bool startRecording = false;

            foreach (char c in str)
            {
                if ((!startRecording) && (c == '('))
                {
                    startRecording = true;
                    continue;
                }

                if (c == ')')
                {
                    gv.mod.listOfExitWeatherNames.Add(retString);
                    retString = "";
                    startRecording = false;
                }

                if (startRecording)
                {
                    retString += c;
                }
            }

            foreach (char c in str)
            {
                if ((!startRecording) && (c == '['))
                {
                    startRecording = true;
                    continue;
                }

                if (c == ']')
                {
                    int chance = Convert.ToInt32(retString);
                    gv.mod.listOfExitWeatherChances.Add(chance);
                    retString = "";
                    startRecording = false;
                }

                if (startRecording)
                {
                    retString += c;
                }
            }

            foreach (char c in str)
            {
                if ((!startRecording) && (c == '{'))
                {
                    startRecording = true;
                    continue;
                }

                if (c == '}')
                {
                    int duration = Convert.ToInt32(retString);
                    gv.mod.listOfExitWeatherDurations.Add(duration);
                    retString = "";
                    startRecording = false;
                }

                if (startRecording)
                {
                    retString += c;
                }
            }

        }

        public void doPropMoves()
        {
            //foreach (Prop propObject in gv.mod.currentArea.Props)
            //{
                //propObject.lastLocationX = propObject.LocationX;
                //propObject.lastLocationY = propObject.LocationY;
            //}
            
            #region Synchronization: update the position of time driven movers (either when the party switches area or when a time driven mover enters the current area)

            //Synchronization: check for all time driven movers either 1. found when entering an area (three variants: move into current area, move on current area, move out of current area) or 2. coming in from outside while party is already on current area
            //three nested loops running through area/prop/waypoint
            for (int i = gv.mod.moduleAreasObjects.Count - 1; i >= 0; i--)
            {
                //the check for the two conditions itself; donOnEnterAreaUpdate is set in the region above 
                if ((gv.mod.moduleAreasObjects[i].Filename != gv.mod.currentArea.Filename) || (doOnEnterAreaUpdate))
                {
                    for (int j = gv.mod.moduleAreasObjects[i].Props.Count - 1; j >= 0; j--)
                    {
                        int relevantAreaIndex = 0;
                        int relevantPropIndex = 0;
                        int relevantWaypointIndex = 0;
                        bool foundProp = false;
                        int nearestPointInTime = 0;
                        string relevantPropTag = "";

                        if ((gv.mod.moduleAreasObjects[i].Props[j].MoverType == "daily") || (gv.mod.moduleAreasObjects[i].Props[j].MoverType == "weekly") || (gv.mod.moduleAreasObjects[i].Props[j].MoverType == "monthly") || (gv.mod.moduleAreasObjects[i].Props[j].MoverType == "yearly"))
                        {

                            int listEndCheckedIndexOfNextWaypoint = 0;
                            for (int k = gv.mod.moduleAreasObjects[i].Props[j].WayPointList.Count - 1; k >= 0; k--)
                            {
                                List<string> timeUnitsList = new List<string>();
                                int currentTimeInInterval = 0;

                                //convert the string from the toolset for departure time into separate ints, filtering out ":" and blanks
                                //format in toolset is number:number:number
                                //with these ranges [0 to 336]:[0 to 23]:[0 to 59]
                                //actually it's 1 to 336 for for intuitive feeling, but below code treats zero and 1 the same way
                                //think: 1 equals monday, 2 equals tuesday and so forth
                                timeUnitsList = gv.mod.moduleAreasObjects[i].Props[j].WayPointList[k].departureTime.Split(':').Select(x => x.Trim()).ToList();

                                int dayCounter = Convert.ToInt32(timeUnitsList[0]);
                                int hourCounter = Convert.ToInt32(timeUnitsList[1]);
                                int minuteCounter = Convert.ToInt32(timeUnitsList[2]);

                                //catch entries of zero
                                //counter is reduced by one to make below calculation work the same for day/minutes/hours
                                if ((dayCounter == 0) || (dayCounter == 1))
                                {
                                    dayCounter = 0;
                                }
                                else
                                {
                                    dayCounter = (dayCounter - 1);
                                }

                                //turn the the three counters into one number for departure time (in seconds)
                                int convertedDepartureTime = dayCounter * 86400 + hourCounter * 3600 + minuteCounter * 60;

                                //automatically overwritwe departure time for last in line waypoint to be at the end of the respective time interval 
                                //and factor in the duration of one step 
                                //this makes sure that within each time cycle every waypoint is only used once

                                if (k == gv.mod.moduleAreasObjects[i].Props[j].WayPointList.Count - 1)
                                {
                                    if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("daily"))
                                    {
                                        convertedDepartureTime = 86400 - (gv.mod.currentArea.TimePerSquare * 60 + 1);//times 60 is necceessary as world time and time per square are measured in minutes  
                                    }
                                    if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("weekly"))
                                    {
                                        convertedDepartureTime = 604800 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                    }
                                    if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("monthly"))
                                    {
                                        convertedDepartureTime = 2419200 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                    }
                                    if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("yearly"))
                                    {
                                        convertedDepartureTime = 29030400 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                    }
                                }

                                if (k == 0)
                                {
                                    if (convertedDepartureTime < (gv.mod.currentArea.TimePerSquare * 60 + 1))
                                    {
                                        convertedDepartureTime = gv.mod.currentArea.TimePerSquare * 60 + 1;
                                    }
                                }

                                //use modulo operation to get the current time (in seconds) in each of the intervals
                                //the intervalls endlessly run from zero to maximum length to zero to maximum length and so forth
                                if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("daily"))
                                {
                                    currentTimeInInterval = (gv.mod.WorldTime * 60) % 86400;
                                }
                                if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("weekly"))
                                {
                                    currentTimeInInterval = (gv.mod.WorldTime * 60) % 604800;
                                }
                                if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("monthly"))
                                {
                                    currentTimeInInterval = (gv.mod.WorldTime * 60) % 2419200;
                                }
                                if (gv.mod.moduleAreasObjects[i].Props[j].MoverType.Equals("yearly"))
                                {
                                    currentTimeInInterval = (gv.mod.WorldTime * 60) % 29030400;
                                }

                                //we look for waypoints whose time has already been reached in this step
                                if (currentTimeInInterval >= convertedDepartureTime)
                                {
                                    //we filter those waypoints out who are older than other waypoitns whose time has already been reached
                                    if (convertedDepartureTime > nearestPointInTime)
                                    {
                                        //we store and overwrite the waypoints whose time has been reached, overwriting the older ones so long until only the youngest waypoint whose time has already been reached remains
                                        nearestPointInTime = convertedDepartureTime;
                                        relevantAreaIndex = i;
                                        relevantPropIndex = j;
                                        relevantWaypointIndex = k;
                                        relevantPropTag = gv.mod.moduleAreasObjects[i].Props[j].PropTag;
                                        foundProp = true;
                                    }
                                }
                            }

                            //a waypint whose time has been reached has been found in above step, it's the youngest of these
                            if (foundProp)
                            {
                                //activate the filter again for the next props in the loop
                                foundProp = false;

                                //check whether the waypoint found was a last in line wayoint
                                //this is important as we will look whether the waypoint after the found one has a different area name
                                //note: if areea name of next waypoint is different than current one, the prop is transitioned to the other area (going in or out)
                                if (relevantWaypointIndex >= gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList.Count - 1)
                                {
                                    listEndCheckedIndexOfNextWaypoint = 0;
                                }
                                else
                                {
                                    listEndCheckedIndexOfNextWaypoint = relevantWaypointIndex + 1;
                                }

                                //we check the situation that the party enters a fresh area
                                //there are three situations to handle:
                                //1. the current waypoint is on different map, but the next waypoint is on current map: move prop to next waypoint (move into area)
                                //2. the current waypoint and the next are on current map: move prop to current waypoint (move on current area)
                                //3. the current waypoint is on this map, but the next waypoint is on different map: move prop to next waypoint (move out of current area)

                                if (doOnEnterAreaUpdate == true)
                                {
                                    //1. the current waypoint is on different map, but the next waypoint is on current map: move prop to next waypoint (move into area)
                                    if ((gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName == gv.mod.currentArea.Filename) && (gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName != gv.mod.currentArea.Filename))
                                    {
                                        //apply only for props that are not already in current area
                                        bool isOnCurrentAreaAlready = false;
                                        foreach (Prop p in gv.mod.currentArea.Props)
                                        {
                                            if (p.PropTag == relevantPropTag)
                                            {
                                                isOnCurrentAreaAlready = true;
                                            }
                                        }

                                        if (isOnCurrentAreaAlready == false)
                                        {
                                            //we assign the index of next in line waypoint
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = listEndCheckedIndexOfNextWaypoint;
                                            //set move to target coordinates
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y.ToString());
                                        }
                                        //prop already exists on current area, so we only relocate it, but no transfer
                                        else
                                        {
                                            //we assign the index of next in line waypoint
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = listEndCheckedIndexOfNextWaypoint;
                                            //set move to target coordinates
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                            gv.sf.osController("osSetPropLocation.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y.ToString());
                                        }
                                    }

                                    //2. the current waypoint and the next are on current map: move prop to current waypoint (move on current area, venetually transfer in from other area)
                                    else if ((gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName == gv.mod.currentArea.Filename) && (gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName == gv.mod.currentArea.Filename))
                                    {
                                        //apply only for props that are not already in current area
                                        bool isOnCurrentAreaAlready = false;
                                        foreach (Prop p in gv.mod.currentArea.Props)
                                        {
                                            if (p.PropTag == relevantPropTag)
                                            {
                                                isOnCurrentAreaAlready = true;
                                            }
                                        }
                                        //prop is not on current area, so transfer and tehn relocate it
                                        if (isOnCurrentAreaAlready == false)
                                        {
                                            //note: the index will be updated a few lines down in the normal move section to the correct target
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = relevantWaypointIndex;
                                            //note: the move to target coordinates will be updated a few lines down in the normal move section
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].Y;
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].Y.ToString());
                                        }
                                        //the prop is already on the current area, so just relocate it on area
                                        else
                                        {
                                            //note: the index will be updated a few lines down in the normal move section to the correct target
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = relevantWaypointIndex;
                                            //note: the move to target coordinates will be updated a few lines down in the normal move section
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].Y;
                                            gv.sf.osController("osSetPropLocation.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].Y.ToString());
                                        }
                                    }
                                    //3. remove from current area (move out of current area)
                                    else if ((gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName != gv.mod.currentArea.Filename) && (gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName == gv.mod.currentArea.Filename))
                                    {
                                        //apply only for props that ARE already in current area
                                        bool isOnCurrentAreaAlready = false;
                                        foreach (Prop p in gv.mod.currentArea.Props)
                                        {
                                            if (p.PropTag == relevantPropTag)
                                            {
                                                isOnCurrentAreaAlready = true;
                                            }
                                        }

                                        if (isOnCurrentAreaAlready)
                                        {
                                            //we assign the index of next in line waypoint
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = listEndCheckedIndexOfNextWaypoint;
                                            //set move to target coordinates
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y.ToString());
                                        }

                                    }
                                    //4. remove from echo prop (and transfer to fitting area)
                                    else if ((gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName != gv.mod.currentArea.Filename) && (gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[relevantWaypointIndex].areaName != gv.mod.currentArea.Filename))
                                    {
                                        //apply only for props that ARE already in current area
                                        bool isOnCurrentAreaAlready = false;
                                        foreach (Prop p in gv.mod.currentArea.Props)
                                        {
                                            if (p.PropTag == relevantPropTag)
                                            {
                                                isOnCurrentAreaAlready = true;
                                            }
                                        }

                                        if (isOnCurrentAreaAlready)
                                        {
                                            //we assign the index of next in line waypoint
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = listEndCheckedIndexOfNextWaypoint;
                                            //set move to target coordinates
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                            gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].passOneMove = true;
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y.ToString());
                                        }
                                    }
                                }
                                //we handle props entering the current area while the party is in it
                                //we will look for props whose next in line waypoint is on current map: we then move the prop to next in line waypoint
                                //note: this will allow props entering the current map even if the departure time of first waypoint on current map is not reached yet
                                //note: this is not run for props already on the current map (see condition at the very start that exempts current area from the loops), so no worries about affecting those already existing props
                                else
                                {
                                    if (gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName == gv.mod.currentArea.Filename)
                                    {
                                        gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointListCurrentIndex = listEndCheckedIndexOfNextWaypoint;
                                        //set move to target coordinates
                                        gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.X = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                        gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].CurrentMoveToTarget.Y = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                        gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].passOneMove = true;
                                        int xLocForFloaty = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X;
                                        int yLocForFloaty = gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y;
                                        
                                        gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].PropTag, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].areaName, gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X.ToString(), gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].Y.ToString());

                                        //gv.mod.moduleAreasObjects[relevantAreaIndex].Props[relevantPropIndex].WayPointList[listEndCheckedIndexOfNextWaypoint].X
                                        //xxxxx
                                        //added floaty text that announces the area transfer
                                        //string shownAreaName = "";
                                        //for (int a = gv.mod.moduleAreasObjects.Count - 1; a >= 0; a--)
                                        //{
                                        //if (gv.mod.moduleAreasObjects[a].Filename == gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName)
                                        //{
                                        //shownAreaName = gv.mod.moduleAreasObjects[a].inGameAreaName;
                                        //}
                                        //}
                                        //IBMessageBox.Show(gv, "Prop just appeared");
                                        gv.screenMainMap.addFloatyText(xLocForFloaty, yLocForFloaty, "Just arrived here", "white", 4000);

                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region move ALL movers on current map (post, random, patrol, daily, weekly, monthly, yearly; also handle chasing)
            //begin moving the existing props in this map

            for (int i = gv.mod.currentArea.Props.Count - 1; i >= 0; i--)
            {
                //clear the lists with pixel destination coordinates of props
                gv.mod.currentArea.Props[i].destinationPixelPositionXList.Clear();
                gv.mod.currentArea.Props[i].destinationPixelPositionXList = new List<int>();
                gv.mod.currentArea.Props[i].destinationPixelPositionYList.Clear();
                gv.mod.currentArea.Props[i].destinationPixelPositionYList = new List<int>();
                gv.mod.currentArea.Props[i].pixelMoveSpeed = 1;

                //set the currentPixel position of the props
                int xOffSetInSquares = gv.mod.currentArea.Props[i].LocationX - gv.mod.PlayerLocationX;
                int yOffSetInSquares = gv.mod.currentArea.Props[i].LocationY - gv.mod.PlayerLocationY;
                int playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffsetX * gv.squareSize);
                int playerPositionYInPix = gv.playerOffsetY * gv.squareSize;

                gv.mod.currentArea.Props[i].currentPixelPositionX = playerPositionXInPix + (xOffSetInSquares * gv.squareSize);
                gv.mod.currentArea.Props[i].currentPixelPositionY = playerPositionYInPix + (yOffSetInSquares * gv.squareSize);

                //if (gv.mod.currentArea.Props[i].passOneMove == true)
                //{
                    //gv.mod.currentArea.Props[i].passOneMove = false;
                    //continue;
                //}
                //else
                if (1 ==1)
                {
                    /*
                    #region delay a mover for one turn on same square as party
                    //I suggest to modify this, so the prop will only wait for one turn and then move on, regardless of shared location with player
                    //otherwise the player can pin down a mover forever which feels weird imho
                    if ((gv.mod.currentArea.Props[i].LocationX == gv.mod.PlayerLocationX) && (gv.mod.currentArea.Props[i].LocationY == gv.mod.PlayerLocationY))
                    {
                        if (gv.sf.GetLocalInt(gv.mod.currentArea.Props[i].PropTag, "hasAlreadyWaited") == -1)
                        {
                            gv.sf.SetLocalInt(gv.mod.currentArea.Props[i].PropTag, "hasAlreadyWaited", "1");
                            //do nothing since prop and player are on the same square
                            continue;
                        }
                    }
                    else
                    {
                        gv.sf.SetLocalInt(gv.mod.currentArea.Props[i].PropTag, "hasAlreadyWaited", "-1");
                    }
                    #endregion
                    */

                    #region DISABLED: dont move props further away than ten squares
                    //Here I would suggest a full disable - the illsuion of a living wold would not work with a time freeze bubble outside 10 square radius
                    //if (getDistance(new Coordinate(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY), new Coordinate(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY)) > 10)
                    //{
                    //do nothing since prop and player are far away from each other
                    //continue;			
                    //}
                    #endregion

                    if ((gv.mod.currentArea.Props[i].isMover) && (gv.mod.currentArea.Props[i].isActive))
                    {
                        //determine move distance first
                        int moveDist = this.getMoveDistance(gv.mod.currentArea.Props[i]);
                        //gv.mod.currentArea.Props[i].pixelMoveSpeed = moveDist;


                        #region Chaser code
                        if ((gv.mod.currentArea.Props[i].isChaser) && (!gv.mod.currentArea.Props[i].ReturningToPost))
                        {
                            //determine if start chasing or stop chasing (set isCurrentlyChasing to true or false)
                            if (!gv.mod.currentArea.Props[i].isCurrentlyChasing)
                            {
                                //not chasing so see if in detect distance and set to true
                                if (getDistance(new Coordinate(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY), new Coordinate(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY)) <= gv.mod.currentArea.Props[i].ChaserDetectRangeRadius)
                                {
                                    gv.mod.currentArea.Props[i].isCurrentlyChasing = true;
                                    gv.mod.currentArea.Props[i].ChaserStartChasingTime = gv.mod.WorldTime;
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Chasing...", "red", 1500);
                                    if (gv.mod.debugMode)
                                    {
                                        //gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "following you", "red", 4000);
                                        gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " start chasing " + gv.mod.currentArea.Props[i].ChaserChaseDuration + " seconds</font><BR>");
                                    }
                                }
                            }
                            else //is chasing so see if out of follow range and set to false
                            {
                                if (getDistance(new Coordinate(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY), new Coordinate(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY)) >= gv.mod.currentArea.Props[i].ChaserGiveUpChasingRangeRadius)
                                {
                                    gv.mod.currentArea.Props[i].isCurrentlyChasing = false;
                                    gv.mod.currentArea.Props[i].ReturningToPost = true;
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Lost interest...", "green", 1500);
                                    if (gv.mod.debugMode)
                                    {
                                        //gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Nevermind...", "green", 1000);
                                        //gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "nevermind", "green", 4000);
                                        gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " stop chasing on range</font><BR>");
                                    }
                                }
                                else if (gv.mod.WorldTime - gv.mod.currentArea.Props[i].ChaserStartChasingTime >= gv.mod.currentArea.Props[i].ChaserChaseDuration)
                                {
                                    gv.mod.currentArea.Props[i].isCurrentlyChasing = false;
                                    gv.mod.currentArea.Props[i].ReturningToPost = true;
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Lost interest...", "green", 1500);
                                    if (gv.mod.debugMode)
                                    {
                                        //gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "nevermind", "green", 4000);
                                        gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " stop chasing on duration</font><BR>");
                                    }
                                }
                                else
                                {
                                    if (gv.mod.debugMode)
                                    {
                                        int timeRemain = gv.mod.currentArea.Props[i].ChaserChaseDuration - (gv.mod.WorldTime - gv.mod.currentArea.Props[i].ChaserStartChasingTime);
                                        gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " chasing " + timeRemain + " seconds left</font><BR>");
                                    }
                                }
                            }
                        }
                        //check to see if currently chasing, if so, chase
                        if ((gv.mod.currentArea.Props[i].isChaser) && (gv.mod.currentArea.Props[i].isCurrentlyChasing))
                        {
                            //move the distance
                            this.moveToTarget(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY, gv.mod.currentArea.Props[i], moveDist);
                            if (moveDist > 1)
                            {
                                gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Double move", "yellow", 1500);
                            }
                            doPropBarkString(gv.mod.currentArea.Props[i]);
                            if (gv.mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</font><BR>");
                                gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "(" + gv.mod.currentArea.Props[i].LocationX + "," + gv.mod.currentArea.Props[i].LocationY + ")", "yellow", 4000);
                            }
                        }
                        #endregion

                        #region Mover type: post
                        //not chasing so do mover type
                        else if (gv.mod.currentArea.Props[i].MoverType.Equals("post"))
                        {
                            //move towards post location if not already there
                            if ((gv.mod.currentArea.Props[i].LocationX == gv.mod.currentArea.Props[i].PostLocationX) && (gv.mod.currentArea.Props[i].LocationY == gv.mod.currentArea.Props[i].PostLocationY))
                            {
                                //already there so do not move
                                gv.mod.currentArea.Props[i].ReturningToPost = false;
                            }
                            else
                            {
                                this.moveToTarget(gv.mod.currentArea.Props[i].PostLocationX, gv.mod.currentArea.Props[i].PostLocationY, gv.mod.currentArea.Props[i], moveDist);
                                if (moveDist > 1)
                                {
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Double move", "yellow", 1500);
                                }
                                if (gv.mod.debugMode)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</font><BR>");
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "(" + gv.mod.currentArea.Props[i].LocationX + "," + gv.mod.currentArea.Props[i].LocationY + ")", "yellow", 4000);
                                }
                            }
                            doPropBarkString(gv.mod.currentArea.Props[i]);
                        }
                        #endregion

                        #region Mover type: random
                        else if (gv.mod.currentArea.Props[i].MoverType.Equals("random"))
                        {
                            gv.mod.currentArea.Props[i].randomMoverTimerForNextTarget += 1;
                            //check to see if at target square already and if so, change target
                            if (((gv.mod.currentArea.Props[i].LocationX == gv.mod.currentArea.Props[i].CurrentMoveToTarget.X) && (gv.mod.currentArea.Props[i].LocationY == gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y)) || (gv.mod.currentArea.Props[i].randomMoverTimerForNextTarget >= 20))
                            {
                                gv.mod.currentArea.Props[i].randomMoverTimerForNextTarget = 0;
                                Coordinate newCoor = this.getNewRandomTarget(gv.mod.currentArea.Props[i]);
                                //gv.screenMainMap.addFloatyText(prp.LocationX, prp.LocationY, "(" + newCoor.X + "," + newCoor.Y + ")", "blue", 4000);
                                gv.mod.currentArea.Props[i].CurrentMoveToTarget = new Coordinate(newCoor.X, newCoor.Y);
                                gv.mod.currentArea.Props[i].ReturningToPost = false;
                            }
                            //move to target
                            this.moveToTarget(gv.mod.currentArea.Props[i].CurrentMoveToTarget.X, gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y, gv.mod.currentArea.Props[i], moveDist);
                            if (moveDist > 1)
                            {
                                gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Double move", "yellow", 1500);
                            }
                            //gv.screenMainMap.addFloatyText(prp.LocationX, prp.LocationY, "(" + prp.CurrentMoveToTarget.X + "," + prp.CurrentMoveToTarget.Y + ")", "red", 4000);
                            if (gv.mod.debugMode)
                            {
                                gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</font><BR>");
                                gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "(" + gv.mod.currentArea.Props[i].LocationX + "," + gv.mod.currentArea.Props[i].LocationY + ")", "yellow", 4000);
                            }
                            doPropBarkString(gv.mod.currentArea.Props[i]);
                        }
                        #endregion

                        #region Mover type: patrol
                        else if (gv.mod.currentArea.Props[i].MoverType.Equals("patrol"))
                        {
                            bool mustWait = false;
                            if (gv.mod.currentArea.Props[i].WayPointList.Count > 0)
                            {
                                //move towards next waypoint location if not already there
                                if ((gv.mod.currentArea.Props[i].LocationX == gv.mod.currentArea.Props[i].CurrentMoveToTarget.X) && (gv.mod.currentArea.Props[i].LocationY == gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y))
                                {
                                    if ((gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].WaitDuration > 0) && (gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].turnsAlreadyWaited <= gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].WaitDuration))
                                    {
                                        gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].turnsAlreadyWaited += 1;
                                        mustWait = true;
                                    }
                                    else
                                    {
                                        gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].turnsAlreadyWaited = 0;
                                        //already there so set next way point location (revert to index 0 if at last way point)
                                        if (gv.mod.currentArea.Props[i].WayPointListCurrentIndex >= gv.mod.currentArea.Props[i].WayPointList.Count - 1)
                                        {
                                            gv.mod.currentArea.Props[i].WayPointListCurrentIndex = 0;
                                        }
                                        else
                                        {
                                            gv.mod.currentArea.Props[i].WayPointListCurrentIndex++;
                                        }
                                        gv.mod.currentArea.Props[i].CurrentMoveToTarget.X = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X;
                                        gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y;
                                        gv.mod.currentArea.Props[i].ReturningToPost = false;
                                    }

                                }
                                //move to next target
                                if (!mustWait)
                                {
                                    this.moveToTarget(gv.mod.currentArea.Props[i].CurrentMoveToTarget.X, gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y, gv.mod.currentArea.Props[i], moveDist);
                                    if (moveDist > 1)
                                    {
                                        gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Double move", "yellow", 1500);
                                    }
                                }
                                if (gv.mod.debugMode)
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</font><BR>");
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "(" + gv.mod.currentArea.Props[i].LocationX + "," + gv.mod.currentArea.Props[i].LocationY + ")", "yellow", 4000);
                                }
                            }
                            doPropBarkString(gv.mod.currentArea.Props[i]);
                        }
                        #endregion

                        #region time driven movers (daily, weekly, monthly, yearly)
                        else if (gv.mod.currentArea.Props[i].MoverType.Equals("daily") || gv.mod.currentArea.Props[i].MoverType.Equals("weekly") || gv.mod.currentArea.Props[i].MoverType.Equals("monthly") || gv.mod.currentArea.Props[i].MoverType.Equals("yearly"))
                        {
                            bool departureTimeReached = false;
                            List<string> timeUnitsList = new List<string>();
                            int currentTimeInInterval = 0;
                            bool registerRemoval = false;

                            timeUnitsList = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].departureTime.Split(':').Select(x => x.Trim()).ToList();

                            int dayCounter = Convert.ToInt32(timeUnitsList[0]);
                            int hourCounter = Convert.ToInt32(timeUnitsList[1]);
                            int minuteCounter = Convert.ToInt32(timeUnitsList[2]);

                            if ((dayCounter == 0) || (dayCounter == 1))
                            {
                                dayCounter = 0;
                            }
                            else
                            {
                                dayCounter = (dayCounter - 1);
                            }

                            int convertedDepartureTime = dayCounter * 86400 + hourCounter * 3600 + minuteCounter * 60;

                            if (gv.mod.currentArea.Props[i].WayPointListCurrentIndex == gv.mod.currentArea.Props[i].WayPointList.Count - 1)
                            {
                                if (gv.mod.currentArea.Props[i].MoverType.Equals("daily"))
                                {
                                    convertedDepartureTime = 86400 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                }
                                if (gv.mod.currentArea.Props[i].MoverType.Equals("weekly"))
                                {
                                    convertedDepartureTime = 604800 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                }
                                if (gv.mod.currentArea.Props[i].MoverType.Equals("monthly"))
                                {
                                    convertedDepartureTime = 2419200 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                }
                                if (gv.mod.currentArea.Props[i].MoverType.Equals("yearly"))
                                {
                                    convertedDepartureTime = 29030400 - (gv.mod.currentArea.TimePerSquare * 60 + 1);
                                }
                            }

                            if (gv.mod.currentArea.Props[i].WayPointListCurrentIndex == 0)
                            {
                                if (convertedDepartureTime < (gv.mod.currentArea.TimePerSquare * 60 + 1))
                                {
                                    convertedDepartureTime = gv.mod.currentArea.TimePerSquare * 60 + 1;
                                }
                            }

                            if (gv.mod.currentArea.Props[i].MoverType.Equals("daily"))
                            {
                                currentTimeInInterval = (gv.mod.WorldTime * 60) % 86400;
                            }
                            if (gv.mod.currentArea.Props[i].MoverType.Equals("weekly"))
                            {
                                currentTimeInInterval = (gv.mod.WorldTime * 60) % 604800;
                            }
                            if (gv.mod.currentArea.Props[i].MoverType.Equals("monthly"))
                            {
                                currentTimeInInterval = (gv.mod.WorldTime * 60) % 2419200;
                            }
                            if (gv.mod.currentArea.Props[i].MoverType.Equals("yearly"))
                            {
                                currentTimeInInterval = (gv.mod.WorldTime * 60) % 29030400;
                            }

                            if (currentTimeInInterval >= convertedDepartureTime)
                            {
                                departureTimeReached = true;
                            }

                            if (gv.mod.currentArea.Props[i].WayPointList.Count > 0)
                            {

                                if (departureTimeReached)
                                {
                                    //already there so set next way point location (revert to index 0 if at last way point)
                                    if (gv.mod.currentArea.Props[i].WayPointListCurrentIndex >= gv.mod.currentArea.Props[i].WayPointList.Count - 1)
                                    {
                                        gv.mod.currentArea.Props[i].WayPointListCurrentIndex = 0;

                                        if (gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName != gv.mod.currentArea.Filename)
                                        {
                                            gv.mod.currentArea.Props[i].CurrentMoveToTarget.X = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X;
                                            gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y;
                                            gv.mod.currentArea.Props[i].ReturningToPost = false;
                                            //added floaty text that announces the area transfer
                                            string shownAreaName = "";
                                            for (int a = gv.mod.moduleAreasObjects.Count - 1; a >= 0; a--)
                                            {
                                                if (gv.mod.moduleAreasObjects[a].Filename == gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName)
                                                {
                                                    shownAreaName = gv.mod.moduleAreasObjects[a].inGameAreaName;
                                                }
                                            }

                                            gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "Heading off towards " + shownAreaName, "white", 4000);
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.currentArea.Props[i].PropTag, gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName, gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X.ToString(), gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y.ToString());
                                            registerRemoval = true;
                                        }
                                    }
                                    else
                                    {
                                        gv.mod.currentArea.Props[i].WayPointListCurrentIndex++;
                                        if (gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName != gv.mod.currentArea.Filename)
                                        {
                                            gv.mod.currentArea.Props[i].CurrentMoveToTarget.X = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X;
                                            gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y;
                                            gv.mod.currentArea.Props[i].ReturningToPost = false;
                                            //added floaty text that announces the area transfer
                                            string shownAreaName = "";
                                            for (int a = gv.mod.moduleAreasObjects.Count - 1; a >= 0; a--)
                                            {
                                                if (gv.mod.moduleAreasObjects[a].Filename == gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName)
                                                {
                                                    shownAreaName = gv.mod.moduleAreasObjects[a].inGameAreaName;
                                                }
                                            }

                                            gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "Heading off towards " + shownAreaName, "white", 4000);
                                            gv.sf.osController("osSetPropLocationAnyArea.cs", gv.mod.currentArea.Props[i].PropTag, gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].areaName, gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X.ToString(), gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y.ToString());
                                            registerRemoval = true;
                                        }
                                    }
                                    if (!registerRemoval)
                                    {
                                        gv.mod.currentArea.Props[i].CurrentMoveToTarget.X = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].X;
                                        gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y = gv.mod.currentArea.Props[i].WayPointList[gv.mod.currentArea.Props[i].WayPointListCurrentIndex].Y;
                                        gv.mod.currentArea.Props[i].ReturningToPost = false;
                                    }

                                }

                                //move to next target
                                if (!registerRemoval)
                                {
                                    if ((gv.mod.currentArea.Props[i].LocationX == gv.mod.currentArea.Props[i].CurrentMoveToTarget.X) && (gv.mod.currentArea.Props[i].LocationY == gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y))
                                    {

                                    }
                                    else
                                    {
                                        this.moveToTarget(gv.mod.currentArea.Props[i].CurrentMoveToTarget.X, gv.mod.currentArea.Props[i].CurrentMoveToTarget.Y, gv.mod.currentArea.Props[i], moveDist);
                                        if (moveDist > 1)
                                        {
                                            gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i], "Double move", "yellow", 1500);
                                        }
                                    }
                                }

                                if ((gv.mod.debugMode) && (!registerRemoval))
                                {
                                    gv.cc.addLogText("<font color='yellow'>" + gv.mod.currentArea.Props[i].PropTag + " moves " + moveDist + "</font><BR>");
                                    gv.screenMainMap.addFloatyText(gv.mod.currentArea.Props[i].LocationX, gv.mod.currentArea.Props[i].LocationY, "(" + gv.mod.currentArea.Props[i].LocationX + "," + gv.mod.currentArea.Props[i].LocationY + ")", "yellow", 4000);
                                }
                            }
                            if (!registerRemoval)
                            {
                                doPropBarkString(gv.mod.currentArea.Props[i]);
                            }
                        }
                        #endregion
                    }
                }
            }

            #endregion

        }
        public void doPropBarkString(Prop prp)
        {
            List<BarkString> chosenBarks = new List<BarkString>();
            Random rnd3 = new Random();
            int decider = 0;

            if (prp.WayPointList.Count > 0)
            {
                if ((prp.LocationX == prp.CurrentMoveToTarget.X) && (prp.LocationY == prp.CurrentMoveToTarget.Y))
                {
                    //do barks for waypoint
                    //if (prp.WayPointList[prp.WayPointListCurrentIndex].BarkStringsAtWayPoint.Count > 0)
                    //{
                    foreach (BarkString b in prp.WayPointList[prp.WayPointListCurrentIndex].BarkStringsAtWayPoint)
                    {
                        if (gv.sf.RandInt(100) < b.ChanceToShow)
                        {
                            chosenBarks.Add(b);
                        }
                    }
                    if (chosenBarks.Count > 0)
                    {
                        decider = rnd3.Next(0, chosenBarks.Count);
                        if (!gv.mod.useSmoothMovement)
                        {
                            gv.screenMainMap.addFloatyText(prp.LocationY, prp.LocationY, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);
                        }
                        else
                        {
                            gv.screenMainMap.addFloatyText(prp, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);

                        }

                        //gv.screenMainMap.addFloatyText(prp.LocationX, prp.LocationY, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);
                    }
                    //}
                }
                else
                {
                    //do barks for patrol, random, chasing or time driven
                    if (prp.WayPointListCurrentIndex == 0)
                    {
                        //if (prp.WayPointList[prp.WayPointListCurrentIndex].BarkStringsAtWayPoint.Count > 0)
                        //{
                        foreach (BarkString b in prp.WayPointList[prp.WayPointList.Count - 1].BarkStringsOnTheWayToNextWayPoint)
                        {
                            if (gv.sf.RandInt(100) < b.ChanceToShow)
                            {
                                chosenBarks.Add(b);
                            }
                        }
                        if (chosenBarks.Count > 0)
                        {
                            decider = rnd3.Next(0, chosenBarks.Count);
                            if (!gv.mod.useSmoothMovement)
                            {
                                gv.screenMainMap.addFloatyText(prp.LocationY, prp.LocationY, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);
                            }
                            else
                            {
                                gv.screenMainMap.addFloatyText(prp, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);

                            }

                            //gv.screenMainMap.addFloatyText(prp.LocationX, prp.LocationY, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);
                        }
                        //}
                    }
                    else
                    {
                        foreach (BarkString b in prp.WayPointList[prp.WayPointListCurrentIndex - 1].BarkStringsOnTheWayToNextWayPoint)
                        {
                            if (gv.sf.RandInt(100) < b.ChanceToShow)
                            {
                                chosenBarks.Add(b);
                                //gv.screenMainMap.addFloatyText(prp.LocationX, prp.LocationY, b.FloatyTextOneLiner, b.Color, b.LengthOfTimeToShowInMilliSeconds);
                                //break;
                            }
                        }
                        if (chosenBarks.Count > 0)
                        {
                            decider = rnd3.Next(0, chosenBarks.Count);
                            if (!gv.mod.useSmoothMovement)
                            {
                                gv.screenMainMap.addFloatyText(prp.LocationY, prp.LocationY, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);
                            }
                            else
                            {
                                gv.screenMainMap.addFloatyText(prp, chosenBarks[decider].FloatyTextOneLiner, chosenBarks[decider].Color, chosenBarks[decider].LengthOfTimeToShowInMilliSeconds);

                            }
                        }
                    }
                }
            }
        }
        public int getMoveDistance(Prop prp)
        {
            if (gv.sf.RandInt(100) <= prp.ChanceToMove2Squares)
            {
                return 2;
            }
            else if (gv.sf.RandInt(100) <= prp.ChanceToMove0Squares)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public Coordinate getNewRandomTarget(Prop prp)
        {
            Coordinate newCoor = new Coordinate();

            //X range
            int minX = prp.PostLocationX - prp.RandomMoverRadius;
            if (minX < 0) { minX = 0; }
            int maxX = prp.PostLocationX + prp.RandomMoverRadius;
            if (maxX > gv.mod.currentArea.MapSizeX - 1) { maxX = gv.mod.currentArea.MapSizeX - 1; }

            //Y range
            int minY = prp.PostLocationY - prp.RandomMoverRadius;
            if (minY < 0) { minY = 0; }
            int maxY = prp.PostLocationY + prp.RandomMoverRadius;
            if (maxY > gv.mod.currentArea.MapSizeY - 1) { maxY = gv.mod.currentArea.MapSizeY - 1; }

            //get random location...check if location is valid first...do for loop and exit when found one, try 10 times
            for (int i = 0; i < 10; i++)
            {
                int x = gv.sf.RandInt(minX, maxX);
                int y = gv.sf.RandInt(minY, maxY);
                //if (gv.mod.currentArea.Tiles.get(y * gv.mod.currentArea.MapSizeX + x).Walkable)
                if (!gv.mod.currentArea.GetBlocked(x, y))
                {
                    newCoor.X = x;
                    newCoor.Y = y;
                    return newCoor;
                }
            }
            return new Coordinate(prp.LocationX, prp.LocationY);
        }
        public void moveToTarget(int targetX, int targetY, Prop prp, int moveDistance)
        {
            //store last location
            //prp.lastLocationX = prp.LocationX;
            //prp.lastLocationX = prp.LocationX;

            if (gv.mod.useSmoothMovement)
            {
                if (!recursiveCall)
                {
                    fallBackSquareX = prp.LocationX;
                    fallBackSquareY = prp.LocationY;
                }
                Random rnd2 = new Random();
                for (int i = 0; i < moveDistance; i++)
                {
                    //if (i != 0)
                    //{
                        //attempt to catch situations where a creature would otherwise move "over" the party without triggering convo/encounter
                        //if (gv.triggerIndex == 0 && gv.triggerPropIndex == 0)
                        //{
                            //doPropTriggers();
                        //}
                    //}
                    gv.pfa.resetGrid();
                    Coordinate newCoor = gv.pfa.findNewPoint(new Coordinate(prp.LocationX, prp.LocationY), new Coordinate(targetX, targetY), prp);
                    if ((newCoor.X == -1) && (newCoor.Y == -1))
                    {
                        //didn't find a path, don't move
                        //why do we eliminate the moveto target here? Was fitting for a static world (that would never open a new path anyway). In a dynamic world I think we better keep the moveto information?
                        //as props can move through each other now (or that's the plan) it might still work with theese lines
                        //then again, mayhaps dynamic collision controlled waits are still useful, like e.g. blocked city gates (collission static props) would be nice to queue NPCs in the morning
                        //prp.CurrentMoveToTarget.X = prp.LocationX;
                        //prp.CurrentMoveToTarget.Y = prp.LocationY;
                        //gv.Invalidate();
                        //gv.Render();
                        return;
                    }

                    //new code for preventing movers from ending on the same square
                    bool nextStepSquareIsOccupied = false;
                    foreach (Prop otherProp in gv.mod.currentArea.Props)
                    {

                        //check whether an active mover prop is on the field found as next step on the path
                        if ((otherProp.LocationX == newCoor.X) && (otherProp.LocationY == newCoor.Y) && (otherProp.isMover) && (otherProp.isActive))
                        {
                            nextStepSquareIsOccupied = true;
                            break;
                        }
                    }

                    if (nextStepSquareIsOccupied)
                    {

                        bool originSquareOccupied = false;
                        //check wheter the next field found on path is the destination square, i.e. the waypoint our prop is headed to                                  
                        if ((newCoor.X == prp.CurrentMoveToTarget.X) && (newCoor.Y == prp.CurrentMoveToTarget.Y))
                        {
                            prp.destinationPixelPositionXList.Clear();
                            prp.destinationPixelPositionXList = new List<int>();
                            prp.destinationPixelPositionYList.Clear();
                            prp.destinationPixelPositionYList = new List<int>();

                            //let's find out whether our prop can stay on its origin square, i.e. skip move, or whether it already comes from an occupied square and has to "sidestep"
                            //Note: moving along path, double move, wont work when the target square is the destination square, i.e. the end of the path
                            foreach (Prop otherProp2 in gv.mod.currentArea.Props)
                            {
                                if ((otherProp2.LocationX == prp.LocationX) && (otherProp2.LocationY == prp.LocationY) && (otherProp2.isMover) && (otherProp2.isActive))
                                {
                                    if (otherProp2.PropTag != prp.PropTag)
                                    {
                                        originSquareOccupied = true;
                                        break;
                                    }
                                }
                            }

                            int decider2 = rnd2.Next(0, 10);
                            //another step forward, ie (at least) 2 steps on path
                            //check whether to stay on origin square ("step back")
                            if ((!originSquareOccupied) && (decider2 < 9))
                            {
                                return;
                            }

                            //sidestep to nearby free square because destination square and origin square are both occupied
                            else
                            {
                                prp.LocationX = fallBackSquareX;
                                prp.LocationY = fallBackSquareY;

                            }

                        }

                        //the (occupied) target square is not the destination, i.e. path end, square
                        else
                        {
                            originSquareOccupied = false;
                            //check whether origin square is occupied, too 
                            foreach (Prop otherProp2 in gv.mod.currentArea.Props)
                            {
                                if ((otherProp2.LocationX == prp.LocationX) && (otherProp2.LocationY == prp.LocationY) && (otherProp2.isMover) && (otherProp2.isActive))
                                {
                                    if (otherProp2.PropTag != prp.PropTag)
                                    {
                                        originSquareOccupied = true;
                                        break;
                                    }
                                }
                            }
                            //origin square is occupied, waiting is no option therefore, so we must do (at least) double move forward (target square is occupied, too)
                            if (originSquareOccupied)
                            {
                                //careful, watch for infinite loop, recursive calling here
                                prp.LocationX = newCoor.X;
                                prp.LocationY = newCoor.Y;

                                int xOffSetInSquares = 0;
                                int yOffSetInSquares = 0;
                                if (gv.mod.PlayerLocationX >= prp.LocationX)
                                {
                                    xOffSetInSquares = prp.LocationX - gv.mod.PlayerLocationX;
                                }
                                else
                                {
                                    xOffSetInSquares = prp.LocationX - gv.mod.PlayerLocationX;
                                }
                                if (gv.mod.PlayerLocationY >= prp.LocationY)
                                {
                                    yOffSetInSquares = prp.LocationY - gv.mod.PlayerLocationY;
                                }
                                else
                                {
                                    yOffSetInSquares = prp.LocationY - gv.mod.PlayerLocationY;
                                }
                                int playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffsetX * gv.squareSize);
                                int playerPositionYInPix = gv.playerOffsetY * gv.squareSize;

                                if ((xOffSetInSquares <= 10) && (xOffSetInSquares >= -10) && (yOffSetInSquares <= 10) && (yOffSetInSquares >= -10))
                                {
                                    prp.destinationPixelPositionXList.Add(playerPositionXInPix + (xOffSetInSquares * gv.squareSize));
                                    prp.destinationPixelPositionYList.Add(playerPositionYInPix + (yOffSetInSquares * gv.squareSize));
                                }
                                recursiveCall = true;
                                moveToTarget(targetX, targetY, prp, 1);
                                recursiveCall = false;
                                return;
                            }
                            //origin square not occupied, so waiting would be an alternative to double move: 50/50 situation
                            else
                            {
                                //make random roll to randomly choose between the two next alternatives
                                int decider = rnd2.Next(0, 10);
                                //another step forward, ie (at least) 2 steps on path
                                if (decider < 5)
                                {
                                    prp.LocationX = newCoor.X;
                                    prp.LocationY = newCoor.Y;

                                    int xOffSetInSquares = 0;
                                    int yOffSetInSquares = 0;
                                    if (gv.mod.PlayerLocationX >= prp.LocationX)
                                    {
                                        xOffSetInSquares = prp.LocationX - gv.mod.PlayerLocationX;
                                    }
                                    else
                                    {
                                        xOffSetInSquares = prp.LocationX - gv.mod.PlayerLocationX;
                                    }
                                    if (gv.mod.PlayerLocationY >= prp.LocationY)
                                    {
                                        yOffSetInSquares = prp.LocationY - gv.mod.PlayerLocationY;
                                    }
                                    else
                                    {
                                        yOffSetInSquares = prp.LocationY - gv.mod.PlayerLocationY;
                                    }
                                    int playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffsetX * gv.squareSize);
                                    int playerPositionYInPix = gv.playerOffsetY * gv.squareSize;

                                    if ((xOffSetInSquares <= 5) && (xOffSetInSquares >= -5) && (yOffSetInSquares <= 5) && (yOffSetInSquares >= -5))
                                    {
                                        prp.destinationPixelPositionXList.Add(playerPositionXInPix + (xOffSetInSquares * gv.squareSize));
                                        prp.destinationPixelPositionYList.Add(playerPositionYInPix + (yOffSetInSquares * gv.squareSize));
                                    }

                                    recursiveCall = true;
                                    moveToTarget(targetX, targetY, prp, 1);
                                    recursiveCall = false;
                                    return;
                                }
                                //Skip whole move, ie 0 steps on path (rolled a 1 as random roll)
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                    //target square is (finally) not occupied
                    else
                    {
                        //WIP
                        if ((newCoor.X < prp.LocationX) && (!prp.PropFacingLeft)) //move left
                        {
                            //TODO                        prp.token = gv.cc.flip(prp.token);
                            prp.PropFacingLeft = true;
                        }
                        else if ((newCoor.X > prp.LocationX) && (prp.PropFacingLeft)) //move right
                        {
                            //TODO                        prp.token = gv.cc.flip(prp.token);
                            prp.PropFacingLeft = false;
                        }//3

                        prp.LocationX = newCoor.X;
                        prp.LocationY = newCoor.Y;

                        int xOffSetInSquares = 0;
                        int yOffSetInSquares = 0;
                        if (gv.mod.PlayerLocationX >= prp.LocationX)
                        {
                            xOffSetInSquares = prp.LocationX - gv.mod.PlayerLocationX;
                        }
                        else
                        {
                            xOffSetInSquares = prp.LocationX - gv.mod.PlayerLocationX;
                        }
                        if (gv.mod.PlayerLocationY >= prp.LocationY)
                        {
                            yOffSetInSquares = prp.LocationY - gv.mod.PlayerLocationY;
                        }
                        else
                        {
                            yOffSetInSquares = prp.LocationY - gv.mod.PlayerLocationY;
                        }
                        int playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffsetX * gv.squareSize);
                        int playerPositionYInPix = gv.playerOffsetY * gv.squareSize;

                        if ((xOffSetInSquares <= 5) && (xOffSetInSquares >= -5) && (yOffSetInSquares <= 5) && (yOffSetInSquares >= -5))
                        {
                            prp.destinationPixelPositionXList.Add(playerPositionXInPix + (xOffSetInSquares * gv.squareSize));
                            prp.destinationPixelPositionYList.Add(playerPositionYInPix + (yOffSetInSquares * gv.squareSize));
                        }

                    }
                }
                prp.pixelMoveSpeed = prp.destinationPixelPositionXList.Count;
            }
            else
            {
                Random rnd2 = new Random();
                for (int i = 0; i < moveDistance; i++)
                {
                    gv.pfa.resetGrid();
                    Coordinate newCoor = gv.pfa.findNewPoint(new Coordinate(prp.LocationX, prp.LocationY), new Coordinate(targetX, targetY), prp);
                    if ((newCoor.X == -1) && (newCoor.Y == -1))
                    {
                        //didn't find a path, don't move
                        //why do we eliminate the moveto target here? Was fitting for a static world (that would never open a new path anyway). In a dynamic world I think we better keep the moveto information?
                        //as props can move through each other now (or that's the plan) it might still work with theese lines
                        //then again, mayhaps dynamic collision controlled waits are still useful, like e.g. blocked city gates (collission static props) would be nice to queue NPCs in the morning
                        //prp.CurrentMoveToTarget.X = prp.LocationX;
                        //prp.CurrentMoveToTarget.Y = prp.LocationY;
                        //gv.Invalidate();
//                        gv.Render();
                        return;
                    }

                    //new code for preventing movers from ending on the same square
                    bool nextStepSquareIsOccupied = false;
                    foreach (Prop otherProp in gv.mod.currentArea.Props)
                    {
                        //check whether an active mover prop is on the field found as next step on the path
                        if ((otherProp.LocationX == newCoor.X) && (otherProp.LocationY == newCoor.Y) && (otherProp.isMover == true) && (otherProp.isActive == true))
                        {
                            nextStepSquareIsOccupied = true;
                            break;
                        }
                    }

                    if (nextStepSquareIsOccupied == true)
                    {
                        bool originSquareOccupied = false;
                        //check wheter the next field found on path is the destination square, i.e. the waypoint our prop is headed to                                  
                        if ((newCoor.X == prp.CurrentMoveToTarget.X) && (newCoor.Y == prp.CurrentMoveToTarget.Y))
                        {
                            //let's find out whether our prop can stay on its origin square, i.e. skip move, or whether it already comes from an occupied square and has to "sidestep"
                            //Note: moving along path, double move, wont work when the target square is the destination square, i.e. the end of the path
                            foreach (Prop otherProp2 in gv.mod.currentArea.Props)
                            {
                                if ((otherProp2.LocationX == prp.LocationX) && (otherProp2.LocationY == prp.LocationY) && (otherProp2.isMover == true) && (otherProp2.isActive == true))
                                {
                                    if (otherProp2.PropTag != prp.PropTag)
                                    {
                                        originSquareOccupied = true;
                                        break;
                                    }
                                }
                            }

                            int decider2 = rnd2.Next(0, 2);
                            //another step forward, ie (at least) 2 steps on path
                            //check whether to stay on origin square ("step back")
                            if ((originSquareOccupied == false) && (decider2 == 0))
                            {//4
                                //memo to self: check what invalidate does                                         
                                //gv.Invalidate();
//                                gv.Render();
                                return;
                            }//4

                            //sidestep to nearby free square because destination square and origin square are both occupied
                            else
                            {//4
                                //find alternative target spot, as near to (occupied) destination square as possible
                                int targetTile = newCoor.Y * gv.mod.currentArea.MapSizeX + newCoor.X;//the index of the original target spot in the current area's tiles list
                                List<int> freeTilesByIndex = new List<int>();// a new list used to store the indices of all free tiles on current area
                                int tileLocX = 0;//just temporary storage in for locations of tiles
                                int tileLocY = 0;//just temporary storage in for locations of tiles
                                double floatTileLocY = 0;//was uncertain about rounding and conversion details, therefore need this one (see below)
                                bool tileIsFree = true;//identify a tile suited as new alternative target loaction
                                int nearestTileByIndex = -1;//store the nearest tile by index; as the relevant loop runs this will be replaced several times likely with ever nearer tiles
                                int dist = 0;//distance between the orignally intended target location (i.e. destination squre) and a free tile
                                int deltaX = 0;//temporary value used for distance calculation 
                                int deltaY = 0;//temporary value used for distance calculation 

                                //FIRST PART: get all FREE tiles on the current area
                                for (int j = 0; j < gv.mod.currentArea.Tiles.Count; j++)
                                {
                                    //get the x and y location of current tile by calculation derived from index number, assuming that counting starts at top left corner of a map (0x, 0y)
                                    //and that each horizintal x-line is counted first, then counting next horizonal x-line starting from the left again
                                    tileIsFree = true;
                                    //Note: When e.g. MapsizeY is 7, the y values range from 0 to 6
                                    tileLocX = j % gv.mod.currentArea.MapSizeY;
                                    //Note: ensure rounding down here 
                                    floatTileLocY = j / gv.mod.currentArea.MapSizeX;
                                    tileLocY = (int)Math.Floor(floatTileLocY);

                                    //look at content of currently checked tile, with three checks for walkable, occupied by creature, occupied by pc
                                    //walkbale check
                                    if (gv.mod.currentArea.Tiles[j].Walkable == false)
                                    {
                                        tileIsFree = false;
                                    }

                                    //party occupied check
                                    if (tileIsFree == true)
                                    {
                                        if ((gv.mod.PlayerLocationX == tileLocX) && (gv.mod.PlayerLocationY == tileLocY))
                                        {
                                            tileIsFree = false;
                                        }
                                    }

                                    //creature occupied check
                                    if (tileIsFree == true)
                                    {
                                        foreach (Prop occupyingProp in gv.mod.currentArea.Props)
                                        {
                                            if ((occupyingProp.LocationX == tileLocX) && (occupyingProp.LocationY == tileLocY))
                                            {
                                                tileIsFree = false;
                                                break;
                                            }
                                        }
                                    }

                                    //this writes all free tiles into a fresh list; please note that the values of the elements of this new list are our relevant index values
                                    //therefore it's not the index (which doesnt correalte to locations) in this list that's relevant, but the value of the element at that index
                                    if (tileIsFree == true)
                                    {
                                        freeTilesByIndex.Add(j);
                                    }
                                }

                                //SECOND PART: find the free tile NEAREST to originally intended summon location
                                for (int k = 0; k < freeTilesByIndex.Count; k++)
                                {//5
                                    dist = 0;

                                    //get location x and y of the tile stored at the index number j, i.e. get the value of elment indexed with i and transform to x and y location
                                    tileLocX = freeTilesByIndex[k] % gv.mod.currentArea.MapSizeY;
                                    floatTileLocY = freeTilesByIndex[k] / gv.mod.currentArea.MapSizeX;
                                    tileLocY = (int)Math.Floor(floatTileLocY);

                                    //get distance between the current free tile and the originally intended target location (i.e. teh deistination square in this case, aka path end)
                                    deltaX = (int)Math.Abs((tileLocX - prp.LocationX));
                                    deltaY = (int)Math.Abs((tileLocY - prp.LocationY));
                                    if (deltaX > deltaY)
                                    {
                                        dist = deltaX;
                                    }
                                    else
                                    {
                                        dist = deltaY;
                                    }


                                    //only very close to target tiles 
                                    if (dist < 3)
                                    {//6
                                        gv.pfa.resetGrid();
                                        Coordinate newCoor2 = gv.pfa.findNewPoint(new Coordinate(tileLocX, tileLocY), new Coordinate(targetX, targetY), prp, tileLocX, tileLocY, 6);
                                        if ((newCoor2.X != -1) && (newCoor2.Y != -1) && (prp.lengthOfLastPath < 4))
                                        {
                                            nearestTileByIndex = freeTilesByIndex[k];
                                            break;
                                        }
                                    }
                                }

                                if (nearestTileByIndex != -1)
                                {
                                    //get the nearest tile's x and y location and use it as target square coordinates
                                    tileLocX = nearestTileByIndex % gv.mod.currentArea.MapSizeY;
                                    floatTileLocY = nearestTileByIndex / gv.mod.currentArea.MapSizeX;
                                    tileLocY = (int)Math.Floor(floatTileLocY);

                                    prp.LocationX = tileLocX;
                                    prp.LocationY = tileLocY;
                                }
                            }
                        }

                        //the (occupied) target square is not the destination, i.e. path end, square
                        else
                        {
                            originSquareOccupied = false;
                            //check whether origin square is occupied, too 
                            foreach (Prop otherProp2 in gv.mod.currentArea.Props)
                            {
                                if ((otherProp2.LocationX == prp.LocationX) && (otherProp2.LocationY == prp.LocationY) && (otherProp2.isMover == true) && (otherProp2.isActive == true))
                                {
                                    if (otherProp2.PropTag != prp.PropTag)
                                    {
                                        originSquareOccupied = true;
                                        break;
                                    }
                                }
                            }
                            //origin square is occupied, waiting is no option therefore, so we must do (at least) double move forward (target square is occupied, too)
                            if (originSquareOccupied == true)
                            {
                                //careful, watch for infinite loop, recursive calling here
                                prp.LocationX = newCoor.X;
                                prp.LocationY = newCoor.Y;
                                moveToTarget(targetX, targetY, prp, 1);
                                return;
                            }
                            //origin square not occupied, so waiting would be an alternative to double move: 50/50 situation
                            else
                            {
                                //make random roll to randomly choose between the two next alternatives
                                int decider = rnd2.Next(0, 2);
                                //another step forward, ie (at least) 2 steps on path
                                if (decider == 0)
                                {
                                    prp.LocationX = newCoor.X;
                                    prp.LocationY = newCoor.Y;
                                    //recursive call, careful
                                    moveToTarget(targetX, targetY, prp, 1);
                                    return;
                                }
                                //Skip whole move, ie 0 steps on path (rolled a 1 as random roll)
                                else
                                {
                                    //memo to self: check what invalidate does
                                    //gv.Invalidate();
//                                    gv.Render();
                                    return;
                                }
                            }
                        }
                    }
                    //target square is (finally) not occupied
                    else
                    {
                        //WIP
                        if ((newCoor.X < prp.LocationX) && (!prp.PropFacingLeft)) //move left
                        {
                            //TODO                        prp.token = gv.cc.flip(prp.token);
                            prp.PropFacingLeft = true;
                        }
                        else if ((newCoor.X > prp.LocationX) && (prp.PropFacingLeft)) //move right
                        {
                            //TODO                        prp.token = gv.cc.flip(prp.token);
                            prp.PropFacingLeft = false;
                        }
                        prp.LocationX = newCoor.X;
                        prp.LocationY = newCoor.Y;

                    }
                }
            }
        }

        public void applyEffects()
        {
            try
            {
                foreach (Player pc in gv.mod.playerList)
                {
                    foreach (Effect ef in pc.effectsList)
                    {
                        //decrement duration of all
                        ef.durationInUnits -= gv.mod.currentArea.TimePerSquare;
                        if (!ef.usedForUpdateStats) //not used for stat updates
                        {
                            doEffectScript(pc, ef);
                        }
                    }
                }
                //if remaining duration <= 0, remove from list
                foreach (Player pc in gv.mod.playerList)
                {
                    for (int i = pc.effectsList.Count; i > 0; i--)
                    {
                        if (pc.effectsList[i - 1].durationInUnits <= 0)
                        {
                            pc.effectsList.RemoveAt(i - 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }
        }
        public void doEffectScript(object src, Effect ef)
        {
            if (ef.effectScript.Equals("efGeneric"))
            {
                gv.sf.efGeneric(src, ef);
            }

            else if (ef.effectScript.Equals("efHeld"))
            {
                gv.sf.efHeld(src, ef);
            }
            else if (ef.effectScript.Equals("efSleep"))
            {
                gv.sf.efSleep(src, ef);
            }
            else if (ef.effectScript.Equals("efRegenMinor"))
            {
                gv.sf.efRegenMinor(src, ef);
            }
            else if (ef.effectScript.Equals("efPoisonedLight"))
            {
                gv.sf.efPoisoned(src, ef, 2);
            }
            else if (ef.effectScript.Equals("efPoisonedMedium"))
            {
                gv.sf.efPoisoned(src, ef, 4);
            }
        }
        public void doPropTriggers()
        {
            try
            {
                //search area for any props that share the party location
                bool foundOne = false;
                foreach (Prop prp in gv.mod.currentArea.Props)
                {
                    bool doNotTriggerProp = false;
                    if ((prp.isMover == false) || ((prp.MoverType == "Post") && (prp.isChaser == false)))
                    {
                        if (gv.realTimeTimerMilliSecondsEllapsed >= gv.mod.realTimeTimerLengthInMilliSeconds)
                        {
                            doNotTriggerProp = true;
                        }
                    }
                                        
                    if ((prp.LocationX == gv.mod.PlayerLocationX) && (prp.LocationY == gv.mod.PlayerLocationY) && (prp.isActive) && (doNotTriggerProp == false))
                    {
                        //prp.wasTriggeredLastUpdate = true;
                        foundOne = true;
                        blockSecondPropTriggersCall = true;
                        gv.triggerPropIndex++;
                        if ((gv.triggerPropIndex == 1) && (!prp.ConversationWhenOnPartySquare.Equals("none")))
                        {

                            if (prp.unavoidableConversation == true)
                            {
                                calledConvoFromProp = true;
                                gv.sf.ThisProp = prp;
                                if ((gv.mod.useSmoothMovement) && (prp.isMover))
                                {
                                    for (int i = 0; i < 30; i++)
                                    {
                                        gv.Render(0);
                                    }
                                }
                                doConversationBasedOnTag(prp.ConversationWhenOnPartySquare);
                                break;
                            }
                            else if (gv.mod.avoidInteraction == false)
                            {
                                calledConvoFromProp = true;
                                gv.sf.ThisProp = prp;
                                
                                if ((gv.mod.useSmoothMovement) && (prp.isMover))
                                {
                                    for (int i = 0; i < 30; i++)
                                    {
                                                                                gv.Render(0);
                                    }
                                }
                                
                                doConversationBasedOnTag(prp.ConversationWhenOnPartySquare);
                                break;
                            }
                            else
                            {
                                foundOne = false;
                                break;
                            }

                        }
                        else if ((gv.triggerPropIndex == 2) && (!prp.EncounterWhenOnPartySquare.Equals("none")))
                        {
                            calledEncounterFromProp = true;
                            gv.sf.ThisProp = prp;
                            
                            if ((gv.mod.useSmoothMovement) && (prp.isMover))
                            {
                                for (int i = 0; i < 30; i++)
                                {
                                                                        gv.Render(0);
                                }
                            }
                           
                            doEncounterBasedOnTag(prp.EncounterWhenOnPartySquare);
                            break;
                        }
                        else if (gv.triggerPropIndex < 3)
                        {
                            gv.mod.isRecursiveCall = true;
                            doPropTriggers();
                            gv.mod.isRecursiveCall = false;
                            break;
                        }
                        if (gv.triggerPropIndex > 2)
                        {
                            gv.triggerPropIndex = 0;
                            //set flags back to false
                            calledConvoFromProp = false;
                            calledEncounterFromProp = false;
                            foundOne = false;
                            //delete prop if flag is set to do so and break foreach loop
                            if (prp.DeletePropWhenThisEncounterIsWon)
                            {
                                gv.mod.currentArea.Props.Remove(prp);
                            }
                            break;
                        }
                    }
                }
                if (!foundOne)
                {
                    doTrigger();
                }
            }
            catch (Exception ex)
            {
                if (gv.mod.debugMode)
                {
                    gv.sf.MessageBox("failed to do prop trigger: " + ex.ToString());
                    gv.errorLog(ex.ToString());
                }
            }
        }
        public void doTrigger()
        {
            if (gv.realTimeTimerMilliSecondsEllapsed < gv.mod.realTimeTimerLengthInMilliSeconds)
            {
                try
                {
                    Trigger trig = gv.mod.currentArea.getTriggerByLocation(gv.mod.PlayerLocationX, gv.mod.PlayerLocationY);
                    if ((trig != null) && (trig.Enabled))
                    {
                        blockSecondPropTriggersCall = true;
                        //iterate through each event                  
                        //#region Event1 stuff
                        //check to see if enabled and parm not "none"
                        /*for (int i = 0; i < 15; i++)
                        {
                            gv.Render();
                        }*/
                        gv.triggerIndex++;

                        if ((gv.triggerIndex == 1) && (trig.EnabledEvent1) && (!trig.Event1FilenameOrTag.Equals("none")))
                        {
                            //check to see what type of event
                            if (trig.Event1Type.Equals("container"))
                            {
                                doContainerBasedOnTag(trig.Event1FilenameOrTag);
                                doTrigger();
                            }
                            else if (trig.Event1Type.Equals("transition"))
                            {
                                doTransitionBasedOnAreaLocation(trig.Event1FilenameOrTag, trig.Event1TransPointX, trig.Event1TransPointY);
                            }
                            else if (trig.Event1Type.Equals("conversation"))
                            {
                                if (trig.conversationCannotBeAvoided == true)
                                {
                                    doConversationBasedOnTag(trig.Event1FilenameOrTag);
                                }
                                else if (gv.mod.avoidInteraction == false)
                                {
                                    doConversationBasedOnTag(trig.Event1FilenameOrTag);
                                }
                            }
                            else if (trig.Event1Type.Equals("encounter"))
                            {
                                doEncounterBasedOnTag(trig.Event1FilenameOrTag);
                            }
                            else if (trig.Event1Type.Equals("script"))
                            {
                                doScriptBasedOnFilename(trig.Event1FilenameOrTag, trig.Event1Parm1, trig.Event1Parm2, trig.Event1Parm3, trig.Event1Parm4);
                                doTrigger();
                            }
                            else if (trig.Event1Type.Equals("ibscript"))
                            {
                                doIBScriptBasedOnFilename(trig.Event1FilenameOrTag, trig.Event1Parm1);
                                doTrigger();
                            }
                            //do that event
                            if (trig.DoOnceOnlyEvent1)
                            {
                                trig.EnabledEvent1 = false;
                            }
                        }
                        //#endregion
                        //#region Event2 stuff
                        //check to see if enabled and parm not "none"
                        else if ((gv.triggerIndex == 2) && (trig.EnabledEvent2) && (!trig.Event2FilenameOrTag.Equals("none")))
                        {
                            //check to see what type of event
                            if (trig.Event2Type.Equals("container"))
                            {
                                doContainerBasedOnTag(trig.Event2FilenameOrTag);
                                doTrigger();
                            }
                            else if (trig.Event2Type.Equals("transition"))
                            {
                                doTransitionBasedOnAreaLocation(trig.Event2FilenameOrTag, trig.Event2TransPointX, trig.Event2TransPointY);
                            }
                            else if (trig.Event2Type.Equals("conversation"))
                            {
                                if (trig.conversationCannotBeAvoided == true)
                                {
                                    doConversationBasedOnTag(trig.Event2FilenameOrTag);
                                }
                                else if (gv.mod.avoidInteraction == false)
                                {
                                    doConversationBasedOnTag(trig.Event2FilenameOrTag);
                                }
                            }
                            else if (trig.Event2Type.Equals("encounter"))
                            {
                                doEncounterBasedOnTag(trig.Event2FilenameOrTag);
                            }
                            else if (trig.Event2Type.Equals("script"))
                            {
                                doScriptBasedOnFilename(trig.Event2FilenameOrTag, trig.Event2Parm1, trig.Event2Parm2, trig.Event2Parm3, trig.Event2Parm4);
                                doTrigger();
                            }
                            else if (trig.Event1Type.Equals("ibscript"))
                            {
                                doIBScriptBasedOnFilename(trig.Event2FilenameOrTag, trig.Event2Parm1);
                                doTrigger();
                            }
                            //do that event
                            if (trig.DoOnceOnlyEvent2)
                            {
                                trig.EnabledEvent2 = false;
                            }
                        }
                        //#endregion
                        //#region Event3 stuff
                        //check to see if enabled and parm not "none"
                        else if ((gv.triggerIndex == 3) && (trig.EnabledEvent3) && (!trig.Event3FilenameOrTag.Equals("none")))
                        {
                            //check to see what type of event
                            if (trig.Event3Type.Equals("container"))
                            {
                                doContainerBasedOnTag(trig.Event3FilenameOrTag);
                                doTrigger();
                            }
                            else if (trig.Event3Type.Equals("transition"))
                            {
                                doTransitionBasedOnAreaLocation(trig.Event3FilenameOrTag, trig.Event3TransPointX, trig.Event3TransPointY);
                            }
                            else if (trig.Event3Type.Equals("conversation"))
                            {
                                if (trig.conversationCannotBeAvoided == true)
                                {
                                    doConversationBasedOnTag(trig.Event3FilenameOrTag);
                                }
                                else if (gv.mod.avoidInteraction == false)
                                {
                                    doConversationBasedOnTag(trig.Event3FilenameOrTag);
                                }
                            }
                            else if (trig.Event3Type.Equals("encounter"))
                            {
                                doEncounterBasedOnTag(trig.Event3FilenameOrTag);
                            }
                            else if (trig.Event3Type.Equals("script"))
                            {
                                doScriptBasedOnFilename(trig.Event3FilenameOrTag, trig.Event3Parm1, trig.Event3Parm2, trig.Event3Parm3, trig.Event3Parm4);
                                doTrigger();
                            }
                            else if (trig.Event1Type.Equals("ibscript"))
                            {
                                doIBScriptBasedOnFilename(trig.Event3FilenameOrTag, trig.Event3Parm1);
                                doTrigger();
                            }
                            //do that event
                            if (trig.DoOnceOnlyEvent3)
                            {
                                trig.EnabledEvent3 = false;
                            }
                        }
                        else if (gv.triggerIndex < 4)
                        {
                            doTrigger();
                        }
                        //#endregion
                        if (gv.triggerIndex > 3)
                        {
                            gv.triggerIndex = 0;
                            if (trig.DoOnceOnly)
                            {
                                trig.Enabled = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (gv.mod.debugMode)
                    {
                        gv.sf.MessageBox("failed to do trigger: " + ex.ToString());
                        gv.errorLog(ex.ToString());
                    }
                }
            }
        }

        public void doContainerBasedOnTag(string tag)
        {

            try
            {
                Container container = gv.mod.getContainerByTag(tag);
                gv.screenType = "itemSelector";
                gv.screenItemSelector.resetItemSelector(container.containerItemRefs, "container", "main");
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }

        }
        public void doConversationBasedOnTag(string tag)
        {
           
            //if (gv.mod.doConvo)
            //{
                try
                {
                    gv.mod.allowImmediateRetransition = true;
                    LoadCurrentConvo(tag);
                    gv.screenType = "convo";
                    gv.screenConvo.startConvo();
                }
                catch (Exception ex)
                {
                    gv.sf.MessageBox("failed to open conversation with tag: " + tag);
                }
            //}
            //else
            //{
                //gv.mod.doConvo = true;
            //}
        }
        public void doSpellBasedOnScriptOrEffectTag(Spell spell, object source, object target)
        {
            gv.sf.AoeTargetsList.Clear();

            if (!spell.spellEffectTag.Equals("none"))
            {
                gv.sf.spGeneric(spell, source, target);
            }

            //WIZARD SPELLS
            else if (spell.spellScript.Equals("spFlameFingers"))
            {
                gv.sf.spFlameFingers(source, target, spell);
            }
            else if (spell.spellScript.Equals("spMageBolt"))
            {
                gv.sf.spMageBolt(source, target);
            }
            else if (spell.spellScript.Equals("spSleep"))
            {
                gv.sf.spSleep(source, target, spell);
            }
            else if (spell.spellScript.Equals("spMageArmor"))
            {
                gv.sf.spMageArmor(source, target);
            }
            else if (spell.spellScript.Equals("spMinorRegen"))
            {
                gv.sf.spMinorRegen(source, target);
            }
            else if (spell.spellScript.Equals("spWeb"))
            {
                gv.sf.spWeb(source, target, spell);
            }
            else if (spell.spellScript.Equals("spIceStorm"))
            {
                gv.sf.spIceStorm(source, target, spell);
            }
            else if (spell.spellScript.Equals("spFireball"))
            {
                gv.sf.spFireball(source, target, spell);
            }
            else if (spell.spellScript.Equals("spLightning"))
            {
                gv.sf.spLightning(source, target, spell);
            }
            
            //CLERIC SPELLS
            else if (spell.tag.Equals("minorHealing"))
            {
                gv.sf.spHeal(source, target, 8);
            }
            else if (spell.tag.Equals("moderateHealing"))
            {
                gv.sf.spHeal(source, target, 16);
            }
            else if (spell.tag.Equals("massMinorHealing"))
            {
                gv.sf.spMassHeal(source, target, 8);
            }
            else if (spell.spellScript.Equals("spBless"))
            {
                gv.sf.spBless(source, target);
            }
            else if (spell.spellScript.Equals("spMagicStone"))
            {
                gv.sf.spMagicStone(source, target);
            }
            else if (spell.spellScript.Equals("spBlastOfLight"))
            {
                gv.sf.spBlastOfLight(source, target, spell);
            }
            else if (spell.spellScript.Equals("spHold"))
            {
                gv.sf.spHold(source, target);
            }
        }
        public void doScriptBasedOnFilename(string filename, string prm1, string prm2, string prm3, string prm4)
        {
            if (!filename.Equals("none"))
            {
                //send to ga, gc, og, or os controllers
                if (filename.StartsWith("gc"))
                {
                    gv.sf.gcController(filename, prm1, prm2, prm3, prm4);
                }
                else if (filename.StartsWith("ga"))
                {
                    gv.sf.gaController(filename, prm1, prm2, prm3, prm4);
                }
                else if (filename.StartsWith("og"))
                {
                    gv.sf.ogController(filename, prm1, prm2, prm3, prm4);
                }
                else if (filename.StartsWith("os"))
                {
                    gv.sf.osController(filename, prm1, prm2, prm3, prm4);
                }
            }
        }
        public void doIBScriptBasedOnFilename(string filename, string parms)
        {
            try
            {
                if (!filename.Equals("none"))
                {
                    IBScriptEngine e = new IBScriptEngine(gv, filename, parms);
                    e.RunScript();
                }
            }
            catch (Exception ex)
            {
                gv.sf.MessageBox("failed to run IBScript: " + filename);
            }
        }
        public void doEncounterBasedOnTag(string name)
        {
            try
            {
                gv.mod.currentEncounter = gv.mod.getEncounter(name);
                if (gv.mod.currentEncounter.encounterCreatureRefsList.Count > 0)
                {
                    gv.screenCombat.doCombatSetup();
                    int foundOnePc = 0;
                    foreach (Player chr in gv.mod.playerList)
                    {
                        if (chr.hp > 0)
                        {
                            foundOnePc = 1;
                        }
                    }
                    if (foundOnePc == 0)
                    {
                        //IBMessageBox.Show(game, "Party is wiped out...game over");
                    }
                }
                else
                {
                    //IBMessageBox.Show(game, "no creatures left here"); 
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }
        }

        public bool goWest()
        {
            bool doTransition = false;

            bool foundNeighbourArea = false;
            int indexOfNeighbourMap = 1000000;

            if ((gv.mod.PlayerLocationX == gv.mod.borderAreaSize) && (gv.mod.currentArea.westernNeighbourArea == ""))
            {
                gv.cc.addLogText("red", "No neigbhbouring area existent.");
            }

            if ((gv.mod.PlayerLocationX == gv.mod.borderAreaSize) && (gv.mod.currentArea.westernNeighbourArea != ""))
            {
                for (int i = 0; i <= gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.westernNeighbourArea)
                    {
                        foundNeighbourArea = true;
                        indexOfNeighbourMap = i;
                        break;
                    }
                }

                if (foundNeighbourArea)
                {
                    if (((gv.mod.moduleAreasObjects[indexOfNeighbourMap].MapSizeY - 1) >= gv.mod.PlayerLocationY))
                    {
                        //check for block on other side
                        if (gv.mod.moduleAreasObjects[indexOfNeighbourMap].GetBlocked(gv.mod.moduleAreasObjects[indexOfNeighbourMap].MapSizeX - 1 - gv.mod.borderAreaSize, gv.mod.PlayerLocationY) == false)
                        {
                            int xTargetCoordinate = gv.mod.moduleAreasObjects[indexOfNeighbourMap].MapSizeX - 1 - gv.mod.borderAreaSize;
                            int yTargetCoordinate = gv.mod.PlayerLocationY;
                            gv.mod.allowImmediateRetransition = true;
                            gv.cc.doTransitionBasedOnAreaLocation(gv.mod.moduleAreasObjects[indexOfNeighbourMap].Filename, xTargetCoordinate, yTargetCoordinate);
                            doTransition = true;
                        }
                        else
                        {
                            gv.cc.addLogText("red", "Something blocks the path from the other side.");
                        }
                    }
                    else
                    {
                        gv.cc.addLogText("red", "The neigbhbouring area does not touch this border section.");
                    }
                    
                }
                else
                {
                    gv.cc.addLogText("red", "No known neigbhbouring area existent.");
                }
            }

            return doTransition;
        }

        public bool goEast()
        {
            bool doTransition = false;

            bool foundNeighbourArea = false;
            int indexOfNeighbourMap = 1000000;

            if ((gv.mod.PlayerLocationX == (gv.mod.currentArea.MapSizeX - 1 - gv.mod.borderAreaSize)) && (gv.mod.currentArea.easternNeighbourArea == ""))
            {
                gv.cc.addLogText("red", "No neigbhbouring area existent.");
            }

            if ((gv.mod.PlayerLocationX == (gv.mod.currentArea.MapSizeX - 1 - gv.mod.borderAreaSize)) && (gv.mod.currentArea.easternNeighbourArea != ""))
            {
                for (int i = 0; i <= gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.easternNeighbourArea)
                    {
                        foundNeighbourArea = true;
                        indexOfNeighbourMap = i;
                        break;
                    }
                }

                if (foundNeighbourArea)
                {
                    if (((gv.mod.moduleAreasObjects[indexOfNeighbourMap].MapSizeY - 1) >= gv.mod.PlayerLocationY))
                    {
                        //check for block on other side
                        if (gv.mod.moduleAreasObjects[indexOfNeighbourMap].GetBlocked(gv.mod.borderAreaSize, gv.mod.PlayerLocationY) == false)
                        {
                            int xTargetCoordinate = gv.mod.borderAreaSize;
                            int yTargetCoordinate = gv.mod.PlayerLocationY;
                            gv.mod.allowImmediateRetransition = true;
                            gv.cc.doTransitionBasedOnAreaLocation(gv.mod.moduleAreasObjects[indexOfNeighbourMap].Filename, xTargetCoordinate, yTargetCoordinate);
                            doTransition = true;
                        }
                        else
                        {
                            gv.cc.addLogText("red", "Something blocks the path from the other side.");
                        }
                    }
                    else
                    {
                        gv.cc.addLogText("red", "The neigbhbouring area does not touch this border section.");
                    }

                }
                else
                {
                    gv.cc.addLogText("red", "No known neigbhbouring area existent.");
                }
            }

            return doTransition;
        }

        public bool goNorth()
        {
            bool doTransition = false;

            bool foundNeighbourArea = false;
            int indexOfNeighbourMap = 1000000;

            if ((gv.mod.PlayerLocationY == gv.mod.borderAreaSize) && (gv.mod.currentArea.northernNeighbourArea == ""))
            {
                gv.cc.addLogText("red", "No neigbhbouring area existent.");
            }

            if ((gv.mod.PlayerLocationY == gv.mod.borderAreaSize) && (gv.mod.currentArea.northernNeighbourArea != ""))
            {
                for (int i = 0; i <= gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.northernNeighbourArea)
                    {
                        foundNeighbourArea = true;
                        indexOfNeighbourMap = i;
                        break;
                    }
                }

                if (foundNeighbourArea)
                {
                    if (((gv.mod.moduleAreasObjects[indexOfNeighbourMap].MapSizeX - 1) >= gv.mod.PlayerLocationX))
                    {
                        //check for block on other side
                        if (gv.mod.moduleAreasObjects[indexOfNeighbourMap].GetBlocked(gv.mod.PlayerLocationX, gv.mod.moduleAreasObjects[indexOfNeighbourMap].MapSizeY - 1 - gv.mod.borderAreaSize) == false)
                        {
                            int xTargetCoordinate = gv.mod.PlayerLocationX;
                            int yTargetCoordinate = gv.mod.moduleAreasObjects[indexOfNeighbourMap].MapSizeY - 1 - gv.mod.borderAreaSize;
                            gv.mod.allowImmediateRetransition = true;
                            gv.cc.doTransitionBasedOnAreaLocation(gv.mod.moduleAreasObjects[indexOfNeighbourMap].Filename, xTargetCoordinate, yTargetCoordinate);
                            doTransition = true;
                        }
                        else
                        {
                            gv.cc.addLogText("red", "Something blocks the path from the other side.");
                        }
                    }
                    else
                    {
                        gv.cc.addLogText("red", "The neigbhbouring area does not touch this border section.");
                    }

                }
                else
                {
                    gv.cc.addLogText("red", "No known neigbhbouring area existent.");
                }
            }

            return doTransition;
        }

        public bool goSouth()
        {
            bool doTransition = false;

            bool foundNeighbourArea = false;
            int indexOfNeighbourMap = 1000000;

            if ((gv.mod.PlayerLocationY == (gv.mod.currentArea.MapSizeY - 1 - gv.mod.borderAreaSize)) && (gv.mod.currentArea.southernNeighbourArea == ""))
            {
                gv.cc.addLogText("red", "No neigbhbouring area existent.");
            }

            if ((gv.mod.PlayerLocationY == (gv.mod.currentArea.MapSizeY - 1 - gv.mod.borderAreaSize)) && (gv.mod.currentArea.southernNeighbourArea != ""))
            {
                for (int i = 0; i <= gv.mod.moduleAreasObjects.Count; i++)
                {
                    if (gv.mod.moduleAreasObjects[i].Filename == gv.mod.currentArea.southernNeighbourArea)
                    {
                        foundNeighbourArea = true;
                        indexOfNeighbourMap = i;
                        break;
                    }
                }

                if (foundNeighbourArea)
                {
                    if (((gv.mod.moduleAreasObjects[indexOfNeighbourMap].MapSizeX - 1) >= gv.mod.PlayerLocationX))
                    {
                        //check for block on other side
                        if (gv.mod.moduleAreasObjects[indexOfNeighbourMap].GetBlocked(gv.mod.PlayerLocationX, gv.mod.borderAreaSize) == false)
                        {
                            int xTargetCoordinate = gv.mod.PlayerLocationX;
                            int yTargetCoordinate = gv.mod.borderAreaSize;
                            gv.mod.allowImmediateRetransition = true;
                            gv.cc.doTransitionBasedOnAreaLocation(gv.mod.moduleAreasObjects[indexOfNeighbourMap].Filename, xTargetCoordinate, yTargetCoordinate);
                            doTransition = true;
                        }
                        else
                        {
                            gv.cc.addLogText("red", "Something blocks the path from the other side.");
                        }
                    }
                    else
                    {
                        gv.cc.addLogText("red", "The neigbhbouring area does not touch this border section.");
                    }

                }
                else
                {
                    gv.cc.addLogText("red", "No known neigbhbouring area existent.");
                }
            }

            return doTransition;
        }

        public void doTransitionBasedOnAreaLocation(string areaFilename, int x, int y)
        {
            try
            {

                if ((gv.mod.justTransitioned == false) || (gv.mod.allowImmediateRetransition == true))
                {
                    gv.mod.PlayerLocationX = x;
                    gv.mod.PlayerLocationY = y;
                    if (gv.mod.allowImmediateRetransition == true)
                    {
                        gv.mod.allowImmediateRetransition = false;
                    }
                
                    storeCurrentWeatherSettings();
                    gv.mod.justTransitioned = true;
                    gv.mod.justTransitioned2 = true;
                    //}
                    gv.mod.arrivalSquareX = gv.mod.PlayerLocationX;
                    gv.mod.arrivalSquareY = gv.mod.PlayerLocationY;
                    //TDO: reset light!
                    //gv.mod.currentArea.Filename = areaFilename;
                    
                    //hmmm, is double (see below, must verify later)
                    if (gv.mod.playMusic)
                    {
                        gv.stopMusic();
                        gv.stopAmbient();
                        //gv.mod.resetWeatherSound = true;
                        //check later why this was needed, likely remove
                        //gv.weatherSounds2.controls.stop();
                    }

                    gv.mod.setCurrentArea(areaFilename, gv);
                    resetLightAndDarkness();
                    doIllumination();

                    if (gv.mod.currentArea.areaWeatherName == "")
                    {
                        gv.mod.currentArea.useFullScreenEffectLayer5 = false;
                        gv.mod.currentArea.useFullScreenEffectLayer6 = false;
                        gv.mod.currentArea.useFullScreenEffectLayer7 = false;
                        gv.mod.currentArea.useFullScreenEffectLayer8 = false;
                        gv.mod.currentArea.useFullScreenEffectLayer9 = false;
                        gv.mod.currentArea.useFullScreenEffectLayer10 = false;
                    }
                    if (gv.mod.currentArea.effectChannelScript1 == "")
                    {
                        gv.mod.currentArea.useFullScreenEffectLayer1 = false;
                    }
                    if (gv.mod.currentArea.effectChannelScript2 == "")
                    {
                        gv.mod.currentArea.useFullScreenEffectLayer2 = false;
                    }
                    if (gv.mod.currentArea.effectChannelScript3 == "")
                    {
                        gv.mod.currentArea.useFullScreenEffectLayer3 = false;
                    }
                    if (gv.mod.currentArea.effectChannelScript4 == "")
                    {
                        gv.mod.currentArea.useFullScreenEffectLayer4 = false;
                    }
                    //karl
                    //gv.log.AddHtmlTextToLog("<font color='red'>" + areaFilename + "</font>");
                    //gv.log.AddHtmlTextToLog("<font color='red'>" + gv.mod.currentArea.Filename + "</font>");


                    //weather related inserts
                    /*
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX1 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX2 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX3 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX4 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX5 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX6 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX7 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX8 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX9 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX10 = 0;

                    gv.mod.currentArea.fullScreenAnimationFrameCounterY1 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY2 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY3 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY4 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY5 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY6 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY7 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY8 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY9 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY10 = 0;

                    gv.mod.currentArea.fullScreenAnimationFrameCounter1 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter2 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter3 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter4 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter5 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter6 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter7 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter8 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter9 = 0;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter10 = 0;

                    gv.mod.currentArea.fullScreenEffectLayerIsActive1 = true;
                    gv.mod.currentArea.fullScreenEffectLayerIsActive2 = true;
                    gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                    gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                    gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                    gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                    gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                    gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                    gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                    gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                    */

                    //new ideas
                    /*
                    gv.mod.currentArea.numberOfCyclesPerOccurence1 = 0;
                    gv.mod.currentArea.numberOfCyclesPerOccurence2 = 0;
                    gv.mod.currentArea.numberOfCyclesPerOccurence3 = 0;
                    gv.mod.currentArea.numberOfCyclesPerOccurence4 = 0;
                    gv.mod.currentArea.numberOfCyclesPerOccurence5 = 0;
                    gv.mod.currentArea.numberOfCyclesPerOccurence6 = 0;
                    gv.mod.currentArea.numberOfCyclesPerOccurence7 = 0;
                    gv.mod.currentArea.numberOfCyclesPerOccurence8 = 0;
                    gv.mod.currentArea.numberOfCyclesPerOccurence9 = 0;
                    gv.mod.currentArea.numberOfCyclesPerOccurence10 = 0;

                    gv.mod.currentArea.cycleCounter1 = 0;
                    gv.mod.currentArea.cycleCounter2 = 0;
                    gv.mod.currentArea.cycleCounter3 = 0;
                    gv.mod.currentArea.cycleCounter4 = 0;
                    gv.mod.currentArea.cycleCounter5 = 0;
                    gv.mod.currentArea.cycleCounter6 = 0;
                    gv.mod.currentArea.cycleCounter7 = 0;
                    gv.mod.currentArea.cycleCounter8 = 0;
                    gv.mod.currentArea.cycleCounter9 = 0;
                    gv.mod.currentArea.cycleCounter10 = 0;

                    gv.mod.currentArea.changeCounter1 = 0;
                    gv.mod.currentArea.changeCounter2 = 0;
                    gv.mod.currentArea.changeCounter3 = 0;
                    gv.mod.currentArea.changeCounter4 = 0;
                    gv.mod.currentArea.changeCounter5 = 0;
                    gv.mod.currentArea.changeCounter6 = 0;
                    gv.mod.currentArea.changeCounter7 = 0;
                    gv.mod.currentArea.changeCounter8 = 0;
                    gv.mod.currentArea.changeCounter9 = 0;
                    gv.mod.currentArea.changeCounter10 = 0;

                    gv.mod.currentArea.changeFrameCounter1 = 1;
                    gv.mod.currentArea.changeFrameCounter2 = 1;
                    gv.mod.currentArea.changeFrameCounter3 = 1;
                    gv.mod.currentArea.changeFrameCounter4 = 1;
                    gv.mod.currentArea.changeFrameCounter5 = 1;
                    gv.mod.currentArea.changeFrameCounter6 = 1;
                    gv.mod.currentArea.changeFrameCounter7 = 1;
                    gv.mod.currentArea.changeFrameCounter8 = 1;
                    gv.mod.currentArea.changeFrameCounter9 = 1;
                    gv.mod.currentArea.changeFrameCounter10 = 1;
                    */

                    //try to keep weather consistency intact
                    //gv.mod.currentWeatherName = "";

                    //end of new ideas
                    doChannelScripts();
                    doWeather();

                    gv.screenMainMap.resetMiniMapBitmap();

                    doOnEnterAreaUpdate = true;
                    doPropMoves();
                    doOnEnterAreaUpdate = false;
                    //if (gv.mod.playMusic)
                    //{
                        //gv.startMusic();
                        //gv.startAmbient();
                    //}
                    gv.triggerIndex = 0;
                    doTrigger();
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
            }

            //for testing sprites (temporary disable)
            //createSpritesForTesting();

            /*
            try
            {
                if (gv.mod.loadedTileBitmaps != null)
                {
                    foreach (SharpDX.Direct2D1.Bitmap bm in gv.mod.loadedTileBitmaps)
                    {
                        bm.Dispose();
                    }
                }

                //these two lists keep an exact order so each bitmap stored in one corrsponds with a name in the other

                gv.mod.loadedTileBitmaps.Clear();
                gv.mod.loadedTileBitmapsNames.Clear();
            }
            catch { }
            */
        }

        public void storeCurrentWeatherSettings()
        {

        gv.mod.overrideDelayCounter1 = gv.mod.currentArea.overrideDelayCounter1;
        gv.mod.cycleCounter1 = gv.mod.currentArea.cycleCounter1;
        gv.mod.fullScreenAnimationFrameCounter1 = gv.mod.currentArea.fullScreenAnimationFrameCounter1;
        gv.mod.changeCounter1 = gv.mod.currentArea.changeCounter1;
        gv.mod.changeFrameCounter1 = gv.mod.currentArea.changeFrameCounter1;
        gv.mod.fullScreenAnimationSpeedX1 = gv.mod.currentArea.fullScreenAnimationSpeedX1; 
        gv.mod.fullScreenAnimationSpeedY1 = gv.mod.currentArea.fullScreenAnimationSpeedY1;
        gv.mod.fullScreenAnimationFrameCounterX1 = gv.mod.currentArea.fullScreenAnimationFrameCounterX1;
        gv.mod.fullScreenAnimationFrameCounterY1 = gv.mod.currentArea.fullScreenAnimationFrameCounterY1;

        gv.mod.overrideDelayCounter2 = gv.mod.currentArea.overrideDelayCounter2;
        gv.mod.cycleCounter2 = gv.mod.currentArea.cycleCounter2;
        gv.mod.fullScreenAnimationFrameCounter2 = gv.mod.currentArea.fullScreenAnimationFrameCounter2;
        gv.mod.changeCounter2 = gv.mod.currentArea.changeCounter2;
        gv.mod.changeFrameCounter2 = gv.mod.currentArea.changeFrameCounter2;
        gv.mod.fullScreenAnimationSpeedX2 = gv.mod.currentArea.fullScreenAnimationSpeedX2;
        gv.mod.fullScreenAnimationSpeedY2 = gv.mod.currentArea.fullScreenAnimationSpeedY2;
        gv.mod.fullScreenAnimationFrameCounterX2 = gv.mod.currentArea.fullScreenAnimationFrameCounterX2;
        gv.mod.fullScreenAnimationFrameCounterY2 = gv.mod.currentArea.fullScreenAnimationFrameCounterY2;
        
        gv.mod.overrideDelayCounter3 = gv.mod.currentArea.overrideDelayCounter3;
        gv.mod.cycleCounter3 = gv.mod.currentArea.cycleCounter3;
        gv.mod.fullScreenAnimationFrameCounter3 = gv.mod.currentArea.fullScreenAnimationFrameCounter3;
        gv.mod.changeCounter3 = gv.mod.currentArea.changeCounter3;
        gv.mod.changeFrameCounter3 = gv.mod.currentArea.changeFrameCounter3;
        gv.mod.fullScreenAnimationSpeedX3 = gv.mod.currentArea.fullScreenAnimationSpeedX3;
        gv.mod.fullScreenAnimationSpeedY3 = gv.mod.currentArea.fullScreenAnimationSpeedY3;
        gv.mod.fullScreenAnimationFrameCounterX3 = gv.mod.currentArea.fullScreenAnimationFrameCounterX3;
        gv.mod.fullScreenAnimationFrameCounterY3 = gv.mod.currentArea.fullScreenAnimationFrameCounterY3;

        gv.mod.overrideDelayCounter4 = gv.mod.currentArea.overrideDelayCounter4;
        gv.mod.cycleCounter4 = gv.mod.currentArea.cycleCounter4;
        gv.mod.fullScreenAnimationFrameCounter4 = gv.mod.currentArea.fullScreenAnimationFrameCounter4;
        gv.mod.changeCounter4 = gv.mod.currentArea.changeCounter4;
        gv.mod.changeFrameCounter4 = gv.mod.currentArea.changeFrameCounter4;
        gv.mod.fullScreenAnimationSpeedX4 = gv.mod.currentArea.fullScreenAnimationSpeedX4;
        gv.mod.fullScreenAnimationSpeedY4 = gv.mod.currentArea.fullScreenAnimationSpeedY4;
        gv.mod.fullScreenAnimationFrameCounterX4 = gv.mod.currentArea.fullScreenAnimationFrameCounterX4;
        gv.mod.fullScreenAnimationFrameCounterY4 = gv.mod.currentArea.fullScreenAnimationFrameCounterY4;

        gv.mod.overrideDelayCounter5 = gv.mod.currentArea.overrideDelayCounter5;
        gv.mod.cycleCounter5 = gv.mod.currentArea.cycleCounter5;
        gv.mod.fullScreenAnimationFrameCounter5 = gv.mod.currentArea.fullScreenAnimationFrameCounter5;
        gv.mod.changeCounter5 = gv.mod.currentArea.changeCounter5;
        gv.mod.changeFrameCounter5 = gv.mod.currentArea.changeFrameCounter5;
        gv.mod.fullScreenAnimationSpeedX5 = gv.mod.currentArea.fullScreenAnimationSpeedX5;
        gv.mod.fullScreenAnimationSpeedY5 = gv.mod.currentArea.fullScreenAnimationSpeedY5;
        gv.mod.fullScreenAnimationFrameCounterX5 = gv.mod.currentArea.fullScreenAnimationFrameCounterX5;
        gv.mod.fullScreenAnimationFrameCounterY5 = gv.mod.currentArea.fullScreenAnimationFrameCounterY5;

        gv.mod.overrideDelayCounter6 = gv.mod.currentArea.overrideDelayCounter6;
        gv.mod.cycleCounter6 = gv.mod.currentArea.cycleCounter6;
        gv.mod.fullScreenAnimationFrameCounter6 = gv.mod.currentArea.fullScreenAnimationFrameCounter6;
        gv.mod.changeCounter6 = gv.mod.currentArea.changeCounter6;
        gv.mod.changeFrameCounter6 = gv.mod.currentArea.changeFrameCounter6;
        gv.mod.fullScreenAnimationSpeedX6 = gv.mod.currentArea.fullScreenAnimationSpeedX6;
        gv.mod.fullScreenAnimationSpeedY6 = gv.mod.currentArea.fullScreenAnimationSpeedY6;
        gv.mod.fullScreenAnimationFrameCounterX6 = gv.mod.currentArea.fullScreenAnimationFrameCounterX6;
        gv.mod.fullScreenAnimationFrameCounterY6 = gv.mod.currentArea.fullScreenAnimationFrameCounterY6;

        gv.mod.overrideDelayCounter7 = gv.mod.currentArea.overrideDelayCounter7;
        gv.mod.cycleCounter7 = gv.mod.currentArea.cycleCounter7;
        gv.mod.fullScreenAnimationFrameCounter7 = gv.mod.currentArea.fullScreenAnimationFrameCounter7;
        gv.mod.changeCounter7 = gv.mod.currentArea.changeCounter7;
        gv.mod.changeFrameCounter7 = gv.mod.currentArea.changeFrameCounter7;
        gv.mod.fullScreenAnimationSpeedX7 = gv.mod.currentArea.fullScreenAnimationSpeedX7;
        gv.mod.fullScreenAnimationSpeedY7 = gv.mod.currentArea.fullScreenAnimationSpeedY7;
        gv.mod.fullScreenAnimationFrameCounterX7 = gv.mod.currentArea.fullScreenAnimationFrameCounterX7;
        gv.mod.fullScreenAnimationFrameCounterY7 = gv.mod.currentArea.fullScreenAnimationFrameCounterY7;

        gv.mod.overrideDelayCounter8 = gv.mod.currentArea.overrideDelayCounter8;
        gv.mod.cycleCounter8 = gv.mod.currentArea.cycleCounter8;
        gv.mod.fullScreenAnimationFrameCounter8 = gv.mod.currentArea.fullScreenAnimationFrameCounter8;
        gv.mod.changeCounter8 = gv.mod.currentArea.changeCounter8;
        gv.mod.changeFrameCounter8 = gv.mod.currentArea.changeFrameCounter8;
        gv.mod.fullScreenAnimationSpeedX8 = gv.mod.currentArea.fullScreenAnimationSpeedX8;
        gv.mod.fullScreenAnimationSpeedY8 = gv.mod.currentArea.fullScreenAnimationSpeedY8;
        gv.mod.fullScreenAnimationFrameCounterX8 = gv.mod.currentArea.fullScreenAnimationFrameCounterX8;
        gv.mod.fullScreenAnimationFrameCounterY8 = gv.mod.currentArea.fullScreenAnimationFrameCounterY8;

        gv.mod.overrideDelayCounter9 = gv.mod.currentArea.overrideDelayCounter9;
        gv.mod.cycleCounter9 = gv.mod.currentArea.cycleCounter9;
        gv.mod.fullScreenAnimationFrameCounter9 = gv.mod.currentArea.fullScreenAnimationFrameCounter9;
        gv.mod.changeCounter9 = gv.mod.currentArea.changeCounter9;
        gv.mod.changeFrameCounter9 = gv.mod.currentArea.changeFrameCounter9;
        gv.mod.fullScreenAnimationSpeedX9 = gv.mod.currentArea.fullScreenAnimationSpeedX9;
        gv.mod.fullScreenAnimationSpeedY9 = gv.mod.currentArea.fullScreenAnimationSpeedY9;
        gv.mod.fullScreenAnimationFrameCounterX9 = gv.mod.currentArea.fullScreenAnimationFrameCounterX9;
        gv.mod.fullScreenAnimationFrameCounterY9 = gv.mod.currentArea.fullScreenAnimationFrameCounterY9;

        gv.mod.overrideDelayCounter10 = gv.mod.currentArea.overrideDelayCounter10;
        gv.mod.cycleCounter10 = gv.mod.currentArea.cycleCounter10;
        gv.mod.fullScreenAnimationFrameCounter10 = gv.mod.currentArea.fullScreenAnimationFrameCounter10;
        gv.mod.changeCounter10 = gv.mod.currentArea.changeCounter10;
        gv.mod.changeFrameCounter10 = gv.mod.currentArea.changeFrameCounter10;
        gv.mod.fullScreenAnimationSpeedX10 = gv.mod.currentArea.fullScreenAnimationSpeedX10;
        gv.mod.fullScreenAnimationSpeedY10 = gv.mod.currentArea.fullScreenAnimationSpeedY10;
        gv.mod.fullScreenAnimationFrameCounterX10 = gv.mod.currentArea.fullScreenAnimationFrameCounterX10;
        gv.mod.fullScreenAnimationFrameCounterY10 = gv.mod.currentArea.fullScreenAnimationFrameCounterY10;

      
        }

        public void doItemScriptBasedOnUseItem(Player pc, ItemRefs itRef, bool destroyItemAfterUse)
        {
            Item it = gv.mod.getItemByResRefForInfo(itRef.resref);
            bool foundScript = false;
            if (it.onUseItem.Equals("itHealLight.cs"))
            {
                gv.sf.itHeal(pc, it, 8);
                foundScript = true;
            }
            else if (it.onUseItem.Equals("itHealMedium.cs"))
            {
                gv.sf.itHeal(pc, it, 16);
                foundScript = true;
            }
            else if (it.onUseItem.Equals("itRegenSPLight.cs"))
            {
                gv.sf.itSpHeal(pc, it, 20);
                foundScript = true;
            }
            else if (it.onUseItem.Equals("itForceRest.cs"))
            {
                gv.sf.itForceRest();
                foundScript = true;
            }
            if ((foundScript) && (destroyItemAfterUse))
            {
                gv.sf.RemoveItemFromInventory(itRef, 1);
            }
        }

        //TESTING STUFF
        public void createSpritesForTesting()
        {
            for (int i = 0; i < 100; i++)
            {
                Sprite spr = new Sprite(gv, "hit_symbol", gv.sf.RandInt(1000), gv.sf.RandInt(1000), (float)(gv.sf.RandInt(100) + 1) / 1000f, (float)(gv.sf.RandInt(100) + 1) / 1000f, 0, (float)(gv.sf.RandInt(100) + 1) / 10000f, 1.0f, gv.sf.RandInt(10000) + 3000, false, 100);
                gv.screenMainMap.spriteList.Add(spr);
            }            
        }

        public void createRain(string density)
        {
            if (gv.mod.isRaining == true)
            {
                float rainChance = 0;
                if (density == "lightRain")
                {
                    rainChance = gv.sf.RandInt(10) + 10;
                }
                else if (density == "heavyRain")
                {
                    rainChance = gv.sf.RandInt(40) + 25;
                }
                else if (density == "rain")
                {
                    rainChance = gv.sf.RandInt(20) + 15;
                }

                float storedIncrement = 0;
                for (int i = 1; i < 61; i++)
                {
                    float increment = gv.screenWidth / 60;
                    storedIncrement += increment;
                    if (gv.sf.RandInt(100) < rainChance)
                    {
                        Sprite spr = new Sprite(gv, "rainDrop", storedIncrement - (gv.squareSize/2), -(float)(gv.sf.RandInt(10)), (float)(gv.sf.RandInt(5) + 35) / 650f, (float)(gv.sf.RandInt(80) + 170) / 500f, 0, 0, 0.18f* 0.65f, 0.335f * 0.73f, gv.sf.RandInt(10000) + 6000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "rain", false, 0);
                        gv.screenMainMap.spriteList.Add(spr);
                    }
                }
            }
        }

        public void createSnow(string density)
        {
            if (gv.mod.isSnowing == true)
            {
                float snowChance = 0;
                if (density == "lightSnow")
                {
                    snowChance = gv.sf.RandInt(5) + 7;
                }
                else if (density == "heavySnow")
                {
                    snowChance = gv.sf.RandInt(30) + 15;
                }
                else if (density == "snow")
                {
                    snowChance = gv.sf.RandInt(12) + 7;
                }

                float storedIncrement = 0;
                for (int i = 1; i < 61; i++)
                {
                    float increment = gv.screenWidth / 60;
                    storedIncrement += increment;
                    if (gv.sf.RandInt(100) < snowChance)
                    {
                        int scaleMulti = gv.sf.RandInt(50) + 75;
                        Sprite spr = new Sprite(gv, "snowFlake", storedIncrement - (gv.squareSize / 2), -(float)(gv.sf.RandInt(10)), 0, (float)(gv.sf.RandInt(80) + 170) / 6000f, 0, 0, 0.425f * scaleMulti/100f, 0.425f * scaleMulti/100f, gv.sf.RandInt(10000) + 15000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "snow", false, 0);
                        gv.screenMainMap.spriteList.Add(spr);
                    }
                }
            }
        }

        public void createSandstorm(string density)
        {
            if (gv.mod.isSandstorm == true)
            {
                float sandstormChance = 0;
                if (density == "lightSandStorm")
                {
                    sandstormChance = gv.sf.RandInt(10) + 14;
                }
                else if (density == "heavySandStorm")
                {
                    sandstormChance = gv.sf.RandInt(50) + 35;
                }
                else if (density == "sandStorm")
                {
                    sandstormChance = gv.sf.RandInt(30) + 25;
                }

                float storedIncrement = 0;
                if (gv.mod.sandStormBlowingTo.Contains("NE"))
                {
                    for (int i = 1; i < 61; i++)
                    {
                        float increment = (gv.screenHeight) / 60;
                        storedIncrement += increment;
                        if (gv.sf.RandInt(100) < sandstormChance)
                        {
                            int scaleMulti = gv.sf.RandInt(50) + 75;
                            float thisParticleSpeed = (75f + gv.sf.RandInt(50)) / 100f;
                            //(float)(gv.sf.RandInt(80) + 170) / 600f
                            Sprite spr = new Sprite(gv, "sandGrain", -(float)(gv.sf.RandInt(10)), storedIncrement - (gv.squareSize / 2), thisParticleSpeed * gv.mod.sandStormDirectionX, thisParticleSpeed * gv.mod.sandStormDirectionY, 0, 0, 0.235f * scaleMulti / 100f, 0.235f * scaleMulti/100f, 4000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "sandstorm", false, 0);
                            gv.screenMainMap.spriteList.Add(spr);
                        }
                    }

                    storedIncrement = 0;

                    for (int i = 1; i < 61; i++)
                    {
                        float increment = (gv.screenWidth) / 60;
                        storedIncrement += increment;
                        if (gv.sf.RandInt(100) < sandstormChance)
                        {
                            int scaleMulti = gv.sf.RandInt(50) + 75;
                            float thisParticleSpeed = (75f + gv.sf.RandInt(50)) / 100f;
                            //(float)(gv.sf.RandInt(80) + 170) / 600f
                            Sprite spr = new Sprite(gv, "sandGrain", storedIncrement - (gv.squareSize / 2), gv.screenHeight + (float)(gv.sf.RandInt(10)), thisParticleSpeed * gv.mod.sandStormDirectionX, thisParticleSpeed * gv.mod.sandStormDirectionY, 0, 0, 0.235f * scaleMulti / 100f, 0.235f * scaleMulti/100f, 4000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "sandstorm", false, 0);
                            gv.screenMainMap.spriteList.Add(spr);
                        }
                    }
                }

                if (gv.mod.sandStormBlowingTo.Contains("NW"))
                {
                    for (int i = 1; i < 61; i++)
                    {
                        float increment = (gv.screenHeight) / 60;
                        storedIncrement += increment;
                        if (gv.sf.RandInt(100) < sandstormChance)
                        {
                            int scaleMulti = gv.sf.RandInt(50) + 75;
                            float thisParticleSpeed = (75f + gv.sf.RandInt(50)) / 100f;
                            //(float)(gv.sf.RandInt(80) + 170) / 600f
                            Sprite spr = new Sprite(gv, "sandGrain", gv.screenWidth + (float)(gv.sf.RandInt(10)), storedIncrement - (gv.squareSize / 2), thisParticleSpeed * gv.mod.sandStormDirectionX, thisParticleSpeed * gv.mod.sandStormDirectionY, 0, 0, 0.235f * scaleMulti / 100f, 0.235f * scaleMulti/100f, 4000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "sandstorm", false, 0);
                            gv.screenMainMap.spriteList.Add(spr);
                        }
                    }

                    storedIncrement = 0;

                    for (int i = 1; i < 61; i++)
                    {
                        float increment = (gv.screenWidth) / 60;
                        storedIncrement += increment;
                        if (gv.sf.RandInt(100) < sandstormChance)
                        {
                            int scaleMulti = gv.sf.RandInt(50) + 75;
                            float thisParticleSpeed = (75f + gv.sf.RandInt(50)) / 100f;
                            //(float)(gv.sf.RandInt(80) + 170) / 600f
                            Sprite spr = new Sprite(gv, "sandGrain", storedIncrement - (gv.squareSize / 2), gv.screenHeight + (float)(gv.sf.RandInt(10)), thisParticleSpeed * gv.mod.sandStormDirectionX, thisParticleSpeed * gv.mod.sandStormDirectionY, 0, 0, 0.235f * scaleMulti / 100f, 0.235f * scaleMulti/100f, 4000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "sandstorm", false, 0);
                            gv.screenMainMap.spriteList.Add(spr);
                        }
                    }
                }


                if (gv.mod.sandStormBlowingTo.Contains("SW"))
                {
                    for (int i = 1; i < 61; i++)
                    {
                        float increment = (gv.screenHeight) / 60;
                        storedIncrement += increment;
                        if (gv.sf.RandInt(100) < sandstormChance)
                        {
                            int scaleMulti = gv.sf.RandInt(50) + 75;
                            float thisParticleSpeed = (75f + gv.sf.RandInt(50)) / 100f;
                            //(float)(gv.sf.RandInt(80) + 170) / 600f
                            Sprite spr = new Sprite(gv, "sandGrain", gv.screenWidth + (float)(gv.sf.RandInt(10)), storedIncrement - (gv.squareSize / 2), thisParticleSpeed * gv.mod.sandStormDirectionX, thisParticleSpeed * gv.mod.sandStormDirectionY, 0, 0, 0.235f * scaleMulti / 100f, 0.235f * scaleMulti/100f, 4000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "sandstorm", false, 0);
                            gv.screenMainMap.spriteList.Add(spr);
                        }
                    }

                    storedIncrement = 0;

                    for (int i = 1; i < 61; i++)
                    {
                        float increment = (gv.screenWidth) / 60;
                        storedIncrement += increment;
                        if (gv.sf.RandInt(100) < sandstormChance)
                        {
                            int scaleMulti = gv.sf.RandInt(50) + 75;
                            float thisParticleSpeed = (75f + gv.sf.RandInt(50)) / 100f;
                            //(float)(gv.sf.RandInt(80) + 170) / 600f
                            Sprite spr = new Sprite(gv, "sandGrain", storedIncrement - (gv.squareSize / 2), -(float)(gv.sf.RandInt(10)), thisParticleSpeed * gv.mod.sandStormDirectionX, thisParticleSpeed * gv.mod.sandStormDirectionY, 0, 0, 0.235f * scaleMulti / 100f, 0.235f * scaleMulti/100f, 4000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "sandstorm", false, 0);
                            gv.screenMainMap.spriteList.Add(spr);
                        }
                    }
                }

                if (gv.mod.sandStormBlowingTo.Contains("SE"))
                {
                    for (int i = 1; i < 61; i++)
                    {
                        float increment = (gv.screenHeight) / 60;
                        storedIncrement += increment;
                        if (gv.sf.RandInt(100) < sandstormChance)
                        {
                            int scaleMulti = gv.sf.RandInt(50) + 75;
                            float thisParticleSpeed = (75f + gv.sf.RandInt(50)) / 100f;
                            //(float)(gv.sf.RandInt(80) + 170) / 600f
                            Sprite spr = new Sprite(gv, "sandGrain", -(float)(gv.sf.RandInt(10)), storedIncrement - (gv.squareSize / 2), thisParticleSpeed * gv.mod.sandStormDirectionX, thisParticleSpeed * gv.mod.sandStormDirectionY, 0, 0, 0.235f * scaleMulti / 100f, 0.235f * scaleMulti/100f, 4000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "sandstorm", false, 0);
                            gv.screenMainMap.spriteList.Add(spr);
                        }
                    }

                    storedIncrement = 0;

                    for (int i = 1; i < 61; i++)
                    {
                        float increment = (gv.screenWidth) / 60;
                        storedIncrement += increment;
                        if (gv.sf.RandInt(100) < sandstormChance)
                        {
                            int scaleMulti = gv.sf.RandInt(50) + 75;
                            float thisParticleSpeed = (75f + gv.sf.RandInt(50)) / 100f;
                            //(float)(gv.sf.RandInt(80) + 170) / 600f
                            Sprite spr = new Sprite(gv, "sandGrain", storedIncrement - (gv.squareSize / 2), -(float)(gv.sf.RandInt(10)), thisParticleSpeed * gv.mod.sandStormDirectionX, thisParticleSpeed * gv.mod.sandStormDirectionY, 0, 0, 0.235f * scaleMulti / 100f, 0.235f * scaleMulti/100f, 4000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "sandstorm", false, 0);
                            gv.screenMainMap.spriteList.Add(spr);
                        }
                    }
                }



            }
        }

        public void createClouds(string cloudType, float speedMultiplier, float positionModifierX, float positionModifierY)
        {
                float veloX = 0;
                float veloY = 0;

            if (gv.mod.windDirection.Contains("North"))
                {
                    veloX = 0;
                    veloY = -1f / 50f;
                }
                if (gv.mod.windDirection.Contains("South"))
                {
                    veloX = 0;
                    veloY = 1f / 50f;
                }
            if (gv.mod.windDirection.Contains("East"))
            {
                veloX = 1f / 50f;
                veloY = 0;
            }
            if (gv.mod.windDirection.Contains("West"))
            {
                veloX = -1f / 50f;
                veloY = 0;
            }
            if (gv.mod.windDirection.Contains("NE"))
            {
                veloX = 1f / 50f;
                veloY = -1f / 50f;
            }
            if (gv.mod.windDirection.Contains("SW"))
            {
                veloX = -1f / 50f;
                veloY = 1f / 50f;
            }
            if (gv.mod.windDirection.Contains("SE"))
            {
                veloX = 1f / 50f;
                veloY = 1f / 50f; 
            }
            if (gv.mod.windDirection.Contains("NW"))
            {
                veloX = -1f / 50f;
                veloY = -1f / 50f;
            }

            Sprite spr = new Sprite(gv, cloudType, gv.screenWidth/2 - gv.screenHeight/2 + positionModifierX, gv.screenHeight/2 - gv.screenHeight/2 + positionModifierY, veloX * speedMultiplier, veloY * speedMultiplier, 0, 0, gv.sf.RandInt(60)/10f + 7.5f, gv.sf.RandInt(60)/10f + 7.5f, gv.sf.RandInt(80000) + 48000, false, 100,gv.mod.fullScreenEffectOpacityWeather,0,"clouds",true,0);
            gv.screenMainMap.spriteList.Add(spr);   
        }

        public void createFog(string fogType, float speedMultiplier, float positionModifierX, float positionModifierY)
        {
            float veloX = 0.012f + gv.sf.RandInt(20)/40000f;
            float veloY = 0.012f + gv.sf.RandInt(20)/40000f;

            int decider = gv.sf.RandInt(2);
            if (decider == 1)
            {
                veloX = veloX * -1;
            }

            decider = gv.sf.RandInt(2);
            if (decider == 1)
            {
                veloY = veloY * -1;
            }

            Sprite spr = new Sprite(gv, fogType, gv.screenWidth / 2 - gv.screenHeight / 2 + positionModifierX, gv.screenHeight / 2 - gv.screenHeight / 2 + positionModifierY, veloX * speedMultiplier * 0.7f, veloY * speedMultiplier * 0.7f, 0, 0, 14.5f,14.5f, gv.sf.RandInt(80000) + 48000, false, 100, gv.mod.fullScreenEffectOpacityWeather, 0, "fog", false, 0);
            gv.screenMainMap.spriteList.Add(spr);
        }

        public void createLightning(string lightningType, float posX, float posY, float scaleMod)
        {
            Sprite spr = new Sprite(gv, lightningType, posX, posY, 0, 0, 0, 0, 5.0f * scaleMod, 5.0f * scaleMod, 4000, false, 45, 1.0f, 0, "lightning", false, 8);
            gv.screenMainMap.spriteList.Add(spr);

            if ((gv.mod.useWeatherSound) && (gv.mod.playMusic))
            {
                gv.weatherSounds3.settings.volume = (int)(50 * weatherSoundMultiplier);
                gv.mod.weatherSoundsName3 = "lightning";
                string soundName = gv.mod.weatherSoundsName3;

                if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName))
                {
                    gv.weatherSounds3.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName;
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3"))
                {
                    gv.weatherSounds3.URL = gv.mainDirectory + "\\modules\\" + gv.mod.moduleName + "\\music\\" + soundName + ".mp3";
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName))
                {
                    gv.weatherSounds3.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName;
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3"))
                {
                    gv.weatherSounds3.URL = gv.mainDirectory + "\\default\\NewModule\\music\\" + soundName + ".mp3";
                }
                else
                {
                    gv.weatherSounds3.URL = "";
                }
                if ((gv.weatherSounds3.URL != "") && (gv.weatherSounds3 != null))
                {
                    gv.weatherSounds3.controls.stop();
                    gv.weatherSounds3.controls.play();
                }

            }
            }

        //MISC FUNCTIONS
        public int getDistance(Coordinate start, Coordinate end)
        {
            int dist = 0;
            int deltaX = (int)Math.Abs((start.X - end.X));
            int deltaY = (int)Math.Abs((start.Y - end.Y));
            if (deltaX > deltaY)
                dist = deltaX;
            else
                dist = deltaY;
            return dist;
        }
        public System.Drawing.Bitmap LoadBitmapGDI(string filename) //change this to LoadBitmapGDI
        {
            System.Drawing.Bitmap bm = null;
            bm = LoadBitmapGDI(filename, gv.mod); //change this to LoadBitmapGDI
            return bm;
        }
        public System.Drawing.Bitmap LoadBitmapGDI(string filename, Module mdl) //change this to LoadBitmapGDI
        {
            System.Drawing.Bitmap bm = null;

            try
            {
                //hurghy
                if ((mdl.currentArea.sourceBitmapName != "") && (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".png")))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".png");
                }
                else if ((mdl.currentArea.sourceBitmapName != "") && (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".PNG")))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".PNG");
                }
                else if ((mdl.currentArea.sourceBitmapName != "") && (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".jpg")))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + mdl.currentArea.sourceBitmapName + "\\" + filename + ".jpg");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\tiles\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".jpg"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename + ".jpg");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\graphics\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\ui\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\pctokens\\" + filename);
                }                
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".PNG"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename + ".PNG");
                }
                else if (File.Exists(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\modules\\" + mdl.moduleName + "\\portraits\\" + filename);
                }
                //NewModule folders
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".jpg"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename + ".jpg");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\ui\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\pctokens\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\pctokens\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\pctokens\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\pctokens\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\tiles\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename + ".png"))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename + ".png");
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename))
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\portraits\\" + filename);
                }

                else
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\missingtexture.png");
                }		
            }
            catch (Exception ex)
            {
                if (bm == null)
                {
                    bm = new System.Drawing.Bitmap(gv.mainDirectory + "\\default\\NewModule\\graphics\\missingtexture.png");
                    return bm;
                }
                gv.errorLog(ex.ToString());
            }

            return bm;
        }

        public string loadTextToString(string filename)
        {
            string txt = null;
            try
            {
                if (File.Exists(GetModulePath() + "\\data\\" + filename))
                {
                    txt = File.ReadAllText(GetModulePath() + "\\data\\" + filename);
                }
                else if (File.Exists(gv.mainDirectory + "\\default\\NewModule\\data\\" + filename))
                {
                    txt = File.ReadAllText(gv.mainDirectory + "\\default\\NewModule\\data\\" + filename);
                }
            }
            catch (Exception ex)
            {
                gv.errorLog(ex.ToString());
                return null;
            }
            return txt;
        }
        public float MeasureString(string text, SharpDX.DirectWrite.FontWeight fw, SharpDX.DirectWrite.FontStyle fs, float fontHeight)
        {
            // Measure string width.
            SharpDX.DirectWrite.TextFormat tf = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, fw, fs, SharpDX.DirectWrite.FontStretch.Normal, fontHeight) { TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Near };
            SharpDX.DirectWrite.TextLayout tl = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, text, tf, gv.Width, gv.Height);
            float returnWidth = tl.Metrics.Width;
            if (tf != null)
            {
                tf.Dispose();
                tf = null;
            }
            if (tl != null)
            {
                tl.Dispose();
                tl = null;
            }
            return returnWidth;
        }
        public CoordinateF MeasureStringSize(string text, SharpDX.DirectWrite.FontWeight fw, SharpDX.DirectWrite.FontStyle fs, float fontHeight)
        {
            // Measure string width.
            SharpDX.DirectWrite.TextFormat tf = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, fw, fs, SharpDX.DirectWrite.FontStretch.Normal, fontHeight) { TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Near };
            SharpDX.DirectWrite.TextLayout tl = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, text, tf, gv.Width, gv.Height);
            CoordinateF returnSize = new CoordinateF(tl.Metrics.Width, tl.Metrics.Height);
            if (tf != null)
            {
                tf.Dispose();
                tf = null;
            }
            if (tl != null)
            {
                tl.Dispose();
                tl = null;
            }
            return returnSize;
        }
        public void MakeDirectoryIfDoesntExist(string filenameAndFullPath)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filenameAndFullPath);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
        }
        /*public System.Drawing.Bitmap flip(System.Drawing.Bitmap src)
        {
            src.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return src;
        }*/
        /*public System.Drawing.Bitmap FlipHorz(System.Drawing.Bitmap src)
        {
            src.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return src;
        }*/

        //DIRECT2D STUFF
        public SharpDX.Direct2D1.Bitmap GetFromBitmapList(string fileNameWithOutExt)
        {
            //check to see if in list already and return bitmap it if found
            if (commonBitmapList.ContainsKey(fileNameWithOutExt))
            {
                return commonBitmapList[fileNameWithOutExt];
            }
            //try loading and adding to list and return bitmap
            else
            {
                commonBitmapList.Add(fileNameWithOutExt, LoadBitmap(fileNameWithOutExt));
                return commonBitmapList[fileNameWithOutExt];
            }
        }
        public SharpDX.Direct2D1.Bitmap GetFromTileBitmapList(string fileNameWithOutExt)
        {
            //check to see if in list already and return bitmap it if found
            if (tileBitmapList.ContainsKey(fileNameWithOutExt))
            {
                return tileBitmapList[fileNameWithOutExt];
            }
            //try loading and adding to list and return bitmap
            else
            {
                tileBitmapList.Add(fileNameWithOutExt, LoadBitmap(fileNameWithOutExt));
                return tileBitmapList[fileNameWithOutExt];
            }
        }
        public System.Drawing.Bitmap GetFromTileGDIBitmapList(string fileNameWithOutExt)
        {
            //check to see if in list already and return bitmap it if found
            if (tileGDIBitmapList.ContainsKey(fileNameWithOutExt))
            {
                return tileGDIBitmapList[fileNameWithOutExt];
            }
            //try loading and adding to list and return bitmap
            else
            {
                tileGDIBitmapList.Add(fileNameWithOutExt, LoadBitmapGDI(fileNameWithOutExt));
                return tileGDIBitmapList[fileNameWithOutExt];
            }
        }
        public void DisposeOfBitmap(ref SharpDX.Direct2D1.Bitmap bmp)
        {
            if (bmp != null)
            {
                bmp.Dispose();
                bmp = null;
            }
        }
        public SharpDX.Direct2D1.Bitmap LoadBitmap(string file, Module mdl) //change this to LoadBitmap
        {
            // Loads from file using System.Drawing.Image
            using (var bitmap = LoadBitmapGDI(file, mdl)) //change this to LoadBitmapGDI
            {
                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                var size = new Size2(bitmap.Width, bitmap.Height);

                // Transform pixels from BGRA to RGBA
                int stride = bitmap.Width * sizeof(int);
                using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // Not optimized 
                            byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }
                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;
                    return new SharpDX.Direct2D1.Bitmap(gv.renderTarget2D, size, tempStream, stride, bitmapProperties);
                }
            }
        }
        public SharpDX.Direct2D1.Bitmap LoadBitmap(string file) //change this to LoadBitmap
        {
            // Loads from file using System.Drawing.Image
            using (var bitmap = LoadBitmapGDI(file)) //change this to LoadBitmapGDI
            {
                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                var size = new Size2(bitmap.Width, bitmap.Height);

                // Transform pixels from BGRA to RGBA
                int stride = bitmap.Width * sizeof(int);
                using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // Not optimized 
                            byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }
                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;
                    return new SharpDX.Direct2D1.Bitmap(gv.renderTarget2D, size, tempStream, stride, bitmapProperties);
                }
            }
        }
        public SharpDX.Direct2D1.Bitmap ConvertGDIBitmapToD2D(System.Drawing.Bitmap gdibitmap)
        {
            var sourceArea = new System.Drawing.Rectangle(0, 0, gdibitmap.Width, gdibitmap.Height);
            var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
            var size = new Size2(gdibitmap.Width, gdibitmap.Height);

            // Transform pixels from BGRA to RGBA
            int stride = gdibitmap.Width * sizeof(int);
            using (var tempStream = new DataStream(gdibitmap.Height * stride, true, true))
            {
                // Lock System.Drawing.Bitmap
                var bitmapData = gdibitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                // Convert all pixels 
                for (int y = 0; y < gdibitmap.Height; y++)
                {
                    int offset = bitmapData.Stride * y;
                    for (int x = 0; x < gdibitmap.Width; x++)
                    {
                        // Not optimized 
                        byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        int rgba = R | (G << 8) | (B << 16) | (A << 24);
                        tempStream.Write(rgba);
                    }
                }
                gdibitmap.UnlockBits(bitmapData);
                tempStream.Position = 0;
                return new SharpDX.Direct2D1.Bitmap(gv.renderTarget2D, size, tempStream, stride, bitmapProperties);
            }
        }
        public List<FormattedLine> ProcessHtmlString(string text, int width, List<string> tagStack)
        {
            bool tagMode = false;
            string tag = "";
            FormattedWord newWord = new FormattedWord();
            FormattedLine newLine = new FormattedLine();
            List<FormattedLine> logLinesList = new List<FormattedLine>();
            int lineHeight = 0;
            float xLoc = 0;

            char previousChar = ' ';
            char nextChar = ' ';
            int charIndex = -1;
            foreach (char c in text)
            {
                charIndex++;

                //get the previous char and the next char, used to get ' < ' and ' >= '
                if (charIndex > 0)
                {
                    previousChar = text[charIndex - 1];
                }
                if (charIndex < text.Length - 1)
                {
                    nextChar = text[charIndex + 1];
                }
                string combinedChars = previousChar.ToString() + c.ToString() + nextChar.ToString();

                #region Start/Stop Tags
                //start a tag and check for end of word
                if ((c == '<') && (!combinedChars.Contains("<=")) && (!combinedChars.Equals(" < ")))
                {
                    tagMode = true;

                    if (newWord.text != "")
                    {
                        newWord.fontStyle = GetFontStyle(tagStack);
                        newWord.fontWeight = GetFontWeight(tagStack);
                        newWord.underlined = GetIsUnderlined(tagStack);
                        newWord.fontSize = GetFontSizeInPixels(tagStack);
                        newWord.color = GetColor(tagStack);
                        if (gv.textFormat != null)
                        {
                            gv.textFormat.Dispose();
                            gv.textFormat = null;
                        }

                        if (gv.textLayout != null)
                        {
                            gv.textLayout.Dispose();
                            gv.textLayout = null;
                        }
                        gv.textFormat = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, newWord.fontWeight, newWord.fontStyle, SharpDX.DirectWrite.FontStretch.Normal, newWord.fontSize) { TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Near };
                        gv.textLayout = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, newWord.text + " ", gv.textFormat, gv.Width, gv.Height);
                        //font = new Font(gv.family, newWord.fontSize, newWord.fontStyle);
                        float height = gv.textLayout.Metrics.Height;
                        float wordWidth = gv.textLayout.Metrics.WidthIncludingTrailingWhitespace * 1.2f;
                        if (height > lineHeight) { lineHeight = (int)height; }
                        //int wordWidth = (int)(frm.gCanvas.MeasureString(newWord.word, font)).Width;
                        float modifiedWidth = 0;
                        if (gv.mod.useMinimalisticUI)
                        {
                            modifiedWidth = width + gv.pS;
                        }
                        else
                        {
                            modifiedWidth = width;
                        }
                        if (xLoc + wordWidth > modifiedWidth) //word wrap
                        {
                            //end last line and add it to the log
                            newLine.lineHeight = lineHeight;
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new FormattedLine();
                            newLine.wordsList.Add(newWord);
                            gv.mod.logFadeCounter = 120;
                            gv.mod.logOpacity = 1f;
                            xLoc = 0;
                        }
                        else //no word wrap, just add word
                        {
                            newLine.wordsList.Add(newWord);
                            gv.mod.logFadeCounter = 120;
                            gv.mod.logOpacity = 1f;
                        }
                        //instead of drawing, just add to line list 
                        //DrawString(g, word, font, brush, xLoc, yLoc);
                        xLoc += wordWidth;
                        newWord = new FormattedWord();
                    }
                    continue;
                }
                //end a tag
                else if ((c == '>') && (!combinedChars.Equals(" > ")) && (!combinedChars.Contains(">=")))
                {
                    //check for ending type tag
                    if (tag.StartsWith("/"))
                    {
                        //if </>, remove corresponding tag from stack
                        string tagMinusSlash = tag.Substring(1);
                        if (tag.StartsWith("/font"))
                        {
                            for (int i = tagStack.Count - 1; i > 0; i--)
                            {
                                if (tagStack[i].StartsWith("font"))
                                {
                                    tagStack.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            tagStack.Remove(tagMinusSlash);
                        }
                    }
                    else
                    {
                        //check for line break
                        if ((tag.ToLower() == "br") || (tag == "BR"))
                        {
                            newWord.fontStyle = GetFontStyle(tagStack);
                            newWord.fontWeight = GetFontWeight(tagStack);
                            newWord.underlined = GetIsUnderlined(tagStack);
                            newWord.fontSize = GetFontSizeInPixels(tagStack);
                            newWord.color = GetColor(tagStack);
                            //end last line and add it to the log
                            newLine.lineHeight = lineHeight;
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new FormattedLine();
                            //newLine.wordsList.Add(newWord);
                            xLoc = 0;
                        }
                        //else if <>, add this tag to the stack
                        tagStack.Add(tag);
                    }
                    tagMode = false;
                    tag = "";
                    continue;
                }
                #endregion

                #region Words
                if (!tagMode)
                {
                    if (c != ' ') //keep adding to word until hit a space
                    {
                        newWord.text += c;
                    }
                    else //hit a space so end word
                    {
                        newWord.fontStyle = GetFontStyle(tagStack);
                        newWord.fontWeight = GetFontWeight(tagStack);
                        newWord.underlined = GetIsUnderlined(tagStack);
                        newWord.fontSize = GetFontSizeInPixels(tagStack);
                        newWord.color = GetColor(tagStack);
                        if (gv.textFormat != null)
                        {
                            gv.textFormat.Dispose();
                            gv.textFormat = null;
                        }

                        if (gv.textLayout != null)
                        {
                            gv.textLayout.Dispose();
                            gv.textLayout = null;
                        }
                        gv.textFormat = new SharpDX.DirectWrite.TextFormat(gv.factoryDWrite, gv.family.Name, gv.CurrentFontCollection, newWord.fontWeight, newWord.fontStyle, SharpDX.DirectWrite.FontStretch.Normal, newWord.fontSize) { TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Near };
                        gv.textLayout = new SharpDX.DirectWrite.TextLayout(gv.factoryDWrite, newWord.text + " ", gv.textFormat, gv.Width, gv.Height);
                        //font = new Font(gv.family, newWord.fontSize, newWord.fontStyle);
                        //float adjustedWordDimensionsForScreenDensity = (100f / gv.squareSize);
                        float adjustedWordWidthForScreenDensity = (1920f / gv.Width);
                        float adjustedWordHeightForScreenDensity = (1080f / gv.Height);

                        float wordWidth = (gv.textLayout.Metrics.WidthIncludingTrailingWhitespace) * adjustedWordWidthForScreenDensity;
                        float height = gv.textLayout.Metrics.Height * adjustedWordHeightForScreenDensity;
                        if (height > lineHeight) { lineHeight = (int)height; }

                        if (xLoc + wordWidth > width) //word wrap
                        {
                            //end last line and add it to the log
                            newLine.lineHeight = lineHeight;
                            logLinesList.Add(newLine);
                            //start a new line and add this word
                            newLine = new FormattedLine();
                            newLine.wordsList.Add(newWord);
                            gv.mod.logFadeCounter = 120;
                            gv.mod.logOpacity = 1f;
                            xLoc = 0;
                        }
                        else //no word wrap, just add word
                        {
                            newLine.wordsList.Add(newWord);
                            gv.mod.logFadeCounter = 120;
                            gv.mod.logOpacity = 1f;
                        }
                        //instead of drawing, just add to line list 
                        //DrawString(g, word, font, brush, xLoc, yLoc);
                        xLoc += wordWidth;
                        newWord = new FormattedWord();
                    }
                }
                else if (tagMode)
                {
                    tag += c;
                }
                #endregion
            }
            tagStack.Clear();
            return logLinesList;
        }
        private SharpDX.Color GetColor(List<string> tagStack)
        {
            //will end up using the last color on the stack
            SharpDX.Color clr = SharpDX.Color.White;
            foreach (string s in tagStack)
            {
                if ((s == "font color='red'") || (s == "font color = 'red'"))
                {
                    clr = SharpDX.Color.Red;
                }
                else if ((s == "font color='lime'") || (s == "font color = 'lime'"))
                {
                    clr = SharpDX.Color.Lime;
                }
                else if ((s == "font color='black'") || (s == "font color = 'black'"))
                {
                    clr = SharpDX.Color.Black;
                }
                else if ((s == "font color='white'") || (s == "font color = 'white'"))
                {
                    clr = SharpDX.Color.White;
                }
                else if ((s == "font color='silver'") || (s == "font color = 'silver'"))
                {
                    clr = SharpDX.Color.Gray;
                }
                else if ((s == "font color='grey'") || (s == "font color = 'grey'"))
                {
                    clr = SharpDX.Color.DimGray;
                }
                else if ((s == "font color='aqua'") || (s == "font color = 'aqua'"))
                {
                    clr = SharpDX.Color.Aqua;
                }
                else if ((s == "font color='fuchsia'") || (s == "font color = 'fuchsia'"))
                {
                    clr = SharpDX.Color.Fuchsia;
                }
                else if ((s == "font color='yellow'") || (s == "font color = 'yellow'"))
                {
                    clr = SharpDX.Color.Yellow;
                }
            }
            return clr;
        }
        private SharpDX.DirectWrite.FontStyle GetFontStyle(List<string> tagStack)
        {
            SharpDX.DirectWrite.FontStyle style = SharpDX.DirectWrite.FontStyle.Normal;
            foreach (string s in tagStack)
            {
                if (s == "i")
                {
                    style = style | SharpDX.DirectWrite.FontStyle.Italic;
                }
            }
            return style;
        }
        private SharpDX.DirectWrite.FontWeight GetFontWeight(List<string> tagStack)
        {
            SharpDX.DirectWrite.FontWeight style = SharpDX.DirectWrite.FontWeight.Normal;
            foreach (string s in tagStack)
            {
                if (s == "b")
                {
                    style = style | SharpDX.DirectWrite.FontWeight.Bold;
                }
            }
            return style;
        }
        private bool GetIsUnderlined(List<string> tagStack)
        {
            bool isUnderlined = false;
            foreach (string s in tagStack)
            {
                if (s == "u")
                {
                    isUnderlined = true; ;
                }
            }
            return isUnderlined;
        }
        private float GetFontSizeInPixels(List<string> tagStack)
        {
            float fSize = gv.drawFontRegHeight * (float)gv.squareSize / 100.0f;
            foreach (string s in tagStack)
            {
                if (s == "big")
                {
                    fSize = gv.drawFontLargeHeight * (float)gv.squareSize / 100.0f;
                }
                else if (s == "small")
                {
                    fSize = gv.drawFontSmallHeight * (float)gv.squareSize / 100.0f;
                }
            }
            return fSize;
        }
    }
}
