using Network;
using UnityEngine;
using UnityEngine.UI;

// TODO: TEMPORARY
public class DisconnectButton : MonoBehaviour
{
    [SerializeField] private Button _disconnectButton;

    private void OnValidate()
    {
        if (_disconnectButton == null)
            _disconnectButton = GetComponent<Button>();
    }
    private void Awake()
    {
        if (_disconnectButton == null)
            _disconnectButton = GetComponent<Button>();
    }

    void Start()
    {
        _disconnectButton.onClick.AddListener(NetworkManager.Disconnect);
    }
}
