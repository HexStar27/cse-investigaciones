/// Esta clase se dedica a pautar los eventos del Gameplay Loop.
/// Al cambiar el estado se está indicando que se está pasando a ese evento del juego.
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using Hexstar.CSE;
using System.Collections.Generic;
using System.Threading.Tasks;

[System.Serializable]
public enum EstadosDelGameplay { InicioDia = 0, InicioCaso = 1, FinCaso = 2, FinDia = 3 };
public class GameplayCycle : MonoBehaviour
{
	public static GameplayCycle Instance { get; private set; }
	public int EstadosPosibles { get; private set; }
	private static readonly int multiplicadorExamenes = 3;

	[SerializeField] DayCounter _dayCounter;

	[Header("Status")]
	[SerializeField] int estadoActual = 0;
	[SerializeField] bool gameover = false;
	bool habiaCasosEnArchivoGuardado = false;

	Queue<int> pilaEstadosSiguientes = new Queue<int>();

	public void SetState(EstadosDelGameplay estado)
	{
		if (gameover) return;
		pilaEstadosSiguientes.Enqueue((int)estado);
	}
	public void SetState(int estado)
	{
		//El estado existe y no está activada la flag de fin de juego
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
			case 3: await FinDia();		break;
		}
	}

	public int GetState()
	{
		return estadoActual;
	}

	private async Task InicioDia()
	{
		if (ResourceManager.Dia == 0)
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
			await OperacionesGameplay.EjecutarEventoAleatorio();
		}

		//Establecer consultas disponibles y cargar los casos necesarios al iniciar el día
		if (habiaCasosEnArchivoGuardado)
        {
			habiaCasosEnArchivoGuardado = false;
			await PuzzleManager.RecargarCasosDePartidaGuardada(ResourceManager.checkpoint.casosCargados);
        }
        else
        {
			ResourceManager.ConsultasDisponibles = ResourceManager.ConsultasMaximas;
			await PuzzleManager.PrepararCasosInicioDia(ResourceManager.ConsultasMaximas,
			multiplicadorExamenes * ResourceManager.DificultadActual);
		}
		OperacionesGameplay.ActualizarDC();
	}

	private async Task InicioCaso()
	{
		//Cambiar la música (probablemente)

		//Mostrar pistas en cajón de pistas
		CajonPistas.instancia.RellenarCajonConCasoActivo();
		//Introducir pistas en almacén de bloques
		AlmacenDePalabras.CargarPistasDeCasoActivo();
		//Introducir las palabras del almacén al selector
		SelectorPalabras.instancia.RellenarSelector();
		await Task.Yield();
	}

	private async Task FinCaso()
	{
		//Ver si lo ha completado o no
		bool completado = PuzzleManager.Instance.solucionCorrecta;

		//Otorgar efectos correspondientes
		//TODO...

		//Iniciar siguiente día si no quedan consultas disponibles después de aplicar efectos,
		if (ResourceManager.ConsultasDisponibles == 0) SetState(EstadosDelGameplay.FinDia);
		else await PuzzleManager.RellenarCasoFinCaso(1);

		//Aumentar dificultad si ha completado un caso examen
		if (completado && PuzzleManager.Instance.casoExamen)
		{
			ResourceManager.DificultadActual++;
		}

		//Limpiar cajón de pistas y palabras en el selector
		CajonPistas.instancia.VaciarCajon();
		AlmacenDePalabras.palabras[(int)TabType.Pistas] = new List<string>();
		SelectorPalabras.instancia.RellenarSelector();

		PuzzleManager.LimpiarFlags();
	}

	private async Task FinDia()
    {
		int dia = ++ResourceManager.Dia;

		//MostrarDía
		await _dayCounter.InitAnimation(dia - 1, dia);

		SetState(EstadosDelGameplay.InicioDia);
	}

	public async Task GameOver()
	{
		gameover = true;
		// TODO!
		//1º Oscurecer Pantalla con animación (como con el cambio de día)
		//2º Mostrar boton de volver al menú y cargar último punto de guardado
		//Ambos botones vuelven a cargar una escena, ya sea la misma u otra diferente.
		if (TempMessageController.Instancia != null) 
			TempMessageController.Instancia.GenerarMensaje("Game Over (Debug)");
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
		InicializacionDeSubSistemas();
		if (ResourceManager.checkpoint.casoEnCurso < 0) SetState(EstadosDelGameplay.InicioDia);
		else SetState(EstadosDelGameplay.InicioCaso);

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
		while(!gameover)
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


	private void InicializacionDeSubSistemas()
    {
		AlmacenDePalabras.CargarPalabras();
		habiaCasosEnArchivoGuardado = ResourceManager.checkpoint.casosCargados.Length > 0;
		PuzzleManager.Instance.casoActivo = ResourceManager.checkpoint.casoEnCurso;
	}
}
