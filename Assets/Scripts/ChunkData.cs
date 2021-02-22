using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public string Name { get; private set; }
    public BlockType.Type[,,] Blocks { get; private set; }

    public ChunkData(string name, BlockType.Type[,,] blocks)
    {
        Name = name;
        Blocks = blocks;
    }
}
