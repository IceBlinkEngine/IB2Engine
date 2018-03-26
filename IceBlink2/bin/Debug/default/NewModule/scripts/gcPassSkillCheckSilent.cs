//gcPassSkillCheck.cs - Checks to see if PC passes a skill check (will write roll attempt and report to log)
//parm1 = (int) index of the PC to check for attribute (1st PC is index = 0), can adress directly with index number
//Other options:
//1. leave blank, enter -1, leader or Leader to use the currently selected party leader for the single roll
//2. enter -2, highest or Highest to use the pc with the highest power level (skill bonus from trait, attribute bonus and item bonus combined) for the single roll
//3. enter -3, lowest or Lowest to use the pc with the lowest power level (skill bonus from trait, attribute bonus and item bonus combined) for the single roll
//4. enter -4, average or Average to use average power level (skill bonus from trait, attribute bonus and item bonus combined) of the party for the single roll
//5. enter - 5, allMustSucceed or AllMustSucceed to have everybody rolling separately with their individual power level, all must succeed with check to get apositive outcome
//6. enter - 6, oneMustSucceed or OneMustSucceed to have everybody rolling separately with their individual power level, one success is enough to get apositive outcome
//parm2 = (string) trait tag to check for passing skill check (see standard list below)
//parm3 = (int) difficulty class (DC) the value to check against (can be a 
//            variable, rand(minInt-maxInt), or just type in an integer).
//parm4 = (bool) use static roll of 10, enter true and instead of rolling d20 the fixed value 10 is used 
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