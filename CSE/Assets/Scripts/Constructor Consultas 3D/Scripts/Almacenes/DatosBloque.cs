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
        public TabType[] tiposBloqueados;
        public Color color;

        public DatosBloque(int id, string prefijo, Color color, bool usaFlecha = false, TabType[] tiposBloqueados = null)
        {
            this.id = id;
            this.prefijo = prefijo;
            this.color = color;
            this.tiposBloqueados = tiposBloqueados;
            this.usaFlecha = usaFlecha;
        }
    }
}