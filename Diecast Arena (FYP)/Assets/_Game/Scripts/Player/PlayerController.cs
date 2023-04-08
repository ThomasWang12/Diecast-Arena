using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour {
    [SerializeField] private float _speed = 3;
    [SerializeField] private bool use = false; // #%
    private Rigidbody _rb;

    private void Awake() {
        if (!use) return;

        _rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (!use) return;

        var dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _rb.velocity = dir * _speed;
    }

    public override void OnNetworkSpawn() {
        if (!IsOwner) Destroy(this);
    }
}