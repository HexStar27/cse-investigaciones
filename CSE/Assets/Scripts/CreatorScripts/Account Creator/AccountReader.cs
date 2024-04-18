using Hexstar;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class AccountReader : MonoBehaviour
{
    public TMP_InputField path_to_CSV;
    public Button boton_cargar;
    [Space()]
    public TextMeshProUGUI texto_mensaje;
    public Button boton_enviar;
    public Button boton_confirmacion;
    public Button boton_no_confirmacion;
    public Transform zona_confirmacion;

    string[] correos;
    string[] contras;
    string[] nicks;

    [Space()]
    [SerializeField] bool test;

    private void Cargar()
    {
        if (!File.Exists(path_to_CSV.text))
        {
            texto_mensaje.SetText("El archivo no de la ruta indicada no existe.");
            boton_confirmacion.gameObject.SetActive(false);
            return;
        }

        List<Dictionary<string, object>> data = CSVReader.ReadString(File.ReadAllText(path_to_CSV.text));
        if (data == null)
        {
            texto_mensaje.SetText("No se pudo leer correctamente el archivo.");
            boton_confirmacion.gameObject.SetActive(false);
            return;
        }

        int n = data.Count;
        string[] ids = new string[n];
        correos = new string[n];
        contras = new string[n];
        nicks = new string[n];
        //print("Generando " + n + " cuentas.");
        for (int i = 0; i < n; i++)
        {
            //print(i);
            ids[i] = (string)data[i]["id"];
            correos[i] = (string)data[i]["correo"];
            if (!correos[i].Contains('@'))
            {
                correos[i] = ids[i] + "@correo";
                nicks[i] = "Player" + i.ToString() + DateTime.Now.ToShortDateString();
            }
            else
            {
                nicks[i] = correos[i].Split('@')[0];
            }

            if (data[i].ContainsKey("contra"))
            {
                contras[i] = (string)data[i]["contra"];
            }
            else contras[i] = "A" + ids[i].Substring(1, 6) + "B";
        }
        texto_mensaje.SetText( n+" cuentas listas para ser creadas.");
        boton_confirmacion.gameObject.SetActive(true);
    }

    private void Confirmacion()
    {
        zona_confirmacion.gameObject.SetActive(true);
        texto_mensaje.SetText("Esperando confirmación.");
    }

    private void NoEnviar()
    {
        zona_confirmacion.gameObject.SetActive(false);
        texto_mensaje.SetText(correos.Length + " cuentas listas para ser creadas.");
    }

    private async void Enviar()
    {
        int n = correos.Length;
        for (int i = 0; i < n && !test; i++)
        {
            texto_mensaje.SetText("Enviando "+(i+1)+" de "+n);
            await SesionHandler.ACrearCuenta(nicks[i], correos[i], contras[i]);
        }
        texto_mensaje.SetText(n + " cuentas creadas!");
        zona_confirmacion.gameObject.SetActive(false);
    }

    private void Awake()
    {
        SesionHandler.Initialize();
        boton_cargar.onClick.AddListener(Cargar);
        boton_enviar.onClick.AddListener(Enviar);
        boton_confirmacion.onClick.AddListener(Confirmacion);
        boton_no_confirmacion.onClick.AddListener(NoEnviar);

        boton_confirmacion.gameObject.SetActive(false);
        zona_confirmacion.gameObject.SetActive(false);
    }
}
