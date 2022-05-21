using Hexstar.CSE;

[System.Serializable]
public class Caso
{
	public int id,dif,coste;
	public bool examen;
	public string titulo;
	public string resumen;

	public DatosPista[] pistas;

	public enum Efectos {Evento=0, Reto=1, Penalizacion=2, Papeleo=3, Incertidumbre=4, MasDificultad=5, FinDelJuego=6, //Efectos generales
						MasRecursos=7, MejoraInfraestructura=8, InvitoYo=9, Rapido=10, Ayuda=11, Fama=12, //Recompensas
						Intrusion=13, Sabotaje=14, Emboscada=15, Infame=16}; //Penalizaciones

	public enum Condiciones {Ganar=0, Perder=1, MaxConsultas1=2, MaxConsultas2=3, TiempoLimite30s=4, TiempoLimite1min=5, TiempoLimite2min=6, TiempoLimite3min=7, Siempre=8};

	[System.Serializable]
	public struct EventoCaso
	{
		public Condiciones condicion;
		public Efectos efecto;
		public int cantidad;
	};

	public int eventoId;
	public EventoCaso[] eventosCaso;
}
