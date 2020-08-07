using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Network
{
    public class MainMenu : MonoBehaviour
    {
        /// <summary>
        /// Player name
        /// </summary>
        public TMP_InputField nameInputField;
    
        /// <summary>
        /// IP and Port address input field
        /// </summary>
        public TMP_InputField addressInputField;

        /// <summary>
        /// Connect to server button
        /// </summary>
        public Button connectButton;

        private void Start()
        {
            nameInputField.text = PlayerPrefs.GetString("PlayerName");
            addressInputField.text = PlayerPrefs.GetString("Address");
            addressInputField.onValueChanged.AddListener(SetAddress);
        }

        /// <summary>
        /// Save network address to PlayerPreferences
        /// </summary>
        /// <param name="address">Network address</param>
        private void SetAddress(string address)
        {
            PlayerPrefs.SetString("Address", address);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Save player name to PlayerPreferences 
        /// </summary>
        /// <param name="playerName">Player name</param>
        private void SetName(string playerName)
        {
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Connect to the server
        /// </summary>
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
