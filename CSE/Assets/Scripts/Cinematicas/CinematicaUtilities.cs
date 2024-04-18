using SimpleJSON;
using UnityEngine.Events;
using UnityEngine;
using System;

namespace Hexstar {
    public static class CinematicaUtilities
    {
        internal static InstruccionesReferenciaCinematicas instrucciones = null;
        public static void InicializarCinematicaConJSON(this ControladorCinematica controlador, string texto)
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
            
            controlador.independiente = false;
            if (json.HasKey("meta"))
            {
                if(json["meta"].HasKey("independiente"))
                {
                    controlador.independiente = json["meta"]["independiente"].AsBool;
                }
            }
        }

        /// <summary>
        /// Will return the empty string if the JSON file is valid. Will return the error message otherwise.
        /// </summary>
        public static string CinematicaValida(string json)
        {
            string msgToSend = "";
            try
            {
                var jNodo = JSON.Parse(json);
                var nodosJson = jNodo["cinematica"];
                int n = nodosJson.Count;
                for (int i = 0; i < n; i++)
                {
                    var nodo = nodosJson[i];
                    if (!nodo.HasKey("instrucciones")) throw new Exception("Clave \"instrucciones\" no encontrada en el nodo " + i);
                    try { CheckInstrucciones(nodo["instrucciones"]); }
                    catch (Exception e) { throw new Exception(e.Message + " en la instrucción " + i); }
                    
                    if (!nodo.HasKey("separador")) throw new Exception("Clave \"separador\" no encontrada en el nodo " + i);
                    if (!nodo.HasKey("indices")) throw new Exception("Clave \"indices\" no encontrada en el nodo " + i);
                }
            }
            catch(Exception ex)
            {
                msgToSend = ex.Message;
            }
            return msgToSend;
        }
        private static void CheckInstrucciones(JSONNode instrLeidas)
        {
            int nIns = instrLeidas.Count;
            for (int i = 0; i < nIns; i++)
            {
                var ins = instrLeidas[i].ToString().Trim(' ', '\"').Split('-');
                string funcName = ins[0];
                string param = ins.Length > 1 ? ins[1] : "";
                CheckInstruccion(funcName, param);
            }
        }
        public static void CheckInstruccion(string funcion, string valor)
        {
            string f = funcion.Trim();
            valor = valor.Trim();
            switch (f)
            {
                case "EmpezarDialogo": break;
                case "HoldPause": break;
                case "SetCameraStalker": int.Parse(valor.Trim()); break;
                case "SetGameplayCycleStalker": int.Parse(valor.Trim()); break;
                case "PauseBeforeNextCycle": break;
                case "SetDialogueEvent": break;
                case "ExternalCall": break;
                case "IncluirCaso": int.Parse(valor.Trim()); break;
                default: throw new System.Exception(funcion + " no es una instrucción válida.");
            };
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