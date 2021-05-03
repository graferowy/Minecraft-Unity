using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private BlockType.Type selectedBlockType;
    [SerializeField]
    private LayerMask _masksToIgnore;
    [SerializeField]
    private int _interactionRange;
    private Camera _camera;
    private World _world;

    private void Awake()
    {
        _camera = Camera.main;
        _world = GameObject.Find("World").GetComponent<World>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BuildBlock();
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            RemoveBlock();
        }
    }

    private void BuildBlock()
    {
        RaycastHit hit;

        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, _interactionRange, ~_masksToIgnore))
        {
            Vector3 blockPos = hit.point + hit.normal / 2.0f;
            Vector3 chunkPos = hit.transform.position;
            int blockPosX = (int)(Mathf.Round(blockPos.x) - chunkPos.x);
            int blockPosY = (int)(Mathf.Round(blockPos.y) - chunkPos.y);
            int blockPosZ = (int)(Mathf.Round(blockPos.z) - chunkPos.z);
            Vector3 newBlockPos = new Vector3(blockPosX, blockPosY, blockPosZ);
            ValidatePositions(hit.normal, ref newBlockPos, ref chunkPos);
            _world.BuildBlock(World.GenerateChunkName(chunkPos), newBlockPos, selectedBlockType);
        }
    }

    private void RemoveBlock()
    {
        RaycastHit hit;

        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, _interactionRange, ~_masksToIgnore))
        {
            Vector3 blockPos = hit.point - hit.normal / 2.0f;
            Vector3 chunkPos = hit.transform.position;
            int blockPosX = (int)(Mathf.Round(blockPos.x) - chunkPos.x);
            int blockPosY = (int)(Mathf.Round(blockPos.y) - chunkPos.y);
            int blockPosZ = (int)(Mathf.Round(blockPos.z) - chunkPos.z);
            _world.DestroyBlock(World.GenerateChunkName(chunkPos), new Vector3(blockPosX, blockPosY, blockPosZ));
        }
    }

    private void ValidatePositions(Vector3 normal, ref Vector3 blockPos, ref Vector3 chunkPos)
    {
        if (normal.x > 0 && blockPos.x >= World.chunkSize)
        {
            blockPos.x = 0;
            chunkPos.x += World.chunkSize;
        }
        
        if (normal.x < 0 && blockPos.x < 0)
        {
            blockPos.x = World.chunkSize;
            chunkPos.x -= World.chunkSize;
        }
        
        if (normal.y > 0 && blockPos.y >= World.chunkSize)
        {
            blockPos.y = 0;
            chunkPos.y += World.chunkSize;
        }
        
        if (normal.y < 0 && blockPos.x < 0)
        {
            blockPos.y = World.chunkSize;
            chunkPos.y -= World.chunkSize;
        }
        
        if (normal.z > 0 && blockPos.z >= World.chunkSize)
        {
            blockPos.z = 0;
            chunkPos.z += World.chunkSize;
        }
        
        if (normal.z < 0 && blockPos.z < 0)
        {
            blockPos.z = World.chunkSize;
            chunkPos.z -= World.chunkSize;
        }
    }
}
