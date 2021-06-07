using UnityEngine;

public class Controles : MonoBehaviour
{
    /// <summary>
    /// Realiza control de habilidad normal en el que la tirada "a" debe superar la tirada "b" para que resulte en éxito
    /// </summary>
    /// <param name="a">Información anterior a la tirada</param>
    /// <param name="b">Información del valor a superar</param>
    /// <returns>El resultado del control</returns>
    public static ResultadoControl Control_Habilidad(DatosTirada a, DatosTirada b)
    {
        ResultadoControl res = new ResultadoControl();

        int vA = Tiradas.D100(), racha = 0, vFinal = vA;

        //Comprueba si es pifia
        if (Tiradas.Pifia(vA, a.buenaSuerte, a.malaSuerte, a.valorBase >= 200))
        {
            res.pifiaA = true;
            res.valorFinalA = vA;
            res.nivelPifiaA = Tiradas.CalcularNivelPifia();

            return res;
        }

        //Comprueba si es abierta y calcula el valor final
        while(Tiradas.Abierta(vA,racha))
        {
            vA = Tiradas.D100();
            racha++;
            vFinal += vA;
        }
        res.nAbiertasA = racha;
        res.abiertaA = racha > 0;
        vFinal += a.valorBase + a.bono;
        res.valorFinalA = vFinal;

        //Comprueba si tiene éxito o no
        if(vFinal > b.valorBase + b.bono)
            res.superado = true;

        return res;
    }

    /// <summary>
    /// Realiza control de habilidad enfrentada en el que la tirada "a" debe superar la tirada "b" para que resulte en éxito
    /// </summary>
    /// <param name="a">Información anterior a la tirada de la primera entidad</param>
    /// <param name="b">Información snterior a la tirada de la segunda entidad</param>
    /// <returns>El resultado del control</returns>
    public static ResultadoControl Control_Habilidad_Enfrentada(DatosTirada a, DatosTirada b)
    {
        ResultadoControl res = new ResultadoControl();

        int vA = Tiradas.D100(), rachaA = 0, vFinalA = vA;
        int vB = Tiradas.D100(), rachaB = 0, vFinalB = vB;

        //Calculando todo sobre las pifias
        bool pifiaA = Tiradas.Pifia(vA, a.buenaSuerte, a.malaSuerte, a.valorBase >= 200);
        bool pifiaB = Tiradas.Pifia(vB, b.buenaSuerte, b.malaSuerte, b.valorBase >= 200);

        if(pifiaA || pifiaB)
        {
            if (pifiaA)
            {
                res.pifiaA = true;
                res.nivelPifiaA = Tiradas.CalcularNivelPifia();
            }
            if (pifiaB)
            {
                res.pifiaB = true;
                res.nivelPifiaB = Tiradas.CalcularNivelPifia();
            }
            if (!pifiaA && pifiaB)
                res.superado = true;
            else if (pifiaA && pifiaB)
                res.empate = true;

            return res;
        }

        //Calculando todo sobre las abiertas de A
        while (Tiradas.Abierta(vA, rachaA))
        {
            vA = Tiradas.D100();
            rachaA++;
            vFinalA += vA;
        }
        res.nAbiertasA = rachaA;
        res.abiertaA = rachaA > 0;
        vFinalA += a.valorBase + a.bono;
        res.valorFinalA = vFinalA;

        //Calculando todo sobre las abiertas de B
        while (Tiradas.Abierta(vB, rachaB))
        {
            vB = Tiradas.D100();
            rachaB++;
            vFinalB += vB;
        }
        res.nAbiertasB = rachaB;
        res.abiertaB = rachaB > 0;
        vFinalB += b.valorBase + b.bono;
        res.valorFinalB = vFinalB;

        //Se comprueba quién gana
        if (vFinalA > vFinalB) res.superado = true;
        else if (vFinalA == vFinalB) res.empate = true;

        return res;
    }

    /// <summary>
    /// Realiza control de resistencia en el que la tirada "a" debe superar la tirada "b" para que resulte en éxito
    /// </summary>
    /// <param name="a">Información anterior a la tirada de la primera entidad</param>
    /// <param name="b">Información del valor a superar</param>
    /// <returns>El resultado del control</returns>
    public static ResultadoControl Control_Resistencia(DatosTirada a, DatosTirada b)
    {
        ResultadoControl res = new ResultadoControl();
        int vA = Tiradas.D100();

        if (vA == 100) res.superado = true;

        vA += a.valorBase + a.bono;
        res.valorFinalA = vA;

        if(vA > b.valorBase || a.valorBase + a.bono - 20 > b.valorBase)
        {
            res.superado = true;
        }
        res.valorFinalB = b.valorBase + b.bono;

        return res;
    }

    /// <summary>
    /// Realiza control de características en el que la tirada "a" debe superar la tirada "b" para que resulte en éxito
    /// </summary>
    /// <param name="a">Información anterior a la tirada de la primera entidad</param>
    /// <param name="b">Información del valor a superar</param>
    /// <returns>El resultado del control</returns>
    public static ResultadoControl Control_Caracteristias(DatosTirada a, DatosTirada b)
    {
        ResultadoControl res = new ResultadoControl();

        int vA = Tiradas.D10() + a.valorBase + a.bono;

        if (vA > b.valorBase + b.bono) res.superado = true;

        res.valorFinalA = vA;
        res.valorFinalB = b.valorBase + b.bono;

        return res;
    }

    /// <summary>
    /// Realiza control de características enfrentada en el que la tirada "a" debe superar la tirada "b" para que resulte en éxito
    /// </summary>
    /// <param name="a">Información anterior a la tirada de la primera entidad</param>
    /// <param name="b">Información anterior a la tirada de la primera entidad</param>
    /// <returns>El resultado del control</returns>
    public static ResultadoControl Control_Caracteristias_Enfrentadas(DatosTirada a, DatosTirada b)
    {
        ResultadoControl res = new ResultadoControl();

        int vA = Tiradas.D10() + a.valorBase + a.bono;
        int vB = Tiradas.D10() + b.valorBase + b.bono;

        if (vA > vB) res.superado = true;
        else if (vA == vB) res.empate = true;

        res.valorFinalA = vA;
        res.valorFinalB = vB;

        return res;
    }

}

/// <summary>
/// Datos necesarios para realizar una tirada
/// Deben de mandarse 2 para los controles, 1 para el jugador y 1 para el contrincante
/// </summary>
public struct DatosTirada
{
    public int dado, valorBase, bono;
    public bool buenaSuerte, malaSuerte;
}

public struct ResultadoControl
{
    public bool superado, empate, pifiaA, pifiaB, abiertaA, abiertaB;
    public int valorFinalA, valorFinalB, nAbiertasA, nAbiertasB, nivelPifiaA, nivelPifiaB;
}