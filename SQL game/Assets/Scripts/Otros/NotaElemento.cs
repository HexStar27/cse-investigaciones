using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NotaElemento : Arrastrable2, IListaUIElement
{
    public LayerMask capaCajon;
    private ListaUI listaUI;

    private UnityEvent onListExit = new UnityEvent();
    private UnityEvent onListEnter = new UnityEvent();

    SpriteRenderer sRenderer;

    //Zona de implementación de la interfaz
    public void ActualizarPosicionLocal(Vector3 localPos)
    {
        transform.localPosition = localPos;
    }

    public void AsignarPadre(Transform t)
    {
        transform.parent = t;
    }

    public void AsignarLista(ListaUI lui)
    {
        listaUI = lui;
        if (!listaUI.Buscar(this)) listaUI.Insertar(this);

        //Al formar parte de esa lista ahora no puede verse fuera de esta
        sRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }

    public void DesreferenciarLista()
    {
        if(listaUI!=null)
        {
            listaUI.Eliminar(this);
            listaUI = null;
        }

        //Al dejar de ser parte de la lista ya puede seguir viéndose fuera
        sRenderer.maskInteraction = SpriteMaskInteraction.None;
    }
    //Fin de la zona de interfaz


    private void Awake()
    {
        if (!sRenderer) if (!TryGetComponent(out sRenderer)) sRenderer = gameObject.AddComponent<SpriteRenderer>();
        AsignarAcciones();
    }

    private void AsignarAcciones()
    {
        onListExit.AddListener(DesreferenciarLista);
        onListEnter.AddListener(EntrarEnLista);

        accionAlInteractuar_.Insert(0,onListExit);
        accionAlInteractuar_.Insert(1,onListEnter);
    }

    private void EntrarEnLista()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(MousePosDetector.MousePos(), Vector2.zero, 15, capaCajon);
        if (hit2D)
        {
            if (hit2D.collider.TryGetComponent(out ListaUI l))
                AsignarLista(l);
            
        }
    }
}
