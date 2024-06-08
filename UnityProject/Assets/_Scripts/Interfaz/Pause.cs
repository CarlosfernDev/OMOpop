using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    private bool isPaused = false;
    [SerializeField] private GameObject PauseCanvas;
    [SerializeField] private Button StartButton;

    public void PauseActive()
    {
        if (GameManager.Instance.roomStatus == GameManager.gameState.Over)
            return;


        isPaused = true;
        PauseCanvas.SetActive(true);
        StartButton.Select();
    }

    public void PauseDesactive()
    {
        isPaused = false;
        PauseCanvas.SetActive(false);
    }

    public void MainMenu()
    {
        MySceneManager.Instance.NextScene(1);
        WebSocketManager.Instance.LeaveGame();
    }

    public void inputPause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            return;
        }

        if (isPaused)
        {
            PauseDesactive();
            return;
        }
        PauseActive();
    }
}
