/// Esta clase hace lo siguiente:
/// -Se genera y se muestra en un canvas.
/// -Al pulsarse, calculará si puede ser comprada.
/// -Al pasar el ratón por encima, se mostrará el título, un mini resumen del caso y los posibles efectos que contiene.
/// -Si se compra, desaparece, se pasan las pistas al almacén de palabras, y se le dice al GameplayCycle y al PuzzleManager 
/// que se ha empezado un caso.

using UnityEngine;
using Hexstar.CSE;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CasoMapa : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Caso caso;
	[HideInInspector] public CasoDescripcion menuHover;
	[HideInInspector] public AlmacenDePalabras almacenPalabras;
	public TextMeshProUGUI coste;

	public void Comprar()
	{
		if(SePuedeComprar())
		{
			int p = (int)TabType.Pistas;
			int n = caso.pistas.Length;
			int m = almacenPalabras.palabras[p].Length;
			string[] palabras = new string[n+m];
			for(int i = 0; i < n; i++)
			{
				palabras[i] = caso.pistas[i].palabra;
			}
			almacenPalabras.palabras[p].CopyTo(palabras, n);
			almacenPalabras.palabras[p] = palabras;	

			menuHover.Abrir(false);
			PuzzleManager.Instance.casoActivo = caso;
			ResourceManager.AgentesDisponibles -= caso.coste;
			GameplayCycle.Instance.SetState(1); // Inicio Caso

			Destroy(gameObject);
		}
		else
		{
			Debug.Log("No se puede :(");
			// Todo: Efecto de sonido o algo indicando que no puede hacer la acción.
		}
	}

	private bool SePuedeComprar()
	{
		return caso.coste <= ResourceManager.AgentesDisponibles;
	}

	public void EstablecerSprite(Sprite sprite)
	{
		GetComponent<Image>().sprite = sprite;
		coste.SetText(caso.coste.ToString());
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		menuHover.LeerCaso(caso);
		menuHover.Abrir(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		menuHover.Abrir(false);
	}
}
