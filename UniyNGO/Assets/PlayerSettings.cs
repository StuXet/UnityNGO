using Unity.Netcode;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerScoreUI; // Assign in the inspector
    private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player: 0");
    private NetworkVariable<int> playerScore = new NetworkVariable<int>(0); // Player's score

    public override void OnNetworkSpawn()
    {
        networkPlayerName.OnValueChanged += OnPlayerNameChanged;
        playerScore.OnValueChanged += OnScoreChanged;

        if (IsServer)
        {
            networkPlayerName.Value = "Player: " + (OwnerClientId + 1);
        }

        OnPlayerNameChanged(default, networkPlayerName.Value);
        OnScoreChanged(0, playerScore.Value);
    }

    private void OnDestroy()
    {
        networkPlayerName.OnValueChanged -= OnPlayerNameChanged;
        playerScore.OnValueChanged -= OnScoreChanged;
    }

    private void OnPlayerNameChanged(FixedString128Bytes oldName, FixedString128Bytes newName)
    {
        playerName.text = newName.ToString();
    }

    private void OnScoreChanged(int oldScore, int newScore)
    {
        playerScoreUI.text = "Score: " + newScore;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc()
    {
        playerScore.Value++;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnPlayerServerRpc(Vector3 spawnPosition)
    {
        // Update position on the server
        transform.position = spawnPosition;

        // Notify all clients about the position change
        RespawnPlayerClientRpc(spawnPosition);
    }

    [ClientRpc]
    private void RespawnPlayerClientRpc(Vector3 spawnPosition)
    {
        if (IsOwner) // Only update the client that owns this player
        {
            transform.position = spawnPosition;
        }
    }

}