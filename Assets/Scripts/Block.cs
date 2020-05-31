using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public enum BlockSide { FRONT, BACK, LEFT, RIGHT, TOP, BOTTOM };

    BlockType blockType;
    bool isTransparent;
    Chunk chunkParent;
    Vector3 blockPosition;

    static int[] triangles = new int[] { 3, 1, 0, 3, 2, 1 };

    static Vector3[] forwardVector = new Vector3[] { Vector3.forward, Vector3.forward,
                                               Vector3.forward, Vector3.forward};
    static Vector3[] backVector = new Vector3[] { Vector3.back, Vector3.back,
                                               Vector3.back, Vector3.back};
    static Vector3[] leftVector = new Vector3[] { Vector3.left, Vector3.left,
                                               Vector3.left, Vector3.left};
    static Vector3[] rightVector = new Vector3[] { Vector3.right, Vector3.right,
                                               Vector3.right, Vector3.right};
    static Vector3[] upVector = new Vector3[] { Vector3.up, Vector3.up,
                                               Vector3.up, Vector3.up};
    static Vector3[] downVector = new Vector3[] { Vector3.down, Vector3.down,
                                               Vector3.down, Vector3.down};

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

        if (blockType.isTransparent)
            isTransparent = true;
        else
            isTransparent = false;
    }

    public void CreateBlock()
    {
        if (blockType.isTransparent)
            return;

        if (HasTransparentNeighbour(BlockSide.FRONT))
            CreateBlockSide(BlockSide.FRONT);

        if (HasTransparentNeighbour(BlockSide.BACK))
            CreateBlockSide(BlockSide.BACK);

        if (HasTransparentNeighbour(BlockSide.LEFT))
            CreateBlockSide(BlockSide.LEFT);

        if (HasTransparentNeighbour(BlockSide.RIGHT))
            CreateBlockSide(BlockSide.RIGHT);

        if (HasTransparentNeighbour(BlockSide.TOP))
            CreateBlockSide(BlockSide.TOP);

        if (HasTransparentNeighbour(BlockSide.BOTTOM))
            CreateBlockSide(BlockSide.BOTTOM);
    }

    bool HasTransparentNeighbour(BlockSide blockSide)
    {
        Block[,,] chunkBlocks = chunkParent.chunkBlocks;
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

        if (neighbourPosition.x >= 0 && neighbourPosition.x < chunkBlocks.GetLength(0) &&
            neighbourPosition.y >= 0 && neighbourPosition.y < chunkBlocks.GetLength(1) &&
            neighbourPosition.z >= 0 && neighbourPosition.z < chunkBlocks.GetLength(2))
        {
            return chunkBlocks[(int)neighbourPosition.x, (int)neighbourPosition.y, (int)neighbourPosition.z].isTransparent;
        }

        return true;
    }

    void CreateBlockSide(BlockSide side)
    {
        Vector2[] uvs = blockType.GetUV(side);

        Mesh mesh = new Mesh();
        mesh = GenerateBlockSide(mesh, side, uvs);

        GameObject blockSide = new GameObject("block side");
        blockSide.transform.position = blockPosition;
        blockSide.transform.parent = chunkParent.chunkObject.transform;

        MeshFilter meshFilter = blockSide.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshFilter.mesh = mesh;
    }

    Mesh GenerateBlockSide(Mesh mesh, BlockSide side, Vector2[] uv)
    {
        switch (side)
        {
            case BlockSide.FRONT:
                mesh.vertices = frontVertices;
                mesh.normals = forwardVector;
                mesh.uv = blockType.GetBlockUVs(side);
                mesh.triangles = triangles;
                break;
            case BlockSide.BACK:
                mesh.vertices = backVertices;
                mesh.normals = backVector;
                mesh.uv = blockType.GetBlockUVs(side);
                mesh.triangles = triangles;
                break;
            case BlockSide.LEFT:
                mesh.vertices = leftVertices;
                mesh.normals = leftVector;
                mesh.uv = blockType.GetBlockUVs(side);
                mesh.triangles = triangles;
                break;
            case BlockSide.RIGHT:
                mesh.vertices = rightVertices;
                mesh.normals = rightVector;
                mesh.uv = blockType.GetBlockUVs(side);
                mesh.triangles = triangles;
                break;
            case BlockSide.TOP:
                mesh.vertices = topVertices;
                mesh.normals = upVector;
                mesh.uv = blockType.GetBlockUVs(side);
                mesh.triangles = triangles;
                break;
            case BlockSide.BOTTOM:
                mesh.vertices = bottomVertices;
                mesh.normals = downVector;
                mesh.uv = blockType.GetBlockUVs(side);
                mesh.triangles = triangles;
                break;
        }
        return mesh;
    }

    public BlockType GetBlockType()
    {
        return this.blockType;
    }
}