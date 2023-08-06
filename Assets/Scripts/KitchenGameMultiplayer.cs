using System;
using Interface;
using UnityEngine;
using Unity.Netcode;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 4;
    public static KitchenGameMultiplayer Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChange;

    [SerializeField] private KitchenObjectListSO kitchenObjectListSo;
    [SerializeField] private List<Color> playerColorList;

    private NetworkList<PlayerData> _playerDataNetworkList; 

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerDataNetworkList = new NetworkList<PlayerData>();  
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChange?.Invoke(this,EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectedCallback(ulong clientID)
    {
        for (var i = 0; i < _playerDataNetworkList.Count; i++)
        {
            var playerData = _playerDataNetworkList[i];
            if(playerData.ClientId==clientID)
                //Disconnected!
                _playerDataNetworkList.RemoveAt(i);
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientID)
    {
        _playerDataNetworkList.Add(new PlayerData
        {
            ClientId = clientID,
            ColorId = GetFirstUnusedColorID()
        });
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started!";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full!";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);  
        
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientID)
    {
        OnFailedToJoinGame?.Invoke(this,EventArgs.Empty);
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSoIndex(kitchenObjectSo),kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSoIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        var kitchenObjectSo = GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);
        
        var kitchenObjTransform = Instantiate(kitchenObjectSo.prefab);
        
        var kitchenObjectOnNetwork = kitchenObjTransform.GetComponent<NetworkObject>();
        kitchenObjectOnNetwork.Spawn(true);
        
        var kitchenObject = kitchenObjTransform.GetComponent<KitchenObject.KitchenObject>();

        kitchenObjectParentNetworkObjectReference.TryGet(out var kitchenObjectParentNetworkObject);
        var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSoIndex(KitchenObjectSO kitchenObjectSo)
    {
        return kitchenObjectListSo.kitchenObjectList.IndexOf(kitchenObjectSo);
    }

    public KitchenObjectSO GetKitchenObjectSoFromIndex(int kitchenObjectSoIndex)
    {
        return kitchenObjectListSo.kitchenObjectList[kitchenObjectSoIndex]; 
    }

    public void DestroyKitchenObject(KitchenObject.KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenNetworkObjectReference)
    {
        kitchenNetworkObjectReference.TryGet(out var kitchenNetworkObject);
        var kitchenObject = kitchenNetworkObject.GetComponent<KitchenObject.KitchenObject>();
        ClearKitchenObjectOnParentClientRpc(kitchenNetworkObjectReference);
        kitchenObject.DestroySelf();
    }
    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenNetworkObjectReference)
    {
        kitchenNetworkObjectReference.TryGet(out var kitchenNetworkObject);
        var kitchenObject = kitchenNetworkObject.GetComponent<KitchenObject.KitchenObject>();
        kitchenObject.ClearKitchenObjectParent();
    }
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < _playerDataNetworkList.Count;
    }
    
    public int GetPlayerDataIndexFromClientID(ulong clientID)
    {
        for (var i = 0; i < _playerDataNetworkList.Count; i++)
        {
            if (_playerDataNetworkList[i].ClientId == clientID)
                return i;
        }
        return -1;
    }
    public  PlayerData GetPlayerDataFromClientID(ulong clientID)
    {
        foreach (var playerData in _playerDataNetworkList)
        {
            if (playerData.ClientId == clientID)
                return playerData;
        }
        return default;
    }
    
   
    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientID(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return _playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int colorID)
    {
        return playerColorList[colorID];
    }

    public void ChangePlayerColor(int colorID)
    {
        ChangePlayerColorServerRpc(colorID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorID, ServerRpcParams serverRpcParams = default)
    {
        if(!IsColorAvailable(colorID)) return; //Color is not available.
        
        var playerDataIndex = GetPlayerDataIndexFromClientID(serverRpcParams.Receive.SenderClientId); //Grab
        var playerData = _playerDataNetworkList[playerDataIndex]; // Modify
        playerData.ColorId = colorID; 
        _playerDataNetworkList[playerDataIndex] = playerData; //Upload new changes
    }
    private bool IsColorAvailable(int colorID)
    {
        foreach (var playerData in _playerDataNetworkList)
        {
            if (playerData.ColorId == colorID)
            {
                //Already use
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorID()
    {
        for (var i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
                return i;
        }
        return -1;
    }

    public void KickPlayer(ulong clientID)
    {
        NetworkManager.Singleton.DisconnectClient(clientID);
        NetworkManager_Server_OnClientDisconnectedCallback(clientID);
    }
}
