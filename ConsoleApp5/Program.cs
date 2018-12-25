using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Priority_Queue;
using System.Text.RegularExpressions;

namespace ConsoleApp5
{
    class Program
    {

        static void Main(string[] args)
        {
            string input = @"C:\Users\Vojta\Documents\input.txt";
            Map map = new Map();
            map.ComputeConstellations(input);
            map.FindAmountOfConstellations();

            Console.WriteLine("Finished...");
            Console.ReadLine();
        }

    }

   public class Map
    {
        Dictionary<int, List<int>> Constellations = new Dictionary<int, List<int>>();
        List<Coords> Coordinates = new List<Coords>();

        public void ComputeConstellations(string input)
        {
            var lines = File.ReadAllLines(input);
            foreach (var line in lines) Coordinates.Add(new Coords(line));
            foreach (var line in Coordinates.Select((line, index) => Tuple.Create(line, index)).ToList())
            {
                Constellations[line.Item2] = new List<int>();
                foreach(var coords in Coordinates)
                {
                    if (line.Item1 != coords && coords.IsInConstellation(line.Item1))
                    {
                        Constellations[line.Item2].Add(Coordinates.IndexOf(coords));
                    }
                }
            }
        }

        public void FindAmountOfConstellations()
        {
            int constellationCount = 0;
            while (Constellations.Keys.Count > 0)
            {
                List<int> constellationsToRemove = BreadthFirstSearch(Constellations.Keys.First());
                foreach (var constellation in constellationsToRemove) Constellations.Remove(constellation);
                constellationCount++;
            }
            Console.WriteLine(constellationCount);
        }

        // Find all connected components
        private List<int> BreadthFirstSearch(int start)
        {
            List<int> visitedCoords = new List<int>() { start };
            Queue<int> searchQueue = new Queue<int>();
            searchQueue.Enqueue(start);
            while (searchQueue.Any())
            {
                int currentVertex = searchQueue.Dequeue();
                foreach (var neighbour in Constellations[currentVertex])
                {
                    if (!visitedCoords.Contains(neighbour))
                    {
                        visitedCoords.Add(neighbour);
                        searchQueue.Enqueue(neighbour);
                    }
                }
            }

            return visitedCoords;
        }

    }

    public struct Coords
    {
        public int W { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public Coords(string line)
        {
            var words = line.Split(',');
            W = Int32.Parse(words[0]);
            X = Int32.Parse(words[1]);
            Y = Int32.Parse(words[2]);
            Z = Int32.Parse(words[3]);
        }

        public bool IsInConstellation(Coords other)
        {
            int distance = 0;
            distance += Math.Abs(W - other.W);
            distance += Math.Abs(X - other.X);
            distance += Math.Abs(Y - other.Y);
            distance += Math.Abs(Z - other.Z);
            return (distance <= 3);
        }

        public static bool operator == (Coords first, Coords second)
        {
            return (first.W == second.W && first.X == second.X && first.Y == second.Y && first.Z == second.Z);
        }

        public static bool operator !=(Coords first, Coords second)
        {
            return !(first==second);
        }
    }


}