using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    private Dictionary<string, int> _tagDictionary;
    [SerializeField] private Transform _SpawnPoint;

    [SerializeField] private BallStats _BallStats;
    [SerializeField] private Rigidbody2D _rb;
    private Vector2 _Velocity;

    bool isHittedX = false;
    bool isHittedY = false;
    bool isLaunched = false;

    private void Awake()
    {
        LearnDictionaryTags();
    }

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
        //Launch();
    }

    
    private void LateUpdate()
    {
        if (!isLaunched)
            return;

       isHittedX = false;
       isHittedY = false;
    }

    private void FixedUpdate()
    {
        if (!isLaunched)
            return;

        _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * Mathf.Clamp(Mathf.Abs(_rb.velocity.x),0.5f, _BallStats.initialSpeed - 0.5f), 
            Mathf.Sign(_rb.velocity.y) * Mathf.Clamp(Mathf.Abs(_rb.velocity.y), 0.5f, _BallStats.initialSpeed - 0.5f));
    }

    public void Launch()
    {
        if (isLaunched)
            return;

        isLaunched = true;

        _rb.isKinematic = false;
        transform.parent = null;

        _Velocity.x = 0.35f;
        _Velocity.y = 0.65f;
        _Velocity = _Velocity * _BallStats.initialSpeed;
        _rb.velocity = _Velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!_tagDictionary.ContainsKey(collision.tag))
            return;

        switch (_tagDictionary[collision.tag])
        {
            case 1:
                PlayerCollision(collision.gameObject, collision.ClosestPoint(transform.position));
                break;
            case 2:
                DeadZoneCollision(collision.gameObject);
                break;
            case 3:
                BrickCollision(collision.gameObject, collision.ClosestPoint(transform.position));
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
            if (!_tagDictionary.ContainsKey(collision.transform.tag))
                return;

        switch (_tagDictionary[collision.tag])
        {
            case 3:
                BrickCollision(collision.gameObject, collision.ClosestPoint(transform.position));
                break;
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (!_tagDictionary.ContainsKey(collision.transform.tag))
    //        return;

    //    switch (_tagDictionary[collision.transform.tag])
    //    {
    //        case 3:
    //            //BrickCollision(collision.gameObject);
    //            break;
    //    }
    //}

    private void Spawn()
    {
        isLaunched = false;
        _rb.isKinematic = true;
        transform.parent = _SpawnPoint.parent;
        transform.position = _SpawnPoint.position;
    }


    private void PlayerCollision(GameObject P, Vector2 HitPosition)
    {
        if (!isLaunched)
            return;

        PlayerController pc = P.GetComponent<PlayerController>();

        float DeltaHitPosition = HitPosition.x - P.transform.position.x;
        float angleIncision = (DeltaHitPosition / (pc._PlayerStats.scale.x/2)) * _BallStats.maxAngle;

        float SymbolX = P.GetComponent<PlayerController>()._InputAxisValue.x;
        //Debug.Log(angleIncision);
        //Debug.Log(Mathf.Sin(angleIncision));
        //_rb.velocity = new Vector2(((SymbolX != 0) ? (Mathf.Sign(SymbolX) * Mathf.Abs(_rb.velocity.x)) : _rb.velocity.x), (-MathF.Sign(P.transform.position.y)) * Mathf.Abs(_rb.velocity.y));
        _rb.velocity = new Vector2(Mathf.Sin(Mathf.Deg2Rad * angleIncision) * _BallStats.initialSpeed, (1 - Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * angleIncision))) * ((-MathF.Sign(P.transform.position.y)) * _BallStats.initialSpeed));
    }

    private void DeadZoneCollision(GameObject P)
    {
        _rb.velocity = Vector2.zero;
        Spawn();
    }

    private void BrickCollision(GameObject P, Vector2 HitPosition)
    {

        // Invertir la velocidad en el eje Y


        // Activar la funcion del bloque 

        // Script del bloque no poner aqui, Funcion publica de restar vida y que compruebe la vida del bloque y otra donde lo haga desaparecer.

        Debug.Log(Mathf.Abs((HitPosition.y - P.transform.position.y)) > (P.transform.localScale.y/2) - 0.047);
        Debug.Log(Mathf.Abs(HitPosition.x - P.transform.position.x) < Mathf.Abs(HitPosition.y - P.transform.position.y));
        Debug.Log(Mathf.Abs((HitPosition.x - P.transform.position.x)));
        Debug.Log(Mathf.Abs((HitPosition.y - P.transform.position.y))); 


        if((((Mathf.Abs((HitPosition.y - P.transform.position.y)) > (P.transform.localScale.y / 2) - 0.047)) || ((Mathf.Abs((HitPosition.x - P.transform.position.x)) < (P.transform.localScale.x / 2) - 0.047))) || isHittedX)
        {
            if (!isHittedY)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Sign(HitPosition.y - P.transform.position.y) * Mathf.Abs(_rb.velocity.y));
                isHittedY = true;
            }
        }
        else
        {
            if (!isHittedX)
            {
                _rb.velocity = new Vector2(Mathf.Sign(HitPosition.x - P.transform.position.x) * Mathf.Abs(_rb.velocity.x), _rb.velocity.y);
                isHittedX = true;
            }
        }


        P.GetComponent<BlockLogic>().RestarVida(1);
    }

    private void LearnDictionaryTags()
    {
        _tagDictionary = new Dictionary<string, int>();
        _tagDictionary.Add("Player", 1);
        _tagDictionary.Add("Deadzone", 2);
        _tagDictionary.Add("Brick", 3);
    }
}
