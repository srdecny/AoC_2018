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
            var file = new StreamReader(File.Open(@"C:\Users\srdecny\Documents\input.txt", FileMode.Open));
            Node root = new Node(file);

            Console.WriteLine(root.CountMetadata());
            Console.ReadLine();

        }
    }

    class Node
    {
        public List<Node> Children { get; set; } = new List<Node>();
        public List<int> Metadata { get; set; } = new List<int>();

       

        public Node(StreamReader input)
        {
            int ChildrenCount = ReadNextInt(input);
            int MetadataCount = ReadNextInt(input);

            for (int i = 0; i < ChildrenCount; i++)
            {
                Children.Add(new Node(input));
            }

            for (int i = 0; i < MetadataCount; i++)
            {
                Metadata.Add(ReadNextInt(input));
            }
        }

        private static int ReadNextInt(StreamReader input)
        {
            // StringBuilder could perform better. Also, I feel like there's a systematic way to do this.
            string output = "";
            int lastChar = 0;
           
            while (!input.EndOfStream)
            {
                lastChar = input.Read();
                if (lastChar == ' ') break;
                output += (char)lastChar;
            }

            return Int32.Parse(output);
        }

        public int CountMetadata()
        {
            int count = 0;
            foreach (var child in Children)
            {
                count += child.CountMetadata();
            }
            foreach (var data in Metadata)
            {
                count += data;
            }
            return count;
        }
    }

    

    

}

