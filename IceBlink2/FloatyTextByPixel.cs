using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class FloatyTextByPixel
    {
        public Coordinate location = new Coordinate();
        public string value = "";
        public string color = "red"; //red, yellow, blue, green, white
        public int timer = 0;
        public int timerLength = 4000; //time in ms
        public int z = 0; //float height multiplier

        //begin implementing floaty text that moves with prop
        public string tagOfCallingProp = "";
        public Prop floatyCarrier2 = new Prop();

        public FloatyTextByPixel(Prop floatyCarrier, string val)
        {
            int intX = (int)floatyCarrier.currentPixelPositionX;
            int intY = (int)floatyCarrier.currentPixelPositionY;
            location = new Coordinate(intX, intY);
            value = val;
            color = "red";
            tagOfCallingProp = floatyCarrier.PropTag;
            floatyCarrier2 = floatyCarrier;
        }
        public FloatyTextByPixel(Prop floatyCarrier, string val, string clr, int length)
        {
            int intX = (int)floatyCarrier.currentPixelPositionX;
            int intY = (int)floatyCarrier.currentPixelPositionY;
            location = new Coordinate(intX, intY);
            value = val;
            color = clr;
            timerLength = length;
            tagOfCallingProp = floatyCarrier.PropTag;
            floatyCarrier2 = floatyCarrier;
        }
        public FloatyTextByPixel(Coordinate coor, string val)
        {
            location = coor;
            value = val;
            color = "red";
        }
        public FloatyTextByPixel(Coordinate coor, string val, string clr)
        {
            location = coor;
            value = val;
            color = clr;
        }
    }
}
