using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hexstar.CSE
{
    public class ContenedorPalabra : MonoBehaviour
    {
        public bool esSeparador = false;
        public RectTransform combinado;
        public TMP_InputField apellido;
        public GameObject AS;
        public TMP_InputField campoNuevoNombre;
        public Button botonListo;
        public TextMeshProUGUI texto;
        private bool usandoAS = false;
        private bool usandoApellido = false;

        public void Inicializar(bool usaAS = false, bool usaApellido = false)
        {
            if(!esSeparador)
            {
                usandoAS = usaAS;
                usandoApellido = usaApellido;

                AS.SetActive(usandoAS);
                apellido.gameObject.SetActive(usandoApellido);
                if (usandoApellido)
                    combinado.sizeDelta = new Vector2(45, combinado.sizeDelta.y);
                else
                    combinado.sizeDelta = new Vector2(0, combinado.sizeDelta.y);
            }
        }

        private void OnEnable()
        {
            if (!esSeparador) botonListo.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            if (!esSeparador) botonListo.onClick.RemoveListener(OnClick);
        }

        public void OnClick()
        {
            campoNuevoNombre.text = campoNuevoNombre.text.Replace(" ", "");
            apellido.text = apellido.text.Replace(" ", "");

            string contenidoFinal = "";

            if (apellido.text != "" && usandoApellido) contenidoFinal += apellido.text + ".";

            contenidoFinal += texto.text;

            if (campoNuevoNombre.text != "" && usandoAS) contenidoFinal += " AS " + campoNuevoNombre.text;

            SelectorPalabras.instancia.IncluirPalabra(contenidoFinal);
        }
    }
}