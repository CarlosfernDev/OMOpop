using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PingStatus : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;
    string disconected = "Disconnected";
    private void OnEnable()
    {
        WebSocketManager.Instance.OnPingChange += UpdateText;
        UpdateText(WebSocketManager.Instance.ping);
    }

    private void OnDisable()
    {
        WebSocketManager.Instance.OnPingChange -= UpdateText;
    }

    public void UpdateText(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _Text.text = "Ping: " + value;
        else
            _Text.text = disconected;

    }
}
