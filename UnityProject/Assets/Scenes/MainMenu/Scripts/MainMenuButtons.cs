using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private TMP_InputField _Label;
    public void JoinPublicLobby()
    {
        WebSocketManager.Instance.username = _Label.text;
        WebSocketManager.Instance.JoinPublicRoom();
    }

    public void ConnectServer()
    {
        WebSocketManager.Instance.StartWS();
    }
}
