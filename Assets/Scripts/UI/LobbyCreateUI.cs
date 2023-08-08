using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button privateLobby;
    [SerializeField] private Button publicLobby;
    [SerializeField] private Button closeBtn;

    
    private void Awake()
    {
        privateLobby.onClick.AddListener(() =>
        {
           KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text,true);
        });
            
        publicLobby.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text,false);

        });
        
        closeBtn.onClick.AddListener(Hide);
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
