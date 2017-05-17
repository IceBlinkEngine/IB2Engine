using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class PathFinderEncounters
    {
        public int[,] grid;
        public int[,] values;
        public List<Coordinate> pathNodes = new List<Coordinate>();
        public bool foundEnd = false;
        public Module mod;
        public GameView gv;

        public PathFinderEncounters(GameView g, Module m)
        {  
            mod = m;  
            gv = g;
        }


    //called from outside to get next move location
    public List<Coordinate> findNewPoint(Creature crt, Coordinate end)
        {
            pathNodes.Clear();
            foundEnd = false;
            Coordinate newPoint = new Coordinate(-1, -1);
            //set start value to 0
            values[crt.combatLocX, crt.combatLocY] = 0;
            foreach (Creature cr in mod.currentEncounter.encounterCreatureList)
            {
                if (cr != crt)
                {
                    //grid[cr.combatLocX, cr.combatLocY] = 1;
                    //block all squares that are made up by all creatures cr (and squares based on their size)  
                                        //also if crt is large, block squares around cr as needed                      
                     int crSize = cr.creatureSize; //1=normal, 2=wide, 3=tall, 4=large  
                     int crtSize = crt.creatureSize; //1=normal, 2=wide, 3=tall, 4=large  
                    
                    #region cr normal  
                    if (crSize == 1)
                    {
                         grid[cr.combatLocX, cr.combatLocY] = 1;
                                                //crt wide  
                                                if (crtSize == 2)
                                                    {
                                                        if (cr.combatLocX > 0)
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY] = 1;
                                                            }
                                                    }
                                                //crt tall  
                                                if (crtSize == 3)
                                                    {
                                                        if (cr.combatLocY > 0)
                                                            {
                                 grid[cr.combatLocX, cr.combatLocY - 1] = 1;
                                                            }
                                                    }
                                                //crt large  
                                                if (crtSize == 4)
                                                    {
                                                        if (cr.combatLocX > 0)
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY] = 1;
                                                            }
                                                        if (cr.combatLocY > 0)
                                                            {
                                 grid[cr.combatLocX, cr.combatLocY - 1] = 1;
                                                            }
                                                        if ((cr.combatLocX > 0) && (cr.combatLocY > 0))
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY - 1] = 1;
                                                            }
                                                    }
                                            }
                    
                    #endregion
 
                    #region cr wide  
                     else if (crSize == 2)
                                            {
                         grid[cr.combatLocX, cr.combatLocY] = 1;
                         grid[cr.combatLocX + 1, cr.combatLocY] = 1;
                                                //crt wide  
                                                if (crtSize == 2)
                                                    {
                                                        if (cr.combatLocX > 0)
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY] = 1;
                                                            }
                                                    }
                                                //crt tall  
                                                if (crtSize == 3)
                                                    {
                                                        if (cr.combatLocY > 0)
                                                            {
                                 grid[cr.combatLocX, cr.combatLocY - 1] = 1;
                                 grid[cr.combatLocX + 1, cr.combatLocY - 1] = 1;
                                                            }
                                                    }
                                                //crt large  
                                                if (crtSize == 4)
                                                    {
                                                        if (cr.combatLocX > 0)
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY] = 1;
                                                            }
                                                        if (cr.combatLocY > 0)
                                                            {
                                 grid[cr.combatLocX, cr.combatLocY - 1] = 1;
                                 grid[cr.combatLocX + 1, cr.combatLocY - 1] = 1;
                                                            }
                                                        if ((cr.combatLocX > 0) && (cr.combatLocY > 0))
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY - 1] = 1;
                                                            }
                                                    }
                                            }
                    
                    #endregion
 
                    #region cr tall  
                     else if (crSize == 3)
                                            {
                        grid[cr.combatLocX, cr.combatLocY] = 1;
                        grid[cr.combatLocX, cr.combatLocY + 1] = 1;
                                                //crt wide  
                                                if (crtSize == 2)
                                                    {
                                                        if (cr.combatLocX > 0)
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY] = 1;
                                 grid[cr.combatLocX - 1, cr.combatLocY + 1] = 1;
                                                            }
                                                    }
                                                //crt tall  
                                                if (crtSize == 3)
                                                    {
                                                        if (cr.combatLocY > 0)
                                                            {
                                 grid[cr.combatLocX, cr.combatLocY - 1] = 1;
                                                            }
                                                    }
                                                //crt large  
                                                if (crtSize == 4)
                                                    {
                                                        if (cr.combatLocX > 0)
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY] = 1;
                                 grid[cr.combatLocX - 1, cr.combatLocY + 1] = 1;
                                                            }
                                                        if (cr.combatLocY > 0)
                                                            {
                                 grid[cr.combatLocX, cr.combatLocY - 1] = 1;
                                                            }
                                                        if ((cr.combatLocX > 0) && (cr.combatLocY > 0))
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY - 1] = 1;
                                                            }
                                                    }
                                            }
                    
                    #endregion
 
                    #region cr large  
                     else if (crSize == 4)
                                            {
                         grid[cr.combatLocX, cr.combatLocY] = 1;
                         grid[cr.combatLocX + 1, cr.combatLocY] = 1;
                         grid[cr.combatLocX, cr.combatLocY + 1] = 1;
                         grid[cr.combatLocX + 1, cr.combatLocY + 1] = 1;
                                                //crt wide  
                                                if (crtSize == 2)
                                                    {
                                                        if (cr.combatLocX > 0)
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY] = 1;
                                 grid[cr.combatLocX - 1, cr.combatLocY + 1] = 1;
                                                            }
                                                    }
                                                //crt tall  
                                                if (crtSize == 3)
                                                    {
                                                        if (cr.combatLocY > 0)
                                                            {
                                 grid[cr.combatLocX, cr.combatLocY - 1] = 1;
                                 grid[cr.combatLocX + 1, cr.combatLocY - 1] = 1;
                                                            }
                                                    }
                                                //crt large  
                                                if (crtSize == 4)
                                                    {
                                                        if (cr.combatLocX > 0)
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY] = 1;
                                 grid[cr.combatLocX - 1, cr.combatLocY + 1] = 1;
                                                            }
                                                        if (cr.combatLocY > 0)
                                                            {
                                 grid[cr.combatLocX, cr.combatLocY - 1] = 1;
                                 grid[cr.combatLocX + 1, cr.combatLocY - 1] = 1;
                                                            }
                                                        if ((cr.combatLocX > 0) && (cr.combatLocY > 0))
                                                            {
                                 grid[cr.combatLocX - 1, cr.combatLocY - 1] = 1;
                                                            }
                                                   }
                                            }
                    
                    #endregion



                }
            }
            foreach (Player p in mod.playerList)
            {
                if (p.isAlive())
                {
                    grid[p.combatLocX, p.combatLocY] = 1;
                    /*int crt3Size = gv.cc.getCreatureSize(crt.cr_tokenFilename); //1=normal, 2=wide, 3=tall, 4=large  
212 +                    //crt wide  
213 +                    if (crt3Size == 2)  
214 +                    {  
215 +                        if (p.combatLocX > 0)  
216 +                        {  
217 +                            grid[p.combatLocX - 1, p.combatLocY] = 1;  
218 +                        }  
219 +                    }  
220 +                    //crt tall  
221 +                    if (crt3Size == 3)  
222 +                    {  
223 +                        if (p.combatLocY > 0)  
224 +                        {  
225 +                            grid[p.combatLocX, p.combatLocY - 1] = 1;  
226 +                        }  
227 +                    }  
228 +                    //crt large  
229 +                    if (crt3Size == 4)  
230 +                    {  
231 +                        if (p.combatLocX > 0)  
232 +                        {  
233 +                            grid[p.combatLocX - 1, p.combatLocY] = 1;  
234 +                        }  
235 +                        if (p.combatLocY > 0)  
236 +                        {  
237 +                            grid[p.combatLocX, p.combatLocY - 1] = 1;  
238 +                        }  
239 +                        if ((p.combatLocX > 0) && (p.combatLocY > 0))  
240 +                        {  
241 +                            grid[p.combatLocX - 1, p.combatLocY - 1] = 1;  
242 +                        }  
243 +                    }*/
                }
            }

            if ((mod.nonAllowedDiagonalSquareX != -1) && (mod.nonAllowedDiagonalSquareY != -1))
            {
                grid[mod.nonAllowedDiagonalSquareX, mod.nonAllowedDiagonalSquareY] = 1;
                mod.nonAllowedDiagonalSquareX = -1;
                mod.nonAllowedDiagonalSquareY = -1;
            }

            //find all props that have collision and set there square to 1
            /*TODO add props to encounters
            foreach (Prop prp in mod.currentEncounter.Props)
            {
        	    if  ( ((prp.HasCollision) && (prp.isActive)) || ((prp.isMover) && (prp.isActive)) )
        	    {
        		    grid[prp.LocationX,prp.LocationY] = 1;
        	    }
            }*/
            grid[crt.combatLocX, crt.combatLocY] = 2; //2 marks the start point in the grid
                                                      //grid[end.X, end.Y] = 3; //3 marks the end point in the grid

            //end point for larger creatures should be more squares around PC  

            if (grid[end.X, end.Y] != 0)
            {
                //ending point is a wall or PC or creature square...skip this square target  
                //return pathNodes;
            }

            grid[end.X, end.Y] = 3; //3 marks the end point in the grid  

            buildPath();

            if (!foundEnd)
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
        }
        //called from outside to reset grid
        //public void resetGrid()
        public void resetGrid(Creature crt)
        {
            grid = new int[mod.currentEncounter.MapSizeX, mod.currentEncounter.MapSizeY];
            values = new int[mod.currentEncounter.MapSizeX, mod.currentEncounter.MapSizeY];
            //create the grid with 1s and 0s
            for (int col = 0; col < mod.currentEncounter.MapSizeY; col++)
    	    {
                for (int row = 0; row < mod.currentEncounter.MapSizeX; row++)
    		    {
    			    if (isWalkable(row,col))
    			    {
    				    grid[row,col] = 0;
    			    }
    			    else
    			    {
                        //grid[row,col] = 1;
                        //define here for large creatures the squares that are not walkable because of their size, not just walls but surrounding squares  
                        grid[row, col] = 1;
                                                //1=normal, 2=wide, 3=tall, 4=large  
                         int crtSize = crt.creatureSize;
                                                //wide  
                                                if (crtSize == 2)
                                                    {
                                                        if (row > 0)
                                                            {
                                 grid[row - 1, col] = 1;
                                                            }
                                                    }
                                                //tall  
                                                if (crtSize == 3)
                                                    {
                                                        if (col > 0)
                                                            {
                                 grid[row, col - 1] = 1;
                                                            }
                                                    }
                                                //large  
                                                if (crtSize == 4)
                                                    {
                                                        if (row > 0)
                                                            {
                                 grid[row - 1, col] = 1;
                                                            }
                                                        if (col > 0)
                                                            {
                                 grid[row, col - 1] = 1;
                                                            }
                                                        if ((row > 0) && (col > 0))
                                                            {
                                 grid[row - 1, col - 1] = 1;
                                                            }
                                                    }
                    }
                }
    	    }
        
            //assign 9999 to every value
            for (int x = 0; x < mod.currentEncounter.MapSizeX; x++)
            {
                for (int y = 0; y < mod.currentEncounter.MapSizeY; y++)
                {
                    values[x,y] = 9999;
                }
            }
        }
        public void buildPath()
        {
            //iterate through all values for next number and evaluate neighbors
            int next = 0;
            int maxCnt = mod.currentEncounter.MapSizeX * mod.currentEncounter.MapSizeY * 4;
            for (int cnt = 0; cnt < maxCnt; cnt++)
            {
                for (int x = 0; x < mod.currentEncounter.MapSizeX; x++)
                {
                    for (int y = 0; y < mod.currentEncounter.MapSizeY; y++)
                    {
                        if (values[x, y] == next)
                        {
                            if ((x + 1 < mod.currentEncounter.MapSizeX) && (evaluateValue(x + 1, y, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= 0) && (evaluateValue(x - 1, y, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y + 1 < mod.currentEncounter.MapSizeY) && (evaluateValue(x, y + 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y - 1 >= 0) && (evaluateValue(x, y - 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x + 1 < mod.currentEncounter.MapSizeX) && (y + 1 < mod.currentEncounter.MapSizeY) && (evaluateValue(x + 1, y + 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= 0) && (y - 1 >= 0) && (evaluateValue(x - 1, y - 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= 0) && (y + 1 < mod.currentEncounter.MapSizeY) && (evaluateValue(x - 1, y + 1, next)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x + 1 < mod.currentEncounter.MapSizeX) && (y - 1 >= 0) && (evaluateValue(x + 1, y - 1, next)))
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
        }
        public bool isWalkable(int col, int row)
        {
            if (mod.currentEncounter.encounterTiles[col * mod.currentEncounter.MapSizeX + row].Walkable)
            {
                return true;
            }
            return false;
        }
    }
}
