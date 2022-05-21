///Esta clase se encarga de guardar los datos relevantes de la partida.
[System.Serializable]
public class PuntoGuardado
{
	public int agentesDisponibles;
    public int consultasDisponibles;
	public int consultasMaximas;
	public int casosCompletados;
	public int dificultadActual;
	public int puntuacion;
	public int dia;
	public int casoEnCurso;
	public int[] eventosActivos;
	//Y demás cosillas por aquí...

	public PuntoGuardado()
	{
		agentesDisponibles = 3;
		consultasDisponibles = 4;
		consultasMaximas = 4;
		casosCompletados = 0;
		dificultadActual = 1;
		puntuacion = 0;
		dia = 0;
		casoEnCurso = -1;
		eventosActivos = new int[0];
	}

	public void Fijar()
	{
		agentesDisponibles = ResourceManager.AgentesDisponibles;
		consultasDisponibles = ResourceManager.ConsultasDisponibles;
		consultasMaximas = ResourceManager.ConsultasMaximas;
		casosCompletados = ResourceManager.CasosCompletados;
		dificultadActual = ResourceManager.DificultadActual;
		puntuacion = ResourceManager.Puntuacion;
		dia = ResourceManager.Dia;
		if (PuzzleManager.Instance.casoActivo != null) casoEnCurso = PuzzleManager.Instance.casoActivo.id;
		else casoEnCurso = -1;
		eventosActivos = new int[0]; //Falta por ponerlo...
	}

	public void Cargar()
	{
		ResourceManager.AgentesDisponibles = agentesDisponibles;
		ResourceManager.ConsultasDisponibles = consultasDisponibles;
		ResourceManager.ConsultasMaximas = consultasMaximas;
		ResourceManager.CasosCompletados = casosCompletados;
		ResourceManager.DificultadActual = dificultadActual;
		ResourceManager.Puntuacion = puntuacion;
		ResourceManager.Dia = dia;
		//Cargar los casos indicados
		//PuzzleManager.Instance.casoActivo = casoEnCurso;
		//if (casoEnCurso >= 0) GameplayCycle.Instance.SetState(1);
	}
}
