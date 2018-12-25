using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Priority_Queue;
using System.Text.RegularExpressions;

namespace ConsoleApp5
{
    class Program
    {

        static void Main(string[] args)
        {
            string input = @"C:\Users\Vojta\Documents\input.txt";
            Engine engine = new Engine();
            engine.ParseInput(input);
            engine.FindSmallestBoost();

            Console.WriteLine("Finished...");
            Console.ReadLine();
        }

    }

    class Group
    {
        public int Id { get; set; }
        public enum Factions { Immune, Infection}
        public enum Damage { bludgeoning = 1, cold = 2, radiation = 4, slashing = 8, fire = 16}
        public int Units { get; set;}
        public int Hitpoints { get; set; }

        public Factions Faction { get; set; }
        public int Initiative { get; set; }

        public int AttackDamage { get; set; }
        public Damage AttackType { get; set; }

        public List<Damage> WeakAgainst { get; set; } = new List<Damage>();
        public List<Damage> ImmuneAgainst { get; set; } = new List<Damage>();

        public bool isDead { get { return Units <= 0; } }

        public int TakesDamageFromGroup(Group group)
        {
            if (ImmuneAgainst.Contains(group.AttackType)) return 0;
            else if (WeakAgainst.Contains(group.AttackType)) return group.EffectiveDamage * 2;
            else return group.EffectiveDamage;
        }

        public int EffectiveDamage { get { return AttackDamage * Units; } }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(Factions), Faction)} Group {Id}";
        }
    }

    public class Engine
    {
        List<Group> Groups = new List<Group>();
        public void ParseInput(string input)
        {
            StreamReader file = new StreamReader(input);
            string line = file.ReadLine();
            int id = 1;

            // Immune system
            while (true)
            {
                line = file.ReadLine();
                if (line == "") break;
                ParseGroupFromLine(line);
                var group = ParseGroupFromLine(line);
                group.Faction = Group.Factions.Immune;
                group.Id = id;
                id++;
                Groups.Add(group);
            }
            file.ReadLine();
            id = 1;
            while (!file.EndOfStream)
            {
                line = file.ReadLine();
                var group = ParseGroupFromLine(line);
                group.Faction = Group.Factions.Infection;
                group.Id = id;
                id++;
                Groups.Add(group);
            }
        }

        private Group ParseGroupFromLine(string line)
        {
            Group group = new Group();
            var words = line.Split(' ');

            group.Initiative = Int32.Parse(words[words.Length - 1]);
            group.Units = Int32.Parse(words[0]);
            group.Hitpoints= Int32.Parse(words[4]);

            Regex attackRegex = new Regex(@"(?<=does )\d+");
            group.AttackDamage = Int32.Parse(attackRegex.Match(line).ToString());

            for (int i = 0; i < words.Count() - 1; i++)
            {
                if (words[i + 1] == "damage")
                {
                    Group.Damage type;
                    Enum.TryParse(words[i], out type);
                    group.AttackType = type;
                    break;
                }
            }

            Regex parenthesesContent = new Regex(@"\((.+)\)");
            if (parenthesesContent.Match(line).Captures.Count > 0)
            {
                string parentheses = parenthesesContent.Match(line).ToString().Replace(")", String.Empty).Replace("(",String.Empty).Replace(",", String.Empty);

                if (parentheses.Contains(";"))
                {
                    if (parentheses.Split(' ')[0] == "immune")
                    {
                        //immune
                        foreach (var word in parentheses.Split(';')[0].Split(' '))
                        {
                            Group.Damage type;
                            if (Enum.TryParse(word, out type))
                            {
                                group.ImmuneAgainst.Add(type);
                            }
                        }

                        // weak
                        foreach (var word in parentheses.Split(';')[1].Split(' '))
                        {
                            Group.Damage type;
                            if (Enum.TryParse(word, out type))
                            {
                                group.WeakAgainst.Add(type);
                            }
                        }
                    }
                    else
                    {
                    // immune
                    foreach (var word in parentheses.Split(';')[1].Split(' '))
                        {
                            Group.Damage type;
                            if (Enum.TryParse(word, out type))
                            {
                                group.ImmuneAgainst.Add(type);
                            }
                        }

                        // weak
                        foreach (var word in parentheses.Split(';')[0].Split(' '))
                        {
                            Group.Damage type;
                            if (Enum.TryParse(word, out type))
                            {
                                group.WeakAgainst.Add(type);
                            }
                        }
                    }
                }
                else if (parentheses.Contains("weak")) // only weak
                {
                    foreach (var word in parentheses.Split(' '))
                    {
                        Group.Damage type;
                        if (Enum.TryParse(word, out type))
                        {
                            group.WeakAgainst.Add(type);
                        }
                    }
                }
                else //only immune
                {
                    foreach (var word in parentheses.Split(' '))
                    {
                        Group.Damage type;
                        if (Enum.TryParse(word, out type))
                        {
                            group.ImmuneAgainst.Add(type);
                        }
                    }
                }

            }

            return group;

        }

        public void DoTheBattle()
        {

            while (true)
            {
                List<Group> Infections = Groups.Where(x => x.Faction == Group.Factions.Infection && !x.isDead).ToList();
                List<Group> Immunes = Groups.Where(x => x.Faction == Group.Factions.Immune && !x.isDead).ToList();

                List<(Group, Group)> WhosAttackingWho = new List<(Group, Group)>();

                foreach (var group in Groups.OrderByDescending(x => x.EffectiveDamage).ThenByDescending(x => x.Initiative))
                {
                    //if (!group.isDead) Console.WriteLine($"{group.ToString()} contains {group.Units} units.");

                    //Console.WriteLine($"Now choosing {group.ToString()}");
                    if (group.Faction == Group.Factions.Immune && Infections.Any())
                    {
                        var target = Infections.OrderByDescending(x => x.TakesDamageFromGroup(group)).ThenByDescending(x => x.EffectiveDamage).ThenByDescending(x => x.Initiative).First();
                        if (target.TakesDamageFromGroup(group) == 0) continue;
                        WhosAttackingWho.Add((group, target));
                        Infections.Remove(target);
                    }
                    else if (Immunes.Any())// Infections
                    {
                        var target = Immunes.OrderByDescending(x => x.TakesDamageFromGroup(group)).ThenByDescending(x => x.EffectiveDamage).ThenByDescending(x => x.Initiative).First();
                        if (target.TakesDamageFromGroup(group) == 0) continue;
                        WhosAttackingWho.Add((group, target));
                        Immunes.Remove(target);
                    }
                }

                foreach (var attackingDefending in WhosAttackingWho.OrderByDescending(x => x.Item1.Initiative))
                {
                    if (attackingDefending.Item1.isDead) continue;
                    int previousUnits = attackingDefending.Item2.Units;
                    int damageDealt = attackingDefending.Item2.TakesDamageFromGroup(attackingDefending.Item1);
                    attackingDefending.Item2.Units -= damageDealt / attackingDefending.Item2.Hitpoints;

                    //Console.WriteLine($"Unit {attackingDefending.Item1.ToString()} is dealing {damageDealt} to {attackingDefending.Item2.ToString()} killing {previousUnits - attackingDefending.Item2.Units}");

                }

                if (Groups.Where(x => x.Faction == Group.Factions.Immune).All(x => x.isDead) || Groups.Where(x => x.Faction == Group.Factions.Infection).All(x => x.isDead))
                {
                    Console.WriteLine($"Infections have left: {Groups.Where(x => x.Faction == Group.Factions.Infection && !x.isDead).Sum(x => x.Units)}");
                    Console.WriteLine($"Immunes have left: {Groups.Where(x => x.Faction == Group.Factions.Immune && !x.isDead).Sum(x => x.Units)}");
                    return;
                }

            
            }
        }

        public void FindSmallestBoost()
        {
            int boost = 77;
            foreach (var group in Groups.Where(x => x.Faction == Group.Factions.Immune))
            {
                group.AttackDamage += boost;
            }
            DoTheBattle();
            //831 too high

        }
    }



}