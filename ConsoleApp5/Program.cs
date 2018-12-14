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
            long limit = 360781;

            Dictionary<long, HashSet<long>> recipes = new Dictionary<long, HashSet<long>>();
            for (long i = 0; i < 10; i++) recipes.Add(i, new HashSet<long>());

            long firstElf = 0;
            long secondElf = 1;
            long recipeCount = 2;

            recipes[3].Add(0);
            recipes[7].Add(1);

            for (long i = 0; i < 1_000_000_000; i++)
            {
                long firstScore = recipes.First(x => x.Value.Contains(firstElf)).Key;
                long secondScore = recipes.First(x => x.Value.Contains(secondElf)).Key;
                foreach (char c in (firstScore + secondScore).ToString())
                {
                    recipes[Int64.Parse(c.ToString())].Add(recipeCount);
                    recipeCount++;

                    if (recipeCount > limit && recipeCount <= limit + 10)
                    {
                        Console.Write(c);
                    }
                }

                firstElf = (firstElf + firstScore + 1 ) % recipeCount;
                secondElf = (secondElf + secondScore + 1) % recipeCount;
            }
            EXIT:
            Console.WriteLine();
            Console.WriteLine("Finished . . .");
            Console.ReadLine();

        }

    }

}

