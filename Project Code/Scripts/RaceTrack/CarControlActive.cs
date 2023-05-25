//script for enabling the car controls after the countdown

using UnityEngine;
using Unity.Netcode;
using UnityStandardAssets.Vehicles.Car;
using Unity.Netcode.Samples;

[RequireComponent(typeof(ClientNetworkTransform))]
public class CarControlActive : NetworkBehaviour
{
    void Start()
    {
        CarEnable(NetworkManager.Singleton.LocalClientId);     //gets the local players client Id
              
    }
    
    private void CarEnable(ulong clientId)
    {
      
        NetworkObject obj = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId); //finds the car that is owned by the particular player
        //Debug.Log("Object name = " + obj.name);
        obj.GetComponent<CarUserControl>().enabled = true;  //enables the car controls on the player car
    }

}
