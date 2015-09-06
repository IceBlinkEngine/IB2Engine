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

        //called from outside to get next move location
        /*public Coordinate findNewPoint(Coordinate start, Coordinate end)
    {
    	foundEnd = false;
    	Coordinate newPoint = new Coordinate(-1,-1);
        //set start location value to 0
        values[start.X,start.Y] = 0;
        //find all props that have collision and set there square to 1
        foreach (Prop prp in mod.currentArea.Props)
        {
        	//if  ( ((prp.HasCollision) && (prp.isActive)) || ((prp.isMover) && (prp.isActive)) )
            if  ((prp.HasCollision) && (prp.isActive))
        	{
        		grid[prp.LocationX,prp.LocationY] = 1;
        	}
        }
        grid[start.X,start.Y] = 2; //2 marks the start point in the grid
		grid[end.X,end.Y] = 3; //3 marks the end point in the grid
        buildPath(start);
        
        if (!foundEnd)
        {
            //do not build path for now so return (-1,-1), later add code for picking a spot to move
        }
        else
        {
            pathNodes.Add(new Coordinate(end.X, end.Y));
            for (int i = 0; i < values[end.X,end.Y]; i++)
            {
                pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1]));
            }
            //build list of path points
            newPoint = pathNodes[pathNodes.Count - 2];
        }
        return newPoint;
    }*/

        public Coordinate findNewPoint(Coordinate start, Coordinate end, Prop callingProp)
        {
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
            buildPath(start);

            if (!foundEnd)
            {
                //do not build path for now so return (-1,-1), later add code for picking a spot to move
            }
            else
            {
                pathNodes.Add(new Coordinate(end.X, end.Y));
                for (int i = 0; i < values[end.X, end.Y]; i++)
                {
                    pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1]));
                }
                //build list of path points
                newPoint = pathNodes[pathNodes.Count - 2];
            }
            //string length = pathNodes.Count.ToString();
            //gv.sf.SetGlobalInt("lengthOfLastPath", length);
            callingProp.lengthOfLastPath = pathNodes.Count;
            pathNodes.Clear();
            return newPoint;
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
            buildPath(start, centerPointX, centerPointY, radius);

            if (!foundEnd)
            {
                //do not build path for now so return (-1,-1), later add code for picking a spot to move
            }
            else
            {
                pathNodes.Add(new Coordinate(end.X, end.Y));
                for (int i = 0; i < values[end.X, end.Y]; i++)
                {
                    pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1]));
                }
                //build list of path points
                newPoint = pathNodes[pathNodes.Count - 2];
            }
            //string length = pathNodes.Count.ToString();
            //gv.sf.SetGlobalInt("lengthOfLastPath", length);
            callingProp.lengthOfLastPath = pathNodes.Count;
            pathNodes.Clear();
            return newPoint;
        }

        //find new point in square part of an area around a center point withhin a radius, here: record bath, too (selected by overload)
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
                //if  ( ((prp.HasCollision) && (prp.isActive)) || ((prp.isMover) && (prp.isActive)) )
                if ((prp.HasCollision) && (prp.isActive))
                {
                    grid[prp.LocationX, prp.LocationY] = 1;
                }
            }
            grid[start.X, start.Y] = 2; //2 marks the start point in the grid
            grid[end.X, end.Y] = 3; //3 marks the end point in the grid
            buildPath(start, centerPointX, centerPointY, radius);

            if (!foundEnd)
            {
                //do not build path for now so return (-1,-1), later add code for picking a spot to move
            }
            else
            {
                pathNodes.Add(new Coordinate(end.X, end.Y));
                for (int i = 0; i < values[end.X, end.Y]; i++)
                {
                    pathNodes.Add(getLowestNeighbor(pathNodes[pathNodes.Count - 1]));
                    //Note to self: might be that the order is reverse here, check when debugging

                    int xOffSetInSquares = 0;
                    int yOffSetInSquares = 0;
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
                    int playerPositionXInPix = gv.oXshift + gv.screenMainMap.mapStartLocXinPixels + (gv.playerOffset * gv.squareSize);
                    int playerPositionYInPix = gv.playerOffset * gv.squareSize;

                    if ((xOffSetInSquares <= 4) && (xOffSetInSquares >= -4) && (yOffSetInSquares <= 4) && (yOffSetInSquares >= -4))
                    {
                        callingProp.destinationPixelPositionXList.Add(playerPositionXInPix + (xOffSetInSquares * gv.squareSize));
                        callingProp.destinationPixelPositionYList.Add(playerPositionYInPix + (yOffSetInSquares * gv.squareSize));
                    }
                    
                    //int x = ((pathNodes[pathNodes.Count - 1].X - mod.PlayerLocationX) * 100) + (4 * 100) + 10 + 600;
                    //int y = ((pathNodes[pathNodes.Count - 1].Y - mod.PlayerLocationY) * 100) + (4 * 100);
                    //callingProp.destinationPixelPositionXList.Add(pathNodes[pathNodes.Count-1].X);
                    //callingProp.destinationPixelPositionYList.Add(pathNodes[pathNodes.Count - 1].Y);
                    //callingProp.pixelMoveSpeed++;
                }
                //build list of path points
                //callingProp.pixelMoveSpeed--;
                newPoint = pathNodes[pathNodes.Count - 2];
            }
            //string length = pathNodes.Count.ToString();
            //gv.sf.SetGlobalInt("lengthOfLastPath", length);
            callingProp.lengthOfLastPath = pathNodes.Count;
            pathNodes.Clear();
            return newPoint;
        }


        //build path for limited area
        public void buildPath(Coordinate start, int centerPointX, int centerPointY, int radius)
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
                            if ((x + 1 < maxX) && (evaluateValue(x + 1, y, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= minX) && (evaluateValue(x - 1, y, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y + 1 < maxY) && (evaluateValue(x, y + 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y - 1 >= minY) && (evaluateValue(x, y - 1, next)))
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
        /*public void buildPath(Coordinate start)
        {
            int minX = start.X - 16;
            if (minX < 0) { minX = 0; }
            int minY = start.Y - 16;
            if (minY < 0) { minY = 0; }
            int maxX = start.X + 16;
            if (maxX > mod.currentArea.MapSizeX) { maxX = mod.currentArea.MapSizeX; }
            int maxY = start.Y + 16;
            if (maxY > mod.currentArea.MapSizeY) { maxY = mod.currentArea.MapSizeY; }
            //iterate through all values for next number and evaluate neighbors
            int next = 0;
            for (int cnt = 0; cnt < 1100; cnt++)
            {
                //1100 is used because 32*32=1024 and rounded up to 1100
                for (int x = minX; x < maxX; x++)
                {*/
        public void buildPath(Coordinate start)
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
                            if ((x + 1 < maxX) && (evaluateValue(x + 1, y, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= minX) && (evaluateValue(x - 1, y, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y + 1 < maxY) && (evaluateValue(x, y + 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y - 1 >= minY) && (evaluateValue(x, y - 1, next)))
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
        public bool evaluateValue(int x, int y, int next)
        {
            //evaluate each surrounding node and replace if greater than next number + 1
            //check for end            
            if (grid[x,y] == 3)
            {
                values[x,y] = next + 1;
                return true; //found end
            }
            //check if open and replace if lower
            if (grid[x,y] == 0)
            {
                if (values[x,y] > next + 1)
                {
                    values[x,y] = next + 1;
                }
            }
            return false; //didn't find end
        }
        public Coordinate getLowestNeighbor(Coordinate p)
        {
            int maxX = mod.currentArea.MapSizeX;
            int maxY = mod.currentArea.MapSizeY;
            Coordinate lowest = new Coordinate();
            int val = 1000;
            if ((p.X + 1 < maxX) && (values[p.X + 1,p.Y] < val))
            {
                val = values[p.X + 1,p.Y];
                lowest = new Coordinate(p.X + 1, p.Y);
            }
            if ((p.X - 1 >= 0) && (values[p.X - 1,p.Y] < val))
            {
                val = values[p.X - 1,p.Y];
                lowest = new Coordinate(p.X - 1, p.Y);
            }
            if ((p.Y + 1 < maxY) && (values[p.X,p.Y + 1] < val))
            {
                val = values[p.X,p.Y + 1];
                lowest = new Coordinate(p.X, p.Y + 1);
            }
            if ((p.Y - 1 >= 0) && (values[p.X,p.Y - 1] < val))
            {
                val = values[p.X,p.Y - 1];
                lowest = new Coordinate(p.X, p.Y - 1);
            }
            return lowest;
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
