using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public delegate void ShipEventHandler(Ship ship);
    public static event ShipEventHandler onShipAdded;
    public static event ShipEventHandler onShipRemoved;
    public event ShipEventHandler onShipSelected;
    public event ShipEventHandler onShipDeselected;

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
    private float planetAttachmentAngle;
    private float planetAttachmentRadius;
    [SerializeField]
    private Animator[] trailAnimators;
    [SerializeField]
    private Collider collider;
    [SerializeField, Min(0.01f)]
    private float landingTime = 2.0f;
    [SerializeField]
    private float inPlanetRotationSpeed;
    private bool attachedToPlanet;

    int planetLayer;
    private Rigidbody rigidbody;
    private GravityRecipient gravityRecipient;
    private int forceAnimationHash = Animator.StringToHash("Force");
    private int landingAnimationHash = Animator.StringToHash("Landing");
    private int takeOffAnimationHash = Animator.StringToHash("TakeOff");

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

            if (!attachedToPlanet)
            {
                // Do the turning and slowing animation
                float time = Time.time - planetEntryTime;

                if (time < landingTime)
                {
                    if (planetEntryPosition == Vector3.zero)
                        planetEntryPosition = transform.position;

                    rigidbody.velocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                    rigidbody.MovePosition(Vector3.Lerp(planetEntryPosition, currentPlanet.groundCollider.ClosestPoint(transform.position) + (transform.position - planetAttachmentPoint.position), time / landingTime));
                    rigidbody.MoveRotation(Quaternion.LookRotation(transform.position - currentPlanet.transform.position));
                }
                else
                {
                    attachedToPlanet = true;
                    Vector3 local = transform.position - currentPlanet.transform.position;
                    //planetAttachmentAngle = Mathf.Atan2(local.x, local.z);
                    planetAttachmentAngle = Vector3.SignedAngle(Vector3.right, transform.position - currentPlanet.transform.position, Vector3.up);
                    planetAttachmentRadius = Vector3.Distance(transform.position, currentPlanet.transform.position);
                }
            }
            else
            {
                planetAttachmentAngle += acceleration.x * inPlanetRotationSpeed * Time.fixedDeltaTime;
                //transform.position = currentPlanet.transform.position + new Vector3(planetAttachmentRadius * Mathf.Cos(Mathf.Deg2Rad * planetAttachmentAngle), 0.0f, planetAttachmentRadius * Mathf.Sin(Mathf.Deg2Rad * planetAttachmentAngle));
                //transform.rotation = Quaternion.LookRotation(transform.position - currentPlanet.transform.position);
                //rigidbody.MovePosition(currentPlanet.transform.position + new Vector3(planetAttachmentRadius * Mathf.Cos(Mathf.Deg2Rad * planetAttachmentAngle), 0.0f, planetAttachmentRadius * Mathf.Sin(Mathf.Deg2Rad * planetAttachmentAngle)));
                transform.position = currentPlanet.transform.position + Vector3.right * planetAttachmentRadius;
                transform.RotateAround(currentPlanet.transform.position, Vector3.up, planetAttachmentAngle);
                transform.rotation = Quaternion.LookRotation(transform.position - currentPlanet.transform.position);
                //rigidbody.MoveRotation(Quaternion.LookRotation(transform.position - currentPlanet.transform.position));
            }
        }
        else
        {
            if (gravityRecipient.enabled == false)
                gravityRecipient.enabled = true;

            Vector3 force = (transform.right * acceleration.x + transform.forward * acceleration.y) * accelerationForce;
            rigidbody.AddForce(force);

            if (rigidbody.velocity != Vector3.zero)
                rigidbody.MoveRotation(Quaternion.LookRotation(rigidbody.velocity, Vector3.up));
        }

        foreach (Animator animator in trailAnimators)
        {
            animator.SetFloat(forceAnimationHash, acceleration.magnitude);
        }
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

            foreach (Animator animator in trailAnimators)
            {
                animator.SetTrigger(landingAnimationHash);
            }
        }
    }

    public void OnSelect()
    {
        Debug.Log(gameObject.name + " selected!");
        onShipSelected?.Invoke(this);
    }

    public void OnDeselect()
    {
        Debug.Log(gameObject.name + " deselected!");
        onShipDeselected?.Invoke(this);
    }

    public void OnAccelerate(Vector2 value)
    {
        acceleration = value;
    }
}
