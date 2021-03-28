using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactuable1: MonoBehaviour
{
    [Header("Sobre las animaciones al activarse")]
    [SerializeField]
#pragma warning disable 0649
    private Animator clickAnimator;
#pragma warning restore 0649

    public string clickAnimation;
    
    [Header("Acciones que realizará")]
    [SerializeField]
    protected UnityEvent AccionAlInteractuar_ = new UnityEvent();

    public void ActivarAccion()
    {
        if (clickAnimator != null) clickAnimator.Play(clickAnimation);

        AccionAlInteractuar_.Invoke();
    }
}
