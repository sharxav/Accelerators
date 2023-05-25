//Reference - https://github.com/DapperDino/Unity-Multiplayer-Tutorials/blob/main/Assets/Tutorials/Lobby/Scripts/Networking/ClientGameNetPortal.cs
//handles client connections
using System;
using System.Collections;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(GameNetPortal))]
public class ClientGameNetPortal : MonoBehaviour
{
    public static ClientGameNetPortal Instance => instance;
    private static ClientGameNetPortal instance;

    public ClientDisconnection ClientDisconnection { get; private set; } = new ClientDisconnection();

    public event Action<ConnectStatus> ConnectionEstablished;

    private GameNetPortal gameNetPortal;
   
    [SerializeField] private TMP_InputField inp;

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
        gameNetPortal.ConnectionEstablished += OnConnected;
        gameNetPortal.OnDisconnectReasonReceived += OnDisconnectReasonReceived;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }


    private void OnDestroy()
    {
        if (gameNetPortal == null) { return; }

        gameNetPortal.OnNetworkReady -= OnNetworkReadied;
        gameNetPortal.ConnectionEstablished -= OnConnected;
        gameNetPortal.OnDisconnectReasonReceived -= OnDisconnectReasonReceived;


        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;

    }

    public void StartClient()
    {
        Debug.Log("Client started..");
        var payload = JsonUtility.ToJson(new ConnectionPayload()
        {
            password = inp.text,
            clientGUID = Guid.NewGuid().ToString(),
            clientScene = SceneManager.GetActiveScene().buildIndex,
            playerName = PlayerPrefs.GetString("PlayerName", "Missing Name")

        });
        Debug.Log("Client Enters" + inp.text);


        byte[] payloadBytes = Encoding.UTF32.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
        StartCoroutine(ServerConnection(NetworkManager.Singleton.LocalClientId));

    }

    public void OnNetworkReadied()
    {
        if (!NetworkManager.Singleton.IsClient) { return; }

       


    }


    public void UserDisconnectRequested()
    {

        ClientDisconnection.SetClientDisconnection(ConnectStatus.UserRequestedDisconnect);



        OnClientDisconnect(NetworkManager.Singleton.LocalClientId);

        SceneManager.LoadScene("LobbyUI", LoadSceneMode.Single);

        NetworkManager.Singleton.Shutdown();
        Destroy(this);


    }

    private void OnConnected(ConnectStatus status)
    {
        if (status != ConnectStatus.Success)
        {
            ClientDisconnection.SetClientDisconnection(status);
        }

        ConnectionEstablished?.Invoke(status);
    }

    private void OnDisconnectReasonReceived(ConnectStatus status)
    {
        ClientDisconnection.SetClientDisconnection(status);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost)
        {
            

            if (SceneManager.GetActiveScene().name != "LobbyUI")
            {
                if (!ClientDisconnection.HasTransitionReason)
                {
                    ClientDisconnection.SetClientDisconnection(ConnectStatus.GenericDisconnect);
                }

                SceneManager.LoadScene("LobbyUI", LoadSceneMode.Single);

            }

        }
    }
    //tracks server connection
    private IEnumerator ServerConnection(ulong clientId)
    {
        bool flag = true;
        while (flag)
        {
            yield return new WaitForSeconds(5.0f);
            if (!NetworkManager.Singleton.IsConnectedClient)
            {
                flag = false;
                SceneManager.LoadScene("LobbyUI", LoadSceneMode.Single);
                Debugger.instance.LogInfo("Server down..");

            }

        }
    }


}