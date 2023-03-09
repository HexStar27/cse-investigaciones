using UnityEngine;
using Hexstar.CSE;
using TMPro;

public class BloqueInfo3D : MonoBehaviour
{
    [HideInInspector] public BloqueMov3D mov;
    [SerializeField] protected DatosBloque datos;
    [SerializeField] protected TextMeshPro prefijo = null;
    [SerializeField] protected TextMeshPro cuadroTexto = null;
    [SerializeField] protected GameObject hijoPrefab = null;
    [SerializeField] protected GameObject flechaPrefab = null;
    [HideInInspector] public BloqueInfo3D hijoActual;
    [HideInInspector] public Flecha3D flechaActual;


    public void Inicializar(DatosBloque datos)
    {
        this.datos = datos;
        ActualizarConfiguracion();
    }

    public void AbrirSelector()
    {
        SelectorPalabras.instancia.Abrir(this, cuadroTexto.text);
        BloquearTiposPalabras();
    }

    public virtual void SeleccionarContenido(string nuevoContenido)
    {
        cuadroTexto.text = nuevoContenido;
    }

    public string ConsultaParcial()
    {
        return datos.prefijo + " " + cuadroTexto.text;
    }

    public string Prefijo()
    {
        return datos.prefijo;
    }

    private void Awake()
    {
        mov = GetComponent<BloqueMov3D>();
    }

    public virtual void BloquearTiposPalabras()
    {
        int n = datos.tiposBloqueados.Length;
        for (int i = 0; i < n; i++)
        {
            SelectorPalabras.instancia.tabs.Bloquear((int)datos.tiposBloqueados[i]);
        }

        //Seleccionar una pestaña que no esté bloqueada
        if (SelectorPalabras.instancia.tabs.selectedTab != null)
        {
            if (!SelectorPalabras.instancia.tabs.selectedTab.Desbloqueado())
                SeleccionarTabDisponible();
        }
        else SeleccionarTabDisponible();

    }

    protected void SeleccionarTabDisponible()
    {
        bool selected = false;
        for (int i = 0; i < SelectorPalabras.instancia.tabs.tabButtons.Count; i++)
        {
            if (SelectorPalabras.instancia.tabs.tabButtons[i].Desbloqueado())
            {
                SelectorPalabras.instancia.tabs.OnTabSelected(SelectorPalabras.instancia.tabs.tabButtons[i]);
                selected = true;
            }
        }
        if (!selected) SelectorPalabras.instancia.tabs.OnTabSelected(null);
    }

    protected virtual void ActualizarConfiguracion()
    {
        prefijo.text = datos.prefijo;
        /* Código zombie... (Ya no se usan las flechas...)
        if (datos.usaFlecha && hijoPrefab != null)
        {
            BloqueInfo3D h = Instantiate(hijoPrefab, transform.parent).GetComponent<BloqueInfo3D>();
            DatosBloque datos = new DatosBloque(-1, ")",Color.white);
            h.Inicializar(datos);
            hijoActual = h;
            h.gameObject.SetActive(false);

            Flecha3D f = Instantiate(flechaPrefab).GetComponent<Flecha3D>();
            f.Inicializar(transform, h.transform);
            f.Activar(false);
            flechaActual = f;
        }
        */
    }
}
