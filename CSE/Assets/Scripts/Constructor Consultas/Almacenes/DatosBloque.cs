using System;
using UnityEngine;

namespace Hexstar.CSE
{
    [Serializable]
    public struct DatosBloque
    {
        public int id;
        public bool usaFlecha;
        public string prefijo;
        public int tiposAccesibles;
        public Color color;

        public DatosBloque(int id, string prefijo, Color color, bool usaFlecha = false, int tiposAccesibles = 0)
        {
            this.id = id;
            this.prefijo = prefijo;
            this.color = color;
            this.tiposAccesibles = tiposAccesibles;
            this.usaFlecha = usaFlecha;
        }
    }
}