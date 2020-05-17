using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ChunkUtils
{
    static int firstLayerOffset = 0;
    static int secondLayerOffset = 0;
    static int caveOffset = 0;
    static int maxHeight = 64;
    static float increment = 0.02f;
    static float caveIncrement = 0.08f;

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

    public static float CalculateCaveProbability(float x, float y, float z)
    {
        x = x * caveIncrement + caveOffset;
        y = y * caveIncrement + caveOffset;
        z = z * caveIncrement + caveOffset;

        return PerlinNoise3D(x, y, z);
    }

    static float PerlinNoise3D(float x, float y, float z)
    {
        float XY = PerlinNoise(x, y);
        float XZ = PerlinNoise(x, z);
        float YZ = PerlinNoise(y, z);

        float YX = PerlinNoise(y, x);
        float ZX = PerlinNoise(z, x);
        float ZY = PerlinNoise(z, y);

        return (XY + XZ + YZ + YX + ZX + ZY) / 6.0f;
    }

    public static void GenerateRandomOffset()
    {
        firstLayerOffset = Random.Range(0, 1000);
        secondLayerOffset = Random.Range(0, 1000);
        caveOffset = Random.Range(0, 1000);
    }
}