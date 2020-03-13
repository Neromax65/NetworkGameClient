using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Network.NetworkData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {

        public static NetworkManager Instance;
    
        private Client _client;
        [SerializeField] private GameObject[] spawnablePrefabs;
        private Dictionary<int, NetworkObject> _networkObjects;
    
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
            // logger = Debug.unityLogger;
            _networkObjects = new Dictionary<int, NetworkObject>();
            DontDestroyOnLoad(gameObject);
        }


        void Start()
        {
            _client.DataReceived += OnDataReceived;
            PlayerName = PlayerPrefs.GetString("PlayerName");
        }

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

        public static void RegisterNetworkObject(NetworkObject networkObject)
        {
            if (Instance._networkObjects.ContainsKey(networkObject.networkId) || Instance._networkObjects.ContainsValue(networkObject))
            {
                Debug.LogError($"Trying to register network object with id: {networkObject.networkId}, that already registered.");
                return;
            }
            Instance._networkObjects.Add(networkObject.networkId, networkObject);
        }
        
        public static void UnregisterNetworkObject(NetworkObject networkObject)
        {
            if (!Instance._networkObjects.ContainsKey(networkObject.networkId) || !Instance._networkObjects.ContainsValue(networkObject))
            {
                Debug.LogError($"Trying to unregister network object with id: {networkObject.networkId}, that is not in registered dictionary.");
                return;
            }
            Instance._networkObjects.Remove(networkObject.networkId);
        }
        
        void Update()
        {
        
        }

        public static void Disconnect()
        {
            Instance._networkObjects.Clear();
            Instance._client.CloseConnection();
            SceneManager.LoadScene(0);
        }

        public static void SendDataToServer(INetworkData data)
        {
            Instance._client.SendDataAdd(data);
        }

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
            Instance._client.SendDataAdd(new Data_Spawn()
            {
                PrefabIndex = prefabIndex,
                PosX = position.x,
                PosY = position.y,
                PosZ = position.z,
                RotX = rotation.x,
                RotY = rotation.y,
                RotZ = rotation.z,
                RotW = rotation.w
            });
            return spawnedObject;
        }

        private void OnDataReceived(INetworkData data)
        {
            // DataReceived?.Invoke(data);
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
                default:
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
            if (syncTransform)
            {
                syncTransform.OnPositionDataReceived(data);
            }
            else
            {
                networkObject.transform.position = new Vector3(data.X, data.Y, data.Z);
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
            if (syncTransform)
            {
                syncTransform.OnRotationDataReceived(data);
            }
            else
            {
                networkObject.transform.rotation = new Quaternion(data.X, data.Y, data.Z, data.W);
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
            if (syncTransform)
            {
                syncTransform.OnScaleDataReceived(data);
            }
            else
            {
                networkObject.transform.localScale = new Vector3(data.X, data.Y, data.Z);
            }
        }
        
        private void OnSpawnDataReceived(Data_Spawn data)
        {
            var prefab = spawnablePrefabs[data.PrefabIndex];
            var position = new Vector3(data.PosX, data.PosY, data.PosZ);
            var rotation = new Quaternion(data.RotX, data.RotY, data.RotZ, data.RotW);
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
                _client.CloseConnection();
        }

        private void OnApplicationQuit()
        {
            _client.CloseConnection();
        }
    }
}
