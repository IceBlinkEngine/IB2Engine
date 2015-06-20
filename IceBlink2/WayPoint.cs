using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;

namespace IceBlink2
{
    public class WayPoint 
    {
	    public int X = 0;
	    public int Y = 0;
	    public int WaitDuration = 6; //in seconds
        [JsonIgnore]
	    public int StartWaitDurationTime = 0; //in seconds, this is set to the world time once at waypoint
	    public List<BarkString> BarkStringsAtWayPoint = new List<BarkString>();
	    public List<BarkString> BarkStringsOnTheWayToNextWayPoint = new List<BarkString>();
    
        public WayPoint()
        {    	
        }
        public WayPoint(int x, int y)
        {
    	    X = x;
    	    Y = y;
        }
    }
}
