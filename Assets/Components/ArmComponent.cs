using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POESharp;

public class ArmComponent : MonoBehaviour
{
    public int version;
    public string[] entries;
    public int size1x; public int size1y;
    public int apax; public int apay;
    public string feature;
    public int bepax; public int bepay;
    public string[] apaEntries;
    public string[][] entityLines;

    public void SetData(Arm a) {
        version = a.version;
        entries = a.entries;
        size1x = a.size1x;
        size1y = a.size1y;
        apax = a.apax;
        apay = a.apay;
        feature = a.name;
        bepax = a.bepax;
        bepay = a.bepay;
        apaEntries = a.apaEntries;
        gameObject.AddComponent<ArmKEntryComponent>().SetData(a.kEntry, entries);
    }
}
