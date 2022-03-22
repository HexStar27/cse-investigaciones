using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hexstar.CSE
{
    public class ElementoPista : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector] public DatosPista datos;
        [SerializeField] TextMeshProUGUI titulo;

        public void Inicializar(DatosPista datos)
        {
            this.datos = datos;
            //La info se pone en la descripción y en el almacén de palabras en
            //la columna de pistas. Esto se hace al inicializar
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CajonPistas.instancia.MostrarDescripcion(datos.descripcion);
        }
    }
}