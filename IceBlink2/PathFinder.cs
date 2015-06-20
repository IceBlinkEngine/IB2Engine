using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class PathFinder 
    {
	    public int[,] grid;
        public int[,] values = new int[7,7];
        public List<Coordinate> pathNodes = new List<Coordinate>();
        public bool foundEnd = false;
        public Module mod;
	
	    public PathFinder(Module m)
	    {
		    mod = m;
	    }
		
	    //called from outside to get next move location
        public Coordinate findNewPoint(Creature crt, Coordinate end)
        {
    	    foundEnd = false;
    	    Coordinate newPoint = new Coordinate(-1,-1);
            //set start value to 0
            values[crt.combatLocX,crt.combatLocY] = 0;                
            foreach (Creature cr in mod.currentEncounter.encounterCreatureList)
		    {
        	    if (cr != crt)
        	    grid[cr.combatLocX,cr.combatLocY] = 1;
		    }
		    foreach (Player p in mod.playerList)
		    {
			    if ((!p.charStatus.Equals("Dead")) && (p.hp > 0))
			    {
				    grid[p.combatLocX,p.combatLocY] = 1;
			    }
		    }
		    grid[crt.combatLocX,crt.combatLocY] = 2;
		    grid[end.X,end.Y] = 3;
            buildPath();
        
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
        }
    
        //called from outside to reset grid
        public void resetGrid()
        {
    	    grid = new int[7,7];
            //create the grid with 1s and 0s
    	    for (int col = 0; col < 7;col++)
    	    {
    		    for (int row = 0; row < 7; row++)
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
        
            //assign 9 to every value
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    values[x,y] = 99;
                }
            }
        }
    
        //helper functions
        public void buildPath()
        {
            //iterate through all values for next number and evaluate neighbors
            int next = 0;            
            for (int cnt = 0; cnt < 99; cnt++)
            {
                for (int x = 0; x < 7; x++)
                {
                    for (int y = 0; y < 7; y++)
                    {
                        if (values[x,y] == next)
                        {
                            if ((x + 1 < 7) && (evaluateValue(x + 1, y, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= 0) && (evaluateValue(x - 1, y, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y + 1 < 7) && (evaluateValue(x, y + 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y - 1 >= 0) && (evaluateValue(x, y - 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x + 1 < 7) && (y + 1 < 7) && (evaluateValue(x + 1, y + 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= 0) && (y - 1 >= 0) && (evaluateValue(x - 1, y - 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= 0) && (y + 1 < 7) && (evaluateValue(x - 1, y + 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x + 1 < 7) && (y - 1 >= 0) && (evaluateValue(x + 1, y - 1, next)))
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
    	    Coordinate lowest = new Coordinate();
            int val = 1000;
            if ((p.X + 1 < 7) && (values[p.X + 1,p.Y] < val))
            {
                val = values[p.X + 1,p.Y];
                lowest = new Coordinate(p.X + 1, p.Y);
            }
            if ((p.X - 1 >= 0) && (values[p.X - 1,p.Y] < val))
            {
                val = values[p.X - 1,p.Y];
                lowest = new Coordinate(p.X - 1, p.Y);
            }
            if ((p.Y + 1 < 7) && (values[p.X,p.Y + 1] < val))
            {
                val = values[p.X,p.Y + 1];
                lowest = new Coordinate(p.X, p.Y + 1);
            }
            if ((p.Y - 1 >= 0) && (values[p.X,p.Y - 1] < val))
            {
                val = values[p.X,p.Y - 1];
                lowest = new Coordinate(p.X, p.Y - 1);
            }
            if ((p.X + 1 < 7) && (p.Y + 1 < 7) && (values[p.X + 1,p.Y + 1] < val))
            {
                val = values[p.X + 1,p.Y + 1];
                lowest = new Coordinate(p.X + 1, p.Y + 1);
            }
            if ((p.X - 1 >= 0) && (p.Y - 1 >= 0) && (values[p.X - 1,p.Y - 1] < val))
            {
                val = values[p.X - 1,p.Y - 1];
                lowest = new Coordinate(p.X - 1, p.Y - 1);
            }
            if ((p.X - 1 >= 0) && (p.Y + 1 < 7) && (values[p.X - 1,p.Y + 1] < val))
            {
                val = values[p.X - 1,p.Y + 1];
                lowest = new Coordinate(p.X - 1, p.Y + 1);
            }
            if ((p.X + 1 < 7) && (p.Y - 1 >= 0) && (values[p.X + 1,p.Y - 1] < val))
            {
                val = values[p.X + 1,p.Y - 1];
                lowest = new Coordinate(p.X + 1, p.Y - 1);
            }
            return lowest;
        }
        public bool isWalkable(int col, int row)
	    {
    	    if (mod.currentEncounter.encounterTiles[col * 7 + row].Walkable)
            {
                return true;
            }            
            return false;		
	    }
    }
}
