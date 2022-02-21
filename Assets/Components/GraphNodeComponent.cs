using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POESharp;

public class GraphNodeComponent : MonoBehaviour
{
    public int x;
    public int y;
    public int[] edges;
    public string tileType;
    public string orientation;
    public string[] tags;
    public int existance;
    public int height;

    public char dungeonUnk;
    public int lockPositionX;
    public int lockPositionY;

    public void SetData(Graph.Node node) {
        x = node.x;
        y = node.y;
        edges = node.edgeConnections;
        tileType = node.tileType;
        orientation = node.orientation;
        tags = node.descriptions;
        existance = node.existance;
        height = node.height;

        dungeonUnk = node.dungeonUnk;
        lockPositionX = node.lockPositionX;
        lockPositionY = node.lockPositionY;
    }
}
