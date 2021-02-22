using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 PlayerPosition => _playerPosition.ToVector3();
    public int FirstLayerOffset { get; private set; }
    public int SecondLayerOffset { get; private set; }
    public int TypeOffset { get; private set; }
    public int MoistureOffset { get; private set; }
    public int TemperatureOffset { get; private set; }
    public List<ChunkData> ChunkData { get; private set; }
    private SerializableVector3 _playerPosition;

    public GameData(Vector3 playerPosition)
    {
        _playerPosition = new SerializableVector3(playerPosition);
        FirstLayerOffset = ChunkUtils.firstLayerOffset;
        SecondLayerOffset = ChunkUtils.secondLayerOffset;
        TypeOffset = ChunkUtils.typeOffset;
        MoistureOffset = ChunkUtils.moistureOffset;
        TemperatureOffset = ChunkUtils.temperatureOffset;
        ChunkData = new List<ChunkData>();
        BuildChunkData();
    }

    private void BuildChunkData()
    {
        foreach (var chunk in World.chunks)
        {
            var chunkData = new ChunkData(chunk.Key, chunk.Value.GetBlockTypes());
            ChunkData.Add(chunkData);
        }
    }
}
