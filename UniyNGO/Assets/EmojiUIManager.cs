using UnityEngine;
using Unity.Netcode;
using TMPro;

public class EmojiUIManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI emojiDisplay;
    private NetworkVariable<int> currentEmojiIndex = new NetworkVariable<int>(0);
    private InputSystem_Actions inputActions;

    private readonly string[] emojis = { "", "😊", "😂" };

    private void OnEnable()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        inputActions.UI.Num1.performed += _ => SetEmojiServerRpc(1);
        inputActions.UI.Num2.performed += _ => SetEmojiServerRpc(2);
        inputActions.UI.Num3.performed += _ => SetEmojiServerRpc(0);

        currentEmojiIndex.OnValueChanged += UpdateEmojiUI;
    }

    private void OnDisable()
    {
        inputActions.Disable();
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
