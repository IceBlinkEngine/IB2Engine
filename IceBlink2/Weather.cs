using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class Weather
    {
        public string tag = "newWeatherTag";
        public string name = "newWeatherName";
        public List<WeatherTypeList> weatherTypeLists = new List<WeatherTypeList>();

        
        //use add in lbx
        /*
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Weather system")]
        public List<FullScreenEffectLayer> WeatherLayers
        {
            get { return _WeatherLayers; }
            set { _WeatherLayers = value; }
        }
        */
        /*
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Tag of the Weather (Must be unique)")]
        public string tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        [CategoryAttribute("01 - Main"), DescriptionAttribute("Name of the Weather")]
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        */
        //public Weather()
        //{
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
        //}
/*
        public Weather DeepCopy()
        {
            Weather other = (Weather)this.MemberwiseClone();
            return other;
        }
        */

        /*
        public List<WeatherEffect> EntryWeatherEffectsList = new List<WeatherEffect>();
        public List<int> EntryWeatherChances = new List<int>();
        public List<int> EntryWeatherDurations= new List<int>();

        //the string stands for name of exit weather, their order should be the same as the weather effect entries in each exit weather list
        //e.g. cloudsA might be the name of an exit list, it could contain rainWithCloudsA and rainWithColudsB as weather effects; 
        //rainWithCloudsA will use first duration and chance elemnt of the respective lists, rainWithCloudsB the second elements... 
        public Dictionary<string, List<WeatherEffect>> ExitWeatherEffectsListDictionary = new Dictionary<string, List<WeatherEffect>>();
        public Dictionary<string, int> ExitWeatherChances = new Dictionary<string, int>();
        public Dictionary<string, int> ExitWeatherDurations = new Dictionary<string, int>();
        */

        public Weather()
        {

        }
    }
}
