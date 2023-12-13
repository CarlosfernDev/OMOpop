using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartTimmer : MonoBehaviour
{
    int TimerValue;
    private float TimeReference;
    private int MaxTimerValue;

    string firsttext = "The match start in: ";
    [SerializeField] private TMP_Text _text;

    Coroutine TimerRoutine;

    private void Awake()
    {
        if (WebSocketManager.Instance.TimerValue != null)
        {
            StartTimmerFunction(WebSocketManager.Instance.TimerValue);
        }

        WebSocketManager.Instance.OnStartTimer = StartTimmerFunction;
        WebSocketManager.Instance.OnEndTimer = EndTimmerFunction;
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

    public void EndTimmerFunction()
    {
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
    }
}
