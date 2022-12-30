using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using POESharp;
using Newtonsoft.Json;

[ExecuteInEditMode]
public class SmImportComponent : MonoBehaviour
{
    public GameObject smdPrefab;
    public string gameFolder = @"E:\Extracted\PathOfExile\3.18.Sentinel\";
    public bool importSMD;
    public bool importAll;

    void Update() {
        if(importSMD) {
            importSMD = false;
            Mesh mesh = ImportSMD(EditorUtility.OpenFilePanel("Import smd", gameFolder, "smd,fmt"));
            Instantiate(smdPrefab);
            smdPrefab.name = mesh.name;
            smdPrefab.GetComponent<MeshFilter>().sharedMesh = mesh;
        }
        if(importAll) {
            importAll = false;
            ImportAll(gameFolder);
        }
    }

    void ImportAll(string folder) {
        int debug = 0;
        AssetDatabase.StartAssetEditing();
        foreach(string path in Directory.EnumerateFiles(folder, "*.fmt", SearchOption.AllDirectories)) {
            string relative = path.Substring(folder.Length); relative = relative + ".mesh";
            string meshPath = Path.Combine(Application.dataPath, relative);
            if (File.Exists(meshPath)) continue;
            if (!Directory.Exists(Path.GetDirectoryName(meshPath))) Directory.CreateDirectory(Path.GetDirectoryName(meshPath));
            Mesh mesh = ImportSMD(path);
            if (mesh == null) continue;
            AssetDatabase.CreateAsset(mesh, Path.Combine("Assets", relative));
            debug++;
            //if (debug > 50) break;
        }
        AssetDatabase.StopAssetEditing();
    }

    Mesh ImportSMD(string path) {
        PoeMesh smd = path.EndsWith("smd") ? (PoeMesh) new Smd(path) : (PoeMesh) new Fmt(path);
        if (smd.triCount == 0 || smd.vertCount == 0) return null;
        Vector3[] verts = new Vector3[smd.vertCount];
        Vector2[] uvs = new Vector2[smd.vertCount];
        for(int i = 0; i < verts.Length; i++) {
            verts[i] = new Vector3(smd.x[i] / 100, smd.z[i] / -100, smd.y[i] / -100);
            uvs[i] = new Vector2(Mathf.HalfToFloat(smd.u[i]), Mathf.HalfToFloat(smd.v[i]));
        }
        int[] tris = new int[smd.idx.Length];
        for(int i = 0; i < smd.idx.Length; i+= 3) {
            tris[i] = smd.idx[i];
            tris[i + 1] = smd.idx[i + 2];
            tris[i + 2] = smd.idx[i + 1];
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        return mesh;

        //GameObject o = Instantiate(smdPrefab);
        //o.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //o.GetComponent<MeshFilter>().sharedMesh = mesh;
        //return o;
    }
}
