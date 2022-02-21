using System;
using POESharp;
using System.IO;

namespace PoeSharpTests {
    class Program {
        static void Main(string[] args) {
            //Arm arm = new Arm(@"F:\Extracted\PathOfExile\3.17.Siege\Metadata\Terrain\Dungeon\Rooms\Unique\exit_2b.arm");
            //Console.WriteLine(arm.name);
            foreach(string path in Directory.EnumerateFiles(@"E:\Extracted\PathOfExile\3.17.Siege\Metadata\Terrain\", "*.*gr", SearchOption.AllDirectories)) {
                //Console.Write(Path.GetFileName(path));
                Graph g = new Graph(path);
                Console.WriteLine($"{g.version} {path}");
            }
        }
    }
}
