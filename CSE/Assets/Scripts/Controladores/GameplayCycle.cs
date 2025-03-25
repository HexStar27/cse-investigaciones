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
using System.Collections;
using CSE.Local;

[System.Serializable] public enum EstadosDelGameplay { InicioDia = 0, InicioCaso = 1, FinCaso = 2, FinDia = 3 };
public class GameplayCycle : MonoBehaviour, ISingleton
{
	public static GameplayCycle Instance { get; private set; }

	public int EstadosPosibles { get; private set; }

	[SerializeField] DayCounter _dayCounter;
	[SerializeField] Transform gameOverMenu;
	[SerializeField] AudioSource bgmSource;
	AudioClip bgm_resolviendoCaso;
	AudioClip bgm_oficineando;
	AudioClip bgm_gameOver;
	AudioClip bgm_intro_gameOver;
	[SerializeField] AwaiterUntilPressed popupFinDia;

	static int estadoActual = 0;
	static bool gameover = false;
	static bool terminarBucleExtractor = false;
	static bool recargarCasosDePartidaGuardada = false;

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
        StartCoroutine(PrepareOutOfCaseTrack());

        if (ResourceManager.Dia == 0) //Inicio del juego
		{
			ResourceManager.ConsultasMaximas = 4;
			ResourceManager.AgentesDisponibles = ResourceManager.agentesInciales;
            DataUpdater.Instance.ResetSingleton();
        }

        // Carga los casos necesarios al iniciar el día
        await PuzzleManager.PrepararCasosParaInicioDia(recargarCasosDePartidaGuardada);
        if (recargarCasosDePartidaGuardada) recargarCasosDePartidaGuardada = false;
		else
		{
			ResourceManager.ConsultasDisponibles = ResourceManager.ConsultasMaximas;
        }
		if (ResourceManager.Dia > 0 || MenuPartidaController.continuandoPartida) await DataUpdater.Instance.ShowConsultasDisponibles();

        await AlmacenEventos.EncargarseDeEventosAptos();

        if (ResourceManager.AgentesDisponibles <= 0 || CheckGameOverEvent())
        {
            await GameOver();
            return;
        }

        CheckEndOfDay();
        //OperacionesGameplay.ActualizarDangerController();
    }

	private async Task InicioCaso()
	{
		startedCasesInDay++;
		PlayCaseTrack();

        AlmacenDePalabras.CargarPistasDeCasoActivo();

		await Task.Yield();
	}

	private async Task FinCaso()
	{
		int nConsultas = PuzzleManager.NConsultasDuranteElCaso;
		float tiempo = PuzzleManager.UltimoTiempoEmpleado;
        bool ganado = PuzzleManager.SolucionCorrecta;
		if (ganado) completedCasesInDay++;

		await PuzzleManager.GetCasoActivo().ComprobarYAplicarBounties(ganado,nConsultas,tiempo);

		StartCoroutine(PrepareOutOfCaseTrack());

		await AlmacenEventos.EncargarseDeEventosAptos();

		if (ResourceManager.AgentesDisponibles <= 0 || CheckGameOverEvent()) await GameOver();
		else
		{
			CheckEndOfDay();

            if (ganado && PuzzleManager.CasoActivoEsExamen())
			{
				ResourceManager.DificultadActual++;
				TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".msg.temp.mas_dificultad"));
				XAPI_Builder.CreateStatement_DifficultyIncrease(ResourceManager.DificultadActual);
			}
		}

        //Elimina las pistas del caso
		AlmacenDePalabras.palabras[(int)TabType.Pistas].Clear();

		// Ya no se usa debido a que ahora los casos se cargan a través de eventos de historia.
		//await PuzzleManager.LoadCasosSiguientes(); // Los hijos correspondientes del caso
        PuzzleManager.QuitarCasoActivo(); // F la referencia
		PuzzleManager.SacarCasosDelBanquillo();	// Los no colocados
    }

	private async Task FinDia()
    {
		//MostrarDía
        int dia = ++ResourceManager.Dia;
		await _dayCounter.InitAnimation(dia - 1, dia);

		//Limpieza
		RestartCameraState();
		if (QueryModeController.IsQueryModeOnManual()) Intelisense.instance.pantalla.text ="";
		BloqueTabletaElemento.DestruirBloquesEnMesa();

		PuzzleManager.AplicarCaducidadDeCasosCargados();
		PuzzleManager.SolucionCorrecta = false;

        XAPI_Builder.CreateStatement_DayFinished(completedCasesInDay, startedCasesInDay);
        EnqueueState(EstadosDelGameplay.InicioDia);
	}

	public void CheckEndOfDay()
	{
		if (popupFinDia == null) return;
		if (AlmacenEventos.EjecutandoEventos) return;
        if (ResourceManager.ConsultasDisponibles == 0 || PuzzleManager.CasosRestantesEnMapa() == 0)
        {
            //Esperar confirmación del jugador para continuar
            popupFinDia.ActivarPopUp();
        }
		else popupFinDia.Press();
    }
	public void Enqueue4EndOfDay() 
	{
		EnqueueState(EstadosDelGameplay.FinDia);
	}

    private void RestartCameraState()
	{
		var ilcs = InscryptionLikeCameraState.Instance;
		if (ilcs == null) return;
        var cs = ilcs.GetComponent<CameraState>();
		ilcs.SetCameraState(cs);
		ilcs.SetEstadoActual(0);
		cs.Transition(0);
	}

	public AudioSource Get_BGM_Source() { return bgmSource; }
	public void PlayInstead(AudioClip audio, bool oneshot = false)
	{
		bgmSource.Stop();
		if (oneshot) bgmSource.PlayOneShot(audio);
		else
		{
			bgmSource.clip = audio;
			bgmSource.Play();
		}
    }
	private IEnumerator PrepareOutOfCaseTrack()
    {
		if ((bgmSource.clip == bgm_oficineando) && bgmSource.isPlaying) yield break;
		
		if(bgmSource.clip != null)
		{
            WaitWhile mientrasSuene = new(() => { return bgmSource.isPlaying && !gameover; });
            yield return mientrasSuene;
        }
		if (gameover) yield break;
        bgmSource.clip = bgm_oficineando;
        bgmSource.Play();
	}
	private void PlayCaseTrack()
	{
		if (bgmSource == null) return; 
		bgmSource.Stop();
		bgmSource.clip = bgm_resolviendoCaso;
        bgmSource.Play();
    }

	[ContextMenu("Forzar GameOver")]
	public async Task GameOver()
	{
		//Debería ser el fin del juego cuando 
		gameover = true;
        InscryptionLikeCameraState.SetBypass(true);
        if (gameOverMenu != null) gameOverMenu.gameObject.SetActive(true);
        if (bgmSource != null)
		{
			bgmSource.Stop(); //Cambiar por música de gameover
			bgmSource.PlayOneShot(bgm_intro_gameOver);
			await Task.Delay(5100);
			bgmSource.clip = bgm_gameOver;
			bgmSource.Play();
		}
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
        MenuPausa.onExitLevel?.Invoke();
        ResourceManager.checkpoint.CargarDatosAlSistema();
		GameManager.CargarEscena(GameManager.GameScene.ESCENA_PRINCIPAL);
	}
	public void VolverAlMenuPrincipal() { 
		GameManager.CargarEscena(GameManager.GameScene.MENU_PARTIDA); 
	}

    private void Awake()
	{
        if (Instance == null) Instance = this;
		EstadosPosibles = 4;

		BucleExtractor();

		bgm_resolviendoCaso = Resources.Load<AudioClip>("Audio/Music/WF_5.-_Hunch");
		bgm_oficineando = Resources.Load<AudioClip>("Audio/Music/WF_12.-_Bossa_Relajossa");
		bgm_gameOver = Resources.Load<AudioClip>("Audio/Music/MT_Estas_Fuegado");
		bgm_intro_gameOver = Resources.Load<AudioClip>("Audio/Music/MT_Ups");
    }
	private async void Start()
	{
		int d = ResourceManager.Dia;
		Task t = _dayCounter.InitAnimation(d - 1, d);
        await TryLoadGame();
		await Task.WhenAll(t);
		EnqueueState(EstadosDelGameplay.InicioDia);
	}

    public void ResetSingleton()
    {
		// Limpiando estados del GCycle
		estadoActual = 0;
		gameover = false;

        pilaEstadosSiguientes.Clear();
		pauseMessageDict.Clear();

		// Reiniciando valores globales de clases secundarias
		Boton3D.globalStop = false;
		InscryptionLikeCameraState.SetBypass(false);
    }
    private void OnEnable()
	{
        ResourceManager.OnOutOfQueries.AddListener(OperacionesGameplay.SinConsultas);
		MenuPausa.onExitLevel.AddListener(ResetSingleton);
	}
	private void OnDisable()
	{
        MenuPausa.onExitLevel.RemoveListener(ResetSingleton);
        ResourceManager.OnOutOfQueries.RemoveListener(OperacionesGameplay.SinConsultas);
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
        terminarBucleExtractor = false;
        while (!gameover && !terminarBucleExtractor)
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


    private async Task TryLoadGame()
    {
		AlmacenDePalabras.CargarPalabras();
		recargarCasosDePartidaGuardada = MenuPartidaController.continuandoPartida;
		await AlmacenEventos.DescargarEventosServidor();
		//A lo mejor hace falta meter una barra de carga...
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
