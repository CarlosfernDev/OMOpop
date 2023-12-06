using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLogic : MonoBehaviour
{
    public int vidaMaxima = 1; // Puedes ajustar la vida inicial del bloque
    private int vidaActual;

    public string ID;


    //Funcion publica de restar vida y que compruebe la vida del bloque y otra donde lo haga desaparecer.

    private void Start()
    {
        vidaActual = vidaMaxima;
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pelota"))
        {
            RestarVida();
            comprobarVida();
        }
    }*/

    public void RestarVida(int value)
    {
        // Restar vida al bloque
        vidaActual -= value;
        comprobarVida();
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
    }
}
