//gaMoveRosterMemberToParty.cs - Moves a character (PC) from the Party Roster to the PartyList 
//parm1 = (string) the tag of the PC (leave blank to use index instead)
//parm2 = (int) index of the PC in the PartyRosterList (0 is first PC in list, leave blank to use name/tag instead)
//parm3 = not used
//parm4 = not used
//
//WARNING: If the PartyRosterList is empty or the PartyList is already at its maximum size (module setting MaxPartySize), the script will abort