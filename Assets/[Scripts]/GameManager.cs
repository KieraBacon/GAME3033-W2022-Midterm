using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private AudioClip gameMusic;

    private GameState _gameState;
    public GameState gameState { get { return _gameState; } set { _gameState = value; onGameStateChanged?.Invoke(value); } }

    private void Awake()
    {
        TimeManager.StartNewLevel();
        TimeManager.timeScale = 1;
        TimeManager.paused = false;
        AudioManager.PlayMusic(gameMusic);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Ship.onShipRemoved += OnShipRemoved;
        Planet.onOccupationChanged += OnOccupationChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Ship.onShipRemoved -= OnShipRemoved;
        Planet.onOccupationChanged -= OnOccupationChanged;
    }

    private void OnShipRemoved(Ship ship)
    {
        foreach (PlayerController playerController in FindObjectsOfType<PlayerController>())
        {
            if (playerController.numControlledShips <= 0)
            {
                gameState = GameState.Loss;
            }
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

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        TimeManager.StartNewLevel();
    }
}
