//gcPassSkillCheck.cs - Checks to see if PC passes a skill check.
//parm1 = (int) index of the PC to check for attribute (1st PC is index = 0), leave blank 
//            or enter -1 to use the currently selected party leader
//parm2 = (string) trait tag to check for passing skill check (see standard list below)
//parm3 = (int) difficulty class (DC) the value to check against (can be a 
//            variable, rand(minInt-maxInt), or just type in an integer).
//parm4 = none
//
//Calculation: check if (1d20 + intelligence modifier + disarm device trait skill modifier) > DC
//
//Standard Trait tags that are skill type traits:
// (note: the engine will use the highest trait skill the PC has so you do not need
// to enter tags such as bluff2 or bluff3, just enter bluff.
//bluff
//diplomacy
//disabledevice
//intimidate
//pickpocket
//spot
//stealth