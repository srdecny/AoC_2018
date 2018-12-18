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
            string input = @"C:\Users\Vojta\Documents\input.txt";
            Map map = new Map();
            map.Load(input);
            map.SimulateMinutes(1000000000);
            Console.WriteLine(map.GetResourceValue());
            Console.ReadLine();
            // 770775 high
        }

    }

    class Map
    {
        static int maxX = 50;
        static int maxY = 50;
        char[,] Grid = new char[maxX, maxY];
        Dictionary<int, List<int>> history = new Dictionary<int, List<int>>();

        public void Load(string input)
        {
            StreamReader file = new StreamReader(input);
            int y = 0;
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                foreach (var character in line.Select((symbol, x) => new { symbol, x }))
                {
                    Grid[character.x, y] = character.symbol;
                }
                y++;
            }

        }

        public void SimulateMinutes(int minutes)
        {
            List<int> loopHashCodes = new List<int>()
                {
                    33,
                    3609,
                    2884,
                    6213,
                    4532,
                    7366,
                    2906,
                    1797,
                    4314,
                    7245,
                    6489,
                    7867, // start of the loop
                    5351,
                    884 ,
                    8125,
                    7815,
                    7277,
                    7501,
                    7477,
                    1981,
                    1859,
                    6000,
                    5758,
                    2396,
                    6288,
                    6146,
                    4188,
                    6469,
                };

            for (int i = 1; i <= minutes; i++)
            {
                char[,] newGrid = new char[maxX, maxY];
                for (int y = 0; y < maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        int treeCount = GetNeighbours((x, y)).Count(acre => Grid[acre.x, acre.y] == '|');
                        int lumberyardCount = GetNeighbours((x, y)).Count(acre => Grid[acre.x, acre.y] == '#');

                        switch (Grid[x, y])
                        {
                            case '.':
                                if (treeCount >= 3) newGrid[x, y] = '|';
                                else newGrid[x, y] = '.';
                                break;
                            case '#':
                                if (lumberyardCount >= 1 && treeCount >= 1) newGrid[x, y] = '#';
                                else newGrid[x, y] = '.';
                                break;
                            case '|':
                                if (lumberyardCount >= 3) newGrid[x, y] = '#';
                                else newGrid[x, y] = '|';
                                break;
                        }
                    }
                }

                Grid = newGrid;
                // loop starts at 417
                if (GetHashCode() == 1797)
                {
                    Console.WriteLine(GetResourceValue());
                }
                
            }
        }

        public static List<(int x, int y)> GetNeighbours((int x, int y) coords)
        {
            List<(int x, int y)> neighbours = new List<(int x, int y)>();
            if (coords.x + 1 < maxX) neighbours.Add((coords.x + 1, coords.y));
            if (coords.y + 1 < maxY) neighbours.Add((coords.x, coords.y + 1));
            if (coords.x - 1 >= 0) neighbours.Add((coords.x - 1, coords.y));
            if (coords.y - 1 >= 0) neighbours.Add((coords.x, coords.y - 1));

            if (coords.x + 1 < maxX && coords.y + 1 < maxY ) neighbours.Add((coords.x + 1, coords.y + 1));
            if (coords.y + 1 < maxY && coords.x - 1 >= 0) neighbours.Add((coords.x - 1, coords.y + 1));
            if (coords.x - 1 >= 0 && coords.y - 1 >= 0) neighbours.Add((coords.x - 1, coords.y - 1));
            if (coords.y - 1 >= 0 && coords.x + 1 < maxX) neighbours.Add((coords.x + 1, coords.y - 1));
            return neighbours;
        }

        public int GetResourceValue()
        {
            int lumberyards = 0;
            int trees = 0;
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (Grid[x, y] == '|') trees++;
                    if (Grid[x, y] == '#') lumberyards++;
                }

            }
            return trees * lumberyards;
        }

        public void PrintMap()
        {
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    Console.ResetColor();

                    if (Grid[x, y] == '|')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.Write("|");
                    }
                    else if (Grid[x, y] == '#')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.Write("#");

                    }
                    else
                    {
                        Console.Write(".");
                    }

                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public override int GetHashCode()
        {
            int bucketCount = 8192;
            int result = 0;
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (Grid[x, y] == '|') result += 7 * x * y;
                    if (Grid[x, y] == '#') result += 13 * x * y;
                }

            }
            return result % bucketCount;
        }
    }
}

