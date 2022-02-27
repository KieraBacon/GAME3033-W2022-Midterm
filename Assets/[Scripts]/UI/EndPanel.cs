using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanel : MonoBehaviour
{
    [SerializeField]
    private GameManager.GameState relevantGameState;
    [SerializeField]
    private GameObject panel;

    private void Start()
    {
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        GameManager.onGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.onGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameManager.GameState gameState)
    {
        if (gameState == relevantGameState)
        {
            panel.SetActive(true);
        }
    }
}
