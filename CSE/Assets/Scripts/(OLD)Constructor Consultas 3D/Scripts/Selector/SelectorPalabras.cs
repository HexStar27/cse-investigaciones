using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Hexstar.CSE
{
    public class SelectorPalabras : MonoBehaviour
    {
        public GameObject padreCanvas;
        public TextMeshProUGUI contenidoActual;

        public TabGroup tabs;
        public GameObject palabraPrefab;
        public GameObject separadorPrefab;
        public ContentScaler[] listasPalabras = new ContentScaler[0];
        List<List<ContenedorPalabra>> contenedores = new List<List<ContenedorPalabra>>();

        public GameObject palabraSeleccionadaPrefab;
        public ContentScaler listaSeleccionados;
        List<PalabraSeleccionada> seleccionadas = new List<PalabraSeleccionada>();
        private char[] trimParams = { ',', ' ' };

        private bool isOpen;
        private bool isSelect = false;
        bool lastWasOperator = false;
        private Action onChange;

        public static SelectorPalabras instancia;

        private BloqueInfo3D bloqueActual;

        private void Start()
        {
            RellenarSelector();
            instancia = this;
            onChange = RegenerarCadena;
        }

        public void Instanciar()
        {
            instancia = this;
        }

        public void Abrir(BloqueInfo3D bloque, string conjunto)
        {
            if (isOpen) return;
            isSelect = bloque.Prefijo() == "SELECT" || bloque.Prefijo() == "";
            bloqueActual = bloque;
            Actualizar(conjunto);
            padreCanvas.SetActive(true);
            isOpen = true;
        }

        public void Cerrar()
        {
            if (bloqueActual) bloqueActual.SeleccionarContenido(contenidoActual.text);
            LimpiarPalabrasSeleccionadas();
            tabs.ResetBloqueos();
            padreCanvas.SetActive(false);
            isOpen = false;
        }

        public bool IsOpened()
        {
            return isOpen;
        }

        public void Actualizar(string conjunto)
        {
            contenidoActual.text = conjunto;
            onChange?.Invoke();
            Parse(conjunto);
        }

        public void RegenerarCadena()
        {
            StringBuilder sb = new StringBuilder();
            int n = seleccionadas.Count;
            for(int i = 0; i < n; i++)
            {
                //TODO ???
            }
            //3º guardarlo en el contenidoActual
        }

        public void IncluirPalabra(string palabra, bool incluirEnLista = true)
        {
            //Si la "palabra" es un operador, no poner comas.
            bool isop = AlmacenDePalabras.IsOperador(palabra);
            if (contenidoActual.text.Length > 0)
            {
                if (!isop && !lastWasOperator) contenidoActual.text += ", " + palabra;
                else contenidoActual.text += " " + palabra;
            }
            else contenidoActual.text += palabra;
            
            if(incluirEnLista) ColocarPalabraEnLista(palabra);

            lastWasOperator = isop;
        }

        public void RetirarPalabra(PalabraSeleccionada p)
        {
            string palabra = p.palabra.text;
            if (contenidoActual.text.Contains(palabra))
            {
                int inicio = contenidoActual.text.IndexOf(palabra);
                int final = inicio + palabra.Length;

                if (final < contenidoActual.text.Length)
                {
                    int offsetI = 0,offsetF = 0;
                    if (contenidoActual.text[final] + 1 == '\"')
                    {
                        offsetI = -1;
                        offsetF = 1;
                    }
                    if (contenidoActual.text[final + offsetF] == ',') offsetF += 2;
                    contenidoActual.text = contenidoActual.text.Remove(inicio + offsetI, palabra.Length + offsetF);
                }
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


        /// <returns>La cadena resultante</returns>
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
                    Debug.LogError("(" + name + ") No se puede actualizar una lista nula.");

            if (listasPalabras.Length != AlmacenDePalabras.palabras.Length)
            {
                Debug.LogError("No concuerdan las listas con los tipos de palabras en el almacén.");
            }
            else
            {
                //Destruye todo lo que había anteriormente en los contenedores
                foreach (var contenedor in contenedores)
                {
                    foreach (var p in contenedor)
                    {
                        Destroy(p.gameObject, 1);
                    }
                }
                contenedores.Clear();

                //Se van creando contenedores de uno en uno...
                for (int i = 0; i < listasPalabras.Length; i++)
                {
                    contenedores.Add(new List<ContenedorPalabra>());
                    List<string> palabras = AlmacenDePalabras.GetLista(i);
                    for (int j = 0; j < palabras.Count; j++)
                    {
                        if (palabras[j].StartsWith("Tabla:")) //La palabra es un título
                        {
                            ContenedorPalabra o = Instantiate(separadorPrefab, listasPalabras[i].transform).GetComponent<ContenedorPalabra>();
                            o.texto.text = palabras[j];
                            contenedores[i].Add(o);
                        }
                        else if (palabras[j] == palabras[j].ToUpper()) //La palabra es única del propio lenguaje de SQL
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
            var crudo = conjunto.Split(' ');
            int n = crudo.Length;
            for(int i = 0; i < n; i++)
            {
                if (crudo[i] == null) continue;
                var limpito = crudo[i].Trim(trimParams);
                if (limpito.Length == 0) continue;
                palabras.Add(crudo[i]);
                //Debug.Log(limpito);
            }

            //Colocarlas en la lista de palabras activas.
            foreach (var palabra in palabras)
            {
                ColocarPalabraEnLista(palabra);
            }
        }

        private void ColocarPalabraEnLista(string palabra)
        {
            Vector3 pos = listaSeleccionados.transform.position;
            PalabraSeleccionada palabraSelec = Instantiate(palabraSeleccionadaPrefab, pos, transform.rotation, listaSeleccionados.transform).GetComponent<PalabraSeleccionada>();
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
                    IncluirPalabra(seleccionadas[i].palabra.text, false);
                }
            }
        }
    }
}