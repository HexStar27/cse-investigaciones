using UnityEngine;

public class BloqueOperador : Bloque, IBloque
{
    IBloque siguiente;
    IBloque anterior;
    IBloque anidado;
    IBloque anidado2;

    public IBloque Anidado(uint i = 0)
    {
        if (i == 0)
            return anidado;
        else
            return anidado2;
    }

    public void Anidar(IBloque b, uint i = 0)
    {
        if (i == 0) anidado = b;
        else anidado2 = b;
    }

    public IBloque Anterior(){ return anterior; }

    public void ConectarAnterior(IBloque b)
    {
        anterior = b;
        if (b is null)
        {
            transform.SetParent(null);
        }
        else if (b is Bloque)
        {
            Bloque aux = (Bloque)b;
            transform.parent = aux.transform;
            Posicionar();
        }
    }

    public void ConectarSiguiente(IBloque b){ siguiente = b; }

    public uint Longitud()
    {
        uint l = 1;
        if (Anidado(0) != null)
            l += Anidado(0).Longitud();
        if (Anidado(1) != null)
            l += Anidado(1).Longitud();

        if (Siguiente() != null)
            l += Siguiente().Longitud();

        return l;
    }

    public uint NAnidados(){ return 2; }

    private void Posicionar() 
    {
        transform.localPosition = new Vector3(0, -BloqueController.alturaBloque, 0);
    }

    public IBloque Siguiente(){ return siguiente; }

    public string TrozoCadena()
    {
        string cad = scriptableBlock.cadena + " ";
        if (Anidado(0) != null)
            cad += Anidado(0).TrozoCadena();
        if (Anidado(1) != null)
            cad += Anidado(1).Longitud();

        if (Siguiente() != null)
            cad += Siguiente().TrozoCadena();

        return cad;
    }

    public uint ZonaEntradaBloque(float x, float y)
    {
        Vector2 pos = transform.position;
        x -= pos.x;
        y -= pos.y;

        if (scriptableBlock.anidadosBoxPos.Length > 0 &&
            scriptableBlock.anidadosBoxSize.Length == scriptableBlock.anidadosBoxPos.Length)
        {
            //En realidad este bucle for siempre va a hacer 2 iteraciones pero
            //me ha dado pereza escribir 2 condicionales if de semejante tamaño.
            for (uint i = 0; i < scriptableBlock.anidadosBoxPos.Length; i++)
            if (x < scriptableBlock.anidadosBoxPos[i].x + scriptableBlock.anidadosBoxSize[i].x &&
                x > scriptableBlock.anidadosBoxPos[i].x - scriptableBlock.anidadosBoxSize[i].x &&
                y < scriptableBlock.anidadosBoxPos[i].y + scriptableBlock.anidadosBoxSize[i].y &&
                y > scriptableBlock.anidadosBoxPos[i].y - scriptableBlock.anidadosBoxSize[i].y) return i;
        }
        else Debug.LogError("El ScriptableBlock asignado al bloque " + gameObject.name + " no tiene suficientes posiciones asignadas para los huecos de bloques anidados.");

        //Se comprueba que se está indicando el 
        float limiteX = scriptableBlock.siguienteBox.x * 0.5f;
        if (x < limiteX && x > -limiteX && y > scriptableBlock.siguienteBox.y * -0.75f &&
            y < scriptableBlock.siguienteBox.y * -0.25f) return 0;
        else return 3;
    }
}
