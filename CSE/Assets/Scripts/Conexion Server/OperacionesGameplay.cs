﻿// Esta clase se va a encargar de mandar peticiones específicas del juego al servidor a través del Conexión Handler

using System.Threading.Tasks;
using UnityEngine;
using Hexstar;
using Hexstar.Dialogue;
using Hexstar.CSE;
using SimpleJSON;

using ResultType = UnityEngine.Networking.UnityWebRequest.Result;
using CSE.Local;
using System;

public class OperacionesGameplay : MonoBehaviour
{
    [SerializeField] private GameObject dangerController;
    private static GameObject s_dangerController;
    private static AudioClip s_clip_caseBridge;
    
    public static int s_lastScore = 0;

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
            PuzzleManager.NConsultasDuranteElCaso++;
            await DataUpdater.Instance.ShowConsultasDisponibles();
            //await t;
            ActualizarDangerController();
        }
        //else await t;

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
            
            if (response == "{}") return;

            bool completado = response[0] != '-'; // Número negativo => false
            if(completado)
            {
                // "op" es el índice de la solución alcanzada en la lista de soluciones del caso
                // (sólo el servidor tiene acceso a esto)
                if (int.TryParse(response, out int op))
                {
                    ControladorDialogos.SetDialogueEvent("case_solution_" + caso.id, op.ToString());
                    // ASÍ LOS EVENTOS Y CINEMÁTICAS PUEDEN REACCIONAR A QUÉ SOLUCIÓN SE ALCANZÓ!
                }
                else if (response == "true")
                {
                    ControladorDialogos.SetDialogueEvent("case_solution_" + caso.id, "0");
                    // Por defecto se indica que se ha completado el caso con la solución base.
                }
                else
                {
                    ControladorDialogos.SetDialogueEvent("case_solution_" + caso.id, "false");
                    completado = false;
                }
            }
            
            PuzzleManager.SolucionCorrecta = completado;

            PuzzleManager.NConsultasDuranteElCaso++;
            ResourceManager.ConsultasDisponibles--;
            await DataUpdater.Instance.ShowConsultasDisponibles();

            //Informar del resultado al jugador
            if (completado)
            {
                float t = PuzzleManager.GetSetTiempoEmpleado();
                
                MeterCasoEnCompletados(1); // 1 = Ganado
                GameplayCycle.Instance.PlayInstead(s_clip_caseBridge,true);


                await CalcularYGuardarPuntuacion();

                CSE.XAPI_Builder.CreateStatement_TrySolveCase(true, true, caso, t,
                    PuzzleManager.NConsultasDuranteElCaso, s_lastScore);
                
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
            MeterCasoEnCompletados(0); // 0 = Rendido
            GameplayCycle.Instance.PlayInstead(s_clip_caseBridge,true);

            TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".msg.temp.eliminar_caso"));
            PuzzleManager.SolucionCorrecta = false;
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
            MeterCasoEnCompletados(0); // 0 = Rendido
            PuzzleManager.SolucionCorrecta = false;
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
            MeterCasoEnCompletados(-1); // -1 = Perdido
            GameplayCycle.Instance.PlayInstead(s_clip_caseBridge,true);
            TerminarFaseCaso();
        }
    }

    private static void MeterCasoEnCompletados(int estado)
    {
        int idCaso = PuzzleManager.GetIdCasoActivo();
        if (idCaso < 0) return; //Por si acaso...
        ResourceManager.CasosCompletados.Add(idCaso);
        ResourceManager.CasosCompletados_ListaDeEstados.Add(estado);

        ControladorDialogos.SetDialogueEvent("win_condition", estado switch {
            -1 => "lose", 0 => "surrender", 1 => "win",
            _ => throw new NotImplementedException(),
        });
    }

    public static async Task CalcularYGuardarPuntuacion()
    {
        int id = PuzzleManager.GetIdCasoActivo();
        int time = Mathf.FloorToInt(PuzzleManager.UltimoTiempoEmpleado);
        int retosCompletados = PuzzleManager.GetCasoActivo().PeekCompletedBounties(true, PuzzleManager.NConsultasDuranteElCaso, time);
        WWWForm form = new WWWForm();
        form.AddField("authorization", SesionHandler.sessionKEY);
        form.AddField("caso", id);
        form.AddField("consultas", PuzzleManager.NConsultasDuranteElCaso);
        form.AddField("tiempo", time);
        form.AddField("examen", PuzzleManager.CasoActivoEsExamen() ? 1 : 0);
        form.AddField("reto", retosCompletados);
        form.AddField("dificultad", ResourceManager.DificultadActual);
        form.AddField("consulta", LectorConsulta.GetQuery());
        await ConexionHandler.APost(ConexionHandler.baseUrl + "score/calculate", form);
        if (!CheckInternetConection()) return;

        string json = ConexionHandler.ExtraerJson(ConexionHandler.download);
        if (json == "{}") Debug.LogWarning("Ha habido un error en el servidor al calcular la puntuación :(");
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
            form.AddField("used", PuzzleManager.NConsultasDuranteElCaso);
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
