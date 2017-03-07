using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Drawing;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace IceBlink2
{
    public class Player
    {
        public string tokenFilename = "blank.png";
        public string portraitFilename = "F0404_L";
        [JsonIgnore]
        public Bitmap token;
        [JsonIgnore]
        public Bitmap portrait;
        [JsonIgnore]
        public bool combatFacingLeft = true;
        [JsonIgnore]
        public int combatFacing = 4; //numpad directions (7,8,9,4,6,1,2,3)
        public bool steathModeOn = false;
        public bool mainPc = false;
        public bool nonRemoveablePc = false;
        public int combatLocX = 0;
        public int combatLocY = 0;
        public int moveDistance = 5;
        public int moveOrder = 0;
        public string name = "CharacterName";
        public string tag = "newTag";
        public string raceTag = "newRace";
        [JsonIgnore]
        public Race race = new Race();
        public string classTag = "fighter";
        [JsonIgnore]
        public PlayerClass playerClass = new PlayerClass();
        public List<string> knownSpellsTags = new List<string>();
        public List<string> knownTraitsTags = new List<string>();
        public List<string> learningSpellsTags = new List<string>();  
        public List<string> learningTraitsTags = new List<string>();

        public List<string> knownUsableTraitsTags = new List<string>();
        public List<string> knownInCombatUsableTraitsTags = new List<string>();
        public List<string> knownOutsideCombatUsableTraitsTags = new List<string>();
        public List<Effect> effectsList = new List<Effect>();
        public List<string> pcTags = new List<string>();
        public int classLevel = 1;
        public bool isMale = true;
        public string charStatus = "Alive"; //Alive, Dead, Held
        public int baseFortitude = 0;
        public int baseWill = 0;
        public int baseReflex = 0;
        public int fortitude = 0;
        public int will = 0;
        public int reflex = 0;
        public int strength = 10;
        public int dexterity = 10;
        public int intelligence = 10;
        public int charisma = 10;
        public int luck = 10;
        public int constitution = 10;
        public int wisdom = 10;
        public int baseStr = 10;
        public int baseDex = 10;
        public int baseInt = 10;
        public int baseCha = 10;
        public int baseLuck = 10;
        public int baseWis = 10;
        public int baseCon = 10;
        public int ACBase = 10;
        public int AC = 10;
        public int classBonus = 0;
        public int baseAttBonus = 1;
        public int baseAttBonusAdders = 0;
        public int hp = 10;
        public int hpMax = 10;
        public int sp = 50;
        public int spMax = 50;
        public int XP = 0;
        public int XPNeeded = 200;
        public int hpRegenTimePassedCounter = 0;
        public int spRegenTimePassedCounter = 0;
        public ItemRefs HeadRefs = new ItemRefs();
        public ItemRefs NeckRefs = new ItemRefs();
        public ItemRefs BodyRefs = new ItemRefs();
        public ItemRefs MainHandRefs = new ItemRefs();
        public ItemRefs OffHandRefs = new ItemRefs();
        public ItemRefs RingRefs = new ItemRefs();
        public ItemRefs Ring2Refs = new ItemRefs();
        public ItemRefs FeetRefs = new ItemRefs();
        public ItemRefs AmmoRefs = new ItemRefs();
        public int damageTypeResistanceTotalAcid = 0;
        public int damageTypeResistanceTotalCold = 0;
        public int damageTypeResistanceTotalNormal = 0;
        public int damageTypeResistanceTotalElectricity = 0;
        public int damageTypeResistanceTotalFire = 0;
        public int damageTypeResistanceTotalMagic = 0;
        public int damageTypeResistanceTotalPoison = 0;
        public int hpLastTurn = -1;

        public Player()
        {

        }
        public Player DeepCopy()
        {
            Player copy = new Player();
            copy.tokenFilename = this.tokenFilename;
            copy.portraitFilename = this.portraitFilename;
            copy.combatLocX = this.combatLocX;
            copy.combatLocY = this.combatLocY;
            copy.moveDistance = this.moveDistance;
            copy.mainPc = this.mainPc;
            copy.nonRemoveablePc = this.nonRemoveablePc;
            copy.name = this.name;
            copy.tag = this.tag;
            copy.raceTag = this.raceTag;
            copy.classTag = this.classTag;
            copy.classLevel = this.classLevel;
            copy.isMale = this.isMale;
            copy.steathModeOn = this.steathModeOn;
            copy.charStatus = this.charStatus; //Alive, Dead, Held
            copy.baseFortitude = this.baseFortitude;
            copy.baseWill = this.baseWill;
            copy.baseReflex = this.baseReflex;
            copy.fortitude = this.fortitude;
            copy.will = this.will;
            copy.reflex = this.reflex;
            copy.strength = this.strength;
            copy.dexterity = this.dexterity;
            copy.intelligence = this.intelligence;
            copy.charisma = this.charisma;
            copy.luck = this.luck;
            copy.constitution = this.constitution;
            copy.wisdom = this.wisdom;
            copy.baseStr = this.baseStr;
            copy.baseDex = this.baseDex;
            copy.baseInt = this.baseInt;
            copy.baseCha = this.baseCha;
            copy.baseLuck = this.baseLuck;
            copy.baseCon = this.baseCon;
            copy.baseWis = this.baseWis;
            copy.ACBase = this.ACBase;
            copy.AC = this.AC;
            copy.classBonus = this.classBonus;
            copy.baseAttBonus = this.baseAttBonus;
            copy.baseAttBonusAdders = this.baseAttBonusAdders;
            copy.hp = this.hp;
            copy.hpMax = this.hpMax;
            copy.sp = this.sp;
            copy.spMax = this.spMax;
            copy.XP = this.XP;
            copy.XPNeeded = this.XPNeeded;
            copy.HeadRefs = this.HeadRefs.DeepCopy();
            copy.NeckRefs = this.NeckRefs.DeepCopy();
            copy.BodyRefs = this.BodyRefs.DeepCopy();
            copy.MainHandRefs = this.MainHandRefs.DeepCopy();
            copy.OffHandRefs = this.OffHandRefs.DeepCopy();
            copy.RingRefs = this.RingRefs.DeepCopy();
            copy.Ring2Refs = this.Ring2Refs.DeepCopy();
            copy.FeetRefs = this.FeetRefs.DeepCopy();
            copy.AmmoRefs = this.AmmoRefs.DeepCopy();
            copy.hpRegenTimePassedCounter = this.hpRegenTimePassedCounter;
            copy.spRegenTimePassedCounter = this.spRegenTimePassedCounter;
            copy.damageTypeResistanceTotalAcid = this.damageTypeResistanceTotalAcid;
            copy.damageTypeResistanceTotalCold = this.damageTypeResistanceTotalCold;
            copy.damageTypeResistanceTotalNormal = this.damageTypeResistanceTotalNormal;
            copy.damageTypeResistanceTotalElectricity = this.damageTypeResistanceTotalElectricity;
            copy.damageTypeResistanceTotalFire = this.damageTypeResistanceTotalFire;
            copy.damageTypeResistanceTotalMagic = this.damageTypeResistanceTotalMagic;
            copy.damageTypeResistanceTotalPoison = this.damageTypeResistanceTotalPoison;
            copy.hpLastTurn = this.hpLastTurn;

            copy.knownSpellsTags = new List<string>();
            foreach (string s in this.knownSpellsTags)
            {
                copy.knownSpellsTags.Add(s);
            }

            copy.pcTags = new List<string>();
            foreach (string s in this.pcTags)
            {
                copy.pcTags.Add(s);
            }

            copy.knownTraitsTags = new List<string>();
            foreach (string s in this.knownTraitsTags)
            {
                copy.knownTraitsTags.Add(s);
            }

            copy.knownInCombatUsableTraitsTags = new List<string>();
            foreach (string s in this.knownInCombatUsableTraitsTags)
            {
                copy.knownInCombatUsableTraitsTags.Add(s);
            }

            copy.knownOutsideCombatUsableTraitsTags = new List<string>();
            foreach (string s in this.knownOutsideCombatUsableTraitsTags)
            {
                copy.knownOutsideCombatUsableTraitsTags.Add(s);
            }

            copy.knownUsableTraitsTags = new List<string>();
            foreach (string s in this.knownUsableTraitsTags)
            {
                copy.knownUsableTraitsTags.Add(s);
            }

            copy.effectsList = new List<Effect>();
            foreach (Effect ef in this.effectsList)
            {
                copy.effectsList.Add(ef);
            }

            return copy;
        }
        public bool IsReadyToAdvanceLevel()
        {
            XPNeeded = this.playerClass.xpTable[this.classLevel];
            if (this.XP >= XPNeeded)
            {
                return true;
            }
            return false;
        }
        public void LevelUp()
        {
            // change level by one, level++
            this.classLevel++;
            // UpdateStats
            this.hp += this.playerClass.hpPerLevelUp + ((this.constitution - 10) / 2);
            this.sp += this.playerClass.spPerLevelUp + ((this.intelligence - 10) / 2);
            XPNeeded = this.playerClass.xpTable[this.classLevel];
        }

        public bool isUnconcious()
        {
            if ((this.hp <= 0) && (this.hp > -20))
            {
                return true;
            }
            return false;
        }
        public bool isDead()
        {
            if (this.hp <= -20)
            {
                return true;
            }
            return false;
        }
        public bool isAlive()
        {
            if (this.hp > 0)
            {
                return true;
            }
            return false;
        }
        public bool isHeld()
        {
            foreach (Effect ef in this.effectsList)
            {
                if (ef.statusType.Equals("Held"))
                {
                    return true;
                }
            }
            return false;
        }
        public bool isImmobile()
        {
            foreach (Effect ef in this.effectsList)
            {
                if (ef.statusType.Equals("Immobile"))
                {
                    return true;
                }
            }
            return false;
        }
        public bool isInvisible()
        {
            foreach (Effect ef in this.effectsList)
            {
                if (ef.statusType.Equals("Invisible"))
                {
                    return true;
                }
            }
            return false;
        }
        public bool isSilenced()
        {
            foreach (Effect ef in this.effectsList)
            {
                if (ef.statusType.Equals("Silenced"))
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> getSpellsToLearn()
        {
            List<string> spellTagsList = new List<string>();
            foreach (SpellAllowed sa in this.playerClass.spellsAllowed)
            {
                if (sa.allow)
                {
                    if (sa.atWhatLevelIsAvailable <= this.classLevel)
                    {
                        if (!hasSpellAlready(sa))
                        {
                            spellTagsList.Add(sa.tag);
                        }
                    }
                }
            }
            return spellTagsList;
        }
        public bool hasSpellAlready(SpellAllowed sa)
        {
            /*
            foreach (string s in this.knownSpellsTags)
            {
                if (sa.tag.Equals(s))
                {
                    return true;
                }
            }
            */
            if (this.knownSpellsTags.Contains(sa.tag)) { return true; }
            if (this.learningSpellsTags.Contains(sa.tag)) { return true; }

            return false;
        }
        public List<string> getTraitsToLearn(Module mod)
        {
            List<string> traitTagsList = new List<string>();
            foreach (TraitAllowed ta in this.playerClass.traitsAllowed)
            {
                if (ta.allow)
                {
                    if (ta.atWhatLevelIsAvailable <= this.classLevel)
                    {
                        if (!hasTraitAlready(ta))
                        {
                            //check to see if needs prereq and if you have it
                            Trait tr = mod.getTraitByTag(ta.tag);
                            if (!tr.prerequisiteTrait.Equals("none"))
                            {
                                //requires prereq so check if you have it
                                if (this.knownTraitsTags.Contains(tr.prerequisiteTrait))
                                {
                                    traitTagsList.Add(ta.tag);
                                }
                            }
                            else //does not require prereq so add to list
                            {
                                traitTagsList.Add(ta.tag);
                            }
                        }
                    }
                }
            }
            return traitTagsList;
        }
        public bool hasTraitAlready(TraitAllowed ta)
        {
            //return this.knownTraitsTags.Contains(ta.tag);
            if (this.knownTraitsTags.Contains(ta.tag)) { return true; }
            if (this.learningTraitsTags.Contains(ta.tag)) { return true; }
            return false;
        }

        public Effect getEffectByTag(string tag)
        {
            foreach (Effect ef in this.effectsList)
            {
                if (ef.tag.Equals(tag)) return ef;
            }
            return null;
        }
        public bool IsInEffectList(string effectTag)
        {
            foreach (Effect ef in this.effectsList)
            {
                if (ef.tag.Equals(effectTag))
                {
                    return true;
                }
            }
            return false;
        }
        public void AddEffect(Effect ef)
        {
            this.effectsList.Add(ef);
        }
        public void AddEffectByObject(Effect effect, int classLevel)
        {
            if (!effect.isPermanent)
            {
                Effect ef = effect.DeepCopy();
                ef.classLevelOfSender = classLevel;
                //ef.startingTimeInUnits = startTime; //mod.WorldTime;
                //stackable effect and duration (just add effect to list)
                if (ef.isStackableEffect)
                {
                    //add to the list
                    AddEffect(ef);
                }
                //stackable duration (add to list if not there, if there add to duration)
                else if ((!ef.isStackableEffect) && (ef.isStackableDuration))
                {
                    if (!IsInEffectList(ef.tag)) //Not in list so add to list
                    {
                        AddEffect(ef);
                    }
                    else //is in list so add durations together
                    {
                        Effect e = this.getEffectByTag(ef.tag);
                        if (!e.isPermanent)
                        {
                            e.durationInUnits += ef.durationInUnits;
                            if (classLevel > e.classLevelOfSender)
                            {
                                e.classLevelOfSender = classLevel;
                            }
                        }
                    }
                }
                //none stackable (add to list if not there)
                else if ((!ef.isStackableEffect) && (!ef.isStackableDuration))
                {
                    if (!IsInEffectList(ef.tag)) //Not in list so add to list
                    {
                        AddEffect(ef);
                    }
                    else //is in list so reset duration
                    {
                        Effect e = this.getEffectByTag(ef.tag);
                        if (!e.isPermanent)
                        {
                            e.durationInUnits = ef.durationInUnits;
                            if (classLevel > e.classLevelOfSender)
                            {
                                e.classLevelOfSender = classLevel;
                            }
                        }
                        //e.startingTimeInUnits = startTime;
                        //e.currentDurationInUnits = 0;
                    }
                }
            }
        }
    }
}
