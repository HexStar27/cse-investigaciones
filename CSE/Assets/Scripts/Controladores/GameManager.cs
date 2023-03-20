using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static UnityEvent OnPause = new UnityEvent();
    public static UnityEvent OnUnpause = new UnityEvent();

    public static string user;
    private static bool isLoading;

    public static void CerrarAplicacion()
    {
        Application.Quit();
    }

    public static void CargarEscena(int escenaId)
    {
        SceneManager.LoadScene(escenaId);
    }

    public static IEnumerator CarganConPantallaDeCarga(int escenaId)
    {
        if (!isLoading)
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

    /* Código zombi...
    public void CargarEscenaConPCarga(int escenaId)
    {
        if(!isLoading) StartCoroutine(CarganConPantallaDeCarga(escenaId));
    }

    public static void Pausar()
    {
        OnPause.Invoke();
    }

    public static void Despausar()
    {
        OnUnpause.Invoke();
    }*/

}
