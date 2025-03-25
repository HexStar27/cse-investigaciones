// Esta clase se va a encargar del inicio de sesión, y de mantener la sesión iniciada durante la partida.
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xAPI = CSE.XAPI_Builder;

namespace Hexstar
{
	public static class SesionHandler
	{
		public static string sessionKEY = "";
		public static string ciphKey;
		public static string ciphIv;

		public static string email;
		public static string nickname;

		/// <summary>
		/// Manda petición para obtener la KEY al servidor enviando el correo y la contraseña cifrada
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		public static async Task AIniciarSesion(string email, string password)
        {
			string url = ConexionHandler.baseUrl + "login";
			WWWForm formulario = new WWWForm();
			formulario.AddField("username", email);
			formulario.AddField("password", Cifrar(password));
			await ConexionHandler.APost(url, formulario);
			SetKey(ConexionHandler.download);
			SesionHandler.email = email;
		}

		public static void ResetSesionValues()
        {
			email = null;
			sessionKEY = null;
			nickname = null;
        }

		public static async Task ACrearCuenta(string nick, string email, string password)
        {
			string url = ConexionHandler.baseUrl + "signin";
			WWWForm formulario = new WWWForm();
			formulario.AddField("nickname",nick);
			formulario.AddField("email", email);
			formulario.AddField("password", Cifrar(password));
			await ConexionHandler.APost(url, formulario);
			SetKey(ConexionHandler.download);
			SesionHandler.email = email;
		}

		public static async Task GetNickname()
		{
			string url = ConexionHandler.baseUrl + "nickname";
			WWWForm formulario = new WWWForm();
			formulario.AddField("authorization", sessionKEY);
			formulario.AddField("email", email);
			await ConexionHandler.APost(url, formulario);
			nickname = ConexionHandler.ExtraerJson(ConexionHandler.download);
			nickname = nickname.Trim('"');
			xAPI.AutoSetupActor();
        }

		private static void SetKey(string dh)
		{
			int s = dh.IndexOf("\"token\":\"");
			if(s < 0)
			{
				sessionKEY = "";
				return;
			}
			s += 9;
			int i;
			for (i = s; i < dh.Length && (dh[i] != '\"' || dh[i] != '\''); i++){}
			sessionKEY = dh.Substring(s,i-s-2);
		}

		/// <summary>
		/// Cifra un mensaje :v
		/// </summary>
		/// <param name="texto"></param>
		/// <returns></returns>
		public static string Cifrar(string texto)
		{
            AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider
            {
                BlockSize = 128,
                KeySize = 256,
                Key = Encoding.ASCII.GetBytes(ciphKey),
                IV = Encoding.ASCII.GetBytes(ciphIv),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            byte[] txtByteData = Encoding.ASCII.GetBytes(texto);
			ICryptoTransform trnsfrm = AEScryptoProvider.CreateEncryptor(AEScryptoProvider.Key, AEScryptoProvider.IV);

			byte[] result = trnsfrm.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
			return Convert.ToBase64String(result);
		}

		/// <summary>
		/// Call this at the start of the game. This will load the required content to be able to Cipher strings, like Passwords
		/// </summary>
		public static void Initialize()
		{
			TextAsset data = Resources.Load("Secreto/key") as TextAsset;
			string t = data.text;
			int n = t.Length;
			int corte = t.IndexOf('\n');
			//for (int i = 0; i < n; i++) if (t[i] == '\n') corte = i;

			ciphIv = t.Substring(corte + 1);
			ciphKey = t.Substring(0, corte - 1);
		}
	}
}