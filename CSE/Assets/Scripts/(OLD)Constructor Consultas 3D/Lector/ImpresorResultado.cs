using TMPro;
using UnityEngine;

public class ImpresorResultado : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textoResultado;

    public static ImpresorResultado Instancia { get; set; }

    public void IntroducirResultado(string r)
    {
        textoResultado.text = r;
    }

    private void Awake()
    {
        Instancia = this;
    }
}
