using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkUtils : MonoBehaviour
{
    static int offset = 0;
    static int maxHeight = 16;
    static float increment = 0.035f;

    public static float GenerateHeight(float x, float z)
    {
        float height = Map(1, maxHeight, 0, 1, PerlinNoise(x * increment + offset, z * increment + offset));
        return height;
    }

    static float Map(float from, float to, float from2, float to2, float value)
    {
        if (value <= from2)
            return from;

        if (value >= to2)
            return to;

        return (to - from) * ((value - from2) / (to2 - from2)) + from;
    }

    static float PerlinNoise(float x, float z)
    {
        float height = Mathf.PerlinNoise(x, z);
        return height;
    }

    public static int GenerateRandomOffset()
    {
        offset = Random.Range(0, 1000);
        return offset;
    }
}
