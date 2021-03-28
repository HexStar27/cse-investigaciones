using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//Este debería ser MonoSingleton
public class GameManager : MonoBehaviour
{
    public static string user;

    private bool isLoading;

    public void CerrarAplicacion()
    {
        Application.Quit();
    }

    public void CargarEscena(int escenaId)
    {
        SceneManager.LoadScene(escenaId);
    }

    public void CargarEscenaConPCarga(int escenaId)
    {
        if(!isLoading) StartCoroutine(CarganConPantallaDeCarga(escenaId));
    }

    IEnumerator CarganConPantallaDeCarga(int escenaId)
    {
        isLoading = true;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(escenaId);
        
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            //Poner aquí todo lo que controla la pantalla de carga
            yield return null;
        }
        
        isLoading = false;
    }
}
