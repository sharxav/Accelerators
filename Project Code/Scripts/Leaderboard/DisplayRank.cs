//displays the leaderboard information
using Unity.Netcode;
using UnityEngine;

public class DisplayRank : NetworkBehaviour
{
    
    [SerializeField] private LeaderboardPlayerCard[] leaderboardPanels;

    public override void OnNetworkSpawn()
    {

        if (IsClient)
        {
            Debug.Log("Client in Leaderboard");

        }

        if (IsServer)
        {
           
            Debug.Log("Server in Leaderboard");

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }

        }
    }

   private void OnDestroy()
    {
        Debug.Log("Destroying Leaderboard");
        base.OnDestroy();


        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            
        }
    }

    
    private void HandleClientConnected(ulong clientId)
    {
        var playerRank = ServerGameNetPortal.Instance.GetPlayerRank(clientId);
        if (!playerRank.HasValue)
        {
            return;
        }

        DisplayClientRpc(playerRank.Value.ClientId, playerRank.Value.Rank, playerRank.Value.PlayerName, playerRank.Value.PlayerTime);    
    }

    [ClientRpc]
     private void DisplayClientRpc(ulong clientId, int rank, string playerName,string playerTime)
    {
        Debug.Log(clientId + " " + rank + " " + playerName + " " + playerTime);
        leaderboardPanels[clientId].Display(rank,playerName,playerTime);
    }
  

    


}






