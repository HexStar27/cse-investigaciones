using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hexstar
{
    public class ControladorCinematica : MonoBehaviour
    {
        public static ControladorCinematica Instance { get; private set; }

        public List<ElementoSeparador> elementosSeparadoresDeReferencia = new();
        [SerializeField] int nodoInicial = 0;
        int nodoActual = 0;
        public TextAsset cinematicJSON;
        [SerializeField] internal NodoCinematica[] nodos = new NodoCinematica[0];
        [Header("Extras")]
        public UnityEvent alTerminarCinematica = new();
        private bool pausado;

        public bool showDebug;

        [ContextMenu("IniciarCinematica")]
        public void IniciarCinematica()
        {
            StartCoroutine(Loop());
        }

        [ContextMenu("Imprimir estado actual")]
        public void DebugState()
        {
            print(nodoActual);
        }

        [ContextMenu("Leer archivo JSON")]
        public void CargarJSON()
        {
            if (cinematicJSON != null)
            {
                CinematicaUtilities.InicializarCinematicaConJSON(this, cinematicJSON.text);
            }
            else Debug.LogWarning("cinematicJSON TextAsset is null, proceding to ignore action to load it.");
        }
        public void InterpretarCadenaComoJSON(string supuestoJSON)
        {
            CinematicaUtilities.InicializarCinematicaConJSON(this, supuestoJSON);
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
            nodoActual = nodoInicial;

            if (nodoActual < 0 || nodoActual >= nodos.Length)
                Debug.LogError("El nodo inicial está fuera de rango");

            if (showDebug) print("Comenzando");
            while (nodoActual >= 0 && nodoActual < nodos.Length)
            {
                if(pausado)
                {
                    if (showDebug) print("Pausado");
                    yield return pausa;
                    if (showDebug) print("Despausado");
                }

                if (showDebug) print("Llamando eventos del nodo " + nodoActual);
                nodos[nodoActual].evento.Invoke();
                if (nodos[nodoActual].separador != null)
                {
                    if (showDebug) print("Esperando aviso del separador");
                    yield return StartCoroutine(nodos[nodoActual].separador.Cuerpo());
                    nodos[nodoActual].separador.Reiniciar();
                    if (showDebug) print("Fin del separador");
                }
                else if (showDebug) print("Sin separador...");

                if (nodos[nodoActual].NoNextIndex()) nodoActual++;
                else nodoActual = nodos[nodoActual].Siguiente();
            }
            if (showDebug) print("Terminando cinemática...");
            alTerminarCinematica?.Invoke();
            if (showDebug) print("Fin cinemática");
        }

        public void SeleccionarSiguienteNodo(int idx)
        {
            nodos[nodoActual].SeleccionarSiguiente(idx);
            if (showDebug) print("Siguiente nodo seleccionado para el nodo "+nodoActual+": " + nodos[nodoActual].Siguiente());
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}