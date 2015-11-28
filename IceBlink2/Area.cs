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
        public bool useFullScreenEffectLayer1 = false;
        public bool useFullScreenEffectLayer2 = false;
        public bool useFullScreenEffectLayer3 = false;
        public bool useFullScreenEffectLayer4 = false;
        public bool useFullScreenEffectLayer5 = false;
        public bool useFullScreenEffectLayer6 = false;
        public bool fullScreenEffectLayerIsActive1 = true;
        public bool fullScreenEffectLayerIsActive2 = true;
        public bool fullScreenEffectLayerIsActive3 = true;
        public bool fullScreenEffectLayerIsActive4 = true;
        public bool fullScreenEffectLayerIsActive5 = true;
        public bool fullScreenEffectLayerIsActive6 = true;

        //up to six anmation layers to be freely sred at bottom or top level of the draw stackpile
        //bmp size of animation sources is free, but I found 150x150 is a good size, using more than four of these will slightly slow down though
        // for 5 or 6 layers at the same time better use 100x100
        //testAnim
        public string fullScreenEffectLayerName1 = "testAnim";
        public string fullScreenEffectLayerName2 = "full_screen_effect_layer2";
        public string fullScreenEffectLayerName3 = "full_screen_effect_layer3";
        public string fullScreenEffectLayerName4 = "full_screen_effect_layer4";
        public string fullScreenEffectLayerName5 = "full_screen_effect_layer5";
        public string fullScreenEffectLayerName6 = "full_screen_effect_layer6";
        //these six are set by author in toolset for supposedly slow or fast effects, higher is faster
        public float fullScreenAnimationSpeed1 = 1.0f;
        public float fullScreenAnimationSpeed2 = 1.35f;
        public float fullScreenAnimationSpeed3 = 1.7f;
        public float fullScreenAnimationSpeed4 = 1.0f;
        public float fullScreenAnimationSpeed5 = 1.0f;
        public float fullScreenAnimationSpeed6 = 1.0f;
        //up, down, left, right, individual
        public string fullScreenAnimationMovePattern1 = "individual";
        public string fullScreenAnimationMovePattern2 = "down";
        public string fullScreenAnimationMovePattern3 = "down";
        public string fullScreenAnimationMovePattern4 = "down";
        public string fullScreenAnimationMovePattern5 = "down";
        public string fullScreenAnimationMovePattern6 = "down";
        
        public int individualDelayBetweenFrames1 = 20;
        public int individualDelayBetweenFrames2 = 10;
        public int individualDelayBetweenFrames3 = 10;
        public int individualDelayBetweenFrames4 = 10;
        public int individualDelayBetweenFrames5 = 10;
        public int individualDelayBetweenFrames6 = 10;
        public int individualFrameCounter1 = 1;
        public int individualNumberOfFrames1 = 4;
        public float individualDelayCounter1 = 0;
        public int individualFrameCounter2 = 1;
        public int individualNumberOfFrames2 = 1;
        public float individualDelayCounter2 = 0;
        public int individualFrameCounter3 = 1;
        public int individualNumberOfFrames3 = 1;
        public float individualDelayCounter3 = 0;
        public int individualFrameCounter4 = 1;
        public int individualNumberOfFrames4 = 1;
        public float individualDelayCounter4= 0;
        public int individualFrameCounter5 = 1;
        public int individualNumberOfFrames5 = 1;
        public float individualDelayCounter5 = 0;
        public int individualFrameCounter6 = 1;
        public int individualNumberOfFrames6 = 1;
        public float individualDelayCounter6 = 0;
        public bool FullScreenEffectLayer1IsTop = true;
        public bool FullScreenEffectLayer2IsTop = false;
        public bool FullScreenEffectLayer3IsTop = true;
        public bool FullScreenEffectLayer4IsTop = true;
        public bool FullScreenEffectLayer5IsTop = true;
        public bool FullScreenEffectLayer6IsTop = true;
        public int fullScreenEffectChanceToOccur1 = 15;
        public int fullScreenEffectChanceToOccur2 = 100;
        public int fullScreenEffectChanceToOccur3 = 100;
        public int fullScreenEffectChanceToOccur4 = 100;
        public int fullScreenEffectChanceToOccur5 = 100;
        public int fullScreenEffectChanceToOccur6 = 100;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence1 = 4;
        public int numberOfCyclesPerOccurence2 = 0;
        public int numberOfCyclesPerOccurence3 = 0;
        public int numberOfCyclesPerOccurence4 = 0;
        public int numberOfCyclesPerOccurence5 = 0;
        public int numberOfCyclesPerOccurence6 = 0;
        public float cycleCounter1 = 0;
        public float cycleCounter2 = 0;
        public float cycleCounter3 = 0;
        public float cycleCounter4 = 0;
        public float cycleCounter5 = 0;
        public float cycleCounter6 = 0;

        //only relevant for movement pattern "random"
        //to do: actually implement this
        public int numberOfRenderCallsBeforeRedirection1 = 120;
        public int numberOfRenderCallsBeforeRedirection2 = 120;
        public int numberOfRenderCallsBeforeRedirection3 = 120;
        public int numberOfRenderCallsBeforeRedirection4 = 120;
        public int numberOfRenderCallsBeforeRedirection5 = 120;
        public int numberOfRenderCallsBeforeRedirection6 = 120;


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
