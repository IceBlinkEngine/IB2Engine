//gcCheckForItem.cs - Checks to see if an item(s) is/are in the party/PC inventory
//parm1 = (string) item resref
//parm2 = (int) required quantity in inventory (irrelevant if checking for equipped items in parm3 or parm 4, which always check for a single item -> just use 1 here i this case)
//parm3 = (bool) "true" or "false"; must item be equipped by at least one player? (irrelevant if checking for item being equipped by a specific player in parm 4)  
//parm4 = (string) "leader" or -1: requires the current party leader to have the item equipped; you can also write a character's name here (with correct capitalization please) and then the engine checks whether the character with this name has the item equipped
