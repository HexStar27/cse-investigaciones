using UnityEngine;
using TMPro;

public class CasoDescripcion : MonoBehaviour
{
    [SerializeField] GameObject _panel;
    [SerializeField] TextMeshProUGUI _titulo;
    [SerializeField] TextMeshProUGUI _resumen;
    [SerializeField] TextMeshProUGUI _efectos;
    [SerializeField] HighScoreTable _panelPuntuaciones;

    GameObject _puntuacionesParent;

    private void Awake()
    {
        _puntuacionesParent = _panelPuntuaciones.transform.parent.gameObject;
    }

    public void LeerCaso(Caso c, int idx)
    {
        _titulo.SetText(c.titulo);
        _resumen.SetText(c.resumen);

        //Quedaría poner los efectos
        _efectos.SetText("WIP:\n-Próximamente\n-Efectos super xulos\n-Recompensas\n\nY mucho más.");

        int from = 0;
        if (idx > 0) from = PuzzleManager.Instance.puntuacionesPorCaso[idx - 1];
        _panelPuntuaciones.ShowOnlyRange(from, PuzzleManager.Instance.puntuacionesPorCaso[idx]);
    }

    public void Abrir(bool value)
    {
        _panel.SetActive(value);
        _puntuacionesParent.SetActive(value);
    }
}
