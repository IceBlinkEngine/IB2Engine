using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class Tile
    {
        public string Layer0Filename = "";
        public string Layer1Filename = "t_blank";
        public string Layer2Filename = "t_blank";
        public string Layer3Filename = "t_blank";
        public string Layer4Filename = "t_blank";
        public string Layer5Filename = "t_blank";
        public int Layer1Rotate = 0;
        public int Layer2Rotate = 0;
        public int Layer3Rotate = 0;
        public int Layer4Rotate = 0;
        public int Layer5Rotate = 0;
        public int Layer1Xshift = 0;
        public int Layer2Xshift = 0;
        public int Layer3Xshift = 0;
        public int Layer4Xshift = 0;
        public int Layer5Xshift = 0;
        public int Layer1Yshift = 0;
        public int Layer2Yshift = 0;
        public int Layer3Yshift = 0;
        public int Layer4Yshift = 0;
        public int Layer5Yshift = 0;
        public bool Layer1Mirror = false;
        public bool Layer2Mirror = false;
        public bool Layer3Mirror = false;
        public bool Layer4Mirror = false;
        public bool Layer5Mirror = false;
        public int Layer1Xscale = 0;
        public int Layer2Xscale = 0;
        public int Layer3Xscale = 0;
        public int Layer4Xscale = 0;
        public int Layer5Xscale = 0;
        public int Layer1Yscale = 0;
        public int Layer2Yscale = 0;
        public int Layer3Yscale = 0;
        public int Layer4Yscale = 0;
        public int Layer5Yscale = 0;
        public bool Walkable = true;
        public bool LoSBlocked = false;
        public bool Visible = false;
        public bool blockFullScreenEffectLayer1 = false;
        public bool blockFullScreenEffectLayer2 = false;
        public bool blockFullScreenEffectLayer3 = false;
        public bool blockFullScreenEffectLayer4 = false;
        public bool blockFullScreenEffectLayer5 = false;
        public bool blockFullScreenEffectLayer6 = false;
        public bool blockFullScreenEffectLayer7 = false;
        public bool blockFullScreenEffectLayer8 = false;
        public bool blockFullScreenEffectLayer9 = false;
        public bool blockFullScreenEffectLayer10 = false;
        public int fadeMode = 0;
        public float opacity = 1;
        public float opacity5 = 1;
        public float opacity6 = 1;
        public float opacity7 = 1;
        [JsonIgnore]
        public Bitmap tileBitmap0;
        [JsonIgnore]
        public Bitmap tileBitmap1;
        [JsonIgnore]
        public Bitmap tileBitmap2;
        [JsonIgnore]
        public Bitmap tileBitmap3;
        [JsonIgnore]
        public Bitmap tileBitmap4;
        [JsonIgnore]
        public Bitmap tileBitmap5;
        public bool isCentreOfLightCircle = false;
        public bool isOtherPartOfLightCircle = false;
        public bool isFocalPoint = false;
        public int lightRadius = 0;
        public float flickerDelayCounter = 0;
        public float flicker = 0;
        public bool flickerRise = true;
        public bool affectedByFlickerAlready = false;
        public bool hasHalo = false;



        //these must be turned into lists in order work with tiles lit by multiple sources
        public List<string> tilePositionInLitArea = new List<string>();
        //public List<Tile> tilesOfThisLightSource = new List<Tile>();
        public List<string> tileLightSourceTag = new List<string>();
        public List <bool> isLit = new List<bool>();
        //each tilePosiitionInLitArea get a priority number assigned, only the position data for highest priority number
        //is used during draw call
        public List<int> priority = new List<int>();
        public List <Coordinate> lightSourceCoordinate = new List<Coordinate>();

        public List<float> lightSourceFocalHaloIntensity = new List<float>();
        public List<float> lightSourceRingHaloIntensity = new List<float>();
        //public float partyFocalHaloIntensity = 1f;

        public Tile()
        {
        }
    }
}
