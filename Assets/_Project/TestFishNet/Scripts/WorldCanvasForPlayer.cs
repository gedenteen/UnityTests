using System;
using FishNet;
using UnityEngine;
using UnityEngine.UI;

public class WorldCanvasForPlayer : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private bool _lockYAxis;
    [SerializeField] private Button _buttonDespawn;

    private void Awake()
    {
        _buttonDespawn.onClick.AddListener(Despawn);
    }

    private void Start()
    {
        _targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!_targetCamera)
            _targetCamera = Camera.main;
        if (!_targetCamera)
            return;

        Vector3 toCamera = _targetCamera.transform.position - transform.position;

        if (_lockYAxis)
        {
            toCamera.y = 0f;
            if (toCamera.sqrMagnitude < 1e-6f)
                return;
        }

        Vector3 up = _lockYAxis ? Vector3.up : _targetCamera.transform.up;
        transform.rotation = Quaternion.LookRotation(-toCamera, up);
    }

    private void OnDestroy()
    {
        _buttonDespawn.onClick.RemoveListener(Despawn);
    }

    private void Despawn()
    {
        InstanceFinder.ServerManager.Despawn(transform.parent.gameObject);
    }
}
