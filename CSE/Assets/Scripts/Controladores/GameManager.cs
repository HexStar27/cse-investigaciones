using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static UnityEvent OnPause = new UnityEvent();
    public static UnityEvent OnUnpause = new UnityEvent();

    public enum GameScene { INICIO_SESION=0, MENU_PARTIDA=1, ESCENA_PRINCIPAL=2 };

    public static string user =  "Unidentified User";
    private static bool isLoading;

    public static void CerrarAplicacion()
    {
        Application.Quit();
    }

    public static void CargarEscena(GameScene escena)
    {
        SceneManager.LoadScene((int)escena);
    }

    public static IEnumerator CarganConPantallaDeCarga(int escenaId) // No se usa
    {
        if (!isLoading)
        {
            isLoading = true;
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(escenaId);
            yield return new WaitUntil(() => asyncLoad.isDone);

            isLoading = false;
        }
    }
}
