using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;
using UnityEngine.EventSystems;

public class Intelisense : MonoBehaviour
{
    public static Intelisense instance;

    [SerializeField] KeywordDB keywords;
    public TMP_InputField pantalla;
    [SerializeField] RectTransform sugerencias;
    [SerializeField] RectTransform contenidoSugerencias;
    private List<GameObject> poolSugerencias = new List<GameObject>();
    private List<int> indices = new List<int>();

    [SerializeField] Vector2 letterSize;
    [SerializeField] GameObject sugerenciaPrefab;

    bool ms;
    bool moverSugerencias {
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
        pantalla.onSelect.AddListener(FocusScreen);
        pantalla.onDeselect.AddListener(UnFocusScreen);
    }

    private void OnDisable()
    {
        pantalla.onValueChanged.RemoveListener(Edicion);
        pantalla.onSelect.RemoveListener(FocusScreen);
        pantalla.onDeselect.RemoveListener(UnFocusScreen);
    }

    private void FixedUpdate()
    {
        if (moverSugerencias) SugerenciasACaret();
        //if (moveS) SeleccionSugerencias();
        if (shouldApplySuggestion) Autocomplete();
        else if (shouldAddCouple) AddCouple();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) moverSugerencias = false;
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Backspace)) EraseCurrentCaretWord();
    }

    /// <summary>
    /// Cuando se ejecuta, la cámara no se moverá por la escena
    /// </summary>
    private void FocusScreen(string _)
    {
        InscryptionLikeCameraState.SetBypass(true);
    }
    /// <summary>
    /// Cuando se ejecuta, la cámara se vovlerá a poder mover por la escena
    /// </summary>
    private void UnFocusScreen(string _)
    {
        InscryptionLikeCameraState.SetBypass(false);
    }


    private void EraseCurrentCaretWord()
    {
        int caretP = pantalla.caretPosition;
        if (caretP == 0) return;
        int len = pantalla.text.Length;
        int i = caretP - 1;
        for (; i > 0 && isNotSpace(pantalla.text[i]) && isNotDot(pantalla.text[i]); i--) { }
        int inicio = 0;
        if (i != 0) inicio = i + 1;
        StringBuilder sb = new StringBuilder();
        sb.Append(pantalla.text[..inicio]);
        sb.Append(pantalla.text[caretP..len]);
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

            int i = pantalla.caretPosition;
            bool searchingStartOfWord = true;
            while (i > 0 && searchingStartOfWord) 
            {
                char c = pantalla.text[--i];
                // Keep looking if it's not any of the word separator characters
                searchingStartOfWord = isNotSpace(c) && isNotDot(c) && isNotComma(c);
            }
            int inicio = 0;
            if (i != 0) inicio = i + 1;

            int j = pantalla.caretPosition;
            int len = pantalla.text.Length;
            for (; j < len && isNotSpace(pantalla.text[j]); j++) { }
            
            StringBuilder sb = new StringBuilder();
            sb.Append(pantalla.text[..inicio]);
            sb.Append(palabra);
            sb.Append(' ');
            sb.Append(pantalla.text[j..len]);
            pantalla.text = sb.ToString();
            pantalla.caretPosition = inicio + palabra.Length + 1;
        }
        moverSugerencias = false;
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
        moverSugerencias = false;
    }

    private void SetupAutocomplete()
    {
        pantalla.onValidateInput += Validate;
    }
    private char Validate(string input, int index, char charToValidate)
    {
        if (charToValidate == '\t' && moverSugerencias == true)
        {
            charToValidate = '\0';

            shouldApplySuggestion = true;
            moverSugerencias = false;
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

    private void Edicion(string nuevoValor)
    {
        if (pantalla.caretPosition == 0)
        {
            moverSugerencias = false;
            return;
        }
        char c = nuevoValor[pantalla.caretPosition - 1];
        if (isNotSpace(c))
        {
            moverSugerencias = true;
            LoadWords();
        }
        else moverSugerencias = false;
    }

    private void AddToPool(Keyword keyword, int idx = 0)
    {
        GameObject go = Instantiate(sugerenciaPrefab, contenidoSugerencias);
        go.transform.GetComponentInChildren<TextMeshProUGUI>().text = keyword.name +" "+ keyword.description;
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
        string patron = pantalla.text[inicio..caretP];
        //Usar un pool para las palabras...
        keywords.Load();

        int opAcceso = patron.IndexOf('.');
        if (opAcceso > 1) // Modo acceso a tabla
        {
            var data = patron.Split('.');
            indices = keywords.GetIndexOfColumnsFromTable(data[0], data[1]);
        }
        else // Modo genérico
        {
            indices = keywords.GetIndexOcurrenciesOf(patron);
        }

        for (int j = 0; j < poolSugerencias.Count; j++) Destroy(poolSugerencias[j]);
        poolSugerencias.Clear();
        if (indices.Count == 0) moverSugerencias = false;
        else for (int j = 0; j < indices.Count; j++) AddToPool(keywords.kw[indices[j]], j);
    }

    private void SugerenciasACaret()
    {
        if (!sugerencias.gameObject.activeInHierarchy) return;
        int cp = pantalla.caretPosition - 1;
        if (cp < 0) cp = 0;
        if (!isNotSpace(pantalla.text[cp])) moverSugerencias = false; 
        Vector2 pos = CaretPos();
        pos.x *= letterSize.x;
        pos.y = pos.y * letterSize.y + letterSize.y;
        sugerencias.anchoredPosition = pos;
    }

    private bool isNotSpace(char c) => (c != ' ' && c != '\t' && c != '\n');
    private bool isNotDot(char c) => (c != '.');
    private bool isNotComma(char c) => (c != ',');

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