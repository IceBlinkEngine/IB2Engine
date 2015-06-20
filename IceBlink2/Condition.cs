using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    [Serializable]
    public class Condition
    {
        public string c_script;
        public bool c_and; //and = true   or = false
        public bool c_not; //checked = true   unchecked = false
        public string c_parameter_1;
        public string c_parameter_2;
        public string c_parameter_3;
        public string c_parameter_4;
                
        public Condition()
        {
        }
        public Condition DeepCopy()
        {
            Condition other = (Condition)this.MemberwiseClone();
            return other;
        }
    }
}
