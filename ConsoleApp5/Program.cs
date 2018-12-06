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
        // Ugly as a sin.
        static void Main(string[] args)
        {
            int lastSpreadPoint = 0;
            List<Point> points = new List<Point>();
            Dictionary<int, int> pointsClaimedItems = new Dictionary<int, int>();
            HashSet<(int, int)> claimedPoints = new HashSet<(int, int)>(); // already claimed in round N-2 and later.

            var file = new StreamReader(File.Open(@"C:\Users\srdecny\Documents\input.txt", FileMode.Open));
            int id = 0;
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                string[] coords = line.Split(new string[] { ", " }, StringSplitOptions.None);
                points.Add(new Point(Int32.Parse(coords[0]), Int32.Parse(coords[1])));
                points.Last().Id = id;
                pointsClaimedItems.Add(id, 1); // each Id has one starting point
                claimedPoints.Add(ValueTuple.Create(points.Last().Coordinates.Item1, points.Last().Coordinates.Item2));
                id++;
            }

            List<Point> boxedPoints = Point.CalculateBoxedPoints(points);
            List<Point> spreadingPoints = new List<Point>(points); // points claimed in round N-1, these will spread around
            while (!boxedPoints.All(x => x.stoppedSpreading == true))
            {
                Dictionary<(int, int), List<int>> stakedPoints = new Dictionary<(int, int), List<int>>();
                foreach(var point in spreadingPoints)
                {
                    foreach (var coord in Point.GenerateNeighbours(point))
                    {
                        if (!claimedPoints.Contains(coord))
                        {
                            if (!stakedPoints.ContainsKey(coord)) { stakedPoints.Add(coord, new List<int>()); }
                            stakedPoints[coord].Add(point.Id);
                        }
                    }
                }

                foreach (var point in spreadingPoints) { claimedPoints.Add(point.Coordinates); }
                spreadingPoints.Clear();

                var contestedPoints = stakedPoints.Where(x => x.Value.Distinct().Count() > 1).Select(x => x.Key).ToList();
                foreach (var point in contestedPoints) { claimedPoints.Add(point); }
                var uncontestedPoints = stakedPoints.Where(x => x.Value.Distinct().Count() == 1).Select(x => x).ToList();
                foreach(var point in uncontestedPoints)
                {
                    claimedPoints.Add(point.Key);
                    pointsClaimedItems[point.Value[0]]++;
                    spreadingPoints.Add(new Point(point.Key.Item1, point.Key.Item2));
                    spreadingPoints.Last().Id = point.Value[0];
                }

                // stop spreading
                var pointIds = boxedPoints.Where(x => x.stoppedSpreading == false).Distinct().Select(x => x.Id).ToList();
                foreach(var point in spreadingPoints.Distinct())
                {
                    if (pointIds.Contains(point.Id))
                    {
                        pointIds.Remove(point.Id);
                    }
                }

                foreach (var stoppedPoint in pointIds)
                {
                    boxedPoints.Where(x => x.Id == stoppedPoint).First().stoppedSpreading = true;
                    lastSpreadPoint++;
                    if (lastSpreadPoint == 25) // experimentally verified the 25th point is the last to spread
                    {
                        var stoppedPointIds = boxedPoints.Where(x => x.stoppedSpreading).Select(x => x.Id).ToList();
                        var max = pointsClaimedItems.Where(x => stoppedPointIds.Contains(x.Key)).Max(x => x.Value);
                        Console.WriteLine(max);
                        Console.ReadLine();
                    }
                }




            }


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

            foreach(var point in points)
            {
                if (point.Coordinates.Item1 > minX &&
                    point.Coordinates.Item2 > minY &&
                    point.Coordinates.Item1 < maxX &&
                    point.Coordinates.Item2 < maxY)
                {
                    boxedPoints.Add(point);
                }
            }
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
    }


}
