using System;
using System.Collections.Generic;  
using System.Linq;  
using System.Text;  
using System.Threading.Tasks;  
  
namespace IceBlink2
{  
    public class Settings
     {  
         public bool debugMode = false;  
         public bool com_showGrid = false;  
         public bool map_showGrid = false;  
         public bool playSoundFx = false;
         public bool playMusic = false;
         public bool com_use11x11 = false;  
         public bool map_use11x11 = false;  
         public bool showClock = true;  
         public bool showFullParty = false;  
         public bool showMiniMap = false;  
         public bool showTogglePanel = true; //both  
         public bool showLogPanel = true; //both   
         public bool showHP = false;  
         public bool showSP = false;  
         public bool showMO = false;  
         public int combatAnimationSpeed = 100;
         public int attackAnimationSpeed = 100;
        //public bool showMoveOrder = false;
        public bool showIniBar = true;

        public Settings()
         {  
   
         }  
     }  
 }  
