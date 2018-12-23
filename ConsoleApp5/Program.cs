using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Priority_Queue;

namespace ConsoleApp5
{
    class Program
    {

        static void Main(string[] args)
        {
            string input = @"C:\Users\Vojta\Documents\input.txt";
            Map map = new Map();
            map.ParseInput(input);
            map.FindStrongestNanobot();

            Console.WriteLine("Finished...");
            Console.ReadLine();

            // 949 high
        }

    }

    public struct Coords
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }
        public Coords(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }  

    public struct Nanobot
    {
        public Coords Coordinates { get; }
        public long Range { get; }
        
        public Nanobot(Coords coords, long range)
        {
            Coordinates = coords;
            Range = range;
        }
    }

    class Map
    {
        private List<Nanobot> Nanobots = new List<Nanobot>();
        public void ParseInput(string input)
        {
            StreamReader reader = new StreamReader(input);
            var positionDelimiters = "pos=<,>".ToCharArray();
            var rangeDelimiters = " r=".ToCharArray();

            while (!reader.EndOfStream)
            {
                var words = reader.ReadLine().Split(',');
                var positionX = words[0].Split(positionDelimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int64.Parse(x)).ToArray();
                var positionY = words[1].Split(positionDelimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int64.Parse(x)).ToArray();
                var positionZ = words[2].Split(positionDelimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int64.Parse(x)).ToArray();
                var range = words[3].Split(rangeDelimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int64.Parse(x)).ToArray();
                Nanobots.Add(new Nanobot(new Coords(positionX[0], positionY[0], positionZ[0]), range[0]));
            }
        }

        public void FindStrongestNanobot()
        {
            int maxNanobotAmount = Int32.MinValue;

            int nanobotAmount = 0;
            Nanobot strongestNanobot = Nanobots.OrderByDescending(x => x.Range).First();
            foreach (var otherNanobot in Nanobots)
            {
                if (CalculateManhattanDistance(strongestNanobot.Coordinates, otherNanobot.Coordinates) <= strongestNanobot.Range) nanobotAmount++;
            }
            if (nanobotAmount > maxNanobotAmount) maxNanobotAmount = nanobotAmount;
            Console.WriteLine(maxNanobotAmount);
        }

        private long CalculateManhattanDistance(Coords first, Coords second)
        {
            long distance = 0;
            distance += Math.Abs(first.X - second.X);
            distance += Math.Abs(first.Y - second.Y);
            distance += Math.Abs(first.Z - second.Z);
            return distance;
        }
    }


}