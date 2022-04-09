using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boton3D : MonoBehaviour
{
    [System.Serializable]
    public class Evento : UnityEvent {}
    public Evento onClick;

    private void OnMouseEnter()
    {

    }

    private void OnMouseExit()
    {
        
    }

    private void OnMouseUp()
    {
        onClick.Invoke();
    }
}
