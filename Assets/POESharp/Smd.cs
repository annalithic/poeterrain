using System;
using System.IO;
using System.Text;

namespace POESharp {
    public class Smd {
        public ushort shapeCount;
        public uint triCount;
        public uint vertCount;

        public int[] idx;

        public float[] x;
        public float[] y;
        public float[] z;
        public ushort[] u;
        public ushort[] v;

        public Smd(string path) {
            using (BinaryReader r = new BinaryReader(File.OpenRead(path))) {
                byte version = r.ReadByte();

                if (version == 3) {
                    r.ReadByte();
                    shapeCount = r.ReadUInt16();
                    r.BaseStream.Seek(41, SeekOrigin.Current);
                    triCount = r.ReadUInt32();
                    vertCount = r.ReadUInt32();
                    r.BaseStream.Seek(8, SeekOrigin.Current);
                    r.BaseStream.Seek(8 * shapeCount, SeekOrigin.Current);
                } else {
                    triCount = r.ReadUInt32();
                    vertCount = r.ReadUInt32();
                    r.ReadByte();
                    shapeCount = r.ReadUInt16();
                    int shapeNameLength = r.ReadInt32();
                    r.BaseStream.Seek(24, SeekOrigin.Current); //bbox
                    r.BaseStream.Seek(8 * shapeCount, SeekOrigin.Current);
                    if (version == 2) r.BaseStream.Seek(4, SeekOrigin.Current);
                    r.BaseStream.Seek(shapeNameLength, SeekOrigin.Current);
                }




                idx = new int[triCount * 3];
                if (vertCount < 65535) {
                    for (int i = 0; i < idx.Length; i++) idx[i] = r.ReadUInt16();
                } else {
                    for (int i = 0; i < idx.Length; i++) idx[i] = r.ReadInt32();
                }

                x = new float[vertCount];
                y = new float[vertCount];
                z = new float[vertCount];
                u = new ushort[vertCount];
                v = new ushort[vertCount];
                for (int vert = 0; vert < vertCount; vert++) {
                    x[vert] = r.ReadSingle();
                    y[vert] = r.ReadSingle();
                    z[vert] = r.ReadSingle();
                    r.BaseStream.Seek(8, SeekOrigin.Current);
                    u[vert] = r.ReadUInt16();
                    v[vert] = r.ReadUInt16();
                    r.BaseStream.Seek(8, SeekOrigin.Current);
                }

            }
        }

        public string Print() {
            StringBuilder s = new StringBuilder();
            for(int i = 0; i < 20; i++) {
                if (i >= vertCount) break;
                s.Append($"{x[i]} {y[i]} {z[i]} - {u[i]} {v[i]}\n");
            }
            return s.ToString();
        }
    }
}
