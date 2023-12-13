using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    public enum gameState {Waiting, Playing, Over};
    public gameState roomStatus = gameState.Waiting;
    

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
}
