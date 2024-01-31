/// Esta clase se dedica a pautar los eventos del Gameplay Loop.
/// Al cambiar el estado se está indicando que se está pasando a ese evento del juego.
////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using CSE;
using Hexstar.CSE;
using Hexstar.Dialogue;
using Hexstar.CSE.SistemaEventos;
using CSE.Feedback;

[System.Serializable] public enum EstadosDelGameplay { InicioDia = 0, InicioCaso = 1, FinCaso = 2, FinDia = 3 };
public class GameplayCycle : MonoBehaviour, ISingleton
{
	public static GameplayCycle Instance { get; private set; }

	public int EstadosPosibles { get; private set; }

	[SerializeField] DayCounter _dayCounter;
	[SerializeField] Transform gameOverMenu;
	[SerializeField] AudioSource bgmSource;
	[SerializeField] Hexstar.ControladorCinematica controladorCinematica;
	[SerializeField] AwaiterUntilPressed popupFinDia;

	static int estadoActual = 0;
	static bool gameover = false;
	static bool terminarBucleExtractor = false;
	static bool recargarCasosDePartidaAnterior = false;

	public static UnityEvent OnCycleTaskFinished { get; set; } = new();
	private static Queue<int> pilaEstadosSiguientes = new Queue<int>();

	private static Dictionary<string, bool> pauseMessageDict = new();

	private int startedCasesInDay = 0, completedCasesInDay = 0;

	public static void EnqueueState(EstadosDelGameplay estado)
	{
		if (gameover) return;
		pilaEstadosSiguientes.Enqueue((int)estado);
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

	public static int GetState()
	{
		return estadoActual;
	}

	private async Task InicioDia()
	{
        startedCasesInDay = 0;
        completedCasesInDay = 0;
        PuzzleManager.MostrarObjetivoDeCasoEnPantalla(false);

        if (ResourceManager.Dia == 0) //Inicio del juego
		{
			ResourceManager.ConsultasMaximas = 4;

			if (!TutorialIsEnabled())
			{
				SetTutorialEvent(true);
				controladorCinematica.CargarJSON();
                controladorCinematica.IniciarCinematica();
			}
			ResourceManager.AgentesDisponibles = ResourceManager.agentesInciales;
            DataUpdater.Instance.ResetSingleton();
        }
		else //El resto de días
		{
			await AlmacenEventos.EncargarseDeEventosAptos();

            if (ResourceManager.AgentesDisponibles <= 0 || CheckGameOverEvent())
			{
				await GameOver();
				return;
			}
		}

        // Carga los casos necesarios al iniciar el día
        await PuzzleManager.PrepararCasosParaInicioDia(recargarCasosDePartidaAnterior);

        if (recargarCasosDePartidaAnterior) recargarCasosDePartidaAnterior = false;
		else
		{
			ResourceManager.ConsultasDisponibles = ResourceManager.ConsultasMaximas;
			if (ResourceManager.Dia > 0) await DataUpdater.Instance.ShowAgentesDisponibles();
        }

        OperacionesGameplay.ActualizarDangerController();
	}

	private async Task InicioCaso()
	{
		startedCasesInDay++;
        //TODO: Cambiar la música. Ahora mismo no tengo otra

        CajonPistas.instancia.RellenarCajonConCasoActivo();
		AlmacenDePalabras.CargarPistasDeCasoActivo();
		PuzzleManager.MostrarObjetivoDeCasoEnPantalla(true);

		await Task.Yield();
	}

	private async Task FinCaso()
	{
		int nConsultas = PuzzleManager.ConsultasRealizadasActuales;
		float tiempo = PuzzleManager.UltimoTiempoEmpleado;
        bool ganado = PuzzleManager.SolucionCorrecta;
		if (ganado) completedCasesInDay++;

		PuzzleManager.MostrarObjetivoDeCasoEnPantalla(false);
		await PuzzleManager.GetCasoActivo().ComprobarYAplicarBounties(ganado,nConsultas,tiempo);

		await AlmacenEventos.EncargarseDeEventosAptos();

		if (ResourceManager.AgentesDisponibles <= 0 || CheckGameOverEvent()) await GameOver();
		else
		{
			if (ResourceManager.ConsultasDisponibles == 0)
			{
				EnqueueState(EstadosDelGameplay.FinDia);

				//Esperar confirmación del jugador para continuar
				if (popupFinDia != null) popupFinDia.ActivarPopUp();

				XAPI_Builder.CreateStatement_DayFinished(!ganado, completedCasesInDay, startedCasesInDay);
			}

			if (ganado && PuzzleManager.CasoActivoEsExamen())
			{
				ResourceManager.DificultadActual++;
				TempMessageController.Instancia.GenerarMensaje("LA DIFICULTAD HA SIDO AUMENTADA");
				XAPI_Builder.CreateStatement_DifficultyIncrease(ResourceManager.DificultadActual);
			}
		}

        //Elimina las pistas del caso
        CajonPistas.instancia.VaciarCajon();
		AlmacenDePalabras.palabras[(int)TabType.Pistas].Clear();

        PuzzleManager.LimpiarFlagsDeCasoActual();
	}

	private async Task FinDia()
    {
		//MostrarDía
        int dia = ++ResourceManager.Dia;
		await _dayCounter.InitAnimation(dia - 1, dia);

		//Limpieza
		RestartCameraState();
		Intelisense.instance.pantalla.text ="";
		BloqueTabletaElemento.DestruirBloquesEnMesa();

        EnqueueState(EstadosDelGameplay.InicioDia);
	}

	private void RestartCameraState()
	{
		var mainCam = Camera.main;
        var ilcs = mainCam.GetComponent<InscryptionLikeCameraState>();
		var cs = mainCam.GetComponent<CameraState>();
		ilcs.SetCameraState(cs);
		ilcs.SetEstadoActual(0);
		cs.Transition(0);
	}

	public AudioSource Get_BGM_Source() { return bgmSource; }

	public async Task GameOver()
	{
		//Debería ser el fin del juego cuando 
		gameover = true;
        InscryptionLikeCameraState.SetBypass(true);
        if (bgmSource != null) bgmSource.Stop(); //Cambiar por música de gameover
		if (gameOverMenu != null) gameOverMenu.gameObject.SetActive(true);
		await Task.Yield();
	}

	private static bool CheckGameOverEvent()
	{
		string  v = ControladorDialogos.GetDialogueEventValue("game_over");
		if(int.TryParse(v,System.Globalization.NumberStyles.Integer, 
			new System.Globalization.CultureInfo("es-ES"),out int res))
		{
			return res > 0; // Evento activado => Ejecutar Game Over cuando sea posible.
		}
		return false;
	}

	public void ReintentarDesdePuntoDeGuardado()
    {
		ResourceManager.checkpoint.CargarDatosAlSistema();
		GameManager.CargarEscena(GameManager.GameScene.ESCENA_PRINCIPAL);
	}
	public void VolverAlMenuPrincipal() { 
		GameManager.CargarEscena(GameManager.GameScene.MENU_PARTIDA); 
	}

	private void SetTutorialEvent(bool enabled)
	{
		ControladorDialogos.SetDialogueEvent("tutorial", enabled ? "true" : "false");
	}
    private bool TutorialIsEnabled()
    {
        string v = ControladorDialogos.GetDialogueEventValue("tutorial");
        return v.Equals("true");
    }

    private void Awake()
	{
		if(Instance == null) Instance = this;
		EstadosPosibles = 4;

		BucleExtractor();
	}

	private void Start()
	{
		TryLoadGame();
		if (ResourceManager.checkpoint.casoEnCurso < 0) EnqueueState(EstadosDelGameplay.InicioDia);
		else EnqueueState(EstadosDelGameplay.InicioCaso);
	}

    public void ResetSingleton()
    {
		estadoActual = 0;
		gameover = false;

        pilaEstadosSiguientes.Clear();
		pauseMessageDict.Clear();
    }
    private void OnEnable()
	{
		//PauseGameplayCycle(false, "self");
		ResourceManager.OnOutOfQueries.AddListener(OperacionesGameplay.SinConsultas);
		MenuPausa.onExitLevel.AddListener(ResetSingleton);
	}
	private void OnDisable()
	{
        MenuPausa.onExitLevel.RemoveListener(ResetSingleton);
        ResourceManager.OnOutOfQueries.RemoveListener(OperacionesGameplay.SinConsultas);
		//PauseGameplayCycle(true, "self");
		terminarBucleExtractor = true;

    }
    private void OnApplicationQuit()
    {
        OperacionesGameplay.Snapshot();
        MenuPartidaController.GuardarPartidaEnCurso_S();
        XAPI_Builder.CreateStatement_GameSession(false); // finishing session
		XAPI_Builder.SendAllStatements();
    }

    private async void BucleExtractor()
	{
		while(!gameover && !terminarBucleExtractor)
		{
			if(pilaEstadosSiguientes.Count > 0)
			{
				int e = pilaEstadosSiguientes.Dequeue();
				while (IsPaused()) await Task.Delay(100);
				await ApplyState(e);
				OnCycleTaskFinished?.Invoke();
            }
			else
			{
				await Task.Delay(50);
			}
		}
        terminarBucleExtractor = false;
    }
	public static void PauseGameplayCycle(bool pause, string objName)
	{
		//print(objName+" pause set to: "+pause);
		if (!pauseMessageDict.TryAdd(objName, pause)) 
			pauseMessageDict[objName] = pause;
	}
    public static bool IsPaused()
	{
		//El sistema está pausado si hay al menos un mensaje de pausa activado.
		bool pausado = false;
		foreach(bool val in pauseMessageDict.Values) pausado |= val;
		return pausado;
	}


	private void TryLoadGame()
    {
		AlmacenDePalabras.CargarPalabras();
		recargarCasosDePartidaAnterior = ResourceManager.checkpoint.casosCargados.Length > 0;
		PuzzleManager.CasoActivoIndice = ResourceManager.checkpoint.casoEnCurso;
		Task.Run(() => AlmacenEventos.DescargarEventosServidor());
	}

    [ContextMenu("Loggear Estado de Pausas")]
    private void LogPauseStates()
    {
		if (pauseMessageDict == null) return;

		StringBuilder sb = new();
        foreach(var elem in pauseMessageDict)
		{
			sb.Append(elem.Key + " -> " + elem.Value+"\n");
		}
		print(sb.ToString());
    }
}
