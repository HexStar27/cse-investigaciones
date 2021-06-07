public struct Resistencias
{
    public int fisica, enfermedades, venenos, magica, mental;

    /// <summary>
    /// Inicializa calculando el valor para todas las resistencias
    /// </summary>
    /// <param name="presenciaBase">La presencia del personaje</param>
    /// <param name="bonoCons">El bono de la constitución</param>
    /// <param name="bonoPod">El bono del Poder</param>
    /// <param name="bonoVol">El bono de la Voluntad</param>
    /// <param name="otros">Bonos de raza, ventajas y especiales ordenados en el orden: RF, RE, RV, RM, RP</param>
    public Resistencias(int presenciaBase, int bonoCons, int bonoPod, int bonoVol)
    {
        fisica = presenciaBase + bonoCons;
        enfermedades = presenciaBase + bonoCons;
        venenos = presenciaBase + bonoCons;
        magica = presenciaBase + bonoPod;
        mental = presenciaBase + bonoVol;
    }

    /// <summary>
    /// Calcula el valor para todas las resistencias a la vez
    /// </summary>
    /// <param name="presenciaBase">La presencia del personaje</param>
    /// <param name="bonoCons">El bono de la constitución</param>
    /// <param name="bonoPod">El bono del Poder</param>
    /// <param name="bonoVol">El bono de la Voluntad</param>
    /// <param name="otros">Bonos de raza, ventajas y especiales ordenados en el orden: RF, RE, RV, RM, RP</param>
    public void CalcularResistencias(int presenciaBase, int bonoCons, int bonoPod, int bonoVol)
    {
        fisica = presenciaBase + bonoCons;
        enfermedades = presenciaBase + bonoCons;
        venenos = presenciaBase + bonoCons;
        magica = presenciaBase + bonoPod;
        mental = presenciaBase + bonoVol;
    }
}
