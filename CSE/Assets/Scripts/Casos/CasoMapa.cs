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
using System.Collections.Generic;

public class CasoMapa : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public int indiceCaso;
	[HideInInspector] public CasoDescripcion menuHover;
	public TextMeshProUGUI coste;

	[Header("Audio clues")]
	[SerializeField] AudioClip audioHover;
	[SerializeField] AudioClip audioSelect, audioError;
	AudioSource speaker; //El AudioSource del padre

    private void Start()
    {
		speaker = transform.parent.GetComponent<AudioSource>();
    }

    public void Comprar()
	{
		if(!SePuedeComprar())
		{
			TempMessageController.Instancia.GenerarMensaje("Necesitas más agentes para desbloquear el caso");
			if (speaker != null) speaker.PlayOneShot(audioError);
			return;
		}
		// Ya hay otro caso activo
		if (GameplayCycle.Instance.GetState() == (int)EstadosDelGameplay.InicioCaso)
		{
			TempMessageController.Instancia.GenerarMensaje("Sólo se puede resolver un caso a la vez");
			if (speaker != null) speaker.PlayOneShot(audioError);
			return;
		}

		Caso datosCaso = PuzzleManager.GetCasoCargado(indiceCaso);
		if (datosCaso.pistas == null) Debug.LogError("El caso asignado a la instancia de CasoMapa no tiene pistas.");
		
		//Cargar pistas
		int n = datosCaso.pistas.Length;
		List<string> palabras = new List<string>();
		for (int i = 0; i < n; i++)
			palabras.Add(datosCaso.pistas[i].palabra);

		AlmacenDePalabras.palabras[(int)TabType.Pistas] = palabras;

		menuHover.Abrir(false);
		//Actualizar estado después de compra
		PuzzleManager.IniciarStatsCaso(indiceCaso);
		ResourceManager.AgentesDisponibles -= datosCaso.coste;
		if (datosCaso.secundario == false) ResourceManager.UltimoCasoPrincipalEmpezado = datosCaso.id;
		GameplayCycle.Instance.SetState(EstadosDelGameplay.InicioCaso);
		if (speaker != null) speaker.PlayOneShot(audioSelect);
		Destroy(gameObject); //F
	}

	private bool SePuedeComprar()
	{
		Caso c = PuzzleManager.GetCasoCargado(indiceCaso);
		return c.coste <= ResourceManager.AgentesDisponibles;
	}

	public void CargarDatosCaso()
	{
		Caso c = PuzzleManager.GetCasoCargado(indiceCaso);
		if(indiceCaso < 0) coste.SetText(99.ToString());
		else coste.SetText(c.coste.ToString());
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		menuHover.LeerCaso(PuzzleManager.GetCasoCargado(indiceCaso),indiceCaso);
		menuHover.Abrir(true);
		if (speaker != null) speaker.PlayOneShot(audioHover);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		menuHover.Abrir(false);
	}
}
