/// Esta clase se va a encargar de mandar peticiones específicas del juego al servidor a través del Conexión Handler

using System.Collections.Generic;
using UnityEngine;
using Hexstar;
using System;

public class OperacionesGameplay : MonoBehaviour
{
    private static Action[] LUTEfectos = new Action[16];
    public static int eventoId = 0;


    public void RealizarConsultaD()
    {
        RealizarConsulta();
    }
    public static async void RealizarConsulta()
    {
        WWWForm form = new WWWForm();
        form.AddField("authorization",SesionHandler.sessionKEY);
        form.AddField("consulta", LectorConsulta.GetQuery());

        await ConexionHandler.APost(ConexionHandler.baseUrl + "case/check", form);
        string resultado = ConexionHandler.ExtraerJson(ConexionHandler.download);
        if (resultado.Length < 3)
        {
            ResourceManager.ConsultasDisponibles--;
            return;
        }
        resultado = resultado.Substring(1, resultado.Length - 2);
        ImpresorResultado.Instancia.IntroducirResultado(resultado);

        ResourceManager.ConsultasDisponibles--;
    }

    public void ComprobarCasoD()
    {
        ComprobarCaso();
    }
    public static async void ComprobarCaso()
    {
        //Sólo comprueba el caso si hay uno activo
        if (GameplayCycle.Instance.GetState() == 1)
        {
            //Se ha completado el caso?
            WWWForm form = new WWWForm();
            form.AddField("authorization", SesionHandler.sessionKEY);
            form.AddField("caseid", PuzzleManager.Instance.casoActivo.id);
            form.AddField("caso", LectorConsulta.GetQuery());
            await ConexionHandler.APost(ConexionHandler.baseUrl + "case/solve", form);
            string response = ConexionHandler.ExtraerJson(ConexionHandler.download);
            bool completado = response[0] == 't'; // "true"
            PuzzleManager.Instance.solucionCorrecta = completado;
            if (completado)
            {
                //Informar de que es correcto
                TempMessageController.Instancia.GenerarMensaje("INFORMACIÓN CORRECTA, CRIMEN RESUELTO");
                ResourceManager.CasosCompletados++;
                TerminarCaso();
            }
            else
            {
                //Informar de que no es correcto
                TempMessageController.Instancia.GenerarMensaje("Crimen no resuelto...");
            }

            ResourceManager.ConsultasDisponibles--;
        }
        else TempMessageController.Instancia.GenerarMensaje("Actualmente no se está resolviendo ningún caso");
    }

    private static void TerminarCaso()
    {
        if (GameplayCycle.Instance.GetState() == 1)
        {
            PuzzleManager.Instance.casoActivo = null;
            GameplayCycle.Instance.SetState(2);
        }
    }

    public void RendirseD()
    {
        Rendirse();
    }
    public static void Rendirse()
    {
        if(GameplayCycle.Instance.GetState() == 1)
        {
            PuzzleManager.Instance.solucionCorrecta = false;
            TerminarCaso();
            TempMessageController.Instancia.GenerarMensaje("Dejando caso... :(");
        }
        else
        {
            TempMessageController.Instancia.GenerarMensaje("No se puede descartar caso, no hay casos activos.");
        }
    }

    public static void SinConsultas()
    {
        if (GameplayCycle.Instance.GetState() == 1) GameplayCycle.Instance.SetState(2);
        else GameplayCycle.Instance.SetState(0);
    }

    /// <summary>
    /// Introduce los eventos recividos del servidor al banco de eventos del juego
    /// (No los ejecuta, solo los almacena)
    /// </summary>
    public static void CargarEventos()
    {
        //1º Acceder a servidor y pedir eventos según el nivel de dificultad
        int nEventos = 0;
        Evento[] eventos = new Evento[0];
        //StartCoroutine(ConexionHandler.Get(ConexionHandler.baseUrl + "event");
        //2º Añadir cada evento al banco de eventos
        BancoEventos banco = BancoEventos.Instance();
        if (banco != null) banco.Clear();
        else Debug.Log("El banco de eventos no está inicializado...");
        for (int i = 0; i < nEventos; i++)
        {
            banco.Add(eventos[i]);
        }
    }

    /// <summary>
    /// Crea un punto de guardado con los datos actuales
    /// </summary>
    public static void Snapshot()
    {
        ResourceManager.checkpoint.Fijar();
    }

    public static void EjecutarEventoAleatorio()
    {
        return; //No habrán eventos en la Alpha
        // 1º Obtener el evento
        //int nEventos = BancoEventos.Instance().Count();
        //int e = UnityEngine.Random.Range(0,nEventos);
        //Evento evento = BancoEventos.Instance().Get(e);
        // 2º Realizar cambios del evento
        //BancoEventos.Instance().Activate(evento);
    }

    public static void AplicarEfectosCaso(Caso caso, bool ganado, int consultasUsadas, float tiempoEmpleado)
    {
        foreach (var ev in caso.eventosCaso)
        {
            int i = (int)ev.efecto;
            bool aplicar = false;
            switch (ev.condicion)
            {
                case Caso.Condiciones.Ganar:
                    aplicar = ganado;
                    break;
                case Caso.Condiciones.Perder:
                    aplicar = !ganado;
                    break;
                case Caso.Condiciones.MaxConsultas1:
                    aplicar = consultasUsadas <= 1 && ganado;
                    break;
                case Caso.Condiciones.MaxConsultas2:
                    aplicar = consultasUsadas <= 2 && ganado;
                    break;
                case Caso.Condiciones.TiempoLimite30s:
                    aplicar = tiempoEmpleado <= 30 && ganado;
                    break;
                case Caso.Condiciones.TiempoLimite1min:
                    aplicar = tiempoEmpleado <= 60 && ganado;
                    break;
                case Caso.Condiciones.TiempoLimite2min:
                    aplicar = tiempoEmpleado <= 120 && ganado;
                    break;
                case Caso.Condiciones.TiempoLimite3min:
                    aplicar = tiempoEmpleado <= 180 && ganado;
                    break;
                case Caso.Condiciones.Siempre:
                    aplicar = true;
                    break;
            }

            if (i == 2) aplicar = !aplicar; //Las penalizaciones ocurren cuando no se ha alcanzado una condición

            if (aplicar)
            {
                if (i == 0 && caso.eventoId > 0) eventoId = caso.eventoId; // Evento
                
                LUTEfectos[i]();
            }
        }
    }

    public static void InicializarLUTEventos()
    {
        LUTEfectos[0] = () => //Evento
        {
            // TODO
            TempMessageController.Instancia.GenerarMensaje("El mensaje...");
        };
        LUTEfectos[1] = () => //Reto
        {
            int i = UnityEngine.Random.Range(7,13);
            LUTEfectos[i]();
        };
        LUTEfectos[2] = () => //Penalizacion
        {
            int i = UnityEngine.Random.Range(13,17);
            LUTEfectos[i]();
        };
        LUTEfectos[3] = () => //Papeleo
        {
            // TODO
            TempMessageController.Instancia.GenerarMensaje("El mensaje...");
        };
        LUTEfectos[4] = () => //Incertidumbre
        {
            // TODO
            TempMessageController.Instancia.GenerarMensaje("El mensaje...");
        };
        LUTEfectos[5] = () => //MasDificultad
        {
            ResourceManager.DificultadActual++;
            //PuntoGuardado.Fijar();
            TempMessageController.Instancia.GenerarMensaje("LA DIFICULTAD HA SIDO AUMENTADA");
        };
        LUTEfectos[6] = () => //FinDelJuego...
        {
            // TODO
        };
        //Hasta el 16...
    }
}
