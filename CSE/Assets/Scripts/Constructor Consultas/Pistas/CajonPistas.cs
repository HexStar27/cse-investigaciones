using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Hexstar.CSE
{
    public class CajonPistas : MonoBehaviour
    {
        public AlmacenDePalabras almacenPalabras;
        public static CajonPistas instancia;
        private List<ElementoPista> pistas = new List<ElementoPista>();
        public ContentScaler content;
        public Transform content2;
        public DescripcionPista descripcionMesh;
        public GameObject pistaPrefab;

        private void Awake()
        {
            instancia = this;
        }

        public void Abrir(bool value)
        {
            gameObject.SetActive(value);
        }

        //Actualiza las pistas disponibles en el selector de palabras
        public void ActualizarElementosRelacionados()
        {
            string[] palabrasPistas = new string[pistas.Count];
            for (int i = 0; i < palabrasPistas.Length; i++)
            {
                palabrasPistas[i] = pistas[i].datos.palabra;
            }
            almacenPalabras.palabras[AlmacenDePalabras.TipoAIndice(TabType.Pistas)] = palabrasPistas;

            SelectorPalabras.instancia.RellenarSelector();
        }

        public void MostrarDescripcion(string descripcion)
        {
            descripcionMesh.texto.text = descripcion;
            descripcionMesh.gameObject.SetActive(true);
        }

        public void EsconderDescripcion()
        {
            descripcionMesh.gameObject.SetActive(false);
        }

        public void RellenarCajonConCasoActivo()
        {
            int n = PuzzleManager.Instance.casoActivo.pistas.Length;
            ElementoPista o;

            Transform t = content!=null ? content.transform : content2;

            for (int i = 0; i < n; i++)
            {
                o = Instantiate(pistaPrefab,t).GetComponent<ElementoPista>();
                o.Inicializar(PuzzleManager.Instance.casoActivo.pistas[i]);
                IntroducirPista(o, t);
            }
            //ActualizarElementosRelacionados(); //TODO: Descomentar cuando tenga puesto el Selector de palabras
        }

        public void IntroducirPista(ElementoPista pista, Transform t, bool actualizar = false)
        {
            pistas.Add(pista);
            pista.transform.SetParent(t);
            if(content !=null) content.Actualizar();
            if (actualizar) ActualizarElementosRelacionados();
        }

        public void EliminarPista(ElementoPista pista, bool actualizar = false)
        {
            pistas.Remove(pista);
            Destroy(pista.gameObject);
            if (content != null) content.Actualizar();
            if(actualizar)ActualizarElementosRelacionados();
        }

        public void VaciarCajon()
        {
            almacenPalabras.palabras[AlmacenDePalabras.TipoAIndice(TabType.Pistas)] = new string[0];

            for (int i = pistas.Count - 1; i >= 0; i--)
            {
                Destroy(pistas[i].gameObject);
            }
            pistas.Clear();
            if (content != null) content.Actualizar();
            ActualizarElementosRelacionados();

            //TODO: No eliminar los casos que sean de historia
        }

        //Tanto introducir como eliminar pista se van a encargar de modificar 
        //la lista, el contenido en el scroll, y el contenido en el selector de palabras y el almacén.
    }
}