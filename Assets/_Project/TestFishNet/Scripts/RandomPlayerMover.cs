using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RandomPhysicsPlayerMover : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _changeDirectionInterval = 1f;

    private Rigidbody _rigidbody;
    private Vector3 _direction;
    private float _timer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Каждый клиент управляет только своим игроком.
        if (!IsOwner)
            return;

        _timer -= Time.fixedDeltaTime;
        if (_timer <= 0f)
            PickRandomDirection();

        Vector3 movement = _direction * (_moveSpeed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + movement);
    }

    private void PickRandomDirection()
    {
        _timer = _changeDirectionInterval;

        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        _direction = new Vector3(randomCircle.x, 0f, randomCircle.y);
    }
}