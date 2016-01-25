using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class TileEnc 
    {
	    public string Layer1Filename = "t_grass";
        public string Layer2Filename = "t_blank";
        public string Layer3Filename = "t_blank";
        /*public int Layer1Rotate = 0;
        public int Layer2Rotate = 0;
        public int Layer3Rotate = 0;
        public bool Layer1Flip = false;
        public bool Layer2Flip = false;
        public bool Layer3Flip = false;*/
        public bool Walkable = true;
	    public bool LoSBlocked = false;

        public Bitmap tileBitmap1;
        public Bitmap tileBitmap2;
        public Bitmap tileBitmap3;

        public TileEnc()
	    {
	
	    }
    }
}
