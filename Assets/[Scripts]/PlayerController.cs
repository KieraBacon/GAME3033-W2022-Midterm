using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public event Ship.ShipEventHandler onShipAdded;
    public event Ship.ShipEventHandler onShipRemoved;
    public event Ship.ShipEventHandler onShipSelected;
    public event Ship.ShipEventHandler onShipDeselected;
    public event Planet.OccupationStateEventHandler onPlanetOccupationChanged;

    private PlayerInput playerInput;
    private CameraController cameraController;
    private InputAction panCameraAction;
    private InputAction overlookCameraAction;
    private InputAction accelerateAction;
    private InputAction selectNextAction;
    private InputAction selectPrevAction;
    private InputAction launchAction;
    private InputAction pauseAction;
    private LinkedList<Planet> occupiedPlanets = new LinkedList<Planet>();
    public int numOccupiedPlanets => occupiedPlanets.Count;
    private LinkedList<Ship> controlledShips = new LinkedList<Ship>();
    public int numControlledShips => controlledShips.Count;
    private LinkedListNode<Ship> currentShipNode;
    private static bool isQuitting = false;
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
        cameraController = FindObjectOfType<CameraController>();
        panCameraAction = playerInput.currentActionMap.FindAction("PanCamera");
        overlookCameraAction = playerInput.currentActionMap.FindAction("OverlookCamera");
        accelerateAction = playerInput.currentActionMap.FindAction("Accelerate");
        selectNextAction = playerInput.currentActionMap.FindAction("SelectNext");
        selectPrevAction = playerInput.currentActionMap.FindAction("SelectPrev");
        launchAction = playerInput.currentActionMap.FindAction("Launch");
        pauseAction = playerInput.currentActionMap.FindAction("Pause");
    }

    private void OnEnable()
    {
        Ship.onShipAdded += OnShipAdded;
        Ship.onShipRemoved += OnShipRemoved;

        panCameraAction.performed += OnPanCameraPerformed;
        overlookCameraAction.performed += OnOverlookCameraPerformed;
        overlookCameraAction.canceled += OnOverlookCameraCancelled;
        accelerateAction.performed += OnAcceleratePerformed;
        selectNextAction.performed += OnSelectNextPerformed;
        selectPrevAction.performed += OnSelectPrevPerformed;
        launchAction.performed += OnLaunch;
        pauseAction.performed += OnPause;
        Planet.onOccupationChanged += OnOccupationStateChanged;
    }

    private void OnDisable()
    {
        Ship.onShipAdded -= OnShipAdded;
        Ship.onShipRemoved -= OnShipRemoved;

        panCameraAction.performed -= OnPanCameraPerformed;
        overlookCameraAction.performed -= OnOverlookCameraPerformed;
        overlookCameraAction.canceled -= OnOverlookCameraCancelled;
        accelerateAction.performed -= OnAcceleratePerformed;
        selectNextAction.performed -= OnSelectNextPerformed;
        selectPrevAction.performed -= OnSelectPrevPerformed;
        launchAction.performed -= OnLaunch;
        pauseAction.performed -= OnPause;
    }

    private void Start()
    {
        // Because we cannot rely on the 'OnEnable of this script happening before the OnEnable of the ships,
        // we need to check to make sure that the controlled ships array is already filled with everything it needs.'
        controlledShips.Clear();
        foreach (Ship ship in FindObjectsOfType<Ship>())
        {
            OnShipAdded(ship);
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnShipAdded(Ship ship)
    {
        if (!ship || isQuitting) return;

        controlledShips.AddLast(ship);
        onShipAdded?.Invoke(ship);
    }

    private void OnShipRemoved(Ship ship)
    {
        if (!ship || isQuitting) return;

        if (currentlySelectedShip && currentlySelectedShip == ship)
            SelectNext();
        controlledShips.Remove(ship);
        onShipRemoved?.Invoke(ship);
    }

    private void OnPanCameraPerformed(InputAction.CallbackContext obj)
    {
        cameraController.OnPanCameraPerformed(obj.ReadValue<Vector2>());
    }

    private void OnOverlookCameraPerformed(InputAction.CallbackContext obj)
    {
        cameraController.OnOverlookCameraSet(true);
    }

    private void OnOverlookCameraCancelled(InputAction.CallbackContext obj)
    {
        cameraController.OnOverlookCameraSet(false);
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

    private void FocusCamera()
    {
        if (!cameraController.CanSeeGameObject(currentlySelectedShip.gameObject))
        {
            cameraController.FocusGameObject(currentlySelectedShip.gameObject);
        }
    }

    public void Select(Ship ship)
    {
        currentlySelectedShip?.OnDeselect();
        onShipDeselected?.Invoke(currentlySelectedShip);

        currentlySelectedShip = ship;

        currentlySelectedShip.OnSelect();
        onShipSelected?.Invoke(currentlySelectedShip);

        FocusCamera();
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

        FocusCamera();
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

        FocusCamera();
    }

    private void OnLaunch(InputAction.CallbackContext obj)
    {
        currentlySelectedShip?.OnLaunch();
    }

    private void OnPause(InputAction.CallbackContext obj)
    {
        GameManager.Pause();
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
