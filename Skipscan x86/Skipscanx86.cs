using Skipscan_x86;
using System.Diagnostics;
using Iced.Intel;
using ScottPlot.Palettes;
using System.IO;

var generator = new Random();

Prefix.Initialize();

int GenerateLexicographic(ref GasFormatter format, int prefixLength, int instructionLength, int count, ref string text)
{
    var instructions = InstructionsGenerator.GenerateLexicographicWithRandomPrefixes(generator, prefixLength, instructionLength, count);
    var output = new StringOutput();
    var legal = 0;

    for (int i = 0; i < instructions.Count; ++i)
    {
        var bytes = instructions[i].FullInstructionBytes.ToByteArray();
        var code = new ByteArrayCodeReader(bytes);
        var decoded = Decoder.Create(64, code).Decode();

        if (decoded.IsInvalid)
            continue;
        
        format.Format(decoded, output);
        var mnemonic = output.ToStringAndReset();

        text += string.Format("[{0}] {1}\n", instructions[i].FullInstructionBytes, mnemonic);
        ++legal;
    }

    return legal;
}

int GenerateRandom(ref GasFormatter format, int prefixLength, int instructionLength, int count, ref string text)
{
    var legal = 0;
    var instructions = InstructionsGenerator.GenerateRandom(generator, prefixLength, instructionLength, count);
    var output = new StringOutput();

    for (int i = 0; i < instructions.Count; ++i)
    {
        var bytes = instructions[i].FullInstructionBytes.ToByteArray();
        var code = new ByteArrayCodeReader(bytes);
        var decoded = Decoder.Create(64, code).Decode();

        if (decoded.IsInvalid)
            continue;

        format.Format(decoded, output);
        var mnemonic = output.ToStringAndReset();

        text += string.Format("[{0}] {1}\n", instructions[i].FullInstructionBytes, mnemonic);
        ++legal;
    }

    return legal;
}

void TestLexicographic(string path)
{
    var count = 100000;
    var text = "";
    var format = new GasFormatter();
    var legal = 0;

    for (int j = 1; j <= 4; ++j)
    {
        for (int i = 1 + j; i <= 15; ++i)
            legal += GenerateLexicographic(ref format, j, i, count, ref text);
    }

    Console.Clear();
    Console.WriteLine("Legal instructions found: {0}", legal);
    File.WriteAllText(path, text);
    Process.Start("notepad.exe", path);
}

void TestRandom(string path)
{
    var count = 100000;
    var format = new GasFormatter();
    var legal = 0;

    var text = "";

    for (int j = 1; j <= 4; ++j)
    {
        for (int i = 1 + j; i <= 15; ++i)
            legal += GenerateRandom(ref format, j, i, count, ref text);
    }

    Console.Clear();
    Console.WriteLine("Legal instructions found: {0}", legal);
    File.WriteAllText(path, text);
    Process.Start("notepad.exe", path);
}

while (true)
{
    Console.Clear();
    Console.WriteLine("Skipscan_x86");
    Console.WriteLine("Choose test type: ");
    Console.WriteLine("[1] Depth First Search Test");
    Console.WriteLine("[2] Lexicographic generated test");
    Console.WriteLine("[3] Randomly generated test");
    Console.WriteLine("[4] Exit");

    var key = Console.ReadKey().Key;

    switch (key)
    {
        case ConsoleKey.D1:
            Console.Clear();
            Console.WriteLine("The test is being performed, wait for the results...");
            var tree = new InstructionTree();
            tree.GenerateTestSet(Syntax.GAS);
            Process.Start("notepad.exe", "results.txt");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            break;

        case ConsoleKey.D2:
            Console.Clear();
            Console.WriteLine("The test is being performed, wait for the results...");
            TestLexicographic("results.txt");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            break;

        case ConsoleKey.D3:
            Console.Clear();
            Console.WriteLine("The test is being performed, wait for the results...");
            TestRandom("results.txt");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            break;

        default:
            return;
    }
}