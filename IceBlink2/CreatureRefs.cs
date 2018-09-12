using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class CreatureRefs 
    {
	    public string creatureResRef = "";
	    public string creatureTag = "";
	    public int creatureStartLocationX = 0;
	    public int creatureStartLocationY = 0;
        public int spawnAtStartOfRoundX = 1;
        public int spawnAnotherEveryXRoundsAfterFirstSpawn = 0;
    
        public CreatureRefs()
        {
    	
        }
    }
}
