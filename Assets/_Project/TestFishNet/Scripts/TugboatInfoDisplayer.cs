using System;
using FishNet.Transporting.Tugboat;
using TMPro;
using UnityEngine;

public class TugboatInfoDisplayer : MonoBehaviour
{
    [SerializeField] private Tugboat _tugboat;
    [SerializeField] private TMP_Text _textIp;
    [SerializeField] private TMP_Text _textPort;

    private void Start()
    {
        _textIp.text = "IP " + _tugboat.GetClientAddress().ToString();
        _textPort.text = "Port " + _tugboat.GetPort().ToString();
    }
}
