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
            long[,] grid = new long[300, 300];
            long[,] integralImage = new long[300, 300];

            long maxScore = long.MinValue;
            (long, long) maxCoords = ValueTuple.Create(301, 301);
            long maxSize = -1;
            object myLock = new object();
            
            Parallel.ForEach(Enumerable.Range(0, 300), x =>
            {
                Parallel.ForEach(Enumerable.Range(0, 300), y =>
                {
                    grid[x, y] = CalculatePowerLevel(x, y);
                });
            });

            for (long x = 0; x < 300; x++)
            {
                for (long y = 0; y < 300; y++)
                {
                    integralImage[x, y] = grid[x, y];
                    if (x - 1 >= 0) integralImage[x, y] += integralImage[x - 1, y];
                    if (y - 1 >= 0) integralImage[x, y] += integralImage[x, y - 1];
                    if (x - 1 >= 0 && y - 1 >= 0) integralImage[x, y] -= integralImage[x - 1, y - 1];
                   
                }
            }

            Parallel.ForEach(Enumerable.Range(0, 300), x =>
            {
                Parallel.ForEach(Enumerable.Range(0, 300), y =>
                {
                    Parallel.ForEach(CalculateSquareSizes(x, y), size =>
                    {
                        long score = integralImage[x + size, y + size];
                        if (x - 1 >= 0 && y - 1 >= 0) score += integralImage[x - 1, y - 1];
                        if (x - 1 >= 0) score -= integralImage[x - 1, y + size];
                        if (y - 1 >= 0) score -= integralImage[x + size, y - 1];

                        if (x == 242 && y == 67)
                        {
                            ;
                        }

                        if (score > maxScore)
                        {
                            lock (myLock)
                            {
                                if (score > maxScore)
                                {
                                    maxScore = score;
                                    maxCoords = ValueTuple.Create(x, y);
                                    maxSize = size;

                                    
                                }
                            }
                        }

                    });
                });
            });

            Console.WriteLine(maxCoords.Item1 + 1); 
            Console.WriteLine(maxCoords.Item2 + 1); 
            Console.WriteLine(maxScore);
            Console.WriteLine(maxSize + 1);
            Console.ReadLine();

        }

        private static long CalculatePowerLevel(long x, long y)
        {
            long serial = 18;
            long powerLevel = 0;

            //Find the fuel cell's rack ID, which is its X coordinate plus 10.
            long id = x + 10 + 1;
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


        // possible square sizes, starting from the upper left corner
        private static List<long> CalculateSquareSizes(long x, long y)
        {
            List<long> sizes = new List<long>() { 0 };
            for (long size = 1; size <= 300; size++)
            {
                if (x + size < 300 && y + size < 300) sizes.Add(size);
                else break;
            }
            return sizes;
        }

    }
    
}

