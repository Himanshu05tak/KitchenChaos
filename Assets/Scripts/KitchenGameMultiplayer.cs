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

    private int GetKitchenObjectSoIndex(KitchenObjectSO kitchenObjectSo)
    {
        return kitchenObjectListSo.kitchenObjectList.IndexOf(kitchenObjectSo);
    }

    private KitchenObjectSO GetKitchenObjectSoFromIndex(int kitchenObjectSoIndex)
    {
        return kitchenObjectListSo.kitchenObjectList[kitchenObjectSoIndex]; 
    }
}
