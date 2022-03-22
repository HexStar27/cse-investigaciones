using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Hexstar
{
    public class DialogosCSE : MonoBehaviour
    {
        [SerializeField] private GameObject cajaTexto = null;
        [SerializeField] private TextMeshProUGUI tmpro = null;
        private char[] m_procesando = null;
        private int m_indiceLetra = 0;
        private AudioSource m_sonidoLetra = null;
        private WaitForSeconds m_intervalo = new WaitForSeconds(0.02f);
        private LinkedList<string> m_buffer = new LinkedList<string>();

        //Flags
        bool f_vaciar = false;
        bool f_cerrar = false;
        bool f_saltar = false;
        bool f_finished = false;
        bool f_limpiar = false;

        public UnityEvent onFinishPlacement;

        private void OnEnable()
        {
            if (!tmpro) Debug.LogError("No se ha asignado un TextMeshProUGUI en " + name);
            if (!cajaTexto) Debug.LogError("No se ha asignado una caja de texto en " + name);

            StartCoroutine(Cuerpo());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Muestra la caja de texto
        /// </summary>
        public void AbrirTexto()
        {
            f_cerrar = false;
            cajaTexto.SetActive(true);
        }

        /// <summary>
        /// Muestra la caja de texto, reinicia flags y Limpia tanto la caja como el buffer.
        /// </summary>
        public void AbrirTextoNuevo()
        {
            f_saltar = false;
            f_cerrar = false;
            cajaTexto.SetActive(true);
            Vaciar();
            LimpiarBuffer();
            f_finished = true;
        }

        /// <summary>
        /// Cierra la caja de texto y reinicia flags
        /// </summary>
        public void CerrarTexto()
        {
            cajaTexto.SetActive(false);
            f_cerrar = true;
            f_vaciar = false;
            f_limpiar = false;
            f_saltar = false;
        }

        /// <summary>
        /// Limpia el contenido de la caja
        /// </summary>
        public void Vaciar()
        {
            f_vaciar = true;
        }

        /// <summary>
        /// Limpia el contenido del buffer
        /// </summary>
        public void LimpiarBuffer()
        {
            f_limpiar = true;
        }

        /// <summary>
        /// Especifica el sonido a ejecutar por cada letra que se pone en la caja
        /// </summary>
        /// <param name="sonido">El sonido especificado</param>
        public void SeleccionarSonido(AudioSource sonido)
        {
            m_sonidoLetra = sonido;
        }

        /// <summary>
        /// Escribe en la caja el texto pasado SIN IMPORTAR LO QUE PASE.
        /// </summary>
        /// <param name="texto"></param>
        public void IntroducirTexto(string texto)
        {
            if (f_finished)
            {
                m_procesando = texto.ToCharArray();
                m_indiceLetra = 0;
            }
            else m_buffer.AddLast(texto);

            f_finished = false;
        }

        /// <summary>
        /// Modifica la velocidad a la que muestra las letras el texto
        /// Susceptible a fixedTime
        /// </summary>
        /// <param name="vel">EL intervalo de tiempo de colocación de cada letra</param>
        public void FijarVelocidadTexto(float vel)
        {
            if (vel <= 0) Debug.LogError("NO TE RECOMIENDO poner la velocidad a ese valor.");
            m_intervalo = new WaitForSeconds(vel);
        }

        /// <summary>
        /// Indica al programa que coloque todo el texto de golpe
        /// </summary>
        public void SaltarColocacionTexto()
        {
            f_saltar = true;
        }

        public bool HaTerminado()
        {
            return f_finished;
        }

        private IEnumerator Cuerpo()
        {
            //WaitForFixedUpdate fixedFrame = new WaitForFixedUpdate();
            WaitWhile cerrado = new WaitWhile(()=> { return f_cerrar; });
            WaitWhile masTexto = new WaitWhile(()=> { return f_finished; });

            while(true)
            {
                //Espera a que vuelva a estar abierto
                if(f_cerrar) yield return cerrado;

                //Vacía el texto de la caja
                if(f_vaciar)
                {
                    tmpro.text = "";
                    f_vaciar = false;
                }

                if (f_limpiar)
                {
                    m_buffer.Clear();
                    f_limpiar = false;
                }

                if (m_procesando != null)
                {
                    //Escribe todo el texto de golpe
                    if (f_saltar)
                    {
                        for (; m_indiceLetra < m_procesando.Length; m_indiceLetra++)
                        {
                            tmpro.text += m_procesando[m_indiceLetra];
                        }

                        f_saltar = false;
                    }

                    //Continua colocando letras, comportamiento normal
                    if (m_indiceLetra < m_procesando.Length)
                    {
                        tmpro.text += m_procesando[m_indiceLetra];
                        m_indiceLetra++;
                    }

                    if (m_indiceLetra >= m_procesando.Length)
                    {
                        tmpro.text += "\n";
                        f_finished = true;
                    }

                    if (m_sonidoLetra)
                    {
                        m_sonidoLetra.Stop();//??
                        m_sonidoLetra.Play();
                    }
                }

                if (f_finished)
                {
                    if (m_buffer.Count > 0 && m_procesando != null)
                    {
                        m_procesando = m_buffer.First.Value.ToCharArray();
                        m_buffer.RemoveFirst();
                        m_indiceLetra = 0;
                        f_finished = false;
                    }
                    else
                    {
                        onFinishPlacement.Invoke();
                        yield return masTexto;
                    }
                }
                else yield return m_intervalo;
            }
        }
    }
}