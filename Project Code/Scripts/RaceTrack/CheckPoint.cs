//script for detecting which player passed the checkpoint
using UnityEngine;
using Unity.Netcode;

public class CheckPoint : MonoBehaviour
{
    public static CheckPoint Instance => instance;
    private static CheckPoint instance;
    private void Awake()
    {
        instance = this;
    }

    //called when a gameobject collides with the trigger object
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.name == "ColliderFront")
        {
            Debug.Log("Checkpoint Passed " + this);
            CheckpointManager.Instance.AddCheckpoints(other.GetComponentInParent<NetworkObject>().OwnerClientId,this);  
                     
        }
    }

 }



        
    

