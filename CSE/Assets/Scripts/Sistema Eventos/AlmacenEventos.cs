using CSE.Local;
using Hexstar.Dialogue;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

namespace Hexstar.CSE.SistemaEventos
{
    public static class AlmacenEventos
    {
        private static List<Evento> listaDeEventos = new();
        /// <summary>
        /// Lista que indica si un evento de "listaDeEventos" ha sido ejecutado o no.
        /// </summary>
        private static List<bool> eventosEjecutados = new();
        public static bool EsperandoEvento { get; set; }
        internal static bool EventoOcupado { get; set; }

        /// <summary>
        /// Obtiene los eventos almacenados en el servidor y lo prepara todo listo para usar
        /// </summary>
        public async static Task DescargarEventosServidor() 
        {
            await ConexionHandler.AGet(ConexionHandler.baseUrl + "event/all");
            CargarEventosArchivo(ConexionHandler.download);
            ActualizarEventosEjecutados();
        }

        /// <summary>
        /// Ejecuta y despacha los eventos automáticamente uno a uno.
        /// </summary>
        public static async Task EncargarseDeEventosAptos()
        {
            int n = listaDeEventos.Count;
            for (int i = 0; i < n; i++)
            {
                if (eventosEjecutados[i]) continue;

                Evento eventoDisponible = listaDeEventos[i];
                if (eventoDisponible.CumpleCondiciones()) await eventoDisponible.Ejecutar();
            }
        }

        /// <summary>
        /// Parsea los eventos de un texto al almacén.
        /// </summary>
        private static void CargarEventosArchivo(string text)
        {
            var json = JSON.Parse(text);
            if (json == null) throw new NullReferenceException();
            var array = json["res"];
            for(int i = 0; i < array.Count; i++)
            {
                Evento ev = JsonUtility.FromJson<Evento>(array[i]["data"].ToString());
                if (ev == null) { Debug.LogWarning("Unable to parse json into Evento :("); continue; }
                ev.ParsearCondicion();
                IncluirNuevoEvento(ev);
            }
        }
        private static void IncluirNuevoEvento(Evento e)
        {
            listaDeEventos.Add(e);
            eventosEjecutados.Add(false);
        }

        /// <summary>
        /// Señala los eventos que ya se han ejecutado para que no se vuelvan a ejecutar.
        /// Diseñado para utilizarse al descargar los eventos del servidor.
        /// </summary>
        private static void ActualizarEventosEjecutados()
        {
            for(int i = 0; i < ResourceManager.EventosEjecutados.Count; i++)
            {
                int id = ResourceManager.EventosEjecutados[i];
                if (id >= 0 && id < listaDeEventos.Count) throw new IndexOutOfRangeException();
                eventosEjecutados[id] = true;
            }
        }

        /// <summary>
        /// Señala el evento indicado para que no se vuelvan a ejecutar.
        /// </summary>
        internal static bool MarcarEventoComoEjecutado(int idEvento)
        {
            if (idEvento < 0 || idEvento >= listaDeEventos.Count) return false;

            if (!ResourceManager.EventosEjecutados.Contains(idEvento)) 
                ResourceManager.EventosEjecutados.Add(idEvento);

            if (eventosEjecutados[idEvento]) return false;
            return eventosEjecutados[idEvento] = true;
        }

        /// <summary>
        /// (En desuso) Comprueba si las condiciones de los eventos disponibles se cumplen 
        /// devolviendo una lista con estos eventos. ¡Usa EncargarseDeLosEventosAptos() 
        /// si quieres que los eventos se manejen automáticamente uno después de otro!
        /// </summary>
        public static List<Evento> ObtenerEventosQueCumplenCondición()
        {
            List<Evento> output = new();
            int n = listaDeEventos.Count;
            for (int i = 0; i < n; i++)
            {
                if (eventosEjecutados[i]) continue;

                Evento current = listaDeEventos[i];
                if (current.condicion.EsValida()) output.Add(current);
            }
            return output;
        }
    }


    [Serializable] public class Evento
    {
        public enum Comprobable { AL_INICIO_DIA = 0, AL_FIN_CASO = 2 };

        //La ID debería ser siempre igual al índice dentro de la lista.
        [HideInInspector] public int id;
        public string titulo;
        public bool diario;
        public Comprobable momentoComprobable;
        public string condicionUnparsed;
        [NonSerialized] public CondicionEvento condicion;
        [Range(0, 100)] public int probabilidad = 100;
        public string cinematicFile;
        public string dialogueDataBaseFile;
        public string tableCodesNuevos;
        public string modVarGameplay;

        public Evento(int id) { this.id = id; }

        private static readonly char[] separadores = { ',', ' ' };

        /// <summary>
        /// Aplica los diferentes elementos del evento en orden.
        /// AVISO: EJECUTAR UN EVENTO PUEDE PAUSAR EL HILO EN EL QUE SE EJECUTA.
        /// Por lo tanto NUNCA ejecutar el evento en el MAIN THREAD.
        /// </summary>
        public async Task Ejecutar()
        {
            // Calcula la probabilidad de que se ejecute (Se calcula al estilo Call Of Cthulhu el ttrpg)
            if (UnityEngine.Random.Range(0, 101) > probabilidad) return;

            // Se ejecutan los elementos del evento en orden
            if (cinematicFile.Length > 0)
            {
                if (dialogueDataBaseFile.Length <= 0)
                    Debug.LogWarning("Se está inicializando una cinemática sin una DialogueDataBase... ¿Estás seguro de que esa era la intención?");
                else ControladorDialogos.ddb.LoadFromString(dialogueDataBaseFile);
                ControladorCinematica.Instance.InicializarCinematicaConJSON(cinematicFile);
                
                if(!ControladorCinematica.Instance.independiente)
                {
                    ControladorCinematica.Instance.alTerminarCinematica.AddListener(FinishedCinematic);
                    AlmacenEventos.EventoOcupado = true;
                    ControladorCinematica.Instance.IniciarCinematica();

                    while (AlmacenEventos.EventoOcupado) await Task.Delay(100);
                    ControladorCinematica.Instance.alTerminarCinematica.RemoveListener(FinishedCinematic);
                }
                else ControladorCinematica.Instance.IniciarCinematica();
            }
            if (modVarGameplay.Length > 0)
            {
                await ReadModVarGameplay();
            }
            if (tableCodesNuevos.Length > 0)
            {   
                string[] tc = tableCodesNuevos.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                ResourceManager.TableCodes.AddRange(tc);

                TempMessageController.Instancia.GenerarMensaje(Localizator.GetString(".nuevas_tablas"));
            }

            ResourceManager.EventosEjecutados.Add(id);
            if(!diario) AlmacenEventos.MarcarEventoComoEjecutado(id);
        }

        private async Task ReadModVarGameplay()
        {
            string[] vars = modVarGameplay.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < vars.Length; i++)
            {
                string code = vars[i].Trim().ToUpper();
                int num;

                int opPlus = code.IndexOf('+');
                int opMinus = code.IndexOf('-');
                if (opPlus >= 0 && opMinus == -1)
                {
                    num = int.Parse(code[opPlus..], NumberStyles.Integer);
                    code = code[..opPlus];
                }
                else if (opMinus >= 0 && opPlus == -1)
                {
                    num = int.Parse(code[opMinus..], NumberStyles.Integer);
                    code = code[..opMinus];
                }
                else
                {
                    Debug.LogWarning("There's an error trying to parse the Event " + id +
                            " in the modVarGameplay part (Could not parse operation: " + code + ").");
                    continue;
                }

                switch (code)
                {
                    case "AD": // Agentes Disponibles
                        ResourceManager.AgentesDisponibles += num;
                        await DataUpdater.Instance.ShowAgentesDisponibles();
                        break;
                    case "CD": // Consultas Disponibles
                        ResourceManager.ConsultasDisponibles += num;
                        await DataUpdater.Instance.ShowConsultasDisponibles();
                        break;
                    case "CM": // Consultas Máximas
                        ResourceManager.ConsultasMaximas += num;
                        await DataUpdater.Instance.ShowConsultasMaximas();
                        break;
                    case "RP": // Reputación Pueblo
                        ResourceManager.ReputacionPueblo += num;
                        break;
                    case "RE": // Reputación Empresas
                        ResourceManager.ReputacionEmpresas += num;
                        break;
                    default:
                        Debug.LogWarning("There's an error trying to parse the Event "+id+
                            " in the modVarGameplay part (Wrong Code: "+code+").");
                        break;
                }
            }
            await Task.Yield();
        }
        private void FinishedCinematic() => AlmacenEventos.EventoOcupado = false;

        /// <summary>
        /// Comprueba que el evento cumpla todas las condiciones necesarias para ser ejecutado en este momento.
        /// </summary>
        public bool CumpleCondiciones() => MomentoCorrecto() && condicion.EsValida();
        private bool MomentoCorrecto() => GameplayCycle.GetState() == (int)momentoComprobable;

        /// <summary>
        /// Construye una condición en base a la cadena condicionUnparsed
        /// </summary>
        public void ParsearCondicion()
        {
            condicion = ParserCondiciones.Procesar_a_AST(condicionUnparsed);
        }
        /// <summary>
        /// Construye una condición en base a la cadena pasada
        /// </summary>
        public bool IncluirCondicion(string nuevaCondicion)
        {
            condicionUnparsed = nuevaCondicion;
            condicion = ParserCondiciones.Procesar_a_AST(condicionUnparsed);
            return condicion != null;
        }
    }
}

// Gramática usada en eventos:
//////////////////////////////////////////////////
// TableCodesNuevos:
// Ejemplo -> "codigo,codigo,codigo,codigo"
//////////////////////////////////////////////////
// ModVarGameplay:
// Ejemplo -> "AD+1, CD-1, CM+2, RP+7, RE-10"
//////////////////////////////////////////////////
// CinematicFile:
// - Se usa el mismo formato que acepta el sistema de cinemáticas
//////////////////////////////////////////////////
// DialogueDataBaseFile:
// - Se usa el mismo formato que acepta el sistema de diálogos
//////////////////////////////////////////////////
/* Condición:
 
 * CONDICION -> AND | OR | CONDICION_SIMPLE
 * AND -> "AND(" LISTA ")"
 * OR -> "OR(" LISTA ")"
 * LISTA -> CONDICION | CONDICION ',' LISTA
 * CONDICION_SIMPLE -> VAR OP INT
 * VAR -> "CASO_COMPLETADO" | "CASO_PERDIDO" | "CASO_ABANDONADO" | "DIA_ACTUAL" | "REP_PUEBLO" | "REP_EMPRESA"
 * OP -> "=" | "<" | ">"

Ejemplo 1: (Básico)
    "CASO_COMPLETADO=3"

Ejemplo 2: (Árbol)
    "AND( CASO_COMPLETADO=9,
	    CASO_COMPLETADO=2,
	    CASO_PERDIDO=5,
	    DIA_ACTUAL>10,
	    OR( CASO_COMPLETADO=4,
		    CASO_ABANDONADO=8
	    ),
	    REP_PUEBLO>20
    )"
 */
//////////////////////////////////////////////////