using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShipSelectionButton : MonoBehaviour
{
    [SerializeField]
    private Ship ship;
    private Button button;
    private PlayerController playerController;

    private void Awake()
    {
        button = GetComponent<Button>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        playerController.Select(ship);
    }
}
