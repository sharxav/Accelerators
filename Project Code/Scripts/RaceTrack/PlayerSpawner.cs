//script for spawning player car
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject player=null;
    [SerializeField] private Transform[] spawnPoints;
    private List<Transform> remainingspawnPoints;
    private List<ulong> playersInGame = new List<ulong>();
    int nextIndex = 0;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                Debug.Log("Server Spawn");
                playersInGame.Add
                    (client.ClientId);
            }
            remainingspawnPoints = new List<Transform>(spawnPoints);

        }
        if(IsClient)
        {
            Debug.Log("Client Spawn");
            SubmitRequestServerRpc();
        }
    }

    [ServerRpc(RequireOwnership =false)]
    void SubmitRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        if(!playersInGame.Contains(rpcParams.Receive.SenderClientId))
        {
            return;
        }
        SpawnPlayer(rpcParams.Receive.SenderClientId);
        playersInGame.Remove(rpcParams.Receive.SenderClientId);
        if(playersInGame.Count!=0)
        {
            return;
        }

        Debug.Log("All Players Spawned!!");
    }

    //function for spawning player cars
    public void SpawnPlayer(ulong clientId)
    {
        Debug.Log("Client Id = " + clientId);
   
        Transform transform = remainingspawnPoints[nextIndex];
        remainingspawnPoints.RemoveAt(nextIndex);

        NetworkObject playerInstance = Instantiate(player, transform.position, transform.rotation);
        playerInstance.SpawnAsPlayerObject(clientId, false);
        nextIndex++;
       
    }

   
}


