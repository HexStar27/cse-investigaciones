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
            palabras[0] = new string[2];
            palabras[0][0] = "ekisde";
            palabras[0][1] = "clientes";
            palabras[1] = new string[5];
            palabras[1][0] = "*";
            palabras[1][1] = "Tabla: ekisde";
            palabras[1][2] = "vnt_id";
            palabras[1][3] = "Tabla: clientes";
            palabras[1][4] = "clt_id";
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
    }
}