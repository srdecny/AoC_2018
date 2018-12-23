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
            Dictionary<Coords, int> intersectionPoints = new Dictionary<Coords, int>();
            List<Sphere> intersectionSpheres = new List<Sphere>();
            Sphere highestIntersectionSphere = Intersections.OrderByDescending(x => x.Value).First().Key;
            foreach (var otherSphere in Spheres)
            {
                if (highestIntersectionSphere.Range == otherSphere.Range && highestIntersectionSphere.Coordinates.X == otherSphere.Coordinates.X) continue;
                var potentialIntersection = CalculateSphereIntersection(highestIntersectionSphere, otherSphere);
                if (potentialIntersection.HasValue) intersectionSpheres.Add(potentialIntersection.Value);
            }

            foreach (var sphere in intersectionSpheres)
            {
                for (long x = sphere.Coordinates.X - sphere.Range; x < sphere.Coordinates.X + sphere.Range; x++)
                {
                    for (long y = sphere.Coordinates.Y - sphere.Range; y < sphere.Coordinates.Y + sphere.Range; y++)
                    {
                        for (long z = sphere.Coordinates.Z - sphere.Range; z < sphere.Coordinates.Z + sphere.Range; z++)
                        {
                            int intersection = 0;
                            var coords = new Coords(x, y, z);
                            if (intersectionPoints.ContainsKey(coords)) continue;
                            foreach (var anotherSphere in Spheres)
                            {
                                if (CalculateManhattanDistance(coords, anotherSphere.Coordinates) <= sphere.Range) intersection++;

                            }
                            intersectionPoints[coords] = intersection;
                        }

                    }

                }
            }

            Console.WriteLine(intersectionPoints.OrderByDescending(x => x.Value).First().Key);
        }

       public Sphere? CalculateSphereIntersection(Sphere first, Sphere second)
        {
            long distance = CalculateManhattanDistance(first.Coordinates, second.Coordinates);
            if (first.Range + second.Range < distance) return null;




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
    }


}