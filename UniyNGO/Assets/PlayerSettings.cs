using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>(
        "Player: 0",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        // Subscribe to changes in the networkPlayerName to update the UI
        networkPlayerName.OnValueChanged += OnPlayerNameChanged;

        if (IsServer)
        {
            // Assign a unique name to each player based on their Client ID
            networkPlayerName.Value = "Player: " + (OwnerClientId + 1);
        }
        else
        {
            // Request the server to assign the player name
            RequestPlayerNameServerRpc();
        }

        // Update the UI immediately with the current value (in case it was set before spawn)
        OnPlayerNameChanged(default, networkPlayerName.Value);
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnValueChanged event to avoid memory leaks
        networkPlayerName.OnValueChanged -= OnPlayerNameChanged;
    }

    private void OnPlayerNameChanged(FixedString128Bytes oldName, FixedString128Bytes newName)
    {
        // Update the UI with the new name
        playerName.text = newName.ToString();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPlayerNameServerRpc(ServerRpcParams rpcParams = default)
    {
        // Assign the player name on the server for the requesting client
        networkPlayerName.Value = "Player: " + (OwnerClientId + 1);
    }
}
