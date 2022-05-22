using System;
using UnityEngine;

namespace Hexstar.CSE
{
    public enum TabType {Tablas = 0, Columnas = 1, Pistas = 2, Funciones = 3, Operadores = 4};

    [Serializable]
    public class AlmacenDePalabras : MonoBehaviour
    {
        public string[][] palabras = new string[5][];

        private void Awake()
        {
            //Debug
            for(int i = 1; i < 5; i++)
            {
                palabras[i] = new string[0];
            }
            palabras[0] = new string[4];
            palabras[0][0] = "alojadoEn";
            palabras[0][1] = "coches";
            palabras[0][2] = "ciudadanos";
            palabras[0][3] = "viviendas";
            palabras[1] = new string[21];
            palabras[1][0] = "*";
            palabras[1][1] = "Tabla: alojadoEn";
            palabras[1][2] = "vivienda";
            palabras[1][3] = "ciudadano";
            palabras[1][4] = "Tabla: coches";
            palabras[1][5] = "matricula";
            palabras[1][6] = "titular_id";
            palabras[1][7] = "marca";
            palabras[1][8] = "color";
            palabras[1][9] = "Tabla: ciudadanos";
            palabras[1][10] = "dni";
            palabras[1][11] = "nombre";
            palabras[1][12] = "apellidos";
            palabras[1][13] = "edad";
            palabras[1][14] = "colorPelo";
            palabras[1][15] = "altura";
            palabras[1][16] = "Tabla: viviendas";
            palabras[1][17] = "id";
            palabras[1][18] = "direccion";
            palabras[1][19] = "titular";
            palabras[1][20] = "esPiso";
            palabras[4] = new string[15];
            palabras[4][0] = "+";
            palabras[4][1] = "-";
            palabras[4][2] = "x";
            palabras[4][3] = "/";
            palabras[4][4] = "=";
            palabras[4][5] = "<";
            palabras[4][6] = ">";
            palabras[4][7] = "<=";
            palabras[4][8] = ">=";
            palabras[4][9] = "<>";
            palabras[4][10] = "(";
            palabras[4][11] = ")";
            palabras[4][12] = "AND";
            palabras[4][13] = "OR";
            palabras[4][14] = "BETWEEN";

        }

        public string[] GetLista(TabType tab)
        {
            return palabras[(int)tab];
        }
        public string[] GetLista(int indice)
        {
            return palabras[indice];
        }

        public static int TipoAIndice(TabType tipo)
        {
            return (int)tipo;
        }

        public bool IsOperador(string op)
        {
            int n = palabras[(int)TabType.Operadores].Length;
            for (int i = 0; i < n; i++)
            {
                if (palabras[(int)TabType.Operadores][i].Equals(op)) return true;
            }
            return false;
        }

        public bool ContainsOperador(string op)
        {
            int n = palabras[(int)TabType.Operadores].Length;
            for (int i = 0; i < n; i++)
            {
                if (op.Contains(palabras[(int)TabType.Operadores][i])) return true;
            }
            return false;
        }
    }
}