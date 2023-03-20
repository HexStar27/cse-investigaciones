using UnityEngine;

public class AlmacenTester : MonoBehaviour
{
    private void Start()
    {
        Hexstar.SesionHandler.Initialize();
        _ = Hexstar.SesionHandler.AIniciarSesion("ezequiel.lemoscardenas@alum.uca.es", "lem149");
    }

    public void DoSomething()
    {
        Hexstar.CSE.AlmacenDePalabras.CargarPalabras();
    }
}
