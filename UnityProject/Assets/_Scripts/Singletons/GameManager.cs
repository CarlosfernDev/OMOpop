using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Pause _Pause;

    public enum gameState {Waiting, Playing, Over};
    public gameState roomStatus = gameState.Waiting;

    public GameObject GameOverCanvas;
    public TMP_Text EndText;
    public Button _Button;

    Dictionary<string, string> PositionNumber;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        LearnDictionary();
        WebSocketManager.Instance.OnEndMatch += SetEndMatch;
    }

    private void OnDisable()
    {
        WebSocketManager.Instance.OnEndMatch -= SetEndMatch;
    }

    private void Start()
    {
        WebSocketManager.Instance.OnStartTimer = SetWaiting;
    }

    public void SetWaiting(int value)
    {
        roomStatus = gameState.Waiting;
    }

    public void SetMatch()
    {
        roomStatus = gameState.Playing;
    }

    public void SetEndMatch(string value)
    {
        roomStatus = gameState.Over;
        _Pause.PauseDesactive();
        GameOverCanvas.SetActive(true);
        _Button.Select();
        EndText.text = value + (PositionNumber.ContainsKey(value) ? PositionNumber[value] : "th") + " place";
    }

    public void LearnDictionary()
    {
        PositionNumber = new Dictionary<string, string>();
        PositionNumber.Add("1","st");
        PositionNumber.Add("2", "nd");
        PositionNumber.Add("3", "rd");
    }
}
