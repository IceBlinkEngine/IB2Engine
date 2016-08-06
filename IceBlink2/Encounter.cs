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
        public List<TileEnc> encounterTiles = new List<TileEnc>();
	    public List<CreatureRefs> encounterCreatureRefsList = new List<CreatureRefs>();
        [JsonIgnore]
	    public List<Creature> encounterCreatureList = new List<Creature>();
        public List<ItemRefs> encounterInventoryRefsList = new List<ItemRefs>();
        public List<Coordinate> encounterPcStartLocations = new List<Coordinate>();
	    public int goldDrop = 0;
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
    
	    public Encounter()
	    {
		
	    }
    }    
}
