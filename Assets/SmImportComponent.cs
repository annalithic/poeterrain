using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using POESharp;
using Newtonsoft.Json;
using PoeTerrain;

[ExecuteInEditMode]
public class SmImportComponent : MonoBehaviour
{
    public GameObject smdPrefab;
    public string gameFolder = @"E:\Extracted\PathOfExile\3.18.Sentinel\";
    public bool importSMD;
    public bool importAll;
    public bool test;

    void Update() {
        if(importSMD) {
            importSMD = false;
            string path = EditorUtility.OpenFilePanel("Import smd", gameFolder, "smd,fmt");
            Mesh mesh = ImportSMD(path);
            GameObject newObj = Instantiate(smdPrefab);
            newObj.name = Path.GetFileNameWithoutExtension(path);
            newObj.GetComponent<MeshFilter>().sharedMesh = mesh;
        }
        if(importAll) {
            importAll = false;
            ImportAll(gameFolder);
        }
        if(test) {
            test = false;
            string smdPath = EditorUtility.OpenFilePanel("Import smd", @"E:\Extracted\PathOfExile\3.21.Crucible\Art\Models\MONSTERS", "smd");
            string astPath = EditorUtility.OpenFilePanel("Import ast", Path.GetDirectoryName(smdPath), "ast");
            Debug.Log(smdPath + " | " + astPath);
            Mesh smd = ImportSMD(smdPath);
            Debug.Log(smd.vertices.Length);
            Ast ast = new Ast(astPath);
            Debug.Log(ast.animations.Length);

            //Ast ast = new Ast(@"E:\Extracted\PathOfExile\3.21.Crucible\Art\Models\MONSTERS\Anchorman\Animations\rig.ast");

            for (int i = 0; i < ast.animations.Length; i++) {
                Test(smd, ast, i, Vector3.right * 200 * i);
            }
        }
    }

    void Test(Mesh mesh, Ast ast, int animation, Vector3 pos) {

        //Matrix4x4 testMat = Matrix4x4.Translate(new Vector3(1, 2, 3));
        //Debug.Log($"{testMat.m00} {testMat.m10} {testMat.m20} {testMat.m30} | {testMat.m01} {testMat.m11} {testMat.m21} {testMat.m31} | {testMat.m02} {testMat.m12} {testMat.m22} {testMat.m32} | {testMat.m03} {testMat.m13} {testMat.m23} {testMat.m33}");
        //return;

        //string smdPath = @"E:\Extracted\PathOfExile\3.21.Crucible\Art\Models\MONSTERS\Anchorman\Anchorman_armour_c18cc675.smd";
        //string smdPath = @"E:\Extracted\PathOfExile\3.21.Crucible\Art\Models\MONSTERS\MortarSquid\rig_99b74fee.smd";


        Transform[] bones = new Transform[ast.bones.Length];


        GameObject newObj = new GameObject(ast.animations[animation].name);

        ImportBone(bones, ast, 0, animation, newObj.transform);
        if (mesh.bindposes == null || mesh.bindposes.Length == 0) {
            Matrix4x4[] bindPoses = new Matrix4x4[ast.bones.Length];
            for (int i = 0; i < bones.Length; i++) {
                bindPoses[i] = bones[i].worldToLocalMatrix;
            }
            mesh.bindposes = bindPoses;
        }


        SkinnedMeshRenderer renderer = newObj.AddComponent<SkinnedMeshRenderer>();
        Material mat = Resources.Load<Material>("0");
        renderer.sharedMaterial = mat;

        renderer.bones = bones;
        renderer.rootBone = bones[0];
        renderer.sharedMesh = mesh;

        newObj.transform.Translate(pos);
        newObj.transform.Rotate(new Vector3(90, 0, 0));
    }

    Vector3 TranslationFromMatrix(float[] transform) {
       return new Vector3(transform[12], transform[13], transform[14]);
    }

    Quaternion RotationFromMatrix(float[] transform) {
        Vector3 forward;
        forward.x = transform[8];
        forward.y = transform[9];
        forward.z = transform[10];

        Vector3 upwards;
        upwards.x = transform[4];
        upwards.y = transform[5];
        upwards.z = transform[6];
        return Quaternion.LookRotation(forward, upwards);
    }

    void ImportBone(Transform[] bones, Ast ast, int boneIndex, int animation = 0, Transform parent = null) {
        bones[boneIndex] = new GameObject(ast.bones[boneIndex].name).transform;

        //this is just for gizmo drawing
        //var collider = bones[boneIndex].gameObject.AddComponent<SphereCollider>();
        //collider.radius = 5;

        if (parent != null) bones[boneIndex].SetParent(parent);

        bones[boneIndex].localPosition = TranslationFromMatrix(ast.bones[boneIndex].transform);
        bones[boneIndex].localRotation = RotationFromMatrix(ast.bones[boneIndex].transform);


        //bones[boneIndex].localPosition = new Vector3(
        //    ast.animations[animation].tracks[boneIndex].positionKeys[0][1],
        //    ast.animations[animation].tracks[boneIndex].positionKeys[0][2],
        //    ast.animations[animation].tracks[boneIndex].positionKeys[0][3]
        //);
        //bones[boneIndex].localRotation = new Quaternion(
        //    ast.animations[animation].tracks[boneIndex].rotationKeys[0][1],
        //    ast.animations[animation].tracks[boneIndex].rotationKeys[0][2],
        //    ast.animations[animation].tracks[boneIndex].rotationKeys[0][3],
        //    ast.animations[animation].tracks[boneIndex].rotationKeys[0][4]
        //);
        //if (parent == null) bone.name = ast.animations[animation].name;

        JankBoneComponent component = bones[boneIndex].gameObject.AddComponent<JankBoneComponent>();
        component.positionTimes = new float[ast.animations[animation].tracks[boneIndex].positionKeys.Length];
        component.positions = new Vector3[ast.animations[animation].tracks[boneIndex].positionKeys.Length];
        for (int i = 0; i < component.positions.Length; i++) {
            component.positionTimes[i] = ast.animations[animation].tracks[boneIndex].positionKeys[i][0];
            component.positions[i] = new Vector3(
                ast.animations[animation].tracks[boneIndex].positionKeys[i][1],
                ast.animations[animation].tracks[boneIndex].positionKeys[i][2],
                ast.animations[animation].tracks[boneIndex].positionKeys[i][3]
            );
        }

        component.rotationTimes = new float[ast.animations[animation].tracks[boneIndex].rotationKeys.Length];
        component.rotations = new Quaternion[ast.animations[animation].tracks[boneIndex].rotationKeys.Length];
        for (int i = 0; i < component.rotations.Length; i++) {
            component.rotationTimes[i] = ast.animations[animation].tracks[boneIndex].rotationKeys[i][0];
            component.rotations[i] = new Quaternion(
                ast.animations[animation].tracks[boneIndex].rotationKeys[i][1],
                ast.animations[animation].tracks[boneIndex].rotationKeys[i][2],
                ast.animations[animation].tracks[boneIndex].rotationKeys[i][3],
                ast.animations[animation].tracks[boneIndex].rotationKeys[i][4]
            );
        }

        if (ast.bones[boneIndex].sibling != 255) ImportBone(bones, ast, ast.bones[boneIndex].sibling, animation, parent);
        if (ast.bones[boneIndex].child != 255) ImportBone(bones, ast, ast.bones[boneIndex].child, animation, bones[boneIndex]);
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
        PoeMesh poeMesh = path.EndsWith("smd") ? (PoeMesh) new Smd(path) : (PoeMesh) new Fmt(path);
        if (poeMesh.triCount == 0 || poeMesh.vertCount == 0) return null;
        Vector3[] verts = new Vector3[poeMesh.vertCount];
        Vector2[] uvs = new Vector2[poeMesh.vertCount];
        for(int i = 0; i < verts.Length; i++) {
            verts[i] = new Vector3(poeMesh.x[i], poeMesh.y[i], poeMesh.z[i]);
            uvs[i] = new Vector2(Mathf.HalfToFloat(poeMesh.u[i]), Mathf.HalfToFloat(poeMesh.v[i]));
        }
        int[] tris = new int[poeMesh.idx.Length];
        for(int i = 0; i < poeMesh.idx.Length; i+= 3) {
            tris[i] = poeMesh.idx[i];
            tris[i + 1] = poeMesh.idx[i + 1];
            tris[i + 2] = poeMesh.idx[i + 2];
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        if(path.EndsWith(".smd")) {
            Smd smd = (Smd)poeMesh;
            BoneWeight[] weights = new BoneWeight[poeMesh.vertCount];
            for (int i = 0; i < weights.Length; i++) {
                System.Array.Sort(smd.boneWeights[i]);
                //if (i < 100) Debug.Log($"{smd.boneWeights[i][0].weight} | {smd.boneWeights[i][1].weight} | {smd.boneWeights[i][2].weight} | {smd.boneWeights[i][2].weight}  -  {smd.boneWeights[i][0].id} | {smd.boneWeights[i][1].id} | {smd.boneWeights[i][2].id} | {smd.boneWeights[i][3].id}");
                weights[i] = new BoneWeight() {
                    boneIndex0 = smd.boneWeights[i][0].id,
                    boneIndex1 = smd.boneWeights[i][1].id,
                    boneIndex2 = smd.boneWeights[i][2].id,
                    boneIndex3 = smd.boneWeights[i][3].id,
                    weight0 = smd.boneWeights[i][0].weight / 255f,
                    weight1 = smd.boneWeights[i][1].weight / 255f,
                    weight2 = smd.boneWeights[i][2].weight / 255f,
                    weight3 = smd.boneWeights[i][3].weight / 255f
                };
            }
            mesh.boneWeights = weights;
        }



        return mesh;

        //GameObject o = Instantiate(smdPrefab);
        //o.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //o.GetComponent<MeshFilter>().sharedMesh = mesh;
        //return o;
    }
}
