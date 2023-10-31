using UnityEngine;
using Hexstar;

public class MenuPartidaController : MonoBehaviour
{
    public static bool existePartida = false;
    private static bool cerrojo = false;

    // Cargar Partida
    public void CargarPartidaGuardada()
    {
        if (!cerrojo) TryGetSavedFile(existePartida);
    }
    public static void CargarPartidaGuardada_S()
    {
        if (!cerrojo) TryGetSavedFile(existePartida);
    }

    // Guardar Partida
    public void GuardarPartidaEnCurso()
    {
        if(!cerrojo) GuardarPartidaEnServer();
    }
    public static void GuardarPartidaEnCurso_S()
    {
        if (!cerrojo) GuardarPartidaEnServer();
    }

    // Crear Partida
    public void CrearPartidaNueva() { CrearPartidaNueva_S(); }
    public static void CrearPartidaNueva_S()
    {
        PuntoGuardado pg = new PuntoGuardado();
        ResourceManager.checkpoint = pg;
        pg.Cargar();
        existePartida = true;
        GameManager.CargarEscena(2);
    }


    private void Start()
    {
        TryGetSavedFile(false);
    }

    public void Salir()
    {
        GameManager.CerrarAplicacion();
    }

    public void CerrarSesion()
    {
        SesionHandler.ResetSesionValues();
        GameManager.CargarEscena(0);
    }

    private static async void TryGetSavedFile(bool loadScene)
    {
        if(!existePartida)
        {
            cerrojo = true;
            PuntoGuardado pg;
            WWWForm form = new WWWForm();
            form.AddField("authorization", SesionHandler.sessionKEY);
            form.AddField("user", GameManager.user);
            await ConexionHandler.APost(ConexionHandler.baseUrl + "load", form);
            string json = ConexionHandler.ExtraerJson(ConexionHandler.download);
            if (json.Length > 5) // Valor arbitrario lo suficientemente grande
            {
                pg = JsonConverter.PasarJsonAObjeto<PuntoGuardado>(json);
                existePartida = true;
            }
            else //Si devuelve una cadena vacía significa que no ha encontrado un archivo de guardado.
            {
                pg = new PuntoGuardado();
                existePartida = false;
            }

            ResourceManager.checkpoint = pg;
            pg.Cargar();

            cerrojo = false;
        }

        if(loadScene) GameManager.CargarEscena(2);
    }

    private static async void GuardarPartidaEnServer()
    {
        cerrojo = true;
        WWWForm form = new WWWForm();
        form.AddField("authorization", SesionHandler.sessionKEY);
        form.AddField("user", GameManager.user);
        form.AddField("save", JsonConverter.ConvertirAJson(ResourceManager.checkpoint));
        await ConexionHandler.APost(ConexionHandler.baseUrl +"save", form);
        string json = ConexionHandler.ExtraerJson(ConexionHandler.download);
        //Debug.Log("Respuesta:\n" + json);
        cerrojo = false;
    }
}
