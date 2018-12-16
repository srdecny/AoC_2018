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
            string input = @"C:\Users\Vojta\Documents\input.txt";
            Registry registry = new Registry();
            registry.DeduceOperations(input);
        }

    }

    class Registry
    {
        int A;
        int B;
        int C;
        int D;

        public int this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return A;
                    case 1:
                        return B;
                    case 2:
                        return C;
                    case 3:
                        return D;
                }
                throw new Exception("wtf");
            }
            set
            {
                switch (i)
                {
                    case 0:
                        A = value;
                        break;
                    case 1:
                        B = value;
                        break;
                    case 2:
                        C = value;
                        break;
                    case 3:
                        D = value;
                        break;
                    default:
                        throw new Exception("wtf");
                }
            }
        }

        public int[] GetRegistryValues()
        {
            return new int[4] { A, B, C, D };
        }

        public void SetRegistryValues(int[] values)
        {
            A = values[0];
            B = values[1];
            C = values[2];
            D = values[3];
        }

        public enum Operations { addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr }

        public void DeduceOperations(string input)
        {
            StreamReader file = new StreamReader(input);
            Dictionary<int, List<Operations>> deducedOperations = new Dictionary<int, List<Operations>>();
            for (int i = 0; i < 16; i++)
            {
                deducedOperations.Add(i, new List<Operations>());
                for (int opcode = 0; opcode < 16; opcode++) deducedOperations[i].Add((Operations)opcode);
            }
            int moreThanThreeOpcodes = 0;
            char[] delimiters = "BeforeAfter: [,]".ToCharArray();
            while (true)
            {
                string before = file.ReadLine();
                string operation = file.ReadLine();
                string after = file.ReadLine();
                file.ReadLine();

                if (before == "") break;

                int[] intBefore = before.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToArray();
                int[] intOperation = operation.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToArray();
                int[] intAfter = after.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToArray();

                if (analyzeOperation(intBefore, intOperation, intAfter).Count() >= 3) moreThanThreeOpcodes++;
            }
            Console.WriteLine(moreThanThreeOpcodes);
            Console.ReadLine();

        }

        private List<Operations> analyzeOperation(int[] before, int[] operation, int[] after)
        {
            List<Operations> validOpcodes = new List<Operations>();

            foreach (var opcode in Enum.GetValues(typeof(Operations)).Cast<Operations>())
            {
                if (SanityCheck(opcode, operation))
                {
                    SetRegistryValues(before);
                    performOperation(opcode, operation);
                    if (GetRegistryValues().SequenceEqual(after))
                    {
                        validOpcodes.Add(opcode);
                    }
                }
            }
            return validOpcodes;
        }

        private void performOperation(Operations operation, int[] values)
        {
            switch (operation)
            {
                case Operations.addr:
                    this[values[3]] = this[values[1]] + this[values[2]];
                    break;
                case Operations.addi:
                    this[values[3]] = this[values[1]] + values[2];
                    break;
                case Operations.mulr:
                    this[values[3]] = this[values[1]] * this[values[2]];
                    break;
                case Operations.muli:
                    this[values[3]] = this[values[1]] * values[2];
                    break;
                case Operations.banr:
                    this[values[3]] = this[values[1]] & this[values[2]];
                    break;
                case Operations.bani:
                    this[values[3]] = this[values[1]] & values[2];
                    break;
                case Operations.borr:
                    this[values[3]] = this[values[1]] | this[values[2]];
                    break;
                case Operations.bori:
                    this[values[3]] = this[values[1]] | values[2];
                    break;
                case Operations.setr:
                    this[values[3]] = this[values[1]];
                    break;
                case Operations.seti:
                    this[values[3]] = values[1];
                    break;
                case Operations.gtir:
                    this[values[3]] = values[1] > this[values[2]] ? 1 : 0;
                    break;
                case Operations.gtri:
                    this[values[3]] = this[values[1]] > values[2] ? 1 : 0;
                    break;
                case Operations.gtrr:
                    this[values[3]] = this[values[1]] > this[values[2]] ? 1 : 0;
                    break;
                case Operations.eqir:
                    this[values[3]] = values[1] == this[values[2]] ? 1 : 0;
                    break;
                case Operations.eqri:
                    this[values[3]] = this[values[1]] == values[2] ? 1 : 0;
                    break;
                case Operations.eqrr:
                    this[values[3]] = this[values[1]] == this[values[2]] ? 1 : 0;
                    break;
                default:
                    throw new Exception("Wtf");
            }
        }

        private bool SanityCheck(Operations opcode, int[] values)
        {
            switch (opcode)
            {
                case Operations.addr:
                case Operations.mulr:
                case Operations.banr:
                case Operations.borr:
                case Operations.setr:
                    return (values[3] <= 3 && values[2] <= 3 && values[1] <= 3);
                case Operations.addi:
                case Operations.muli:
                case Operations.bani:
                case Operations.bori:
                case Operations.seti:
                    return (values[3] <= 3 && values[1] <= 3);
                case Operations.gtir:
                case Operations.eqir:
                    return (values[3] <= 3 && values[2] <= 3);
                case Operations.gtri:
                case Operations.eqri:
                    return (values[3] <= 3 && values[1] <= 3);
                case Operations.eqrr:
                case Operations.gtrr:
                return (values[3] <= 3 && values[2] <= 3 && values[1] <= 3);
                    
            }
            throw new Exception("wtf");
        }

      
    }
}

