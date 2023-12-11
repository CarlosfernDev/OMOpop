using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextOnlinePlayerRoom : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;
    string disconected = "1";
    private void OnEnable()
    {
        WebSocketManager.Instance.OnLocalPlayerChange += UpdateText;
        UpdateText(WebSocketManager.Instance.playersConnectedInRoom);
    }

    private void OnDisable()
    {
        WebSocketManager.Instance.OnLocalPlayerChange -= UpdateText;
    }

    public void UpdateText(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _Text.text = value;
        else
            _Text.text = disconected;

    }
}