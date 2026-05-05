using System;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCubeCreator : NetworkBehaviour
{
    public NetworkObject CubePrefab;
    public SyncMaterialColor MySyncMaterialColor;
    public float SpawnRate = 0.5f;

    private float _timer = 0;

    private void Start()
    {
        MySyncMaterialColor.Color.Value = Random.ColorHSV();
    }

    private void Update()
    {
        // Only the local player object should perform these actions.
        if (!IsOwner)
            return;

        _timer += Time.deltaTime;
        if (_timer >= SpawnRate)
        {
            SpawnCube();
            _timer -= SpawnRate;
        }
    }

    // We are using a ServerRpc here because the Server needs to do all network object spawning.
    [ServerRpc]
    private void SpawnCube()
    {
        NetworkObject obj = Instantiate(CubePrefab, transform.position, Quaternion.identity);
        obj.GetComponent<SyncMaterialColor>().Color.Value = Random.ColorHSV();
        Spawn(obj); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
    }
}