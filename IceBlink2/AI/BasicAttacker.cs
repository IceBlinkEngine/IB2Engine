using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2.AI
{
    public class BasicAttacker : IModel
    {
        public void InvokeAI(GameView gv, ScreenCombat sc, Creature crt)
        {
            if (gv.mod.debugMode)
            {
                gv.cc.addLogText("<font color='red'>" + crt.cr_name + " <font color='white'>is a BasicAttacker</font><BR>");
            }

            Player pc = sc.targetClosestPC(crt);
            if ((crt.cr_ai.Equals("bloodHunter")))
            {
                pc = sc.targetPCWithLeastHPInCombinedRange(crt);
                if (pc == null)
                {
                    pc = sc.targetClosestPC(crt);
                }
            }
            if ((crt.cr_ai.Equals("mindHunter")))
            {
                pc = sc.targetPCWithHighestSPInCombinedRange(crt);
                if (pc == null)
                {
                    pc = sc.targetClosestPC(crt);
                }
            }
            if ((crt.cr_ai.Equals("softTargetHunter")))
            {
                pc = sc.targetPCWithWorstACInCombinedRange(crt);
                if (pc == null)
                {
                    pc = sc.targetClosestPC(crt);
                }
            }

            if (pc == null)
            {
                sc.endCreatureTurn(crt);
            }
            else
            {
                gv.sf.CombatTarget = pc;
                int dist = sc.CalcDistance(crt, crt.combatLocX, crt.combatLocY, pc.combatLocX, pc.combatLocY);

                if (((crt.cr_ai.Equals("bloodHunter"))) || ((crt.cr_ai.Equals("mindHunter"))) || ((crt.cr_ai.Equals("softTargetHunter"))))
                {
                    if ((pc.tag == crt.targetPcTag) || (crt.targetPcTag == "none"))
                    {

                        int endX = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
                        int endY = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);
                        int startX = crt.combatLocX * gv.squareSize + (gv.squareSize / 2);
                        int startY = crt.combatLocY * gv.squareSize + (gv.squareSize / 2);

                        if ((dist <= crt.cr_attRange) && (sc.isVisibleLineOfSight(new Coordinate(startX, startY), new Coordinate(endX, endY))))
                        {
                            gv.sf.ActionToTake = "Attack";
                        }
                        else
                        {
                            gv.sf.ActionToTake = "Move";
                        }
                    }
                    else
                    {
                        int gotacha = 0;
                    }
                }
                else
                {
                    if (dist <= crt.cr_attRange)
                    {
                        gv.sf.ActionToTake = "Attack";
                    }
                    else
                    {
                        gv.sf.ActionToTake = "Move";
                    }
                }
            }
        }
    }
}
