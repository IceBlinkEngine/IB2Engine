using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class WeatherEffect
    {

        public string tag = "newWeatherEffectTag"; //item unique tag name
        public string name = "newWeatherEffectName"; //weather effect name in toolset
        public List<FullScreenEffectLayer> WeatherLayers = new List<FullScreenEffectLayer>();

        

        public WeatherEffect()
        {
            /*
            if (WeatherLayers.Count < 7)
            {
                for (int i = 0; i < 6; i++)
                {
                    FullScreenEffectLayer newLayer = new FullScreenEffectLayer();
                    WeatherLayers.Add(newLayer);
                }
            }
            */

        }

        public WeatherEffect DeepCopy()
        {
            WeatherEffect other = (WeatherEffect)this.MemberwiseClone();
            return other;
        }



        /*
        public string WeatherEffectName = "";
        public List<FullScreenEffectLayer> WeatherLayers = new List<FullScreenEffectLayer>();
        */
       
    }
}
