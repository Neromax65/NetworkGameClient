using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Network.NetworkData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {

        public static NetworkManager Instance;
    
        private Client _client;
        [SerializeField] private GameObject[] spawnablePrefabs;
        [SerializeField] private MainMenu _mainMenu;
        public static Dictionary<int, NetworkIdentity> NetworkIdentities;
    
        public string PlayerName { get; private set; }
    
        public static event Action<INetworkData> DataReceived;

        private void OnValidate()
        {
            if (_mainMenu == null)
                _mainMenu = FindObjectOfType<MainMenu>();
        }

        private void Awake()
        {
            Instance = this;
            _client = new Client();
            NetworkIdentities = new Dictionary<int, NetworkIdentity>();
            DontDestroyOnLoad(gameObject);
        }


        void Start()
        {
            // _client.Connect("127.0.0.1", 9000);
            _client.DataReceived += OnDataReceived;
            _mainMenu.nameInputField.onValueChanged.AddListener(SetPlayerName);
            PlayerName = PlayerPrefs.GetString("PlayerName");
        }

        private void SetPlayerName(string playerName)
        {
            // Debug.Log("Changing name and saving player prefs");
            PlayerName = playerName;
            PlayerPrefs.SetString("PlayerName", PlayerName);
            PlayerPrefs.Save();
        }

        public void Connect()
        {
            var address = _mainMenu.addressInputField.text.Split(':');
            string ip = address[0];
            int.TryParse(address[1], out int port);
            try
            {
                _mainMenu.connectButton.interactable = false;
                Instance._client.Connect(ip, port);
            }
            catch (SocketException ex)
            {
                Debug.LogError($"Connection error: {ex.Message}");
                _mainMenu.connectButton.interactable = true;
                return;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: {ex.Message}");
                _mainMenu.connectButton.interactable = true;
                return;
            }
            SendDataToServer(new Data_Connect());
            SceneManager.LoadScene(1);
        }

        void Update()
        {
        
        }

        public void Disconnect()
        {
            SendDataToServer(new Data_Disconnect());
            _client.CloseConnection();
            SceneManager.LoadScene(0);
        }

        public static void SendDataToServer(INetworkData data)
        {
            Instance._client.SendData(data);
        }

        public static GameObject SpawnGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var spawnedObject = Instantiate(prefab, position, rotation);
            var prefabIndex = Array.IndexOf(Instance.spawnablePrefabs, prefab);
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

        private void OnDestroy()
        {
            DontDestroyOnLoad(this);
        }
    }
}
