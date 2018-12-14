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
            long limit = 59414;
            long bufferSize = limit.ToString().Count();
            long[] buffer = new long[bufferSize];

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
                    LeftShiftArray(buffer, 1);
                    buffer[bufferSize - 1] = Int64.Parse(c.ToString());
                    recipeCount++;

                    long bufferSum = 0;
                    char[] numbersArray = limit.ToString().ToCharArray();
                    if (numbersArray.Contains(c))
                    {
                        for (long b = 0; b < bufferSize; b++)
                        {
                            bufferSum += buffer[b] * (long)Math.Pow(10, (bufferSize - b - 1));
                        }
                        if (bufferSum == limit)
                        {
                            Console.WriteLine(recipeCount - bufferSize);
                            goto EXIT;
                        }
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

        // https://stackoverflow.com/a/38483135
        public static void LeftShiftArray<T>(T[] arr, long shift)
        {
            shift = shift % arr.Length;
            T[] buffer = new T[shift];
            Array.Copy(arr, buffer, shift);
            Array.Copy(arr, shift, arr, 0, arr.Length - shift);
            Array.Copy(buffer, 0, arr, arr.Length - shift, shift);
        }

    }

}

