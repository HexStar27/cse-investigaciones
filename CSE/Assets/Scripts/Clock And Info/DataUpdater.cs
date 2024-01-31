/// Esta clase se encarga de mostrar la puntuación y los recursos en el "reloj".

using UnityEngine;
using TMPro;
using CSE.Feedback;
using System.Threading.Tasks;

/// <summary>
/// Formatea los datos relevantes para el jugador y los muestra en una pantalla en el escritorio del juego.
/// </summary>
public class DataUpdater : MonoBehaviour, ISingleton
{
	public static DataUpdater Instance { get; private set; }
	[SerializeField] CameraState cameraFocuser;
	[SerializeField] CameraState mainCamState;
	[SerializeField] Boton3D QCV_Button;

	[Header("Estados de cámara")]
	[SerializeField] int estadoParaAD = 6;
    [SerializeField] int estadoParaCX = 7;

	private void Start()
	{
		GameplayCycle.OnCycleTaskFinished.AddListener(QCV_StateController);
    }

	public async Task ShowAgentesDisponibles()
	{ await AsyncUpdaterActivation("AgentesDisponibles", ResourceManager.AgentesDisponibles, estadoParaAD); }
	public async Task ShowConsultasDisponibles()
	{
		if (!IsQCVBlocked()) Force_QCV_State(true);
        await AsyncUpdaterActivation("ConsultasDisponibles", ResourceManager.ConsultasDisponibles, estadoParaCX);
		if (!IsQCVBlocked()) Force_QCV_State(false);
    }
    public async Task ShowConsultasMaximas()
    {
        if (!IsQCVBlocked()) Force_QCV_State(true);
        await AsyncUpdaterActivation("ConsultasMaximas", ResourceManager.ConsultasMaximas, estadoParaCX);
        if (!IsQCVBlocked()) Force_QCV_State(false);
    }

    private void QCV_StateController() => Force_QCV_State(IsQCVBlocked());

	private void Force_QCV_State(bool force)
	{
        if (force)
        {
            QCV_Button.SetAnimationBlock(true);
            QCV_Button.animator.Play(QCV_Button.enterAnim);
        }
        else
        {
            QCV_Button.SetAnimationBlock(false);
            QCV_Button.animator.Play(QCV_Button.exitAnim);
        }
    }

	private bool IsQCVBlocked() { return GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso; }


    public async Task AsyncUpdaterActivation(string updaterId, int value, int camFocusState)
	{
        var updater = InscryptionableFeedback.dictionary[updaterId];
		if (updater != null)
		{
			//1º Mover cámara
			int lastState = mainCamState.GetState();
			cameraFocuser.Transition(camFocusState);
			await Task.Delay((int)(1000f * (1f / cameraFocuser.speed)));
			//2º Ejecutar animación
			await updater.DirectChange(value);
			await Task.Delay(350); //Waits some extra time
			//3º Restaurar cámara
			mainCamState.Transition(lastState);
			await Task.Delay((int)(1000f * (1f / mainCamState.speed)));
		}
		else throw new System.Exception("El updater \"" + updaterId + "\" no se encuentra en la lista de feedback.");
    }

	[Header("Time Stuff")]
	public TextMeshPro contenedor;
	private string contenido;

	public void FixedUpdate()
	{
		ObtenerTiempo();
		contenedor.SetText(contenido);
	}

	private void ObtenerTiempo()
	{
		int crono = -1;
		bool casoIniciado = GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso;
		bool puntuacionEnPantalla = PuntuacionController.puntuacionEnPantalla;
        if (casoIniciado && !puntuacionEnPantalla)
			crono = Mathf.RoundToInt(PuzzleManager.GetSetTiempoEmpleado());

		contenido = AntiOverflowTime(crono);
	}

	//Limitación: Cada pantallita del reloj puede tener hasta 4 carácteres
	private string AntiOverflowTime(int num)
    {
		if(num < 0) return "--    --";
		
		int segs = num % 60;
		int min = num / 60;
		
		if (min >= 60) return "+1h!";
		else return min.ToString("00") + "    " + segs.ToString("00");
    }
	private string AntiOverflowInt(int num)
	{
		if(num >= 100000)
		{
			return "O_O'";
		}
		else if(num >= 10000)
		{
			return num / 1000 + "K";
		}
		else if(num >= 1000)
		{
			return num / 1000 + "." + (num % 1000) / 100 + "K";
		}
		return num.ToString();
	}

	public void ResetSingleton()
	{
        InscryptionableFeedback.dictionary["AgentesDisponibles"].SilentChange(ResourceManager.AgentesDisponibles.ToString());
        InscryptionableFeedback.dictionary["ConsultasDisponibles"].SilentChange(ResourceManager.ConsultasDisponibles.ToString());
        InscryptionableFeedback.dictionary["ConsultasMaximas"].SilentChange(ResourceManager.ConsultasMaximas.ToString());
    }

    private void Awake()
    {
        Instance = this;
    }
}
