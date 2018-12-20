using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Fare;

namespace ConsoleApp5
{
    class Program
    {

        static void Main(string[] args)
        {
            string input = @"C:\Users\srdecny\Documents\input.txt";
            Map map = new Map();
            map.ParseMap(input);
            

        }

    }

    public class Map
    {

        StreamReader file;
        Dictionary<Coords, List<Coords>> Doors = new Dictionary<Coords, List<Coords>>();
        HashSet<Coords> forkedCoords = new HashSet<Coords>() { new Coords(0, 0) };
        public void ParseMap(string input)
        {
            file = new StreamReader(input);
            RecurseParsing(new Coords(0, 0));
            BreadthFirstSearch();
        }

        public void RecurseParsing(Coords startCoords)
        {
            Coords currentCoords = startCoords;
            forkedCoords.Remove(currentCoords);
            while (!file.EndOfStream)
            {
                char nextChar = (char)file.Read();
                switch (nextChar)
                {
                    case ')':
                    case '$':
                        forkedCoords.Add(currentCoords);
                        return;
                    case '(':
                        RecurseParsing(currentCoords);
                        break;
                    case '^':
                        break;
                    case '|':   
                        forkedCoords.Add(currentCoords);
                        currentCoords = startCoords;
                        break;
                    default:
                        var newCoords = getNextCoords(nextChar, currentCoords);
                        if (!Doors.ContainsKey(currentCoords))
                        {
                            Doors.Add(currentCoords, new List<Coords>());
                        }
                        if (!Doors.ContainsKey(newCoords))
                        {
                            Doors.Add(newCoords, new List<Coords>());
                        }
                        Doors[newCoords].Add(currentCoords);
                        Doors[currentCoords].Add(newCoords);
                        currentCoords = newCoords;
                        break;
                }
            }
            
        }

        public void BreadthFirstSearch()
        {
            Dictionary<Coords, int> distances = new Dictionary<Coords, int>();
            Queue<Coords> searchQueue = new Queue<Coords>();
            searchQueue.Enqueue(new Coords(0, 0));
            distances.Add(new Coords(0, 0), 0);

            while (searchQueue.Any())
            {
                var searchedCoords = searchQueue.Dequeue();
                foreach (var neighbour in Doors[searchedCoords])
                {
                    if (!distances.ContainsKey(neighbour))
                    {
                        searchQueue.Enqueue(neighbour);
                        distances.Add(neighbour, distances[searchedCoords] + 1);
                    }
                }
            }

            Console.WriteLine(distances.Count(x => x.Value >= 1000));
            Console.ReadLine();
        }

        private static Coords getNextCoords(char direction, Coords coords)
        {
            switch (direction)
            {
                case 'N':
                    return new Coords(coords.X, coords.Y + 1);
                case 'S':
                    return new Coords(coords.X, coords.Y - 1);
                case 'W':
                    return new Coords(coords.X -1 , coords.Y);
                case 'E':
                    return new Coords(coords.X + 1, coords.Y);
            }
            throw new Exception("wtf");

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

