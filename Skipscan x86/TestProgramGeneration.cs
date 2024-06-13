using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Skipscan_x86
{
    public static class TestProgramGeneration
    {
        static string[] MultiFunctionalInstructions = new string[]
        {
            "mov",
            "sub",
            "call",
            "div",
            "jge",
            "jle",
            "ret",
            "loop",
            "enter",
            "leave"
        };

        static string[] InstructionsCoveredByMultifunctionals = new string[]
        {
            "jmp",
            "push",
            "pop",
            "dec",
            "test",
            "cmp",
            "jg",
            "je",
            "jl"
        };

        static Regex[] Patterns = new Regex[]
        {
            new Regex(@"mov+\s+%[a-zA-Z0-9]+,+\([%][a-zA-Z0-9]+,+[%][a-zA-Z0-9]+,*[1248]\)"),
            new Regex(@"sub+\s+%[a-zA-Z0-9]+,+%[a-zA-Z0-9]"),
            new Regex(@"call+\s+[a-z0-9]"),
            new Regex(@"div+\s+[a-z0-9]"),
            new Regex(@"ret"),
            new Regex(@"loop+\s+[a-z0-9]")
        };

        private static bool IsMultiFunctional(string instruction)
        {
            foreach (var multiFunctionalInstruction in MultiFunctionalInstructions)
            {
                if (instruction.Contains(multiFunctionalInstruction))
                    return true;
            }

            return false;
        }
        
        private static bool MatchesCriteria(string instruction)
        {
            foreach (var pattern in Patterns)
            {
                if (pattern.IsMatch(instruction))
                    return true;
            }

            return false;
        }

        private static bool IsCoveredByMultifunctionals(string instruction)
        {
            foreach (var covered in InstructionsCoveredByMultifunctionals)
            {
                if (instruction.Contains(covered))
                    return true;
            }

            return false;
        }

        public static void GenerateTestProgram()
        {
            var lines = File.ReadAllLines("results.txt");
            var newLines = String.Empty;
            newLines += "#vim: syntax=gas\n";
            newLines += ".globl _start\n";
            newLines += "\n.text\n";
            newLines += "\n_start:\n";

            foreach (var line in lines)
            {
                var instruction = line.Split(']', StringSplitOptions.RemoveEmptyEntries)[1];

                if (MatchesCriteria(instruction) && IsMultiFunctional(instruction))
                    newLines += "\t" + instruction + "\n";
                else if (!IsMultiFunctional(line) && !IsCoveredByMultifunctionals(instruction))
                    newLines += "\t" + instruction + "\n";
            }

            File.WriteAllText("newResults.txt", newLines);
            Process.Start("notepad.exe", "newResults.txt");
        }
    }
}
