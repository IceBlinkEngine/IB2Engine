//osSetCreatureSp.cs - Set the SP of a given Creature
//parm1 = (string) tag of the creature or "thisCreature" for the creature calling this script (leave blank to use index instead)
//parm2 = (int) index of the creature in the encounter's creature List (0 is first creature, leave blank to use tag instead)
//parm3 = (string) operator for adjusting the SP (=, +, -. /, *)
//parm4 = (int) value to use with the operator (can be a variable, rand(minInt-maxInt), or just type in a number)