using UnityEngine;
using TMPro;
using System;
using System.Text;
using System.Collections.Generic;

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
    [SerializeField] Vector2 letterPadding;
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
        int len = pantalla.text.Length;
        int caretP = Math.Min(pantalla.caretPosition, len);
        if (caretP == 0) return;
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
                searchingStartOfWord = isNotSpace(c) && isNotDot(c) && isNotComma(c) && isNotSpecial(c);
            }
            int inicio = 0;
            if (i > 0) inicio = i + 1;

            int len = pantalla.text.Length;
            int j = Math.Min(pantalla.caretPosition, len - 1);
            for (; j < len && isNotSpace(pantalla.text[j]) && isNotSpecial(pantalla.text[j]); j++) { }
            
            StringBuilder sb = new StringBuilder();
            sb.Append(pantalla.text[..inicio]);
            sb.Append(palabra);
            if(pantalla.caretPosition >= len) sb.Append(' ');
            if (j < len) sb.Append(pantalla.text[j..len]);
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
            int len = pantalla.text.Length;
            int cp = Math.Min(pantalla.caretPosition, len);
            int i = cp - 1;
            for (; i > 0 && isNotSpace(pantalla.text[i]); i--) { }
            int inicio = 0;
            if (i != 0) inicio = i + 1;
            int j = cp;
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
        else if (charToValidate == '\"' || charToValidate == '(' || charToValidate == '\'')
        {
            // Check left adyacent char
            if (index > 0 && charToValidate != '(')
            {
                char left = input[index-1];
                if (isLetter(left) || charToValidate == left) return charToValidate;
            }
            // Check right adyacent char
            if (index < input.Length-1)
            {
                char right = input[index + 1];
                if (isLetter(right) || charToValidate == right) return charToValidate;
            }
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

    /// <summary>
    /// Función que actualiza las sugerencias. Se debe llamar cada vez que se modifica el texto.
    /// </summary>
    private void Edicion(string nuevoValor)
    {
        if (pantalla.caretPosition <= 0)
        {
            moverSugerencias = false;
            return;
        }
        int i = Math.Min(pantalla.caretPosition, nuevoValor.Length) - 1;
        //print(i+"/"+nuevoValor.Length);
        char c = nuevoValor[i];
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
        for (; i > 0 && isNotSpace(pantalla.text[i]) && isNotSpecial(pantalla.text[i]); i--) {}
        int inicio = 0;
        if (i != 0) inicio = i+1;
        string patron = pantalla.text[inicio..caretP];
        //Usar un pool para las palabras...
        keywords.Load();

        int opAcceso = patron.IndexOf('.');
        if (opAcceso > 1) // Modo acceso a tabla
        {
            var data = patron.Split('.');
            indices = keywords.GetIndexOfColumnsFromTable(data[0], data[1],true);
        }
        else // Modo genérico
        {
            indices = keywords.GetIndexOcurrenciesOf(patron,true);
        }

        for (int j = 0; j < poolSugerencias.Count; j++) Destroy(poolSugerencias[j]);
        poolSugerencias.Clear();
        if (indices.Count == 0) moverSugerencias = false;
        else for (int j = 0; j < indices.Count; j++) AddToPool(keywords.kw[indices[j]], j);
    }

    private void SugerenciasACaret()
    {
        if (!sugerencias.gameObject.activeInHierarchy) return;
        int cp = Math.Clamp(pantalla.caretPosition, 0, pantalla.text.Length - 1);
        if (!isNotSpace(pantalla.text[cp])) moverSugerencias = false; 
        Vector2 pos = CaretPos();
        pos.x *= letterSize.x + letterPadding.x;
        pos.y = pos.y * (letterSize.y + letterPadding.y) + letterSize.y;
        sugerencias.anchoredPosition = pos;
    }

    private bool isNotSpace(char c) => (c != ' ' && c != '\t' && c != '\n');
    private bool isNotSpecial(char c) => (c != '\'' && c != '\"' && c != '(');
    private bool isNotDot(char c) => (c != '.');
    private bool isNotComma(char c) => (c != ',');

    // No correcto completamente pero vale para la mayoría de casos
    private bool isLetter(char c) => ((c >= 65 && c <= 90) || (c >= 97 && c <= 122));
    

    private Vector2 CaretPos()
    {
        string s = pantalla.text[..(Math.Max(0,pantalla.caretPosition-1))];
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