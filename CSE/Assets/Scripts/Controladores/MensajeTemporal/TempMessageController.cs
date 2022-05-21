/// Esta clase se encarga de mostrar mensajes informativos sobre lo que está pasando en el juego
/// Ej: "Se ha aplicado el efecto PATATA"

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TempMessageController : MonoBehaviour
{
    public static TempMessageController Instancia { get; set; }

    [SerializeField] private Animator _anim;
    [SerializeField] private TextMeshProUGUI _texto;
    [SerializeField] Queue<string> cola = new Queue<string>();
    public int duracion = 1500;
    private SemaphoreSlim semaforo = new SemaphoreSlim(0);
    [SerializeField] string nombreAnimacion1 = "Mostrar";
    [SerializeField] string nombreAnimacion2 = "Cerrar";

    private bool terminado = true;

    public void InsetarMensajeEnCola(string mensaje)
    {
        cola.Enqueue(mensaje);
        semaforo.Release(1);
    }

    public void LimpiarCola()
    {
        cola.Clear();
    }

    private void Awake()
    {
        Instancia = this;
        ConsumirMensajes();
    }

    public int SemaphoreState() { return semaforo.CurrentCount; }
    public bool Terminado() { return terminado; }

    private async void ConsumirMensajes()
    {
        while (true)
        {
            terminado = true;
            await semaforo.WaitAsync();
            terminado = false;
            string mensaje = cola.Dequeue();
            _texto.SetText(mensaje);
            _anim.Play(nombreAnimacion1);
            await Task.Delay(duracion);
            _anim.Play(nombreAnimacion2);
            await Task.Delay(500);
        }
    }
}
