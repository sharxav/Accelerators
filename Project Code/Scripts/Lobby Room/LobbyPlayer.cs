//script that handles lobby players and its synchronization
//Reference - https://github.com/DapperDino/Unity-Multiplayer-Tutorials/blob/main/Assets/Tutorials/Lobby/Scripts/UI/LobbyUI.cs
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LobbyPlayer : NetworkBehaviour
{
    [SerializeField] private LobbyPlayerCard[] lobbyPlayerCards;
    [SerializeField] private Button startGameButton;
    private NetworkList<LobbyPlayerState> lobbyPlayers;
    public static LobbyPlayer Instance => instance;
    private static LobbyPlayer instance;
    [SerializeField] private TMP_Text codeTxt,lobbyNameTxt;
   
  
    private void Awake()
    {
        lobbyPlayers = new NetworkList<LobbyPlayerState>();
    }
    public  override void OnNetworkSpawn()
    {
       
        if (IsClient)
        {
            Debug.Log("Entered Lobby");
            lobbyPlayers.OnListChanged += LobbyStateChange;
        }

        if(IsServer)
        {

            codeTxt.text = PlayerPrefs.GetString("LobbyCode");
           
            Debug.Log("Server in Lobby");
            startGameButton.gameObject.SetActive(true);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                OnClientConnect(client.ClientId);
            }

        }
    }

    //to display the lobby join code and name in the lobby room
   [ServerRpc]
    private void DisplayCodeServerRpc(string code,string lobbyname)
    {
        DisplayCodeClientRpc(code,lobbyname);
    }

    [ClientRpc]
    private void DisplayCodeClientRpc(string code,string lobbyname)
    {
     if(!IsLocalPlayer)
        {
            codeTxt.text = code;
            lobbyNameTxt.text = lobbyname;

        }
    }

    private void OnDestroy()
    {
        Debug.Log("Destroying Lobby");
        base.OnDestroy();
        lobbyPlayers.OnListChanged -=LobbyStateChange;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }


    //checks the status of each player
    private bool CheckIfReady()
    {
        if (lobbyPlayers.Count < 1)
        {
            return false;
        }

        foreach (var val in lobbyPlayers)
        {
            if (!val.isReady)
            {
                return false;
            }
        }

        return true;
    }
    private void OnClientConnect(ulong clientId)
    {
        var playerData = ServerGameNetPortal.Instance.GetPlayerData(clientId);
        if (!playerData.HasValue)
        {
            return;
        }

        lobbyPlayers.Add(new LobbyPlayerState(clientId, playerData.Value.PlayerName,false));
        DisplayCodeServerRpc(PlayerPrefs.GetString("LobbyCode"),PlayerPrefs.GetString("LobbyName"));
    }
    private void OnClientDisconnect(ulong clientId)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == clientId)
            {
                lobbyPlayers.RemoveAt(i);
                break;
            }
        }
    }

   //player status change
   [ServerRpc(RequireOwnership = false)]
    private void ToggleServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                
                lobbyPlayers[i] = new LobbyPlayerState(
                    lobbyPlayers[i].ClientId,
                    lobbyPlayers[i].PlayerName,
                    !lobbyPlayers[i].isReady

                ) ;

          
            }
        }
    }

    //host checks whether it can start the game
    [ServerRpc(RequireOwnership = false)]
    private void BeginServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId != NetworkManager.Singleton.LocalClientId) { return; }

        if (!CheckIfReady()) { return; }

        ServerGameNetPortal.Instance.StartGame();
        Debug.Log("Game started");
       
    }
    public void OnLeave()
    {
        
        GameNetPortal.Instance.RequestDisconnect();
    }

    public void OnReady()
    {
        ToggleServerRpc();
    }
    public void OnStartGameClicked()
    {
        BeginServerRpc();
    }

    //event call to update the player state
    private void LobbyStateChange(NetworkListEvent<LobbyPlayerState> lobbyState)
    {
        
        for (int i = 0; i < lobbyPlayerCards.Length; i++)
        {
            
            if (lobbyPlayers.Count > i)
            {
                lobbyPlayerCards[i].UpdateDisplay(lobbyPlayers[i]);
            }
            else{
                lobbyPlayerCards[i].DisableDisplay();
                }
        }
        if (IsHost)
        {
            startGameButton.interactable = CheckIfReady();
        }
    }

}
