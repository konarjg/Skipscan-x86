using Iced.Intel;

namespace Skipscan_x86
{
    public enum Syntax
    {
        GAS,
        INTEL,
        NASM,
        MASM
    }

    public class InstructionTree
    {
        public List<byte[]> Layers;

        public InstructionTree() 
        {
            Layers = new List<byte[]>();

            for (int i = 0; i < 15; ++i)
            {
                var bytes = new byte[256];

                for (int j = 0; j < 256; ++j)
                    bytes[j] = (byte)j;

                Layers.Add(bytes);
            }
        }

        private ByteWord GetBytes(List<string> path)
        {
            var bytes = "0x";

            for (int i = 0; i < path.Count; ++i)
                bytes += path[i];

            return ByteWord.FromHex(bytes);
        }

        private (string, int) DecodeInstruction(ByteWord bytes, Syntax syntax)
        {
            var code = new ByteArrayCodeReader(bytes.ToByteArray());
            var decoder = Decoder.Create(64, code);
            var instruction = decoder.Decode();

            if (instruction.IsInvalid)
            {
                if (bytes.Length() > 15)
                    return ("(bad)", 1);

                var newBytes = bytes.ToString();
                newBytes += "00";
                return DecodeInstruction(ByteWord.FromHex(newBytes), syntax);
            }

            dynamic format = default;

            switch (syntax)
            {
                case Syntax.GAS:
                    format = new GasFormatter();
                    break;

                case Syntax.INTEL:
                    format = new IntelFormatter();
                    break;

                case Syntax.NASM:
                    format = new NasmFormatter();
                    break;

                case Syntax.MASM:
                    format = new MasmFormatter();
                    break;
            }

            var output = new StringOutput();

            format.Format(ref instruction, output);
            var mnemonic = output.ToStringAndReset();

            return (mnemonic, instruction.Length);
        }

        private void SaveMnemonic(ref string text, ByteWord bytes, string mnemonic)
        {
            if (!mnemonic.Contains("(bad)"))
                text += string.Format("[{0}] {1}\n", bytes, mnemonic);
        }

        private bool TestValidityAndIncreaseDepth(ref List<string> path, ref int[] index, ref string mnemonic, ref int depth)
        {
            if (mnemonic.Contains("(bad)"))
            {
                ++depth;

                if (depth == 15)
                    return false;

                index[depth] = 1;
                path.Add(Layers[depth][0].Hex());

                return false;
            }

            return true;
        }

        private bool TestLengthAndIncreaseDepth(ref int[] index, ref List<string> path, ref int depth, ref int length, ref int prevLen, ref string text, ref ByteWord bytes, ref string mnemonic)
        {
            if (depth + 1 < length && length > prevLen)
            {
                SaveMnemonic(ref text, bytes, mnemonic);

                prevLen = length;
                ++depth;
                index[depth] = 1;
                path.Add(Layers[depth][0].Hex());
                return false;
            }

            return true;
        }

        private bool TestTriggerSignalAndDecreaseDepth(ref List<string> path, ref int depth, ref int[] index)
        {
            if (path[depth] == "ff" && depth > 0)
            {
                path.RemoveAt(depth);

                for (int i = depth; i < 15; ++i)
                    index[i] = 0;

                --depth;

                return true;
            }

            return false;
        }

        private bool TestFeatureValueAndSetDepthByteToFF(ref List<string> path, ref string node, ref int depth, ref string text, ref ByteWord bytes, ref string mnemonic)
        {
            if ((depth + 1 > 3 && node == "01") 
                || (depth + 1 > 4 && node == "02"))
            {
                SaveMnemonic(ref text, bytes, mnemonic);

                path.RemoveAt(depth);
                path.Insert(depth, "ff");
                return true;
            }

            return false;
        }

        public void DFS(Syntax syntax, (int, int) start, int depth, ref string text)
        {
            var node = Layers[start.Item1][start.Item2].Hex();
            var path = new List<string>();

            path.Add(node);
            var index = new int[15];
            index[0] = start.Item2;

            int prevLen = 1;

            path.Add(Layers[0][0].Hex());

            while (depth < 15 && depth >= 0)
            {
                var bytes = GetBytes(path);
                var decoded = DecodeInstruction(bytes, syntax);
                var mnemonic = decoded.Item1;
                var length = decoded.Item2;

                if (!TestValidityAndIncreaseDepth(ref path, ref index, ref mnemonic, ref depth)
                    || !TestLengthAndIncreaseDepth(ref index, ref path, ref depth, ref length, ref prevLen, ref text, ref bytes, ref mnemonic)
                    || TestTriggerSignalAndDecreaseDepth(ref path, ref depth, ref index)
                    || TestFeatureValueAndSetDepthByteToFF(ref path, ref node, ref depth, ref text, ref bytes, ref mnemonic)
                    )
                    continue;

                SaveMnemonic(ref text, bytes, mnemonic);

                if (index[depth] > 255)
                {
                    index[depth] = 255;
                    ++depth;
                    continue;
                }

                if (depth == 15)
                    return;

                node = Layers[depth][index[depth]].Hex();
                path.RemoveAt(depth);
                path.Insert(depth, node);
                ++index[depth];
            }
        }

        public void GenerateTestSet(Syntax syntax)
        {
            var format = new GasFormatter();
            var output = new StringOutput();
            var text = "";

            for (int i = 0; i < 256; ++i)
                DFS(syntax, (0, i), 0, ref text);

            Console.Clear();
            Console.WriteLine("Legal instructions found: {0}", text.Count(x => x == '\n'));
            File.WriteAllText("results.txt", text);
        }
    }
}
