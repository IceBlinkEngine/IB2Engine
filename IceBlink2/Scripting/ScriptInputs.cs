using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2.Scripting
{
    class ScriptInputs : Dictionary<String, String>
    {
        // This is meant to be the only method that scripts can call
        // Eventually, find a way to restrict the set methods
        public string GetValue(string key)
        {
            return this[key];
        }
    }
}
