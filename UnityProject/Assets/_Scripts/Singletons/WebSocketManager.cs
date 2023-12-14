using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WebSocketSharp;

public class SendBlock
{
    public int BlockID;
    public int BlockRemain;
}

public class OneData
{
    public string Value1;
}
[Serializable]
public class Data
{
    public string action = " ";
    public string roomID = " ";
    public string user = " ";
    public string Json;
}

public class WebSocketManager : MonoBehaviour
{
    private string url;
    public string username;
    public string roomID;
    public string playersConnectedInRoom;
    public string playersConnected;

    public static WebSocketManager Instance;
    public Action OnWebSocketInstance;
    public Action<string> OnPingChange;
    public Action<string> OnLocalPlayerChange;
    public Action<string> OnPlayerChange;

    public int TimerValue = 0;
    public bool StartMatch = false;
    public Action<int> OnStartTimer;
    public Action OnEndTimer;
    public Action OnStartMatch;

    public Action OnServerClose;
    public Action OnServerConnect;


    WebSocketSharp.WebSocket ws;
    public string ping;
    public bool IsConnected;
    public List<Data> misDatos;

    Coroutine SincronizarRoutine;
    [SerializeField] private SendBlock testDatos;

    #region UnityFunctions
    private void Awake()
    {
        TimerValue = 0;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        if (Application.isEditor)
        {
            url = Resources.Load("Url").ToString();
        }
        else
        {
            if (!System.IO.Directory.Exists(Application.dataPath + "/UrlConfig"))
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + "/UrlConfig");
            }

            string path = Application.dataPath + "/UrlConfig/Url.txt";

            if (!System.IO.File.Exists(path))
            {
                System.IO.File.WriteAllText(path, Resources.Load("Url").ToString());
            }

            string[] text = System.IO.File.ReadAllLines(path);
            url = text[0];
        }

        StartWS();

    }

    private void OnDisable()
    {
        ws.Close();
    }

    public void StartWS()
    {
        if (ws != null && ws.IsAlive)
            return;

        try
        {
            ws.Close();
            if (SincronizarRoutine != null)
                StopCoroutine(SincronizarRoutine);
        }
        catch (Exception)
        {
            Debug.Log("I can't disconnect");
        }

        try
        {
            ws = new WebSocket(url);
            ws.OnMessage += Ws_OnMessage;
            ws.Connect();

            if (OnServerConnect != null)
                OnServerConnect();

            IsConnected = true;

            SincronizarRoutine = StartCoroutine(sincronizar());

            //OnWebSocketInstance.Invoke();
        }
        catch (Exception)
        {
            Debug.Log("I can't connect");
        }
    }
    #endregion

    #region SendMessage

    public bool PreMessage()
    {
        if (!ws.Ping())
        {
            return false;
        }

        return true;
    }

    public void SendMesage(string value)
    {
        if (!PreMessage())
            return;

        Data _data = new Data();
        _data.roomID = roomID;
        _data.user = username;
        _data.action = value;
        ws.Send(JsonUtility.ToJson(_data));
    }

    public void onSendMessageToAll()
    {
        if (!PreMessage())
            return;

        Data _data = new Data();
        _data.roomID = roomID;
        _data.user = username;
        _data.action = "onSendMessageToAll";
        ws.Send(JsonUtility.ToJson(_data));
    }

    public void JoinPublicRoom()
    {
        if (!PreMessage())
            return;

        Data _data = new Data();
        _data.action = "JoinPublicRoom";
        _data.roomID = roomID;
        try
        {
            ws.Send(JsonUtility.ToJson(_data));
        }
        catch (Exception)
        {
            Debug.Log("error");
        }
    }

    public void SetLocalPlayerData(string Value)
    {
        playersConnectedInRoom = Value;
        if (OnLocalPlayerChange != null)
        {
            OnLocalPlayerChange(Value);
        }
    }

    public void SendBlock(SendBlock value)
    {
        if (!PreMessage())
            return;

        Data _data = new Data();
        _data.action = "SendBlock";
        _data.roomID = roomID;
        _data.Json = JsonUtility.ToJson(value);

        ws.Send(JsonUtility.ToJson(_data));
    }

    public void SendBlockRemain(int value)
    {
        if (!PreMessage())
            return;

        Data _data = new Data();
        _data.action = "BlocksRemain";
        _data.roomID = roomID;

        OneData Class = new OneData();
        Class.Value1 = value.ToString();

        _data.Json = JsonUtility.ToJson(Class);

        ws.Send(JsonUtility.ToJson(_data));
    }

    #endregion

    #region GetMessage
    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        //print("mensaje recibido");
        Debug.Log(">>> " + e.Data.ToString());
        try
        {
            //Debug.Log("Recibido exitosamente");
            //int i = 0;
            //if(misDatos.Length != 0){
            //    i = misDatos.Length;
            //}

            misDatos.Add(JsonUtility.FromJson<Data>(e.Data.ToString()));
            //Debug.Log(JsonUtility.FromJson<Data>(e.Data.ToString()).Json.ToString());
        }
        catch (Exception v)
        {
            Debug.Log("error procesando el dato");
        }
        //misDatos = JsonUtility.FromJson<SendBlock>(Json);
    }

    IEnumerator sincronizar()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        while (true)
        {
            float TimeReference;

            if (misDatos != null)
            {
                List<Data> backUpDatos = new List<Data>(misDatos);
                misDatos.Clear();

                foreach (Data midato in backUpDatos)
                {
                    switch (midato.action)
                    {
                        case "SendBlock":
                            SendBlock JsonBlock = JsonUtility.FromJson<SendBlock>(midato.Json.ToString());
                            ReciveBlock(JsonBlock);
                            break;
                        case "JoinPublicRoom":
                            ReciveJoinPublicRoom(midato.roomID);
                            break;
                        case "SetPing":
                            string JsonPing = JsonUtility.FromJson<OneData>(midato.Json.ToString()).Value1;
                            GetPing(JsonPing);
                            break;
                        case "GetPlayerNumber":
                            string JsonPlayer = JsonUtility.FromJson<OneData>(midato.Json.ToString()).Value1;
                            GetUser(JsonPlayer);
                            break;
                        case "SetLocalPlayerCount":
                            string JsonLocalPlayer = JsonUtility.FromJson<OneData>(midato.Json.ToString()).Value1;
                            SetLocalPlayerData(JsonLocalPlayer);
                            break;
                        case "TimerStarter":
                            string JsonLocalTimer = JsonUtility.FromJson<OneData>(midato.Json.ToString()).Value1;
                            SetPublicTimmer(JsonLocalTimer);
                            break;
                        case "TimerEnded":
                            if (OnEndTimer != null)
                                OnEndTimer.Invoke();
                            break;
                        case "StartMatch":
                            StartMatch = true;
                            if (OnStartMatch != null)
                                OnStartMatch.Invoke();
                            break;
                        default:
                            break;
                    }
                }
            }
            if (!ws.Ping(DateTime.UtcNow.ToString("O")))
            {
                Debug.Log("Conexion cerrada");

                if (OnPingChange != null)
                {
                    ping = null;
                    OnPingChange(null);
                    IsConnected = false;
                }

                if (OnServerClose != null)
                    OnServerClose();

                break;
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    private void SetPublicTimmer(string value)
    {
        if (OnStartTimer != null)
        {
            OnStartTimer.Invoke(int.Parse(value));
        }
        TimerValue = int.Parse(value);
    }

    private void ReciveJoinPublicRoom(string recivedRoomID)
    {
        roomID = recivedRoomID;
        MySceneManager.Instance.NextScene(20);
    }

    private void ReciveBlock(SendBlock value)
    {
        TileBlock.BlockIdDictionary[value.BlockID].CanISend = false;
        StartCoroutine(GetBlock(TileBlock.BlockIdDictionary[value.BlockID]));
    }

    private void GetPing(string Ping)
    {
        ping = Ping;
        if (OnPingChange != null)
        {
            OnPingChange(Ping);
        }
    }

    private void GetUser(string Value)
    {
        playersConnected = Value;
        if (OnPlayerChange != null)
        {
            OnPlayerChange(Value);
        }

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
