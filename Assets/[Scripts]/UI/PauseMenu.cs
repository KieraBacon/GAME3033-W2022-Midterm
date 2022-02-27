using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Menu))]
public class PauseMenu : MonoBehaviour
{
    private Menu menu;

    private void Awake()
    {
        menu = GetComponent<Menu>();
    }

    private void OnEnable()
    {
        GameManager.onPause += OnPause;
    }

    private void OnDisable()
    {
        GameManager.onPause -= OnPause;
    }

    private void OnPause()
    {
        if (menu.isShown)
            menu.Hide();
        else
            menu.Show();
    }
}
