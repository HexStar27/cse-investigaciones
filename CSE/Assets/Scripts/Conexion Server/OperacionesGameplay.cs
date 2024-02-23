// Esta clase se va a encargar de mandar peticiones específicas del juego al servidor a través del Conexión Handler

using System.Threading.Tasks;
using UnityEngine;
using Hexstar;
using Hexstar.Dialogue;
using Hexstar.CSE;
using SimpleJSON;

using ResultType = UnityEngine.Networking.UnityWebRequest.Result;
using CSE.Local;

public class OperacionesGameplay : MonoBehaviour
{
    private static int eventoId = 0;

    [SerializeField] private GameObject dangerController;
    private static GameObject s_dangerController;
    private static AudioClip s_clip_caseBridge;
    
    public static int s_lastScore = 0;

    public static int EventoId { get => eventoId; set => eventoId = value; }

    private void Awake()
    {
        if (dangerController != null) s_dangerController = dangerController;
        s_clip_caseBridge = Resources.Load<AudioClip>("Audio/Music/MT_CaseBridge");
    }

    public void RealizarConsultaD()
    {
        RealizarConsulta();
    }
    public static async void RealizarConsulta()
    {
        string consulta = LectorConsulta.GetQuery();
        if (consulta.Length <= 0)
        {
            TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".msg.temp.consulta_vacia"));
            CSE.XAPI_Builder.CreateStatement_TrySendQuery(true,false,false);
            return;
        }

        WWWForm form = new WWWForm();
        form.AddField("authorization",SesionHandler.sessionKEY);
        form.AddField("consulta", consulta);

        await ConexionHandler.APost(ConexionHandler.baseUrl + "case/check", form);
        if (!CheckInternetConection()) return;
        string resultado = ConexionHandler.ExtraerJson(ConexionHandler.download);
        resultado = resultado[1..^1];
        ImpresorResultado.Instancia.IntroducirResultado(resultado);

        if (GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso)
        {
            ResourceManager.ConsultasDisponibles--;
            PuzzleManager.ConsultasRealizadasActuales++;
            await DataUpdater.Instance.ShowConsultasDisponibles();
            ActualizarDangerController();
        }

        bool correctSyntax = !ImpresorResultado.Instancia.LastQueryWasNotCorrect();
        bool isLost = ResourceManager.ConsultasDisponibles == 0;
        CSE.XAPI_Builder.CreateStatement_TrySendQuery(false, correctSyntax, isLost);
    }

    public static void ActualizarDangerController()
    {
        if (s_dangerController == null) return;
        bool resolviendoCaso = GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso;
        bool leQuedanConsultas = ResourceManager.ConsultasDisponibles <= 1;
        s_dangerController.SetActive(leQuedanConsultas && resolviendoCaso);
    }

    public void ComprobarCasoD() => ComprobarCaso();
    
    public static async void ComprobarCaso()
    {
        string consulta = LectorConsulta.GetQuery();
        if (consulta.Length <= 0)
        {
            TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".msg.temp.consulta_vacia"));
            CSE.XAPI_Builder.CreateStatement_TrySolveCase(false, false, null, .0f, 0, 0);
            return;
        }

        //Sólo comprueba el caso si hay uno activo
        if (GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso)
        {
            //Pregunta al servidor si se ha completado el caso
            Caso caso = PuzzleManager.GetCasoActivo();
            WWWForm form = new WWWForm();
            form.AddField("authorization", SesionHandler.sessionKEY);
            form.AddField("caseid", caso.id);
            form.AddField("caso", LectorConsulta.GetQuery());
            await ConexionHandler.APost(ConexionHandler.baseUrl + "case/solve", form);
            if (!CheckInternetConection()) return;
            string response = ConexionHandler.ExtraerJson(ConexionHandler.download);
            bool completado = response[0] == 't'; // "true"
            
            PuzzleManager.SolucionCorrecta = completado;

            PuzzleManager.ConsultasRealizadasActuales++;
            ResourceManager.ConsultasDisponibles--;
            await DataUpdater.Instance.ShowConsultasDisponibles();

            //Informar del resultado al jugador
            if (completado)
            {
                float t = PuzzleManager.GetSetTiempoEmpleado();
                
                MeterCasoEnCompletadosYCargarSiguiente(1); // 1 = Ganado
                GameplayCycle.Instance.PlayInstead(s_clip_caseBridge,true);

                await CalcularYGuardarPuntuacion();
                if (CheckTutorialPlaying()) TutorialChecker.SetWinCondition(TutorialChecker.WinCondition.WIN);

                CSE.XAPI_Builder.CreateStatement_TrySolveCase(true, true, caso, t,
                    PuzzleManager.ConsultasRealizadasActuales, s_lastScore);
                
                TerminarFaseCaso();
            }
            else
            {
                TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".msg.temp.caso_no_resuelto"));
                _ = LED_Controller.Instance.TurnRed();
                bool casoNoTerminadoPeroPerdido = ResourceManager.ConsultasDisponibles == 0;
                CSE.XAPI_Builder.CreateStatement_TrySolveCase(casoNoTerminadoPeroPerdido, false, caso, .0f, 0, 0);
            }            
            ActualizarDangerController();
        }
        else TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".msg.temp.no_caso_activo"));
    }
    private static bool CheckTutorialPlaying()
    {
        string v = ControladorDialogos.GetDialogueEventValue("tutorial");
        return v.Equals("true");
    }

    /// <summary>
    /// Si hay algún caso empezado, pide que el estado del gameplay cambie a "FinCaso".
    /// </summary>
    private static void TerminarFaseCaso()
    {
        if (GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso)
        {
            GameplayCycle.EnqueueState(EstadosDelGameplay.FinCaso);
        }
    }

    public void RendirseD()
    {
        Rendirse();
    }
    public static void Rendirse()
    {
        if(GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso)
        {
            if (CheckTutorialPlaying()) TutorialChecker.SetWinCondition(TutorialChecker.WinCondition.SURR);
            
            MeterCasoEnCompletadosYCargarSiguiente(0); // 0 = Rendido
            GameplayCycle.Instance.PlayInstead(s_clip_caseBridge,true);

            TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".msg.temp.eliminar_caso"));
            TerminarFaseCaso();

            CSE.XAPI_Builder.CreateStatement_Surrender();
        }
        else
        {
            TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".msg.temp.no_caso_activo"));
        }
    }
    public static void SilentSurrender()
    {
        if (GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso)
        {
            MeterCasoEnCompletadosYCargarSiguiente(0); // 0 = Rendido
            TerminarFaseCaso();
            CSE.XAPI_Builder.CreateStatement_Surrender();
        }
    }
    /// <summary>
    /// Se debe ejecutar cuando el jugador se queda sin consultas disponibles.
    /// </summary>
    public static void SinConsultas()
    {
        if (GameplayCycle.GetState() == (int)EstadosDelGameplay.InicioCaso && !PuzzleManager.SolucionCorrecta)
        {
            if (CheckTutorialPlaying()) TutorialChecker.SetWinCondition(TutorialChecker.WinCondition.LOST);

            MeterCasoEnCompletadosYCargarSiguiente(-1); // -1 = Perdido
            GameplayCycle.Instance.PlayInstead(s_clip_caseBridge,true);
            TerminarFaseCaso();
        }
    }

    private static void MeterCasoEnCompletadosYCargarSiguiente(int estado)
    {
        int idCaso = PuzzleManager.GetIdCasoActivo();
        if (idCaso < 0) return; //Por si acaso...
        ResourceManager.CasosCompletados.Add(idCaso);
        ResourceManager.CasosCompletados_ListaDeEstados.Add(estado);
    }

    public static async Task CalcularYGuardarPuntuacion()
    {
        int id = PuzzleManager.GetIdCasoActivo();
        int time = Mathf.FloorToInt(PuzzleManager.UltimoTiempoEmpleado);
        WWWForm form = new WWWForm();
        form.AddField("authorization", SesionHandler.sessionKEY);
        form.AddField("caso", id);
        form.AddField("consultas", PuzzleManager.ConsultasRealizadasActuales);
        form.AddField("tiempo", time);
        form.AddField("examen", PuzzleManager.CasoActivoEsExamen() ? 1 : 0);
        form.AddField("reto", PuzzleManager.NRetosCumplidos);
        form.AddField("dificultad", ResourceManager.DificultadActual);
        form.AddField("consulta", LectorConsulta.GetQuery());
        await ConexionHandler.APost(ConexionHandler.baseUrl + "score/calculate", form);
        if (!CheckInternetConection()) return;

        string json = ConexionHandler.ExtraerJson(ConexionHandler.download);
        if (json == "{}") Debug.LogError("Ha habido un error en el servidor al calcular la puntuación :(");
        else
        {
            JSONNode jNodo = JSON.Parse(ConexionHandler.download);
            int puntuacion = jNodo["res"].AsInt;
            s_lastScore = puntuacion;

            form = new WWWForm();
            form.AddField("authorization", SesionHandler.sessionKEY);
            form.AddField("user", SesionHandler.email);
            form.AddField("caso", id);
            form.AddField("punt", puntuacion);
            form.AddField("dif", ResourceManager.DificultadActual);
            form.AddField("used", PuzzleManager.ConsultasRealizadasActuales);
            form.AddField("time", time);
            await ConexionHandler.APost(ConexionHandler.baseUrl + "score/save", form);
            if (!CheckInternetConection()) return;

            json = ConexionHandler.ExtraerJson(ConexionHandler.download);
            if (json == "{}") Debug.LogError("Ha habido un error al intentar guardar la puntuación :(");
            else
            {
                await LED_Controller.Instance.TurnGreen();
                await PuntuacionController.PresentarPuntuaciones(puntuacion);
                while (PuntuacionController.puntuacionEnPantalla) { await Task.Delay(200); }
            }
        }
    }

    /// <summary>
    /// Interpreta la última conexión con el servidor, devuelve verdadero si la conexión es correcta, si la conexión falló enviará un mensaje al jugador.
    /// </summary>
    private static bool CheckInternetConection()
    {
        if(ConexionHandler.result == ResultType.ConnectionError)
        {
            string msg = "# <color=\"red\">" + Localizator.GetString(".msg.temp.no_internet") + "<color=\"white\"> #";
            TempMessageController.Instancia.GenerarMensaje(msg);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Crea un punto de guardado con los datos actuales
    /// </summary>
    public static void Snapshot()
    {
        ResourceManager.checkpoint.CopiarDatosDelSistema();
    }
}
