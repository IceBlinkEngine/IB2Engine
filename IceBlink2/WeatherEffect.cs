using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class WeatherEffect
    {
        public List<FullScreenEffectLayer> WeatherLayers = new List<FullScreenEffectLayer>();

        public WeatherEffect()
        {
            for (int i = 0; i < 10; i++)
            {
                FullScreenEffectLayer newLayer = new FullScreenEffectLayer();
                WeatherLayers.Add(newLayer);
            }
        }
    }
}
