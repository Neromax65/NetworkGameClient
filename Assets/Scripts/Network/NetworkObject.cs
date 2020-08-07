using Network.NetworkData;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// Class represent object that should be shared through network
    /// </summary>
    public class NetworkObject : MonoBehaviour
    {
        /// <summary>
        /// Network identity of object
        /// </summary>
        public int networkId;
        
        /// <summary>
        /// Index of Unity prefab, that represent this object, -1 means no prefab
        /// </summary>
        public int prefabIndex = -1;
        
        /// <summary>
        /// Player network identity, that owns this object 
        /// </summary>
        public int owningPlayerId;

        private void Awake()
        {
            networkId = NetworkIdGenerator.Generate();
        }
        
        void Start()
        {
            RegisterSelf();
        }

        /// <summary>
        /// Register this object on the network
        /// </summary>
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

        /// <summary>
        /// Unregister this object on the network
        /// </summary>
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
            UnregisterSelf();
        }
    }
}
