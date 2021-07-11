using System.Threading.Tasks;
using UnityEngine;

public class BloqueController : MonoBehaviour
{
    public static float alturaBloque = 1;
    public static float alturaConsola = 0f;
    private static PositionLimiter pl;
    private static IBloque raiz;

    private void OnEnable()
    {
        if (pl == null) 
            if (!TryGetComponent(out pl))
            {
                pl = gameObject.AddComponent<PositionLimiter>();
                pl.bottom = -1;
                ActualizarPL();
            }
    }

    public static void AsignarBloque(IBloque b){ raiz = b; }

    public static uint NumBloques(){ return raiz.Longitud(); }

    public static string ObtenerCadenaConsulta()
    {
        if (raiz != null) return raiz.TrozoCadena();
        else return "";
    }

    public static IBloque Raiz() { return raiz; }

    public void DebugPrint()
    {
        Debug.Log(ObtenerCadenaConsulta());
    }

    /// <summary>
    /// Comprueba que alguno de los bloques que custodie sea el bloque pasado 'b'
    /// </summary>
    /// <param name="b">EL bloque a comprobar</param>
    /// <returns>Verdadero si está unido al controller</returns>
    public static bool UnidoA(IBloque b)
    {
        IBloque siguiente = raiz;
        bool unido = false;
        while(siguiente != null && unido == false)
        {
            if (siguiente == b) unido = true;
            siguiente = siguiente.Siguiente();
        }
        return unido;
    }

    /// <summary>
    /// Actualiza el perímetro disponible del PositionLimiter
    /// </summary>
    public static void ActualizarPL()
    {
        if(raiz != null)
        {
            pl.top = raiz.Longitud() * alturaBloque + 2.5f;
        }
    }

}
