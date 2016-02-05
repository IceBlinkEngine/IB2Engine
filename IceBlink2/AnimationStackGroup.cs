using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class AnimationStackGroup
    {
        public List<Sprite> SpriteGroup = new List<Sprite>();
        public string soundToPlay = "none";
        public bool turnFloatyTextOn = false;

        public AnimationStackGroup()
        {

        }
    }
}
