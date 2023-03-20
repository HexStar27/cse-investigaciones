using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hexstar.CSE
{
    public class ElementoPista : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [HideInInspector] public DatosPista datos;
        [SerializeField] TextMeshProUGUI titulo;

        public void Inicializar(DatosPista datos)
        {
            this.datos = datos;
            titulo.SetText(datos.titulo);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CajonPistas.instancia.MostrarDescripcion(datos.descripcion);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CajonPistas.instancia.MostrarDescripcion(datos.descripcion);
        }
    }
}