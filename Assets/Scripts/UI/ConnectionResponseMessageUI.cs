using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeBtn;

    private void Awake()
    {
        closeBtn.onClick.AddListener(Hide);
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayerOnFailedToJoinGame;

        Hide();
    }

    private void KitchenGameMultiplayerOnFailedToJoinGame(object sender, EventArgs e)
    {
        Show();

        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if (messageText.text == "")
            messageText.text = "Failed to Connect!";
    }
    private void Show()
    {
        gameObject.SetActive(true);   
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayerOnFailedToJoinGame;
    }
}
