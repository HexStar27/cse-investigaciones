using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlmacenBloques", menuName = "Hexstar/Bloques/AlmacenBloques2")]
public class AlmacenDeBloquesSimple : ScriptableObject
{
    [System.Serializable] public struct Bloque
    {
        public GameObject prefab;
        public int disponibleEnDificultad;
        public string titulo;
    }

    public List<Bloque> bloquesPrefab = new List<Bloque>();

    public List<Bloque> BloquesDisponiblesEnDificultad(int dificultadActual)
    {
        List<Bloque> lista = new List<Bloque>();
        foreach (var bloque in bloquesPrefab)
            if (bloque.disponibleEnDificultad <= dificultadActual)
                lista.Add(bloque);
        return lista;
    }
}
