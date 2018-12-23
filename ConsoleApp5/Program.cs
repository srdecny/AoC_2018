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
            Map map = new Map();
            map.CalculateGeologicIndex();
            map.CalculateErosionLevel();
            map.CalculateRiskLevel();
            //map.SumRiskLevel();
            map.Dijkstra();

            // 795 low
            // 1072 high
            // 1068 for another person
            // 1064 not
            Console.WriteLine("Finished...");
            Console.ReadLine();
        }

    }

    public struct Coords
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public Coords(int x, int y, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }  

    public class Map
    {
        static Coords MaximumSize = new Coords(200, 1100); // the total area searched

        static int Depth = 5616;
        //static int Depth = 510;
        //static Coords Target = new Coords(10,10);
        static Coords Target = new Coords(10,785);
        long[,] GeologicIndex = new long[MaximumSize.X + 1, MaximumSize.Y + 1];
        int[,] ErosionLevel = new int[MaximumSize.X + 1, MaximumSize.Y + 1];
        int[,] RiskLevel = new int[MaximumSize.X + 1, MaximumSize.Y + 1];

        public void CalculateGeologicIndex()
        {
            for (int x = 0; x <= MaximumSize.X; x++)
            {
                for (int y = 0; y <= MaximumSize.Y; y++)
                {

                    if (x == 0 && y == 0) GeologicIndex[x, y] = 0;
                    else if (x == Target.X && y == Target.Y) GeologicIndex[x, y] = 0;
                    else if (x == 0) GeologicIndex[x, y] = y * 48271;
                    else if (y == 0) GeologicIndex[x, y] = x * 16807;
                    else GeologicIndex[x, y] = GetErosionLevel(new Coords(x, y));
                }
            }
        }

        public void CalculateErosionLevel()
        {
            for (int x = 0; x <= MaximumSize.X; x++)
            {
                for (int y = 0; y <= MaximumSize.Y; y++)
                {
                    
                    ErosionLevel[x, y] = (int)((GeologicIndex[x, y] + Depth) % 20183);
                }
            }
        }

        public long GetErosionLevel(Coords coords)
        {
            long LeftErosionLevel = ((GeologicIndex[coords.X - 1, coords.Y] + Depth) % 20183);
            long RightErosionLevel = ((GeologicIndex[coords.X, coords.Y - 1] + Depth) % 20183);
            return LeftErosionLevel * RightErosionLevel;
        }
        public void CalculateRiskLevel()
        {
            for (int x = 0; x <= MaximumSize.X; x++)
            {
                for (int y = 0; y <= MaximumSize.Y; y++)
                {
                    RiskLevel[x, y] = ErosionLevel[x, y] % 3;
                }
            }

        }
        public void SumRiskLevel()
        {
            int level = 0;
            for (int x = 0; x <= Target.X; x++)
            {
                for (int y = 0; y <= Target.Y; y++)
                {
                    level += RiskLevel[x, y];
                }
            }
            Console.WriteLine(level);

        }

        public void Dijkstra()
        {
            // z coordinates == current equipment
            Dictionary<Coords, List<Edge>> EdgeMap = new Dictionary<Coords, List<Edge>>();
            Dictionary<Coords, int> shortestPath = new Dictionary<Coords, int>();
            SimplePriorityQueue<Coords> searchQueue = new SimplePriorityQueue<Coords>();

            for (int x = 0; x < MaximumSize.X; x++)
            {
                for (int y = 0; y < MaximumSize.Y; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        Coords currentCoords = new Coords(x, y, z);
                        EdgeMap[currentCoords] = new List<Edge>();
                        shortestPath.Add(currentCoords, int.MaxValue);
                        int currentTerrain = RiskLevel[currentCoords.X, currentCoords.Y];

                        // Add all changes of gear
                        foreach (int equipment in Enumerable.Range(0, 3))
                        {
                            if (equipment != z)
                            {
                                switch (currentTerrain)
                                {
                                    // rocky, only torch and gear
                                    case 0:
                                        if (equipment == 0 || equipment == 1)
                                        {
                                            EdgeMap[currentCoords].Add(new Edge(7, new Coords(x, y, equipment)));
                                        }
                                        break;

                                    // wet, only gear or neither
                                    case 1:
                                        if (equipment == 2 || equipment == 1)
                                        {
                                            EdgeMap[currentCoords].Add(new Edge(7, new Coords(x, y, equipment)));
                                        }
                                        break;
                                    // narrow, only torch or neither
                                    case 2:
                                        if (equipment == 0 || equipment == 2)
                                        {
                                            EdgeMap[currentCoords].Add(new Edge(7, new Coords(x, y, equipment)));
                                        }
                                        break;
                                }

                            
                            }
                        }

                        // Add all possible neighbours
                        foreach (var neighbour in GetNeighbourCoords(currentCoords))
                        {
                            int neighbourTerrain = RiskLevel[neighbour.X, neighbour.Y];
                            switch (currentCoords.Z)
                            {
                                // Torch equipped, can move to rocky or narrow
                                case 0:
                                    if (neighbourTerrain == 0 || neighbourTerrain == 2)
                                    {
                                        EdgeMap[currentCoords].Add(new Edge(1, neighbour));
                                    }
                                    break;
                                // gear equipped, can move to wet or rocky
                                case 1:
                                    if (neighbourTerrain == 1 || neighbourTerrain == 0)
                                    {
                                        EdgeMap[currentCoords].Add(new Edge(1, neighbour));
                                    }
                                    break;
                                // neither equipped, can move to wet or narrow
                                case 2:
                                    if (neighbourTerrain == 1 || neighbourTerrain == 2)
                                    {
                                        EdgeMap[currentCoords].Add(new Edge(1, neighbour));
                                    }
                                    break;
                            }
                        }

                    
                    }
                }
            }

            searchQueue.Enqueue(new Coords(0, 0, 0), AStarHeuristics(new Coords(0,0,0), Target));
            shortestPath[new Coords(0, 0, 0)] = 0;
            HashSet<Coords> visitedCoords = new HashSet<Coords>();
            HashSet<Coords> openCoords = new HashSet<Coords>();
            openCoords.Add(new Coords(0, 0, 0));
            while (searchQueue.Any())
            {
                var searchedCooords = searchQueue.Dequeue();
                int currentTime = shortestPath[searchedCooords];

                visitedCoords.Add(searchedCooords);
                openCoords.Remove(searchedCooords);

                if (searchedCooords.X == Target.X && searchedCooords.Y == Target.Y && searchedCooords.Z == 0)
                {
                    Console.WriteLine($"Target found, time elapsed: {currentTime}");
                }

                foreach (var edge in EdgeMap[searchedCooords])
                {
                    if (visitedCoords.Contains(edge.To))
                    {
                        continue;
                    }

                    int edgeDistance = edge.Value + currentTime;
                    if (!openCoords.Contains(edge.To))
                    {
                        openCoords.Add(edge.To);
                        searchQueue.Enqueue(edge.To, edgeDistance + AStarHeuristics(edge.To, Target));
                    }
                    else if (edgeDistance >= shortestPath[edge.To])
                    {
                        continue;
                    }

                    shortestPath[edge.To] = edgeDistance;
                    searchQueue.TryUpdatePriority(edge.To, edgeDistance + AStarHeuristics(edge.To, Target));
                }


            }


        }

        private struct Edge
        {
            public int Value { get; }
            public Coords To { get;}
            public Edge(int value, Coords to)
            {
                Value = value;
                To = to;
            }
        }

        private List<Coords> GetNeighbourCoords(Coords coords)
        {
            List<Coords> neighbours = new List<Coords>();

            if (coords.X + 1 < MaximumSize.X) neighbours.Add(new Coords(coords.X + 1, coords.Y, coords.Z));
            if (coords.Y + 1 < MaximumSize.Y) neighbours.Add(new Coords(coords.X, coords.Y + 1, coords.Z));
            if (coords.Y - 1 > 0) neighbours.Add(new Coords(coords.X, coords.Y - 1, coords.Z));
            if (coords.X - 1 > 0) neighbours.Add(new Coords(coords.X - 1, coords.Y, coords.Z));

            return neighbours;
        }

        private void PrintMap(int highlightX = -1, int highlightY = -1)
        {
            for (int y = 0; y < MaximumSize.Y; y++)
            {
                for (int x = 0; x < MaximumSize.X; x++)
                {
                    if (x == highlightX && y == highlightY) Console.BackgroundColor = ConsoleColor.Yellow;

                    switch (RiskLevel[x, y])
                    {
                        case 0:
                            Console.Write(".");
                            break;
                        case 1:
                            Console.Write("=");
                            break;
                        case 2:
                            Console.Write("|");
                            break;
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private int AStarHeuristics(Coords from, Coords to)
        {
            int guesstimate = 0;
            if (from.Z != to.Z) guesstimate += 7;
            guesstimate += Math.Abs(to.X - from.X);
            guesstimate += Math.Abs(to.Y - from.Y);
            return guesstimate;
        }
    }


}