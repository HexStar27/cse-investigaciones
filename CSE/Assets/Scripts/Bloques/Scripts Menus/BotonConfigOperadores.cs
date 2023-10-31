using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Código para el botón de configuración que llevará el bloque operador
public class BotonConfigOperadores : MonoBehaviour
{
    [SerializeField] GameObject opcionMenuPrefab;
    [SerializeField] TMPro.TextMeshPro textoDelBloque;
    [SerializeField] List<string> operadores;

    private static readonly int menuIndex = 0;
    List<GameObject> elementos = new List<GameObject>();

    private Hexstar.CSE.BlockMovAndConexion bloque;
    public class SelectionEvent : UnityEvent<string> { }
    [HideInInspector] public SelectionEvent onSelectOP = new SelectionEvent();

    public void Abrir()
    {
        SelectorDeMenus.Instance.SeleccionarMenu(menuIndex);
        var parent = SelectorDeMenus.Instance.GetMenuSelected();
        foreach (string op in operadores)
        {
            var obj = Instantiate(opcionMenuPrefab, parent);
            var elem = obj.GetComponent<BotonElementoMenuOP>();
            elem.SetTXT(op);
            elem.evento.AddListener(SeleccionarOpcion);
            elementos.Add(obj);
        }
    }

    private void SeleccionarOpcion(string val)
    {
        textoDelBloque.text = val;
        onSelectOP?.Invoke(val);
    }

    public void Cerrar()
    {
        for (int i = elementos.Count - 1; i >= 0; i--) Destroy(elementos[i]);
        elementos.Clear();
    }

    private void OnMouseUpAsButton()
    {
        if (bloque == null) Abrir();
        else if(bloque.IsClick()) Abrir();
    }

    private void OnEnable()
    {
        SelectorDeMenus.onCloseMenu.AddListener(Cerrar);
        bloque = GetComponent<Hexstar.CSE.BlockMovAndConexion>();
    }
    private void OnDisable()
    {
        SelectorDeMenus.onCloseMenu.RemoveListener(Cerrar);
    }
}
