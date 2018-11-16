//gaChangeTileBridge.cs - change the bridge state of the x,y square on current map; if applied to an existing bridge of the same type(p3), it removes that bridge, otherwise it creates a new one where none was before; keep in mind please that bridges require a three tile wall on a flat nine tile, one level lower plateau to work out correctly; dig into the middle of that wall; this also for tunnels of lenght one square; the manipulated x,y square is not allowed to be located right on the border squares of a map (0, mapSize-1)
//parm1 = (int) square's grid X location
//parm2 = (int) square's grid Y location
//parm3 = (string) use abbreviation "EW" for manipulating a horizontal bridge, "NS" for a vertical bridge (without"")
//parm4 = (string) optional text of floaty that appears on x,y when changing bridge state (leave none or "" for no text)

