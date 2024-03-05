using Hexstar;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CreadorDeCuentas : MonoBehaviour
{
    // Start is called before the first frame update
    public TextAsset csv;
    string[] correos;
    string[] contras;
    string[] nicks;

    public bool send = true;

    void Start()
    {
        SesionHandler.Initialize();
        Generate();
        if(send) Send();
    }

    private void Generate()
    {
        List<Dictionary<string, object>> data = CSVReader.Read(csv);
        if (data == null) Debug.LogError("File not found.");

        int n = data.Count;
        string[] ids = new string[n];
        correos = new string[n];
        contras = new string[n];
        nicks = new string[n];
        print("Generando "+n+" cuentas.");
        for (int i = 0; i < n; i++)
        {
            print(i);
            ids[i] = (string)data[i]["id"];
            correos[i] = (string)data[i]["correo"];
            if (!correos[i].Contains('@'))
            {
                correos[i] = ids[i] + "@correo";

                string d = DateTime.Now.ToShortDateString();
                var sep = d.Split(new[] { '-', '/' });
                d = "";
                for (int j = 0; j < sep.Length; j+=2) d += sep[j];
                nicks[i] = "Player"+i.ToString()+DateTime.Now.ToShortDateString();
            }
            else
            {
                nicks[i] = correos[i].Split('@')[0];
            }
            contras[i] = "A" + ids[i].Substring(1, 6) + "B";
        }
    }

    private async void Send()
    {
        int n = correos.Length;
        for(int i = 0; i < n; i++)
        {
            await SesionHandler.ACrearCuenta(nicks[i], correos[i], contras[i]);
            print("Cuenta "+i+" creada");
        }
        print("Listo.");
    }
}
