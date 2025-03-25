using System.Collections.Generic;
using UnityEngine;
using Hexstar.CSE.SistemaEventos;
using SimpleJSON;
using TMPro;
using UnityEngine.UI;
using System.IO;

namespace Hexstar.UI
{
    public class UnitTest_Eventos : MonoBehaviour
    {
        public List<Evento> eventos = new();
        public List<string> eventosConvertidos = new();

        [Header("UI")]
        [SerializeField] TMP_InputField rutaGuardado_ui;
        [SerializeField] Button boton_guardar_ui;
        [SerializeField] Button boton_cargar_ui;
        [SerializeField] VerticalLayoutGroup lista_eventos_ui;
        [SerializeField] Button boton_crear_evento_ui;
        [SerializeField] Button boton_destruir_evento_ui;
        [Space()]
        [SerializeField] TMP_InputField nombre_ui;
        [SerializeField] Toggle diario_ui;
        [SerializeField] TMP_Dropdown comprobable_ui;
        [SerializeField] Slider probabilidad_ui;
        [SerializeField] TMP_InputField condicion_ui;
        [SerializeField] RectTransform condicion_validator_ui;
        [SerializeField] Button boton_ddb_ui;
        [SerializeField] Button boton_cerrar_ddb_ui;
        [SerializeField] TextMeshProUGUI ddb_entriesCount_ui;
        [Space()]
        [SerializeField] TMP_InputField cinematica_ui;
        [SerializeField] TMP_InputField table_codes_ui;
        [SerializeField] TMP_InputField gameplay_vars_ui;
        [Space()]
        [SerializeField] Animator anim;
        [Header("Prefabs")]
        [SerializeField] GameObject lista_eventos_elem;

        /// <summary>
        /// Convierte todos los eventos de la lista de eventos a JSON individualmente.
        /// </summary>
        public void Convertir_A_JSON()
        {
            eventosConvertidos ??= new(); // <=> If null then new()
            eventosConvertidos.Clear();
            for (int i = 0; i < eventos.Count; i++)
            {
                var json = eventos[i].Serializar();

                eventosConvertidos.Add(json.ToString());
            }
        }

        public void CargarCSVDeDirectorio()
        {
            if (File.Exists(rutaGuardado_ui.text))
            {
                eventos.Clear();
                eventosConvertidos.Clear();
                for (int i = lista_eventos_ui.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(lista_eventos_ui.transform.GetChild(i).gameObject);
                }
            }

            try
            {
                string data = File.ReadAllText(rutaGuardado_ui.text);
                var csv = CSVReader.ReadString(data);
                for (int i = 0; i < csv.Count; i++)
                {
                    string textEv = (string)csv[i]["data"];
                    textEv = textEv[1..^1];
                    AddExistingEvent(new(JSON.Parse(textEv)));
                }
            }
            catch (System.Exception e) { TempMessageController.Instancia.GenerarMensaje(e.Message); }
            
            if (eventos.Count > 0) SelectEvent(0);
        }

        public void GuardarCSVEnDirectorio()
        {
            Convertir_A_JSON(); // Rellenar eventosConvertidos
            //Juntarlo todo en un CSV
            List<Dictionary<string, object>> csv = new(eventosConvertidos.Count);
            for (int i = 0; i < eventosConvertidos.Count; i++)
            {
                csv.Add(new(2) { { "id", eventos[i].id }, 
                                 { "data", eventosConvertidos[i] } });
            }
            string todosLosEventos = CSVReader.Convert2string(csv);
            try
            {
                var stream = File.CreateText(rutaGuardado_ui.text);
                stream.Write(todosLosEventos);
                stream.Close();
            }
            catch (System.Exception e) { TempMessageController.Instancia.GenerarMensaje(e.Message); }
        }

        #region UI_STUFF
        int eventSelectedInList = 0;
        public void AddNewEvent()
        {
            int id = eventos.Count;
            var ev = new Evento(id) { titulo = "Nuevo Evento" };
            eventos.Add(ev);
            var elem = Instantiate(lista_eventos_elem, lista_eventos_ui.transform).GetComponent<ListaEventoElem>();
            elem.pointing_to = id;
            elem.Setup();
            SelectEvent(id);
        }
        private void AddExistingEvent(Evento ev)
        {
            int id = eventos.Count;
            eventos.Add(ev);
            var elem = Instantiate(lista_eventos_elem, lista_eventos_ui.transform).GetComponent<ListaEventoElem>();
            elem.GetComponentInChildren<TextMeshProUGUI>().text = ev.titulo;
            elem.pointing_to = id;
            elem.Setup();
        }

        public void RemoveEvent()
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;

            for (int i = eventSelectedInList + 1; i < eventos.Count; i++)
            {
                var elem = lista_eventos_ui.transform.GetChild(i).GetComponent<ListaEventoElem>();
                elem.pointing_to = --eventos[i].id;
            }

            Destroy(lista_eventos_ui.transform.GetChild(eventSelectedInList).gameObject);
            eventos.RemoveAt(eventSelectedInList);
        }

        public void SelectEvent(int i)
        {
            if (i < 0 || i >= eventos.Count) return;
            eventSelectedInList = i;
            UpdateUI();
        }
        public void UpdateUI()
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
            var ev = eventos[eventSelectedInList];

            nombre_ui.SetTextWithoutNotify(ev.titulo);
            diario_ui.SetIsOnWithoutNotify(ev.diario);
            comprobable_ui.SetValueWithoutNotify((int)ev.momentoComprobable);
            probabilidad_ui.SetValueWithoutNotify(ev.probabilidad);

            condicion_ui.SetTextWithoutNotify(ev.condicionUnparsed);
            ddb_entriesCount_ui.SetText("Entries Found: " + CountDDBEntries(ev.dialogueDataBaseFile));
            cinematica_ui.SetTextWithoutNotify(ev.cinematicFile);
            table_codes_ui.SetTextWithoutNotify(ev.tableCodesNuevos);
            gameplay_vars_ui.SetTextWithoutNotify(ev.modVarGameplay);
        }

        private void UpdateTitulo(string val)
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
            eventos[eventSelectedInList].titulo = val;

            var elem_lista = lista_eventos_ui.transform.GetChild(eventSelectedInList);
            elem_lista.GetComponentInChildren<TextMeshProUGUI>().text = val;
        }
        private void UpdateDiario(bool val)
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
            eventos[eventSelectedInList].diario = val;
        }
        private void UpdateComprobable(int val)
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
            eventos[eventSelectedInList].momentoComprobable = val switch
            {
                0 => Evento.Comprobable.AL_INICIO_DIA,
                1 => Evento.Comprobable.AL_FIN_CASO,
                _ => throw new System.NotImplementedException(),
            };
        }
        private void UpdateProbabilidad(float val)
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
            eventos[eventSelectedInList].probabilidad = (int)val;
        }
        private void UpdateCondicion(string val)
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
            bool ok;
            try
            {
                ok = eventos[eventSelectedInList].IncluirCondicion(val);
            }
            catch (System.Exception e)
            {
                ok = false;
                //Debug.LogException(e);
                TempMessageController.Instancia.GenerarMensaje(e.Message);
            }
            condicion_validator_ui.gameObject.SetActive(!ok);
        }
        private void EditDDB()
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
            // Cargar ddb a editor
            string json = eventos[eventSelectedInList].dialogueDataBaseFile;
            DialogueEditorHelper.CargarTextoDDB(json);
            ddb_entriesCount_ui.text = "Entries Found: " + CountDDBEntries(json);
            // Mostrar editor
            anim.SetTrigger("Go2DE");
        }
        private void CloseDDB()
        {
            if (DialogueRow4Helper.HasAnyErrors()) return;
            anim.SetTrigger("Go2EC");
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
            string json = DialogueEditorHelper.GuardarTextoDDB();
            eventos[eventSelectedInList].dialogueDataBaseFile = json;
            ddb_entriesCount_ui.text = "Entries Found: " + CountDDBEntries(json);
        }
        private void UpdateCinematica(string val)
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;

            val = val.Replace("\r","");
            val = val.Replace("\n","");
            val = val.Replace("\t","");

            //Error checking
            var parse = JSON.Parse(val);
            if (parse == null) Debug.LogWarning("El archivo de cinemáticas no es un JSON válido");
            string errMsg = CinematicaUtilities.CinematicaValida(val);
            if (!errMsg.Equals("")) TempMessageController.Instancia.GenerarMensaje(errMsg);
            
            eventos[eventSelectedInList].cinematicFile = val;
        }
        private void UpdateTC(string val)
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
            eventos[eventSelectedInList].tableCodesNuevos = val;
        }
        private void UpdateModVars(string val)
        {
            if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
            eventos[eventSelectedInList].modVarGameplay = val;
        }

        private int CountDDBEntries(string ddb)
        {
            if (ddb.Equals("")) return 0;
            var j = JSON.Parse(ddb);
            if (j == null) return 0;
            return j.Count;
        }

        #endregion

        private void OnEnable()
        {
            ListaEventoElem.ute = this;
            boton_guardar_ui.onClick.AddListener(GuardarCSVEnDirectorio);
            boton_cargar_ui.onClick.AddListener(CargarCSVDeDirectorio);

            boton_crear_evento_ui.onClick.AddListener(AddNewEvent);
            boton_destruir_evento_ui.onClick.AddListener(RemoveEvent);

            nombre_ui.onEndEdit.AddListener(UpdateTitulo);
            diario_ui.onValueChanged.AddListener(UpdateDiario);
            comprobable_ui.onValueChanged.AddListener(UpdateComprobable);
            probabilidad_ui.onValueChanged.AddListener(UpdateProbabilidad);
            condicion_ui.onEndEdit.AddListener(UpdateCondicion);
            boton_ddb_ui.onClick.AddListener(EditDDB);
            boton_cerrar_ddb_ui.onClick.AddListener(CloseDDB);

            cinematica_ui.onEndEdit.AddListener(UpdateCinematica);
            table_codes_ui.onEndEdit.AddListener(UpdateTC);
            gameplay_vars_ui.onEndEdit.AddListener(UpdateModVars);
        }
        private void OnDisable()
        {
            boton_guardar_ui.onClick.RemoveListener(GuardarCSVEnDirectorio);
            boton_cargar_ui.onClick.RemoveListener(CargarCSVDeDirectorio);

            boton_crear_evento_ui.onClick.RemoveListener(AddNewEvent);
            boton_destruir_evento_ui.onClick.RemoveListener(RemoveEvent);

            nombre_ui.onEndEdit.RemoveListener(UpdateTitulo);
            diario_ui.onValueChanged.RemoveListener(UpdateDiario);
            comprobable_ui.onValueChanged.RemoveListener(UpdateComprobable);
            probabilidad_ui.onValueChanged.RemoveListener(UpdateProbabilidad);
            condicion_ui.onEndEdit.RemoveListener(UpdateCondicion);
            boton_ddb_ui.onClick.RemoveListener(EditDDB);
            boton_cerrar_ddb_ui.onClick.RemoveListener(CloseDDB);

            cinematica_ui.onEndEdit.RemoveListener(UpdateCinematica);
            table_codes_ui.onEndEdit.RemoveListener(UpdateTC);
            gameplay_vars_ui.onEndEdit.RemoveListener(UpdateModVars);
        }
    }
}