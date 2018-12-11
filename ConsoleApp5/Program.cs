using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApp5
{
    class Program
    {

        static void Main(string[] args)
        {
            (int, int) maxCoords = ValueTuple.Create(301, 301);
            int maxScore = Int32.MinValue;
            int[,] grid = new int[300, 300];
            object myLock = new object();
            
            Parallel.ForEach(Enumerable.Range(0, 300), x =>
            {
                Parallel.ForEach(Enumerable.Range(0, 300), y =>
                {
                    grid[x, y] = CalculatePowerLevel(x, y);
                });
            });

            Parallel.ForEach(Enumerable.Range(1, 298), x =>
            {
                Parallel.ForEach(Enumerable.Range(1, 298), y =>
                {
                    int score = CalculateNeighbours(x, y).Sum(neighbour => grid[neighbour.Item1, neighbour.Item2]);

                    if (score > maxScore)
                    {
                        lock (myLock)
                        {
                            if (score > maxScore)
                            {
                                maxCoords = ValueTuple.Create(x, y);
                                maxScore = score;
                            }
                        }
                    }
                });
            });

            Console.WriteLine(maxCoords); // the center
            Console.ReadLine();

        }

        private static int CalculatePowerLevel(int x, int y)
        {
            int serial = 8444;
            int powerLevel = 0;

            //Find the fuel cell's rack ID, which is its X coordinate plus 10.
            int id = x + 10 + 1;
            //Begin with a power level of the rack ID times the Y coordinate.
            powerLevel = id * (y + 1);
            //Increase the power level by the value of the grid serial number(your puzzle input).
            powerLevel += serial;
            //Set the power level to itself multiplied by the rack ID.
            powerLevel *= id;
            //Keep only the hundreds digit of the power level(so 12345 becomes 3; numbers with no hundreds digit become 0).
            powerLevel = powerLevel >= 100 ? powerLevel = powerLevel.ToString()[(powerLevel.ToString().Length - 3)] - 48 : powerLevel = 0;
            //Subtract 5 from the power level.
            return powerLevel - 5;

        }

        private static List<(int, int)> CalculateNeighbours(int coordsX, int coordsY)
        {
            List<(int, int)> neighbours = new List<(int, int)>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    neighbours.Add(ValueTuple.Create(x + coordsX, y + coordsY));
                }
            }
            return neighbours;
        }

    }




    

    

   








}

