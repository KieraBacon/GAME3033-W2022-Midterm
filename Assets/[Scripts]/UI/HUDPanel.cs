using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HUDPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI shipsText;
    [SerializeField]
    private TextMeshProUGUI timeText;
    [SerializeField]
    private TextMeshProUGUI planetsText;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        SetShipsText(playerController.numControlledShips);
        SetTimeText(TimeManager.gameTime);
        SetPlanetsText(playerController.numOccupiedPlanets, FindObjectsOfType<Planet>().Length);
    }

    private void OnEnable()
    {
        playerController.onShipAdded += OnShipQtyChanged;
        playerController.onShipRemoved += OnShipQtyChanged;
        playerController.onPlanetOccupationChanged += OnPlanetOccupationChanged;
    }

    private void OnDisable()
    {
        playerController.onShipAdded += OnShipQtyChanged;
        playerController.onShipRemoved += OnShipQtyChanged;
        playerController.onPlanetOccupationChanged += OnPlanetOccupationChanged;
    }

    private void Update()
    {
        SetTimeText(TimeManager.gameTime);
    }

    private void SetShipsText(int ships)
    {
        shipsText.text = ships.ToString();
    }

    private void SetTimeText(float time)
    {
        timeText.text = TimeSpan.FromSeconds(time).ToString(@"d\d\ hh\hmm\mss").TrimStart(' ', 'd', 'h', 'm', 's', '0');
        for (int i = 0; i < 2; i++)
            if (timeText.text.Length < 2)
                timeText.text = "0" + timeText.text;
    }

    private void SetPlanetsText(int current, int total)
    {
        planetsText.text = current + "/" + total;
    }

    private void OnShipQtyChanged(Ship ship)
    {
        SetShipsText(playerController.numControlledShips);
    }

    private void OnPlanetOccupationChanged(Planet planet, Planet.OccupationState occupationState)
    {
        SetPlanetsText(playerController.numOccupiedPlanets, FindObjectsOfType<Planet>().Length);
    }

}
