using System.IO;

namespace POESharp.Util {
    public static class TextReaderEx {
        public static int ReadLineInt(this TextReader r) {
            return int.Parse(r.ReadLine());
        }
        public static string ReadLineString(this TextReader r) {
            return r.ReadLine().Trim('\"');
        }

        public static void ReadLineInt(this TextReader r, out int a, out int b) {
            string[] words = r.ReadLine().Split(' ');
            a = int.Parse(words[0]); b = int.Parse(words[1]);
        }
    }
    public class WordReader {

        int pos;
        string[] words;

        public WordReader(string[] words) { this.words = words; }

        public int ReadInt() {
            if (pos >= words.Length) return int.MinValue;
            pos++;
            return int.Parse(words[pos - 1]);
        }

        public float ReadFloat() {
            if (pos >= words.Length) return float.MinValue;
            pos++;
            return float.Parse(words[pos - 1]);
        }

        public string ReadString(bool inQuotes = true) {
            if (pos >= words.Length) return "";
            pos++;
            return words[pos - 1].Trim('\"');
        }

    }
}
