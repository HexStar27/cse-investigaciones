using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] GameObject _panel;
    [SerializeField] TextMeshProUGUI _texto;
    [SerializeField] string[] _tooltipsDisponibles;

    public void Set(int i)
    {
        if (i < 0 || i >= _tooltipsDisponibles.Length) return;
        _panel.SetActive(true);
        _texto.SetText(_tooltipsDisponibles[i]);
    }

    public void Abrir(bool value)
    {
        _panel.SetActive(value);
    }
}
