using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PoeTerrain;

[ExecuteInEditMode]
public class AstImportComponent : MonoBehaviour
{

    public bool import;
    public GameObject boneprefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(import) {
            import = false;
            ImportAst(EditorUtility.OpenFilePanel("Import Ast", @"E:\Extracted\PathOfExile\3.21.Crucible\Art\Models\MONSTERS", "ast"));
        }
    }

    void ImportAst(string path) {
        Ast ast = new Ast(path);
        //for (int i = 0; i < ast.animations.Length; i++) {
        //    Transform root = ImportBone(ast, 0, i);
        //    root.Rotate(90, 0, 0);
        //}
        Transform astRoot = new GameObject("AST").transform;
        for (int i = 0; i < ast.animations.Length; i++) {
            Transform modelRoot = new GameObject(ast.animations[i].name).transform;
            modelRoot.SetParent(astRoot);
            Transform root = ImportBone(ast, 0, i);
            root.SetParent(modelRoot);
            modelRoot.Rotate(90, 0, 0);
            modelRoot.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            modelRoot.Translate(Vector3.right * 20 * i);
        }
    }

    Transform ImportBone(Ast ast, int boneIndex, int animation = 0, Transform parent = null) {
        Transform bone = parent == null ? Instantiate(boneprefab).transform : Instantiate(boneprefab, parent).transform;

        //bone.localPosition = new Vector3(ast.bones[boneIndex].transform[12], ast.bones[boneIndex].transform[13], ast.bones[boneIndex].transform[14]);
        bone.localPosition = new Vector3(
            ast.animations[animation].tracks[boneIndex].positionKeys[0][1],
            ast.animations[animation].tracks[boneIndex].positionKeys[0][2],
            ast.animations[animation].tracks[boneIndex].positionKeys[0][3]
        );
        bone.localRotation = new Quaternion(
            ast.animations[animation].tracks[boneIndex].rotationKeys[0][1],
            ast.animations[animation].tracks[boneIndex].rotationKeys[0][2],
            ast.animations[animation].tracks[boneIndex].rotationKeys[0][3],
            ast.animations[animation].tracks[boneIndex].rotationKeys[0][4]
        );

        JankBoneComponent component = bone.gameObject.AddComponent<JankBoneComponent>();
        component.positionTimes = new float[ast.animations[animation].tracks[boneIndex].positionKeys.Length];
        component.positions = new Vector3[ast.animations[animation].tracks[boneIndex].positionKeys.Length];
        for(int i = 0; i < component.positions.Length; i++) {
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


        bone.name = ast.bones[boneIndex].name;
        //if (parent == null) bone.name = ast.animations[animation].name;
        if (ast.bones[boneIndex].sibling != 255) ImportBone(ast, ast.bones[boneIndex].sibling, animation, parent);
        if (ast.bones[boneIndex].child != 255) ImportBone(ast, ast.bones[boneIndex].child, animation, bone);
        return bone;
    }
}
