using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ChunkUtils
{
    static int firstLayerOffset = 0;
    static int secondLayerOffset = 0;
    static int typeOffset = 0;
    static int moistureOffset = 0;
    static int temperatureOffset = 0;
    static int maxHeight = 64;

    public static float Generate1stLayerHeight(float x, float z, float increment = 0.02f)
    {
        float height = Map(1, maxHeight, 0, 1, PerlinNoise(x * increment + firstLayerOffset, z * increment + firstLayerOffset));
        return height;
    }

    public static float Generate2ndLayerHeight(float x, float z, int maxHeight, float increment = 0.1f)
    {
        float height = Map(1, maxHeight, 0, 1, PerlinNoise(x * increment + secondLayerOffset, z * increment + secondLayerOffset));
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

    public static float GenerateMoisture(float x, float z, float increment = 0.05f)
    {
        return PerlinNoise(x * increment + moistureOffset, z * increment + moistureOffset);
    }

    public static float GenerateTemperature(float x, float z, float increment = 0.05f)
    {
        return PerlinNoise(x * increment + temperatureOffset, z * increment + temperatureOffset);
    }

    static float PerlinNoise(float x, float z)
    {
        float height = Mathf.PerlinNoise(x, z);
        return height;
    }

    public static float CalculateBlockProbability(float x, float y, float z, float increment = 0.08f)
    {
        x = x * increment + typeOffset;
        y = y * increment + typeOffset;
        z = z * increment + typeOffset;

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
        typeOffset = Random.Range(0, 1000);
        moistureOffset = Random.Range(0, 1000);
        temperatureOffset = Random.Range(0, 1000);
    }
}