//gcCheckIsInDarkness.cs - Check to see whether the target (see parm1) is either in blue hue night darkness or dungeon blackness (see parm2). Will return true if the target is on a square filled by the type of darkness checked for. Illuminated squares or generally daytime will return false. 
//parm1 = (string) who shall be checked - choices are:"party" (for current party location, use this as default normally), "partyLast" (for last location party was on, only for exceptional cases), "thisProp" (use when attached to a prop), "thisPropLast" or simply the tag of a prop 
//parm2 = (string) the amount of darkness to check for  - can either be "noLight" (think: indoor, dungeon, blackness) or "night" (think: outdoor, starry night, blue hue)
//parm3 = not used
//parm4 = not used