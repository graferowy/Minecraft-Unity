using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveFile();
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            LoadFile();
        }
    }

    private void SaveFile()
    {
        string fileName = Application.persistentDataPath + "/save.dat";
        Debug.Log(fileName);
        FileStream file;

        if (File.Exists(fileName))
        {
            file = File.OpenWrite(fileName);
        }
        else
        {
            file = File.Create(fileName);
        }

        GameData data = new GameData(GetPlayerPosition());
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(file, data);
        file.Close();
    }

    private void LoadFile()
    {
        string fileName = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(fileName))
        {
            file = File.OpenRead(fileName);
        }
        else
        {
            Debug.LogError("Save file not found!");
            return;
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        GameData data = (GameData) binaryFormatter.Deserialize(file);
        file.Close();
        
        SetPlayerPosition(data.PlayerPosition);
        SetOffsets(data);
        SetChunksData(data);
    }
    
    private Vector3 GetPlayerPosition()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        return player.transform.position;
    }

    private void SetPlayerPosition(Vector3 position)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = position;
    }

    private void SetOffsets(GameData data)
    {
        ChunkUtils.firstLayerOffset = data.FirstLayerOffset;
        ChunkUtils.secondLayerOffset = data.SecondLayerOffset;
        ChunkUtils.typeOffset = data.TypeOffset;
        ChunkUtils.moistureOffset = data.MoistureOffset;
        ChunkUtils.temperatureOffset = data.TemperatureOffset;
    }

    private void SetChunksData(GameData data)
    {
        var chunkData = data.ChunkData;
        var world = GameObject.Find("World").GetComponent<World>();
        world.GenerateLoadedWorld(chunkData);
    }
}
