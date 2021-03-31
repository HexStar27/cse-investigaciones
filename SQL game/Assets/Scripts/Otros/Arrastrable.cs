using UnityEngine;
using UnityEngine.Animations;

public class Arrastrable : MonoBehaviour
{
    public Camera cPrincipal;

    public float posZ = 0;

    [Header("Límites de desplazamiento")]
    public Transform objetivo;
    private Vector3 posicion;
    public float width;
    public float height;

    Vector3 mousePos;
    Vector3 startPos;
    Vector3 finalPos;

    bool isHeld;

    private void Awake()
    {
        if(!cPrincipal)
        {
            cPrincipal = Camera.main;
        }

        if(objetivo != null)
        {
            posicion = objetivo.position;
        }
        else
        {
            Debug.LogError("Este objeto debería tener una referencia a un transform para poder fijar el límite del desplazamiento");
            posicion = Vector3.zero;
        }
    }

    protected virtual void Update()
    {
        if(isHeld)
        {
            mousePos = MousePosDetector.MousePos();
            finalPos = new Vector3(mousePos.x, mousePos.y, posZ) - startPos;

            if(finalPos.x < posicion.x - width)
                finalPos.x = posicion.x - width;
            else if(finalPos.x > posicion.x + width)
                finalPos.x = posicion.x + width;
            
            if (finalPos.y < posicion.y - height)
                finalPos.y = posicion.y - height;
            else if (finalPos.y > posicion.y + height)
                finalPos.y = posicion.y + height;
            
            transform.position = finalPos;
        }
    }

    protected virtual void OnMouseDown()
    {
        mousePos = MousePosDetector.MousePos();
        startPos = new Vector3(mousePos.x - transform.position.x, mousePos.y - transform.localPosition.y, 0);
        isHeld = true;
    }

    protected virtual void OnMouseUp()
    {
        isHeld = false;
    }

    public void ForceDrag()
    {
        mousePos = MousePosDetector.MousePos();
        startPos = new Vector3(mousePos.x - transform.position.x, mousePos.y - transform.localPosition.y, 0);
        isHeld = true;
    }

    public void ForceDrop()
    {
        isHeld = false;
    }
}
