///Esta clase se encarga de todo lo relacionado con los casos del juego

using System.Collections.Generic;
using UnityEngine;
using Hexstar;
using SimpleJSON;
using System.Threading.Tasks;

/// <summary>
/// singleton
/// </summary>
public class PuzzleManager : MonoBehaviour, ISingleton
{
	private static readonly int primerCasoPrincipalDelJuego = 1;
	public static int ConsultasRealizadasActuales { get; set; } = 0;
	private static float tiempoInicioCaso = .0f;
	public static int CasoActivoIndice { get; set; } = -1;
	public static bool SolucionCorrecta { get; set; } = false;
	public static float UltimoTiempoEmpleado { get; set; } = 0f;
	public static int NRetosCumplidos { get; set; } = 0;

	private static PuzzleManager Instance { get; set; }

	public static List<int> PuntuacionesPorCaso { get; private set; } = new List<int>();
	private static List<Caso> casosCargados = new List<Caso>();
	private GameObject[] casosGO = new GameObject[0];

	[SerializeField] RectTransform _map;
	[SerializeField] Vector2 _cellSize = new(50, 50);
	
	[SerializeField] TMPro.TextMeshProUGUI textoObjetivoMision;
	[SerializeField] Transform fondoMapaMejoraVision;

	[Header("Prefabs necesarios:")]
	[SerializeField] GameObject casoMapaPrefab;
	[SerializeField] GameObject casoMapaSPrefab;
	[SerializeField] GameObject casoMapaEPrefab;

	[Header("Debug")]
	[SerializeField] bool showMapGrid = false;

	/// <summary>
	/// Establece el caso activo y reinicia los contadores de consulta y tiempo empleado
	/// </summary>
	public static void IniciarStatsCaso(int indiceCaso)
    {
		CasoActivoIndice = indiceCaso;
		ConsultasRealizadasActuales = 0;
		tiempoInicioCaso = Time.realtimeSinceStartup;
    }
	
	/// <summary>
	/// Tiempo desde que se inició el caso hasta ahora
	/// </summary>
	public static float GetSetTiempoEmpleado() { return UltimoTiempoEmpleado = Time.realtimeSinceStartup - tiempoInicioCaso; }

    /// <summary>
    /// Carga casos principales, secundarios, y rellena el mapa.
    /// </summary>
    /// <param name="buscarEnArchivoGuardado"> si verdadero, cargará los datos que encuentre en el archivo de guardado</param>
    public static async Task PrepararCasosParaInicioDia(bool buscarEnArchivoGuardado = false)
    {
		Instance.QuitarTodos();

		if(buscarEnArchivoGuardado)
		{
            int n = ResourceManager.checkpoint.casosCargados.Length;
            for (int i = 0; i < n; i++)
                await Instance.LoadCasoEspecifico(ResourceManager.checkpoint.casosCargados[i]);
        }
		else //Procedimiento normal
		{
            //Cargar casos principales
            int ultimoCaso = ResourceManager.UltimoCasoPrincipalEmpezado;
            if (ultimoCaso < 0) await Instance.LoadCasoEspecifico(primerCasoPrincipalDelJuego);
            else await Instance.LoadSiguientesPrincipales();
        }

        Instance.MostrarCasosEnPantalla();
		ObtenerPuntuacionesDeCasosCargados();
	}

	/*
	public static async Task RellenarCasoFinCaso(int casosNuevos)
    {
		await Instance.LoadCasosSecundarios(casosNuevos);
		Instance.MostrarCasosEnPantalla();
		ObtenerPuntuacionesDeCasosCargados(true);
	}*/

	private static async void ObtenerPuntuacionesDeCasosCargados(bool dontClearJustAdd = false)
    {
		var puntuaciones = CasoDescripcion.Instance.panelPuntuaciones;
		if(!dontClearJustAdd)
        {
            puntuaciones.DeleteElements();
			PuntuacionesPorCaso.Clear();
		}
		if (casosCargados == null) return;
		foreach(var c in casosCargados)
        {
            puntuaciones.SetCasoID(c.id);
			await puntuaciones.SetupScore(false);
			PuntuacionesPorCaso.Add(puntuaciones.elements.Count);
		}
    }

	/// <summary>
	/// Resetea los valores relacionados con el caso actual
	/// </summary>
	public static void LimpiarFlagsDeCasoActual()
    {
		CasoActivoIndice = -1;
		SolucionCorrecta = false;
    }


	public static Caso GetCasoCargado(int idx) => casosCargados[idx];
	public static int GetTotalCasosCargados() => casosCargados.Count;
	public static Caso GetCasoActivo() => CasoActivoIndice < 0 ? null : casosCargados[CasoActivoIndice];
    public static int GetIdCasoActivo() => CasoActivoIndice < 0 ? -1 : casosCargados[CasoActivoIndice].id;
	public static bool CasoActivoEsExamen() => CasoActivoIndice >= 0 && casosCargados[CasoActivoIndice].examen;
    

    /// <summary>
    /// Pide al servidor el caso con la id especificada y lo añade a la lista
    /// </summary>
    public async Task LoadCasoEspecifico(int idCaso)
    {
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("caso", idCaso);
		await ConexionHandler.APost(ConexionHandler.baseUrl + "case/get", form);
		ParsearRawJsonACasos(ConexionHandler.download);
	}

	/// <summary>
	/// Añade el (o los) siguientes casos principales a la lista
	/// </summary>
	public async Task LoadSiguientesPrincipales()
	{
		if (ResourceManager.CasosCompletados_ListaDeEstados.Count == 0) return;
        int ultimoGanado = ResourceManager.CasosCompletados_ListaDeEstados[^1];

        WWWForm form = new();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("id", ResourceManager.UltimoCasoPrincipalEmpezado);
		form.AddField("win",  ultimoGanado);
		await ConexionHandler.APost(ConexionHandler.baseUrl + "case/next", form);
		ParsearRawJsonACasos(ConexionHandler.download);
	}

	/// <summary>
	/// Carga en la lista todos los casos que encuentre en la respuesta del servidor
	/// </summary>
	private void ParsearRawJsonACasos(string download)
	{
		string json = ConexionHandler.ExtraerJson(download);
		if (json == "{}")
		{
			Debug.LogError("Ha habido un error en el servidor al pedir los casos :(");
			TempMessageController.Instancia.GenerarMensaje("Error de conexión, comprueba que tenga acceso a internet y vuelve a cargar el juego :(");
			return;
		}

		JSONNode jNodo = JSON.Parse(download);
		int n = jNodo["res"].Count;
		for (int i = 0; i < n; i++)
		{
			string jsonCaso = jNodo["res"][i]["data"].ToString();
            if (jsonCaso == null) throw new System.Exception("Ha habido un error al parsear el caso.");
            CargarJsonComoCaso(jsonCaso);
		}
	}

	/// <summary>
	/// Carga en la lista un caso con los datos del json especificado.
	/// </summary>
	private void CargarJsonComoCaso(string json) 
	{
        Caso c = JsonConverter.PasarJsonAObjeto<Caso>(json);
		if (c != null) casosCargados.Add(c);
    }

	public void QuitarTodos()
	{
		DestruirObjetosCasoMapa();
		casosCargados.Clear();
		LimpiarFlagsDeCasoActual();
	}

	public void DestruirObjetosCasoMapa()
    {
		int n = casosGO.Length;
		for (int i = 0; i < n; i++)
		{
			Destroy(casosGO[i]);
		}
	}

	public static bool HayAlMenos1CasoPrincipal()
	{
		int n = casosCargados.Count;
		for (int i = 0; i < n; i++)
		{
			if (!casosCargados[i].secundario) return true;
		}
		return false;
	}

	public void MostrarCasosEnPantalla()
	{
		//1º Calcular grid de la pantalla
		Vector2 padding = new(75, 75); //Padding del mapa (NO de cada celda individual) 
		Vector2 paddedSize = _map.sizeDelta - 2*padding;
		
		Vector2 offset = padding - _map.sizeDelta / 2;
		Vector2 gridSize = new(
			Mathf.Max(1f, Mathf.Floor(paddedSize.x / _cellSize.x)),
			Mathf.Max(1f, Mathf.Floor(paddedSize.y / _cellSize.y)));

        int n = casosCargados.Count;
		int maxMapCapacity = (int)(gridSize.x * gridSize.y);
        if (n > maxMapCapacity)
		{
			Debug.LogError("No deberían de haber más casos cargados que posiciones disponibles en el mapa (max=" + maxMapCapacity + ") ...");
			n = maxMapCapacity;
        }

        //2º Inicializar objetos CasoMapa (1 por caso almacenado)
        DestruirObjetosCasoMapa();
        casosGO = new GameObject[n];
        for (int i = 0; i < n; i++)
        {
			GameObject objeto;
			if (casosCargados[i].examen) objeto = Instantiate(casoMapaEPrefab, _map);			// Caso Examen
			else if (casosCargados[i].secundario) objeto = Instantiate(casoMapaSPrefab, _map);	// Caso Secundario
			else objeto = Instantiate(casoMapaPrefab, _map);									// Caso Principal
            
			if (objeto.TryGetComponent(out CasoMapa casoM))
            {
				casoM.CargarDatosCaso(i);
            }
			//3º Posicionar
			Vector2 gPos = gridSize.HashCaso2Pos(casosCargados[i]);
			Vector2 mPos = gPos * _cellSize + offset;

            if (AlreadyACaseThere(mPos,i)) //Evitar superposición
            {
				Debug.Log("Evitando superposición");
				Vector2 newPos = FindFreeCell(gridSize, gPos, offset, i);
				if(newPos != -Vector2.one) mPos = newPos;
            }
            objeto.transform.localPosition = mPos;
            casosGO[i] = objeto;
        }
    }

	private Vector2 FindFreeCell(Vector2 gridSize, Vector2 wantedCell, Vector2 mapOffset, int objectsAssgined)
	{
		Vector2 cellPos,propossedCell;
		for(int i = 1; i < gridSize.x; i++)
		{
			propossedCell.x = (wantedCell.x + i) % gridSize.x;
			
			for (int j = 1; j < gridSize.y; j++)
			{
				propossedCell.y = (wantedCell.y + j) % gridSize.y;

				cellPos = propossedCell * _cellSize +mapOffset;
				if (!AlreadyACaseThere(cellPos,objectsAssgined)) return cellPos;
			}
		}
		return -Vector2.one;
	}
	private bool AlreadyACaseThere(Vector3 pos, int n = 0)
	{
		bool thereIs = false;
		while (casosGO[n] != null) n++;
		for (int i = 0; i < n && !thereIs; i++) thereIs |= casosGO[i].transform.localPosition == pos;
		return thereIs;
	}

	/// <summary>
	/// (DEPRECATED) Más adelante el objetivo se podrá mirar en el informe del cajón de pistas.
	/// </summary>
	/// <param name="value"></param>
	public static void MostrarObjetivoDeCasoEnPantalla(bool value)
    {
		bool ok = value && CasoActivoIndice >= 0;
        Instance.textoObjetivoMision.text = "";
        Instance.fondoMapaMejoraVision.gameObject.SetActive(ok);
        if (ok)
		{
			string resumenCasoActual = casosCargados[CasoActivoIndice].resumen;
            Instance.textoObjetivoMision.text = resumenCasoActual;
        }
	}

	private void Awake()
	{
		Instance = this;
	}
	private void Start()
	{
        CasoDescripcion.Instance.panelPuntuaciones.InitializePrefab();
    }

	public void ResetSingleton()
	{
        ConsultasRealizadasActuales = 0;
		UltimoTiempoEmpleado = 0f;
		NRetosCumplidos = 0;
		tiempoInicioCaso = .0f;
		QuitarTodos();
		PuntuacionesPorCaso.Clear();
    }
    private void OnEnable()
	{
		MenuPausa.onExitLevel.AddListener(ResetSingleton);
	}
	private void OnDisable()
	{
        MenuPausa.onExitLevel.RemoveListener(ResetSingleton);
    }

	private void OnDrawGizmosSelected()
	{
		if(showMapGrid)
		{
            Vector2 padding = new(75, 75); //Padding del mapa (NO de cada celda individual) 
            Vector2 paddedSize = _map.sizeDelta - 2 * padding;

            Vector2 offset = padding - _map.sizeDelta / 2;
            Vector2 gridSize = new(
                Mathf.Max(1f, Mathf.Floor(paddedSize.x / _cellSize.x)),
                Mathf.Max(1f, Mathf.Floor(paddedSize.y / _cellSize.y)));

			Vector2 gFrom = offset;
			Vector2 gTo = new Vector2(0, gridSize.y * _cellSize.y) + offset;
			for (int i = 0; i < gridSize.x; i++)
			{
				gFrom.x += _cellSize.x;
				gTo.x += _cellSize.x;
                Vector3 from = _map.transform.TransformPoint(gFrom);
				Vector3 to = _map.transform.TransformPoint(gTo);
                Gizmos.DrawLine(from, to);
            }

            gFrom = offset;
			gTo = new Vector2(gridSize.x * _cellSize.x, 0) + offset;
            for (int j = 0; j < gridSize.y; j++)
            {
                gFrom.y += _cellSize.y;
                gTo.y += _cellSize.y;
                Vector3 from = _map.transform.TransformPoint(gFrom);
				Vector3 to = _map.transform.TransformPoint(gTo);
				Gizmos.DrawLine(from, to);
            }
        }
	}

}

public static class ListAlgorithms
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

	/// <summary>
	/// Una simple función hash que convierte el título de un caso en una posición para el mapa. <br>
	/// De esta forma los casos siempre tendrán la misma posición "aleatoria" cada ve que se cargue el juego.<br>
	/// </summary>
	/// <returns>El índice en el grid para el caso</returns>
	public static Vector2 HashCaso2Pos(this Vector2 tamGrid, Caso caso)
	{
		//TODO: Asegurarse de que no puedan aparecer 2 casos en una misma posición...
		int lTitulo = caso.titulo.Length;
		Vector2 r = new();
		for (int i = 0; i < lTitulo; i++)
		{
			r[i % 2] += caso.titulo[i];
		}
		r.x %= tamGrid.x;
		r.y %= tamGrid.y;
		return r;
	}
}