using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void GameStateChangedEventHandler(GameState gameState);
    public static GameStateChangedEventHandler onGameStateChanged;

    public enum GameState
    {
        InGame,
        Loss,
        Victory,
    }

    private GameState _gameState;
    public GameState gameState { get { return _gameState; } set { _gameState = value; onGameStateChanged?.Invoke(value); } }
    private PlayerController playerController;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        playerController.onShipRemoved += OnShipRemoved;
        Planet.onOccupationChanged += OnOccupationChanged;
    }

    private void OnDisable()
    {
        playerController.onShipRemoved -= OnShipRemoved;
        Planet.onOccupationChanged -= OnOccupationChanged;
    }

    private void OnShipRemoved(Ship ship)
    {
        if (playerController.numControlledShips <= 0)
        {
            gameState = GameState.Loss;
        }
    }

    private void OnOccupationChanged(Planet planet, Planet.OccupationState occupationState)
    {
        bool won = true;
        foreach (Planet p in FindObjectsOfType<Planet>())
        {
            if (!p.isOccupied)
            {
                won = false;
                break;
            }
        }
        if (won)
        {
            gameState = GameState.Victory;
        }
    }
}
