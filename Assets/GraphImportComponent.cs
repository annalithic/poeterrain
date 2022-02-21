using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POESharp;
using System.IO;
using UnityEditor;

[ExecuteInEditMode]
public class GraphImportComponent : MonoBehaviour
{

    int heightMult = 32;

    public string path;
    public bool import;
    public GameObject nodePrefab;
    public GameObject edgePrefab;


    void Update() {
        if(import) {
            path = path.Trim('"');
            import = false;
            ImportGraph(path);
        }
    }

    void ImportGraph(string path) {
        if (!path.EndsWith(".tgr") && !(path.EndsWith(".dgr"))) return;
        Graph g = new Graph(path);

        Transform root = new GameObject(Path.GetFileNameWithoutExtension(g.path)).transform;
        foreach(var node in g.nodes) {
            Transform nodeObj = Instantiate(nodePrefab, root).transform;
            nodeObj.localPosition = new Vector3(node.x, node.height * heightMult, node.y);
            if (node.tileType != "") nodeObj.name = node.tileType;
            nodeObj.GetComponent<GraphNodeComponent>().SetData(node);
        }

        foreach (var edge in g.edges) {
            Transform edgeObj = Instantiate(edgePrefab, root).transform;
            LineRenderer l = edgeObj.GetComponent<LineRenderer>();
            l.SetPositions(new Vector3[] { 
                new Vector3(g.nodes[edge.start].x, g.nodes[edge.start].height * heightMult, g.nodes[edge.start].y), 
                new Vector3(g.nodes[edge.end].x, g.nodes[edge.end].height * heightMult, g.nodes[edge.end].y) });
            Material mat = Resources.Load<Material>(edge.edgeType.Replace('/', '_'));
            if(mat == null) {
                Material matTemplate = Resources.Load<Material>("TerrainMat");
                mat = Instantiate(matTemplate);
                mat.color = Random.ColorHSV(0, 1, 0.3f, 0.6f, 0.2f, 0.8f);
                AssetDatabase.CreateAsset(mat, $"Assets/Resources/{edge.edgeType.Replace('/', '_')}.mat");
            }
            l.sharedMaterial = mat;
            edgeObj.name = edge.edgeType;
        }
    }
}
