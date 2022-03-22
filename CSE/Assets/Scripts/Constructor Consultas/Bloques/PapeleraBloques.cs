using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexstar.CSE
{
    public class PapeleraBloques : MonoBehaviour
    {
        public static PapeleraBloques instancia;
        public GameObject papelera;
        public Vector2 limitesPapelera;

        private void Awake()
        {
            Instanciar();
        }

        public void Instanciar()
        {
            instancia = this;
        }

        public void Activar(bool value)
        {
            papelera.SetActive(value);
        }

        public bool TocandoPapelera(Vector2 mousePos)
        {
            return mousePos.x > limitesPapelera.x && mousePos.y > limitesPapelera.y;
        }
    }
}