using UnityEngine;
using UnityEngine.Events;

namespace Hexstar
{
    [System.Serializable]
    internal class NodoCinematica
    {
        public UnityEvent evento = null;
        public ElementoSeparador separador = null;
        [SerializeField] protected int[] indicesPosibles = new int[0];
        private int indiceSeleccionado = 0;
        /// <summary>
        /// Indica el nodo que debe ser ejecutado después de este
        /// </summary>
        /// <returns>El índice del nodo en el controlador</returns>
        public virtual int Siguiente()
        {
            if (indicesPosibles.Length == 0)
                Debug.LogError("El Nodo de la cinemática debe tener al menos 1 indice posible siguiente " +
                    "para poder llamar a esta función. Si no desea continuar con más nodos, debe poner -1 como índice.");

            return indicesPosibles[indiceSeleccionado];
        }

        public virtual void SeleccionarSiguiente(int seleccion)
        {
            if (seleccion < 0 || seleccion >= indicesPosibles.Length) seleccion = 0;
            indiceSeleccionado = seleccion;
        }

        public virtual bool NoNextIndex() { return indicesPosibles.Length == 0; }
        public virtual void SetNewIndexList(ref int[] indexList)
        {
            indicesPosibles = new int[indexList.Length];
            indexList.CopyTo(indicesPosibles,0);
        }
    }

    //Escribir aquí los nodos derivados del NodoCinematica
}