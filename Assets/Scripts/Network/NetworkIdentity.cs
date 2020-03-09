using System;
using Network.NetworkData;
using UnityEngine;
using UnityEngine.Serialization;

namespace Network
{
    public class NetworkIdentity : MonoBehaviour
    {
        public int networkId;
        public int prefabIndex;
        public int owningPlayerId;

        private void Awake()
        {
            networkId = NetworkIdGenerator.Generate();
        }
        
        void Start()
        {
            Register();
        }

        void Register()
        {
            
            NetworkManager.SendDataToServer(new Data_Register()
            {
                Id = networkId,
                PrefabIndex = prefabIndex,
                OwningPlayerId = owningPlayerId
            });
        }

        void Unregister()
        {
            
        }
    }
}
