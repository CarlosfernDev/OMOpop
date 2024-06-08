using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockLogic : MonoBehaviour
{
    public int vidaMaxima = 1; // Puedes ajustar la vida inicial del bloque
    private int vidaActual;

    public int ID;

    [SerializeField] private GameObject BlockUI;

    [HideInInspector]public bool CanISend = true;

    [SerializeField] private TMP_Text _Text;

    [SerializeField] private SpriteRenderer _Sprite;
    [SerializeField] private Color StandarColor;
    [SerializeField] private Color DisableColor;
    [SerializeField] private Color SpawningColor;
    [SerializeField] private Collider2D _Collider;

    //Funcion publica de restar vida y que compruebe la vida del bloque y otra donde lo haga desaparecer.

    private void Start()
    {
        TileBlock.AddID(ID,this);
        vidaActual = vidaMaxima;
        _Text.text = vidaActual.ToString();
    }

    private void OnEnable()
    {
        if (CanISend)
        {
            _Sprite.color = StandarColor;
            TileBlock.BlocksNumber += 1;
            vidaActual = vidaMaxima;
            _Text.text = vidaActual.ToString();
        }
        else
        {
            StartCoroutine(SetetingBlock());
        }
        Debug.Log(TileBlock.BlocksNumber);
    }

    private void OnDisable()
    {
        TileBlock.BlocksNumber -= 1;
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
        StartCoroutine(AddVidaRoutine(value));
    }

    public void RestarVida(int value)
    {
        // Restar vida al bloque
        vidaActual = Mathf.Clamp(vidaActual - value,0,999);
        _Text.text = vidaActual.ToString();
        comprobarVida();
        TileBlock.BlocksNumber -= 1;
        WebSocketManager.Instance.SendBlockRemain(TileBlock.BlocksNumber);
        Debug.Log(TileBlock.BlocksNumber);
    }

    public void comprobarVida()
    {
        if(vidaActual > 1)
        {
            BlockUI.SetActive(true);
        }
        else if(vidaActual == 1)
        {
            BlockUI.SetActive(false);
        }

        // Verificar si la vida llegó a cero para romper el bloque
        if (vidaActual <= 0)
        {
            TileBlock.BlocksNumber += 1;
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

    IEnumerator AddVidaRoutine(int value)
    {
        yield return new WaitForSeconds(1f);
        vidaActual = Mathf.Clamp(vidaActual + value, 0, 999);
        _Text.text = vidaActual.ToString();
        comprobarVida();
        TileBlock.BlocksNumber += 1;
        Debug.Log(TileBlock.BlocksNumber);
    }

    IEnumerator SetetingBlock()
    {
        _Collider.enabled = false;
        _Sprite.color = DisableColor;
        float timereference = Time.time;
        bool NewColor = false;
        while (true)
        {
            if(timereference + 1 < Time.time)
            {
                break;
            }

            if (NewColor)
            {
                _Sprite.color = DisableColor;
                NewColor = false;
            }
            else
            {
                _Sprite.color = SpawningColor;
                NewColor = true;
            }

            yield return new WaitForSeconds(0.15f);
        }
        _Sprite.color = DisableColor;
        TileBlock.BlocksNumber += 1;
        _Collider.enabled = true;
    }
}
