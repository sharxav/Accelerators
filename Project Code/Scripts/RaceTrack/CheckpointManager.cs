//script to determine the most recent checkpoint passed by the player
using System;
using System.Collections.Generic;
using UnityEngine;


public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance => instance;
    private static CheckpointManager instance;
    [SerializeField] private CheckPoint[] checkpoints; //checkpoints present on the race track
    private Dictionary<ulong,CheckPoint> checkpointList; //saves the client Id and most recent checkpoint passed
   
    private void Awake()
    {
        instance = this;
        checkpointList = new Dictionary<ulong,CheckPoint>();
    }


    public void AddCheckpoints(ulong index,CheckPoint cp)
    {
       
        if(checkpointList.ContainsKey(index))
        {
            int i = Array.IndexOf(checkpoints, checkpointList[index]);
            if(checkpoints[i+1]==cp)                                     //checks if the checkpoint passed is the correct checkpoint
            {
                checkpointList[index] = cp;
            }
            else if(checkpoints[i]==cp)                                //checks if the user is passing a checkpoint again
            {
                Debugger.instance.LogInfo("You have already passed this checkpoint");
            }
            
        }
        else
        {
            if (cp == checkpoints[0])         //initial checkpoint passed
            {
                Debugger.instance.LogInfo("Right checkpoint");
                checkpointList.Add(index, cp);
                Debug.Log("Checkpoint" + cp);
            }
        }
    }

    //returns the position of the most recent checkpoint
    public Transform LastCheckpoint(ulong index)
    {
        Debug.Log("Last checkpoint for " + index + " is " + checkpointList[index]);
        if(checkpointList.ContainsKey(index))
        {
            return checkpointList[index].transform;
        }
        return null;
        
    }
    
}

