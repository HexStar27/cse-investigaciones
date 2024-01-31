using UnityEngine;

public class ControladorDeOtrosEfectos : MonoBehaviour
{
    public static ControladorDeOtrosEfectos Instance { 
        private set { Instance = value; }
        get { return Instance; }
    }

    [SerializeField] CameraState focus;
    [SerializeField] CameraState principal;
    [SerializeField] InscryptionLikeCameraState ilcs;

    public void AplicarEfecto(string value)
    {
        var vals = value.Split();
        string accion = vals[0];
        switch (accion)
        {
            case "focus":
                SetFocus(int.Parse(vals[1]));
                break;
            case "cam":
                SetCam(int.Parse(vals[1]));
                break;
            case "fix_state":
                bool bypass = bool.Parse(vals[1]);
                InscryptionLikeCameraState.SetBypass(bypass);
                break;
            default:
                print("No se ha podido aplicar el efecto indicado en la cadena "+value);
                break;
        }
    }

    private void SetFocus(int idx)
    {
        if (idx >= 0)
        {
            focus.Transition(idx);
        }
    }
    private void SetCam(int idx)
    {
        if (idx >= 0)
        {
            principal.Transition(idx);
            ilcs.SetEstadoActual(idx);
        }
    }
}
