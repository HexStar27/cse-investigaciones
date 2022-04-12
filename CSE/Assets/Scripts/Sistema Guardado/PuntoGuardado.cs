///Esta clase se encarga de guardar los datos relevantes de la partida.

using UnityEngine;

[System.Serializable]
public class PuntoGuardado : MonoBehaviour
{
	private int _agentesDisponibles;
    private int _consultasDisponibles;
	private int _consultasMaximas;
	private int _casosCompletados;
	private int _dificultadActual;
	private int _puntuacion;
	private int _dia;
	private Caso _casoEnCurso;
	private Evento[] _eventosActivos;
	//Y demás cosillas por aquí...

	public void Fijar(int ad, int cd, int cm, int cc, int da, int p, int d, Caso caso, Evento[] eventos)
	{
		_agentesDisponibles = ad;
		_consultasDisponibles = cd;
		_consultasMaximas = cm;
		_casosCompletados = cc;
		_dificultadActual = da;
		_puntuacion = p;
		_dia = d;
		_casoEnCurso = caso;
		_eventosActivos = eventos;
	}

	public void Cargar()
	{
		ResourceManager.AgentesDisponibles = _agentesDisponibles;
		ResourceManager.ConsultasDisponibles = _consultasDisponibles;
		ResourceManager.ConsultasMaximas = _consultasMaximas;
		ResourceManager.CasosCompletados = _casosCompletados;
		ResourceManager.DificultadActual = _dificultadActual;
		ResourceManager.Puntuacion = _puntuacion;
		ResourceManager.Dia = _dia;
		PuzzleManager.Instance.casoActivo = _casoEnCurso;
		if (_casoEnCurso != null) GameplayCycle.Instance.SetState(1);
	}
}
