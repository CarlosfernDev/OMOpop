using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class StartTimmer : MonoBehaviour
{
    int TimerValue;
    public bool isStandBy = false;
    private float TimeReference;
    private int MaxTimerValue;

    string firsttext = "The match start in: ";
    string standbyText = "Waiting for players.";
    string standbyTextWithMore = "Waiting for players. Press enter to skip.";
    [SerializeField] private TMP_Text _text;

    Coroutine TimerRoutine;

    private void Awake()
    {
        if (WebSocketManager.Instance.TimerValue != 0)
        {
            StartTimmerFunction(WebSocketManager.Instance.TimerValue);
        }
        else
        {
            if(WebSocketManager.Instance.StartMatch)
                EndTimmerFunction();
            else
            {
                StandBy();
                WebSocketManager.Instance.StartMatch = false;
            }

        }

        WebSocketManager.Instance.OnStartTimer += StartTimmerFunction;
        WebSocketManager.Instance.OnEndTimer += StandBy;
        WebSocketManager.Instance.OnStartMatch += EndTimmerFunction;
    }

    private void OnDisable()
    {
        WebSocketManager.Instance.OnStartTimer -= StartTimmerFunction;
        WebSocketManager.Instance.OnEndTimer -= StandBy;
        WebSocketManager.Instance.OnStartMatch -= EndTimmerFunction;
    }

    public void StartTimmerFunction(int value)
    {
        _text.gameObject.SetActive(true);

        TimerValue = value;
        MaxTimerValue = value;
        TimeReference = Time.time;
        UpdateText();

        if (TimerRoutine != null)
            StopCoroutine(TimerRoutine);

        TimerRoutine = StartCoroutine(Timer(false));
    }

    public void StandBy()
    {
        isStandBy = true;

        if (TimerRoutine != null)
            StopCoroutine(TimerRoutine);

        if (int.Parse(WebSocketManager.Instance.playersConnectedInRoom) <= 1)
        {
            _text.text = standbyText;
        }
        else
        {
            _text.text = standbyTextWithMore;
        }
    }

    public void EndTimmerFunction()
    {
        isStandBy = false;

        if (TimerRoutine != null)
            StopCoroutine(TimerRoutine);

        TimerValue = 3;
        MaxTimerValue = 3;
        TimeReference = Time.time;
        UpdateText();

        TimerRoutine = StartCoroutine(Timer(true));
    }

    private void UpdateText()
    {
        _text.text = firsttext + TimerValue.ToString();
    }

    public void EnterInput(InputAction.CallbackContext ctx)
    {
        if (int.Parse(WebSocketManager.Instance.playersConnectedInRoom) <= 1)
        {
            return;
        }
        if (!ctx.performed)
        {
            return;
        }

        if (!isStandBy)
            return;

        WebSocketManager.Instance.SendMesage("StartMatch");
    }

    IEnumerator Timer(bool value)
    {
        while (Time.time - TimeReference <= MaxTimerValue)
        {
            yield return new WaitForSeconds(1f);
            TimerValue = MaxTimerValue - (int)(Time.time - TimeReference);
            UpdateText();
        }
        TimerValue = 0;
        UpdateText();

        if (value)
        {
            GameManager.Instance.roomStatus = GameManager.gameState.Playing;
            _text.gameObject.SetActive(false);
        }
        else
        {
            StandBy();
        }
    }
}
