//ogGetWorldTime.cs - this stores time in minutes into a global int

//parm1 (int) key of global int to store time info into; raw number in minutes; the engine automatically extends the key entered here 
//by either DateInformation or AutomaticCountDown, 
//depending on p2 setting (eg you enter DeadLineMike and it becomes DeadLineMikeAutomaticCountDown, if p2 is set so) 

//parm2 (string) type of time stored: 
//DateInformation (current world time in minutes + parm3 as aditional time in hours, it all is converted to minutes) or
//AutomaticCountDown (only parm3 as time of the countdown in hours, converted to minutes for storage and manipulation)
//all keys, fully spelled out like DeadLineMikeAutomaticCountDown, can be used via <> in convos and in journal,
//like typing in a convo <DeadLineMikeAutomaticCountDown> to display the remaining time to the player
//they are automatically converted into a well readable time format
//furthermore, you can manipulate them like any global int via scripts, decreasing or extending a countdown manually or shifting a point in time
//AutomaticCountDown types are automatically decreased by the engine as ingame time passes

//parm3 (int) either the length of the countDown in hours (AutomaticCountDown) or an amount of time in hours added on top of current world time (DateInformation type)
//usse 0 in combination with DateInformation type to simply ge the curent world time
