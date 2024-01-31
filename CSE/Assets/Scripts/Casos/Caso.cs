using Hexstar.CSE;
using System.Threading.Tasks;

[System.Serializable]
public class Caso
{
	public int id,dif,coste;
	public bool secundario;
	public bool examen;
	public string titulo;
	public string resumen;

	public DatosPista[] pistas;

	[System.Serializable]
	public struct Bounty
	{
        public enum TipoEfecto
        {
            AGENTES_DISPONIBLES = 0, CONSULTAS_DISPONIBLES, CONSULTAS_MAXIMAS,
            REPUTACION_PUEBLO, RESPUTACION_EMPRESA
        };
        public enum Condicion
        {
            Ganar = 0, Perder = 1, MaxConsultasT1 = 2, MaxConsultasT2 = 3, TiempoLimiteT1 = 4,
            TiempoLimiteT2 = 5, TiempoLimiteT3 = 6, TiempoLimiteT4 = 7, Siempre = 8
        };
        
		public Condicion condicion;
		public TipoEfecto efecto;
		public int cantidad;

		/// <summary>
		/// Comprueba si se cumple la condición y aplica el efecto si se da el caso.
		/// </summary>
		/// <param name="ganado">Se ha resuelto el caso</param>
		/// <param name="consultasUsadas">Número de consultas usadas para terminar el caso</param>
		/// <param name="tiempoEmpleado">Tiempo que ha tardado el jugador en terminar el caso (en segundos)</param>
		internal async Task ComprobarYAplicar(bool ganado, int consultasUsadas, float tiempoEmpleado)
		{
			bool superado = condicion switch
			{
				Condicion.Ganar => ganado,
				Condicion.Perder => !ganado,
				Condicion.MaxConsultasT1 => consultasUsadas <= 4,
				Condicion.MaxConsultasT2 => consultasUsadas <= 2,
				Condicion.TiempoLimiteT1 => tiempoEmpleado <= 210f, // 3m 30s
				Condicion.TiempoLimiteT2 => tiempoEmpleado <= 150f,	// 2m 30s
				Condicion.TiempoLimiteT3 => tiempoEmpleado <= 90f,	// 1m 30s
				Condicion.TiempoLimiteT4 => tiempoEmpleado <= 45f,	// 45s
				Condicion.Siempre => true,
				_ => true,
			};

			await BountyMessageController.SendMessage(this,superado);

            if (superado)
			{
				switch (efecto)
				{
					case TipoEfecto.AGENTES_DISPONIBLES:
						ResourceManager.AgentesDisponibles += cantidad;
                        await DataUpdater.Instance.ShowAgentesDisponibles();
						break;
					case TipoEfecto.CONSULTAS_DISPONIBLES:
                        ResourceManager.ConsultasDisponibles += cantidad;
						await DataUpdater.Instance.ShowConsultasDisponibles();
                        break;
					case TipoEfecto.CONSULTAS_MAXIMAS:
                        ResourceManager.ConsultasMaximas += cantidad;
						await DataUpdater.Instance.ShowConsultasMaximas();
                        break;
					case TipoEfecto.REPUTACION_PUEBLO:
                        ResourceManager.ReputacionPueblo += cantidad;
                        break;
					case TipoEfecto.RESPUTACION_EMPRESA:
                        ResourceManager.ReputacionEmpresas += cantidad;
                        break;
				}
				PuzzleManager.NRetosCumplidos++;
			}
		}
	};

	public Bounty[] bounties;

	/// <summary>
	/// Comprueba que se cumplan las condiciones de todos los bounties y los ejecuta.
	/// </summary>
	public async Task ComprobarYAplicarBounties(bool ganado, int consultasUsadas, float tiempoEmpleado)
	{
		if (bounties == null) return;
		foreach(var b in bounties) await b.ComprobarYAplicar(ganado,consultasUsadas,tiempoEmpleado);
	}
}
