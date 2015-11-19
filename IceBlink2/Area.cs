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
        public string fullScreenEffectLayerName1 = "full_screen_effect_layer1";
        public string fullScreenEffectLayerName2 = "full_screen_effect_layer2";
        public string fullScreenEffectLayerName3 = "full_screen_effect_layer3";

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
