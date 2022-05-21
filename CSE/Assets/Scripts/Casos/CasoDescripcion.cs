using UnityEngine;
using TMPro;

public class CasoDescripcion : MonoBehaviour
{
    [SerializeField] GameObject _panel;
    [SerializeField] TextMeshProUGUI _titulo;
    [SerializeField] TextMeshProUGUI _resumen;
    [SerializeField] TextMeshProUGUI _efectos;


    public void LeerCaso(Caso c)
    {
        _titulo.SetText(c.titulo);
        _resumen.SetText(c.resumen);

        //Quedaría poner los efectos
        _efectos.SetText("WIP:\n-Próximamente\n-Efectos super xulos\n-Recompensas\n\nY mucho más.");
    }

    public void Abrir(bool value)
    {
        _panel.SetActive(value);
    }
}
