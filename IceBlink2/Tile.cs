using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class Tile
    {
        public string Layer1Filename = "t_blank";
        public string Layer2Filename = "t_blank";
        public string Layer3Filename = "t_blank";
        public string Layer4Filename = "t_blank";
        public string Layer5Filename = "t_blank";
        /*public int Layer1Rotate = 0;
        public int Layer2Rotate = 0;
        public int Layer3Rotate = 0;
        public int Layer4Rotate = 0;
        public int Layer5Rotate = 0;
        public bool Layer1Flip = false;
        public bool Layer2Flip = false;
        public bool Layer3Flip = false;
        public bool Layer4Flip = false;
        public bool Layer5Flip = false;*/
        public bool Walkable = true;
        public bool LoSBlocked = false;
        public bool Visible = false;
        public bool blockFullScreenEffectLayer1 = false;
        public bool blockFullScreenEffectLayer2 = false;
        public bool blockFullScreenEffectLayer3 = false;
        public bool blockFullScreenEffectLayer4 = false;
        public bool blockFullScreenEffectLayer5 = false;
        public bool blockFullScreenEffectLayer6 = false;

        public Tile()
        {
        }
    }
}
