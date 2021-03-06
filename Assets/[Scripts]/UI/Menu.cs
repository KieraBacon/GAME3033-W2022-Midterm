using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public delegate void MenuActivationEventHandler(Menu menu);
    public event MenuActivationEventHandler onMenuShown;
    public event MenuActivationEventHandler onMenuHidden;

    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private GameObject defaultSelection;
    [SerializeField]
    private bool pauseWhileOpen;
    public bool isShown => panel.activeInHierarchy;

    private void Start()
    {
        if (panel.activeInHierarchy)
            Show();
    }

    public void Show(bool selectDefault = true)
    {
        panel.SetActive(true);
        onMenuShown?.Invoke(this);
        EventSystem.current.SetSelectedGameObject(defaultSelection);

        if (pauseWhileOpen)
            TimeManager.paused = true;
    }

    public void Hide()
    {
        panel.SetActive(false);
        onMenuHidden?.Invoke(this);

        if (pauseWhileOpen)
            TimeManager.paused = false;
    }
}
