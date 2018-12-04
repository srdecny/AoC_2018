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
            List<int> IDs = new List<int>();
            var file = new StreamReader(File.Open(@"C:\Users\Vojta\Documents\input.txt", FileMode.Open));
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                string[] words = line.Split();
                IDs.Add(Int32.Parse(words[0].Split('#')[1]));
            }
            file.Close();


            var file2 = new StreamReader(File.Open(@"C:\Users\Vojta\Documents\input.txt", FileMode.Open));
            while (!file2.EndOfStream)
            {
                string line = file2.ReadLine();
                string[] words = line.Split();

                int id = Int32.Parse(words[0].Split('#')[1]);

                int fromY = Int32.Parse(words[2].Split(fromSeparators)[0]);
                int fromX = Int32.Parse(words[2].Split(fromSeparators)[1]);

                int sizeY = Int32.Parse(words[3].Split('x')[0]);
                int sizeX = Int32.Parse(words[3].Split('x')[1]);

                for (int x = fromX; x < fromX + sizeX; x++)
                {
                    for (int y = fromY; y < fromY + sizeY; y++)
                    {   
                        if (cloth[x, y] != 0)
                        {
                            IDs.Remove(id);
                            IDs.Remove(cloth[x, y]);
                        }
                        cloth[x, y] = id;
                    }
                }

            }
            //PrintCloth(cloth);
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

        static void PrintCloth(int[,] cloth)
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
