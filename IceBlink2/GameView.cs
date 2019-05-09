using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Timers;
//using System.Drawing.Drawing2D;
using System.IO;
//using System.Linq;
//using System.Text;
using System.Windows.Forms;
//using System.Runtime.InteropServices;
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
using Message = System.Windows.Forms.Message;
using Color = System.Drawing.Color;
//using Point = System.Drawing.Point;
using RectangleF = System.Drawing.RectangleF;
using Rectangle = System.Drawing.Rectangle;
using System.Diagnostics;

namespace IceBlink2
{
    public partial class GameView : IBForm
    {
        //this class is handled differently than Android version
        public int elapsed = 0;
        public int elapsed2 = 0;
        public float screenDensity;
        public int screenWidth;
        public int screenHeight;
        public int squareSizeInPixels = 100;
        public int squareSize; //in dp (squareSizeInPixels * screenDensity)
        public int pS; // = squareSize / 10 ... used for small UI and text location adjustments based on squaresize
        public int squaresInWidth = 19;
        public int squaresInHeight = 10;
        public int ibbwidthL = 340;
        public int ibbwidthR = 100;
        public int ibbheight = 100;
        public int ibpwidth = 110;
        public int ibpheight = 170;
        public int playerOffset = 4;
        public int playerOffsetX = 9;
        //changed playerOffsetY from 4 to 5 in order to show map full screen (i.e. underneath UI, too)
        public int playerOffsetY = 5;
        public int oXshift = 0;
        public int oYshift = 35;
        public string mainDirectory;
        public bool showHotKeys = false;
        public int mousePositionX = 0;
        public int mousePositionY = 0;
        public bool moveTimerRuns = false;
        public float moveTimerCounter = 0;
        public int standardTokenSize = 100;


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
        public string screenType = "splash"; //launcher, title, moreGames, main, party, inventory, combatInventory, shop, journal, combat, combatCast, convo
        public AnimationState animationState = AnimationState.None;
        public int triggerIndex = 0;
        public int triggerPropIndex = 0;

        public IB2HtmlLogBox log;
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
        public ScreenTraitUseSelector screenTraitUseSelector;
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
        public WMPLib.WindowsMediaPlayer weatherSounds1;
        public WMPLib.WindowsMediaPlayer weatherSounds2;
        public WMPLib.WindowsMediaPlayer weatherSounds3;
       
        public SoundPlayer soundPlayer = new SoundPlayer();
        public Dictionary<string, Stream> oSoundStreams = new Dictionary<string, Stream>();
        public System.Media.SoundPlayer playerButtonEnter = new System.Media.SoundPlayer();
        public System.Media.SoundPlayer playerButtonClick = new System.Media.SoundPlayer();
       
        public string currentMainMusic = "";
        public string currentAmbientMusic = "";
        public string currentCombatMusic = "";

        //timers
        public Timer gameTimer = new Timer();
        //public Timer moveTimer = new Timer();
        public Stopwatch gameTimerStopwatch = new Stopwatch();
        public long previousTime = 0;
        public long previousTime2 = 0;
        public bool stillProcessingGameLoop = false;
        public float fps = 0;
        public int reportFPScount = 0;

        public Timer animationTimer = new Timer();
        //public Timer floatyTextTimer = new Timer();
        //public Timer floatyTextMainMapTimer = new Timer();
        public Timer areaMusicTimer = new Timer();
        public Timer areaSoundsTimer = new Timer();
        public Timer weatherSounds1Timer = new Timer();
        public Timer weatherSounds2Timer = new Timer();
        public Timer weatherSounds3Timer = new Timer();
        
        public float floatPixMovedPerTick = 4f;
        public int realTimeTimerMilliSecondsEllapsed = 0;
        public int smoothMoveTimerLengthInMilliSeconds = 16;
        public int fullScreenEffectTimerMilliSecondsElapsedRain = 0;
        public int fullScreenEffectTimerMilliSecondsElapsedSnow = 0;
        public int fullScreenEffectTimerMilliSecondsElapsedSandstorm = 0;
        public int fullScreenEffectTimerMilliSecondsElapsedClouds = 0;
        public string rainType = "";
        public string cloudType = "";
        public string fogType = "";
        public string snowType = "";
        public string sandstormType = "";
        public int smoothMoveCounter = 0;
        public bool useLargeLayout = true;
        public Timer aTimer = new Timer();

        public GameView()
        {
            InitializeComponent();

            cc = new CommonCode(this);
            mod = new Module();

            //timer for ouer scrolling when held system
            //it will simulate clicks in reglaur intervalls
            //timer is started on key down and stopped on key down
            //the interval will dterine teh frequnecy
            //nebaled might start and stop it (or start/stop), reset with enabled, too?
            //interval lenght will be crucial to get the speed feel right
            //will be curcial to get it feel right

            //public Timer aTimer = new Timer();
            aTimer.Tick += new System.EventHandler(onTimedEvent);
            aTimer.Interval = 1;
            aTimer.Enabled = false;

            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GameView_MouseWheel);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameView_onKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GameView_onKeyUp);
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
            float sqrW = (float)screenWidth / (squaresInWidth + 2f/10f);
            float sqrH = (float)screenHeight / (squaresInHeight + 3f/10f);
            if (sqrW > sqrH)
            {
                squareSize = (int)(sqrH);
            }
            else
            {
                squareSize = (int)(sqrW);
            }
            if ((squareSize >= 99) && (squareSize < 105))
            {
                squareSize = 100;
            }
            //makes it a straight number in any case
            //better for working with half fractions of it (two half ints always form a complete whole)
            //squareSize = 2 * (int)(squareSize / 2f);
            screenDensity = (float)squareSize / (float)squareSizeInPixels;
            oXshift = (screenWidth - (squareSize * squaresInWidth)) / 2;

            pS = squareSize / 10; //used for small UI and text location adjustments based on squaresize for consistent look on all devices/screen resolutions

            InitializeRenderer(); //uncomment this for DIRECT2D ADDITIONS

            //CREATES A FONTFAMILY
            ResetGDIFont();
            ResetDirect2DFont();
            
            animationTimer.Tick += new System.EventHandler(this.AnimationTimer_Tick);

            log = new IB2HtmlLogBox(this);
            log.numberOfLinesToShow = 22;
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
            gameTimer.Interval = 1; //~60 fps
            gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick);
            gameTimerStopwatch.Start();
            previousTime = gameTimerStopwatch.ElapsedMilliseconds;
            gameTimer.Start();

            //moveTimer.Interval = 375;
            //moveTimer.Enabled = false;
            //moveTimer.Tick += new System.EventHandler(this.moveTimer_Tick);
            //moveTimer.Tick += new System.EventHandler(this.gameTimer_Tick);
            //moveTimerStopwatch.Start();
            //previousTime = gameTimerStopwatch.ElapsedMilliseconds;
            //moveTimer.Start();


        }

        public void onTimedEvent(object source, EventArgs e)
        {
            //check for diretion (set direction on button down)
            //move code for each specific direction
            //int zuz = 0;
            //else if (gv.mod.currentArea.GetBlocked(gv.mod.PlayerLocationX + 1, gv.mod.PlayerLocationY, gv.mod.PlayerLocationX, gv.mod.PlayerLocationY, gv.mod.PlayerLastLocationX + gv.mod.lastXadjustment, gv.mod.PlayerLastLocationY + gv.mod.lastYadjustment) == false || gv.mod.currentArea.isOverviewMap)

            if (this.mod.scrollingDirection == "up")
            {
              

                    mod.doTriggerInspiteOfScrolling = false;
                    bool blockMoveBecausOfCurrentScrolling = false;

                    if (mod.useScrollingSystem)
                    {
                        if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                        {
                            blockMoveBecausOfCurrentScrolling = true;
                        }
                    }

                    blockMoveBecausOfCurrentScrolling = false;

                    if (!blockMoveBecausOfCurrentScrolling)
                    {
                        if (mod.useScrollingSystem)
                        {
                            //single press
                            if (!mod.isScrollingNow)
                            {
                                mod.isScrollingNow = true;
                                //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                                mod.scrollingTimer = 100;
                                mod.scrollingDirection = "up";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goNorth();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY - 1, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                    mod.breakActiveSearch = false;
                                    //gv.mod.wasJustCalled = false;
                                    //if (!gv.mod.wasJustCalled)
                                    //{
                                    //if (gv.screenType == "main")
                                    //{
                                    screenMainMap.moveUp(true);
                                    //}
                                    //gv.mod.wasJustCalled = true;
                                    //}
                                //}
                            }
                            }
                            //continued press
                            //else if (moveDelay2())
                            else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                            {
                                mod.isScrollingNow = true;
                                mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingOverhang2 = 0;
                                mod.scrollingDirection = "up";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goNorth();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY - 1, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                    mod.breakActiveSearch = false;
                                    //gv.mod.wasJustCalled = false;
                                    //if (!gv.mod.wasJustCalled)
                                    //{
                                    //if (gv.screenType == "main")
                                    //{
                                    screenMainMap.moveUp(true);
                                    //}
                                    //gv.mod.wasJustCalled = true;
                                    //}
                                //}
                            }
                            }
                        }
                    }

                
            }
            //down
            if (this.mod.scrollingDirection == "down")
            {
                        mod.doTriggerInspiteOfScrolling = false;
                        bool blockMoveBecausOfCurrentScrolling = false;

                        if (mod.useScrollingSystem)
                        {
                            if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                            {
                                blockMoveBecausOfCurrentScrolling = true;
                            }
                        }

                        blockMoveBecausOfCurrentScrolling = false;

                        if (!blockMoveBecausOfCurrentScrolling)
                        {
                            if (mod.useScrollingSystem)
                            {
                                //single press
                                if (!mod.isScrollingNow)
                                {
                                    mod.isScrollingNow = true;
                                    //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                                    mod.scrollingTimer = 100;
                                    mod.scrollingDirection = "down";
                                    mod.doTriggerInspiteOfScrolling = true;
                                    bool isTransition = cc.goSouth();
                                    if (!isTransition)
                                    {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY + 1, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                    mod.breakActiveSearch = false;
                                    //gv.mod.wasJustCalled = false;
                                    //if (!gv.mod.wasJustCalled)
                                    //{
                                    //if (gv.screenType == "main")
                                    //{
                                    screenMainMap.moveDown(true);
                                    //}
                                    //gv.mod.wasJustCalled = true;
                                    //}
                                //}
                                    }
                                }
                                //continued press
                                //else if (moveDelay2())
                                else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                                {
                                    mod.isScrollingNow = true;
                                    mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                    mod.scrollingOverhang2 = 0;
                                    mod.scrollingDirection = "down";
                                    mod.doTriggerInspiteOfScrolling = true;
                                    bool isTransition = cc.goSouth();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY + 1, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                    mod.breakActiveSearch = false;
                                    //gv.mod.wasJustCalled = false;
                                    //if (!gv.mod.wasJustCalled)
                                    //{
                                    //if (gv.screenType == "main")
                                    //{
                                    screenMainMap.moveDown(true);
                                    //}
                                    //gv.mod.wasJustCalled = true;
                                    //}
                                //}
                            }
                                }
                            }
                        
                    
                }
            }
            //left
            if (this.mod.scrollingDirection == "left")
            {
               


                    mod.doTriggerInspiteOfScrolling = false;
                    bool blockMoveBecausOfCurrentScrolling = false;

                    if (mod.useScrollingSystem)
                    {
                        if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                        {
                            blockMoveBecausOfCurrentScrolling = true;
                        }
                    }

                    blockMoveBecausOfCurrentScrolling = false;

                    if (!blockMoveBecausOfCurrentScrolling)
                    {
                        if (mod.useScrollingSystem)
                        {
                            //single press
                            if (!mod.isScrollingNow)
                            {
                                mod.isScrollingNow = true;
                                //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                                mod.scrollingTimer = 100;
                                mod.scrollingDirection = "left";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goWest();
                                if (!isTransition)
                                {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX - 1, mod.PlayerLocationY, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                    mod.breakActiveSearch = false;
                                    //gv.mod.wasJustCalled = false;
                                    //if (!gv.mod.wasJustCalled)
                                    //{
                                    //if (gv.screenType == "main")
                                    //{
                                    screenMainMap.moveLeft(true);
                                    //}
                                    //gv.mod.wasJustCalled = true;
                                    //}
                                //}
                                }
                            }
                            //continued press
                            //else if (moveDelay2())
                            else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                            {
                                mod.isScrollingNow = true;
                                mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingOverhang2 = 0;
                                mod.scrollingDirection = "left";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goWest();
                                if (!isTransition)
                                {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX - 1, mod.PlayerLocationY, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                    mod.breakActiveSearch = false;
                                    //gv.mod.wasJustCalled = false;
                                    //if (!gv.mod.wasJustCalled)
                                    //{
                                    //if (gv.screenType == "main")
                                    //{
                                    screenMainMap.moveLeft(true);
                                    //}
                                    //gv.mod.wasJustCalled = true;
                                    //}
                                //}
                                }
                            }
                        }
                    
                }
            }
            //right
            if (this.mod.scrollingDirection == "right")
            {
               
                    mod.doTriggerInspiteOfScrolling = false;
                    bool blockMoveBecausOfCurrentScrolling = false;

                    if (mod.useScrollingSystem)
                    {
                        if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                        {
                            blockMoveBecausOfCurrentScrolling = true;
                        }
                    }

                    blockMoveBecausOfCurrentScrolling = false;

                    if (!blockMoveBecausOfCurrentScrolling)
                    {
                        if (mod.useScrollingSystem)
                        {
                            //single press
                            if (!mod.isScrollingNow)
                            {
                                mod.isScrollingNow = true;
                                //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                                mod.scrollingTimer = 100;
                                mod.scrollingDirection = "right";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goEast();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX + 1, mod.PlayerLocationY, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                    mod.breakActiveSearch = false;
                                    //gv.mod.wasJustCalled = false;
                                    //if (!gv.mod.wasJustCalled)
                                    //{
                                    //if (gv.screenType == "main")
                                    //{
                                    screenMainMap.moveRight(true);
                                    //}
                                    //gv.mod.wasJustCalled = true;
                                    //}
                                //}
                            }
                            }
                            //continued press
                            //else if (moveDelay2())
                            else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                            {
                                mod.isScrollingNow = true;
                                mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                mod.scrollingOverhang2 = 0;
                                mod.scrollingDirection = "right";
                                mod.doTriggerInspiteOfScrolling = true;
                                bool isTransition = cc.goEast();
                            if (!isTransition)
                            {
                                //if (mod.currentArea.GetBlocked(mod.PlayerLocationX + 1, mod.PlayerLocationY, mod.PlayerLocationX, mod.PlayerLocationY, mod.PlayerLastLocationX + mod.lastXadjustment, mod.PlayerLastLocationY + mod.lastYadjustment) == false || mod.currentArea.isOverviewMap)
                                //{
                                    mod.breakActiveSearch = false;
                                    //gv.mod.wasJustCalled = false;
                                    //if (!gv.mod.wasJustCalled)
                                    //{
                                    //if (gv.screenType == "main")
                                    //{
                                    screenMainMap.moveRight(true);
                                    //}
                                    //gv.mod.wasJustCalled = true;
                                    //}
                                //}
                            }
                            }
                        }
                    }
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
            screenTraitUseSelector = new ScreenTraitUseSelector(mod, this);
            screenConvo = new ScreenConvo(mod, this);		    
		    screenMainMap = new ScreenMainMap(mod, this);
            screenCombat = new ScreenCombat(mod, this);
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

            cc.downStairFlankShadowLeft = cc.LoadBitmap("downStairFlankShadowLeft");
            cc.downStairShadow = cc.LoadBitmap("downStairShadow");
            cc.downStairFlankShadowRight = cc.LoadBitmap("downStairFlankShadowRight");
            cc.bridgeShadow = cc.LoadBitmap("bridgeShadow");
            cc.highlight90 = cc.LoadBitmap("highlight90");
            cc.highlightGreen = cc.LoadBitmap("highlightGreen");
            cc.leftCurtain = cc.LoadBitmap("leftCurtain");
            cc.rightCurtain = cc.LoadBitmap("rightCurtain");
            cc.longShadow = cc.LoadBitmap("longShadow");
            cc.longShadowCorner = cc.LoadBitmap("longShadowCorner");
            cc.shortShadow = cc.LoadBitmap("shortShadow");
            cc.shortShadowCorner = cc.LoadBitmap("shortShadowCorner");
            cc.longShadowCornerHalf = cc.LoadBitmap("longShadowCornerHalf");
            cc.longShadowCornerHalfMirror = cc.LoadBitmap("longShadowCornerHalfMirror");
            cc.shortShadowCorner2 = cc.LoadBitmap("shortShadowCorner2");
            cc.smallStairNEMirror = cc.LoadBitmap("smallStairNEMirror");
            cc.smallStairNENormal = cc.LoadBitmap("smallStairNENormal");
            cc.corner3 = cc.LoadBitmap("corner3");
            cc.entranceLightNorth2 = cc.LoadBitmap("entranceLightNorth2");

            cc.tooHigh = cc.LoadBitmap("tooHigh");
            cc.tooDeep = cc.LoadBitmap("tooDeep");

            cc.btnIni = cc.LoadBitmap("btn_ini");
            cc.btnIniGlow = cc.LoadBitmap("btn_ini_glow");
            cc.walkPass = cc.LoadBitmap("walk_pass");
            cc.walkBlocked = cc.LoadBitmap("walk_block");
            //gehörtdiewelt
            cc.encounter_indicator = cc.LoadBitmap("encounter_indicator");
            cc.mandatory_conversation_indicator = cc.LoadBitmap("mandatory_conversation_indicator");
            cc.optional_conversation_indicator = cc.LoadBitmap("optional_conversation_indicator");
            cc.challengeHidden = cc.LoadBitmap("challengeHidden");
            cc.challengeSkull = cc.LoadBitmap("challengeSkull");
            cc.isChasingSymbol = cc.LoadBitmap("chasing");

            cc.losBlocked = cc.LoadBitmap("los_block");
            cc.black_tile = cc.LoadBitmap("black_tile");
            cc.black_tile_NE = cc.LoadBitmap("black_tile_NE");
            cc.black_tile_NW = cc.LoadBitmap("black_tile_NW");
            cc.black_tile_SE = cc.LoadBitmap("black_tile_SW");
            cc.black_tile_SW = cc.LoadBitmap("black_tile_SE");
            cc.black_tile2 = cc.LoadBitmap("black_tile2");
            cc.turn_marker = cc.LoadBitmap("turn_marker");
            cc.pc_dead = cc.LoadBitmap("pc_dead");
            cc.pc_stealth = cc.LoadBitmap("pc_stealth");
            cc.offScreen = cc.LoadBitmap("offScreen");
            cc.offScreenBlack = cc.LoadBitmap("offScreenBlack");
            cc.offScreen5 = cc.LoadBitmap("offScreen5");
            cc.black_tile4 = cc.LoadBitmap("black_tile4");
            cc.black_tile5 = cc.LoadBitmap("black_tile5");
            cc.offScreen6 = cc.LoadBitmap("offScreen6");
            cc.offScreen7 = cc.LoadBitmap("offScreen7");
            cc.offScreenTrans = cc.LoadBitmap("offScreenTrans");
            cc.death_fx = cc.LoadBitmap("death_fx");
            cc.hitSymbol = cc.LoadBitmap("hit_symbol");
            cc.missSymbol = cc.LoadBitmap("miss_symbol");
            cc.highlight_green = cc.LoadBitmap("highlight_green");
            cc.highlight_red = cc.LoadBitmap("highlight_red");
            cc.tint_dawn = cc.LoadBitmap("tint_dawn");
            cc.tint_sunrise = cc.LoadBitmap("tint_sunrise");
            cc.tint_sunset = cc.LoadBitmap("tint_sunset");
            cc.tint_dusk = cc.LoadBitmap("tint_dusk");
            cc.tint_night = cc.LoadBitmap("tint_night");
            cc.night_tile_NW = cc.LoadBitmap("night_tile_NW");
            cc.night_tile_NE = cc.LoadBitmap("night_tile_NE");
            cc.night_tile_SE = cc.LoadBitmap("night_tile_SW");
            cc.night_tile_SW = cc.LoadBitmap("night_tile_SE");
            cc.light_torch = cc.LoadBitmap("light_torch");
            cc.prp_lightYellow = cc.LoadBitmap("prp_lightYellow");
            cc.prp_lightRed = cc.LoadBitmap("prp_lightRed");
            cc.prp_lightGreen = cc.LoadBitmap("prp_lightGreen");
            cc.prp_lightBlue = cc.LoadBitmap("prp_lightBlue");
            cc.prp_lightPurple = cc.LoadBitmap("prp_lightPurple");
            cc.prp_lightOrange = cc.LoadBitmap("prp_lightOrange");

            cc.light_torchOLD = cc.LoadBitmap("light_torchOLD");
            //off for now
            //cc.tint_rain = cc.LoadBitmap("tint_rain");
            cc.ui_portrait_frame = cc.LoadBitmap("ui_portrait_frame");
            cc.ui_bg_fullscreen = cc.LoadBitmap("ui_bg_fullscreen");
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
            log.tagStack.Clear();
            log.logLinesList.Clear();
            log.currentTopLineIndex = 0;
            if (mod.useSmoothMovement == true)
            {
                //16 milliseconds a tick, equals - theoretically - about 60 FPS
                
                //these are the pix moved per tick, designed so that a square is traversed within realTimeTimerLengthInMilliSeconds 
                //update: actually as the 60 FPS are never reached, we will see little stops between prop moves with realtime timer on
                floatPixMovedPerTick = ((float)squareSize / 90f) * mod.allAnimationSpeedMultiplier;
                //IBMessageBox.Show(this, "floatPixMovedPerTick after first is:" + floatPixMovedPerTick.ToString());
                //due to a mistake of mine 4 pix were moved always beforehand, trying a dynamically calculated average of 7.5 pix now, increases speed by 90%
                //floatPixMovedPerTick = floatPixMovedPerTick / (((float)mod.realTimeTimerLengthInMilliSeconds / 1000f * 2f / 3f)) * 6.675f;
                floatPixMovedPerTick = floatPixMovedPerTick / ((1500f / 1000f * 2f / 3f)) * 6.675f;
                //IBMessageBox.Show(this, "floatPixMovedPerTick after second is is:" + floatPixMovedPerTick.ToString());
                //IBMessageBox.Show(this, "real time timer length is:" + realTimeTimerLengthInMilliSeconds.ToString());
            }

            //reset fonts
            ResetGDIFont();
            ResetDirect2DFont();
            //reset log number of lines based on the value from the Module's mod file
            log.numberOfLinesToShow = mod.logNumberOfLines;
            //log.numberOfLinesToShow = 24;

            mod.debugMode = false;
		    mod.loadAreas(this);
		    //mod.setCurrentArea(mod.startingArea, this);
            bool foundArea = mod.setCurrentArea(mod.startingArea, this);
            if (!foundArea)
            {
                MessageBox.Show("Area: " + mod.startingArea + " does not exist in the module...check the spelling or make sure your are pointing to the correct starting area that you intended");
            }

            mod.PlayerLocationX = mod.startingPlayerPositionX;
		    mod.PlayerLocationY = mod.startingPlayerPositionY;
		    cc.title = cc.LoadBitmap("title");
            LoadStandardImages();
		    cc.LoadRaces();
		    cc.LoadPlayerClasses();
		    cc.LoadItems();
		    cc.LoadContainers();
		    cc.LoadShops();
		    cc.LoadEffects();
		    cc.LoadSpells();
		    cc.LoadTraits();
            cc.LoadFactions();
            cc.LoadWeathers();
            cc.LoadWeatherEffects();
		    cc.LoadCreatures();
		    cc.LoadEncounters();
		    cc.LoadJournal();

            //Add new methods for laoding entTiles and Tiles diretcly from the module file
            //already in via LaodEncounters();
            //

            //hurghj
            //if (!mod.useAllTileSystem)
            //{
            //    cc.LoadTileBitmapList();
            //}
				
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
            cc.setPanelsStart();
		    cc.setControlsStart();
            cc.setPortraitsStart();
		    cc.setToggleButtonsStart();
            //TODO log.ResetLogBoxUiBitmaps();

		    createScreens();
		    initializeSounds();
		
		    cc.LoadTestParty();
		
		    //load all the message box helps/tutorials
            cc.stringStrength = cc.loadTextToString("MessageStrengthExplanation.txt");
            cc.stringDexterity = cc.loadTextToString("MessageDexterityExplanation.txt");
            cc.stringConstitution = cc.loadTextToString("MessageConstitutionExplanation.txt");
            cc.stringIntelligence = cc.loadTextToString("MessageIntelligenceExplanation.txt");
            cc.stringWisdom = cc.loadTextToString("MessageWisdomExplanation.txt");
            cc.stringCharisma = cc.loadTextToString("MessageCharismaExplanation.txt");
            cc.stringBeginnersGuide = cc.loadTextToString("MessageBeginnersGuide.txt");
		    cc.stringPlayersGuide = cc.loadTextToString("MessagePlayersGuide.txt");
		    cc.stringPcCreation = cc.loadTextToString("MessagePcCreation.txt");
		    cc.stringMessageCombat = cc.loadTextToString("MessageCombat.txt");
		    cc.stringMessageInventory = cc.loadTextToString("MessageInventory.txt");
		    cc.stringMessageParty = cc.loadTextToString("MessageParty.txt");
		    cc.stringMessageMainMap = cc.loadTextToString("MessageMainMap.txt");
	    }

        public void ResetGDIFont()
        {
            if (File.Exists(mainDirectory + "\\modules\\" + mod.moduleName + "\\fonts\\" + mod.fontFilename))
            {
                family = LoadFontFamily(mainDirectory + "\\modules\\" + mod.moduleName + "\\fonts\\" + mod.fontFilename, out myFonts);
            }
            else
            {
                family = LoadFontFamily(mainDirectory + "\\default\\NewModule\\fonts\\Metamorphous-Regular.ttf", out myFonts);
            }
            float multiplr = (float)squareSize / 100.0f;
            drawFontLarge = new Font(family, 24.0f * multiplr);
            drawFontReg = new Font(family, 20.0f * multiplr);
            drawFontSmall = new Font(family, 16.0f * multiplr);
            drawFontLargeHeight = 32.0f * multiplr * mod.fontD2DScaleMultiplier;
            drawFontRegHeight = 26.0f * multiplr * mod.fontD2DScaleMultiplier;
            drawFontSmallHeight = 20.0f * multiplr * mod.fontD2DScaleMultiplier;
        }
        private FontFamily LoadFontFamily(string fileName, out PrivateFontCollection _myFonts)
        {
            //IN MEMORY _myFonts point to the myFonts created in the load event.
            _myFonts = new PrivateFontCollection();//here is where we assing memory space to myFonts 
            _myFonts.AddFontFile(fileName);//we add the full path of the ttf file
            return _myFonts.Families[0];//returns the family object as usual.
        }
        private void ResetDirect2DFont()
        {
            string folderPath = "";
            if (Directory.Exists(mainDirectory + "\\modules\\" + mod.moduleName + "\\fonts"))
            {
                folderPath = mainDirectory + "\\modules\\" + mod.moduleName + "\\fonts";
            }
            else
            {
                folderPath = mainDirectory + "\\default\\NewModule\\fonts";
            }
            CurrentResourceFontLoader = new ResourceFontLoader(factoryDWrite, folderPath);
            CurrentFontCollection = new SharpDX.DirectWrite.FontCollection(factoryDWrite, CurrentResourceFontLoader, CurrentResourceFontLoader.Key);
            FontFamilyName = mod.fontName;
        }

        #region Area Music/Sounds
        public void setupMusicPlayers()
        {
            try
            {
                areaMusic = new WMPLib.WindowsMediaPlayer();
                areaMusic.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(AreaMusic_PlayStateChange);
                areaMusic.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                areaMusic.settings.setMode("Loop", true);
                areaMusic.settings.volume = 50;

                areaSounds = new WMPLib.WindowsMediaPlayer();
                areaSounds.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(AreaSounds_PlayStateChange);
                areaSounds.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                areaSounds.settings.setMode("Loop", true);

                //for winds
                weatherSounds1 = new WMPLib.WindowsMediaPlayer();
                //weatherSounds1.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(WeatherSounds1_PlayStateChange);
                weatherSounds1.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                weatherSounds1.settings.setMode("Loop", true);
                weatherSounds1.settings.volume = 50;
                //for rain
                weatherSounds2 = new WMPLib.WindowsMediaPlayer();
                //weatherSounds2.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(WeatherSounds2_PlayStateChange);
                weatherSounds2.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                weatherSounds2.settings.setMode("Loop", true);
                weatherSounds2.settings.volume = 50;
                //for lightning
                weatherSounds3 = new WMPLib.WindowsMediaPlayer();
                weatherSounds3.settings.volume = 50;
                //channel 3 is for lightning, no loop needed
                //weatherSounds3.settings.setMode("Loop", true);

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
            if (type.Equals("doAnimation"))
            {
                animationTimer.Enabled = true;
                if (delay < 1)
                {
                    delay = 1;
                }
                animationTimer.Interval = delay;
                //animationTimer.Interval = 1;
                animationTimer.Start();
            }
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if ((!screenCombat.blockAnimationBridge) || (!mod.useCombatSmoothMovement))
            {
                animationTimer.Enabled = false;
                animationTimer.Stop();
                screenCombat.doAnimationController();
            }
        }
        /*
        private void moveTimer_Tick(object sender, EventArgs e)
        {
            int x = mousePositionX - (int)(squareSize*4f/10f);
            int y = mousePositionY - (int)(squareSize*55f/100f);
            int gridX = x / squareSize;
            int gridY = y / squareSize;
            int actualx = mod.PlayerLocationX + (gridX - playerOffsetX);
            int actualy = mod.PlayerLocationY + (gridY - playerOffsetY);

            if (mod.PlayerLocationX == actualx && mod.PlayerLocationY - 1 == actualy)
            {
                bool isTransition = cc.goNorth();
                if (!isTransition)
                {
                    if (mod.PlayerLocationY > 0)
                    {
                        if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY - 1) == false)
                        {
                            mod.PlayerLastLocationX = mod.PlayerLocationX;
                            mod.PlayerLastLocationY = mod.PlayerLocationY;
                            mod.PlayerLocationY--;
                            cc.doUpdate();
                        }
                    }
                }
            }
            else if (mod.PlayerLocationX == actualx && mod.PlayerLocationY + 1 == actualy)
            {

                bool isTransition = cc.goSouth();
                if (!isTransition)
                {
                    int mapheight = mod.currentArea.MapSizeY;
                    if (mod.PlayerLocationY < (mapheight - 1))
                    {
                        if (mod.currentArea.GetBlocked(mod.PlayerLocationX, mod.PlayerLocationY + 1) == false)
                        {
                            mod.PlayerLastLocationX = mod.PlayerLocationX;
                            mod.PlayerLastLocationY = mod.PlayerLocationY;
                            mod.PlayerLocationY++;
                            cc.doUpdate();
                        }
                    }
                }
            }
            else if (mod.PlayerLocationX - 1 == actualx && mod.PlayerLocationY == actualy)
            {
                bool isTransition = cc.goWest();
                if (!isTransition)
                {
                    if (mod.PlayerLocationX > 0)
                    {
                        if (mod.currentArea.GetBlocked(mod.PlayerLocationX - 1, mod.PlayerLocationY) == false)
                        {
                            mod.PlayerLastLocationX = mod.PlayerLocationX;
                            mod.PlayerLastLocationY = mod.PlayerLocationY;
                            mod.PlayerLocationX--;
                            foreach (Player pc in mod.playerList)
                            {
                                if (!pc.combatFacingLeft)
                                {
                                    pc.combatFacingLeft = true;
                                }
                            }
                            cc.doUpdate();
                        }
                    }
                }
            }
            else if (mod.PlayerLocationX + 1 == actualx && mod.PlayerLocationY == actualy)
            {
                bool isTransition = cc.goEast();
                if (!isTransition)
                {
                    int mapwidth = mod.currentArea.MapSizeX;
                    if (mod.PlayerLocationX < (mapwidth - 1))
                    {
                        if (mod.currentArea.GetBlocked(mod.PlayerLocationX + 1, mod.PlayerLocationY) == false)
                        {
                            mod.PlayerLastLocationX = mod.PlayerLocationX;
                            mod.PlayerLastLocationY = mod.PlayerLocationY;
                            mod.PlayerLocationX++;
                            foreach (Player pc in mod.playerList)
                            {
                                if (pc.combatFacingLeft)
                                {
                                    pc.combatFacingLeft = false;
                                }
                            }
                            cc.doUpdate();
                        }
                    }
                }
            }

        }
        */
        
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (!stillProcessingGameLoop)
            {
                if (this.screenType != "main")
                {
                    this.aTimer.Stop();
                }
                stillProcessingGameLoop = true; //starting the game loop so do not allow another tick call to run until finished with this tick call.
                long current = gameTimerStopwatch.ElapsedMilliseconds; //get the current total amount of ms since the game launched
                elapsed = (int)(current - previousTime); //calculate the total ms elapsed since the last time through the game loop
                /*
                while (elapsed < 16)
                {
                    current = gameTimerStopwatch.ElapsedMilliseconds;
                    elapsed = (int)(current - previousTime);
                }
                */
                //if (elapsed > 50)
                //{
                    //int i = 0;
                //}
                //mod.scrollingOverhang = mod.scrollingOverhang + 3f*(elapsed/20f);
                //if (mod.scrollingOverhang > 0)
                //{
                    //mod.scrollingOverhang = 0;
                //}
                Update(elapsed); //runs AI and physics
                Render(elapsed); //draw the screen frame
                if (reportFPScount >= 10)
                {
                    reportFPScount = 0;
                    fps = 1000 / (current - previousTime);
                }
                reportFPScount++;
                previousTime = current; //remember the current time at the beginning of this tick call for the next time through the game loop to calculate elapsed ti
                stillProcessingGameLoop = false; //finished game loop so okay to let the next tick call enter the game loop      
            }
        }
        private void Update(int elapsed)
        {
            
            //iterate through spriteList and handle any sprite location and animation frame calculations
            if (screenType.Equals("main"))
            {
                screenMainMap.Update(elapsed);
            }
            else if (screenType.Equals("combat"))
            {
                screenCombat.Update(elapsed);
            }
      
        }

        //DRAW ROUTINES
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
            if (text == "NA")
            {
                DrawText(text, xLoc, yLoc, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, 1.0f, SharpDX.Color.Red, false);
            }
            else if (text.Contains("Points available"))
            {
                DrawText(text, xLoc, yLoc, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, 1.0f, SharpDX.Color.Lime, false);
            }
            else
            {
                DrawText(text, xLoc, yLoc, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, 1.0f, SharpDX.Color.White, false);
            }
        }
        public void DrawText(string text, float x, float y, FontWeight fw, SharpDX.DirectWrite.FontStyle fs, SharpDX.Color fontColor)
        {
            DrawText(text, x, y, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, 1.0f, fontColor, false);
        }
        public void DrawText(string text, float xLoc, float yLoc, float scaler, SharpDX.Color fontColor)
        {
            //tr
            DrawText(text, xLoc, yLoc, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, scaler, fontColor, false);
        }
        public void DrawText(string text, IbRect rect, float scaler, SharpDX.Color fontColor)
        {
            DrawText(text, rect, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, scaler, fontColor);
        }
        public void DrawTextLeft(string text, IbRect rect, float scaler, SharpDX.Color fontColor)
        {
            DrawTextLeft(text, rect, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, scaler, fontColor);
        }
        public void DrawTextCenter(string text, IbRect rect, float scaler, SharpDX.Color fontColor)
        {
            DrawTextCenter(text, rect, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, scaler, fontColor);
        }
        public void DrawTextEndBound(string text, IbRect rect, float scaler, SharpDX.Color fontColor)
        {
            DrawTextEndBound(text, rect, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, scaler, fontColor);
        }

        public void DrawTextEndBound(string text, IbRect rect, FontWeight fw, SharpDX.DirectWrite.FontStyle fs, float scaler, SharpDX.Color fontColor)
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
            //RectangleF rectF = new RectangleF(rect.Left, rect.Top + oYshift, rect.Width, rect.Height);
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, fontColor))
            {
                textFormat = new TextFormat(factoryDWrite, FontFamilyName, CurrentFontCollection, fw, fs, FontStretch.Normal, thisFontHeight) { TextAlignment = TextAlignment.Trailing, ParagraphAlignment = ParagraphAlignment.Near };
                textLayout = new TextLayout(factoryDWrite, text, textFormat, rect.Width, rect.Height);
                renderTarget2D.DrawTextLayout(new Vector2(rect.Left, rect.Top + oYshift), textLayout, scb, DrawTextOptions.None);
            }
        }
        public void DrawTextCenter(string text, IbRect rect, FontWeight fw, SharpDX.DirectWrite.FontStyle fs, float scaler, SharpDX.Color fontColor)
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
            //RectangleF rectF = new RectangleF(rect.Left, rect.Top + oYshift, rect.Width, rect.Height);
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, fontColor))
            {
                textFormat = new TextFormat(factoryDWrite, FontFamilyName, CurrentFontCollection, fw, fs, FontStretch.Normal, thisFontHeight) { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Near };
                textLayout = new TextLayout(factoryDWrite, text, textFormat, rect.Width, rect.Height);
                renderTarget2D.DrawTextLayout(new Vector2(rect.Left, rect.Top + oYshift), textLayout, scb, DrawTextOptions.None);
            }
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
            //RectangleF rectF = new RectangleF(rect.Left, rect.Top + oYshift, rect.Width, rect.Height);
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, fontColor))
            {
                textFormat = new TextFormat(factoryDWrite, FontFamilyName, CurrentFontCollection, fw, fs, FontStretch.Normal, thisFontHeight) { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Near };
                textLayout = new TextLayout(factoryDWrite, text, textFormat, rect.Width, rect.Height);
                renderTarget2D.DrawTextLayout(new Vector2(rect.Left, rect.Top + oYshift), textLayout, scb, DrawTextOptions.None);
            }
        }
        //XXXXXXXXXXXXx
        public void DrawTextLeft(string text, IbRect rect, FontWeight fw, SharpDX.DirectWrite.FontStyle fs, float scaler, SharpDX.Color fontColor)
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
            //RectangleF rectF = new RectangleF(rect.Left, rect.Top + oYshift, rect.Width, rect.Height);
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, fontColor))
            {
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
            SharpDX.RectangleF r = new SharpDX.RectangleF(rect.Left, rect.Top + oYshift, rect.Width, rect.Height);
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, penColor))
            {
                renderTarget2D.DrawRectangle(r, scb, penWidth);
            }
        }
        public void DrawLine(int lastX, int lastY, int nextX, int nextY, SharpDX.Color penColor, int penWidth)
        {
            using (SolidColorBrush scb = new SolidColorBrush(renderTarget2D, penColor))
            {
                renderTarget2D.DrawLine(new Vector2(lastX,lastY), new Vector2(nextX, nextY), scb, penWidth);
            }
        }
        public void DrawBitmapGDI(System.Drawing.Bitmap bitmap, IbRect source, IbRect target)
        {
            Rectangle tar = new Rectangle(target.Left, target.Top + oYshift, target.Width, target.Height);
            Rectangle src = new Rectangle(source.Left, source.Top, source.Width, source.Height);
            gCanvas.DrawImage(bitmap, tar, src, GraphicsUnit.Pixel);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target)
        {
            //org
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, bool isParty)
        {
            //klone
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, isParty);
        }

        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRectF source, IbRectF target)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, bool mirror, bool isParty)
        {
            //chnaged to recognize party being drawn
            //sabrina
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, mirror, isParty);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRectF source, IbRectF target, bool mirror)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, mirror, false);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, int angleInDegrees, bool mirror)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInDegrees, mirror);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInRadians, mirror);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror, float opacity)
        {
            //test
            //orgi
            if (bitmap == cc.night_tile_NE || bitmap == cc.night_tile_NW|| bitmap == cc.night_tile_SW || bitmap == cc.night_tile_SE || bitmap == cc.tint_night)
            {
                //opacity *= 0.6f;
                opacity *= mod.nightTimeDarknessOpacity;
             
            }
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInRadians, mirror, opacity);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror, float opacity, bool isParty)
        {
            //test
            //kloni
            if (bitmap == cc.night_tile_NE || bitmap == cc.night_tile_NW || bitmap == cc.night_tile_SW || bitmap == cc.night_tile_SE || bitmap == cc.tint_night)
            {
                //opacity *= 0.6f;
                opacity *= mod.nightTimeDarknessOpacity;

            }
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInRadians, mirror, opacity, isParty);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, int angleInDegrees, bool mirror, int Xshift, int Yshift)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInDegrees, mirror, Xshift, Yshift, 0, 0);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror, int Xshift, int Yshift)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInRadians, mirror, Xshift, Yshift, 0, 0);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, int angleInDegrees, bool mirror, int Xshift, int Yshift, int Xscale, int Yscale)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInDegrees, mirror, Xshift, Yshift, Xscale, Yscale);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, int angleInDegrees, bool mirror, int Xshift, int Yshift, int Xscale, int Yscale, float opacity)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInDegrees, mirror, Xshift, Yshift, Xscale, Yscale, opacity);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, float angleInRadians, bool mirror, int Xshift, int Yshift, int Xscale, int Yscale)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            DrawD2DBitmap(bitmap, src, tar, angleInRadians, mirror, Xshift, Yshift, Xscale, Yscale);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, bool mirror, float opac)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            //calling new overloaded draw that takes in opacity, too
            DrawD2DBitmap(bitmap, src, tar, mirror, opac);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRectF source, IbRectF target, bool mirror, float opac)
        {
            SharpDX.RectangleF tar = new SharpDX.RectangleF(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);
            //calling new overloaded draw that takes in opacity, too
            DrawD2DBitmap(bitmap, src, tar, mirror, opac);
        }
        public void DrawBitmap(SharpDX.Direct2D1.Bitmap bitmap, IbRect source, IbRect target, bool mirror, float opac, bool NearestNeighbourInterpolation)
        {
            SharpDX.Rectangle tar = new SharpDX.Rectangle(target.Left, target.Top + oYshift, target.Width, target.Height);
            SharpDX.Rectangle src = new SharpDX.Rectangle(source.Left, source.Top, source.Width, source.Height);
            //calling new overloaded draw that takes in opacity, too
            DrawD2DBitmap(bitmap, src, tar, mirror, opac, NearestNeighbourInterpolation);
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
                state += "Get Highest Feature Level:";
                var featureLvl = SharpDX.Direct3D11.Device.GetSupportedFeatureLevel();
                state += " Highest Feature Level is: " + featureLvl.ToString() + " :Create Device:";
                try
                {
                    SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, new[] { featureLvl }, desc, out _device, out _swapChain);
                }
                catch (Exception ex)
                {
                    this.errorLog(state + "<--->" + ex.ToString());
                    MessageBox.Show("Failed on Create Device using a feature level of " + featureLvl.ToString() + ". Will try using feature level 'Level_9_1' and DriverType.Software instead of DriverType.Hardware");
                    SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Software, DeviceCreationFlags.BgraSupport, new[] { SharpDX.Direct3D.FeatureLevel.Level_9_1 }, desc, out _device, out _swapChain);                    
                }

                if (_device == null)
                {
                    MessageBox.Show("Failed to create a device, closing IceBlink 2. Please send us your 'IB2ErrorLog.txt' file for more debugging help.");
                    Application.Exit();
                }

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
        }
        public void BeginDraw()
        {
            _device.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, this.Width, this.Height));
            _device.ImmediateContext.OutputMerger.SetTargets(_backBufferView);
            renderTarget2D.BeginDraw();
        }        
        public void EndDraw()
        {
            renderTarget2D.EndDraw();
            _swapChain.Present(1, PresentFlags.None);
        }
        public void Render(float elapsed)
        {
            BeginDraw(); //uncomment this for DIRECT2D ADDITIONS  
          
            renderTarget2D.Clear(Color4.Black); //uncomment this for DIRECT2D ADDITIONS

            if ((mod.useUIBackground) && (!screenType.Equals("main")) && (!screenType.Equals("combat")) && (!screenType.Equals("launcher")) && (! screenType.Equals("title")))
            {
                drawUIBackground();
            }            
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
            else if (screenType.Equals("learnSpellLevelUp") || screenType.Equals("learnSpellLevelUpCombat"))
            {
                screenSpellLevelUp.redrawSpellLevelUp(false);
            }
            else if (screenType.Equals("learnTraitCreation"))
            {
                screenTraitLevelUp.redrawTraitLevelUp(true);
            }
            else if (screenType.Equals("learnTraitLevelUp") || screenType.Equals("learnTraitLevelUpCombat"))
            {
                screenTraitLevelUp.redrawTraitLevelUp(false);
            }
            else if (screenType.Equals("main"))
            {
                screenMainMap.redrawMain(elapsed);
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
            else if (screenType.Equals("combatTraitUse"))
            {
                screenTraitUseSelector.redrawTraitUseSelector(true);
            }
            else if (screenType.Equals("mainMapTraitUse"))
            {
                screenTraitUseSelector.redrawTraitUseSelector(false);
            }
            else if (screenType.Equals("convo"))
            {
                screenConvo.redrawConvo();
            }
            else if (screenType.Equals("partyBuild"))
            {
                screenPartyBuild.redrawPartyBuild();
            }
            else if (screenType.Equals("partyRoster"))
            {
                screenPartyRoster.redrawPartyRoster();
            }
            if (mod.debugMode)
            {
                int txtH = (int)drawFontRegHeight;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        DrawText("FPS:" + fps.ToString(), new IbRect(x + 5, screenHeight - txtH - 5 + y - oYshift, 100, 100), 1.0f, SharpDX.Color.Black);
                    }
                }
                DrawText("FPS:" + fps.ToString(), new IbRect(5, screenHeight - txtH - 5 - oYshift, 100, 100), 1.0f, SharpDX.Color.White);
            }

            EndDraw(); //uncomment this for DIRECT2D ADDITIONS
        }
        private void RenderCallback()
        {
            //bloodbus
            //Render(0);
        }
        public void drawUIBackground()
        {
            try
            {
                IbRect src = new IbRect(0, 0, cc.ui_bg_fullscreen.PixelSize.Width, cc.ui_bg_fullscreen.PixelSize.Height);
                IbRect dst = new IbRect(0, -oYshift, screenWidth, screenHeight);
                DrawBitmap(cc.ui_bg_fullscreen, src, dst);
            }
            catch
            { }
        }

        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target)
        {
            //too high
            //org2
            DrawD2DBitmap(bitmap, source, target, false, false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, bool isParty)
        {
            //too high
            //klone2
            DrawD2DBitmap(bitmap, source, target, false, isParty);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, bool mirror, bool isParty)
        {
            //too high
            //sabrina2
            DrawD2DBitmap(bitmap, source, target, 0, mirror, 1.0f , 0, 0, 0, 0, false, isParty);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, int angleInDegrees, bool mirror)
        {
            //too high
            DrawD2DBitmap(bitmap, source, target, angleInDegrees, mirror, 1.0f, 0, 0, 0, 0, false, false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, float angleInRadians, bool mirror)
        {
            //too high
            DrawD2DBitmap(bitmap, source, target, angleInRadians, mirror, 1.0f, 0, 0, 0, 0, false, false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, float angleInRadians, bool mirror, float opacity)
        {
            //orgi2
            //too high
            DrawD2DBitmap(bitmap, source, target, angleInRadians, mirror, opacity, 0, 0, 0, 0, false, false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, float angleInRadians, bool mirror, float opacity, bool isParty)
        {
            //kloni2
            //too high
            DrawD2DBitmap(bitmap, source, target, angleInRadians, mirror, opacity, 0, 0, 0, 0, false, isParty);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, bool mirror, float opac)
        {
            //too high
            DrawD2DBitmap(bitmap, source, target, 0, mirror, opac, 0, 0, 0, 0, false,false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, bool mirror, float opac, bool NearestNeighbourInterpolation)
        {
            //too high
            DrawD2DBitmap(bitmap, source, target, 0, mirror, opac, 0, 0, 0, 0, NearestNeighbourInterpolation,false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, int angleInDegrees, bool mirror, int Xshift, int Yshift, int Xscale, int Yscale)
        {
            //too high
            DrawD2DBitmap(bitmap, source, target, angleInDegrees, mirror, 1.0f, Xshift, Yshift, Xscale, Yscale, false, false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, int angleInDegrees, bool mirror, int Xshift, int Yshift, int Xscale, int Yscale, float opacity)
        {
            //too high
            DrawD2DBitmap(bitmap, source, target, angleInDegrees, mirror, 1.0f, Xshift, Yshift, Xscale, Yscale, false, opacity);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, float angleInRadians, bool mirror, int Xshift, int Yshift, int Xscale, int Yscale)
        {
            DrawD2DBitmap(bitmap, source, target, angleInRadians, mirror, 1.0f, Xshift, Yshift, Xscale, Yscale, false, false);
        }
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, int angleInDegrees, bool mirror, float opac, int Xshift, int Yshift, int Xscale, int Yscale, bool NearestNeighbourInterpolation, bool isParty)
        {
            //too high
            //convert degrees to radians
            //sabrina3
            float angleInRadians = (float)(Math.PI * 2 * (float)angleInDegrees / (float)360);
            DrawD2DBitmap(bitmap, source, target, angleInRadians, mirror, opac, Xshift, Yshift, Xscale, Yscale, false, isParty);
        }

        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, int angleInDegrees, bool mirror, float opac, int Xshift, int Yshift, int Xscale, int Yscale, bool NearestNeighbourInterpolation, float opacity)
        {
            //too high
            //convert degrees to radians
            float angleInRadians = (float)(Math.PI * 2 * (float)angleInDegrees / (float)360);
            DrawD2DBitmap(bitmap, source, target, angleInRadians, mirror, opacity, Xshift, Yshift, Xscale, Yscale, false, false);
        }

        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, float angleInRadians, bool mirror, float opac, int Xshift, int Yshift, int Xscale, int Yscale, bool NearestNeighbourInterpolation, bool isParty)
        {
            //sabrina4
            if (mod.useScrollingSystem)
            {
                if (this.screenType == "main")
                {
                    if (mod.isScrollingNow && !isParty)
                    {
                        if (mod.scrollingDirection == "up")
                        {
                            target.Y = target.Y - (int)(squareSize * (mod.scrollingTimer/100f));
                        }

                        if (mod.scrollingDirection == "down")
                        {
                            target.Y = target.Y + (int)(squareSize * (mod.scrollingTimer / 100f));
                        }

                        if (mod.scrollingDirection == "left")
                        {
                            target.X = target.X - (int)(squareSize * (mod.scrollingTimer / 100f));
                        }

                        if (mod.scrollingDirection == "right")
                        {
                            target.X = target.X + (int)(squareSize * (mod.scrollingTimer / 100f));
                        }
                    }
                }
            }
            
            //org 1
            int mir = 1;
            if (mirror) { mir = -1; }
            float xshf = (float)Xshift * 2 * screenDensity;
            float yshf = (float)Yshift * 2 * screenDensity;
            float xscl = 1f + (((float)Xscale * 2 * screenDensity) / squareSize);
            float yscl = 1f + (((float)Yscale * 2 * screenDensity) / squareSize);

            Vector2 center = new Vector2(target.Left + (target.Width / 2), target.Top + (target.Height / 2));
            //vertauensvoll
            if ((angleInRadians == 361) && (mirror))
            {
                angleInRadians = 90;
                renderTarget2D.Transform = SharpDX.Matrix.Transformation2D(center, 0, new Vector2(xscl,mir * yscl), center, angleInRadians, new Vector2(xshf, yshf));

            }
            else if (angleInRadians == 361 && !mirror)
            {
                angleInRadians = 270;
                renderTarget2D.Transform = SharpDX.Matrix.Transformation2D(center, 0, new Vector2(mir * xscl, yscl), center, angleInRadians, new Vector2(xshf, yshf));

            }
            else if ((angleInRadians == 362) && (mirror))
            {
                //90
                angleInRadians = 180;
                renderTarget2D.Transform = SharpDX.Matrix.Transformation2D(center, 0, new Vector2(mir* xscl, yscl), center, angleInRadians, new Vector2(xshf, yshf));

            }
            else if ((angleInRadians == 362) && (!mirror))
            {
                angleInRadians = 270;
                renderTarget2D.Transform = SharpDX.Matrix.Transformation2D(center, 0, new Vector2(mir * xscl, yscl), center, angleInRadians, new Vector2(xshf, yshf));

            }
            else
            {
                renderTarget2D.Transform = SharpDX.Matrix.Transformation2D(center, 0, new Vector2(mir * xscl, yscl), center, angleInRadians, new Vector2(xshf, yshf));

            }
            //renderTarget2D.Transform = SharpDX.Matrix.Transformation2D(center, 0, new Vector2(mir * xscl, yscl), center, angleInRadians, new Vector2(xshf, yshf));
            SharpDX.RectangleF trg = new SharpDX.RectangleF(target.Left, target.Top, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);

            /*
            if (bitmap == cc.offScreen)
            {
                if ((target.Left <= (screenWidth / 2 - mod.pixDistanceToBorderWest)) && (target.Left >= (screenWidth / 2 - mod.pixDistanceToBorderWest - squareSize)))
                {
                    bitmap = cc.offScreenTrans;
                }
            }
            */

            if (NearestNeighbourInterpolation)
            {
                renderTarget2D.DrawBitmap(bitmap, trg, opac, BitmapInterpolationMode.NearestNeighbor, src);
            }
            else
            {
                renderTarget2D.DrawBitmap(bitmap, trg, opac, BitmapInterpolationMode.Linear, src);
            }            
            renderTarget2D.Transform = Matrix3x2.Identity;
            //TBSüd
        }

        /*
        public void DrawD2DBitmap(SharpDX.Direct2D1.Bitmap bitmap, SharpDX.RectangleF source, SharpDX.RectangleF target, float angleInRadians, bool mirror, float opac, int Xshift, int Yshift, int Xscale, int Yscale, bool NearestNeighbourInterpolation, float opacity)
        {
            int mir = 1;
            if (mirror) { mir = -1; }
            float xshf = (float)Xshift * 2 * screenDensity;
            float yshf = (float)Yshift * 2 * screenDensity;
            float xscl = 1f + (((float)Xscale * 2 * screenDensity) / squareSize);
            float yscl = 1f + (((float)Yscale * 2 * screenDensity) / squareSize);

            Vector2 center = new Vector2(target.Left + (target.Width / 2), target.Top + (target.Height / 2));
            renderTarget2D.Transform = SharpDX.Matrix.Transformation2D(center, 0, new Vector2(mir * xscl, yscl), center, angleInRadians, new Vector2(xshf, yshf));
            SharpDX.RectangleF trg = new SharpDX.RectangleF(target.Left, target.Top, target.Width, target.Height);
            SharpDX.RectangleF src = new SharpDX.RectangleF(source.Left, source.Top, source.Width, source.Height);

            /*
            if (bitmap == cc.offScreen)
            {
                if ((target.Left <= (screenWidth / 2 - mod.pixDistanceToBorderWest)) && (target.Left >= (screenWidth / 2 - mod.pixDistanceToBorderWest - squareSize)))
                {
                    bitmap = cc.offScreenTrans;
                }
            }
            

            if (NearestNeighbourInterpolation)
            {
                renderTarget2D.DrawBitmap(bitmap, trg, opac, BitmapInterpolationMode.NearestNeighbor, src);
            }
            else
            {
                renderTarget2D.DrawBitmap(bitmap, trg, opac, BitmapInterpolationMode.Linear, src);
            }
            renderTarget2D.Transform = Matrix3x2.Identity;
        }
    */
        //INPUT STUFF
        //private void form_onKeyDown(object sender, KeyEventArgs e)
        //{
        //onKeyDown(sender, e);
        //}

        private void GameView_onKeyUp(object sender, KeyEventArgs e)
        {
            if (screenType.Equals("main"))
            {
               
                if (e.KeyCode == Keys.Up && this.screenMainMap.showMoveKeys && mod.blockUpKey)
                {
                    mod.blockUpKey = false;
                }
                if (e.KeyCode == Keys.Down && this.screenMainMap.showMoveKeys && mod.blockDownKey)
                {
                    mod.blockDownKey = false;
                }
                if (e.KeyCode == Keys.Left && this.screenMainMap.showMoveKeys && mod.blockLeftKey)
                {
                    mod.blockLeftKey = false;
                }
                if (e.KeyCode == Keys.Right && this.screenMainMap.showMoveKeys && mod.blockRightKey)
                {
                    mod.blockRightKey = false;
                }

                if (!mod.blockRightKey && !mod.blockLeftKey && !mod.blockUpKey && !mod.blockDownKey)
                {
                    aTimer.Stop();
                    mod.scrollModeSpeed = 1.0f;
                }              
            }
        }

        private void GameView_onKeyDown(object sender, KeyEventArgs e)
        {
            //superscroll
            if (screenType.Equals("main"))
            {
                if (e.KeyCode == Keys.Up && this.screenMainMap.showMoveKeys && !mod.blockUpKey)
                {
                    mod.blockUpKey = true;
                    aTimer.Stop();
                    if (!moveTimerRuns)
                    {

                        mod.doTriggerInspiteOfScrolling = false;
                        bool blockMoveBecausOfCurrentScrolling = false;

                        if (mod.useScrollingSystem)
                        {
                            if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                            {
                                blockMoveBecausOfCurrentScrolling = true;
                            }
                        }

                        blockMoveBecausOfCurrentScrolling = false;

                        if (!blockMoveBecausOfCurrentScrolling)
                        {
                            if (mod.useScrollingSystem)
                            {
                                //single press
                                if (mod.isScrollingNow)
                                {
                                    mod.isScrollingNow = true;
                                    //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                                    mod.scrollingTimer = 100;
                                    mod.scrollingDirection = "up";
                                    mod.doTriggerInspiteOfScrolling = true;
                                    bool isTransition = cc.goNorth();
                                    if (!isTransition)
                                    {
                                        mod.breakActiveSearch = false;
                                        //gv.mod.wasJustCalled = false;
                                        //if (!gv.mod.wasJustCalled)
                                        //{
                                        //if (gv.screenType == "main")
                                        //{
                                        screenMainMap.moveUp(true);
                                        //}
                                        //gv.mod.wasJustCalled = true;
                                        //}
                                    }
                                }
                                //continued press
                                //else if (moveDelay2())
                                else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                                {
                                    mod.isScrollingNow = true;
                                    mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                    mod.scrollingOverhang2 = 0;
                                    mod.scrollingDirection = "up";
                                    mod.doTriggerInspiteOfScrolling = true;
                                    bool isTransition = cc.goNorth();
                                    if (!isTransition)
                                    {
                                        mod.breakActiveSearch = false;
                                        //gv.mod.wasJustCalled = false;
                                        //if (!gv.mod.wasJustCalled)
                                        //{
                                        //if (gv.screenType == "main")
                                        //{
                                        screenMainMap.moveUp(true);
                                        //}
                                        //gv.mod.wasJustCalled = true;
                                        //}
                                    }
                                }
                            }


                        }
                    }
                    aTimer.Start();
                    mod.scrollModeSpeed = 0.45f;
                }
                else if (e.KeyCode == Keys.Down && this.screenMainMap.showMoveKeys && !mod.blockDownKey)
                {
                    mod.blockDownKey = true;
                    aTimer.Stop();
                    if (!moveTimerRuns)
                    {

                        mod.doTriggerInspiteOfScrolling = false;
                        bool blockMoveBecausOfCurrentScrolling = false;

                        if (mod.useScrollingSystem)
                        {
                            if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                            {
                                blockMoveBecausOfCurrentScrolling = true;
                            }
                        }

                        blockMoveBecausOfCurrentScrolling = false;

                        if (!blockMoveBecausOfCurrentScrolling)
                        {
                            if (mod.useScrollingSystem)
                            {
                                //single press
                                if (mod.isScrollingNow)
                                {
                                    mod.isScrollingNow = true;
                                    //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                                    mod.scrollingTimer = 100;
                                    mod.scrollingDirection = "down";
                                    mod.doTriggerInspiteOfScrolling = true;
                                    bool isTransition = cc.goSouth();
                                    if (!isTransition)
                                    {
                                        mod.breakActiveSearch = false;
                                        //gv.mod.wasJustCalled = false;
                                        //if (!gv.mod.wasJustCalled)
                                        //{
                                        //if (gv.screenType == "main")
                                        //{
                                        screenMainMap.moveDown(true);
                                        //}
                                        //gv.mod.wasJustCalled = true;
                                        //}
                                    }
                                }
                                //continued press
                                //else if (moveDelay2())
                                else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                                {
                                    mod.isScrollingNow = true;
                                    mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                    mod.scrollingOverhang2 = 0;
                                    mod.scrollingDirection = "down";
                                    mod.doTriggerInspiteOfScrolling = true;
                                    bool isTransition = cc.goSouth();
                                    if (!isTransition)
                                    {
                                        mod.breakActiveSearch = false;
                                        //gv.mod.wasJustCalled = false;
                                        //if (!gv.mod.wasJustCalled)
                                        //{
                                        //if (gv.screenType == "main")
                                        //{
                                        screenMainMap.moveDown(true);
                                        //}
                                        //gv.mod.wasJustCalled = true;
                                        //}
                                    }
                                }
                            }


                        }
                    }
                    aTimer.Start();
                    mod.scrollModeSpeed = 0.45f;
                }
                else if (e.KeyCode == Keys.Right && this.screenMainMap.showMoveKeys && !mod.blockRightKey)
                {
                    mod.blockRightKey = true;
                    aTimer.Stop();
                    if (!moveTimerRuns)
                    {

                        mod.doTriggerInspiteOfScrolling = false;
                        bool blockMoveBecausOfCurrentScrolling = false;

                        if (mod.useScrollingSystem)
                        {
                            if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                            {
                                blockMoveBecausOfCurrentScrolling = true;
                            }
                        }

                        blockMoveBecausOfCurrentScrolling = false;

                        if (!blockMoveBecausOfCurrentScrolling)
                        {
                            if (mod.useScrollingSystem)
                            {
                                //single press
                                if (mod.isScrollingNow)
                                {
                                    mod.isScrollingNow = true;
                                    //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                                    mod.scrollingTimer = 100;
                                    mod.scrollingDirection = "right";
                                    mod.doTriggerInspiteOfScrolling = true;
                                    bool isTransition = cc.goEast();
                                    if (!isTransition)
                                    {
                                        mod.breakActiveSearch = false;
                                        //gv.mod.wasJustCalled = false;
                                        //if (!gv.mod.wasJustCalled)
                                        //{
                                        //if (gv.screenType == "main")
                                        //{
                                        screenMainMap.moveRight(true);
                                        //}
                                        //gv.mod.wasJustCalled = true;
                                        //}
                                    }
                                }
                                //continued press
                                //else if (moveDelay2())
                                else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                                {
                                    mod.isScrollingNow = true;
                                    mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                    mod.scrollingOverhang2 = 0;
                                    mod.scrollingDirection = "right";
                                    mod.doTriggerInspiteOfScrolling = true;
                                    bool isTransition = cc.goEast();
                                    if (!isTransition)
                                    {
                                        mod.breakActiveSearch = false;
                                        //gv.mod.wasJustCalled = false;
                                        //if (!gv.mod.wasJustCalled)
                                        //{
                                        //if (gv.screenType == "main")
                                        //{
                                        screenMainMap.moveRight(true);
                                        //}
                                        //gv.mod.wasJustCalled = true;
                                        //}
                                    }
                                }
                            }


                        }
                    }
                    aTimer.Start();
                    mod.scrollModeSpeed = 0.45f;
                }
                else if (e.KeyCode == Keys.Left && this.screenMainMap.showMoveKeys && !mod.blockLeftKey)
                {
                    mod.blockLeftKey = true;
                    aTimer.Stop();
                    if (!moveTimerRuns)
                    {

                        mod.doTriggerInspiteOfScrolling = false;
                        bool blockMoveBecausOfCurrentScrolling = false;

                        if (mod.useScrollingSystem)
                        {
                            if (mod.scrollingTimer != 100 && mod.scrollingTimer != 0)
                            {
                                blockMoveBecausOfCurrentScrolling = true;
                            }
                        }

                        blockMoveBecausOfCurrentScrolling = false;

                        if (!blockMoveBecausOfCurrentScrolling)
                        {
                            if (mod.useScrollingSystem)
                            {
                                //single press
                                if (mod.isScrollingNow)
                                {
                                    mod.isScrollingNow = true;
                                    //gv.mod.scrollingTimer = 100 + gv.mod.scrollingOverhang2;
                                    mod.scrollingTimer = 100;
                                    mod.scrollingDirection = "left";
                                    mod.doTriggerInspiteOfScrolling = true;
                                    bool isTransition = cc.goWest();
                                    if (!isTransition)
                                    {
                                        mod.breakActiveSearch = false;
                                        //gv.mod.wasJustCalled = false;
                                        //if (!gv.mod.wasJustCalled)
                                        //{
                                        //if (gv.screenType == "main")
                                        //{
                                        screenMainMap.moveLeft(true);
                                        //}
                                        //gv.mod.wasJustCalled = true;
                                        //}
                                    }
                                }
                                //continued press
                                //else if (moveDelay2())
                                else if (mod.scrollingTimer <= mod.lastScrollStep || mod.scrollingTimer >= 100)
                                {
                                    mod.isScrollingNow = true;
                                    mod.scrollingTimer = 100 + mod.scrollingOverhang2;
                                    mod.scrollingOverhang2 = 0;
                                    mod.scrollingDirection = "left";
                                    mod.doTriggerInspiteOfScrolling = true;
                                    bool isTransition = cc.goWest();
                                    if (!isTransition)
                                    {
                                        mod.breakActiveSearch = false;
                                        //gv.mod.wasJustCalled = false;
                                        //if (!gv.mod.wasJustCalled)
                                        //{
                                        //if (gv.screenType == "main")
                                        //{
                                        screenMainMap.moveLeft(true);
                                        //}
                                        //gv.mod.wasJustCalled = true;
                                        //}
                                    }
                                }
                            }


                        }
                    }
                    aTimer.Start();
                    mod.scrollModeSpeed = 0.45f;
                }
            }
        } 

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
                //TODO log.onMouseDown(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseDown);
        }
        private void GameView_MouseUp(object sender, MouseEventArgs e)
        {
            if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                //TODO log.onMouseUp(sender, e);
            }
            onMouseEvent(sender, e, MouseEventType.EventType.MouseUp);
        }
        private void GameView_MouseMove(object sender, MouseEventArgs e)
        {
            if ((screenType.Equals("main")) || (screenType.Equals("combat")))
            {
                mousePositionX = e.X;
                mousePositionY = e.Y;

                //TODO log.onMouseMove(sender, e);
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
                //if (touchEnabled)
                //{
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
                        screenSpellLevelUp.onTouchSpellLevelUp(e, eventType, true, false);   	
                    }
                    else if (screenType.Equals("learnSpellLevelUp"))
                    {
                        screenSpellLevelUp.onTouchSpellLevelUp(e, eventType, false, false);     	
                    }
                    else if (screenType.Equals("learnSpellLevelUpCombat"))
                    {
                        screenSpellLevelUp.onTouchSpellLevelUp(e, eventType, false, true);
                    }
                    else if (screenType.Equals("learnTraitCreation"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(e, eventType, true, false);   	
                    }
                    else if (screenType.Equals("learnTraitLevelUp"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(e, eventType, false, false);     	
                    }
                    //allscreentypes
                    else if (screenType.Equals("learnTraitLevelUpCombat"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(e, eventType, false, true);
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
                    if (touchEnabled)
                    {
                        screenCombat.onTouchCombat(e, eventType);
                    }
                    }
                    else if (screenType.Equals("combatCast"))
                    {
                    if (touchEnabled)
                    {
                        screenCastSelector.onTouchCastSelector(e, eventType, true);
                    }
                    }
                    else if (screenType.Equals("mainMapCast"))
                    {
                        screenCastSelector.onTouchCastSelector(e, eventType, false);
                    }
                    else if (screenType.Equals("combatTraitUse"))
                    {
                    if (touchEnabled)
                    {
                        screenTraitUseSelector.onTouchCastSelector(e, eventType, true);
                    }
                    }
                    else if (screenType.Equals("mainMapTraitUse"))
                    {
                        screenTraitUseSelector.onTouchCastSelector(e, eventType, false);
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
                //}
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
                //if (touchEnabled)
                //{
                    if (keyData == Keys.H)
                    {
                        if (showHotKeys) { showHotKeys = false; }
                        else { showHotKeys = true; }
                        /*
                        if (screenType.Equals("main"))
                        {
                            sf.MessageBoxHtml(cc.loadTextToString("MessageParty.txt"));
                        }
                        else if (screenType.Equals("combat"))
                        {
                            sf.MessageBoxHtml(cc.loadTextToString("MessageParty.txt"));
                        }
                        */
                    }
                    //meins
                if (keyData == Keys.Return || keyData == Keys.Escape)
                {

                    if ((screenType == "combat" || screenType == "main" || screenType == "partyBuild" || screenType == "launcher" || screenType == "title" || screenType == "partyBuild") && (keyData == Keys.Escape))
                    {
                        this.Close();
                    }
                    if (screenType == "pcCreation" && keyData == Keys.Escape)
                    {
                        screenType = "partyBuild";
                    }
                    if (screenType == "combatInventory")
                    {
                        //bool inCombat = false;
                        if (mod.currentEncounter != null)
                        {
                            if (!mod.currentEncounter.isOver)
                            {
                                //inCombat = true;
                                if (screenCombat.canMove)
                                {
                                    screenCombat.currentCombatMode = "move";
                                }
                                else
                                {
                                    screenCombat.currentCombatMode = "attack";
                                }
                                screenType = "combat";
                                screenInventory.doCleanUp();
                            }
                        }
                    }

                    if (screenType == "inventory")
                    {
                        screenType = "main";
                        screenInventory.doCleanUp();
                    }

                    if (screenType == "itemSelector")
                    {
                        if (screenItemSelector.itemSelectorType.Equals("container"))
                        {
                            screenType = "main";
                        }
                        else if (screenItemSelector.itemSelectorType.Equals("equip"))
                        {
                            if (screenItemSelector.callingScreen.Equals("party"))
                            {
                                screenType = "party";
                            }
                            else if (screenItemSelector.callingScreen.Equals("combatParty"))
                            {
                                screenType = "combatParty";
                            }
                        }
                        screenItemSelector.doCleanUp();
                    }

                    if (screenType == "journal")
                    {
                        screenType = "main";
                        screenJournal.journalCleanup();
                    }

                    if (screenType == "party")
                    {
                        screenType = "main";
                    }

                    if (screenType == "combatParty")
                    {
                        if (screenCombat.canMove)
                        {
                            screenCombat.currentCombatMode = "move";
                        }
                        else
                        {
                            screenCombat.currentCombatMode = "attack";
                        }
                        screenType = "combat";
                    }

                    if (screenType.Equals("combatCast"))
                    {
                        if (screenCombat.canMove)
                        {
                            screenCombat.currentCombatMode = "move";
                        }
                        else
                        {
                            screenCombat.currentCombatMode = "attack";
                        }
                        screenType = "combat";
                        screenCastSelector.doCleanUp();
                    }


                    if (screenType.Equals("mainMapCast"))
                    {
                        screenType = "main";
                        screenCastSelector.doCleanUp();
                    }

                    if (screenType.Equals("combatTraitUse"))
                    {
                        if (screenCombat.canMove)
                        {
                            screenCombat.currentCombatMode = "move";
                        }
                        else
                        {
                            screenCombat.currentCombatMode = "attack";
                        }
                        screenType = "combat";
                        screenTraitUseSelector.doCleanUp();
                    }

                    if (screenType.Equals("mainMapTraitUse"))
                    {
                        screenType = "main";
                        screenTraitUseSelector.doCleanUp();
                    }

                    //shop
                    if (screenType.Equals("shop"))
                    {
                        screenShop.armorFilterOn = false;
                        screenShop.generalFilterOn = false;
                        screenShop.weaponFilterOn = false;
                        screenShop.inventorySlotIndex = 0;
                        screenShop.inventoryPageIndex = 0;
                        screenShop.btnPageIndex.Text = "1/40";
                        screenShop.btnAll.glowOn = false;
                        screenShop.btnWeapons.glowOn = false;
                        screenShop.btnArmors.glowOn = false;
                        screenShop.btnGeneral.glowOn = false;

                        screenShop.armorFilterOnShop = false;
                        screenShop.generalFilterOnShop = false;
                        screenShop.weaponFilterOnShop = false;
                        screenShop.shopSlotIndex = 0;
                        screenShop.shopPageIndex = 0;
                        screenShop.btnShopPageIndex.Text = "1/40";
                        screenShop.btnAllShop.glowOn = false;
                        screenShop.btnWeaponsShop.glowOn = false;
                        screenShop.btnArmorsShop.glowOn = false;
                        screenShop.btnGeneralShop.glowOn = false;
                        screenType = "main";
                    }

                    if (screenType == "portraitSelector")
                    {
                        if (keyData == Keys.Return)
                        {
                            //return to calling screen
                            if (screenPortraitSelector.callingScreen.Equals("party"))
                            {
                                screenParty.gv.mod.playerList[cc.partyScreenPcIndex].portraitFilename = screenPortraitSelector.playerPortraitList[screenPortraitSelector.GetIndex()];
                                screenType = "party";
                                screenParty.portraitLoad(screenParty.gv.mod.playerList[cc.partyScreenPcIndex]);
                            }
                            else if (screenPortraitSelector.callingScreen.Equals("pcCreation"))
                            {
                                //set PC portrait filename to the currently selected image
                                screenPcCreation.pc.portraitFilename = screenPortraitSelector.playerPortraitList[screenPortraitSelector.GetIndex()];
                                screenType = "pcCreation";
                                screenPcCreation.portraitLoad(screenPcCreation.pc);
                            }
                            screenPortraitSelector.doCleanUp();
                        }

                        if (keyData == Keys.Escape)
                        {
                            //do nothing, return to calling screen
                            if (screenPortraitSelector.callingScreen.Equals("party"))
                            {
                                screenType = "party";
                            }
                            else if (screenPortraitSelector.callingScreen.Equals("pcCreation"))
                            {
                                screenType = "pcCreation";
                            }
                            screenPortraitSelector.doCleanUp();
                        }


                    }

                    if (screenType == "tokenSelector")
                    {
                        if (keyData == Keys.Return)
                        {
                            //return to calling screen
                            if (screenTokenSelector.callingScreen.Equals("party"))
                            {
                                screenParty.gv.mod.playerList[cc.partyScreenPcIndex].tokenFilename = screenTokenSelector.playerTokenList[screenTokenSelector.GetIndex()];
                                screenType = "party";
                                screenParty.tokenLoad(screenParty.gv.mod.playerList[cc.partyScreenPcIndex]);
                            }
                            else if (screenTokenSelector.callingScreen.Equals("pcCreation"))
                            {
                                //set PC portrait filename to the currently selected image
                                screenPcCreation.pc.tokenFilename = screenTokenSelector.playerTokenList[screenTokenSelector.GetIndex()];
                                screenType = "pcCreation";
                                screenPcCreation.tokenLoad(screenPcCreation.pc);
                            }
                            screenTokenSelector.doCleanUp();
                        }

                        if (keyData == Keys.Escape)
                        {
                            //do nothing, return to calling screen
                            if (screenTokenSelector.callingScreen.Equals("party"))
                            {
                                screenType = "party";
                            }
                            else if (screenTokenSelector.callingScreen.Equals("pcCreation"))
                            {
                                screenType = "pcCreation";
                            }
                            screenTokenSelector.doCleanUp();
                        }
                    }
                    if (screenType == "partyRoster")
                    {
                        if (mod.playerList.Count > 0)
                        {
                            //check to see if any non-removeable PCs are in roster
                            if (screenPartyRoster.checkForNoneRemovablePcInRoster())
                            {
                                return;
                            }
                            //check to see if mainPc is in party
                            if (screenPartyRoster.checkForMainPc())
                            {
                                screenType = "main";
                            }
                            else
                            {
                                sf.MessageBoxHtml("You must have the Main PC (the first PC you created) in your party before exiting this screen");
                            }
                        }
                    }
                    //todo
                    // "learnSpellLevelUpCombat", always info only (single return button, btnselect, esc and return)
                    //"learnTraitLevelUpCombat", always info only (single return button, btnselect, esc and return)
                    //"learnSpellLevelUp", can be inof only
                    //"learnTraitLevelUp", can be info only
                    //"learnTraitCreation", never info only
                    //"learnSpellCreation", never info only
                    /*
                    else if (screenType.Equals("learnTraitCreation"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(e, eventType, true, false);
                    }
                    else if (screenType.Equals("learnTraitLevelUp"))
                    {
                        screenTraitLevelUp.onTouchTraitLevelUp(e, eventType, false, false);
                    }
                    else if (screenType.Equals("learnTraitLevelUpCombat"))
                    */

                    if (screenType == "learnTraitLevelUp")
                    {
                        if (screenTraitLevelUp.infoOnly)
                        {
                                screenType = "party";
                        }
                        else
                        {
                            if (keyData == Keys.Return)
                            {
                                screenTraitLevelUp.doSelectedTraitToLearn(false);
                            }

                            if (keyData == Keys.Escape)
                            {
                                screenParty.traitGained = "";
                                screenParty.spellGained = "";
                                screenTraitLevelUp.pc.learningTraitsTags.Clear();
                                screenTraitLevelUp.pc.learningEffects.Clear();
                                screenTraitLevelUp.pc.learningSpellsTags.Clear();
                                screenTraitLevelUp.traitToLearnIndex = 1;
                                screenTraitLevelUp.pc.classLevel--;
                                screenType = "party";
                            }

                        }

                    }

                    if (screenType == "learnTraitLevelUpCombat")
                    {
                        if (screenTraitLevelUp.infoOnly)
                        {
                            screenType = "combatParty";
                        }
                    }

                    if (screenType == "learnTraitCreation")
                    {
                            if (keyData == Keys.Return)
                            {
                                screenTraitLevelUp.doSelectedTraitToLearn(true);
                            }

                            if (keyData == Keys.Escape)
                            {
                                screenParty.traitGained = "";
                                screenParty.spellGained = "";
                                screenTraitLevelUp.pc.learningTraitsTags.Clear();
                                screenTraitLevelUp.pc.learningEffects.Clear();
                                screenTraitLevelUp.pc.learningSpellsTags.Clear();
                                screenTraitLevelUp.traitToLearnIndex = 1;
                                screenType = "pcCreation";
                        }
                    }


                    //for spells
                    if (screenType == "learnSpellLevelUp")
                    {
                        if (screenSpellLevelUp.infoOnly)
                        {
                            screenType = "party";
                        }
                        else
                        {
                            if (keyData == Keys.Return)
                            {
                                screenSpellLevelUp.doSelectedSpellToLearn(false);
                            }

                            if (keyData == Keys.Escape)
                            {
                                screenParty.traitGained = "";
                                screenParty.spellGained = "";
                                screenSpellLevelUp.pc.learningTraitsTags.Clear();
                                screenSpellLevelUp.pc.learningEffects.Clear();
                                screenSpellLevelUp.pc.learningSpellsTags.Clear();
                                screenSpellLevelUp.spellToLearnIndex = 1;
                                screenSpellLevelUp.pc.classLevel--;
                                screenType = "party";
                            }
                        }

                    }

                    if (screenType == "learnSpellLevelUpCombat")
                    {
                        if (screenSpellLevelUp.infoOnly)
                        {
                            screenType = "combatParty";
                        }
                    }

                    if (screenType == "learnSpellCreation")
                    {
                        if (keyData == Keys.Return)
                        {
                            screenSpellLevelUp.doSelectedSpellToLearn(true);
                        }

                        if (keyData == Keys.Escape)
                        {
                            screenParty.traitGained = "";
                            screenParty.spellGained = "";
                            screenSpellLevelUp.pc.learningTraitsTags.Clear();
                            screenSpellLevelUp.pc.learningEffects.Clear();
                            screenSpellLevelUp.pc.learningSpellsTags.Clear();
                            screenSpellLevelUp.spellToLearnIndex = 1;
                            screenType = "pcCreation";
                        }
                    }



                }

                /*
                if (keyData == Keys.Escape)
                {
                    this.Close();
                }
                */
                
                if (screenType.Equals("main"))
                    {
                    //if (!mod.blockMainKeyboard)
                    //{
                        screenMainMap.onKeyUp(keyData);
                    //}
                   
                    //else
                    //{
                        //int hghg = 1;
                    //}
                        //krah krah
                }
                    else if (screenType.Equals("combat"))
                    {
                    if (touchEnabled)
                    {
                        screenCombat.onKeyUp(keyData);
                    }
                    }
                    else if (screenType.Equals("convo"))
                    {
                        screenConvo.onKeyUp(keyData);
                    }
                    else if (screenType.Equals("itemSelector"))
                    {
                        screenItemSelector.onKeyUp(keyData);
                    }
                /*
                else
                {
                    if (keyData == Keys.Up | keyData == Keys.D8)
                    {
                        int i = 1;
                        //IBHtmlMessageBox.SetCurrentTopLineIndex(-12);
                        //this.Invalidate();
                    }

                    if (keyData == Keys.Down | keyData == Keys.D2)
                    {
                        //SetCurrentTopLineIndex(12);
                        //this.Invalidate();
                    }
                }
                */
            }
            catch (Exception ex)
            {
                errorLog(ex.ToString());
            }
        }

       

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            //if (screenType != "main")
            //{
                onKeyboardEvent(keyData);
            //}
            //mod.keyIsHeld = true;
                
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //ON FORM CLOSING
        private void GameView_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            DialogResult dlg = IBMessageBox.Show(this, "Are you sure you wish to exit the game?", enumMessageButton.YesNo);
            if (dlg == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            if (dlg == DialogResult.No)
            {
                e.Cancel = true;
            }
            
            //if ()
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

        private void GameView_Load(object sender, EventArgs e)
        {

        }
    }
}
