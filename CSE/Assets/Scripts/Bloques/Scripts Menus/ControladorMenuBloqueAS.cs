using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Este script va en el menú para establecer un valor al bloque de tipo valor (número, cadena, fecha)
public class ControladorMenuBloqueAS : MonoBehaviour
{
    private static ControladorMenuBloqueAS instance;
    public static ControladorMenuBloqueAS Instance { get { return instance; } }

    [SerializeField] ToggleGroup tGroup;
    private ConfiguradorBloqueValor bloqueConfigurandoActual;

    private string pistaActual;

    public void SeleccionarBloqueAConfigurar(ConfiguradorBloqueValor bloque)
    {
        bloqueConfigurandoActual = bloque;
    }

    public void Cerrar()
    {
        if(bloqueConfigurandoActual != null) bloqueConfigurandoActual.CambiarTexto(pistaActual);
    }

    private void Transmitir(string value) 
    { 
        pistaActual = value;
    }

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        ElementoMenuAS.onToggle.AddListener(Transmitir);
    }
    private void OnDisable()
    {
        Cerrar();
        ElementoMenuAS.onToggle.RemoveListener(Transmitir);
    }
}
