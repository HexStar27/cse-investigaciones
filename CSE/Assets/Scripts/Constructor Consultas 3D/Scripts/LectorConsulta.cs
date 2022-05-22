using UnityEngine;

public class LectorConsulta : MonoBehaviour
{
	public static BloqueMov3D raiz;

    private void Awake()
    {
        raiz = GetComponent<BloqueMov3D>();
    }

    public static string GetQuery()
	{
        if (raiz == null) return "";
        if (raiz.siguiente == null) return "";

        BloqueMov3D actual = raiz.siguiente; // El primer bloque
        string c = actual.info.ConsultaParcial();
        actual = actual.siguiente;

        while (actual != null) // El resto de bloques (si hay)
        {
            c += " " + actual.info.ConsultaParcial();
            actual = actual.siguiente;
        }

        c = c.Replace(" x ", " * ");
        return c;
    }
}
