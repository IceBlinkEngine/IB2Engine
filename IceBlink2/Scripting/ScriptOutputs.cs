using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2.Scripting
{
    class ScriptOutputs : Dictionary<String, String>
    {
        // This is meant to be the only method that scripts can call
        // Eventually, find a way to restrict the set methods
        public void SetValue(string key, string value)
        {
            this[key] = value;
        }
    }
}
