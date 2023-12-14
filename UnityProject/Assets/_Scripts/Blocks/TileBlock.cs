using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlock : MonoBehaviour
{
    static public Dictionary<int, BlockLogic> BlockIdDictionary;
    static public int BlocksNumber;

    private void Awake()
    {
        BlocksNumber = 0;
        startDictionary(); 
    }

    private void Start()
    {
        // To do mandar los bloques que me falta a la lista
    }

    void startDictionary()
    {
        BlockIdDictionary = new Dictionary<int, BlockLogic>();
    }

    static public void AddID(int ID, BlockLogic value)
    {
        if (!BlockIdDictionary.ContainsKey(ID))
        {
            BlockIdDictionary.Add(ID, value);
        }
        else
        {
            Debug.LogWarning("La ID: " + ID + " existe en " + value.gameObject.name);
        }
    }
}
