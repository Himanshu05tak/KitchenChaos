using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeBtn;

    private void Awake()
    {
        closeBtn.onClick.AddListener(Hide);
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinLobbyStarted += KitchenGameLobby_OnJoinLobbyStarted;
        KitchenGameLobby.Instance.OnJoinLobbyFailed += KitchenGameLobby_OnJoinLobbyFailed;
        KitchenGameLobby.Instance.OnQuickJoinLobbyFailed += KitchenGameLobby_OnQuickJoinLobbyFailed;

        Hide();
    }

    private void KitchenGameLobby_OnQuickJoinLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Couldn't find a Lobby to Quick join!");
    }

    private void KitchenGameLobby_OnJoinLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to join Lobby!");
    }

    private void KitchenGameLobby_OnJoinLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Joining Lobby...");
    }

    private void KitchenGameLobby_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to create Lobby!");
    }

    private void KitchenGameLobby_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        ShowMessage(NetworkManager.Singleton.DisconnectReason == ""
            ? "Failed to connect"
            : NetworkManager.Singleton.DisconnectReason);
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
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
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinLobbyStarted -= KitchenGameLobby_OnJoinLobbyStarted;
        KitchenGameLobby.Instance.OnJoinLobbyFailed -= KitchenGameLobby_OnJoinLobbyFailed;
        KitchenGameLobby.Instance.OnQuickJoinLobbyFailed -= KitchenGameLobby_OnQuickJoinLobbyFailed;
    }
}
