///Esta clase se encarga de guardar los datos relevantes de la partida.
[System.Serializable]
public class PuntoGuardado
{
	private int _agentesDisponibles;
    private int _consultasDisponibles;
	private int _consultasMaximas;
	private int _casosCompletados;
	private int _dificultadActual;
	private int _puntuacion;
	private int _dia;
	private int _casoEnCurso;
	private int[] _eventosActivos;
	//Y demás cosillas por aquí...

	public PuntoGuardado()
	{
		_agentesDisponibles = 3;
		_consultasDisponibles = 4;
		_consultasMaximas = 4;
		_casosCompletados = 0;
		_dificultadActual = 1;
		_puntuacion = 0;
		_dia = 0;
		_casoEnCurso = -1;
		_eventosActivos = new int[0];
	}

	public void Fijar()
	{
		_agentesDisponibles = ResourceManager.AgentesDisponibles;
		_consultasDisponibles = ResourceManager.ConsultasDisponibles;
		_consultasMaximas = ResourceManager.ConsultasMaximas;
		_casosCompletados = ResourceManager.CasosCompletados;
		_dificultadActual = ResourceManager.DificultadActual;
		_puntuacion = ResourceManager.Puntuacion;
		_dia = ResourceManager.Dia;
		_casoEnCurso = PuzzleManager.Instance.casoActivo.id;
		_eventosActivos = new int[0]; //Falta por ponerlo...
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
		//Cargar los casos indicados
		//PuzzleManager.Instance.casoActivo = _casoEnCurso;
		if (_casoEnCurso >= 0) GameplayCycle.Instance.SetState(1);
	}
}
