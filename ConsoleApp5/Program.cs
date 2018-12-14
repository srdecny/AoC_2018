using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApp5
{
    class Program
    {

        static void Main(string[] args)
        {
            Map map = new Map();
            map.Load();
            map.RunSimulation();
            Console.WriteLine("END");
            Console.ReadLine();
        }

    }

    public class Cart
    {
        public enum Memory { Left, Straight, Right };
        public enum Position { Up, Down, Left, Right};
        
        // HACK HACK HACK
        public int Id { get; set; }

        public Position CurrentPosition { get; set; }
        public Memory CurrentMemory { get; set; }

        public Cart(char position, int id)
        {
            Id = id;
            switch (position)
            {
                case '>':
                    CurrentPosition = Position.Right;
                    break;
                case '<':
                    CurrentPosition = Position.Left;
                    break;
                case '^':
                    CurrentPosition = Position.Up;
                    break;
                case 'v':
                    CurrentPosition = Position.Down;
                    break;
            }
            CurrentMemory = Memory.Left;
        }

        public Memory GetNextMemory()
        {
            var oldMemory = CurrentMemory;
            switch (CurrentMemory)
            {
                case Memory.Left:
                    CurrentMemory = Memory.Straight;
                    break;
                case Memory.Straight:
                    CurrentMemory = Memory.Right;
                    break;
                case Memory.Right:
                    CurrentMemory = Memory.Left;
                    break;
                default:
                    throw new Exception("wtf");
            }
            return oldMemory;
        }
        
    }

    

    public class Map
    {
        Dictionary<(int x, int y), Cart> Carts = new Dictionary<(int x, int y), Cart>();
        char[,] grid = new char[150, 150];

        public void Load()
        {
            char[] carts = "^v><".ToCharArray();
            StreamReader file = new StreamReader(@"C:\Users\srdecny\Documents\input.txt");
            int y = 0;
            int id = 0;
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                line.Select((symbol, x) => new { symbol, x }).ToList().ForEach(character =>
                {
                    if (carts.Contains(character.symbol))
                    {
                        Carts.Add((character.x, y), new Cart(character.symbol, id));
                        id++;
                        if (character.symbol == '^' || character.symbol == 'v') grid[character.x, y] = '|';
                        else grid[character.x, y] = '-';
                    }
                    else
                    {
                        grid[character.x, y] = character.symbol;
                    }
                });
                y++;
            }
        }

        public void RunSimulation()
        {
            for (int iter = 0; iter < 100000; iter++)
            {
                if (Carts.Count() == 1)
                {
                    ;
                }
                Dictionary<(int x, int y), Cart> movingCarts = new Dictionary<(int x, int y), Cart>();
                HashSet<int> cartsToRemove = new HashSet<int>();
                foreach (var cart in Carts) movingCarts.Add(cart.Key, cart.Value);
                foreach (var cart in Carts.OrderBy(x => x.Key.y).ThenBy(x => x.Key.x))
                {
                    char currentTrack = grid[cart.Key.x, cart.Key.y];
                    (int x, int y) newCoords = (-1, -1);
                    switch (currentTrack)
                    {
                        case '|':
                            if (cart.Value.CurrentPosition == Cart.Position.Up) newCoords = MoveCart(Cart.Position.Up, cart.Value, cart.Key);
                            else newCoords = MoveCart(Cart.Position.Down, cart.Value, cart.Key);
                            break;
                        case '-':
                            if (cart.Value.CurrentPosition == Cart.Position.Right) newCoords = MoveCart(Cart.Position.Right, cart.Value, cart.Key);
                            else newCoords = MoveCart(Cart.Position.Left, cart.Value, cart.Key);
                            break;
                        case '/':
                            if (cart.Value.CurrentPosition == Cart.Position.Up) newCoords = MoveCart(Cart.Position.Right, cart.Value, cart.Key);
                            else if (cart.Value.CurrentPosition == Cart.Position.Down) newCoords = MoveCart(Cart.Position.Left, cart.Value, cart.Key);
                            else if (cart.Value.CurrentPosition == Cart.Position.Right) newCoords = MoveCart(Cart.Position.Up, cart.Value, cart.Key);
                            else if (cart.Value.CurrentPosition == Cart.Position.Left) newCoords = MoveCart(Cart.Position.Down, cart.Value, cart.Key);
                            break;
                        case '\\':
                            if (cart.Value.CurrentPosition == Cart.Position.Up) newCoords = MoveCart(Cart.Position.Left, cart.Value, cart.Key);
                            else if (cart.Value.CurrentPosition == Cart.Position.Down) newCoords = MoveCart(Cart.Position.Right, cart.Value, cart.Key);
                            else if (cart.Value.CurrentPosition == Cart.Position.Right) newCoords = MoveCart(Cart.Position.Down, cart.Value, cart.Key);
                            else if (cart.Value.CurrentPosition == Cart.Position.Left) newCoords = MoveCart(Cart.Position.Up, cart.Value, cart.Key);
                            break;
                        case '+':
                            switch (cart.Value.GetNextMemory())
                            {
                                case Cart.Memory.Straight:
                                   newCoords = MoveCart(cart.Value.CurrentPosition, cart.Value, cart.Key);
                                    break;
                                case Cart.Memory.Left:
                                    if (cart.Value.CurrentPosition == Cart.Position.Up) newCoords = MoveCart(Cart.Position.Left, cart.Value, cart.Key);
                                    else if (cart.Value.CurrentPosition == Cart.Position.Down) newCoords = MoveCart(Cart.Position.Right, cart.Value, cart.Key);
                                    else if (cart.Value.CurrentPosition == Cart.Position.Right) newCoords = MoveCart(Cart.Position.Up, cart.Value, cart.Key);
                                    else if (cart.Value.CurrentPosition == Cart.Position.Left) newCoords = MoveCart(Cart.Position.Down, cart.Value, cart.Key);
                                    break;
                                case Cart.Memory.Right:
                                    if (cart.Value.CurrentPosition == Cart.Position.Up) newCoords = MoveCart(Cart.Position.Right, cart.Value, cart.Key);
                                    else if (cart.Value.CurrentPosition == Cart.Position.Down) newCoords = MoveCart(Cart.Position.Left, cart.Value, cart.Key);
                                    else if (cart.Value.CurrentPosition == Cart.Position.Right) newCoords = MoveCart(Cart.Position.Down, cart.Value, cart.Key);
                                    else if (cart.Value.CurrentPosition == Cart.Position.Left) newCoords = MoveCart(Cart.Position.Up, cart.Value, cart.Key);
                                    break;
                            }
                            break;
                        default:
                            //throw new Exception("wtf");
                            break;
                    }

                    if (grid[newCoords.x, newCoords.y] == ' ')
                    {
                        PrintMap(newCoords);
                        ;
                    }
                    // someone crashed into me
                    if (!(movingCarts.Keys.Contains(cart.Key)))
                    {
                        ; // already removed from the map
                    }
                    // crashed into someone
                    else if (movingCarts.Keys.Contains(newCoords))
                    {
                        movingCarts.Remove(cart.Key);
                        movingCarts.Remove(newCoords);
                    }
                    // not crashed into anybody
                    else
                    {
                        movingCarts.Remove(cart.Key);
                        movingCarts.Add(newCoords, cart.Value);
                    }
                }

                if ((Carts.Count() - movingCarts.Count()) % 2 != 0)
                {
                    PrintMap((-1, -1));
                    ;

                }
                Carts = movingCarts;
                if (Carts.Count == 1)
                {
                    // 80, 39 | 81,39
                    Console.WriteLine(Carts.First().Key);
                    break;
                }
            }
            ;

        }

        public static (int x, int y) MoveCart(Cart.Position direction, Cart cart, (int x, int y) coords)
        {
            switch (direction)
            {
                case Cart.Position.Up:
                    cart.CurrentPosition = Cart.Position.Up;
                    return (coords.x, coords.y -1);
                case Cart.Position.Down:
                    cart.CurrentPosition = Cart.Position.Down;
                    return (coords.x, coords.y + 1);
                case Cart.Position.Left:
                    cart.CurrentPosition = Cart.Position.Left;
                    return (coords.x - 1, coords.y);
                case Cart.Position.Right:
                    cart.CurrentPosition = Cart.Position.Right;
                    return (coords.x + 1, coords.y);
                default:
                    return (-1, -1);
            }
        }

        public void PrintMap((int x, int y) coords )
        {
            for (int y = 0; y < 150; y++)
            {
                for (int x = 0; x < 150; x++)
                {
                    if (coords.x == x && coords.y == y) Console.BackgroundColor = ConsoleColor.Yellow;

                    if (Carts.ContainsKey((x, y)))
                    {
                        if (!(Console.BackgroundColor == ConsoleColor.Yellow)) Console.BackgroundColor = ConsoleColor.Red;
                        switch (Carts[(x, y)].CurrentPosition)
                        {
                            case Cart.Position.Down:
                                Console.Write("v");
                                break;
                            case Cart.Position.Up:
                                Console.Write("^");
                                break;
                            case Cart.Position.Left:
                                Console.Write("<");
                                break;
                            case Cart.Position.Right:
                                Console.Write(">");
                                break;

                        }
                    }
                    else
                    {
                        Console.Write(grid[x, y]);
                    }
                    Console.ResetColor();

                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        
    }
}

