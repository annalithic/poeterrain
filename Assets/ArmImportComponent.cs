using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POESharp;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class ArmImportComponent : MonoBehaviour {
    public string path;
    public GameObject doodadPrefab;
    public GameObject tilePrefab;
    public GameObject warpPrefab;
    public int size;
    public bool import = false;

    public Material matPrefab;
    public Material invisMat;
    public bool createMat = false;
  

    // Update is called once per frame
    void Update()
    {
        
        if(import) {
            path = path.Trim('\"');
            import = false;
            Import(path);
            //Import(@"F:\Extracted\PathOfExile\3.17.Siege\Camp\Rooms\camp.arm");
        }
    }

    void CreateMaterials(Arm arm) {
        Material[] mats = new Material[arm.entries.Length];
        for (int i = 0; i < mats.Length; i++) {
            Material mat = Resources.Load<Material>(arm.entries[i].Replace('/', '_'));
            if (mat != null) {
                mats[i] = mat;
            } else {
                mat = Instantiate(matPrefab);
                mat.color = Random.ColorHSV(0, 1, 0.3f, 0.6f, 0.2f, 0.8f);
                AssetDatabase.CreateAsset(mat, $"Assets/Resources/{arm.entries[i].Replace('/', '_')}.mat");
            }
        }
    }

    void Import(string path) {
        if (!path.EndsWith(".arm")) {
            List<string> arms = new List<string>();
            foreach(string arm in Directory.EnumerateFiles(path, "*.arm", SearchOption.AllDirectories)) {
                arms.Add(arm);
            }
            arms.Sort();
            
            int offset = 0;
            foreach(string arm in arms) {
                Debug.Log(arm.Substring(path.Length + 1));
                offset += ImportRoom(arm, new Vector3(offset, 0, 0), arm.Substring(path.Length + 1).Replace('\\', '_')) + 3;
            }
        } else {
            ImportRoom(path);
        }
    }

    void ImportRoom(string path) { ImportRoom(path, Vector3.zero); }
    int ImportRoom(string path, Vector3 offset, string name = "") {


        Arm arm = new Arm(path);

        CreateMaterials(arm);

        Material[] mats = new Material[arm.entries.Length + 1];
        mats[0] = matPrefab;
        for(int i = 1; i < mats.Length; i++) {
            Material mat = Resources.Load<Material>(arm.entries[i - 1].Replace('/', '_'));
            if (mat != null) {
                mats[i] = mat;
            } else Debug.Log("ERROR " + arm.entries[i - 1]);
        }

        if (name == "") name = Path.GetFileNameWithoutExtension(path);
        Transform root = new GameObject(name + (arm.name != "" ? " (" + arm.name + ")" : "")).transform;
        root.position = offset;
        root.gameObject.AddComponent<ArmComponent>().SetData(arm);


        Transform tileRoot = new GameObject("tiles").transform;
        tileRoot.SetParent(root, false);

        for(int y = 0; y < arm.kEntries.GetLength(1); y++) {
            for (int x = 0; x < arm.kEntries.GetLength(0); x++) {
                if(arm.kEntries[x,y].type == Arm.KEntry.Type.k) {
                    Arm.KEntry e = arm.kEntries[x, y];
                    Transform obj = Instantiate(tilePrefab, tileRoot).transform;
                    obj.localPosition = new Vector3(x* size, 0, y* size);
                    if (e.origin == 1 || e.origin == 2) obj.localPosition += new Vector3((e.sizeX - 1) * size * -1, 0, 0);
                    if (e.origin == 2 || e.origin == 3) obj.localPosition += new Vector3(0, 0, (e.sizeY - 1) * size * -1);

                    Mesh m = Resources.Load<Mesh>(e.MeshDescription());
                    if(m == null) {
                        m = CreateMesh(e);
                        AssetDatabase.CreateAsset(m, "Assets/Resources/" + e.MeshDescription() + ".mesh");
                    }
                    
                    obj.GetComponent<MeshFilter>().sharedMesh = m;

                    
                    MeshRenderer r = obj.GetComponent<MeshRenderer>();

                    Material featureMat = invisMat;
                    if (e.feature != 0) {
                        obj.name = mats[e.feature].name;
                        featureMat = mats[e.feature];
                    }

                    Material[] newSharedMats = new Material[] {
                        mats[e.groundTypeDownLeft], mats[e.groundTypeDownRight], mats[e.groundTypeUpRight], mats[e.groundTypeUpLeft],
                        mats[e.edgeTypeDown], mats[e.edgeTypeRight], mats[e.edgeTypeUp], mats[e.edgeTypeLeft], featureMat,
                    };
                    if (newSharedMats[4] == mats[0]) newSharedMats[4] = newSharedMats[0];
                    if (newSharedMats[5] == mats[0]) newSharedMats[5] = newSharedMats[1];
                    if (newSharedMats[6] == mats[0]) newSharedMats[6] = newSharedMats[2];
                    if (newSharedMats[7] == mats[0]) newSharedMats[7] = newSharedMats[3];

                    r.sharedMaterials = newSharedMats;
                    /*
                    

                    Material[] newSharedMats = new Material[] {
                        mats[e.groundTypeDownRight], mats[e.groundTypeUpLeft],mats[e.groundTypeDownLeft],mats[e.groundTypeUpRight],
                        mats[e.edgeTypeLeft], mats[e.edgeTypeRight],mats[e.edgeTypeUp],mats[e.edgeTypeDown],
                        roomMat
                    };

                    if (newSharedMats[4] == mats[0]) newSharedMats[4] = newSharedMats[1];
                    if (newSharedMats[5] == mats[0]) newSharedMats[5] = newSharedMats[0];
                    if (newSharedMats[6] == mats[0]) newSharedMats[6] = newSharedMats[3];
                    if (newSharedMats[7] == mats[0]) newSharedMats[7] = newSharedMats[2];


                    */


                    //obj.localScale = new Vector3(size * e.sizeX, 1, size * e.sizeY);
                    ArmKEntryComponent kc = obj.GetComponent<ArmKEntryComponent>();
                    kc.SetData(e, arm.entries);
                    //string[] entries = new string[e.values.Length];
                    //for(int i = 0; i < entries.Length; i++) {
                    //    entries[i] = (i > 1 && e.values[i] - 1 > 0 && e.values[i] - 1 < arm.entries.Length) ? arm.entries[e.values[i] - 1] : (e.values[i]).ToString();
                    //}
                    //kc.values = e.values;
                    //kc.valuesEntriesTEMP = entries;
                }
                else if(arm.kEntries[x, y].type == Arm.KEntry.Type.s || arm.kEntries[x, y].type == Arm.KEntry.Type.f) {
                    Arm.KEntry e = arm.kEntries[x, y];
                    Transform obj = Instantiate(tilePrefab, tileRoot).transform;
                    obj.localPosition = new Vector3(x * size, 0, y * size);
                    obj.localScale = Vector3.one * 3;
                    obj.name = arm.kEntries[x, y].type.ToString();
                    if (e.origin == 1 || e.origin == 2) obj.localPosition += new Vector3((e.sizeX - 1) * size * -1, 0, 0);
                    if (e.origin == 2 || e.origin == 3) obj.localPosition += new Vector3(0, 0, (e.sizeY - 1) * size * -1);
                }
            }
        }
        
        Transform doodadRoot = new GameObject("doodads").transform;
        doodadRoot.SetParent(root, false);
            
        /*
        foreach (Arm.Doodad doodad in arm.doodads) {
            Transform obj = Instantiate(doodadPrefab, doodadRoot).transform;
            obj.localPosition = new Vector3(doodad.x, doodad.z, doodad.y);
            obj.name = doodad.artFile;
        }
        
        //warp points
        foreach(string line in arm.unkEntDLines) {
            POESharp.Util.WordReader r = new POESharp.Util.WordReader(line.Split(' '));
            int x = r.ReadInt(); int y = r.ReadInt(); float z = r.ReadFloat(); string name = r.ReadString();
            Transform warp = Instantiate(warpPrefab, root).transform;
            warp.localPosition = new Vector3(x, 3, y);
            warp.name = name;
        }
        */
        return arm.kEntry.sizeX * 3;
    }

    Mesh CreateMesh(Arm.KEntry e) {
        float x = e.sizeX * 3;
        float y = e.sizeY * 3;
        
        float edgeDown = e.edgeLengthDown == x ? x / 2 - 0.5f : e.edgeLengthDown;
        float edgeRight = e.edgeLengthRight == y ? y / 2 - 0.5f : e.edgeLengthRight;
        float edgeUp = e.edgeLengthUp == x ? x / 2 - 0.5f : e.edgeLengthUp;
        float edgeLeft = e.edgeLengthLeft == y ? y / 2 - 0.5f : e.edgeLengthLeft;

        float midX = (edgeDown + x - edgeUp) / 2f;
        if (e.edgeLengthDown == x) midX = x - edgeUp - 0.5f;
        else if (e.edgeLengthUp == x) midX = edgeDown + 0.5f;

        float midY = (edgeRight + y - edgeLeft) / 2f;
        if (e.edgeLengthRight == y) midY = y - edgeLeft - 0.5f;
        else if (e.edgeLengthLeft == y) midY = edgeRight + 0.5f;

        Vector3[] verts = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(edgeDown, 0, 0),
            new Vector3(midX - 0.5f, 0, midY - 0.5f),
            new Vector3(0, 0, y - edgeLeft - 1),

            new Vector3(x, 0, 0),
            new Vector3(x, 0, edgeRight),
            new Vector3(midX + 0.5f, 0, midY - 0.5f),
            new Vector3(edgeDown + 1, 0, 0),

            new Vector3(x, 0, y),
            new Vector3(x - edgeUp, 0, y),
            new Vector3(midX + 0.5f, 0, midY + 0.5f),
            new Vector3(x, 0, edgeRight + 1),

            new Vector3(0, 0, y),
            new Vector3(0, 0, y - edgeLeft),
            new Vector3(midX - 0.5f, 0, midY + 0.5f),
            new Vector3(x - edgeUp - 1, 0, y),

            new Vector3(midX, 0, midY),

            new Vector3(0.5f, 0.01f, 0.5f),
            new Vector3(x - 0.5f, 0.01f, 0.5f),
            new Vector3(x - 0.5f, 0.01f, y -0.5f),
            new Vector3(0.5f, 0.01f, y - 0.5f),

        };

        Vector3[] normals = new Vector3[] { 
            Vector3.up, Vector3.up, Vector3.up, Vector3.up, 
            Vector3.up, Vector3.up, Vector3.up, Vector3.up, 
            Vector3.up, Vector3.up, Vector3.up, Vector3.up, 
            Vector3.up, Vector3.up, Vector3.up, Vector3.up, 
            Vector3.up,
            Vector3.up, Vector3.up, Vector3.up, Vector3.up,
        };

        int[] gD = new int[] {
            0, 2, 1,
            2, 0, 3,
        };
        int[] gR = new int[] {
            4, 6, 5,
            6, 4, 7,
        };
        int[] gU = new int[] {
            8, 10, 9,
            10, 8, 11,
        };
        int[] gL = new int[] {
            12, 14, 13,
            14, 12, 15,
        };

        int[] eD = new int[] {
            1, 2, 6, 
            6, 7, 1,
            2, 16, 6,
        };

        int[] eR = new int[] {
            5, 6, 10,
            10, 11, 5,
            6, 16, 10,
        };

        int[] eU = new int[] {
            9, 10, 14,
            14, 15, 9,
            10, 16, 14,
        };

        int[] eL = new int[] {
            13, 14, 2,
            2, 3, 13,
            14, 16, 2
        };

        int[] ft = new int[] {
            17, 19, 18,
            19, 17, 20,
        };

        Mesh m = new Mesh();
        m.subMeshCount = 9;

        m.vertices = verts;
        m.normals = normals;
        m.SetTriangles(gD, 0);
        m.SetTriangles(gR, 1);
        m.SetTriangles(gU, 2);
        m.SetTriangles(gL, 3);
        m.SetTriangles(eD, 4);
        m.SetTriangles(eR, 5);
        m.SetTriangles(eU, 6);
        m.SetTriangles(eL, 7);
        m.SetTriangles(ft, 8);
        return m;

    }
}
