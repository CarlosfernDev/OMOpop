using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public PlayerStats _PlayerStats;
    [SerializeField] private BallController _BallController;

    [Header("Inputs")]
    [HideInInspector] public Vector2 _InputAxisValue;
    private float _Velocity;


    [Header("Momentum bola")]
    private float _LastDirection = 1;
    private float DeltaVelocity;
    private float _TargetPosition;

    void Start()
    {
        _TargetPosition = transform.position.x;
    }

    private void FixedUpdate()
    {
        MovePrediction();
    }

    #region Move Functions

    private void MovePrediction()
    {
        DeltaVelocity = (_PlayerStats.speed) * Time.deltaTime;
        if (!PredictionRaycast())
        {
            _TargetPosition = _TargetPosition + (DeltaVelocity * _InputAxisValue.x);
            Move();
        }
        else
        {
            //SimpleMove();
        }
    }

    void SimpleMove()
    {
        transform.position = new Vector2(Mathf.SmoothDamp(transform.position.x, _TargetPosition, ref _Velocity, 0.005f), transform.position.y);
    }

    bool PredictionRaycast()
    {
        RaycastHit2D Hit = Physics2D.Raycast(transform.position, Vector2.right * _LastDirection, _PlayerStats.predictRaycastDistance + (DeltaVelocity), _PlayerStats.wallLayer);

        if (!Hit)
            return false;

        _TargetPosition = Hit.point.x - (_LastDirection * _PlayerStats.scale.x / 2);
        return true;
    }

    void Move()
    {
        transform.position = new Vector2(Mathf.SmoothDamp(transform.position.x, _TargetPosition, ref _Velocity, _PlayerStats.smoothTime), transform.position.y);
    }

    #endregion

    #region Inputs
    public void InputMove(InputAction.CallbackContext ctx)
    {

        // Agarramos el contexto y lo transformamos en un vector 2 y lo pasamos a un valor
        _InputAxisValue = ctx.ReadValue<Vector2>();
        if (_InputAxisValue.x != 0)
        {
            _LastDirection = Mathf.Sign(_InputAxisValue.x) * (Mathf.Abs(_InputAxisValue.x) + Mathf.Abs(_InputAxisValue.y) / 2);
        }
    }

    // Referencia para inputs de un boton
    public void InputLaunch(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            return;
        }

        _BallController.Launch();
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + Vector2.right * (_PlayerStats.predictRaycastDistance + DeltaVelocity));
    }
}
