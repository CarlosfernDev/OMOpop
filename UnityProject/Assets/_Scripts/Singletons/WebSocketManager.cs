using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class SendBlock
{
    public int RoomID;
    public int BlockID;
}

public class Data
{
    public string action = " ";
    public string Json;
}

public class WebSocketManager : MonoBehaviour
{

    public static WebSocketManager Instance;
    public Action OnWebSocketInstance;

    WebSocketSharp.WebSocket ws;
    public Data misDatos;

    [SerializeField] private SendBlock testDatos;

    #region UnityFunctions
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
    #endregion

    #region SendMessage

    public void onSendMessageToAll()
    {
        Data _data = new Data();
        _data.action = "sendToAll";
        ws.Send(JsonUtility.ToJson(_data));
    }

    public void SendBlock(SendBlock value)
    {
        Data Message = new Data();
        Message.action = "SendBlock";
        Message.Json = JsonUtility.ToJson(value);

        ws.Send(JsonUtility.ToJson(Message));
    }

    #endregion

    #region GetMessage
    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        print("mensaje recibido");
        Debug.Log(">>> " + e.Data.ToString());
        misDatos = JsonUtility.FromJson<Data>(e.Data.ToString());
        //misDatos = JsonUtility.FromJson<SendBlock>(Json);
        print("----fin mensaje recibido");
    }

    IEnumerator sincronizar()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        while (true)
        {
            if (misDatos != null)
            {
                switch (misDatos.action)
                {
                    case "SendBlock":
                        SendBlock Json = JsonUtility.FromJson<SendBlock>(misDatos.Json.ToString());
                        ReciveBlock(Json);
                        break;
                    default:
                        break;
                }
                misDatos = null;
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

    }

    private void ReciveBlock(SendBlock value)
    {
        TileBlock.BlockIdDictionary[value.BlockID].CanISend = false;
        StartCoroutine(GetBlock(TileBlock.BlockIdDictionary[value.BlockID]));
    }
    #endregion

    #region Corutines
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
    #endregion
}
