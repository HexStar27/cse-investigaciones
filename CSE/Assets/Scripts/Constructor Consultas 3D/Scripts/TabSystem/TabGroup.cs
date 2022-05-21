using System.Collections.Generic;
using UnityEngine;

namespace Hexstar
{
    public class TabGroup : MonoBehaviour
    {
        public List<TabButton> tabButtons;
        public Sprite tabIdle;
        public Sprite tabHover;
        public Sprite tabActive;
        public Sprite tabDisabled;
        [HideInInspector] public TabButton selectedTab;

        public List<GameObject> objectsToSwap;

        public void Suscribe(TabButton button)
        {
            if (tabButtons == null)
            {
                tabButtons = new List<TabButton>();
            }
            tabButtons.Add(button);
        }

        public void OnTabEnter(TabButton button)
        {
            ResetTabs();
            if (selectedTab == null || button != selectedTab)
                button.background.sprite = tabHover;
        }

        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }

        public void OnTabSelected(TabButton button)
        {
            if (selectedTab != null) selectedTab.Deselect();

            selectedTab = button;
            int nobjects = objectsToSwap.Count;
            if (selectedTab == null)
            {
                for (int i = 0; i < nobjects; i++)
                {
                    objectsToSwap[i].SetActive(false);
                }
                ResetTabs();
            }
            else
            {
                selectedTab.Selected();
                ResetTabs();
                button.background.sprite = tabActive;

                int index = button.transform.GetSiblingIndex();
                for (int i = 0; i < nobjects; i++)
                {
                    objectsToSwap[i].SetActive(i == index);
                }
            }
        }

        public void ResetTabs()
        {
            foreach (var button in tabButtons)
            {
                if ((selectedTab == null || button != selectedTab) && button.Desbloqueado())
                    button.background.sprite = tabIdle;
            }
        }

        public void Bloquear(int botonIndice)
        {
            if (botonIndice >= 0 && botonIndice < tabButtons.Count)
            {
                tabButtons[botonIndice].Bloquear(true);
                tabButtons[botonIndice].background.sprite = tabDisabled;
            }
        }

        public void ResetBloqueos()
        {
            foreach (var button in tabButtons)
            {
                if (button != null)
                {
                    button.Bloquear(false);
                    if (selectedTab != null && button == selectedTab)
                        button.background.sprite = tabActive;
                    else
                        button.background.sprite = tabIdle;
                }
            }
        }
    }
}