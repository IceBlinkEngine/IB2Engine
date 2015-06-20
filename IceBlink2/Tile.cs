using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class Tile
    {
        //[JsonIgnore]
        public string Layer1Filename = "t_grass";
        //[JsonIgnore]
        public string Layer2Filename = "t_blank";
        //[JsonIgnore]
        public bool Walkable = true;
        //[JsonIgnore]
        public bool LoSBlocked = false;
        public bool Visible = false;

        public Tile()
        {
        }
    }
}
