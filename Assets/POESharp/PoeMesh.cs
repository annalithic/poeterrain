using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POESharp {
    public class PoeMesh {
        public uint triCount;
        public uint vertCount;

        public int[] idx;
        public float[] x;
        public float[] y;
        public float[] z;
        public ushort[] u;
        public ushort[] v;
    }
}
