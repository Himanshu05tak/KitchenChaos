using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;

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
        if (_joinedLobby != null || !AuthenticationService.Instance.IsSignedIn) return;
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
    public async void QuickJoin()
    {
        OnJoinLobbyStarted?.Invoke(this,EventArgs.Empty);
        try
        {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
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
