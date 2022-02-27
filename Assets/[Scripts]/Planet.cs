using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Planet : MonoBehaviour
{
    public delegate void OccupationStateEventHandler(Planet planet, OccupationState occupationState);
    public static OccupationStateEventHandler onOccupationChanged;

    public enum OccupationState
    {
        Unoccupied,
        Occupied,
    }

    [SerializeField]
    private SphereCollider _groundCollider;
    public SphereCollider groundCollider => _groundCollider;
    public float surfaceRadius => groundCollider.radius * groundCollider.transform.lossyScale.x;
    [SerializeField]
    private MeshRenderer _groundMesh;
    [SerializeField]
    private Material unoccupiedMaterial;
    [SerializeField]
    private Material occupiedMaterial;
    [SerializeField]
    private SphereCollider _atmosphereCollider;
    public SphereCollider atmosphereCollider => _atmosphereCollider;
    public float atmosphereRadius => atmosphereCollider.radius * atmosphereCollider.transform.lossyScale.x;

    private OccupationState occupationState = OccupationState.Unoccupied;
    public bool isOccupied => occupationState == OccupationState.Occupied;

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

    [Header("Ships")]
    [SerializeField]
    private int maxShips;
    private LinkedList<Ship> attachedShips = new LinkedList<Ship>();
    public int numShips => attachedShips.Count;
    public bool atCapacity => numShips >= maxShips;
    private Vector3 _velocity;
    [SerializeField]
    private AudioClip occupationSound;

    public Vector3 velocity => _velocity;

    private void OnValidate()
    {
        orbitAngle = orbitStartingAngle;
        SetToOrbitPosition(orbitStartingAngle);
        SetToRotation(rotationStartingAngle);
        SetOccupationState(occupationState);
        ClearNullFromAttachedShips();
        SetShipPositions();
    }

    private void Start()
    {
        _velocity = Vector3.zero;
        orbitAngle = orbitStartingAngle;
        SetToOrbitPosition(orbitStartingAngle);
        SetToRotation(rotationStartingAngle);
        SetOccupationState(occupationState);
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
        Vector3 currentPos = transform.position;
        transform.localPosition = new Vector3(orbitRadius * Mathf.Cos(Mathf.Deg2Rad * angle), 0.0f, orbitRadius * Mathf.Sin(Mathf.Deg2Rad * angle));
        _velocity = (transform.position - currentPos) / Time.fixedDeltaTime;
    }

    private void SetToRotation(float angle)
    {
        transform.rotation = Quaternion.AngleAxis(angle, rotationAxis);
    }

    private void SetOccupationState(OccupationState occupationState)
    {
        this.occupationState = occupationState;
        _groundMesh.material = occupationState == OccupationState.Unoccupied ? unoccupiedMaterial : occupiedMaterial;
        onOccupationChanged?.Invoke(this, occupationState);
    }

    public void AttachShip(Ship ship, bool notify = true)
    {
        ClearNullFromAttachedShips();
        if (!attachedShips.Contains(ship))
        {
            attachedShips.AddLast(ship);
            if (attachedShips.Count > 0)
            {
                SetOccupationState(OccupationState.Occupied);
                if (notify)
                    AudioManager.PlayClip(occupationSound);
            }
        }
    }

    public void DetachShip(Ship ship)
    {
        ClearNullFromAttachedShips();
        attachedShips.Remove(ship);
        // Once a planet is occupied, it stays occupied forever.
        //if (attachedShips.Count <= 0)
        //    SetOccupationState(OccupationState.Unoccupied);
    }

    private void ClearNullFromAttachedShips()
    {
        LinkedListNode<Ship> node = attachedShips.First;
        for (int i = 0; i < attachedShips.Count && node != null; i++)
        {
            if (node.Value == null || node.Value.currentPlanet != this)
            {
                attachedShips.Remove(node);
            }

            if (i > 0)
            {
                node = node.Next;
            }
        }
    }

    public int GetAttachedShipIndex(Ship ship)
    {
        ClearNullFromAttachedShips();

        if (!attachedShips.Contains(ship)) return -1;
        if (ship.currentPlanet != this)
        {
            attachedShips.Remove(ship);
            return -1;
        }

        LinkedListNode<Ship> node = attachedShips.First;
        for (int i = 0; i < attachedShips.Count && node != null && node.Value != null; i++)
        {
            if (node.Value == ship)
                return i;

            if (i > 0)
            {
                if (node.Next.Value == null)
                    attachedShips.Remove(node.Next);

                node = node.Next;
            }
        }

        return -1;
    }

    public void SetShipPositions()
    {
        ClearNullFromAttachedShips();

        float step = 360.0f / numShips;
        LinkedListNode<Ship> node = attachedShips.First;

        for (int i = 0; i < attachedShips.Count && node != null && node.Value != null; i++)
        {
            float planetAttachmentAngle = step * i;
            node.Value.SetLocationRelativeToPlanet(planetAttachmentAngle);

            if (i > 0)
            {
                node = node.Next;
            }
        }
    }
}
