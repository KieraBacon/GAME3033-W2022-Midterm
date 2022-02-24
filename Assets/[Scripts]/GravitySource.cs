using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    public static LinkedList<GravitySource> allGravitySources = new LinkedList<GravitySource>();
    [SerializeField]
    private float _strength;
    public float strength => _strength;

    private void OnEnable()
    {
        allGravitySources.AddLast(this);
    }

    private void OnDisable()
    {
        allGravitySources.Remove(this);
    }
}
