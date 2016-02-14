using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class Weather
    {
        public bool useFullScreenEffectLayer = false;
        public bool fullScreenEffectLayerIsActive = true;
        //add list with all already existing fullScreenEffect layer names for documentation
        public string fullScreenEffectLayerName = "sea";
        public float fullScreenAnimationSpeed = 1.0f;
        public float fullScreenAnimationSpeedX = 0.5f;
        public float fullScreenAnimationSpeedY = -1.15f;
        public bool fullScreenEffectLayerIsTop = false;
        public int fullScreenEffectChanceToOccur = 100;
        //zero signifies an endlessly running animation
        public int numberOfCyclesPerOccurence = 0;
        public float cycleCounter = 0;
        public bool containEffectInsideAreaBorders = false;
        public int activateTargetChannelInParallelToThisChannel = 0;

        public bool isChanging = true;
        public float changeCounter = 0;
        public float changeLimit = 15;
        public float changeFrameCounter = 1;
        public float changeNumberOfFrames = 6;
        public bool useCyclicFade = false;

        public float fullScreenAnimationFrameCounterX = 0;
        public float fullScreenAnimationFrameCounterY = 1.5f;
        public int fullScreenAnimationFrameCounter = 0;

        //setting up the override movement patterns
        //a value of -100 ("" in case of overrideIsNoScrollSource2) means that the default setting of the overide animation pattern for this parameter shall be used
        //so far existing directional overrides: 
        //rain (2 layers recommended; make one layer's y speed a little slower than default -2.8, like -2.4) 
        //clouds (1 layer recommended; defaults at 0.5y, 0.5x, 750 delay)
        //snow (2 layers recommended; make one layer's y speed a little faster than default -0.55, like -0.65, mayhaps slower for x (default: 0.45 to e.g. 0.4), and overrideDelayLimit1 a little less than defaut 470, like 380) 
        //fog (2 layers recommended, make one layer's overrideDelayLimit1 a little less than default 125, like 110)
        public string directionalOverride = "fog";
        public float overrideSpeedX = -100f;
        public float overrideSpeedY = -100;
        public int overrideDelayLimit = 40;
        public int overrideDelayCounter = 10000;
        public string overrideIsNoScrollSource = "";
        public bool changeableByWeatherScript = false;

        public Weather()
        {

        }
    }
}
