using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorInitializer : MonoBehaviour
{
    [SerializeField] Hexstar.CSE.SelectorPalabras selector;
    [SerializeField] Hexstar.CSE.CajonPistas cajon;
    private void Awake()
    {
        selector.transform.parent.gameObject.SetActive(true);
        selector.Instanciar();
        selector.transform.parent.gameObject.SetActive(false);

        cajon.transform.parent.gameObject.SetActive(true);
        cajon.transform.parent.gameObject.SetActive(false);
    }
}
