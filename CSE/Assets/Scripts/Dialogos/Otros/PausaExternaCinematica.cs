using UnityEngine;
using UnityEngine.Events;

public class PausaExternaCinematica : MonoBehaviour
{
    public UnityEvent onActivation = new();
    public GameplayCycleStalker stalker;

    private bool alreadyConnected = false;
    public void EscucharSiguiente()
    {
        if(!alreadyConnected)
        {
            stalker.onCycleReady.AddListener(Activar);
            alreadyConnected = true;
        }
    }
    private void Activar()
    {
        alreadyConnected = false;
        onActivation?.Invoke();
        stalker.onCycleReady.RemoveListener(Activar);
    }
}
