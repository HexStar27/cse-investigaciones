/// Esta clase se dedica a pautar los eventos del Gameplay Loop.
/// Al cambiar el estado se está indicando que se está pasando a ese evento del juego.
////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using Hexstar.CSE;

public class GameplayCycle : MonoBehaviour
{
	public static GameplayCycle Instance { get; private set; }

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
		//Creo que voy a tener que hacer esta parte con async y demás, para tener las cosas más en orden
		int dia = ++ResourceManager.Dia;

		if (dia == 1)
		{
			ResourceManager.ConsultasMaximas = 4;
			OperacionesGameplay.Instancia.CargarEventos();
			//Establecer número de agentes al inicial el primer día
			ResourceManager.AgentesDisponibles = ResourceManager.agentesInciales;
		}
		else //El resto de días
		{
			OperacionesGameplay.Instancia.EjecutarEventoAleatorio();
		}

		//MostrarDía => al terminar... => Saludo de NPC
		_dayCounter.InitAnimation(dia - 1, dia);

		//Establecer consultas disponibles
		ResourceManager.ConsultasDisponibles = ResourceManager.ConsultasMaximas;

		//Cargar los casos disponibles
		PuzzleManager.Instance.QuitarTodos();
		PuzzleManager.Instance.LoadCasos(ResourceManager.ConsultasMaximas);
		PuzzleManager.Instance.MostrarCasosEnPantalla();
		
		//Cargar caso examen si necesario
	}

	private void InicioCaso()
	{
		//Cambiar la música (probablemente)

		//Mostrar pistas en cajón de pistas
		CajonPistas.instancia.RellenarCajonConCasoActivo();
	}

	private void FinCaso()
	{
		//Creo que voy a tener que hacer esta parte con async y demás, para tener las cosas más en orden

		//Ver si lo ha completado o no

		//Otorgar efectos correspondientes

		//Rellenar mapa con el caso que falta
		PuzzleManager.Instance.LoadCasos(1);
		PuzzleManager.Instance.MostrarCasosEnPantalla();

		//Iniciar siguiente día si no quedan consultas disponibles después de aplicar efectos
		if(ResourceManager.ConsultasDisponibles == 0)
		{
			SetState(0);
		}
	}

	private void Awake()
	{
		Instance = this;
		_coreBehabiour[0] = InicioDia;
		_coreBehabiour[1] = InicioCaso;
		_coreBehabiour[2] = FinCaso;
	}
}
