using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextCodeRoom : MonoBehaviour
{
    [SerializeField] private TMP_Text _Text;
    private string Offline = "Offline";
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _Text.text = WebSocketManager.Instance.roomID;
        }
        catch (Exception)
        {
            _Text.text = Offline;
        }
    }
}
