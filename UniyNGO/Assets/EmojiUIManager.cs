using UnityEngine;
using Unity.Netcode;
using TMPro;

public class EmojiUIManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI emojiDisplay; // Assign the TMP Text component
    private NetworkVariable<int> currentEmojiIndex = new NetworkVariable<int>(0); // 0: No emoji, 1: Emoji1, 2: Emoji2

    private readonly string[] emojis = { "", "😊", "😂" }; // Add more emojis as needed

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetEmojiServerRpc(1); // Display Emoji 1
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetEmojiServerRpc(2); // Display Emoji 2
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetEmojiServerRpc(0); // Clear Emoji
        }
    }

    private void OnEnable()
    {
        currentEmojiIndex.OnValueChanged += UpdateEmojiUI;
    }

    private void OnDisable()
    {
        currentEmojiIndex.OnValueChanged -= UpdateEmojiUI;
    }

    [ServerRpc]
    private void SetEmojiServerRpc(int emojiIndex)
    {
        currentEmojiIndex.Value = emojiIndex;
    }

    private void UpdateEmojiUI(int oldIndex, int newIndex)
    {
        emojiDisplay.text = emojis[newIndex];
    }
}
