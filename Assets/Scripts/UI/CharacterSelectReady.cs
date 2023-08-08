using System;
using System.Linq;
using Unity.Netcode;
using System.Collections.Generic;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnReadyChanged;
    
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
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        var allClientsAreReady = NetworkManager.Singleton.ConnectedClientsIds.All(clientId => _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId]);

        if (!allClientsAreReady) return;
        KitchenGameLobby.Instance.DeleteLobby();
        Loader.Loader.LoadNetwork(Loader.Loader.Scene.GamePlay);
    }
    
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientID)
    {
        _playerReadyDictionary[clientID] = true;
        OnReadyChanged?.Invoke(this,EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientID)
    {
        return _playerReadyDictionary.ContainsKey(clientID) && _playerReadyDictionary[clientID];
    }
}