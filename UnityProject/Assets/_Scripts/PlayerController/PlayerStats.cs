using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dependencies/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public float speed;
    public float smoothTime;

    public Vector2 scale;

    public float predictRaycastDistance;
    public LayerMask wallLayer;
}
