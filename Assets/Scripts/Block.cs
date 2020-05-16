using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    enum BlockSide { FRONT, BACK, LEFT, RIGHT, TOP, BOTTOM };
    public enum BlockType { DIRT, AIR, WATER };

    BlockType blockType;
    bool isTransparent;
    GameObject blockParent;
    Vector3 blockPosition;
    Material blockMaterial;

    Vector3[] vertices = new Vector3[8] { new Vector3(-0.5f, -0.5f,  0.5f),
                                          new Vector3( 0.5f, -0.5f,  0.5f),
                                          new Vector3( 0.5f, -0.5f, -0.5f),
                                          new Vector3(-0.5f, -0.5f, -0.5f),
                                          new Vector3(-0.5f,  0.5f,  0.5f),
                                          new Vector3( 0.5f,  0.5f,  0.5f),
                                          new Vector3( 0.5f,  0.5f, -0.5f),
                                          new Vector3(-0.5f,  0.5f, -0.5f) };
    Vector2[] uv = new Vector2[4] { new Vector2(0f, 0f),
                                    new Vector2(1f, 0f),
                                    new Vector2(0f, 1f),
                                    new Vector2(1f, 1f) };

    public Block(BlockType blockType, GameObject blockParent,
        Vector3 blockPosition, Material blockMaterial)
    {
        this.blockType = blockType;
        this.blockParent = blockParent;
        this.blockPosition = blockPosition;
        this.blockMaterial = blockMaterial;

        if (blockType == BlockType.AIR ||
            blockType == BlockType.WATER)
            isTransparent = true;
        else
            isTransparent = false;
    }

    public void CreateBlock()
    {
        if (blockType == BlockType.AIR)
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
        {
            Block[,,] chunkBlocks = blockParent.GetComponent<Chunk>().chunkBlocks;
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
    }

    void CreateBlockSide(BlockSide side)
    {
        Mesh mesh = new Mesh();
        mesh = GenerateBlockSide(mesh, side);

        GameObject blockSide = new GameObject("block side");
        blockSide.transform.position = blockPosition;
        blockSide.transform.parent = blockParent.transform;

        MeshFilter meshFilter = blockSide.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshFilter.mesh = mesh;
    }

    Mesh GenerateBlockSide(Mesh mesh, BlockSide side)
    {
        switch (side)
        {
            case BlockSide.FRONT:
                mesh.vertices = new Vector3[] { vertices[4], vertices[5],
                                                vertices[1], vertices[0]};
                mesh.normals = new Vector3[] { Vector3.forward, Vector3.forward,
                                               Vector3.forward, Vector3.forward};
                mesh.uv = new Vector2[] { uv[3], uv[2], uv[0], uv[1] };
                mesh.triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case BlockSide.BACK:
                mesh.vertices = new Vector3[] { vertices[6], vertices[7],
                                                vertices[3], vertices[2]};
                mesh.normals = new Vector3[] { Vector3.back, Vector3.back,
                                               Vector3.back, Vector3.back};
                mesh.uv = new Vector2[] { uv[3], uv[2], uv[0], uv[1] };
                mesh.triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case BlockSide.LEFT:
                mesh.vertices = new Vector3[] { vertices[7], vertices[4],
                                                vertices[0], vertices[3]};
                mesh.normals = new Vector3[] { Vector3.left, Vector3.left,
                                               Vector3.left, Vector3.left};
                mesh.uv = new Vector2[] { uv[3], uv[2], uv[0], uv[1] };
                mesh.triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case BlockSide.RIGHT:
                mesh.vertices = new Vector3[] { vertices[5], vertices[6],
                                                vertices[2], vertices[1]};
                mesh.normals = new Vector3[] { Vector3.right, Vector3.right,
                                               Vector3.right, Vector3.right};
                mesh.uv = new Vector2[] { uv[3], uv[2], uv[0], uv[1] };
                mesh.triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case BlockSide.TOP:
                mesh.vertices = new Vector3[] { vertices[7], vertices[6],
                                                vertices[5], vertices[4]};
                mesh.normals = new Vector3[] { Vector3.up, Vector3.up,
                                               Vector3.up, Vector3.up};
                mesh.uv = new Vector2[] { uv[3], uv[2], uv[0], uv[1] };
                mesh.triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case BlockSide.BOTTOM:
                mesh.vertices = new Vector3[] { vertices[0], vertices[1],
                                                vertices[2], vertices[3]};
                mesh.normals = new Vector3[] {Vector3.down, Vector3.down,
                                              Vector3.down, Vector3.down};
                mesh.uv = new Vector2[] { uv[3], uv[2], uv[0], uv[1] };
                mesh.triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
        }

        return mesh;
    }
}
