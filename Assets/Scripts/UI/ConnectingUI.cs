using System;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayerOnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayerOnFailedToJoinGame;
        
        Hide();
    }

    private void KitchenGameMultiplayerOnFailedToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void KitchenGameMultiplayerOnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
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
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayerOnTryingToJoinGame;
    }
}
