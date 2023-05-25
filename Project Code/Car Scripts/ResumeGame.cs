/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Samples;


public class ResumeGame : MonoBehaviour
{
    public static ResumeGame Instance => instance;
    private static ResumeGame instance;
    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Last CheckPoint Passed : " + CheckpointManager.Instance.GetLastCheckPoint());

            ChangeCarPosition(CheckpointManager.Instance.GetLastCheckPoint());

        }
    }
    /*[ServerRpc]
    void ReturnToLastCheckpointServerRpc(CarPosition lastCp)
    {
        if (IsLocalPlayer)
            return;

        ChangePosition(lastCp);
    }

    [ClientRpc]
    void ReturnToLastCheckpointClientRpc(CarPosition lastCp)
    {
        
    }
   
    public void ChangePosition(CarPosition lastCp)
    {
     

        transform.position = lastCp.Position;
        transform.rotation = lastCp.Rotation;
    }
    public void ChangeCarPosition(Transform lastCp)
    {
        
            ChangePosition( new CarPosition
            {
                Rotation=lastCp.transform.rotation,
                Position=lastCp.transform.position
            });
        
      
    }
}
*/