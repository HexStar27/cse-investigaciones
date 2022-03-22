using TMPro;
using UnityEngine;

public class BloqueElemento : Bloque , IBloque
{
    IBloque siguiente;
    IBloque anterior;

    protected TextoBloque contenido;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (contenido == null) 
            if (transform.childCount > 0) 
                if(!transform.GetChild(0).TryGetComponent(out contenido))
                {
                    contenido = transform.GetChild(0).gameObject.AddComponent<TextoBloque>();
                    contenido.CambiarLayer(elemento);
                }
    }

    public IBloque Anidado(uint i){ return null; }

    public void Anidar(IBloque b, uint i) {}

    public IBloque Anterior(){ return anterior; }

    public void ConectarAnterior(IBloque p)
    { 
        anterior = p;
        if (p is null)
        {
            transform.SetParent(null);
        }
        else if (p is Bloque)
        {
            Bloque aux = (Bloque)p;
            transform.parent = aux.transform;
            Posicionar();
        }
    }

    public void ConectarSiguiente(IBloque b){ siguiente = b; }

    public uint Longitud()
    {
        uint l = 1;

        //Este bloque no tiene anidados así que no hace falta ponerlo aquí

        if (Siguiente() != null)
            l += Siguiente().Longitud();

        return l;
    }

    public uint NAnidados(){ return 0; }

    private void Posicionar()
    {
        transform.localPosition = new Vector3(0, -BloqueController.alturaBloque, 0); 
    }

    public IBloque Siguiente(){ return siguiente; }

    public string TrozoCadena()
    {
        string cad = "";
        if (contenido == null && transform.childCount > 0)
        {
            if (transform.GetChild(0).TryGetComponent(out contenido)) cad = contenido.Texto();
        }
        else if (contenido != null) cad = contenido.Texto();

        if (Siguiente() != null)
        {
            if (Siguiente() is BloqueElemento) cad += ", ";
            else cad += " ";

            cad += Siguiente().TrozoCadena();
        }
        else cad += " ";
        //Este bloque no tiene anidados así que no hace falta ponerlo aquí

        return cad;
    }

    public uint ZonaEntradaBloque(float x, float y)
    {
        Vector2 pos = transform.position;
        x -= pos.x;
        y -= pos.y;
        float limiteX = scriptableBlock.siguienteBox.x * 0.5f;
        if (x < limiteX && x > -limiteX && y > scriptableBlock.siguienteBox.y * -0.75f &&
            y < scriptableBlock.siguienteBox.y * 0) return 0;
        else return 1;
    }


    protected void MostrarTexto( bool m)
    {
        contenido.Mostrar(m);
    }

    public override void TransmitirCambioDeCapa(IBloque este, bool colocado)
    {
        Bloque b;
        if (este is Bloque)
        {
            if (este.Siguiente() != null)
            {
                b = (Bloque)este.Siguiente();
                b.TransmitirCambioDeCapa(este.Siguiente(), colocado);
            }

            for (uint i = 0; i < este.NAnidados(); i++)
            {
                if (este.Anidado(i) != null)
                {
                    b = (Bloque)este.Anidado(i);
                    b.TransmitirCambioDeCapa(este.Anidado(i), colocado);
                }
            }

            if (BloqueController.UnidoA(este))
            {
                contenido.CambiarLayer(elementoColocado);
                b = (Bloque)este;
                b.CambiarSpriteLayer(elementoColocado);
            }
            else
            {
                contenido.CambiarLayer(elemento);
                b = (Bloque)este;
                b.CambiarSpriteLayer(elemento);
            }
        }
        if (este.Anterior() == null) 
        {
            if (BloqueController.UnidoA(este)) MostrarTexto(true);
            else MostrarTexto(false);
        }
        else MostrarTexto(true);

    }
}
