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
		int crono = 0;
		int agentes = ResourceManager.AgentesDisponibles;

		if(GameplayCycle.Instance.GetState() == (int)EstadosDelGameplay.InicioCaso)
			crono = Mathf.RoundToInt(PuzzleManager.GetTiempoEmpleado());		

		builder.Append(consultas);
		builder.Append("    ");
		builder.Append(AntiOverflowTime(crono));
		builder.Append("    ");
		builder.Append(agentes);

		contenido = builder.ToString();
	}

	//Limitación: Cada pantallita del reloj puede tener hasta 4 carácteres
	public string AntiOverflowTime(int num)
    {
		int segs = num % 60;
		int min = num / 60;
		if (min >= 10) return "+10m";
		else
		{
			string extra = segs < 10 ? "0" : "";
			return min + ":" + extra + segs;
		}
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
