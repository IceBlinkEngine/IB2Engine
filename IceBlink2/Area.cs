using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Text;
using System.IO;
//using IceBlink;
using System.ComponentModel;
using Newtonsoft.Json;

namespace IceBlink2
{
    public class Area 
    {
	    public string Filename = "newArea";
	    //public bool IsWorldMap = false;
        public bool UseMiniMapFogOfWar = true;
	    public bool areaDark = false;
	    public bool UseDayNightCycle = false;
        public bool useMiniProps = false;
        public bool useSuperTinyProps = false;
	    public int TimePerSquare = 6; //in minutes for now
	    public string MusicFileName = "forest.mp3";
	    public string ImageFileName = "none";
        public int backgroundImageStartLocX = 0;
        public int backgroundImageStartLocY = 0;
	    public int MapSizeX = 16;
	    public int MapSizeY = 16;
	    public string AreaMusic = "none";
	    public int AreaMusicDelay = 0;
	    public int AreaMusicDelayRandomAdder = 0;
	    public string AreaSounds = "none";
	    public int AreaSoundsDelay = 0;
	    public int AreaSoundsDelayRandomAdder = 0;
	    public List<Tile> Tiles = new List<Tile>();
	    public List<Prop> Props = new List<Prop>();
	    public List<string> InitialAreaPropTagsList = new List<string>();
	    public List<Trigger> Triggers = new List<Trigger>();
	    public int NextIdNumber = 100;
        public string OnHeartBeatIBScript = "none";
        public string OnHeartBeatIBScriptParms = "";
	    public List<LocalInt> AreaLocalInts = new List<LocalInt>();
	    public List<LocalString> AreaLocalStrings = new List<LocalString>();
        public string inGameAreaName = "newArea";
        //public string areaWeatherScript = "scotishAutumn";
        public string areaWeatherScript = "";
        public string areaWeatherScriptParms = "";
        public int weatherDurationMultiplierForScale = 1;

        //TODO use the inGameAreaName on main map

        #region full screen effect layers
        //properties for the full screen animation system (for 10 layers)
        #region full screen effect layer 1
        //full screen effect layer 1
        public bool useFullScreenEffectLayer1 = false;
        public bool fullScreenEffectLayerIsActive1 = true;
        //public string fullScreenEffectLayerName1 = "full_screen_effect_layer2x";
        //public string fullScreenEffectLayerName1 = "testAnim1";
        //public string fullScreenEffectLayerName1 = "fogLayerA";
        //public string fullScreenEffectLayerName1 = "cloudLayerA";
        public string fullScreenEffectLayerName1 = "snowLayerA";
        //public string fullScreenEffectLayerName1 = "rainLayerA";
        public float fullScreenAnimationSpeed1 = 1.0f;
        public float fullScreenAnimationSpeedX1 = 0.5f;
        public float fullScreenAnimationSpeedY1 = -1.15f;
        public bool FullScreenEffectLayer1IsTop = true;
        public int fullScreenEffectChanceToOccur1 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence1 = 0;
        public float cycleCounter1 = 0;
        public bool containEffectInsideAreaBorders1 = false;

        public bool isChanging1 = false;
        public float changeCounter1 = 0;
        public float changeLimit1 = 60;
        public float changeFrameCounter1 = 1;
        public float changeNumberOfFrames1 = 4;        
        public bool useCyclicFade1 = true;

        public float fullScreenAnimationFrameCounterX1 = 0;
        public float fullScreenAnimationFrameCounterY1 = 1.5f;
        public int fullScreenAnimationFrameCounter1 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (2 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (2 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (2 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride1 = "snow";
        public float overrideSpeedX1 = -100;
        public float overrideSpeedY1 = -100;
        public int overrideDelayLimit1 = -100;
        public int overrideDelayCounter1 = 10000;
        public string overrideIsNoScrollSource1 = "";
        public bool changeableByWeatherScript1 = true;

        #endregion
        #region full screen effect layer 2
        //full screen effect layer 2
        public bool useFullScreenEffectLayer2 = false;
        public bool fullScreenEffectLayerIsActive2 = true;
        //public string fullScreenEffectLayerName2 = "full_screen_effect_layer1x";
        //public string fullScreenEffectLayerName2 = "fogLayerB";
        public string fullScreenEffectLayerName2 = "snowLayerB";
        //public string fullScreenEffectLayerName2 = "rainLayerB";
        //public string fullScreenEffectLayerName2 = "testAnim1";
        public float fullScreenAnimationSpeed2 = 1.0f;
        public float fullScreenAnimationSpeedX2 = 0.5f;
        public float fullScreenAnimationSpeedY2 = -1.15f;
        public bool FullScreenEffectLayer2IsTop = true;
        public int fullScreenEffectChanceToOccur2 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence2 = 0;
        public float cycleCounter2 = 0;
        public bool containEffectInsideAreaBorders2 = false;

        public bool isChanging2 = false;
        public float changeCounter2 = 0;
        public float changeLimit2 = 60;
        public float changeFrameCounter2 = 1;
        public float changeNumberOfFrames2 = 4;

        public bool useCyclicFade2 = true;

        public float fullScreenAnimationFrameCounterX2 = 0;
        public float fullScreenAnimationFrameCounterY2 = 1;
        public int fullScreenAnimationFrameCounter2 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (2 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (2 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (2 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride2 = "snow";
        public float overrideSpeedX2 = -100;
        public float overrideSpeedY2 = - 100;
        public int overrideDelayLimit2 = -100;
        public int overrideDelayCounter2 = 10000;
        public string overrideIsNoScrollSource2 = "";
        public bool changeableByWeatherScript2 = true;

        #endregion
        #region full screen effect layer 3
        //full screen effect layer 2
        public bool useFullScreenEffectLayer3 = false;
        public bool fullScreenEffectLayerIsActive3 = true;
        //public string fullScreenEffectLayerName3 = "full_screen_effect_layer1x";
        //public string fullScreenEffectLayerName3 = "fogLayerB";
        public string fullScreenEffectLayerName3 = "cloudLayerA";
        //public string fullScreenEffectLayerName3 = "rainLayerB";
        //public string fullScreenEffectLayerName3 = "testAnim1";
        public float fullScreenAnimationSpeed3 = 1.0f;
        public float fullScreenAnimationSpeedX3 = 0.5f;
        public float fullScreenAnimationSpeedY3 = -1.15f;
        public bool FullScreenEffectLayer3IsTop = true;
        public int fullScreenEffectChanceToOccur3 = 100;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence3 = 0;
        public float cycleCounter3 = 0;
        public bool containEffectInsideAreaBorders3 = false;


        public bool isChanging3 = false;
        public float changeCounter3 = 0;
        public float changeLimit3 = 60;
        public float changeFrameCounter3 = 1;
        public float changeNumberOfFrames3 = 4;

        public bool useCyclicFade3 = true;

        public float fullScreenAnimationFrameCounterX3 = 0;
        public float fullScreenAnimationFrameCounterY3 = 1;
        public int fullScreenAnimationFrameCounter3 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (3 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (3 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (3 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride3 = "clouds";
        public float overrideSpeedX3 = -100;
        public float overrideSpeedY3 = -100;
        public int overrideDelayLimit3 = -100;
        public int overrideDelayCounter3 = 10000;
        public string overrideIsNoScrollSource3 = "";
        public bool changeableByWeatherScript3 = true;

        #endregion
        #region full screen effect layer 4
        //full screen effect layer 2
        public bool useFullScreenEffectLayer4 = false;
        public bool fullScreenEffectLayerIsActive4 = true;
        //public string fullScreenEffectLayerName4 = "full_screen_effect_layer1x";
        //public string fullScreenEffectLayerName4 = "fogLayerB";
        public string fullScreenEffectLayerName4 = "snowLayerB";
        //public string fullScreenEffectLayerName4 = "rainLayerB";
        //public string fullScreenEffectLayerName4 = "testAnim1";
        public float fullScreenAnimationSpeed4 = 1.0f;
        public float fullScreenAnimationSpeedX4 = 0.5f;
        public float fullScreenAnimationSpeedY4 = -1.15f;
        public bool FullScreenEffectLayer4IsTop = true;
        public int fullScreenEffectChanceToOccur4 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence4 = 0;
        public float cycleCounter4 = 0;
        public bool containEffectInsideAreaBorders4 = false;
        

        public bool isChanging4 = false;
        public float changeCounter4 = 0;
        public float changeLimit4 = 60;
        public float changeFrameCounter4 = 1;
        public float changeNumberOfFrames4 = 4;

        public bool useCyclicFade4 = true;

        public float fullScreenAnimationFrameCounterX4 = 0;
        public float fullScreenAnimationFrameCounterY4 = 1;
        public int fullScreenAnimationFrameCounter4 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (4 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (4 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (4 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride4 = "snow";
        public float overrideSpeedX4 = -100;
        public float overrideSpeedY4 = -100;
        public int overrideDelayLimit4 = -100;
        public int overrideDelayCounter4 = 10000;
        public string overrideIsNoScrollSource4 = "";
        public bool changeableByWeatherScript4 = true;

        #endregion
        #region full screen effect layer 5
        //full screen effect layer 2
        public bool useFullScreenEffectLayer5 = false;
        public bool fullScreenEffectLayerIsActive5 = true;
        //public string fullScreenEffectLayerName5 = "full_screen_effect_layer1x";
        //public string fullScreenEffectLayerName5 = "fogLayerB";
        public string fullScreenEffectLayerName5 = "snowLayerB";
        //public string fullScreenEffectLayerName5 = "rainLayerB";
        //public string fullScreenEffectLayerName5 = "testAnim1";
        public float fullScreenAnimationSpeed5 = 1.0f;
        public float fullScreenAnimationSpeedX5 = 0.5f;
        public float fullScreenAnimationSpeedY5 = -1.15f;
        public bool FullScreenEffectLayer5IsTop = true;
        public int fullScreenEffectChanceToOccur5 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence5 = 0;
        public float cycleCounter5 = 0;
        public bool containEffectInsideAreaBorders5 = false;

        public bool isChanging5 = false;
        public float changeCounter5 = 0;
        public float changeLimit5 = 60;
        public float changeFrameCounter5 = 1;
        public float changeNumberOfFrames5 = 4;

        public bool useCyclicFade5 = true;

        public float fullScreenAnimationFrameCounterX5 = 0;
        public float fullScreenAnimationFrameCounterY5 = 1;
        public int fullScreenAnimationFrameCounter5 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (5 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (5 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (5 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride5 = "snow";
        public float overrideSpeedX5 = -100;
        public float overrideSpeedY5 = -100;
        public int overrideDelayLimit5 = -100;
        public int overrideDelayCounter5 = 10000;
        public string overrideIsNoScrollSource5 = "";
        public bool changeableByWeatherScript5 = true;

        #endregion
        #region full screen effect layer 6
        //full screen effect layer 2
        public bool useFullScreenEffectLayer6 = false;
        public bool fullScreenEffectLayerIsActive6 = true;
        //public string fullScreenEffectLayerName6 = "full_screen_effect_layer1x";
        //public string fullScreenEffectLayerName6 = "fogLayerB";
        public string fullScreenEffectLayerName6 = "snowLayerB";
        //public string fullScreenEffectLayerName6 = "rainLayerB";
        //public string fullScreenEffectLayerName6 = "testAnim1";
        public float fullScreenAnimationSpeed6 = 1.0f;
        public float fullScreenAnimationSpeedX6 = 0.5f;
        public float fullScreenAnimationSpeedY6 = -1.15f;
        public bool FullScreenEffectLayer6IsTop = true;
        public int fullScreenEffectChanceToOccur6 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence6 = 0;
        public float cycleCounter6 = 0;
        public bool containEffectInsideAreaBorders6 = false;

        public bool isChanging6 = false;
        public float changeCounter6 = 0;
        public float changeLimit6 = 60;
        public float changeFrameCounter6 = 1;
        public float changeNumberOfFrames6 = 4;

        public bool useCyclicFade6 = true;

        public float fullScreenAnimationFrameCounterX6 = 0;
        public float fullScreenAnimationFrameCounterY6 = 1;
        public int fullScreenAnimationFrameCounter6 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (6 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (6 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (6 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride6 = "snow";
        public float overrideSpeedX6 = -100;
        public float overrideSpeedY6 = -100;
        public int overrideDelayLimit6 = -100;
        public int overrideDelayCounter6 = 10000;
        public string overrideIsNoScrollSource6 = "";
        public bool changeableByWeatherScript6 = true;

        #endregion
        #region full screen effect layer 7
        //full screen effect layer 2
        public bool useFullScreenEffectLayer7 = false;
        public bool fullScreenEffectLayerIsActive7 = true;
        //public string fullScreenEffectLayerName7 = "full_screen_effect_layer1x";
        //public string fullScreenEffectLayerName7 = "fogLayerB";
        public string fullScreenEffectLayerName7 = "snowLayerB";
        //public string fullScreenEffectLayerName7 = "rainLayerB";
        //public string fullScreenEffectLayerName7 = "testAnim1";
        public float fullScreenAnimationSpeed7 = 1.0f;
        public float fullScreenAnimationSpeedX7 = 0.5f;
        public float fullScreenAnimationSpeedY7 = -1.15f;
        public bool FullScreenEffectLayer7IsTop = true;
        public int fullScreenEffectChanceToOccur7 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence7 = 0;
        public float cycleCounter7 = 0;
        public bool containEffectInsideAreaBorders7 = false;

        public bool isChanging7 = false;
        public float changeCounter7 = 0;
        public float changeLimit7 = 60;
        public float changeFrameCounter7 = 1;
        public float changeNumberOfFrames7 = 4;

        public bool useCyclicFade7 = true;

        public float fullScreenAnimationFrameCounterX7 = 0;
        public float fullScreenAnimationFrameCounterY7 = 1;
        public int fullScreenAnimationFrameCounter7 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (7 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (7 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (7 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride7 = "snow";
        public float overrideSpeedX7 = -100;
        public float overrideSpeedY7 = -100;
        public int overrideDelayLimit7 = -100;
        public int overrideDelayCounter7 = 10000;
        public string overrideIsNoScrollSource7 = "";
        public bool changeableByWeatherScript7 = true;

        #endregion
        #region full screen effect layer 8
        //full screen effect layer 2
        public bool useFullScreenEffectLayer8 = false;
        public bool fullScreenEffectLayerIsActive8 = true;
        //public string fullScreenEffectLayerName8 = "full_screen_effect_layer1x";
        //public string fullScreenEffectLayerName8 = "fogLayerB";
        public string fullScreenEffectLayerName8 = "snowLayerB";
        //public string fullScreenEffectLayerName8 = "rainLayerB";
        //public string fullScreenEffectLayerName8 = "testAnim1";
        public float fullScreenAnimationSpeed8 = 1.0f;
        public float fullScreenAnimationSpeedX8 = 0.5f;
        public float fullScreenAnimationSpeedY8 = -1.15f;
        public bool FullScreenEffectLayer8IsTop = true;
        public int fullScreenEffectChanceToOccur8 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence8 = 0;
        public float cycleCounter8 = 0;
        public bool containEffectInsideAreaBorders8 = false;

        public bool isChanging8 = false;
        public float changeCounter8 = 0;
        public float changeLimit8 = 60;
        public float changeFrameCounter8 = 1;
        public float changeNumberOfFrames8 = 4;

        public bool useCyclicFade8 = true;

        public float fullScreenAnimationFrameCounterX8 = 0;
        public float fullScreenAnimationFrameCounterY8 = 1;
        public int fullScreenAnimationFrameCounter8 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (8 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (8 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (8 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride8 = "snow";
        public float overrideSpeedX8 = -100;
        public float overrideSpeedY8 = -100;
        public int overrideDelayLimit8 = -100;
        public int overrideDelayCounter8 = 10000;
        public string overrideIsNoScrollSource8 = "";
        public bool changeableByWeatherScript8 = true;

        #endregion
        #region full screen effect layer 9
        //full screen effect layer 2
        public bool useFullScreenEffectLayer9 = false;
        public bool fullScreenEffectLayerIsActive9 = true;
        //public string fullScreenEffectLayerName9 = "full_screen_effect_layer1x";
        //public string fullScreenEffectLayerName9 = "fogLayerB";
        public string fullScreenEffectLayerName9 = "snowLayerB";
        //public string fullScreenEffectLayerName9 = "rainLayerB";
        //public string fullScreenEffectLayerName9 = "testAnim1";
        public float fullScreenAnimationSpeed9 = 1.0f;
        public float fullScreenAnimationSpeedX9 = 0.5f;
        public float fullScreenAnimationSpeedY9 = -1.15f;
        public bool FullScreenEffectLayer9IsTop = true;
        public int fullScreenEffectChanceToOccur9 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence9 = 0;
        public float cycleCounter9 = 0;
        public bool containEffectInsideAreaBorders9 = false;

        public bool isChanging9 = false;
        public float changeCounter9 = 0;
        public float changeLimit9 = 60;
        public float changeFrameCounter9 = 1;
        public float changeNumberOfFrames9 = 4;

        public bool useCyclicFade9 = true;

        public float fullScreenAnimationFrameCounterX9 = 0;
        public float fullScreenAnimationFrameCounterY9 = 1;
        public int fullScreenAnimationFrameCounter9 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (9 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (9 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (9 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride9 = "snow";
        public float overrideSpeedX9 = -100;
        public float overrideSpeedY9 = -100;
        public int overrideDelayLimit9 = -100;
        public int overrideDelayCounter9 = 10000;
        public string overrideIsNoScrollSource9 = "";
        public bool changeableByWeatherScript9 = true;

        #endregion
        #region full screen effect layer 10
        //full screen effect layer 2
        public bool useFullScreenEffectLayer10 = false;
        public bool fullScreenEffectLayerIsActive10 = true;
        //public string fullScreenEffectLayerName10 = "full_screen_effect_layer1x";
        //public string fullScreenEffectLayerName10 = "fogLayerB";
        public string fullScreenEffectLayerName10 = "snowLayerB";
        //public string fullScreenEffectLayerName10 = "rainLayerB";
        //public string fullScreenEffectLayerName10 = "testAnim1";
        public float fullScreenAnimationSpeed10 = 1.0f;
        public float fullScreenAnimationSpeedX10 = 0.5f;
        public float fullScreenAnimationSpeedY10 = -1.15f;
        public bool FullScreenEffectLayer10IsTop = true;
        public int fullScreenEffectChanceToOccur10 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence10 = 0;
        public float cycleCounter10 = 0;
        public bool containEffectInsideAreaBorders10 = false;

        public bool isChanging10 = false;
        public float changeCounter10 = 0;
        public float changeLimit10 = 60;
        public float changeFrameCounter10 = 1;
        public float changeNumberOfFrames10 = 4;

        public bool useCyclicFade10 = true;

        public float fullScreenAnimationFrameCounterX10 = 0;
        public float fullScreenAnimationFrameCounterY10 = 1;
        public int fullScreenAnimationFrameCounter10 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (10 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (10 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (10 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride10 = "snow";
        public float overrideSpeedX10 = -100;
        public float overrideSpeedY10 = -100;
        public int overrideDelayLimit10 = -100;
        public int overrideDelayCounter10 = 10000;
        public string overrideIsNoScrollSource10 = "";
        public bool changeableByWeatherScript10 = true;

        #endregion
        #endregion


        public Area()
	    {	
	    }
	
	    public bool GetBlocked(int playerXPosition, int playerYPosition)
        {        
            if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].Walkable == false)
            {
                return true;
            }
            foreach (Prop p in this.Props)
            {
                if ((p.LocationX == playerXPosition) && (p.LocationY == playerYPosition))
                {
                    if (p.HasCollision)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
	
	    public Trigger getTriggerByLocation(int x, int y)
        {
            foreach (Trigger t in this.Triggers)
            {
                foreach (Coordinate p in t.TriggerSquaresList)
                {
                    if ((p.X == x) && (p.Y == y))
                    {
                        return t;
                    }
                }
            }
            return null;
        }
	    public Trigger getTriggerByTag(String tag)
        {
            foreach (Trigger t in this.Triggers)
            {
                if (t.TriggerTag.Equals(tag))
                {
            	    return t;
                }
            }
            return null;
        }
	    public Prop getPropByLocation(int x, int y)
        {
            foreach (Prop p in this.Props)
            {
                if ((p.LocationX == x) && (p.LocationY == y))
                {
                    return p;
                }            
            }
            return null;
        }
	    public Prop getPropByTag(String tag)
        {
            foreach (Prop p in this.Props)
            {
                if (p.PropTag.Equals(tag))
                {
            	    return p;
                }
            }
            return null;
        }
    }
}
