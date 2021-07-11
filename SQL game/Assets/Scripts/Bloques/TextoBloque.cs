using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextoBloque : MonoBehaviour
{
    [SerializeField] TextMeshPro contenido;
    [SerializeField] string capaPorDefecto = "Elementos";

    private void OnEnable()
    {
        if (contenido == null) if (!TryGetComponent(out contenido)) contenido = gameObject.AddComponent<TextMeshPro>();
        contenido.sortingLayerID = SortingLayer.NameToID(capaPorDefecto);
    }

    public string Texto()
    {
        return contenido.text;
    }

    public void CambiarLayer(string layer)
    {
        contenido.sortingLayerID = SortingLayer.NameToID(layer);
    }
    public void Mostrar(bool mostrar)
    {
        contenido.enabled = mostrar;
    }
}
