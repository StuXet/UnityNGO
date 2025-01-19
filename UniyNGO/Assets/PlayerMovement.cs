using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float positionRange = 3f;

    void Start()
    {
        // Reference to the animator
    }
    public override void OnNetworkSpawn()
    {
        UpdatePositionServerRpc();
    }

    void Update()
    {
        if (!IsOwner) return;
        // Cache input values in floats
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Create movement direction Vector3 and store the vertical and horizontal values in it
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        movementDirection.Normalize();

        // Move the transform in the movement direction
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime, Space.World);

        // Rotate the player to face the movement direction
        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePositionServerRpc()
    {
        transform.position = new Vector3(Random.Range(positionRange, -positionRange), 1, Random.Range(positionRange, -positionRange));
        transform.rotation = new Quaternion(0, 180, 0, 0);
    }    
}
