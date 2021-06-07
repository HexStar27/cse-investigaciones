using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrastrable2 : InteractuableSecuencia
{
    Vector3 mousePos;
    Vector3 startPos;
    Vector3 finalPos;

    bool isHeld;

    [Header("Límites de desplazamiento")]
    public Transform objetivo;
    private Vector3 posicion;
    public bool usarWidth = false;
    public bool usarHeight = false;
    public float width;
    public float height;

    [Header("Configuración adicional")]
    public float posZ = 0;

    protected virtual void OnEnable()
    {
        if (objetivo != null) posicion = objetivo.position;
        else usarWidth = usarHeight = false;
    }

    protected virtual void Update()
    {
        if (isHeld)
        {
            mousePos = MousePosDetector.MousePos();
            finalPos = new Vector3(mousePos.x, mousePos.y, posZ) - startPos;

            if (usarWidth)
            {
                if (finalPos.x < posicion.x - width)
                    finalPos.x = posicion.x - width;
                else if (finalPos.x > posicion.x + width)
                    finalPos.x = posicion.x + width;
            }
            if (usarHeight)
            {
                if (finalPos.y < posicion.y - height)
                    finalPos.y = posicion.y - height;
                else if (finalPos.y > posicion.y + height)
                    finalPos.y = posicion.y + height;
            }

            transform.position = finalPos;
        }
    }

    protected virtual void OnMouseDown()
    {
        ActivarAccion(0);
        mousePos = MousePosDetector.MousePos();
        startPos = new Vector3(mousePos.x - transform.position.x, mousePos.y - transform.localPosition.y, 0);
        isHeld = true;
    }

    protected virtual void OnMouseUp()
    {
        isHeld = false;
        ActivarAccion(1);
    }

    public void ForceDrag()
    {
        mousePos = MousePosDetector.MousePos();
        startPos = new Vector3(mousePos.x - transform.position.x, mousePos.y - transform.localPosition.y, 0);
        isHeld = true;
        ActivarAccion(0);
    }

    public void ForceDrop()
    {
        isHeld = false;
        ActivarAccion(1);
    }
}
