/// Esta clase se dedica a pautar los eventos del Gameplay Loop.
/// Al cambiar el estado se está indicando que se está pasando a ese evento del juego.
////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

public class GameplayCycle : MonoBehaviour
{
	[SerializeField] DayCounter _dayCounter;

	Action[] _coreBehabiour = new Action[3];
	int estadoActual = 0;
	
	public void SetState(int estado)
	{
		if (estado >= 0 && estado < _coreBehabiour.Length) {
			estadoActual = estado;
			_coreBehabiour[estado]();
		}
	}

	public int GetState()
	{
		return estadoActual;
	}

	private void InicioDia()
	{
		int dia = ++ResourceManager.Dia;
		//MostrarDía => al terminar... => Saludo de NPC
		_dayCounter.InitAnimation(dia-1,dia);
		//Cargar casos disponibles

		//Establecer número de agentes al inicial el primer día
		if (dia == 1)
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

	private void Awake()
	{
		_coreBehabiour[0] = InicioDia;
		_coreBehabiour[1] = InicioCaso;
		_coreBehabiour[2] = FinCaso;
	}
}
