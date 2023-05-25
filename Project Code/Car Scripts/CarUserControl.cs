//This file was modified by us
using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Unity.Netcode;
using Unity.Netcode.Samples;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    [RequireComponent(typeof(ClientNetworkTransform))] //Added by us
    public class CarUserControl : NetworkBehaviour   //We changed it to NetworkBehaviour
    {
        private CarController m_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            if (!IsOwner) //if statement was added by us
                return;
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
