/// Esta clase se va a encargar del inicio de sesión, y de mantener la sesión iniciada durante la partida.
using UnityEngine;
using UnityEngine.Networking;

namespace Hexstar
{
	public class SesionHandler : MonoBehaviour
	{
		public static string KEY;

		/// <summary>
		/// Manda petición para obtener la KEY al servidor enviando el correo y la contraseña cifrada
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		public void IniciarSesion(string email, string password)
		{
			string url = ConexionHandler.baseUrl + "login";
			WWWForm formulario = new WWWForm();
			formulario.AddField("username", email);
			formulario.AddField("password", Cifrar(password));
			ConexionHandler.onFinishRequest.AddListener(SetKey);
			StartCoroutine(ConexionHandler.Post(url,formulario));
		}

		private static void SetKey(DownloadHandler dh)
		{
			KEY = dh.text;
			ConexionHandler.onFinishRequest.RemoveListener(SetKey);
		}

		/// <summary>
		/// TODO: Cifra un mensaje :v
		/// </summary>
		/// <param name="texto"></param>
		/// <returns></returns>
		private static string Cifrar(string texto)
		{
			return texto;
		}
	}
}