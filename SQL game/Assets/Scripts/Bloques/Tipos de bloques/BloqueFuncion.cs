using UnityEngine;

public class BloqueFuncion : Bloque, IBloque
{
    IBloque siguiente;
    IBloque anterior;
    IBloque anidado;

    public IBloque Anidado(uint i = 0){ return anidado; }

    public void Anidar(IBloque b, uint i = 0){ anidado = b; }

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
        if (Anidado() != null) //It's ok, se sabe que este solo tiene 1 anidado.
            l += Anidado().Longitud();

        if (Siguiente() != null)
            l += Siguiente().Longitud();

        return l;
    }

    public uint NAnidados(){ return 1; }

    private void Posicionar()
    {
        transform.localPosition = new Vector3(0, -BloqueController.alturaBloque, 0); 
    }

    public IBloque Siguiente(){ return siguiente; }

    public string TrozoCadena()
    {
        string cad = scriptableBlock.cadena + " ";
        if (Anidado() != null)
            cad += Anidado().TrozoCadena();

        if (Siguiente() != null)
            cad += Siguiente().TrozoCadena();

        return cad;
    }

    public uint ZonaEntradaBloque(float x, float y)
    {
        Vector2 pos = transform.position;
        x -= pos.x;
        y -= pos.y;

        if (scriptableBlock.anidadosBoxPos.Length > 0 && scriptableBlock.anidadosBoxSize.Length > 0)
        {
            if (x < scriptableBlock.anidadosBoxPos[0].x + scriptableBlock.anidadosBoxSize[0].x &&
                x > scriptableBlock.anidadosBoxPos[0].x - scriptableBlock.anidadosBoxSize[0].x &&
                y < scriptableBlock.anidadosBoxPos[0].y + scriptableBlock.anidadosBoxSize[0].y &&
                y > scriptableBlock.anidadosBoxPos[0].y - scriptableBlock.anidadosBoxSize[0].y) return 0;
        }
        else Debug.LogError("El ScriptableBlock asignado al bloque "+gameObject.name+" no tiene suficientes posiciones asignadas para los huecos de bloques anidados.");

        //Se comprueba que se está indicando el 
        float limiteX = scriptableBlock.siguienteBox.x * 0.5f;
        if (x < limiteX && x > -limiteX && y > scriptableBlock.siguienteBox.y * -0.75f &&
            y < scriptableBlock.siguienteBox.y * -0.25f) return 0;
        else return 2;
    }
}
