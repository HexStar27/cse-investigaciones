using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    public string logInUri = "localhost:8081/login";
    public string signInUri = "localhost:8081/signin";
    UnityWebRequest serverRequest;
    private bool isRequesting = false;
    private int estado = 0;

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

    IEnumerator IniciarSesion(string correo, string contra)
    {
        isRequesting = true; //Cierro cerrojo
        estado = 0;

        bool errorDeConexion = false;

        if (correo.Contains("@uca.es") || correo.Contains("@alum.uca.es"))
        {
            //Enviando formulario a servidor para comprobar si se encuentra el correo
            WWWForm formulario = new WWWForm();
            formulario.AddField("correo", correo);
            string hash = CrearHash(contra);
            formulario.AddField("hash", hash);

            string respuesta; //Espero la respuesta del servidor
            using (serverRequest = UnityWebRequest.Post(logInUri, formulario))
            {
                yield return serverRequest.SendWebRequest();

                if (serverRequest.isNetworkError) errorDeConexion = true;
                
                respuesta = serverRequest.downloadHandler.text;
            }

            if (respuesta != "2") //Si no da el error correo malo
            {
                if (respuesta != "4") //Si no da el error contraseña mala
                {
                    estado = 0;
                    GameManager.user = correo;
                    //var guardado = DescargarArchivoGuardado(correo);
                    //CargarDatos(guardado);
                }
                else estado = 4;
            }
            else estado = 2;
        }
        else estado = 1;
        isRequesting = false; //Abro cerrojo

        if (errorDeConexion) estado = 6;

        ////DEBUG////
        //Estado(true);
        /////////////
        
        
        gm.CargarEscena(1);
    }

    /// <summary>
    /// Se encarga de registrar el jugador en el servidor siempre y cuando
    /// no esté ya en él y la contraseña sea segura
    /// </summary>
    public void RegistrarJugador()
    {
        //Sólo intentará registrarse si el cerrojo está abierto
        if (!isRequesting)
            StartCoroutine(RegistrarJugador(textoCorreo.text, textoContra.text));
    }

    IEnumerator RegistrarJugador(string correo, string contra)
    {
        isRequesting = true; //Cierro cerrojo
        estado = 0;

        bool errorDeConexion = false;

        if (!terminosCondiciones.isOn) estado = 5;
        else if (correo.Contains("@uca.es") || correo.Contains("@alum.uca.es"))
        {
            //Enviando formulario a servidor para comprobar si se encuentra el correo
            WWWForm formulario = new WWWForm();
            formulario.AddField("correo", correo);
            string hash = CrearHash(contra);
            formulario.AddField("hash", hash);

            string respuesta; //Espero la respuesta del servidor
            using (serverRequest = UnityWebRequest.Post(logInUri, formulario))
            {
                yield return serverRequest.SendWebRequest();

                if (serverRequest.isNetworkError) errorDeConexion = true;
                
                respuesta = serverRequest.downloadHandler.text;
            }


            if (respuesta == "2") //Ese correo no está en el servidor
            {
                if(esBuenaContra(contra))
                {
                    //Se añade un nuevo usuario con el correo y el hash obtenido anteriormente;
                    using (serverRequest = UnityWebRequest.Post(signInUri, formulario))
                    {
                        yield return serverRequest.SendWebRequest();

                        if (serverRequest.isNetworkError)
                        {
                            Debug.Log(serverRequest.error);
                            errorDeConexion = true;
                        }
                        respuesta = serverRequest.downloadHandler.text;
                    }
                    estado = 0;
                }
                else estado = 4;
            }
            else estado = 3;
        }
        else estado = 1;


        isRequesting = false; //Abro cerrojo

        if (errorDeConexion) estado = 6;

        ////DEBUG////
        //Estado(true);
        /////////////

        gm.CargarEscena(1);
    }

    private bool esBuenaContra(string contra)
    {
        bool veredicto = true;
        if (contra.Length < 8) veredicto = false;
        else
        {//¿Contraseña tiene mayúsculas, minúsculas y números?
            bool mayusc = false, minusc = false, nume = false; //esp = false;
            foreach (char letra in contra)
            {
                if (letra >= 97 && letra <= 122) minusc = minusc || true;
                else if (letra >= 65 && letra <= 90) mayusc = mayusc || true;
                else if (letra >= 48 && letra <= 57) nume = nume || true;
                //else esp = esp || true; //Activar si se exige un grado más de seguridad en las contraseñas
            }
            veredicto = mayusc && minusc && nume;
        }

        return veredicto;
    }

    public string CrearHash(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    public int Estado(bool verboseConsole)
    {
        if (verboseConsole)
            switch(estado)
            {
                case 0:
                    Debug.Log("0: La operación ha terminado con éxito.");
                    break;
                case 1:
                    Debug.Log("1: El correo no pertenece a una cuenta de la UCA.");
                    break;
                case 2:
                    Debug.Log("2: El correo no se encuentra registrado en el servidor.");
                    break;
                case 3:
                    Debug.Log("3: El correo ya estaba registrado en el servidor.");
                    break;
                case 4:
                    Debug.Log("4: La contraseña introducida no es válida.");
                    break;
                case 5:
                    Debug.Log("5: DEBES ACEPTAR LOS TÉRMINOS Y CONDICIONES.");
                    break;
                case 6:
                    Debug.Log("6: ¿¿¿No hay internet???");
                    break;
                default:
                    Debug.Log("?: Er█«r Ð░sco┤ociþo");
                    break;
            }
        return estado;
    }
}
