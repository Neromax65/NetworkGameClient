using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Network
{
    public class NetworkClient : MonoBehaviour
    {
        public static NetworkClient Instance;
        private void Awake()
        {
            Instance = this;
            client = new Client();
            client.Connect("127.0.0.1", 9000);
            _connected = true;
        }

        public Client client;
        private bool _connected;
        void Start()
        {
        }

        void FixedUpdate()
        {
            if (!_connected)
                return;
            client.ClientLoop();
        }
        
    }
}
