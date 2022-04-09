///Esta clase se encarga de todo lo relacionado con los casos del juego

using System.Collections.Generic;
using UnityEngine;
using Hexstar.CSE;

public class PuzzleManager : MonoBehaviour
{
	public static PuzzleManager Instance { get; private set; }
	public AlmacenDePalabras almacen;

	[SerializeField] List<Caso> casosCargados = new List<Caso>();
	Caso casoExamen;

	public Caso casoActivo;

	[SerializeField] RectTransform _map;
	[SerializeField] CasoDescripcion _descriptor;

	[SerializeField] Sprite spriteCaso;
	[SerializeField] Sprite spriteCasoExamen;

	public GameObject casoMapaPrefab;

	/// <summary>
	/// Añade casos a la lista
	/// </summary>
	/// <param name="n"></param>
	public void LoadCasos(int n)
	{
		//1º Acceder a servidor pidiendo n casos ( usando el ConexionHanlder )
		//2º Parsear datos
		//3º Crear casos
		for(int i = 0; i < n; i++)
		{
			casosCargados.Add(new Caso()); //Esto habrá que cambiarlo para que meta los casos obtenidos.
		}
	}

	public void LoadCasoExamen()
	{
		//1º Acceder a servidor pidiendo un caso examen ( usando el ConexionHanlder ) según la dificultad actual
		//2º Parsear caso
		//3º Crear caso
	}

	public void QuitarTodos()
	{
		casosCargados.Clear();
		casoExamen = null;
	}
	public void QuitarCaso(Caso c)
	{
		casosCargados.Remove(c);
	}

	public void MostrarCasosEnPantalla()
	{
		//1º Genera Posiciones aleatorias dentro del rango de la "pantalla"
		float offset = 40;
		Vector2 size = 0.5f*_map.sizeDelta;
		int n = casosCargados.Count;
		Vector2[] posiciones = new Vector2[n];
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
		}
	}

	private void Awake()
	{
		Instance = this;
	}
}
