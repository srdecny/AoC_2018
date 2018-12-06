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
        // Ugly as sin.
        static void Main(string[] args)
        {
            List<Point> points = new List<Point>();
            int maxDistance = 10000;
            int validPoints = 0;

            var file = new StreamReader(File.Open(@"C:\Users\srdecny\Documents\input.txt", FileMode.Open));
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                string[] coords = line.Split(new string[] { ", " }, StringSplitOptions.None);
                points.Add(new Point(Int32.Parse(coords[0]), Int32.Parse(coords[1])));
            }

            var boxedPoints = Point.CalculateBoxedPoints(points);
            for (int x = boxedPoints[0].Coordinates.Item1; x < boxedPoints[1].Coordinates.Item1; x++)
            {
                for (int y = boxedPoints[0].Coordinates.Item2; y < boxedPoints[1].Coordinates.Item2; y++)
                {
                    var newPoint = new Point(x, y);
                    int distance = 0;
                    foreach (var point in points)
                    {
                        distance += Point.CalculateAbsDistance(newPoint, point);
                    }
                    if (distance < maxDistance) validPoints++;
                }
            }

            Console.WriteLine(validPoints);
            Console.ReadLine();
            

        }

    }

    public class Point
    {
        public int Id { get; set; }
        public (int, int) Coordinates { get; }
        public bool stoppedSpreading { get; set; } = false;
        public int totalFields { get; set; } = 0;

        public Point(int x, int y)
        {
            Coordinates = ValueTuple.Create(x, y);
        }

        public static List<Point> CalculateBoxedPoints(List<Point> points)
        {
            List<Point> boxedPoints = new List<Point>();
            int minX = points.Min(x => x.Coordinates.Item1);
            int minY = points.Min(x => x.Coordinates.Item2);
            int maxX = points.Max(x => x.Coordinates.Item1);
            int maxY = points.Max(x => x.Coordinates.Item2);

            boxedPoints.Add(new Point(minX, minY));
            boxedPoints.Add(new Point(maxX, maxY));

            return boxedPoints;
        }

        public static List<(int, int)> GenerateNeighbours(Point point)
        {
            List<(int, int)> neighbours = new List<(int, int)>();
            neighbours.Add(ValueTuple.Create(point.Coordinates.Item1 + 1, point.Coordinates.Item2));
            neighbours.Add(ValueTuple.Create(point.Coordinates.Item1 - 1, point.Coordinates.Item2));
            neighbours.Add(ValueTuple.Create(point.Coordinates.Item1, point.Coordinates.Item2 + 1));
            neighbours.Add(ValueTuple.Create(point.Coordinates.Item1, point.Coordinates.Item2 - 1));
            return neighbours;
        }

        public static int CalculateAbsDistance(Point first, Point second)
        {
            return  Math.Abs(first.Coordinates.Item1 - second.Coordinates.Item1) + 
                    Math.Abs(first.Coordinates.Item2 - second.Coordinates.Item2);
        }
    }


}
