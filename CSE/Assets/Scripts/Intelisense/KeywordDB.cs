using System.Collections.Generic;
using UnityEngine;
using Hexstar.CSE;

[CreateAssetMenu(menuName = "Hexstar/KeyWord Database")]
public class KeywordDB : ScriptableObject
{
    [SerializeField] private Keyword[] kWords = new Keyword[0];
    [HideInInspector] public List<Keyword> kw = new List<Keyword>();
    [SerializeField] private AlmacenDeBloquesSimple keywordsByDif;

    /// <summary>
    /// Devuelve una lista de indices de keywords con prioridad.
    /// Primero se ponen los que empiezan por el patron especifico,
    /// luego se ponen los que contienen ese patrón,
    /// y por último se añaden las que contienen los caracteres del patrón pero en cualquier orden.
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public List<int> GetIndexOcurrenciesOf(string pattern, bool skipDoubles = false)
    {
        int n = kw.Count;
        int l = pattern.Length;
        List<bool> comprobado = new List<bool>();
        List<int> ocurrencies = new List<int>();
        for (int i = 0; i < n; i++)
        {
            comprobado.Add(kw[i].SmallerThanPattern(l));
        }
        for (int i = 0; i < n; i++)
        {
            if (comprobado[i]) continue;
            if (kw[i].StartsWith(pattern)) { 
                ocurrencies.Add(i);
                comprobado[i] = true;
            }
        }
        for (int i = 0; i < n; i++)
        {
            if (comprobado[i]) continue;
            if (kw[i].ContainsSection(pattern)) {
                ocurrencies.Add(i);
                comprobado[i] = true;
            }
        }
        //Si la longitud del patrón es 1, entonces la última comprobación
        //es la misma que la que busca secciones y se puede omitir.
        if (l > 1)
        {
            for (int i = 0; i < n; i++)
            {
                if (comprobado[i]) continue;
                if (kw[i].ContainsChars(pattern))
                {
                    ocurrencies.Add(i);
                    comprobado[i] = true;
                }
            }
        }
        //Buscar en la descripción (Última prioridad)
        for (int i = 0; i < n; i++)
        {
            if (comprobado[i]) continue;
            if (kw[i].InDescription(pattern))
            {
                ocurrencies.Add(i);
                comprobado[i] = true;
            }
        }

        if (skipDoubles)
        {
            for (int i = ocurrencies.Count-1; i >= 0; i--)
            {
                var toCheck = kw[ocurrencies[i]];
                for (int j = 0; j < i; j++)
                {
                    if (toCheck.Equals(kw[ocurrencies[j]]))
                    {
                        ocurrencies.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        return ocurrencies;
    }

    public List<int> GetIndexOfColumnsFromTable(string table, string pattern, bool skipDoubles = false)
    {
        List<int> ocurrencies = new List<int>();
        List<string> columnasABuscar = AlmacenDePalabras.GetColumnasDeTabla(table);
        
        for(int i = 0; i < kw.Count; i++)
        {
            if (!kw[i].description.Equals("(Columna)")) continue;
            if (!columnasABuscar.Contains(kw[i].name)) continue;

            if (pattern.Length == 0) ocurrencies.Add(i);
            else if (kw[i].ContainsSection(pattern)) ocurrencies.Add(i);
            
        }
        if (skipDoubles)
        {
            for (int i = ocurrencies.Count - 1; i >= 0; i--)
            {
                var toCheck = kw[ocurrencies[i]];
                for (int j = 0; j < i; j++)
                {
                    if (toCheck.Equals(kw[ocurrencies[j]]))
                    {
                        ocurrencies.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        return ocurrencies;
    }

    public void Load()
    {
        kw.Clear();

        kw.AddRange(ConvertStringToKeywords(AlmacenDePalabras.palabras[0],"(Tabla)"));
        kw.AddRange(ConvertStringToKeywords(AlmacenDePalabras.palabras[1],"(Columna)",true));
        kw.AddRange(ConvertStringToKeywords(AlmacenDePalabras.palabras[2],"(Pista)"));
        kw.AddRange(String2KeywordsByDiff(AlmacenDePalabras.palabras[3],"(Función)"));
        kw.AddRange(String2KeywordsByDiff(AlmacenDePalabras.GetOperadoresEspeciales(), "(Operador)"));

        for (int i = 0; i < kWords.Length; i++) if (ExisteYPuede(kWords[i].name)) kw.Add(kWords[i]);
    }

    private List<Keyword> ConvertStringToKeywords(List<string> lista, string tipo = "", bool filtrar = false)
    {
        List<Keyword> keywords = new();
        foreach(var elem in lista)
        {
            if (filtrar && elem.Contains(':')) continue;
            keywords.Add(new Keyword(elem, tipo));
        }
        return keywords;
    }

    /// <summary>
    /// Will return false ONLY when the keyword is found and is locked by its difficulty
    /// </summary>
    private bool ExisteYPuede(string elem)
    {
        int diff = ResourceManager.DificultadActual;
        int n = keywordsByDif.bloquesPrefab.Count;

        for (int i = 0; i < n; i++)
        {
            var b = keywordsByDif.bloquesPrefab[i];
            if (b.titulo.Equals(elem)) return b.disponibleEnDificultad <= diff;
        }
        return true;
    }
    private List<Keyword> String2KeywordsByDiff(List<string> lista, string tipo = "")
    {
        List<Keyword> keywords = new();
        bool canCheck = keywordsByDif != null;

        foreach (var elem in lista)
        {
            if (canCheck)
            {
                if (ExisteYPuede(elem)) keywords.Add(new Keyword(elem, tipo));
            }
            else keywords.Add(new Keyword(elem, tipo));
        }

        return keywords;
    }
}

[System.Serializable]
public struct Keyword
{
    public string name;
    public string description;

    public Keyword(string n, string d = default)
    {
        name = n;
        description = d;
    }

    public bool Equals(Keyword other)
    {
        return other.name.Equals(name) && other.description.Equals(description);
    }

    public bool SmallerThanPattern(int pSize)
    {
        return name.Length < pSize;
    }

    public bool StartsWith(string inicio)
    {
        string i = inicio.ToLower();
        return name.ToLower().StartsWith(i);
    }

    public bool ContainsSection(string subcadena)
    {
        string i = subcadena.ToLower();
        return name.ToLower().Contains(i);
    }

    public bool ContainsChars(string chars)
    {
        string ca = chars.ToLower();
        foreach (char c in ca)
        {
            if (!name.Contains(c+"")) return false;
        }
        return true;
    }

    public bool InDescription(string desc)
    {
        if (desc.Length < 2) return false;
        return description.ToLower().Contains(desc.ToLower());
    }
};
