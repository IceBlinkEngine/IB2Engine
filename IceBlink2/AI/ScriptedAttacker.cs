using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2.AI
{
    public class ScriptedAttacker : IModel
    {
        private string m_aiScript;
        
        public void InvokeAI(GameView gv, ScreenCombat sc, Creature crt)
        {
            if (gv.mod.debugMode)
            {
                gv.cc.addLogText("<font color='red'>" + crt.cr_name + " <font color='white'>is a ScriptedAttacker</font><BR>");
            }

            // Collect a ton of information here that is relevant to the AI Script like closest PC,
            // damage of weapons - todo
            Scripting.ScriptInputs scriptInputs = new Scripting.ScriptInputs();
            scriptInputs["closestPC"] = sc.targetClosestPC(crt).name;

            // Grab the script engine and pass in the parameters as well as the script (crt.aiScript)
            Scripting.ScriptEngine engine = Scripting.ScriptEngine.getEngine();

            // Run the script and grab the outputs
            Scripting.ScriptOutputs scriptOutputs = engine.RunScript(getScript(crt.ai_script), scriptInputs);

            // Check the outputs for validity - todo

            // Set necessary parameters on GV and SC objects based on the output
            gv.sf.ActionToTake = scriptOutputs["ActionToTake"];
            gv.sf.CombatTarget = scriptOutputs["CombatTarget"];

        }

        private string getScript(string aiScriptPath)
        {
            // Note: This will cache the script so changing the script at runtime will NOT work
            if (this.m_aiScript == null) { 
                System.IO.FileStream scriptStream = new System.IO.FileStream(aiScriptPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.StreamReader sr = new System.IO.StreamReader(scriptStream);
                m_aiScript = sr.ReadToEnd();
            }
            return m_aiScript;
        }
    }
}
