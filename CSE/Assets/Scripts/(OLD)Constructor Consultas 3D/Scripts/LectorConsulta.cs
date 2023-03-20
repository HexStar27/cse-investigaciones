using UnityEngine;

public class LectorConsulta : MonoBehaviour
{
	public static Hexstar.CSE.BlockMovAndConexion input;

    private void Awake()
    {
        TryGetComponent(out input);
    }

    public static string GetQuery()
	{
        if (input == null) return "";
        var bloqueConsulta = input.GetBloqueDerecho();
        if (bloqueConsulta == null) return "";
        return bloqueConsulta.CQU.GetPartialQuery();
    }
}
