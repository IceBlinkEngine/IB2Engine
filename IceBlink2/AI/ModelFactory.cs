using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2.AI
{
    static class ModelFactory
    {
        public static IModel createModel(string modelType)
        {
            switch (modelType)
            {
                case "BasicAttacker":
                    return new BasicAttacker();
                case "GeneralCaster":
                    return new GeneralCaster();
                case "ScriptedAttacker":
                    return new ScriptedAttacker();
                default:
                    return new BasicAttacker();
            }
        }
    }
}
