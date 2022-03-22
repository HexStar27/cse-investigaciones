using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Hexstar
{
    public class ControladorCinematica : MonoBehaviour
    {
        [SerializeField] int nodoInicial = 0;
        [SerializeField] NodoCinematica[] nodos = new NodoCinematica[0];
        [Header("Extras")]
        [SerializeField] UnityEvent alTerminarCinematica = null;
        private bool pausado;

        public void IniciarCinematica()
        {
            StartCoroutine(Loop());
        }

        public void PausarCinematica(bool value)
        {
            pausado = value;
        }

        public void PararCinematica()
        {
            StopCoroutine(Loop());
            pausado = false;
        }

        private IEnumerator Loop()
        {
            WaitWhile pausa = new WaitWhile(()=> { return pausado; });
            int nodoActual = nodoInicial;

            if (nodoActual < 0 || nodoActual >= nodos.Length)
                Debug.LogError("El nodo inicial está fuera de rango");

            while (nodoActual >= 0)
            {
                if(pausado)
                {
                    yield return pausa;
                }

                if (nodoActual >= 0 && nodoActual < nodos.Length)
                {
                    nodos[nodoActual].evento.Invoke();
                    if(nodos[nodoActual].separador)
                    {
                        StartCoroutine(nodos[nodoActual].separador.Cuerpo());
                        yield return new WaitUntil(() =>
                        { return nodos[nodoActual].separador.haTerminado; });
                        nodos[nodoActual].separador.Reiniciar();
                    }

                    nodoActual = nodos[nodoActual].Siguiente();
                }
            }
            alTerminarCinematica.Invoke();
            yield return null;
        }
    }
}