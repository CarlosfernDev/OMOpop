using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

[Serializable]
public class DataSend
{
    public int RoomID;
    public int BlockID;
}

public class WebSocketManager : MonoBehaviour
{

    WebSocketSharp.WebSocket ws;
    public DataSend misDatos;

    [SerializeField] private DataSend testDatos;

    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket("ws://10.2.96.9:3000");
        ws.OnMessage += Ws_OnMessage;
        ws.Connect();
    }

    private void OnDisable()
    {
        ws.Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(JsonUtility.ToJson(testDatos));
            ws.Send(JsonUtility.ToJson(testDatos));
        }
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        print("mensaje recibido");
        Debug.Log(">>> "+e.Data.ToString());
        misDatos = JsonUtility.FromJson<DataSend>(e.Data.ToString());
        print("----fin mensaje recibido");
        // Poner Bloque
    }
}
