//osSetWorldTime.cs - Sets the world time to any value (fast forward or rewind the clock)
//parm1 = (string) operator for adjusting the WorldTime (=, +, -)
//parm2 = (int) value to use with the operator (can be a variable, rand(minInt-maxInt), or just type in a number)
//parm3 = (bool) round down result to a multiple of 6 (true or false) default value is false so if left blank, will use false
//parm4 = none
//WorldTime can not be less than zero (a check will be made)
//Examples:
//  to add 240 to the Worldtime (+,240,true).
//  to add something between 100 and 150 [+,rand(10-15),true]
//  to set the WorldTime to 2311 (=,2311,false)
