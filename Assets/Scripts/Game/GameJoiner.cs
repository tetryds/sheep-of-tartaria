using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sheep
{
    public class GameJoiner : MonoBehaviour
    {
        [SerializeField] TMP_InputField networkAddress;
        [SerializeField] string defaultAddress;

        private void Start()
        {
            networkAddress.text = defaultAddress;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                SheepNetworkManager manager = FindObjectOfType<SheepNetworkManager>();
                
                manager.networkAddress = networkAddress.text;
                manager.StartClient();
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                SheepNetworkManager manager = FindObjectOfType<SheepNetworkManager>();

                manager.StartHost();
            }
        }
    }
}
