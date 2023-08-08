using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Random = UnityEngine.Random;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine.SceneManagement;

public class KitchenGameLobby : MonoBehaviour
{
    public static KitchenGameLobby Instance { get; private set; }

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinLobbyStarted;
    public event EventHandler OnQuickJoinLobbyFailed;    
    public event EventHandler OnJoinLobbyFailed;

    public event EventHandler<LobbyListChangedEventArgs> OnLobbyListChanged;
    public class LobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> LobbyList;
    }
    private Lobby _joinedLobby;
    private float _heartBeatTimer;
    private float _listLobbiesTimer;
    
    private const float HEART_BEAT_TIMER_MAX = 15f;
    private const float LIST_LOBBY_TIMER_MAX = 3f;  
    private const string  KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }
    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized) return;
        var initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(Random.Range(0, 10000).ToString());
        await UnityServices.InitializeAsync(initializationOptions);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
           Debug.Log("TotalPlayer:" + _joinedLobby.Players.Count);
        HandleHeartBeat();
        HandlePeriodicListLobbies();
    }

    private void HandleHeartBeat()
    {
        if (!IsLobbyHost()) return;
        _heartBeatTimer -= Time.deltaTime;
        if (!(_heartBeatTimer <= 0)) return;
        _heartBeatTimer = HEART_BEAT_TIMER_MAX;
        LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
    }

    private void HandlePeriodicListLobbies()
    {
        if (_joinedLobby != null || !AuthenticationService.Instance.IsSignedIn ||
            SceneManager.GetActiveScene().name != Loader.Loader.Scene.LobbyScene.ToString()) return;
        _listLobbiesTimer -= Time.deltaTime;
        if (!(_listLobbiesTimer <= 0)) return;
        _listLobbiesTimer = LIST_LOBBY_TIMER_MAX;
        ListLobbies();
    }

    private async void ListLobbies()
    {
        try
        {
            var queryLobbiesOptions = new QueryLobbiesOptions()
            {
                Filters = new List<QueryFilter>()
                {
                    new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };
            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
        
            OnLobbyListChanged?.Invoke(this,new LobbyListChangedEventArgs {LobbyList = queryResponse.Results});
        }
        catch (Exception e)
        {
            Debug.Log($"Error occured during List of Lobbies {e}");
            throw;
        }
       
    }

    private bool IsLobbyHost()
    {
        return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
 
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this,EventArgs.Empty);
        try
        {
            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT,
                new CreateLobbyOptions { IsPrivate = isPrivate }); 
            var allocation = await AllocateRelay();
            var relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                        KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                    }
                }
            });
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation,"dtls")); //dtls is connection type and The Unity by default recommend. It is some type of Encryption.    
            KitchenGameMultiplayer.Instance.StartHost();
            Loader.Loader.LoadNetwork(Loader.Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"Error occured during Creation of Lobby {e}");
            OnCreateLobbyFailed?.Invoke(this,EventArgs.Empty);
            throw;
        }
    }
    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            var allocationAsync = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYER_AMOUNT - 1);
            return allocationAsync;
        }
        catch (Exception e)
        {
            Debug.Log($"Error occured during RelayAllocation {e}");
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (Exception e)
        {
            Debug.Log($"Error occured during GetRelayJoinCode {e}");
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (Exception e)
        {
            Debug.Log($"Error occured during JoinRelay throw code {e}");
            throw;
        }
    }
    public async void QuickJoin()
    {
        OnJoinLobbyStarted?.Invoke(this,EventArgs.Empty);
        try
        {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            
            var relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            var joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation,"dtls")); //dtls is connection type and The Unity by default recommend. It is some type of Encryption.    

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"Error occured during QuickJoin {e}");
            OnQuickJoinLobbyFailed?.Invoke(this,EventArgs.Empty);
            throw;
        }
    }
    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinLobbyStarted?.Invoke(this,EventArgs.Empty);
        if (string.IsNullOrEmpty(lobbyCode)) 
            OnJoinLobbyFailed?.Invoke(this,EventArgs.Empty);
        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            
            var relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            var joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation,"dtls")); //dtls is connection type and The Unity by default recommend. It is some type of Encryption. 
            
            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"Error occured during JoinWithCode {e}");
            OnJoinLobbyFailed?.Invoke(this,EventArgs.Empty);
            throw;
        }
    }
    public async void JoinWithID(string lobbyID)
    {
        OnJoinLobbyStarted?.Invoke(this,EventArgs.Empty);
        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);
            
            var relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            var joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation,"dtls")); //dtls is connection type and The Unity by default recommend. It is some type of Encryption. 
            
            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"Error occured during JoinWithCode {e}");
            OnJoinLobbyFailed?.Invoke(this,EventArgs.Empty);
            throw;
        }
    }

    public async void DeleteLobby()
    {
        try
        {
            if(_joinedLobby!=null)
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
            _joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"Error occured during RemovingLobby {e}");
            throw;
        }
    }

    public async void LeaveLobby()
    {
        if (_joinedLobby == null) return;
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            //if(IsLobbyHost())
                _joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"Error occured during RemovingLobby {e}");
            throw;
        }
    } 
    
    public async void KickPlayer(string playerID)
    {
        if (!IsLobbyHost()) return;
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerID);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"Error occured during kicking Player {e}");
            throw;
        }
    } 
    public Lobby GetLobby()
    {
        return _joinedLobby;
    }
}
