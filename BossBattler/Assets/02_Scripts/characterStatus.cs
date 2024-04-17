using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterStatus : MonoBehaviour
{
    private float health;
    private float power;
    private float dashTime;

    public float Health { get { return health; } set {  health = value; } }
    public float Power { get { return power; } set { power = value; } }
    public float DashTime { get {  return dashTime; } set {  dashTime = value; } }

}
