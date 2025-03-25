/// Esta clase hace lo siguiente:
/// -Se genera y se muestra en un canvas.
/// -Al pulsarse, calculará si puede ser comprada.
/// -Al pasar el ratón por encima, se mostrará el título, un mini resumen del caso y los posibles efectos que contiene.
/// -Si se compra, desaparece, se pasan las pistas al almacén de palabras, y se le dice al GameplayCycle y al PuzzleManager 
/// que se ha empezado un caso.

using UnityEngine;
using Hexstar.CSE;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using CSE;
using Hexstar.CSE.Informes;
using CSE.Local;
using System.Collections;

public class CasoMapa : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public int idCaso;
	public TextMeshProUGUI costeTM;

	[Header("Audio clues")]
	[SerializeField] AudioClip audioHover;
	[SerializeField] AudioClip audioSelect, audioError;
	AudioSource speaker; //El AudioSource del padre

    private void Start()
    {
		speaker = transform.parent.GetComponent<AudioSource>();
		StartCoroutine(Spawn());
    }

	public void Seleccionar()
	{
        CasoDescripcion.Instance.LeerCaso(this, PuzzleManager.GetCasoCargado(idCaso), idCaso);
        CasoDescripcion.Instance.Abrir(true);
        if (speaker != null) speaker.PlayOneShot(audioSelect);
    }

    public void Comprar()
	{
		if (Caso.CheckingBounties()) return;
		if(!SePuedeComprar())
		{
			TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".caso.necesitas_egentes"));
			if (speaker != null) speaker.PlayOneShot(audioError);
			XAPI_Builder.CreateStatement_CaseRequest(false);
			return;
		}
		if(ResourceManager.ConsultasDisponibles <= 0)
		{
            TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".caso.no_contulas"));
            if (speaker != null) speaker.PlayOneShot(audioError);
            XAPI_Builder.CreateStatement_CaseRequest(false);
            return;
        }
		// Ya hay otro caso activo
		if (GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso)
		{
			TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".caso.ya_hay_caso_activo"));
			if (speaker != null) speaker.PlayOneShot(audioError);
            XAPI_Builder.CreateStatement_CaseRequest(false);
            return;
		}

		Caso datosCaso = PuzzleManager.GetCasoCargado(idCaso);
		if (datosCaso.pistas == null) Debug.LogError("El caso asignado a la instancia de CasoMapa no tiene pistas.");
		
		//Cargar pistas
		int n = datosCaso.pistas.Length;
		List<string> palabras = new List<string>();
		for (int i = 0; i < n; i++)
			palabras.AddRange(datosCaso.pistas[i].palabras);

		AlmacenDePalabras.palabras[(int)TabType.Pistas] = palabras;

		HandleResourcesAndGameplay(datosCaso);
    }

	/// <summary>
	/// El caso se ha comprado y hay que avisar a to kiski para que hagan sus limpiezas
	/// </summary>
	private async void HandleResourcesAndGameplay(Caso datosCaso)
	{
        //Actualizar estado después de compra
        ResourceManager.AgentesDisponibles -= datosCaso.coste;
        await DataUpdater.Instance.ShowAgentesDisponibles();
		CasoDescripcion.Instance.Abrir(false);
        
		PuzzleManager.IniciarStatsCaso(idCaso);
		CarpetaInformesController.Informes.Add(new Informe(datosCaso));
		GameplayCycle.EnqueueState(EstadosDelGameplay.InicioCaso);
        
		if (speaker != null) speaker.PlayOneShot(audioSelect);
        XAPI_Builder.CreateStatement_CaseRequest(true);
		
		PuzzleManager.EliminarCaso(idCaso);
        Destroy(gameObject); //F
    }

	private bool SePuedeComprar()
	{
		Caso c = PuzzleManager.GetCasoCargado(idCaso);
		return c.coste <= ResourceManager.AgentesDisponibles;
	}

	public void CargarDatosCaso(int id, int coste)
	{
		idCaso = id;
		if(idCaso < 0) costeTM.SetText("?");
		else costeTM.SetText(coste.ToString());
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (speaker != null) speaker.PlayOneShot(audioHover);
	}

	public void OnPointerExit(PointerEventData eventData) {}

	private IEnumerator Spawn()
	{
		var anim = GetComponent<Animator>();
		yield return new WaitForSeconds(Random.Range(0,0.5f));
		anim.Play("aparecer");
    }
}
