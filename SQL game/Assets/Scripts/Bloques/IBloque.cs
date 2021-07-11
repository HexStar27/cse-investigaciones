/// <summary>
/// Interfaz para el bloque de una consulta
/// </summary>
public interface IBloque
{
    /// <summary>
    /// Devuelve el siguiente bloque conectado a este. O null si no hay...
    /// </summary>
    /// <returns> El bloque que prosigue a este </returns>
    IBloque Siguiente();

    /// <summary>
    /// Devuelve el bloque anterior conectado a este. O null si no hay...
    /// </summary>
    /// <returns>El bloque superior a este</returns>
    IBloque Anterior();
    
    /// <summary>
    /// Devuelve el bloque que está anidado en este bloque. O null si no hay...
    /// </summary>
    /// <returns> El bloque dentro de este </returns>
    IBloque Anidado(uint i = 0);

    /// <summary>
    /// Devuelve el número máximo de bloques anidados a este mismo bloque.
    /// </summary>
    /// <returns> El número de bloques anidados posibles a la vez</returns>
    uint NAnidados();

    /// <summary>
    /// Devuelve la cadena resultante de unir todos los bloques anidados y siguientes.
    /// </summary>
    /// <returns> Cadena recursiva </returns>
    string TrozoCadena();
    
    /// <summary>
    /// Conecta el bloque 'b' como el siguiente del actual.
    /// </summary>
    /// <param name="b"> El bloque a conectar </param>
    void ConectarSiguiente(IBloque b);

    /// <summary>
    /// Conecta el bloque 'b' como el anterior del actual. (El padre)
    /// </summary>
    /// <param name="b"> El bloque a conectar </param>
    void ConectarAnterior(IBloque b);
    
    /// <summary>
    /// Anida el bloque 'b' a este bloque.
    /// </summary>
    /// <param name="b"> El bloque a anidar en el bloque actual </param>
    void Anidar(IBloque b, uint i = 0);
    
    /// <summary>
    /// Devuelve el total de bloques conectados y anidados desde el nodo llamado
    /// </summary>
    /// <returns> Número de bloques conectados y anidados contanto a este. </returns>
    uint Longitud();

    /// <summary>
    /// Calcula cuál de los huecos para los bloques se está señalando (con la posción (x,y)).
    /// 0 a N-1 para los anidados, y N para el siguiente (siendo N el número total de huecos para anidar.
    /// N+1 si no ha señalado ningún hueco
    /// </summary>
    /// <param name="x">Posición en el eje horizontal</param>
    /// <param name="y">Posición en el eje vertical</param>
    /// <returns>El índice calculado</returns>
    uint ZonaEntradaBloque(float x, float y);
}
