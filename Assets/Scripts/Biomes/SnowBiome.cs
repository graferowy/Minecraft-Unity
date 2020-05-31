using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBiome : Biome
{
    protected override BlockType GenerateSurface()
    {
        return World.blockTypes[BlockType.Type.SNOW];
    }

    protected override BlockType Generate1stLayer()
    {
        return World.blockTypes[BlockType.Type.SNOW];
    }
}
