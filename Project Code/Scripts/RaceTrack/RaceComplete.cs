//script that handles the information required for the leaderboard
using UnityEngine;
using Unity.Netcode;
public class RaceComplete : NetworkBehaviour
{
    [SerializeField] private GameObject RaceManager;
    private int rank = 1;
  
    //called when a car passes the finish line trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "ColliderFront")
        {
            //host saves his rank, name and time information
            if (IsHost)
            {
                var clientId = other.GetComponentInParent<NetworkObject>().OwnerClientId;
                var playerData = ServerGameNetPortal.Instance.GetPlayerData(clientId);
                ServerGameNetPortal.Instance.AddRankDetails(clientId, playerData.Value.PlayerName, rank,PlayerPrefs.GetString("Time"));
                rank++;
                Debug.Log("Server just passed finish line");
                Debug.Log("Server time = " + PlayerPrefs.GetString("Time"));
                other.GetComponentInParent<NetworkObject>().Despawn();   //despawns the player car on crossing the finish line. This removes the car from the scene 
            }                                                             // but it does not terminate the connection
            ///client calls a ServerRpc to save its information
            else
            {
                RequestDespawnServerRpc(other.GetComponentInParent<NetworkObject>().OwnerClientId,PlayerPrefs.GetString("Time"));
            }
        }
        
    }

    [ServerRpc(RequireOwnership =false)]
    private void RequestDespawnServerRpc(ulong ownerClientId,string time)
    {
        Debug.Log("Client just passed finish line.. "+ownerClientId);
        Debug.Log("Client Time = " + time);
        var playerData = ServerGameNetPortal.Instance.GetPlayerData(ownerClientId);
        ServerGameNetPortal.Instance.AddRankDetails(ownerClientId, playerData.Value.PlayerName, rank, time);
        rank++;
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(ownerClientId).Despawn();
    }
}
