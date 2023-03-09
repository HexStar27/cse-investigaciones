using UnityEngine;

public class ConfiguradorBloqueValor : MonoBehaviour
{
    [SerializeField] int menuIndex = 1;
    [SerializeField] TMPro.TextMeshPro textoBloque;
    Hexstar.CSE.BlockMovAndConexion bloque;

    public void CambiarTexto(string value)
    {
        textoBloque.text = value;
    }

    private void OnMouseUpAsButton()
    {
        if(bloque.IsClick())
        {
            SelectorDeMenus.Instance.SeleccionarMenu(menuIndex);
            if (menuIndex == 1) //Valor
            {
                ControladorMenuBloqueValue.Instance.SeleccionarBloqueAConfigurar(this);
            }
            if (menuIndex == 2) //Tabla
            {
                ControladorMenuBloqueTabla.Instance.SeleccionarBloqueAConfigurar(this);
                ControladorMenuBloqueTabla.Instance.AbrirMenuTablas();
            }
            if(menuIndex == 3) //Columna
            {
                ControladorMenuBloqueTabla.Instance.SeleccionarBloqueAConfigurar(this);
                ControladorMenuBloqueTabla.Instance.AbrirMenuColumnas();
            }
        }
    }
    private void Awake()
    {
        bloque = GetComponent<Hexstar.CSE.BlockMovAndConexion>();
    }
}
