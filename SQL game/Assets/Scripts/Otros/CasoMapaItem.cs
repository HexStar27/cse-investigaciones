using TMPro;
using UnityEngine;

public class CasoMapaItem : Boton
{
    [Header("Obligatorio")]
    [SerializeField]
    private AgentController agentController;

    [Header("Configuración")]
    [SerializeField]
    private int nivelDificultad;
    
    [SerializeField]
    private int agentesNecesarios;

    [Header("Visuales")]
    [SerializeField]
    private TextMeshPro tm;

    //public Informe informe;

    private void OnEnable()
    {
        if (!agentController) Debug.LogError("Esta clase debe instanciarse con un agentController de referencia");

        if(!tm)
        {
            tm = gameObject.GetComponentInChildren<TextMeshPro>();
        }

        ActualizarUICaso();
    }

    public void DesbloquearInforme( )//Meter en un futuro como parámetro una clase que almacene los informes que está realizando el jugador
    {
        if (agentController && agentesNecesarios > 0 && agentController.AgentesDisponibles())
        {
            agentController.EnviarAgente();
            agentesNecesarios--;
            ActualizarUICaso();
        }

        if (agentesNecesarios <= 0)
        {
            //Aquí habrá que poner que la clase del parámetro reciva el informe
            gameObject.SetActive(false);
        }
    }

    public void EstablecerAgentesNecesario( int agentesNecesarios)
    {
        this.agentesNecesarios = agentesNecesarios;
    }

    public void EstablecerAgentController(AgentController agentController)
    {
        this.agentController = agentController;
    }


    private void ActualizarUICaso()
    {
        if (tm)
        {
            tm.text = "A x " + agentesNecesarios;
        }
    }
}
