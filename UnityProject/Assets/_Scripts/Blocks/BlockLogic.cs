using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockLogic : MonoBehaviour
{
    public int vidaMaxima = 1; // Puedes ajustar la vida inicial del bloque
    private int vidaActual;

    public int ID;

    [HideInInspector]public bool CanISend = true;

    [SerializeField] private TMP_Text _Text;

    //Funcion publica de restar vida y que compruebe la vida del bloque y otra donde lo haga desaparecer.

    private void Start()
    {
        TileBlock.BlocksNumber += 1;
        TileBlock.AddID(ID,this);
        vidaActual = vidaMaxima;
        _Text.text = vidaActual.ToString();
    }

    private void OnEnable()
    {
        TileBlock.BlocksNumber += 1;
        vidaActual = vidaMaxima;
        _Text.text = vidaActual.ToString();
        Debug.Log(TileBlock.BlocksNumber);
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pelota"))
        {
            RestarVida();
            comprobarVida();
        }
    }*/

    public void AddVida(int value)
    {
        vidaActual = Mathf.Clamp(vidaActual + value, 0, 999);
        _Text.text = vidaActual.ToString();
        comprobarVida();
        TileBlock.BlocksNumber += 1;
        Debug.Log(TileBlock.BlocksNumber);
    }

    public void RestarVida(int value)
    {
        // Restar vida al bloque
        vidaActual = Mathf.Clamp(vidaActual - value,0,999);
        _Text.text = vidaActual.ToString();
        comprobarVida();
        TileBlock.BlocksNumber -= 1;
        Debug.Log(TileBlock.BlocksNumber);
    }

    public void comprobarVida()
    {
        // Verificar si la vida llegó a cero para romper el bloque
        if (vidaActual <= 0)
        {
            RomperBloque();
        }
    }

    public void RomperBloque()
    {
        Debug.Log("Bloque roto");
        gameObject.SetActive(false);
        SendBlock BlockMessage = new SendBlock();
        BlockMessage.BlockID = ID;

        if (!CanISend)
            return;

        WebSocketManager.Instance.SendBlock(BlockMessage);
    }
}
