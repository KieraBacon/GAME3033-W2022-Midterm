using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSphere : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            Ship ship = other.gameObject.GetComponentInParent<Ship>();
            if (ship)
            {
                ship.enabled = false;
                Destroy(ship.gameObject);
            }
        }
    }
}
