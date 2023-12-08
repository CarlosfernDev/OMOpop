using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlock : MonoBehaviour
{
    static public Dictionary<int, BlockLogic> BlockIdDictionary;

    private void Awake()
    {
        startDictionary(); 
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
