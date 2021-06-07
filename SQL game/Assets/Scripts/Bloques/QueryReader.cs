using UnityEngine;

public class QueryReader : MonoBehaviour
{
    public ListaUI lista;
    private string query = "";

    public void ConstruirCadena()
    {
        int n = lista.N_Elementos();

        for(int i = 0; i < n; i++)
        {
            if(lista.Leer(i).Contenido().TryGetComponent(out BloqueElemento bloque))
            {
                query += bloque.scriptableBlock.cadena+" ";
            }
        }
    }

    public string Query() { return query; }

    public void DebugPrint()
    {
        Debug.Log(query);
    }
}
