using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace IceBlink2
{
    public class BarkString
    {
        public string FloatyTextOneLiner = "";
        public int ChanceToShow = 10;
        public string Color = "white";
        public int LengthOfTimeToShowInMilliSeconds = 4000;
         
        public BarkString()
        {

        }
        public BarkString(string text, int chance, string color)
        {
            FloatyTextOneLiner = text;
            ChanceToShow = chance;
            Color = color;
        }
        public BarkString DeepCopy()
        {
            BarkString other = (BarkString)this.MemberwiseClone();            
            return other;
        }
    }
}
