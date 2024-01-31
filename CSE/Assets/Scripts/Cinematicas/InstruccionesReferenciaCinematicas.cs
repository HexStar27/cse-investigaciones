using UnityEngine;
using UnityEngine.Events;

namespace Hexstar
{
    internal class InstruccionesReferenciaCinematicas : MonoBehaviour
    {
        [SerializeField] Dialogue.ControladorDialogos controladorDialogos;
        [SerializeField] CameraStalker camStalker;
        [SerializeField] GameplayCycleStalker gCycleStalker;
        [SerializeField] PausaExternaCinematica pausaExternaCinematica;

        public static UnityEvent<string> onExternalCall = new();

        public void Awake()
        {
            CinematicaUtilities.instrucciones = this;
        }

        public virtual void IncluirInstruccion(ref UnityEvent evento, string funcion, string valor)
        {
            int target = 0;
            string f = funcion.Trim();
            valor = valor.Trim();
            switch (f)
            {
                case "EmpezarDialogo":
                    evento.AddListener(() => {
                        //Debug.Log("Empezando dialogo con valor "+valor);
                        controladorDialogos.EmpezarDialogo(valor);
                    });
                    break;

                case "HoldPause":
                    bool pause = valor.Equals("1");
                    evento.AddListener(() => {
                        //Debug.Log("Cambiando estado de pausa a " + valor);
                        controladorDialogos.HoldPause(pause);
                    });
                    break;

                case "SetCameraStalker":
                    target = int.Parse(valor.Trim());
                    evento.AddListener(() => {
                        //Debug.Log("Activando el cameraStalker con " + valor);
                        camStalker.gameObject.SetActive(true);
                        camStalker.SetTarget(target);
                    });
                    break;

                case "SetGameplayCycleStalker":
                    target = int.Parse(valor.Trim());
                    evento.AddListener(() => {
                        //Debug.Log("Activando el gCycleStalker con " + valor);
                        gCycleStalker.gameObject.SetActive(true);
                        gCycleStalker.AddTarget(target);
                    });
                    break;

                case "PauseBeforeNextCycle":
                    evento.AddListener(() => {
                        //Debug.Log("Pausando antes del siguiente ciclo");
                        pausaExternaCinematica.EscucharSiguiente();
                    });
                    break;

                case "SetDialogueEvent":
                    evento.AddListener(() => {
                        //Debug.Log("Metiendo el valor del evento a "+valor);
                        Dialogue.ControladorDialogos.SetDialogueEvent_CinematicFriendly(valor);
                    });
                    break;

                case "ExternalCall":
                    evento.AddListener(() => {
                        onExternalCall?.Invoke(valor);
                    });
                    break;

                default:
                    MensajeInstruccionNoEncontrada(f);
                    break;
            };
        }

        protected void MensajeInstruccionNoEncontrada(string nombre)
        {
            print("Instruccion " + nombre + " no pertenece a esta lista de instrucciones.");
        }
    }
}