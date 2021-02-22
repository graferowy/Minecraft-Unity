﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk
{
    public Block[,,] chunkBlocks;
    public GameObject chunkObject;
    Material[] blockMaterial;
    public enum chunkStatus { GENERATED, DRAWN, TO_DRAW };
    public chunkStatus status;
    public int VertexIndex { get; set; }

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<int> transparentTriangles = new List<int>();
    public List<int> waterTriangles = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();

    public Chunk(string name, Vector3 position, Material[] material)
    {
        this.chunkObject = new GameObject(name);
        this.chunkObject.transform.position = position;
        this.blockMaterial = material;
        this.status = chunkStatus.GENERATED;
        GenerateChunk(16);
    }
    
    public Chunk(string name, Vector3 position, Material[] material, BlockType.Type[,,] blockTypes)
    {
        this.chunkObject = new GameObject(name);
        this.chunkObject.transform.position = position;
        this.blockMaterial = material;
        this.status = chunkStatus.GENERATED;
        GenerateLoadedChunk(16, blockTypes);
    }

    void GenerateChunk(int chunkSize)
    {
        chunkBlocks = new Block[chunkSize, chunkSize, chunkSize];
        Biome biome = BiomeUtils.SelectBiome(this.chunkObject.transform.position);

        for (int z = 0; z < chunkSize; z++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float worldX = x + chunkObject.transform.position.x;
                    float worldY = y + chunkObject.transform.position.y;
                    float worldZ = z + chunkObject.transform.position.z;
                    BlockType biomeBlock = biome.GenerateTerrain(worldX, worldY, worldZ);
                    chunkBlocks[x, y, z] = new Block(biomeBlock, this, new Vector3(x, y, z));

                    if (biomeBlock == World.blockTypes[BlockType.Type.AIR])
                            this.status = chunkStatus.TO_DRAW;
                }
            }
        }

        if (status == chunkStatus.TO_DRAW)
        {
            string chunkName = (int)this.chunkObject.transform.position.x + "_" + ((int)this.chunkObject.transform.position.y - 16) + "_" + (int)this.chunkObject.transform.position.z;
            Chunk chunkBelow;

            if (World.chunks.TryGetValue(chunkName, out chunkBelow))
            {
                chunkBelow.status = chunkStatus.TO_DRAW;
            }
        }
    }
    
    void GenerateLoadedChunk(int chunkSize, BlockType.Type[,,] blockTypes)
    {
        chunkBlocks = new Block[chunkSize, chunkSize, chunkSize];

        for (int z = 0; z < chunkSize; z++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    chunkBlocks[x, y, z] = new Block(World.blockTypes[blockTypes[x, y, z]], this, new Vector3(x, y, z));
                }
            }
        }
    }

    public void RefreshChunk(string name, Vector3 position)
    {
        this.chunkObject = new GameObject(name);
        this.chunkObject.transform.position = position;

        foreach (Block block in chunkBlocks)
        {
            if (block.GetBlockType() == World.blockTypes[0])
            {
                this.status = chunkStatus.TO_DRAW;

                string chunkName = (int)this.chunkObject.transform.position.x + "_" + ((int)this.chunkObject.transform.position.y - 16) + "_" + (int)this.chunkObject.transform.position.z;
                Chunk chunkBelow;

                if (World.chunks.TryGetValue(chunkName, out chunkBelow))
                {
                    chunkBelow.status = chunkStatus.TO_DRAW;
                }

                break;
            }
        }
    }

    public void DrawChunk(int chunkSize)
    {
        VertexIndex = 0;
        
        for (int z = 0; z < chunkSize; z++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    chunkBlocks[x, y, z].CreateBlock();
                }
            }
        }

        CombineSides();

        this.status = chunkStatus.DRAWN;
    }

    void CombineSides()
    {
        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetTriangles(transparentTriangles.ToArray().Concat(waterTriangles.ToArray()).ToArray(), 1);
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        
        var colliderMesh = new Mesh();
        colliderMesh.vertices = mesh.vertices;
        colliderMesh.subMeshCount = 2;
        colliderMesh.SetTriangles(triangles.ToArray(), 0);
        colliderMesh.SetTriangles(transparentTriangles.ToArray(), 1);

        MeshFilter blockMeshFilter = chunkObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        blockMeshFilter.mesh = mesh;

        MeshRenderer blockMeshRenderer = chunkObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        blockMeshRenderer.materials = blockMaterial;
        
        MeshCollider blockMeshCollider = chunkObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        blockMeshCollider.sharedMesh = colliderMesh;

        foreach (Transform side in chunkObject.transform)
        {
            GameObject.Destroy(side.gameObject);
        }
    }

    public BlockType.Type[,,] GetBlockTypes()
    {
        var arraySize = chunkBlocks.GetLength(0);
        var blockTypes = new BlockType.Type[arraySize, arraySize, arraySize];

        for (int i = 0; i < arraySize; i++)
        {
            for (int j = 0; j < arraySize; j++)
            {
                for (int k = 0; k < arraySize; k++)
                {
                    blockTypes[i, j, k] = chunkBlocks[i, j, k].GetBlockType().blockType;
                }
            }
        }

        return blockTypes;
    }
}