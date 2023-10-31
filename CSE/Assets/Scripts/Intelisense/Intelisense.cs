using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;
using UnityEngine.EventSystems;

public class Intelisense : MonoBehaviour
{
    public static Intelisense instance;

    [SerializeField] KeywordDB keywords;
    [SerializeField] TMP_InputField pantalla;
    [SerializeField] RectTransform sugerencias;
    [SerializeField] RectTransform contenidoSugerencias;
    private List<GameObject> poolSugerencias = new List<GameObject>();
    private List<int> indices = new List<int>();

    [SerializeField] Vector2 letterSize;
    [SerializeField] GameObject sugerenciaPrefab;

    bool ms;
    bool moveS {
        get { return ms; }
        set { 
            ms = value;
            sugerencias.gameObject.SetActive(value);
        }
    }
    bool shouldApplySuggestion = false;
    bool shouldAddCouple = false;

    private void OnEnable()
    {
        instance = this;
        pantalla.onValueChanged.AddListener(Edicion);
        SetupAutocomplete();
    }

    private void OnDisable()
    {
        pantalla.onValueChanged.RemoveListener(Edicion);
    }

    private void FixedUpdate()
    {
        if (moveS) SugerenciasACaret();
        //if (moveS) SeleccionSugerencias();
        if (shouldApplySuggestion) Autocomplete();
        else if (shouldAddCouple) AddCouple();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) moveS = false;
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Backspace)) EraseCurrentCaretWord();
    }

    private void EraseCurrentCaretWord()
    {
        int caretP = pantalla.caretPosition;
        if (caretP == 0) return;
        int len = pantalla.text.Length;
        int i = caretP - 1;
        for (; i > 0 && isNotSpace(pantalla.text[i]); i--) { }
        int inicio = 0;
        if (i != 0) inicio = i + 1;
        StringBuilder sb = new StringBuilder();
        sb.Append(pantalla.text.Substring(0,inicio));
        sb.Append(pantalla.text.Substring(caretP, len - caretP));
        pantalla.text = sb.ToString();
    }

    public void SelectText()
    {
        pantalla.Select();
    }

    private void Autocomplete()
    {
        shouldApplySuggestion = false;
        if (poolSugerencias.Count > 0)
        { //Pillar cadena y meterla en la palabra.
            string palabra = keywords.kw[indices[0]].name;
            int i = pantalla.caretPosition - 1;
            for (; i > 0 && isNotSpace(pantalla.text[i]); i--) { }
            int inicio = 0;
            if (i != 0) inicio = i + 1;
            int j = pantalla.caretPosition;
            int len = pantalla.text.Length;
            for (; j < len && isNotSpace(pantalla.text[j]); j++) { }
            StringBuilder sb = new StringBuilder();
            sb.Append(pantalla.text.Substring(0, inicio));
            sb.Append(palabra);
            sb.Append(' ');
            sb.Append(pantalla.text.Substring(j, len - j));
            pantalla.text = sb.ToString();
            pantalla.caretPosition = inicio + palabra.Length + 1;
        }
        moveS = false;
    }

    public void Autocomplete(int idx)
    {
        shouldApplySuggestion = false;
        if (poolSugerencias.Count > 0)
        { //Pillar cadena y meterla en la palabra.
            string palabra = keywords.kw[indices[idx]].name;
            int i = pantalla.caretPosition - 1;
            for (; i > 0 && isNotSpace(pantalla.text[i]); i--) { }
            int inicio = 0;
            if (i != 0) inicio = i + 1;
            int j = pantalla.caretPosition;
            int len = pantalla.text.Length;
            for (; j < len && isNotSpace(pantalla.text[j]); j++) { }
            StringBuilder sb = new StringBuilder();
            sb.Append(pantalla.text.Substring(0, inicio));
            sb.Append(palabra);
            sb.Append(' ');
            sb.Append(pantalla.text.Substring(j, len - j));
            pantalla.text = sb.ToString();
            pantalla.caretPosition = inicio + palabra.Length + 1;
        }
        moveS = false;
    }

    private void SetupAutocomplete()
    {
        pantalla.onValidateInput += Validate;
    }
    private char Validate(string input, int index, char charToValidate)
    {
        if (charToValidate == '\t' && moveS == true)
        {
            charToValidate = '\0';

            //SeleccionSugerencias();
            shouldApplySuggestion = true;
            moveS = false;
        }
        else if (charToValidate == '\"' || charToValidate == '(')
        {
            shouldAddCouple = true;
        }

        return charToValidate;
    }

    private void AddCouple()
    {
        int i = pantalla.caretPosition - 1;
        char c = pantalla.text[i];
        if (c == '\"') pantalla.text = pantalla.text.Insert(i+1, "\"");
        else if (c == '(') pantalla.text = pantalla.text.Insert(i+1, ")");
        shouldAddCouple = false;
    }

    private void SeleccionSugerencias()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(!AnyElementSelected())
                contenidoSugerencias.GetComponentInChildren<UnityEngine.UI.Button>().Select();
        }
    }

    private bool AnyElementSelected()
    {
        var selectedObj = EventSystem.current.currentSelectedGameObject;
        for (int i = 0; i < contenidoSugerencias.childCount; i++)
        {
            if (selectedObj == contenidoSugerencias.GetChild(i).gameObject) return true;
        }
        return false;
    }

    private void Edicion(string nuevoValor)
    {
        if (pantalla.caretPosition == 0)
        {
            moveS = false;
            return;
        }
        char c = nuevoValor[pantalla.caretPosition - 1];
        if (isNotSpace(c))
        {
            moveS = true;
            LoadWords();
        }
        else moveS = false;
    }

    private void AddToPool(string keyword, int idx = 0)
    {
        GameObject go = Instantiate(sugerenciaPrefab, contenidoSugerencias);
        go.transform.GetComponentInChildren<TextMeshProUGUI>().text = keyword;
        poolSugerencias.Add(go);
        go.GetComponent<KWListButton>().idx = idx;
    }

    private void LoadWords()
    {
        //ObtenerPatrón
        int caretP = pantalla.caretPosition;
        int i = caretP-1;
        for (; i > 0 && isNotSpace(pantalla.text[i]); i--) {}
        int inicio = 0;
        if (i != 0) inicio = i+1;
        string patron = pantalla.text.Substring(inicio,caretP-inicio);

        //Usar un pool para las palabras...
        keywords.Load();
        indices = keywords.GetIndexOcurrenciesOf(patron);
        for (int j = 0; j < poolSugerencias.Count; j++) Destroy(poolSugerencias[j]);
        poolSugerencias.Clear();
        if (indices.Count == 0) moveS = false;
        else for (int j = 0; j < indices.Count; j++) AddToPool(keywords.kw[indices[j]].name, j);
    }

    private void SugerenciasACaret()
    {
        if (!sugerencias.gameObject.activeInHierarchy) return;
        int cp = pantalla.caretPosition - 1;
        if (cp < 0) cp = 0;
        if (!isNotSpace(pantalla.text[cp])) moveS = false; 
        Vector2 pos = CaretPos();
        pos.x *= letterSize.x;
        pos.y = pos.y * letterSize.y + letterSize.y;
        sugerencias.anchoredPosition = pos;
    }

    private bool isNotSpace(char c)
    {
        return c != ' ' && c != '\t' && c != '\n';
    }

    private Vector2 CaretPos()
    {
        string s = pantalla.text.Substring(0, pantalla.caretPosition);
        int cp = s.LastIndexOf('\n');
        Vector2 pos;
        pos.x = pantalla.caretPosition - cp - 1;
        if (cp == -1) pos.y = 0;
        else pos.y = GetNewLineCount(s);
        return pos;
    }

    private int GetNewLineCount(string s)
    {
        int n = s.Length, y = 0;
        for(int i = 0; i < n; i++) if (s[i] == '\n') y++;
        return y;
    }
}


//Lo único que quedaría así por hacer sería:
// -La conexion entre el intelisense y todas las palabras del almacen de palabras...