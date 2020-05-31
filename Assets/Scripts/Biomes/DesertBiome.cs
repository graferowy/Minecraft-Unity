using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertBiome : Biome
{
    public override float firstLayerIncrement { get { return 0.005f; } }

    public override BlockType GenerateTerrain(float x, float y, float z)
    {
        GenerateTerrainValues(x, y, z);

        if (y == generated1stLayerY)
        {
            return GenerateSurface();
        }

        if (y < generated2ndLayerY)
        {
            return Generate2ndLayer();
        }

        if (y < generated1stLayerY)
        {
            return Generate1stLayer();
        }

        return World.blockTypes[BlockType.Type.AIR];
    }

    protected override BlockType GenerateSurface()
    {
        return World.blockTypes[BlockType.Type.SAND];
    }

    protected override BlockType Generate1stLayer()
    {
        return World.blockTypes[BlockType.Type.SAND];
    }
}
