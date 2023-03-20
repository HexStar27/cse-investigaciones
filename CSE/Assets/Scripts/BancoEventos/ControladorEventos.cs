using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorEventos : MonoBehaviour
{
    Queue<Evento> colaEventos = new Queue<Evento>();

    public void ConsumirEventos()
    {
        while(colaEventos.Count > 0)
        {
            var evento = colaEventos.Dequeue();
            //Todo el código necesario para ejecutar eventos
        }
    }
}
