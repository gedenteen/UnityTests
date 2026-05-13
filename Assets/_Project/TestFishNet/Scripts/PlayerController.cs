using System;
using FishNet.Demo.Benchmarks.NetworkTransforms;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TestFishNetSettings _settings;
    [Space]
    [SerializeField] private RandomPhysicsPlayerMover _randomPhysicsPlayerMover;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private WasdPlayerMover _wasdPlayerMover;
    [SerializeField] private WorldCanvasForPlayer _myWorldCanvas;

    private void Awake()
    {
        switch (_settings.MovementType)
        {
            case TestMovementType.RandomNonPhysics:
                Destroy(_randomPhysicsPlayerMover);
                Destroy(_wasdPlayerMover);
                Destroy(_boxCollider);
                Destroy(_rigidbody);
                gameObject.AddComponent<MoveRandomlyNonPhysics>();
                break;
            case TestMovementType.RandomPhysics:
                Destroy(_wasdPlayerMover);
                break;
            case TestMovementType.Manual:
                Destroy(_randomPhysicsPlayerMover);
                break;
        }

        if (!_settings.IsButtonDespawnEnabled)
        {
            Destroy(_myWorldCanvas.gameObject);
        }
    }
}
