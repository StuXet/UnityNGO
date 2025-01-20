using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float positionRange = 3f;

    private InputSystem_Actions inputActions;

    private void OnEnable()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public override void OnNetworkSpawn()
    {
        UpdatePositionServerRpc();
    }

    void Update()
    {
        if (!IsOwner) return;

        // Get movement input
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 movementDirection = new Vector3(input.x, 0, input.y);
        movementDirection.Normalize();

        // Move the player
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
        transform.rotation = Quaternion.identity;
    }
}
