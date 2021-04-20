using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractuableSecuencia : MonoBehaviour
{
    [Header("Sobre las animaciones al activarse")]
    [SerializeField]
#pragma warning disable 0649
    private Animator clickAnimator;
#pragma warning restore 0649

    public string clickAnimation;

    [Header("Acciones que realizará")]
    [SerializeField]
    protected List<UnityEvent> accionAlInteractuar_ = new List<UnityEvent>();
    protected int indice = 0;

    public void ActivarAccion(int indice)
    {
        if (indice >= 0 && indice < accionAlInteractuar_.Count)
        {
            if (clickAnimator != null) clickAnimator.Play(clickAnimation);

            accionAlInteractuar_[indice].Invoke();
        }
        else Debug.LogError("Te has salido del índice, no hay nada en la posición " + indice + ".");
    }

    public void ActivarSiguienteAccion()
    {
        accionAlInteractuar_[indice++].Invoke();
        if (indice >= accionAlInteractuar_.Count) indice = 0;
    }

    public UnityEvent ObtenerEvento(int evento)
    {
        return accionAlInteractuar_[evento];
    }
}
