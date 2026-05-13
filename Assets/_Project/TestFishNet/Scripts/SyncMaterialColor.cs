using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SyncMaterialColor : NetworkBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    public readonly SyncVar<Color> Color = new SyncVar<Color>();

    private void Awake()
    {
        Color.OnChange += OnColorChanged;
    }

    private void OnDestroy()
    {
        Color.OnChange -= OnColorChanged;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeToRandomColor();
        }
    }

    [ServerRpc]
    private void ChangeToRandomColor()
    {
        Color.Value = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        Debug.Log($"SyncMaterialColor: ChangeToRandomColor: {Color.Value}");
    }

    private void OnColorChanged(Color previous, Color next, bool asServer)
    {
        _meshRenderer.material.color = Color.Value;
    }
}