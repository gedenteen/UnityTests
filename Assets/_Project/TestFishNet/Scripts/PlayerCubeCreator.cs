using System;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCubeCreator : NetworkBehaviour
{
    public NetworkObject MySpawnableObject;
    public SyncMaterialColor MySyncMaterialColor;
    public float SpawnRate = 0.5f;

    private float _timer = 0;

    private void Start()
    {
        MySyncMaterialColor.Color.Value = Random.ColorHSV(0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
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
        NetworkObject networkObject = Instantiate(MySpawnableObject, transform.position, Quaternion.identity);
        networkObject.GetComponent<SyncMaterialColor>().Color.Value = MySyncMaterialColor.Color.Value;
        Spawn(networkObject); // NetworkBehaviour shortcut for ServerManager.Spawn(obj);
    }
}