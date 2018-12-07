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

            while (SortedLetters.Count != 0)
            {
                foreach (var letter in SortedLetters)
                {
                    if (Prerequisities[letter].Count == 0 ||
                        Prerequisities[letter].All(x => CompletedLetters.Contains(x)))
                    {
                        CompletedLetters.Add(letter);
                        SortedLetters.Remove(letter);
                        break;
                    }
                }
            }

            foreach (var letter in CompletedLetters) Console.Write(letter);
            Console.ReadLine();

        }

           
    }


}

