//gaChangeTileRamp.cs - change the ramp state of the x,y square on current map; if applied to an existing ramp of the same type(p3), it removes that ramp, otherwise it creates a new one where none was before; keep in mind please that ramps will lead down form the height that tehy are build upon; the manipulated x,y square is not allowed to be located right on the border squares of a map (0, mapSize-1)
//parm1 = (int) square's grid X location
//parm2 = (int) square's grid Y location
//parm3 = (string) use abbreviation "N" or "E" or "S" or "W" (all without"") for ramp leading up upwards towards north, east, south or west; you can also use "Off" to turn off a ramp (or use the same direction as the already existing ramp has for turning it off) 
//parm4 = (bool) use "true" if you want visible stairs drawn on the ramp tile, use "false" if you want no premade stair symbol

