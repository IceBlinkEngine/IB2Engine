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
        
        //for later, not used right now :-)
        public bool useWeather = true;
        public int currentWeatherFrame = 0;
        public string currentWeatherType = "clear";
        public int chanceForLightRain = 0;
        public int chanceForMediumRain = 0;
        public int chanceForHeavyRain = 0;
        public int chanceForThunderStorm = 0;
        public int chanceForHeavyThunderstorm = 0;
        public int chanceForLightSnow = 0;
        public int chanceForMediumSnow = 0;
        public int chanceForHeavySnow = 0;
        public int chanceForLightFog = 0;
        public int chanceForMediumFog = 0;
        public int chanceForHeavyFog = 0;
        public bool useMiniProps = false;
        public bool useSuperTinyProps = false;

        public int numberOfHoursBeforeRollForWeatherChange = 0;
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
	    //public string OnHeartBeatLogicTree = "none";
	    //public string OnHeartBeatParms = "";
        public string OnHeartBeatIBScript = "none";
        public string OnHeartBeatIBScriptParms = "";
	    public List<LocalInt> AreaLocalInts = new List<LocalInt>();
	    public List<LocalString> AreaLocalStrings = new List<LocalString>();
        public string inGameAreaName = "newArea";
        //TODO use the inGameAreaName on main map

        #region full screen effect layers
        //properties for the full screen animation system (for 6 layers)
        #region full screen effect layer 1
        //full screen effect layer 1
        public bool useFullScreenEffectLayer1 = false;
        public bool fullScreenEffectLayerIsActive1 = true;
        public string fullScreenEffectLayerName1 = "fogLayerA";
        //public string fullScreenEffectLayerName1 = "testAnim1";
        public float fullScreenAnimationSpeed1 = 1.0f;
        public float fullScreenAnimationSpeedX1 = 0.5f;
        public float fullScreenAnimationSpeedY1 = -1.15f;
        public bool FullScreenEffectLayer1IsTop = true;
        public int fullScreenEffectChanceToOccur1 = 20;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence1 = 0;
        public float cycleCounter1 = 0;
        public bool containEffectInsideAreaBorders1 = false;
        public int numberOfRenderCallsBeforeRedirection1 = 150;

        public bool isChanging1 = false;
        public float changeCounter1 = 0;
        public float changeLimit1 = 60;
        public float changeFrameCounter1 = 1;
        public float changeNumberOfFrames1 = 4;

        //public int numberOfRenderCallsforRandomCounter1 = 10000000;
       
        
        public bool useCyclicFade1 = true;

        public float fullScreenAnimationFrameCounterX1 = 0;
        public float fullScreenAnimationFrameCounterY1 = 0;
        public int fullScreenAnimationFrameCounter1 = 0;

        //setting up the override movement patterns
        //a value of -100 means that the default setting of the overide animation pattern for this parameter shall be used
        //so ar existing directionaloverrides: snow, clouds, rain, fog
        public string directionalOverride1 = "fog";
        public float overrideSpeedX1 = -100;
        public float overrideSpeedY1 = -100;
        public int overrideDelayLimit1 = -100;
        public int overrideDelayCounter1 = 10000;

        public bool overrideIsNoScrollSource1 = true;
        public bool bounceBack1 = false;


        #endregion

        #region full screen effect layer 2
        //full screen effect layer 2
        public bool useFullScreenEffectLayer2 = false;
        public bool fullScreenEffectLayerIsActive2 = true;
        public string fullScreenEffectLayerName2 = "fogLayerB";
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
        public int numberOfRenderCallsBeforeRedirection2 = 150;

        public bool isChanging2 = false;
        public float changeCounter2 = 0;
        public float changeLimit2 = 60;
        public float changeFrameCounter2 = 1;
        public float changeNumberOfFrames2 = 4;

        //public int numberOfRenderCallsforRandomCounter1 = 10000000;


        public bool useCyclicFade2 = true;

        public float fullScreenAnimationFrameCounterX2 = 0;
        public float fullScreenAnimationFrameCounterY2 = 0;
        public int fullScreenAnimationFrameCounter2 = 0;

        //setting up the override movement patterns
        //a value of -100 means that the default setting of the overide animation pattern for this parameter shall be used
        //so ar existing directionaloverrides: snow, clouds, rain, fog
        public string directionalOverride2 = "fog";
        public float overrideSpeedX2 = -100;
        public float overrideSpeedY2 = -100;
        public int overrideDelayLimit2 = -100;
        public int overrideDelayCounter2 = 10000;

        public bool overrideIsNoScrollSource2 = true;
        public bool bounceBack2 = false;

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
