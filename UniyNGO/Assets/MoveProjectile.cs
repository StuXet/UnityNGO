using UnityEngine;
using Unity.Netcode;

public class MoveProjectile : NetworkBehaviour
{
    public ShootProjectile parent;
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private float shootForce;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * shootForce; // Set initial velocity
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        Debug.Log("Projectile hit: " + other.name);

        // Call the ServerRpc to destroy the projectile and spawn particles
        InstantiateHitParticlesServerRpc();

        // Inform the parent to destroy the projectile
        parent.DestroyServerRpc();
    }

    [ServerRpc]
    private void InstantiateHitParticlesServerRpc()
    {
        // Instantiate and spawn the hit particles
        GameObject hitImpact = Instantiate(hitParticles, transform.position, Quaternion.identity);
        hitImpact.GetComponent<NetworkObject>().Spawn();
        hitImpact.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
    }
}
