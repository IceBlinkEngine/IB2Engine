using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class WeatherTypeListItem
    {
        //public string tag = "newWeatherTypeListItemTag";
        public string tag = "newWeatherTypeListItemTag"; //unique tag name
        //public string name = "newWeatherTypeListItemName";
        public string name = "change weather effect above instead"; //name in toolset
        public string combinedInfo = "change values in 01 instead"; //name in toolset
        public int chance = 0; //chance
        public int duration = 0; //duration
        //private string _weatherEffectTag = "none";
        public string weatherEffectName = "none";

        public WeatherTypeListItem()
        {
            //for (int i = 0; i < 10; i++)
            //{
            //FullScreenEffectLayer newLayer = new FullScreenEffectLayer();
            //WeatherLayers.Add(newLayer);
            //}
        }

        public WeatherTypeListItem DeepCopy()
        {
            WeatherTypeListItem other = (WeatherTypeListItem)this.MemberwiseClone();
            return other;
        }
    }

    /*
    public string WeatherEffectName = "";
    public List<FullScreenEffectLayer> WeatherLayers = new List<FullScreenEffectLayer>();
    */
    
    }
