using System.Collections.Generic;
/// Esta clase se encarga de guardar y cargar los datos relevantes 
/// de la partida del ResourceManager y demás sistemas únicos.
[System.Serializable]
public class PuntoGuardado
{
	public int agentesDisponibles;
    public int consultasDisponibles;
	public int consultasMaximas;

	public int dificultadActual;
	public int dia;
	public int casoEnCurso;			//Indice del caso en curso del array de casos cargados
	public int[] casosCargados;
	public string[] tableCodes;

	public int[] casosCompletados;
	public int ultimoCasoPrincipalEmpezado;
	public int[] casosCompletados_listaDeEstados;
	public Hexstar.CSE.Informes.Informe[] informes;

	public string dialogueEventList;
	public int[] eventosEjecutados;
	public bool hasCompletedTutorial;

	public PuntoGuardado()
	{
		agentesDisponibles = 3;
		consultasDisponibles = 4;
		consultasMaximas = 4;
		casosCompletados = new int[0];
		dificultadActual = 1;
		dia = 0;
		casosCargados = new int[0];
		casoEnCurso = -1;
		ultimoCasoPrincipalEmpezado = -1;
		casosCompletados_listaDeEstados = new int[0];
        eventosEjecutados = new int[0];
		tableCodes = new string[0];
		hasCompletedTutorial = false;
		dialogueEventList = "";
		informes = new Hexstar.CSE.Informes.Informe[0];
    }

	public void CopiarDatosDelSistema()
	{
		agentesDisponibles = ResourceManager.AgentesDisponibles;
		consultasDisponibles = ResourceManager.ConsultasDisponibles;
		consultasMaximas = ResourceManager.ConsultasMaximas;
		casosCompletados = ResourceManager.CasosCompletados.ToArray();
		dificultadActual = ResourceManager.DificultadActual;
		dia = ResourceManager.Dia;

		List<int> idCasos = new List<int>();
		int n = PuzzleManager.GetTotalCasosCargados();
        for (int i = 0; i < n; i++)
		{
			int id = PuzzleManager.GetCasoCargado(i).id;
            if (!ResourceManager.CasosCompletados.Contains(id))
				idCasos.Add(id);
        }
		casosCargados = idCasos.ToArray();
		
		casoEnCurso = PuzzleManager.CasoActivoIndice;
		ultimoCasoPrincipalEmpezado = ResourceManager.UltimoCasoPrincipalEmpezado;
		casosCompletados_listaDeEstados = ResourceManager.CasosCompletados_ListaDeEstados.ToArray();

		eventosEjecutados = ResourceManager.EventosEjecutados.ToArray();
		tableCodes = ResourceManager.TableCodes.ToArray();

		dialogueEventList = Hexstar.Dialogue.ControladorDialogos.GetAllEventsFromDict();

		informes = Hexstar.CSE.Informes.CarpetaInformesController.Informes.ToArray();
    }

	public void CargarDatosAlSistema()
	{
		ResourceManager.AgentesDisponibles = agentesDisponibles;
		ResourceManager.ConsultasDisponibles = consultasDisponibles;
		ResourceManager.ConsultasMaximas = consultasMaximas;

        List<int> completados = new();
		completados.AddRange(casosCompletados);
		ResourceManager.CasosCompletados = completados;

		List<int> estados_cc = new();
		estados_cc.AddRange(casosCompletados_listaDeEstados);
		ResourceManager.CasosCompletados_ListaDeEstados = estados_cc;

        List<int> ee = new();
        ee.AddRange(eventosEjecutados);
        ResourceManager.EventosEjecutados = ee;

        ResourceManager.UltimoCasoPrincipalEmpezado = ultimoCasoPrincipalEmpezado;
		ResourceManager.DificultadActual = dificultadActual;
		ResourceManager.Dia = dia;

		List<string> codes = new List<string>();
		codes.AddRange(tableCodes);
		ResourceManager.TableCodes = codes;
		
		Hexstar.Dialogue.ControladorDialogos.SetAllEventsToList(dialogueEventList, true);

		Hexstar.CSE.Informes.CarpetaInformesController.Informes.Clear();
        for (int i = 0; i < informes.Length; i++)
			Hexstar.CSE.Informes.CarpetaInformesController.Informes.Add(informes[i]);
    }
}
