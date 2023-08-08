using System;
using POESharp;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PoeSharpTests {
    class Program {
        static void Main(string[] args) {

            Smd smd = new Smd(@"E:\Extracted\PathOfExile\3.21.Crucible\Art\Models\MONSTERS\Anchorman\Anchorman_armour_c18cc675.smd");

            Console.WriteLine("test"); return;


            foreach(string matPath in Directory.EnumerateFiles(@"F:\Extracted\PathOfExile\ZZZZZZZZZZZZZZZZZZ3.18.Sentinel\Art\Models\Monsters", "*.mat", SearchOption.AllDirectories)) {
                Console.WriteLine(matPath.Substring(57));
                string text = File.ReadAllText(matPath, System.Text.Encoding.Unicode);
                JObject mat = JObject.Parse(text);
                if(mat["graphinstances"] != null)
                foreach (var instance in mat["graphinstances"]) {
                    Console.Write(Path.GetFileNameWithoutExtension(instance["parent"].Value<string>()) + " ");
                }
                    
                Console.WriteLine('\n');

            }


            //Arm arm = new Arm(@"F:\Extracted\PathOfExile\3.17.Siege\Metadata\Terrain\Dungeon\Rooms\Unique\exit_2b.arm");
            //Console.WriteLine(arm.name);
            //foreach(string path in Directory.EnumerateFiles(@"E:\Extracted\PathOfExile\3.17.Siege\Metadata\Terrain\", "*.*gr", SearchOption.AllDirectories)) {
            //Console.Write(Path.GetFileName(path));
            //    Graph g = new Graph(path);
            //    Console.WriteLine($"{g.version} {path}");
            //}

            //Smd smd = new Smd(@"E:\Extracted\PathOfExile\3.18.Sentinel\Art\Models\MONSTERS\GoddessOfMalaise\rig_5529a689.smd");
            //Console.WriteLine(smd.Print());

            /*
            int debug = 0;
            string folder = @"E:\Extracted\PathOfExile\3.18.Sentinel\";
            foreach (string path in Directory.EnumerateFiles(folder, "*.smd", SearchOption.AllDirectories)) {
                string relative = path.Substring(folder.Length);
                string meshPath = Path.Combine(relative.Replace(".smd", ".mesh"));
                Console.WriteLine(meshPath);
                debug++; if (debug > 100) break;
            }
            
            
            foreach(string path in Directory.EnumerateFiles(@"E:\Extracted\PathOfExile\3.18.Sentinel\Art\Models\MONSTERS\", "*.fmt", SearchOption.AllDirectories)) {
                Fmt smd = new Fmt(path);
                if(smd.vertCount > 0) {
                    Console.WriteLine(path);
                    Console.WriteLine(smd.unk);
                    Console.WriteLine(smd.Print());
                }

            }
            */
        }

        static void CopyTextures(string sourcefolder, string destfolder) {

        }
    }


}
