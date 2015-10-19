﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Media;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Direct3D;
using SharpDX;
using FontFamily = System.Drawing.FontFamily;
using Font = System.Drawing.Font;
//using Bitmap = System.Drawing.Bitmap;
using Message = System.Windows.Forms.Message;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using RectangleF = System.Drawing.RectangleF;
using Rectangle = System.Drawing.Rectangle;


namespace IceBlink2
{
    public partial class GameView : IBForm
    {
        //this class is handled differently than Android version
        public float screenDensity;
        public int screenWidth;
        public int screenHeight;
        public int squareSizeInPixels = 100;
        public int squareSize; //in dp (squareSizeInPixels * screenDensity)
        public int squaresInWidth = 19;
        public int squaresInHeight = 10;
        public int ibbwidthL = 340;
        public int ibbwidthR = 100;
        public int ibbheight = 100;
        public int ibpwidth = 110;
        public int ibpheight = 170;
        public int playerOffset = 4;
        public int oXshift = 0;
        public int oYshift = 35;
        public string mainDirectory;
        public bool showHotKeys = false;

        public Graphics gCanvas;

        //DIRECT2D STUFF
        public SharpDX.Direct3D11.Device _device;
        public SwapChain _swapChain;
        public Texture2D _backBuffer;
        public RenderTargetView _backBufferView;
        public SharpDX.Direct2D1.Factory factory2D;
        public SharpDX.DirectWrite.Factory factoryDWrite;
        public RenderTarget renderTarget2D;
        public SolidColorBrush sceneColorBrush;
        public ResourceFontLoader CurrentResourceFontLoader;
        public SharpDX.DirectWrite.FontCollection CurrentFontCollection;
        public string FontFamilyName;
        public TextFormat textFormat;
        public TextLayout textLayout;

        public string versionNum = "v1.00";
        public string fixedModule = "";
        public PrivateFontCollection myFonts; //CREATE A FONT COLLECTION
        public FontFamily family;
        public Font drawFontReg;
        public Font drawFontLarge;
        public Font drawFontSmall;
        public float drawFontRegHeight;
        public float drawFontLargeHeight;
        public float drawFontSmallHeight;
        public SolidBrush drawBrush = new SolidBrush(Color.White);
        //public Paint mUiTextPaint = null;
        //public Paint mSheetTextPaint = null;
        //public Paint floatyTextPaint = null;
        //public Context gameContext;
        //public Paint uiFontPaint = null;
        //public Typeface uiFont;
        public string screenType = "splash"; //launcher, title, moreGames, main, party, inventory, combatInventory, shop, journal, combat, combatCast, convo
        public AnimationState animationState = AnimationState.None;
        public int triggerIndex = 0;
        public int triggerPropIndex = 0;

        public IbbHtmlLogBox log;
        public CommonCode cc;
        public Module mod;
        public ScriptFunctions sf;
        public PathFinderAreas pfa;
        public ScreenParty screenParty;
        public ScreenInventory screenInventory;
        public ScreenItemSelector screenItemSelector;
        public ScreenPortraitSelector screenPortraitSelector;
        public ScreenTokenSelector screenTokenSelector;
        public ScreenPcSelector screenPcSelector;
        public ScreenJournal screenJournal;
        public ScreenShop screenShop;
        public ScreenCastSelector screenCastSelector;
        public ScreenConvo screenConvo;
        public ScreenTitle screenTitle;
        public ScreenPcCreation screenPcCreation;
        public ScreenSpellLevelUp screenSpellLevelUp;
        public ScreenTraitLevelUp screenTraitLevelUp;
        public ScreenLauncher screenLauncher;
        public ScreenCombat screenCombat;
        public ScreenMainMap screenMainMap;
        public ScreenPartyBuild screenPartyBuild;
        public ScreenPartyRoster screenPartyRoster;
        public bool touchEnabled = true;
        public WMPLib.WindowsMediaPlayer areaMusic;
        public WMPLib.WindowsMediaPlayer areaSounds;
        public SoundPlayer soundPlayer = new SoundPlayer();
        public Dictionary<string, Stream> oSoundStreams = new Dictionary<string, Stream>();
        public System.Media.SoundPlayer playerButtonEnter = new System.Media.SoundPlayer();
        public System.Media.SoundPlayer playerButtonClick = new System.Media.SoundPlayer();
       
        //public SoundPool sounds;
        //public Map<String, Integer> soundsList = new HashMap<String, Integer>();
        public string currentMainMusic = "";
        public string currentAmbientMusic = "";
        public string currentCombatMusic = "";

        //timers
        public Timer animationTimer = new Timer();
        public Timer floatyTextTimer = new Timer();
        public Timer floatyTextMainMapTimer = new Timer();
        public Timer areaMusicTimer = new Timer();
        public Timer areaSoundsTimer = new Timer();
        public Timer realTimeTimer = new Timer();
        public Timer smoothMoveTimer = new Timer();

        public float floatPixMovedPerTick = 4f;
        //the int for pixMovedPErTick is not used anymore
        //public int pixMovedPerTick = 4;
        public int realTimeTimerLengthInMilliSeconds = 1500;
        public int smoothMoveTimerLengthInMilliSeconds = 16;
        public int smoothMoveCounter = 0;
        //public bool useSmoothMovement = true;
        //public bool useRealTimeTimer = false; 

        //public bool logUpdated = false;
        //public int drawCount = 0;
        //public int mouseCount = 0;
                
        public GameView()
        {
            InitializeComponent();

            cc = new CommonCode(this);
            mod = new Module();

            
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GameView_MouseWheel);
            mainDirectory = Directory.GetCurrentDirectory();

            this.IceBlinkButtonClose.setupAll(this);
            this.IceBlinkButtonResize.setupAll(this);
            try
            {
                playerButtonClick.SoundLocation = mainDirectory + "\\default\\NewModule\\sounds\\btn_click.wav";
                playerButtonClick.Load();
            }
            catch (Exception ex) { errorLog(ex.ToString()); } 
            try
            {
                playerButtonEnter.SoundLocation = mainDirectory + "\\default\\NewModule\\sounds\\btn_hover.wav";
                playerButtonEnter.Load();
            }
            catch (Exception ex) { errorLog(ex.ToString()); }

            //this is the standard way, comment out the next 3 lines if manually forcing a screen resolution for testing UI layouts
            this.WindowState = FormWindowState.Maximized;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            
            //for testing other screen sizes, manually enter a resolution here
            //typical resolutions: 1366x768, 1920x1080, 1280x1024, 1280x800, 1024x768, 800x600, 1440x900
            //this.Width = 1366;
            //this.Height = 768;

            screenWidth = this.Width; //getResources().getDisplayMetrics().widthPixels;
            screenHeight = this.Height; //getResources().getDisplayMetrics().heightPixels;
            float sqrW = (float)screenWidth / (float)squaresInWidth;
            float sqrH = (float)screenHeight / (float)squaresInHeight;
            if (sqrW > sqrH)
            {
                squareSize = (int)(sqrH);
            }
            else
            {
                squareSize = (int)(sqrW);
            }
            if ((squareSize > 100) && (squareSize < 105))
            {
                squareSize = 100;
            }
            screenDensity = (float)squareSize / (float)squareSizeInPixels;
            oXshift = (screenWidth - (squareSize * squaresInWidth)) / 2;
            
            InitializeRenderer(); //uncomment this for DIRECT2D ADDITIONS

            //CREATES A FONTFAMILY
            //(LOOK THE out word in the parameter sent to the method, that will modify myFonts object)
            family = LoadFontFamily(mainDirectory + "\\default\\NewModule\\fonts\\Metamorphous-Regular.ttf", out myFonts);
            //family = LoadFontFamily(mainDirectory + "\\default\\NewModule\\fonts\\Orbitron Light.ttf", out myFonts);
            float multiplr = (float)squareSize / 100.0f;
            drawFontLarge = new Font(family, 24.0f * multiplr);
            drawFontReg = new Font(family, 20.0f * multiplr);
            drawFontSmall = new Font(family, 16.0f * multiplr);
            drawFontLargeHeight = 32.0f * multiplr;
            drawFontRegHeight = 26.0f * multiplr;
            drawFontSmallHeight = 20.0f * multiplr;
            InitCustomFont();
            
            animationTimer.Tick += new System.EventHandler(this.AnimationTimer_Tick);
            floatyTextTimer.Tick += new System.EventHandler(this.FloatyTextTimer_Tick);
            floatyTextMainMapTimer.Tick += new System.EventHandler(this.FloatyTextMainMapTimer_Tick);

            //cc = new CommonCode(this);            
            //mod = new Module();

            log = new IbbHtmlLogBox(this, oXshift + (int)(3 * screenDensity), oYshift, 6 * squareSize - (int)(6 * screenDensity), 7 * squareSize);
            log.numberOfLinesToShow = 20;
            cc.addLogText("red", "screenDensity: " + screenDensity);
            cc.addLogText("fuchsia", "screenWidth: " + screenWidth);
            cc.addLogText("lime", "screenHeight: " + screenHeight);
            cc.addLogText("yellow", "squareSize: " + squareSize);
            cc.addLogText("yellow", "sqrW: " + sqrW);
            cc.addLogText("yellow", "sqrH: " + sqrH);
            cc.addLogText("yellow", "");
            cc.addLogText("red", "Welcome to IceBlink 2");
            cc.addLogText("fuchsia", "You can scroll this message log box, use mouse wheel or scroll bar");
            
            //TODOinitializeMusic();
            setupMusicPlayers();
            //TODOinitializeCombatMusic();
            

            if (fixedModule.Equals("")) //this is the IceBlink Engine app
            {
                screenLauncher = new ScreenLauncher(mod, this);
                screenLauncher.loadModuleFiles();
                screenType = "launcher";
            }
            else //this is a fixed module
            {
                mod = cc.LoadModule(fixedModule + "/" + fixedModule + ".mod", false);
                resetGame();
                cc.LoadSaveListItems();
                screenType = "title";
            }

        }

        public void createScreens()
	    {
		    sf = new ScriptFunctions(mod, this);
		    pfa = new PathFinderAreas(mod);
		    screenParty = new ScreenParty(mod, this);
		    screenInventory = new ScreenInventory(mod, this);
            screenItemSelector = new ScreenItemSelector(mod, this);
            screenPortraitSelector = new ScreenPortraitSelector(mod, this);
            screenTokenSelector = new ScreenTokenSelector(mod, this);
            screenPcSelector = new ScreenPcSelector(mod, this);
		    screenJournal = new ScreenJournal(mod, this);	
		    screenShop = new ScreenShop(mod, this);
		    screenCastSelector = new ScreenCastSelector(mod, this);
		    screenConvo = new ScreenConvo(mod, this);
		    //logicTreeRun = new LogicTreeRun(mod, this);
		    screenCombat = new ScreenCombat(mod, this);
		    screenMainMap = new ScreenMainMap(mod, this);
		    screenTitle = new ScreenTitle(mod, this);
		    screenPcCreation = new ScreenPcCreation(mod, this);
		    screenSpellLevelUp = new ScreenSpellLevelUp(mod, this);
		    screenTraitLevelUp = new ScreenTraitLevelUp(mod, this);		
		    screenLauncher = new ScreenLauncher(mod, this);
		    screenPartyBuild = new ScreenPartyBuild(mod, this);
            screenPartyRoster = new ScreenPartyRoster(mod,this);
	    }
        public void LoadStandardImages()
        {
            cc.walkPass = cc.LoadBitmap("walk_pass");
            cc.walkBlocked = cc.LoadBitmap("walk_block");
            cc.losBlocked = cc.LoadBitmap("los_block");
            cc.black_tile = cc.LoadBitmap("black_tile");
            cc.turn_marker = cc.LoadBitmap("turn_marker");
            cc.pc_dead = cc.LoadBitmap("pc_dead");
            cc.pc_stealth = cc.LoadBitmap("pc_stealth");
            cc.hitSymbol = cc.LoadBitmap("hit_symbol");
            cc.missSymbol = cc.LoadBitmap("miss_symbol");
            cc.tint_dawn = cc.LoadBitmap("tint_dawn");
            cc.tint_sunrise = cc.LoadBitmap("tint_sunrise");
            cc.tint_sunset = cc.LoadBitmap("tint_sunset");
            cc.tint_dusk = cc.LoadBitmap("tint_dusk");
            cc.tint_night = cc.LoadBitmap("tint_night");
            cc.facing1 = cc.LoadBitmap("facing1");
            cc.facing2 = cc.LoadBitmap("facing2");
            cc.facing3 = cc.LoadBitmap("facing3");
            cc.facing4 = cc.LoadBitmap("facing4");
            cc.facing6 = cc.LoadBitmap("facing6");
            cc.facing7 = cc.LoadBitmap("facing7");
            cc.facing8 = cc.LoadBitmap("facing8");
            cc.facing9 = cc.LoadBitmap("facing9");
        }	
	    public void resetGame()
	    {
		    //mod = new Module();
		    mod = cc.LoadModule(mod.moduleName + ".mod", false);
            if (mod.useRealTimeTimer == true)
            {
                realTimeTimer.Interval = realTimeTimerLengthInMilliSeconds;
                realTimeTimer.Tick += new System.EventHandler(this.realTimeTimer_Tick);
            }
            if (mod.useSmoothMovement == true)
            {
                //16 milliseconds a tick, equals - theoretically - about 60 FPS
                smoothMoveTimer.Interval = smoothMoveTimerLengthInMilliSeconds;

                //these are the pix moved per tick, designed so that a square is traversed within realTimeTimerLengthInMilliSeconds 
                //update: actually as the 60 FPS are never reached, we will see little stops between prop moves with realtime timer on
                floatPixMovedPerTick = (float)squareSize / 90f;
                //IBMessageBox.Show(this, "floatPixMovedPerTick after first is:" + floatPixMovedPerTick.ToString());
                //due to a mistake of mine 4 pix were moved always beforehand, trying a dynamically calculated average of 7.5 pix now, increases speed by 90%
                floatPixMovedPerTick = floatPixMovedPerTick / (((float)realTimeTimerLengthInMilliSeconds / 1000f * 2f / 3f)) * 6.5f;
                //IBMessageBox.Show(this, "floatPixMovedPerTick after second is is:" + floatPixMovedPerTick.ToString());
                //IBMessageBox.Show(this, "real time timer length is:" + realTimeTimerLengthInMilliSeconds.ToString());
                


                smoothMoveTimer.Tick += new System.EventHandler(this.smoothMoveTimer_Tick);
                smoothMoveTimer.Start();
            }


           if (mod.useOrbitronFont == true)
           {
               family = LoadFontFamily(mainDirectory + "\\default\\NewModule\\fonts\\Orbitron Light.ttf", out myFonts);
               float multiplr = (float)squareSize / 100.0f;
               drawFontLarge = new Font(family, 24.0f * multiplr);
               drawFontReg = new Font(family, 20.0f * multiplr);
               drawFontSmall = new Font(family, 16.0f * multiplr);
               drawFontLargeHeight = 32.0f * multiplr;
               drawFontRegHeight = 26.0f * multiplr;
               drawFontSmallHeight = 20.0f * multiplr;
               InitCustomFont();
           }


		    mod.debugMode = false;
		    mod.loadAreas(this);
		    mod.setCurrentArea(mod.startingArea, this);
		    mod.PlayerLocationX = mod.startingPlayerPositionX;
		    mod.PlayerLocationY = mod.startingPlayerPositionY;
		    cc.title = cc.LoadBitmap("title"); // BitmapFactory.decodeResource(getResources(), R.drawable.nar_lanterna);
            LoadStandardImages();
		    cc.LoadRaces();
		    cc.LoadPlayerClasses();
		    cc.LoadItems();
		    cc.LoadContainers();
		    cc.LoadShops();
		    cc.LoadEffects();
		    cc.LoadSpells();
		    cc.LoadTraits();
		    cc.LoadCreatures();
		    cc.LoadEncounters();
		    cc.LoadJournal();	
		    cc.LoadTileBitmapList();
				
		    foreach (Container c in mod.moduleContainersList)
            {
                c.initialContainerItemRefs.Clear();
                foreach (ItemRefs i in c.containerItemRefs)
                {
                    c.initialContainerItemRefs.Add(i.DeepCopy());
                }
            }
            foreach (Shop s in mod.moduleShopsList)
            {
                s.initialShopItemRefs.Clear();
                foreach (ItemRefs i in s.shopItemRefs)
                {
                    s.initialShopItemRefs.Add(i.DeepCopy());
                }
            }
            foreach (Area a in mod.moduleAreasObjects)
            {
                a.InitialAreaPropTagsList.Clear();
                foreach (Prop p in a.Props)
                {
                    a.InitialAreaPropTagsList.Add(p.PropTag);
                }            
            }
        
		    cc.nullOutControls();
		    cc.setControlsStart();
            cc.setPortraitsStart();

		    cc.setToggleButtonsStart();
		    createScreens();
		    initializeSounds();
		
		    cc.LoadTestParty();		
		    //LogText.clear();
		
		    //load all the message box helps/tutorials
		    cc.stringBeginnersGuide = cc.loadTextToString("MessageBeginnersGuide.txt");
		    cc.stringPlayersGuide = cc.loadTextToString("MessagePlayersGuide.txt");
		    cc.stringPcCreation = cc.loadTextToString("MessagePcCreation.txt");
		    cc.stringMessageCombat = cc.loadTextToString("MessageCombat.txt");
		    cc.stringMessageInventory = cc.loadTextToString("MessageInventory.txt");
		    cc.stringMessageParty = cc.loadTextToString("MessageParty.txt");
		    cc.stringMessageMainMap = cc.loadTextToString("MessageMainMap.txt");
	    }
        
        private FontFamily LoadFontFamily(string fileName, out PrivateFontCollection _myFonts)
        {
            //IN MEMORY _myFonts point to the myFonts created in the load event.
            _myFonts = new PrivateFontCollection();//here is where we assing memory space to myFonts 
            _myFonts.AddFontFile(fileName);//we add the full path of the ttf file
            return _myFonts.Families[0];//returns the family object as usual.
        }
        private void InitCustomFont()
        {
            CurrentResourceFontLoader = new ResourceFontLoader(factoryDWrite);
            CurrentFontCollection = new SharpDX.DirectWrite.FontCollection(factoryDWrite, CurrentResourceFontLoader, CurrentResourceFontLoader.Key);
            FontFamilyName = "Metamorphous";
            if (mod.useOrbitronFont == true)
            {
                FontFamilyName = "Orbitron Light";
            }
            //comboBoxFonts.Items.Add("Pericles");
            //comboBoxFonts.Items.Add("Kootenay");
            //comboBoxFonts.SelectedIndex = 0;
        }

	    //MUSIC AND SOUNDS	    
	    /*public Runnable doMusicDelay = new Runnable()
	    {
		    @Override
		    public void run()
		    {
			    startMusic();
		    }
	    };*/	
	    /*public Runnable doAmbientDelay = new Runnable()
	    {
		    @Override
		    public void run()
		    {
			    startAmbient();
		    }
	    };*/	
	    /*public Runnable doCombatMusicDelay = new Runnable()
	    {
		    @Override
		    public void run()
		    {
			    startCombatMusic();
		    }
	    };
	    */

        #region Area Music/Sounds
        public void setupMusicPlayers()
        {
            try
            {
                areaMusic = new WMPLib.WindowsMediaPlayer();
                areaMusic.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(AreaMusic_PlayStateChange);
                areaMusic.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                areaMusic.settings.volume = 50;

                areaSounds = new WMPLib.WindowsMediaPlayer();
                areaSounds.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(AreaSounds_PlayStateChange);
                areaSounds.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);

                startMusic();
                startAmbient();
            }
            catch (Exception ex)
            {
                cc.addLogText("red","Failed to setup Music Player...Audio will be disabled. Most likely due to not having Windows Media Player installed or having an incompatible version.");
                errorLog(ex.ToString());
            }
        }
        public void startMusic()
        {
            try
            {
                if ((currentMainMusic.Equals(mod.currentArea.AreaMusic)) && (areaMusic != null))
                {
                    areaMusic.controls.play();
                }
                else
                {
                    areaMusic.controls.stop();
                   
                    if (mod.currentArea.AreaMusic != "none")
                    {
                        if (File.Exists(this.mainDirectory + "\\modules\\" + this.mod.moduleName + "\\music\\" + mod.currentArea.AreaMusic))
                        {
                            areaMusic.URL = this.mainDirectory + "\\modules\\" + this.mod.moduleName + "\\music\\" + mod.currentArea.AreaMusic;
                        }
                        else if (File.Exists(this.mainDirectory + "\\modules\\" + this.mod.moduleName + "\\music\\" + mod.currentArea.AreaMusic + ".mp3"))
                        {
                            areaMusic.URL = this.mainDirectory + "\\modules\\" + this.mod.moduleName + "\\music\\" + mod.currentArea.AreaMusic + ".mp3";
                        }
                        else if (File.Exists(this.mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaMusic + ".mp3"))
                        {
                            areaMusic.URL = this.mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaMusic + ".mp3";
                        }
                        else if (File.Exists(this.mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaMusic))
                        {
                            areaMusic.URL = this.mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaMusic;
                        }
                        else
                        {
                            areaMusic.URL = "";
                        }
                        if (areaMusic.URL != "")
                        {
                            areaMusic.controls.stop();
                            areaMusic.controls.play();
                        }
                    }
                    else
                    {
                        areaMusic.URL = "";
                    }
                }
            }
            catch (Exception ex)
            {
                cc.addLogText("red", "Failed on startMusic(): " + ex.ToString());
                errorLog(ex.ToString());
            }        
        }
        public void startAmbient()
        {
            try
            {
                if ((currentAmbientMusic.Equals(mod.currentArea.AreaSounds)) && (areaSounds != null))
                {
                    areaSounds.controls.play();
                }
                else
                {
                    areaSounds.controls.stop();

                    if (mod.currentArea.AreaSounds != "none")
                    {
                        if (File.Exists(mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentArea.AreaSounds))
                        {
                            areaSounds.URL = mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentArea.AreaSounds;
                        }
                        else if (File.Exists(mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentArea.AreaSounds + ".mp3"))
                        {
                            areaSounds.URL = mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentArea.AreaSounds + ".mp3";
                        }
                        else if (File.Exists(mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaSounds))
                        {
                            areaSounds.URL = mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaSounds;
                        }
                        else if (File.Exists(mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaSounds + ".mp3"))
                        {
                            areaSounds.URL = mainDirectory + "\\default\\NewModule\\music\\" + mod.currentArea.AreaSounds + ".mp3";
                        }
                        else
                        {
                            areaSounds.URL = "";
                        }
                        if (areaSounds.URL != "")
                        {
                            areaSounds.controls.stop();
                            areaSounds.controls.play();
                        }
                    }
                    else
                    {
                        areaSounds.URL = "";
                    }
                }
            }
            catch (Exception ex)
            {
                cc.addLogText("red", "Failed on startAmbient(): " + ex.ToString());
                errorLog(ex.ToString());
            }
        }
        public void startCombatMusic()
        {
            try
            {
                if ((currentCombatMusic.Equals(mod.currentEncounter.AreaMusic)) && (areaMusic != null))
                {
                    areaMusic.controls.play();
                }
                else
                {
                    areaMusic.controls.stop();
                    
                    if (mod.currentEncounter.AreaMusic != "none")
                    {
                        if (File.Exists(mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentEncounter.AreaMusic + ".mp3"))
                        {
                            areaMusic.URL = mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentEncounter.AreaMusic + ".mp3";
                        }
                        else if (File.Exists(mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentEncounter.AreaMusic))
                        {
                            areaMusic.URL = mainDirectory + "\\modules\\" + mod.moduleName + "\\music\\" + mod.currentEncounter.AreaMusic;
                        }
                        else if (File.Exists(mainDirectory + "\\default\\NewModule\\music\\" + mod.currentEncounter.AreaMusic + ".mp3"))
                        {
                            areaMusic.URL = mainDirectory + "\\default\\NewModule\\music\\" + mod.currentEncounter.AreaMusic + ".mp3";
                        }
                        else if (File.Exists(mainDirectory + "\\default\\NewModule\\music\\" + mod.currentEncounter.AreaMusic))
                        {
                            areaMusic.URL = mainDirectory + "\\default\\NewModule\\music\\" + mod.currentEncounter.AreaMusic;
                        }
                        else
                        {
                            areaMusic.URL = "";
                        }
                        if (areaMusic.URL != "")
                        {
                            areaMusic.controls.stop();
                            areaMusic.controls.play();
                        }
                    }
                    else
                    {
                        areaMusic.URL = "";
                    }
                }
            }
            catch (Exception ex)
            {
                cc.addLogText("red", "Failed on playCombatAreaMusicSounds()" + ex.ToString());
                errorLog(ex.ToString());
            }
        }
        private void AreaMusic_PlayStateChange(int NewState)
        {
            try
            {
                if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
                {
                    delayMusic();
                }
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on AreaMusic_PlayStateChange()" + ex.ToString());
                errorLog(ex.ToString());
            }
        }
        private void AreaSounds_PlayStateChange(int NewState)
        {
            try
            {
                if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
                {
                    delaySounds();
                }
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on AreaSounds_PlayStateChange()" + ex.ToString());
                errorLog(ex.ToString());
            }
        }
        private void Player_MediaError(object pMediaObject)
        {
            cc.addLogText("Cannot play media file.");
        }
        private void delayMusic()
        {
            try
            {
                int rand = sf.RandInt(mod.currentArea.AreaMusicDelayRandomAdder);
                areaMusicTimer.Enabled = false;
                areaMusic.controls.stop();
                areaMusicTimer.Interval = mod.currentArea.AreaMusicDelay + rand;
                areaMusicTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on delayMusic()" + ex.ToString());
                errorLog(ex.ToString());
            }
        }
        private void areaMusicTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (areaMusic.URL != "")
                {
                    areaMusic.controls.play();
                }
                areaMusicTimer.Enabled = false;
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on areaMusicTimer_Tick()" + ex.ToString());
                errorLog(ex.ToString());
            }
        }
        private void delaySounds()
        {
            try
            {
                int rand = sf.RandInt(mod.currentArea.AreaSoundsDelayRandomAdder);
                areaSoundsTimer.Enabled = false;
                areaSounds.controls.stop();
                areaSoundsTimer.Interval = mod.currentArea.AreaSoundsDelay + rand;
                areaSoundsTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on delaySounds()" + ex.ToString());
                errorLog(ex.ToString());
            }
        }
        private void areaSoundsTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (areaSounds.URL != "")
                {
                    areaSounds.controls.play();
                }
                areaSoundsTimer.Enabled = false;
            }
            catch (Exception ex)
            {
                cc.addLogText("Failed on areaSoundsTimer_Tick()" + ex.ToString());
                errorLog(ex.ToString());
            }
        }
        #endregion

	    public void startMusicOld()
	    {            
		    /*if ((currentMainMusic.equals(mod.currentArea.AreaMusic)) && (playerMain != null))
		    {
			    playerMain.start();
		    }
		    else
		    {
			    String filename = mod.currentArea.AreaMusic;
			    currentMainMusic = filename;
			    playerMain = new MediaPlayer();
			    playerMain.setOnPreparedListener(new OnPreparedListener() 
			    {
	        	    public void onPrepared(MediaPlayer playerMain) 
	        	    {
	        		    playerMain.start();
	        	    }
			    });
			    playerMain.setOnCompletionListener(new MediaPlayer.OnCompletionListener() 
			    {
			        public void onCompletion(MediaPlayer playerMain) 
			        {
			            //start the delay timer
			    	    try
		                {
		                    int rand = sf.RandInt(mod.currentArea.AreaMusicDelayRandomAdder);
		                    int delay = mod.currentArea.AreaMusicDelay + rand;
		                    postDelayed(doMusicDelay, delay);
		                }
		                catch (Exception ex)
		                {
		                    //game.errorLog("Failed on delayMusic()" + ex.ToString());
		                }
			        }
			    });
			
			    AssetFileDescriptor afd = null;
			    try 
			    {
				    afd = this.gameContext.getAssets().openFd("music/" + filename + ".mp3");
			    } 
			    catch (Exception e) 
			    {
				    e.printStackTrace();
			    }
			    try 
			    {
				    playerMain.setDataSource(afd.getFileDescriptor(),afd.getStartOffset(),afd.getLength());
			    } 
			    catch (Exception e) 
			    {
				    e.printStackTrace();
			    }
			
			    if (afd == null)
		        {
				    File sdCard = Environment.getExternalStorageDirectory();
				    Uri currentMusic = Uri.parse(sdCard.getAbsolutePath() + "/IceBlinkRPG/" + mod.moduleName + "/music/" + filename + ".mp3");
				    playerMain.setAudioStreamType(AudioManager.STREAM_MUSIC);
				    try 
				    {
					    playerMain.setDataSource(gameContext, currentMusic);
				    } 
				    catch (Exception e) 
				    {
					    e.printStackTrace();
				    }
		        }
			
			    try 
			    {
				    playerMain.prepare();
			    } 
			    catch (Exception e) 
			    {
				    e.printStackTrace();
			    }
		    }*/
	    }		
	    public void startAmbientOld()
	    {
		    /*if ((currentAmbientMusic.equals(mod.currentArea.AreaSounds)) && (playerAmbient != null))
		    {
			    playerAmbient.start();
		    }
		    else
		    {
			    String filename = mod.currentArea.AreaSounds;
			    currentAmbientMusic = filename;
			    playerAmbient = new MediaPlayer();
			    playerAmbient.setOnPreparedListener(new OnPreparedListener() 
			    {
	        	    public void onPrepared(MediaPlayer playerAmbient) 
	        	    {
	        		    playerAmbient.start();
	        	    }
			    });
			    playerAmbient.setOnCompletionListener(new MediaPlayer.OnCompletionListener() 
			    {
			        public void onCompletion(MediaPlayer playerAmbient) 
			        {
			            //start the delay timer
			    	    try
		                {
		                    int rand = sf.RandInt(mod.currentArea.AreaSoundsDelayRandomAdder);
		                    int delay = mod.currentArea.AreaSoundsDelayRandomAdder + rand;
		                    postDelayed(doAmbientDelay, delay);
		                }
		                catch (Exception ex)
		                {
		                    //game.errorLog("Failed on delayMusic()" + ex.ToString());
		                }
			        }
			    });
			
			    AssetFileDescriptor afd = null;
			    try 
			    {
				    afd = this.gameContext.getAssets().openFd("music/" + filename + ".mp3");
			    } 
			    catch (Exception e) 
			    {
				    e.printStackTrace();
			    }
			    try 
			    {
				    playerAmbient.setDataSource(afd.getFileDescriptor(),afd.getStartOffset(),afd.getLength());
			    } 
			    catch (Exception e) 
			    {
				    e.printStackTrace();
			    }
			
		        if (afd == null)
		        {
				    File sdCard = Environment.getExternalStorageDirectory();
				    Uri currentMusic = Uri.parse(sdCard.getAbsolutePath() + "/IceBlinkRPG/" + mod.moduleName + "/music/" + filename + ".mp3");
				    playerAmbient.setAudioStreamType(AudioManager.STREAM_MUSIC);
				    try 
				    {
					    playerAmbient.setDataSource(gameContext, currentMusic);
				    } 
				    catch (Exception e) 
				    {
					    e.printStackTrace();
				    }
		        }
		    
			    try 
			    {
				    playerAmbient.prepare();
			    } 
			    catch (Exception e) 
			    {
				    e.printStackTrace();
			    }
		    }*/
	    }
	    public void startCombatMusicOld()
	    {
		    /*if ((currentCombatMusic.equals(mod.currentEncounter.AreaMusic)) && (playerCombat != null))
		    {
			    playerCombat.start();
		    }
		    else
		    {
			    String filename = mod.currentEncounter.AreaMusic;
			    currentCombatMusic = filename;
			    playerCombat = new MediaPlayer();
			    playerCombat.setOnPreparedListener(new OnPreparedListener() 
			    {
	        	    public void onPrepared(MediaPlayer playerCombat) 
	        	    {
	        		    playerCombat.start();
	        	    }
			    });
			    playerCombat.setOnCompletionListener(new MediaPlayer.OnCompletionListener() 
			    {
			        public void onCompletion(MediaPlayer playerCombat) 
			        {
			            //start the delay timer
			    	    try
		                {
		                    int rand = sf.RandInt(mod.currentEncounter.AreaMusicDelayRandomAdder);
		                    int delay = mod.currentEncounter.AreaMusicDelay + rand;
		                    postDelayed(doCombatMusicDelay, delay);
		                }
		                catch (Exception ex)
		                {
		                    //game.errorLog("Failed on delayMusic()" + ex.ToString());
		                }
			        }
			    });
			
			    AssetFileDescriptor afd = null;
			    try 
			    {
				    afd = this.gameContext.getAssets().openFd("music/" + filename + ".mp3");
			    } 
			    catch (Exception e) 
			    {
				    e.printStackTrace();
			    }
			    try 
			    {
				    playerCombat.setDataSource(afd.getFileDescriptor(),afd.getStartOffset(),afd.getLength());
			    } 
			    catch (Exception e) 
			    {
				    e.printStackTrace();
			    }
			
			    if (afd == null)
		        {
				    File sdCard = Environment.getExternalStorageDirectory();
				    Uri currentMusic = Uri.parse(sdCard.getAbsolutePath() + "/IceBlinkRPG/" + mod.moduleName + "/music/" + filename + ".mp3");
				    playerCombat.setAudioStreamType(AudioManager.STREAM_MUSIC);
				    try 
				    {
					    playerCombat.setDataSource(gameContext, currentMusic);
				    } 
				    catch (Exception e) 
				    {
					    e.printStackTrace();
				    }
		        }
			
			    try 
			    {
				    playerCombat.prepare();
			    } 
			    catch (Exception e) 
			    {
				    e.printStackTrace();
			    }
		    }*/
	    }
	    public void stopMusic()
	    {
            areaMusic.controls.pause();
	    }
	    public void stopAmbient()
	    {
            areaSounds.controls.pause();
	    }
	    public void stopCombatMusic()
	    {
            areaMusic.controls.pause();
	    }
	    public void initializeSounds()
	    {
            oSoundStreams.Clear();
            string jobDir = "";
            jobDir = this.mainDirectory + "\\modules\\" + mod.moduleName + "\\sounds";
            foreach (string f in Directory.GetFiles(jobDir, "*.*", SearchOption.AllDirectories))
            {
                oSoundStreams.Add(Path.GetFileNameWithoutExtension(f), File.OpenRead(Path.GetFullPath(f)));
            }
	    }
	    public void PlaySound(string filenameNoExtension)
	    {            
            if ((filenameNoExtension.Equals("none")) || (filenameNoExtension.Equals("")) || (!mod.playSoundFx))
            {
                //play nothing
                return;
            }
            else
            {
                try
                {
                    soundPlayer.Stream = oSoundStreams[filenameNoExtension];
                    soundPlayer.Play();
                }
                catch (Exception ex)
                {
                    errorLog(ex.ToString());
                    if (mod.debugMode) //SD_20131102
                    {
                        cc.addLogText("<font color='yellow'>failed to play sound" + filenameNoExtension + "</font><BR>");
                    }
                    initializeSounds();
                }
            }            
	    }

        //Animation Timer Stuff
        public void postDelayed(string type, int delay)
        {
            //gv.postDelayed("doAnimation", 2 * mod.combatAnimationSpeed);
            if (type.Equals("doAnimation"))
            {
                animationTimer.Enabled = true;
                animationTimer.Interval = delay;
                animationTimer.Start();
            }
            else if (type.Equals("doFloatyText"))
            {
                floatyTextTimer.Enabled = true;
                floatyTextTimer.Interval = delay;
                floatyTextTimer.Start();
            }
            else if (type.Equals("doFloatyTextMainMap"))
            {
                floatyTextMainMapTimer.Enabled = true;
                floatyTextMainMapTimer.Interval = delay;
                floatyTextMainMapTimer.Start();
            }
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationTimer.Enabled = false;
            animationTimer.Stop();
            Render();
            //Invalidate();
            screenCombat.doAnimationController();
        }
        private void realTimeTimer_Tick(object sender, EventArgs e)
        {
            if (screenType.Equals("main"))
            {
                realTimeTimer.Stop();
                cc.doUpdate();
                realTimeTimer.Start();
            }
        }

        private void smoothMoveTimer_Tick(object sender, EventArgs e)
        {
            if (screenType.Equals("main"))
            {
                
                smoothMoveTimer.Stop();
                Render();
                smoothMoveTimer.Start();

            }
        }

        private void FloatyTextTimer_Tick(object sender, EventArgs e)
        {
            floatyTextTimer.Enabled = false;
            floatyTextTimer.Stop();
            //Invalidate();
            Render();

            int pH = (int)((float)screenHeight / 200.0f);
            //move all floaty text up one %pixel
            if (this.cc.floatyTextCounter < 10)
            {
                foreach (FloatyText ft in this.cc.floatyTextList)
                {
                    ft.location.Y -= pH;
                }
                //call again until counter hits 10
                this.cc.floatyTextCounter++;
                screenCombat.doFloatyTextLoop();
            }
            else
            {
                this.cc.floatyTextCounter = 0;
                screenCombat.floatyTextOn = false;
                this.cc.floatyTextList.Clear();
            }
            Render();
        }

        private void FloatyTextMainMapTimer_Tick(object sender, EventArgs e)
        {
            floatyTextMainMapTimer.Enabled = false;
            floatyTextMainMapTimer.Stop();
            //Invalidate();
            Render();

            if (screenMainMap.floatyTextPool.Count > 0)
            {
                for (int i = screenMainMap.floatyTextPool.Count - 1; i >= 0; i--)
                {
                    if (screenMainMap.floatyTextPool[i].timer > screenMainMap.floatyTextPool[i].timerLength)
                    {
                        screenMainMap.floatyTextPool.RemoveAt(i);
                    }
                    else
                    {
                        screenMainMap.floatyTextPool[i].z++; //increase float height multiplier
                        screenMainMap.floatyTextPool[i].timer += 400;
                    }
                }
                screenMainMap.doFloatyTextLoop();
            }
        }

        //DRAW ROUTINES
        public void drawBlackMap()
        {
            /*
            Paint paint = new Paint();
            Rect r = new Rect(0, 0, 7 * gv.squareSize, (8 * gv.squareSize) + (int)(10 * gv.screenDensity));
            paint.setStyle(Paint.Style.FILL);
            paint.setColor(Color.BLACK);
            canvas.drawRect(r, paint);
            */
        }
        public void drawLog()
        {
            //log.updateLog();
            log.onDrawLogBox();            
        }
        public void onTouchLog()
        {
            //int eventAction = event.getAction();
            /*TODOswitch (eventAction)
            {
                case MotionEvent.ACTION_DOWN:
                    //int x = (int) event.getX();
                    //int y = (int) event.getY();

                    if ((x > topleftLog.X) && (x < bottomrightLog.X) && (y > topleftLog.Y) && (y < bottomrightLog.Y))
                    {
                        touchDownY = y;
                        logLocYatTouchDownY = logLocY;
                    }
                    break;

                case MotionEvent.ACTION_MOVE:
                    //x = (int) event.getX();
                    //y = (int) event.getY();

                    if ((x > topleftLog.X) && (x < bottomrightLog.X) && (y > topleftLog.Y) && (y < bottomrightLog.Y))
                    {
                        logLocY = logLocYatTouchDownY - (touchDownY - y);
                    }
                    break;
            }*/
        }
        
        public void CleanUpDrawTextResources()
        {
            if (textFormat != null)
            {
                textFormat.Dispose();
                textFormat = null;
            }
            if (textLayout != null)
            {
                textLayout.Dispose();
                textLayout = null;
            }
        }
        public void DrawText(string text, float xLoc, float yLoc)
        {
            DrawText(text, xLoc, yLoc, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, 1.0f, SharpDX.Color.White, false);
        }
        public void DrawText(string text, float x, float y, FontWeight fw, SharpDX.DirectWrite.FontStyle fs, SharpDX.Color fontColor)
        {
            DrawText(text, x, y, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, 1.0f, fontColor, false);
        }
        public void DrawText(string text, float xLoc, float yLoc, float scaler, SharpDX.Color fontColor)
        {
            DrawText(text, xLoc, yLoc, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, scaler, fontColor, false);
        }
        public void DrawText(string text, IbRect rect, float scaler, SharpDX.Color fontColor)
        {
            DrawText(text, rect, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, scaler, fontColor);
        }
        public void DrawText(string text, IbRect rect, FontWeight fw, SharpDX.DirectWrite.FontStyle fs, float scaler, SharpDX.Color fontColor)
        {
            CleanUpDrawTextResources();
            float thisFontHeight = drawFontRegHeight;
            if (scaler > 1.05f)
            {
                thisFontHeight = drawFontLargeHeight;
            }
            else if (scaler < 0.95f)
            {
                thisFontHeight = drawFontSmallHeight;
            }
            RectangleF rectF = new RectangleF(rect.Left, rect.Top + oYshift, rect.Width, rect.Height);
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, fontColor))
            {
                //textFormat = new TextFormat(factoryDWrite, thisFont.FontFamily.Name, fw, fs, FontStretch.Normal, thisFont.Height) { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
                textFormat = new TextFormat(factoryDWrite, FontFamilyName, CurrentFontCollection, fw, fs, FontStretch.Normal, thisFontHeight) { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
                textLayout = new TextLayout(factoryDWrite, text, textFormat, rect.Width, rect.Height);
                renderTarget2D.DrawTextLayout(new Vector2(rect.Left, rect.Top + oYshift), textLayout, scb, DrawTextOptions.None);
            }
        }
        public void DrawText(string text, float x, float y, FontWeight fw, SharpDX.DirectWrite.FontStyle fs, float scaler, SharpDX.Color fontColor, bool isUnderlined)
        {
            CleanUpDrawTextResources();
            float thisFontHeight = drawFontRegHeight;
            if (scaler > 1.05f)
            {
                thisFontHeight = drawFontLargeHeight;
            }
            else if (scaler < 0.95f)
            {
                thisFontHeight = drawFontSmallHeight;
            }
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, fontColor))
            {
                //textFormat = new TextFormat(factoryDWrite, thisFont.FontFamily.Name, fw, fs, FontStretch.Normal, thisFont.Height) { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
                textFormat = new TextFormat(factoryDWrite, FontFamilyName, CurrentFontCollection, fw, fs, FontStretch.Normal, thisFontHeight) { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
                textLayout = new TextLayout(factoryDWrite, text, textFormat, this.Width, this.Height);
                if (isUnderlined)
                {
                    textLayout.SetUnderline(true, new TextRange(0, text.Length - 1));
                }
                renderTarget2D.DrawTextLayout(new Vector2(x, y + oYshift), textLayout, scb, DrawTextOptions.None);
            }
        }
        public void DrawRoundRectangle(IbRect rect, int rad, SharpDX.Color penColor, int penWidth)
        {
            RoundedRectangle r = new RoundedRectangle();
            r.Rect = new SharpDX.RectangleF(rect.Left, rect.Top + oYshift, rect.Width, rect.Height);
            r.RadiusX = rad;
            r.RadiusY = rad;
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, penColor))
            {
                renderTarget2D.DrawRoundedRectangle(r, scb, penWidth);
            }
        }   
        public void DrawRectangle(IbRect rect, SharpDX.Color penColor, int penWidth)
        {
            //Pen p = new Pen(penColor, penWidth);
            SharpDX.RectangleF r = new SharpDX.RectangleF(rect.Left, rect.Top + oYshift, rect.Width, rect.Height);
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, penColor))
            {
                renderTarget2D.DrawRectangle(r, scb, penWidth);
            }
            
            //gCanvas.DrawRectangle(p, r);
            //p.Dispose();
        }
        public void DrawLine(int lastX, int lastY, int nextX, int nextY, SharpDX.Color penColor, int penWidth)
        {
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, penColor))
            {
                renderTarget2D.DrawLine(new Vector2(lastX,lastY), new Vector2(nextX, nextY), scb, penWidth);
            }
            //Pen p = new Pen(penColor, penWidth);
            //gCanvas.DrawLine(p, lastX, lastY, nextX, nextY);
            //p.Dispose();
        }
        public void DrawBitmapGDI(System.Drawing.Bitmap bitmap, IbRect source, IbRect target) //change this to DrawBitmapGDI
        {
            Rectangle tar = new Rectangle(target.Left, target.Top + oYshift, target.Width, target.Height);
            Rectangle src = new Rectangle(source.Left, source.Top, source.Width, source.Height);
            gCanvas.DrawImage(bitmap, tar, src, GraphicsUnit.Pixel);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target) //change this to DrawBitmap
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, bool mirror, bool flip) //change this to DrawBitmap
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, mirror, flip);
        }
        protected override void OnPaint(PaintEventArgs e)
	    {
            Render();
            //base.OnPaint(e);
            //gCanvas = e.Graphics;
            /*BeginDraw(); //uncomment this for DIRECT2D ADDITIONS
            renderTarget2D.Clear(Color4.Black); //uncomment this for DIRECT2D ADDITIONS
            
            if (screenType.Equals("title"))
		    {
    		    screenTitle.redrawTitle();
		    }
    	    else if (screenType.Equals("launcher"))
		    {
    		    screenLauncher.redrawLauncher();
		    }
    	    else if (screenType.Equals("pcCreation"))
		    {
			    screenPcCreation.redrawPcCreation();
		    }
    	    else if (screenType.Equals("learnSpellCreation"))
    	    {
    		    screenSpellLevelUp.redrawSpellLevelUp(true);    	
    	    }
    	    else if (screenType.Equals("learnSpellLevelUp"))
    	    {
    		    screenSpellLevelUp.redrawSpellLevelUp(false);    	
    	    }
    	    else if (screenType.Equals("learnTraitCreation"))
    	    {
    		    screenTraitLevelUp.redrawTraitLevelUp(true);    	
    	    }
    	    else if (screenType.Equals("learnTraitLevelUp"))
    	    {
    		    screenTraitLevelUp.redrawTraitLevelUp(false);    	
    	    }
    	    else if (screenType.Equals("main"))
		    {
			    screenMainMap.redrawMain();
		    }
		    else if (screenType.Equals("party"))
		    {
			    screenParty.redrawParty();
		    }
		    else if (screenType.Equals("combatParty"))
		    {
			    screenParty.redrawParty();
		    }
		    else if (screenType.Equals("inventory"))
		    {
			    screenInventory.redrawInventory();
		    }
            else if (screenType.Equals("itemSelector"))
            {
                screenItemSelector.redrawItemSelector();
            }
            else if (screenType.Equals("portraitSelector"))
            {
                screenPortraitSelector.redrawPortraitSelector();
            }
            else if (screenType.Equals("tokenSelector"))
            {
                screenTokenSelector.redrawTokenSelector();
            }
            else if (screenType.Equals("pcSelector"))
            {
                screenPcSelector.redrawPcSelector();
            }
		    else if (screenType.Equals("combatInventory"))
		    {
			    screenInventory.redrawInventory();
		    }
		    else if (screenType.Equals("journal"))
		    {
			    screenJournal.redrawJournal();
		    }
		    else if (screenType.Equals("shop"))
		    {
			    screenShop.redrawShop();
		    }
		    else if (screenType.Equals("combat"))
		    {
			    screenCombat.redrawCombat();
		    }
		    else if (screenType.Equals("combatCast"))
		    {
			    screenCastSelector.redrawCastSelector(true);
		    }
		    else if (screenType.Equals("mainMapCast"))
		    {
			    screenCastSelector.redrawCastSelector(false);
		    }
		    else if (screenType.Equals("convo"))
		    {
                //if (mod.avoidInteraction == false)
                //{
                    screenConvo.redrawConvo();
                //}
		    }
		    else if (screenType.Equals("partyBuild"))
		    {
			    screenPartyBuild.redrawPartyBuild();
		    }
            else if (screenType.Equals("partyRoster"))
            {
                screenPartyRoster.redrawPartyRoster();
            }
            EndDraw(); //uncomment this for DIRECT2D ADDITIONS
            */
        }

        //DIRECT2D STUFF
        public void InitializeRenderer()
        {
            string state = "";
            try
            {                
                // SwapChain description
                state += "Creating Swap Chain:";
                var desc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription =
                        new ModeDescription(this.Width, this.Height,
                                            new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    OutputHandle = this.Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SwapEffect.Discard,
                    Usage = Usage.RenderTargetOutput
                };

                // Create Device and SwapChain
                state += "Create Device:";
                SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, new[] { SharpDX.Direct3D.FeatureLevel.Level_11_0 }, desc, out _device, out _swapChain);

                // Ignore all windows events
                state += "Create Factory:";
                SharpDX.DXGI.Factory factory = _swapChain.GetParent<SharpDX.DXGI.Factory>();
                factory.MakeWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAll);

                // New RenderTargetView from the backbuffer
                state += "Creating Back Buffer:";
                _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
                
                state += "Create RenderTargetView:";
                _backBufferView = new RenderTargetView(_device, _backBuffer);
                
                factory2D = new SharpDX.Direct2D1.Factory();
                using (var surface = _backBuffer.QueryInterface<Surface>())
                {
                    renderTarget2D = new RenderTarget(factory2D, surface, new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
                }
                renderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;

                //TEXT STUFF
                state += "Creating Text Factory:";
                factoryDWrite = new SharpDX.DirectWrite.Factory();
                sceneColorBrush = new SolidColorBrush(renderTarget2D, SharpDX.Color.Blue);
                renderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;
            }
            catch (SharpDXException ex)
            {
                MessageBox.Show("SharpDX error message appended to IB2ErrorLog.txt");
                this.errorLog(state + "<--->" + ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("SharpDX error message appended to IB2ErrorLog.txt");
                this.errorLog(state + "<--->" + ex.ToString());
            }

            // ##### This app specific #########
            // Load D2D1Bitmap
            //newBus.busBitmap = LoadFromFile(renderTarget2D, "sharpdx.png");
            // Initialize a TextFormat
            //textFormat = new TextFormat(factoryDWrite, "Calibri", 20) { TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };            
            // Initialize a TextLayout
            //textLayout = new TextLayout(factoryDWrite, "load: " + timer1 + "  process: " + timer2, textFormat, this.Width, this.Height);
        }
        public void BeginDraw()
        {
            _device.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, this.Width, this.Height));
            _device.ImmediateContext.OutputMerger.SetTargets(_backBufferView);
            renderTarget2D.BeginDraw();
        }
        public void Draw()
        {
            /*
            renderTarget2D.Clear(Color4.Black);
            // Draw the TextLayout
            //int locX = 100;
            if (mirror == -1)
            {
                DrawBitmap(newBus.busBitmap, new SharpDX.RectangleF(0, 0, newBus.busBitmap.PixelSize.Width, newBus.busBitmap.PixelSize.Height), new SharpDX.RectangleF(0, 0, newBus.busBitmap.PixelSize.Width, newBus.busBitmap.PixelSize.Height), true, false);
            }
            else if (flip == -1)
            {
                DrawBitmap(newBus.busBitmap, new SharpDX.RectangleF(0, 0, newBus.busBitmap.PixelSize.Width, newBus.busBitmap.PixelSize.Height), new SharpDX.RectangleF(0, 0, newBus.busBitmap.PixelSize.Width, newBus.busBitmap.PixelSize.Height), false, true);
            }
            else
            {
                DrawBitmap(newBus.busBitmap, new SharpDX.RectangleF(0, 0, newBus.busBitmap.PixelSize.Width, newBus.busBitmap.PixelSize.Height), new SharpDX.RectangleF(0, 0, newBus.busBitmap.PixelSize.Width, newBus.busBitmap.PixelSize.Height), false, false);
            }
            //renderTarget2D.Transform = Matrix3x2.Transformation(1, 1, 0, 0, 0);
            for (int j = 0; j < 40; j++)
            {
                for (int x = 0; x < 20; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        DrawBitmap(newBus.busBitmap, new SharpDX.RectangleF(0, 0, newBus.busBitmap.PixelSize.Width, newBus.busBitmap.PixelSize.Height), new SharpDX.RectangleF(x * 50 + shift, y * 50 + shift, 50, 50));
                    }
                }
            }
            renderTarget2D.DrawTextLayout(new Vector2(0, 0), textLayout, sceneColorBrush, DrawTextOptions.None); // ##### This app specific
            */
        }
        public void EndDraw()
        {
            renderTarget2D.EndDraw();
            _swapChain.Present(1, PresentFlags.None);
        }
        public void Render()
        {
            BeginDraw(); //uncomment this for DIRECT2D ADDITIONS  
          
            renderTarget2D.Clear(Color4.Black); //uncomment this for DIRECT2D ADDITIONS

            if (screenType.Equals("title"))
            {
                screenTitle.redrawTitle();
            }
            else if (screenType.Equals("launcher"))
            {
                screenLauncher.redrawLauncher();
            }
            else if (screenType.Equals("pcCreation"))
            {
                screenPcCreation.redrawPcCreation();
            }
            else if (screenType.Equals("learnSpellCreation"))
            {
                screenSpellLevelUp.redrawSpellLevelUp(true);
            }
            else if (screenType.Equals("learnSpellLevelUp"))
            {
                screenSpellLevelUp.redrawSpellLevelUp(false);
            }
            else if (screenType.Equals("learnTraitCreation"))
            {
                screenTraitLevelUp.redrawTraitLevelUp(true);
            }
            else if (screenType.Equals("learnTraitLevelUp"))
            {
                screenTraitLevelUp.redrawTraitLevelUp(false);
            }
            else if (screenType.Equals("main"))
            {
                screenMainMap.redrawMain();
            }
            else if (screenType.Equals("party"))
            {
                screenParty.redrawParty();
            }
            else if (screenType.Equals("combatParty"))
            {
                screenParty.redrawParty();
            }
            else if (screenType.Equals("inventory"))
            {
                screenInventory.redrawInventory();
            }
            else if (screenType.Equals("itemSelector"))
            {
                screenItemSelector.redrawItemSelector();
            }
            else if (screenType.Equals("portraitSelector"))
            {
                screenPortraitSelector.redrawPortraitSelector();
            }
            else if (screenType.Equals("tokenSelector"))
            {
                screenTokenSelector.redrawTokenSelector();
            }
            else if (screenType.Equals("pcSelector"))
            {
                screenPcSelector.redrawPcSelector();
            }
            else if (screenType.Equals("combatInventory"))
            {
                screenInventory.redrawInventory();
            }
            else if (screenType.Equals("journal"))
            {
                screenJournal.redrawJournal();
            }
            else if (screenType.Equals("shop"))
            {
                screenShop.redrawShop();
            }
            else if (screenType.Equals("combat"))
            {
                screenCombat.redrawCombat();
            }
            else if (screenType.Equals("combatCast"))
            {
                screenCastSelector.redrawCastSelector(true);
            }
            else if (screenType.Equals("mainMapCast"))
            {
                screenCastSelector.redrawCastSelector(false);
            }
            else if (screenType.Equals("convo"))
            {
                //if (mod.avoidInteraction == false)
                //{
                screenConvo.redrawConvo();
                //}
            }
            else if (screenType.Equals("partyBuild"))
            {
                screenPartyBuild.redrawPartyBuild();
            }
            else if (screenType.Equals("partyRoster"))
            {
                screenPartyRoster.redrawPartyRoster();
            }
            EndDraw(); //uncomment this for DIRECT2D ADDITIONS
        }

        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target)
        {
            DrawD2DBitmap(bitmap, source, target, false, false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, bool mirror, bool flip)
        {
            if ((mirror) && (flip))
            {
                renderTarget2D.Transform = Matrix3x2.Transformation(-1, -1, 0, 0, 0);
                renderTarget2D.DrawBitmap(bitmap,
                                            new SharpDX.RectangleF((target.Left + bitmap.PixelSize.Width) * -1,
                                                (target.Top + bitmap.PixelSize.Height) * -1,
                                                target.Width,
                                                target.Height),
                                            1.0f,
                                            BitmapInterpolationMode.Linear,
                                            new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height));
            }
            else if (flip)
            {
                renderTarget2D.Transform = Matrix3x2.Transformation(1, -1, 0, 0, 0);
                renderTarget2D.DrawBitmap(bitmap,
                                            new SharpDX.RectangleF(target.Left,
                                                (target.Top + bitmap.PixelSize.Height) * -1,
                                                target.Width,
                                                target.Height),
                                            1.0f,
                                            BitmapInterpolationMode.Linear,
                                            new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height));
            }
            else if (mirror)
            {
                renderTarget2D.Transform = Matrix3x2.Transformation(-1, 1, 0, 0, 0);
                
                //left shift for rendering right facing party and combat cretures, those were shifted about half a square too far in right direction on my laptop
                float leftShiftAdjustment = (screenWidth / 1920f) * bitmap.PixelSize.Width; 
                renderTarget2D.DrawBitmap(bitmap,
                                            //new SharpDX.RectangleF((target.Left + bitmap.PixelSize.Width - squareSize/2) * -1,
                                                new SharpDX.RectangleF((target.Left + leftShiftAdjustment) * -1,
                                                target.Top,
                                                target.Width,
                                                target.Height),
                                            1.0f,
                                            BitmapInterpolationMode.Linear,
                                            new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height));
            }
            else
            {
                renderTarget2D.DrawBitmap(bitmap,
                                            new SharpDX.RectangleF(target.Left,
                                                target.Top,
                                                target.Width,
                                                target.Height),
                                            1.0f,
                                            BitmapInterpolationMode.Linear,
                                            new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height));
            }
            //return transform back to original
            renderTarget2D.Transform = Matrix3x2.Transformation(1, 1, 0, 0, 0);
        }

        //INPUT STUFF
        private void GameView_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                log.onMouseWheel(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseWheel);
        }
        private void GameView_MouseDown(object sender, MouseEventArgs e)
        {
            if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                log.onMouseDown(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseDown);
        }
        private void GameView_MouseUp(object sender, MouseEventArgs e)
        {
            if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                log.onMouseUp(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseUp);
        }
        private void GameView_MouseMove(object sender, MouseEventArgs e)
        {
            Render();
            if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                log.onMouseMove(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseMove);
        }
        private void GameView_MouseClick(object sender, MouseEventArgs e)
        {
            onMouseEvent(sender, e, MouseEventType.EventType.MouseClick);
        }
        public void onMouseEvent(object sender, MouseEventArgs e, MouseEventType.EventType eventType)
        {
            try 
            {
                //mouseCount++;
                if (touchEnabled)
                {
                    if (screenType.Equals("main"))
                    {
                        screenMainMap.onTouchMain(e, eventType);	
                    }
                    else if (screenType.Equals("launcher"))
                    {
                        screenLauncher.onTouchLauncher(e, eventType);
                    }
                    else if (screenType.Equals("pcCreation"))
                    {
                        screenPcCreation.onTouchPcCreation(e, eventType);
                    }
                    else if (screenType.Equals("learnSpellCreation"))
                    {
                        screenSpellLevelUp.onTouchSpellLevelUp(e, eventType, true);   	
                    }
                    else if (screenType.Equals("learnSpellLevelUp"))
                    {
                        screenSpellLevelUp.onTouchSpellLevelUp(e, eventType, false);     	
                    }
                    else if (screenType.Equals("learnTraitCreation"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(e, eventType, true);   	
                    }
                    else if (screenType.Equals("learnTraitLevelUp"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(e, eventType, false);     	
                    }
                    else if (screenType.Equals("title"))
                    {
                        screenTitle.onTouchTitle(e, eventType);
                    }
                    else if (screenType.Equals("party"))
                    {
                        screenParty.onTouchParty(e, eventType, false);
                    }
                    else if (screenType.Equals("combatParty"))
                    {
                        screenParty.onTouchParty(e, eventType, true);
                    }
                    else if (screenType.Equals("inventory"))
                    {
                        screenInventory.onTouchInventory(e, eventType, false);
                    }
                    else if (screenType.Equals("combatInventory"))
                    {
                        screenInventory.onTouchInventory(e, eventType, true);
                    }
                    else if (screenType.Equals("itemSelector"))
                    {
                        screenItemSelector.onTouchItemSelector(e, eventType);
                    }
                    else if (screenType.Equals("portraitSelector"))
                    {
                        screenPortraitSelector.onTouchPortraitSelector(e, eventType);
                    }
                    else if (screenType.Equals("tokenSelector"))
                    {
                        screenTokenSelector.onTouchTokenSelector(e, eventType);
                    }
                    else if (screenType.Equals("pcSelector"))
                    {
                        screenPcSelector.onTouchPcSelector(e, eventType);
                    }
                    else if (screenType.Equals("journal"))
                    {
                        screenJournal.onTouchJournal(e, eventType);
                    }
                    else if (screenType.Equals("shop"))
                    {
                        screenShop.onTouchShop(e, eventType);
                    }
                    else if (screenType.Equals("combat"))
                    {
                        screenCombat.onTouchCombat(e, eventType);
                    }
                    else if (screenType.Equals("combatCast"))
                    {
                        screenCastSelector.onTouchCastSelector(e, eventType, true);
                    }
                    else if (screenType.Equals("mainMapCast"))
                    {
                        screenCastSelector.onTouchCastSelector(e, eventType, false);
                    }
                    else if (screenType.Equals("convo"))
                    {
                        screenConvo.onTouchConvo(e, eventType);
                    }
                    else if (screenType.Equals("partyBuild"))
                    {
                        screenPartyBuild.onTouchPartyBuild(e, eventType);
                    }
                    else if (screenType.Equals("partyRoster"))
                    {
                        screenPartyRoster.onTouchPartyRoster(e, eventType);
                    }
                    //this.Invalidate();
                }
            }
            catch (Exception ex) 
            {
                errorLog(ex.ToString());   		
            }		
        }

        public void onKeyboardEvent(Keys keyData)
        {
            try
            {
                if (touchEnabled)
                {
                    if (keyData == Keys.H)
                    {
                        if (showHotKeys) { showHotKeys = false; }
                        else { showHotKeys = true; }
                    }
                    if (screenType.Equals("main"))
                    {
                        screenMainMap.onKeyUp(keyData);
                        Render();
                        //this.Invalidate();
                    }
                    else if (screenType.Equals("combat"))
                    {
                        screenCombat.onKeyUp(keyData);
                        Render();
                        //this.Invalidate();
                    }
                    else if (screenType.Equals("convo"))
                    {
                        screenConvo.onKeyUp(keyData);
                        Render();
                        //this.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                errorLog(ex.ToString());
            }
        }        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            onKeyboardEvent(keyData);
                
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //ON FORM CLOSING
        private void GameView_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dlg = IBMessageBox.Show(this, "Are you sure you wish to exit?", enumMessageButton.YesNo);
            if (dlg == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            if (dlg == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        private void GameView_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        public void errorLog(string text)
        {
            if (mainDirectory == null) 
            { 
                mainDirectory = Directory.GetCurrentDirectory(); 
            }
            using (StreamWriter writer = new StreamWriter(mainDirectory + "//IB2ErrorLog.txt", true))
            {
                writer.Write(DateTime.Now + ": ");
                writer.WriteLine(text);
            }
        }
    }
}
