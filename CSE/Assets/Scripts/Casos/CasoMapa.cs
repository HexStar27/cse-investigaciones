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

public class CasoMapa : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public int indiceCaso;
	public TextMeshProUGUI coste;

	[Header("Audio clues")]
	[SerializeField] AudioClip audioHover;
	[SerializeField] AudioClip audioSelect, audioError;
	AudioSource speaker; //El AudioSource del padre

    private void Start()
    {
		speaker = transform.parent.GetComponent<AudioSource>();
    }

	public void Seleccionar()
	{
        CasoDescripcion.Instance.LeerCaso(this, PuzzleManager.GetCasoCargado(indiceCaso), indiceCaso);
        CasoDescripcion.Instance.Abrir(true);
        if (speaker != null) speaker.PlayOneShot(audioSelect);
    }

    public void Comprar()
	{
		if(!SePuedeComprar())
		{
			TempMessageController.Instancia.GenerarMensaje("NECESITAS MÁS AGENTES");
			if (speaker != null) speaker.PlayOneShot(audioError);
			XAPI_Builder.CreateStatement_CaseRequest(false);
			return;
		}
		if(ResourceManager.ConsultasDisponibles <= 0)
		{
            TempMessageController.Instancia.GenerarMensaje("NO TE QUEDAN CONSULTAS");
            if (speaker != null) speaker.PlayOneShot(audioError);
            XAPI_Builder.CreateStatement_CaseRequest(false);
            return;
        }
		// Ya hay otro caso activo
		if (GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso)
		{
			TempMessageController.Instancia.GenerarMensaje("SÓLO UN CASO A LA VEZ");
			if (speaker != null) speaker.PlayOneShot(audioError);
            XAPI_Builder.CreateStatement_CaseRequest(false);
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

		HandleResourcesAndGameplay(datosCaso);
    }

	private async void HandleResourcesAndGameplay(Caso datosCaso)
	{
        //Actualizar estado después de compra
        ResourceManager.AgentesDisponibles -= datosCaso.coste;
        await DataUpdater.Instance.ShowAgentesDisponibles();
        CasoDescripcion.Instance.Abrir(false);
		//await animación de imprimir papel de informe?
        PuzzleManager.IniciarStatsCaso(indiceCaso);

        if (datosCaso.secundario == false) ResourceManager.UltimoCasoPrincipalEmpezado = datosCaso.id;
		CarpetaInformesController.Informes.Add(new Informe(datosCaso));
        GameplayCycle.EnqueueState(EstadosDelGameplay.InicioCaso);
        if (speaker != null) speaker.PlayOneShot(audioSelect);
        XAPI_Builder.CreateStatement_CaseRequest(true);
        Destroy(gameObject); //F
    }

	private bool SePuedeComprar()
	{
		Caso c = PuzzleManager.GetCasoCargado(indiceCaso);
		return c.coste <= ResourceManager.AgentesDisponibles;
	}

	public void CargarDatosCaso(int indice)
	{
		indiceCaso = indice;
		Caso c = PuzzleManager.GetCasoCargado(indiceCaso);
		if(indiceCaso < 0) coste.SetText("?");
		else coste.SetText(c.coste.ToString());
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (speaker != null) speaker.PlayOneShot(audioHover);
	}

	public void OnPointerExit(PointerEventData eventData) {}
}
