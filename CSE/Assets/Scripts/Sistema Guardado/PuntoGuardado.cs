﻿using System.Collections.Generic;
using UnityEngine;
///Esta clase se encarga de guardar los datos relevantes de la partida.
[System.Serializable]
public class PuntoGuardado
{
	public int agentesDisponibles;
    public int consultasDisponibles;
	public int consultasMaximas;
	public int casosCompletados;
	public int dificultadActual;
	public int dia;
	public int[] casosCargados; //?
	public int casoEnCurso;
	public int[] eventosActivos;
	public string[] tableCodes;
	//Y demás cosillas por aquí...

	public PuntoGuardado()
	{
		agentesDisponibles = 3;
		consultasDisponibles = 4;
		consultasMaximas = 4;
		casosCompletados = 0;
		dificultadActual = 1;
		dia = 0;
		casosCargados = new int[0];
		casoEnCurso = -1;
		eventosActivos = new int[0];
		tableCodes = new string[0];
}

	public void Fijar()
	{
		agentesDisponibles = ResourceManager.AgentesDisponibles;
		consultasDisponibles = ResourceManager.ConsultasDisponibles;
		consultasMaximas = ResourceManager.ConsultasMaximas;
		casosCompletados = ResourceManager.CasosCompletados;
		dificultadActual = ResourceManager.DificultadActual;
		dia = ResourceManager.Dia;

		List<int> idCasos = new List<int>();
		foreach (var caso in PuzzleManager.Instance.casosCargados) idCasos.Add(caso.id);
		casosCargados = idCasos.ToArray();
		casoEnCurso = PuzzleManager.Instance.casoActivo;
		eventosActivos = new int[0]; //Falta por ponerlo...
		tableCodes = ResourceManager.TableCodes.ToArray();
	}

	public void Cargar()
	{
		ResourceManager.AgentesDisponibles = agentesDisponibles;
		ResourceManager.ConsultasDisponibles = consultasDisponibles;
		ResourceManager.ConsultasMaximas = consultasMaximas;
		ResourceManager.CasosCompletados = casosCompletados;
		ResourceManager.DificultadActual = dificultadActual;
		ResourceManager.Dia = dia;

		List<string> codes = new List<string>();
		codes.AddRange(tableCodes);
		ResourceManager.TableCodes = codes;
	}
}