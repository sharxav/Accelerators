//handles all server - side activities
//Reference - https://github.com/DapperDino/Unity-Multiplayer-Tutorials/blob/main/Assets/Tutorials/Lobby/Scripts/Networking/ServerGameNetPortal.cs
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameNetPortal : MonoBehaviour
{
    public static ServerGameNetPortal Instance => instance;
    private static ServerGameNetPortal instance;

    private Dictionary<string, PlayerData> clientData;
    private Dictionary<ulong, string> clientIdToGuid;
    private Dictionary<ulong, int> clientSceneMap;
    private Dictionary<ulong, PlayerRank> rankData;
    private bool gameInProgress;
    private GameNetPortal gameNetPortal;



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);

        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameNetPortal = GetComponent<GameNetPortal>();
        gameNetPortal.OnNetworkReady += OnNetworkReadied;

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnServerStart;

        clientData = new Dictionary<string, PlayerData>();
        clientIdToGuid = new Dictionary<ulong, string>();
        clientSceneMap = new Dictionary<ulong, int>();
        rankData = new Dictionary<ulong, PlayerRank>();
    }

    private void OnDestroy()
    {
        if (gameNetPortal == null) { return; }

       gameNetPortal.OnNetworkReady -= OnNetworkReadied;

        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted -= OnServerStart;


    }

    //returns player information
    public PlayerData? GetPlayerData(ulong clientId)
    {
        if (clientIdToGuid.TryGetValue(clientId, out string clientGuid))
        {
            if (clientData.TryGetValue(clientGuid, out PlayerData playerData))
            {
                return playerData;
            }
            else
            {
                Debug.LogWarning($"No data found!!");
            }
        }
        else
        {
            Debug.LogWarning("Player GUID not found");
        }

        return null;
    }

    //returns player rank information
    public PlayerRank? GetPlayerRank(ulong clientId)
    {

        if (rankData.TryGetValue(clientId, out PlayerRank playerRank))
        {
            return playerRank;
        }
        else
        {
            Debug.LogWarning("No data found!!");
        }


        return null;
    }

    //adds player rank details
    public void AddRankDetails(ulong clientId, string playerName, int rank, string playerTime)
    {
        if (rankData.ContainsKey(clientId))
        {
            rankData[clientId] = new PlayerRank(playerName, rank, playerTime, clientId);
        }
        else
        {
            rankData.Add(clientId, new PlayerRank(playerName, rank, playerTime, clientId));
        }

        Debug.Log("RankData ClientId = " + clientId);
        Debug.Log("RankData PlayerName = " + playerName);
        Debug.Log("RankData rank = " + rank);
        Debug.Log("RankData playerTime =" + playerTime);

        if (rank == NetworkManager.Singleton.ConnectedClients.Count)
        {
            EndRound();
        }
    }

    public int ReturnConnectedClients()
    {
        return NetworkManager.Singleton.ConnectedClients.Count;
    }
    public void OnNetworkReadied()
    {
        if (!NetworkManager.Singleton.IsServer) { return; }

      
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        gameNetPortal.OnClientSceneChanged += OnClientSceneChange;

        NetworkManager.Singleton.SceneManager.LoadScene("LobbyRoom", LoadSceneMode.Single);

        if (NetworkManager.Singleton.IsHost)
        {
            clientSceneMap[NetworkManager.Singleton.LocalClientId] = SceneManager.GetActiveScene().buildIndex;
        }
    }

    //handles client disconnection
    private void OnClientDisconnect(ulong clientId)
    {
        clientSceneMap.Remove(clientId);
        rankData.Remove(clientId);

        if (clientIdToGuid.TryGetValue(clientId, out string guid))
        {
            clientIdToGuid.Remove(clientId);

            if (clientData[guid].ClientId == clientId)
            {
                clientData.Remove(guid);
            }
        }



        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
           
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            gameNetPortal.OnClientSceneChanged -= OnClientSceneChange;

        }

    }

    private void OnClientSceneChange(ulong clientId, int sceneIndex)
    {
        clientSceneMap[clientId] = sceneIndex;
    }

    public void UserDisconnectRequested()
    {


        OnClientDisconnect(NetworkManager.Singleton.LocalClientId);

        ClearData();

        SceneManager.LoadScene("LobbyUI", LoadSceneMode.Single);
        NetworkManager.Singleton.Shutdown();
        Destroy(this);
    }

    private void OnServerStart()
    {
        if (!NetworkManager.Singleton.IsHost) { return; }

        string clientGuid = Guid.NewGuid().ToString();
        string playerName = PlayerPrefs.GetString("PlayerName", "Missing Name");


        clientData.Add(clientGuid, new PlayerData(playerName, NetworkManager.Singleton.LocalClientId));
        clientIdToGuid.Add(NetworkManager.Singleton.LocalClientId, clientGuid);

    }

    private void ClearData()
    {
        clientData.Clear();
        clientIdToGuid.Clear();
        clientSceneMap.Clear();
        rankData.Clear();

        gameInProgress = false;
    }

    public void StartGame()
    {
        gameInProgress = true;


        NetworkManager.Singleton.SceneManager.LoadScene("RaceArea02", LoadSceneMode.Single);

    }

    public void EndRound()
    {
        gameInProgress = false;
        NetworkManager.Singleton.SceneManager.LoadScene("Leaderboard", LoadSceneMode.Single);
    }

    //player authentication
    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
       
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {

            callback(false, null, true, null, null);
            return;
        }

        string payload = Encoding.UTF32.GetString(connectionData);
        var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);
        Debug.Log("Password = " + connectionPayload.password);
        bool approveConnection = connectionPayload.password == PlayerPrefs.GetString("LobbyCode");
        Debug.Log("Server Gets" + connectionPayload.password);
        if (approveConnection)
        {
            ConnectStatus gameReturnStatus = ConnectStatus.Success;


            if (gameInProgress)
            {
                gameReturnStatus = ConnectStatus.GameInProgress;
            }
            else if (clientData.Count >= 4)
            {
                gameReturnStatus = ConnectStatus.ServerOverload;
            }

            if (gameReturnStatus == ConnectStatus.Success)
            {
                clientSceneMap[clientId] = connectionPayload.clientScene;
                clientIdToGuid[clientId] = connectionPayload.clientGUID;
                clientData[connectionPayload.clientGUID] = new PlayerData(connectionPayload.playerName, clientId);
            }

            callback(false, 0, true, null, null);

            gameNetPortal.ServerToClientConnectResult(clientId, gameReturnStatus);

        }
        else
        {

            callback(false, 0, false, null, null);
        }


    }



}