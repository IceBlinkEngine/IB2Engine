using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    [Serializable]
    public class Action
    {
        public string a_script;
        public string a_parameter_1;
        public string a_parameter_2;
        public string a_parameter_3;
        public string a_parameter_4;
                
        public Action()
        {
        }

        public Action DeepCopy()
        {
            Action other = (Action)this.MemberwiseClone();
            return other;
        }
    }
}
