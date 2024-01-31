using UnityEngine;
using UnityEngine.Events;

public class ConfiguradorBloqueValor : MonoBehaviour
{
    [SerializeField] int menuIndex = 1;
    [SerializeField] TMPro.TextMeshPro textoBloque;
    Hexstar.CSE.BlockMovAndConexion bloque;
    
    public class STRINGEVENT : UnityEvent<string> { }
    public STRINGEVENT onTextChanged = new STRINGEVENT();

    public void CambiarTexto(string value)
    {
        textoBloque.text = value;
        onTextChanged?.Invoke(value);

        CSE.XAPI_Builder.CreateStatement_BlockAction(CSE.XAPI_Builder.BlockAction.MODIFIED, bloque.GetBlockTitle());
    }

    public bool FreeOfAS()
    {
        var d = bloque.GetBloqueDerecho();
        if (d == null) return true;
        return d.CQU.GetBlockTextContent() != "AS ";
    }

    public string GetText() { return textoBloque.text; }

    private void OnMouseUpAsButton()
    {
        if(bloque.IsClick())
        {
            SelectorDeMenus.Instance.SeleccionarMenu(menuIndex);
            if (menuIndex == 1) //Valor
            {
                ControladorMenuBloqueValue.Instance.SeleccionarBloqueAConfigurar(this);
            }
            else if (menuIndex == 2) //Tabla
            {
                ControladorMenuBloqueTabla.Instance.SeleccionarBloqueAConfigurar(this);
                ControladorMenuBloqueTabla.Instance.AbrirMenuTablas();
            }
            else if (menuIndex == 3) //Columna
            {
                ControladorMenuBloqueTabla.Instance.SeleccionarBloqueAConfigurar(this);
                ControladorMenuBloqueTabla.Instance.AbrirMenuColumnas();
            }
            else if (menuIndex == 4) // AS
            {
                ControladorMenuBloqueAS.Instance.SeleccionarBloqueAConfigurar(this);
            }
        }
    }
    public Hexstar.CSE.BlockMovAndConexion GetBloque() => bloque;

    private void Awake()
    {
        bloque = GetComponent<Hexstar.CSE.BlockMovAndConexion>();
    }
}
