using System.Collections.Generic;
using UnityEngine;

public class Personaje : MonoBehaviour
{
    public Caracteristicas caracteristicas;
    public string nombre, sexo, tez;
    public int apariencia;
    public string ojos, pelo;
    private int tamanno;
    public float altura;
    public int peso;

    public string[] inventario;

    public int puntosVidaActuales, puntosVida, regeneracion, movimiento, cansancio, turno, acciones;

    int nivel, presencia;
    public Raza raza;
    public Resistencias resistencia;

    public Habilidades habilidades;

    public Armadura[] armaduras;
    public Arma[] armas;
    public int armaEquipada;

    public List<string> notasYVentajas;

    private void OnEnable()
    {
        ActualizarTurno();
        ActualizarPresencia();
        ActualizarResistencias();
        
        //También hay que actualizar todo sobre la raza, el resto de calculos, y los bonos de la categoria

    }

    /// <summary>
    /// Recalcula el turno teniendo en cuenta el arma equipada
    /// </summary>
    public void ActualizarTurno()
    {
        turno = 20 + caracteristicas.Bono(caracteristicas.TotalPuntos("Destreza")) + caracteristicas.Bono(caracteristicas.TotalPuntos("Agilidad"));
        int penEquipo = 0;
        foreach (Armadura a in armaduras)
        {
            penEquipo += a.penalizadorNatural;
        }

        if (armaEquipada == -1) //Desarmado
            penEquipo += 20;
        else
            penEquipo += armas[armaEquipada].velocidad;
        turno += penEquipo; //Penalizadores de armadura y arma
        turno += 0; //Bono innato de la categoría...
    }

    /// <summary>
    /// Equipa el arma indicada
    /// </summary>
    /// <param name="indice">La posición del arma a equipar del vector de armas, -1 si se quiere ir desarmado</param>
    public void EquiparArma(int indice)
    {
        if (indice >= armas.Length) indice = armas.Length - 1;
        if (indice < -1) indice = -1;
        armaEquipada = indice;
        ActualizarTurno();
    }

    /// <summary>
    /// Recalcula la presencia...
    /// </summary>
    public void ActualizarPresencia()
    {
        presencia = 30 + (nivel - 1) * 5;
    }

    /// <summary>
    /// Actualiza los valores de las resistencias
    /// </summary>
    public void ActualizarResistencias()
    {
        resistencia.CalcularResistencias(presencia,
            caracteristicas.Bono(caracteristicas.TotalPuntos("Constitución")),
            caracteristicas.Bono(caracteristicas.TotalPuntos("Poder")),
            caracteristicas.Bono(caracteristicas.TotalPuntos("Voluntad")));
    }
}

public struct Caracteristicas
{
    private static readonly int[] bonos = {-30,-20, -10, -5, 0, 5, 5, 10, 10, 15, 20, 20, 25, 25, 30, 35, 35, 40, 40, 45};
    public Habilidades caracteristicas;// fuerza, destreza, agilidad, constitucion, inteligencia, voluntad, poder, percepcion;

    public Caracteristicas(int relleno = 0)
    {
        caracteristicas = new Habilidades();
        caracteristicas.NuevaHabilidad("Fuerza", relleno);
        caracteristicas.NuevaHabilidad("Destreza", relleno);
        caracteristicas.NuevaHabilidad("Agilidad", relleno);
        caracteristicas.NuevaHabilidad("Constitución", relleno);
        caracteristicas.NuevaHabilidad("Inteligencia", relleno);
        caracteristicas.NuevaHabilidad("Voluntad", relleno);
        caracteristicas.NuevaHabilidad("Poder", relleno);
        caracteristicas.NuevaHabilidad("Percepción", relleno);
    }

    /// <summary>
    /// Devuelve el valor de la característica indicada
    /// </summary>
    /// <param name="nombre">El nombre de la característica</param>
    /// <returns></returns>
    public int TotalPuntos(string nombre)
    {
        return caracteristicas.habilidades[nombre].Final();
    }

    /// <summary>
    /// Calcula en bono que deberiá dar una característica específica
    /// </summary>
    /// <param name="caracteristica">El valor de la característica</param>
    /// <returns>El valor del bono correspondiente</returns>
    public int Bono(int caracteristica)
    {
        if (caracteristica < 1) caracteristica = 1;
        else if (caracteristica > 20) caracteristica = 20;

        return bonos[caracteristica - 1];
    }
}