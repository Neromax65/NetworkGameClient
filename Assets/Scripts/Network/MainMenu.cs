using System;
using System.Net.Sockets;
using Network.NetworkData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Network
{
    public class MainMenu : MonoBehaviour
    {
        public TMP_InputField nameInputField;
    
        public TMP_InputField addressInputField;

        public Button connectButton;

        private void Start()
        {
            nameInputField.text = PlayerPrefs.GetString("PlayerName");
            addressInputField.text = PlayerPrefs.GetString("Address");
            addressInputField.onValueChanged.AddListener(SetAddress);
        }

        private void SetAddress(string address)
        {
            PlayerPrefs.SetString("Address", address);
            PlayerPrefs.Save();
        }
        
        private void SetName(string playerName)
        {
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.Save();
        }
        
        public void Connect()
        {
            var address = addressInputField.text.Split(':');
            string ip = address[0];
            int.TryParse(address[1], out int port);
            try
            {
                connectButton.interactable = false;
                NetworkManager.Instance.Connect(ip, port);
            }
            catch (Exception ex)
            {
                connectButton.interactable = true;
                return;
            }
            SceneManager.LoadScene(1);
        }
    }
}
