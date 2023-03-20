﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ElementoMenuValor : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI pista;
    public class Evento : UnityEvent<string> { };
    public static Evento onToggle = new Evento();
    public Toggle toggle;
    public bool useCuotes = true;

    public void Activado() //Poner escuchando en toggle
    {
        string text = useCuotes ? "\"" + pista.text + "\"" : pista.text;
        if(toggle.isOn) onToggle?.Invoke(text);
    }
    public void SetText(string value) { pista.text = value; }
    
    private void OnEnable()
    {
        toggle.isOn = false;
    }
}