using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using Newtonsoft.Json;

namespace IceBlink2
{
    public class Encounter 
    {
	    public string encounterName = "newEncounter";
	    public string MapImage = "none";
	    public bool UseMapImage = false;
        public bool UseDayNightCycle = false;
        public int MapSizeX = 7;
        public int MapSizeY = 7;
        public List<Trigger> Triggers = new List<Trigger>();
        public List<TileEnc> encounterTiles = new List<TileEnc>();
	    public List<CreatureRefs> encounterCreatureRefsList = new List<CreatureRefs>();
        [JsonIgnore]
	    public List<Creature> encounterCreatureList = new List<Creature>();
        public List<ItemRefs> encounterInventoryRefsList = new List<ItemRefs>();
        public List<Coordinate> encounterPcStartLocations = new List<Coordinate>();
        public List<Effect> effectsList = new List<Effect>();
	    public int goldDrop = 0;
        public List<Prop> propsList = new List<Prop>();
        public string AreaMusic = "none";
	    public int AreaMusicDelay = 0;
	    public int AreaMusicDelayRandomAdder = 0;
	    public string OnSetupCombatIBScript = "none";
        public string OnSetupCombatIBScriptParms = "";
        public string OnStartCombatRoundIBScript = "none";
        public string OnStartCombatRoundIBScriptParms = "";
        public string OnStartCombatTurnIBScript = "none";
        public string OnStartCombatTurnIBScriptParms = "";
        public string OnEndCombatIBScript = "none";
        public string OnEndCombatIBScriptParms = "";
        public bool isOver = false;
        public int triggerScriptCalledFromSquareLocX = 0;
        public int triggerScriptCalledFromSquareLocY = 0;
    
	    public Encounter()
	    {

		}

        public Prop getPropByLocation(int x, int y)
         {  
             foreach (Prop p in this.propsList)  
             {  
                 if ((p.LocationX == x) && (p.LocationY == y))  
                 {  
                     return p;  
                 }  
             }  
             return null;  
         }  
         public Prop getPropByTag(string tag)
         {  
             foreach (Prop p in this.propsList)  
             {
                    if (p.PropTag.Equals(tag))
                        {
                            return p;
                        }
                }  
             return null;  
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

            public Effect getEffectByTag(string tag)
            {
            foreach (Effect ef in this.effectsList)
            {
                if (ef.tag.Equals(tag)) return ef;
            }
            return null;
            }

        public bool IsInEffectListAtSameLocation(string effectTag, Coordinate coor)
        {
            foreach (Effect ef in this.effectsList)
            {
                if (ef.tag.Equals(effectTag))
                {
                    if ((ef.combatLocX == coor.X) && (ef.combatLocY == coor.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddEffect(Effect ef)
        {
            this.effectsList.Add(ef);
        }

        public void AddEffectByObject(Effect ef, int classLevel)
        {
            ef.classLevelOfSender = classLevel;
            //stackable effect and duration (just add effect to list)
            if (ef.isStackableEffect)
            {
                //add to the list
                AddEffect(ef);
            }
            //stackable duration (add to list if not there, if there add to duration)
            else if ((!ef.isStackableEffect) && (ef.isStackableDuration))
            {
                if (!IsInEffectListAtSameLocation(ef.tag, new Coordinate(ef.combatLocX, ef.combatLocY))) //Not in list so add to list
                {
                    AddEffect(ef);
                }
                else //is in list so add durations together
                {
                    Effect e = this.getEffectByTag(ef.tag);
                    e.durationInUnits += ef.durationInUnits;
                    if (classLevel > e.classLevelOfSender)
                    {
                        e.classLevelOfSender = classLevel;
                    }
                }
            }
            //none stackable (add to list if not there)
            else if ((!ef.isStackableEffect) && (!ef.isStackableDuration))
            {
                if (!IsInEffectListAtSameLocation(ef.tag, new Coordinate(ef.combatLocX, ef.combatLocY))) //Not in list so add to list
                {
                    AddEffect(ef);
                }
                else //is in list so reset duration
                {
                    Effect e = this.getEffectByTag(ef.tag);
                    e.durationInUnits = ef.durationInUnits;
                    if (classLevel > e.classLevelOfSender)
                    {
                        e.classLevelOfSender = classLevel;
                    }
                }
            }
        }


    }    
}


