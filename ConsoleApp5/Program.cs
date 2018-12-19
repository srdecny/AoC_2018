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
            string input = @"C:\Users\srdecny\Documents\input.txt";
            Registry registry = new Registry();
            registry.ParseInput(input);
            registry.DecompiledProgram();
        }

    }

    class Registry
    {
        Dictionary<int, Instruction> Program = new Dictionary<int, Instruction>();
        int instructionRegistry;

        int A;
        int B;
        int C;
        int D;
        int E;
        int F;

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
                    case 4:
                        return E;
                    case 5:
                        return F;
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
                    case 4:
                        E = value;
                        break;
                    case 5:
                        F = value;
                        break;
                    default:
                        throw new Exception("wtf");
                }
            }
        }

        public int[] GetRegistryValues()
        {
            return new int[6] { A, B, C, D, E, F};
        }

        public void SetRegistryValues(int[] values)
        {
            A = values[0];
            B = values[1];
            C = values[2];
            D = values[3];
            E = values[4];
            F = values[5];
        }

        public enum Operations { addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr }

        private void performOperation(Instruction instruction)
        {
            Operations operation = instruction.Opcode;
            int[] values = instruction.Values;
            switch (operation)
            {
                case Operations.addr:
                    this[values[2]] = this[values[0]] + this[values[1]];
                    break;
                case Operations.addi:
                    this[values[2]] = this[values[0]] + values[1];
                    break;
                case Operations.mulr:
                    this[values[2]] = this[values[0]] * this[values[1]];
                    break;
                case Operations.muli:
                    this[values[2]] = this[values[0]] * values[1];
                    break;
                case Operations.banr:
                    this[values[2]] = this[values[0]] & this[values[1]];
                    break;
                case Operations.bani:
                    this[values[2]] = this[values[0]] & values[1];
                    break;
                case Operations.borr:
                    this[values[2]] = this[values[0]] | this[values[1]];
                    break;
                case Operations.bori:
                    this[values[2]] = this[values[0]] | values[1];
                    break;
                case Operations.setr:
                    this[values[2]] = this[values[0]];
                    break;
                case Operations.seti:
                    this[values[2]] = values[0];
                    break;
                case Operations.gtir:
                    this[values[2]] = values[0] > this[values[1]] ? 1 : 0;
                    break;
                case Operations.gtri:
                    this[values[2]] = this[values[0]] > values[1] ? 1 : 0;
                    break;
                case Operations.gtrr:
                    this[values[2]] = this[values[0]] > this[values[1]] ? 1 : 0;
                    break;
                case Operations.eqir:
                    this[values[2]] = values[0] == this[values[1]] ? 1 : 0;
                    break;
                case Operations.eqri:
                    this[values[2]] = this[values[0]] == values[1] ? 1 : 0;
                    break;
                case Operations.eqrr:
                    this[values[2]] = this[values[0]] == this[values[1]] ? 1 : 0;
                    break;
                default:
                    throw new Exception("Wtf");
            }
        }

        public void ParseInput(string input)
        {
            StreamReader file = new StreamReader(input);
            string line = file.ReadLine();
            instructionRegistry = Int32.Parse(line.Split(' ')[1]);
            int instructionCount = 0;
            while (!file.EndOfStream)
            {
                line = file.ReadLine();
                string[] words = line.Split(' ');
                int[] values = new int[3];
                values[0] = Int32.Parse(words[1]);
                values[1] = Int32.Parse(words[2]);
                values[2] = Int32.Parse(words[3]);
                Operations opcode;
                Enum.TryParse(words[0], out opcode);
                Program.Add(instructionCount, new Instruction(opcode, values));
                instructionCount++;

            }
        }

        public void RunProgram()
        {
            int instructionCount = Program.Values.Count();
            int instructionPointer = 0;
            this[0] = 1;

            while (instructionPointer < instructionCount)
            {
                

                Console.Write($"{instructionPointer}:  ");
                DumpRegistry();

                this[instructionRegistry] = instructionPointer;
                performOperation(Program[instructionPointer]);
                instructionPointer = this[instructionRegistry];
                instructionPointer++;
            }
            Console.WriteLine(GetRegistryValues()[0]);
            Console.ReadLine();


        }

        private struct Instruction
        {
            public Operations Opcode { get; }
            public int[] Values { get; }

            public Instruction(Operations opcode, int[] values)
            {
                Opcode = opcode;
                Values = values;
            }

            public override string ToString()
            {
               return $"Opcode: {Opcode.ToString()}, values: [{Values[0]}, {Values[1]},{Values[2]}]";
            }
        }

        private void DumpRegistry()
        {
            Console.WriteLine($"A: {A}, B: {B}, C: {C}, D: {D}, E: {E}, F: {F}");
        }

        public void DecompiledProgram()
        {
            long a = 0;
            long c = 10551432;


            for (long b = 1; b <= c; b++)
            {
                if (c % b == 0) a += c / b;
            }

            Console.WriteLine(a);
            Console.ReadLine();

        }

        #region input
        /*
        #ip 5
        addi 5 16 5
        seti 1 8 3
        seti 1 1 1
        mulr 3 1 4
        eqrr 4 2 4
        addr 4 5 5
        addi 5 1 5
        addr 3 0 0
        addi 1 1 1
        gtrr 1 2 4
        addr 5 4 5
        seti 2 7 5
        addi 3 1 3
        gtrr 3 2 4
        addr 4 5 5
        seti 1 5 5
        mulr 5 5 5
        addi 2 2 2
        mulr 2 2 2
        mulr 5 2 2
        muli 2 11 2
        addi 4 8 4
        mulr 4 5 4
        addi 4 20 4
        addr 2 4 2
        addr 5 0 5
        seti 0 4 5
        setr 5 8 4
        mulr 4 5 4
        addr 5 4 4
        mulr 5 4 4
        muli 4 14 4
        mulr 4 5 4
        addr 2 4 2
        seti 0 7 0
        seti 0 9 5
        */
        #endregion


    }
}
