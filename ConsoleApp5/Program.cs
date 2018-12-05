using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {
            int minimumPolymerLength = Int32.MaxValue;

            for (int i = 65; i <= 90; i++)
            {
                var stack = new Stack<char>();
                var file = new StreamReader(File.Open(@"C:\Users\srdecny\Documents\input.txt", FileMode.Open));

                while (!file.EndOfStream)
                {
                    char nextChar = (char)file.Read();
                    if (!(nextChar - i == 0 || nextChar - i == 32)) {

                        if (stack.Count >= 1)
                        {
                            char previousChar = stack.Peek();
                            if (Math.Abs(previousChar - nextChar) == 32)
                            {
                                stack.Pop();    
                            }
                            else
                            {
                                stack.Push(nextChar);
                            }
                        }
                        else
                        {
                            stack.Push(nextChar);
                        }
                    }
                }

                stack.Pop(); // the newline char at EOF
                minimumPolymerLength = Math.Min(minimumPolymerLength, stack.Count());
                file.Close();
            }

            Console.WriteLine(minimumPolymerLength);
            Console.ReadLine();

                   
        }


    }
}
