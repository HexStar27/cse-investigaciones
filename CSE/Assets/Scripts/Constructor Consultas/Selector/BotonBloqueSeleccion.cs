using UnityEngine;
using UnityEngine.EventSystems;

namespace Hexstar.CSE
{
    public class BotonBloqueSeleccion : MonoBehaviour, IPointerClickHandler
    {
        public BloqueConsulta bloque;

        public void OnPointerClick(PointerEventData eventData)
        {
            bloque.AbrirSelector();
        }
    }
}