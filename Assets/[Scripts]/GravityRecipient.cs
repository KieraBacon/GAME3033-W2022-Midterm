using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityRecipient : MonoBehaviour
{
    [SerializeField]
    private float rotationalForce;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        foreach (GravitySource gravitySource in GravitySource.allGravitySources)
        {
            // Force
            Vector3 direction = (gravitySource.transform.position - transform.position).normalized;
            float distance = Mathf.Max(Vector3.Distance(gravitySource.transform.position, transform.position), 0.1f);
            float strengthOverDistance = gravitySource.strength / distance;
            rigidbody.AddForce(direction * strengthOverDistance);

            if (rigidbody.velocity != Vector3.zero)
                rigidbody.MoveRotation(Quaternion.LookRotation(rigidbody.velocity, Vector3.up));
            //// Torque
            //float angleDiff = Vector3.Angle(transform.forward, rb.velocity);
            //Vector3 cross = Vector3.Cross(transform.forward, rb.velocity);
            //rb.AddTorque(cross * Mathf.Sign(cross.z) * angleDiff * rotationalForce * strengthOverDistance);
        }
    }
}
