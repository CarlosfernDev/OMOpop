using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dependencies/BallStats", order = 2)]
public class BallStats : ScriptableObject
{
    public float initialSpeed;
    public float speedIncrease;
    public float maxAngle;
}
