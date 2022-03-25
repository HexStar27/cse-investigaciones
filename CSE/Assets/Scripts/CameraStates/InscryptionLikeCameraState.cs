using UnityEngine;

public class InscryptionLikeCameraState : MonoBehaviour
{
    [SerializeField] private CameraState camS;
    public bool loopable = false;
    private bool ready = true;
    private int estadoActual = 0;
    public int initialState = 1;

    private void Ready()
    {
        ready = true;
    }

    private void OnEnable()
    {
        camS.onFinished.AddListener(Ready);

        if(initialState < 0 || initialState >= camS.States())
        {
            initialState = 0;
        }
        estadoActual = initialState;
        camS.Transition(initialState);
    }
    private void OnDisable()
    {
        camS.onFinished.RemoveListener(Ready);
    }

    void Update()
    {
        if (ready)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                int n = camS.States();
                estadoActual++;
                if (estadoActual >= n)
                {
                    if (loopable) estadoActual = 0;
                    else estadoActual = n-1;
                }
                camS.Transition(estadoActual);
                ready = false;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                int n = camS.States();
                estadoActual--;
                if (estadoActual < 0)
                {
                    if (loopable) estadoActual = n-1;
                    else estadoActual = 0;
                }
                camS.Transition(estadoActual);
                ready = false;
            }

        }
    }
}
