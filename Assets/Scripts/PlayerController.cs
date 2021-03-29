using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
        if (Input.GetMouseButtonDown(1))
        {
            RemoveBlock();
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
            _world.DestroyBlock(hit.transform.name, new Vector3(blockPosX, blockPosY, blockPosZ));
        }
    }
}
