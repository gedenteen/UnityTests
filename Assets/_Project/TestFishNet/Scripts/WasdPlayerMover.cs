using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WasdPlayerMover : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 6f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        float x = 0f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;

        float z = 0f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;
        if (Input.GetKey(KeyCode.W)) z += 1f;

        Vector3 dir = new Vector3(x, 0f, z);
        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        Vector3 delta = dir * (_moveSpeed * Time.fixedDeltaTime);
        Vector3 newPosition = _rigidbody.position + delta;
        newPosition.y = _rigidbody.position.y;
        _rigidbody.MovePosition(newPosition);
    }
}