using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class World : MonoBehaviour
{
    public Texture2D[] atlasTextures;
    public static Dictionary<string, Rect> atlasDictionary = new Dictionary<string, Rect>();
    public static Dictionary<string, Chunk> chunks = new Dictionary<string, Chunk>();
    public int columnHeight = 16;
    public int chunkSize = 16;
    public int worldRadius = 2;
    Material blockMaterial;

    GameObject player;
    Vector2 lastPlayerPosition;
    Vector2 currentPlayerPosition;

    public static List<BlockType> blockTypes = new List<BlockType>();

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        UpdatePlayerPosition();
    }

    void Start()
    {
        Texture2D atlas = GetTextureAtlas();
        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = atlas;
        blockMaterial = material;
        ChunkUtils.GenerateRandomOffset();
        GenerateBlockTypes();
        GenerateWorld();
        StartCoroutine(BuildWorld(true));
    }

    private void Update()
    {
        UpdatePlayerPosition();

        if (lastPlayerPosition != currentPlayerPosition)
        {
            lastPlayerPosition = currentPlayerPosition;
            GenerateWorld();
            StartCoroutine(BuildWorld());
        }
    }

    IEnumerator BuildWorld(bool isFirst = false)
    {
        foreach (Chunk chunk in chunks.Values.ToList())
        {
            if (chunk.status == Chunk.chunkStatus.TO_DRAW)
            {
                chunk.DrawChunk(chunkSize);
            }

            yield return null;
        }

        if (isFirst)
        {
            player.SetActive(true);
        }
    }

    void GenerateWorld()
    {
        for (int z = -worldRadius + (int)currentPlayerPosition.y - 1; z <= worldRadius + (int)currentPlayerPosition.y + 1; z++)
        {
            for (int x = -worldRadius + (int)currentPlayerPosition.x - 1; x <= worldRadius + (int)currentPlayerPosition.x + 1; x++)
            {
                for (int y = 0; y < columnHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    string chunkName = GenerateChunkName(chunkPosition);
                    Chunk chunk;

                    if (z == -worldRadius + (int)currentPlayerPosition.y - 1 || z == worldRadius + (int)currentPlayerPosition.y + 1
                        || x == -worldRadius + (int)currentPlayerPosition.x - 1 || x == worldRadius + (int)currentPlayerPosition.x + 1)
                    {
                        if (chunks.TryGetValue(chunkName, out chunk))
                        {
                            chunk.status = Chunk.chunkStatus.GENERATED;
                            Destroy(chunk.chunkObject);
                        }

                        continue;
                    }

                    if (chunks.TryGetValue(chunkName, out chunk))
                    {
                        if (chunk.status == Chunk.chunkStatus.GENERATED)
                        {
                            chunk.RefreshChunk(chunkName, chunkPosition);
                            chunk.chunkObject.transform.parent = this.transform;
                        }
                    }
                    else
                    {
                        chunk = new Chunk(chunkName, chunkPosition, blockMaterial);
                        chunk.chunkObject.transform.parent = this.transform;
                        chunks.Add(chunkName, chunk);
                    }
                }
            }
        }
    }

    void GenerateBlockTypes()
    {
        BlockType air = new BlockType("air", true, true);
        air.sideUV = SetBlockTypeUV("air");
        air.GenerateBlockUVs();
        blockTypes.Add(air);

        BlockType dirt = new BlockType("dirt", false, true);
        dirt.sideUV = SetBlockTypeUV("dirt");
        dirt.GenerateBlockUVs();
        blockTypes.Add(dirt);

        BlockType brick = new BlockType("brick", false, true);
        brick.sideUV = SetBlockTypeUV("brick");
        brick.GenerateBlockUVs();
        blockTypes.Add(brick);

        BlockType grass = new BlockType("grass", false, false);
        grass.topUV = SetBlockTypeUV("grass");
        grass.sideUV = SetBlockTypeUV("grass_side");
        grass.bottomUV = SetBlockTypeUV("dirt");
        grass.GenerateBlockUVs();
        blockTypes.Add(grass);

        BlockType stone = new BlockType("stone", false, true);
        stone.sideUV = SetBlockTypeUV("stone");
        stone.GenerateBlockUVs();
        blockTypes.Add(stone);
    }

    Vector2[] SetBlockTypeUV(string name)
    {
        if (name == "air")
        {
            return new Vector2[4] { new Vector2(0f, 0f),
                                    new Vector2(1f, 0f),
                                    new Vector2(0f, 1f),
                                    new Vector2(1f, 1f)};
        }

        return new Vector2[4]
            {
                new Vector2(atlasDictionary[name].x,
                            atlasDictionary[name].y),

                new Vector2(atlasDictionary[name].x + atlasDictionary[name].width,
                            atlasDictionary[name].y),

                new Vector2(atlasDictionary[name].x,
                            atlasDictionary[name].y + atlasDictionary[name].height),

                new Vector2(atlasDictionary[name].x + atlasDictionary[name].width,
                            atlasDictionary[name].y + atlasDictionary[name].height)
            };
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

    void UpdatePlayerPosition()
    {
        currentPlayerPosition.x = Mathf.Floor(player.transform.position.x / 16);
        currentPlayerPosition.y = Mathf.Floor(player.transform.position.z / 16);
    }
}