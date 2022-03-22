using UnityEngine;

public class BotonAyuda : MonoBehaviour
{
    public string linkAyudaSQL = "https://www.w3schools.com/sql/sql_examples.asp";

    public void AbrirPaginaDeAyuda()
    {
        Application.OpenURL(linkAyudaSQL);
    }
}
