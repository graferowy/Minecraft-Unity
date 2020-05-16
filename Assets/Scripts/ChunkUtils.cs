using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkUtils
{
    static int firstLayerOffset = 0;
    static int secondLayerOffset = 0;
    static int maxHeight = 64;
    static float increment = 0.02f;

    public static float Generate1stLayerHeight(float x, float z)
    {
        float height = Map(1, maxHeight, 0, 1, PerlinNoise(x * increment + firstLayerOffset, z * increment + firstLayerOffset));
        return height;
    }

    public static float Generate2ndLayerHeight(float x, float z, int maxHeight)
    {
        float height = Map(1, maxHeight, 0, 1, PerlinNoise(x * increment * 5 + secondLayerOffset, z * increment * 5 + secondLayerOffset));
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

    public static void GenerateRandomOffset()
    {
        firstLayerOffset = Random.Range(0, 1000);
        secondLayerOffset = Random.Range(0, 1000);

    }
}