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
            int playerCount = 458;
            int lastMarble = 7201900;

            int[] playerScore = new int[playerCount];
            MarbleCircle circle = new MarbleCircle();
            

            for (int marble = 2; marble <= lastMarble; marble++)
            {
                playerScore[marble % playerCount] += circle.addMarble(marble);
                //circle.printMarbles();
            }

            Console.WriteLine(playerScore.Max());
            Console.ReadLine();
        }

        

    }

    class MarbleCircle
    {
        int lastMarbleIndex = 1;
        List<int> Marbles = new List<int>() { 0, 1 };

        public int addMarble(int marble)
        {
            if (marble % 23 == 0)
            {
                return scoreMarble(marble);
            }
            else
            {
                int marbleIndex = MathMod(lastMarbleIndex + 2, Marbles.Count());
                if (marbleIndex == 0)
                {
                    Marbles.Add(marble);
                    lastMarbleIndex = Marbles.Count() - 1;
                }
                else
                {
                    Marbles.Insert(marbleIndex, marble);
                    lastMarbleIndex = marbleIndex;
                }
                return 0;
            }
        }

        private int scoreMarble(int marble)
        {
            int marbleIndex = MathMod (lastMarbleIndex - 7,Marbles.Count());
            int result = marble + Marbles[marbleIndex];
            Marbles.RemoveAt(marbleIndex);
            if (marbleIndex == 0)
            {
                lastMarbleIndex = 0;
            }
            else
            {
                lastMarbleIndex = marbleIndex;
            }
            return result;
        }

        public void printMarbles()
        {
            foreach (var i in Marbles) Console.Write(i + " ");
            Console.WriteLine();
        }
        static int MathMod(int a, int b)
        {
            return (((a % b) + b) % b);
        }
    }

    


    

    

}

