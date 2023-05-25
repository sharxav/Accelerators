using UnityEngine;
using Unity.Netcode;
using Cinemachine;



public class CameraFollow : NetworkBehaviour
    {
        private CinemachineVirtualCamera virtualCamera;
        public static CameraFollow Instance => instance;
        private static CameraFollow instance;
      

        private void Awake()
        {
            instance = this;
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }


        public void FollowPlayer(Transform transform)

        {
        

        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;
        
        
        }

        
    }


    