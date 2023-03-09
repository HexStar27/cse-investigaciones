using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Hexstar;
using System.Threading.Tasks;

public class MainMenuOp : MonoBehaviour
{
    [Header("Botones de interfaz")]
#pragma warning disable 0649
    [SerializeField]
    private Toggle terminosCondiciones;
    [SerializeField]
    private TMP_InputField textoCorreo;
    [SerializeField]
    private InputField textoContra;
#pragma warning restore 0649

    [Header("Conexión a servidor")]
    //Sobre la conexión al servidor
    private bool isRequesting = false;

    [System.Serializable] public class OnAlgo : UnityEvent{};
    public OnAlgo onError;
    public OnAlgo onSuccess;

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
        bool exito = SesionHandler.sessionKEY != "";
        if (exito) //Si hemos recibido una KEY, entonces el inicio de sesión es correcto
        {
            GameManager.user = correo;
            isRequesting = false; //Abro cerrojo
            onSuccess.Invoke();
            await Task.Delay(750); //Tiempo para ejecutar animación de credenciales aceptadas.
            GameManager.CargarEscena(1);
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

    public void Salir()
    {
        GameManager.CerrarAplicacion();
    }
}
