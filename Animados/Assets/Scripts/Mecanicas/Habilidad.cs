using System.Collections.Generic;

public struct Habilidad
{
    public int valor, bono;
    public Habilidad(int valor, int bono)
    {
        this.valor = valor;
        this.bono = bono;
    }
    public int Final()
    {
        return valor + bono;
    }
}

public class Habilidades
{
    public Dictionary<string, Habilidad> habilidades;

    /// <summary>
    /// Añade una nueva habilidad
    /// </summary>
    /// <param name="titulo">Nombre de la habilidad</param>
    /// <param name="valor">Valor de la habilidad</param>
    public void NuevaHabilidad(string titulo, int valor = 0, int bono = 0)
    {
        habilidades.Add(titulo, new Habilidad(valor, bono));
    }

    /// <summary>
    /// Elimina la habilidad :v
    /// </summary>
    /// <param name="titulo">Nombre de la habilidad</param>
    public void EliminarHabilidad(string titulo)
    {
        habilidades.Remove(titulo);
    }

    /// <summary>
    /// El valor de la habilidad especificada
    /// </summary>
    /// <param name="titulo">El nombre de la habilidad</param>
    /// <returns>Valor</returns>
    public int Valor(string titulo)
    {
        habilidades.TryGetValue(titulo, out Habilidad i);
        return i.valor + i.bono;
    }
}


public class Dificultad
{
    public ScriptableDificultad dificultades;
    public Dictionary<int, string> difMap;

    public Dificultad(ScriptableDificultad sd)
    {
        for(int i = 0; i < dificultades.intervalos.Count; i++)
        {
            NuevoNivelDificultad(dificultades.intervalos[i], 
                            dificultades.descripciones[i]);
        }
    }

    /// <summary>
    /// Incluye un nuevo nivel de dificultad
    /// </summary>
    /// <param name="valor">Valor a superar</param>
    /// <param name="descripcion">Descripción del resultado</param>
    public void NuevoNivelDificultad(int valor, string descripcion)
    {
        difMap.Add(valor, descripcion);
    }

    /// <summary>
    /// Elimina un nivel de dificultad :v
    /// </summary>
    /// <param name="valor">Valor representante de la dificultad</param>
    public void EliminarNivelDificultad(int valor)
    {
        difMap.Remove(valor);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valor"></param>
    /// <returns></returns>
    public string Descripcion(int valor)
    {
        difMap.TryGetValue(valor, out string i);
        return i;
    }
}

/*
public class HabilidadesPrimarias
{
    //De combate
    public Habilidad hAtaque, hParada, hEsquiva, llevarArmadura;
    public Habilidad kiAGI, kiCON, kiDES, kiFUE, kiPOD, kiVOL;
    public Habilidad acumKiAGI, acumKiCON, acumKiDES, acumKiFUE, acumKiPOD, acumKiVOL;
    public Habilidad conocimientoMarcial;
    
    public TablaArma[] tablaArmas;
    public TablaEstilo[] tablaEstilos;
    public ArteMarcial[] artesMarciales;
    
    //Sobrenaturales
    public Habilidad zeon;
    public Habilidad acumulacionMagica;
    public Habilidad proyeccionMagica;
    public Habilidad convocar;
    public Habilidad dominar;
    public Habilidad atar;
    public Habilidad desconvocar;
    public TablaProyeccion[] tablasDeProyeccionMagica;
    
    //Psíquicas
    public Habilidad cv;
    public Habilidad proyeccionPsiquica;
    public TablaProyeccion[] tablasDeProyeccionPsiquica;
    public PatronMental[] patronesMentales;
}//*/

/*
public class HabilidadesSecundarias
{
    //Atléticas
    public Habilidad acrobacias;
    public Habilidad atletismo;
    public Habilidad montar;
    public Habilidad nadar;
    public Habilidad saltar;
    public Habilidad pilotar;

    //Sociales
    public Habilidad estilo;
    public Habilidad intimidar;
    public Habilidad liderazgo;
    public Habilidad persuasion;
    public Habilidad comercio;
    public Habilidad callejeo;
    public Habilidad etiqueta;

    //Percepción
    public Habilidad advertir;
    public Habilidad buscar;
    public Habilidad rastrear;

    //Intelectuales
    public Habilidad animales;
    public Habilidad ciencia;
    public Habilidad ley;
    public Habilidad herbolaria;
    public Habilidad historia;
    public Habilidad tactica;
    public Habilidad medicina;
    public Habilidad memorizar;
    public Habilidad navegacion;
    public Habilidad ocultismo;
    public Habilidad tasacion;
    public Habilidad valoracionMagica;

    //Vigor
    public Habilidad frialdad;
    public Habilidad proezasDeFuerza;
    public Habilidad resistenciaAlDolor;

    //Subterfugio
    public Habilidad cerrajeria;
    public Habilidad disfraz;
    public Habilidad ocultarse;
    public Habilidad robo;
    public Habilidad sigilo;
    public Habilidad tramperia;
    public Habilidad venenos;

    //Creativas
    public Habilidad arte;
    public Habilidad baile;
    public Habilidad forja;
    public Habilidad musica;
    public Habilidad trucoDeManos;
    public Habilidad orfebreria;
    public Habilidad confeccion;
}//*/