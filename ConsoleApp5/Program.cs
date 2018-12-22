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
            Map map = new Map();
            map.CalculateGeologicIndex();
            map.CalculateErosionLevel();
            map.CalculateRiskLevel();
            Console.ReadLine();
            //804 low
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

    public class Map
    {
        static int Depth = 5616;
        static Coords Target = new Coords(10, 785);
        long[,] GeologicIndex = new long[Target.X + 1, Target.Y + 1];
        int[,] ErosionLevel = new int[Target.X + 1, Target.Y + 1];

        public void CalculateGeologicIndex()
        {
            for (int x = 0; x <= Target.X; x++)
            {
                for (int y = 0; y <= Target.Y; y++)
                {
                    if (x == 0 && y == 0) GeologicIndex[x, y] = 0;
                    else if (x == 0) GeologicIndex[x, y] = y * 48271;
                    else if (y == 0) GeologicIndex[x, y] = x * 16807;
                    else GeologicIndex[x, y] = GetErosionLevel(new Coords(x, y));
                }
            }
        }

        public void CalculateErosionLevel()
        {
            for (int x = 0; x <= Target.X; x++)
            {
                for (int y = 0; y <= Target.Y; y++)
                {
                    ErosionLevel[x, y] = (int) ((GeologicIndex[x, y] + Depth) % 20183);
                }
            }
        }

        public int GetErosionLevel(Coords coords)
        {
            int LeftErosionLevel = (int)((GeologicIndex[coords.X - 1, coords.Y] + Depth) % 20183);
            int RightErosionLevel = (int)((GeologicIndex[coords.X, coords.Y - 1] + Depth) % 20183);
            return LeftErosionLevel * RightErosionLevel;
        }
        public void CalculateRiskLevel()
        {
            int RiskLevel = 0;
            for (int x = 0; x <= Target.X; x++)
            {
                for (int y = 0; y <= Target.Y; y++)
                {
                    RiskLevel += ErosionLevel[x, y] % 3;
                }
            }
            Console.WriteLine(RiskLevel);

        }
    }


}