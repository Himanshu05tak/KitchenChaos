using System;
using Interface;
using UnityEngine;
using Unity.Netcode;
using ScriptableObjects;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    
    [SerializeField] private KitchenObjectListSO kitchenObjectListSo; 
    
    private const int MAX_PLAYER_AMOUNT = 4;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
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
        
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void OnClientDisconnectCallback(ulong clientID)
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
}
