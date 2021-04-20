using System.Collections.Generic;
using UnityEngine;

public class ListaUI : MonoBehaviour
{
    public float height = 8;
    public float distanciaElementos = 1;
    public Transform aguantaPistas;
    public PositionLimiter pl;
    private SpriteMask mascara;
#pragma warning disable IDE0044 // Agregar modificador de solo lectura
    private List<IListaUIElement> elementos = new List<IListaUIElement>();
#pragma warning restore IDE0044 // Agregar modificador de solo lectura

    private void OnEnable()
    {
        if (!mascara) if (!TryGetComponent(out mascara)) mascara = gameObject.AddComponent<SpriteMask>();
        if (!aguantaPistas)
        {
            if (!TryGetComponent(out aguantaPistas))
            {
                GameObject o = new GameObject();
                aguantaPistas = Instantiate(o, transform).transform;
                pl = o.AddComponent<PositionLimiter>();
            }
        }
        else if(!pl) if(!aguantaPistas.TryGetComponent(out pl)) pl = pl = aguantaPistas.gameObject.AddComponent<PositionLimiter>();
    }

    public void Insertar(IListaUIElement elemento)
    {
        elementos.Add(elemento);
        elemento.AsignarPadre(aguantaPistas);
        ActualizarPosiciones();
    }

    public void Eliminar(IListaUIElement elemento)
    {
        elemento.AsignarPadre(transform.parent);
        elementos.Remove(elemento);
        ActualizarPosiciones();
    }

    public bool Buscar(IListaUIElement elemento)
    {
        return elementos.Contains(elemento);
    }

    private void ActualizarPosiciones()
    {
        for(int i = 0; i < elementos.Count; i++)
        {
            elementos[i].ActualizarPosicionLocal(new Vector3(0, i - height, 0));
        }

        pl.bottom = -elementos.Count;
        pl.top = height - elementos.Count + 4;
    }
}
