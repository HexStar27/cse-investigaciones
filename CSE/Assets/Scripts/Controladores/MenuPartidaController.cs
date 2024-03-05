using UnityEngine;
using Hexstar;
using CSE;

public class MenuPartidaController : MonoBehaviour
{
    public static bool existePartida = false;
    private static bool cerrojo = false;
    public static bool continuandoPartida = false;

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
        pg.CargarDatosAlSistema();
        existePartida = true;
        continuandoPartida = false;
        XAPI_Builder.CreateStatement_GameSession(true, true); // Starting session + new game
        GameManager.CargarEscena(GameManager.GameScene.ESCENA_PRINCIPAL);
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
        GameManager.CargarEscena(GameManager.GameScene.INICIO_SESION);
    }

    /// <summary>
    /// Try to load a saved file from the server. Will create a new file if none was found.
    /// </summary>
    /// <param name="loadScene"></param>
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
                continuandoPartida = true;
            }
            else //Si devuelve una cadena vacía significa que no ha encontrado un archivo de guardado.
            {
                pg = new PuntoGuardado();
                existePartida = false;
                continuandoPartida = false;
            }

            ResourceManager.checkpoint = pg;
            pg.CargarDatosAlSistema();

            cerrojo = false;
        }
        else continuandoPartida = true;

        if (loadScene)
        {
            GameManager.CargarEscena(GameManager.GameScene.ESCENA_PRINCIPAL);
            XAPI_Builder.CreateStatement_GameSession(true,!existePartida); // starting session
        }
    }

    private static async void GuardarPartidaEnServer()
    {
        cerrojo = true;
        string archivoGuardado = JsonConverter.ConvertirAJson(ResourceManager.checkpoint);
        WWWForm form = new WWWForm();
        form.AddField("authorization", SesionHandler.sessionKEY);
        form.AddField("user", GameManager.user);
        form.AddField("save", archivoGuardado);
        await ConexionHandler.APost(ConexionHandler.baseUrl +"save", form);
        string json = ConexionHandler.ExtraerJson(ConexionHandler.download);
        //Debug.Log("Respuesta:\n" + json);
        cerrojo = false;
    }
}
