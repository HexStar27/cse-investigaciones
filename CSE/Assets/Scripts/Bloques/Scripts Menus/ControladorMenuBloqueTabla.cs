using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Almacen = Hexstar.CSE.AlmacenDePalabras;

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
        List<string> tablas = Almacen.GetLista(Hexstar.CSE.TabType.Tablas);
        GenerarElementosMenu(tablas,zonaMenuTablas);
    }

    public void AbrirMenuColumnas()
    {
        List<string> columnas = Almacen.GetLista(Hexstar.CSE.TabType.Columnas);
        if(bloqueConfigurandoActual.FreeOfAS())
        {
            columnas = IncluirElementosRenombrados(columnas);
        }
        GenerarElementosMenu(Filtrar(columnas),zonaMenuColumna);
    }

    private List<string> Filtrar(List<string> lista)
    {
        List<string> columnas = new List<string>(lista);
        //Filtrado de columnas según tablas en bloques.
        int idx = 0;
        bool deletionMode = false;
        string tabla = "";
        bool Busqueda(MenuTablesFilter ctx) { return ctx.currentTable == tabla; }
        while (idx < columnas.Count)
        {
            string txt = columnas[idx];
            if (txt.Contains(":"))
            {
                tabla = txt.Substring(7); // "Tabla: "
                //Filtrar columnas si Tabla no presente
                if (txt.Contains("Columnas Renombradas:")) deletionMode = false;
                else deletionMode = !MenuTablesFilter.activados.Find(Busqueda);
            }
            else if (deletionMode) columnas.RemoveAt(idx--);
            idx++;
        }
        return columnas;
    }

    private List<string> IncluirElementosRenombrados(List<string> col)
    {
        List<string> columnas = new List<string>(col);
        var mapaTablas = Almacen.GenerarMapaSeparacionColumnasEnTablas();

        int nColumnasTotales = Almacen.palabras[1].Count;
        //Incluir columnas con prefijo de la tabla renombrada
        foreach (var par in Almacen.aliasParaTablas)
        {
            string tabla = par.Key;
            int tIdx = mapaTablas[tabla] + 1;
            List<string> renombrados = par.Value;
            var mc = Almacen.MapaDeColumnasQueContenganX(columnas, ":");
            int idx = mc[tabla];
            int nRenomb = renombrados.Count;
            for (int r = 0; r < nRenomb; r++)
            {
                int c = 0;
                bool bien = true;
                while (tIdx + c < nColumnasTotales && bien)
                {
                    bien = !Almacen.palabras[1][c + tIdx].Contains(":");
                    if (bien)
                    {
                        columnas.Insert(idx + 1+c, renombrados[r] + "." + Almacen.palabras[1][tIdx + c++]);
                    }
                }
            }
        }
        //Incluir columnas renombradas al final
        columnas.Add("Columnas Renombradas:");
        foreach (var par in Almacen.aliasParaColumnas)
        {
            List<string> renombres = par.Value;
            for(int i = 0; i < renombres.Count; i++) columnas.Add(renombres[i]);
        }
        return columnas;
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
            if (t.Contains(":")) //Es un separador por tablas
            {
                o.GetComponent<Toggle>().enabled = false;
                o.transform.GetChild(0).gameObject.SetActive(false);
            }
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
        opcionSeleccionada = "";
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
