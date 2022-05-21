/// Esta clase se encarga de mostrar mensajes informativos sobre lo que está pasando en el juego
/// Ej: "Se ha aplicado el efecto PATATA"

using UnityEngine;

public class TempMessageController : MonoBehaviour
{
    public static TempMessageController Instancia { get; set; }

    [SerializeField] private GameObject prefabMensaje;
    [SerializeField] private RectTransform parent;
    public float alturaMensajes = 100;
    private float tiempoReinicio = 1 + ((float)TempMessage.duracion)/1000;
    private float contador = 0;
    private bool terminado = true;
    private int acumulaciones = 0;

    public void GenerarMensaje(string mensaje)
    {
        terminado = false;
        contador = tiempoReinicio;
        float offset = alturaMensajes * acumulaciones++;
        TempMessage a = Instantiate(prefabMensaje, parent.position + new Vector3(0,-offset,0), Quaternion.identity, parent).GetComponent<TempMessage>();
        parent.anchoredPosition = new Vector2(parent.anchoredPosition.x,offset);
        a.EjecutarMensaje(mensaje);
    }


    private void Awake()
    {
        Instancia = this;
    }

    private void FixedUpdate()
    {
        if(contador <= 0)
        {
            terminado = true;
            acumulaciones = 0;
        }
        else
        {
            contador -= Time.fixedDeltaTime;
        }
    }

    public bool Terminado() { return terminado; }
}
