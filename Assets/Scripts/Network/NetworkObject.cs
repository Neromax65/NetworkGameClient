using System;
using Network.NetworkData;
using UnityEngine;
using UnityEngine.Serialization;

namespace Network
{
    public class NetworkObject : MonoBehaviour
    {
        public int networkId;
        public int prefabIndex = -1;
        public int owningPlayerId;

        private void Awake()
        {
            networkId = NetworkIdGenerator.Generate();
        }
        
        void Start()
        {
            RegisterSelf();
        }

        void RegisterSelf()
        {
            NetworkManager.RegisterNetworkObject(this);
            NetworkManager.SendDataToServer(new Data_Register()
            {
                Id = networkId,
                PrefabIndex = prefabIndex,
                OwningPlayerId = owningPlayerId
            });
        }

        void UnregisterSelf()
        {
            NetworkManager.UnregisterNetworkObject(this);
            NetworkManager.SendDataToServer(new Data_Unregister()
            {
                Id = networkId
            });
        }

        private void OnDestroy()
        {
            // Unregister();
        }
    }
}
