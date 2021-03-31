using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionHilo : MonoBehaviour
{
    public Camera cPrincipal;
    private LineRenderer lr;

    public float posZ = 0;
    
    public bool isLinked = false;

    Vector3 mousePos;
    Vector3 objetivoPos;
    Transform objetivoTrans;

    bool isHeld;

    [SerializeField]
    private LayerMask capaInteractuable = 0;
    private RaycastHit2D rayCast;

    private void Awake()
    {
        if (!cPrincipal)
        {
            cPrincipal = Camera.main;
        }

        if(!lr)
        {
            if(!TryGetComponent(out lr))
            {
                lr = gameObject.AddComponent<LineRenderer>();
                Debug.LogWarning("Ese objeto debería poseer un LineRenderer. No pasa nada, ya lo he creado pero en un principio esto es un error de diseño y debería notificarse");
            }
        }
    }

    private void Update()
    {
        lr.SetPosition(0, transform.position);
        if (isLinked)
        {
            objetivoPos = objetivoTrans.position - Vector3.forward * cPrincipal.transform.position.z;
            lr.SetPosition(1, objetivoPos);
        }
        else
        {
            lr.SetPosition(1, transform.position);
        }

        if (isHeld)
        {
            mousePos = MousePosDetector.MousePos();

            //La posicion del segundo punto se iguala a la del raton
            lr.SetPosition(1, mousePos - Vector3.forward * cPrincipal.transform.position.z);
        }
        
    }

    private void OnMouseDown()
    {
        mousePos = MousePosDetector.MousePos();

        //El primer punto se pone en la posición del objeto
        lr.SetPosition(0, transform.position);

        isHeld = true;
    }

    private void OnMouseUp()
    {
        //Comprobar que haya objeto con tag pin usando raycast y que no sea él mismo
        //Poner la posicion del segundo punto al del objeto
        
        rayCast = Physics2D.Raycast(mousePos, Vector2.zero, 15, capaInteractuable);

        //Punto predeterminado
        lr.SetPosition(1, transform.position);
        isLinked = false;

        //Unir a otro objeto de unión si está libre
        if (rayCast.collider && rayCast.collider.CompareTag("pin")) //Ha detectado un collider y tiene el tag pin
        {
            objetivoTrans = rayCast.collider.transform;
            objetivoPos = objetivoTrans.position - Vector3.forward * cPrincipal.transform.position.z;

            if (rayCast.collider.TryGetComponent(out UnionHilo uh))
            {
                if(!uh.Pareja(transform))
                {
                    lr.SetPosition(1, objetivoPos);
                    isLinked = true;
                }
            }
        }
        
        isHeld = false;
    }

    /// <summary>
    /// Comprueba si está unido con el transform pasado
    /// </summary>
    /// <param name="tu">El transform del objeto a comprobar</param>
    /// <returns>Devuelve true si este objeto está linkeado con el pasado por parámetro/returns>
    public bool Pareja(Transform tu)
    {
        if (isLinked)
        {
            return tu == objetivoTrans;
        }
        else return false;
    }
}
