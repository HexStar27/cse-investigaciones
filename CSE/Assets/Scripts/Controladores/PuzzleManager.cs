///Esta clase se encarga de todo lo relacionado con los casos del juego

using System.Collections.Generic;
using UnityEngine;
using Hexstar;
using Hexstar.CSE;
using SimpleJSON;
using System.Threading.Tasks;
using CSE.Local;

/// <summary>
/// singleton
/// </summary>
public class PuzzleManager : MonoBehaviour, ISingleton
{
	private static readonly int primerCasoPrincipalDelJuego = 1;
	public static int NConsultasDuranteElCaso { get; set; } = 0;
	private static Caso CasoActivo { get; set; } = null;
	public static bool SolucionCorrecta { get; set; } = false;
	public static float UltimoTiempoEmpleado { get; set; } = 0f;
	private static float tiempoInicioCaso = .0f;
	public static int NRetosCumplidos { get; set; } = 0;

	private static PuzzleManager Instance { get; set; }

	
	private readonly static Dictionary<int,Caso> casosCargados = new();
	private readonly static Dictionary<int,CasoMapa> casosGO = new();
	private readonly static List<int> idCasosNoColocados = new();


	[SerializeField] RectTransform _map;
	[SerializeField] Vector2 _cellSize = new(50, 50);
	[SerializeField] Vector2 _paddingMapa = new(75, 75);
	//Variables del mapa derivadas...
	Vector2 offset, paddedSize, gridSize;

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
	public static void IniciarStatsCaso(int idCaso)
    {
		CasoActivo = casosCargados[idCaso];
		NConsultasDuranteElCaso = 0;
        NRetosCumplidos = 0;
        tiempoInicioCaso = Time.realtimeSinceStartup;
    }
	
	/// <summary>
	/// Tiempo desde que se inició el caso hasta ahora
	/// </summary>
	public static float GetSetTiempoEmpleado() { return UltimoTiempoEmpleado = Time.realtimeSinceStartup - tiempoInicioCaso; }

    /// <summary>
    /// Carga casos principales y rellena el mapa. Para el inicio del día.
    /// </summary>
    /// <param name="buscarEnArchivoGuardado"> si verdadero, cargará los datos que encuentre en el archivo de guardado</param>
    public static async Task PrepararCasosParaInicioDia(bool buscarEnArchivoGuardado = false)
    {
		if(buscarEnArchivoGuardado) // Carga los casos de las ids guardadas (casos aún no completados supuestamente)
		{
            Instance.QuitarTodos();
            int n = Mathf.Min(ResourceManager.checkpoint.casosCargados.Length, ResourceManager.checkpoint.caducidadCasosCargados.Count);
			for (int i = 0; i < n; i++)
			{
				int id = ResourceManager.checkpoint.casosCargados[i];
                await LoadCasoEspecifico(id);
				casosCargados[id].caducidad = ResourceManager.checkpoint.caducidadCasosCargados[i];
			}
        }
		else if (ResourceManager.CasosCompletados.Count == 0)
		{
            // Si no ha completado ninguno, cargar el del tutorial
            await LoadCasoEspecifico(primerCasoPrincipalDelJuego);
		}
        

        Instance.PrepararCasosMapa();
		ObtenerPuntuacionesDeTodosLosCasos();
	}

	/// <summary>
	/// No se aplicará la caducidad a aquellos casos cargados que no estén presentes en el mapa ya que sería injusto
	/// </summary>
	public static void AplicarCaducidadDeCasosCargados()
	{
		List<int> casosAEliminar = new();
		var e = casosCargados.Values.GetEnumerator();
		while(e.MoveNext())
		{
			Caso c = e.Current;
			if (idCasosNoColocados.Contains(c.id)) continue; // No está presente en el mapa
			if (c.caducidad < 0) continue; // Se ignora la caducidad
			
			c.caducidad--;
			if (c.caducidad <= 0)
			{
				casosAEliminar.Add(c.id);
                ResourceManager.CasosCompletados.Add(c.id);
                ResourceManager.CasosCompletados_ListaDeEstados.Add(0); //Cuenta como caso ABANDONADO
                //No se comprueban los bounties (ni aunque perder sea uno de ellos)
            }
		}
		for (int i = 0; i < casosAEliminar.Count; i++) EliminarCaso(casosAEliminar[i]);
	}

	/// <summary>
	/// Pide las puntuaciones de cada caso al servidor. Elimina las puntuaciones cargadas anteriormente.
	/// </summary>
	private static async void ObtenerPuntuacionesDeTodosLosCasos()
    {
		if (casosCargados == null) return;

		CasoDescripcion.Instance.ResetSingleton();
		foreach(var c in casosCargados) await CasoDescripcion.Instance.RellenarPuntuacionesDeCaso(c.Key);
    }

	public static async void InsertarCasoExtra(int idCaso)
	{
        await LoadCasoEspecifico(idCaso);
		
		Instance.InstanciarCasoMapa(casosCargados[idCaso]);
		Instance.IntentarColorcarCasosNoColocados();
        
		await CasoDescripcion.Instance.RellenarPuntuacionesDeCaso(idCaso);
    }

	public static void QuitarCasoActivo() => CasoActivo = null;

    public static Caso GetCasoCargado(int idCaso) => casosCargados[idCaso];
	public static Caso GetCasoActivo() => CasoActivo;
    public static int GetIdCasoActivo() => CasoActivo != null ? CasoActivo.id : -1;
	public static bool CasoActivoEsExamen() => CasoActivo != null && CasoActivo.examen;
    public static List<Caso> GetTodosLosCasosCargados()
    {
		List<Caso> cs = new();
		foreach (var curr in casosCargados.Values) cs.Add(curr);
		return cs;
    }

    /// <summary>
    /// Pide al servidor el caso con la id especificada y lo añade a la lista
    /// </summary>
    public static async Task LoadCasoEspecifico(int idCaso)
    {
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("caso", idCaso);
		await ConexionHandler.APost(ConexionHandler.baseUrl + "case/get", form);
		ParsearRawJsonACasos(ConexionHandler.download);
	}

	/// <summary>
	/// Añade el (o los) siguientes casos a la lista según el resultado del último caso completado.
	/// Debe usarse justo después de completar un caso.
	/// </summary>
	public static async Task LoadCasosSiguientes()
	{
		if (ResourceManager.CasosCompletados_ListaDeEstados.Count == 0) return;
		int id = ResourceManager.CasosCompletados[^1];
        int ganado = ResourceManager.CasosCompletados_ListaDeEstados[^1];

        WWWForm form = new();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("id", id);
		form.AddField("win", ganado);
		await ConexionHandler.APost(ConexionHandler.baseUrl + "case/next", form);
		ParsearRawJsonACasos(ConexionHandler.download);
	}

	/// <summary>
	/// Carga en la lista todos los casos que encuentre en la respuesta del servidor
	/// </summary>
	private static void ParsearRawJsonACasos(string download)
	{
		string json = ConexionHandler.ExtraerJson(download);
		if (json == "{}")
		{
			Debug.LogWarning("Ha habido un error en el servidor al pedir los casos :(");
			TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".no_internet_msg_2"));
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
	private static void CargarJsonComoCaso(string json) 
	{
        Caso c = JsonConverter.PasarJsonAObjeto<Caso>(json);
		if (c != null) casosCargados.TryAdd(c.id,c);
    }

	public void QuitarTodos()
	{
		DestruirObjetosCasoMapa();
		casosCargados.Clear();
        QuitarCasoActivo();
	}

	public void DestruirObjetosCasoMapa()
    {
		int n = casosGO.Count;
		foreach(var cm in casosGO.Values)
		{
			Destroy(cm.gameObject);
		}
		casosGO.Clear();
	}

	public static int CasosRestantesEnMapa()
	{
		int r = 0;
		var e = casosGO.Values.GetEnumerator();
		while (e.MoveNext()) if (e.Current != null && e.Current.gameObject.activeSelf) r++;
		return r;
	}


	/// <summary>
	/// Se encarga de calcular las dimensiones del mapa, y de instanciar y colocar los casos en el mapa.<br></br>
	/// Elimina todos los casos anteriores que habían en el mapa en el proceso...
	/// </summary>
    public void PrepararCasosMapa()
	{
		CalcularGridMapa();
        int maxMapCapacity = (int)(gridSize.x * gridSize.y);

        //Inicializando casos en mapa
        DestruirObjetosCasoMapa();
		idCasosNoColocados.Clear();
		foreach (var par in casosCargados)
		{
			if (casosGO.Count >= maxMapCapacity) break;
			InstanciarCasoMapa(par.Value);
		}
    }

	/// <summary>
	/// Prepara el caso "c", lo intenta posicionar y lo guarda en casosGO.
	/// Si no encuentra espacio, no lo posiciona, lo desactiva, y lo envía a la lista de casos no colocados.
	/// </summary>
	private void InstanciarCasoMapa(Caso c)
	{
        GameObject objeto;
        if (c.examen) objeto = Instantiate(casoMapaEPrefab, _map);          // Caso Examen
        else if (c.secundario) objeto = Instantiate(casoMapaSPrefab, _map); // Caso Secundario
        else objeto = Instantiate(casoMapaPrefab, _map);                    // Caso Principal

		//Inicialización
        CasoMapa casoM = objeto.GetComponent<CasoMapa>();
        casoM.CargarDatosCaso(c.id, c.coste);

        //Posicionamiento
        Vector2 gridCasePos = gridSize.HashCaso2Pos(c);
        Vector2 mapCasePos = gridCasePos * _cellSize + offset;

        if (AlreadyACaseThere(mapCasePos)) //Evitar colisión
        {
            if (showMapGrid) Debug.Log("Evitando colisión");
            Vector2 newPos = FindFreeCell(gridSize, gridCasePos, offset);
            if (newPos != -Vector2.one) objeto.transform.localPosition = newPos;
            else //No se pudo colocar en el mapa
            {
                idCasosNoColocados.Add(c.id);
				objeto.SetActive(false);
            }
        }
        else objeto.transform.localPosition = mapCasePos;
        casosGO.Add(c.id, casoM); // IMPORTANTE añadir DESPUÉS de encontrarle una posición
    }
	private void CalcularGridMapa()
	{
        offset = _paddingMapa - _map.sizeDelta / 2;
        paddedSize = _map.sizeDelta - 2 * _paddingMapa;
        gridSize = new(
            Mathf.Max(1f, Mathf.Floor(paddedSize.x / _cellSize.x)),
            Mathf.Max(1f, Mathf.Floor(paddedSize.y / _cellSize.y)));
    }

    public static void SacarCasosDelBanquillo() => Instance.IntentarColorcarCasosNoColocados();
    public void IntentarColorcarCasosNoColocados()
	{
        while (idCasosNoColocados.Count > 0)
		{
			int id = idCasosNoColocados[0];
			CasoMapa casoM = casosGO[id];

            Vector2 gridCasePos = gridSize.HashCaso2Pos(casosCargados[id]);
            Vector2 posCandidate = gridCasePos * _cellSize + offset;
			bool bloqueado = AlreadyACaseThere(posCandidate);
            if (bloqueado) //Evitar superposición
            {
                if (showMapGrid) Debug.Log("Evitando superposición");
                Vector2 newPos = FindFreeCell(gridSize, gridCasePos, offset);
				if (newPos != -Vector2.one) { posCandidate = newPos; bloqueado = false; }
			}

			if(bloqueado) break; //Si no ha conseguido encontrar un hueco, el resto de casos tampoco lo harán...
			else
			{
                idCasosNoColocados.RemoveAt(0);
                casoM.gameObject.SetActive(true);
                casoM.transform.localPosition = posCandidate;
			}
        }
    }
    
	public static void EliminarCaso(int idCaso)
	{
		casosCargados.Remove(idCaso);
		casosGO.Remove(idCaso);
    }

	private Vector2 FindFreeCell(Vector2 gridSize, Vector2 mapOffset, Vector2 wantedCell)
	{
		Vector2 cellPos,propossedCell;
		for(int i = 1; i < gridSize.x; i++)
		{
			propossedCell.x = (wantedCell.x + i) % gridSize.x;
			
			for (int j = 1; j < gridSize.y; j++)
			{
				propossedCell.y = (wantedCell.y + j) % gridSize.y;

				cellPos = propossedCell * _cellSize +mapOffset;
				if (!AlreadyACaseThere(cellPos)) return cellPos;
			}
		}
		return -Vector2.one;
	}
	private bool AlreadyACaseThere(Vector3 pos)
	{
		bool thereIs = false;
		var enumerator = casosGO.Values.GetEnumerator();
		while(enumerator.MoveNext() && !thereIs)
		{
			CasoMapa c = enumerator.Current;
			if (c == null || !c.gameObject.activeSelf) continue;
			thereIs |= c.transform.localPosition == pos;
		}
		return thereIs;
	}

	/// <summary>
	/// (DEPRECATED) Más adelante el objetivo se podrá mirar en el informe del cajón de pistas.
	/// </summary>
	/// <param name="value"></param>
	public static void MostrarObjetivoDeCasoEnPantalla(bool value)
    {
		bool ok = value && CasoActivo != null;
        Instance.textoObjetivoMision.text = "";
        Instance.fondoMapaMejoraVision.gameObject.SetActive(ok);
        if (ok)
		{
			string resumenCasoActual = CasoActivo.resumen;
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
        NConsultasDuranteElCaso = 0;
		UltimoTiempoEmpleado = 0f;
		NRetosCumplidos = 0;
		tiempoInicioCaso = .0f;
		QuitarTodos();
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