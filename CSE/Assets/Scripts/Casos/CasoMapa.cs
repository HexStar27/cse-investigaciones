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

public class CasoMapa : MonoBehaviour
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
			int n = almacenPalabras.palabras[p].Length;
			caso.pistas.CopyTo(almacenPalabras.palabras[p], n);

			PuzzleManager.Instance.casoActivo = caso;
			GameplayCycle.Instance.SetState(1); // Inicio Caso

			Destroy(gameObject);
		}
		else
		{
			// Todo: Efecto de sonido o algo indicando que no puede hacer la acción.
		}
	}

	private bool SePuedeComprar()
	{
		return caso.coste <= ResourceManager.AgentesDisponibles;
	}

	private void OnMouseEnter()
	{
		Debug.Log("Ekis");
		menuHover.LeerCaso(caso);
		menuHover.Abrir(true);
	}
	private void OnMouseExit()
	{
		menuHover.Abrir(false);
		Debug.Log("De");
	}
	public void EstablecerSprite(Sprite sprite)
	{
		GetComponent<Image>().sprite = sprite;
		coste.SetText(caso.coste.ToString());
	}
}
