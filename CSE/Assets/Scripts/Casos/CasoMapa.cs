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

	private static readonly int casoEmpezado = 1;

	public void Comprar()
	{
		if(!SePuedeComprar())
		{
			TempMessageController.Instancia.GenerarMensaje("Necesitas más agentes para desbloquear el caso");
			// Todo: Efecto de sonido
			return;
		}

		if (GameplayCycle.Instance.GetState() == casoEmpezado) // Ya hay otro caso activo
		{
			TempMessageController.Instancia.GenerarMensaje("Sólo se puede resolver un caso a la vez");
			// Todo: Efecto de sonido
			return;
		}

		int p = (int)TabType.Pistas;
		if (caso.pistas == null) Debug.LogError("El caso asignado a la instancia de CasoMapa no tiene pistas.");
		int n = caso.pistas.Length;
		string[] palabras = new string[n];
		for (int i = 0; i < n; i++)
		{
			palabras[i] = caso.pistas[i].palabra;
		}
		almacenPalabras.palabras[p] = palabras;

		menuHover.Abrir(false);
		PuzzleManager.Instance.casoActivo = caso;
		ResourceManager.AgentesDisponibles -= caso.coste;
		GameplayCycle.Instance.SetState(1); // Inicio Caso

		Destroy(gameObject);
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
