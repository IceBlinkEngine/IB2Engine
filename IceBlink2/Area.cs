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
        //public string rememberedWeatherName = "";
        //public float rememberedWeatherDuration = 0;
        //public float skyCoverCloudsChance = 0;
        //public float skyCoverSeveriy

        public int linkedAreasCounter = 0;
        public string masterOfThisArea = "none";
        public List<string> linkedAreas = new List<string>();
        public List<int> linkNumbers = new List<int>();
        public int linkNumberOfThisArea = -1;

        public int averageHeightOnThisMap = 0; 

        public bool PlayerIsUnderBridge = false;
        public string Filename = "newArea";
        public int AreaVisibleDistance = 4;
        public bool RestingAllowed = false;
        public bool UseMiniMapFogOfWar = true;
	    public bool areaDark = false;
	    public bool UseDayNightCycle = false;
        public bool useMiniProps = false;
        public bool useSuperTinyProps = false;
	    public int TimePerSquare = 6; //in minutes for now
        //Music file name not used
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
        //[JsonIgnore]
        //[JsonProperty(Required = Required.Always)]
        //[JsonObject(]
	    public List<Tile> Tiles = new List<Tile>();
	    public List<Prop> Props = new List<Prop>();
	    public List<string> InitialAreaPropTagsList = new List<string>();
	    public List<Trigger> Triggers = new List<Trigger>();
	    public int NextIdNumber = 100;
        public string OnHeartBeatIBScript = "none";
        public string OnHeartBeatIBScriptParms = "";
	    public List<LocalInt> AreaLocalInts = new List<LocalInt>();
	    public List<LocalString> AreaLocalStrings = new List<LocalString>();
        public string inGameAreaName = "";
        public string areaWeatherScript = "";
        public string areaWeatherScriptParms = "";
        public string effectChannelScript1 = "";
        public string effectChannelScript2 = "";
        public string effectChannelScript3 = "";
        public string effectChannelScript4 = "";
        public string effectChannelScriptParms1 = "";
        public string effectChannelScriptParms2 = "";
        public string effectChannelScriptParms3 = "";
        public string effectChannelScriptParms4 = "";
        //public WeatherEffect areaWeather = new WeatherEffect();
        public string areaWeatherName = "";
        public int weatherDurationMultiplierForScale = 1;
        public string westernNeighbourArea = "";
        public string easternNeighbourArea = "";
        public string northernNeighbourArea = "";
        public string southernNeighbourArea = "";

        public bool useLightSystem = false;
        public float flickerSlowDownFactor = 1f;
        public float shifterSlowDownFactor = 1f;
        public bool noFlicker = false;
        public bool noPositionShift = false;
        public float minimumDarkness = 12;
        public float maxLightMultiplier = 1;


        public string sourceBitmapName = "";

        public List<bool> tileVisibilityList = new List<bool>();

        //TODO use the inGameAreaName on main map

        #region full screen effect layers
        //properties for the full screen animation system (for 10 layers)
        #region full screen effect layer 1
        //this layer is best used for very bottom level animation, typically the sea, but could e.g. be a scrolling starfield in space, too 
        //full screen effect layer 1
        public bool useFullScreenEffectLayer1 = false;
        public bool fullScreenEffectLayerIsActive1 = true;
        public string fullScreenEffectLayerName1 = "sea";
        public float fullScreenAnimationSpeed1 = 1.0f;
        public float fullScreenAnimationSpeedX1 = 0.5f;
        public float fullScreenAnimationSpeedY1 = -1.15f;
        public bool FullScreenEffectLayer1IsTop = false;
        public int fullScreenEffectChanceToOccur1 = 100;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence1 = 0;
        public float cycleCounter1 = 0;
        public bool containEffectInsideAreaBorders1 = false;
        public int activateTargetChannelInParallelToThisChannel1 = 0;

        public bool isChanging1 = true;
        public float changeCounter1 = 0;
        public float changeLimit1 = 15;
        public float changeFrameCounter1 = 1;
        public float changeNumberOfFrames1 = 6;        
        public bool useCyclicFade1 = false;

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
        public string directionalOverride1 = "fog";
        public float overrideSpeedX1 = -100f;
        public float overrideSpeedY1 = -100;
        public int overrideDelayLimit1 = 40;
        public int overrideDelayCounter1 = 10000;
        public string overrideIsNoScrollSource1 = "";
        public bool changeableByWeatherScript1 = false;

        #endregion
        #region full screen effect layer 2
        //full screen effect layer 2
        public bool useFullScreenEffectLayer2 = false;
        public bool fullScreenEffectLayerIsActive2 = true;
        //public string fullScreenEffectLayerName2 = "full_screen_effect_layer1x";
        //public string fullScreenEffectLayerName2 = "fogLayerB";
        public string fullScreenEffectLayerName2 = "pixShadow";
        //public string fullScreenEffectLayerName2 = "rainLayerB";
        //public string fullScreenEffectLayerName2 = "testAnim1";
        public float fullScreenAnimationSpeed2 = 1.0f;
        public float fullScreenAnimationSpeedX2 = 0f;
        public float fullScreenAnimationSpeedY2 = 0f;
        public bool FullScreenEffectLayer2IsTop = true;
        public int fullScreenEffectChanceToOccur2 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence2 = 0;
        public float cycleCounter2 = 0;
        public bool containEffectInsideAreaBorders2 = false;
        public int activateTargetChannelInParallelToThisChannel2 = 0;

        public bool isChanging2 = true;
        public float changeCounter2 = 0;
        public float changeLimit2 = 4;
        public float changeFrameCounter2 = 1;
        public float changeNumberOfFrames2 = 10;

        public bool useCyclicFade2 = false;

        public float fullScreenAnimationFrameCounterX2 = 0;
        public float fullScreenAnimationFrameCounterY2 = 0;
        public int fullScreenAnimationFrameCounter2 = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (2 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (2 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (2 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride2 = "linear";
        public float overrideSpeedX2 = 0f;
        public float overrideSpeedY2 = 0f;
        public int overrideDelayLimit2 = -100;
        public int overrideDelayCounter2 = 10000;
        public string overrideIsNoScrollSource2 = "";
        public bool changeableByWeatherScript2 = false;
        public bool drawWithLessVisibleSeamsButMorePixelated = false;

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
        public int activateTargetChannelInParallelToThisChannel3 = 0;

        public bool isChanging3 = false;
        public float changeCounter3 = 0;
        public float changeLimit3 = 60;
        public float changeFrameCounter3 = 1;
        public float changeNumberOfFrames3 = 4;

        public bool useCyclicFade3 = true;

        public float fullScreenAnimationFrameCounterX3 = 0;
        public float fullScreenAnimationFrameCounterY3 = 0;
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
        public int activateTargetChannelInParallelToThisChannel4 = 0;


        public bool isChanging4 = false;
        public float changeCounter4 = 0;
        public float changeLimit4 = 60;
        public float changeFrameCounter4 = 1;
        public float changeNumberOfFrames4 = 4;

        public bool useCyclicFade4 = true;

        public float fullScreenAnimationFrameCounterX4 = 0;
        public float fullScreenAnimationFrameCounterY4 = 0;
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
        public int activateTargetChannelInParallelToThisChannel5 = 0;

        public bool isChanging5 = false;
        public float changeCounter5 = 0;
        public float changeLimit5 = 60;
        public float changeFrameCounter5 = 1;
        public float changeNumberOfFrames5 = 4;

        public bool useCyclicFade5 = true;

        public float fullScreenAnimationFrameCounterX5 = 0;
        public float fullScreenAnimationFrameCounterY5 = 0;
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
        public int activateTargetChannelInParallelToThisChannel6 = 0;

        public bool isChanging6 = false;
        public float changeCounter6 = 0;
        public float changeLimit6 = 60;
        public float changeFrameCounter6 = 1;
        public float changeNumberOfFrames6 = 4;

        public bool useCyclicFade6 = true;

        public float fullScreenAnimationFrameCounterX6 = 0;
        public float fullScreenAnimationFrameCounterY6 = 0;
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
        public int activateTargetChannelInParallelToThisChannel7 = 0;

        public bool isChanging7 = false;
        public float changeCounter7 = 0;
        public float changeLimit7 = 60;
        public float changeFrameCounter7 = 1;
        public float changeNumberOfFrames7 = 4;

        public bool useCyclicFade7 = true;

        public float fullScreenAnimationFrameCounterX7 = 0;
        public float fullScreenAnimationFrameCounterY7 = 0;
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
        public int activateTargetChannelInParallelToThisChannel8 = 0;

        public bool isChanging8 = false;
        public float changeCounter8 = 0;
        public float changeLimit8 = 60;
        public float changeFrameCounter8 = 1;
        public float changeNumberOfFrames8 = 4;

        public bool useCyclicFade8 = true;

        public float fullScreenAnimationFrameCounterX8 = 0;
        public float fullScreenAnimationFrameCounterY8 = 0;
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
        public int activateTargetChannelInParallelToThisChannel9 = 0;

        public bool isChanging9 = false;
        public float changeCounter9 = 0;
        public float changeLimit9 = 60;
        public float changeFrameCounter9 = 1;
        public float changeNumberOfFrames9 = 4;

        public bool useCyclicFade9 = true;

        public float fullScreenAnimationFrameCounterX9 = 0;
        public float fullScreenAnimationFrameCounterY9 = 0;
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
        public int activateTargetChannelInParallelToThisChannel10 = 0;

        public bool isChanging10 = false;
        public float changeCounter10 = 0;
        public float changeLimit10 = 60;
        public float changeFrameCounter10 = 1;
        public float changeNumberOfFrames10 = 4;

        public bool useCyclicFade10 = true;

        public float fullScreenAnimationFrameCounterX10 = 0;
        public float fullScreenAnimationFrameCounterY10 = 0;
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
	
	    public bool GetBlocked(int playerXPosition, int playerYPosition, int lastPlayerXPosition, int lastPlayerYPosition, int lastLastPlayerXPosition, int lastLastPlayerYPosition)
        {
            if ((this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].isEWBridge) || (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].isNSBridge))
            {
                if ((this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel) < (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel))
                {
                    this.PlayerIsUnderBridge = false;
                }
            }
            else
            {
                this.PlayerIsUnderBridge = false;
            }

            if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].Walkable == false)
            {
                return true;
            }
            
            if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel != this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel)
            {
                bool allowMove = false;

                //enter transition to link from master section
                if ((this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].transitionToMasterDirection != "none") && (this.masterOfThisArea == "none"))
                {
                    //let us first sort by intended direction of palyer move

                    //stepping toward north square
                    if (lastPlayerYPosition - 1 == playerYPosition)
                    {
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].transitionToMasterDirection == "S")
                        {
                            if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel + 1 == this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel)
                            {
                                allowMove = true;
                            }
                        }
                    }

                    //stepping toward south square
                    if (lastPlayerYPosition + 1 == playerYPosition)
                    {
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].transitionToMasterDirection == "N")
                        {
                            if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel + 1 == this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel)
                            {
                                allowMove = true;
                            }
                        }
                    }

                    //stepping toward east square
                    if (lastPlayerXPosition + 1 == playerXPosition)
                    {
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].transitionToMasterDirection == "W")
                        {
                            if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel + 1 == this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel)
                            {
                                allowMove = true;
                            }
                        }
                    }

                    //stepping toward west square
                    if (lastPlayerXPosition - 1 == playerXPosition)
                    {
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].transitionToMasterDirection == "E")
                        {
                            if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel + 1 == this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel)
                            {
                                allowMove = true;
                            }
                        }
                    }

                }

                //ramp section
                //bool allowMove = false;
                //player is on ramp and climbs down
                if ((this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].isRamp) && (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel + 1 == this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel))
                {
                    //only allow if the player is not rying to climb down via high end of ramp
                    //let us first sort by intended direction of palyer move
                    
                    //stepping toward north square
                    if (lastPlayerYPosition - 1 == playerYPosition)
                    {
                        //make sure north end of current ramp squre is not high
                        if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].hasDownStairShadowS)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }

                    //stepping toward south square
                    if (lastPlayerYPosition + 1 == playerYPosition)
                    {
                        //make sure south end of current ramp squre is not high
                        if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].hasDownStairShadowN)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }

                   
                    //stepping toward east square
                    if (lastPlayerXPosition + 1 == playerXPosition)
                    {
                        //make sure east end of current ramp squre is not high
                        if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].hasDownStairShadowW)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }

                    //stepping toward west square
                    if (lastPlayerXPosition - 1 == playerXPosition)
                    {
                        //make sure west end of current ramp squre is not high
                        if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].hasDownStairShadowE)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }
                }

                //player enters ramp and climbs up
                if ((this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].isRamp) && (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel - 1 == this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel))
                {
                    //we must check that target ramp is not facing toward player with high side
                    //stepping toward north square
                    if (lastPlayerYPosition - 1 == playerYPosition)
                    {
                        //make sure north end of target ramp squre is not high
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].hasDownStairShadowN)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }

                    //stepping toward south square
                    if (lastPlayerYPosition + 1 == playerYPosition)
                    {
                        //make sure north end of target ramp squre is not high
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].hasDownStairShadowS)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }


                    //stepping toward east square
                    if (lastPlayerXPosition + 1 == playerXPosition)
                    {
                        //make sure west end of target ramp squre is not high
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].hasDownStairShadowE)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }

                    //stepping toward west square
                    if (lastPlayerXPosition - 1 == playerXPosition)
                    {
                        //make sure east end of target ramp squre is not high
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].hasDownStairShadowW)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }

                    //allowMove = true;
                }

                //EW bridge: player tries to leave bridge to NS side
                if ((this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].isEWBridge))
                {
                    //this.PlayerIsUnderBridge = true;
                    //stepping toward north square
                    if (lastPlayerYPosition - 1 == playerYPosition)
                    {
                        //came from north or south and is under bridge now
                        if ((lastLastPlayerYPosition - 1 == lastPlayerYPosition) || (lastLastPlayerYPosition + 1 == lastPlayerYPosition))
                        {
                            //allow the move under the bridge (which is a move to one height level lower)
                            allowMove = true;
                        }
                    }

                    //stepping toward south square
                    if (lastPlayerYPosition + 1 == playerYPosition)
                    {
                        //came from north and is under bridge now
                        if ((lastLastPlayerYPosition - 1 == lastPlayerYPosition) || (lastLastPlayerYPosition + 1 == lastPlayerYPosition))
                        {
                            //allow the move under the bridge (which is a move to one height level lower)
                            allowMove = true;
                        }
                    }
                }

                //NS bridge: player tries to leave bridge to EW side
                if ((this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].isNSBridge))
                {
                    //this.PlayerIsUnderBridge = true;
                    //stepping toward western square
                    if (lastPlayerXPosition - 1 == playerXPosition)
                    {
                        //came from west or east and is under bridge now
                        if ((lastLastPlayerXPosition - 1 == lastPlayerXPosition) || (lastLastPlayerXPosition + 1 == lastPlayerXPosition))
                        {
                            //allow the move under the bridge (which is a move to one height level lower)
                            allowMove = true;
                        }
                    }

                    //stepping toward eastern square
                    if (lastPlayerXPosition + 1 == playerXPosition)
                    {
                        //came from west or east and is under bridge now
                        if ((lastLastPlayerXPosition - 1 == lastPlayerXPosition) || (lastLastPlayerXPosition + 1 == lastPlayerXPosition))
                        {
                            //allow the move under the bridge (which is a move to one height level lower)
                            allowMove = true;
                        }
                    }
                }

                //Player tries to go under a bridge (with strict map building rules always possible)
                //note: on top of bridge scenario is filtered out by beginning condition of height level difference
                if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].isEWBridge || this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].isNSBridge)
                {
                    this.PlayerIsUnderBridge = true;
                    allowMove = true;
                }

                if (!allowMove)
                {
                    return true;
                }        
            }
            
            //same height
           else
           {
                bool allowMove = true;
                
                //NS bridge: player tries to leave bridge to NS side
                if ((this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].isNSBridge))
                {
                    //stepping toward northern square
                    if (lastPlayerYPosition - 1 == playerYPosition)
                    {
                        //came from west or east and is under bridge now
                        if ((lastLastPlayerXPosition - 1 == lastPlayerXPosition) || (lastLastPlayerXPosition + 1 == lastPlayerXPosition))
                        {
                            //prevent bridge climbing from under the bridge
                            allowMove = false;
                        }
                    }

                    //stepping toward southern square
                    if (lastPlayerYPosition + 1 == playerYPosition)
                    {
                        //came from west or east and is under bridge now
                        if ((lastLastPlayerXPosition - 1 == lastPlayerXPosition) || (lastLastPlayerXPosition + 1 == lastPlayerXPosition))
                        {
                            //prevent bridge climbing from under the bridge
                            allowMove = false;
                        }
                    }
                }

                //EW bridge: player tries to leave bridge to EW side
                if ((this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].isEWBridge))
                {
                    //stepping toward western square
                    if (lastPlayerXPosition - 1 == playerXPosition)
                    {
                        //came from north or south and is under bridge now
                        if ((lastLastPlayerYPosition - 1 == lastPlayerYPosition) || (lastLastPlayerYPosition + 1 == lastPlayerYPosition))
                        {
                            //prevent bridge climbing from under the bridge
                            allowMove = false;
                        }
                    }

                    //stepping toward eastern square
                    if (lastPlayerXPosition + 1 == playerXPosition)
                    {
                        //came from north or south and is under bridge now
                        if ((lastLastPlayerYPosition - 1 == lastPlayerYPosition) || (lastLastPlayerYPosition + 1 == lastPlayerYPosition))
                        {
                            //prevent bridge climbing from under the bridge
                            allowMove = false;
                        }
                    }
                }

                //cannot enter same height square when leaving over botttom end of current ramp square
                if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].isRamp)
                {
                    //stepping toward north square
                    if (lastPlayerYPosition - 1 == playerYPosition)
                    {
                        //make sure north end of target ramp squre is not high
                        if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].hasDownStairShadowN)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }

                    //stepping toward south square
                    if (lastPlayerYPosition + 1 == playerYPosition)
                    {
                        //make sure north end of target ramp squre is not high
                        if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].hasDownStairShadowS)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }


                    //stepping toward east square
                    if (lastPlayerXPosition + 1 == playerXPosition)
                    {
                        //make sure west end of target ramp squre is not high
                        if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].hasDownStairShadowE)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }

                    //stepping toward west square
                    if (lastPlayerXPosition - 1 == playerXPosition)
                    {
                        //make sure east end of target ramp squre is not high
                        if (this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].hasDownStairShadowW)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }
                }

                //cannot enter same height square when entering over botttom end of target ramp square
                if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].isRamp)
                {
                    //stepping toward north square
                    if (lastPlayerYPosition - 1 == playerYPosition)
                    {
                        //make sure north end of target ramp squre is not high
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].hasDownStairShadowS)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }

                    //stepping toward south square
                    if (lastPlayerYPosition + 1 == playerYPosition)
                    {
                        //make sure north end of target ramp squre is not high
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].hasDownStairShadowN)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }


                    //stepping toward east square
                    if (lastPlayerXPosition + 1 == playerXPosition)
                    {
                        //make sure west end of target ramp squre is not high
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].hasDownStairShadowW)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }

                    //stepping toward west square
                    if (lastPlayerXPosition - 1 == playerXPosition)
                    {
                        //make sure east end of target ramp squre is not high
                        if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].hasDownStairShadowE)
                        {

                        }
                        else
                        {
                            allowMove = true;
                        }
                    }
                }

                if (!allowMove)
                {
                    //block
                    return true;
                }
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

        public bool GetBlocked(int playerXPosition, int playerYPosition)
        {
            if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].Walkable == false)
            {
                return true;
            }

            /*
            if (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel != this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel)
            {
                //ramp section

                bool rampAllowsHeightTraversal = false;

                //player is on ramp and climbs down
                if ((this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].isRamp) && (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel + 1 == this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel))
                {
                    rampAllowsHeightTraversal = true;
                }

                //player enters ramp and climbs up
                if ((this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].isRamp) && (this.Tiles[playerYPosition * this.MapSizeX + playerXPosition].heightLevel - 1 == this.Tiles[lastPlayerYPosition * this.MapSizeX + lastPlayerXPosition].heightLevel))
                {
                    rampAllowsHeightTraversal = true;
                }

                if (!rampAllowsHeightTraversal)
                {
                    return true;
                }
            }
            */

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
