using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class Weather
    {
        public List<WeatherEffect> EntryWeatherEffectsList = new List<WeatherEffect>();
        public List<int> EntryWeatherChances = new List<int>();
        public List<int> EntryWeatherDurations= new List<int>();

        //the string stands for name of exit weather, their order should be the same as the weather effect entries in each exit weather list
        //e.g. cloudsA might be the name of an exit list, it could contain rainWithCloudsA and rainWithColudsB as weather effects; 
        //rainWithCloudsA will use first duration and chance elemnt of the respective lists, rainWithCloudsB the second elements... 
        public Dictionary<string, List<WeatherEffect>> ExitWeatherEffectsListDictionary = new Dictionary<string, List<WeatherEffect>>();
        public Dictionary<string, int> ExitWeatherChances = new Dictionary<string, int>();
        public Dictionary<string, int> ExitWeatherDurations = new Dictionary<string, int>();

        public Weather()
        {

        }
    }
}
