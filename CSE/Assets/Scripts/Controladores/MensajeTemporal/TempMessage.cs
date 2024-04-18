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

    public int extraDuration = 0;

    public async void EjecutarMensaje(string mensaje)
    {
        GameplayCycle.PauseGameplayCycle(true, "TempMessage");
        _texto.SetText("");

        _anim.Play(nombreAnimacion1);
        await Task.Delay(250);

        await ColocarLetrasConsecutivas(mensaje);
        await Task.Delay(250 + extraDuration);


        _anim.Play(nombreAnimacion2);
        await Task.Delay(500);

        GameplayCycle.PauseGameplayCycle(false, "TempMessage");
        Destroy(gameObject);
    }

    private async Task ColocarLetrasConsecutivas(string msg)
    {
        int n = msg.Length;
        int delay = duracion / n;
        for (int i = 0; i < n; i++)
        {
            _texto.text += msg[i];
            await Task.Delay(delay);
        }
    }
}
