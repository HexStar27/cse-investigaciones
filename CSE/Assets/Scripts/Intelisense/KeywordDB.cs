using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hexstar/KeyWord Database")]
public class KeywordDB : ScriptableObject
{
    [SerializeField] private Keyword[] kWords = new Keyword[0];
    [HideInInspector] public List<Keyword> kw = new List<Keyword>();

    /// <summary>
    /// Devuelve una lista de indices de keywords con prioridad.
    /// Primero se ponen los que empiezan por el patron especifico,
    /// luego se ponen los que contienen ese patrón,
    /// y por último se añaden las que contienen los caracteres del patrón pero en cualquier orden.
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public List<int> GetIndexOcurrenciesOf(string pattern)
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
        //Si la longitud del patrón es 1, entonces la última comprobación es la misma que la anterior
        //y se puede omitir.
        if (l == 1) return ocurrencies;
        for (int i = 0; i < n; i++)
        {
            if (comprobado[i]) continue;
            if (kw[i].ContainsChars(pattern)) {
                ocurrencies.Add(i);
                comprobado[i] = true;
            }
        }

        return ocurrencies;
    }

    public void Load()
    {
        kw.Clear();
        for(int i = 0; i <= 3; i++)
        {
            foreach (string palabra in Hexstar.CSE.AlmacenDePalabras.palabras[i]) kw.Add(new Keyword(palabra));
        }
        for (int i = 0; i < kWords.Length; i++) kw.Add(kWords[i]);
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
};
