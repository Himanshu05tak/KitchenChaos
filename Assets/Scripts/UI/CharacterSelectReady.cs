using System.Linq;
using Unity.Netcode;
using System.Collections.Generic;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }
    private Dictionary<ulong, bool> _playerReadyDictionary;
    private void Awake()
    {
        Instance = this;
        _playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }
        
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        var allClientsAreReady = NetworkManager.Singleton.ConnectedClientsIds.All(clientId => _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId]);

        if (allClientsAreReady)
            Loader.Loader.LoadNetwork(Loader.Loader.Scene.GamePlay);
    }
}