using System;
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
    
    [Header("Orbit")]
    [SerializeField]
    private float orbitRadius;
    [SerializeField]
    private float orbitSpeed;
    [SerializeField]
    private float orbitStartingAngle;
    private float orbitAngle;

    [Header("Rotation")]
    [SerializeField]
    private Vector3 rotationAxis;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float rotationStartingAngle;
    private float rotationAngle;

    private void OnValidate()
    {
        SetToOrbitPosition(orbitStartingAngle);
        SetToRotation(rotationStartingAngle);
    }

    private void Start()
    {
        orbitAngle = orbitStartingAngle;
    }

    private void FixedUpdate()
    {
        orbitAngle += Time.fixedDeltaTime * orbitSpeed;
        SetToOrbitPosition(orbitAngle);

        rotationAngle += Time.fixedDeltaTime * rotationSpeed;
        SetToRotation(rotationAngle);

    }

    private void SetToOrbitPosition(float angle)
    {
        transform.localPosition = new Vector3(orbitRadius * Mathf.Cos(Mathf.Deg2Rad * angle), 0.0f, orbitRadius * Mathf.Sin(Mathf.Deg2Rad * angle));
    }

    private void SetToRotation(float angle)
    {
        transform.rotation = Quaternion.AngleAxis(angle, rotationAxis);
    }
}
