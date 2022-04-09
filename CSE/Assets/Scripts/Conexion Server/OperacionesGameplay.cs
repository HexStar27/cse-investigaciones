/// Esta clase se va a encargar de mandar peticiones específicas del juego al servidor a través del Conexión Handler

using System.Collections;
using UnityEngine;
using Hexstar;

public class OperacionesGameplay : MonoBehaviour
{
    private static OperacionesGameplay instancia;

    public static OperacionesGameplay Instancia { get => instancia; private set => instancia = value; }

    public void RealizarConsulta()
    {
        Debug.Log("Realizando consulta...");
        ResourceManager.ConsultasDisponibles--;
    }

    public void ComprobarCaso()
    {
        //Sólo comprueba el caso si hay uno activo
        if (GameplayCycle.Instance.GetState() == 1)
        {
            Debug.Log("Comprobando caso...");
            //Se ha completado el caso?
            // TODO
            bool completado = false;
            if(completado)
            {
                //Informar de que es correcto
            }
            else
            {
                //Informar de que no es correcto
            }

            ResourceManager.ConsultasDisponibles--;
        }
    }

    public void SinConsultas()
    {
        if(GameplayCycle.Instance.GetState()==1) GameplayCycle.Instance.SetState(2);
        else GameplayCycle.Instance.SetState(0);
    }

    public void CargarEventos()
    {
        //1º Acceder a servidor y pedir eventos según el nivel de dificultad
        int nEventos = 0;
        Evento[] eventos = new Evento[0];
        //2º Añadir cada evento al banco de eventos
        BancoEventos banco = BancoEventos.instance();
        banco.Clear();
        for(int i = 0; i < nEventos; i++)
        {
            banco.Set(eventos[i]);
        }
    }

    public void EjecutarEventoAleatorio()
    {
        // 1º Obtener el evento
        int nEventos = BancoEventos.instance().Count();
        int e = Random.Range(0,nEventos);
        Evento evento = BancoEventos.instance().Get(e);
        // 2º Realizar cambios del evento
        // TODO
    }

    private void Awake()
    {
        instancia = this;
    }

    private void OnEnable()
    {
        ResourceManager.OnOutOfQueries.AddListener(SinConsultas);
    }
    private void OnDisable()
    {
        ResourceManager.OnOutOfQueries.RemoveListener(SinConsultas);
    }
}
