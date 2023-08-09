﻿using System.IO;
using System.Collections.Generic;
using System.Text;
using System;

namespace POESharp.Util {

    public static class IOUtil {
        public static void Seek(this BinaryReader r, int count) { r.BaseStream.Seek(count, SeekOrigin.Current); }

        public static float[] ReadBBox(this BinaryReader r) {
            float[] bbox = new float[6];
            for (int i = 0; i < bbox.Length; i++) bbox[i] = r.ReadSingle();
            return bbox;
        }
    }

    public static class TextReaderEx {
        public static int ReadLineInt(this TextReader r) {
            return int.Parse(r.ReadLine());
        }
        public static string ReadLineString(this TextReader r) {
            return r.ReadLine().Trim('\"');
        }

        public static int ReadValueInt(this TextReader r, string assertVal) {
            string[] words = r.ReadLine().Trim().Split();
            if (words[0] != assertVal)
                Console.WriteLine($"ASSERTION FAILED [{words[0]}] != [{assertVal}]");
            return int.Parse(words[words.Length - 1]);
        }


        public static void ReadLineInt(this TextReader r, out int a, out int b) {
            string[] words = r.ReadLine().Split(' ');
            a = int.Parse(words[0]); b = int.Parse(words[1]);
        }

        public static string ReadValueString(this TextReader r, string assertVal) {
            var words = r.ReadLine().Trim().SplitQuotes();
            if (words[0] != assertVal) Console.WriteLine($"ASSERTION FAILED {words[0]} != {assertVal}");
            return words[words.Length - 1].Trim('"');
        }

        public static float[] ReadValueBbox(this TextReader r, string assertVal) {
            var words = r.ReadLine().Trim().Split();
            if (words[0] != assertVal) Console.WriteLine($"ASSERTION FAILED {words[0]} != {assertVal}");
            float[] bbox = new float[6];
            for (int i = 0; i < bbox.Length; i++) bbox[i] = float.Parse(words[i + 1]);
            return bbox;
        }




        public static string[] SplitQuotes(this string s) {
            List<string> newWords = new List<string>();
            string[] words = s.Split(' ');
            for(int i = 0; i < words.Length; i++) {
                if (words[i][0] == '"' && words[i][words[i].Length - 1] != '"') {
                    string combined = words[i];

                    do {
                        i++;
                        combined += words[i];
                    } while (words[i][words[i].Length - 1] != '"');
                    newWords.Add(combined);
                } else newWords.Add(words[i]);
            }
            return newWords.ToArray();
        }
    }
    public class WordReader {

        int pos;
        string[] words;

        public WordReader(string line, int start = 0) { words = line.Split(' '); pos = start; }

        public WordReader(string[] words, int start = 0) { this.words = words; pos = start; }

        public void Read(string line, int start = 0) { pos = start; words = line.Split(' '); }

        public void Skip(int count = 1) { pos += count; }

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

        public char ReadChar() {
            if (pos >= words.Length) return '0';
            pos++;
            return words[pos-1][0];
        }

        public int[] ReadIntArray() {
            int[] array = new int[ReadInt()];
            for (int i = 0; i < array.Length; i++) array[i] = ReadInt();
            return array;
        }

        public string[] ReadStringArray(bool inQuotes = true) {
            string[] array = new string[ReadInt()];
            for (int i = 0; i < array.Length; i++) array[i] = ReadString(inQuotes);
            return array;
        }

    }
}
