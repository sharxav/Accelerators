//allows a player to return to his/her most recent checkpoint 
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Samples;

[RequireComponent(typeof(ClientNetworkTransform))]
public class ResumeGame : NetworkBehaviour
{
    private void FixedUpdate()
    {
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Checking whether this car is present = " + GetComponentInParent<NetworkObject>().OwnerClientId);

            transform.position = (CheckpointManager.Instance.LastCheckpoint(GetComponentInParent<NetworkObject>().OwnerClientId)).position;
     

        }
    }
}
