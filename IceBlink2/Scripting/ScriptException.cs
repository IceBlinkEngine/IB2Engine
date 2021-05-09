using System;

namespace IceBlink2.Scripting
{
    class ScriptException : Exception
    {
        public ScriptException(Exception ex) : base("Exception in script", ex)
        {
        }
    }
}
