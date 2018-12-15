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
            map.Load();
            map.ProcessBattle();
        }

    }

    public class Unit
    {
        public bool isAlive { get; set; } = true;
        public enum CreatureType { Elf, Goblin }
        public Coords Coords { get; set; }
        public CreatureType Type { get; }
        public int Hitpoints { get; set; } = 200;
        public Unit(Coords coords, CreatureType type)
        {
            Coords = coords;
            Type = type;
        }

        public static CreatureType GetEnemy(CreatureType type)
        {
            if (type == CreatureType.Elf) return CreatureType.Goblin;
            else return CreatureType.Elf;
        }

        public override int GetHashCode()
        {
            return Coords.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Unit)) return false;

            Unit otherUnit = (Unit)obj;
            return (Coords.Equals(otherUnit.Coords)) && (Type == otherUnit.Type);
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

        public override int GetHashCode()
        {
            // https://stackoverflow.com/a/682481/5127149
            return ((Y << 16) ^ X);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Coords)) return false;

            Coords otherCoords = (Coords)obj;
            return (X == otherCoords.X && Y == otherCoords.Y);

        }

    }

    public class Map
    {
        HashSet<Unit> Elves = new HashSet<Unit>();
        HashSet<Unit> Goblins = new HashSet<Unit>();
        HashSet<Coords> Walls = new HashSet<Coords>();
        HashSet<Coords> Airs = new HashSet<Coords>();

        public void Load()
        {
            var file = new StreamReader(@"C:\Users\Vojta\Documents\input.txt");
            int y = 0;
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                foreach (var c in line.Select((character, x) => new {character, x}))
                {
                    var coords = new Coords(c.x, y);
                    switch (c.character)
                    {
                        case 'E':
                            Elves.Add(new Unit(coords, Unit.CreatureType.Elf));
                            break;
                        case 'G':
                            Goblins.Add(new Unit(coords, Unit.CreatureType.Goblin));
                            break;
                        case '.':
                            Airs.Add(coords);
                            break;
                        case '#':
                            Walls.Add(coords);
                            break;
                        default:
                            throw new Exception("wtf");
                    }
                }
                y++;
            }
        }

        public void ProcessBattle()
        {
            int rounds = 0;
            while (true)
            {
                foreach (var unit in Elves.Concat(Goblins).OrderBy(x => x.Coords.Y).ThenBy(x => x.Coords.X))
                {
                    if (unit.isAlive)
                    {
                        var enemyType = Unit.GetEnemy(unit.Type);
                        if (GetAliveUnits(enemyType).Count == 0) goto EXIT;
                        
                        // attacking?
                        var enemiesInRange = GetEnemyInRange(unit.Coords, enemyType);
                        if (enemiesInRange.Count == 0)
                        {
                            var newCoords = BreadthFirstSearch(unit.Coords, enemyType);
                            if (newCoords.Count() > 0)
                            {
                                //PrintMap(newCoords.First().X, newCoords.First().Y);
                                Airs.Remove(newCoords.First());
                                Airs.Add(unit.Coords);
                                unit.Coords = newCoords.First();
                            }
                            
                        }
                        enemiesInRange = GetEnemyInRange(unit.Coords, enemyType);
                        if (enemiesInRange.Count > 0)
                        {
                            var combatTarget = GetAliveUnits(enemyType).Where(x => enemiesInRange.Contains(x.Coords)).OrderBy(x => x.Hitpoints).First();
                            combatTarget.Hitpoints -= 3;
                            if (combatTarget.Hitpoints < 0)
                            {
                                Airs.Add(combatTarget.Coords);
                                GetAliveUnits(enemyType).Remove(combatTarget);
                                combatTarget.isAlive = false;
                            }
                        }
                    }
                }
                Console.WriteLine(rounds);
                PrintMap();
                rounds++;
            }
            EXIT:
            int hitpointsLeft = GetAliveUnits(Unit.CreatureType.Elf).Concat(GetAliveUnits(Unit.CreatureType.Goblin)).Sum(x => x.Hitpoints);
            Console.WriteLine(hitpointsLeft * rounds);
            PrintMap();
            Console.ReadLine();
            // 281400 high
            // 260712 high


        }

        private List<Coords> GetEnemyInRange(Coords coords, Unit.CreatureType enemyType)
        {
            List<Coords> result = new List<Coords>();
            var neighbours = GetNeighbours(coords);
            return GetAliveUnits(enemyType).Select(x => x.Coords).Intersect(neighbours).ToList();
        }

        private HashSet<Coords> GetNeighbours(Coords coords)
        {
            HashSet<Coords> neighbours = new HashSet<Coords>();
            neighbours.Add(new Coords(coords.X, coords.Y-1));
            neighbours.Add(new Coords(coords.X- 1, coords.Y));
            neighbours.Add(new Coords(coords.X + 1, coords.Y));
            neighbours.Add(new Coords(coords.X, coords.Y +1));
            return neighbours;
        }

        private HashSet<Unit> GetAliveUnits(Unit.CreatureType type)
        {
            if (type == Unit.CreatureType.Elf) return new HashSet<Unit>(Elves.Where(x => x.isAlive));
            else return new HashSet<Unit>(Goblins.Where(x => x.isAlive));
        }

        private List<Coords> BreadthFirstSearch(Coords start, Unit.CreatureType enemyType)
        {
            HashSet<Coords> searchedCoords = new HashSet<Coords>();
            Dictionary<Coords, Coords> shortestDistance = new Dictionary<Coords, Coords>();
            Queue<Coords> searchQueue = new Queue<Coords>();
            var enemyHashSet = GetAliveUnits(enemyType);

            searchQueue.Enqueue(start);
            while (searchQueue.Any())
            {
                var currentCoord = searchQueue.Dequeue();
                searchedCoords.Add(currentCoord);

                List<Coords> potentialGoals = new List<Coords>();
                foreach (var neighbour in GetNeighbours(currentCoord))
                {
                    if (!searchedCoords.Contains(neighbour) && Airs.Contains(neighbour))
                    {
                        searchQueue.Enqueue(neighbour);
                        if (!shortestDistance.ContainsKey(neighbour)) shortestDistance.Add(neighbour, currentCoord);
                        if (enemyHashSet.Select(x => x.Coords).Intersect(GetNeighbours(neighbour)).Count() > 0) potentialGoals.Add(neighbour);
                    }
                }

                if (potentialGoals.Count > 0)
                {
                    Coords potentialGoal = potentialGoals.OrderBy(x => x.Y).ThenBy(x => x.X).First();
                    while (shortestDistance.ContainsKey(potentialGoal) && shortestDistance.ContainsKey(shortestDistance[potentialGoal]))
                    {
                        potentialGoal = shortestDistance[potentialGoal];   
                    }
                    return new List<Coords>() { potentialGoal };

                }

            }
            return new List<Coords>();
        }

        private void PrintMap(int highlightX = -1, int highlightY = -1)
        {
            int maxX = Walls.Max(wall => wall.X);
            int maxY = Walls.Max(wall => wall.Y);

            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    Coords coords = new Coords(x, y);
                    if (Walls.Contains(coords))
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.Write("#");
                    }
                    else if (Airs.Contains(coords))
                    {   if (x == highlightX && y == highlightY) Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.Write(".");
                    }
                    else if (GetAliveUnits(Unit.CreatureType.Elf).Select(e => e.Coords).Contains(coords))
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write("E");
                    }
                    else if (GetAliveUnits(Unit.CreatureType.Goblin).Select(e => e.Coords).Contains(coords))
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write("G");
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

    }

}

