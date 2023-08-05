using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameLobbyBtn;
    [SerializeField] private Button joinGameBtn;


    private void Awake()
    {
        createGameLobbyBtn.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.StartHost();
            Loader.Loader.LoadNetwork(Loader.Loader.Scene.CharacterSelectScene);
        });
            
        joinGameBtn.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.StartClient();
        });
    }
}