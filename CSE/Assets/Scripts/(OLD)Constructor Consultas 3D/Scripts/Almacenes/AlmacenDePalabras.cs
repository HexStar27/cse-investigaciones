using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Collections.Generic;

namespace Hexstar.CSE
{
    public enum TabType { Tablas = 0, Columnas = 1, Pistas = 2, Funciones = 3, Operadores = 4 };

    public static class AlmacenDePalabras
    {
        //5 Listas de palabras, una para cada tipo
        public static List<string>[] palabras = { new List<string>(), new List<string>(),
            new List<string>(), new List<string>(), new List<string>() };

        private static readonly string[] operadores = {
            "+","-","*","/", //Aritméticos (4)
            "=","<",">","<=",">=","<>", //Comparación (6)
            "(",")", //Otros... (2)
            "AND", "OR", "NOT", "BETWEEN", "IN", "LIKE", "ALL", "ANY", "EXIST", //Lógicos (3-3-3)
        };
        //Not supported yet: IN, LIKE, subqueries (se arreglaría con un bloque doble con flecha)
        public static readonly int[] difficultyToOPsAvilables = {10,15,17,21,21};
        private static readonly string[] funciones =
        {
            "LENGTH",
            "ABS",
            "AVG",
            "COUNT",
            "GREATEST",
            "LEAST",
            "MAX",
            "MIN",
            "ROUND",
            "SIGN",
            "SUM",
        };

        public static void CargarPistasDeCasoActivo()
        {
            int pistaIdx = (int)TabType.Pistas;
            palabras[pistaIdx] = new List<string>();
            Caso c = PuzzleManager.GetCasoActivo();
            foreach(var pista in c.pistas) palabras[pistaIdx].Add(pista.palabra);
        }

        public static void CargarPalabras()
        {
            for (int i = 0; i < 5; i++) palabras[i] = new List<string>();
            //1º Cargar tablas y columnas
            
            //Cambiar esto por los codigos que se guardaran en los archivos de guardado más adelante...
            string codigos = "\"ejemplo0\", \"ejemplo1\"";

            WWWForm form1 = new WWWForm();
            form1.AddField("authorization",SesionHandler.sessionKEY);
            form1.AddField("codigos", "{\"codigos\":["+codigos+"]}");
            ConexionHandler.onFinishRequest.AddListener(TablasYColumnas);
            _ = ConexionHandler.APost(ConexionHandler.baseUrl + "meta", form1);

            //2º Cargar operadores y funciones
            palabras[4].AddRange(operadores);
        }

        private static async void TablasYColumnas(DownloadHandler download)
        {
            string obtenido = ConexionHandler.ExtraerJson(download.text);
            ConexionHandler.onFinishRequest.RemoveListener(TablasYColumnas);

            //1º Tablas
            palabras[1].Add("*");
            JSONNode datos = JSON.Parse(obtenido);
            int n = datos.Count;
            for (int j = 0; j < n; j++)
            {
                var nodo = datos[j]["content"]["tables"];
                int m = nodo.Count;
                for (int i = 0; i < m; i++) palabras[0].Add(nodo[i].Value);
            }

            //2º Columnas
            for (int i = 0; i < palabras[0].Count; i++)
            {
                WWWForm form = new WWWForm();
                form.AddField("authorization", SesionHandler.sessionKEY);
                form.AddField("table", palabras[0][i]);
                //Debug.Log("Enviando "+ palabras[0][i]);
                await ConexionHandler.APost(ConexionHandler.baseUrl + "meta/col", form);
                Columnas(ConexionHandler.download);
            }
        }

        private static void Columnas(string download)
        {
            string obtenido = ConexionHandler.ExtraerJson(download);
            JSONNode datos = JSON.Parse(obtenido);

            string t = datos["table"];
            var col = datos["columns"];
            int n = col.Count;

            palabras[1].Add("Tabla: "+t);
            for(int i = 0; i < n; i++)
            {
                palabras[1].Add(col[i]["COLUMN_NAME"].Value);
                //Debug.Log(col[i]["COLUMN_NAME"].Value);
            }
        }


        public static List<string> GetLista(TabType tab)
        {
            return palabras[(int)tab];
        }
        public static List<string> GetLista(int indice)
        {
            return palabras[indice];
        }

        public static int TipoAIndice(TabType tipo)
        {
            return (int)tipo;
        }

        public static bool IsOperador(string op)
        {
            int n = palabras[(int)TabType.Operadores].Count;
            for (int i = 0; i < n; i++)
            {
                if (palabras[(int)TabType.Operadores][i].Equals(op)) return true;
            }
            return false;
        }

        public static bool ContainsOperador(string op)
        {
            int n = palabras[(int)TabType.Operadores].Count;
            for (int i = 0; i < n; i++)
            {
                if (op.Contains(palabras[(int)TabType.Operadores][i])) return true;
            }
            return false;
        }
    }
}