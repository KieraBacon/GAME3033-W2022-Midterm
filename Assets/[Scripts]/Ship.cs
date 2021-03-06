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
    [SerializeField]
    private Planet _currentPlanet;
    public Planet currentPlanet => _currentPlanet;
    [SerializeField]
    private float accelerationForce;
    private Vector2 acceleration;
    [SerializeField]
    private Transform planetAttachmentPoint;
    private Vector3 planetEntryPosition;
    private Quaternion planetEntryRotation;
    private float planetEntryTime = float.NegativeInfinity;
    private float planetAttachmentAngle;
    private float planetAttachmentRadius;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Collider collider;
    [SerializeField, Min(0.01f)]
    private float landingDuration = 2.0f;
    [SerializeField]
    private float inPlanetRotationSpeed;
    [SerializeField, Min(0.01f)]
    private float launchDuration = 2.0f;
    private float launchTime = float.NegativeInfinity;
    [SerializeField]
    private float launchSpeed;
    private Vector3 launchPosition;
    [SerializeField]
    private AudioClip selectionSound;
    [SerializeField]
    private AudioClip thrusterSound;
    [SerializeField]
    private float fuelExpenditure;
    private float _fuel;
    public float fuel => _fuel;
    [SerializeField]
    private Bar fuelBar;

    int planetLayer;
    private Rigidbody rigidbody;
    private GravityRecipient gravityRecipient;
    private int landingAnimationHash = Animator.StringToHash("Landing");
    private int launchAnimationHash = Animator.StringToHash("Launch");
    private int forceAnimationHash = Animator.StringToHash("Force");
    private int headingAnimationHash = Animator.StringToHash("Heading");

    private void OnValidate()
    {
        SetupInitialPositioning();
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        gravityRecipient = GetComponent<GravityRecipient>();
        planetLayer = LayerMask.NameToLayer("Planet");
        //planetAttachmentPointDistance = Vector3.Distance(transform.position, planetAttachmentPoint.position);
    }

    private void Start()
    {
        SetupInitialPositioning();
        _fuel = 1;
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
        if (_currentPlanet)
        {
            if (fuelBar.gameObject.activeInHierarchy)
                fuelBar.gameObject.SetActive(false);
            if (gravityRecipient.enabled == true)
                gravityRecipient.enabled = false;

            if (planetEntryTime > 0)
            {
                float entryTime = Time.time - planetEntryTime;
                if (entryTime < landingDuration)
                {
                    if (planetEntryPosition == Vector3.zero)
                        planetEntryPosition = transform.position;

                    rigidbody.velocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                    rigidbody.MovePosition(Vector3.Lerp(planetEntryPosition, _currentPlanet.groundCollider.ClosestPoint(transform.position) + (transform.position - planetAttachmentPoint.position), entryTime / landingDuration));
                    Quaternion slerp = Quaternion.Slerp(planetEntryRotation, Quaternion.LookRotation(transform.position - _currentPlanet.transform.position), Mathf.SmoothStep(0, 1, entryTime / landingDuration));
                    rigidbody.MoveRotation(slerp);
                }
                else
                {
                    planetEntryPosition = Vector3.zero;
                    planetEntryTime = float.NegativeInfinity;
                    planetAttachmentAngle = Vector3.SignedAngle(Vector3.right, transform.position - _currentPlanet.transform.position, Vector3.up);
                    planetAttachmentRadius = Vector3.Distance(transform.position, _currentPlanet.transform.position);
                    _fuel = 1;
                }
            }
            else if (launchTime > 0)
            {
                float exitTime = Time.time - launchTime;

                if (exitTime < launchDuration)
                {
                    if (launchPosition == Vector3.zero)
                        launchPosition = transform.position - _currentPlanet.transform.position;

                    rigidbody.MovePosition(Vector3.Lerp(_currentPlanet.transform.position + launchPosition, _currentPlanet.transform.position + (launchPosition * 3), exitTime / launchDuration));
                    rigidbody.MoveRotation(Quaternion.LookRotation(transform.position - _currentPlanet.transform.position));
                }
                else
                {
                    launchPosition = Vector3.zero;
                    launchTime = float.NegativeInfinity;
                    rigidbody.velocity = transform.forward * launchSpeed + _currentPlanet.velocity;
                    _currentPlanet.DetachShip(this);
                    _currentPlanet = null;
                    fuelBar.gameObject.SetActive(true);
                }
            }
            else
            {
                SetLocationRelativeToPlanet();
            }
        }
        else
        {
            if (gravityRecipient.enabled == false)
                gravityRecipient.enabled = true;


            if (acceleration != Vector2.zero && _fuel > 0)
            {
                Vector3 force = (transform.right * acceleration.x + transform.forward * acceleration.y) * accelerationForce;
                rigidbody.AddForce(force);
                _fuel -= fuelExpenditure * Time.fixedDeltaTime;
                fuelBar.amount = _fuel;
            }

            if (rigidbody.velocity != Vector3.zero)
                rigidbody.MoveRotation(Quaternion.LookRotation(rigidbody.velocity, Vector3.up));
        }


        float fuelMult = _fuel > 0 ? 1 : 0;
        animator.SetFloat(forceAnimationHash, acceleration.y * fuelMult);
        animator.SetFloat(headingAnimationHash, acceleration.x * fuelMult);
    }

    private void SetupInitialPositioning()
    {
        if (_currentPlanet)
        {
            _currentPlanet.AttachShip(this, false);
            _currentPlanet.SetShipPositions();
        }
    }

    public void SetLocationRelativeToPlanet(float overrideAngle)
    {
        if (!_currentPlanet) return;

        planetAttachmentAngle = overrideAngle;
        float attachmentDistance = Vector3.Distance(transform.position, planetAttachmentPoint.position);
        transform.position = _currentPlanet.transform.position + Vector3.right * (_currentPlanet.surfaceRadius + attachmentDistance);
        transform.RotateAround(_currentPlanet.transform.position, Vector3.up, planetAttachmentAngle);
        transform.rotation = Quaternion.LookRotation(transform.position - _currentPlanet.transform.position);
    }

    public void SetLocationRelativeToPlanet()
    {
        if (!_currentPlanet) return;

        planetAttachmentAngle += acceleration.x * inPlanetRotationSpeed * Time.fixedDeltaTime;
        SetLocationRelativeToPlanet(planetAttachmentAngle);
    }

    internal void OnLaunch()
    {
        if (!currentPlanet) return;

        SetLocationRelativeToPlanet();
        animator.SetTrigger(launchAnimationHash);
        AudioManager.PlayClip(thrusterSound);
        launchTime = Time.time;

        Debug.Log("Launch at " + launchTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_currentPlanet && other.gameObject.layer == planetLayer)
        {
            _currentPlanet = other.GetComponentInParent<Planet>();
            if (!_currentPlanet) return;

            Debug.Log("_currentPlanet.isOccupied: " + _currentPlanet.isOccupied);
            if (_currentPlanet.isOccupied)
            {
                enabled = false;
                Destroy(gameObject);
            }
            else
            {
                planetEntryRotation = transform.rotation;
                rigidbody.angularVelocity = Vector3.zero;
                planetEntryTime = Time.time;
                animator.SetTrigger(landingAnimationHash);
                AudioManager.PlayClip(thrusterSound);
                gravityRecipient.enabled = false;
                fuelBar.gameObject.SetActive(false);
                _currentPlanet.AttachShip(this);
            }
        }
    }

    public void OnSelect()
    {
        Debug.Log(gameObject?.name + " selected!");
        onShipSelected?.Invoke(this);
        AudioManager.PlayClip(selectionSound);
    }

    public void OnDeselect()
    {
        Debug.Log(gameObject?.name + " deselected!");
        onShipDeselected?.Invoke(this);
    }

    public void OnAccelerate(Vector2 value)
    {
        acceleration = value;
    }
}
