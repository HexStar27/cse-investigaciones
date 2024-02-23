using CSE.Local;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }
    [SerializeField] GameObject _panel;
    [SerializeField] TextMeshProUGUI _texto;
    [SerializeField] string[] _tooltipsDisponibles;

    public void Set(int i)
    {
        if (i < 0 || i >= _tooltipsDisponibles.Length) return;
        SetString(_tooltipsDisponibles[i]);
    }

    public void SetString(string text)
    {
        _panel.SetActive(true);
        _texto.SetText(text);
    }

    public void SetLocalizedString(string key)
    {
        _panel.SetActive(true);
        _texto.SetText(Localizator.GetString(key));
    }

    public void Abrir(bool value)
    {
        _panel.SetActive(value);
    }

    private void Awake()
    {
        Instance = this;
    }
}
