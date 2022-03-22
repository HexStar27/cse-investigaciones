using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hexstar.CSE
{
    public class SelectorPalabras : MonoBehaviour
    {
        public GameObject padreCanvas;
        public TextMeshProUGUI contenidoActual;
        public AlmacenDePalabras almacen;

        public TabGroup tabs;
        public GameObject palabraPrefab;
        public GameObject separadorPrefab;
        public ContentScaler[] listasPalabras = new ContentScaler[0];
         List<List<ContenedorPalabra>> contenedores = new List<List<ContenedorPalabra>>();

        public GameObject palabraSeleccionadaPrefab;
        public ContentScaler listaSeleccionados;
         List<PalabraSeleccionada> seleccionadas = new List<PalabraSeleccionada>();
        private char[] trimParams = { ',', ' ' };

        private bool isSelect = false;


        public static SelectorPalabras instancia;

        private BloqueConsulta bloqueActual;

        private void Start()
        {
            RellenarSelector();
            instancia = this;
        }

        public void Instanciar()
        {
            instancia = this;
        }

        public void Abrir(BloqueConsulta bloque, string conjunto)
        {
            isSelect = bloque.Prefijo() == "SELECT" || bloque.Prefijo() == "";
            bloqueActual = bloque;
            Actualizar(conjunto);
            padreCanvas.SetActive(true);
        }

        public void Cerrar()
        {
            if(bloqueActual) bloqueActual.SeleccionarContenido(contenidoActual.text);
            LimpiarPalabrasSeleccionadas();
            tabs.ResetBloqueos();
            padreCanvas.SetActive(false);
        }

        public void Actualizar(string conjunto)
        {
            contenidoActual.text = conjunto;
            Parse(conjunto);
        }

        public void IncluirPalabra(string palabra)
        {
            if (contenidoActual.text.Length > 0) contenidoActual.text += ", " + palabra;
            else contenidoActual.text += palabra;
            ColocarPalabraEnLista(palabra);
        }

        public void RetirarPalabra(PalabraSeleccionada p)
        {
            string palabra = p.palabra.text;
            if (contenidoActual.text.Contains(palabra))
            {
                int inicio = contenidoActual.text.IndexOf(palabra);
                int final = inicio + palabra.Length;

                if (final < contenidoActual.text.Length)
                    contenidoActual.text = contenidoActual.text.Remove(inicio, palabra.Length + 2);
                else if (final == contenidoActual.text.Length && inicio >= 2)
                    contenidoActual.text = contenidoActual.text.Remove(inicio - 2, palabra.Length + 2);
                else
                    contenidoActual.text = contenidoActual.text.Remove(inicio, palabra.Length);


                contenidoActual.text.Trim(trimParams);
            }
            seleccionadas.Remove(p);
        }

        public void RetirarPalabra(string palabra)
        {
            if (contenidoActual.text.Contains(palabra))
            {
                int inicio = contenidoActual.text.IndexOf(palabra);
                int final = inicio + palabra.Length;

                if (final < contenidoActual.text.Length)
                    contenidoActual.text = contenidoActual.text.Remove(inicio, palabra.Length + 2);
                else if (final == contenidoActual.text.Length && inicio >= 2)
                    contenidoActual.text = contenidoActual.text.Remove(inicio - 2, palabra.Length + 2);
                else
                    contenidoActual.text = contenidoActual.text.Remove(inicio, palabra.Length);


                contenidoActual.text.Trim(trimParams);
            }
            PalabraSeleccionada fuk = seleccionadas.Find((value) => { return value.palabra.text == palabra; });
            seleccionadas.Remove(fuk);
        }

        public string ConjuntoFinal()
        {
            return contenidoActual.text;
        }

        /// <summary>
        /// Obtiene las palabras del almacén y las coloca en el selector en 
        /// sus correspondientes pestañas
        /// </summary>
        public void RellenarSelector()
        {            
            foreach (var lista in listasPalabras)
                if (lista == null) 
                    Debug.LogError("("+name+") No se puede actualizar una lista nula.");

            if (listasPalabras.Length != almacen.palabras.Length)
            {
                Debug.LogError("No concuerdan las listas con los tipos de palabras en el almacén.");
            }
            else
            {
                contenedores.Clear();
                for (int i = 0; i < listasPalabras.Length; i++)
                {
                    contenedores.Add(new List<ContenedorPalabra>());
                    string[] palabras = almacen.GetLista(i);
                    for (int j = 0; j < palabras.Length; j++)
                    {
                        if (palabras[j].StartsWith("Tabla:")) //La palabra es un título
                        {
                            ContenedorPalabra o = Instantiate(separadorPrefab, listasPalabras[i].transform).GetComponent<ContenedorPalabra>();
                            o.texto.text = palabras[j];
                            contenedores[i].Add(o);
                        }
                        else if(palabras[j] == palabras[j].ToUpper()) //La palabra es única del propio lenguaje de SQL
                        {
                            ContenedorPalabra o = Instantiate(palabraPrefab, listasPalabras[i].transform).GetComponent<ContenedorPalabra>();
                            o.Inicializar(false, false);
                            o.texto.text = palabras[j];
                            contenedores[i].Add(o);
                        }
                        else //Para el resto de palabras...
                        {
                            ContenedorPalabra o = Instantiate(palabraPrefab, listasPalabras[i].transform).GetComponent<ContenedorPalabra>();

                            //Sólo se puede añadir el AS en la primera pestaña o en la segunda si es un bloque SELECT
                            //Sólo se puede añadir el apellido en la segunda pestaña
                            bool usarAS = i == 0 || (i == 1 && isSelect);
                            bool usarApellido = i == 1;
                            o.Inicializar(usarAS, usarApellido);
                            o.texto.text = palabras[j];
                            contenedores[i].Add(o);
                        }
                    }
                    listasPalabras[i].Actualizar();
                }
            }
        }

        private void Parse(string conjunto)
        {
            List<string> palabras = new List<string>();

            //Obtener las palabras escogidas
            for(int i = 0; i < conjunto.Length; i++)
            {
                int j = i;
                bool comaEncontrada = false;
                for(; j < conjunto.Length && !comaEncontrada; j++)
                {
                    if (conjunto[j] == ',') comaEncontrada = true;
                }

                if (j < conjunto.Length)
                    palabras.Add(conjunto.Substring(i, j - i - 1));
                else if (j == conjunto.Length)
                    palabras.Add(conjunto.Substring(i, j - i));

                i = j;
            }

            //Colocarlas en la lista de palabras activas.
            foreach (var palabra in palabras)
            {
                ColocarPalabraEnLista(palabra);
            }
        }

        private void ColocarPalabraEnLista(string palabra)
        {
            Vector3 pos = new Vector3(0, 0, 0);
            PalabraSeleccionada palabraSelec = Instantiate(palabraSeleccionadaPrefab, pos, Quaternion.identity, listaSeleccionados.transform).GetComponent<PalabraSeleccionada>();
            palabraSelec.Inicializar(palabra);
            seleccionadas.Add(palabraSelec);
        }

        private void LimpiarPalabrasSeleccionadas()
        {
            while (seleccionadas.Count > 0)
                seleccionadas[0].Eliminar();
            
        }

        public void IntercambiarPosicionPalabras(PalabraSeleccionada p)
        {
            int pos = seleccionadas.IndexOf(p);
            if (pos < seleccionadas.Count - 1 && pos >= 0)
            {                
                //Intercambiarla en "los arrays"
                string aux = p.palabra.text;
                seleccionadas[pos].palabra.text = seleccionadas[pos + 1].palabra.text;
                seleccionadas[pos + 1].palabra.text = aux;

                //Intercambiarla en la cadena
                contenidoActual.text = "";
                for(int i = 0; i < seleccionadas.Count; i++)
                {
                    if (contenidoActual.text.Length > 0) contenidoActual.text += ", " + seleccionadas[i].palabra.text;
                    else contenidoActual.text += seleccionadas[i].palabra.text;
                }
            }
        }
    }
}