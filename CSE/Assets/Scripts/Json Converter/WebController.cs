using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebController : MonoBehaviour
{
    public delegate void resultadoBusqueda(string texto);

    public IEnumerator GetRequest(string uri, resultadoBusqueda resultado)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError) resultado("Connection Error: " + webRequest.error);
            else if (webRequest.result == UnityWebRequest.Result.ProtocolError) resultado("HTTP Error: " + webRequest.error);
            else resultado(webRequest.downloadHandler.text);

            webRequest.Dispose(); //No se si debo de ponerlo...
        }
    }

    public IEnumerator SendPost(string uri, WWWForm formulario, resultadoBusqueda resultado)
    {
        yield return null;
    }
}
