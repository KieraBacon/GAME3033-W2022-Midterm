using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSwitcher : MonoBehaviour
{
    [System.Serializable]
    public struct SubmenuButtonPair
    {
        public Button button;
        public Menu menu;
        public GameObject selection;
    }
    [SerializeField]
    private SubmenuButtonPair[] submenuButtonPairs;

    private LinkedList<Menu> submenus = new LinkedList<Menu>();
    private bool activeSwitching = false;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Menu submenu = transform.GetChild(i).GetComponent<Menu>();
            if (submenu)
                submenus.AddLast(submenu);
        }
    }

    private void OnEnable()
    {
        foreach (SubmenuButtonPair pair in submenuButtonPairs)
        {
            pair.button.onClick.AddListener(() => Switch(pair.menu, pair.selection));
        }

        foreach (Menu submenu in submenus)
        {
            submenu.onMenuShown += OnSubmenuShown;
            submenu.onMenuHidden += OnSubmenuHidden;
        }
    }

    private void OnDisable()
    {
        foreach (SubmenuButtonPair pair in submenuButtonPairs)
        {
            pair.button.onClick.RemoveListener(() => Switch(pair.menu, pair.selection));
        }


        foreach (Menu submenu in submenus)
        {
            submenu.onMenuShown -= OnSubmenuShown;
            submenu.onMenuHidden -= OnSubmenuHidden;
        }
    }

    public void Switch(Menu menu, GameObject selectable = null)
    {
        activeSwitching = true;

        foreach (Menu submenu in submenus)
        {
            if (submenu.isShown && submenu != menu)
            {
                submenu.Hide();
            }
            else if (!submenu.isShown && submenu == menu)
            {
                if (selectable)
                {
                    submenu.Show(false);
                    EventSystem.current.SetSelectedGameObject(selectable.gameObject);
                }
                else
                {
                    submenu.Show(true);
                }
            }
        }

        activeSwitching = false;
    }

    private void OnSubmenuShown(Menu menu)
    {
        if (activeSwitching) return;

        Switch(menu);
    }

    private void OnSubmenuHidden(Menu menu)
    {
        if (activeSwitching) return;

        Switch(submenus.First.Value);
    }
}
