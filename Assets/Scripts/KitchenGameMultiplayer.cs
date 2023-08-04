using Manager;
using Interface;
using UnityEngine;
using Unity.Netcode;
using ScriptableObjects;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO kitchenObjectListSo; 
    private void Awake()
    {
        Instance = this;
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (GameManager.Instance.IsWaitingToStart())
        {
            connectionApprovalResponse.Approved = true;
            connectionApprovalResponse.CreatePlayerObject = true;
        }
        else
            connectionApprovalResponse.Approved = false;
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
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
