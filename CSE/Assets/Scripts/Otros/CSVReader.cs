using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

public class CSVReader
{
    static string SPLIT_RE = @";(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string,object>> Read(TextAsset data)
    {
        if (data == null)
        {
            Debug.LogWarning("Archivo no encontrado");
            return null;
        }
        return ReadString(data.text);
    }

    public static List<Dictionary<string, object>> Read(string file)
    {
        TextAsset data = Resources.Load<TextAsset>(file);
        if (data == null)
        {
            Debug.LogWarning("Archivo no encontrado");
            return null;
        }
        return ReadString(data.text);
    }

    public static List<Dictionary<string, object>> ReadString(string text)
    {
        var list = new List<Dictionary<string, object>>();
        var lines = Regex.Split(text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                //value = value.Trim(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value.Replace("·", "\"");
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public static string Convert2string(List<Dictionary<string,object>> csv)
    {
        int n = csv.Count;
        if (n == 0) return "";
        StringBuilder sb = new();
        List<string> cols = new();
        
        // Obteniendo Columnas
        foreach (var k in csv[0].Keys) cols.Add(k);
        int m = cols.Count;
        if (m == 0) return "";

        // Introduciendo primera fila (nombre de las columnas)
        for (int i = 0; i < cols.Count; i++) sb.Append(cols[i] + ';');
        sb[^1] = '\r';
        sb.Append('\n');

        // Introduciendo el resto de filas
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                object obj = csv[i][cols[j]];
                if (obj == null) sb.Append(';'); // Podría colisionar dos líneas más abajo?
                else if (obj.IsNumber()) sb.Append(obj.ToString() + ';');                   // if Number
                else sb.Append('\"' + obj.ToString().Replace("\"", "·").Replace("\n","")+ "\";");        // if String
            }
            sb[^1] = '\r';
            sb.Append('\n');
        }
        return sb.ToString();
    }
}

public static class DontFuckWithMe
{
    public static bool IsNumber(this object obj)
    {
        return obj is sbyte || obj is byte ||
            obj is short || obj is ushort ||
            obj is int || obj is uint ||
            obj is long || obj is ulong ||
            obj is float || obj is double || obj is decimal;
    }
}