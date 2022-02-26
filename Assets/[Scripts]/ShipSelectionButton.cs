using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShipSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Ship ship;
    private Button button;
    private PlayerController playerController;
    private Animator animator;
    private int pulseAnimationHash = Animator.StringToHash("Pulse");
    private bool pointerIsOver = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        playerController = FindObjectOfType<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClicked);
        ship.onShipSelected += OnShipSelected;
        ship.onShipDeselected += OnShipDeselected;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClicked);
        ship.onShipSelected -= OnShipSelected;
        ship.onShipDeselected -= OnShipDeselected;
    }

    private void OnButtonClicked()
    {
        playerController.Select(ship);
    }

    private void OnShipSelected(Ship ship)
    {
        animator.SetBool(pulseAnimationHash, true);
    }

    private void OnShipDeselected(Ship ship)
    {
        if (!pointerIsOver)
            animator.SetBool(pulseAnimationHash, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerIsOver = true;
        animator.SetBool(pulseAnimationHash, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerIsOver = false;

        if (playerController.currentlySelectedShip != ship)
            animator.SetBool(pulseAnimationHash, false);
    }
}
