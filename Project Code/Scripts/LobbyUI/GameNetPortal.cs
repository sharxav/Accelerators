//a gateway between the clientNetportal and serverNetportal
//Reference - https://github.com/DapperDino/Unity-Multiplayer-Tutorials/blob/main/Assets/Tutorials/Lobby/Scripts/Networking/GameNetPortal.cs
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum ConnectStatus
{
    Undefined,
    Success,                              
    UserRequestedDisconnect,  
    GenericDisconnect,
    GameInProgress,
    ServerOverload
}

[Serializable]
public class ConnectionPayload
{
    public string password;
    public string clientGUID;
    public int clientScene = -1;
    public string playerName;

}

public class GameNetPortal : MonoBehaviour
{
    public static GameNetPortal Instance => instance;
    private static GameNetPortal instance;
    private ClientGameNetPortal m_ClientPortal;
    private ServerGameNetPortal m_ServerPortal;

    public event Action OnNetworkReady;


    public event Action<ConnectStatus> ConnectionEstablished;
    public event Action<ConnectStatus> OnDisconnectReasonReceived;

    public event Action<ulong, int> OnClientSceneChanged;

    public event Action OnUserDisconnectRequested;
    [SerializeField] private Button CreateLobbyBtn, JoinLobbyBtn;
    [SerializeField] private TMP_InputField JoinLobbyInp, LobbyNameInp;
    [SerializeField] private TMP_Text CreateLobbytxt, JoinLobbytxt;
   
   

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

        NetworkManager.Singleton.OnServerStarted += OnNetworkReadied;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;


    }

    private void OnDestroy()
    {

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= OnNetworkReadied;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;

            if (NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneChange;
            }

            if (NetworkManager.Singleton.CustomMessagingManager == null) { return; }

            UnregisterClientMessageHandlers();

        }
    }



    public void StartHost()
    {


        NetworkManager.Singleton.StartHost();
        PlayerPrefs.SetString("LobbyName", LobbyNameInp.text);
        RegisterClientMessageHandlers();
    }

    public void RequestDisconnect()
    {

        m_ClientPortal.UserDisconnectRequested();
        m_ServerPortal.UserDisconnectRequested();
      //  OnUserDisconnectRequested?.Invoke();


    }

    private void OnClientConnect(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {

            return;
        }


        OnNetworkReadied();
        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneChange;
    }

    private void OnSceneChange(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;

        OnClientSceneChanged?.Invoke(sceneEvent.ClientId, SceneManager.GetSceneByName(sceneEvent.SceneName).buildIndex);
    }

    private void OnNetworkReadied()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            ConnectionEstablished?.Invoke(ConnectStatus.Success);
        }

        OnNetworkReady?.Invoke();

    }


    public void CreateLobby()
    {
        JoinLobbytxt.gameObject.SetActive(false);
        JoinLobbyInp.gameObject.SetActive(false);
        JoinLobbyBtn.gameObject.SetActive(false);

        CreateLobbytxt.gameObject.SetActive(true);
        LobbyNameInp.gameObject.SetActive(true);
        CreateLobbyBtn.gameObject.SetActive(true);
    }

    public void JoinLobby()
    {
        JoinLobbytxt.gameObject.SetActive(true);
        JoinLobbyInp.gameObject.SetActive(true);
        JoinLobbyBtn.gameObject.SetActive(true);

        CreateLobbytxt.gameObject.SetActive(false);
        LobbyNameInp.gameObject.SetActive(false);
        CreateLobbyBtn.gameObject.SetActive(false);
    }

    //Custom Messaging
    public void ServerToClientConnectResult(ulong netId, ConnectStatus status)
    {
        var writer = new FastBufferWriter(sizeof(ConnectStatus), Allocator.Temp);
        writer.WriteValueSafe(status);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ServerToClientConnectResult", netId, writer);
    }

    public void ServerToClientSetDisconnectReason(ulong netId, ConnectStatus status)
    {
        var writer = new FastBufferWriter(sizeof(ConnectStatus), Allocator.Temp);
        writer.WriteValueSafe(status);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ServerToClientSetDisconnectReason", netId, writer);
    }

  
   

    private void RegisterClientMessageHandlers()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ServerToClientConnectResult", (senderClientId, reader) =>
        {
            reader.ReadValueSafe(out ConnectStatus status);
            Instance.ConnectionEstablished(status);
        });

        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ServerToClientSetDisconnectReason", (senderClientId, reader) =>
        {
            reader.ReadValueSafe(out ConnectStatus status);
            Instance.OnDisconnectReasonReceived(status);
        });
    }

    private void UnregisterClientMessageHandlers()
    {
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ServerToClientConnectResult");
        NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ServerToClientSetDisconnectReason");
    }




}