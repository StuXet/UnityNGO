using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ShootProjectile : NetworkBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private List<GameObject> spawnedProjectiles = new List<GameObject>();

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootServerRpc();
        }
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        Vector3 spawnPosition = shootTransform.position + shootTransform.forward * 1f; // Spawn slightly in front of the player
        GameObject go = Instantiate(projectile, spawnPosition, shootTransform.rotation);
        spawnedProjectiles.Add(go);

        var moveProjectile = go.GetComponent<MoveProjectile>();
        if (moveProjectile != null)
        {
            moveProjectile.parent = this;
        }

        go.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
        if (spawnedProjectiles.Count == 0)
        {
            Debug.LogWarning("No projectiles to destroy.");
            return;
        }

        // Safely access the first projectile and destroy it
        GameObject toDestroy = spawnedProjectiles[0];
        if (toDestroy != null)
        {
            toDestroy.GetComponent<NetworkObject>().Despawn();
            spawnedProjectiles.Remove(toDestroy);
            Destroy(toDestroy);
        }
    }
}
