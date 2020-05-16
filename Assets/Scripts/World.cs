using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Texture2D[] atlasTextures;
    public static Dictionary<string, Rect> atlasDictionary = new Dictionary<string, Rect>();
    public static Dictionary<string, Chunk> chunks = new Dictionary<string, Chunk>();
    public int columnHeight = 16;
    public int chunkSize = 16;
    public int worldSize = 5;
    Material blockMaterial;

    void Start()
    {
        Texture2D atlas = GetTextureAtlas();
        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = atlas;
        blockMaterial = material;
        StartCoroutine(BuildWorld());
    }

    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldSize; z++)
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < columnHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    string chunkName = GenerateChunkName(chunkPosition);
                    Chunk chunk = new Chunk(chunkName, chunkPosition, blockMaterial);
                    chunk.chunkObject.transform.parent = this.transform;
                    chunks.Add(chunkName, chunk);
                }
            }
        }

        foreach (KeyValuePair<string, Chunk> chunk in chunks)
        {
            chunk.Value.DrawChunk(chunkSize);

            yield return null;
        }
    }

    string GenerateChunkName(Vector3 chunkPosition)
    {
        return (int)chunkPosition.x + "_" +
            (int)chunkPosition.y + "_" +
            (int)chunkPosition.z;
    }

    Texture2D GetTextureAtlas()
    {
        Texture2D textureAtlas = new Texture2D(8192, 8192);
        Rect[] rectCoordinates = textureAtlas.PackTextures(atlasTextures, 0, 8192, false);
        textureAtlas.Apply();

        for (int i = 0; i < rectCoordinates.Length; i++)
        {
            atlasDictionary.Add(atlasTextures[i].name.ToLower(), rectCoordinates[i]);
        }

        return textureAtlas;
    }
}