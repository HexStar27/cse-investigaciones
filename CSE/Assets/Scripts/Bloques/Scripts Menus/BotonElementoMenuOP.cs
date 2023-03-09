using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//Script para el elemento operador del menú de opciones para operadores
public class BotonElementoMenuOP : MonoBehaviour
{
    [SerializeField] Button a;
    [SerializeField] TMPro.TextMeshProUGUI texto;
    public class EVENTO : UnityEvent<string> { };
    public EVENTO evento = new EVENTO();
    public void SetTXT(string val) { texto.text = val; }

    private void Awake()
    {
        a.onClick.AddListener(()=>evento.Invoke(texto.text));
    }
}
