/// Esta clase se va a encargar de mandar peticiones específicas del juego al servidor a través del Conexión Handler

using System.Collections;
using UnityEngine;
using Hexstar;
using System;

public class OperacionesGameplay : MonoBehaviour
{
    public static OperacionesGameplay Instancia { get; private set; }

    private static Action[] LUTEfectos = new Action[16];
    public int eventoId = 0;

    public void RealizarConsulta()
    {
        Debug.Log("Realizando consulta...");
        // TODO
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
                ResourceManager.CasosCompletados++;
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
        if (banco != null) banco.Clear();
        else Debug.Log("El banco de eventos no está inicializado...");
        for(int i = 0; i < nEventos; i++)
        {
            banco.Set(eventos[i]);
        }
    }

    public void EjecutarEventoAleatorio()
    {
        // 1º Obtener el evento
        int nEventos = BancoEventos.instance().Count();
        int e = UnityEngine.Random.Range(0,nEventos);
        Evento evento = BancoEventos.instance().Get(e);
        // 2º Realizar cambios del evento
        // TODO
    }

    public void AplicarEfectosCaso(Caso caso, bool ganado, int consultasUsadas, float tiempoEmpleado)
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

    private void Awake()
    {
        Instancia = this;
        InicializarLUTEventos();
    }

    private void OnEnable()
    {
        ResourceManager.OnOutOfQueries.AddListener(SinConsultas);
    }
    private void OnDisable()
    {
        ResourceManager.OnOutOfQueries.RemoveListener(SinConsultas);
    }

    private void InicializarLUTEventos()
    {
        LUTEfectos[0] = () => //Evento
        {
            // TODO
            TempMessageController.Instancia.InsetarMensajeEnCola("El mensaje...");
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
            TempMessageController.Instancia.InsetarMensajeEnCola("El mensaje...");
        };
        LUTEfectos[4] = () => //Incertidumbre
        {
            // TODO
            TempMessageController.Instancia.InsetarMensajeEnCola("El mensaje...");
        };
        LUTEfectos[5] = () => //MasDificultad
        {
            ResourceManager.DificultadActual++;
            TempMessageController.Instancia.InsetarMensajeEnCola("LA DIFICULTAD HA SIDO AUMENTADA");
        };
        LUTEfectos[6] = () => //FinDelJuego...
        {
            // TODO
        };
        //Hasta el 16...
    }
}
