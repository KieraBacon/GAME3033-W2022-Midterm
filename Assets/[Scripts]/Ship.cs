using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public delegate void ShipEventHandler(Ship ship);
    public static event ShipEventHandler onShipAdded;
    public static event ShipEventHandler onShipRemoved;

    [SerializeField]
    private float decelerationConstant;
    [SerializeField]
    private float rotationConstant;
    private Planet currentPlanet;
    [SerializeField]
    private float accelerationForce;
    private Vector2 acceleration;

    int planetLayer;
    private Rigidbody rigidbody;
    private GravityRecipient gravityRecipient;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        gravityRecipient = GetComponent<GravityRecipient>();
        planetLayer = LayerMask.NameToLayer("Planet");
    }

    private void OnEnable()
    {
        onShipAdded?.Invoke(this);
    }

    private void OnDisable()
    {
        onShipRemoved?.Invoke(this);
    }

    private void FixedUpdate()
    {
        if (currentPlanet)
        {
            if (gravityRecipient.enabled == true)
                gravityRecipient.enabled = false;

            // Do the turning and slowing animation
            if (rigidbody.velocity != Vector3.zero)
                rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, Vector3.zero, decelerationConstant);
            if (rigidbody.angularVelocity != Vector3.zero)
                rigidbody.angularVelocity = Vector3.MoveTowards(rigidbody.angularVelocity, Vector3.zero, decelerationConstant);
            if (transform.forward != transform.position - currentPlanet.transform.position)
                rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.position - currentPlanet.transform.position, Vector3.up), rotationConstant));
        }
        else
        {
            if (gravityRecipient.enabled == false)
                gravityRecipient.enabled = true;
        }

        rigidbody.AddForce(acceleration * accelerationForce);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == planetLayer)
        {
            currentPlanet = other.GetComponent<Planet>();
        }
    }

    public void OnSelect()
    {
        Debug.Log(gameObject.name + " selected!");
    }

    public void OnDeselect()
    {
        Debug.Log(gameObject.name + " deselected!");
    }

    public void OnAccelerate(Vector2 value)
    {
        acceleration = value;
    }
}
