using System;
using FishNet;
using FishNet.Managing;
using UnityEngine;
using UnityEngine.UI;

public class UiForPlayer : MonoBehaviour
{
    [SerializeField] private Button _buttonDisconnect;

    private void Awake()
    {
        if (!InstanceFinder.NetworkManager.IsClientStarted)
            Destroy(gameObject);

        _buttonDisconnect.onClick.AddListener(Disconnect);
    }

    private void OnDestroy()
    {
        _buttonDisconnect.onClick.RemoveListener(Disconnect);
    }

    private void Disconnect()
    {
        InstanceFinder.ClientManager.StopConnection();
        Debug.Log($"Player disconnected");
    }
}
