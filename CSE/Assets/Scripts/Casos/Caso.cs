using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName ="Caso",menuName ="Hexstar/Caso")]
public class Caso:ScriptableObject
{
	public int id,dif,coste;
	public bool examen;
	public string titulo;
	public string resumen;

	public string[] pistas;

	public enum Efectos {Pago=0, Reto=1, Penalizacion=2, Papeleo=3, Incertidumbre=4, MasDificultad=5, FinDelJuego=6,
						MasRecursos=7, MejoraInfraestructura=8, InvitoYo=9, Rapido=10, Ayuda=11, Fama=12, 
						Intrusion=13, Sabotaje=14, Emboscada=15, Infame=16};

	public Efectos[] efectos;
}
