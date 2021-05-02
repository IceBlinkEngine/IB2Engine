//gaGetLocalInt.cs - Get a local int and store in a Gloabl Int (create a new one if currently doesn't exist)
//parm1 = (string) tag of the object that the local belongs to (or use thisprop or thisarea, see below)
//parm2 = (string) local variable name
//parm3 = (string) global variable name (key) to store the returned Local Int value (will create a new GlobalInt if currently doesn't exist)
//parm4 = none
//
//Can use thisarea or thisprop to get the current area tag or use the tag of the Prop that the party
//is currently standing on top of.