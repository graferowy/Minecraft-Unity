using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Block[,,] chunkBlocks;
    public GameObject chunkObject;
    Material blockMaterial;
    public enum chunkStatus { GENERATED, DRAWN, TO_DRAW };
    public chunkStatus status;

    public Chunk(string name, Vector3 position, Material material)
    {
        this.chunkObject = new GameObject(name);
        this.chunkObject.transform.position = position;
        this.blockMaterial = material;
        this.status = chunkStatus.GENERATED;
        GenerateChunk(16);
    }

    void GenerateChunk(int chunkSize)
    {
        chunkBlocks = new Block[chunkSize, chunkSize, chunkSize];

        for (int z = 0; z < chunkSize; z++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float worldX = x + chunkObject.transform.position.x;
                    float worldY = y + chunkObject.transform.position.y;
                    float worldZ = z + chunkObject.transform.position.z;
                    float caveProbability = ChunkUtils.PerlinNoise3D(worldX, worldY, worldZ);
                    int generated1stLayerY = (int)ChunkUtils.Generate1stLayerHeight(worldX, worldZ);
                    int generated2ndLayerY = (int)ChunkUtils.Generate2ndLayerHeight(worldX, worldZ, generated1stLayerY);

                    if (worldY == generated1stLayerY)
                        chunkBlocks[x, y, z] = new Block(World.blockTypes[3], this, new Vector3(x, y, z));
                    else if (caveProbability > 0.5f && worldY < generated1stLayerY - 5)
                        chunkBlocks[x, y, z] = new Block(World.blockTypes[0], this, new Vector3(x, y, z));
                    else if (worldY < generated2ndLayerY)
                        chunkBlocks[x, y, z] = new Block(World.blockTypes[4], this, new Vector3(x, y, z));
                    else if (worldY < generated1stLayerY)
                        chunkBlocks[x, y, z] = new Block(World.blockTypes[1], this, new Vector3(x, y, z));
                    else
                    {
                        this.status = chunkStatus.TO_DRAW;
                        chunkBlocks[x, y, z] = new Block(World.blockTypes[0], this, new Vector3(x, y, z));
                    }
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
        MeshFilter[] meshFilters = chunkObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineSides = new CombineInstance[meshFilters.Length];

        int index = 0;
        foreach (MeshFilter meshFilter in meshFilters)
        {
            combineSides[index].mesh = meshFilter.sharedMesh;
            combineSides[index].transform = meshFilter.transform.localToWorldMatrix;
            index++;
        }

        MeshFilter blockMeshFilter = chunkObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        blockMeshFilter.mesh = new Mesh();
        blockMeshFilter.mesh.CombineMeshes(combineSides);

        MeshRenderer blockMeshRenderer = chunkObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        blockMeshRenderer.material = blockMaterial;

        chunkObject.AddComponent(typeof(MeshCollider));

        foreach (Transform side in chunkObject.transform)
        {
            GameObject.Destroy(side.gameObject);
        }
    }
}