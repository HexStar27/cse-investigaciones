using UnityEngine;

[System.Serializable]
public class Evento : ScriptableObject
{
	public enum Tipo { UnUso=0, Permanente=1 };
	public enum Trigger { AlInicioDia=0, AlConsultar=1, AlEmpezarCaso=2 };
	public enum Efecto { FreeShow=0, Gratis10Prob=1, Gratis20Prob=2, Nosekemas=3 };

	public Tipo tipo;
	public Trigger trigger;
	public Efecto efecto;
}
