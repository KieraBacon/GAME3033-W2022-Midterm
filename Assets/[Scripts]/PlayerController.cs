using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public event Ship.ShipEventHandler onShipRemoved;
    public event Ship.ShipEventHandler onShipSelected;
    public event Ship.ShipEventHandler onShipDeselected;
    public event Planet.OccupationStateEventHandler onPlanetOccupationChanged;

    private PlayerInput playerInput;
    private InputAction accelerateAction;
    private InputAction selectNextAction;
    private InputAction selectPrevAction;
    private InputAction launchAction;
    private LinkedList<Planet> occupiedPlanets = new LinkedList<Planet>();
    public int numOccupiedPlanets => occupiedPlanets.Count;
    private LinkedList<Ship> controlledShips = new LinkedList<Ship>();
    public int numControlledShips => controlledShips.Count;
    private LinkedListNode<Ship> currentShipNode;
    public Ship currentlySelectedShip {
        get { return currentShipNode?.Value; }
        set {
            LinkedListNode<Ship> found = controlledShips.Find(value);
            if (found != null)
            {
                currentlySelectedShip?.OnDeselect();
                onShipDeselected?.Invoke(currentlySelectedShip);
                currentShipNode = found;
                value.OnSelect();
                onShipSelected?.Invoke(value);
            }
        }
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        accelerateAction = playerInput.currentActionMap.FindAction("Accelerate");
        selectNextAction = playerInput.currentActionMap.FindAction("SelectNext");
        selectPrevAction = playerInput.currentActionMap.FindAction("SelectPrev");
        launchAction = playerInput.currentActionMap.FindAction("Launch");
    }

    private void OnEnable()
    {
        Ship.onShipAdded += OnShipAdded;
        Ship.onShipRemoved += OnShipRemoved;

        accelerateAction.performed += OnAcceleratePerformed;
        selectNextAction.performed += OnSelectNextPerformed;
        selectPrevAction.performed += OnSelectPrevPerformed;
        launchAction.performed += OnLaunch;
        Planet.onOccupationChanged += OnOccupationStateChanged;
    }

    private void OnDisable()
    {
        Ship.onShipAdded -= OnShipAdded;
        Ship.onShipRemoved -= OnShipRemoved;

        accelerateAction.performed -= OnAcceleratePerformed;
        selectNextAction.performed -= OnSelectNextPerformed;
        selectPrevAction.performed -= OnSelectPrevPerformed;
    }

    private void Start()
    {
        // Because we cannot rely on the 'OnEnable of this script happening before the OnEnable of the ships,
        // we need to check to make sure that the controlled ships array is already filled with everything it needs.'
        controlledShips.Clear();
        foreach (Ship ship in FindObjectsOfType<Ship>())
        {
            controlledShips.AddLast(ship);
        }
    }

    private void OnShipAdded(Ship ship)
    {
        if (!ship) return;

        controlledShips.AddLast(ship);
    }

    private void OnShipRemoved(Ship ship)
    {
        if (!ship) return;

        if (currentlySelectedShip && currentlySelectedShip == ship)
            SelectNext();
        controlledShips.Remove(ship);
        onShipRemoved?.Invoke(ship);
    }

    private void OnAcceleratePerformed(InputAction.CallbackContext obj)
    {
        currentlySelectedShip?.OnAccelerate(obj.ReadValue<Vector2>());
    }

    private void OnSelectNextPerformed(InputAction.CallbackContext obj)
    {
        SelectNext();
    }

    private void OnSelectPrevPerformed(InputAction.CallbackContext obj)
    {
        SelectPrev();
    }

    public void Select(Ship ship)
    {
        currentlySelectedShip = ship;
    }

    private void SelectNext()
    {
        currentlySelectedShip?.OnDeselect();
        onShipDeselected?.Invoke(currentlySelectedShip);

        if (!currentlySelectedShip)
            currentShipNode = controlledShips.First;
        else if (currentShipNode == controlledShips.Last)
            currentShipNode = controlledShips.First;
        else
            currentShipNode = currentShipNode.Next;

        currentlySelectedShip.OnSelect();
        onShipSelected?.Invoke(currentlySelectedShip);

    }

    private void SelectPrev()
    {
        currentlySelectedShip?.OnDeselect();
        onShipDeselected?.Invoke(currentlySelectedShip);

        if (!currentlySelectedShip)
            currentShipNode = controlledShips.First;
        else if (currentShipNode == controlledShips.First)
            currentShipNode = controlledShips.Last;
        else
            currentShipNode = currentShipNode.Previous;

        currentlySelectedShip.OnSelect();
        onShipSelected?.Invoke(currentlySelectedShip);
    }

    private void OnLaunch(InputAction.CallbackContext obj)
    {
        currentlySelectedShip?.OnLaunch();
    }

    private void OnOccupationStateChanged(Planet planet, Planet.OccupationState occupationState)
    {
        if (occupationState == Planet.OccupationState.Occupied)
        {
            if (!occupiedPlanets.Contains(planet))
            {
                occupiedPlanets.AddLast(planet);
                onPlanetOccupationChanged?.Invoke(planet, occupationState);
            }
        }
    }
}
