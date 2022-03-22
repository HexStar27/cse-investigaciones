using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace Hexstar
{
    public class ConexionHandler : MonoBehaviour
    {
        public readonly static string baseUrl = "http://giidb.uca.es:443/";
        public static bool debugMode = false;

        public class DownloadEvent : UnityEvent<DownloadHandler> { }
        public static DownloadEvent onFinishRequest = new DownloadEvent();

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
                    else Debug.Log("Form upload complete!");
                }
                onFinishRequest.Invoke(request.downloadHandler);
            }
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