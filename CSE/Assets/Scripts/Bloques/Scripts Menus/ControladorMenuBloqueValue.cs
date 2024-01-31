using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Este script va en el menú para establecer un valor al bloque de tipo valor (número, cadena, fecha)
public class ControladorMenuBloqueValue : MonoBehaviour
{
    private static ControladorMenuBloqueValue instance;
    public static ControladorMenuBloqueValue Instance { get { return instance; } }

    [SerializeField] GameObject elementoPrefab;
    [SerializeField] Transform contentObject;
    [SerializeField] ToggleGroup tGroup;
    private List<GameObject> elementosActivos = new List<GameObject>();
    private ConfiguradorBloqueValor bloqueConfigurandoActual;

    private string pistaActual;

    public void SeleccionarBloqueAConfigurar(ConfiguradorBloqueValor bloque)
    {
        bloqueConfigurandoActual = bloque;
    }

    public void Abrir()
    {
        var caso = PuzzleManager.GetCasoActivo();
        if (caso == null) return;

        var pistas = caso.pistas;
        for(int i = 0; i < pistas.Length; i++)
        {
            string pista = pistas[i].palabra;
            GameObject o = Instantiate(elementoPrefab, contentObject);
            elementosActivos.Add(o);
            var elem = o.GetComponent<ElementoMenuValor>();
            elem.SetText(pista);
            elem.toggle.group = tGroup;
        }
    }

    public void Cerrar()
    {
        for (int i = elementosActivos.Count - 1; i >= 0; i--) Destroy(elementosActivos[i]);
        elementosActivos.Clear();

        if (bloqueConfigurandoActual != null)
        {
            bloqueConfigurandoActual.CambiarTexto(pistaActual);
        }
    }

    private void Transmitir(string value) { pistaActual = value; }

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        ElementoMenuValor.onToggle.AddListener(Transmitir);
        Abrir();
    }
    private void OnDisable()
    {
        Cerrar();
        ElementoMenuValor.onToggle.RemoveListener(Transmitir);
    }
}
