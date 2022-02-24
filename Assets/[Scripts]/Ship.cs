using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField]
    private float decelerationConstant;
    [SerializeField]
    private float rotationConstant;
    [SerializeField]
    private Planet currentPlanet;

    int planetLayer;
    private Rigidbody rigidbody;
    private GravityRecipient gravityRecipient;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        gravityRecipient = GetComponent<GravityRecipient>();
        planetLayer = LayerMask.NameToLayer("Planet");
    }

    private void FixedUpdate()
    {
        if (currentPlanet)
        {
            if (rigidbody.velocity != Vector3.zero)
                rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, Vector3.zero, decelerationConstant);
            if (rigidbody.angularVelocity != Vector3.zero)
                rigidbody.angularVelocity = Vector3.MoveTowards(rigidbody.angularVelocity, Vector3.zero, decelerationConstant);
            if (transform.forward != transform.position - currentPlanet.transform.position)
                rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.position - currentPlanet.transform.position, Vector3.up), rotationConstant));

            //rigidbody.MoveRotation(Quaternion.FromToRotation(transform.forward, Vector3.RotateTowards(transform.forward, transform.position - currentPlanet.transform.position, decelerationConstant, decelerationConstant)));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == planetLayer)
        {
            gravityRecipient.enabled = false;
            currentPlanet = other.GetComponent<Planet>();
            
                
            //rigidbody.velocity = Vector3.zero;
            //rigidbody.angularVelocity = Vector3.zero;
            //
            //
            //Debug.DrawRay(transform.position, transform.position - other.transform.position, Color.green, 20);
            //
            //Debug.Log("0" + transform.rotation);
            //rigidbody.MoveRotation(Quaternion.FromToRotation(transform.forward, transform.position - other.transform.position));
            //Debug.Log("1" + transform.rotation);
            //transform.LookAt(transform.position - other.transform.position);
            //Debug.Log("2" + transform.rotation);
        }
    }
}
