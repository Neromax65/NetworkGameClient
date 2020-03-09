using System;
using TMPro;
using UnityEngine;
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
        }
    }
}
