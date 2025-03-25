using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hexstar.Dialogue {
    public class ControladorDialogos : MonoBehaviour
    {
        public static DialogueDataBase ddb;
        public ControladorCajaDialogos dialogueBox;

        public UnityEvent onDialogueFinish = new();
        public UnityEvent onNextEntry = new();

        private static Dictionary<string, string> dialogueEventList = new();

        [Header("Necesario para los Efectos")]
        [SerializeField] Animator oscurecedor;
        [SerializeField] List<ActorDialogo> actores;
        [SerializeField] RectTransform retrato;
        [SerializeField] RectTransform textoCaja;
        [SerializeField] Image retratoRenderer;
        [SerializeField] BancoImagenesDialogo bancoImagenes;
        [SerializeField] Image imagenExtra;
        [SerializeField] RectTransform zonaImagen;
        [SerializeField] ControladorOpcionesDialogo controladorOpciones;
        [SerializeField] ControladorDeOtrosEfectos otrosEfectos;

        [Header("Conexión con el sistema de cinemáticas")]
        [SerializeField] ControladorCinematica controladorCinematica;
        [SerializeField] ElementoSeparador separadorTipoEvento;

        [Header("Debug")]
        public bool showDialogDebug = false;


        private int iEntrada = -1; //Indice global en la DDB
        private string grupoDialogoInicial = ""; //Grupo en el que se empezó el diálogo
        private int primerIndiceDelGrupo = 0; //Indice absoluto en la base de datos de la primera entrada del grupo actual

        private void Awake()
        {
            if(ddb == null) ddb = ScriptableObject.CreateInstance<DialogueDataBase>();
        }
        private void Start()
        {
            SetNickname();
        }

        public static void SetDialogueEvent(string dEvent, string value)
        {
            if(!dialogueEventList.TryAdd(dEvent,value))
            {
                dialogueEventList[dEvent] = value;
            }
        }
        public static string GetDialogueEventValue(string dEvent)
        {
            if (dialogueEventList.TryGetValue(dEvent, out string value))
                return value;
            else 
                return "";
        }
        public static string GetAllEventsFromDict()
        {
            System.Text.StringBuilder sb = new();
            foreach(var entry in dialogueEventList)
            {
                sb.Append(entry.Key);
                sb.Append('=');
                sb.Append(entry.Value);
                sb.Append(',');
            }
            return sb.ToString();
        }
        public static void SetAllEventsToList(string text, bool clear = false)
        {
            if (clear) dialogueEventList.Clear();
            if (text.Equals("")) return;

            var entries = text.Split(',');
            int n = entries.Length;
            for(int i = 0; i < n; i++)
            {
                var tokens = entries[i].Split('=');
                if(tokens.Length >= 2)
                    SetDialogueEvent(tokens[0].Trim(), tokens[1].Trim());
                else
                    SetDialogueEvent(tokens[0].Trim(), "");
            }
        }

        public static void SetDialogueEvent_CinematicFriendly(string instruction)
        {
            if (!instruction.Contains('='))
            {
                Debug.LogError("To set a dialogue event via instruction, an '=' is required.");
                return;
            }

            var tokens = instruction.Split('=');
            if (tokens[0].Trim().Equals(""))
            {
                Debug.LogError("The event must have a name, it cannot be an empty string.");
                return;
            }

            SetDialogueEvent(tokens[0].Trim(), tokens[1].Trim());
        }

        public void EmpezarDialogo(string nombreGrupo)
        {
            if (ddb == null) return;

            primerIndiceDelGrupo = ddb.GetFirstIndexOfGroup(nombreGrupo);
            if (primerIndiceDelGrupo < 0)
            {
                Debug.LogError("No se ha encontrado ningun grupo de dialogo con el titulo "+nombreGrupo);
                return;
            }

            grupoDialogoInicial = nombreGrupo;
            iEntrada = primerIndiceDelGrupo;
            dialogueBox.AbrirTextoNuevo();
            SetGlobalStop(true);
            if (showDialogDebug) print("Empezando diálogo en el grupo " + grupoDialogoInicial + " con índice " + iEntrada);
            MostrarRetrato(null);
            SiguienteDU();
        }
            
        public void SiguienteDU()
        {
            int n = ddb.entries.Count;
            if (iEntrada >= n || iEntrada < 0) //Puntero fuera de la base de datos
            {
                if (showDialogDebug) print("Alcanzado la entrada "+iEntrada+", cerrando diálogo...");
                CerrarDialogo();
                return;
            }

            var entry = ddb.entries[iEntrada];
            if (!entry.group.Equals(grupoDialogoInicial)) //Puntero fuera del grupo de diálogo inicial
            {
                if (showDialogDebug) print("El índice de entrada ha salido del grupo de diálogo incial");
                CerrarDialogo();
                return;
            }

            if (dialogueBox.HaTerminado())
            {
                iEntrada++; //Incremento antes de procesar efectos para que estos tengan prioridad al modificar el valor
                if (showDialogDebug) print("Procesando efectos");
                for (int i = 0; i < entry.fx.Count; i++) ProcesarEfecto(entry.fx[i]);

                string texto = entry.txt;
                texto = TranslateReferences(texto);
                
                dialogueBox.Vaciar();
                if (showDialogDebug) print("Introduciendo texto en la caja de diálogos");
                // Introduce texto sólo si no es una cadena vacía. Esto nos permite tener entradas
                // donde sólo hay efectos y que todas estas se ejecuten consecutivamente
                // sin esperar el input del jugador.
                if (texto.Equals("")) SiguienteDU();
                else dialogueBox.IntroducirTexto(texto);

                
                if (iEntrada - 1 != primerIndiceDelGrupo) onNextEntry?.Invoke();
            }
            else
            {
                if (showDialogDebug) print("Saltando colocación del texto...");
                dialogueBox.SaltarColocacionTexto();
            }
        }

        private void CerrarDialogo()
        {
            if (dialogueBox.EstaAbierto())
            {
                dialogueBox.CerrarTexto();
                iEntrada = -1;
                grupoDialogoInicial = "";
                onDialogueFinish?.Invoke();
                SetGlobalStop(false);
            }
            if (separadorTipoEvento != null) separadorTipoEvento.Terminar();
        }

        private string TranslateReferences(string texto)
        {
            int idx = texto.IndexOf("<val[");
            while(idx >= 0)
            {
                int off = idx + 5;
                int fin = off;
                while(fin < texto.Length && texto[fin] != ']') fin++;
                string key = texto[off..fin];
                string value = GetDialogueEventValue(key);
                if (showDialogDebug) print("Traduciendo key de diálogo " + key + " a " + value);
                texto = texto.Replace("<val[" + key + "]>",value);
                idx = texto.IndexOf("<val[");
            }
            return texto;
        }

        /// <summary>
        /// Devuelve el Nickname del jugador si recibe "nick" como key
        /// </summary>
        private void SetNickname()
        {
            string nick = SesionHandler.nickname ?? "Alfonso";
            SetDialogueEvent("nick", nick);
        }

        private void ProcesarEfecto(DialogueFX fx)
        {
            if (showDialogDebug) print("Procesando efecto \""+fx.tipo.ToString()+"\"");
            switch (fx.tipo)
            {
                case DialogueFX.Tipo.CambioPerfil:
                    var vals = fx.value.Split();
                    if (vals.Length <= 1) MostrarRetrato(null);
                    else MostrarRetrato(GetPortrait(vals[0], vals[1]));
                    break;
                case DialogueFX.Tipo.MostrarImagen:
                    zonaImagen.gameObject.SetActive(fx.value != "");
                    imagenExtra.sprite = bancoImagenes.GetSprite(fx.value);
                    break;
                case DialogueFX.Tipo.Ramificar:
                    string data = GetDialogueEventValue(fx.value);
                    if (data.Equals("BREAK")) iEntrada = -1;
                    else if (data != "") iEntrada = ddb.GetIndexOfLabelByGroup(iEntrada - 1, data);
                    break;
                case DialogueFX.Tipo.DarOpciones:
                    if(controladorOpciones != null)
                    {
                        var ops = fx.value.Split('|');
                        List<string> contenido = new();
                        List<int> etiquetas = new();
                        for(int i = 0; i < ops.Length; i++)
                        {
                            var par = ops[i].Split("@");
                            if (par.Length != 2) Debug.LogError("Dialogue Parse Error in: " + ops[i]);
                            contenido.Add(par[0].Trim());
                            
                            string labelOrIndex = par[1].Trim();
                            if (int.TryParse(labelOrIndex, out int result)) etiquetas.Add(result);
                            else
                            {
                                int idx = ddb.GetIndexOfLabelByGroup(iEntrada - 1, labelOrIndex);
                                if (idx < 0) Debug.LogWarning("CUIDADO, se intentó buscar SIN ÉXITO durante la creacion de opciones un índice para la etiqueta " + labelOrIndex + ".");
                                etiquetas.Add(idx);
                            }
                        }
                        controladorOpciones.CrearOpciones(contenido, etiquetas);
                        ControladorOpcionesDialogo.onOptionSelected.AddListener(ProcesarInfoDeOpcionEscogida);
                    }
                    break;
                case DialogueFX.Tipo.Oscurecer:
                    if(oscurecedor != null)
                    {
                        if (fx.value == "true") oscurecedor.Play("Oscurecer");
                        else oscurecedor.Play("Desoscurecer");
                    }
                    break;
                case DialogueFX.Tipo.IrAEntrada:
                    if (int.TryParse(fx.value, out int indice))
                    {
                        iEntrada = indice;
                    }
                    else
                    {
                        iEntrada = ddb.GetIndexOfLabelByGroup(iEntrada - 1, fx.value.Trim());
                    }
                    break;
                case DialogueFX.Tipo.EstablecerEvento:
                    var s = fx.value.Split(':');
                    SetDialogueEvent(s[0].Trim(), s[1].Trim());
                    break;
                default:
                    if(otrosEfectos != null) 
                        otrosEfectos.AplicarEfecto(fx.value);
                    break;
            }
        }

        private Sprite GetPortrait(string actor, string expression)
        {
            var a = actores.Find((i) => { return i.name == actor; });
            if (a != null) return a.GetExpression(expression);
            return null;
        }

        private void MostrarRetrato(Sprite img)
        {
            if(img != null)
            {
                retrato.gameObject.SetActive(true);
                retratoRenderer.sprite = img;
                textoCaja.anchorMin = new(0.13f, 0.1f);
                textoCaja.offsetMin = Vector2.zero;
            }
            else //EsconderRetrato
            {
                retrato.gameObject.SetActive(false);
                textoCaja.anchorMin = new(0f, 0.1f); 
                textoCaja.offsetMin = Vector2.zero;
            }
        }

        private void ProcesarInfoDeOpcionEscogida(int entrada, int opcionId)
        {
            if (showDialogDebug) print("Opción escogida: entrada="+entrada+", indiceOpcion="+opcionId);
            iEntrada = entrada;
            controladorCinematica.SeleccionarSiguienteNodo(opcionId);

            if (showDialogDebug) print(dialogueBox.HaTerminado() ? "Pasando a la siguiente DU" : "Forzando terminación del texto para pasar a la siguiente DU");
            if (dialogueBox.HaTerminado()) SiguienteDU();
            else
            {
                dialogueBox.onFinishPlacement.AddListener(DU_Retardado);
                dialogueBox.SaltarColocacionTexto();
            }
            ControladorOpcionesDialogo.onOptionSelected.RemoveListener(ProcesarInfoDeOpcionEscogida);
        }

        // Esto existe porque tal y como están conectados el ControladorCajaDialogos y el ControladorDialogos, no tengo
        // una forma de saltarme la colocación del texto y ejecutar el siguiente nodo de diálogos en el mismo frame...
        // (algo que sólo va a ocurrir cuando se eligen opciones mientras el texto se coloca)
        // Tampoco es ineficiente, pero es código feísimo.
        private void DU_Retardado()
        {
            dialogueBox.onFinishPlacement.RemoveListener(DU_Retardado);
            SiguienteDU();
        }

        private void SetGlobalStop(bool stop)
        {
            Boton3D.globalStop = stop;
            GameplayCycle.PauseGameplayCycle(stop, "dialogo");
        }

        public void HoldPause(bool pause)
        {
            GameplayCycle.PauseGameplayCycle(pause, "dialogo-related");
        }
    }
}