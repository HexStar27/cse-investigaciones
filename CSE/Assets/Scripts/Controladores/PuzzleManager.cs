///Esta clase se encarga de todo lo relacionado con los casos del juego

using System.Collections.Generic;
using UnityEngine;
using Hexstar;
using SimpleJSON;
using System.Threading.Tasks;

public class PuzzleManager : MonoBehaviour
{
	public static PuzzleManager Instance { get; private set; }

	public List<Caso> casosCargados = new List<Caso>();
	public List<int> puntuacionesPorCaso = new List<int>();
	private GameObject[] casosGO = new GameObject[0];
	[Header("Info:")]
	public bool casoExamen; //¿Es el caso activo de tipo examen?
	public int casoActivo; //Índice de la lista de casosCargados
	public bool solucionCorrecta = false;

	[SerializeField] RectTransform _map;
	[SerializeField] CasoDescripcion _descriptor;
	[SerializeField] HighScoreTable _hst;

	public GameObject casoMapaPrefab;
	public GameObject casoMapaSPrefab;
	public GameObject casoMapaBPrefab;

	public static async Task PrepararCasosInicioDia(int casosACargar, int thresholdExamen)
    {
		Instance.QuitarTodos();
		await Instance.LoadCasos(casosACargar);
		//Cargar caso examen si necesario
		if (ResourceManager.CasosCompletados >= thresholdExamen) await Instance.LoadCasoExamen();
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
		Instance.LimpiarPantalla();
		await Instance.LoadCasos(casosNuevos);
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
	/// Añade casos a la lista
	/// </summary>
	/// <param name="n"></param>
	public async Task LoadCasos(int n)
	{
		//1º Acceder a servidor pidiendo n casos
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("dif", ResourceManager.DificultadActual);
		form.AddField("casos", n);
		await ConexionHandler.APost(ConexionHandler.baseUrl+"case",form);
		
		//2º Parsear y guardarlos en casosCargados
		ParsearJsonACasos(ConexionHandler.download);
	}

	public async Task LoadCasoEspecifico(int id)
    {
		//1º Acceder a servidor pidiendo el caso 'id'
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("caso", id);
		await ConexionHandler.APost(ConexionHandler.baseUrl + "case/get", form);
		
		//2º Parsear y añadirlo a casosCargados
		ParsearJsonACasos(ConexionHandler.download);
	}

	private void ParsearJsonACasos(string download)
	{
		//2º Parsear datos
		string json = ConexionHandler.ExtraerJson(download);
		if (json == "{}")
		{
			Debug.LogError("Ha habido un error en el servidor al pedir los casos :(");
			return;
		}
		//3º Añadir casos
		JSONNode jNodo = JSON.Parse(download);
		int n = jNodo["res"].Count;
		for (int i = 0; i < n; i++)
		{
			string cs = jNodo["res"][i].ToString();
			Caso c = JsonConverter.PasarJsonAObjeto<Caso>(cs);
			if (c != null) casosCargados.Add(c);
		}
	}

	public async Task LoadCasoExamen()
	{
		//1º Acceder a servidor pidiendo un caso examen según la dificultad actual
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.sessionKEY);
		form.AddField("dif", ResourceManager.DificultadActual);
		await ConexionHandler.APost(ConexionHandler.baseUrl + "case/exam",form);

		//2º Parsear y añadirlo a casosCargados
		ParsearJsonACasos(ConexionHandler.download);
	}

	public void QuitarTodos()
	{
		//Destruye todos los casos en la pantalla
		LimpiarPantalla();
		// Limpia todas las listas y flags
		casosCargados.Clear();
		LimpiarFlags();
	}
	public void QuitarCaso(Caso c)
	{
		int i = casosCargados.IndexOf(c);
		casosCargados.Remove(c);
		if(casosGO[i] != null)
		{
			Destroy(casosGO[i]);
			casosGO[i] = null;
		}
	}
	public void LimpiarPantalla()
    {
		int n = casosGO.Length;
		for (int i = 0; i < n; i++)
		{
			Destroy(casosGO[i]);
		}
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
			if(casosCargados[i].examen) objeto = Instantiate(casoMapaBPrefab, _map);
			else objeto = Instantiate(casoMapaPrefab, _map);
            
			if (objeto.TryGetComponent(out CasoMapa casoM))
            {
                casoM.caso = i;
                casoM.menuHover = _descriptor;
				casoM.CargarDatosCaso();
            }
            //3º Posicionar
            objeto.transform.localPosition = posiciones[i];
            casosGO[i] = objeto;
        }
    }

	private void Awake()
	{
		Instance = this;
		_hst.InitializePrefab();
	}
}
