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
            long playerCount = 458;
            long lastMarble = 7201900;

            long[] playerScore = new long[playerCount];
            MarbleCircle circle = new MarbleCircle();
            

            for (long marble = 1; marble <= lastMarble; marble++)
            {
                playerScore[marble % playerCount] += circle.addMarble(marble);
            }

            Console.WriteLine(playerScore.Max());
            Console.ReadLine();
        }

        

    }

    class MarbleCircle
    {
        LinkedListNode<long> currentNode;
        LinkedList<long> Marbles = new LinkedList<long>();

        public MarbleCircle()
        {
            LinkedListNode<long> node = new LinkedListNode<long>(0);
            Marbles.AddFirst(node);
            currentNode = node;
        }

        public long addMarble(long marble)
        {
            if (marble % 23 == 0)
            {
                return scoreMarble(marble);
            }
            else
            {
                Marbles.AddAfter(currentNode.NextOrFirst(), marble);
                currentNode = currentNode.NextOrFirst().NextOrFirst();
                return 0;
            }
        }

        private long scoreMarble(long marble)
        {
            for (int i = 0; i < 6; i++)
            {
                currentNode = currentNode.PreviousOrLast();
            }
            long result = marble + currentNode.PreviousOrLast().Value;
            Marbles.Remove(currentNode.PreviousOrLast());
            return result;

        }

       
    }

    static class CircularLinkedList
    {
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }

        public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
        {
            return current.Previous ?? current.List.Last;
        }
    }








}

