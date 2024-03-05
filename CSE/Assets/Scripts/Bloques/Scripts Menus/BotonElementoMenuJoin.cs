using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//Script para el elemento operador del menú de opciones para operadores
public class BotonElementoMenuJoin : MonoBehaviour
{
    [SerializeField] Button a;
    [SerializeField] TMPro.TextMeshProUGUI texto;
    [SerializeField] Image img;
    public class EVENTO : UnityEvent<string> { };
    public EVENTO evento = new EVENTO();
    public void Setup(string val, Sprite spr) 
    { 
        texto.text = val;
        img.sprite = spr;
    }

    private void Awake()
    {
        a.onClick.AddListener(()=>evento.Invoke(texto.text));
    }
}
