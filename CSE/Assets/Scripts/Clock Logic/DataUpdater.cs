/// Esta clase se encarga de mostrar la puntuación y los recursos en el "reloj".

using UnityEngine;
using TMPro;
using System.Text;

public class DataUpdater : MonoBehaviour
{
	public TextMeshPro contenedor;
	private string contenido;

	public void FixedUpdate()
	{
		ObtenerDatos();
		contenedor.SetText(contenido);
	}

	public void ObtenerDatos()
	{
		StringBuilder builder = new StringBuilder();
		int consultas = ResourceManager.ConsultasDisponibles;
		int punt = ResourceManager.Puntuacion;
		int agentes = ResourceManager.AgentesDisponibles;

		builder.Append(consultas);
		builder.Append("    ");
		builder.Append(AntiOverflowInt(punt));
		builder.Append("    ");
		builder.Append(agentes);

		contenido = builder.ToString();
	}

	public string AntiOverflowInt(int num)
	{
		if(num >= 100000)
		{
			return "O_O'";
		}
		else if(num >= 10000)
		{
			return num / 1000 + "K";
		}
		else if(num >= 1000)
		{
			return num / 1000 + "." + (num % 1000) / 100 + "K";
		}
		return num.ToString();
	}
}
