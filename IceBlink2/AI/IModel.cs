using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2.AI
{
    public interface IModel
    {
        void InvokeAI(GameView gv, ScreenCombat sc, Creature attacker);
    }
}
