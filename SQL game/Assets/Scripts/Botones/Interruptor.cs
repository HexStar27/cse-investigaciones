using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interruptor : InteractuableSecuencia
{
    private void OnMouseDown()
    {
        ActivarSiguienteAccion();
    }
    public void EjemploFunc(int i)
    {
        Debug.Log("¡" + gameObject.name + " ha sido presionado con valor " + i + "!");
    }
}
