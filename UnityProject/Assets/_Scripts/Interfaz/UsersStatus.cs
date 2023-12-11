using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UsersStatus : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;
    string disconected = "Players: N/A";
    private void OnEnable()
    {
        WebSocketManager.Instance.OnPlayerChange += UpdateText;
        UpdateText(WebSocketManager.Instance.playersConnected);
    }

    private void OnDisable()
    {
        WebSocketManager.Instance.OnPlayerChange -= UpdateText;
    }

    public void UpdateText(string value)
    {
        Debug.Log("Se conecto alguien " + value);

        if (!string.IsNullOrEmpty(value))
            _Text.text = "Players: " + value;
        else
            _Text.text = disconected;

    }
}
