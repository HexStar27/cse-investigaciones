/// Esta clase se dedica a pautar los eventos del Gameplay Loop.
/// Al cambiar el estado se está indicando que se está pasando a ese evento del juego.
////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using Hexstar.CSE;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameplayCycle : MonoBehaviour
{
	public static GameplayCycle Instance { get; private set; }
	public int EstadosPosibles { get; private set; }

	[SerializeField] DayCounter _dayCounter;
	public AlmacenDePalabras almacenPalabras;

	int estadoActual = 0;
	bool gameover = false;

	Queue<int> pilaEstadosSiguientes = new Queue<int>();

	public void SetState(int estado)
	{
		if (estado >= 0 && estado < EstadosPosibles && !gameover) {
			pilaEstadosSiguientes.Enqueue(estado);
		}
	}

	private async Task ApplyState(int e)
	{
		estadoActual = e;
		switch(e)
		{
			case 0: await InicioDia();	break;
			case 1: await InicioCaso();	break;
			case 2: await FinCaso();	break;
		}
	}

	public int GetState()
	{
		return estadoActual;
	}

	private async Task InicioDia()
	{
		int dia = ++ResourceManager.Dia;

		if (dia == 1)
		{
			ResourceManager.ConsultasMaximas = 4;
			//OperacionesGameplay.Instancia.CargarEventos();
			//Establecer número de agentes al inicial el primer día
			ResourceManager.AgentesDisponibles = ResourceManager.agentesInciales;
		}
		else //El resto de días
		{
			if (ResourceManager.AgentesDisponibles <= 0)
			{
				await GameOver(); //Fin del juego
				return;
			}

			//OperacionesGameplay.Instancia.EjecutarEventoAleatorio();
		}

		//MostrarDía => al terminar... => Saludo de NPC
		Task a = _dayCounter.InitAnimation(dia - 1, dia);

		//Establecer consultas disponibles
		ResourceManager.ConsultasDisponibles = ResourceManager.ConsultasMaximas;

		//Cargar los casos disponibles
		PuzzleManager.Instance.QuitarTodos();
		await PuzzleManager.Instance.LoadCasos(ResourceManager.ConsultasMaximas);
		//Cargar caso examen si necesario
		if (ResourceManager.CasosCompletados >= 4 * ResourceManager.DificultadActual)
		{
			await PuzzleManager.Instance.LoadCasoExamen();
		}
		PuzzleManager.Instance.MostrarCasosEnPantalla();

		await Task.WhenAll(a);
	}

	private async Task InicioCaso()
	{
		//Cambiar la música (probablemente)

		//Mostrar pistas en cajón de pistas
		CajonPistas.instancia.RellenarCajonConCasoActivo();
		await Task.Yield();
	}

	private async Task FinCaso()
	{
		//Ver si lo ha completado o no
		bool completado = PuzzleManager.Instance.solucionCorrecta;

		//Otorgar efectos correspondientes

		//Rellenar mapa con el caso que falta
		await PuzzleManager.Instance.LoadCasos(1);

		//Iniciar siguiente día si no quedan consultas disponibles después de aplicar efectos
		if(ResourceManager.ConsultasDisponibles == 0)
		{
			SetState(0);
		}
		else PuzzleManager.Instance.MostrarCasosEnPantalla();

		//Aumentar dificultad si ha completado un caso examen
		if (completado && PuzzleManager.Instance.casoActivo == PuzzleManager.Instance.casoExamen)
		{
			ResourceManager.DificultadActual++;
		}

		//Limpiar cajón de pistas y palabras en el selector
		CajonPistas.instancia.VaciarCajon();
		almacenPalabras.palabras[(int)TabType.Pistas] = new string[0];
		SelectorPalabras.instancia.RellenarSelector();


		PuzzleManager.Instance.casoActivo = null;
	}

	public async Task GameOver()
	{
		gameover = true;
		// TODO!
		//1º Oscurecer Pantalla con animación (como con el cambio de día)
		//2º Mostrar boton de volver al menú y cargar último punto de guardado
		//Ambos botones vuelven a cargar una escena, ya sea la misma u otra diferente.
		await Task.Yield();
	}

	private void Awake()
	{
		if(Instance == null) Instance = this;
		EstadosPosibles = 3;

		BucleExtractor();
	}

	private void Start()
	{
		if (ResourceManager.Dia == 0) SetState(0);
	}

	private void OnEnable()
	{
		ResourceManager.OnOutOfQueries.AddListener(OperacionesGameplay.SinConsultas);
	}
	private void OnDisable()
	{
		ResourceManager.OnOutOfQueries.RemoveListener(OperacionesGameplay.SinConsultas);
	}

	private async void BucleExtractor()
	{
		while(true)
		{
			if(pilaEstadosSiguientes.Count > 0)
			{
				int e = pilaEstadosSiguientes.Dequeue();
				await ApplyState(e);
			}
			else
			{
				await Task.Delay(100);
			}
		}
	}
}
