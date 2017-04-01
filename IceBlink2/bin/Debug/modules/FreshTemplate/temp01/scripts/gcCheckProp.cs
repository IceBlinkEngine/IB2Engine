//gcCheckProp.cs - Checks the value of a Prop property (true/false type) in the current area to see if it is true.
//parm1 = (string) tag of the prop or "thisProp" for the prop calling this script (leave blank to use index instead)
//parm2 = (int) index of the Prop in the List of props for the current area (0 is first Prop, leave blank to use tag instead)
//parm3 = (string) Property name to check (see list below).
//parm4 = none
//NOTE: Prop must be in current area or the default value of "false" will be returned.
//PROPERTY NAME TO CHECK 
//(enter the single letter, not the full name. example: enter "s" not "isShown" for checking to see if prop isShown value is currently set to true)
//s = isShown
//m = isMover
//a = isActive
//c = isChaser
//h = HasCollisions