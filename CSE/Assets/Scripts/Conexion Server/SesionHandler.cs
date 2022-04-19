/// Esta clase se va a encargar del inicio de sesión, y de mantener la sesión iniciada durante la partida.
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Hexstar
{
	public class SesionHandler : MonoBehaviour
	{
		public static string KEY;
		public static string ciphKey;
		public static string ciphIv;

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
		public static string Cifrar(string texto)
		{
			AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
			AEScryptoProvider.BlockSize = 128;
			AEScryptoProvider.KeySize = 256;
			AEScryptoProvider.Key = Encoding.ASCII.GetBytes(ciphKey);
			AEScryptoProvider.IV = Encoding.ASCII.GetBytes(ciphIv);
			AEScryptoProvider.Mode = CipherMode.CBC;
			AEScryptoProvider.Padding = PaddingMode.PKCS7;

			byte[] txtByteData = Encoding.ASCII.GetBytes(texto);
			ICryptoTransform trnsfrm = AEScryptoProvider.CreateEncryptor(AEScryptoProvider.Key, AEScryptoProvider.IV);

			byte[] result = trnsfrm.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
			return Convert.ToBase64String(result);
		}
	}
}