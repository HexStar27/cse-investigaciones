/// Esta clase se encarga de controlar los recursos del jugador
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class ResourceManager
{
    public static PuntoGuardado checkpoint = new PuntoGuardado();
    public static readonly int agentesInciales = 3;
    private static int agentesDisponibles;
    private static int consultasDisponibles;
    private static int consultasMaximas;
    private static int casosCompletados; //A.K.A. reputación
    private static int dificultadActual;
    private static int puntuacion;
    private static int dia;
    private static List<string> tableCodes;

    public class OnEvent : UnityEvent { }
    public static OnEvent OnOutOfAgents = new OnEvent();
    public static OnEvent OnOutOfQueries = new OnEvent();

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
            if (consultasMaximas < 0) consultasMaximas = 0;
        }
    }
    public static int CasosCompletados
    {
        get => casosCompletados; set
        {
            casosCompletados = value;
            if (casosCompletados < 0) casosCompletados = 0;
        }
    }
    public static int DificultadActual
    {
        get => dificultadActual; set
        {
            dificultadActual = value;
            if (dificultadActual < 0) dificultadActual = 0;
        }
    }
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

    public static List<string> TableCodes
    {
        get => tableCodes;
        set => tableCodes = value;
    }
}
