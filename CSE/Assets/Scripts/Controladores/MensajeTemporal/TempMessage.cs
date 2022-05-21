using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TempMessage : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private TextMeshProUGUI _texto;
    public static readonly int duracion = 1000;
    static readonly string nombreAnimacion1 = "Mostrar";
    static readonly string nombreAnimacion2 = "Cerrar";

    public async void EjecutarMensaje(string mensaje)
    {
        _texto.SetText(mensaje);
        _anim.Play(nombreAnimacion1);
        await Task.Delay(500 + duracion);
        _anim.Play(nombreAnimacion2);
        await Task.Delay(500);//Lo que tarda en terminar animación aprox
        Destroy(gameObject);
    }
}
