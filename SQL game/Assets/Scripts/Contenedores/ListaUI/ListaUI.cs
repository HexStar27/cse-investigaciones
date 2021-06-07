using System.Collections.Generic;
using UnityEngine;

public class ListaUI : MonoBehaviour
{
    public float height = 8;
    public float distanciaElementos = 1;
    public Transform aguantaElementos;
    public PositionLimiter pl;
#pragma warning disable IDE0044 // Agregar modificador de solo lectura
    private List<IListaUIElement> elementos = new List<IListaUIElement>();
#pragma warning restore IDE0044 // Agregar modificador de solo lectura

    private void OnEnable()
    {
        //if (!mascara) if (!TryGetComponent(out mascara)) mascara = gameObject.AddComponent<SpriteMask>();
        if (!aguantaElementos)
        {
            if (!TryGetComponent(out aguantaElementos))
            {
                GameObject o = new GameObject();
                aguantaElementos = Instantiate(o, transform).transform;
                pl = o.AddComponent<PositionLimiter>();
            }
        }
        else if(!pl) if(!aguantaElementos.TryGetComponent(out pl)) pl = pl = aguantaElementos.gameObject.AddComponent<PositionLimiter>();
    }

    public void InsertarEn(IListaUIElement elemento, int pos)
    {
        elementos.Insert(pos, elemento);
        elemento.AsignarPadre(aguantaElementos);
        ActualizarPosiciones();
    }

    public void Insertar(IListaUIElement elemento)
    {
        elementos.Add(elemento);
        elemento.AsignarPadre(aguantaElementos);
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

    public IListaUIElement Leer(int indice)
    {
        return elementos[indice];
    }

    public int N_Elementos()
    {
        return elementos.Count;
    }

    private void ActualizarPosiciones()
    {
        for(int i = 0; i < elementos.Count; i++)
        {
            elementos[i].ActualizarPosicionLocal(new Vector3(0, -i - transform.localPosition.y, transform.position.z - 1));
        }

        pl.bottom = height - (distanciaElementos * 0.5f);
        float size = elementos.Count * distanciaElementos;
        
        if (size >= height*2) pl.top = pl.bottom + size - (height*2) + transform.localPosition.y + 1;
        else pl.top = pl.bottom + transform.localPosition.y;
    }
}
