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
        static void Main(string[] args)
        {
            Dictionary<string, int[]> SleepGram = new Dictionary<string, int[]>();
            string CurrentGuard = "";
            int sleepsFrom = 0;
            int sleepsTo = 0;

            string[] lines = File.ReadAllLines(@"C:\Users\Vojta\Documents\input.txt");
            List<GuardLine> GuardLines = new List<GuardLine>();
            foreach (var line in lines) GuardLines.Add(new GuardLine(line));
            GuardLines.Sort();

            foreach (var guardline in GuardLines)
            {
                string[] words = guardline.line.Split();

                switch (words[2])
                {
                    case "falls":
                        sleepsFrom = parseMinutes(words[1]);
                        break;

                    case "wakes":
                        sleepsTo = parseMinutes(words[1]);

                        for (int i = sleepsFrom; i < sleepsTo; i++)
                        {
                            SleepGram[CurrentGuard][i]++;
                        }
                        break;

                    case "Guard":
                        CurrentGuard = words[3];
                        if (!SleepGram.Keys.Contains(CurrentGuard))
                        {
                            SleepGram.Add(CurrentGuard, new int[60]);
                        }
                        break;

                    default:
                        throw new Exception("wtf");
                }

            }

            Dictionary<string, (int, int)> MostCommonMinute = new Dictionary<string, (int, int)>();
            foreach (var kvp in SleepGram)
            {
                int minute = kvp.Value.ToList().IndexOf(kvp.Value.Max());
                int count = kvp.Value.Max();
                MostCommonMinute.Add(kvp.Key, (minute, count));
            }

            var result = MostCommonMinute.OrderByDescending(x => x.Value.Item2).First();
            Console.WriteLine(result.Key + " " + result.Value);
            Console.ReadLine();
                        


        }

        public static int parseMinutes(string date)
        {
            // 00:26]
            return Int32.Parse(date.Split(':')[1].Substring(0, 2));
        }
    }

    public class GuardLine : IComparable<GuardLine>
    {
        public string line;

        public GuardLine(string s)
        {
            line = s;
        }

        public int CompareTo(GuardLine other)
        {
            string thisDate = line.Split()[0] + " " + line.Split()[1];
            string otherDate = other.line.Split()[0] + " " + other.line.Split()[1];

            thisDate = thisDate.Replace("[", "");
            thisDate = thisDate.Replace("]", "");
            otherDate = otherDate.Replace("[", "");
            otherDate = otherDate.Replace("]", "");

            DateTime thisDateTime = DateTime.ParseExact(thisDate, "yyyy-MM-dd mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime otherDateTime = DateTime.ParseExact(otherDate, "yyyy-MM-dd mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            if (thisDateTime < otherDateTime) return -1;
            else return 1;
        }
    }
}
