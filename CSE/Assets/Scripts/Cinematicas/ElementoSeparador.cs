using System.Collections;
using UnityEngine;

namespace Hexstar
{
    /// <summary>
    /// Clase base para los separadores, 
    /// no hace nada por lo que bloqueará la cinemática
    /// </summary>
    [CreateAssetMenu(fileName = "Separador", menuName = "Hexstar/Cinematicas/Separador")] 
    public class ElementoSeparador : ScriptableObject
    {
        public enum Tipo { EsperarSegundos, EsperarEvento, EsperarContador, OperarAND, OperarOR};
        [HideInInspector] public bool haTerminado;
        public Tipo tipo;
        //
        [HideInInspector] public float segundos = 0;
        //
        [HideInInspector] public int contadorMax = 3;
        [HideInInspector] public int contador = 3;
        //
        [HideInInspector] public ElementoSeparador separadorA;
        [HideInInspector] public ElementoSeparador separadorB;


        /// <summary>
        /// Reinicia las flags del separador para que pueda volver a ser usado
        /// </summary>
        public virtual void Reiniciar()
        {
            haTerminado = false;
            contador = contadorMax;
        }

        /// <summary>
        /// Función que sirve para ponerla en eventos de otros objetos
        /// Si recive una señal indica que el separador ha terminado
        /// </summary>
        public void Terminar()
        {
            if (tipo == Tipo.EsperarEvento) haTerminado = true;
            else Debug.LogWarning("Has suscrito el separador a un evento pero este separador no está configurado para eso.");
        }

        /// <summary>
        /// Funcionamiento del separador para el resto de opciones que no sean "EsperarEvento"
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator Cuerpo()
        {
            if(separadorA) separadorA.Reiniciar();
            if(separadorB) separadorB.Reiniciar();
            haTerminado = false;
            switch (tipo)
            {
                case Tipo.EsperarContador:
                    yield return new WaitUntil(() => { return contador <= 0; });
                    haTerminado = true;
                    break;
                case Tipo.EsperarSegundos:
                    yield return new WaitForSeconds(segundos);
                    haTerminado = true;
                    break;
                case Tipo.OperarAND:
                    if (!separadorA || !separadorB) Debug.LogError("Ha usado un operador binario para este separador pero no se han asignado suficientes separadores al operador.");
                    yield return new WaitUntil(() => { return separadorA.haTerminado && separadorB.haTerminado; });
                    break;
                case Tipo.OperarOR:
                    if (!separadorA || !separadorB) Debug.LogError("Ha usado un operador binario para este separador pero no se han asignado suficientes separadores al operador.");
                    yield return new WaitUntil(() => { return separadorA.haTerminado || separadorB.haTerminado; });
                    break;
                case Tipo.EsperarEvento:
                    yield return new WaitUntil(()=> { return haTerminado; });
                    break;
                default:
                    break;
            }
        }
    }
}