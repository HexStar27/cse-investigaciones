using UnityEngine;
using Hexstar;
using System.Collections;

public class MenuPartidaController : MonoBehaviour
{
    bool existePartida = false;
    private bool cerrojo = false;

    public void CargarPartidaGuardada()
    {
        if(existePartida)
        {
            GameManager.Instancia.CargarEscena(2);
        }
        else if (!cerrojo)
        {
            StartCoroutine(TryGetSavedFile());
        }
    }

    private void Start()
    {
        StartCoroutine(TryGetSavedFile());
    }

    private IEnumerator TryGetSavedFile()
    {
        cerrojo = true;
        PuntoGuardado pg;
        WWWForm form = new WWWForm();
        form.AddField("authorization", SesionHandler.KEY);
        form.AddField("user", GameManager.user);
        yield return StartCoroutine(ConexionHandler.Post(ConexionHandler.baseUrl+"load",form));
        string json = ConexionHandler.ExtraerJson(ConexionHandler.download);
        if (json.Length > 5) // Valor arbitrario lo suficientemente grande
        {
            pg = JsonConverter.PasarJsonAObjeto<PuntoGuardado>(json);
            OperacionesGameplay.Instancia.LoadSave(pg);
            Debug.Log("Archivo encontrado");
        }
        existePartida = json.Length > 5;
        cerrojo = false;
    }
}
