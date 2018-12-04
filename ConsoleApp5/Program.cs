using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace ConsoleApp5
{
    class Program
    {
        static int ClothSize = 1024;
        static void Main(string[] args)
        {
            int[,] cloth = new int[ClothSize, ClothSize];
            char[] fromSeparators = new char[] { ',', ':' };

            var file = new StreamReader(File.Open(@"C:\Users\Vojta\Documents\input.txt", FileMode.Open));
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                string[] words = line.Split();


                int fromY = Int32.Parse(words[2].Split(fromSeparators)[0]);
                int fromX = Int32.Parse(words[2].Split(fromSeparators)[1]);

                int sizeY = Int32.Parse(words[3].Split('x')[0]);
                int sizeX = Int32.Parse(words[3].Split('x')[1]);

                for (int x = fromX; x < fromX + sizeX; x++)
                {
                    for (int y = fromY; y < fromY + sizeY; y++)
                    {
                        cloth[x, y]++;
                    }
                }

            }

            int result = 0;
            for (int x = 0; x < ClothSize; x++)
            {
                for (int y = 0; y < ClothSize; y++)
                {
                    if (cloth[x, y] > 1) result++;
                }
            }
            Console.WriteLine(result);
            Console.ReadLine();

        }

        void PrintCloth(int[,] cloth)
        {
            for (int x = 0; x < ClothSize; x++)
            {
                for (int y = 0; y < ClothSize; y++)
                {
                    if (cloth[x, y] > 0) Console.Write('#');
                    else Console.Write(".");
                }
                Console.WriteLine();
            }
        }
    }
}
