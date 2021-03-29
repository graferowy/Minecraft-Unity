﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public enum BlockSide { FRONT, BACK, LEFT, RIGHT, TOP, BOTTOM };

    BlockType blockType;
    Chunk chunkParent;
    Vector3 blockPosition;

    static int[] triangles = new int[] { 3, 1, 0, 3, 2, 1 };

    static Vector3[] vertices = new Vector3[8] { new Vector3(-0.5f, -0.5f,  0.5f),
                                          new Vector3( 0.5f, -0.5f,  0.5f),
                                          new Vector3( 0.5f, -0.5f, -0.5f),
                                          new Vector3(-0.5f, -0.5f, -0.5f),
                                          new Vector3(-0.5f,  0.5f,  0.5f),
                                          new Vector3( 0.5f,  0.5f,  0.5f),
                                          new Vector3( 0.5f,  0.5f, -0.5f),
                                          new Vector3(-0.5f,  0.5f, -0.5f) };

    static Vector3[] frontVertices = new Vector3[] { vertices[4], vertices[5],
                                                vertices[1], vertices[0]};
    static Vector3[] backVertices = new Vector3[] { vertices[6], vertices[7],
                                                vertices[3], vertices[2]};
    static Vector3[] leftVertices = new Vector3[] { vertices[7], vertices[4],
                                                vertices[0], vertices[3]};
    static Vector3[] rightVertices = new Vector3[] { vertices[5], vertices[6],
                                                vertices[2], vertices[1]};
    static Vector3[] topVertices = new Vector3[] { vertices[7], vertices[6],
                                                vertices[5], vertices[4]};
    static Vector3[] bottomVertices = new Vector3[] { vertices[0], vertices[1],
                                                vertices[2], vertices[3]};

    public Block(BlockType blockType, Chunk chunkParent, Vector3 blockPosition)
    {
        this.blockType = blockType;
        this.chunkParent = chunkParent;
        this.blockPosition = blockPosition;
    }

    public void CreateBlock()
    {
        if (blockType.isTransparent)
            return;

        if (HasTransparentNeighbour(BlockSide.FRONT))
            GenerateBlockSide(BlockSide.FRONT);

        if (HasTransparentNeighbour(BlockSide.BACK))
            GenerateBlockSide(BlockSide.BACK);

        if (HasTransparentNeighbour(BlockSide.LEFT))
            GenerateBlockSide(BlockSide.LEFT);

        if (HasTransparentNeighbour(BlockSide.RIGHT))
            GenerateBlockSide(BlockSide.RIGHT);

        if (HasTransparentNeighbour(BlockSide.TOP))
            GenerateBlockSide(BlockSide.TOP);

        if (HasTransparentNeighbour(BlockSide.BOTTOM))
            GenerateBlockSide(BlockSide.BOTTOM);
    }

    bool HasTransparentNeighbour(BlockSide blockSide)
    {
        Vector3 neighbourPosition = new Vector3(0, 0, 0);

        if (blockSide == BlockSide.FRONT)
            neighbourPosition = new Vector3(blockPosition.x, blockPosition.y, blockPosition.z + 1);
        else if (blockSide == BlockSide.BACK)
            neighbourPosition = new Vector3(blockPosition.x, blockPosition.y, blockPosition.z - 1);
        else if (blockSide == BlockSide.TOP)
            neighbourPosition = new Vector3(blockPosition.x, blockPosition.y + 1, blockPosition.z);
        else if (blockSide == BlockSide.BOTTOM)
            neighbourPosition = new Vector3(blockPosition.x, blockPosition.y - 1, blockPosition.z);
        else if (blockSide == BlockSide.RIGHT)
            neighbourPosition = new Vector3(blockPosition.x + 1, blockPosition.y, blockPosition.z);
        else if (blockSide == BlockSide.LEFT)
            neighbourPosition = new Vector3(blockPosition.x - 1, blockPosition.y, blockPosition.z);

        Block[,,] chunkBlocks = chunkParent.chunkBlocks;        

        if (neighbourPosition.x < 0 || neighbourPosition.x >= World.chunkSize || 
            neighbourPosition.y < 0 || neighbourPosition.y >= World.chunkSize ||
            neighbourPosition.z < 0 || neighbourPosition.z >= World.chunkSize)
        {
            Vector3 neighbourChunkPosition = this.chunkParent.chunkObject.transform.position;
            neighbourChunkPosition.x += (neighbourPosition.x - blockPosition.x) * World.chunkSize;
            neighbourChunkPosition.y += (neighbourPosition.y - blockPosition.y) * World.chunkSize;
            neighbourChunkPosition.z += (neighbourPosition.z - blockPosition.z) * World.chunkSize;

            string neighbourChunkName = World.GenerateChunkName(neighbourChunkPosition);
            
            Chunk neighbourChunk;

            if (World.chunks.TryGetValue(neighbourChunkName, out neighbourChunk))
            {
                chunkBlocks = neighbourChunk.chunkBlocks;
            }
            else
            {
                return true;
            }
        }

        if (neighbourPosition.x < 0) neighbourPosition.x = World.chunkSize - 1;
        if (neighbourPosition.y < 0) neighbourPosition.y = World.chunkSize - 1;
        if (neighbourPosition.z < 0) neighbourPosition.z = World.chunkSize - 1;
        if (neighbourPosition.x >= World.chunkSize) neighbourPosition.x = 0;
        if (neighbourPosition.y >= World.chunkSize) neighbourPosition.y = 0;
        if (neighbourPosition.z >= World.chunkSize) neighbourPosition.z = 0;

        var neighbourBlockType = chunkBlocks[(int)neighbourPosition.x, (int)neighbourPosition.y, (int)neighbourPosition.z].blockType;

        if (this.blockType.isTranslucent && neighbourBlockType.isTranslucent && !neighbourBlockType.isTransparent)
            return false;
        
        return neighbourBlockType.isTransparent || neighbourBlockType.isTranslucent;
    }

    void GenerateBlockSide(BlockSide side)
    {
        switch (side)
        {
            case BlockSide.FRONT:
                foreach (Vector3 frontVertex in frontVertices) 
                    chunkParent.vertices.Add(blockPosition + frontVertex);
                break;
            case BlockSide.BACK:
                foreach (Vector3 backVertex in backVertices) 
                    chunkParent.vertices.Add(blockPosition + backVertex);
                break;
            case BlockSide.LEFT:
                foreach (Vector3 leftVertex in leftVertices) 
                    chunkParent.vertices.Add(blockPosition + leftVertex);
                break;
            case BlockSide.RIGHT:
                foreach (Vector3 rightVertex in rightVertices) 
                    chunkParent.vertices.Add(blockPosition + rightVertex);
                break;
            case BlockSide.TOP:
                foreach (Vector3 topVertex in topVertices) 
                    chunkParent.vertices.Add(blockPosition + topVertex);
                break;
            case BlockSide.BOTTOM:
                foreach (Vector3 bottomVertex in bottomVertices) 
                    chunkParent.vertices.Add(blockPosition + bottomVertex);
                break;
        }
        
        foreach (Vector2 blockUV in blockType.GetBlockUVs(side)) 
            chunkParent.uvs.Add(blockUV);
        
        foreach (int triangle in triangles)
        {
            if (this.blockType.isLiquid())
                chunkParent.waterTriangles.Add(chunkParent.VertexIndex + triangle);
            else if (this.blockType.isTranslucent || this.blockType.isTransparent)
                chunkParent.transparentTriangles.Add(chunkParent.VertexIndex + triangle);
            else
                chunkParent.triangles.Add(chunkParent.VertexIndex + triangle);
        }

        chunkParent.VertexIndex += 4;
    }

    public BlockType GetBlockType()
    {
        return this.blockType;
    }

    public void SetBlockType(BlockType type)
    {
        this.blockType = type;
    }
}