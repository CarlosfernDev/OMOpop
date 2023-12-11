using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableButtonNoOnline : MonoBehaviour
{
    [SerializeField] private Button _Button;
    void Start()
    {
        _Button.interactable = WebSocketManager.Instance.IsConnected;
    }

    private void OnEnable()
    {
        WebSocketManager.Instance.OnServerConnect += EnableButton;
        WebSocketManager.Instance.OnServerClose += DisableButton;
    }

    private void OnDisable()
    {
        WebSocketManager.Instance.OnServerConnect -= EnableButton;
        WebSocketManager.Instance.OnServerClose -= DisableButton;
    }

    public void EnableButton()
    {
        _Button.interactable = true;
    }

    private void DisableButton()
    {
        _Button.interactable = false;
    }
}
