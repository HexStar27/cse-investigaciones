using UnityEngine;

public class InscryptionLikeCameraState : MonoBehaviour
{
    public static InscryptionLikeCameraState Instance { get; private set; }

    [SerializeField] private CameraState camS;
    public bool loopable = false;
    protected static bool bypass = false;
    private bool ready = true;
    private int estadoActual = 0;
    public int initialState = 1;
    public bool useScroll = true;

    public static void SetBypass(bool value)
    {
        bypass = value;
    }

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

    public void SetEstadoActual(int nuevoEstado)
    {
        if (nuevoEstado < 0) nuevoEstado = 0;
        if (nuevoEstado >= camS.States()) nuevoEstado = camS.States() - 1;
        estadoActual = nuevoEstado;
    }
    public void SetCameraState(CameraState cam) { camS = cam; }

    public int GetEstadoActual() { return estadoActual; }
    public CameraState GetCamState() { return camS; }

    public void Go2NextState()
    {
        if (ready && !bypass && !MenuPausa.Paused)
        {
            int n = camS.States();
            estadoActual++;
            if (estadoActual >= n)
            {
                if (loopable) estadoActual = 0;
                else estadoActual = n - 1;
            }
            camS.Transition(estadoActual);
            ready = false;
        }
    }
    public void Go2PrevState()
    {
        if (ready && !bypass && !MenuPausa.Paused)
        {
            int n = camS.States();
            estadoActual--;
            if (estadoActual < 0)
            {
                if (loopable) estadoActual = n - 1;
                else estadoActual = 0;
            }
            camS.Transition(estadoActual);
            ready = false;
        }
    }

    void Update()
    {
        if (bypass || MenuPausa.Paused) return;
        if (ready)
        {
            float scrollDelta = 0;
            if (useScroll) scrollDelta = Input.mouseScrollDelta.y;

            if (Input.GetKeyDown(KeyCode.W) || scrollDelta > 0) Go2NextState();
            else if (Input.GetKeyDown(KeyCode.S) || scrollDelta < 0) Go2PrevState();
        }
    }

    void Awake()
    {
        Instance = this;
    }
}
