//gcCheckIsClassLevel.cs - Checks to see if PC has a certain level of a class.
//parm1 = (int) index of the PC to check for Class level (1st PC is index = 0), leave blank 
//         or enter -1 to use the currently selected party leader.
//parm2 = (string) tag of the Class to check for level
//parm3 = (int) class level, returns true if this level or greater
//parm4 = none
//Example: to check if the current party leader PC is a ranger (-1, ranger, 1)
//         to check if the main PC is a level 4 or greater wizard (0, wizard, 4)