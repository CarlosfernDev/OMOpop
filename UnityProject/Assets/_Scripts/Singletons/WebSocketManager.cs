using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class DataSend
{
    public int RoomID;
    public int BlockID;
}

public class Data
{
    public string action = " ";
    /*string payload = new
    {
        int playerID = "";
    int roomID = "";
    int playerName = "";
    int message = "";
    };*/
}

public class WebSocketManager : MonoBehaviour
{

    public static WebSocketManager Instance;
    public Action OnWebSocketInstance;

    WebSocketSharp.WebSocket ws;
    public DataSend misDatos;

    [SerializeField] private DataSend testDatos;

    private void Awake()
    {
        Instance = this;

        ws = new WebSocket("ws://192.168.1.131:3000");
        ws.OnMessage += Ws_OnMessage;
        ws.Connect();


        StartCoroutine(sincronizar());

        //OnWebSocketInstance.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            onSendMessageToAll();
        }
    }

    private void OnDisable()
    {
        ws.Close();
    }

    public void SendBlock(DataSend value)
    {
        ws.Send(JsonUtility.ToJson(value));
    }

    public void onSendMessageToAll()
    {
        Data _data = new Data();
        _data.action = "sendToAll";
        ws.Send(JsonUtility.ToJson(_data));
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        print("mensaje recibido");
        Debug.Log(">>> " + e.Data.ToString());
        misDatos = JsonUtility.FromJson<DataSend>(e.Data.ToString());

        //Debug.Log(TileBlock.BlockIdDictionary[misDatos.BlockID].gameObject.name);
        //StartCoroutine(GetBlock(TileBlock.BlockIdDictionary[misDatos.BlockID].gameObject));

        print("----fin mensaje recibido");
        // Poner Bloque
    }

    IEnumerator GetBlock(BlockLogic value)
    {
        yield return new WaitForSeconds(1f);

        if (value.gameObject.activeSelf)
        {
            value.AddVida(1);
        }
        else
        {
            value.gameObject.SetActive(true);
            Debug.Log("Activada");
        }
    }

    IEnumerator sincronizar()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        while (true)
        {
            if (misDatos != null)
            {
                TileBlock.BlockIdDictionary[misDatos.BlockID].CanISend = false;

                StartCoroutine(GetBlock(TileBlock.BlockIdDictionary[misDatos.BlockID]));
                misDatos = null;
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

    }
}
