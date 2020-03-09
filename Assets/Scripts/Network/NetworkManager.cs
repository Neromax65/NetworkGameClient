using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Network.NetworkData;
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
        // [FormerlySerializedAs("_mainMenu")] [SerializeField] private MainMenu mainMenu;
        public static Dictionary<int, NetworkObject> NetworkIdentities;
    
        public string PlayerName { get; private set; }
    
        public static event Action<INetworkData> DataReceived;

        private void OnValidate()
        {
            // if (mainMenu == null)
            //     mainMenu = FindObjectOfType<MainMenu>();
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _client = new Client();
            NetworkIdentities = new Dictionary<int, NetworkObject>();
            DontDestroyOnLoad(gameObject);
        }


        void Start()
        {
            // _client.Connect("127.0.0.1", 9000);
            _client.DataReceived += OnDataReceived;
            // mainMenu.nameInputField.onValueChanged.AddListener(SetPlayerName);
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

        void Update()
        {
        
        }

        public static void Disconnect()
        {
            Instance._client.CloseConnection();
            SceneManager.LoadScene(0);
        }

        public static void SendDataToServer(INetworkData data)
        {
            Instance._client.SendData(data);
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
            Instance._client.SendData(new Data_Spawn()
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
            DataReceived?.Invoke(data);
            switch (data.Command)
            {
                case Command.Spawn:
                    OnSpawnDataReceived(data as Data_Spawn);
                    break;
                default:
                    break;
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
            if (_client != null)
            {
                await _client.ClientLoop();
            }
        }

        private void OnApplicationQuit()
        {
            _client.CloseConnection();
        }
    }
}
