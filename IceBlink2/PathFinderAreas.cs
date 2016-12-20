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

        public Coordinate findNewPoint(Coordinate start, Coordinate end, Prop callingProp)
        {
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
            buildPath(start, callingProp);

            if (!foundEnd)
            {
                //do not build path for now so return (-1,-1), later add code for picking a spot to move
            }
            else
            {
                pathNodes.Add(new Coordinate(end.X, end.Y));
                for (int i = 0; i < values[end.X, end.Y]; i++)
                {
                    pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1], callingProp));
                    //pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1 - i], callingProp));
                }
                //build list of path points
                newPoint = pathNodes[pathNodes.Count - 2];
            }
            callingProp.lengthOfLastPath = pathNodes.Count;
            pathNodes.Clear();
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
        public Coordinate findNewPoint(Coordinate start, Coordinate end, Prop callingProp, int centerPointX, int centerPointY, int radius)
        {
            resetGrid();
            foundEnd = false;
            Coordinate newPoint = new Coordinate(-1, -1);
            //set start location value to 0
            values[start.X, start.Y] = 0;
            //find all props that have collision and set there square to 1
            foreach (Prop prp in mod.currentArea.Props)
            {
                //if  ( ((prp.HasCollision) && (prp.isActive)) || ((prp.isMover) && (prp.isActive)) )
                if ((prp.HasCollision) && (prp.isActive))
                {
                    grid[prp.LocationX, prp.LocationY] = 1;
                }
            }
            grid[start.X, start.Y] = 2; //2 marks the start point in the grid
            grid[end.X, end.Y] = 3; //3 marks the end point in the grid
            buildPath(start, centerPointX, centerPointY, radius, callingProp);

            if (!foundEnd)
            {
                //do not build path for now so return (-1,-1), later add code for picking a spot to move
            }
            else
            {
                pathNodes.Add(new Coordinate(end.X, end.Y));
                for (int i = 0; i < values[end.X, end.Y]; i++)
                {
                    pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1], callingProp));
                }
                //build list of path points
                newPoint = pathNodes[pathNodes.Count - 2];
            }
            callingProp.lengthOfLastPath = pathNodes.Count;
            pathNodes.Clear();
            return newPoint;
        }

        //find new point in square part of an area around a center point withhin a radius, here: record path, too (selected by overload)
        public Coordinate findNewPoint(Coordinate start, Coordinate end, Prop callingProp, int centerPointX, int centerPointY, int radius, bool recordPath, GameView g)
        {

            gv = g;
            resetGrid();
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
            buildPath(start, centerPointX, centerPointY, radius, callingProp);

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

                    pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1], callingProp));
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


        //build path for limited area
        public void buildPath(Coordinate start, int centerPointX, int centerPointY, int radius, Prop callingProp)
        {
            int minX = centerPointX - radius;
            if (minX < 0) { minX = 0; };
            int minY = centerPointY - radius;
            if (minY < 0) { minY = 0; };
            int maxX = centerPointX + radius + 1;
            if (maxX > mod.currentArea.MapSizeX - 1) { maxX = mod.currentArea.MapSizeX - 1; }
            int maxY = centerPointY + radius + 1;
            if (maxY > mod.currentArea.MapSizeY - 1) { maxY = mod.currentArea.MapSizeY - 1; }
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
                            if ((x + 1 < maxX) && (evaluateValue(x + 1, y, next, callingProp, x, y)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= minX) && (evaluateValue(x - 1, y, next, callingProp, x, y)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y + 1 < maxY) && (evaluateValue(x, y + 1, next, callingProp, x, y)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y - 1 >= minY) && (evaluateValue(x, y - 1, next, callingProp,x ,y)))
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

        //build limted area part grid
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
                        grid[row, col] = 0;
                    }
                    else
                    {
                        grid[row, col] = 1;
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



        //called from outside to reset grid
        public void resetGrid()
        {
    	    grid = new int[mod.currentArea.MapSizeX,mod.currentArea.MapSizeY];
    	    values = new int[mod.currentArea.MapSizeX,mod.currentArea.MapSizeY];
            //create the grid with 1s and 0s
    	    for (int col = 0; col < mod.currentArea.MapSizeX;col++)
    	    {
    		    for (int row = 0; row < mod.currentArea.MapSizeY; row++)
    		    {
    			    if (isWalkable(col,row))
    			    {
    				    grid[row,col] = 0;
    			    }
    			    else
    			    {
    				    grid[row,col] = 1;
    			    }
    		    }
    	    }
        
            //assign 9999 to every value
            for (int x = 0; x < mod.currentArea.MapSizeX; x++)
            {
                for (int y = 0; y < mod.currentArea.MapSizeY; y++)
                {
                    values[x,y] = 9999;
                }
            }
        }

        //helper functions
        public void buildPath(Coordinate start, Prop callingProp)
        {
            int minX = 0;
            int minY = 0;
            int maxX = mod.currentArea.MapSizeX;
            int maxY = mod.currentArea.MapSizeY;
            int numberOfSquaresInArea = mod.currentArea.MapSizeX * mod.currentArea.MapSizeY;

            //iterate through all values for next number and evaluate neighbors
            int next = 0;
            for (int cnt = 0; cnt < numberOfSquaresInArea; cnt++)
            {
                //1100 is used because 32*32=1024 and rounded up to 1100, NOTE; old explanation, delete then
                for (int x = minX; x < maxX; x++)
                {            
                    for (int y = minY; y < maxY; y++)
                    {
                        if (values[x,y] == next)
                        {
                            if ((x + 1 < maxX) && (evaluateValue(x + 1, y, next, callingProp, x, y)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= minX) && (evaluateValue(x - 1, y, next, callingProp, x, y)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y + 1 < maxY) && (evaluateValue(x, y + 1, next, callingProp, x, y)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y - 1 >= minY) && (evaluateValue(x, y - 1, next, callingProp, x, y)))
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
        public bool evaluateValue(int x, int y, int next, Prop callingProp, int originX, int originY)
        {

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

            //if (mod.currentArea.Tiles)
            //mod.currentArea.Tiles[yy * mod.currentArea.MapSizeX + xx];


            //get the next -1 squares
            //List<Tile> formerTiles = new List<Tile>();
            Tile easternTile =  new Tile();
            Tile westernTile = new Tile();
            Tile southernTile = new Tile();
            Tile northernTile = new Tile();
            bool easternTileIsPriorTile = false;
            bool westernTileIsPriorTile = false;
            bool northernTileIsPriorTile = false;
            bool southernTileIsPriorTile = false;

            bool allowAssignment = false;
            
            
                //former to east
                if (originX + 1 <= mod.currentArea.MapSizeX - 1)
                {
                    if (values[originX + 1, originY] == next - 1)
                    {
                        easternTile = mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX + 1];
                        easternTileIsPriorTile = true;
                    }
                }

                //former to west
                if (originX - 1 >= 0)
                {
                    if (values[originX - 1, originY] == next - 1)
                    {
                        westernTile = mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX - 1];
                        westernTileIsPriorTile = true;
                    }
                }

                //former to south
                if (originY + 1 <= mod.currentArea.MapSizeY - 1)
                {
                    if (values[originX, originY + 1] == next - 1)
                    {
                        southernTile = mod.currentArea.Tiles[(originY + 1) * mod.currentArea.MapSizeX + originX];
                        southernTileIsPriorTile = true;
                    }
                }

                //former to north
                if (originY - 1 >= 0)
                {
                    if (values[originX, originY - 1] == next - 1)
                    {
                        northernTile = mod.currentArea.Tiles[(originY - 1) * mod.currentArea.MapSizeX + originX];
                        northernTileIsPriorTile = true;
                    }
                }

                //Which square are we on? Branch from here.

                //1. Current tile (next, orifinX, originY) is normal tile
                if (!mod.currentArea.Tiles[(originY) * mod.currentArea.MapSizeX + originX].isRamp && !mod.currentArea.Tiles[(originY) * mod.currentArea.MapSizeX + originX].isEWBridge && !mod.currentArea.Tiles[(originY) * mod.currentArea.MapSizeX + originX].isNSBridge)
                {
                    //has same height level as target square?
                    if (mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel)
                    {
                        allowAssignment = true;
                    }

                    //target is ramp, then additionally entering from one height level lower (climb up ramp) works
                    else if ((mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel + 1 == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel) && mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].isRamp)
                    {
                        allowAssignment = true;
                    }

                    //target is EW-Bridge and origin square is north or south of bridge , then additionally entering from one height level lower (climb up ramp) works
                    else if ((mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel + 1 == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel) && mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].isEWBridge)
                    {
                        //checking northern or southern square as target
                        if ((originY > y) || (originY < y))
                        {
                            allowAssignment = true;
                        }
                    }

                    //target is NS-Bridge and origin square is east or west of bridge , then additionally entering from one height level lower (climb up ramp) works
                    else if ((mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel + 1 == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel) && mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].isNSBridge)
                    {
                        //checking eastern or western square as target
                        if ((originX > x) || (originX < x))
                        {
                            allowAssignment = true;
                        }
                    }
                }

                //2.Current tile is ramp
                else if (mod.currentArea.Tiles[(originY) * mod.currentArea.MapSizeX + originX].isRamp)
                {
                    //target is normal tile, entering from same or one height level higher is possible
                    //same height (plain)
                    if (mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel)
                    {
                        allowAssignment = true;
                    }
                    //target lower (walk down)
                    else if (mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel - 1 == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel)
                    {
                        allowAssignment = true;
                    }

                    //target is EW-Bridge and origin square is north or south of bridge , then additionally entering from one height level lower (climb up ramp) works
                    else if ((mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel + 1 == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel) && mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].isEWBridge)
                    {
                        //checking northern or southern square as target
                        if ((originY > y) || (originY < y))
                        {
                            allowAssignment = true;
                        }
                    }

                    //target is NS-Bridge and origin square is east or west of bridge , then additionally entering from one height level lower (climb up ramp) works
                    else if ((mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel + 1 == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel) && mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].isNSBridge)
                    {
                        //checking eastern or western square as target
                        if ((originX > x) || (originX < x))
                        {
                            allowAssignment = true;
                        }
                    }
                }

                //3. Current tile is EW-bridge
                else if (mod.currentArea.Tiles[(originY) * mod.currentArea.MapSizeX + originX].isEWBridge)
                {
                    //a. check west or east
                    if ((originX > x) || (originX < x))
                    {
                        //must move OVER bridge in western or eastern direction
                        if ((easternTileIsPriorTile) || (westernTileIsPriorTile))
                        {
                            //has same height level as target square?
                            if (mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel)
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
                            if (mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel - 1 == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel)
                            {
                                allowAssignment = true;
                            }
                        }
                    }
                }

                //4. Current tile is NS-bridge, 
                else if (mod.currentArea.Tiles[(originY) * mod.currentArea.MapSizeX + originX].isNSBridge)
                {
                    //a. check north or south
                    if ((originY > y) || (originY < y))
                    {
                        //must move OVER bridge in northern or southern direction
                        if ((northernTileIsPriorTile) || (southernTileIsPriorTile))
                        {
                            //has same height level as target square?
                            if (mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel)
                            {
                                allowAssignment = true;
                            }
                        }
                    }

                    //b. check east or west
                    if ((originX > x) || (originX < x))
                    {
                        //must move UNDER bridge in northern or southern direction
                        if ((easternTileIsPriorTile) || (westernTileIsPriorTile))
                        {
                            //has height level of target square - 1?
                            if (mod.currentArea.Tiles[originY * mod.currentArea.MapSizeX + originX].heightLevel - 1 == mod.currentArea.Tiles[y * mod.currentArea.MapSizeX + x].heightLevel)
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
        public Coordinate getLowestNeighbor(Coordinate p, Prop callingProp)
        {
            int maxX = mod.currentArea.MapSizeX;
            int maxY = mod.currentArea.MapSizeY;
            Coordinate lowest = new Coordinate();
            int val = 1000;

            //checking towards east
            if ((p.X + 1 < maxX) && (values[p.X + 1, p.Y] < val))
            {
                bool allowAssignment = false;

                //same hieght level target always works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X + 1].heightLevel)
                {
                    allowAssignment = true;
                }

                /*
                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isEWBridge || mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X + 1].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }
                */

                //if on ramp currently, one level lower also works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isRamp)
                {
                    if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel - 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X + 1].heightLevel)
                    {
                        allowAssignment = true;
                    }
                }

                //if target is ramp, one level higher also works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X+1].isRamp)
                {
                    if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel + 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X + 1].heightLevel)
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
            if ((p.X - 1 >= 0) && (values[p.X - 1, p.Y] < val))
            {
                bool allowAssignment = false;

                //same hieght level target always works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X - 1].heightLevel)
                {
                    allowAssignment = true;
                }

                /*
                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isEWBridge || mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X + 1].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }
                */

                //if on ramp currently, one level lower also works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isRamp)
                {
                    if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel - 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X - 1].heightLevel)
                    {
                        allowAssignment = true;
                    }
                }

                //if target is ramp, one level higher also works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X-1].isRamp)
                {
                    if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel + 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X - 1].heightLevel)
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
            if ((p.Y + 1 < maxY) && (values[p.X, p.Y + 1] < val))
            {
                bool allowAssignment = false;

                //same hieght level target always works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel == mod.currentArea.Tiles[(p.Y+1) * mod.currentArea.MapSizeX + p.X].heightLevel)
                {
                    allowAssignment = true;
                }

                /*
                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isEWBridge || mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X + 1].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }
                */

                //if on ramp currently, one level lower also works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isRamp)
                {
                    if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel - 1 == mod.currentArea.Tiles[(p.Y+1) * mod.currentArea.MapSizeX + p.X].heightLevel)
                    {
                        allowAssignment = true;
                    }
                }

                //if target is ramp, one level higher also works
                if (mod.currentArea.Tiles[(p.Y+1) * mod.currentArea.MapSizeX + p.X].isRamp)
                {
                    if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel + 1 == mod.currentArea.Tiles[(p.Y+1) * mod.currentArea.MapSizeX + p.X].heightLevel)
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
            if ((p.Y - 1 >= 0) && (values[p.X, p.Y - 1] < val))
            {
                bool allowAssignment = false;

                //same hieght level target always works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel == mod.currentArea.Tiles[(p.Y - 1) * mod.currentArea.MapSizeX + p.X].heightLevel)
                {
                    allowAssignment = true;
                }

                /*
                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isEWBridge || mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X + 1].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }
                */

                //if on ramp currently, one level lower also works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isRamp)
                {
                    if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel - 1 == mod.currentArea.Tiles[(p.Y - 1) * mod.currentArea.MapSizeX + p.X].heightLevel)
                    {
                        allowAssignment = true;
                    }
                }

                //if target is ramp, one level higher also works
                if (mod.currentArea.Tiles[(p.Y-1) * mod.currentArea.MapSizeX + p.X].isRamp)
                {
                    if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].heightLevel + 1 == mod.currentArea.Tiles[(p.Y - 1) * mod.currentArea.MapSizeX + p.X].heightLevel)
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
            
            int maxX = mod.currentArea.MapSizeX;
            int maxY = mod.currentArea.MapSizeY;
            Coordinate lowest = new Coordinate();
            int val = 1000;

            //checking towards east
            if ((p.X + 1 < maxX) && (values[p.X + 1,p.Y] < val))
            {
                bool allowAssignment = false;
                
                //same hieght level target always works
                if (callingProp.LocationZ == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X + 1].heightLevel)
                {
                    allowAssignment = true;
                }
                
                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isEWBridge || mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X + 1].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }

                //if on ramp currently, one level lower also works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isRamp)
                {
                    if (callingProp.LocationZ - 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X + 1].heightLevel)
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
                if (callingProp.LocationZ == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X - 1].heightLevel)
                {
                    allowAssignment = true;
                }

                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isEWBridge || mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X - 1].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }

                //if on ramp currently, one level lower also works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isRamp)
                {
                    if (callingProp.LocationZ - 1 == mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X - 1].heightLevel)
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
                if (callingProp.LocationZ == mod.currentArea.Tiles[(p.Y + 1) * mod.currentArea.MapSizeX + p.X].heightLevel)
                {
                    allowAssignment = true;
                }

                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isEWBridge || mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.currentArea.Tiles[(p.Y + 1) * mod.currentArea.MapSizeX + p.X].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }

                //if on ramp currently, one level lower also works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isRamp)
                {
                    if (callingProp.LocationZ - 1 == mod.currentArea.Tiles[(p.Y + 1) * mod.currentArea.MapSizeX + p.X].heightLevel)
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
                if (callingProp.LocationZ == mod.currentArea.Tiles[(p.Y - 1) * mod.currentArea.MapSizeX + p.X].heightLevel)
                {
                    allowAssignment = true;
                }

                //if under bridge currently, also one height level lower also works
                //determine via lastlocationZ of prop
                //this assumes bridges of length one with -1 height squares on sides and same height squares on end
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isEWBridge || mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isNSBridge)
                {
                    if ((callingProp.lastLocationZ + 1 == callingProp.LocationZ) && (callingProp.LocationZ - 1 == mod.currentArea.Tiles[(p.Y - 1) * mod.currentArea.MapSizeX + p.X].heightLevel))
                    {
                        allowAssignment = true;
                    }
                }

                //if on ramp currently, one level lower also works
                if (mod.currentArea.Tiles[p.Y * mod.currentArea.MapSizeX + p.X].isRamp)
                {
                    if (callingProp.LocationZ - 1 == mod.currentArea.Tiles[(p.Y - 1) * mod.currentArea.MapSizeX + p.X].heightLevel)
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
        public bool isWalkable(int col, int row)
        {
            if (mod.currentArea.Tiles[col * mod.currentArea.MapSizeX + row].Walkable)
            {
                return true;
            }
            return false;
        }
    }
}
