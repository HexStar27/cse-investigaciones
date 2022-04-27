using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Hexstar;

public class MainMenuOp : MonoBehaviour
{
    public GameManager gm;

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

    /// <summary>
    /// Se encarga de iniciar sesión en el servidor para poder descargar
    /// la partida guardada en este.
    /// </summary>
    public void IniciarSesion()
    {
        //Sólo intentará iniciar sesión si el cerrojo está abierto
        if (!isRequesting)
            StartCoroutine(IniciarSesion(textoCorreo.text, textoContra.text));
    }

    public IEnumerator IniciarSesion(string correo, string contra)
    {
        isRequesting = true; //Cierro cerrojo

        //Enviando formulario a servidor para comprobar si se encuentra el correo
        yield return StartCoroutine(SesionHandler.Instance.IniciarSesion(correo,contra));
        bool exito = SesionHandler.KEY != "";
        if (exito) //Si hemos recibido una KEY, entonces el inicio de sesión es correcto
        {
            GameManager.user = correo;
            isRequesting = false; //Abro cerrojo
            yield return StartCoroutine(VAMONOOO());
            gm.CargarEscena(1);
        }
        else
        {
            isRequesting = false; //Abro cerrojo
            onError.Invoke();
        }
    }

    private IEnumerator VAMONOOO()
    {
        WaitForSeconds tiempo = new WaitForSeconds(0.75f);
        onSuccess.Invoke();
        yield return tiempo;
    }
}
