using System;
using UnityEngine;

namespace Hexstar.CSE
{
    public enum TabType {Tablas, Columnas, Pistas, Funciones, Operadores };

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
            palabras[2] = new string[2];
            palabras[2][0] = "Pista Debug";
            palabras[2][1] = "Ekisde";
        }

        public string[] GetLista(TabType tab)
        {
            switch(tab)
            {
                case TabType.Tablas:
                    return palabras[0];
                case TabType.Columnas:
                    return palabras[1];
                case TabType.Pistas:
                    return palabras[2];
                case TabType.Funciones:
                    return palabras[3];
                case TabType.Operadores:
                    return palabras[4];
                default:
                    return new string[0];
            }
        }
        public string[] GetLista(int indice)
        {
            return palabras[indice];
        }

        public static int TipoAIndice(TabType tipo)
        {
            switch (tipo)
            {
                case TabType.Tablas:
                    return 0;
                case TabType.Columnas:
                    return 1;
                case TabType.Pistas:
                    return 2;
                case TabType.Funciones:
                    return 3;
                case TabType.Operadores:
                    return 4;
                default:
                    return -1;
            }
        }
    }
}