//gaSetLocalInt.cs - Set a local Int (create a new one if currently doesn't exist)
//parm1 = (string) tag of the object that the local belongs to (or use thisprop or thisarea, see below)
//parm2 = (string) local variable name
//parm3 = (int) value or (string) increment "++" or (string) decrement "--"
//parm4 = none
//
//Can use thisarea or thisprop to get the current area tag or use the tag of the Prop that the party
//is currently standing on top of.