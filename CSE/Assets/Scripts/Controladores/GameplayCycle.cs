/// Esta clase se dedica a pautar los eventos del Gameplay Loop.
/// Al cambiar el estado se está indicando que se está pasando a ese evento del juego.
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.Events;

public class GameplayCycle : MonoBehaviour
{
	public class CoreEvent : UnityEvent { };
	CoreEvent[] coreBehabiour;
	int estadoActual = 0;
	
	public void SetState(int estado)
	{
		if (estado > 0 && estado < coreBehabiour.Length) {
			estadoActual = estado;
			coreBehabiour[estado].Invoke();
		}
	}

	public int GetState()
	{
		return estadoActual;
	}

	private void InicioDia()
	{
		int dia = ResourceManager.Dia;
		//MostrarDía => al terminar... => Saludo de NPC
		
		//Cargar casos disponibles

		//Establecer número de agentes al inicial el día
		if (dia == 0)
		{
			ResourceManager.AgentesDisponibles = ResourceManager.agentesInciales;
		}
		//Establecer consultas disponibles
		ResourceManager.ConsultasDisponibles = ResourceManager.ConsultasMaximas;
	}

	private void InicioCaso()
	{

	}

	private void FinCaso()
	{

	}

	private void OnEnable()
	{
		coreBehabiour[0].AddListener(InicioDia);
		coreBehabiour[1].AddListener(InicioCaso);
		coreBehabiour[2].AddListener(FinCaso);
	}

	private void OnDisable()
	{
		foreach (var evento in coreBehabiour)
		{
			evento.RemoveAllListeners();
		}
	}
}
