using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorMenuBloqueTabla : MonoBehaviour
{
    private static ControladorMenuBloqueTabla instance;
    public static ControladorMenuBloqueTabla Instance { get { return instance; } }

    private ConfiguradorBloqueValor bloqueConfigurandoActual;
    private List<GameObject> elementosActivosMenus = new List<GameObject>();
    private string opcionSeleccionada;

    [SerializeField] GameObject elementoPrefab;
    [SerializeField] Transform zonaMenuTablas;
    [SerializeField] Transform zonaMenuColumna;
    [SerializeField] ToggleGroup tGroupTablas;
    [SerializeField] ToggleGroup tGroupColumnas;

    public void SeleccionarBloqueAConfigurar(ConfiguradorBloqueValor bloque)
    {
        bloqueConfigurandoActual = bloque;
    }

    public void AbrirMenuTablas()
    {
        List<string> tablas = Hexstar.CSE.AlmacenDePalabras.GetLista(Hexstar.CSE.TabType.Tablas);
        GenerarElementosMenu(tablas,zonaMenuTablas);
    }

    public void AbrirMenuColumnas()
    {
        List<string> columnas = Hexstar.CSE.AlmacenDePalabras.GetLista(Hexstar.CSE.TabType.Columnas);
        GenerarElementosMenu(columnas,zonaMenuColumna);
    }

    private void GenerarElementosMenu(List<string> nombres, Transform zonaMenus)
    {
        foreach (var t in nombres)
        {
            GameObject o = Instantiate(elementoPrefab, zonaMenus);
            elementosActivosMenus.Add(o);
            var elem = o.GetComponent<ElementoMenuValor>();
            elem.SetText(t);
            elem.toggle.group = tGroupTablas;
            elem.useCuotes = false;
        }
        var scaler = zonaMenus.GetComponent<ContentScaler>();
        if(scaler != null) scaler.Actualizar();
    }

    public void CerrarMenus()
    {
        for (int i = elementosActivosMenus.Count - 1; i >= 0; i--) Destroy(elementosActivosMenus[i]);
        elementosActivosMenus.Clear();

        if (bloqueConfigurandoActual != null) bloqueConfigurandoActual.CambiarTexto(opcionSeleccionada);
        bloqueConfigurandoActual = null;
    }

    private void TransmitirTabla(string value) 
    { 
        if (zonaMenuTablas.gameObject.activeSelf) opcionSeleccionada = value; 
    }
    private void TransmitirColumna(string value)
    {
        if (zonaMenuColumna.gameObject.activeSelf) opcionSeleccionada = value;
    }

    private void Awake()
    {
        instance = this;
        ElementoMenuValor.onToggle.AddListener(TransmitirTabla);
        ElementoMenuValor.onToggle.AddListener(TransmitirColumna);
        SelectorDeMenus.onCloseMenu.AddListener(CerrarMenus);
    }
}
