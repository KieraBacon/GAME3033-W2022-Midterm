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
    [SerializeField]
    private Transform planetAttachmentPoint;
    private float planetAttachmentPointDistance;
    private Vector3 planetEntryVelocity;
    private Vector3 planetEntryPosition;
    private Quaternion planetEntryRotation;
    private Quaternion planetEntryTargetRotation;
    private float planetEntryTime;

    int planetLayer;
    private Rigidbody rigidbody;
    private GravityRecipient gravityRecipient;
    [SerializeField]
    private Collider collider;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        gravityRecipient = GetComponent<GravityRecipient>();
        planetLayer = LayerMask.NameToLayer("Planet");
        planetAttachmentPointDistance = Vector3.Distance(transform.position, planetAttachmentPoint.position);
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
            float distanceFromPlanet = Vector3.Distance(transform.position, currentPlanet.transform.position) - planetAttachmentPointDistance;
            float radiusOfPlanet = currentPlanet.groundCollider.radius * currentPlanet.groundCollider.transform.lossyScale.x;
            float radiusOfAtmosphere = currentPlanet.atmosphereCollider.radius * currentPlanet.atmosphereCollider.transform.lossyScale.x;
            float normalizedDistanceToGround = 1 - Mathf.Clamp((distanceFromPlanet - radiusOfPlanet) / radiusOfAtmosphere, 0, 1);
            float time = Time.time - planetEntryTime;

            if (time < 1)
            {
                // Lerp the velocity down to zero
                rigidbody.velocity = Vector3.Lerp(planetEntryVelocity, Vector3.zero, Mathf.Max(time, normalizedDistanceToGround));

                // Rotate to face away from the ground
                Quaternion slerp = Quaternion.Slerp(planetEntryRotation, Quaternion.LookRotation(transform.position - currentPlanet.transform.position), Mathf.SmoothStep(0, 1, time));
                rigidbody.MoveRotation(slerp);
            }
            else if (time < 2)
            {
                if (planetEntryPosition == Vector3.zero)
                    planetEntryPosition = transform.position;

                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.MoveRotation(Quaternion.LookRotation(transform.position - currentPlanet.transform.position));
                rigidbody.MovePosition(Vector3.Slerp(planetEntryPosition, currentPlanet.groundCollider.ClosestPoint(transform.position) + (transform.position - planetAttachmentPoint.position), Mathf.SmoothStep(0, 1, time - 1)));
            }
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
        if (!currentPlanet && other.gameObject.layer == planetLayer)
        {
            currentPlanet = other.GetComponentInParent<Planet>();
            planetEntryVelocity = rigidbody.velocity;
            planetEntryRotation = transform.rotation;
            planetEntryTargetRotation = Quaternion.LookRotation(transform.position - currentPlanet.transform.position);
            rigidbody.angularVelocity = Vector3.zero;
            planetEntryTime = Time.time;
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
