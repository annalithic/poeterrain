using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POESharp;

public class ArmKEntryComponent : MonoBehaviour
{
    public int sizeX;
    public int sizeY;
    public string edgeTypeDown;
    public string edgeTypeRight;
    public string edgeTypeUp;
    public string edgeTypeLeft;
    public int edgeLengthDown;
    public int unk2;
    public int edgeLengthRight;
    public int unk4;
    public int edgeLengthUp;
    public int unk6;
    public int edgeLengthLeft;
    public int unk8;
    public string groundTypeDownLeft;
    public string groundTypeDownRight;
    public string groundTypeUpRight;
    public string groundTypeUpLeft;
    public int heightDownLeft;
    public int heightDownRight;
    public int heightUpRight;
    public int heightUpLeft;
    public string feature;
    public int origin;

    public void SetData(Arm.KEntry k, string[] names) {
        sizeX = k.sizeX;
        sizeY = k.sizeY;
        edgeTypeDown = GetName(k.edgeTypeDown, names);
        edgeTypeRight = GetName(k.edgeTypeRight, names);
        edgeTypeUp = GetName(k.edgeTypeUp, names);
        edgeTypeLeft = GetName(k.edgeTypeLeft, names);
        edgeLengthDown = k.edgeLengthDown;
        unk2 = k.unk2;
        edgeLengthRight = k.edgeLengthRight;
        unk4 = k.unk4;
        edgeLengthUp = k.edgeLengthUp;
        unk6 = k.unk6;
        edgeLengthLeft = k.edgeLengthLeft;
        unk8 = k.unk8;
        groundTypeDownLeft = GetName(k.groundTypeDownLeft, names);
        groundTypeDownRight = GetName(k.groundTypeDownRight, names);
        groundTypeUpRight = GetName(k.groundTypeUpRight, names);
        groundTypeUpLeft = GetName(k.groundTypeUpLeft, names);
        heightDownLeft = k.heightDownLeft;
        heightDownRight = k.heightDownRight;
        heightUpRight = k.heightUpRight;
        heightUpLeft = k.heightUpLeft;
        feature = GetName(k.feature, names);
        origin = k.origin;
    }

    string GetName(int i, string[] names) {
        if (i == 0) return "void";
        return names[i - 1];
    }
}
