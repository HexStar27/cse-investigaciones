using SimpleJSON;
using UnityEngine.Events;
using UnityEngine;

namespace Hexstar {
    public static class CinematicaUtilities
    {
        internal static InstruccionesReferenciaCinematicas instrucciones = null;
        public static void InicializarCinematicaConJSON(ControladorCinematica controlador, string texto)
        {
            var json = JSON.Parse(texto);
            var nodosJson = json["cinematica"];
            int n = nodosJson.Count;
            NodoCinematica[] nodos = new NodoCinematica[n];
            for (int i = 0; i < n; i++)
            {
                var nodo = nodosJson[i];
                nodos[i] = new NodoCinematica();
                ProcesarInstrucciones(nodo["instrucciones"], ref nodos[i]);
                ProcesarSeparador(nodo["separador"], controlador, ref nodos[i]);
                ProcesarIndices(nodo["indices"], ref nodos[i]);
            }
            controlador.nodos = nodos;
        }

        private static void ProcesarInstrucciones(JSONNode instrLeidas, ref NodoCinematica nodo)
        {
            nodo.evento = new UnityEvent();
            if (instrucciones == null) return;

            int nIns = instrLeidas.Count;
            for(int i = 0; i < nIns; i++)
            {
                var ins = instrLeidas[i].ToString().Trim(' ', '\"').Split('-');
                string funcName = ins[0];
                string param = ins.Length > 1 ? ins[1] : "";
                instrucciones.IncluirInstruccion(ref nodo.evento, funcName, param);
            }
        }
        private static void ProcesarSeparador(JSONNode separador, ControladorCinematica controlador, ref NodoCinematica nodo)
        {
            string nombreSeparador = separador.ToString().Trim('\"');
            if (nombreSeparador.Equals("")) return;
            ElementoSeparador elemSeparador = controlador.elementosSeparadoresDeReferencia.Find(
                (elem) => { return elem.name.Equals(nombreSeparador); 
            });
            if (elemSeparador == null) Debug.LogError("Separador "+nombreSeparador+" no encontrado.");
            nodo.separador = elemSeparador;
        }
        private static void ProcesarIndices(JSONNode indices, ref NodoCinematica nodo)
        {
            int n = indices.Count;
            int[] nuevaLista = new int[n];
            for (int i = 0; i < n; i++)
            {
                nuevaLista[i] = indices[i].AsInt;
            }
            nodo.SetNewIndexList(ref nuevaLista);
        }
    }
}