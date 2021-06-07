public class Item
{
	public string nombre;
	public string descripccion;
	public int coste;
	public int peso;
	public int calidad;
	public int presencia;
}

public enum TipoAtaque { FIL, CON, PEN, CAL, ELE, FRI, ENE};
public enum TamArma { Normal, Enorme, Gigante};
public enum ReglaEspecial { };
public enum TipoArma { Asta, Corta, Cuerda, Escudo, Espada, Hacha, Mandoble, Maza, Lanzamiento, Proyectiles, Municion };

public class Armadura : Item
{
	public TipoAtaque[] tipo;
	public int[] cantidad;
	public int restriccionMovimiento;
	public int requerimientoDeArmadura;
	public int penalizadorNatural;
}

public class Arma : Item
{

	public TipoArma tipoArma;
	public TipoAtaque criticoPrimario;
	public TipoAtaque criticoSecundario;
	public int damage;
	public int velocidad; //modificador de turno
	public int fuerzaRequerida;
	public TamArma tamanio;
	public ReglaEspecial regla;
}

public class ArmaDistancia : Arma
{
	public int cadenciaFuego, recarga, alcance;
}