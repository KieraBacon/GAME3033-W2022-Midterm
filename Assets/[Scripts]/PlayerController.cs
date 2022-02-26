using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction accelerateAction;
    private InputAction selectNextAction;
    private InputAction selectPrevAction;
    private LinkedList<Ship> controlledShips = new LinkedList<Ship>();
    private LinkedListNode<Ship> currentShipNode;
    public Ship currentShip {
        get { return currentShipNode?.Value; }
        set {
            LinkedListNode<Ship> found = controlledShips.Find(value);
            if (found != null)
            {
                currentShip?.OnDeselect();
                currentShipNode = found;
                value.OnSelect();
            }
        }
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        accelerateAction = playerInput.currentActionMap.FindAction("Accelerate");
        selectNextAction = playerInput.currentActionMap.FindAction("SelectNext");
        selectPrevAction = playerInput.currentActionMap.FindAction("SelectPrev");
    }

    private void OnEnable()
    {
        Ship.onShipAdded += OnShipAdded;
        Ship.onShipRemoved += OnShipRemoved;

        accelerateAction.performed += OnAcceleratePerformed;
        selectNextAction.performed += OnSelectNextPerformed;
        selectPrevAction.performed += OnSelectPrevPerformed;
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

        if (currentShip && currentShip == ship)
            SelectNext();
        controlledShips.Remove(ship);
    }

    private void OnAcceleratePerformed(InputAction.CallbackContext obj)
    {
        currentShip?.OnAccelerate(obj.ReadValue<Vector2>());
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
        currentShip = ship;
    }

    private void SelectNext()
    {
        currentShip?.OnDeselect();

        if (!currentShip)
            currentShipNode = controlledShips.First;
        else if (currentShipNode == controlledShips.Last)
            currentShipNode = controlledShips.First;
        else
            currentShipNode = currentShipNode.Next;

        currentShip.OnSelect();
    }

    private void SelectPrev()
    {
        currentShip?.OnDeselect();

        if (!currentShip)
            currentShipNode = controlledShips.First;
        else if (currentShipNode == controlledShips.First)
            currentShipNode = controlledShips.Last;
        else
            currentShipNode = currentShipNode.Previous;

        currentShip.OnSelect();
    }
}
