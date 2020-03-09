﻿using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;
using UnityEngine.UI;

// TODO: TEMPORARY
public class DisconnectButton : MonoBehaviour
{
    private Button _disconnectButton;

    private void OnValidate()
    {
        if (_disconnectButton == null)
            _disconnectButton = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _disconnectButton.onClick.AddListener(NetworkManager.Instance.Disconnect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
