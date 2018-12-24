using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using MathNet.Spatial.Euclidean;

namespace ConsoleApp5
{
    class Program
    {

        static void Main(string[] args)
        {
            string input = @"C:\Users\srdecny\Documents\input.txt";
            Map map = new Map();
            map.ParseInput(input);
            map.CalculateSphereIntersections();
            map.CalculateHighestDensityPoint();

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

    public struct Sphere
    {
        public Coords Coordinates { get; }
        public long Range { get; }
        
        public Sphere(Coords coords, long range)
        {
            Coordinates = coords;
            Range = range;
        }
    }

    class Map
    {
        private Dictionary<Sphere, int> Intersections = new Dictionary<Sphere, int>();
        private List<Sphere> Spheres = new List<Sphere>();
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
                Spheres.Add(new Sphere(new Coords(positionX[0], positionY[0], positionZ[0]), range[0]));
            }
        }

        public void CalculateSphereIntersections()
        {

            foreach (var sphere in Spheres)
            {
                int sphereAmount = 0;
                foreach (var otherSphere in Spheres)
                {
                    if (CalculateManhattanDistance(sphere.Coordinates, otherSphere.Coordinates) <= sphere.Range) sphereAmount++;
                }
                Intersections[sphere] = sphereAmount;
            }
        }

        public void CalculateHighestDensityPoint()
        {
            List<Vector3D> intersectionVectors = new List<Vector3D>();
            Sphere highestIntersectionSphere = Intersections.OrderByDescending(x => x.Value).First().Key;
            foreach (var otherSphere in Spheres)
            {
                if (highestIntersectionSphere.Range == otherSphere.Range && highestIntersectionSphere.Coordinates.X == otherSphere.Coordinates.X) continue;
                var potentialIntersection =  CalculateVector(highestIntersectionSphere, otherSphere);
                if (potentialIntersection.HasValue) intersectionVectors.Add(potentialIntersection.Value);
            }

            var bestCoords = HillClimbingAlgorithm(highestIntersectionSphere.Coordinates);
            Console.ReadLine();
            

        }

       public Vector3D? CalculateVector(Sphere first, Sphere second)
        {
            long distance = CalculateManhattanDistance(first.Coordinates, second.Coordinates);

            if (distance <= first.Range + second.Range)
            {
                return new Vector3D(second.Coordinates.X - first.Coordinates.X,
                                    second.Coordinates.Y - first.Coordinates.Y,
                                    second.Coordinates.Z - first.Coordinates.Z);
            }
            return null;
        }
       

        private long CalculateManhattanDistance(Coords first, Coords second)
        {
            long distance = 0;
            distance += Math.Abs(first.X - second.X);
            distance += Math.Abs(first.Y - second.Y);
            distance += Math.Abs(first.Z - second.Z);
            return distance;
        }

        private struct Circle
        {
            public Coords Coordinates { get; }
            public long Radius { get; }
            public Circle(Coords coords, long radius)
            {
                Coordinates = coords;
                Radius = radius;
            }
        }

        private List<Coords> GetNeighbours(Coords start)
        {
            int range = 10000;
            List<Coords> neighbours = new List<Coords>();
            for (long x = start.X - range; x < start.X + range; x++)
            {
                for (long y = start.Y - range; y < start.Y + range; y++)
                {
                    for (long z = start.Z - range; z < start.Z + range; z++)
                    {
                        neighbours.Add(new Coords(start.X + x, start.Y + y, start.Z + z));
                    }

                }

            }
            return neighbours;
        }

        private Coords HillClimbingAlgorithm(Coords start)
        {
            Coords currentCoords = start;
            int currentEval = -1;
            bool foundBetterScore = false;
            object myLock = new object();

            while (true)
            {

                Parallel.ForEach(GetNeighbours(currentCoords), (neighbour) =>
                {
                    if (GetScoreOfCoords(neighbour) > currentEval)
                    {
                        lock (myLock)
                        {
                            if (GetScoreOfCoords(neighbour) > currentEval) {
                                currentEval = GetScoreOfCoords(neighbour);
                                currentCoords = neighbour;
                                foundBetterScore = true;
                                Console.WriteLine($"Current score: {currentEval}");
                            }
                        }
                    }
                });


                if (!foundBetterScore)
                {
                    Console.WriteLine($"{CalculateManhattanDistance(currentCoords, new Coords(0, 0, 0))}");
                    return currentCoords;
                }
                foundBetterScore = false;
            }
        }

        private int GetScoreOfCoords(Coords coords)
        {
            int score = 0;
            foreach (var sphere in Spheres)
            {
                if (sphere.Range >= CalculateManhattanDistance(coords, sphere.Coordinates)) score++;
            }
            return score;
        }
    }


}