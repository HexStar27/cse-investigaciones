/// Esta clase se encarga de mostrar mensajes informativos sobre lo que está pasando en el juego
/// Ej: "Se ha aplicado el efecto PATATA"

using UnityEngine;

public class TempMessageController : MonoBehaviour
{
    public static TempMessageController Instancia { get; set; }

    //Añadir: Animator, Cola y demás...
    public bool terminado = true;

    public void InsetarMensajeEnCola(string mensaje)
    {

    }

    public void LimpiarCola()
    {

    }

    private void Awake()
    {
        Instancia = this;
    }
}
