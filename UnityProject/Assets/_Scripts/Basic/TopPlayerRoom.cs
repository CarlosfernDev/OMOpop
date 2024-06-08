using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TopPlayerRoom : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;
    string disconected = "1";
    private void OnEnable()
    {
        WebSocketManager.Instance.OnTopChange += UpdateText;
        UpdateText(WebSocketManager.Instance.Top);
    }

    private void OnDisable()
    {
        WebSocketManager.Instance.OnTopChange -= UpdateText;
    }

    public void UpdateText(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _Text.text = value;
        else
            _Text.text = disconected;

    }
}