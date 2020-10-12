using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Biome
{
    public virtual float typeIncrement { get { return 0.08f; } }
    public virtual float firstLayerIncrement { get { return 0.02f; } }
    public virtual float secondLayerIncrement { get { return 0.1f; } }
    public virtual float waterLayerY { get { return 25; } }

    protected float typeProbability;
    protected int generated1stLayerY;
    protected int generated2ndLayerY;

    public virtual BlockType GenerateTerrain(float x, float y, float z)
    {
        GenerateTerrainValues(x, y, z);

        if (y == generated1stLayerY)
        {
            return GenerateSurface();
        }

        if (typeProbability > 0.5f && y < generated1stLayerY - 5)
        {
            return GenerateCave();
        }

        if (y < generated2ndLayerY)
        {
            return Generate2ndLayer();
        }

        if (y < generated1stLayerY)
        {
            return Generate1stLayer();
        }

        if (y < waterLayerY)
        {
            return GenerateWaterLayer();
        }

        return World.blockTypes[BlockType.Type.AIR];
    }

    protected virtual BlockType GenerateSurface()
    {
        return World.blockTypes[BlockType.Type.GRASS];
    }

    protected virtual BlockType GenerateCave()
    {
        return World.blockTypes[BlockType.Type.CAVE];
    }

    protected virtual BlockType Generate2ndLayer()
    {
        return World.blockTypes[BlockType.Type.STONE];
    }

    protected virtual BlockType Generate1stLayer()
    {
        return World.blockTypes[BlockType.Type.DIRT];
    }
    
    protected virtual BlockType GenerateWaterLayer()
    {
        return World.blockTypes[BlockType.Type.WATER];
    }

    public virtual void GenerateTerrainValues(float x, float y, float z)
    {
        typeProbability = ChunkUtils.CalculateBlockProbability(x, y, z, typeIncrement);
        generated1stLayerY = (int)ChunkUtils.Generate1stLayerHeight(x, z, firstLayerIncrement);
        generated2ndLayerY = (int)ChunkUtils.Generate2ndLayerHeight(x, z, generated1stLayerY, secondLayerIncrement);
    }
}