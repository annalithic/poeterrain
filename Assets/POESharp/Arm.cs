using System.IO;
using POESharp.Util;
namespace POESharp {
    public class Arm {
        public int version;

        public string[] entries;

        public int size1x; public int size1y;
        public int apax; public int apay;
        public string name;
        public int bepax; public int bepay;

        public KEntry kEntry;

        public string[] apaEntries;
        public string[] unkEntALines;
        public string[] unkEntBLines;
        public string[] unkEntCLines;
        public string[] unkEntDLines;
        public string[] unkEntELines;
        public string[] unkEntFLines;

        //public string[] kLines;
        public KEntry[,] kEntries;

        public Doodad[] doodads;



        public Arm(string path) {
            using (TextReader r = new StreamReader(File.OpenRead(path))) {
                version = int.Parse(r.ReadLine().Substring(8));
                entries = new string[r.ReadLineInt()];
                for (int i = 0; i < entries.Length; i++) entries[i] = r.ReadLineString();
                r.ReadLineInt(out size1x, out size1y);
                r.ReadLineInt(out apax, out apay);
                name = r.ReadLineString();
                r.ReadLineInt(out bepax, out bepay);
                kEntry = new KEntry(new WordReader(r.ReadLine().Split(' ')));
                apaEntries = new string[(apax + apay) * 2];
                for (int i = 0; i < apaEntries.Length; i++) apaEntries[i] = r.ReadLine();

                unkEntALines = new string[r.ReadLineInt()]; for (int i = 0; i < unkEntALines.Length; i++) unkEntALines[i] = r.ReadLine();
                unkEntBLines = new string[r.ReadLineInt()]; for (int i = 0; i < unkEntBLines.Length; i++) unkEntBLines[i] = r.ReadLine();
                unkEntCLines = new string[r.ReadLineInt()]; for (int i = 0; i < unkEntCLines.Length; i++) unkEntCLines[i] = r.ReadLine();
                unkEntDLines = new string[r.ReadLineInt()]; for (int i = 0; i < unkEntDLines.Length; i++) unkEntDLines[i] = r.ReadLine();
                unkEntELines = new string[r.ReadLineInt()]; for (int i = 0; i < unkEntELines.Length; i++) unkEntELines[i] = r.ReadLine();
                unkEntFLines = new string[r.ReadLineInt()]; for (int i = 0; i < unkEntFLines.Length; i++) unkEntFLines[i] = r.ReadLine();

                kEntries = new KEntry[kEntry.sizeX, kEntry.sizeY];
                for (int y = 0; y < kEntry.sizeY; y++) {
                    WordReader w = new WordReader(r.ReadLine().Split(' '));
                    for(int x = 0; x < kEntry.sizeX; x++) {
                        kEntries[x, y] = new KEntry(w);
                    }
                }

                doodads = new Doodad[r.ReadLineInt()]; for (int i = 0; i < doodads.Length; i++) doodads[i] = new Doodad(r.ReadLine().Split(' '));
            }
        }

        public class KEntry {
            public enum Type {
                k,
                s,
                n,
                o,
                f

            }

            public Type type;

            public int sizeX;
            public int sizeY;
            public int edgeTypeDown;
            public int edgeTypeRight;
            public int edgeTypeUp;
            public int edgeTypeLeft;
            public int edgeLengthDown;
            public int unk2;
            public int edgeLengthRight;
            public int unk4;
            public int edgeLengthUp;
            public int unk6;
            public int edgeLengthLeft;
            public int unk8;
            public int groundTypeDownLeft;
            public int groundTypeDownRight;
            public int groundTypeUpRight;
            public int groundTypeUpLeft;
            public int unk9;
            public int unk10;
            public int unk11;
            public int unk12;
            public int feature;
            public int origin;



            //public int[] values;

            public KEntry(WordReader r) {
                type = (Type)System.Enum.Parse(typeof(Type), r.ReadString(false));
                if (type == Type.k) {
                    sizeX = r.ReadInt();
                    sizeY = r.ReadInt();
                    edgeTypeDown = r.ReadInt();
                    edgeTypeRight = r.ReadInt();
                    edgeTypeUp = r.ReadInt();
                    edgeTypeLeft = r.ReadInt();
                    edgeLengthDown = r.ReadInt();
                    unk2 = r.ReadInt();
                    edgeLengthRight = r.ReadInt();
                    unk4 = r.ReadInt();
                    edgeLengthUp = r.ReadInt();
                    unk6 = r.ReadInt();
                    edgeLengthLeft = r.ReadInt();
                    unk8 = r.ReadInt();
                    
                    groundTypeDownLeft = r.ReadInt();
                    groundTypeDownRight = r.ReadInt();
                    groundTypeUpRight = r.ReadInt();
                    groundTypeUpLeft = r.ReadInt();
                    unk9 = r.ReadInt();
                    unk10 = r.ReadInt();
                    unk11 = r.ReadInt();
                    unk12 = r.ReadInt();
                    feature = r.ReadInt();
                    origin = r.ReadInt();
                    //values = new int[24];
                    //for (int i = 0; i < values.Length; i++) values[i] = r.ReadInt();
                } else if (type == Type.f) {
                    feature = r.ReadInt();
                }
            }

        }



        public class Doodad {
            public int x;
            public int y;
            public float z;
            public float unk1;
            public float unk2;
            public float unk3;
            public float unk4;
            public int unk5;
            public int unk6;
            public float unk7;
            public float scale;
            public string artFile;
            public string objectFile;

            public Doodad(string[] words) {
                if (words.Length < 13) return;
                WordReader reader = new WordReader(words);
                x = reader.ReadInt();
                y = reader.ReadInt();
                z = reader.ReadFloat();
                unk1 = reader.ReadFloat();
                unk2 = reader.ReadFloat();
                unk3 = reader.ReadFloat();
                unk4 = reader.ReadFloat();
                unk5 = reader.ReadInt();
                unk6 = reader.ReadInt();
                if (reader.ReadInt() == 1) unk7 = reader.ReadFloat();
                scale = reader.ReadFloat();
                artFile = reader.ReadString();
                objectFile = reader.ReadString();
            }
        }
    }
}