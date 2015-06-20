using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
//using IceBlink;
using System.ComponentModel;
using Newtonsoft.Json;

namespace IceBlink2
{
    public class Encounter 
    {
	    public String encounterName = "newEncounter";
	    public String MapImage = "none";
	    public bool UseMapImage = false;
        public bool UseDayNightCycle = false;
        public List<TileEnc> encounterTiles = new List<TileEnc>();
	    //m=mud, b=black, g=grass, t=tree, r=rock, w=stone wall, s=stone floor
        public List<CreatureRefs> encounterCreatureRefsList = new List<CreatureRefs>();
        [JsonIgnore]
	    public List<Creature> encounterCreatureList = new List<Creature>();
        public List<ItemRefs> encounterInventoryRefsList = new List<ItemRefs>();
        public List<Coordinate> encounterPcStartLocations = new List<Coordinate>();
	    public int goldDrop = 0;
	    public String AreaMusic = "none";
	    public int AreaMusicDelay = 0;
	    public int AreaMusicDelayRandomAdder = 0;
	    public String OnStartCombatRoundLogicTree = "none";
	    public String OnStartCombatRoundParms = "";
	    public String OnStartCombatTurnLogicTree = "none";
	    public String OnStartCombatTurnParms = "";
	    public String OnEndCombatLogicTree = "none";
	    public String OnEndCombatParms = "";
    
	    public Encounter()
	    {
		
	    }
    }    
}
