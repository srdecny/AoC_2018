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
            HashSet<int> currentPlants = new HashSet<int>();
            Dictionary<int, bool> plantRules = new Dictionary<int, bool>();
            StreamReader file = new StreamReader(@"C:\Users\srdecny\Documents\input.txt");

            string line = file.ReadLine();
            line.Skip(15).Select((x, i) => new { x, i }).Where(c => c.x == '#').Select(c => c.i).ToList().ForEach(x => currentPlants.Add(x));
            line = file.ReadLine();
            while (!file.EndOfStream)
            {
                line = file.ReadLine();
                int binary = line.Take(5).Select((x, i) => new { x, i }).Where(c => c.x == '#').Sum(c => (int)Math.Pow(2, c.i));
                plantRules.Add(binary, line[9] == '#' ? true : false);
            }

            int iterations = 20;
            HashSet<int> newPlants = new HashSet<int>();

            for (int iter = 0; iter < iterations; iter++)
            {
                newPlants = new HashSet<int>();
                int min = currentPlants.Min() - 3;
                int max = currentPlants.Max() + 3;


                for (int pot = min; pot <= max; pot++)
                {
                    int sum = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (currentPlants.Contains(pot + i - 2)) sum += (int)Math.Pow(2, i);
                    }
                    if (plantRules[sum]) newPlants.Add(pot);
                }
                currentPlants = newPlants;
            }
            int totalSum = currentPlants.Sum();
            Console.WriteLine(totalSum);
            Console.ReadLine();
        }

    }
    
}

