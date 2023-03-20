using UnityEngine;
using Hexstar;
using System.Collections.Generic;

public class TesteoCasoAJson : MonoBehaviour
{
	[SerializeField] Caso caso = new Caso();
	[SerializeField] Caso[] resultado = new Caso[0];
	private void Start()
	{
		List<Caso> casos = new List<Caso>();
		casos.Add(caso);
		casos.Add(caso);
		casos.Add(caso);
		string json = JsonConverter.ConvertirAJson(casos);
		Debug.Log("Json creado: " + json);
		JsonConverter.PasarJsonAObjeto(json,casos);
		resultado = casos.ToArray();
	}
}
