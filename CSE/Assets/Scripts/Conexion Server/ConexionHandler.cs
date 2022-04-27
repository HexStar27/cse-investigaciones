using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Hexstar
{
    public class ConexionHandler : MonoBehaviour
    {
        public readonly static string baseUrl = "https://giibd.uca.es/";
        public static bool debugMode = true;

        public class DownloadEvent : UnityEvent<DownloadHandler> { }
        public static DownloadEvent onFinishRequest = new DownloadEvent();
        public static string download;

        public void GET(string url)
        {
            StartCoroutine(Get(url));
        }

        public void POST(string url, WWWForm form)
        {
            //Ej -> form.AddField("myField", "myData");
            StartCoroutine(Post(url, form));
        }

        public static IEnumerator Get(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (debugMode)
                {
                    if (request.isNetworkError) Debug.Log("Error: " + request.error);
                    else Debug.Log("Received: " + request.downloadHandler.text);
                }
                download = request.downloadHandler.text;
                onFinishRequest.Invoke(request.downloadHandler);
            }
        }

        public static IEnumerator Post(string url, WWWForm form)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                yield return request.SendWebRequest();

                if (debugMode)
                {
                    if (request.isNetworkError || request.isHttpError) Debug.Log(request.error);
                }
                download = request.downloadHandler.text;
                onFinishRequest.Invoke(request.downloadHandler);
            }
        }

        public static string ExtraerJson(string download)
        {
            int i;
            int n = download.Length;
            bool correcto;

            i = download.IndexOf("Correcto");
            correcto = i != -1 && i < 25; //Cuidao con el 25...

            if(correcto)
            {
                i = download.IndexOf("res");
                if (i != -1)
                {
                    i += 5;
                    n = n - i - 1;
                    return download.Substring(i, n);
                }
                else Debug.LogError("Ha habido un error leyendo \"res\" en la respuesta recibida");
            }
            else Debug.LogError("El servidor no ha aceptado la petición :(\n"+download);
            
            return "{}";
        }


        //Para ejemplo
        public class ClaseSucia
        {
            public int i = 0;
            public string h = "pepe";
            public float[] f = { 0f, 1f };
        }
        public void GetJsonExample(DownloadHandler descarga)
        {
            //Bastante OP
            ClaseSucia dic = JsonConverter.PasarJsonAObjeto<ClaseSucia>(descarga.text);
            Debug.Log(dic.i + "," + dic.h + ". " + dic.f[0]);
        }
    }
}