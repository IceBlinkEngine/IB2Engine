using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2.Scripting
{
    class ScriptEngine
    {
        private static Jint.Engine m_JintEngine;
        private static ScriptEngine m_singleton;

        ScriptEngine()
        {
            m_JintEngine = new Jint.Engine(cfg => cfg.AllowClr(typeof(ScriptEngine).Assembly));
        }

        public static ScriptEngine getEngine()
        {
            return m_singleton;
        }

        public ScriptOutputs RunScript(string scriptContent, ScriptInputs scriptInputs)
        {
            ScriptOutputs outputs = new ScriptOutputs();
            m_JintEngine.Execute(scriptContent).SetValue("inputs", scriptInputs).SetValue("outputs", outputs);
            return outputs;

        }

        public ScriptOutputs RunScript(System.IO.Stream scriptStream, ScriptInputs scriptInputs)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(scriptStream);
            string script = sr.ReadToEnd();
            sr.Close();
            return this.RunScript(script, scriptInputs);
        }

    }
}
