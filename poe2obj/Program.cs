using System;
using System.IO;
using POESharp;

namespace poe2obj {
    class Program {

        static string file1 = @"F:\Extracted\PathOfExile\3.20.Sanctum\ROOT\Art\Models\Terrain\Jungle\Doodads\Props\SunkenCityTransport\rig_a54fc14e.smd";
        static string file2 = @"F:\Extracted\PathOfExile\3.20.Sanctum\ROOT\Art\Models\MONSTERS\GargoyleGolem\rig_8aad72c3.smd";
        static string file3 = @"F:\Extracted\PathOfExile\3.20.Sanctum\ROOT\Art\Models\Terrain\Jungle\Tiles\VaalInterior\VaalGenerator\rig_e305e0b1.smd";

        static void Main(string[] args) {
            Smd2Obj(file1);
            Smd2Obj(file2);
            Smd2Obj(file3);

        }

        static void Smd2Obj(string path) {
            Smd smd = new Smd(path);
            Console.WriteLine(path);
            using (TextWriter w = new StreamWriter(Path.GetFileNameWithoutExtension(path) + ".obj")) {
                for (int vert = 0; vert < smd.x.Length; vert++) {
                    w.WriteLine($"v {smd.x[vert] / 100} {smd.z[vert] * -0.01} {smd.y[vert] * -0.01}");
                }
                for(int shape = 0; shape < smd.shapeStart.Length; shape++) {
                    w.WriteLine($"o {smd.shapeNames[shape]}");
                    for(uint i = smd.shapeStart[shape]; i < (smd.shapeStart[shape] + smd.shapeLength[shape]); i += 3) {
                        w.WriteLine($"f {smd.idx[i] + 1} {smd.idx[i + 1] + 1} {smd.idx[i + 2] + 1}");
                    }
                }
            }
        }
    }
}
