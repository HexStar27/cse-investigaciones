using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boton3D : MonoBehaviour
{
    [System.Serializable]
    public class Evento : UnityEvent {}
    public Evento onClick;
    public Evento onEnter;
    public Evento onExit;

    private void OnMouseEnter()
    {
        onEnter.Invoke();
    }

    private void OnMouseExit()
    {
        onExit.Invoke();
    }

    private void OnMouseUp()
    {
        onClick.Invoke();
    }
}
