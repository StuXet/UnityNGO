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
            rb.linearVelocity = transform.forward * shootForce;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("Player"))
        {
            HandlePlayerHitServerRpc(other.gameObject.GetComponent<NetworkObject>().OwnerClientId);
        }

        InstantiateHitParticlesServerRpc();
        parent.DestroyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandlePlayerHitServerRpc(ulong targetClientId)
    {
        var targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject.GetComponent<PlayerSettings>();
        var killerPlayer = parent.GetComponent<PlayerSettings>();

        // Add score to killer
        killerPlayer.AddScoreServerRpc();

        // Respawn the target player
        Vector3 respawnPosition = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
        targetPlayer.RespawnPlayerServerRpc(respawnPosition);
    }

    [ServerRpc]
    private void InstantiateHitParticlesServerRpc()
    {
        GameObject hitImpact = Instantiate(hitParticles, transform.position, Quaternion.identity);
        hitImpact.GetComponent<NetworkObject>().Spawn();
    }
}
