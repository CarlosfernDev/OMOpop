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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch()
    {
        if (isLaunched)
            return;

        isLaunched = true;

        _rb.isKinematic = false;
        transform.parent = null;

        _Velocity.x = 1;
        _Velocity.y = 1;
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

        float DeltaHitPosition = P.transform.position.x - HitPosition.x;
        float angleIncision = (DeltaHitPosition / (pc._PlayerStats.scale.x/2)) * _BallStats.maxAngle;

        float SymbolX = P.GetComponent<PlayerController>()._InputAxisValue.x;
        Debug.Log(angleIncision);
        Debug.Log(Mathf.Sin(Mathf.Deg2Rad * angleIncision));
        //_rb.velocity = new Vector2(((SymbolX != 0) ? (Mathf.Sign(SymbolX) * Mathf.Abs(_rb.velocity.x)) : _rb.velocity.x), (-MathF.Sign(P.transform.position.y)) * Mathf.Abs(_rb.velocity.y));
        _rb.velocity = new Vector2(Mathf.Sin(Mathf.Deg2Rad * angleIncision) * -_BallStats.initialSpeed, Mathf.Cos(Mathf.Deg2Rad * angleIncision) * ((-MathF.Sign(P.transform.position.y)) * _BallStats.initialSpeed));
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


        _rb.velocity = new Vector2(-Mathf.Sign(HitPosition.x - transform.position.x)  * Mathf.Abs(_rb.velocity.x), -Mathf.Sign(HitPosition.y - transform.position.y) * Mathf.Abs(_rb.velocity.y));
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
