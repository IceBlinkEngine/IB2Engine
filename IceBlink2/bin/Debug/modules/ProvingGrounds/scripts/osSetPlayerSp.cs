//osSetPlayerSp.cs - Set the SP of a given PC
//parm1 = (string) name/tag of the player (leave blank to use index instead)
//parm2 = (int) index of the PC in the playerList (0 is first PC, -1 is the current party leader, leave blank to use name/tag instead)
//parm3 = (string) operator for adjusting the SP (=, +, -. /, *)
//parm4 = (int) value to use with the operator (can be a variable, rand(minInt-maxInt), or just type in a number)
//for example, to add 20 sp to miki (miki,,+,20). To set the first PC's sp to something between 10 and 15 [ ,0,=,rand(10-15)]
