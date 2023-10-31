using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ImpresorResultado : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textoResultado;
    [SerializeField] Boton3D IO_Button;
    [SerializeField] int maxLineLength = 40;
    [SerializeField] RectTransform contentFather;

    public static ImpresorResultado Instancia { get; set; }

    public void IntroducirResultado(string r)
    {
        textoResultado.text = QueryResultBeautifierV2(r);
        if (IO_Button != null && textoResultado.transform.localScale.x == 0) IO_Button.SendClick();
    }

    private string QueryResultBeautifier(string json)
    {
        json = "{query:[" + json + "]}";
        var nodo = SimpleJSON.JSON.Parse(json);
        var filas = nodo["query"].AsArray;

        List<string> columnas = new List<string>();
        foreach (var key in filas[0].Keys) columnas.Add(key);

        int[] anchura = CalcularAnchura(columnas.Count, ref filas);

        StringBuilder filasBienColocadas = new StringBuilder();
        for(int i = 0; i < filas.Count; i++)
        {
            SimpleJSON.JSONNode fila = filas[i];
            AddRow(ref filasBienColocadas, ref anchura, ref fila);
        }

        return EncabezadoTabla(ref columnas, ref anchura) + filasBienColocadas.ToString() + FinTabla(ref anchura);
    }

    private string QueryResultBeautifierV2(string json)
    {
        json = "{query:[" + json + "]}";
        var nodo = SimpleJSON.JSON.Parse(json);
        var filas = nodo["query"].AsArray;

        List<string> columnas = new List<string>();
        TableCreator tc = new TableCreator();
        tc.maxWidth = maxLineLength;
        //Calculamos numero de columnas
        foreach (var key in filas[0].Keys)
        {
            print(key);
            columnas.Add(key);
            tc.AddColumn(key, 2);
        }
        //Calculamos anchura de columnas y altura de cada fila.
        for (int i = 0; i < filas.Count; i++)
        {
            var vals = filas[i].Values;
            List<string> datosFila = new List<string>();
            while (vals.MoveNext()) datosFila.Add(vals.Current);
            tc.AddRow(datosFila, 2);
        }

        //Metemos texto de cada fila
        StringBuilder filasBienColocadas = new StringBuilder();
        for (int i = 0; i < filas.Count; i++)
        {
            SimpleJSON.JSONNode fila = filas[i];
            AddSeparator(ref filasBienColocadas,tc);
            AddRowV2(ref filasBienColocadas, ref tc, ref fila, i);
        }

        return EncabezadoTablaV2(tc, columnas) + filasBienColocadas.ToString() + FinTablaV2(tc.currentWidth+columnas.Count);
    }

    private void AddRow(ref StringBuilder sb, ref int[] anchura, ref SimpleJSON.JSONNode fila)
    {
        int idxCol = 0;
        var vals = fila.Values;
        sb.Append("|");
        while (vals.MoveNext())
        {
            sb.Append(' ');
            string value = vals.Current;
            sb.Append(value);
            int l = anchura[idxCol] - value.Length - 1;
            for (int j = 0; j < l; j++) sb.Append(" ");
            sb.Append("|");

            idxCol++;
        }
        sb.Append("\n");
    }

    private void AddRowV2(ref StringBuilder sb, ref TableCreator tc, ref SimpleJSON.JSONNode fila, int idxRow)
    {        
        int h = tc.heightsPerRow[idxRow];

        for (int hi = 0; hi < h; hi++)
        {
            int idxCol = 0;
            sb.Append("|");
            var vals = fila.Values; //Reiniciar bucle
            while (vals.MoveNext())
            {
                sb.Append(' ');
                string value = vals.Current;
                int cw = tc.widthPerColumn[idxCol];
                print(value+" + "+cw);
                int fw = cw - 2;

                if(value.Length > fw) //Hay que hacer división
                {
                    string cacho;
                    int offset = hi * fw;
                    if (hi == h-1) //El resto
                    {
                        int resto = value.Length % fw;
                        cacho = value.Substring(offset, resto) + new string(' ',fw-resto);
                    }
                    else //La división
                    {
                        int tam = value.Length / fw;
                        cacho = value.Substring(offset, tam);
                    }
                    sb.Append(cacho);
                }
                else if(hi < 0) //Ponemos todo espacios en blanco
                {
                    sb.Append(new string(' ', fw));
                }
                else //Ponemos la cadena directamente
                {
                    sb.Append(value);
                    int difer = fw - value.Length;
                    sb.Append(new string(' ', difer));
                }
                sb.Append(" |");

                idxCol++;
            }
            sb.Append("\n");

        }
    }
    
    private string FinTabla(ref int[] anchura)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append('\\');
        for (int i = 0; i < anchura.Length; i++)
        {
            int length = anchura[i];
            for (int j = 0; j < length; j++) sb.Append('-');
            if (i < anchura.Length - 1) sb.Append('-');
        }
        sb.Append("/\n");
        return sb.ToString();
    }

    private string FinTablaV2(int l)
    {
        return "\\" + new string('-', l - 2) + "/\n";
    }

    private string EncabezadoTabla(ref List<string> columnas, ref int[] anchura)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append('/');
        for (int i = 0; i < anchura.Length; i++)
        {
            int length = anchura[i];
            for (int j = 0; j < length; j++) sb.Append('-');
            if (i < anchura.Length - 1) sb.Append('-');
        }
        sb.Append("\\\n|");
        for(int i = 0; i < anchura.Length; i++)
        {
            sb.Append(" ");
            sb.Append(columnas[i]);
            int l = anchura[i] - columnas[i].Length - 1;
            for (int j = 0; j < l; j++) sb.Append(" ");
            sb.Append("|");
        }
        sb.Append("\n+");
        for (int i = 0; i < anchura.Length; i++)
        {
            int length = anchura[i] - 1;
            sb.Append('-');
            for (int j = 0; j < length; j++) sb.Append('-');
            sb.Append('+');
        }
        sb.Append("\n");

        return sb.ToString();
    }

    private string EncabezadoTablaV2(TableCreator tc, List<string> columnas)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append('/');
        int n = tc.widthPerColumn.Count;
        for (int i = 0; i < n; i++)
        {
            int length = tc.widthPerColumn[i];
            for (int j = 0; j < length; j++) sb.Append('-');
            if (i < tc.widthPerColumn.Count - 1) sb.Append('-');
        }
        sb.Append("\\\n|");
        for (int i = 0; i < n; i++)
        {
            sb.Append(" ");
            sb.Append(columnas[i]);
            int l = tc.widthPerColumn[i] - columnas[i].Length - 1;
            for (int j = 0; j < l; j++) sb.Append(" ");
            sb.Append("|");
        }
        sb.Append("\n");

        return sb.ToString();
    }

    private void AddSeparator(ref StringBuilder sb, TableCreator tc)
    {
        int n = tc.widthPerColumn.Count;
        sb.Append('+');
        for (int i = 0; i < n; i++)
        {
            int length = tc.widthPerColumn[i] - 1;
            sb.Append('-');
            for (int j = 0; j < length; j++) sb.Append('-');
            sb.Append('+');
        }
        sb.Append("\n");
    }

    private int[] CalcularAnchura(int nColumnas, ref SimpleJSON.JSONArray filas)
    {
        int[] anchura = new int[nColumnas];

        var cols = filas[0].Keys;
        int c = 0;
        while (cols.MoveNext()) //Equivale a foreach supuestamente...
        {
            string value = cols.Current;
            if (anchura[c] < value.Length) anchura[c] = value.Length;
            c++;
        }

        for (int i = 0; i < filas.Count; i++)
        {
            var vals = filas[i].Values;
            c = 0;
            while (vals.MoveNext()) //Equivale a foreach supuestamente...
            {
                string value = vals.Current;
                if (anchura[c] < value.Length) anchura[c] = value.Length;
                c++;
            }
        }
        for (int i = 0; i < anchura.Length; i++) anchura[i] += 2;
        
        return anchura;
    }

    private void Awake()
    {
        Instancia = this;
    }

    private void Test()
    {
        TableCreator tc = new TableCreator();
        tc.AddColumn("123", 2);
        tc.AddColumn("123", 2);
        tc.AddColumn("123", 2);
        ImprimirTC(tc);
        tc.AddRow(new List<string> { "Pepito", "1", "1" }, 2);
        ImprimirTC(tc);
        tc.AddRow(new List<string> { "MECAGONTOLOCAGABLEMADREMIAWILLYPEROQUEESTASHACIENDOCOMPAÑERO", "1", "12" }, 2);
        ImprimirTC(tc);
    }
    private void ImprimirTC(TableCreator tc)
    {
        Debug.Log("WpC: " + tc.widthPerColumn.Count);
        for (int i = 0; i < tc.widthPerColumn.Count; i++) Debug.Log(i);
        Debug.Log("CW");
        Debug.Log(tc.currentWidth);
        Debug.Log("HpR: " + tc.heightsPerRow.Count);
        for (int i = 0; i < tc.heightsPerRow.Count; i++) Debug.Log(i);
        Debug.Log("----------------------------");
    }
}
