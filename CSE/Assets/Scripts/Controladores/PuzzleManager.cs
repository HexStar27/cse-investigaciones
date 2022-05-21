///Esta clase se encarga de todo lo relacionado con los casos del juego

using System.Collections.Generic;
using UnityEngine;
using Hexstar.CSE;
using Hexstar;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class PuzzleManager : MonoBehaviour
{
	public static PuzzleManager Instance { get; private set; }
	public AlmacenDePalabras almacen;

	[SerializeField] List<Caso> casosCargados = new List<Caso>();
	private GameObject[] casosGO = new GameObject[0];
	public Caso casoExamen;

	public Caso casoActivo;
	public bool solucionCorrecta = false;

	[SerializeField] RectTransform _map;
	[SerializeField] CasoDescripcion _descriptor;

	[SerializeField] Sprite spriteCaso;
	[SerializeField] Sprite spriteCasoExamen;

	public GameObject casoMapaPrefab;

	/// <summary>
	/// Añade casos a la lista
	/// </summary>
	/// <param name="n"></param>
	public async Task LoadCasos(int n)
	{
		//1º Acceder a servidor pidiendo n casos ( usando el ConexionHanlder )
		ConexionHandler.onFinishRequest.AddListener(ParsearJsonACasos);
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.KEY);
		form.AddField("dif", ResourceManager.DificultadActual);
		form.AddField("casos", n);

		await ConexionHandler.APost(ConexionHandler.baseUrl+"case",form);
	}

	private void ParsearJsonACasos(DownloadHandler download)
	{
		//2º Parsear datos
		string json = ConexionHandler.ExtraerJson(download.text);
		json = json.Substring(1, json.Length - 2);
		var cads = json.Split('#');
		
		List<Caso> casos = new List<Caso>();
		foreach (var t in cads)
		{
			Caso c = JsonConverter.PasarJsonAObjeto<Caso>(t);
			casos.Add(c);
		}
		
		//3º Crear casos
		int n = casos.Count;
		for (int i = 0; i < n; i++)
		{
			casosCargados.Add(casos[i]);
		}
		ConexionHandler.onFinishRequest.RemoveListener(ParsearJsonACasos);

		//MostrarCasosEnPantalla();
	}

	public async Task LoadCasoExamen()
	{
		//1º Acceder a servidor pidiendo un caso examen ( usando el ConexionHanlder ) según la dificultad actual
		ConexionHandler.onFinishRequest.AddListener(ParsearJsonACasos);
		WWWForm form = new WWWForm();
		form.AddField("authorization", SesionHandler.KEY);
		form.AddField("dif", ResourceManager.DificultadActual);

		await ConexionHandler.APost(ConexionHandler.baseUrl + "case/exam",form);
	}

	public void QuitarTodos()
	{
		int n = casosGO.Length;
		for(int i = 0; i < n; i++)
		{
			Destroy(casosGO[i]);
		}
		casosCargados.Clear();
		casoExamen = null;
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

	public void MostrarCasosEnPantalla()
	{
		//1º Genera Posiciones aleatorias dentro del rango de la "pantalla"
		float offset = 40;
		Vector2 size = 0.5f*_map.sizeDelta;
		int n = casosCargados.Count;
		Vector2[] posiciones = new Vector2[n];

		int N = n + (casoExamen!=null?1:0);
		casosGO = new GameObject[N];

		for (int i = 0; i < n; i++)
		{
			posiciones[i].x = UnityEngine.Random.Range(offset - size.x, size.x - 2*offset);
			posiciones[i].y = UnityEngine.Random.Range(2*offset - size.y, size.y - offset);
		}
		//2º Inicializar CasosMapa (1 por caso almacenado)
		if(casoExamen != null)
		{
			GameObject o = Instantiate(casoMapaPrefab, _map);
			CasoMapa cm = o.GetComponent<CasoMapa>();
			cm.caso = casoExamen;
			cm.almacenPalabras = almacen;
			cm.menuHover = _descriptor;
			cm.EstablecerSprite(spriteCasoExamen);
			Vector2 p = new Vector2( UnityEngine.Random.Range(offset, size.x - offset), 
									UnityEngine.Random.Range(offset, size.y - offset));
			//3º Posicionar
			o.transform.localPosition = p;
			casosGO[n] = o;
		}
		GameObject objeto;
		CasoMapa casoM;
		for(int i = 0; i < n; i++)
		{
			objeto = Instantiate(casoMapaPrefab, _map);
			casoM = objeto.GetComponent<CasoMapa>();
			casoM.caso = casosCargados[i];
			casoM.almacenPalabras = almacen;
			casoM.menuHover = _descriptor;
			casoM.EstablecerSprite(spriteCaso);
			//3º Posicionar
			objeto.transform.localPosition = posiciones[i];
			casosGO[i] = objeto;
		}
	}

	private void Awake()
	{
		Instance = this;
	}
}
