using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectorDeMenus : MonoBehaviour
{
    private static SelectorDeMenus instance;
    public static SelectorDeMenus Instance { get { return instance; } }

    [SerializeField] private List<Transform> listaMenus;
    int currentlyActive = -1;
    [SerializeField] private List<Transform> acomodadores;

    public static UnityEvent onCloseMenu = new UnityEvent();

    private void Awake()
    {
        instance = this;
    }

    public void SeleccionarMenu(int idx)
    {
        if (idx < 0 || idx >= listaMenus.Count) return;
        foreach (var m in listaMenus) m.gameObject.SetActive(false);
        listaMenus[idx].gameObject.SetActive(true);
        currentlyActive = idx;
        foreach (var m in acomodadores) m.gameObject.SetActive(true);
    }

    public void CerrarMenus()
    {
        foreach (var m in listaMenus) m.gameObject.SetActive(false);
        currentlyActive = -1;
        foreach (var m in acomodadores) m.gameObject.SetActive(false);
        onCloseMenu?.Invoke();
    }

    public Transform GetMenuSelected()
    {
        if (currentlyActive == -1) return null;
        return listaMenus[currentlyActive];
    }
}
