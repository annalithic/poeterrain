using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JankBoneComponent : MonoBehaviour
{
    public float[] positionTimes;
    public Vector3[] positions;
    public float[] rotationTimes;
    public Quaternion[] rotations;
    public float time;

    private void Update() {
        time = time + (Time.deltaTime * 30);
        int position = 0;
        for(int i = 0; i < positionTimes.Length; i++) {
            if (positionTimes[i] > time) break;
            position++;
        }
        if(position == positionTimes.Length) {
            position = 0;
            time = 0;
        }
        transform.localPosition = positions[position];

        int rotation = 0;
        for (int i = 0; i < rotationTimes.Length; i++) {
            if (rotationTimes[i] > time) break;
            rotation++;
        }
        transform.localRotation = rotations[rotation];
    }
}
