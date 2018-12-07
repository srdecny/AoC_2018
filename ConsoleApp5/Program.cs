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
            HashSet<string> Letters = new HashSet<string>();
            Dictionary<string, List<string>> Prerequisities = new Dictionary<string, List<string>>();
            List<string> CompletedLetters = new List<string>();
            List<WorkItem> WorkQueue = new List<WorkItem>();

            var file = new StreamReader(File.Open(@"C:\Users\srdecny\Documents\input.txt", FileMode.Open));
            while (!file.EndOfStream)
            {
                string[] words = file.ReadLine().Split();
                Letters.Add(words[1]);
                Letters.Add(words[7]);
                if (!Prerequisities.ContainsKey(words[7]))
                {
                    Prerequisities[words[7]] = new List<string>();
                }
                if (!Prerequisities.ContainsKey(words[1]))
                {
                    Prerequisities[words[1]] = new List<string>();
                }

                Prerequisities[words[7]].Add(words[1]);
            }

            List<string> SortedLetters = Letters.OrderBy(x => x).ToList();

            int Clock = 0;

            while (CompletedLetters.Count != Letters.Count)
            {
                List<string> avaliableLetters = new List<string>();

                foreach (var letter in SortedLetters)
                {

                    if (Prerequisities[letter].Count == 0 ||
                        Prerequisities[letter].All(x => CompletedLetters.Contains(x)))
                    {
                        avaliableLetters.Add(letter);
                    }
                }

                foreach (var avaliableLetter in avaliableLetters)
                {
                    if (WorkQueue.Count < 5)
                    {
                        WorkQueue.Add(new WorkItem(avaliableLetter));
                        SortedLetters.Remove(avaliableLetter);
                    }
                    else
                    {
                        break;
                    }
                }
            
                Clock++;
                // We can't modify WorkQueue directly since it would invalidate the iterator. 
                List<WorkItem> newQueue = new List<WorkItem>();
                foreach (var item in WorkQueue)
                {
                    item.RemainingSeconds--;
                    if (item.RemainingSeconds > 0)
                    {
                        newQueue.Add(item);
                    }
                    else
                    {
                        CompletedLetters.Add(item.Letter);
                    }
                }
                WorkQueue = newQueue;

            }
            Console.WriteLine(Clock);
            Console.ReadLine();

        }

           
    }

    class WorkItem
    {
        public int RemainingSeconds { get; set; }
        public string Letter { get; set; }

        public WorkItem(string l)
        {
            RemainingSeconds = calculateDuration(l);
            Letter = l;
        }

        static int calculateDuration(string letter)
        {
            return 60 + Convert.ToChar(letter[0]) - 64;
        }
    }

}

