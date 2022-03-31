///Esta clase se encarga de todo lo relacionado con los casos del juego

using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
	List<Caso> casosCargados = new List<Caso>();

	public void LoadCasos(int n)
	{
		//1º Acceder a servidor pidiendo n casos ( usando el ConexionHanlder )
		//2º Parsear datos
		//3º Crear casos
	}

	public void LoadCasoExamen()
	{
		//1º Acceder a servidor pidiendo un caso examen ( usando el ConexionHanlder )
		//2º Parsear caso
		//3º Crear caso
	}

	public void QuitarTodos()
	{
		casosCargados.Clear();
	}
	public void QuitarCaso(Caso c)
	{
		casosCargados.Remove(c);
	}

	public void MostrarCasosEnPantalla()
	{
		//1º Genera Posiciones aleatorias dentro del rango de la "pantalla"
		//2º Inicializar Representador de casos (1 por caso almacenado)
		//3º Posicionar
	}
}
