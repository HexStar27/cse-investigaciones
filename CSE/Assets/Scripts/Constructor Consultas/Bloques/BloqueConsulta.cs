using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hexstar.CSE
{
    [RequireComponent(typeof(RectTransform))]
    public class BloqueConsulta : MonoBehaviour
    {
        public BotonBloqueMov movimientoBloque;
        [HideInInspector] public RectTransform rTransform;
        [SerializeField] int id;
        [SerializeField] TextMeshProUGUI prefijo = null;
        [SerializeField] int tiposAccesibles;
        [SerializeField] Image imagen = null;
        [SerializeField] TextMeshProUGUI cuadroTexto = null;
        [SerializeField] bool usaFlecha;
        [SerializeField] GameObject hijoPrefab = null;
        [SerializeField] GameObject flechaPrefab = null;
        [HideInInspector] public BloqueConsulta hijoActual;
        [HideInInspector] public FlechaDeBloque flechaActual;
        

        private void Awake()
        {
            rTransform = GetComponent<RectTransform>();
            ActualizarConfiguracion();
        }

        /// <summary>
        /// Inicializa el bloque con la estructura de datos de guardado de bloques
        /// </summary>
        public void Inicializar(DatosBloque datos)
        {
            id = datos.id;
            prefijo.text = datos.prefijo;
            tiposAccesibles = datos.tiposAccesibles;
            imagen.color = datos.color;
            usaFlecha = datos.usaFlecha;

            ActualizarConfiguracion();
        }

        /// <summary>
        /// Abre el selector usando el contexto de este bloque
        /// </summary>
        public void AbrirSelector()
        {
            BloquearTiposPalabras();
            SelectorPalabras.instancia.Abrir(this, cuadroTexto.text);
        }

        public void SeleccionarContenido(string nuevoContenido)
        {
            cuadroTexto.text = nuevoContenido;
        }

        public string ConsultaParcial()
        {
            return prefijo.text + " " + cuadroTexto.text;
        }

        public void BloquearTiposPalabras()
        {
            for(int i = 0; i < 5; i++) //Hay 5 tipos de palabras
            {
                if ((tiposAccesibles & (1 << (i))) == 0) // Tipo NO es accesible
                {
                    SelectorPalabras.instancia.tabs.Bloquear(i);
                }
            }

            //Seleccionar una pestaña que no esté bloqueada
            if(SelectorPalabras.instancia.tabs.selectedTab != null)
            {
                if (!SelectorPalabras.instancia.tabs.selectedTab.Desbloqueado())
                    SeleccionarTabDisponible();                
            }
            else SeleccionarTabDisponible();
            
        }

        private void SeleccionarTabDisponible()
        {
            for (int i = 0; i < SelectorPalabras.instancia.tabs.tabButtons.Count; i++)
            {
                if (SelectorPalabras.instancia.tabs.tabButtons[i].Desbloqueado())
                    SelectorPalabras.instancia.tabs.OnTabSelected(SelectorPalabras.instancia.tabs.tabButtons[i]);
            }
        }

        public string Prefijo()
        {
            return prefijo.text;
        }

        /// <summary>
        /// Prepara las estructuras para el bloque hijo
        /// </summary>
        private void ActualizarConfiguracion()
        {
            if (usaFlecha && hijoPrefab != null)
            {
                BloqueConsulta h = Instantiate(hijoPrefab, transform.parent).GetComponent<BloqueConsulta>();
                DatosBloque datos = new DatosBloque(-1, ")", imagen.color);
                h.Inicializar(datos);
                hijoActual = h;
                h.gameObject.SetActive(false);

                FlechaDeBloque f = Instantiate(flechaPrefab).GetComponent<FlechaDeBloque>();
                f.Inicializar(rTransform, h.rTransform);
                f.Activar(false);
                flechaActual = f;
            }
        }

        public bool UsandoFlecha()
        {
            return usaFlecha;
        }

        public int ID()
        {
            return id;
        }
    }
}