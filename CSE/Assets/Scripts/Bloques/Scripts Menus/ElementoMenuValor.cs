using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ElementoMenuValor : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI pista;
    [SerializeField] TMPro.TMP_InputField inputF;
    public class Evento : UnityEvent<string> { };
    public static Evento onToggle = new Evento();
    public Toggle toggle;
    public bool useCuotes = true;
    public bool useIField = false;

    public void Activado() //Poner escuchando en toggle
    {
        string valor = useIField ? inputF.text : pista.text;
        string text = useCuotes ? "\"" + valor + "\"" : pista.text;
        if(toggle.isOn) onToggle?.Invoke(text);
    }
    public void SetText(string value) { pista.text = value; }
    
    private void OnEnable()
    {
        toggle.isOn = false;
    }
}
