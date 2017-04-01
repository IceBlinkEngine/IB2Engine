//osSetPlayerBaseDex.cs - Set the Player's Dexterity Attribute Base to a new value permanently
//parm1 = (string) name/tag of the player (leave blank to use index instead)
//parm2 = (int) index of the PC in the playerList (0 is first PC, -1 is the current party leader, leave blank to use name/tag instead)
//parm3 = (string) operator for adjusting the attribute (=, +, -. /, *)
//parm4 = (int) value to use with the operator (can be a variable, rand(minInt-maxInt), or just type in a number)
//for example, to add 1 to miki's dexterity (miki,,+,1). To set the first PC's dexterity to something between 10 and 15 [ ,0,=,rand(10-15)]
