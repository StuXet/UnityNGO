using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ShootProjectile : NetworkBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private List<GameObject> spawnedProjectiles = new List<GameObject>();

    private InputSystem_Actions inputActions;

    private void OnEnable()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        // Bind the Shoot action to the method
        inputActions.Player.Attack.performed += _ => ShootServerRpc();
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= _ => ShootServerRpc();
        inputActions.Disable();
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
            moveProjectile.parent = this; // Set reference to this ShootProjectile instance
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
            spawnedProjectiles.Remove(toDestroy);
            toDestroy.GetComponent<NetworkObject>().Despawn();
            Destroy(toDestroy);
        }
    }
}
