using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockType
{
    public enum Type { AIR, CAVE, DIRT, BRICK, GRASS, STONE, CARBON, DIAMOND, SNOW, SAND, GLASS }
    public string name { get; private set; }
    public bool isTransparent { get; private set; }
    public bool isTranslucent { get; private set; }
    public bool everySideSame { get; private set; }

    public Vector2[] topUV { private get; set; }
    public Vector2[] sideUV { private get; set; }
    public Vector2[] bottomUV { private get; set; }

    List<Vector2[]> blockUVs = new List<Vector2[]>();

    public BlockType(string typeName, bool isTransparent, bool isTranslucent, bool everySideSame)
    {
        this.name = typeName;
        this.isTransparent = isTransparent;
        this.isTranslucent = isTranslucent;
        this.everySideSame = everySideSame;
    }

    public Vector2[] GetUV(Block.BlockSide side)
    {
        if (everySideSame ||
            (side != Block.BlockSide.TOP && side != Block.BlockSide.BOTTOM))
            return this.sideUV;

        if (side == Block.BlockSide.TOP)
            return this.topUV;
        else
            return this.bottomUV;
    }

    public Vector2[] GetBlockUVs(Block.BlockSide side)
    {
        if (everySideSame ||
            (side != Block.BlockSide.TOP && side != Block.BlockSide.BOTTOM))
            return this.blockUVs[0];

        if (side == Block.BlockSide.TOP)
            return this.blockUVs[1];
        else
            return this.blockUVs[2];
    }

    public void GenerateBlockUVs()
    {
        this.blockUVs.Add(new Vector2[] { sideUV[3], sideUV[2], sideUV[0], sideUV[1] });

        if (everySideSame)
            return;

        if (topUV.Length > 0)
        {
            this.blockUVs.Add(new Vector2[] { topUV[3], topUV[2], topUV[0], topUV[1] });
        }

        if (bottomUV.Length > 0)
        {
            this.blockUVs.Add(new Vector2[] { bottomUV[3], bottomUV[2], bottomUV[0], bottomUV[1] });
        }
    }
}