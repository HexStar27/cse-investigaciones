using System.Collections.Generic;
using UnityEngine;
using Hexstar;
using Hexstar.CSE.SistemaEventos;
using SimpleJSON;
using TMPro;
using UnityEngine.UI;
using Hexstar.UI;

public class UnitTest_Eventos : MonoBehaviour
{
    public List<Evento> eventos = new();
    public List<string> eventosConvertidos = new();

    [Header("UI")]
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
    [Space()]
    [SerializeField] TMP_InputField cinematica_ui;
    [SerializeField] TMP_InputField table_codes_ui;
    [SerializeField] TMP_InputField gameplay_vars_ui;
    [Header("Prefabs")]
    [SerializeField] GameObject lista_eventos_elem;

    /// <summary>
    /// Convierte todos los eventos de la lista de eventos a JSON individualmente.
    /// </summary>
    public void Convertir_A_JSON()
    {
        eventosConvertidos ??= new(); // <=> If null then new()
        eventosConvertidos.Clear();
        for(int i = 0; i < eventos.Count; i++)
        {
            eventosConvertidos.Add(JsonConverter.ConvertirAJson(eventos[i]));
        }
    }
    /// <summary>
    /// Junta todos los eventos JSON en un JSON final listo para ser enviado al servidor.
    /// </summary>
    public string CrearPaquetitoJSON()
    {
        JSONObject jsonEventos = new();
        jsonEventos.Add("EVENTOS", new JSONArray());
        for(int i = 0; i < eventosConvertidos.Count; i++)
        {
            jsonEventos["EVENTOS"].Add(new JSONString(eventosConvertidos[i]));
        }

        return jsonEventos.ToString();
    }

    private void OnApplicationQuit()
    {
        Convertir_A_JSON();
        print(CrearPaquetitoJSON());
    }

    #region UI_STUFF
    int eventSelectedInList = 0;
    public void AddNewEvent() 
    {
        int id = eventos.Count;
        var ev = new Evento(id) { titulo = "Nuevo Evento"};
        eventos.Add(ev);
        var elem = Instantiate(lista_eventos_elem, lista_eventos_ui.transform).GetComponent<ListaEventoElem>();
        elem.pointing_to = id;
        elem.Setup();
        SelectEvent(id);
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
        eventos[eventSelectedInList].momentoComprobable = val switch {
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
        catch(System.Exception e)
        {
            ok = false;
            Debug.LogException(e);
            TempMessageController.Instancia.GenerarMensaje(e.Message);
        }
        condicion_validator_ui.gameObject.SetActive(!ok);
    }
    private void UpdateCinematica(string val)
    {
        if (eventSelectedInList < 0 || eventSelectedInList >= eventos.Count) return;
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
    #endregion

    private void OnEnable()
    {
        ListaEventoElem.ute = this;
        boton_crear_evento_ui.onClick.AddListener(AddNewEvent);
        boton_destruir_evento_ui.onClick.AddListener(RemoveEvent);

        nombre_ui.onEndEdit.AddListener(UpdateTitulo);
        diario_ui.onValueChanged.AddListener(UpdateDiario);
        comprobable_ui.onValueChanged.AddListener(UpdateComprobable);
        probabilidad_ui.onValueChanged.AddListener(UpdateProbabilidad);
        condicion_ui.onEndEdit.AddListener(UpdateCondicion);

        cinematica_ui.onEndEdit.AddListener(UpdateCinematica);
        table_codes_ui.onEndEdit.AddListener(UpdateTC);
        gameplay_vars_ui.onEndEdit.AddListener(UpdateModVars);
    }
    private void OnDisable()
    {
        boton_crear_evento_ui.onClick.RemoveListener(AddNewEvent);
        boton_destruir_evento_ui.onClick.RemoveListener(RemoveEvent);

        nombre_ui.onEndEdit.RemoveListener(UpdateTitulo);
        diario_ui.onValueChanged.RemoveListener(UpdateDiario);
        comprobable_ui.onValueChanged.RemoveListener(UpdateComprobable);
        probabilidad_ui.onValueChanged.RemoveListener(UpdateProbabilidad);
        condicion_ui.onEndEdit.RemoveListener(UpdateCondicion);

        cinematica_ui.onEndEdit.RemoveListener(UpdateCinematica);
        table_codes_ui.onEndEdit.RemoveListener(UpdateTC);
        gameplay_vars_ui.onEndEdit.RemoveListener(UpdateModVars);
    }
}
