using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
   [SerializeField] private int playerIndex;
   [SerializeField] private PlayerVisual playerVisual;
   [SerializeField] private GameObject readyGameObject;
   [SerializeField] private Button kickBtn;
   [SerializeField] private TextMeshPro playerName;

   private void Awake()
   {
       kickBtn.onClick.AddListener(() =>
       {
           var playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
           KitchenGameLobby.Instance.KickPlayer(playerData.PlayerID.ToString());
           KitchenGameMultiplayer.Instance.KickPlayer(playerData.ClientId);
       });
   }

   private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChange += KitchenGameMultiplayer_OnPlayerDataNetworkListChange;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        
        kickBtn.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChange(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            var playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.ClientId));
            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.ColorId));
            playerName.text = playerData.PlayerName.ToString();
        }
        else
        {
            Hide();
        }
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
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChange -= KitchenGameMultiplayer_OnPlayerDataNetworkListChange;
    }
}
