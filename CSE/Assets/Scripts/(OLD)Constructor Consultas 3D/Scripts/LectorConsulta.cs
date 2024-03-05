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
        if(QueryModeController.IsQueryModeOnManual())
        {
            return Intelisense.instance.pantalla.text;
        }
        else
        {
            if (input == null) return "";
            var bloqueConsulta = input.GetBloqueDerecho();
            if (bloqueConsulta == null) return "";
            return bloqueConsulta.CQU.GetPartialQuery();
        }
    }

    private void Setup_xAPI_Statement()
    {
        if (input.GetBloqueDerecho() != null) CSE.XAPI_Builder.CreateStatement_QueryConstruction(GetQuery());
    }
    
    private void OnEnable()
    {
        input.onBlockConectionChanged.AddListener(Setup_xAPI_Statement);
    }
    private void OnDisable()
    {
        input.onBlockConectionChanged.RemoveListener(Setup_xAPI_Statement);
    }
}
