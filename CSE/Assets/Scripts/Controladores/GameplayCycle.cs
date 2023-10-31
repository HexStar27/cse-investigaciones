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

	[SerializeField] DayCounter _dayCounter;
	[SerializeField] Transform gameOverMenu;
	[SerializeField] AudioSource bgmSource;

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
		if (ResourceManager.Dia == 0) //Inicio del juego
		{
			ResourceManager.ConsultasMaximas = 4;
			//OperacionesGameplay.Instancia.CargarEventos();
			ResourceManager.AgentesDisponibles = ResourceManager.agentesInciales;
		}
		else //El resto de días
		{
			if (ResourceManager.AgentesDisponibles <= 0)
			{
				await GameOver(); //Fin del juego
				return;
			}
			//Comprobar si hay eventos de efecto aplicables en la pila
			await OperacionesGameplay.EjecutarEventoAleatorio(); //Cambiar por lo nuevo
		}

		//Cargar los casos necesarios al iniciar el día
		if (habiaCasosEnArchivoGuardado)
        {
			habiaCasosEnArchivoGuardado = false;
			await PuzzleManager.RecargarCasosDePartidaGuardada(ResourceManager.checkpoint.casosCargados);
        }
        else
        {
			ResourceManager.ConsultasDisponibles = ResourceManager.ConsultasMaximas;
			await PuzzleManager.PrepararCasosInicioDia();
		}
		OperacionesGameplay.ActualizarDC();
	}

	private async Task InicioCaso()
	{
		//TODO: Cambiar la música. Ahora mismo no tengo otra)

		CajonPistas.instancia.RellenarCajonConCasoActivo();
		AlmacenDePalabras.CargarPistasDeCasoActivo();
		PuzzleManager.MostrarObjetivoDeCasoEnPantalla(true);

		await Task.Yield();
	}

	private async Task FinCaso()
	{
		bool completado = PuzzleManager.Instance.solucionCorrecta;

		//Quitar resumen del caso de la pantalla superior
		PuzzleManager.MostrarObjetivoDeCasoEnPantalla(false);

		//Comprobar si hay eventos de efecto aplicables en la pila
		//TODO...

		if (ResourceManager.AgentesDisponibles <= 0) await GameOver();
		else
		{
			//Iniciar siguiente día si no quedan consultas disponibles después de aplicar efectos
			if (ResourceManager.ConsultasDisponibles == 0) SetState(EstadosDelGameplay.FinDia);

			if (completado && PuzzleManager.Instance.casoExamen)
				ResourceManager.DificultadActual++;
		}

		CajonPistas.instancia.VaciarCajon();
		AlmacenDePalabras.palabras[(int)TabType.Pistas] = new List<string>();

		if (!completado) PuzzleManager.LimpiarFlags();
	}

	private async Task FinDia()
    {
		int dia = ++ResourceManager.Dia;

		//MostrarDía
		await _dayCounter.InitAnimation(dia - 1, dia);

		SetState(EstadosDelGameplay.InicioDia);
	}

	public AudioSource Get_BGM_Source() { return bgmSource; }

	public async Task GameOver()
	{
		gameover = true;
		InscryptionLikeCameraState.bypass = true;
		if (bgmSource != null) bgmSource.Stop(); //Cambiar por música de gameover
		if (gameOverMenu != null) gameOverMenu.gameObject.SetActive(true);
		await Task.Yield();
	}

	public void ReintentarDesdePuntoDeGuardado()
    {
		ResourceManager.checkpoint.Cargar();
		GameManager.CargarEscena(2);
	}
	public void VolverAlMenuPrincipal() { GameManager.CargarEscena(1); }

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
