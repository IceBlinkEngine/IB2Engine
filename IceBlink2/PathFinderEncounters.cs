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

            //need to add choke point code that makes narrow points impssable for wide, tall and large cretaures
            //alos add code for blocking vmoing to close t map borders
            if (crt.creatureSize != 1)
            {
                int bound0 = grid.GetUpperBound(0);
                int bound1 = grid.GetUpperBound(1);

                for (int x = 0;x <= bound0; x++)
                {
                    for (int y = 0; y <= bound1; y++)
                    {
                        //found a wall, now block additional squares for our oersized mover
                        if (grid[x,y] == 1)
                        {
                            //wide (x+1)
                            if (crt.creatureSize == 2)
                            {
                                //cannot stand left of blocked square
                                if (x > 0)
                                {
                                    grid[x - 1, y] = 1;
                                }
                            }

                            //tall (y+1)
                            if (crt.creatureSize == 3)
                            {
                                //cannot stand high of blocked square
                                if (y > 0)
                                {
                                    grid[x, y-1] = 1;
                                }

                            }


                            //large (x+1, y+1)
                            if (crt.creatureSize == 4)
                            {
                                //cannot stand left of blocked square
                                if (x > 0)
                                {
                                    grid[x - 1, y] = 1;
                                }

                                //cannot stand high of blocked square
                                if (y > 0)
                                {
                                    grid[x, y - 1] = 1;
                                }

                                //cannot stand diagonally left and high
                                if ((x > 0) && (y > 0))
                                {
                                    grid[x - 1, y - 1] = 1;
                                }
                            }
                        }
                    }
                } 
            }

            foreach (Creature cr in mod.currentEncounter.encounterCreatureList)
            {

                if ((cr != crt) && (cr.hp > 0))
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
                        if (cr.combatLocX < mod.currentEncounter.MapSizeX - 1)
                        {
                            grid[cr.combatLocX + 1, cr.combatLocY] = 1;
                        }
                        
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
                        if (cr.combatLocY < mod.currentEncounter.MapSizeY - 1)
                        {
                            grid[cr.combatLocX, cr.combatLocY + 1] = 1;
                        }
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

            buildPath(crt);

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
                    if (isWalkable(row, col))
                    {
                        grid[row, col] = 0;
                    }
                    else
                    {
                        //grid[row,col] = 1;
                        //define here for large creatures the squares that are not walkable because of their size, not just walls but surrounding squares  
                        grid[row, col] = 1;
                    }
                }
            }

            /*
            foreach (Creature c in mod.currentEncounter.encounterCreatureList)
            {
                if (c.hp > 0)
                {
                    grid[c.combatLocX, c.combatLocY] = 1;

                    //1=normal, 2=wide, 3=tall, 4=large  
                    int crtSize = c.creatureSize;
                    //wide  
                    if (crtSize == 2)
                    {
                        if (c.combatLocX > 0)
                        {
                            grid[c.combatLocX - 1, c.combatLocY] = 1;
                        }
                    }
                    //tall  
                    if (crtSize == 3)
                    {
                        if (c.combatLocY > 0)
                        {
                            grid[c.combatLocX, c.combatLocY - 1] = 1;
                        }
                    }
                    //large  
                    if (crtSize == 4)
                    {
                        if (c.combatLocX > 0)
                        {
                            grid[c.combatLocX - 1, c.combatLocY] = 1;
                        }
                        if (c.combatLocY > 0)
                        {
                            grid[c.combatLocX, c.combatLocY - 1] = 1;
                        }
                        if ((c.combatLocX > 0) && (c.combatLocY > 0))
                        {
                            grid[c.combatLocX - 1, c.combatLocY - 1] = 1;
                        }
                    }
                }
            }
            */

            /*
            foreach (Player p in mod.playerList)
            {
                if (p.hp > 0)
                {
                    grid[p.combatLocX, p.combatLocY] = 1;
                }
            }
            */

            //assign 9999 to every value
            for (int x = 0; x < mod.currentEncounter.MapSizeX; x++)
            {
                for (int y = 0; y < mod.currentEncounter.MapSizeY; y++)
                {
                    values[x,y] = 9999;
                }
            }
        }
        public void buildPath(Creature crt)
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

                            if ((x + 1 < mod.currentEncounter.MapSizeX) && (evaluateValue(x + 1, y, next, crt)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= 0) && (evaluateValue(x - 1, y, next, crt)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y + 1 < mod.currentEncounter.MapSizeY) && (evaluateValue(x, y + 1, next, crt)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((y - 1 >= 0) && (evaluateValue(x, y - 1, next, crt)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x + 1 < mod.currentEncounter.MapSizeX) && (y + 1 < mod.currentEncounter.MapSizeY) && (evaluateValue(x + 1, y + 1, next, crt)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= 0) && (y - 1 >= 0) && (evaluateValue(x - 1, y - 1, next, crt)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x - 1 >= 0) && (y + 1 < mod.currentEncounter.MapSizeY) && (evaluateValue(x - 1, y + 1, next, crt)))
                            {
                                foundEnd = true;
                                return;
                            }
                            if ((x + 1 < mod.currentEncounter.MapSizeX) && (y - 1 >= 0) && (evaluateValue(x + 1, y - 1, next, crt)))
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

        public bool containsPCorCrt (int x, int y)
        {
            foreach (Player p in gv.mod.playerList)
            {
                if (p.isAlive())
                {
                    if ((p.combatLocX == x) && (p.combatLocY == y))
                    {
                        return true;
                    }
                }
            }

            foreach (Creature c in gv.mod.currentEncounter.encounterCreatureList)
            {
                if (c.hp > 0)
                {
                    if (c.combatLocX == x && c.combatLocY == y)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool evaluateValue(int x, int y, int next, Creature crt)
        {


            ///resetGrid(crt);
            if (grid[6, 2] == 3)
            {
                if (x== 5 && y == 3)
                {

                }

                if (next == 3)
                {
                    int i = 0;
                }
            }
            //evaluate each surrounding node and replace if greater than next number + 1
            //check for end            
            if (grid[x,y] == 3)
            {
                values[x,y] = next + 1;
                return true; //found end
            }

            //wide, tall and large creature can reach the end of path a square sooner direction dependent)
            //wide Creature
            if (crt.creatureSize == 2)
            {
                if (x + 1 < gv.mod.currentEncounter.MapSizeX)
                {
                    if (grid[x + 1, y] == 3)
                    {
                        //if (grid[x, y] == 0)
                        if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x,y))
                        {
                            values[x, y] = next + 1;
                            values[x+1, y] = next + 2;
                            return true; //found end
                        }
                    }
                }

                if (x + 1 < gv.mod.currentEncounter.MapSizeX && y + 1 < gv.mod.currentEncounter.MapSizeY)
                {
                    if (grid[x + 1, y + 1] == 3)
                    {
                        if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        {
                            values[x, y] = next + 1;
                            values[x+1, y+1] = next + 2;
                            return true; //found end
                        }
                    }
                }

                if (x + 1 < gv.mod.currentEncounter.MapSizeX && y - 1 >= 0)
                {
                    if (grid[x + 1, y - 1] == 3)
                    {
                        if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        {
                            values[x, y] = next + 1;
                            values[x+1, y-1] = next + 2;
                            return true; //found end
                        }
                    }
                }
            }

            //tall Creature
            if (crt.creatureSize == 3)
            {
                if (y + 1 < gv.mod.currentEncounter.MapSizeY)
                {
                    if (grid[x, y + 1] == 3)
                    {
                        if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        {
                            values[x, y] = next + 1;
                            values[x, y+1] = next + 2;
                            return true; //found end
                        }
                    }
                }

                if (x + 1 < gv.mod.currentEncounter.MapSizeX && y + 1 < gv.mod.currentEncounter.MapSizeY)
                {
                    if (grid[x + 1, y + 1] == 3)
                    {
                        if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        {
                            values[x, y] = next + 1;
                            values[x+1, y+1] = next + 2;
                            return true; //found end
                        }
                    }
                }

                if (x -1 >= 0 && y + 1 < gv.mod.currentEncounter.MapSizeY)
                {
                    if (grid[x - 1, y + 1] == 3)
                    {
                        if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        {
                            values[x, y] = next + 1;
                            values[x-1, y+1] = next + 2;
                            return true; //found end
                        }
                    }
                }
            }

            //large creature
            if (crt.creatureSize == 4)
            {
                //keep in mind the 4th squre! (even more reach)
                //checking the wide aspect wile considering the extra size in vertical
                //same height, 9, +y,-x
                if (x + 1 < gv.mod.currentEncounter.MapSizeX && y + 1 < gv.mod.currentEncounter.MapSizeY && x - 1 >= 0)
                {
                    if (grid[x + 1, y] == 3)
                    {

                        //if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        if ((gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y)) && (gv.mod.currentEncounter.encounterTiles[(y+1) * gv.mod.currentEncounter.MapSizeX + x-1].Walkable && !containsPCorCrt(x-1, y+1)) && (gv.mod.currentEncounter.encounterTiles[(y+1) * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y+1)))
                        //if (grid[x, y] == 0 && grid[x + 1, y + 1] == 0 && grid[x, y + 1] == 0)
                        {
                            values[x, y] = next + 1;
                            values[x+1, y] = next + 2;
                            return true; //found end
                        }
                    }
                }
                //TODO: catch limit exceptions from here on....
                //one heigher 6 +y,-x
                if (x + 1 < gv.mod.currentEncounter.MapSizeX && y + 1 < gv.mod.currentEncounter.MapSizeY && x - 1 >= 0)
                {
                    if (grid[x + 1, y + 1] == 3)
                    {
                        //if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        if ((gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y)) && (gv.mod.currentEncounter.encounterTiles[(y + 1) * gv.mod.currentEncounter.MapSizeX + x - 1].Walkable && !containsPCorCrt(x - 1, y + 1)) && (gv.mod.currentEncounter.encounterTiles[(y + 1) * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y + 1)))
                        //if (grid[x, y] == 0 && grid[x + 1, y + 1] == 0 && grid[x, y + 1] == 0)
                        {
                            values[x, y] = next + 1;
                            values[x+1, y+1] = next + 2;
                            return true; //found end
                        }
                    }
                }
                //two heigher 3 +y/-x
                if (x + 1 < gv.mod.currentEncounter.MapSizeX && y + 2 < gv.mod.currentEncounter.MapSizeY && x - 1 >= 0)
                {
                    if (grid[x + 1, y + 2] == 3)
                    {
                        //if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        if ((gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y)) && (gv.mod.currentEncounter.encounterTiles[(y + 1) * gv.mod.currentEncounter.MapSizeX + x - 1].Walkable && !containsPCorCrt(x - 1, y + 1)) && (gv.mod.currentEncounter.encounterTiles[(y + 1) * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y + 1)))
                        //if (grid[x, y] == 0 && grid[x + 1, y + 1] == 0 && grid[x, y + 1] == 0)
                        {
                            values[x, y] = next + 1;
                            values[x, y+1] = next + 2;
                            values[x+1, y+2] = next + 3;
                            return true; //found end
                        }
                    }
                }

                /*
                //one lower
                if (x + 1 < gv.mod.currentEncounter.MapSizeX && y + 1 < gv.mod.currentEncounter.MapSizeY && y - 1 >= 0 && x - 1 >= 0)
                {
                    if (grid[x + 1, y - 1] == 3)
                    {
                        //if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        if ((gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y)) && (gv.mod.currentEncounter.encounterTiles[(y + 1) * gv.mod.currentEncounter.MapSizeX + x - 1].Walkable && !containsPCorCrt(x - 1, y + 1)) && (gv.mod.currentEncounter.encounterTiles[(y + 1) * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y + 1)))
                        //if (grid[x, y] == 0 && grid[x + 1, y + 1] == 0 && grid[x, y + 1] == 0)
                        {
                            values[x, y] = next + 1;
                            values[x+1, y-1] = next + 2;
                            return true; //found end
                        }
                    }
                }
                */


                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                //same height (1)+x/-y
                if (y + 1 < gv.mod.currentEncounter.MapSizeY && x - 1 >= 0 && y - 1 >= 0 && x + 1 < gv.mod.currentEncounter.MapSizeX)
                {
                    if (grid[x, y + 1] == 3)
                    {
                        //if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        if ((gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y)) && (gv.mod.currentEncounter.encounterTiles[(y) * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x + 1, y)) && (gv.mod.currentEncounter.encounterTiles[(y - 1) * gv.mod.currentEncounter.MapSizeX + x + 1].Walkable && !containsPCorCrt(x + 1, y - 1)))
                        //if (grid[x, y] == 0 && grid[x + 1, y + 1] == 0 && grid[x, y + 1] == 0)
                        {
                            values[x, y] = next + 1;
                            values[x, y+1] = next + 2;
                            return true; //found end
                        }
                    }
                }
                //one heigher(2) +x/-y
                if (x + 1 < gv.mod.currentEncounter.MapSizeX && y + 1 < gv.mod.currentEncounter.MapSizeY && x - 1 >= 0 && y - 1 >= 0)
                {
                    if (grid[x + 1, y + 1] == 3)
                    {
                        //if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        if ((gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y)) && (gv.mod.currentEncounter.encounterTiles[(y) * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x + 1, y)) && (gv.mod.currentEncounter.encounterTiles[(y - 1) * gv.mod.currentEncounter.MapSizeX + x + 1].Walkable && !containsPCorCrt(x + 1, y - 1)))
                        //if (grid[x, y] == 0 && grid[x + 1, y + 1] == 0 && grid[x, y + 1] == 0)
                        {
                            values[x, y] = next + 1;
                            values[x+1, y+1] = next + 2;
                            return true; //found end
                        }
                    }
                }
                
                //(x), +y/-x
                if (x + 1 < gv.mod.currentEncounter.MapSizeX && y + 1 < gv.mod.currentEncounter.MapSizeY && y - 1 >= 0 && x - 1 >= 0)
                {
                    if (grid[x + 1, y - 1] == 3)
                    {
                        //if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        if ((gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y)) && (gv.mod.currentEncounter.encounterTiles[(y + 1) * gv.mod.currentEncounter.MapSizeX + x - 1].Walkable && !containsPCorCrt(x - 1, y + 1)) && (gv.mod.currentEncounter.encounterTiles[(y + 1) * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y + 1)))
                        //if (grid[x, y] == 0 && grid[x + 1, y + 1] == 0 && grid[x, y + 1] == 0)
                        {
                            values[x, y] = next + 1;
                            values[x+1, y-1] = next + 2;
                            return true; //found end
                        }
                    }
                }
                
                //one lower (shift) +x/-y
                if (y + 1 < gv.mod.currentEncounter.MapSizeY && x - 1 >= 0 && y - 1 >= 0 && x + 1 < gv.mod.currentEncounter.MapSizeX)
                {
                    if (grid[x - 1, y + 1] == 3)
                    {
                        //if (grid[x, y] == 0 && grid[x + 1, y + 1] == 0 && grid[x, y + 1] == 0)
                        //if (gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y))
                        if ((gv.mod.currentEncounter.encounterTiles[y * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x, y)) && (gv.mod.currentEncounter.encounterTiles[(y) * gv.mod.currentEncounter.MapSizeX + x].Walkable && !containsPCorCrt(x +1, y)) && (gv.mod.currentEncounter.encounterTiles[(y - 1) * gv.mod.currentEncounter.MapSizeX + x + 1].Walkable && !containsPCorCrt(x + 1, y - 1)))
                        {
                            values[x, y] = next + 1;
                            values[x-1, y+1] = next + 2;
                            return true; //found end
                        }
                    }
                }

                //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

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
        public bool isWalkable(int row, int col)
        {
            if (mod.currentEncounter.encounterTiles[col * mod.currentEncounter.MapSizeX + row].Walkable)
            {
                return true;
            }
            return false;
        }
    }
}
