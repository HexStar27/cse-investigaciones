using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Hexstar;
using System.Threading.Tasks;
using ResultType = UnityEngine.Networking.UnityWebRequest.Result;

public class MainMenuOp : MonoBehaviour
{
    [Header("Botones de interfaz")]
#pragma warning disable 0649
    [SerializeField] private Toggle terminosCondiciones;
    [SerializeField] private TMP_InputField textoCorreo;
    [SerializeField] private InputField textoContra;
    [SerializeField] private TMP_InputField textNick;
    [SerializeField] private Transform conexionFallidaMSG;
    [SerializeField] private TMP_InputField server_selector;
#pragma warning restore 0649

    [Header("Conexión a servidor")]
    //Sobre la conexión al servidor
    private bool isRequesting = false;

    [System.Serializable] public class OnAlgo : UnityEvent{};
    public OnAlgo onError;
    public OnAlgo onSuccess;

    private static readonly string _serverUrl = "ServerUrl";

    private void Awake()
    {
        ConexionHandler.baseUrl = PlayerPrefs.GetString(_serverUrl, ConexionHandler.defaultBaseUrl);
        server_selector.text = ConexionHandler.baseUrl;
        server_selector.onEndEdit.AddListener(EstablecerServidor);
    }

    private void Start()
    {
        SesionHandler.Initialize(); //Setup cipher stuff
    }

    /// <summary>
    /// Se encarga de iniciar sesión en el servidor para poder descargar
    /// la partida guardada en este.
    /// </summary>
    public async void IniciarSesion(string correo, string contra)
    {
        if (isRequesting) return;
        isRequesting = true; //Cierro cerrojo

        //Enviando formulario a servidor para comprobar si se encuentra el correo
        await SesionHandler.AIniciarSesion(correo,contra);
        CheckConnection();
        bool exito = SesionHandler.sessionKEY != "";
        if (exito) //Si hemos recibido una KEY, entonces el inicio de sesión es correcto
        {
            GameManager.user = correo;
            isRequesting = false; //Abro cerrojo
            onSuccess.Invoke();
            await Task.Delay(750); //Tiempo para ejecutar animación de credenciales aceptadas.
            GameManager.CargarEscena(GameManager.GameScene.MENU_PARTIDA);
        }
        else
        {
            isRequesting = false; //Abro cerrojo
            onError.Invoke();
        }
    }

    public void IniciarSesionButtonFriendly()
    {
        //El SesionHandler se encargará de cifrar la contraseña así que se la pasamos sin cifrar aquí.
        IniciarSesion(textoCorreo.text, textoContra.text);
    }

    public async void CrearCuenta()
    {
        if (isRequesting) return;
        isRequesting = true; //Cierro cerrojo

        string correo = textoCorreo.text;
        await SesionHandler.ACrearCuenta(textNick.text, correo, textoContra.text);
        CheckConnection();
        bool exito = SesionHandler.sessionKEY != "";
        if (exito)
        {
            GameManager.user = correo;
            isRequesting = false; //Abro cerrojo
            onSuccess.Invoke();
            await Task.Delay(750); //Tiempo para ejecutar animación de credenciales aceptadas.
            GameManager.CargarEscena(GameManager.GameScene.MENU_PARTIDA);
        }
        else
        {
            isRequesting = false; //Abro cerrojo
            onError.Invoke();
        }
    }

    public void CheckConnection()
    {
        conexionFallidaMSG.gameObject.SetActive(ConexionHandler.result == ResultType.ConnectionError);
    }

    public void Salir()
    {
        GameManager.CerrarAplicacion();
    }

    private void EstablecerServidor(string url)
    {
        if (url == "") url = ConexionHandler.defaultBaseUrl;
        if (!url.EndsWith('/')) url += "/";
        if (!url.EndsWith("game/")) url += "game/";
        ConexionHandler.baseUrl = url;
        server_selector.SetTextWithoutNotify(url);
        PlayerPrefs.SetString(_serverUrl, url);
    }
}
