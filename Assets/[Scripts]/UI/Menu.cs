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
    public bool isShown => panel.activeInHierarchy;

    public void Show(bool selectDefault = true)
    {
        panel.SetActive(true);
        onMenuShown?.Invoke(this);
        EventSystem.current.SetSelectedGameObject(defaultSelection);
    }

    public void Hide()
    {
        panel.SetActive(false);
        onMenuHidden?.Invoke(this);
    }
}
