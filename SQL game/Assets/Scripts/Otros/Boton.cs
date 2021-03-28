using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boton : Interactuable1
{
    private void OnMouseDown()
    {
        ActivarAccion();
    }

    public void EjemploFunc()
    {
        Debug.Log("¡He sido presionado!");
    }
}
