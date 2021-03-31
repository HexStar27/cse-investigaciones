using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesplazadorVertical : MonoBehaviour
{
    public Transform objetivo;
    private Vector3 oldPos;
    private float oldMy;
    private bool isHeld;

    [Header("Configuración")]
    public float velocidad = 30;


    void Start()
    {
        if (!objetivo) Debug.LogError("El desplazador necesita un objeto al que desplazar. Corregir inmediatamente.");
    }

    void Update()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            objetivo.position += Vector3.up * Input.mouseScrollDelta.y * Time.deltaTime * velocidad;
        }
        else if(isHeld)
        {
            objetivo.position = oldPos + (Vector3.up * (MousePosDetector.MousePos().y - oldMy));
        }
    }

    private void OnMouseDown()
    {
        oldPos = objetivo.position;
        oldMy = MousePosDetector.MousePos().y;
        isHeld = true;
    }
    private void OnMouseUp()
    {
        isHeld = false;
    }
}
