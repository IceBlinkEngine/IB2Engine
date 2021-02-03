using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2.AI
{
    public class GeneralCaster : IModel
    {
        public void InvokeAI(GameView gv, ScreenCombat sc, Creature crt)
        {
            if (gv.mod.debugMode)
            {
                gv.cc.addLogText("<font color='red'>" + crt.cr_name + " <font color='white'>is a GeneralCaster</font><BR>");
            }

            /*
            //to do:add range and vis checks already here, more casting
                int endX = pnt.X * gv.squareSize + (gv.squareSize / 2);
            int endY = pnt.Y * gv.squareSize + (gv.squareSize / 2);
            int startX = crt.combatLocX * gv.squareSize + (gv.squareSize / 2);
            int startY = crt.combatLocY * gv.squareSize + (gv.squareSize / 2);
            if ((getDistance(pnt, new Coordinate(crt.combatLocX, crt.combatLocY)) <= gv.sf.SpellToCast.range)
                    && (isVisibleLineOfSight(new Coordinate(startX, startY), new Coordinate(endX, endY))))


            */
            gv.sf.SpellToCast = null;
            //check if should cast spell or attack/move  
            int castpercent = gv.sf.RandInt(100);
            if (crt.percentChanceToCastSpell < castpercent)
            {
                //don't cast this round, instead try and attack or move  
                //Player pc = targetClosestPC(crt);
                //gv.sf.CombatTarget = pc;
                //gv.sf.ActionToTake = "Attack";
                new BasicAttacker().InvokeAI(gv, sc, crt);
                return;
            }

            //List<int> existingSpellNumbers = new List<int>();
            List<int> usedSpellNumbers = new List<int>();
            int remainingCastCeiling = 100;

            /*
            for (int i = 0; i < crt.knownSpellsTags.Count; i++)
            {
                existingSpellNumbers.Add(i);
            }
            */

            //just pick a random spell from KnownSpells
            //try a few times to pick a random spell that has enough SP
            for (int i = 0; i < crt.knownSpellsTags.Count; i++)
            {
                int rnd = gv.sf.RandInt(crt.knownSpellsTags.Count) - 1;
                while (usedSpellNumbers.Contains(rnd))
                {
                    rnd = gv.sf.RandInt(crt.knownSpellsTags.Count) - 1;
                }
                usedSpellNumbers.Add(rnd);

                bool isRandomCaster = false;

                //list with cats chnces is empty (old creature, never touched again)
                if (crt.castChances.Count == 0)
                {
                    isRandomCaster = true;
                }

                //all existing cast chances at 0
                if (!isRandomCaster)
                {
                    isRandomCaster = true;
                    foreach (LocalInt l in crt.castChances)
                    {
                        if (l.Value > 0)
                        {
                            isRandomCaster = false;
                            break;
                        }
                    }
                }

                Spell sp = gv.mod.getSpellByTag(crt.knownSpellsTags[rnd]);
                if (!isRandomCaster)
                {
                    int castingChance = 0;
                    foreach (LocalInt lint in crt.castChances)
                    {
                        if (lint.Key == crt.knownSpellsTags[rnd])
                        {
                            castingChance = lint.Value;
                        }
                    }

                    int rnd2 = gv.sf.RandInt(remainingCastCeiling);
                    remainingCastCeiling -= castingChance;

                    if (remainingCastCeiling < 2)
                    {
                        remainingCastCeiling = 1;
                    }

                    if (rnd2 > castingChance)
                    {
                        continue;

                    }
                }

                if (sp != null)
                {
                    if (sp.costSP <= crt.sp)
                    {
                        //gv.sf.SpellToCast = sp;

                        if (sp.spellTargetType.Equals("Enemy"))
                        {
                            Player pc = sc.targetClosestPC(crt);

                            bool inRange = false;
                            int endX = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
                            int endY = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);
                            int startX = crt.combatLocX * gv.squareSize + (gv.squareSize / 2);
                            int startY = crt.combatLocY * gv.squareSize + (gv.squareSize / 2);
                            if ((sc.getDistance(new Coordinate(pc.combatLocX, pc.combatLocY), new Coordinate(crt.combatLocX, crt.combatLocY)) <= sp.range)
                                    && (sc.isVisibleLineOfSight(new Coordinate(startX, startY), new Coordinate(endX, endY))))
                            {
                                inRange = true;
                            }

                            if (!inRange)
                            {
                                foreach (Player p in gv.mod.playerList)
                                {
                                    if (p.hp > 0)
                                    {
                                        endX = p.combatLocX * gv.squareSize + (gv.squareSize / 2);
                                        endY = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);

                                        if ((sc.getDistance(new Coordinate(p.combatLocX, p.combatLocY), new Coordinate(crt.combatLocX, crt.combatLocY)) <= sp.range)
                                        && (sc.isVisibleLineOfSight(new Coordinate(startX, startY), new Coordinate(endX, endY))))
                                        {
                                            inRange = true;
                                            pc = p;
                                            break;
                                        }
                                    }
                                }
                            }


                            if ((pc != null) && inRange)
                            {
                                gv.sf.SpellToCast = sp;
                                gv.sf.CombatTarget = pc;
                                gv.sf.ActionToTake = "Cast";
                                break;
                            }
                            else
                            {
                                //endCreatureTurn(crt);
                                //BasicAttacker(crt);
                                continue;
                            }
                        }
                        else if (sp.spellTargetType.Equals("PointLocation"))
                        {
                            gv.sf.SpellToCast = sp;
                            Coordinate bestLoc = sc.targetBestPointLocation(crt);
                            if (bestLoc.X == -1 && bestLoc.Y == -1)
                            {
                                //didn't find a target so use closest PC
                                Player pc = sc.targetClosestPC(crt);

                                bool inRange = false;
                                int endX = pc.combatLocX * gv.squareSize + (gv.squareSize / 2);
                                int endY = pc.combatLocY * gv.squareSize + (gv.squareSize / 2);
                                int startX = crt.combatLocX * gv.squareSize + (gv.squareSize / 2);
                                int startY = crt.combatLocY * gv.squareSize + (gv.squareSize / 2);
                                if ((sc.getDistance(new Coordinate(pc.combatLocX, pc.combatLocY), new Coordinate(crt.combatLocX, crt.combatLocY)) <= sp.range)
                                        && (sc.isVisibleLineOfSight(new Coordinate(startX, startY), new Coordinate(endX, endY))))
                                {
                                    inRange = true;
                                }

                                if ((pc != null) && inRange)
                                {
                                    gv.sf.SpellToCast = sp;
                                    gv.sf.CombatTarget = new Coordinate(pc.combatLocX, pc.combatLocY);
                                }
                                else
                                {
                                    //endCreatureTurn(crt);
                                    //BasicAttacker(crt);
                                    gv.sf.SpellToCast = null;
                                    continue;
                                }
                            }
                            else
                            {
                                gv.sf.SpellToCast = sp;
                                gv.sf.CombatTarget = sc.targetBestPointLocation(crt);
                            }
                            gv.sf.ActionToTake = "Cast";
                            break;
                        }
                        else if (sp.spellTargetType.Equals("Friend"))
                        {
                            bool isHPHealing = false;
                            bool isSPRestoring = false;
                            Effect effectToCheck = new Effect();
                            foreach (EffectTagForDropDownList efTFDDL in sp.spellEffectTagList)
                            {
                                effectToCheck = gv.mod.getEffectByTag(efTFDDL.tag);
                                if (effectToCheck.doHeal == true && effectToCheck.healHP == true)
                                {
                                    isHPHealing = true;
                                    break;
                                }
                            }

                            if (sp.spellScript == "spHeal")
                            {
                                isHPHealing = true;
                            }

                            if (sp.spellEffectTag != "none" && sp.spellEffectTag != "None" && sp.spellEffectTag != "")
                            {
                                effectToCheck = gv.mod.getEffectByTag(sp.spellEffectTag);
                                if (effectToCheck.doHeal == true && effectToCheck.healHP == true)
                                {
                                    isHPHealing = true;
                                    // break;
                                }
                            }

                            //if not healing, let us see if sp restoring
                            if (!isHPHealing)
                            {
                                foreach (EffectTagForDropDownList efTFDDL in sp.spellEffectTagList)
                                {
                                    effectToCheck = gv.mod.getEffectByTag(efTFDDL.tag);
                                    if (effectToCheck.doHeal == true && effectToCheck.healHP == false)
                                    {
                                        isSPRestoring = true;
                                        break;
                                    }
                                }

                                if (sp.spellEffectTag != "none" && sp.spellEffectTag != "None" && sp.spellEffectTag != "")
                                {
                                    effectToCheck = gv.mod.getEffectByTag(sp.spellEffectTag);
                                    if (effectToCheck.doHeal == true && effectToCheck.healHP == false)
                                    {
                                        isSPRestoring = true;
                                        //break;
                                    }
                                }

                            }

                            Creature targetCrt = new Creature();
                            //add sp healing
                            if (isHPHealing)
                            {
                                //also only have these helper functions only return cretaure in spell range and visible
                                gv.sf.SpellToCast = sp;
                                targetCrt = sc.GetCreatureWithMostDamaged(crt);
                            }
                            else if (isSPRestoring)
                            {
                                //also only have these helper functions only return cretaure in spell range and visible

                                gv.sf.SpellToCast = sp;
                                targetCrt = sc.GetCreatureWithMostSPMissing(crt);
                            }
                            else
                            {
                                //is buff, no heal hp or heal sp
                                //also only have these helper functions only return cretaure in spell range and visible
                                gv.sf.SpellToCast = sp;
                                targetCrt = sc.targetClosestCreatureInRangeAndVisible(crt);
                            }

                            /*
                            if (targetCrt == null)
                            {
                                gv.sf.SpellToCast = null;
                            }
                            */

                            if (targetCrt != null)
                            {
                                gv.sf.SpellToCast = sp;
                                gv.sf.CombatTarget = targetCrt;
                                gv.sf.ActionToTake = "Cast";
                                break;
                            }
                            else //didn't find a target that needs HP
                            {
                                gv.sf.SpellToCast = null;
                                continue;
                            }

                            /*
                            //target is another creature (currently assumed that spell is a heal spell)
                            Creature targetCrt = GetCreatureWithMostDamaged();
                            if (targetCrt != null)
                            {
                                gv.sf.CombatTarget = targetCrt;
                                gv.sf.ActionToTake = "Cast";
                                break;
                            }
                            else //didn't find a target that needs HP
                            {
                                gv.sf.SpellToCast = null;
                                continue;
                            }
                            */
                        }
                        else if (sp.spellTargetType.Equals("Self"))
                        {
                            //target is self (currently assumed that spell is a heal spell)
                            Creature targetCrt = crt;
                            if (targetCrt != null)
                            {
                                gv.sf.SpellToCast = sp;
                                gv.sf.CombatTarget = targetCrt;
                                gv.sf.ActionToTake = "Cast";
                                break;
                            }
                        }
                        else //didn't find a target so set to null so that will use attack instead
                        {
                            gv.sf.SpellToCast = null;
                        }
                    }
                }
            }

            if (gv.sf.SpellToCast == null) //didn't find a spell that matched the criteria so use attack instead
            {
                new BasicAttacker().InvokeAI(gv, sc, crt);
                /*
                Player pc = targetClosestPC(crt);
                if (pc == null)
                {
                    endCreatureTurn(crt);
                }
                else
                {
                    gv.sf.CombatTarget = pc;
                    gv.sf.ActionToTake = "Attack";
                }
                */
            }
        }
    }
}
