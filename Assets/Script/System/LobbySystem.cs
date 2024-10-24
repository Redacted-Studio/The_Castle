using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbySystem : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private float heartbeatTime = 15f;
    [SerializeField] private float lobbyUpdateTime = 1.1f;

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float currentHeartbeatTimer;
    private float currentLobbyUpdateTime;

    private static LobbySystem _instance;
    public static LobbySystem Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Lobby system is null");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in successfully with ID: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "riley it okay im joy " + Random.Range(0, 9999);

        Debug.Log("Player name: " + playerName);
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby == null) return;

        currentHeartbeatTimer -= Time.deltaTime;
        if (currentHeartbeatTimer <= 0f)
        {
            currentHeartbeatTimer = heartbeatTime;
            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (hostLobby == null || joinedLobby == null) return;

        currentLobbyUpdateTime -= Time.deltaTime;
        if (currentLobbyUpdateTime <= 0f)
        {
            currentLobbyUpdateTime = lobbyUpdateTime;

            if (hostLobby != null)
            {
                hostLobby = await LobbyService.Instance.GetLobbyAsync(hostLobby.Id);
            }
            else if (joinedLobby != null)
            {
                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
            }
            
        }
    }

    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "testLobby";
            int maxPlayer = 4;

            currentHeartbeatTimer = heartbeatTime;

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer()
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, options);

            hostLobby = lobby;

            Debug.LogFormat("Lobby created. Name: {0}, ID: {1}, Code: {2}.", lobby.Name, lobby.Id, lobby.LobbyCode);

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogWarning("Lobby creation failed. Details:");
            Debug.LogWarning(ex);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse list = await Lobbies.Instance.QueryLobbiesAsync(options);

            Debug.Log("Lobbies found: " + list.Results.Count);

            int i = 0;
            foreach (var result in list.Results)
            {
                i++;
                Debug.LogFormat("{0}. {1} ({2})", i, result.Name, result.Id);
            }
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogWarning("List lobby fetching failed. Details:");
            Debug.LogWarning(ex);
        }
    }

    private async void JoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions
            {
                Player = GetPlayer()
            };

            joinedLobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);

            Debug.LogFormat("Quick joined to lobby with ID: {0}", joinedLobby.Id);

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogWarning("Quick joining lobby failed. Details:");
            Debug.LogWarning(ex);
        }
    }

    public async void JoinLobbyByID(string lobbyID)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };

            joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyID, options);

            Debug.LogFormat("Joined lobby with code: {0}", lobbyID);

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogWarning("Joining lobby failed. Details:");
            Debug.LogWarning(ex);
        }
    }

    private async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, options);

            Debug.LogFormat("Joined lobby with code: {0}", lobbyCode);

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogWarning("Joining lobby failed. Details:");
            Debug.LogWarning(ex);
        }
    }

    private async void LeaveLobby()
    {
        try
        {
            if (hostLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, AuthenticationService.Instance.PlayerId);
                hostLobby = null;
            }
            else if (joinedLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            else
            {
                Debug.LogWarning("Player is not joining any lobby.");
            }
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogWarning("Leaving lobby failed. Details:");
            Debug.LogWarning(ex);
        }
    }

    private async void RemovePlayer()
    {

    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.LogFormat("Player count: {0}", lobby.Players.Count);

        int i = 0;
        foreach (var player in lobby.Players)
        {
            i++;
            Debug.LogFormat("{0}. {1} ({2})", i, player.Data["PlayerName"].Value, player.Id);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    #region "Debugging functions"

    public void DebugCreateLobby()
    {
        CreateLobby();
    }

    public void DebugListLobbies()
    {
        ListLobbies();
    }

    public void DebugJoinLobby()
    {
        JoinLobby();
    }

    public void DebugJoinLobbyByCode(string code)
    {
        JoinLobbyByCode(code);
    }

    #endregion
}
