using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hexstar.CSE
{
    public class PalabraSeleccionada : MonoBehaviour
    {
        [SerializeField] Button botonEliminar;
        public TextMeshProUGUI palabra;
        public RectTransform rTrans;

        public void Inicializar(string palabra)
        {
            this.palabra.text = palabra;
        }

        public void Eliminar()
        {
            SelectorPalabras.instancia.RetirarPalabra(this);
            Destroy(gameObject, 0);
        }

        public void Bajar()
        {
            SelectorPalabras.instancia.IntercambiarPosicionPalabras(this);
        }
    }
}