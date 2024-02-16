/// Esta clase se encarga de controlar los recursos del jugador
using System.Collections.Generic;
using UnityEngine.Events;

public static class ResourceManager
{
    public static PuntoGuardado checkpoint = new();
    public static readonly int agentesInciales = 3;
    private static int agentesDisponibles;
    private static int consultasDisponibles;
    private static int consultasMaximas;
    private static int dificultadActual;
    private static int puntuacion;
    private static int dia;

    public static UnityEvent OnOutOfAgents = new();
    public static UnityEvent OnOutOfQueries = new();

    /// <summary>
    /// Cantidad de agentes que el jugador puede usar para obtener informes de casos.
    /// </summary>
    public static int AgentesDisponibles
    {
        get => agentesDisponibles; set
        {
            agentesDisponibles = value;
            if (agentesDisponibles <= 0)
            {
                agentesDisponibles = 0;
                OnOutOfAgents.Invoke();
            }
        }
    }
    public static int ConsultasDisponibles
    {
        get => consultasDisponibles; set
        {
            consultasDisponibles = value;
            if (consultasDisponibles > consultasMaximas) consultasDisponibles = consultasMaximas;
            if (consultasDisponibles <= 0)
            {
                consultasDisponibles = 0;
                OnOutOfQueries.Invoke();
            }
        }
    }
    public static int ConsultasMaximas
    {
        get => consultasMaximas; set
        {
            consultasMaximas = value;
            if (consultasMaximas < 1) consultasMaximas = 1;
        }
    }

    /// <summary>
    /// Grado de dificultad del juego, 
    /// el jugador desbloquea elementos del gameplay según vaya aumentando este valor.
    /// </summary>
    public static int DificultadActual
    {
        get => dificultadActual; set
        {
            dificultadActual = value;
            if (dificultadActual < 0) dificultadActual = 0;
        }
    }

    /// <summary>
    /// Puntuación total del jugador (suma de la putuación de todos los casos completados).
    /// </summary>
    public static int Puntuacion
    {
        get => puntuacion; set
        {
            puntuacion = value;
            if (puntuacion < 0) puntuacion = 0;
        }
    }

    public static int Dia
    {
        get => dia;
        set
        {
            dia = value;
            if (dia < 0) dia = 0;
        }
    }

    /// <summary>
    /// Lista de códigos que desbloquean tablas de la base de datos al jugador.
    /// </summary>
    public static List<string> TableCodes { get; set; } = new();

    /// <summary>
    /// Un caso es completado cuando se ha comprado y posteriormente se ha terminado,
    /// ya sea ganándolo, perdiéndolo o abandonándolo.
    /// </summary>
    public static List<int> CasosCompletados { get; set; } = new();

    /// <summary>
    /// Indica cómo se ha completado cada caso de la lista "CasosCompletados".
    /// </summary>
    /// <value>
    ///  1 = Caso Ganado / 
    ///  0 = Caso Abandonado / 
    /// -1 = Caso Perdido.
    /// </value>
    public static List<int> CasosCompletados_ListaDeEstados { get; set; } = new();

    /// <summary>
    /// ID de eventos que han sido ejecutados durante la aventura del jugador.
    /// </summary>
    public static List<int> EventosEjecutados { get; set; } = new();

    /// <summary>
    /// Indica el favor del pueblo. Cuanto más alto, mejor para el jugador.
    /// </summary>
    public static int ReputacionPueblo { get; set; }
    /// <summary>
    /// Indica la relación con el resto de empresas. Cuanto más alto, mejor para el jugador.
    /// </summary>
    public static int ReputacionEmpresas { get; set; }
}
