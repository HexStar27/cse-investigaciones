using UnityEngine;
using Hexstar;
using System.Collections;
using System.Threading.Tasks;

public class MenuPartidaController : MonoBehaviour
{
    public static bool existePartida = false;
    private static bool cerrojo = false;

    // Cargar Partida
    public void CargarPartidaGuardada()
    {
        if (!cerrojo)
        {
            TryGetSavedFile(existePartida);
        }
    }
    public static void CargarPartidaGuardada_S()
    {
        if (!cerrojo)
        {
            TryGetSavedFile(existePartida);
        }
    }

    // Guardar Partida
    public void GuardarPartidaEnCurso()
    {
        if(!cerrojo)
        {
            GuardarPartidaEnServer();
        }
    }

    public static void GuardarPartidaEnCurso_S()
    {
        if (!cerrojo)
        {
            GuardarPartidaEnServer();
        }
    }

    // Crear Partida
    public void CrearPartidaNueva()
    {
        PuntoGuardado pg = new PuntoGuardado();
        ResourceManager.checkpoint = pg;
        pg.Cargar();
        existePartida = true;
        GameManager.Instancia.CargarEscena(2);
    }

    public static void CrearPartidaNueva_S()
    {
        PuntoGuardado pg = new PuntoGuardado();
        ResourceManager.checkpoint = pg;
        pg.Cargar();
        existePartida = true;
        GameManager.Instancia.CargarEscena(2);
    }


    private void Start()
    {
        TryGetSavedFile(false);
    }

    private static async void TryGetSavedFile(bool loadScene)
    {
        if(!existePartida)
        {
            cerrojo = true;
            PuntoGuardado pg;
            WWWForm form = new WWWForm();
            form.AddField("authorization", SesionHandler.KEY);
            form.AddField("user", GameManager.user);
            await ConexionHandler.APost(ConexionHandler.baseUrl + "load", form);
            string json = ConexionHandler.ExtraerJson(ConexionHandler.download);

            if (json.Length > 5) // Valor arbitrario lo suficientemente grande
            {
                pg = JsonConverter.PasarJsonAObjeto<PuntoGuardado>(json);
            }
            else
            {
                pg = new PuntoGuardado();
            }

            ResourceManager.checkpoint = pg;
            pg.Cargar();
            existePartida = json.Length > 5;

            cerrojo = false;
        }

        if(loadScene) GameManager.Instancia.CargarEscena(2);
        await Task.Yield();
    }

    private static async void GuardarPartidaEnServer()
    {
        cerrojo = true;
        WWWForm form = new WWWForm();
        form.AddField("authorization", SesionHandler.KEY);
        form.AddField("user", GameManager.user);
        form.AddField("save", JsonConverter.ConvertirAJson(ResourceManager.checkpoint));
        await ConexionHandler.APost(ConexionHandler.baseUrl +"save", form);
        cerrojo = false;
    }
}
