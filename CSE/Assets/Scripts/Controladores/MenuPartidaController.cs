using UnityEngine;
using Hexstar;
using System.Collections;

public class MenuPartidaController : MonoBehaviour
{
    public bool existePartida = false;
    private bool cerrojo = false;

    public void CargarPartidaGuardada()
    {
        if (!cerrojo)
        {
            StartCoroutine(TryGetSavedFile(existePartida));
        }
    }

    private void Start()
    {
        StartCoroutine(TryGetSavedFile(false));
    }

    public void CrearPartidaNueva()
    {
        PuntoGuardado pg = new PuntoGuardado();
        ResourceManager.checkpoint = pg;
        pg.Cargar();
        existePartida = true;
        GameManager.Instancia.CargarEscena(2);
    }

    private IEnumerator TryGetSavedFile(bool loadScene)
    {
        if(!existePartida)
        {
            cerrojo = true;
            PuntoGuardado pg;
            WWWForm form = new WWWForm();
            form.AddField("authorization", SesionHandler.KEY);
            form.AddField("user", GameManager.user);
            yield return StartCoroutine(ConexionHandler.Post(ConexionHandler.baseUrl + "load", form));
            string json = ConexionHandler.ExtraerJson(ConexionHandler.download);

            if (json.Length > 5) // Valor arbitrario lo suficientemente grande
            {
                pg = JsonConverter.PasarJsonAObjeto<PuntoGuardado>(json);
                Debug.Log("Archivo encontrado");
            }
            else pg = new PuntoGuardado();

            ResourceManager.checkpoint = pg;
            pg.Cargar();
            existePartida = json.Length > 5;

            cerrojo = false;
        }

        if(loadScene) GameManager.Instancia.CargarEscena(2);
    }

    public IEnumerator GuardarPartidaEnServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("authorization", SesionHandler.KEY);
        form.AddField("user", GameManager.user);
        form.AddField("save", JsonConverter.ConvertirAJson(ResourceManager.checkpoint));
        yield return StartCoroutine(ConexionHandler.Post(ConexionHandler.baseUrl +"/save", form));
    }
}
