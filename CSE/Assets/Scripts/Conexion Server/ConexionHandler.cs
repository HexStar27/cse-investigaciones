using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Runtime.CompilerServices;
using System;
using System.Threading.Tasks;

namespace Hexstar
{
    public static class ConexionHandler
    {
        public readonly static string defaultBaseUrl = "https://cse.uca.es/game/";
        public static string baseUrl = "https://cse.uca.es/game/";
        public static bool debugMode = false;

        public class DownloadEvent : UnityEvent<DownloadHandler> { }
        public static DownloadEvent onFinishRequest = new DownloadEvent();
        public static string download;
        public static UnityWebRequest.Result result;

        public static async Task AGet(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();

                if (debugMode)
                {
                    if (request.result == UnityWebRequest.Result.ConnectionError) Debug.Log("Error: " + request.error);
                    else Debug.Log("Received: " + request.downloadHandler.text);
                }
                download = request.downloadHandler.text;
                result = request.result;
                onFinishRequest.Invoke(request.downloadHandler);
            }
        }

        public static async Task APost(string url, WWWForm form)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                await request.SendWebRequest();

                if (debugMode)
                {
                    if (request.result == UnityWebRequest.Result.ConnectionError ||
                        request.result == UnityWebRequest.Result.ProtocolError) Debug.Log(request.error);
                }
                download = request.downloadHandler.text;
                result = request.result;
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
            else Debug.LogWarning("El servidor no ha aceptado la petición :(\n"+download);
            
            return "{}";
        }
    }

    internal class UnityWebRequestAwaiter : INotifyCompletion
    {
        private UnityWebRequestAsyncOperation asyncOp;
        private Action continuation;

        public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
        {
            this.asyncOp = asyncOp;
            asyncOp.completed += OnRequestCompleted;
        }

        public bool IsCompleted { get { return asyncOp.isDone; } }
        public void GetResult() { } // No se usa pero es requerido por la interfaz INotifyCompletion.
        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
        private void OnRequestCompleted(AsyncOperation obj)
        {
            continuation();
        }
    }
    internal static class ExtensionMethods
    {
        internal static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
}
