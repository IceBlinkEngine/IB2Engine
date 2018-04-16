using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using Newtonsoft.Json;

namespace IceBlink2
{
    public class Faction 
    {
	    public string name = "newFaction";
	    public string tag = "newFactionTag";

        public int strength = 0;
        public bool showStrengthInJournal = false;

        public int rank = 1;
        public bool showRankInJournal = false;
        public bool displayRankInWords = false;

        public int intervalOfFactionStrengthChangeInHours = 0;
        public int amountOfFactionStrengthChangePerInterval = 0;
        public bool showChangeRateInJournal = false;

        public int timePassedInThisInterval = 0;

        public string nameRank1 = "Rank 1";
        public string nameRank2 = "Rank 2";
        public string nameRank3 = "Rank 3";
        public string nameRank4 = "Rank 4";
        public string nameRank5 = "Rank 5";
        public string nameRank6 = "Rank 6";
        public string nameRank7 = "Rank 7";
        public string nameRank8 = "Rank 8";
        public string nameRank9 = "Rank 9";
        public string nameRank10 = "Rank 10";

        public int accumulatedBuffStrengthRank1 = 0;
        public int accumulatedBuffStrengthRank2 = 1;
        public int accumulatedBuffStrengthRank3 = 2;
        public int accumulatedBuffStrengthRank4 = 3;
        public int accumulatedBuffStrengthRank5 = 5;
        public int accumulatedBuffStrengthRank6 = 7;
        public int accumulatedBuffStrengthRank7 = 10;
        public int accumulatedBuffStrengthRank8 = 13;
        public int accumulatedBuffStrengthRank9 = 16;
        public int accumulatedBuffStrengthRank10 = 20;

        public int factionStrengthRequiredForRank2 = 100;
        public int factionStrengthRequiredForRank3 = 100;
        public int factionStrengthRequiredForRank4 = 100;
        public int factionStrengthRequiredForRank5 = 100;
        public int factionStrengthRequiredForRank6 = 100;
        public int factionStrengthRequiredForRank7 = 100;
        public int factionStrengthRequiredForRank8 = 100;
        public int factionStrengthRequiredForRank9 = 100;
        public int factionStrengthRequiredForRank10 = 100;

        public Faction()
	    {
		
	    }
	
	    public Faction DeepCopy()
	    {
		    Faction copy = new Faction();
            copy.name = this.name;
            copy.tag = this.tag;

            copy.strength = this.strength;
            copy.showStrengthInJournal = this.showStrengthInJournal;
            //shown strength will always be a number

            copy.rank = this.rank;
            copy.showRankInJournal = this.showRankInJournal;
            copy.displayRankInWords = this.displayRankInWords;

            copy.showChangeRateInJournal = this.showChangeRateInJournal;
            copy.intervalOfFactionStrengthChangeInHours = this.intervalOfFactionStrengthChangeInHours;
            copy.amountOfFactionStrengthChangePerInterval = this.amountOfFactionStrengthChangePerInterval;
            //growth rate and hours needed is always shown technically (if shown), like strength

            copy.timePassedInThisInterval = this.timePassedInThisInterval;

            //labels for strength levels and bool for buff cretaures of action (AC, tohit, saves all +x per faction rank)
            copy.nameRank1 = this.nameRank1;
            copy.nameRank2 = this.nameRank2;
            copy.nameRank3 = this.nameRank3;
            copy.nameRank4 = this.nameRank4;
            copy.nameRank5 = this.nameRank5;
            copy.nameRank6 = this.nameRank6;
            copy.nameRank7 = this.nameRank7;
            copy.nameRank8 = this.nameRank8;
            copy.nameRank9 = this.nameRank9;
            copy.nameRank10 = this.nameRank10;
        
            copy.accumulatedBuffStrengthRank1 = this.accumulatedBuffStrengthRank1;
            copy.accumulatedBuffStrengthRank2 = this.accumulatedBuffStrengthRank2;
            copy.accumulatedBuffStrengthRank3 = this.accumulatedBuffStrengthRank3;
            copy.accumulatedBuffStrengthRank4 = this.accumulatedBuffStrengthRank4;
            copy.accumulatedBuffStrengthRank5 = this.accumulatedBuffStrengthRank5;
            copy.accumulatedBuffStrengthRank6 = this.accumulatedBuffStrengthRank6;
            copy.accumulatedBuffStrengthRank7 = this.accumulatedBuffStrengthRank7;
            copy.accumulatedBuffStrengthRank8 = this.accumulatedBuffStrengthRank8;
            copy.accumulatedBuffStrengthRank9 = this.accumulatedBuffStrengthRank9;
            copy.accumulatedBuffStrengthRank10 = this.accumulatedBuffStrengthRank10;

            copy.factionStrengthRequiredForRank2 = this.factionStrengthRequiredForRank2;
            copy.factionStrengthRequiredForRank3 = this.factionStrengthRequiredForRank3;
            copy.factionStrengthRequiredForRank4 = this.factionStrengthRequiredForRank4;
            copy.factionStrengthRequiredForRank5 = this.factionStrengthRequiredForRank5;
            copy.factionStrengthRequiredForRank6 = this.factionStrengthRequiredForRank6;
            copy.factionStrengthRequiredForRank7 = this.factionStrengthRequiredForRank7;
            copy.factionStrengthRequiredForRank8 = this.factionStrengthRequiredForRank8;
            copy.factionStrengthRequiredForRank9 = this.factionStrengthRequiredForRank9;
            copy.factionStrengthRequiredForRank10 = this.factionStrengthRequiredForRank10;

            return copy;
	    }
    }
}
