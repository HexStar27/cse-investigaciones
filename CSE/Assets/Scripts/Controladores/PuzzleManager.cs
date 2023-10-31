///Esta clase se encarga de todo lo relacionado con los casos del juego

using System.Collections.Generic;
using UnityEngine;
using Hexstar;
using SimpleJSON;
using System.Threading.Tasks;

public class PuzzleManager : MonoBehaviour
{
	private static readonly int primerCasoPrincipalDelJuego = 1;
	public static int consultasRealizadasActuales = 0;
	private static float tiempoEmpleado = .0f;

	public static PuzzleManager Instance { get; private set; }

	public List<Caso> casosCargados = new List<Caso>();
	public List<int> puntuacionesPorCaso = new List<int>();
	private GameObject[] casosGO = new GameObject[0];
	[Header("Info:")]
	public bool casoExamen; //"Es caso activo de tipo examen?"
	public int casoActivo; //Índice de la lista de casosCargados
	public bool solucionCorrecta = false;

	[SerializeField] RectTransform _map;
	[SerializeField] TMPro.TextMeshProUGUI textoObjetivoMision;
	[SerializeField] CasoDescripcion _descriptor;
	[SerializeField] HighScoreTable _hst;

	public GameObject casoMapaPrefab;
	public GameObject casoMapaSPrefab;
	public GameObject casoMapaBPrefab;

	public static void IniciarStatsCaso(int indiceCaso)
    {
		Instance.casoActivo = indiceCaso;
		consultasRealizadasActuales = 0;
		tiempoEmpleado = Time.realtimeSinceStartup;
    }
	public static float GetTiempoEmpleado() { return Time.realtimeSinceStartup - tiempoEmpleado; }
	public static void TerminarStatsCaso() { tiempoEmpleado -= Time.realtimeSinceStartup; }

	public static async Task PrepararCasosInicioDia( bool primerDia = false)
    {
		Instance.QuitarTodos();

		int ultimoCaso = ResourceManager.UltimoCasoPrincipalEmpezado;
		if (primerDia || ultimoCaso < 0) await Instance.LoadCasoEspecifico(primerCasoPrincipalDelJuego);
		else await Instance.LoadSiguientesPrincipales();

		int huecosLibres = ResourceManager.ConsultasMaximas - Instance.casosCargados.Count;
		if (huecosLibres < 0) huecosLibres = 0;
		await Instance.LoadCasosSecundarios(huecosLibres);
		
		Instance.MostrarCasosEnPantalla();
		ObtenerPuntuacionesDeCasosCargados();
	}

	public static async Task RecargarCasosDePartidaGuardada(int[] listaIDs)
    {
		Instance.QuitarTodos();
		for (int i = 0; i < listaIDs.Length; i++) 
			await Instance.LoadCasoEspecifico(listaIDs[i]);
		Instance.MostrarCasosEnPantalla();
		ObtenerPuntuacionesDeCasosCargados();
	}

	public static async Task RellenarCasoFinCaso(int casosNuevos)
    {
		Instance.LimpiarMapaDeCasos();
		await Instance.LoadCasosSecundarios(casosNuevos);
		Instance.MostrarCasosEnPantalla();
		ObtenerPuntuacionesDeCasosCargados(true);
	}

	private static async void ObtenerPuntuacionesDeCasosCargados(bool dontClearJustAdd = false)
    {
		if(!dontClearJustAdd)
        {
			Instance._hst.DeleteElements();
			Instance.puntuacionesPorCaso.Clear();
		}
		foreach(var c in Instance.casosCargados)
        {
			Instance._hst.SetCasoID(c.id);
			await Instance._hst.SetupScore(false);
			Instance.puntuacionesPorCaso.Add(Instance._hst.elements.Count);
		}
    }

	public static void LimpiarFlags()
    {
		Instance.casoActivo = -1;
		Instance.casoExamen = false;
		Instance.solucionCorrecta = false;
    }


	public static Caso GetCasoCargado(int idx) { return Instance.casosCargados[idx]; }
	public static Caso GetCasoActivo()
    {
		if (Instance.casoActivo < 0) return null;
		return Instance.casosCargados[Instance.casoActivo];
    }

	/// <summary>
	/// Añade N casos secundarios a la lista
	/// </summary>
	/// <param name="n"></param>
	public async Task LoadCasosSecundarios(int n)
	{
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("dif", ResourceManager.DificultadActual);
		form.AddField("casos", n);
		await ConexionHandler.APost(ConexionHandler.baseUrl+"case",form);
		
		ParsearJsonACasos(ConexionHandler.download);
	}

	/// <summary>
	/// Añade el caso con la id especificada a la lista
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public async Task LoadCasoEspecifico(int id)
    {
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("caso", id);
		await ConexionHandler.APost(ConexionHandler.baseUrl + "case/get", form);
		
		ParsearJsonACasos(ConexionHandler.download);
	}

	/// <summary>
	/// Añade el (o los) siguientes casos principales a la lista
	/// </summary>
	/// <returns></returns>
	public async Task LoadSiguientesPrincipales()
	{
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("id", ResourceManager.UltimoCasoPrincipalEmpezado);
		form.AddField("win", ResourceManager.UltimoCasoPrincipalGanado ? 1 : 0);
		await ConexionHandler.APost(ConexionHandler.baseUrl + "case/get", form);

		ParsearJsonACasos(ConexionHandler.download);
	}

	///(Obsoleto) La función "LoadSiguientesPrincipales" es mejor
	public async Task LoadCasoExamen()
	{
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("dif", ResourceManager.DificultadActual);
		await ConexionHandler.APost(ConexionHandler.baseUrl + "case/exam", form);

		ParsearJsonACasos(ConexionHandler.download);
	}

	private void ParsearJsonACasos(string download)
	{
		//1º Parsear datos
		string json = ConexionHandler.ExtraerJson(download);
		if (json == "{}")
		{
			Debug.LogError("Ha habido un error en el servidor al pedir los casos :(");
			return;
		}
		//2º Añadir casos
		JSONNode jNodo = JSON.Parse(download);
		int n = jNodo["res"].Count;
		for (int i = 0; i < n; i++)
		{
			string cs = jNodo["res"][i].ToString();
			Caso c = JsonConverter.PasarJsonAObjeto<Caso>(cs);
			if (c != null) casosCargados.Add(c);
		}
	}

	public void QuitarTodos()
	{
		LimpiarMapaDeCasos();
		casosCargados.Clear();
		LimpiarFlags();
	}

	public void LimpiarMapaDeCasos()
    {
		int n = casosGO.Length;
		for (int i = 0; i < n; i++)
		{
			Destroy(casosGO[i]);
		}
	}

	public bool HayAlMenos1CasoPrincipal()
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
		//1º Genera Posiciones aleatorias dentro del rango de la "pantalla"
		float offset = 40;
		Vector2 size = 0.5f*_map.sizeDelta;
		int n = casosCargados.Count;
		Vector2[] posiciones = new Vector2[n];
		casosGO = new GameObject[n];

		for (int i = 0; i < n; i++) // Precalcular posiciones
		{
			posiciones[i].x = UnityEngine.Random.Range(offset - size.x, size.x - 2*offset);
			posiciones[i].y = UnityEngine.Random.Range(2*offset - size.y, size.y - offset);
		}

		//2º Inicializar CasosMapa (1 por caso almacenado)
		GameObject objeto;
        for (int i = 0; i < n; i++)
        {
			if (casosCargados[i].examen) objeto = Instantiate(casoMapaBPrefab, _map);
			else if (casosCargados[i].secundario) objeto = Instantiate(casoMapaSPrefab, _map);
			else objeto = Instantiate(casoMapaPrefab, _map);
            
			if (objeto.TryGetComponent(out CasoMapa casoM))
            {
                casoM.indiceCaso = i;
                casoM.menuHover = _descriptor;
				casoM.CargarDatosCaso();
            }
            //3º Posicionar
            objeto.transform.localPosition = posiciones[i];
            casosGO[i] = objeto;
        }
    }

	public static void MostrarObjetivoDeCasoEnPantalla(bool value)
    {
		if (value && Instance.casoActivo >= 0)
		{
			Instance.textoObjetivoMision.text = Instance.casosCargados[Instance.casoActivo].resumen;
		}
		else Instance.textoObjetivoMision.text = "";
    }

	private void Awake()
	{
		Instance = this;
		_hst.InitializePrefab();
	}
}
