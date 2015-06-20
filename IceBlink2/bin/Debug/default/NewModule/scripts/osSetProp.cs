//osSetProp.cs - Sets the value of a Prop property (true/false type) in the current area (Prop must be in the current area).
//parm1 = (string) tag of the prop or "thisProp" for the prop calling this script (leave blank to use index instead)
//parm2 = (int) index of the Prop in the List of props for the current area (0 is first Prop, leave blank to use tag instead)
//parm3 = (string) Property name to check (see list below).
//parm4 = (boolean) enter either "true" or "false" to set the chosen (parm3) property's value
//PROPERTY NAME TO SET 
//(enter the single letter, not the full name. example: enter "s" not "isShown")
//s = isShown
//m = isMover
//a = isActive
//c = isChaser
//h = HasCollisions