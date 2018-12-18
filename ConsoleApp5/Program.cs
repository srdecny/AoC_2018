using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ConsoleApp5
{
    class Program
    {

        static void Main(string[] args)
        {
            string input = @"C:\Users\Vojta\Documents\input.txt";
            Map map = new Map();
            map.LoadMap(input);
            map.BreadthFirstSearch();
        }

    }

    class Map
    {
        HashSet<Coords> Clay = new HashSet<Coords>();
        Dictionary<Coords, Coords> PreviousCoords = new Dictionary<Coords, Coords>();

        public void LoadMap(string input)
        {
            StreamReader file = new StreamReader(input);
            string line;
            char[] delimiters = ", ".ToCharArray();
            char[] coordinateDelimiters = "xy=.".ToCharArray();
            while (!file.EndOfStream)
            {
                line = file.ReadLine();
                var coordinates = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                IEnumerable<int> xRange;
                IEnumerable<int> yRange;
                List<int> theX;
                List<int> theY;

                if (coordinates[0][0] == 'x')
                {
                    theX = coordinates[0].Split(coordinateDelimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToList();
                    theY = coordinates[1].Split(coordinateDelimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToList();
                }
                else
                {
                    theX = coordinates[1].Split(coordinateDelimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToList();
                    theY = coordinates[0].Split(coordinateDelimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToList();
                }

                if (theX.Count == 1) xRange = Enumerable.Range(theX[0], 1);
                else xRange = Enumerable.Range(theX[0], Math.Abs(theX[1] - theX[0] + 1));

                if (theY.Count == 1) yRange = Enumerable.Range(theY[0], 1);
                else yRange = Enumerable.Range(theY[0], Math.Abs(theY[1] - theY[0] + 1));

                foreach (var x in xRange)
                {
                    foreach (var y in yRange)
                    {
                        Clay.Add(new Coords(x, y));
                    }
                }
            }
        }

        public void BreadthFirstSearch()
        {
            Stack<Coords> splittedWater = new Stack<Coords>();
            HashSet<int> waterLevels = new HashSet<int>();
            int waterCount = 1;
            int maxY = Clay.Max(x => x.Y);
            //int maxY = 100;
            Coords waterCell = new Coords(500, 0);
            splittedWater.Push(new Coords(500, 1));
            PreviousCoords.Add(new Coords(500, 1), new Coords(500 ,0));

            while (splittedWater.Any())
            {
                newWaterCell:
                waterCell = splittedWater.Pop();
                while (waterCell.Y <= maxY)
                {
                    // too high
                    if (splittedWater.Any())
                    {
                        if (waterCell.Y < splittedWater.Max(w => w.Y)) break;
                    }

                    if (waterCell.X == 497 && waterCell.Y == 1174)
                    {
                        break;
                    }

                    Coords down = new Coords(waterCell.X, waterCell.Y + 1);
                    Coords left = new Coords(waterCell.X - 1, waterCell.Y);
                    Coords right = new Coords(waterCell.X + 1, waterCell.Y);


                    bool canMoveDown = !(PreviousCoords.Keys.Contains(down)) && !(Clay.Contains(down));
                    bool canMoveLeft = !(PreviousCoords.Keys.Contains(left)) && !(Clay.Contains(left));
                    bool canMoveRight = !(PreviousCoords.Keys.Contains(right)) && !(Clay.Contains(right));

                    // cannot move down because there's water down
                    if (!canMoveDown && PreviousCoords.Keys.Contains(down) && canMoveLeft && canMoveRight)
                    {
                        var downWater = PreviousCoords[down];
                        if (waterCell.X != downWater.X || waterCell.Y != downWater.Y)
                        {
                            //PrintMap(waterCell.X, waterCell.Y);
                            break;
                        }
                    }

                    if (canMoveDown)
                    {
                     
                        PreviousCoords.Add(down, waterCell);
                        waterCell = down;
                        waterCount++;
                        if (down.Y + 1 > maxY) break;

                    }
                    else if (canMoveLeft && canMoveRight)
                    {
                        PreviousCoords.Add(right, waterCell);
                        PreviousCoords.Add(left, waterCell);
                        splittedWater.Push(right);
                        waterCell = left;
                        waterCount += 2;

                    }
                    else if (canMoveLeft)
                    {
                        

                        PreviousCoords.Add(left, waterCell);
                        waterCell = left;
                        waterCount++;

                        
                    }
                    else if (canMoveRight)
                    {

                        
                        PreviousCoords.Add(right, waterCell);
                        waterCell = right;
                        waterCount++;
                        
                    }
                    else
                    {
                        int scope = waterCell.X;
                        // check if we haven't already overflowed on the other side
                        if (Clay.Contains(left)) // moving right, see the left side
                        {
                            while (true)
                            {
                                if (Clay.Contains(new Coords(scope, waterCell.Y)))
                                {
                                    break;
                                }
                                else if (!PreviousCoords.ContainsKey(new Coords(scope, waterCell.Y)))
                                {
                                    goto newWaterCell;
                                }
                                scope++;

                            }
                        }
                        else // moving left, see the right side
                        {
                            while (true)
                            {
                                if (Clay.Contains(new Coords(scope, waterCell.Y)))
                                {
                                    break;
                                }
                                else if (!PreviousCoords.ContainsKey(new Coords(scope, waterCell.Y)))
                                {
                                    goto newWaterCell;
                                }
                                scope--;
                            }
                        }

                        waterCell = PreviousCoords[waterCell];
                    }

                }
            }
            PrintMap();
            Console.WriteLine(PreviousCoords.Keys.Distinct().Count());
            Console.ReadLine();
            // 1461857 high
            // 42385 high
            // 42378 not 
            // 41032 not
            // 41033 not
            // 41031 not
        }

        public void PrintMap(int highlightX = -1, int highlightY = -1)
        {
            for (int y = 0; y < 1700; y++)
            {
                Console.Write($"{y}: ");
                for (int x = 200; x < 680; x++)
                {
                    if (Clay.Contains(new Coords(x, y)))
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.Write("C");

                    }
                    else if (PreviousCoords.Keys.Contains(new Coords(x, y)))
                    {
                        if (x == highlightX && y == highlightY) Console.BackgroundColor = ConsoleColor.Yellow;
                        else Console.BackgroundColor = ConsoleColor.Blue;
                        Console.Write("W");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
            Console.WriteLine();

        }
    }

    public struct Coords
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
   
    

}

