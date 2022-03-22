/// Esta clase se encarga de controlar los recursos del jugador
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    public static readonly int agentesInciales = 3;
    private static int agentesDisponibles;
    private static int consultasDisponibles;
    private static int consultasMaximas;
    private static int casosCompletados;
    private static int dificultadActual;
    private static int puntuacion;
    private static int dia;

    public class OnEvent : UnityEvent { }
    public static OnEvent OnOutOfAgents;
    public static OnEvent OnOutOfQueries;

    public static int AgentesDisponibles
    {
        get => agentesDisponibles; set
        {
            if (agentesDisponibles <= 0)
            {
                agentesDisponibles = 0;
                OnOutOfAgents.Invoke();
            }
            else agentesDisponibles = value;
        }
    }
    public static int ConsultasDisponibles
    {
        get => consultasDisponibles; set
        {
            if(consultasDisponibles <= 0)
            {
                consultasDisponibles = 0;
                OnOutOfQueries.Invoke();
            }
            else consultasDisponibles = value;
        }
    }
    public static int ConsultasMaximas
    {
        get => consultasMaximas; set
        {
            if (consultasMaximas <= 0) consultasMaximas = 0;
            else consultasMaximas = value;
        }
    }
    public static int CasosCompletados
    {
        get => casosCompletados; set
        {
            if (casosCompletados <= 0) casosCompletados = 0;
            else casosCompletados = value;
        }
    }
    public static int DificultadActual
    {
        get => dificultadActual; set
        {
            if (dificultadActual <= 0) dificultadActual = 0;
            else dificultadActual = value;
        }
    }
    public static int Puntuacion
    {
        get => puntuacion; set
        {
            if (puntuacion <= 0) puntuacion = 0;
            else puntuacion = value;
        }
    }
    public static int Dia
    {
        get => dia;
        set
        {
            if (dia < 0) dia = 0;
            else dia = value;
        }
    }

}
