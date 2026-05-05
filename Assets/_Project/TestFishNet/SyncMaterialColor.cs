using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
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

    private void OnColorChanged(Color previous, Color next, bool asServer)
    {
        _meshRenderer.material.color = Color.Value;
    }
}