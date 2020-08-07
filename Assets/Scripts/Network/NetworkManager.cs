using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Network.NetworkData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    /// <summary>
    /// Main class to operate network functionality
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton implementation
        /// </summary>
        public static NetworkManager Instance;
    
        /// <summary>
        /// Current network client
        /// </summary>
        private Client _client;
        
        /// <summary>
        /// Collection of prefabs that can be spawn through network
        /// </summary>
        [SerializeField] private GameObject[] spawnablePrefabs;
        
        /// <summary>
        /// Dictionary that stores all network objects that belong to this client
        /// </summary>
        private Dictionary<int, NetworkObject> _networkObjects;
    
        /// <summary>
        /// Name of the player
        /// </summary>
        public string PlayerName { get; private set; }
    
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _client = new Client();
            _networkObjects = new Dictionary<int, NetworkObject>();
            DontDestroyOnLoad(gameObject);
        }


        void Start()
        {
            _client.DataReceived += OnDataReceived;
            PlayerName = PlayerPrefs.GetString("PlayerName");
        }

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <param name="ip">IP-address</param>
        /// <param name="port">Port</param>
        public void Connect(string ip, int port)
        {
            try
            {
                _client.Connect(ip, port);
            }
            catch (SocketException ex)
            {
                Debug.LogErrorFormat($"Connection error: {ex.Message}");
                return;    
            }
            SendDataToServer(new Data_Connect()
            {
                PlayerName = PlayerName
            });
        }

        /// <summary>
        /// Register new network object
        /// </summary>
        /// <param name="networkObject">Network object to register</param>
        public static void RegisterNetworkObject(NetworkObject networkObject)
        {
            if (Instance._networkObjects.ContainsKey(networkObject.networkId) || Instance._networkObjects.ContainsValue(networkObject))
            {
                Debug.LogError($"Trying to register network object with id: {networkObject.networkId}, that already registered.");
                return;
            }
            Instance._networkObjects.Add(networkObject.networkId, networkObject);
        }
        
        /// <summary>
        /// Remove object from registered network objects
        /// </summary>
        /// <param name="networkObject">Object to unregister</param>
        public static void UnregisterNetworkObject(NetworkObject networkObject)
        {
            if (!Instance._networkObjects.ContainsKey(networkObject.networkId) || !Instance._networkObjects.ContainsValue(networkObject))
            {
                Debug.LogError($"Trying to unregister network object with id: {networkObject.networkId}, that is not in registered dictionary.");
                return;
            }
            Instance._networkObjects.Remove(networkObject.networkId);
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public static void Disconnect()
        {
            Instance._networkObjects.Clear();
            Instance._client.Disconnect();
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// Send network data to server
        /// </summary>
        /// <param name="data">Data to send</param>
        public static void SendDataToServer(INetworkData data)
        {
            Instance._client.SendDataAddToPacket(data);
        }

        /// <summary>
        /// Spawn new network object
        /// </summary>
        /// <param name="prefab">GameObject prefab to spawn</param>
        /// <param name="position">Initial position</param>
        /// <param name="rotation">Initial rotation</param>
        /// <returns>Spawned GameObject</returns>
        public static GameObject SpawnGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var spawnedObject = Instantiate(prefab, position, rotation);
            if (!Instance.spawnablePrefabs.Contains(prefab))
            {
                Debug.LogError($"You trying to spawn prefab to network, that is not in the SpawnablePrefabs list.");
                return spawnedObject;
            }
            var prefabIndex = Array.IndexOf(Instance.spawnablePrefabs, prefab);
            spawnedObject.GetComponent<NetworkObject>().prefabIndex = prefabIndex;
            Instance._client.SendDataAddToPacket(new Data_Spawn()
            {
                PrefabIndex = prefabIndex,
                Position = position,
                Rotation = rotation
            });
            return spawnedObject;
        }

        /// <summary>
        /// Data received callback
        /// </summary>
        /// <param name="data">Received network data</param>
        private void OnDataReceived(INetworkData data)
        {
            switch (data.Command)
            {
                case Command.Position:
                    OnPositionDataReceived(data as Data_Position);
                    break;
                case Command.Rotation:
                    OnRotationDataReceived(data as Data_Rotation);
                    break;
                case Command.Scale:
                    OnScaleDataReceived(data as Data_Scale);
                    break;
                case Command.Spawn:
                    OnSpawnDataReceived(data as Data_Spawn);
                    break;
            }
        }

        private void OnPositionDataReceived(Data_Position data)
        {
            if (!_networkObjects.ContainsKey(data.Id))
            {
                Debug.LogError($"Could not find network object with id: {data.Id}");
                return;
            }
            NetworkObject networkObject = _networkObjects[data.Id];
            SyncTransform syncTransform = networkObject.GetComponent<SyncTransform>();
            
            if (syncTransform && !data.Instantly)
            {
                syncTransform.OnPositionDataReceived(data);
            }
            else
            {
                networkObject.transform.position = data.Position;
            }
        }

        private void OnRotationDataReceived(Data_Rotation data)
        {
            if (!_networkObjects.ContainsKey(data.Id))
            {
                Debug.LogError($"Could not find network object with id: {data.Id}");
                return;
            }
            NetworkObject networkObject = _networkObjects[data.Id];
            SyncTransform syncTransform = networkObject.GetComponent<SyncTransform>();
            if (syncTransform && !data.Instantly)
            {
                syncTransform.OnRotationDataReceived(data);
            }
            else
            {
                networkObject.transform.rotation = data.Rotation;
            }
        }
        
        private void OnScaleDataReceived(Data_Scale data)
        {
            if (!_networkObjects.ContainsKey(data.Id))
            {
                Debug.LogError($"Could not find network object with id: {data.Id}");
                return;
            }
            NetworkObject networkObject = _networkObjects[data.Id];
            SyncTransform syncTransform = networkObject.GetComponent<SyncTransform>();
            if (syncTransform && !data.Instantly)
            {
                syncTransform.OnScaleDataReceived(data);
            }
            else
            {
                networkObject.transform.localScale = data.Scale;
            }
        }
        
        private void OnSpawnDataReceived(Data_Spawn data)
        {
            var prefab = spawnablePrefabs[data.PrefabIndex];
            var position = data.Position;
            var rotation = data.Rotation;
            Instantiate(prefab, position, rotation);
        }

        private async void FixedUpdate()
        {
            if (_client?.ServerConnection != null && _client.ServerConnection.Connected)
            {
                await _client.ClientLoop();
            }
        }

        private void OnDestroy()
        {
            if (this == Instance)
                _client.Disconnect();
        }

        private void OnApplicationQuit()
        {
            _client.Disconnect();
        }
    }
}
