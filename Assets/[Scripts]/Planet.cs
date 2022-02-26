using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField]
    private SphereCollider _groundCollider;
    public SphereCollider groundCollider => _groundCollider;
    [SerializeField]
    private SphereCollider _atmosphereCollider;
    public SphereCollider atmosphereCollider => _atmosphereCollider;

}
