using UnityEngine;

/// <summary>
/// Controla el tema de las tiradas de dados
/// </summary>
public class Tiradas : MonoBehaviour
{
    public static int D10()
    {
        return 1 + (Mathf.RoundToInt(Random.value * 10) % 10);
    }

    public static int D100()
    {
        return 1 + (Mathf.RoundToInt(Random.value * 100) % 100);
    }

    /// <summary>
    /// Comprueba si la tirada es una abierta
    /// </summary>
    /// <param name="dado"> El valor del dado tirado</param>
    /// <param name="racha"> El número tiradas abiertas sucesivas</param>
    /// <returns></returns>
    public static bool Abierta(int dado, int racha)
    {
        return dado >= 90 + racha || dado == 100;
    }

    /// <summary>
    /// Comprueba si la tirada es una pifia
    /// </summary>
    /// <param name="dado"> El valor del dado tirado</param>
    /// <param name="buenaSuerte"> Verdadero si tiene la ventaja "Buena suerte"</param>
    /// <param name="malaSuerte"> Verdadero si tiene la desventaja "Mala suerte"</param>
    /// <param name="maestria"> Verdadero si ha alcanzado la maestría en la habilidad final que se usa</param>
    /// <returns> Verdadero si es una pifia, falso si no es el caso</returns>
    public static bool Pifia(int dado, bool buenaSuerte, bool malaSuerte, bool maestria)
    {
        if (malaSuerte) dado--;
        else if (buenaSuerte) dado++;
        if (maestria) dado++;
        return dado <= 3;
    }

    /// <summary>
    /// Tira un dado de 100 y devuelve un nivel de pifia genérico
    /// </summary>
    /// <returns>Valores posibles: 1, 2, 3</returns>
    public static int CalcularNivelPifia()
    {
        int p = D100();
        if (p <= 50) return 1;
        else if (p <= 95) return 2;
        else return 3;
    }

    /// <summary>
    /// Indica qué consecuencias tendrá el nivel de pifia calculado.
    /// </summary>
    /// <param name="d100">Valor de la pifia (Dado de 100)</param>
    /// <returns>Descripción del resultado</returns>
    public static string ResultadoDePifiaDeHabilidad(int d100)
    {
        if (d100 <= 50) return "La acción simplemente no tiene éxito.";
        if (d100 <= 95) return "La acción fracasa terriblemente de un modo perjudicial.";
        return "La acción es un fracaso absoluto y altamente trágico.";
    }
}
