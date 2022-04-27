/// Esta clase se va a encargar del inicio de sesión, y de mantener la sesión iniciada durante la partida.
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Hexstar
{
	public class SesionHandler : MonoBehaviour
	{
		public static string KEY = "";
		public static string ciphKey;
		public static string ciphIv;
		[SerializeField] private string key,iv;
		public static SesionHandler Instance { get; set; }

		/// <summary>
		/// Manda petición para obtener la KEY al servidor enviando el correo y la contraseña cifrada
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		public IEnumerator IniciarSesion(string email, string password)
		{
			string url = ConexionHandler.baseUrl + "login";
			WWWForm formulario = new WWWForm();
			formulario.AddField("username", email);
			formulario.AddField("password", Cifrar(password));
			yield return StartCoroutine(ConexionHandler.Post(url,formulario));
			SetKey(ConexionHandler.download);
		}

		private static void SetKey(string dh)
		{
			int s = dh.IndexOf("\"token\":\"");
			if(s < 0)
			{
				KEY = "";
				return;
			}
			s += 9;
			int i;
			for (i = s; i < dh.Length && (dh[i] != '\"' || dh[i] != '\''); i++){}
			KEY = dh.Substring(s,i-s-2);
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

		private void Awake()
		{
			if (Instance == null)
			{
				ciphIv = iv;
				ciphKey = key;
				Instance = this;
			}
			DontDestroyOnLoad(gameObject);
		}
	}
}