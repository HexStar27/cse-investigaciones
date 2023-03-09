/// Esta clase se encarga de mostrar mensajes informativos sobre lo que está pasando en el juego
/// Ej: "Se ha aplicado el efecto PATATA"

using UnityEngine;

public class TempMessageController : MonoBehaviour
{
    public static TempMessageController Instancia { get; set; }

    [SerializeField] private GameObject prefabMensaje;
    [SerializeField] private RectTransform parent;
    private float tiempoReinicio = 1 + ((float)TempMessage.duracion)/1000;
    private float contador = 0;
    [SerializeField] private bool terminado = true;

    public void GenerarMensaje(string mensaje)
    {
        terminado = false;
        contador = tiempoReinicio;
        TempMessage a = Instantiate(prefabMensaje, parent).GetComponent<TempMessage>();
        a.EjecutarMensaje(mensaje);
    }


    private void Awake()
    {
        Instancia = this;
    }

    private void FixedUpdate()
    {
        if(contador <= 0)
            terminado = true;
        else
            contador -= Time.fixedDeltaTime;
    }

    public bool Terminado() { return terminado; }
}
