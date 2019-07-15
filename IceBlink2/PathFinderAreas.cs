using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class PathFinderAreas
    {
        public int[,] grid;
        public int[,] values;
        public List<Coordinate> pathNodes = new List<Coordinate>();
        public bool foundEnd = false;
        public Module mod;
        public GameView gv;

        public PathFinderAreas(Module m)
        {
            mod = m;
        }

        public Coordinate findNewPoint(Coordinate start, Coordinate end, Prop callingProp, int index)
        {
           
            foundEnd = false;
            Coordinate newPoint = new Coordinate(-1, -1);
            //set start location value to 0
            values[start.X, start.Y] = 0;
            //find all props that have collision and set there square to 1
            foreach (Prop prp in mod.moduleAreasObjects[index].Props)
            {
                if ((prp.HasCollision) && (prp.isActive))
                {
                    grid[prp.LocationX, prp.LocationY] = 1;
                }
            }
            grid[start.X, start.Y] = 2; //2 marks the start point in the grid
            grid[end.X, end.Y] = 3; //3 marks the end point in the grid
            buildPath(start, callingProp, index);



            if (!foundEnd)
            {
                //do not build path for now so return (-1,-1), later add code for picking a spot to move
                int i = 1;
            }
            else
            {
                pathNodes.Add(new Coordinate(end.X, end.Y));
                for (int i = 0; i < values[end.X, end.Y]; i++)
                {
                    pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1], callingProp, values[end.X, end.Y], index));
                    //pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1 - i], callingProp));
                }
                //build list of path points
                newPoint = pathNodes[pathNodes.Count - 2];
                //if (newPoint.X == 5 && newPoint.Y == 6)
                //{
                    //int hgh = 0;
                //}
            }
            callingProp.lengthOfLastPath = pathNodes.Count;
            callingProp.oldPath.Clear();
            foreach (Coordinate c in pathNodes)
            {
                if (!callingProp.isCurrentlyChasing)
                {
                    callingProp.oldPath.Add(c);
                }
            }
            pathNodes.Clear();

            /*
            foreach (Prop propObject in gv.mod.currentArea.Props)
            {
                if ((propObject.lastLocationX != propObject.LocationX) || (propObject.lastLocationY != propObject.LocationY))
                {
                    propObject.lastLocationZ = propObject.LocationZ;
                    //propObject.lastLocationX = propObject.LocationX;
                    //propObject.lastLocationY = propObject.LocationY;   
                }

                //updating props heightLevel
                propObject.LocationZ = gv.mod.currentArea.Tiles[propObject.LocationY * gv.mod.currentArea.MapSizeX + propObject.LocationX].heightLevel;

            }
            */
            if ((newPoint.X != -1 && newPoint.Y != -1) && (newPoint.X != callingProp.LocationX || newPoint.Y != callingProp.LocationY))
            {
                callingProp.lastLocationX = callingProp.LocationX;
                callingProp.lastLocationY = callingProp.LocationY;
            }

            return newPoint;

            /*
            {
                //do not build path for now so return (-1,-1), later add code for picking a spot to move
            }
            else
            {
                pathNodes.Add(new Coordinate(end.X, end.Y));
                for (int i = 0; i < 9999; i++) //keep going until full path back to crt is built
                {
                    pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1]));
                    if ((pathNodes[pathNodes.Count - 1].X == crt.combatLocX ) && (pathNodes[pathNodes.Count - 1].Y == crt.combatLocY ))
                    {
                        break;
                    }
                }
                //build list of path points
                //newPoint = pathNodes[pathNodes.Count - 2];
            }
            return pathNodes;
            */


        }


        //find new point in square part of an area around a center point withhin a radius
        public Coordinate findNewPoint(Coordinate start, Coordinate end, Prop callingProp, int centerPointX, int centerPointY, int radius, int index)
        {
           
            //resetGrid(propAreaIndex);
            foundEnd = false;
            Coordinate newPoint = new Coordinate(-1, -1);
            //set start location value to 0
            values[start.X, start.Y] = 0;
            //find all props that have collision and set there square to 1
            foreach (Prop prp in mod.moduleAreasObjects[index].Props)
            {
                //if  ( ((prp.HasCollision) && (prp.isActive)) || ((prp.isMover) && (prp.isActive)) )
                if ((prp.HasCollision) && (prp.isActive))
                {
                    grid[prp.LocationX, prp.LocationY] = 1;
                }
            }
            grid[start.X, start.Y] = 2; //2 marks the start point in the grid
            grid[end.X, end.Y] = 3; //3 marks the end point in the grid
            buildPath(start, centerPointX, centerPointY, radius, callingProp, index);

            if (!foundEnd)
            {
                //do not build path for now so return (-1,-1), later add code for picking a spot to move
            }
            else
            {
                pathNodes.Add(new Coordinate(end.X, end.Y));
                for (int i = 0; i < values[end.X, end.Y]; i++)
                {
                    pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1], callingProp, values[end.X, end.Y], index));
                }
                //build list of path points
                newPoint = pathNodes[pathNodes.Count - 2];
            }
            callingProp.lengthOfLastPath = pathNodes.Count;
            pathNodes.Clear();

            //added as part of height level system
            if ((newPoint.X != -1 && newPoint.Y != -1) && (newPoint.X != callingProp.LocationX || newPoint.Y != callingProp.LocationY))
            {
                callingProp.lastLocationX = callingProp.LocationX;
                callingProp.lastLocationY = callingProp.LocationY;
            }

            return newPoint;
        }

        //NOT USED
        //find new point in square part of an area around a center point withhin a radius, here: record path, too (selected by overload)
  /*      
        public Coordinate findNewPoint(Coordinate start, Coordinate end, Prop callingProp, int centerPointX, int centerPointY, int radius, bool recordPath, GameView g)
        {

            gv = g;
           
            int propAreaIndex = 0;
            bool breakOuter = false;
            foreach (int i in gv.cc.getNearbyAreas())
            {
                foreach (Prop p in gv.mod.moduleAreasObjects[i].Props)
                {
                    if (callingProp.PropTag == p.PropTag)
                    {
                        propAreaIndex = i;
                        breakOuter = true;
                        break;
                    }
                }
                if (breakOuter)
                {
                    break;
                }
            }
            resetGrid(propAreaIndex);
            foundEnd = false;
            Coordinate newPoint = new Coordinate(-1, -1);
            //set start location value to 0
            values[start.X, start.Y] = 0;
            //find all props that have collision and set there square to 1
            foreach (Prop prp in mod.currentArea.Props)
            {
                if ((prp.HasCollision) && (prp.isActive))
                {
                    grid[prp.LocationX, prp.LocationY] = 1;
                }
            }
            grid[start.X, start.Y] = 2; //2 marks the start point in the grid
            grid[end.X, end.Y] = 3; //3 marks the end point in the grid
            buildPath(start, centerPointX, centerPointY, radius, callingProp, propAreaIndex);

            if (!foundEnd)
            {
                //do not build path for now so return (-1,-1), later add code for picking a spot to move
            }
            else
            {

                int xOffSetInSquares = 0;
                int yOffSetInSquares = 0;

                pathNodes.Add(new Coordinate(end.X, end.Y));

                for (int i = 0; i < (values[end.X, end.Y] - 2); i++)
                {

                    xOffSetInSquares = 0;
                    yOffSetInSquares = 0;
                    int playerPositionXInPix = 0;
                    int playerPositionYInPix = 0;
                    
                    if (pathNodes.Count == 1)
                    {
                        if (mod.PlayerLocationX >= pathNodes[pathNodes.Count - 1].X)
                        {
                            xOffSetInSquares = pathNodes[pathNodes.Count - 1].X - mod.PlayerLocationX;
                            
                        }
                        else
                        {
                            xOffSetInSquares = pathNodes[pathNodes.Count - 1].X - mod.PlayerLocationX;
                        }
                        if (mod.PlayerLocationY >= pathNodes[pathNodes.Count - 1].Y)
                        {
                            yOffSetInSquares = pathNodes[pathNodes.Count - 1].Y - mod.PlayerLocationY;
                        }
                        else
                        {
                            yOffSetInSquares = pathNodes[pathNodes.Count - 1].Y - mod.PlayerLocationY;
                        }
                        playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffsetX * gv.squareSize);
                        playerPositionYInPix = gv.playerOffsetY * gv.squareSize;
                        
                        callingProp.destinationPixelPositionXList.Add(playerPositionXInPix + (xOffSetInSquares * gv.squareSize));
                        callingProp.destinationPixelPositionYList.Add(playerPositionYInPix + (yOffSetInSquares * gv.squareSize));

                    }

                    pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1], callingProp, values[end.X, end.Y], propAreaIndex));
                    //Note to self: might be that the order is reverse here, check when debugging

                    int shiftXDifference = pathNodes[pathNodes.Count - 1].Y - pathNodes[pathNodes.Count - 2].Y;
                    int shiftYDifference = pathNodes[pathNodes.Count - 1].X - pathNodes[pathNodes.Count - 2].X;
                    pathNodes[pathNodes.Count - 1].X = pathNodes[pathNodes.Count - 2].X;
                    pathNodes[pathNodes.Count - 1].Y = pathNodes[pathNodes.Count - 2].Y;
                    pathNodes[pathNodes.Count - 1].X += shiftXDifference;
                    pathNodes[pathNodes.Count - 1].Y += shiftYDifference;

                    xOffSetInSquares = 0;
                    yOffSetInSquares = 0;
                    if (mod.PlayerLocationX >= pathNodes[pathNodes.Count - 1].X)
                    {
                        xOffSetInSquares = pathNodes[pathNodes.Count - 1].X - mod.PlayerLocationX;
                    }
                    else
                    {
                        xOffSetInSquares = pathNodes[pathNodes.Count - 1].X - mod.PlayerLocationX;
                    }
                    if (mod.PlayerLocationY >= pathNodes[pathNodes.Count - 1].Y)
                    {
                        yOffSetInSquares = pathNodes[pathNodes.Count - 1].Y - mod.PlayerLocationY;
                    }
                    else
                    {
                        yOffSetInSquares = pathNodes[pathNodes.Count - 1].Y - mod.PlayerLocationY;
                    }
                    playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffsetX * gv.squareSize);
                    playerPositionYInPix = gv.playerOffsetY * gv.squareSize;

                    callingProp.destinationPixelPositionXList.Add(playerPositionXInPix + (xOffSetInSquares * gv.squareSize));
                    callingProp.destinationPixelPositionYList.Add(playerPositionYInPix + (yOffSetInSquares * gv.squareSize));

                }
                //build list of path points
                newPoint = pathNodes[pathNodes.Count - 1];
            }
            callingProp.lengthOfLastPath = pathNodes.Count - 1;
            pathNodes.Clear();
            callingProp.destinationPixelPositionXList.Reverse();
            callingProp.destinationPixelPositionYList.Reverse();


            return newPoint;
        }
*/    

        //build path for limited area
        public void buildPath(Coordinate start, int centerPointX, int centerPointY, int radius, Prop callingProp, int index)
        {
            try
            {
                int minX = centerPointX - radius;
                if (minX < 0) { minX = 0; };
                int minY = centerPointY - radius;
                if (minY < 0) { minY = 0; };
                int maxX = centerPointX + radius + 1;
                if (maxX > mod.moduleAreasObjects[index].MapSizeX - 1) { maxX = mod.moduleAreasObjects[index].MapSizeX - 1; }
                int maxY = centerPointY + radius + 1;
                if (maxY > mod.moduleAreasObjects[index].MapSizeY - 1) { maxY = mod.moduleAreasObjects[index].MapSizeY - 1; }
                int numberOfSquaresInArea = (2 * radius + 1) * (2 * radius + 1);

                //iterate through all values for next number and evaluate neighbors
                int next = 0;
                for (int cnt = 0; cnt < numberOfSquaresInArea; cnt++)
                {
                    //1100 is used because 32*32=1024 and rounded up to 1100, NOTE; old explanation, delete then
                    for (int x = minX; x < maxX; x++)
                    {
                        for (int y = minY; y < maxY; y++)
                        {
                            if (values[x, y] == next)
                            {
                                if ((x + 1 < maxX) && (evaluateValue(x + 1, y, next, callingProp, x, y, index)))
                                {
                                    foundEnd = true;
                                    return;
                                }
                                if ((x - 1 >= minX) && (evaluateValue(x - 1, y, next, callingProp, x, y, index)))
                                {
                                    foundEnd = true;
                                    return;
                                }
                                if ((y + 1 < maxY) && (evaluateValue(x, y + 1, next, callingProp, x, y, index)))
                                {
                                    foundEnd = true;
                                    return;
                                }
                                if ((y - 1 >= minY) && (evaluateValue(x, y - 1, next, callingProp, x, y, index)))
                                {
                                    foundEnd = true;
                                    return;
                                }
                            }
                        }
                    }
                    next++;
                }
            }
            catch
            {
                int gfhhf = 0;
                gv.cc.addLogText("<font color='yellow'>" + "CRASH in buildPath limited area " + "</font><BR>");
            }
        }

        //build limted area part grid
        /*
        public void resetGrid(int centerPointX, int centerPointY, int radius)
        {
            grid = new int[centerPointX + radius + 1, centerPointY + radius + 1];
            values = new int[centerPointX + radius + 1, centerPointY + radius + 1];
            //create the grid with 1s and 0s
            for (int col = centerPointX - radius; col < centerPointX + radius + 1; col++)
            {
                for (int row = centerPointY - radius; row < centerPointY + radius + 1; row++)
                {
                    if (isWalkable(col, row))
                    {
                        grid[col,row] = 0;
                    }
                    else
                    {
                        grid[col, row] = 1;
                    }
                }
            }

            //assign 9999 to every value
            for (int x = centerPointX - radius; x < centerPointX + radius + 1; x++)
            {
                for (int y = centerPointY - radius; y < centerPointY + radius + 1; y++)
                {
                    values[x, y] = 9999;
                }
            }
        }
        */


        //called from outside to reset grid
        public void resetGrid(int index)
        {
    	    grid = new int[mod.moduleAreasObjects[index].MapSizeX,mod.moduleAreasObjects[index].MapSizeY];
    	    values = new int[mod.moduleAreasObjects[index].MapSizeX,mod.moduleAreasObjects[index].MapSizeY];
            //create the grid with 1s and 0s
    	    for (int col = 0; col < mod.moduleAreasObjects[index].MapSizeX;col++)
    	    {
    		    for (int row = 0; row < mod.moduleAreasObjects[index].MapSizeY; row++)
    		    {
    			    if (isWalkable(col,row,index))
    			    {
    				    grid[col,row] = 0;
    			    }
    			    else
    			    {
    				    grid[col,row] = 1;
    			    }
    		    }
    	    }
        
            //assign 9999 to every value
            for (int x = 0; x < mod.moduleAreasObjects[index].MapSizeX; x++)
            {
                for (int y = 0; y < mod.moduleAreasObjects[index].MapSizeY; y++)
                {
                    values[x,y] = 9999;
                }
            }
        }

        //helper functions
        public void buildPath(Coordinate start, Prop callingProp, int index)
        {
            try
            {
                int minX = 0;
                int minY = 0;
                int maxX = mod.moduleAreasObjects[index].MapSizeX;
                int maxY = mod.moduleAreasObjects[index].MapSizeY;
                int numberOfSquaresInArea = mod.moduleAreasObjects[index].MapSizeX * mod.moduleAreasObjects[index].MapSizeY;

                //iterate through all values for next number and evaluate neighbors
                int next = 0;
                for (int cnt = 0; cnt < numberOfSquaresInArea; cnt++)
                {
                    //1100 is used because 32*32=1024 and rounded up to 1100, NOTE; old explanation, delete then
                    for (int x = minX; x < maxX; x++)
                    {
                        for (int y = minY; y < maxY; y++)
                        {
                            if (values[x, y] == next)
                            {
                                if ((x + 1 < maxX) && (evaluateValue(x + 1, y, next, callingProp, x, y, index)))
                                {
                                    foundEnd = true;
                                    return;
                                }
                                if ((x - 1 >= minX) && (evaluateValue(x - 1, y, next, callingProp, x, y, index)))
                                {
                                    foundEnd = true;
                                    return;
                                }
                                if ((y + 1 < maxY) && (evaluateValue(x, y + 1, next, callingProp, x, y, index)))
                                {
                                    foundEnd = true;
                                    return;
                                }
                                if ((y - 1 >= minY) && (evaluateValue(x, y - 1, next, callingProp, x, y, index)))
                                {
                                    foundEnd = true;
                                    return;
                                }
                            }
                        }
                    }
                    next++;
                }
            }
            catch
            {
                int dhdh = 0;
                gv.cc.addLogText("<font color='yellow'>"  + "CRASH in buildPath " +  "</font><BR>");

            }
        }
        public bool evaluateValue(int x, int y, int next, Prop callingProp, int originX, int originY, int index)
        {
            //if ((originX == 1 && originY == 3 ) && (x == 2 && y == 3))
            //{
            //int i = 0;
            //}
            //this will be set to true if currently checked square (next+1) can be reached
            //we will not need to care for walkable as this was handled already beforehand (setting grid[x,y] to 1, ie closed)
            //whether one can enter depends on the squares eligible as squares beforehand in path (next - 1, tricky, can be several), the current square (originX, originY, ie next) and the target square (x,y, ie next+1)
            //all of these can be normal squares, bridges or ramps and all can have different height levels

            /*

            I. EW bridges - No assginement unless
1. if value(x,y)next-1 square is east or west of bridge, value(x,y)next+1 can be assigned to the opposite squre (east or west) IF that square isWalkable && ((isRamp and has +1 height level)|| (has same height level))
2. if value(x,y)next-1 square is north or south of bridge, value(x,y)next+1 can be assigned to the the opposite square (north or south) IF that square isWalkable && ((isRamp and has same height level)|| (has same height level -1))

I. NS bridges - No assignment unless
1. if value(x,y)next-1 square is north or south of bridge, value(x,y)next+1 can be assigned to the the opposite square (north or south) IF that square isWalkable && ((isRamp and has +1 height level)|| (has same height level))
2. if value(x,y)next-1 square is east or west of bridge, value(x,y)next+1 can be assigned to the opposite square (east or west) IF that square isWalkable && ((isRamp and has same height level)|| (has same height level -1))

Defien when bridge squares can enetred. same height or 1 lower; if ramp also 1 higher

Bridges broader than 1 sqaure?

RULES:
1. Bridges squares dont neighbour each other (ie length is only one and no bridge next to bridge)
2. They have same height squares at their end points and no same height squares at their sides
3. Ramps can be netered from all sides


            */

            //if (mod.moduleAreasObjects[index].Tiles)
            //mod.moduleAreasObjects[index].Tiles[yy * mod.moduleAreasObjects[index].MapSizeX + xx];


            //get the next -1 squares
            //List<Tile> formerTiles = new List<Tile>();
            try
            { 
            Tile easternTile = new Tile();
            Tile westernTile = new Tile();
            Tile southernTile = new Tile();
            Tile northernTile = new Tile();
            bool easternTileIsPriorTile = false;
            bool westernTileIsPriorTile = false;
            bool northernTileIsPriorTile = false;
            bool southernTileIsPriorTile = false;

            bool allowAssignment = false;


            //former to east
            if (originX + 1 <= mod.moduleAreasObjects[index].MapSizeX - 1)
            {
                if (next == 0)
                {
                    if ((callingProp.lastLocationX == (originX + 1)) && (callingProp.lastLocationY == originY))
                    {
                        easternTile = mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX + 1];
                        easternTileIsPriorTile = true;
                    }
                }
                //wrog to assume former here as the bridge woudl not have allow this "jump" move!
                else if (values[originX + 1, originY] == next - 1)
                {
                    easternTile = mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX + 1];
                    easternTileIsPriorTile = true;
                }
            }

            //former to west
            if (originX - 1 >= 0)
            {
                if (next == 0)
                {
                    if ((callingProp.lastLocationX == (originX - 1)) && (callingProp.lastLocationY == originY))
                    {
                        westernTile = mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX - 1];
                        westernTileIsPriorTile = true;
                    }
                }
                else if (values[originX - 1, originY] == next - 1)
                {
                    westernTile = mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX - 1];
                    westernTileIsPriorTile = true;
                }
            }

            //former to south
            if (originY + 1 <= mod.moduleAreasObjects[index].MapSizeY - 1)
            {
                if (next == 0)
                {
                    if ((callingProp.lastLocationX == originX) && (callingProp.lastLocationY == (originY + 1)))
                    {
                        southernTile = mod.moduleAreasObjects[index].Tiles[(originY + 1) * mod.moduleAreasObjects[index].MapSizeX + originX];
                        southernTileIsPriorTile = true;
                    }
                }
                else if (values[originX, originY + 1] == next - 1)
                {
                    southernTile = mod.moduleAreasObjects[index].Tiles[(originY + 1) * mod.moduleAreasObjects[index].MapSizeX + originX];
                    southernTileIsPriorTile = true;
                }
            }

            //former to north
            if (originY - 1 >= 0)
            {
                if (next == 0)
                {
                    if ((callingProp.lastLocationX == originX) && (callingProp.lastLocationY == (originY - 1)))
                    {
                        northernTile = mod.moduleAreasObjects[index].Tiles[(originY - 1) * mod.moduleAreasObjects[index].MapSizeX + originX];
                        northernTileIsPriorTile = true;
                    }
                }
                else if (values[originX, originY - 1] == next - 1)
                {
                    northernTile = mod.moduleAreasObjects[index].Tiles[(originY - 1) * mod.moduleAreasObjects[index].MapSizeX + originX];
                    northernTileIsPriorTile = true;
                }
            }

            //Which square are we on? Branch from here.

            //1. Current tile (next, orifinX, originY) is normal tile
            if (!mod.moduleAreasObjects[index].Tiles[(originY) * mod.moduleAreasObjects[index].MapSizeX + originX].isRamp && !mod.moduleAreasObjects[index].Tiles[(originY) * mod.moduleAreasObjects[index].MapSizeX + originX].isEWBridge && !mod.moduleAreasObjects[index].Tiles[(originY) * mod.moduleAreasObjects[index].MapSizeX + originX].isNSBridge)
            {
                //has same height level as target square?
                if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel)
                {
                    //division
                    if (!mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].isRamp)
                    {
                        allowAssignment = true;
                    }

                    //Coming from north division
                    else if ((originY == y - 1) && (originX == x))
                    {
                        //ramp has bottom end facing to north
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowS)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowE)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowW)
                        {
                            allowAssignment = true;
                        }
                    }
                    //Coming from south division
                    else if ((originY == y + 1) && (originX == x))
                    {
                        //ramp has bottom end facing to south
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowN)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowE)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowW)
                        {
                            allowAssignment = true;
                        }

                    }

                    //Coming from east division
                    else if ((originY == y) && (originX == x + 1))
                    {
                        //ramp has bottom end facing to east
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowW)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowS)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowN)
                        {
                            allowAssignment = true;
                        }
                    }

                    //Coming from west division
                    else if ((originY == y) && (originX == x - 1))
                    {
                        //ramp has bottom end facing to west
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowE)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowN)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowS)
                        {
                            allowAssignment = true;
                        }
                    }

                }

                //target is ramp, then additionally entering from one height level lower (climb up ramp) works
                else if ((mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel) && mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].isRamp)
                {
                    //Coming from north division
                    if ((originY == y - 1) && (originX == x))
                    {
                        //ramp has bottom end facing to north
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowN)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowE)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowW)
                        {
                            allowAssignment = true;
                        }
                    }

                    //Coming from south division
                    if ((originY == y + 1) && (originX == x))
                    {
                        //ramp has bottom end facing to south
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowS)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowE)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowW)
                        {
                            allowAssignment = true;
                        }
                    }

                    //Coming from east division
                    if ((originY == y) && (originX == x + 1))
                    {
                        //ramp has bottom end facing to east
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowE)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowS)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowN)
                        {
                            allowAssignment = true;
                        }
                    }

                    //Coming from west division
                    if ((originY == y) && (originX == x - 1))
                    {
                        //ramp has bottom end facing to west
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowW)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowS)
                        {
                            allowAssignment = true;
                        }
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowN)
                        {
                            allowAssignment = true;
                        }
                    }
                }

                //target is EW-Bridge and origin square is north or south of bridge , then additionally entering from one height level lower works
                else if ((mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel) && mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].isEWBridge)
                {
                    //checking northern or southern square as target
                    if ((originY > y) || (originY < y))
                    {
                        allowAssignment = true;
                    }
                }

                //target is NS-Bridge and origin square is east or west of bridge , then additionally entering from one height level lower works
                else if ((mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel) && mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].isNSBridge)
                {
                    //checking eastern or western square as target
                    if ((originX > x) || (originX < x))
                    {
                        allowAssignment = true;
                    }
                }
            }

            //2.Current tile is ramp
            else if (mod.moduleAreasObjects[index].Tiles[(originY) * mod.moduleAreasObjects[index].MapSizeX + originX].isRamp)
            {
                //target has same or one height level higher (two subbrackets)
                //same height (plain)
                if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel)
                {
                    //division
                    //going towards north
                    if ((originY == y + 1) && (originX == x))
                    {
                        //ORIGIN ramp has bottom end facing to north
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].hasDownStairShadowN)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    //going towards south
                    else if ((originY == y - 1) && (originX == x))
                    {
                        //ORIGIN ramp has bottom end facing to south
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].hasDownStairShadowS)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    //going towards east
                    else if ((originY == y) && (originX == x - 1))
                    {
                        //ORIGIN ramp has bottom end facing to east
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].hasDownStairShadowE)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    //going towards west
                    else if ((originY == y) && (originX == x + 1))
                    {
                        //ORIGIN ramp has bottom end facing to west
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].hasDownStairShadowW)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }
                    //liekely not needed?
                    else
                    {
                        allowAssignment = true;
                    }

                }
                //target lower (walk down)
                else if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel)
                {
                    //division
                    //allowAssignment = true;
                    //going towards north
                    if ((originY == y + 1) && (originX == x))
                    {
                        //ORIGIN ramp has bottom end facing to south
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].hasDownStairShadowS)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    //going towards south
                    else if ((originY == y - 1) && (originX == x))
                    {
                        //ORIGIN ramp has bottom end facing to north
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].hasDownStairShadowN)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    //going towards east
                    else if ((originY == y) && (originX == x - 1))
                    {
                        //ORIGIN ramp has bottom end facing to west
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].hasDownStairShadowW)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    //going towards west
                    else if ((originY == y) && (originX == x + 1))
                    {
                        //ORIGIN ramp has bottom end facing to east
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].hasDownStairShadowE)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }
                    //likely not needed
                    else
                    {
                        allowAssignment = true;
                    }
                }

                //target is EW-Bridge and origin square is north or south of bridge , then additionally entering from one height level lower (climb up ramp) works
                else if ((mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel) && mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].isEWBridge)
                {
                    //checking northern or southern square as target
                    if ((originY > y) || (originY < y))
                    {
                        allowAssignment = true;
                    }
                }

                //target is NS-Bridge and origin square is east or west of bridge , then additionally entering from one height level lower (climb up ramp) works
                else if ((mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel) && mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].isNSBridge)
                {
                    //checking eastern or western square as target
                    if ((originX > x) || (originX < x))
                    {
                        allowAssignment = true;
                    }
                }

                //target is  ramp: we were missing ramp-ramp here and we are walking upward by one square (same or lower height is already caught above)
                else if ((mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].isRamp) && (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel))
                {
                    //division
                    //ADJUST BELOW
                    //going towards north
                    if ((originY == y + 1) && (originX == x))
                    {
                        //TARGET ramp has bottom end facing to north
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowN)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    //going towards south
                    else if ((originY == y - 1) && (originX == x))
                    {
                        //TAREGT ramp has bottom end facing to south
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowS)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    //going towards east
                    else if ((originY == y) && (originX == x - 1))
                    {
                        //TARGET has bottom end facing to east
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowE)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    //going towards west
                    else if ((originY == y) && (originX == x + 1))
                    {
                        //TAREGT ramp has bottom end facing to west
                        if (mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].hasDownStairShadowW)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }
                }
            }

            //3. Current tile is EW-bridge
            else if (mod.moduleAreasObjects[index].Tiles[(originY) * mod.moduleAreasObjects[index].MapSizeX + originX].isEWBridge)
            {
                //a. check west or east
                if ((originX > x) || (originX < x))
                {
                    //must move OVER bridge in western or eastern direction
                    if ((easternTileIsPriorTile) || (westernTileIsPriorTile))
                    {
                        //has same height level as target square?
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }
                }

                //b. check north or south
                if ((originY > y) || (originY < y))
                {
                    //must move UNDER bridge in northern or southern direction
                    if ((southernTileIsPriorTile) || (northernTileIsPriorTile))
                    {
                        //has height level of target square - 1?
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }
                }
            }

            //4. Current tile is NS-bridge, 
            else if (mod.moduleAreasObjects[index].Tiles[(originY) * mod.moduleAreasObjects[index].MapSizeX + originX].isNSBridge)
            {
                //a. check north or south
                if ((originY > y) || (originY < y))
                {
                    //must move OVER bridge in northern or southern direction
                    if ((northernTileIsPriorTile) || (southernTileIsPriorTile))
                    {
                        //has same height level as target square?
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }
                }

                //b. check east or west
                if ((originX > x) || (originX < x))
                {
                    //must move UNDER bridge in eastern or western direction
                    if ((easternTileIsPriorTile) || (westernTileIsPriorTile))
                    {
                        //has height level of target square - 1?
                        if (mod.moduleAreasObjects[index].Tiles[originY * mod.moduleAreasObjects[index].MapSizeX + originX].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[y * mod.moduleAreasObjects[index].MapSizeX + x].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }
                }
            }


            if (allowAssignment)
            {
                //evaluate each surrounding node and replace if greater than next number + 1
                //check for end           
                if (grid[x, y] == 3)
                {
                    values[x, y] = next + 1;
                    return true; //found end
                }
                //check if open and replace if lower
                //this is our walkable check
                if (grid[x, y] == 0)
                {
                    if (values[x, y] > next + 1)
                    {
                        values[x, y] = next + 1;
                    }
                }

                return false; //didn't find end
            }
            else
            {
                return false;
            }
        }
        catch
        {
        int gfhf = 0;
                gv.cc.addLogText("<font color='yellow'>" + "CRASH in evaluateValue " + "</font><BR>");
                return false;
        }
        }
        public Coordinate getLowestNeighbor(Coordinate p, Prop callingProp, int pathLength, int index)
        {
            int maxX = mod.moduleAreasObjects[index].MapSizeX;
            int maxY = mod.moduleAreasObjects[index].MapSizeY;
            Coordinate lowest = new Coordinate();
            int val = 1000;
            int lastTileNumber = -1;
            int priorHeightLevelOnPath = -1;
            //if (p.X == 1 && p.Y == 3)
            //{
                //int i = 1;
            //}
            if (pathNodes.Count > 1)
            {
                lastTileNumber = pathNodes[pathNodes.Count - 2].Y * mod.moduleAreasObjects[index].MapSizeX + pathNodes[pathNodes.Count - 2].X;
            }
            if (lastTileNumber > -1)
            {
                priorHeightLevelOnPath = mod.moduleAreasObjects[index].Tiles[lastTileNumber].heightLevel;
            }
            
            if (pathNodes.Count == pathLength)
            {
                priorHeightLevelOnPath = callingProp.lastLocationZ;
            }

            //pathNodes[0].X = 0;
            //mod.moduleAreasObjects[index].Tiles

                //First sort order: check in 4 directions

                //checking towards EAST
                if ((p.X + 1 < maxX) && (values[p.X + 1, p.Y] < val))
                {
                //NOTE: now adding directional ramps to below thoughts
                //within each direction exist two fundamentally different scenarios: 
                //1. current tile is NO bridge (normal case)
                //2. current tile is bridge:
                //we have to check if we are 
                //a) currently under bridge[priorHeightLevelOnPath is one lower than on current tile]
                //Consequence if true: only allow assigning target tile that is one height level LOWER than current tile  
                //b) currently ontop bridge[priorHeightLevelOnPath is same as current tile] 
                //Consequence if true: only allow assigning target tile that is SAME height level as current tile 

                //note: add fail save check for priorHeightLevelOnPath == -1 (which marks te final targt point that can always be entered, bridge is no problem) 

                //first scenario: current tile is NO bridge
                if (!mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge && !mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                {
                    bool allowAssignment = false;

                    //same height level target always works, UNLESS we are on ramp and try to leave via LOW end OR we go towrds low end of neighbouring ramp
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel)
                    {
                        //try to leave ramp via low end
                        //we are in the check east branch of the code here
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowE)
                        {

                        }
                        //go towards low end of neighbouring ramp
                        else if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].hasDownStairShadowW)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    /*
                    //if under bridge currently, also one height level lower also works
                    //determine via lastlocationZ of prop
                    //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge || mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                    {
                        if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel))
                        {
                            allowAssignment = true;
                        }
                    }
                    */

                    //if on ramp currently, one level lower also works
                    //DEPENDING ON DIRECTION, TODO!
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isRamp)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel)
                        {
                            if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowW)
                            {

                            }
                            else
                            {
                                //division
                                allowAssignment = true;
                            }
                        }
                    }

                    //if target is ramp, one level higher also works, again depending on direction
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].isRamp)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel)
                        {
                            if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].hasDownStairShadowE)
                            {

                            }
                            else
                            {
                                allowAssignment = true;
                            }
                        }
                    }

                    //if target is bridge one level higher and opposite side square has value of target -1, assign
                    //NS
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].isNSBridge)
                    {
                        //check one to east
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel)
                        {
                            //check two to east
                            if (values[p.X + 2, p.Y] == values[p.X + 1, p.Y] - 1)
                            {
                                allowAssignment = true;
                            }
                        }
                    }

                    if (allowAssignment)
                    {
                        val = values[p.X + 1, p.Y];
                        lowest = new Coordinate(p.X + 1, p.Y);
                    }
                }

                // current tile IS bridge
                else
                {
                    bool allowAssignment = false;
                    //currently under bridge
                    if (priorHeightLevelOnPath < mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        //bool allowAssignment = false;

                        //one height level LOWER always works
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel)
                        {
                            allowAssignment = true;
                        }

                        //build rule: no ramps on side of bridges
                        //also ramp as target tile always works (direct ascension from under bridge)
                        //if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X+1].isRamp)
                        //{
                            //allowAssignment = true;
                        //}
                    }

                    //currently ontop bridge
                    if (priorHeightLevelOnPath == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        //bool allowAssignment = false;

                        //same height level always works
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }

                    if (allowAssignment)
                    {
                        val = values[p.X + 1, p.Y];
                        lowest = new Coordinate(p.X + 1, p.Y);
                    }

                }
            }
                //now adjust changes from above for the other three directions; TODO
                //checking towards west
                if ((p.X - 1 >= 0) && (values[p.X - 1, p.Y] < val))
                {
                //first scenario: current tile is NO bridge
                if (!mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge && !mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                {
                    bool allowAssignment = false;

                    //same hieght level target always works
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].heightLevel)
                    {
                        //try to leave ramp via low end
                        //we are in the check west branch of the code here
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowW)
                        {

                        }
                        //go towards low end of neighbouring ramp
                        else if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].hasDownStairShadowE)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }


                    //if under bridge currently, also one height level lower also works
                    //determine via lastlocationZ of prop
                    //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge || mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                    {
                        if ((priorHeightLevelOnPath + 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel) && (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel))
                        {
                            allowAssignment = true;
                        }
                    }


                    //if on ramp currently, one level lower also works
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isRamp)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].heightLevel)
                        {
                            if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowE)
                            {

                            }
                            else
                            {
                                //division
                                allowAssignment = true;
                            }
                        }
                    }

                    //if target is ramp, one level higher also works
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].isRamp)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].heightLevel)
                        {
                            if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].hasDownStairShadowW)
                            {

                            }
                            else
                            {
                                allowAssignment = true;
                            }
                        }
                    }

                    //if target is bridge, one level higher also works
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].isEWBridge || mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].isNSBridge)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }

                    //if target is bridge one level higher and opposite side square has value of target -1, assign
                    //NS
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].isNSBridge)
                    {
                        //check one to west
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].heightLevel)
                        {
                            //check two to west
                            if (values[p.X - 2, p.Y] == values[p.X - 1, p.Y] - 1)
                            {
                                allowAssignment = true;
                            }
                        }
                    }

                    if (allowAssignment)
                    {
                        val = values[p.X - 1, p.Y];
                        lowest = new Coordinate(p.X - 1, p.Y);
                    }
                }
                // current tile IS bridge
                else
                {
                    bool allowAssignment = false;
                    //currently under bridge
                    if (priorHeightLevelOnPath < mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        //bool allowAssignment = false;

                        //one height level LOWER always works
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].heightLevel)
                        {
                            allowAssignment = true;
                        }

                        //build rule: no ramps on side of bridges
                        //also ramp as target tile always works (direct ascension from under bridge)
                        //if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X+1].isRamp)
                        //{
                        //allowAssignment = true;
                        //}
                    }

                    //currently ontop bridge
                    if (priorHeightLevelOnPath == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        //bool allowAssignment = false;

                        //same height level always works
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }

                    if (allowAssignment)
                    {
                        val = values[p.X - 1, p.Y];
                        lowest = new Coordinate(p.X - 1, p.Y);
                    }
                }
            }
                
            //checking towards south
            if ((p.Y + 1 < maxY) && (values[p.X, p.Y + 1] < val))
            {
                //first scenario: current tile is NO bridge
                if (!mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge && !mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                {
                    bool allowAssignment = false;

                    //same height level target always works
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel == mod.moduleAreasObjects[index].Tiles[(p.Y + 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        //we are in the check south branch of the code here
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowS)
                        {

                        }
                        //go towards low end of neighbouring ramp
                        else if (mod.moduleAreasObjects[index].Tiles[(p.Y+1) * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowN)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    /*
                    //if under bridge currently, also one height level lower also works
                    //determine via lastlocationZ of prop
                    //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge || mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                    {
                        if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel))
                        {
                            allowAssignment = true;
                        }
                    }
                    */

                    //if on ramp currently, one level lower also works
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isRamp)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[(p.Y + 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowN)
                            {

                            }
                            else
                            {
                                //division
                                allowAssignment = true;
                            }
                        }
                    }

                    //if target is ramp, one level higher also works
                    if (mod.moduleAreasObjects[index].Tiles[(p.Y + 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].isRamp)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[(p.Y + 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            if (mod.moduleAreasObjects[index].Tiles[(p.Y+1) * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowS)
                            {

                            }
                            else
                            {
                                allowAssignment = true;
                            }
                        }
                    }

                    //if target is bridge, one level higher also works
                    if (mod.moduleAreasObjects[index].Tiles[(p.Y+1) * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge || mod.moduleAreasObjects[index].Tiles[(p.Y+1) * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[(p.Y+1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }

                    //if target is bridge one level higher and opposite side square has value of target -1, assign
                    //EW
                    if (mod.moduleAreasObjects[index].Tiles[(p.Y+1) * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge)
                    {
                        //check one to south
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[(p.Y+1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            //check two to south
                            if (values[p.X, p.Y + 2] == values[p.X, p.Y + 1] - 1)
                            {
                                allowAssignment = true;
                            }
                        }
                    }

                    if (allowAssignment)
                    {
                        val = values[p.X, p.Y + 1];
                        lowest = new Coordinate(p.X, p.Y + 1);
                    }
                }
                // current tile IS bridge
                else
                {
                    bool allowAssignment = false;

                    //currently under bridge
                    if (priorHeightLevelOnPath < mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        //bool allowAssignment = false;

                        //one height level LOWER always works
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[(p.Y+1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            allowAssignment = true;
                        }

                        //build rule: no ramps on side of bridges
                        //also ramp as target tile always works (direct ascension from under bridge)
                        //if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X+1].isRamp)
                        //{
                        //allowAssignment = true;
                        //}
                    }

                    //currently ontop bridge
                    if (priorHeightLevelOnPath == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        ///bool allowAssignment = false;

                        //same height level always works
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel == mod.moduleAreasObjects[index].Tiles[(p.Y+1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }
                    if (allowAssignment)
                    {
                        val = values[p.X, p.Y + 1];
                        lowest = new Coordinate(p.X, p.Y + 1);
                    }
                }
            }
            //checking towards north
            if ((p.Y - 1 >= 0) && (values[p.X, p.Y - 1] < val))
            {
                //first scenario: current tile is NO bridge
                if (!mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge && !mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                {
                    bool allowAssignment = false;

                    //same hieght level target always works
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel == mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        //we are in the check south branch of the code here
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowN)
                        {

                        }
                        //go towards low end of neighbouring ramp
                        else if (mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowS)
                        {

                        }
                        else
                        {
                            allowAssignment = true;
                        }
                    }

                    /*
                    //if under bridge currently, also one height level lower also works
                    //determine via lastlocationZ of prop
                    //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge || mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                    {
                        if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel))
                        {
                            allowAssignment = true;
                        }
                    }
                    */

                    //if on ramp currently, one level lower also works
                    if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isRamp)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowS)
                            {

                            }
                            else
                            {
                                //division
                                allowAssignment = true;
                            }
                        }
                    }

                    //if target is ramp, one level higher also works
                    if (mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].isRamp)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }

                    //if target is bridge, one level higher also works
                    if (mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge || mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                    {
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            if (mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].hasDownStairShadowN)
                            {

                            }
                            else
                            {
                                allowAssignment = true;
                            }
                        }
                    }

                    //if target is bridge one level higher and opposite side square has value of target -1, assign
                    //EW
                    if (mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge)
                    {
                        //check one to north
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel + 1 == mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            //check two to north
                            if (values[p.X, p.Y - 2] == values[p.X, p.Y - 1] - 1)
                            {
                                allowAssignment = true;
                            }
                        }
                    }

                    if (allowAssignment)
                    {
                        val = values[p.X, p.Y - 1];
                        lowest = new Coordinate(p.X, p.Y - 1);
                    }
                }
                // current tile IS bridge
                else
                {
                    bool allowAssignment = false;
                    //currently under bridge
                    if (priorHeightLevelOnPath < mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        //bool allowAssignment = false;

                        //one height level LOWER always works
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel - 1 == mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            allowAssignment = true;
                        }

                        //build rule: no ramps on side of bridges
                        //also ramp as target tile always works (direct ascension from under bridge)
                        //if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X+1].isRamp)
                        //{
                        //allowAssignment = true;
                        //}
                    }

                    //currently ontop bridge
                    if (priorHeightLevelOnPath == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        //bool allowAssignment = false;

                        //same height level always works
                        if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel == mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                        {
                            allowAssignment = true;
                        }
                    }

                    if (allowAssignment)
                    {
                        val = values[p.X, p.Y - 1];
                        lowest = new Coordinate(p.X, p.Y - 1);
                    }
                }
            }

            return lowest;
            /*
            int maxX = mod.currentEncounter.MapSizeX;
            int maxY = mod.currentEncounter.MapSizeY;
            Coordinate lowest = new Coordinate();
            int val = 1000;
            if ((p.X + 1 < maxX) && (values[p.X + 1, p.Y] < val))
            {
                val = values[p.X + 1, p.Y];
                lowest = new Coordinate(p.X + 1, p.Y);
            }
            if ((p.X - 1 >= 0) && (values[p.X - 1, p.Y] < val))
            {
                val = values[p.X - 1, p.Y];
                lowest = new Coordinate(p.X - 1, p.Y);
            }
            if ((p.Y + 1 < maxY) && (values[p.X, p.Y + 1] < val))
            {
                val = values[p.X, p.Y + 1];
                lowest = new Coordinate(p.X, p.Y + 1);
            }
            if ((p.Y - 1 >= 0) && (values[p.X, p.Y - 1] < val))
            {
                val = values[p.X, p.Y - 1];
                lowest = new Coordinate(p.X, p.Y - 1);
            }
            if ((p.X + 1 < maxX) && (p.Y + 1 < maxY) && (values[p.X + 1, p.Y + 1] < val))
            {
                val = values[p.X + 1, p.Y + 1];
                lowest = new Coordinate(p.X + 1, p.Y + 1);
            }
            if ((p.X - 1 >= 0) && (p.Y - 1 >= 0) && (values[p.X - 1, p.Y - 1] < val))
            {
                val = values[p.X - 1, p.Y - 1];
                lowest = new Coordinate(p.X - 1, p.Y - 1);
            }
            if ((p.X - 1 >= 0) && (p.Y + 1 < maxY) && (values[p.X - 1, p.Y + 1] < val))
            {
                val = values[p.X - 1, p.Y + 1];
                lowest = new Coordinate(p.X - 1, p.Y + 1);
            }
            if ((p.X + 1 < maxX) && (p.Y - 1 >= 0) && (values[p.X + 1, p.Y - 1] < val))
            {
                val = values[p.X + 1, p.Y - 1];
                lowest = new Coordinate(p.X + 1, p.Y - 1);
            }
            return lowest;
            
            //work with last location of calling prop
            
            int maxX = mod.moduleAreasObjects[index].MapSizeX;
            int maxY = mod.moduleAreasObjects[index].MapSizeY;
            Coordinate lowest = new Coordinate();
            int val = 1000;

            //checking towards east
            if ((p.X + 1 < maxX) && (values[p.X + 1,p.Y] < val))
            {
                bool allowAssignment = false;
                
                //same hieght level target always works
                if (callingProp.LocationZ == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel)
                {
                    allowAssignment = true;
                }
                
                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge || mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }

                //if on ramp currently, one level lower also works
                if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isRamp)
                {
                    if (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X + 1].heightLevel)
                    {
                        allowAssignment = true;
                    }
                }

                if (allowAssignment)
                {
                    val = values[p.X + 1, p.Y];
                    lowest = new Coordinate(p.X + 1, p.Y);
                }
            }
            //checking towards west
            if ((p.X - 1 >= 0) && (values[p.X - 1,p.Y] < val))
            {
                bool allowAssignment = false;

                //same height level target always works
                if (callingProp.LocationZ == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].heightLevel)
                {
                    allowAssignment = true;
                }

                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge || mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }

                //if on ramp currently, one level lower also works
                if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isRamp)
                {
                    if (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X - 1].heightLevel)
                    {
                        allowAssignment = true;
                    }
                }

                if (allowAssignment)
                {
                    val = values[p.X - 1, p.Y];
                    lowest = new Coordinate(p.X - 1, p.Y);
                }
            }
            //checking towards south
            if ((p.Y + 1 < maxY) && (values[p.X,p.Y + 1] < val))
            {
                bool allowAssignment = false;

                //same hieght level target always works
                if (callingProp.LocationZ == mod.moduleAreasObjects[index].Tiles[(p.Y + 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                {
                    allowAssignment = true;
                }

                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge || mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[(p.Y + 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }

                //if on ramp currently, one level lower also works
                if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isRamp)
                {
                    if (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[(p.Y + 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        allowAssignment = true;
                    }
                }

                if (allowAssignment)
                {
                    val = values[p.X, p.Y + 1];
                    lowest = new Coordinate(p.X, p.Y + 1);
                }
            }
            //checking towards north
            if ((p.Y - 1 >= 0) && (values[p.X,p.Y - 1] < val))
            {
                bool allowAssignment = false;

                //same hieght level target always works
                if (callingProp.LocationZ == mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                {
                    allowAssignment = true;
                }

                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isEWBridge || mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }

                //if on ramp currently, one level lower also works
                if (mod.moduleAreasObjects[index].Tiles[p.Y * mod.moduleAreasObjects[index].MapSizeX + p.X].isRamp)
                {
                    if (callingProp.LocationZ - 1 == mod.moduleAreasObjects[index].Tiles[(p.Y - 1) * mod.moduleAreasObjects[index].MapSizeX + p.X].heightLevel)
                    {
                        allowAssignment = true;
                    }
                }

                if (allowAssignment)
                {
                    val = values[p.X, p.Y - 1];
                    lowest = new Coordinate(p.X, p.Y - 1);
                }
            }
          
            return lowest;
            */
        }
        public bool isWalkable(int col, int row, int index)
        {
            if (mod.moduleAreasObjects[index].Tiles[row * mod.moduleAreasObjects[index].MapSizeX + col].Walkable)
            {
                return true;
            }
            return false;
        }
    }
}
