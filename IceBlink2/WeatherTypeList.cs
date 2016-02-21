using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class WeatherTypeList
    {
        public string tag = "newWeatherTypeListTag";
        public string name = "newWeatherTypeListName";
        public List<WeatherTypeListItem> weatherTypeListItems = new List<WeatherTypeListItem>();

        //likely better allow adding via lbx 
        /*
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Weather system")]
        public List<WeatherTypeListItem> weatherTypeListItems
        {
            get { return _weatherTypeListItems; }
            set { _weatherTypeListItems = value; }
        }
        */

       
        public WeatherTypeList()
        {
            /*
            if (_WeatherLayers.Count < 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    FullScreenEffectLayer newLayer = new FullScreenEffectLayer();
                    WeatherLayers.Add(newLayer);
                }
            }
            */
        }

        public WeatherTypeList DeepCopy()
        {
            WeatherTypeList other = (WeatherTypeList)this.MemberwiseClone();
            return other;
        }
    }

    /*
    public string WeatherEffectName = "";
    public List<FullScreenEffectLayer> WeatherLayers = new List<FullScreenEffectLayer>();
    */
    
    }
