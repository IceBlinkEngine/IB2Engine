using System;
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
        public int playerOffset = 4;
        public int oXshift = 0;
        public int oYshift = 30;
        public string mainDirectory;

        public Graphics gCanvas;

        public string versionNum = "v1.00";
        public string fixedModule = "";
        public PrivateFontCollection myFonts; //CREATE A FONT COLLECTION
        public FontFamily family;
        public Font drawFontReg;
        public Font drawFontLarge;
        public Font drawFontSmall;
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
        public ScreenPcSelector screenPcSelector;
        public ScreenJournal screenJournal;
        public ScreenShop screenShop;
        public ScreenCastSelector screenCastSelector;
        public ScreenConvo screenConvo;
        public LogicTreeRun logicTreeRun;
        public ScreenTitle screenTitle;
        public ScreenPcCreation screenPcCreation;
        public ScreenSpellLevelUp screenSpellLevelUp;
        public ScreenTraitLevelUp screenTraitLevelUp;
        public ScreenLauncher screenLauncher;
        //public ScreenSplash screenSplash;
        public ScreenCombat screenCombat;
        public ScreenMainMap screenMainMap;
        public ScreenPartyBuild screenPartyBuild;
        public ScreenPartyRoster screenPartyRoster;
        public bool touchEnabled = true;
        //public AlertDialog ItemDialog;
        //public AlertDialog ActionDialog;
        //public MediaPlayer playerMain = new MediaPlayer();
        //public MediaPlayer playerAmbient = new MediaPlayer();
        //public MediaPlayer playerCombat = new MediaPlayer();
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

        //public bool logUpdated = false;
        //public int drawCount = 0;
        //public int mouseCount = 0;
                
        public GameView()
        {
            InitializeComponent();
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GameView_MouseWheel);
            mainDirectory = Directory.GetCurrentDirectory();

            this.IceBlinkButtonClose.setupAll(this);
            this.IceBlinkButtonResize.setupAll(this);
            try
            {
                playerButtonClick.SoundLocation = mainDirectory + "\\default\\NewModule\\sounds\\btn_click.wav";
                playerButtonClick.Load();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); } 
            try
            {
                playerButtonEnter.SoundLocation = mainDirectory + "\\default\\NewModule\\sounds\\btn_hover.wav";
                playerButtonEnter.Load();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

            this.WindowState = FormWindowState.Maximized;
            this.Width = 1366;
            this.Height = 768;
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
            if ((squareSize > 100) && (squareSize < 120))
            {
                squareSize = 100;
            }
            screenDensity = (float)squareSize / (float)squareSizeInPixels;
            oXshift = (screenWidth - (squareSize * squaresInWidth)) / 2;

            //CREATES A FONTFAMILY
            //(LOOK THE out word in the parameter sent to the method, that will modify myFonts object)
            family = LoadFontFamily(mainDirectory + "\\default\\NewModule\\fonts\\Metamorphous-Regular.ttf", out myFonts);
            float multiplr = (float)squareSize / 100.0f;
            drawFontLarge = new Font(family, 24.0f * multiplr);
            drawFontReg = new Font(family, 20.0f * multiplr);
            drawFontSmall = new Font(family, 16.0f * multiplr);
            
            animationTimer.Tick += new System.EventHandler(this.AnimationTimer_Tick);
            floatyTextTimer.Tick += new System.EventHandler(this.FloatyTextTimer_Tick);
            floatyTextMainMapTimer.Tick += new System.EventHandler(this.FloatyTextMainMapTimer_Tick);

            cc = new CommonCode(this);            
            mod = new Module();

            

            log = new IbbHtmlLogBox(this, 0 * squareSize + oXshift - 3, 0 * squareSize + oYshift, 6 * squareSize, 7 * squareSize);
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
            screenPcSelector = new ScreenPcSelector(mod, this);
		    screenJournal = new ScreenJournal(mod, this);	
		    screenShop = new ScreenShop(mod, this);
		    screenCastSelector = new ScreenCastSelector(mod, this);
		    screenConvo = new ScreenConvo(mod, this);
		    logicTreeRun = new LogicTreeRun(mod, this);
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
        }	
	    public void resetGame()
	    {
		    //mod = new Module();
		    mod = cc.LoadModule(mod.moduleName + ".mod", false);
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
		
	    public void startMusic()
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
	    public void startAmbient()
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
	    public void startCombatMusic()
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
		    //playerMain.pause();
	    }
	    public void stopAmbient()
	    {
		    //playerAmbient.pause();
	    }
	    public void stopCombatMusic()
	    {
		    //playerCombat.pause();
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
            
            /*
		    sounds = new SoundPool(10, AudioManager.STREAM_MUSIC, 0);
		
		    soundsList.clear();		
		    File sdCard = Environment.getExternalStorageDirectory();
		    File directory = new File (sdCard.getAbsolutePath() + "/IceBlinkRPG/" + mod.moduleName + "/sounds");
		    if (directory.exists())
		    {
			    for (File f : directory.listFiles()) 
			    {
		            if (f.isFile())
		            {
		        	    try
		        	    {
			        	    String filename = f.getName();
			        	    int pos = filename.lastIndexOf(".");
			        	    String fileNameWithOutExt = pos > 0 ? filename.substring(0, pos) : filename;
			        	    int soundID = sounds.load(sdCard.getAbsolutePath() + "/IceBlinkRPG/" + mod.moduleName + "/sounds/" + filename, 1);
			        	    soundsList.put(fileNameWithOutExt, soundID);		        	
		        	    }
		        	    catch (Exception ex)
		        	    {
		        		    int x = 0;
		        	    }
	        	    }
		        }
		    }
		    else
		    {
			    AssetManager assetManager = gameContext.getAssets();
	            String[] files;
			    try 
			    {
				    files = assetManager.list("sounds");
				    for (String filename : files)
		            {
					    //String filename = f.getName();
					    if (filename.endsWith(".wav"))
					    {
			        	    int pos = filename.lastIndexOf(".");
			        	    String fileNameWithOutExt = pos > 0 ? filename.substring(0, pos) : filename;
			                //tileBitmapList.put(fileNameWithOutExt, LoadBitmap(fileNameWithOutExt));
			                int soundID = sounds.load(assetManager.openFd("sounds/" + filename), 1);
			        	    soundsList.put(fileNameWithOutExt, soundID);	
					    }
		            }
			    } 
			    catch (IOException e) 
			    {
				    e.printStackTrace();
			    }	        
		    }
            */
	    }
	    public void PlaySound(string filenameNoExtension)
	    {
            //if (mod.playSoundFx)
            //{                
                if ((filenameNoExtension.Equals("none")) || (filenameNoExtension.Equals("")))
                {
                    //play nothing
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
                        if (mod.debugMode) //SD_20131102
                        {
                            cc.addLogText("<font color='yellow'>failed to play sound" + filenameNoExtension + "</font><BR>");
                        }
                        initializeSounds();
                    }
                }
            //}
            /*
		    if (mod.playSoundFx)
		    {
			    float rate = 1.0f;
			    if (mod.combatAnimationSpeed < 25) { rate = 2.0f; }
			    //play attack sound for melee
	    	    if ((filename.equals("none")) || (filename.equals("")))
	    	    {
	    		    //play nothing
	    	    }
	    	    else
	    	    {
	    		    try
	    		    {
	    			    //.SoundPool.play(int soundID, float leftVolume, float rightVolume, int priority, int loop, float rate)
	    			    sounds.play(soundsList.get(filename), mod.soundVolume, mod.soundVolume, 1, 0, rate);
	    		    }
	    		    catch (Exception ex)
	    		    { 
	    			    if (mod.debugMode) //SD_20131102
	                    {
	        			    cc.addLogText("<font color='yellow'>failed to play sound" + filename + "</font><BR>");
	        		    }
	    		    }
	    	    }
		    }
            */
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
            Invalidate();
            screenCombat.doAnimationController();
        }
        private void FloatyTextTimer_Tick(object sender, EventArgs e)
        {
            floatyTextTimer.Enabled = false;
            floatyTextTimer.Stop();
            Invalidate();

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
        }
        private void FloatyTextMainMapTimer_Tick(object sender, EventArgs e)
        {
            floatyTextMainMapTimer.Enabled = false;
            floatyTextMainMapTimer.Stop();
            Invalidate();

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
            log.onDrawLogBox(gCanvas);            
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
        
        public void DrawText(string text, int xLoc, int yLoc)
        {
            DrawText(text, xLoc, yLoc, 1.0f, Color.White);
        }
        public void DrawText(string text, int xLoc, int yLoc, float scaler, Color fontColor)
        {
            //ANDROID
            //canvas.drawText(text, xLoc + gv.oXshift, yLoc + txtH, gv.floatyTextPaint);
                //floatyTextPaint = new Paint();
		        //floatyTextPaint.setStyle(Paint.Style.FILL);
		        //floatyTextPaint.setColor(Color.YELLOW);
		        //floatyTextPaint.setAntiAlias(true);
                //Typeface uiTypeface = Typeface.createFromAsset(gameContext.getAssets(), "fonts/Metamorphous-Regular.ttf");
		        //floatyTextPaint.setTypeface(uiTypeface);
                //floatyTextPaint.setTextSize(squareSize/4);

            //PC
            //device.DrawString(crtRef.creatureTag, drawFont, drawBrush, new Point(cspx, cspy+25));
                //Font drawFont = new Font("Arial", 6);
                //SolidBrush drawBrush = new SolidBrush(Color.Yellow);

            Font thisFont = drawFontReg;
            if (scaler > 1.05f)
            {
                thisFont = drawFontLarge;
            }
            else if (scaler < 0.95f)
            {
                thisFont = drawFontSmall;
            }
            drawBrush.Color = fontColor;
            gCanvas.DrawString(text, thisFont, drawBrush, new Point(xLoc, yLoc + oYshift));
        }
        public void DrawText(string text, IbRect rect, float scaler, Color fontColor)
        {
            //ANDROID
            //canvas.drawText(text, xLoc + gv.oXshift, yLoc + txtH, gv.floatyTextPaint);
            //floatyTextPaint = new Paint();
            //floatyTextPaint.setStyle(Paint.Style.FILL);
            //floatyTextPaint.setColor(Color.YELLOW);
            //floatyTextPaint.setAntiAlias(true);
            //Typeface uiTypeface = Typeface.createFromAsset(gameContext.getAssets(), "fonts/Metamorphous-Regular.ttf");
            //floatyTextPaint.setTypeface(uiTypeface);
            //floatyTextPaint.setTextSize(squareSize/4);

            //PC
            //device.DrawString(crtRef.creatureTag, drawFont, drawBrush, new Point(cspx, cspy+25));
            //Font drawFont = new Font("Arial", 6);
            //SolidBrush drawBrush = new SolidBrush(Color.Yellow);

            Font thisFont = drawFontReg;
            if (scaler > 1.05f)
            {
                thisFont = drawFontLarge;
            }
            else if (scaler < 0.95f)
            {
                thisFont = drawFontSmall;
            }
            RectangleF rectF = new RectangleF(rect.Left, rect.Top + oYshift, rect.Width, rect.Height);
            drawBrush.Color = fontColor;
            gCanvas.DrawString(text, thisFont, drawBrush, rectF);
        }
        public void DrawRoundRectangle(IbRect rect, int rad, Color penColor, int penWidth)
        {
            //ANDROID canvas.drawRoundRect(new RectF(x + gv.oXshift, y, x + gv.squareSize + gv.oXshift + spellAoEinPixels + spellAoEinPixels, y + gv.squareSize + spellAoEinPixels + spellAoEinPixels), cornerRadius, cornerRadius, pnt);
            //PC      device.DrawRectangle(blackPen, target);
            
            GraphicsPath gp = new GraphicsPath();
            Pen p = new Pen(penColor, penWidth);

            gp.AddArc(rect.Left, rect.Top + oYshift, rad, rad, 180, 90);
            gp.AddArc(rect.Left + rect.Width - rad, rect.Top + oYshift, rad, rad, 270, 90);
            gp.AddArc(rect.Left + rect.Width - rad, rect.Top + oYshift + rect.Height - rad, rad, rad, 0, 90);
            gp.AddArc(rect.Left, rect.Top + oYshift + rect.Height - rad, rad, rad, 90, 90);
            gp.CloseFigure();            

            gCanvas.DrawPath(p, gp);

            p.Dispose();
            gp.Dispose();
        }   
        public void DrawLine(int lastX, int lastY, int nextX, int nextY, Color penColor, int penWidth)
        {
            Pen p = new Pen(penColor, penWidth);
            gCanvas.DrawLine(p, lastX, lastY, nextX, nextY);
            p.Dispose();
        }
        public void DrawBitmap(Bitmap bitmap, IbRect source, IbRect target)
        {
            //device.DrawImage(g_walkPass, target, src, GraphicsUnit.Pixel);
            //canvas.drawBitmap(gv.cc.turn_marker, src, dst, null);
            Rectangle tar = new Rectangle(target.Left, target.Top + oYshift, target.Width, target.Height);
            Rectangle src = new Rectangle(source.Left, source.Top, source.Width, source.Height);
            gCanvas.DrawImage(bitmap, tar, src, GraphicsUnit.Pixel);
        }    
        protected override void OnPaint(PaintEventArgs e)
	    {
            base.OnPaint(e);
            gCanvas = e.Graphics;

            //draw count stuff for debugging
            //DrawText("draws:" + drawCount.ToString(), 0, 0, 1.0f, Color.White);
            //DrawText("mouse:" + mouseCount.ToString(), 0, 15, 1.0f, Color.White);
            //drawCount++;
            
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
                    this.Invalidate();
                }
            }
            catch (Exception ex) 
            {
                //print exception    		
            }		
        }

        public void onKeyboardEvent(Keys keyData)
        {
            try
            {
                if (touchEnabled)
                {
                    if (screenType.Equals("main"))
                    {
                        screenMainMap.onKeyUp(keyData);
                        this.Invalidate();
                    }
                    else if (screenType.Equals("combat"))
                    {
                        screenCombat.onKeyUp(keyData);
                        this.Invalidate();
                    }
                    else if (screenType.Equals("convo"))
                    {
                        screenConvo.onKeyUp(keyData);
                        this.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                //print exception
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
    }
}
