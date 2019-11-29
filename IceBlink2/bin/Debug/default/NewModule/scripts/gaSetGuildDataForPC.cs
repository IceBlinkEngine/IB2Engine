//gaSetGuildDataForPC.cs - Set up guild allegiance for a pc, including the name of the guild and the currently achieved title; 
//also modify the numerical rank number; guild name and achieved title are shown on the party screen if they do not equal "none"
//parm1 = (string) is the "name of pc" OR "leader"/"-1" (for leader/speaker) OR "number of pc" in group
//parm2 = (string) is guild name text (guildName), like e.g. "Church: Burning Heart Heralds", "Guild: Stonemasons", "Desperados"; shown on party screen if different than "none"
//parm3 = (string) is rank name text (guildRankName), like  e.g. "Initiate (Rank 1)", "Rank: Mastermason"; shown on party screen if different than "none"
//parm4 = (int) change existing rank to this nummber, use "++" or "--" for increasing/decreasing the former rank by 1,
//this is not auotmatically displayed (though can manually adjust the rank text above); useful for the guild related gc script
